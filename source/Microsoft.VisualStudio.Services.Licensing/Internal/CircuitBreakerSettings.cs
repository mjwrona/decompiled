// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.CircuitBreakerSettings
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  internal class CircuitBreakerSettings
  {
    internal const string DefaultCommandGroupKey = "Licensing.MsdnLicensingAdapter";
    internal const string DefaultCommandKeyForGetMsdnEntitlements = "GetMsdnEntitlements";
    internal static readonly CommandPropertiesSetter DefaultCommandPropertiesForGetMsdnEntitlements = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithFallbackDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithCircuitBreakerRequestVolumeThreshold(20).WithCircuitBreakerErrorThresholdPercentage(80).WithCircuitBreakerMinBackoff(TimeSpan.FromSeconds(0.0)).WithCircuitBreakerMaxBackoff(TimeSpan.FromSeconds(30.0)).WithCircuitBreakerDeltaBackoff(TimeSpan.FromMilliseconds(300.0)).WithExecutionTimeout(TimeSpan.FromSeconds(5.0)).WithMetricsHealthSnapshotInterval(TimeSpan.FromSeconds(0.5)).WithMetricsRollingStatisticalWindowInMilliseconds(60000).WithMetricsRollingStatisticalWindowBuckets(60);

    internal string CommandGroupKey { get; set; }

    internal string CommandKeyForGetMsdnEntitlements { get; set; }

    internal CommandPropertiesSetter CircuitBreakerSettingsForGetMsdnEntitlements { get; set; }

    internal static CircuitBreakerSettings Default => new CircuitBreakerSettings()
    {
      CommandGroupKey = "Licensing.MsdnLicensingAdapter",
      CommandKeyForGetMsdnEntitlements = "GetMsdnEntitlements",
      CircuitBreakerSettingsForGetMsdnEntitlements = CircuitBreakerSettings.DefaultCommandPropertiesForGetMsdnEntitlements
    };
  }
}
