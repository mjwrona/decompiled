// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Telemetry.NuGetCiDataFactory
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Telemetry
{
  public static class NuGetCiDataFactory
  {
    public static PushPackageCiData GetNuGetPushPackageCiData(
      IVssRequestContext requestContext,
      string protocolVersion,
      FeedCore feed,
      VssNuGetPackageIdentity packageIdentity,
      long packageSize,
      bool fromUpstream,
      string storageType)
    {
      return new PushPackageCiData(requestContext, (IProtocol) Protocol.NuGet, protocolVersion, feed, TelemetryHelpers.WrapStringWithNonEmailGDPRException(packageIdentity.Name.DisplayName), packageIdentity.Version.DisplayVersion, packageSize, fromUpstream, storageType);
    }

    public static QueryPackageCiData GetNuGetQueryCiData(
      IVssRequestContext requestContext,
      string protocolVersion,
      FeedCore feed)
    {
      return new QueryPackageCiData(requestContext, (IProtocol) Protocol.NuGet, protocolVersion, feed);
    }

    public static DeletePackageCiData GetNuGetDeleteCiData(
      IVssRequestContext requestContext,
      string protocolVersion,
      FeedCore feed,
      string packageId,
      string packageVersion,
      long packageSize)
    {
      return new DeletePackageCiData(requestContext, (IProtocol) Protocol.NuGet, protocolVersion, feed, TelemetryHelpers.WrapStringWithNonEmailGDPRException(packageId), packageVersion, packageSize);
    }

    public static DownloadPackageCiData GetNuGetDownloadCiData(
      IVssRequestContext requestContext,
      string protocolVersion,
      FeedCore feed,
      string packageId,
      string packageVersion,
      long packageSize,
      string packageSource,
      string storageType,
      UpstreamSource upstreamSource = null,
      string fileName = null)
    {
      return new DownloadPackageCiData(requestContext, (IProtocol) Protocol.NuGet, protocolVersion, feed, TelemetryHelpers.WrapStringWithNonEmailGDPRException(packageId), packageVersion, packageSize, packageSource, storageType, upstreamSource, fileName);
    }

    public static PromotePackageCiData GetNuGetPromoteCiData(
      IVssRequestContext requestContext,
      FeedCore feed,
      FeedView view,
      FeedView[] existingViews,
      string packageName,
      string packageVersion,
      long packageSize)
    {
      return new PromotePackageCiData(requestContext, (IProtocol) Protocol.NuGet, feed, view, existingViews, TelemetryHelpers.WrapStringWithNonEmailGDPRException(packageName), packageVersion, packageSize);
    }

    public static RelistPackageCiData GetNuGetRelistCiData(
      IVssRequestContext requestContext,
      string protocolVersion,
      FeedCore feed,
      string packageId,
      string packageVersion,
      long packageSize)
    {
      return new RelistPackageCiData(requestContext, (IProtocol) Protocol.NuGet, protocolVersion, feed, TelemetryHelpers.WrapStringWithNonEmailGDPRException(packageId), packageVersion, packageSize);
    }

    public static UnlistPackageCiData GetNuGetUnlistCiData(
      IVssRequestContext requestContext,
      string protocolVersion,
      FeedCore feed,
      string packageId,
      string packageVersion,
      long packageSize)
    {
      return new UnlistPackageCiData(requestContext, (IProtocol) Protocol.NuGet, protocolVersion, feed, TelemetryHelpers.WrapStringWithNonEmailGDPRException(packageId), packageVersion, packageSize);
    }

    public static PermanentDeletePackageCiData GetNuGetPermanentDeleteCiData(
      IVssRequestContext requestContext,
      string protocolVersion,
      FeedCore feed,
      string packageId,
      string packageVersion)
    {
      return new PermanentDeletePackageCiData(requestContext, (IProtocol) Protocol.NuGet, protocolVersion, feed, TelemetryHelpers.WrapStringWithNonEmailGDPRException(packageId), packageVersion);
    }

    public static RestoreToFeedPackageCiData GetNuGetRestoreToFeedCiData(
      IVssRequestContext requestContext,
      string protocolVersion,
      FeedCore feed,
      string packageId,
      string packageVersion)
    {
      return new RestoreToFeedPackageCiData(requestContext, (IProtocol) Protocol.NuGet, protocolVersion, feed, TelemetryHelpers.WrapStringWithNonEmailGDPRException(packageId), packageVersion);
    }
  }
}
