// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.Framework.ActivePolicyEvaluationCache
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Policy.Server.Framework
{
  public class ActivePolicyEvaluationCache : IActivePolicyEvaluationCache, IDisposable
  {
    private const string cacheAreaName = "Policy.Server.ActiveAvaluationPolicy.Results";
    private static readonly string s_layer = nameof (ActivePolicyEvaluationCache);
    private static readonly Guid s_redisNamespace = typeof (ActivePolicyEvaluationCache).GetTypeInfo().Module.ModuleVersionId;
    private IVssRequestContext m_requestContext;
    private TimeSpan m_expirationTime;
    private string m_targetKey;
    private int m_minFileCountToUseCache;
    private Lazy<IMutableDictionaryCacheContainer<string, IDictionary<int, ActivePolicyEvaluationCacheItem>>> m_redisCache;
    private IDictionary<int, ActivePolicyEvaluationCacheItem> m_cachePolicyDictionary = (IDictionary<int, ActivePolicyEvaluationCacheItem>) new Dictionary<int, ActivePolicyEvaluationCacheItem>();
    private bool m_bypassCache;
    private bool m_hasUnsavedChanges;

    public void BypassCache(IVssRequestContext requestContext)
    {
      this.m_cachePolicyDictionary = (IDictionary<int, ActivePolicyEvaluationCacheItem>) new Dictionary<int, ActivePolicyEvaluationCacheItem>();
      this.m_bypassCache = true;
    }

    public void Initialize(
      IVssRequestContext requestContext,
      string targetCacheKey,
      TimeSpan expirationTime,
      int minFileCountToUseCache)
    {
      if (this.IsInitialized)
        throw new InvalidOperationException("Cache is already initialized");
      this.m_requestContext = requestContext;
      this.m_expirationTime = expirationTime;
      this.m_targetKey = targetCacheKey;
      this.m_minFileCountToUseCache = minFileCountToUseCache;
      this.m_redisCache = new Lazy<IMutableDictionaryCacheContainer<string, IDictionary<int, ActivePolicyEvaluationCacheItem>>>((Func<IMutableDictionaryCacheContainer<string, IDictionary<int, ActivePolicyEvaluationCacheItem>>>) (() => this.InternalInitialize(requestContext)));
    }

    private IMutableDictionaryCacheContainer<string, IDictionary<int, ActivePolicyEvaluationCacheItem>> InternalInitialize(
      IVssRequestContext requestContext)
    {
      IMutableDictionaryCacheContainer<string, IDictionary<int, ActivePolicyEvaluationCacheItem>> redisCache = this.GetRedisCache(requestContext);
      if (redisCache == null)
      {
        requestContext.TraceAlways(1390143, TraceLevel.Error, "Policy", "PolicyComponent", "Redis is unavailable, can't connect of fetch the data");
        return (IMutableDictionaryCacheContainer<string, IDictionary<int, ActivePolicyEvaluationCacheItem>>) null;
      }
      IDictionary<int, ActivePolicyEvaluationCacheItem> dictionary;
      if (redisCache.TryGet<string, IDictionary<int, ActivePolicyEvaluationCacheItem>>(requestContext, this.m_targetKey, out dictionary))
        this.m_cachePolicyDictionary = dictionary;
      requestContext.TraceAlways(1390143, TraceLevel.Info, "Policy", "PolicyComponent", "ActivePolicyEvaluationCache fetched cache from Redis: TargetKey=" + this.m_targetKey + ", " + string.Format("TotalCachedPolicies={0}.", (object) this.m_cachePolicyDictionary.Count));
      return redisCache;
    }

    public void Invalidate(IVssRequestContext requestContext, string targetCacheKey, [CallerMemberName] string caller = null)
    {
      try
      {
        IMutableDictionaryCacheContainer<string, IDictionary<int, ActivePolicyEvaluationCacheItem>> redisCache = this.GetRedisCache(requestContext);
        if (this.m_expirationTime == TimeSpan.Zero)
          this.m_expirationTime = TimeSpan.FromSeconds((double) requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/PullRequestDetails/PullRequestDetailsCacheExpirationSeconds", true, 120));
        IEnumerable<TimeSpan?> live = redisCache.TimeToLive(requestContext, (IEnumerable<string>) new string[1]
        {
          targetCacheKey
        });
        TimeSpan timeSpan = this.m_expirationTime - ((live != null ? live.FirstOrDefault<TimeSpan?>() : new TimeSpan?()) ?? this.m_expirationTime);
        redisCache.Invalidate<string, IDictionary<int, ActivePolicyEvaluationCacheItem>>(requestContext, targetCacheKey);
        requestContext.TraceAlways(1390148, TraceLevel.Info, "Policy", ActivePolicyEvaluationCache.s_layer, string.Format("Invalidate redis cache from {0}. TargetKey={1}. timePassedSinceLastUpdate={2}", (object) caller, (object) targetCacheKey, (object) timeSpan));
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(1390148, TraceLevel.Warning, "Policy", ActivePolicyEvaluationCache.s_layer, ex);
      }
    }

    public void Set(
      IVssRequestContext requestContext,
      int policyId,
      ActivePolicyEvaluationCacheItem value)
    {
      this.EnsureInitialized();
      if (this.m_bypassCache)
        return;
      ActivePolicyEvaluationCacheItem evaluationCacheItem;
      if (!this.m_cachePolicyDictionary.TryGetValue(policyId, out evaluationCacheItem))
      {
        this.m_cachePolicyDictionary.Add(policyId, value);
      }
      else
      {
        if (!(evaluationCacheItem != value))
          return;
        this.m_cachePolicyDictionary[policyId] = value;
      }
      this.m_hasUnsavedChanges = true;
      requestContext.Trace(1390140, TraceLevel.Verbose, "Policy", ActivePolicyEvaluationCache.s_layer, "Set value to be cached. TargetKey={0}, PolicyKey={1}.", (object) this.m_targetKey, (object) policyId);
    }

    public void Remove(IVssRequestContext requestContext, int buildId) => throw new NotImplementedException("ActivePolicyEvaluationCache doesn't support cache item removing!");

    public bool TryGet(
      IVssRequestContext requestContext,
      int policyId,
      out ActivePolicyEvaluationCacheItem value)
    {
      value = new ActivePolicyEvaluationCacheItem();
      this.EnsureInitialized();
      if (this.m_bypassCache)
        return false;
      int num = this.m_cachePolicyDictionary.TryGetValue(policyId, out value) ? 1 : 0;
      if (num == 0)
        return num != 0;
      requestContext.Trace(1390141, TraceLevel.Verbose, "Policy", ActivePolicyEvaluationCache.s_layer, "Hit cached value. TargetKey={0}, PolicyKey={1}.", (object) this.m_targetKey, (object) policyId);
      return num != 0;
    }

    public void Dispose()
    {
      if (this.IsInitialized && !this.m_bypassCache && (this.m_requestContext.IsFeatureEnabled("Policy.EventBasedCacheEnabled") || this.m_hasUnsavedChanges))
      {
        this.m_redisCache.Value.Set(this.m_requestContext, (IDictionary<string, IDictionary<int, ActivePolicyEvaluationCacheItem>>) new Dictionary<string, IDictionary<int, ActivePolicyEvaluationCacheItem>>()
        {
          [this.m_targetKey] = this.m_cachePolicyDictionary
        });
        this.m_requestContext.TraceAlways(1390142, TraceLevel.Info, "Policy", ActivePolicyEvaluationCache.s_layer, "Saved cache into Redis. TargetKey={0}.", (object) this.m_targetKey);
        this.m_hasUnsavedChanges = false;
      }
      this.m_redisCache = (Lazy<IMutableDictionaryCacheContainer<string, IDictionary<int, ActivePolicyEvaluationCacheItem>>>) null;
    }

    private IMutableDictionaryCacheContainer<string, IDictionary<int, ActivePolicyEvaluationCacheItem>> GetRedisCache(
      IVssRequestContext requestContext)
    {
      IRedisCacheService service = requestContext.GetService<IRedisCacheService>();
      ContainerSettings containerSettings = new ContainerSettings()
      {
        CiAreaName = "Policy.Server.ActiveAvaluationPolicy.Results",
        KeyExpiry = new TimeSpan?(this.m_expirationTime)
      };
      IVssRequestContext requestContext1 = requestContext;
      Guid redisNamespace = ActivePolicyEvaluationCache.s_redisNamespace;
      ContainerSettings settings = containerSettings;
      return service.GetVolatileDictionaryContainer<string, IDictionary<int, ActivePolicyEvaluationCacheItem>, ActivePolicyEvaluationCache.ActivePolicyEvaluationCacheSecurityToken>(requestContext1, redisNamespace, settings);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureInitialized()
    {
      if (this.m_redisCache == null)
        return;
      IMutableDictionaryCacheContainer<string, IDictionary<int, ActivePolicyEvaluationCacheItem>> dictionaryCacheContainer = this.m_redisCache.Value;
    }

    public bool IsInitialized => this.m_redisCache != null && this.m_redisCache.IsValueCreated;

    public bool IsBypassed => this.m_bypassCache;

    public int MinFileCountToUseCache => this.m_minFileCountToUseCache;

    private class ActivePolicyEvaluationCacheSecurityToken
    {
    }
  }
}
