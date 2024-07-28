// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Telemetry.NpmCiDataFactory
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;

namespace Microsoft.VisualStudio.Services.Npm.Server.Telemetry
{
  public static class NpmCiDataFactory
  {
    public static DeletePackageCiData GetNpmDeletePackageCiData(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      long packageSize,
      string packageSource)
    {
      return new DeletePackageCiData(requestContext, (IProtocol) Protocol.npm, string.Empty, feed, packageName, packageVersion, packageSize, packageSource);
    }

    public static RestoreToFeedPackageCiData GetNpmRestorePackageToFeedCiData(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      string packageSource)
    {
      return new RestoreToFeedPackageCiData(requestContext, (IProtocol) Protocol.npm, string.Empty, feed, packageName, packageVersion, packageSource);
    }

    public static PermanentDeletePackageCiData GetNpmPermanentDeletePackageCiData(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      string packageSource)
    {
      return new PermanentDeletePackageCiData(requestContext, (IProtocol) Protocol.npm, string.Empty, feed, packageName, packageVersion, packageSource);
    }

    public static DownloadPackageCiData GetNpmDownloadPackageCiData(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      long packageSize,
      string packageSource,
      UpstreamSource upstreamSource)
    {
      return new DownloadPackageCiData(requestContext, (IProtocol) Protocol.npm, string.Empty, feed, packageName, packageVersion, packageSize, packageSource, upstreamSource: upstreamSource);
    }

    public static PushPackageCiData GetNpmPushPackageCiData(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      long packageSize,
      bool fromUpstream)
    {
      return new PushPackageCiData(requestContext, (IProtocol) Protocol.npm, string.Empty, feed, packageName, packageVersion, packageSize, fromUpstream);
    }

    public static DistTagSetCiData GetNpmDistTagSetCiData(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      string tag)
    {
      return new DistTagSetCiData(requestContext, (IProtocol) Protocol.npm, string.Empty, feed, packageName, packageVersion, tag);
    }

    public static DistTagRemoveCiData GetNpmDistTagRemoveCiData(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      string tag)
    {
      return new DistTagRemoveCiData(requestContext, (IProtocol) Protocol.npm, string.Empty, feed, packageName, packageVersion, tag);
    }

    public static PromotePackageCiData GetNpmPromotePackageCiData(
      IVssRequestContext requestContext,
      FeedCore feed,
      FeedView view,
      FeedView[] existingViews,
      string packageName,
      string packageVersion,
      long packageSize,
      string packageSource)
    {
      return new PromotePackageCiData(requestContext, (IProtocol) Protocol.npm, feed, view, existingViews, packageName, packageVersion, packageSize, packageSource);
    }

    public static QueryPackageCiData GetNpmQueryCiData(
      IVssRequestContext requestContext,
      FeedCore feed)
    {
      return new QueryPackageCiData(requestContext, (IProtocol) Protocol.npm, string.Empty, feed);
    }

    public static UnlistPackageCiData GetNpmDeprecateCiData(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      long packageSize,
      string packageSource)
    {
      return new UnlistPackageCiData(requestContext, (IProtocol) Protocol.npm, string.Empty, feed, packageName, packageVersion, packageSize, packageSource);
    }

    public static GetPackageCiData GetNpmGetPackageCiData(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageId,
      string packageVersion = null)
    {
      return new GetPackageCiData(requestContext, (IProtocol) Protocol.npm, string.Empty, feed, packageId, packageVersion);
    }

    public static BatchOperationCiData GetNpmBatchCiData(
      IVssRequestContext requestContext,
      FeedCore feed,
      NpmBatchOperationType batchOperation,
      string serializedBatchData,
      int batchSize)
    {
      return new BatchOperationCiData(requestContext, (IProtocol) Protocol.npm, feed, batchOperation.ToString(), serializedBatchData, batchSize);
    }
  }
}
