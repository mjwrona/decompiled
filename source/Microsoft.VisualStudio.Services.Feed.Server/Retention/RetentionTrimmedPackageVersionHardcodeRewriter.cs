// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Retention.RetentionTrimmedPackageVersionHardcodeRewriter
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server.Retention
{
  internal class RetentionTrimmedPackageVersionHardcodeRewriter
  {
    private static readonly Dictionary<string, string> trimmedVersionsMap = new Dictionary<string, string>()
    {
      {
        "20220218.18-1.72.0-feature-9240-experimental-migration-to-mcl-1.72.0-feature-9240-experimental-migration-to-mcl-1.72.0-feature-",
        "20220218.18-1.72.0-feature-9240-experimental-migration-to-mcl-1.72.0-feature-9240-experimental-migration-to-mcl-1.72.0-feature-9240-experimental-migration-to-mcl"
      },
      {
        "20220412.12-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ve",
        "20220412.12-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220408.4-1.73.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ver",
        "20220408.4-1.73.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220207.1-1.71.0-feature-9240-experimental-migration-to-mcl-1.71.0-feature-9240-experimental-migration-to-mcl-1.71.0-feature-9",
        "20220207.1-1.71.0-feature-9240-experimental-migration-to-mcl-1.71.0-feature-9240-experimental-migration-to-mcl-1.71.0-feature-9240-experimental-migration-to-mcl"
      },
      {
        "20220412.21-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ve",
        "20220412.21-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220207.4-1.71.0-feature-9240-experimental-migration-to-mcl-1.71.0-feature-9240-experimental-migration-to-mcl-1.71.0-feature-9",
        "20220207.4-1.71.0-feature-9240-experimental-migration-to-mcl-1.71.0-feature-9240-experimental-migration-to-mcl-1.71.0-feature-9240-experimental-migration-to-mcl"
      },
      {
        "20220406.12-1.73.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ve",
        "20220406.12-1.73.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220412.17-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ve",
        "20220412.17-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220412.14-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ve",
        "20220412.14-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220414.4-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ver",
        "20220414.4-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220408.6-1.73.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ver",
        "20220408.6-1.73.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220413.8-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ver",
        "20220413.8-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220408.2-1.73.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ver",
        "20220408.2-1.73.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220406.13-1.73.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ve",
        "20220406.13-1.73.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220506.5-1.76.0-feature-10500-get-authorization-from-cpd-and-extract-vins-1.76.0-feature-10500-get-authorization-from-cpd-and",
        "20220506.5-1.76.0-feature-10500-Get-Authorization-From-CPD-And-Extract-VINs-1.76.0-feature-10500-Get-Authorization-From-CPD-And-Extract-VINs"
      },
      {
        "20220412.22-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ve",
        "20220412.22-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220412.4-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ver",
        "20220412.4-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220411.4-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ver",
        "20220411.4-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220414.7-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ver",
        "20220414.7-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220412.11-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ve",
        "20220412.11-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220411.3-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ver",
        "20220411.3-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220412.10-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ve",
        "20220412.10-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20211202.3-1.68.0-feature-8690-intrudice-new-artifact-number-1.68.0-feature-8690-intrudice-new-artifact-number-1.68.0-feature-8",
        "20211202.3-1.68.0-feature-8690-intrudice-new-artifact-number-1.68.0-feature-8690-intrudice-new-artifact-number-1.68.0-feature-8690-intrudice-new-artifact-number"
      },
      {
        "20220411.11-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ve",
        "20220411.11-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220412.18-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ve",
        "20220412.18-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220413.4-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ver",
        "20220413.4-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      },
      {
        "20220411.5-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-ver",
        "20220411.5-1.76.0-feature-14412-map-powertype-and-changingmode-from-connectortype-for-homecharging-stations-for-updated-mcl-version"
      }
    };

    public static IEnumerable<ProtocolPackageVersionIdentity> RewriteKnownTrimmedVersions(
      IVssRequestContext requestContext,
      IEnumerable<ProtocolPackageVersionIdentity> packageVersionIdentities)
    {
      return requestContext.IsFeatureEnabled("Packaging.Feed.ForceRewriteTrimmedPackageVersionsForMaven") ? (IEnumerable<ProtocolPackageVersionIdentity>) packageVersionIdentities.Select<ProtocolPackageVersionIdentity, ProtocolPackageVersionIdentity>((Func<ProtocolPackageVersionIdentity, ProtocolPackageVersionIdentity>) (packageVersion => !RetentionTrimmedPackageVersionHardcodeRewriter.IsPackageVersionTrimmed(packageVersion) ? packageVersion : RetentionTrimmedPackageVersionHardcodeRewriter.ClonePackageVersionIdentityWithNewVersion(packageVersion, RetentionTrimmedPackageVersionHardcodeRewriter.trimmedVersionsMap[packageVersion.Version]))).ToList<ProtocolPackageVersionIdentity>() : packageVersionIdentities;
    }

    private static bool IsPackageVersionTrimmed(ProtocolPackageVersionIdentity packageVersion) => packageVersion.Protocol.Equals("maven", StringComparison.OrdinalIgnoreCase) && packageVersion.PackageName.StartsWith("com.daimler") && !string.IsNullOrEmpty(packageVersion.Version) && RetentionTrimmedPackageVersionHardcodeRewriter.trimmedVersionsMap.ContainsKey(packageVersion.Version);

    private static ProtocolPackageVersionIdentity ClonePackageVersionIdentityWithNewVersion(
      ProtocolPackageVersionIdentity packageVersion,
      string newVersion)
    {
      if (!packageVersion.PackageId.HasValue || !packageVersion.PackageVersionId.HasValue)
        return new ProtocolPackageVersionIdentity(packageVersion.Protocol, packageVersion.PackageName, newVersion);
      string protocol = packageVersion.Protocol;
      string packageName = packageVersion.PackageName;
      string version = newVersion;
      Guid? nullable = packageVersion.PackageId;
      Guid packageId = nullable.Value;
      nullable = packageVersion.PackageVersionId;
      Guid packageVersionId = nullable.Value;
      return new ProtocolPackageVersionIdentity(protocol, packageName, version, packageId, packageVersionId);
    }
  }
}
