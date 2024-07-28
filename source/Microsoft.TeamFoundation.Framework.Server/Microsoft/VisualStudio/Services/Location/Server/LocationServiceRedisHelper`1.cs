// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.LocationServiceRedisHelper`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal class LocationServiceRedisHelper<T> : 
    LocationServiceRedisHelper,
    ILocationServiceRedisHelper<T>
  {
    private static readonly ContainerSettings s_cacheContainerSettings = new ContainerSettings()
    {
      ValueSerializer = (IValueSerializer) new JsonTextSerializer(new JsonSerializer()
      {
        TypeNameHandling = TypeNameHandling.None
      }, true),
      CiAreaName = "LocationService",
      NoThrowMode = new bool?(false)
    };

    public virtual void TryClearCache(IVssRequestContext requestContext) => this.SetNamespaceId(requestContext, Guid.NewGuid());

    public void TryClearCache(IVssRequestContext requestContext, T key)
    {
      requestContext.TraceEnter(1903573052, "LocationService", nameof (LocationServiceRedisHelper<T>), nameof (TryClearCache));
      try
      {
        if (!requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.LocationService.RedisCache"))
          return;
        IMutableDictionaryCacheContainer<T, LocationData> container = this.GetCacheContainer(requestContext);
        this.PerformRedisWrite(requestContext, (Action) (() => container.Invalidate<T, LocationData>(requestContext, key)));
      }
      finally
      {
        requestContext.TraceLeave(570083354, "LocationService", nameof (LocationServiceRedisHelper<T>), nameof (TryClearCache));
      }
    }

    public LocationData GetLocationData(IVssRequestContext requestContext, T key)
    {
      requestContext.TraceEnter(756588797, "LocationService", nameof (LocationServiceRedisHelper<T>), nameof (GetLocationData));
      try
      {
        LocationData locationData = (LocationData) null;
        if (requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.LocationService.RedisCache") && !ServicingUtils.IsHostUpgrade(requestContext))
          this.GetCacheContainer(requestContext).TryGet<T, LocationData>(requestContext, key, out locationData);
        requestContext.Trace(locationData == null ? 756588795 : 756588796, TraceLevel.Info, "LocationService", nameof (LocationServiceRedisHelper<T>), "Cache {0} for {1}", locationData == null ? (object) "miss" : (object) "hit", (object) key);
        DateTime? cacheExpirationDate = locationData?.CacheExpirationDate;
        DateTime dateTime = DateTime.MinValue + TimeSpan.FromSeconds(100.0);
        if ((cacheExpirationDate.HasValue ? (cacheExpirationDate.GetValueOrDefault() > dateTime ? 1 : 0) : 0) != 0)
        {
          int num = new Random().Next(100);
          locationData.CacheExpirationDate -= TimeSpan.FromSeconds((double) num);
        }
        return locationData;
      }
      finally
      {
        requestContext.TraceLeave(756588798, "LocationService", nameof (LocationServiceRedisHelper<T>), nameof (GetLocationData));
      }
    }

    public void SetLocationData(IVssRequestContext requestContext, T key, LocationData data)
    {
      requestContext.TraceEnter(756586797, "LocationService", nameof (LocationServiceRedisHelper<T>), nameof (SetLocationData));
      try
      {
        if (!requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.LocationService.RedisCache") || ServicingUtils.IsHostUpgrade(requestContext))
          return;
        ContainerSettings settings = LocationServiceRedisHelper<T>.s_cacheContainerSettings.Clone();
        TimeSpan timeSpan = data.CacheExpirationDate.Subtract(DateTime.UtcNow);
        if (timeSpan.TotalSeconds < 0.0)
        {
          requestContext.Trace(2134830262, TraceLevel.Error, "LocationService", nameof (LocationServiceRedisHelper<T>), "Expiry for key {0} is {1}", (object) key, (object) timeSpan.TotalSeconds);
          settings.KeyExpiry = new TimeSpan?(TimeSpan.FromSeconds(0.0));
        }
        else
        {
          settings.KeyExpiry = new TimeSpan?(timeSpan);
          requestContext.Trace(2134830264, TraceLevel.Info, "LocationService", nameof (LocationServiceRedisHelper<T>), "Expiry for key {0} is {1}", (object) key, (object) timeSpan.TotalSeconds);
        }
        IMutableDictionaryCacheContainer<T, LocationData> container = this.GetCacheContainer(requestContext, settings);
        this.PerformRedisWrite(requestContext, (Action) (() => container.Set(requestContext, (IDictionary<T, LocationData>) new Dictionary<T, LocationData>()
        {
          {
            key,
            data
          }
        })));
      }
      finally
      {
        requestContext.TraceLeave(756548798, "LocationService", nameof (LocationServiceRedisHelper<T>), nameof (SetLocationData));
      }
    }

    private bool PerformRedisWrite(IVssRequestContext requestContext, Action action)
    {
      try
      {
        action();
        return true;
      }
      catch (Exception ex) when (ex is RedisException || ex is CircuitBreakerException)
      {
        requestContext.Trace(368505511, TraceLevel.Error, "LocationService", nameof (LocationServiceRedisHelper<T>), "Redis write failed: {0}", (object) (ex.InnerException?.Message ?? ex.Message));
        return false;
      }
    }

    private IMutableDictionaryCacheContainer<T, LocationData> GetCacheContainer(
      IVssRequestContext requestContext,
      ContainerSettings settings = null)
    {
      Guid namespaceId = this.GetNamespaceId(requestContext);
      return requestContext.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<T, LocationData, LocationServiceRedisHelper.RedisCacheSecurityToken>(requestContext, namespaceId, settings ?? LocationServiceRedisHelper<T>.s_cacheContainerSettings);
    }
  }
}
