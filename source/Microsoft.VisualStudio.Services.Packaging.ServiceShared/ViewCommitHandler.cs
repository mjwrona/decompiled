// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ViewCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class ViewCommitHandler : FeedIndexCommitHandler
  {
    public ViewCommitHandler(FeedIndexCommitHandler successor = null)
      : base(successor)
    {
    }

    public override Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      return commitLogEntry.CommitOperationData is IViewOperationData commitOperationData ? this.ApplyViewOperation(context, feed, commitOperationData) : base.ApplyCommitAsync(context, feed, commitLogEntry);
    }

    private async Task ApplyViewOperation(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      IViewOperationData operationData)
    {
      if (operationData.MetadataSuboperation != MetadataSuboperation.Add)
        throw new ArgumentException(Resources.Error_InvalidViewSuboperation());
      Guid feedPackageId = await FeedIndexCommitHandler.GetFeedPackageId(context, feed, operationData.Identity.Name.NormalizedName);
      try
      {
        await context.FeedIndexClient.PackageVersionViewOperationAsync(feed, feedPackageId, operationData.Identity.Version.NormalizedVersion, (IEnumerable<string>) new string[1]
        {
          operationData.ViewId.ToString()
        }, false);
      }
      catch (FeedViewNotFoundException ex)
      {
      }
    }
  }
}
