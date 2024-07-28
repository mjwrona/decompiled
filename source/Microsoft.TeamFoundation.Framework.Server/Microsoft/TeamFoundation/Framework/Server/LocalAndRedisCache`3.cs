// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocalAndRedisCache`3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class LocalAndRedisCache<K, V, S> : IVssFrameworkService where K : IEquatable<K>
  {
    internal Guid m_redisNamespace;
    internal TimeSpan m_expiryInterval = TimeSpan.FromHours(1.0);
    internal int m_memoryCacheMaxLength;
    internal IMutableDictionaryCacheContainer<K, V> m_redis;
    internal volatile bool m_redis_initialized;
    private LocalAndRedisCacheConfiguration m_initialConfiguration;
    private const string c_caching = "Caching";
    private readonly string m_className;

    public LocalAndRedisCache(LocalAndRedisCacheConfiguration configuration)
    {
      this.m_redis_initialized = false;
      this.m_initialConfiguration = configuration;
      if (!configuration.RedisNamespace.HasValue)
        throw new ArgumentNullException("LocalAndRedisCacheConfiguration must have a RedisNamespace");
      this.m_redisNamespace = configuration.RedisNamespace.Value;
      this.m_className = this.GetType().Name;
    }

    public string RegistryPath() => "/Configuration/Caching/LocalAndRedisCache/" + this.m_className;

    public virtual void ServiceStart(IVssRequestContext requestContext)
    {
      string query = this.RegistryPath() + "/**";
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      service.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.UpdateSettings), true, query);
      RegistryEntryCollection entries = service.ReadEntriesFallThru(requestContext, (RegistryQuery) query);
      this.UpdateSettings(requestContext, entries);
    }

    public virtual void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.UpdateSettings));

    public bool TryGetValue(IVssRequestContext requestContext, K key, out V value)
    {
      LocalAndRedisMemoryCacheService<K, V> service = requestContext.GetService<LocalAndRedisMemoryCacheService<K, V>>();
      if (service.TryGetValue(requestContext, key, out value))
        return true;
      if (!this.IsRedisEnabled(requestContext) || !this.GetRedis(requestContext).TryGet<K, V>(requestContext, key, out value))
        return false;
      service.Set(requestContext, key, value);
      return true;
    }

    public void Set(IVssRequestContext requestContext, K key, V value)
    {
      requestContext.GetService<LocalAndRedisMemoryCacheService<K, V>>().Set(requestContext, key, value);
      if (!this.IsRedisEnabled(requestContext))
        return;
      this.GetRedis(requestContext).Set(requestContext, (IDictionary<K, V>) new Dictionary<K, V>()
      {
        {
          key,
          value
        }
      });
    }

    public void Remove(IVssRequestContext requestContext, K key)
    {
      requestContext.GetService<LocalAndRedisMemoryCacheService<K, V>>().Remove(requestContext, key);
      if (!this.IsRedisEnabled(requestContext))
        return;
      this.GetRedis(requestContext).Invalidate<K, V>(requestContext, key);
    }

    public void SetExpiryInterval(IVssRequestContext requestContext, TimeSpan value)
    {
      this.m_expiryInterval = value;
      requestContext.GetService<LocalAndRedisMemoryCacheService<K, V>>().SetExpiryInterval(value);
      this.SetReinitializeRedis();
    }

    private void UpdateSettings(IVssRequestContext requestContext, RegistryEntryCollection entries)
    {
      this.m_expiryInterval = entries.GetValueFromPath<TimeSpan>(this.RegistryPath() + "/ExpiryInterval", this.m_initialConfiguration.ExpiryInterval ?? LocalAndRedisCacheConfiguration.Default.ExpiryInterval.Value);
      this.m_memoryCacheMaxLength = entries.GetValueFromPath<int>(this.RegistryPath() + "/MemoryCacheMaxLength", this.m_initialConfiguration.MemoryCacheMaxLength ?? LocalAndRedisCacheConfiguration.Default.MemoryCacheMaxLength.Value);
      LocalAndRedisMemoryCacheService<K, V> service = requestContext.GetService<LocalAndRedisMemoryCacheService<K, V>>();
      service.SetMaxCacheLength(this.m_memoryCacheMaxLength);
      service.SetExpiryInterval(this.m_expiryInterval);
      this.SetReinitializeRedis();
    }

    protected void SetReinitializeRedis() => this.m_redis_initialized = false;

    protected virtual bool IsRedisEnabled(IVssRequestContext requestContext) => true;

    private IMutableDictionaryCacheContainer<K, V> GetRedis(IVssRequestContext requestContext)
    {
      if (this.m_redis_initialized)
        return this.m_redis;
      this.m_redis = requestContext.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<K, V, S>(requestContext, this.m_redisNamespace, new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(this.m_expiryInterval),
        CiAreaName = this.m_className,
        NoThrowMode = new bool?(true)
      });
      this.m_redis_initialized = true;
      return this.m_redis;
    }
  }
}
