// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsLocalDataCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.ImsPerfCounters;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsLocalDataCache : IImsLocalDataCache, IVssFrameworkService, IImsDataCache
  {
    private Guid m_serviceHostId;
    private static readonly IDictionary<Type, Func<IVssRequestContext, ImsMemoryCache>> s_knownTypesToCacheResolverMap = (IDictionary<Type, Func<IVssRequestContext, ImsMemoryCache>>) new Dictionary<Type, Func<IVssRequestContext, ImsMemoryCache>>()
    {
      {
        typeof (ImsCacheIdentitiesInScope),
        (Func<IVssRequestContext, ImsMemoryCache>) (context => (ImsMemoryCache) context.GetService<ImsIdentitiesInScopeMemoryCache>())
      },
      {
        typeof (ImsCacheChildren),
        (Func<IVssRequestContext, ImsMemoryCache>) (context => (ImsMemoryCache) context.GetService<ImsChildrenMemoryCache>())
      }
    };
    private const string s_area = "Microsoft.VisualStudio.Services.Identity";
    private const string s_layer = "ImsLocalDataCache";

    public void ServiceStart(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      this.m_serviceHostId = context.ServiceHost.InstanceId;
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public void AddObjectTypes(IVssRequestContext context, IEnumerable<Type> types) => ImsCacheUtils.ValidateTypes(types);

    public IDictionary<K, V> GetObjects<K, V>(IVssRequestContext context, ICollection<K> keys)
      where K : ImsCacheKey
      where V : ImsCacheObject
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForNull<ICollection<K>>(keys, nameof (keys));
      if (!ImsLocalDataCache.IsFeatureEnabled(context))
        return (IDictionary<K, V>) keys.ToDedupedDictionary<K, K, V>((Func<K, K>) (key => key), (Func<K, V>) (key => default (V)));
      string instancename = ImsCacheInstanceNames.GetInstancename<K, V>();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsLocalCache_GetObjects_Requests", instancename).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsLocalCache_GetObjects_Objects", instancename).IncrementBy((long) keys.Count);
      VssPerformanceCounter performanceCounter1 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsLocalCache_GetObjects_RequestsPerSecond", instancename);
      performanceCounter1.Increment();
      performanceCounter1 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsLocalCache_GetObjects_ObjectsPerSecond", instancename);
      performanceCounter1.IncrementBy((long) keys.Count);
      ImsMemoryCache cache;
      Dictionary<K, V> source = !ImsLocalDataCache.TryGetCache<V>(context, out cache) ? keys.ToDedupedDictionary<K, K, V>((Func<K, K>) (key => key), (Func<K, V>) (key => default (V))) : keys.ToDedupedDictionary<K, K, V>((Func<K, K>) (key => key), (Func<K, V>) (key => ImsLocalDataCache.GetObject<V>(context, cache, (ImsCacheKey) key)));
      int num1 = source.Count<KeyValuePair<K, V>>((Func<KeyValuePair<K, V>, bool>) (kvp => (object) kvp.Value != null));
      int num2 = source.Count - num1;
      VssPerformanceCounter performanceCounter2 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsLocalCache_GetObjects_Hits", instancename);
      performanceCounter2.IncrementBy((long) num1);
      performanceCounter2 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsLocalCache_GetObjects_HitsPerSecond", instancename);
      performanceCounter2.IncrementBy((long) num1);
      performanceCounter2 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsLocalCache_GetObjects_Misses", instancename);
      performanceCounter2.IncrementBy((long) num2);
      performanceCounter2 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsLocalCache_GetObjects_MissesPerSecond", instancename);
      performanceCounter2.IncrementBy((long) num2);
      return (IDictionary<K, V>) source;
    }

    private static V GetObject<V>(
      IVssRequestContext context,
      ImsMemoryCache cache,
      ImsCacheKey key)
      where V : ImsCacheObject
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key), "Attempt to get null key.");
      ImsCacheObject imsCacheObject;
      return !cache.TryGetValue(context, key, out imsCacheObject) ? default (V) : imsCacheObject?.Clone() as V;
    }

    public void AddObjects<V>(IVssRequestContext context, IEnumerable<V> objects) where V : ImsCacheObject
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForNull<IEnumerable<V>>(objects, nameof (objects));
      ImsMemoryCache cache;
      if (!ImsLocalDataCache.IsFeatureEnabled(context) || !ImsLocalDataCache.TryGetCache<V>(context, out cache))
        return;
      if (!(objects is ICollection<V> vs))
        vs = (ICollection<V>) objects.ToList<V>();
      ICollection<V> source = vs;
      if (source.Count > 0)
      {
        string instancename = ImsCacheInstanceNames.GetInstancename<V>(source.First<V>().Key.GetType());
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_AddObjects_Requests", instancename).Increment();
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_AddObjects_Objects", instancename).IncrementBy((long) source.Count);
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_AddObjects_RequestsPerSecond", instancename).Increment();
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_AddObjects_ObjectsPerSecond", instancename).IncrementBy((long) source.Count);
      }
      foreach (V v in (IEnumerable<V>) source)
      {
        if ((object) v == null)
          throw new ArgumentNullException(nameof (objects), "Attempt to add null object.");
        cache.Set(context, v.Key, (ImsCacheObject) v.Clone());
      }
    }

    public void RemoveObjects<K, V>(IVssRequestContext context, ICollection<K> keys)
      where K : ImsCacheKey
      where V : ImsCacheObject
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForNull<ICollection<K>>(keys, nameof (keys));
      ImsMemoryCache cache;
      if (!ImsLocalDataCache.TryGetCache<V>(context, out cache))
        return;
      string instancename = ImsCacheInstanceNames.GetInstancename<K, V>();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsLocalCache_RemoveObjects_Requests", instancename).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsLocalCache_RemoveObjects_Objects", instancename).IncrementBy((long) keys.Count);
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsLocalCache_RemoveObjects_RequestsPerSecond", instancename);
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsLocalCache_RemoveObjects_ObjectsPerSecond", instancename);
      performanceCounter.IncrementBy((long) keys.Count);
      foreach (K key in (IEnumerable<K>) keys)
      {
        if ((object) key == null)
          throw new ArgumentNullException(nameof (keys), "Attempt to remove null key.");
        cache.Remove(context, (ImsCacheKey) key);
      }
    }

    private static bool IsFeatureEnabled(IVssRequestContext requestContext) => requestContext.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.LocalCache");

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!this.m_serviceHostId.Equals(requestContext.ServiceHost.InstanceId))
        throw new InvalidRequestContextHostException(FrameworkResources.CacheServiceRequestContextHostMessage((object) this.m_serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }

    private static bool TryGetCache<V>(IVssRequestContext context, out ImsMemoryCache cache) where V : ImsCacheObject
    {
      Func<IVssRequestContext, ImsMemoryCache> func;
      if (!ImsLocalDataCache.s_knownTypesToCacheResolverMap.TryGetValue(typeof (V), out func))
      {
        cache = (ImsMemoryCache) null;
        return false;
      }
      cache = func(context);
      return true;
    }
  }
}
