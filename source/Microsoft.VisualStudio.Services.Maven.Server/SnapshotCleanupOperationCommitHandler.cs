// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.SnapshotCleanupOperationCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.Helpers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class SnapshotCleanupOperationCommitHandler : FeedIndexCommitHandler
  {
    public SnapshotCleanupOperationCommitHandler(FeedIndexCommitHandler successor = null)
      : base(successor)
    {
    }

    public override Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      return commitLogEntry.CommitOperationData is MavenSnapshotCleanupOperationData commitOperationData ? SnapshotCleanupOperationCommitHandler.ApplySnapshotCleanupOperation(context, feed, commitOperationData) : base.ApplyCommitAsync(context, feed, commitLogEntry);
    }

    private static async Task ApplySnapshotCleanupOperation(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      MavenSnapshotCleanupOperationData operationData)
    {
      Guid guid = feed.Id;
      guid.ToString();
      MavenPackageIdentity identity = operationData.Identity;
      guid = await FeedIndexCommitHandler.GetFeedPackageId(context, feed, identity.Name.NormalizedName);
      Guid packageId = guid;
      PackageVersion packageVersionAsync = await context.FeedIndexClient.GetPackageVersionAsync(feed, packageId, identity.Version.NormalizedVersion, true);
      IEnumerable<PackageFile> filesToRemove = SnapshotCleanupOperationCommitHandler.GetFilesToRemove(identity, packageVersionAsync.Files, operationData.SnapshotInstanceIds);
      IFeedIndexClient feedIndexClient = context.FeedIndexClient;
      FeedCore feed1 = feed;
      string packageId1 = packageId.ToString();
      guid = packageVersionAsync.Id;
      string packageVersionId = guid.ToString();
      IEnumerable<PackageFile> files = packageVersionAsync.Files.Except<PackageFile>(filesToRemove);
      await feedIndexClient.UpdatePackageVersionAsync(feed1, packageId1, packageVersionId, files);
      identity = (MavenPackageIdentity) null;
    }

    private static IEnumerable<PackageFile> GetFilesToRemove(
      MavenPackageIdentity identity,
      IEnumerable<PackageFile> files,
      IList<MavenSnapshotInstanceId> snapshotInstancesIds)
    {
      return new MavenSnapshotMetadataFiles<PackageFile>(identity.Name, files, (Func<PackageFile, string>) (x => x.Name)).FilterFilesByInstances((IEnumerable<MavenSnapshotInstanceId>) snapshotInstancesIds);
    }
  }
}
