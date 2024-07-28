// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.RedisCacheService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Redis
{
  internal sealed class RedisCacheService : IRedisCacheService, IVssFrameworkService
  {
    private IVssServiceHost m_serviceHost;
    private Microsoft.VisualStudio.Services.Redis.V1.RedisCacheManager m_redisCacheManagerV1;
    private Microsoft.VisualStudio.Services.Redis.V2.RedisCacheManager m_redisCacheManagerV2;
    private VssRefreshCache<Dictionary<Guid, string>> m_namespaceToPoolMapping;
    private readonly Tracer m_tracer = PerformanceTracer.Global;

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      this.m_serviceHost = requestContext.ServiceHost;
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      this.m_redisCacheManagerV1 = context.GetService<Microsoft.VisualStudio.Services.Redis.V1.RedisCacheManager>();
      this.m_redisCacheManagerV2 = context.GetService<Microsoft.VisualStudio.Services.Redis.V2.RedisCacheManager>();
      this.m_namespaceToPoolMapping = new VssRefreshCache<Dictionary<Guid, string>>(TimeSpan.FromMinutes(1.0), new Func<IVssRequestContext, Dictionary<Guid, string>>(this.GetNamespaceToPoolMapping), true);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public bool IsEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Redis.V2") ? this.m_redisCacheManagerV2.IsEnabled(requestContext) : this.m_redisCacheManagerV1.IsEnabled(requestContext);

    public IMutableDictionaryCacheContainer<K, V> GetVolatileDictionaryContainer<K, V, S>(
      IVssRequestContext requestContext,
      Guid namespaceId,
      ContainerSettings settings = null)
    {
      this.ValidateRequestContext(requestContext);
      RedisCacheService.ValidateContainerSettings(requestContext, namespaceId, settings);
      string connectionPool = this.GetConnectionPool(requestContext, namespaceId);
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Redis.V2"))
      {
        settings = this.m_redisCacheManagerV2.ResolveSettings(requestContext, namespaceId, settings);
        string cacheId = this.m_redisCacheManagerV2.GetCacheId<S>(requestContext, namespaceId);
        return (IMutableDictionaryCacheContainer<K, V>) new Microsoft.VisualStudio.Services.Redis.V2.MutableDictionaryCacheContainer<K, V>((Microsoft.VisualStudio.Services.Redis.V2.IRedisConnectionFactory) new Microsoft.VisualStudio.Services.Redis.V2.RedisConnectionFactory(this.m_redisCacheManagerV2, connectionPool, namespaceId), cacheId, settings);
      }
      settings = this.m_redisCacheManagerV1.ResolveSettings(requestContext, namespaceId, settings);
      string cacheId1 = this.m_redisCacheManagerV1.GetCacheId<S>(requestContext, namespaceId);
      return (IMutableDictionaryCacheContainer<K, V>) new Microsoft.VisualStudio.Services.Redis.V1.MutableDictionaryCacheContainer<K, V>((Microsoft.VisualStudio.Services.Redis.V1.IRedisConnectionFactory) new Microsoft.VisualStudio.Services.Redis.V1.RedisConnectionFactory(this.m_redisCacheManagerV1, connectionPool, namespaceId), cacheId1, settings);
    }

    public IWindowedDictionaryCacheContainer<K> GetWindowedDictionaryContainer<K, S>(
      IVssRequestContext requestContext,
      Guid namespaceId,
      ContainerSettings settings = null)
    {
      this.ValidateRequestContext(requestContext);
      RedisCacheService.ValidateContainerSettings(requestContext, namespaceId, settings);
      string connectionPool = this.GetConnectionPool(requestContext, namespaceId);
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Redis.V2"))
      {
        settings = this.m_redisCacheManagerV2.ResolveSettings(requestContext, namespaceId, settings);
        string cacheId = this.m_redisCacheManagerV2.GetCacheId<S>(requestContext, namespaceId);
        return (IWindowedDictionaryCacheContainer<K>) new Microsoft.VisualStudio.Services.Redis.V2.WindowedDictionaryCacheContainer<K>((Microsoft.VisualStudio.Services.Redis.V2.IRedisConnectionFactory) new Microsoft.VisualStudio.Services.Redis.V2.RedisConnectionFactory(this.m_redisCacheManagerV2, connectionPool, namespaceId), cacheId, settings);
      }
      settings = this.m_redisCacheManagerV1.ResolveSettings(requestContext, namespaceId, settings);
      string cacheId1 = this.m_redisCacheManagerV1.GetCacheId<S>(requestContext, namespaceId);
      return (IWindowedDictionaryCacheContainer<K>) new Microsoft.VisualStudio.Services.Redis.V1.WindowedDictionaryCacheContainer<K>((Microsoft.VisualStudio.Services.Redis.V1.IRedisConnectionFactory) new Microsoft.VisualStudio.Services.Redis.V1.RedisConnectionFactory(this.m_redisCacheManagerV1, connectionPool, namespaceId), cacheId1, settings);
    }

    private string GetConnectionPool(IVssRequestContext requestContext, Guid namespaceId)
    {
      string str;
      return this.m_namespaceToPoolMapping.Get(requestContext).TryGetValue(namespaceId, out str) ? str : "$Default";
    }

    internal Dictionary<Guid, string> GetNamespaceToPoolMapping(IVssRequestContext requestContext)
    {
      Dictionary<Guid, string> namespaceToPoolMapping = new Dictionary<Guid, string>();
      RedisConfiguration redisConfiguration = !requestContext.IsFeatureEnabled("VisualStudio.Services.Redis.V2") ? this.m_redisCacheManagerV1.Configuration : this.m_redisCacheManagerV2.Configuration;
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) (redisConfiguration.Keys.NamespacesRootPath + "/**"));
      foreach (RegistryEntry registryEntry in registryEntryCollection)
      {
        Guid result;
        if (Guid.TryParse(((IEnumerable<string>) registryEntry.Path.Substring(redisConfiguration.Keys.NamespacesRootPath.Length).Split(new char[1]
        {
          '/'
        }, 2, StringSplitOptions.RemoveEmptyEntries)).FirstOrDefault<string>(), out result))
        {
          string valueFromPath = registryEntryCollection.GetValueFromPath<string>(redisConfiguration.Keys.ConnectionPool(result), (string) null);
          if (valueFromPath != null)
            namespaceToPoolMapping[result] = valueFromPath;
        }
      }
      return namespaceToPoolMapping;
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_serviceHost.InstanceId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.RedisCacheServiceRequestContextHostMessage((object) this.m_serviceHost.InstanceId, (object) requestContext.ServiceHost.InstanceId));
    }

    private static void ValidateContainerSettings(
      IVssRequestContext requestContext,
      Guid namespaceId,
      ContainerSettings settings)
    {
      if (settings != null && settings.CiAreaName != null)
        return;
      TraceLevel level = requestContext.ServiceHost.IsProduction ? TraceLevel.Info : TraceLevel.Error;
      requestContext.Trace(8110004, level, "Redis", "RedisCache", "CI area name is not provided for the Redis container '{0}'", (object) namespaceId);
    }
  }
}
