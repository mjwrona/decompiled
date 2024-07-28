// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.ICommandProperties
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public interface ICommandProperties
  {
    bool CircuitBreakerDisabled { get; }

    int CircuitBreakerErrorThresholdPercentage { get; }

    bool CircuitBreakerForceClosed { get; }

    bool CircuitBreakerForceOpen { get; }

    int CircuitBreakerRequestVolumeThreshold { get; }

    TimeSpan CircuitBreakerMinBackoff { get; }

    TimeSpan CircuitBreakerMaxBackoff { get; }

    TimeSpan CircuitBreakerDeltaBackoff { get; }

    TimeSpan ExecutionTimeout { get; }

    int ExecutionMaxConcurrentRequests { get; }

    int FallbackMaxConcurrentRequests { get; }

    int ExecutionMaxRequests { get; }

    int FallbackMaxRequests { get; }

    bool FallbackDisabled { get; }

    TimeSpan MetricsHealthSnapshotInterval { get; }

    int MetricsRollingStatisticalWindowInMilliseconds { get; }

    int MetricsRollingStatisticalWindowBuckets { get; }
  }
}
