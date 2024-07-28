// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsLocalSearchCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsLocalSearchCache : IImsLocalSearchCache, IVssFrameworkService
  {
    private static readonly Dictionary<Type, ImsLocalSearchCache.ImsLocalSearchCacheTraceKind> IndexTypeToTraceKind = new Dictionary<Type, ImsLocalSearchCache.ImsLocalSearchCacheTraceKind>()
    {
      {
        typeof (ImsCacheIdentityIdByDisplayName),
        ImsLocalSearchCache.ImsLocalSearchCacheTraceKind.IdentityIdByDisplayName
      }
    };
    private TeamFoundationTask m_cleanupTask;
    private Guid m_serviceHostId;
    private readonly IDictionary<Guid, IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>> m_searchCaches;
    private readonly IDictionary<Guid, IDictionary<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>>> m_auxiliarySearchCaches;
    private const string s_area = "Microsoft.VisualStudio.Services.Identity";
    private const string s_layer = "ImsLocalSearchCache";

    internal ImsLocalSearchCache()
    {
      this.m_searchCaches = (IDictionary<Guid, IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>>) new ConcurrentDictionary<Guid, IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>>();
      this.m_auxiliarySearchCaches = (IDictionary<Guid, IDictionary<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>>>) new ConcurrentDictionary<Guid, IDictionary<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>>>();
    }

    public void ServiceStart(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      ImsCacheSettings.ImsLocalSearchCacheRegistrySettings settings = this.GetSettings(context.To(TeamFoundationHostType.Deployment));
      if (!(settings.CacheCleanUpInterval != TimeSpan.Zero))
        return;
      this.m_cleanupTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.ProcessCleanup), (object) null, (int) settings.CacheCleanUpInterval.TotalMilliseconds);
      context.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(context, this.m_cleanupTask);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
      if (this.m_cleanupTask == null)
        return;
      context.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().RemoveTask(context, this.m_cleanupTask);
    }

    internal void ProcessCleanup(IVssRequestContext requestContext, object taskArgs)
    {
      try
      {
        requestContext.TraceEnter(1554800, "Microsoft.VisualStudio.Services.Identity", nameof (ImsLocalSearchCache), nameof (ProcessCleanup));
        this.PublishExpiryEventsForSoonToExpireLargeCaches(requestContext);
        ImsCacheSettings.ImsLocalSearchCacheRegistrySettings settings = this.GetSettings(requestContext);
        foreach (IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>> source in (IEnumerable<IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>>) this.m_searchCaches.Values)
        {
          foreach (Type key in source.Where<KeyValuePair<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>>((Func<KeyValuePair<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>, bool>) (x => x.Value.Time + settings.SearchCacheSettings.GetSearchCacheTimeToLive(x.Value.Value.Count) < DateTimeOffset.UtcNow)).Select<KeyValuePair<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>, Type>((Func<KeyValuePair<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>, Type>) (x => x.Key)).ToList<Type>())
            source.Remove(key);
        }
        if (this.m_cleanupTask != null && (double) this.m_cleanupTask.Interval != settings.CacheCleanUpInterval.TotalMilliseconds)
          this.RescheduleCleanupTask(requestContext);
        foreach (KeyValuePair<Guid, IDictionary<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>>> auxiliarySearchCach in (IEnumerable<KeyValuePair<Guid, IDictionary<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>>>>) this.m_auxiliarySearchCaches)
        {
          IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>> dictionary = (IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>) null;
          if (this.m_searchCaches.TryGetValue(auxiliarySearchCach.Key, out dictionary))
          {
            foreach (KeyValuePair<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>> keyValuePair in (IEnumerable<KeyValuePair<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>>>) auxiliarySearchCach.Value)
            {
              if (!dictionary.ContainsKey(keyValuePair.Key))
                auxiliarySearchCach.Value.Remove(keyValuePair.Key);
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1554808, "Microsoft.VisualStudio.Services.Identity", nameof (ImsLocalSearchCache), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1554810, "Microsoft.VisualStudio.Services.Identity", nameof (ImsLocalSearchCache), nameof (ProcessCleanup));
      }
    }

    private void PublishExpiryEventsForSoonToExpireLargeCaches(IVssRequestContext requestContext)
    {
      ImsCacheSettings.ImsLocalSearchCacheRegistrySettings settings = this.GetSettings(requestContext);
      bool shouldPublishForEnterpriseDomain = requestContext.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.SearchCacheWarmup.EnterpriseDomain.Enable");
      foreach (KeyValuePair<Guid, IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>> searchCach in (IEnumerable<KeyValuePair<Guid, IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>>>) this.m_searchCaches)
      {
        Guid key = searchCach.Key;
        foreach (KeyValuePair<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>> keyValuePair in (IEnumerable<KeyValuePair<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>>) searchCach.Value)
        {
          int count = keyValuePair.Value.Value.Count;
          DateTimeOffset time = keyValuePair.Value.Time;
          if (count > settings.SearchCacheSettings.SearchCacheMediumSizeThreshold && time + settings.SearchCacheSettings.GetSearchCacheTimeToLive(count) - settings.SearchCacheSettings.SearchCacheSoonToExpireAlertDuration < DateTimeOffset.UtcNow)
          {
            IdentitySearchHelper.PublishImsSearchCacheExpiryEvent(requestContext, key, shouldPublishForEnterpriseDomain);
            break;
          }
        }
      }
    }

    private void RescheduleCleanupTask(IVssRequestContext context)
    {
      TeamFoundationTask cleanupTask = this.m_cleanupTask;
      TeamFoundationTask task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.ProcessCleanup), (object) null, (int) this.GetSettings(context).CacheCleanUpInterval.TotalMilliseconds);
      ITeamFoundationTaskService service = context.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>();
      service.AddTask(context, task);
      this.m_cleanupTask = task;
      service.RemoveTask(context, cleanupTask);
    }

    public virtual bool HasIndex<T>(IVssRequestContext context, Guid indexId) where T : ImsCacheObject<string, IdentityId>
    {
      this.ValidateRequestContext(context);
      if (!ImsLocalSearchCache.IsFeatureEnabled(context))
        return false;
      ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>> searchCache = (ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>) null;
      return this.TryGetSearchCachesForIndex<T>(indexId, out searchCache);
    }

    public virtual bool IsIndexStale<T>(IVssRequestContext context, Guid indexId) where T : ImsCacheObject<string, IdentityId>
    {
      this.ValidateRequestContext(context);
      ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>> searchCache;
      return ImsLocalSearchCache.IsFeatureEnabled(context) && this.TryGetSearchCachesForIndex<T>(indexId, out searchCache) && this.IsExpired<ISearchable<IdentityId>>(context, searchCache);
    }

    public virtual void CreateIndex<T>(
      IVssRequestContext context,
      Guid indexId,
      ICollection<T> objectsToIndex,
      DateTimeOffset creationTime)
      where T : ImsCacheObject<string, IdentityId>
    {
      this.ValidateRequestContext(context);
      if (!ImsLocalSearchCache.IsFeatureEnabled(context))
        return;
      ArgumentUtility.CheckForEmptyGuid(indexId, nameof (indexId));
      ArgumentUtility.CheckForNull<ICollection<T>>(objectsToIndex, nameof (objectsToIndex));
      if (objectsToIndex.Any<T>((Func<T, bool>) (obj => obj?.Key == null || obj.Value == null)))
        throw new ArgumentNullException(nameof (objectsToIndex), "Attempt to add null object, or an object with null key or value.");
      PrefixSearchArray<IdentityId> prefixSearchArray = new PrefixSearchArray<IdentityId>(objectsToIndex.Select<T, KeyValuePair<string, IdentityId>>((Func<T, KeyValuePair<string, IdentityId>>) (x => new KeyValuePair<string, IdentityId>(x.Key.Id.ToString(), x.Value))));
      ImsLocalSearchCache.TraceCreateIndex<T>(context, objectsToIndex);
      IDictionary<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>> dictionary1 = (IDictionary<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>>) null;
      if (this.m_auxiliarySearchCaches.TryGetValue(indexId, out dictionary1))
        dictionary1.Remove(typeof (T));
      IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>> dictionary2;
      if (!this.m_searchCaches.TryGetValue(indexId, out dictionary2))
        this.m_searchCaches[indexId] = dictionary2 = (IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>) new ConcurrentDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>();
      dictionary2[typeof (T)] = new ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>((ISearchable<IdentityId>) prefixSearchArray, creationTime);
    }

    public virtual void AddToIndex<T>(
      IVssRequestContext context,
      Guid indexId,
      ICollection<T> objects)
      where T : ImsCacheObject<string, IdentityId>
    {
      this.ValidateRequestContext(context);
      if (!ImsLocalSearchCache.IsFeatureEnabled(context))
        return;
      ArgumentUtility.CheckForEmptyGuid(indexId, nameof (indexId));
      ArgumentUtility.CheckForNull<ICollection<T>>(objects, nameof (objects));
      if (objects.Any<T>((Func<T, bool>) (obj => obj?.Key == null || obj.Value == null)))
        throw new ArgumentNullException(nameof (objects), "Attempt to add null object, or an object with null key or value.");
      if (!objects.Any<T>() || !this.TryGetSearchCachesForIndex<T>(indexId, out ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>> _))
        return;
      IList<T> objList = (IList<T>) new List<T>();
      foreach (T obj in (IEnumerable<T>) objects)
      {
        string key = obj.Key.Id.ToString();
        IEnumerable<IdentityId> identityIds = this.SearchIndex<T>(context, indexId, (ICollection<string>) new string[1]
        {
          key
        })[key];
        if (identityIds.IsNullOrEmpty<IdentityId>() || !identityIds.Contains<IdentityId>(obj.Value))
          objList.Add(obj);
      }
      objects = (ICollection<T>) objList;
      if (!objects.Any<T>())
        return;
      IDictionary<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>> dictionary = (IDictionary<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>>) null;
      if (!this.m_auxiliarySearchCaches.TryGetValue(indexId, out dictionary))
        dictionary = this.m_auxiliarySearchCaches[indexId] = (IDictionary<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>>) new ConcurrentDictionary<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>>();
      ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>> timedObject;
      if (!dictionary.TryGetValue(typeof (T), out timedObject))
      {
        PrefixSearchTrie<IdentityId> prefixSearchTrie = new PrefixSearchTrie<IdentityId>(objects.Select<T, KeyValuePair<string, IdentityId>>((Func<T, KeyValuePair<string, IdentityId>>) (x => new KeyValuePair<string, IdentityId>(x.Key.Id.ToString(), x.Value))));
        dictionary[typeof (T)] = new ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>(prefixSearchTrie);
      }
      else
      {
        foreach (T obj in (IEnumerable<T>) objects)
        {
          string key = obj.Key.Id.ToString();
          timedObject.Value.Add(key, obj.Value);
        }
      }
    }

    public virtual IDictionary<string, IEnumerable<IdentityId>> SearchIndex<T>(
      IVssRequestContext context,
      Guid indexId,
      ICollection<string> keys)
      where T : ImsCacheObject<string, IdentityId>
    {
      this.ValidateRequestContext(context);
      if (!ImsLocalSearchCache.IsFeatureEnabled(context) || ImsCacheUtils.CacheBypassRequested(context))
        return (IDictionary<string, IEnumerable<IdentityId>>) keys.ToDedupedDictionary<string, string, IEnumerable<IdentityId>>((Func<string, string>) (key => key), (Func<string, IEnumerable<IdentityId>>) (key => (IEnumerable<IdentityId>) null));
      ArgumentUtility.CheckForEmptyGuid(indexId, nameof (indexId));
      ArgumentUtility.CheckForNull<ICollection<string>>(keys, nameof (keys));
      ImsLocalSearchCache.TraceSearchIndexRequest<T>(context, keys);
      ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>> searchCache;
      if (!this.TryGetSearchCachesForIndex<T>(indexId, out searchCache))
      {
        ImsLocalSearchCache.TraceSearchIndexResponse<T>(context, keys, false);
        return (IDictionary<string, IEnumerable<IdentityId>>) keys.ToDedupedDictionary<string, string, IEnumerable<IdentityId>>((Func<string, string>) (key => key), (Func<string, IEnumerable<IdentityId>>) (key => (IEnumerable<IdentityId>) null));
      }
      ImsLocalSearchCache.TraceSearchIndexResponse<T>(context, keys, true);
      ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>> auxiliarySearchCache;
      return !this.TryGetAuxiliarySearchCachesForIndex<T>(indexId, out auxiliarySearchCache) ? (IDictionary<string, IEnumerable<IdentityId>>) keys.ToDedupedDictionary<string, string, IEnumerable<IdentityId>>((Func<string, string>) (key => key), (Func<string, IEnumerable<IdentityId>>) (key => searchCache.Value.Search(key))) : (IDictionary<string, IEnumerable<IdentityId>>) keys.ToDedupedDictionary<string, string, IEnumerable<IdentityId>>((Func<string, string>) (key => key), (Func<string, IEnumerable<IdentityId>>) (key => searchCache.Value.Search(key).ToList<IdentityId>().Union<IdentityId>((IEnumerable<IdentityId>) auxiliarySearchCache.Value.Search(key).ToList<IdentityId>())));
    }

    public virtual void InvalidateIndex<T>(IVssRequestContext context, Guid indexId) where T : ImsCacheObject<string, IdentityId>
    {
      this.ValidateRequestContext(context);
      if (!ImsLocalSearchCache.IsFeatureEnabled(context))
        return;
      ArgumentUtility.CheckForEmptyGuid(indexId, nameof (indexId));
      ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>> searchCache;
      if (!this.TryGetSearchCachesForIndex<T>(indexId, out searchCache) || this.IsExpired<ISearchable<IdentityId>>(context, searchCache))
        return;
      ImsCacheSettings.ImsLocalSearchCacheRegistrySettings settings = this.GetSettings(context);
      if (searchCache.Value.Count > settings.SearchCacheSettings.SearchCacheTinySizeThreshold)
        return;
      IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>> dictionary = (IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>) null;
      if (!this.m_searchCaches.TryGetValue(indexId, out dictionary))
        return;
      dictionary.Remove(typeof (T));
    }

    private bool TryGetSearchCachesForIndex<T>(
      Guid indexId,
      out ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>> searchCache)
    {
      searchCache = (ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>) null;
      IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>> dictionary = (IDictionary<Type, ImsLocalSearchCache.TimedObject<ISearchable<IdentityId>>>) null;
      return this.m_searchCaches.TryGetValue(indexId, out dictionary) && dictionary.TryGetValue(typeof (T), out searchCache);
    }

    private bool TryGetAuxiliarySearchCachesForIndex<T>(
      Guid indexId,
      out ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>> auxiliarySearchCache)
    {
      auxiliarySearchCache = (ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>) null;
      IDictionary<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>> dictionary = (IDictionary<Type, ImsLocalSearchCache.TimedObject<PrefixSearchTrie<IdentityId>>>) null;
      return this.m_auxiliarySearchCaches.TryGetValue(indexId, out dictionary) && dictionary.TryGetValue(typeof (T), out auxiliarySearchCache);
    }

    private static bool IsFeatureEnabled(IVssRequestContext requestContext) => requestContext.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.LocalSearchCache");

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!this.m_serviceHostId.Equals(requestContext.ServiceHost.InstanceId))
        throw new InvalidRequestContextHostException(FrameworkResources.CacheServiceRequestContextHostMessage((object) this.m_serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }

    private ImsCacheSettings.ImsLocalSearchCacheRegistrySettings GetSettings(
      IVssRequestContext context)
    {
      return context.To(TeamFoundationHostType.Deployment).GetService<ImsCacheSettings>().ImsLocalSearchCacheSettings;
    }

    private bool IsExpired<T>(
      IVssRequestContext context,
      ImsLocalSearchCache.TimedObject<T> searchCache)
      where T : ISearchable<IdentityId>
    {
      TimeSpan searchCacheTimeToLive = this.GetSettings(context.To(TeamFoundationHostType.Deployment)).SearchCacheSettings.GetSearchCacheTimeToLive(searchCache.Value.Count);
      return searchCache.Time + searchCacheTimeToLive < DateTimeOffset.UtcNow;
    }

    private static void TraceSearchIndexRequest<T>(
      IVssRequestContext context,
      ICollection<string> keys)
      where T : ImsCacheObject<string, IdentityId>
    {
      int count = keys.Count;
      if (count > 0)
        context.TraceConditionally(ImsLocalSearchCache.ConvertToTracePoint<T>(count, 1), TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Identity", nameof (ImsLocalSearchCache), (Func<string>) (() => string.Format("ImsLocalSearchCache.SearchIndex for type {0} where keys : {1}", (object) typeof (T), (object) keys.Serialize<ICollection<string>>())));
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.Requests");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.Objects");
      performanceCounter.IncrementBy((long) count);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.RequestsPerSecond");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.ObjectsPerSecond");
      performanceCounter.IncrementBy((long) count);
    }

    private static void TraceSearchIndexResponse<T>(
      IVssRequestContext context,
      ICollection<string> keys,
      bool cacheHit)
      where T : ImsCacheObject<string, IdentityId>
    {
      int count = keys.Count;
      if (count > 0)
        context.TraceConditionally(ImsLocalSearchCache.ConvertToTracePoint<T>(count, 2), TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Identity", nameof (ImsLocalSearchCache), (Func<string>) (() => string.Format("ImsLocalSearchCache.SearchIndex for type {0} where keys : {1}", (object) typeof (T), (object) keys.Serialize<ICollection<string>>())));
      if (cacheHit)
      {
        VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.Hits");
        performanceCounter.Increment();
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.Misses");
        performanceCounter.IncrementBy(1L);
      }
      else
      {
        VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.HitsPerSecond");
        performanceCounter.Increment();
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.SearchIndex.MissesPerSecond");
        performanceCounter.IncrementBy(1L);
      }
    }

    private static void TraceCreateIndex<T>(
      IVssRequestContext context,
      ICollection<T> objectsToIndex)
      where T : ImsCacheObject<string, IdentityId>
    {
      int count = objectsToIndex.Count;
      if (count > 0)
        context.TraceConditionally(ImsLocalSearchCache.ConvertToTracePoint<T>(count, 3), TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Identity", nameof (ImsLocalSearchCache), (Func<string>) (() => string.Format("ImsLocalSearchCache.CreateIndex for type {0} where objects : {1}", (object) typeof (T), (object) objectsToIndex.Serialize<ICollection<T>>())));
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.CreateIndex.Requests");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.CreateIndex.Objects");
      performanceCounter.IncrementBy((long) count);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.CreateIndex.RequestsPerSecond");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsLocalSearchCache.CreateIndex.ObjectsPerSecond");
      performanceCounter.IncrementBy((long) count);
    }

    private static int ConvertToTracePoint<T>(int keysCount, int leastSignificantBit)
    {
      Type key = typeof (T);
      ImsLocalSearchCache.ImsLocalSearchCacheTraceKind searchCacheTraceKind;
      ImsLocalSearchCache.IndexTypeToTraceKind.TryGetValue(key, out searchCacheTraceKind);
      return 380000000 + (int) searchCacheTraceKind * 100000 + keysCount * 10 + leastSignificantBit;
    }

    private enum ImsLocalSearchCacheTraceKind
    {
      None,
      IdentityIdByDisplayName,
    }

    private class TimedObject<T>
    {
      internal TimedObject(T obj)
        : this(obj, DateTimeOffset.UtcNow)
      {
      }

      internal TimedObject(T obj, DateTimeOffset time)
      {
        this.Value = obj;
        this.Time = time;
      }

      internal T Value { get; }

      internal DateTimeOffset Time { get; }
    }
  }
}
