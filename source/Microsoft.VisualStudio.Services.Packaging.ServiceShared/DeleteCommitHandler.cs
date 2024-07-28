// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class DeleteCommitHandler : FeedIndexCommitHandler
  {
    private readonly IAsyncHandler<IDeleteOperationData, DateTime> scheduledPermanentDeleteDateResolvingHandler;

    public DeleteCommitHandler(
      IAsyncHandler<IDeleteOperationData, DateTime> scheduledPermanentDeleteDateResolvingHandler,
      FeedIndexCommitHandler successor = null)
      : base(successor)
    {
      this.scheduledPermanentDeleteDateResolvingHandler = scheduledPermanentDeleteDateResolvingHandler;
    }

    public override Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      return commitLogEntry.CommitOperationData is IDeleteOperationData commitOperationData ? this.ApplyDeleteOperation(context, feed, commitOperationData) : base.ApplyCommitAsync(context, feed, commitLogEntry);
    }

    private async Task ApplyDeleteOperation(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      IDeleteOperationData operationData)
    {
      try
      {
        Guid feedPackageId = await FeedIndexCommitHandler.GetFeedPackageId(context, feed, operationData.Identity.Name.NormalizedName);
        IFeedIndexClient feedIndexClient = context.FeedIndexClient;
        FeedCore feed1 = feed;
        Guid packageId = feedPackageId;
        string packageVersionId = operationData.Identity.Version.NormalizedVersion;
        DateTime deletedDate = operationData.DeletedDate;
        await feedIndexClient.DeletePackageVersionAsync(feed1, packageId, packageVersionId, deletedDate, await this.scheduledPermanentDeleteDateResolvingHandler.Handle(operationData));
        feedIndexClient = (IFeedIndexClient) null;
        feed1 = (FeedCore) null;
        packageId = new Guid();
        packageVersionId = (string) null;
      }
      catch (PackageNotFoundException ex)
      {
      }
    }
  }
}
