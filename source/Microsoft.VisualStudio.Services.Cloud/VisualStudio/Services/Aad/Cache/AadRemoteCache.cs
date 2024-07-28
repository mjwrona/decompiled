// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Cache.AadRemoteCache
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad.Cache
{
  internal class AadRemoteCache : IAadRemoteCache, IVssFrameworkService, IAadCache
  {
    private readonly Type[] types;
    private IDictionary<Type, Guid> namespaces;
    private IDictionary<Type, ContainerSettings> containerSettings;
    private IDictionary<Type, int> chunkSizes;
    internal static readonly IValueSerializer valueSerializer = (IValueSerializer) new JsonTextSerializer(JsonSerializer.Create(new JsonSerializerSettings()));

    internal AadRemoteCache()
      : this(typeof (AadCacheAncestorIds), typeof (AadCacheTenant))
    {
    }

    internal AadRemoteCache(params Type[] types)
    {
      AadCacheUtils.ValidateTypes((IEnumerable<Type>) types);
      this.types = types;
      this.namespaces = (IDictionary<Type, Guid>) new ConcurrentDictionary<Type, Guid>();
      this.containerSettings = (IDictionary<Type, ContainerSettings>) new ConcurrentDictionary<Type, ContainerSettings>();
      this.chunkSizes = (IDictionary<Type, int>) new ConcurrentDictionary<Type, int>();
      foreach (Type type in types)
      {
        this.namespaces[type] = Guid.NewGuid();
        this.containerSettings[type] = AadRemoteCache.MergeWithRedisDefault(new ContainerSettings()
        {
          ValueSerializer = AadRemoteCache.valueSerializer,
          CiAreaName = type.Name
        });
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/Aad/Cache/RemoteCache/...");
      this.ReloadSettings(vssRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().UnregisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    private void OnRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<RegistryEntryCollection>(changedEntries, nameof (changedEntries));
      if (!changedEntries.Any<RegistryEntry>())
        return;
      this.ReloadSettings(context);
    }

    private void ReloadSettings(IVssRequestContext context)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      foreach (Type type in this.types)
      {
        this.namespaces[type] = service.GetValue<Guid>(vssRequestContext, (RegistryQuery) AadCacheConstants.RegistryKeys.RemoteCache.RedisNamespace[type], AadCacheConstants.DefaultSettings.RemoteCache.RedisNamespace[type]);
        IDictionary<Type, ContainerSettings> containerSettings1 = this.containerSettings;
        Type key1 = type;
        ContainerSettings settings = new ContainerSettings();
        settings.ValueSerializer = AadRemoteCache.valueSerializer;
        IVssRegistryService registryService1 = service;
        IVssRequestContext requestContext1 = vssRequestContext;
        RegistryQuery registryQuery = (RegistryQuery) AadCacheConstants.RegistryKeys.RemoteCache.RedisKeyExpiry[type];
        ref RegistryQuery local1 = ref registryQuery;
        TimeSpan? defaultValue1 = new TimeSpan?();
        settings.KeyExpiry = registryService1.GetValue<TimeSpan?>(requestContext1, in local1, defaultValue1);
        settings.CiAreaName = type.Name;
        ContainerSettings containerSettings2 = AadRemoteCache.MergeWithRedisDefault(settings);
        containerSettings1[key1] = containerSettings2;
        IDictionary<Type, int> chunkSizes = this.chunkSizes;
        Type key2 = type;
        IVssRegistryService registryService2 = service;
        IVssRequestContext requestContext2 = vssRequestContext;
        registryQuery = (RegistryQuery) AadCacheConstants.RegistryKeys.RemoteCache.RedisKeysChunkSize[type];
        ref RegistryQuery local2 = ref registryQuery;
        int defaultValue2 = AadCacheConstants.DefaultSettings.RemoteCache.RedisKeysChunkSize[type];
        int num = registryService2.GetValue<int>(requestContext2, in local2, defaultValue2);
        chunkSizes[key2] = num;
      }
    }

    public IEnumerable<AadCacheLookup<T>> GetObjects<T>(
      IVssRequestContext context,
      IEnumerable<AadCacheKey> keys)
      where T : AadCacheObject
    {
      AadRemoteCache.ValidateRequestContext(context);
      ArgumentUtility.CheckForNull<IEnumerable<AadCacheKey>>(keys, nameof (keys));
      int num = keys.Count<AadCacheKey>();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadRemoteCache.GetObjects.Requests").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadRemoteCache.GetObjects.Objects").IncrementBy((long) num);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadRemoteCache.GetObjects.RequestsPerSecond").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadRemoteCache.GetObjects.ObjectsPerSecond").IncrementBy((long) num);
      if (!AadRemoteCache.IsFeatureEnabled(context))
        return keys.Select<AadCacheKey, AadCacheLookup<T>>((Func<AadCacheKey, AadCacheLookup<T>>) (key => new AadCacheLookup<T>(key, AadCacheLookupStatus.Miss, (Exception) null)));
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      IMutableDictionaryCacheContainer<AadCacheKey, T> cacheContainer = this.GetCacheContainer<T>(vssRequestContext);
      List<KeyValuePair<T, bool>> first = new List<KeyValuePair<T, bool>>();
      int chunkSize = 0;
      if (!this.TryGetChunkSize<T>(out chunkSize))
      {
        first.AddRange(cacheContainer.TryGet(vssRequestContext, keys));
      }
      else
      {
        foreach (IList<AadCacheKey> keys1 in keys.Batch<AadCacheKey>(chunkSize))
          first.AddRange(cacheContainer.TryGet(vssRequestContext, (IEnumerable<AadCacheKey>) keys1));
      }
      return first.Zip<KeyValuePair<T, bool>, AadCacheKey, AadCacheLookup<T>>(keys, (Func<KeyValuePair<T, bool>, AadCacheKey, AadCacheLookup<T>>) ((kvp, k) => new AadCacheLookup<T>(k, kvp.Value ? kvp.Key : default (T))));
    }

    private bool TryGetChunkSize<T>(out int chunkSize) where T : AadCacheObject
    {
      chunkSize = 0;
      return this.chunkSizes.TryGetValue(typeof (T), out chunkSize) && chunkSize > 0;
    }

    public void AddObjects<T>(IVssRequestContext context, IEnumerable<T> objects) where T : AadCacheObject
    {
      AadRemoteCache.ValidateRequestContext(context);
      ArgumentUtility.CheckForNull<IEnumerable<T>>(objects, nameof (objects));
      int num = objects.Count<T>();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadRemoteCache.AddObjects.Requests").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadRemoteCache.AddObjects.Objects").IncrementBy((long) num);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadRemoteCache.AddObjects.RequestsPerSecond").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadRemoteCache.AddObjects.ObjectsPerSecond").IncrementBy((long) num);
      if (!AadRemoteCache.IsFeatureEnabled(context))
        return;
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      IMutableDictionaryCacheContainer<AadCacheKey, T> cacheContainer = this.GetCacheContainer<T>(vssRequestContext);
      int chunkSize = 0;
      if (!this.TryGetChunkSize<T>(out chunkSize))
      {
        cacheContainer.Set(vssRequestContext, (IDictionary<AadCacheKey, T>) objects.Where<T>((Func<T, bool>) (x => (object) x != null)).ToDictionary<T, AadCacheKey, T>((Func<T, AadCacheKey>) (x => x.Key), (Func<T, T>) (x => x)));
      }
      else
      {
        foreach (IList<T> source in objects.Batch<T>(chunkSize))
          cacheContainer.Set(vssRequestContext, (IDictionary<AadCacheKey, T>) source.Where<T>((Func<T, bool>) (x => (object) x != null)).ToDictionary<T, AadCacheKey, T>((Func<T, AadCacheKey>) (x => x.Key), (Func<T, T>) (x => x)));
      }
    }

    public void RemoveObjects<T>(IVssRequestContext context, IEnumerable<AadCacheKey> keys) where T : AadCacheObject
    {
      AadRemoteCache.ValidateRequestContext(context);
      ArgumentUtility.CheckForNull<IEnumerable<AadCacheKey>>(keys, nameof (keys));
      int num = keys.Count<AadCacheKey>();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadRemoteCache.RemoveObjects.Requests").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadRemoteCache.RemoveObjects.Objects").IncrementBy((long) num);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadRemoteCache.RemoveObjects.RequestsPerSecond").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Aad.PerfCounters.AadRemoteCache.RemoveObjects.ObjectsPerSecond").IncrementBy((long) num);
      if (!AadRemoteCache.IsFeatureEnabled(context))
        return;
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      this.GetCacheContainer<T>(vssRequestContext).Invalidate(vssRequestContext, keys);
    }

    private IMutableDictionaryCacheContainer<AadCacheKey, T> GetCacheContainer<T>(
      IVssRequestContext context)
    {
      context.CheckServiceHostType(TeamFoundationHostType.Deployment);
      return context.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<AadCacheKey, T, AadRemoteCache.AadRemoteCacheSecurityToken>(context, this.namespaces[typeof (T)], this.containerSettings[typeof (T)]);
    }

    private static ContainerSettings MergeWithRedisDefault(ContainerSettings settings)
    {
      ContainerSettings containerSettings1 = new ContainerSettings()
      {
        CiAreaName = typeof (AadRemoteCache).Name
      };
      if (settings == null)
        return containerSettings1;
      ContainerSettings containerSettings2 = new ContainerSettings();
      TimeSpan? keyExpiry1;
      if (settings.KeyExpiry.HasValue)
      {
        TimeSpan? keyExpiry2 = settings.KeyExpiry;
        TimeSpan maxExpiry = RedisConfiguration.MaxExpiry;
        if ((keyExpiry2.HasValue ? (keyExpiry2.GetValueOrDefault() <= maxExpiry ? 1 : 0) : 0) != 0)
        {
          keyExpiry1 = settings.KeyExpiry;
          goto label_6;
        }
      }
      keyExpiry1 = containerSettings1.KeyExpiry;
label_6:
      containerSettings2.KeyExpiry = keyExpiry1;
      containerSettings2.KeySerializer = settings.KeySerializer ?? containerSettings1.KeySerializer;
      containerSettings2.ValueSerializer = settings.ValueSerializer ?? containerSettings1.ValueSerializer;
      containerSettings2.CiAreaName = settings.CiAreaName ?? containerSettings1.CiAreaName;
      return containerSettings2;
    }

    private static bool IsFeatureEnabled(IVssRequestContext requestContext)
    {
      int num = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Aad.Cache.RemoteCache") ? 1 : 0;
      bool flag = requestContext.GetService<IRedisCacheService>().IsEnabled(requestContext);
      if (num != 0 && !flag)
        requestContext.Trace(2654843, TraceLevel.Warning, "VisualStudio.Services.Aad", "Cache", string.Format("The remote caching feature {0} has been enabled for host {1}, but Redis is either disabled or unconfigured", (object) "Microsoft.VisualStudio.Services.Aad.Cache.RemoteCache", (object) requestContext.ServiceHost.InstanceId));
      return (num & (flag ? 1 : 0)) != 0;
    }

    private static void ValidateRequestContext(IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (!context.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
    }

    internal class AadRemoteCacheSecurityToken
    {
    }
  }
}
