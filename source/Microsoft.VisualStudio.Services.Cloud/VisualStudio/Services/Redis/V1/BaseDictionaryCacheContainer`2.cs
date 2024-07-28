// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V1.BaseDictionaryCacheContainer`2
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Redis.V1
{
  internal abstract class BaseDictionaryCacheContainer<K, TSettings> where TSettings : ContainerSettings
  {
    protected readonly IRedisConnectionFactory m_redisConnectionFactory;
    protected readonly TSettings m_settings;
    protected readonly string m_cacheId;
    protected readonly Microsoft.VisualStudio.Services.Redis.Tracer m_tracer;
    internal const string c_enableWriteBatchingFeature = "VisualStudio.Services.Redis.EnableBatching";
    internal const string c_enableReadBatchingFeature = "VisualStudio.Services.Redis.EnableBatchingOnRead";

    internal BaseDictionaryCacheContainer(
      IRedisConnectionFactory redisConnectionFactory,
      string cacheId,
      TSettings settings)
    {
      this.m_redisConnectionFactory = redisConnectionFactory;
      this.m_settings = settings;
      this.m_cacheId = cacheId;
      this.m_tracer = (Microsoft.VisualStudio.Services.Redis.Tracer) new Microsoft.VisualStudio.Services.Redis.PerformanceTracer(this.m_settings.CiAreaName);
    }

    internal virtual string GetCacheKey(string item) => this.m_cacheId + "/" + item;

    protected string GetCacheKey(K key) => this.GetCacheKey(this.m_settings.KeySerializer.Serialize<K>(key));

    protected T Fallback<T>(
      IVssRequestContext requestContext,
      bool transientError,
      Command<T> command,
      T fallbackValue)
    {
      bool valueOrDefault = this.m_settings.NoThrowMode.GetValueOrDefault();
      if (transientError && !valueOrDefault)
      {
        RedisException redisException = new RedisException((CommandAsync) command);
        redisException.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) null);
        throw redisException;
      }
      return fallbackValue;
    }

    protected void TRACE_ENTER_REDIS(IVssRequestContext requestContext, [CallerMemberName] string methodName = null) => VssPerformanceEventSource.Log.RedisStart(requestContext.UniqueIdentifier, methodName, this.m_settings.CiAreaName, this.m_cacheId);

    protected void TRACE_EXIT_REDIS(IVssRequestContext requestContext, [CallerMemberName] string methodName = null) => VssPerformanceEventSource.Log.RedisStop(requestContext.UniqueIdentifier, methodName, this.m_settings.CiAreaName, this.m_cacheId, -1L);

    public string GetCacheId() => this.m_cacheId;
  }
}
