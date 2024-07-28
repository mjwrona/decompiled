// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RestoreToFeedCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class RestoreToFeedCommitHandler : FeedIndexCommitHandler
  {
    public RestoreToFeedCommitHandler(FeedIndexCommitHandler successor = null)
      : base(successor)
    {
    }

    public override Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      return commitLogEntry.CommitOperationData is IRestoreToFeedOperationData commitOperationData ? this.ApplyRestoreToFeedOperation(context, feed, commitOperationData) : base.ApplyCommitAsync(context, feed, commitLogEntry);
    }

    private async Task ApplyRestoreToFeedOperation(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      IRestoreToFeedOperationData operationData)
    {
      RestoreToFeedCommitHandler sendInTheThisObject = this;
      using (ITracerBlock tracer = context.TracerService.Enter((object) sendInTheThisObject, nameof (ApplyRestoreToFeedOperation)))
      {
        Guid packageId = await FeedIndexCommitHandler.GetFeedPackageId(context, feed, operationData.Identity.Name.NormalizedName);
        Guid packageVersionId = await FeedIndexCommitHandler.GetFeedPackageVersion(context, feed, packageId, operationData.Identity.Version.NormalizedVersion);
        try
        {
          await context.FeedIndexClient.RestorePackageVersionToFeedAsync(feed, packageId, packageVersionId);
        }
        catch (PackageVersionNotFoundException ex)
        {
          tracer.TraceInfo(string.Format("Package not found in recycle bin. Feed Id: {0}, Package Id: {1}, PackageVersion Id: {2}", (object) feed.Id, (object) packageId, (object) packageVersionId));
        }
        packageId = new Guid();
        packageVersionId = new Guid();
      }
    }
  }
}
