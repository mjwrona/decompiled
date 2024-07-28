// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V1.WindowedDictionaryCacheContainer`1
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

namespace Microsoft.VisualStudio.Services.Redis.V1
{
  internal class WindowedDictionaryCacheContainer<K> : 
    BaseDictionaryCacheContainer<K, ContainerSettings>,
    IWindowedDictionaryCacheContainer<K>
  {
    internal WindowedDictionaryCacheContainer(
      IRedisConnectionFactory redisConnectionFactory,
      string cacheId,
      ContainerSettings settings)
      : base(redisConnectionFactory, cacheId, settings)
    {
      if (!settings.KeyExpiry.HasValue || settings.KeyExpiry.Value.TotalMilliseconds < 1.0)
        throw new ArgumentNullException("window duration must be set to 1 millisecond or more");
    }

    internal override string GetCacheKey(string item) => this.m_cacheId + "/" + (object) this.m_settings.KeyExpiry.Value + "/" + item;

    IEnumerable<long> IWindowedDictionaryCacheContainer<K>.Get(
      IVssRequestContext requestContext,
      IEnumerable<K> items)
    {
      if (items == null)
        throw new ArgumentNullException(nameof (items));
      IList<RedisKey> serializedItems = (IList<RedisKey>) new List<RedisKey>();
      foreach (K key in items)
        serializedItems.Add(RedisKey.op_Implicit(this.GetCacheKey(key)));
      this.TRACE_ENTER_REDIS(requestContext, "Get");
      try
      {
        requestContext.Trace(8150003, TraceLevel.Info, "Redis", "RedisCache", "IncrementBy() by {0}", (object) this.m_settings.CiAreaName);
        using (IRedisConnection connection = this.m_redisConnectionFactory.CreateConnection(requestContext))
          return connection.Call<IEnumerable<long>>(requestContext, this.m_tracer, (Func<IRedisDatabase, IEnumerable<long>>) (redisDb => redisDb.GetWindowedValue(requestContext, (IEnumerable<RedisKey>) serializedItems, this.m_settings.KeyExpiry.Value)), (Func<bool, Command<IEnumerable<long>>, IEnumerable<long>>) ((transientError, command) => this.Fallback<IEnumerable<long>>(requestContext, transientError, command, Enumerable.Repeat<long>(-1L, serializedItems.Count))));
      }
      finally
      {
        this.TRACE_EXIT_REDIS(requestContext, "Get");
      }
    }

    IEnumerable<long> IWindowedDictionaryCacheContainer<K>.IncrementBy(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<K, WindowItem>> items)
    {
      if (items == null)
        throw new ArgumentNullException(nameof (items));
      IList<IncrementWindowedValueParameter> serializedItems = (IList<IncrementWindowedValueParameter>) new List<IncrementWindowedValueParameter>();
      foreach (KeyValuePair<K, WindowItem> keyValuePair in items)
      {
        IList<IncrementWindowedValueParameter> windowedValueParameterList = serializedItems;
        RedisKey key = RedisKey.op_Implicit(this.GetCacheKey(keyValuePair.Key));
        WindowItem windowItem = keyValuePair.Value;
        long increment = windowItem.Increment;
        windowItem = keyValuePair.Value;
        long maximum = windowItem.Maximum;
        IncrementWindowedValueParameter windowedValueParameter = new IncrementWindowedValueParameter(key, increment, maximum);
        windowedValueParameterList.Add(windowedValueParameter);
      }
      this.TRACE_ENTER_REDIS(requestContext, "IncrementBy");
      try
      {
        requestContext.Trace(8150003, TraceLevel.Info, "Redis", "RedisCache", "IncrementBy() by {0}", (object) this.m_settings.CiAreaName);
        using (IRedisConnection connection = this.m_redisConnectionFactory.CreateConnection(requestContext))
          return connection.Call<IEnumerable<long>>(requestContext, this.m_tracer, (Func<IRedisDatabase, IEnumerable<long>>) (redisDb => redisDb.IncrementWindowedValue(requestContext, (IEnumerable<IncrementWindowedValueParameter>) serializedItems, this.m_settings.KeyExpiry.Value, this.m_settings.AllowBatching.GetValueOrDefault() && requestContext.IsFeatureEnabled("VisualStudio.Services.Redis.EnableBatching"))), (Func<bool, Command<IEnumerable<long>>, IEnumerable<long>>) ((transientError, command) => this.Fallback<IEnumerable<long>>(requestContext, transientError, command, Enumerable.Repeat<long>(-1L, serializedItems.Count))));
      }
      finally
      {
        this.TRACE_EXIT_REDIS(requestContext, "IncrementBy");
      }
    }

    IEnumerable<TimeSpan?> IWindowedDictionaryCacheContainer<K>.TimeToLive(
      IVssRequestContext requestContext,
      IEnumerable<K> keys)
    {
      RedisKey[] pars = keys != null ? keys.Select<K, RedisKey>((Func<K, RedisKey>) (key => RedisKey.op_Implicit(this.GetCacheKey(key)))).ToArray<RedisKey>() : throw new ArgumentNullException(nameof (keys));
      this.TRACE_ENTER_REDIS(requestContext, "TimeToLive");
      try
      {
        requestContext.Trace(8150003, TraceLevel.Info, "Redis", "RedisCache", "IncrementBy() by {0}", (object) this.m_settings.CiAreaName);
        using (IRedisConnection connection = this.m_redisConnectionFactory.CreateConnection(requestContext))
          return connection.Call<IEnumerable<TimeSpan?>>(requestContext, this.m_tracer, (Func<IRedisDatabase, IEnumerable<TimeSpan?>>) (redisDb => (IEnumerable<TimeSpan?>) redisDb.GetTimeToLiveWindowedValue(requestContext, ((IEnumerable<RedisKey>) pars).ToArray<RedisKey>())), (Func<bool, Command<IEnumerable<TimeSpan?>>, IEnumerable<TimeSpan?>>) ((transientError, command) => this.Fallback<IEnumerable<TimeSpan?>>(requestContext, transientError, command, Enumerable.Repeat<TimeSpan?>(new TimeSpan?(), pars.Length))));
      }
      finally
      {
        this.TRACE_EXIT_REDIS(requestContext, "TimeToLive");
      }
    }
  }
}
