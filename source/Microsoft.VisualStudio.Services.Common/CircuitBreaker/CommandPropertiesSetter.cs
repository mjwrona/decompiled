// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CommandPropertiesSetter
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class CommandPropertiesSetter
  {
    public bool? CircuitBreakerDisabled { get; private set; }

    public int? CircuitBreakerErrorThresholdPercentage { get; private set; }

    public bool? CircuitBreakerForceClosed { get; private set; }

    public bool? CircuitBreakerForceOpen { get; private set; }

    public int? CircuitBreakerRequestVolumeThreshold { get; private set; }

    public TimeSpan? CircuitBreakerMinBackoff { get; private set; }

    public TimeSpan? CircuitBreakerMaxBackoff { get; private set; }

    public TimeSpan? CircuitBreakerDeltaBackoff { get; private set; }

    public TimeSpan? ExecutionTimeout { get; private set; }

    public int? ExecutionMaxConcurrentRequests { get; private set; }

    public int? FallbackMaxConcurrentRequests { get; private set; }

    public int? ExecutionMaxRequests { get; private set; }

    public int? FallbackMaxRequests { get; private set; }

    public bool? FallbackDisabled { get; private set; }

    public TimeSpan? MetricsHealthSnapshotInterval { get; private set; }

    public int? MetricsRollingStatisticalWindowInMilliseconds { get; private set; }

    public int? MetricsRollingStatisticalWindowBuckets { get; private set; }

    public CommandPropertiesSetter()
    {
    }

    public CommandPropertiesSetter(ICommandProperties values)
    {
      this.CircuitBreakerDisabled = values?.CircuitBreakerDisabled;
      this.CircuitBreakerErrorThresholdPercentage = values?.CircuitBreakerErrorThresholdPercentage;
      this.CircuitBreakerForceClosed = values?.CircuitBreakerForceClosed;
      this.CircuitBreakerForceOpen = values?.CircuitBreakerForceOpen;
      this.CircuitBreakerRequestVolumeThreshold = values?.CircuitBreakerRequestVolumeThreshold;
      this.CircuitBreakerMinBackoff = values?.CircuitBreakerMinBackoff;
      this.CircuitBreakerMaxBackoff = values?.CircuitBreakerMaxBackoff;
      this.CircuitBreakerDeltaBackoff = values?.CircuitBreakerDeltaBackoff;
      this.ExecutionTimeout = values?.ExecutionTimeout;
      this.ExecutionMaxConcurrentRequests = values?.ExecutionMaxConcurrentRequests;
      this.ExecutionMaxRequests = values?.ExecutionMaxRequests;
      this.FallbackMaxConcurrentRequests = values?.FallbackMaxConcurrentRequests;
      this.FallbackMaxRequests = values?.FallbackMaxRequests;
      this.FallbackDisabled = values?.FallbackDisabled;
      this.MetricsHealthSnapshotInterval = values?.MetricsHealthSnapshotInterval;
      this.MetricsRollingStatisticalWindowInMilliseconds = values?.MetricsRollingStatisticalWindowInMilliseconds;
      this.MetricsRollingStatisticalWindowBuckets = values?.MetricsRollingStatisticalWindowBuckets;
    }

    public CommandPropertiesSetter WithCircuitBreakerDisabled(bool value)
    {
      this.CircuitBreakerDisabled = new bool?(value);
      return this;
    }

    public CommandPropertiesSetter WithCircuitBreakerErrorThresholdPercentage(int value)
    {
      this.CircuitBreakerErrorThresholdPercentage = new int?(value);
      return this;
    }

    public CommandPropertiesSetter WithCircuitBreakerForceClosed(bool value)
    {
      this.CircuitBreakerForceClosed = new bool?(value);
      return this;
    }

    public CommandPropertiesSetter WithCircuitBreakerForceOpen(bool value)
    {
      this.CircuitBreakerForceOpen = new bool?(value);
      return this;
    }

    public CommandPropertiesSetter WithCircuitBreakerRequestVolumeThreshold(int value)
    {
      this.CircuitBreakerRequestVolumeThreshold = new int?(value);
      return this;
    }

    public CommandPropertiesSetter WithCircuitBreakerMinBackoff(TimeSpan value)
    {
      this.CircuitBreakerMinBackoff = new TimeSpan?(value);
      return this;
    }

    public CommandPropertiesSetter WithCircuitBreakerMaxBackoff(TimeSpan value)
    {
      this.CircuitBreakerMaxBackoff = new TimeSpan?(value);
      return this;
    }

    public CommandPropertiesSetter WithCircuitBreakerDeltaBackoff(TimeSpan value)
    {
      this.CircuitBreakerDeltaBackoff = new TimeSpan?(value);
      return this;
    }

    public CommandPropertiesSetter WithExecutionTimeoutInMilliseconds(int milliseconds)
    {
      this.ExecutionTimeout = new TimeSpan?(TimeSpan.FromMilliseconds((double) milliseconds));
      return this;
    }

    public CommandPropertiesSetter WithExecutionTimeout(TimeSpan value)
    {
      this.ExecutionTimeout = new TimeSpan?(value);
      return this;
    }

    public CommandPropertiesSetter WithExecutionMaxConcurrentRequests(int value)
    {
      this.ExecutionMaxConcurrentRequests = new int?(value);
      return this;
    }

    public CommandPropertiesSetter WithFallbackMaxConcurrentRequests(int value)
    {
      this.FallbackMaxConcurrentRequests = new int?(value);
      return this;
    }

    public CommandPropertiesSetter WithExecutionMaxRequests(int value)
    {
      this.ExecutionMaxRequests = new int?(value);
      return this;
    }

    public CommandPropertiesSetter WithFallbackMaxRequests(int value)
    {
      this.FallbackMaxRequests = new int?(value);
      return this;
    }

    public CommandPropertiesSetter WithFallbackDisabled(bool value)
    {
      this.FallbackDisabled = new bool?(value);
      return this;
    }

    public CommandPropertiesSetter WithMetricsHealthSnapshotInterval(TimeSpan value)
    {
      this.MetricsHealthSnapshotInterval = new TimeSpan?(value);
      return this;
    }

    public CommandPropertiesSetter WithMetricsRollingStatisticalWindowInMilliseconds(int value)
    {
      this.MetricsRollingStatisticalWindowInMilliseconds = new int?(value);
      return this;
    }

    public CommandPropertiesSetter WithMetricsRollingStatisticalWindow(TimeSpan value)
    {
      this.MetricsRollingStatisticalWindowInMilliseconds = new int?((int) value.TotalMilliseconds);
      return this;
    }

    public CommandPropertiesSetter WithMetricsRollingStatisticalWindowBuckets(int value)
    {
      this.MetricsRollingStatisticalWindowBuckets = new int?(value);
      return this;
    }
  }
}
