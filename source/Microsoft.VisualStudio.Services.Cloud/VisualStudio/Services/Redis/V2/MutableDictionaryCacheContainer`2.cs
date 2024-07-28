// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V2.MutableDictionaryCacheContainer`2
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Redis.V2
{
  internal class MutableDictionaryCacheContainer<K, V> : 
    BaseDictionaryCacheContainer<K, ContainerSettings>,
    IMutableDictionaryCacheContainer<K, V>,
    IDictionaryCacheContainer<K, V>,
    ICacheContainer<K, V>,
    IMutableCacheContainer<K, V>
  {
    internal MutableDictionaryCacheContainer(
      IRedisConnectionFactory redisConnectionFactory,
      string cacheId,
      ContainerSettings settings)
      : base(redisConnectionFactory, cacheId, settings)
    {
    }

    IEnumerable<V> IDictionaryCacheContainer<K, V>.Get(
      IVssRequestContext requestContext,
      IEnumerable<K> keys,
      Func<IEnumerable<K>, IDictionary<K, V>> onCacheMiss)
    {
      if (onCacheMiss == null)
        throw new ArgumentNullException(nameof (onCacheMiss));
      IList<KeyValuePair<V, bool>> values;
      bool flag = this.TryGet(requestContext, keys, out values);
      IEnumerable<K> source = keys.Zip<K, KeyValuePair<V, bool>, KeyValuePair<K, bool>>((IEnumerable<KeyValuePair<V, bool>>) values, (Func<K, KeyValuePair<V, bool>, KeyValuePair<K, bool>>) ((k, v) => new KeyValuePair<K, bool>(k, v.Value))).Where<KeyValuePair<K, bool>>((Func<KeyValuePair<K, bool>, bool>) (x => !x.Value)).Select<KeyValuePair<K, bool>, K>((Func<KeyValuePair<K, bool>, K>) (x => x.Key));
      if (!source.Any<K>())
        return values.Select<KeyValuePair<V, bool>, V>((Func<KeyValuePair<V, bool>, V>) (x => x.Key));
      IDictionary<K, V> valuesFromSource = onCacheMiss(source);
      if (flag)
      {
        try
        {
          ((IMutableCacheContainer<K, V>) this).Set(requestContext, valuesFromSource);
        }
        catch (RedisException ex)
        {
        }
        catch (CircuitBreakerException ex)
        {
        }
      }
      return keys.Zip<K, KeyValuePair<V, bool>, V>((IEnumerable<KeyValuePair<V, bool>>) values, (Func<K, KeyValuePair<V, bool>, V>) ((k, kvp) =>
      {
        if (kvp.Value)
          return kvp.Key;
        V v;
        return valuesFromSource.TryGetValue(k, out v) ? v : default (V);
      }));
    }

    IEnumerable<KeyValuePair<V, bool>> IDictionaryCacheContainer<K, V>.TryGet(
      IVssRequestContext requestContext,
      IEnumerable<K> keys)
    {
      IList<KeyValuePair<V, bool>> values;
      this.TryGet(requestContext, keys, out values);
      return (IEnumerable<KeyValuePair<V, bool>>) values;
    }

    private bool TryGet(
      IVssRequestContext requestContext,
      IEnumerable<K> keys,
      out IList<KeyValuePair<V, bool>> values)
    {
      RedisKey[] pars = keys.Select<K, RedisKey>((Func<K, RedisKey>) (key => RedisKey.op_Implicit(this.GetCacheKey(key)))).ToArray<RedisKey>();
      bool redisCallSucceeded = true;
      this.TRACE_ENTER_REDIS(requestContext, nameof (TryGet));
      RedisValue[] result;
      try
      {
        requestContext.Trace(8140001, TraceLevel.Info, "Redis", "RedisCache", "BeginTryGet() by {0}, count={1}", (object) this.m_settings.CiAreaName, (object) pars.Length);
        using (IRedisConnection connection = this.m_redisConnectionFactory.CreateConnection(requestContext))
          result = connection.Call<RedisValue[]>(requestContext, this.m_tracer, (Func<IRedisDatabase, RedisValue[]>) (redisDb => redisDb.GetVolatileValue(requestContext, pars, this.m_settings.AllowBatching.GetValueOrDefault() && requestContext.IsFeatureEnabled("VisualStudio.Services.Redis.EnableBatchingOnRead"))), (Func<bool, Command<RedisValue[]>, RedisValue[]>) ((transientError, command) =>
          {
            redisCallSucceeded = false;
            return new RedisValue[pars.Length];
          }));
        requestContext.TraceConditionally(8140001, TraceLevel.Info, "Redis", "RedisCache", (Func<string>) (() => string.Format("EndTryGet() by {0}, redisCallSucceeded={1}, totalSize={2}", (object) this.m_settings.CiAreaName, (object) redisCallSucceeded, (object) ((IEnumerable<RedisValue>) result).Sum<RedisValue>((Func<RedisValue, int>) (x => x.GetSize())))));
      }
      finally
      {
        this.TRACE_EXIT_REDIS(requestContext, nameof (TryGet));
      }
      values = (IList<KeyValuePair<V, bool>>) new KeyValuePair<V, bool>[result.Length];
      for (int index = 0; index < result.Length; ++index)
      {
        if (!((RedisValue) ref result[index]).IsNull)
        {
          this.m_tracer.RedisCacheHit(requestContext, RedisKey.op_Implicit(pars[index]));
          values[index] = new KeyValuePair<V, bool>(this.Deserialize(RedisValue.op_Implicit(result[index])), true);
        }
        else
          this.m_tracer.RedisCacheMiss(requestContext, RedisKey.op_Implicit(pars[index]));
      }
      return redisCallSucceeded;
    }

    void IDictionaryCacheContainer<K, V>.Invalidate(
      IVssRequestContext requestContext,
      IEnumerable<K> keys)
    {
      RedisKey[] pars = keys.Select<K, RedisKey>((Func<K, RedisKey>) (key => RedisKey.op_Implicit(this.GetCacheKey(key)))).ToArray<RedisKey>();
      this.TRACE_ENTER_REDIS(requestContext, "Invalidate");
      long num;
      try
      {
        requestContext.Trace(8140003, TraceLevel.Info, "Redis", "RedisCache", "Invalidate() by {0}", (object) this.m_settings.CiAreaName);
        using (IRedisConnection connection = this.m_redisConnectionFactory.CreateConnection(requestContext))
          num = connection.Call<long>(requestContext, this.m_tracer, (Func<IRedisDatabase, long>) (redisDb => redisDb.DeleteVolatileValue(requestContext, pars, this.m_settings.AllowBatching.GetValueOrDefault() && requestContext.IsFeatureEnabled("VisualStudio.Services.Redis.EnableBatching"))), (Func<bool, Command<long>, long>) ((transientError, command) => this.Fallback<long>(requestContext, transientError, command, -1L)));
      }
      finally
      {
        this.TRACE_EXIT_REDIS(requestContext, "Invalidate");
      }
      if (num < 0L)
        return;
      Array.ForEach<RedisKey>(pars, (Action<RedisKey>) (x => this.m_tracer.CacheInvalidated(requestContext, RedisKey.op_Implicit(x))));
    }

    IEnumerable<TimeSpan?> IDictionaryCacheContainer<K, V>.TimeToLive(
      IVssRequestContext requestContext,
      IEnumerable<K> keys)
    {
      RedisKey[] pars = keys.Select<K, RedisKey>((Func<K, RedisKey>) (key => RedisKey.op_Implicit(this.GetCacheKey(key)))).ToArray<RedisKey>();
      this.TRACE_ENTER_REDIS(requestContext, "TimeToLive");
      try
      {
        requestContext.Trace(8140004, TraceLevel.Info, "Redis", "RedisCache", "TimeToLive() by {0}, count={1}", (object) this.m_settings.CiAreaName, (object) pars.Length);
        using (IRedisConnection connection = this.m_redisConnectionFactory.CreateConnection(requestContext))
          return (IEnumerable<TimeSpan?>) connection.Call<TimeSpan?[]>(requestContext, this.m_tracer, (Func<IRedisDatabase, TimeSpan?[]>) (redisDb => redisDb.GetTimeToLiveVolatileValue(requestContext, pars)), (Func<bool, Command<TimeSpan?[]>, TimeSpan?[]>) ((transientError, command) => new TimeSpan?[pars.Length]));
      }
      finally
      {
        this.TRACE_EXIT_REDIS(requestContext, "TimeToLive");
      }
    }

    IMutableCacheContainer<K, V> ICacheContainer<K, V>.AsMutable() => (IMutableCacheContainer<K, V>) this;

    bool IMutableCacheContainer<K, V>.Set(
      IVssRequestContext requestContext,
      IDictionary<K, V> items)
    {
      KeyValuePair<string, byte[]>[] serializedItems = items.Select<KeyValuePair<K, V>, KeyValuePair<string, byte[]>>((Func<KeyValuePair<K, V>, KeyValuePair<string, byte[]>>) (item => new KeyValuePair<string, byte[]>(this.GetCacheKey(item.Key), this.Serialize(item.Value)))).ToArray<KeyValuePair<string, byte[]>>();
      this.TRACE_ENTER_REDIS(requestContext, "Set");
      try
      {
        requestContext.TraceConditionally(8140002, TraceLevel.Info, "Redis", "RedisCache", (Func<string>) (() => string.Format("Set() by {0}, count={1}, totalSize={2}", (object) this.m_settings.CiAreaName, (object) serializedItems.Length, (object) ((IEnumerable<KeyValuePair<string, byte[]>>) serializedItems).Sum<KeyValuePair<string, byte[]>>((Func<KeyValuePair<string, byte[]>, int>) (x =>
        {
          byte[] numArray = x.Value;
          return numArray == null ? 0 : numArray.Length;
        })))));
        using (IRedisConnection connection = this.m_redisConnectionFactory.CreateConnection(requestContext))
          return connection.Call<bool>(requestContext, this.m_tracer, (Func<IRedisDatabase, bool>) (redisDb => redisDb.SetVolatileValue(requestContext, ((IEnumerable<KeyValuePair<string, byte[]>>) serializedItems).Select<KeyValuePair<string, byte[]>, SetVolatileValueParameter>((Func<KeyValuePair<string, byte[]>, SetVolatileValueParameter>) (item => new SetVolatileValueParameter(RedisKey.op_Implicit(item.Key), RedisValue.op_Implicit(item.Value), this.m_settings.KeyExpiry))), this.m_settings.AllowBatching.GetValueOrDefault() && requestContext.IsFeatureEnabled("VisualStudio.Services.Redis.EnableBatching"))), (Func<bool, Command<bool>, bool>) ((transientError, command) => this.Fallback<bool>(requestContext, transientError, command, false)));
      }
      finally
      {
        this.TRACE_EXIT_REDIS(requestContext, "Set");
        this.m_tracer.CacheSet(requestContext, (IEnumerable<KeyValuePair<string, byte[]>>) serializedItems);
      }
    }

    IEnumerable<V> IMutableCacheContainer<K, V>.IncrementBy(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<K, V>> items)
    {
      if (items == null)
        throw new ArgumentNullException(nameof (items));
      IEnumerable<IncrementVolatileValueParameter> serializedItems = items.Select<KeyValuePair<K, V>, IncrementVolatileValueParameter>((Func<KeyValuePair<K, V>, IncrementVolatileValueParameter>) (item => new IncrementVolatileValueParameter(RedisKey.op_Implicit(this.GetCacheKey(item.Key)), Convert.ToInt64((object) item.Value), this.m_settings.KeyExpiry)));
      this.TRACE_ENTER_REDIS(requestContext, "IncrementBy");
      IEnumerable<long> source;
      try
      {
        requestContext.Trace(8140005, TraceLevel.Info, "Redis", "RedisCache", "IncrementBy() by {0}", (object) this.m_settings.CiAreaName);
        using (IRedisConnection connection = this.m_redisConnectionFactory.CreateConnection(requestContext))
          source = connection.Call<IEnumerable<long>>(requestContext, this.m_tracer, (Func<IRedisDatabase, IEnumerable<long>>) (redisDb => redisDb.IncrementVolatileValue(requestContext, serializedItems, this.m_settings.AllowBatching.GetValueOrDefault() && requestContext.IsFeatureEnabled("VisualStudio.Services.Redis.EnableBatching"))), (Func<bool, Command<IEnumerable<long>>, IEnumerable<long>>) ((transientError, command) => this.Fallback<IEnumerable<long>>(requestContext, transientError, command, serializedItems.Select<IncrementVolatileValueParameter, long>((Func<IncrementVolatileValueParameter, long>) (x => x.Value)))));
      }
      finally
      {
        this.TRACE_EXIT_REDIS(requestContext, "IncrementBy");
      }
      return source.Select<long, V>((Func<long, V>) (x => (V) Convert.ChangeType((object) x, typeof (V))));
    }

    private byte[] Serialize(V value) => this.m_settings.ValueSerializer.Serialize<V>(value);

    private V Deserialize(byte[] data) => this.m_settings.ValueSerializer.Deserialize<V>(data);
  }
}
