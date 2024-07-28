// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.LatestBuildResultCacheService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class LatestBuildResultCacheService : ILatestBuildResultCacheService, IVssFrameworkService
  {
    private IMutableDictionaryCacheContainer<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>> m_redis;
    private TimeSpan m_redisExpiry;
    private static readonly Guid s_cacheNamespace = new Guid("75B7E99B-BF58-43AD-B72B-60C696A67E26");
    private const string c_cacheRootPath = "/Configuration/Caching/MemoryCache/BuildResultCacheService/";
    private const string c_redisExpiryPath = "/Configuration/Caching/MemoryCache/BuildResultCacheService/RedisExpiry";
    private static readonly RegistryQuery s_cacheSettingsQuery = (RegistryQuery) "/Configuration/Caching/MemoryCache/BuildResultCacheService/*";
    private const int c_defaultRedisExpiry = 90000;

    public (BuildDefinition Definition, BuildResult? BuildResult) GetBuildResult(
      IVssRequestContext requestContext,
      Guid projectId,
      string definition,
      string branchName,
      string stageName,
      string jobName,
      string configuration)
    {
      ArgumentUtility.CheckForNull<string>(definition, nameof (definition));
      BuildDefinition definition1 = requestContext.GetService<IBuildDefinitionService>().GetDefinition(requestContext, projectId, definition);
      if (definition1 == null)
        return ((BuildDefinition) null, new BuildResult?());
      if (string.IsNullOrWhiteSpace(branchName))
        branchName = definition1.Repository.DefaultBranch;
      LatestBuildResultCacheService.DefinitionKey definitionKey = new LatestBuildResultCacheService.DefinitionKey();
      LatestBuildResultCacheService.ResultKey resultKey = new LatestBuildResultCacheService.ResultKey();
      if (requestContext.IsFeatureEnabled("Build2.LatestBuildResultCache"))
      {
        definitionKey = new LatestBuildResultCacheService.DefinitionKey(projectId, definition1.Id);
        resultKey = new LatestBuildResultCacheService.ResultKey(branchName, stageName, jobName);
        BuildResult buildResult;
        if (this.TryGetValue(requestContext, definitionKey, resultKey, out buildResult))
          return (definition1, new BuildResult?(buildResult));
      }
      BuildResult? branchStatus = requestContext.GetService<IBuildService>().GetBranchStatus(requestContext, projectId, definition1.Id, definition1.Repository.Id, definition1.Repository.Type, branchName, stageName, jobName, configuration);
      if (requestContext.IsFeatureEnabled("Build2.LatestBuildResultCache") && branchStatus.HasValue)
        this.Add(requestContext, definitionKey, resultKey, branchStatus.Value);
      return (definition1, branchStatus);
    }

    public void TryUpdateBuildResult(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int buildId,
      BuildResult buildResult)
    {
      if (!requestContext.IsFeatureEnabled("Build2.LatestBuildResultCache"))
        return;
      try
      {
        IMutableDictionaryCacheContainer<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>> container;
        if (!this.GetRedis(requestContext, out container))
          return;
        LatestBuildResultCacheService.DefinitionKey key1 = new LatestBuildResultCacheService.DefinitionKey(projectId, definitionId);
        IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult> dictionary;
        if (!container.TryGet<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>>(requestContext, key1, out dictionary))
          return;
        List<TimelineRecord> records = requestContext.GetService<IBuildService>().GetBuildById(requestContext, projectId, buildId).GetTimelineData(requestContext)?.Timeline.Records;
        foreach (LatestBuildResultCacheService.ResultKey key2 in dictionary.Keys.ToList<LatestBuildResultCacheService.ResultKey>())
        {
          if (string.IsNullOrWhiteSpace(key2.StageName) && string.IsNullOrWhiteSpace(key2.JobName))
            dictionary[key2] = buildResult;
          else if (records != null && records.Any<TimelineRecord>())
            dictionary[key2] = TimelineHelpers.GetTimelineResult((IList<TimelineRecord>) records, key2.StageName, key2.JobName, string.Empty);
          else
            dictionary.Remove(key2);
        }
        if (dictionary.Count > 0)
          container.Set(requestContext, (IDictionary<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>>) new Dictionary<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>>()
          {
            [key1] = dictionary
          });
        else
          container.Invalidate(requestContext, (IEnumerable<LatestBuildResultCacheService.DefinitionKey>) new LatestBuildResultCacheService.DefinitionKey[1]
          {
            key1
          });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (LatestBuildResultCacheService), ex);
      }
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
      ICachedRegistryService service = requestContext.GetService<ICachedRegistryService>();
      this.m_redisExpiry = TimeSpan.FromSeconds((double) service.ReadEntries(requestContext, LatestBuildResultCacheService.s_cacheSettingsQuery).GetValueFromPath<int>("/Configuration/Caching/MemoryCache/BuildResultCacheService/RedisExpiry", 90000));
      service.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChange), false, in LatestBuildResultCacheService.s_cacheSettingsQuery);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.GetService<ICachedRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChange));
      this.m_redis = (IMutableDictionaryCacheContainer<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>>) null;
    }

    private void Add(
      IVssRequestContext requestContext,
      LatestBuildResultCacheService.DefinitionKey definitionKey,
      LatestBuildResultCacheService.ResultKey resultKey,
      BuildResult buildResult)
    {
      IMutableDictionaryCacheContainer<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>> container;
      if (!this.GetRedis(requestContext, out container))
        return;
      try
      {
        IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult> dictionary;
        if (!container.TryGet<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>>(requestContext, definitionKey, out dictionary))
          dictionary = (IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>) new Dictionary<LatestBuildResultCacheService.ResultKey, BuildResult>();
        dictionary[resultKey] = buildResult;
        container.Set(requestContext, (IDictionary<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>>) new Dictionary<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>>()
        {
          [definitionKey] = dictionary
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (LatestBuildResultCacheService), ex);
      }
    }

    private bool TryGetValue(
      IVssRequestContext requestContext,
      LatestBuildResultCacheService.DefinitionKey definitionKey,
      LatestBuildResultCacheService.ResultKey resultKey,
      out BuildResult buildResult)
    {
      IMutableDictionaryCacheContainer<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>> container;
      if (this.GetRedis(requestContext, out container))
      {
        try
        {
          IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult> dictionary;
          if (container.TryGet<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>>(requestContext, definitionKey, out dictionary))
          {
            if (dictionary.TryGetValue(resultKey, out buildResult))
              return true;
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (LatestBuildResultCacheService), ex);
        }
      }
      buildResult = BuildResult.None;
      return false;
    }

    private bool GetRedis(
      IVssRequestContext requestContext,
      out IMutableDictionaryCacheContainer<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>> container)
    {
      IRedisCacheService service = requestContext.GetService<IRedisCacheService>();
      if (service.IsEnabled(requestContext))
      {
        container = this.m_redis;
        if (container == null)
        {
          container = service.GetVolatileDictionaryContainer<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>, LatestBuildResultCacheService.ContainerSecurityToken>(requestContext, LatestBuildResultCacheService.s_cacheNamespace, new ContainerSettings()
          {
            KeyExpiry = new TimeSpan?(this.m_redisExpiry),
            CiAreaName = nameof (LatestBuildResultCacheService),
            NoThrowMode = new bool?(true)
          });
          this.m_redis = container;
        }
        return true;
      }
      container = (IMutableDictionaryCacheContainer<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>>) null;
      return false;
    }

    private void OnRegistryChange(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      RegistryEntry entry;
      if (!changedEntries.TryGetValue("/Configuration/Caching/MemoryCache/BuildResultCacheService/RedisExpiry", out entry))
        return;
      this.m_redisExpiry = TimeSpan.FromSeconds((double) entry.GetValue<int>());
      this.m_redis = (IMutableDictionaryCacheContainer<LatestBuildResultCacheService.DefinitionKey, IDictionary<LatestBuildResultCacheService.ResultKey, BuildResult>>) null;
    }

    internal struct DefinitionKey
    {
      private Guid m_projectId;
      private int m_definitionId;

      public DefinitionKey(Guid projectId, int definitionId)
      {
        this.m_projectId = projectId;
        this.m_definitionId = definitionId;
      }

      public (Guid, int) ContainerKey => (this.m_projectId, this.m_definitionId);
    }

    [TypeConverter(typeof (LatestBuildResultCacheService.ResultKeyTypeConverter))]
    internal struct ResultKey
    {
      public ResultKey(string valueKey) => this.ValueKey = valueKey;

      public ResultKey(string branchName, string stageName, string jobName)
        : this(string.Join("|", new string[3]
        {
          branchName,
          stageName,
          jobName
        }))
      {
      }

      public string ValueKey { get; }

      public string BranchName => ((IEnumerable<string>) this.ValueKey.Split('|')).First<string>();

      public string StageName => ((IEnumerable<string>) this.ValueKey.Split('|')).Skip<string>(1).First<string>();

      public string JobName => ((IEnumerable<string>) this.ValueKey.Split('|')).Last<string>();

      public override string ToString() => this.ValueKey;
    }

    private class ResultKeyTypeConverter : TypeConverter
    {
      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof (string);

      public override object ConvertFrom(
        ITypeDescriptorContext context,
        CultureInfo culture,
        object value)
      {
        return value is string valueKey ? (object) new LatestBuildResultCacheService.ResultKey(valueKey) : base.ConvertFrom(context, culture, value);
      }
    }

    internal sealed class ContainerSecurityToken
    {
    }
  }
}
