// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.YamlCacheService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal sealed class YamlCacheService : IYamlCacheService, IVssFrameworkService
  {
    private static readonly string s_layer = nameof (YamlCacheService);
    private static readonly string s_redisCIArea = "YamlCache";
    private static readonly Guid s_redisNamespace = new Guid("4de68b3a-95b4-421a-927c-ecdfe1b1028a");

    public void Set(IVssRequestContext requestContext, string key, string value)
    {
      requestContext.TraceInfo(10015532, YamlCacheService.s_layer, "Yaml cache set. Key={0}", (object) key);
      requestContext.GetService<YamlMemoryCacheService>().Set(requestContext, key, value);
      IMutableDictionaryCacheContainer<string, string> redisCache = this.GetRedisCache(requestContext);
      if (redisCache == null)
        return;
      Dictionary<string, string> items = new Dictionary<string, string>()
      {
        {
          key,
          value
        }
      };
      try
      {
        redisCache.Set(requestContext, (IDictionary<string, string>) items);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015531, YamlCacheService.s_layer, ex);
      }
    }

    public bool TryGetValue(IVssRequestContext requestContext, string key, out string value)
    {
      YamlMemoryCacheService service = requestContext.GetService<YamlMemoryCacheService>();
      if (service.TryGetValue(requestContext, key, out value))
      {
        requestContext.TraceInfo(10015533, YamlCacheService.s_layer, "Yaml memory cache hit. Key={0}", (object) key);
        return true;
      }
      requestContext.TraceInfo(10015534, YamlCacheService.s_layer, "Yaml memory cache miss. Key={0}", (object) key);
      IMutableDictionaryCacheContainer<string, string> redisCache = this.GetRedisCache(requestContext);
      if (redisCache != null)
      {
        if (redisCache.TryGet<string, string>(requestContext, key, out value))
        {
          requestContext.TraceInfo(10015535, YamlCacheService.s_layer, "Yaml Redis cache hit. Key={0}", (object) key);
          service.Set(requestContext, key, value);
          return true;
        }
        requestContext.TraceInfo(10015536, YamlCacheService.s_layer, "Yaml Redis cache miss. Key={0}", (object) key);
      }
      return false;
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private IMutableDictionaryCacheContainer<string, string> GetRedisCache(
      IVssRequestContext requestContext)
    {
      IRedisCacheService service1 = requestContext.GetService<IRedisCacheService>();
      if (!service1.IsEnabled(requestContext))
        return (IMutableDictionaryCacheContainer<string, string>) null;
      IVssRegistryService service2 = requestContext.GetService<IVssRegistryService>();
      ContainerSettings settings = new ContainerSettings()
      {
        CiAreaName = YamlCacheService.s_redisCIArea,
        KeyExpiry = new TimeSpan?(service2.GetValue<TimeSpan>(requestContext, in RegistryKeys.PipelineRedisCacheExpiryInterval, false, TimeSpan.FromHours(1.0)))
      };
      return service1.GetVolatileDictionaryContainer<string, string, YamlCacheService.YamlCacheSecurityToken>(requestContext, YamlCacheService.s_redisNamespace, settings);
    }

    private sealed class YamlCacheSecurityToken
    {
    }
  }
}
