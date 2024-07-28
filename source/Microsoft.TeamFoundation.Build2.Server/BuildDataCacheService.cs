// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDataCacheService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildDataCacheService : IBuildDataCacheService, IVssFrameworkService
  {
    private static readonly string s_layer = nameof (BuildDataCacheService);
    private static readonly string s_buildDataArea = "BuildDataCache";
    private static readonly Guid s_redisNamespace = typeof (BuildData).GetTypeInfo().Module.ModuleVersionId;
    private static readonly string s_registrySettingsPath = "/Service/Build/Settings/BuildDataRedisCache/";
    private static readonly RegistryQuery s_expiryInterval = (RegistryQuery) (BuildDataCacheService.s_registrySettingsPath + "ExpiryInterval");

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void Set(IVssRequestContext requestContext, BuildData value)
    {
      if (!requestContext.IsFeatureEnabled("Build2.BuildDataCache") || value == null)
        return;
      int id = value.Id;
      requestContext.TraceInfo(12030141, BuildDataCacheService.s_layer, "Build data cache set. Key={0}", (object) id);
      requestContext.GetService<BuildDataMemoryCacheService>().Set(requestContext, id, value);
      try
      {
        IMutableDictionaryCacheContainer<int, BuildData> redisCache = this.GetRedisCache(requestContext);
        if (redisCache == null)
          return;
        Dictionary<int, BuildData> items = new Dictionary<int, BuildData>()
        {
          {
            id,
            value
          }
        };
        redisCache.Set(requestContext, (IDictionary<int, BuildData>) items);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12030124, BuildDataCacheService.s_layer, ex);
      }
    }

    public void Remove(IVssRequestContext requestContext, int buildId)
    {
      if (!requestContext.IsFeatureEnabled("Build2.BuildDataCache"))
        return;
      requestContext.GetService<BuildDataMemoryCacheService>().Remove(requestContext, buildId);
      this.GetRedisCache(requestContext).Invalidate<int, BuildData>(requestContext, buildId);
    }

    public bool TryGet(IVssRequestContext requestContext, int buildId, out BuildData buildData)
    {
      buildData = (BuildData) null;
      if (!requestContext.IsFeatureEnabled("Build2.BuildDataCache"))
        return false;
      BuildDataMemoryCacheService service = requestContext.GetService<BuildDataMemoryCacheService>();
      if (service.TryGetValue(requestContext, buildId, out buildData))
      {
        requestContext.TraceInfo(12030142, BuildDataCacheService.s_layer, "Build data cache get from memory cache. Key={0}", (object) buildId);
        return true;
      }
      IMutableDictionaryCacheContainer<int, BuildData> redisCache = this.GetRedisCache(requestContext);
      if (redisCache == null)
        return false;
      bool flag = redisCache.TryGet<int, BuildData>(requestContext, buildId, out buildData);
      if (flag)
      {
        requestContext.TraceInfo(12030142, BuildDataCacheService.s_layer, "Build data cache get from redis cache. Key={0}, updating memory cache.", (object) buildId);
        service.Set(requestContext, buildId, buildData);
      }
      return flag;
    }

    private IMutableDictionaryCacheContainer<int, BuildData> GetRedisCache(
      IVssRequestContext requestContext)
    {
      IRedisCacheService service1 = requestContext.GetService<IRedisCacheService>();
      IVssRegistryService service2 = requestContext.GetService<IVssRegistryService>();
      ContainerSettings containerSettings = new ContainerSettings()
      {
        CiAreaName = BuildDataCacheService.s_buildDataArea,
        KeyExpiry = new TimeSpan?(service2.GetValue<TimeSpan>(requestContext, in BuildDataCacheService.s_expiryInterval, TimeSpan.FromMinutes(1.0)))
      };
      IVssRequestContext requestContext1 = requestContext;
      Guid redisNamespace = BuildDataCacheService.s_redisNamespace;
      ContainerSettings settings = containerSettings;
      return service1.GetVolatileDictionaryContainer<int, BuildData, BuildDataCacheService.BuildDataCacheSecurityToken>(requestContext1, redisNamespace, settings);
    }

    private class BuildDataCacheSecurityToken
    {
    }
  }
}
