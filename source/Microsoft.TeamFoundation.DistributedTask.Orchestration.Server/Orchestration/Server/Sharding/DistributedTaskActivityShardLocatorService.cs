// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Sharding.DistributedTaskActivityShardLocatorService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Sharding
{
  internal class DistributedTaskActivityShardLocatorService : 
    IDistributedTaskActivityShardLocatorService,
    IVssFrameworkService
  {
    private IDictionary<string, IActivityShardLocator> m_shardLocatorLookup;
    private IList<string> m_hubNames;
    private const int c_maxShardOverrides = 10;

    public IActivityShardLocator GetActivityShardLocator(
      IVssRequestContext requestContext,
      string hubName)
    {
      IActivityShardLocator activityShardLocator;
      if (this.m_shardLocatorLookup.TryGetValue(hubName, out activityShardLocator))
        return activityShardLocator;
      requestContext.TraceError(nameof (DistributedTaskActivityShardLocatorService), "Hub with name '" + hubName + "' is not found");
      return (IActivityShardLocator) null;
    }

    public IList<PipelineShardOverride> SetShardOverrides(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeIdentifier,
      IList<int> definitionIds,
      int? shardId)
    {
      if (shardId.HasValue)
        ArgumentUtility.CheckBoundsInclusive(shardId.Value, 990, 999, nameof (shardId));
      TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, hubName, false);
      if (taskHub == null)
        throw new TaskHubNotFoundException(TaskResources.HubNotFound((object) hubName));
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string overridesSettingPath = DistributedTaskActivityShardLocatorService.GetShardOverridesSettingPath(taskHub.Name);
      ICollection<PipelineShardOverride> source;
      IDictionary<string, PipelineShardOverride> dictionary = !JsonUtilities.TryDeserialize<ICollection<PipelineShardOverride>>(service.GetValue<string>(requestContext, (RegistryQuery) overridesSettingPath, false, (string) null), out source, true) ? (IDictionary<string, PipelineShardOverride>) new Dictionary<string, PipelineShardOverride>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IDictionary<string, PipelineShardOverride>) source.ToDictionary<PipelineShardOverride, string, PipelineShardOverride>((Func<PipelineShardOverride, string>) (x => x.GetIdentifier()), (Func<PipelineShardOverride, PipelineShardOverride>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (int definitionId in (IEnumerable<int>) definitionIds)
      {
        string identifier = PipelineShardOverride.GetIdentifier(scopeIdentifier, definitionId);
        if (shardId.HasValue)
          dictionary[identifier] = new PipelineShardOverride()
          {
            ScopeIdentifier = scopeIdentifier,
            DefinitionId = definitionId,
            ShardId = shardId.Value
          };
        else
          dictionary.Remove(identifier);
      }
      if (dictionary.Count > 10)
        throw new ArgumentOutOfRangeException(nameof (definitionIds), string.Format("Only a maximum of {0} definitions can be throttled for this host. Please evaluate existing throttling on other definitions and try again.", (object) 10));
      service.SetValue<string>(requestContext, overridesSettingPath, dictionary.Values.Serialize<ICollection<PipelineShardOverride>>(true));
      return (IList<PipelineShardOverride>) dictionary.Values.ToImmutableList<PipelineShardOverride>();
    }

    private static string GetShardOverridesSettingPath(string hubName) => string.Format(RegistryKeys.PipelineShardOverridesSettingPathFormat, (object) hubName);

    private void SettingsChangedCallBack(
      IVssRequestContext requestContext,
      RegistryEntryCollection registryValues)
    {
      this.LoadSettings(requestContext);
    }

    private void LoadSettings(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      foreach (string hubName in (IEnumerable<string>) this.m_hubNames)
      {
        IVssRegistryService registryService1 = service;
        IVssRequestContext requestContext1 = requestContext;
        RegistryQuery registryQuery = (RegistryQuery) string.Format("/Service/Orchestration/Settings/ActivityDispatcher/{0}/CountShards", (object) hubName);
        ref RegistryQuery local1 = ref registryQuery;
        int num = registryService1.GetValue<int>(requestContext1, in local1, true, 1);
        if (string.Compare(hubName, "Build", StringComparison.OrdinalIgnoreCase) == 0)
        {
          IVssRegistryService registryService2 = service;
          IVssRequestContext requestContext2 = requestContext;
          registryQuery = (RegistryQuery) string.Format("/Service/Orchestration/Settings/ActivityDispatcher/{0}/CountShardsServer", (object) hubName);
          ref RegistryQuery local2 = ref registryQuery;
          int serverDispatcherShardsCount = registryService2.GetValue<int>(requestContext2, in local2, true, 1);
          string overridesSettingPath = DistributedTaskActivityShardLocatorService.GetShardOverridesSettingPath(hubName);
          IVssRegistryService registryService3 = service;
          IVssRequestContext requestContext3 = requestContext;
          registryQuery = (RegistryQuery) overridesSettingPath;
          ref RegistryQuery local3 = ref registryQuery;
          ICollection<PipelineShardOverride> shardOverrides;
          JsonUtilities.TryDeserialize<ICollection<PipelineShardOverride>>(registryService3.GetValue<string>(requestContext3, in local3), out shardOverrides, true);
          this.m_shardLocatorLookup[hubName] = ShardingStrategyBuilder.Build(requestContext, hubName, num, serverDispatcherShardsCount, shardOverrides);
        }
        else
          this.m_shardLocatorLookup[hubName] = ShardingStrategyBuilder.Build(requestContext, hubName, num, num, (ICollection<PipelineShardOverride>) null);
      }
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
      this.m_hubNames = (IList<string>) requestContext.GetExtensions<TaskHubExtension>(ExtensionLifetime.Service).Select<TaskHubExtension, string>((Func<TaskHubExtension, string>) (x => x.HubName)).Distinct<string>().ToList<string>();
      this.m_shardLocatorLookup = (IDictionary<string, IActivityShardLocator>) new ConcurrentDictionary<string, IActivityShardLocator>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.LoadSettings(requestContext);
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.SettingsChangedCallBack), RegistryKeys.PipelineShardOverridesSettingsRoot + "**", "/Service/Orchestration/Settings/ActivityDispatcher/**");
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.SettingsChangedCallBack));
  }
}
