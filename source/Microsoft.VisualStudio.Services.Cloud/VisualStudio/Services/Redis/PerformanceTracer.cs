// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.PerformanceTracer
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Redis
{
  public class PerformanceTracer : ErrorTracer
  {
    private static readonly Tracer s_globalTracer = (Tracer) new PerformanceTracer("<GLOBAL>");
    private const long c_threshold = 100;
    private const long c_sizeThreshold = 5000;

    public PerformanceTracer(string cacheArea)
      : base(cacheArea)
    {
    }

    public new static Tracer Global => PerformanceTracer.s_globalTracer;

    public override void RedisCacheHit(IVssRequestContext requestContext, string cacheKey)
    {
      this.TraceEvent(requestContext, "Redis cache hit: area={0}, key={1}", (object) this.m_cacheArea, (object) cacheKey);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalCacheHits", this.m_cacheArea).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_CacheHitsPerSec", this.m_cacheArea).Increment();
    }

    public override void RedisCacheMiss(IVssRequestContext requestContext, string cacheKey)
    {
      this.TraceEvent(requestContext, "Redis cache miss: area={0}, key={1}", (object) this.m_cacheArea, (object) cacheKey);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalCacheMisses", this.m_cacheArea).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_CacheMissesPerSec", this.m_cacheArea).Increment();
    }

    public override void CacheInvalidated(IVssRequestContext requestContext, string cacheKey)
    {
      this.TraceEvent(requestContext, "Cache invalidated: area={0}, key={1}", (object) this.m_cacheArea, (object) cacheKey);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalKeyInvalidations", this.m_cacheArea).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_KeyInvalidationsPerSec", this.m_cacheArea).Increment();
    }

    public override void CacheSet(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<string, byte[]>> cacheItems)
    {
      long num = 0;
      foreach (KeyValuePair<string, byte[]> cacheItem in cacheItems)
      {
        byte[] numArray = cacheItem.Value;
        int length = numArray != null ? numArray.Length : 0;
        this.TraceEvent(requestContext, "Cache set: area={0}, key={1}, valueSize={2}", (object) this.m_cacheArea, (object) cacheItem.Key, (object) length);
        if (length > 5000)
          requestContext.Trace(8140012, TraceLevel.Verbose, "Redis", "RedisMonitor", "Cache set: area={0}, key={1}, valueSize={2}", (object) this.m_cacheArea, (object) cacheItem.Key, (object) length);
        num += (long) length;
      }
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_AverageValueSize", this.m_cacheArea);
      performanceCounter.IncrementBy(num);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_AverageValueSizeBase", this.m_cacheArea);
      performanceCounter.Increment();
    }

    public override void CacheExpired(
      IVssRequestContext requestContext,
      long items,
      TimeSpan interval)
    {
      this.TraceEvent(requestContext, "Cache expired: area={0}, items={1}, period={2}", (object) this.m_cacheArea, (object) items, (object) interval);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalKeyExpirations", this.m_cacheArea).IncrementBy(items);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_KeyExpirationsPerSec", this.m_cacheArea).SetValue((long) (int) ((double) items / interval.TotalSeconds));
    }

    public override void RedisCacheEvicted(
      IVssRequestContext requestContext,
      long items,
      TimeSpan interval)
    {
      this.TraceEvent(requestContext, "Redis cache evicted: area={0}, items={1}, period={2}", (object) this.m_cacheArea, (object) items, (object) interval);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalKeyEvictions", this.m_cacheArea).IncrementBy(items);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_KeyEvictionsPerSec", this.m_cacheArea).SetValue((long) (int) ((double) items / interval.TotalSeconds));
    }

    public override void CpuUsage(IVssRequestContext requestContext, int seconds)
    {
      this.TraceEvent(requestContext, "Cache cpu usage: area={0}, cycles={1}", (object) this.m_cacheArea, (object) seconds);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_CpuUsage", this.m_cacheArea).SetValue((long) seconds);
    }

    public override void MemoryUsage(IVssRequestContext requestContext, long bytes)
    {
      this.TraceEvent(requestContext, "Cache memory usage: area={0}, cycles={1}", (object) this.m_cacheArea, (object) bytes);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_MemoryUsage", this.m_cacheArea).SetValue(bytes);
    }

    public override void TransactionFailed(IVssRequestContext requestContext)
    {
      this.TraceEvent(requestContext, "Cache transaction failed: area={0}", (object) this.m_cacheArea);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalFailedTransactions", this.m_cacheArea).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_FailedTransactionsPerSec", this.m_cacheArea).Increment();
    }

    public override void EnterRedis(IVssRequestContext requestContext)
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalCalls", this.m_cacheArea).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_CallsPerSec", this.m_cacheArea).Increment();
    }

    public override void ExitRedis(IVssRequestContext requestContext, long duration)
    {
      this.TraceEvent(requestContext, "Cache operation timing: area={0}, elapsed={1}", (object) this.m_cacheArea, (object) duration);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_AverageCallTime", this.m_cacheArea).IncrementTicks(TimeSpan.FromTicks(duration));
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_AverageCallTimeBase", this.m_cacheArea).Increment();
      long num = duration / 10000L;
      if (num <= 100L)
        return;
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalSlowCalls", this.m_cacheArea).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_SlowCallsPerSec", this.m_cacheArea).Increment();
      requestContext.Trace(8110002, TraceLevel.Warning, "Redis", "RedisCache", "Redis call took too much time: {0} ms, area: {1}", (object) num, (object) this.m_cacheArea);
    }

    public override IDisposable TimedCall(IVssRequestContext requestContext) => (IDisposable) new Microsoft.VisualStudio.Services.Redis.TimedCall(requestContext, (Tracer) this);

    private void TraceEvent(
      IVssRequestContext requestContext,
      string message,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(requestContext, 8130001, TraceLevel.Verbose, "Redis", "RedisMonitor", message, args);
    }

    protected override void EnterMethod(
      IVssRequestContext requestContext,
      string className,
      string methodName,
      object arg)
    {
      requestContext.TraceEnter(8010001, "Redis", className, methodName);
    }

    protected override void LeaveMethod(
      IVssRequestContext requestContext,
      string className,
      string methodName)
    {
      requestContext.TraceLeave(8010002, "Redis", className, methodName);
    }

    protected override void MethodException(
      IVssRequestContext requestContext,
      string className,
      string methodName,
      Exception ex)
    {
      requestContext.TraceException(8010003, TraceLevel.Error, "Redis", className, ex);
    }
  }
}
