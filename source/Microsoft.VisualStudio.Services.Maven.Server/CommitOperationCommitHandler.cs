// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.CommitOperationCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class CommitOperationCommitHandler : FeedIndexCommitHandler
  {
    public CommitOperationCommitHandler(FeedIndexCommitHandler successor = null)
      : base(successor)
    {
    }

    public override Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      return commitLogEntry.CommitOperationData is IMavenCommitOperationData commitOperationData ? CommitOperationCommitHandler.ApplyCommitOperation(context, feed, commitOperationData, commitLogEntry) : base.ApplyCommitAsync(context, feed, commitLogEntry);
    }

    private static async Task ApplyCommitOperation(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      IMavenCommitOperationData operationData,
      ICommitLogEntry commitLogEntry)
    {
      IPackageIdentity identity = operationData.Identity;
      PackageIndexEntry packageIndexEntry = operationData.ConvertToIndexEntry(commitLogEntry, feed);
      try
      {
        Guid packageId = await FeedIndexCommitHandler.GetFeedPackageId(context, feed, identity.Name.NormalizedName);
        PackageVersion packageVersionAsync = await context.FeedIndexClient.GetPackageVersionAsync(feed, packageId, identity.Version.NormalizedVersion, true);
        packageIndexEntry.PackageVersion.Files = (IEnumerable<PackageFile>) packageIndexEntry.PackageVersion.Files.CombineWith<PackageFile>(packageVersionAsync.Files);
        if (operationData.Pom == null)
        {
          await context.FeedIndexClient.UpdatePackageVersionAsync(feed, packageId.ToString(), packageVersionAsync.Id.ToString(), packageIndexEntry.PackageVersion.Files);
          identity = (IPackageIdentity) null;
          packageIndexEntry = (PackageIndexEntry) null;
          return;
        }
        packageId = new Guid();
      }
      catch (PackageNotFoundException ex)
      {
      }
      catch (PackageVersionNotFoundException ex)
      {
      }
      PackageIndexEntryResponse indexEntryResponse = await context.FeedIndexClient.SetIndexEntryAsync(feed, packageIndexEntry);
      try
      {
        FeedView view = context.FeedService.GetFeed(feed.Project.ToProjectIdOrEmptyGuid(), feed.Id.ToString() + "@local").View;
        if (view.Type != FeedViewType.Implicit)
        {
          identity = (IPackageIdentity) null;
          packageIndexEntry = (PackageIndexEntry) null;
        }
        else
        {
          await context.FeedIndexClient.PackageVersionViewOperationAsync(feed, indexEntryResponse.PackageId, indexEntryResponse.PackageVersionId.ToString(), (IEnumerable<string>) new string[1]
          {
            view.Id.ToString()
          }, false);
          identity = (IPackageIdentity) null;
          packageIndexEntry = (PackageIndexEntry) null;
        }
      }
      catch (FeedViewNotFoundException ex)
      {
        identity = (IPackageIdentity) null;
        packageIndexEntry = (PackageIndexEntry) null;
      }
    }
  }
}
