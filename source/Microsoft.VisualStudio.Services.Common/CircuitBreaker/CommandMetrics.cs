// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CommandMetrics
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class CommandMetrics
  {
    private static readonly ConcurrentDictionary<string, CommandMetrics> metrics = new ConcurrentDictionary<string, CommandMetrics>();
    private readonly CommandKey key;
    private readonly CommandGroupKey group;
    private readonly ICommandProperties properties;
    private readonly IEventNotifier eventNotifier;
    private readonly ITime time;
    internal readonly RollingNumber success;
    internal readonly RollingNumber failure;
    internal readonly RollingNumber timeout;
    internal readonly RollingNumber shortCircuited;
    internal readonly RollingNumber concurrencyRejected;
    internal readonly RollingNumber limitRejected;
    internal readonly RollingNumber fallbackSuccess;
    internal readonly RollingNumber fallbackFailure;
    internal readonly RollingNumber fallbackConcurrencyRejected;
    internal readonly RollingNumber fallbackLimitRejected;
    internal long maxConcurrency;
    private volatile HealthCounts healthCountsSnapshot = new HealthCounts(0L, 0L, 0L);
    private long lastHealthCountsSnapshot;

    public static CommandMetrics GetInstance(
      CommandKey key,
      CommandGroupKey commandGroup,
      ICommandProperties properties,
      IEventNotifier eventNotifier,
      ITime time = null)
    {
      return CommandMetrics.metrics.GetOrAdd(key.Name, (Func<string, CommandMetrics>) (w => new CommandMetrics(key, commandGroup, properties, eventNotifier, time)));
    }

    public static CommandMetrics GetInstance(CommandKey key) => CommandMetrics.metrics[key.Name];

    public static IEnumerable<CommandMetrics> Instances => (IEnumerable<CommandMetrics>) CommandMetrics.metrics.Values;

    internal static void Reset() => CommandMetrics.metrics.Clear();

    public CommandKey CommandKey => this.key;

    public CommandGroupKey CommandGroup => this.group;

    public ICommandProperties Properties => this.properties;

    internal CommandMetrics(
      CommandKey key,
      CommandGroupKey commandGroup,
      ICommandProperties properties,
      IEventNotifier eventNotifier,
      ITime time = null)
    {
      this.key = key;
      this.group = commandGroup;
      this.properties = properties;
      this.eventNotifier = eventNotifier;
      this.time = time ?? (ITime) ITimeDefault.Instance;
      this.success = new RollingNumber(this.time, properties.MetricsRollingStatisticalWindowInMilliseconds, properties.MetricsRollingStatisticalWindowBuckets);
      this.failure = new RollingNumber(this.time, properties.MetricsRollingStatisticalWindowInMilliseconds, properties.MetricsRollingStatisticalWindowBuckets);
      this.timeout = new RollingNumber(this.time, properties.MetricsRollingStatisticalWindowInMilliseconds, properties.MetricsRollingStatisticalWindowBuckets);
      this.shortCircuited = new RollingNumber(this.time, properties.MetricsRollingStatisticalWindowInMilliseconds, properties.MetricsRollingStatisticalWindowBuckets);
      this.concurrencyRejected = new RollingNumber(this.time, properties.MetricsRollingStatisticalWindowInMilliseconds, properties.MetricsRollingStatisticalWindowBuckets);
      this.limitRejected = new RollingNumber(this.time, properties.MetricsRollingStatisticalWindowInMilliseconds, properties.MetricsRollingStatisticalWindowBuckets);
      this.fallbackSuccess = new RollingNumber(this.time, properties.MetricsRollingStatisticalWindowInMilliseconds, properties.MetricsRollingStatisticalWindowBuckets);
      this.fallbackFailure = new RollingNumber(this.time, properties.MetricsRollingStatisticalWindowInMilliseconds, properties.MetricsRollingStatisticalWindowBuckets);
      this.fallbackConcurrencyRejected = new RollingNumber(this.time, properties.MetricsRollingStatisticalWindowInMilliseconds, properties.MetricsRollingStatisticalWindowBuckets);
      this.fallbackLimitRejected = new RollingNumber(this.time, properties.MetricsRollingStatisticalWindowInMilliseconds, properties.MetricsRollingStatisticalWindowBuckets);
      this.maxConcurrency = 0L;
    }

    internal void ResetCounter()
    {
      this.success.Reset();
      this.failure.Reset();
      this.timeout.Reset();
      this.shortCircuited.Reset();
      this.concurrencyRejected.Reset();
      this.limitRejected.Reset();
      this.fallbackSuccess.Reset();
      this.fallbackFailure.Reset();
      this.fallbackConcurrencyRejected.Reset();
      this.fallbackLimitRejected.Reset();
      this.healthCountsSnapshot = new HealthCounts(0L, 0L, 0L);
      this.lastHealthCountsSnapshot = this.time.GetCurrentTimeInMillis();
    }

    internal void MarkSuccess()
    {
      this.LastException = (Exception) null;
      this.eventNotifier.MarkEvent(this.group, this.key, EventType.Success);
      this.success.Increment();
    }

    internal void MarkFailure()
    {
      this.eventNotifier.MarkEvent(this.group, this.key, EventType.Failure);
      this.failure.Increment();
    }

    internal void MarkTimeout()
    {
      this.LastException = (Exception) null;
      this.eventNotifier.MarkEvent(this.group, this.key, EventType.Timeout);
      this.timeout.Increment();
    }

    internal void MarkShortCircuited()
    {
      this.eventNotifier.MarkEvent(this.group, this.key, EventType.ShortCircuited);
      this.shortCircuited.Increment();
    }

    internal void MarkConcurrencyRejected()
    {
      this.eventNotifier.MarkEvent(this.group, this.key, EventType.ConcurrencyRejected);
      this.concurrencyRejected.Increment();
    }

    internal void MarkLimitRejected()
    {
      this.eventNotifier.MarkEvent(this.group, this.key, EventType.LimitRejected);
      this.limitRejected.Increment();
    }

    internal void MarkFallbackSuccess()
    {
      this.eventNotifier.MarkEvent(this.group, this.key, EventType.FallbackSuccess);
      this.fallbackSuccess.Increment();
    }

    internal void MarkFallbackFailure()
    {
      this.eventNotifier.MarkEvent(this.group, this.key, EventType.FallbackFailure);
      this.fallbackFailure.Increment();
    }

    internal void MarkFallbackConcurrencyRejected()
    {
      this.eventNotifier.MarkEvent(this.group, this.key, EventType.FallbackConcurrencyRejected);
      this.fallbackConcurrencyRejected.Increment();
    }

    internal void MarkFallbackLimitRejected()
    {
      this.eventNotifier.MarkEvent(this.group, this.key, EventType.FallbackLimitRejected);
      this.fallbackLimitRejected.Increment();
    }

    public HealthCounts GetHealthCounts()
    {
      long healthCountsSnapshot = this.lastHealthCountsSnapshot;
      long currentTimeInMillis = this.time.GetCurrentTimeInMillis();
      if ((double) (currentTimeInMillis - healthCountsSnapshot) >= this.properties.MetricsHealthSnapshotInterval.TotalMilliseconds && Interlocked.CompareExchange(ref this.lastHealthCountsSnapshot, currentTimeInMillis, healthCountsSnapshot) == healthCountsSnapshot)
      {
        long rollingSum1 = this.success.GetRollingSum();
        long rollingSum2 = this.failure.GetRollingSum();
        long rollingSum3 = this.timeout.GetRollingSum();
        long rollingSum4 = this.shortCircuited.GetRollingSum();
        long rollingSum5 = this.concurrencyRejected.GetRollingSum();
        long rollingSum6 = this.limitRejected.GetRollingSum();
        long num = rollingSum2;
        this.healthCountsSnapshot = new HealthCounts(rollingSum1 + num + rollingSum3 + rollingSum4 + rollingSum5 + rollingSum6, rollingSum2 + rollingSum3 + rollingSum4, rollingSum5);
      }
      return this.healthCountsSnapshot;
    }

    public Exception LastException { get; set; }
  }
}
