// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CommandPropertiesDefault
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class CommandPropertiesDefault : ICommandProperties
  {
    private const bool DefaultCircuitBreakerDisabled = false;
    private const int DefaultCircuitBreakerErrorThresholdPercentage = 50;
    private const bool DefaultCircuitBreakerForceClosed = false;
    private const bool DefaultCircuitBreakerForceOpen = false;
    private const int DefaultCircuitBreakerRequestVolumeThreshold = 20;
    private static TimeSpan DefaultCircuitBreakerMinBackoff = TimeSpan.FromSeconds(0.0);
    private static TimeSpan DefaultCircuitBreakerMaxBackoff = TimeSpan.FromSeconds(30.0);
    private static TimeSpan DefaultCircuitBreakerDeltaBackoff = TimeSpan.FromMilliseconds(300.0);
    private static readonly TimeSpan DefaultExecutionTimeout = TimeSpan.FromSeconds(1.0);
    private const int DefaultExecutionMaxConcurrentRequests = 2147483647;
    private const int DefaultFallbackMaxConcurrentRequests = 2147483647;
    private const int DefaultExecutionMaxRequests = 2147483647;
    private const int DefaultFallbackMaxRequests = 2147483647;
    private const bool DefaultFallbackDisabled = false;
    private static readonly TimeSpan DefaultMetricsHealthSnapshotInterval = TimeSpan.FromSeconds(0.5);
    private const int DefaultMetricsRollingStatisticalWindowInMilliseconds = 10000;
    private const int DefaultMetricsRollingStatisticalWindowBuckets = 10;

    public bool CircuitBreakerDisabled { get; protected set; }

    public int CircuitBreakerErrorThresholdPercentage { get; protected set; }

    public bool CircuitBreakerForceClosed { get; protected set; }

    public bool CircuitBreakerForceOpen { get; protected set; }

    public int CircuitBreakerRequestVolumeThreshold { get; protected set; }

    public TimeSpan CircuitBreakerMinBackoff { get; protected set; }

    public TimeSpan CircuitBreakerMaxBackoff { get; protected set; }

    public TimeSpan CircuitBreakerDeltaBackoff { get; protected set; }

    public TimeSpan ExecutionTimeout { get; protected set; }

    public int ExecutionMaxConcurrentRequests { get; protected set; }

    public int FallbackMaxConcurrentRequests { get; protected set; }

    public int ExecutionMaxRequests { get; protected set; }

    public int FallbackMaxRequests { get; protected set; }

    public bool FallbackDisabled { get; protected set; }

    public TimeSpan MetricsHealthSnapshotInterval { get; protected set; }

    public int MetricsRollingStatisticalWindowInMilliseconds { get; protected set; }

    public int MetricsRollingStatisticalWindowBuckets { get; protected set; }

    public CommandPropertiesDefault(CommandPropertiesSetter setter = null)
    {
      setter = setter ?? new CommandPropertiesSetter();
      bool? nullable1 = setter.CircuitBreakerDisabled;
      this.CircuitBreakerDisabled = nullable1.GetValueOrDefault();
      this.CircuitBreakerErrorThresholdPercentage = setter.CircuitBreakerErrorThresholdPercentage ?? 50;
      nullable1 = setter.CircuitBreakerForceClosed;
      this.CircuitBreakerForceClosed = nullable1.GetValueOrDefault();
      nullable1 = setter.CircuitBreakerForceOpen;
      this.CircuitBreakerForceOpen = nullable1.GetValueOrDefault();
      this.CircuitBreakerRequestVolumeThreshold = setter.CircuitBreakerRequestVolumeThreshold ?? 20;
      TimeSpan? nullable2 = setter.CircuitBreakerMinBackoff;
      this.CircuitBreakerMinBackoff = nullable2 ?? CommandPropertiesDefault.DefaultCircuitBreakerMinBackoff;
      nullable2 = setter.CircuitBreakerMaxBackoff;
      this.CircuitBreakerMaxBackoff = nullable2 ?? CommandPropertiesDefault.DefaultCircuitBreakerMaxBackoff;
      nullable2 = setter.CircuitBreakerDeltaBackoff;
      this.CircuitBreakerDeltaBackoff = nullable2 ?? CommandPropertiesDefault.DefaultCircuitBreakerDeltaBackoff;
      nullable2 = setter.ExecutionTimeout;
      this.ExecutionTimeout = nullable2 ?? CommandPropertiesDefault.DefaultExecutionTimeout;
      int? nullable3 = setter.ExecutionMaxConcurrentRequests;
      this.ExecutionMaxConcurrentRequests = nullable3 ?? int.MaxValue;
      nullable3 = setter.ExecutionMaxRequests;
      this.ExecutionMaxRequests = nullable3 ?? int.MaxValue;
      nullable3 = setter.FallbackMaxConcurrentRequests;
      this.FallbackMaxConcurrentRequests = nullable3 ?? int.MaxValue;
      nullable3 = setter.FallbackMaxRequests;
      this.FallbackMaxRequests = nullable3 ?? int.MaxValue;
      nullable1 = setter.FallbackDisabled;
      this.FallbackDisabled = nullable1.GetValueOrDefault();
      nullable2 = setter.MetricsHealthSnapshotInterval;
      this.MetricsHealthSnapshotInterval = nullable2 ?? CommandPropertiesDefault.DefaultMetricsHealthSnapshotInterval;
      nullable3 = setter.MetricsRollingStatisticalWindowInMilliseconds;
      this.MetricsRollingStatisticalWindowInMilliseconds = nullable3 ?? 10000;
      nullable3 = setter.MetricsRollingStatisticalWindowBuckets;
      this.MetricsRollingStatisticalWindowBuckets = nullable3 ?? 10;
    }
  }
}
