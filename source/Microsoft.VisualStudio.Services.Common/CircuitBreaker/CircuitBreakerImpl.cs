// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CircuitBreakerImpl
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  internal class CircuitBreakerImpl : ICircuitBreaker
  {
    private readonly CommandMetrics m_metrics;
    private ITime m_time;
    private AtomicBoolean m_circuitOpen = new AtomicBoolean(false);
    private AtomicLong m_circuitOpenedOrLastTestedTime = new AtomicLong(0L);
    private AtomicLong m_circuitAccessedTime = new AtomicLong(0L);
    private AtomicLong m_attempt = new AtomicLong(0L);
    internal AtomicLong m_backoffInMilliseconds = new AtomicLong(0L);

    internal CircuitBreakerImpl(ICommandProperties properties, CommandMetrics metrics, ITime time = null)
    {
      if (properties == null)
        throw new ArgumentNullException(nameof (properties));
      if (metrics == null)
        throw new ArgumentNullException(nameof (metrics));
      this.m_time = time ?? (ITime) ITimeDefault.Instance;
      this.m_metrics = metrics;
      this.ExecutionSemaphore = (ITryableSemaphore) new TryableSemaphore();
      this.FallbackSemaphore = (ITryableSemaphore) new TryableSemaphore();
      this.ExecutionRequests = (IRollingNumber) new RollingNumber(this.m_time, properties.MetricsRollingStatisticalWindowInMilliseconds, properties.MetricsRollingStatisticalWindowBuckets);
      this.FallbackRequests = (IRollingNumber) new RollingNumber(this.m_time, properties.MetricsRollingStatisticalWindowInMilliseconds, properties.MetricsRollingStatisticalWindowBuckets);
    }

    public bool AllowRequest(ICommandProperties properties)
    {
      this.m_circuitAccessedTime.Value = this.m_time.GetCurrentTimeInMillis();
      if (properties.CircuitBreakerForceOpen)
        return false;
      if (properties.CircuitBreakerForceClosed)
      {
        this.IsOpen(properties);
        return true;
      }
      return !this.IsOpen(properties) || this.AllowSingleTest(properties);
    }

    public bool IsOpen(ICommandProperties properties)
    {
      if ((bool) this.m_circuitOpen)
        return true;
      HealthCounts healthCounts = this.m_metrics.GetHealthCounts();
      if (healthCounts.TotalRequests < (long) properties.CircuitBreakerRequestVolumeThreshold || healthCounts.ErrorPercentage < properties.CircuitBreakerErrorThresholdPercentage || !this.m_circuitOpen.CompareAndSet(false, true))
        return false;
      this.m_circuitOpenedOrLastTestedTime.Value = this.m_time.GetCurrentTimeInMillis();
      this.m_attempt.Value = 1L;
      this.m_backoffInMilliseconds.Value = Math.Max(1L, (long) BackoffTimerHelper.GetExponentialBackoff((int) (long) this.m_attempt, properties.CircuitBreakerMinBackoff, properties.CircuitBreakerMaxBackoff, properties.CircuitBreakerDeltaBackoff).TotalMilliseconds);
      return true;
    }

    public CircuitBreakerStatus GetCircuitBreakerState(ICommandProperties properties)
    {
      if (!(bool) this.m_circuitOpen && !properties.CircuitBreakerForceOpen)
        return CircuitBreakerStatus.Closed;
      return properties.CircuitBreakerForceOpen || this.m_time.GetCurrentTimeInMillis() < (long) this.m_circuitOpenedOrLastTestedTime + (long) this.m_backoffInMilliseconds ? CircuitBreakerStatus.Open : CircuitBreakerStatus.HalfOpen;
    }

    private bool AllowSingleTest(ICommandProperties properties)
    {
      long orLastTestedTime = (long) this.m_circuitOpenedOrLastTestedTime;
      if (!(bool) this.m_circuitOpen || this.m_time.GetCurrentTimeInMillis() <= orLastTestedTime + (long) this.m_backoffInMilliseconds || !this.m_circuitOpenedOrLastTestedTime.CompareAndSet(orLastTestedTime, this.m_time.GetCurrentTimeInMillis()))
        return false;
      this.m_backoffInMilliseconds.Value = Math.Max(1L, (long) BackoffTimerHelper.GetExponentialBackoff((int) this.m_attempt.IncrementAndGet(), properties.CircuitBreakerMinBackoff, properties.CircuitBreakerMaxBackoff, properties.CircuitBreakerDeltaBackoff).TotalMilliseconds);
      return true;
    }

    public ITryableSemaphore ExecutionSemaphore { get; private set; }

    public ITryableSemaphore FallbackSemaphore { get; private set; }

    public IRollingNumber ExecutionRequests { get; private set; }

    public IRollingNumber FallbackRequests { get; private set; }

    internal void Reset()
    {
      this.m_attempt.Value = 0L;
      this.m_circuitOpen.Value = false;
      this.m_metrics.ResetCounter();
    }

    public void MarkSuccess()
    {
      if (!(bool) this.m_circuitOpen)
        return;
      this.Reset();
    }

    public bool IsOlderThan(TimeSpan time) => !(bool) this.m_circuitOpen && (double) (this.m_time.GetCurrentTimeInMillis() - (long) this.m_circuitAccessedTime) > time.TotalMilliseconds;
  }
}
