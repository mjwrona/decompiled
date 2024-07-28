// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V2.RedisCacheManager
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Redis.V2
{
  public sealed class RedisCacheManager : IVssFrameworkService
  {
    private const string s_area = "Redis";
    private const string s_layer = "RedisCacheManager";
    private static readonly TimeSpan s_reconnectionPeriod = TimeSpan.FromMinutes(5.0);
    private readonly CacheNamespace m_cacheNamespace = new CacheNamespace();
    private RedisConfiguration m_redisConfiguration;
    private Microsoft.VisualStudio.Services.Redis.Tracer m_tracer;
    private RegistrySettingsChangedCallback m_configurationChangedCallback;
    private StrongBoxItemChangedCallback m_secretChangedCallback;
    private IDictionary<string, IRedisConnectionPool> m_connectionPools = (IDictionary<string, IRedisConnectionPool>) new Dictionary<string, IRedisConnectionPool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    internal RedisCacheManager()
      : this(RedisConfiguration.Default, Microsoft.VisualStudio.Services.Redis.Tracer.Global, (IEnumerable<IRedisConnectionPool>) null)
    {
    }

    internal RedisCacheManager(
      RedisConfiguration configuration,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer,
      IEnumerable<IRedisConnectionPool> pools)
    {
      this.m_redisConfiguration = configuration.Clone();
      this.m_tracer = tracer;
      foreach (IRedisConnectionPool redisConnectionPool in pools ?? Enumerable.Empty<IRedisConnectionPool>())
        this.m_connectionPools.Add(redisConnectionPool.Name, redisConnectionPool);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => this.Initialize(requestContext);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => this.Finalize(requestContext);

    public bool IsEnabled(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || !this.m_redisConfiguration.IsRedisEnabled(requestContext))
        return false;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return !string.IsNullOrWhiteSpace(vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) this.m_redisConfiguration.Keys.ConnectionString, (string) null));
    }

    public IRedisConnection CreateConnection(
      IVssRequestContext requestContext,
      string poolName = "$Default",
      Guid namespaceId = default (Guid))
    {
      IRedisConnectionPool redisConnectionPool;
      if (!this.m_connectionPools.TryGetValue(poolName, out redisConnectionPool))
        throw new RedisException("Unknown connection pool " + poolName);
      return redisConnectionPool.AcquireConnection(requestContext, namespaceId);
    }

    internal string GetCacheId<S>(IVssRequestContext requestContext, Guid namespaceId) => this.m_cacheNamespace.MakeCacheId<S>(requestContext, namespaceId);

    internal ContainerSettings ResolveSettings(
      IVssRequestContext requestContext,
      Guid namespaceId,
      ContainerSettings settings)
    {
      settings = ContainerSettings.UserOrDefault(requestContext, namespaceId, settings, this.m_redisConfiguration);
      return settings;
    }

    internal void Reset(
      IVssRequestContext requestContext,
      RedisConfiguration configuration,
      Microsoft.VisualStudio.Services.Redis.Tracer tracer)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      this.Finalize(requestContext1);
      this.m_redisConfiguration = configuration.Clone();
      this.m_tracer = tracer;
      this.Initialize(requestContext1);
    }

    internal int GetPoolSize(IVssRequestContext requestContext, string poolName)
    {
      IRedisConnectionPool redisConnectionPool;
      if (!this.m_connectionPools.TryGetValue(poolName, out redisConnectionPool))
        throw new RedisException("Unknown connection pool " + poolName);
      return redisConnectionPool.Size;
    }

    private void Initialize(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      RedisReconnectSync sync1 = new RedisReconnectSync(RedisCacheManager.s_reconnectionPeriod);
      this.m_connectionPools.Add("$Default", (IRedisConnectionPool) new ConnectionPool(requestContext, this.m_redisConfiguration, this.m_tracer, sync1, defaultSettings: new ConnectionPoolSettings()
      {
        DatabaseId = 0,
        PoolSize = 3,
        NeedsPubSub = false
      }));
      this.m_connectionPools.Add("$Probation", (IRedisConnectionPool) new ConnectionPool(requestContext, this.m_redisConfiguration, this.m_tracer, sync1, "$Probation", new ConnectionPoolSettings()
      {
        DatabaseId = 0,
        PoolSize = 3,
        NeedsPubSub = false
      }));
      if (RedisConfiguration.IsClusterEnabled(requestContext))
      {
        this.m_connectionPools.Add("$SignalR", (IRedisConnectionPool) new ConnectionPool(requestContext, this.m_redisConfiguration, this.m_tracer, sync1, "$SignalR", new ConnectionPoolSettings()
        {
          DatabaseId = 0,
          PoolSize = 1,
          NeedsPubSub = true
        }));
      }
      else
      {
        RedisReconnectSync sync2 = new RedisReconnectSync(RedisCacheManager.s_reconnectionPeriod);
        this.m_connectionPools.Add("$SignalR", (IRedisConnectionPool) new ConnectionPool(requestContext, this.m_redisConfiguration, this.m_tracer, sync2, "$SignalR", new ConnectionPoolSettings()
        {
          DatabaseId = 1,
          PoolSize = 1,
          NeedsPubSub = true
        }));
      }
      this.m_configurationChangedCallback = (RegistrySettingsChangedCallback) ((rq, ce) =>
      {
        try
        {
          IDictionary<string, IRedisConnectionPool> connectionPools = this.m_connectionPools;
          if ((connectionPools != null ? (connectionPools.Count > 0 ? 1 : 0) : 0) == 0)
            return;
          foreach (IRedisConnectionPool redisConnectionPool in this.m_connectionPools.Values.ToArray<IRedisConnectionPool>())
            redisConnectionPool.UpdateSettings(rq);
        }
        catch (Exception ex)
        {
          this.m_tracer.RedisError(requestContext, ex);
        }
      });
      this.m_secretChangedCallback = (StrongBoxItemChangedCallback) ((rq, sbin) =>
      {
        try
        {
          IDictionary<string, IRedisConnectionPool> connectionPools = this.m_connectionPools;
          if ((connectionPools != null ? (connectionPools.Count > 0 ? 1 : 0) : 0) == 0)
            return;
          foreach (IRedisConnectionPool redisConnectionPool in this.m_connectionPools.Values.ToArray<IRedisConnectionPool>())
            redisConnectionPool.UpdateSettings(rq);
        }
        catch (Exception ex)
        {
          this.m_tracer.RedisError(requestContext, ex);
        }
      });
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, this.m_configurationChangedCallback, this.m_redisConfiguration.Keys.ConfigurationRootPath + "/**");
      requestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(requestContext, this.m_secretChangedCallback, this.m_redisConfiguration.StrongBoxDrawerName, (IEnumerable<string>) new string[1]
      {
        this.m_redisConfiguration.StrongBoxCachingServiceKey
      });
    }

    private void Finalize(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, this.m_configurationChangedCallback);
      requestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(requestContext, this.m_secretChangedCallback);
      IDictionary<string, IRedisConnectionPool> connectionPools = this.m_connectionPools;
      if ((connectionPools != null ? (connectionPools.Count > 0 ? 1 : 0) : 0) == 0)
        return;
      foreach (IDisposable disposable in (IEnumerable<IRedisConnectionPool>) this.m_connectionPools.Values)
        disposable.Dispose();
      this.m_connectionPools.Clear();
    }

    internal RedisConfiguration Configuration => this.m_redisConfiguration;
  }
}
