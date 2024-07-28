// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.CircuitBreakerSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class CircuitBreakerSettings
  {
    internal const string DefaultCommandGroupKey = "EnterpriseAuthorization.";
    internal const string DefaultCommandKeyForReadIdentitiesInRootScope = "ReadIdentitiesInRootScope";
    internal const string DefaultCommandKeyForReadIdentitiesInChildScope = "ReadIdentitiesInChildScope";
    internal static readonly CommandPropertiesSetter DefaultCommandPropertiesForReadIdentitiesInScope = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithFallbackDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithCircuitBreakerRequestVolumeThreshold(10).WithCircuitBreakerErrorThresholdPercentage(20).WithCircuitBreakerMinBackoff(TimeSpan.FromSeconds(0.0)).WithCircuitBreakerMaxBackoff(TimeSpan.FromSeconds(30.0)).WithCircuitBreakerDeltaBackoff(TimeSpan.FromMilliseconds(300.0)).WithExecutionTimeout(TimeSpan.FromSeconds(80.0)).WithMetricsHealthSnapshotInterval(TimeSpan.FromSeconds(0.5)).WithMetricsRollingStatisticalWindowInMilliseconds(60000).WithMetricsRollingStatisticalWindowBuckets(60);

    internal string CommandGroupKey { get; set; }

    internal string CommandKeyForReadIdentitiesInRootScope { get; set; }

    internal string CommandKeyForReadIdentitiesInChildScope { get; set; }

    internal CommandPropertiesSetter CircuitBreakerSettingsForReadIdentitiesInScope { get; set; }

    internal static CircuitBreakerSettings Default => new CircuitBreakerSettings()
    {
      CommandGroupKey = "EnterpriseAuthorization.",
      CommandKeyForReadIdentitiesInRootScope = "ReadIdentitiesInRootScope",
      CommandKeyForReadIdentitiesInChildScope = "ReadIdentitiesInChildScope",
      CircuitBreakerSettingsForReadIdentitiesInScope = CircuitBreakerSettings.DefaultCommandPropertiesForReadIdentitiesInScope
    };
  }
}
