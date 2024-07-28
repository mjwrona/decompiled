// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsRemoteCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.ImsPerfCounters;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsRemoteCache : IImsRemoteCache, IVssFrameworkService, IImsDataCache
  {
    private static readonly Dictionary<Type, ImsRemoteCache.ImsRemoteCacheTraceKind> ValueTypeToTraceKind = new Dictionary<Type, ImsRemoteCache.ImsRemoteCacheTraceKind>()
    {
      {
        typeof (ImsCacheIdentityId),
        ImsRemoteCache.ImsRemoteCacheTraceKind.IdentityId
      },
      {
        typeof (ImsCacheIdentity),
        ImsRemoteCache.ImsRemoteCacheTraceKind.Identity
      },
      {
        typeof (ImsCacheScopeMembership),
        ImsRemoteCache.ImsRemoteCacheTraceKind.ScopeMembership
      },
      {
        typeof (ImsCacheChildren),
        ImsRemoteCache.ImsRemoteCacheTraceKind.Children
      },
      {
        typeof (ImsCacheDescendants),
        ImsRemoteCache.ImsRemoteCacheTraceKind.Descendants
      },
      {
        typeof (ImsCacheIdentitiesInScope),
        ImsRemoteCache.ImsRemoteCacheTraceKind.IdentitiesInScope
      },
      {
        typeof (ImsCacheIdentitiesByDisplayName),
        ImsRemoteCache.ImsRemoteCacheTraceKind.IdentitiesByDisplayName
      },
      {
        typeof (ImsCacheIdentitiesByAccountName),
        ImsRemoteCache.ImsRemoteCacheTraceKind.IdentitiesByAccountName
      },
      {
        typeof (ImsCacheDisplayNameSearchIndex),
        ImsRemoteCache.ImsRemoteCacheTraceKind.DisplayNameSearchIndex
      },
      {
        typeof (ImsCacheEmailSearchIndex),
        ImsRemoteCache.ImsRemoteCacheTraceKind.EmailSearchIndex
      },
      {
        typeof (ImsCacheAccountNameSearchIndex),
        ImsRemoteCache.ImsRemoteCacheTraceKind.AccountNameSearchIndex
      },
      {
        typeof (ImsCacheAppIdSearchIndex),
        ImsRemoteCache.ImsRemoteCacheTraceKind.AppIdSearchIndex
      },
      {
        typeof (ImsCacheIdentityIdByAppId),
        ImsRemoteCache.ImsRemoteCacheTraceKind.IdentitiesByAppId
      }
    };
    private Guid m_serviceHostId;
    private readonly Dictionary<Type, Guid> m_namespaces = new Dictionary<Type, Guid>();
    private readonly Dictionary<Type, ContainerSettings> m_containerSettings = new Dictionary<Type, ContainerSettings>();
    private readonly HashSet<Type> m_registeredTypes = new HashSet<Type>();
    private readonly ActionTracer m_tracer = new ActionTracer("Microsoft.VisualStudio.Services.Identity", nameof (ImsRemoteCache));
    private static readonly IConfigPrototype<bool> ImsRemoteCacheStorageIsAzureBlobStoreEnabledPrototype;
    private readonly IConfigQueryable<bool> ImsRemoteCacheStorageIsAzureBlobStoreEnabledConfig;
    private static readonly HashSet<Type> m_typesForBlobStore = new HashSet<Type>()
    {
      typeof (ImsCacheDisplayNameSearchIndex),
      typeof (ImsCacheAccountNameSearchIndex),
      typeof (ImsCacheEmailSearchIndex),
      typeof (ImsCacheDomainAccountNameSearchIndex),
      typeof (ImsCacheAppIdSearchIndex)
    };
    private const string s_area = "Microsoft.VisualStudio.Services.Identity";
    private const string s_layer = "ImsRemoteCache";
    internal static readonly IValueSerializer ImsCacheObjectSerializerWithCompression = (IValueSerializer) new JsonTextSerializer(JsonSerializer.Create(new JsonSerializerSettings()), true);
    internal static readonly IValueSerializer ImsCacheObjectSerializerWithoutCompression = (IValueSerializer) new JsonTextSerializer(JsonSerializer.Create(new JsonSerializerSettings()));
    internal static readonly IKeySerializer ImsCacheKeySerializer = (IKeySerializer) new Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheKeySerializer();

    static ImsRemoteCache() => ImsRemoteCache.ImsRemoteCacheStorageIsAzureBlobStoreEnabledPrototype = ConfigPrototype.Create<bool>("Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.AzureBlobStoreIsStorage", false);

    public ImsRemoteCache()
      : this(ConfigProxy.Create<bool>(ImsRemoteCache.ImsRemoteCacheStorageIsAzureBlobStoreEnabledPrototype))
    {
    }

    public ImsRemoteCache(
      IConfigQueryable<bool> imsRemoteCacheStorageIsAzureBlobStoreEnabledConfig)
    {
      this.ImsRemoteCacheStorageIsAzureBlobStoreEnabledConfig = imsRemoteCacheStorageIsAzureBlobStoreEnabledConfig;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_serviceHostId = systemRequestContext.ServiceHost.InstanceId;
      this.ReloadSettings(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private void ReloadSettings(IVssRequestContext context)
    {
      ImsCacheSettings.ImsRemoteCacheRegistrySettings settings = this.GetSettings(context);
      foreach (Type registeredType in this.m_registeredTypes)
      {
        this.m_namespaces[registeredType] = settings.Namespaces[registeredType];
        this.m_containerSettings[registeredType] = ImsRemoteCache.GetContainerSettings(settings.CacheEntryTimeToLive[registeredType], settings.CompressCacheEntry[registeredType]);
      }
    }

    private static ContainerSettings GetContainerSettings(TimeSpan keyExpiry, bool useCompression)
    {
      IValueSerializer valueSerializer = useCompression ? ImsRemoteCache.ImsCacheObjectSerializerWithCompression : ImsRemoteCache.ImsCacheObjectSerializerWithoutCompression;
      return new ContainerSettings()
      {
        ValueSerializer = valueSerializer,
        KeySerializer = ImsRemoteCache.ImsCacheKeySerializer,
        KeyExpiry = new TimeSpan?(keyExpiry),
        CiAreaName = typeof (ImsRemoteCache).Name,
        AllowBatching = new bool?(true)
      };
    }

    public void AddObjectTypes(IVssRequestContext context, IEnumerable<Type> types)
    {
      ImsCacheUtils.ValidateTypes(types);
      if (types.All<Type>((Func<Type, bool>) (type => this.m_registeredTypes.Contains(type))))
        return;
      this.m_registeredTypes.UnionWith(types);
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Deployment);
      context1.GetService<ImsCacheSettings>().ReloadRemoteCacheSettings(context1, this.m_registeredTypes);
      this.ReloadSettings(context);
    }

    public IDictionary<K, V> GetObjects<K, V>(IVssRequestContext context, ICollection<K> keys)
      where K : ImsCacheKey
      where V : ImsCacheObject
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForNull<ICollection<K>>(keys, nameof (keys));
      return (IDictionary<K, V>) this.m_tracer.TraceAction<Dictionary<K, V>>(context, (ActionTracePoints) ImsRemoteCache.TracePoints.GetObjects, (Func<Dictionary<K, V>>) (() =>
      {
        if (!ImsRemoteCache.IsFeatureEnabled(context) || !this.m_registeredTypes.Contains(typeof (V)) || ImsCacheUtils.CacheBypassRequested(context) || ServicingUtils.IsHostUpgrade(context))
          return ((IEnumerable<K>) keys).ToDedupedDictionary<K, K, V>((Func<K, K>) (key => key), (Func<K, V>) (key => default (V)));
        if (keys.Count == 0)
          return ((IEnumerable<K>) keys).ToDedupedDictionary<K, K, V>((Func<K, K>) (key => key), (Func<K, V>) (key => default (V)));
        ImsRemoteCache.TraceGetObjectsRequest<K, V>(context, keys);
        List<KeyValuePair<V, bool>> list = this.GetCacheContainer<V>(context).TryGet(context, (IEnumerable<ImsCacheKey>) keys).ToList<KeyValuePair<V, bool>>();
        ImsRemoteCache.TraceGetObjectsResponse<K, V>(context, (IEnumerable<KeyValuePair<V, bool>>) list);
        if (context.IsTracing(331200000, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity", nameof (ImsRemoteCache)))
        {
          foreach (KeyValuePair<K, V> keyValuePair in list.Zip<KeyValuePair<V, bool>, K, KeyValuePair<K, V>>((IEnumerable<K>) keys, (Func<KeyValuePair<V, bool>, K, KeyValuePair<K, V>>) ((kvp, k) => new KeyValuePair<K, V>(k, kvp.Value ? kvp.Key : default (V)))))
          {
            KeyValuePair<K, V> cacheResult = keyValuePair;
            if ((object) cacheResult.Value == null)
              context.TraceConditionally(331200000, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity", nameof (ImsRemoteCache), (Func<string>) (() => cacheResult.Key?.Serialize()));
            else
              context.TraceConditionally(331200001, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity", nameof (ImsRemoteCache), (Func<string>) (() => cacheResult.Serialize<KeyValuePair<K, V>>()));
          }
        }
        return list.Zip<KeyValuePair<V, bool>, K, KeyValuePair<K, V>>((IEnumerable<K>) keys, (Func<KeyValuePair<V, bool>, K, KeyValuePair<K, V>>) ((kvp, k) => new KeyValuePair<K, V>(k, kvp.Value ? kvp.Key : default (V)))).ToDedupedDictionary<KeyValuePair<K, V>, K, V>((Func<KeyValuePair<K, V>, K>) (kvp => kvp.Key), (Func<KeyValuePair<K, V>, V>) (kvp => kvp.Value));
      }), nameof (GetObjects));
    }

    public void AddObjects<T>(IVssRequestContext context, IEnumerable<T> objects) where T : ImsCacheObject
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForNull<IEnumerable<T>>(objects, nameof (objects));
      this.m_tracer.TraceAction(context, (ActionTracePoints) ImsRemoteCache.TracePoints.AddObjects, (Action) (() =>
      {
        if (!ImsRemoteCache.IsFeatureEnabled(context) || !this.m_registeredTypes.Contains(typeof (T)) || ServicingUtils.IsHostUpgrade(context))
          return;
        Dictionary<ImsCacheKey, T> dedupedDictionary = objects.ToDedupedDictionary<T, ImsCacheKey, T>((Func<T, ImsCacheKey>) (x => x.Key), (Func<T, T>) (x => x));
        if (dedupedDictionary.Count == 0)
          return;
        ImsRemoteCache.TraceAddObjects<T>(context, dedupedDictionary);
        this.GetCacheContainer<T>(context).Set(context, (IDictionary<ImsCacheKey, T>) dedupedDictionary);
      }), nameof (AddObjects));
    }

    public void RemoveObjects<K, V>(IVssRequestContext context, ICollection<K> keys)
      where K : ImsCacheKey
      where V : ImsCacheObject
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForNull<ICollection<K>>(keys, nameof (keys));
      this.m_tracer.TraceAction(context, (ActionTracePoints) ImsRemoteCache.TracePoints.RemoveObjects, (Action) (() =>
      {
        if (!ImsRemoteCache.IsFeatureEnabled(context) || !this.m_registeredTypes.Contains(typeof (V)) || keys.Count == 0)
          return;
        ImsRemoteCache.TraceRemoveObjects<K, V>(context, keys);
        this.GetCacheContainer<V>(context).Invalidate(context, (IEnumerable<ImsCacheKey>) keys);
      }), nameof (RemoveObjects));
    }

    private IMutableDictionaryCacheContainer<ImsCacheKey, T> GetCacheContainer<T>(
      IVssRequestContext context)
    {
      return this.m_tracer.TraceAction<IMutableDictionaryCacheContainer<ImsCacheKey, T>>(context, (ActionTracePoints) ImsRemoteCache.TracePoints.GetCacheContainer, (Func<IMutableDictionaryCacheContainer<ImsCacheKey, T>>) (() =>
      {
        if (!this.m_namespaces.ContainsKey(typeof (T)))
          throw new ArgumentException(string.Format("A namespace for type {0} was not found.", (object) typeof (T)));
        Dictionary<Type, ContainerSettings> containerSettings = this.m_containerSettings;
        if (!containerSettings.ContainsKey(typeof (T)))
        {
          this.m_tracer.TraceError(context, 1460832, (Func<string>) (() => string.Format("A configured container setting for type {0} was not found; using default one.", (object) typeof (T))), nameof (GetCacheContainer));
          containerSettings[typeof (T)] = ImsRemoteCache.GetContainerSettings(ImsCacheConstants.Registry.LocalOrRemoteCache.Defaults.DefaultRemoteCacheEntryTimeToLive, false);
        }
        context.TraceConditionally(1654844, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity", nameof (ImsRemoteCache), (Func<string>) (() =>
        {
          IValueSerializer valueSerializer = containerSettings[typeof (T)].ValueSerializer;
          return string.Format("Serializer without auto type? {0}", (object) (bool) (valueSerializer == ImsRemoteCache.ImsCacheObjectSerializerWithCompression ? (true ? 1 : 0) : (valueSerializer == ImsRemoteCache.ImsCacheObjectSerializerWithoutCompression ? 1 : 0)));
        }));
        if (!this.ShouldStoreInAzureBlobStore(context, typeof (T)))
          return context.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<ImsCacheKey, T, ImsRemoteCache.IdentityCacheSecurityToken>(context, this.m_namespaces[typeof (T)], containerSettings[typeof (T)]);
        ImsCacheTeamFoundationFileServiceAsCacheContainer<ImsCacheKey, T> service = context.GetService<ImsCacheTeamFoundationFileServiceAsCacheContainer<ImsCacheKey, T>>();
        service.ReloadSettings(containerSettings[typeof (T)]);
        return (IMutableDictionaryCacheContainer<ImsCacheKey, T>) service;
      }), nameof (GetCacheContainer));
    }

    private static bool IsFeatureEnabled(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      int num = requestContext.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache") ? 1 : 0;
      bool flag = requestContext.GetService<IRedisCacheService>().IsEnabled(requestContext);
      if (num != 0 && !flag)
        requestContext.Trace(1654843, TraceLevel.Warning, "Microsoft.VisualStudio.Services.Identity", nameof (ImsRemoteCache), string.Format("The remote caching feature {0} has been enabled for host {1}, but Redis is either disabled or unconfigured", (object) "Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache", (object) requestContext.ServiceHost.InstanceId));
      return (num & (flag ? 1 : 0)) != 0;
    }

    private bool ShouldStoreInAzureBlobStore(IVssRequestContext requestContext, Type cacheValueType) => !requestContext.ExecutionEnvironment.IsOnPremisesDeployment && this.ImsRemoteCacheStorageIsAzureBlobStoreEnabledConfig.QueryByCtx<bool>(requestContext.To(TeamFoundationHostType.Application)) && ImsRemoteCache.m_typesForBlobStore.Contains(cacheValueType);

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!this.m_serviceHostId.Equals(requestContext.ServiceHost.InstanceId))
        throw new InvalidRequestContextHostException(FrameworkResources.CacheServiceRequestContextHostMessage((object) this.m_serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }

    private ImsCacheSettings.ImsRemoteCacheRegistrySettings GetSettings(IVssRequestContext context) => context.To(TeamFoundationHostType.Deployment).GetService<ImsCacheSettings>().ImsRemoteCacheSettings;

    public string GetCacheId<V>(IVssRequestContext context) where V : ImsCacheObject
    {
      this.ValidateRequestContext(context);
      return !ImsRemoteCache.IsFeatureEnabled(context) ? (string) null : this.GetCacheContainer<V>(context).GetCacheId();
    }

    private static int ConvertToTracePoint<V>(int keysCount, int leastSignificantBit)
    {
      Type key = typeof (V);
      ImsRemoteCache.ImsRemoteCacheTraceKind remoteCacheTraceKind;
      ImsRemoteCache.ValueTypeToTraceKind.TryGetValue(key, out remoteCacheTraceKind);
      return 330000000 + (int) remoteCacheTraceKind * 100000 + keysCount * 10 + leastSignificantBit;
    }

    private static void TraceGetObjectsRequest<K, V>(
      IVssRequestContext context,
      ICollection<K> keysAsList)
      where K : ImsCacheKey
      where V : ImsCacheObject
    {
      int count = keysAsList.Count;
      if (count > 0)
        context.TraceConditionally(ImsRemoteCache.ConvertToTracePoint<V>(keysAsList.Count, 1), TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Identity", nameof (ImsRemoteCache), (Func<string>) (() => string.Format("ImsRemoteCache.GetObjectsRequest for type {0} where keys : {1}", (object) typeof (V), (object) keysAsList.Serialize<ICollection<K>>())));
      string instancename = ImsCacheInstanceNames.GetInstancename<K, V>();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_GetObjects_Requests", instancename).Increment();
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_GetObjects_Objects", instancename);
      performanceCounter.IncrementBy((long) count);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_GetObjects_RequestsPerSecond", instancename);
      performanceCounter.Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_GetObjects_ObjectsPerSecond", instancename).IncrementBy((long) count);
    }

    private static void TraceGetObjectsResponse<K, V>(
      IVssRequestContext context,
      IEnumerable<KeyValuePair<V, bool>> keyValuePairs)
      where K : ImsCacheKey
      where V : ImsCacheObject
    {
      int num1 = 0;
      int num2 = 0;
      foreach (KeyValuePair<V, bool> keyValuePair in keyValuePairs)
      {
        if (keyValuePair.Value)
          ++num1;
        else
          ++num2;
      }
      int keysCount = num1 + num2;
      if (keysCount > 0)
        context.TraceConditionally(ImsRemoteCache.ConvertToTracePoint<V>(keysCount, 4), TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Identity", nameof (ImsRemoteCache), (Func<string>) (() => string.Format("ImsRemoteCache.GetObjectsResponse for type {0} where values : {1}", (object) typeof (V), (object) keyValuePairs.Serialize<IEnumerable<KeyValuePair<V, bool>>>())));
      string instancename = ImsCacheInstanceNames.GetInstancename<K, V>();
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_GetObjects_Hits", instancename);
      performanceCounter.IncrementBy((long) num1);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_GetObjects_HitsPerSecond", instancename);
      performanceCounter.IncrementBy((long) num1);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_GetObjects_Misses", instancename);
      performanceCounter.IncrementBy((long) num2);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_GetObjects_MissesPerSecond", instancename);
      performanceCounter.IncrementBy((long) num2);
    }

    private static void TraceAddObjects<T>(
      IVssRequestContext context,
      Dictionary<ImsCacheKey, T> objectsMap)
      where T : ImsCacheObject
    {
      int count = objectsMap.Count;
      if (count <= 0)
        return;
      context.TraceConditionally(ImsRemoteCache.ConvertToTracePoint<T>(objectsMap.Count, 2), TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Identity", nameof (ImsRemoteCache), (Func<string>) (() => string.Format("ImsRemoteCache.AddObjects for type {0} where objects : {1}", (object) typeof (T), (object) objectsMap.Serialize<Dictionary<ImsCacheKey, T>>())));
      string instancename = ImsCacheInstanceNames.GetInstancename<T>(objectsMap.First<KeyValuePair<ImsCacheKey, T>>().Key.GetType());
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_AddObjects_Requests", instancename).Increment();
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_AddObjects_Objects", instancename);
      performanceCounter.IncrementBy((long) count);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_AddObjects_RequestsPerSecond", instancename);
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_AddObjects_ObjectsPerSecond", instancename);
      performanceCounter.IncrementBy((long) count);
    }

    private static void TraceRemoveObjects<K, V>(
      IVssRequestContext context,
      ICollection<K> keysAsList)
      where K : ImsCacheKey
      where V : ImsCacheObject
    {
      int count = keysAsList.Count;
      if (count > 0)
        context.TraceConditionally(ImsRemoteCache.ConvertToTracePoint<V>(keysAsList.Count, 3), TraceLevel.Verbose, "Microsoft.VisualStudio.Services.Identity", nameof (ImsRemoteCache), (Func<string>) (() => string.Format("ImsRemoteCache.RemoveObjects for type {0} where keys : {1}", (object) typeof (V), (object) keysAsList.Serialize<ICollection<K>>())));
      string instancename = ImsCacheInstanceNames.GetInstancename<K, V>();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_RemoveObjects_Requests", instancename).Increment();
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_RemoveObjects_Objects", instancename);
      performanceCounter.IncrementBy((long) count);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_RemoveObjects_RequestsPerSecond", instancename);
      performanceCounter.Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ImsRemoteCache_RemoveObjects_ObjectsPerSecond", instancename).IncrementBy((long) count);
    }

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints GetObjects = new TimedActionTracePoints(1460801, 1460807, 1460808, 1460809);
      internal static readonly TimedActionTracePoints AddObjects = new TimedActionTracePoints(1460811, 1460817, 1460818, 1460819);
      internal static readonly TimedActionTracePoints RemoveObjects = new TimedActionTracePoints(1460821, 1460827, 1460828, 1460829);
      internal static readonly TimedActionTracePoints GetCacheContainer = new TimedActionTracePoints(1460831, 1460837, 1460838, 1460839);
    }

    private enum ImsRemoteCacheTraceKind
    {
      None,
      Children,
      Descendants,
      Identity,
      IdentitiesInScope,
      IdentitiesByDisplayName,
      IdentityId,
      ScopeMembership,
      IdentitiesByAccountName,
      DisplayNameSearchIndex,
      EmailSearchIndex,
      AccountNameSearchIndex,
      IdentitiesByAppId,
      AppIdSearchIndex,
    }

    internal class IdentityCacheSecurityToken
    {
    }
  }
}
