// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.GetRegionCircuitBreakerSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.HostManagement
{
  internal class GetRegionCircuitBreakerSettings
  {
    internal const string DefaultCommandGroupKey = "EnterpriseAuthorization.";
    internal const string CommandKeyPrefixForGetRegion = "FrameworkRegionManagementService-GetRegion-";
    internal static readonly CommandPropertiesSetter DefaultCommandPropertiesForGetRegion = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithFallbackDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithCircuitBreakerRequestVolumeThreshold(20).WithCircuitBreakerErrorThresholdPercentage(50).WithCircuitBreakerMinBackoff(TimeSpan.FromSeconds(0.0)).WithCircuitBreakerMaxBackoff(TimeSpan.FromSeconds(30.0)).WithCircuitBreakerDeltaBackoff(TimeSpan.FromMilliseconds(300.0)).WithExecutionTimeout(TimeSpan.FromSeconds(1.0)).WithMetricsHealthSnapshotInterval(TimeSpan.FromSeconds(0.5)).WithMetricsRollingStatisticalWindowInMilliseconds(10000).WithMetricsRollingStatisticalWindowBuckets(10);

    internal string CommandGroupKey { get; set; }

    internal string CommandKeyForGetRegion { get; set; }

    internal CommandPropertiesSetter CircuitBreakerSettingsForGetRegion { get; set; }

    public GetRegionCircuitBreakerSettings(string spsAddress)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(spsAddress, nameof (spsAddress));
      this.CommandGroupKey = "EnterpriseAuthorization.";
      this.CommandKeyForGetRegion = "FrameworkRegionManagementService-GetRegion-" + spsAddress;
      this.CircuitBreakerSettingsForGetRegion = GetRegionCircuitBreakerSettings.DefaultCommandPropertiesForGetRegion;
    }
  }
}
