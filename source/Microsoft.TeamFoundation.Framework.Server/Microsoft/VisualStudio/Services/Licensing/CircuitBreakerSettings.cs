// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.CircuitBreakerSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class CircuitBreakerSettings
  {
    internal const string DefaultCommandGroupKey = "Licensing.Framework";
    internal const string DefaultCommandKeyForGetExtensionRights = "GetOrSetExtensionRights";
    internal static readonly CommandPropertiesSetter DefaultCommandPropertiesForOutBoundCallToLicensing = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithFallbackDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithCircuitBreakerRequestVolumeThreshold(20).WithCircuitBreakerErrorThresholdPercentage(50).WithCircuitBreakerMinBackoff(TimeSpan.FromMilliseconds(100.0)).WithCircuitBreakerMaxBackoff(TimeSpan.FromSeconds(30.0)).WithCircuitBreakerDeltaBackoff(TimeSpan.FromMilliseconds(300.0)).WithExecutionTimeout(TimeSpan.FromSeconds(10.0)).WithMetricsHealthSnapshotInterval(TimeSpan.FromSeconds(0.5)).WithMetricsRollingStatisticalWindowInMilliseconds(60000).WithMetricsRollingStatisticalWindowBuckets(60);

    internal string CommandGroupKey { get; set; }

    internal string CommandKeyForGetExtensionRights { get; set; }

    internal CommandPropertiesSetter CircuitBreakerSettingsForOutBoundCallToLicensing { get; set; }

    internal static CircuitBreakerSettings Default => new CircuitBreakerSettings()
    {
      CommandGroupKey = "Licensing.Framework",
      CommandKeyForGetExtensionRights = "GetOrSetExtensionRights",
      CircuitBreakerSettingsForOutBoundCallToLicensing = CircuitBreakerSettings.DefaultCommandPropertiesForOutBoundCallToLicensing
    };
  }
}
