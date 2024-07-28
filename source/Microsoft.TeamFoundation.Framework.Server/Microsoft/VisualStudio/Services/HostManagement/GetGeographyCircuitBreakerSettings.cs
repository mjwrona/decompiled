// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.GetGeographyCircuitBreakerSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.HostManagement
{
  internal class GetGeographyCircuitBreakerSettings
  {
    internal const string DefaultCommandGroupKey = "EnterpriseAuthorization.";
    internal const string CommandKeyPrefixForGetGeography = "FrameworkGeographyManagementService-GetGeography-";
    internal static readonly CommandPropertiesSetter DefaultCommandPropertiesForGetGeography = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithFallbackDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithCircuitBreakerRequestVolumeThreshold(20).WithCircuitBreakerErrorThresholdPercentage(50).WithCircuitBreakerMinBackoff(TimeSpan.FromSeconds(0.0)).WithCircuitBreakerMaxBackoff(TimeSpan.FromSeconds(30.0)).WithCircuitBreakerDeltaBackoff(TimeSpan.FromMilliseconds(300.0)).WithExecutionTimeout(TimeSpan.FromSeconds(1.0)).WithMetricsHealthSnapshotInterval(TimeSpan.FromSeconds(0.5)).WithMetricsRollingStatisticalWindowInMilliseconds(10000).WithMetricsRollingStatisticalWindowBuckets(10);

    internal string CommandGroupKey { get; set; }

    internal string CommandKeyForGetGeography { get; set; }

    internal CommandPropertiesSetter CircuitBreakerSettingsForGetGeography { get; set; }

    public GetGeographyCircuitBreakerSettings(string spsAddress)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(spsAddress, nameof (spsAddress));
      this.CommandGroupKey = "EnterpriseAuthorization.";
      this.CommandKeyForGetGeography = "FrameworkGeographyManagementService-GetGeography-" + spsAddress;
      this.CircuitBreakerSettingsForGetGeography = GetGeographyCircuitBreakerSettings.DefaultCommandPropertiesForGetGeography;
    }
  }
}
