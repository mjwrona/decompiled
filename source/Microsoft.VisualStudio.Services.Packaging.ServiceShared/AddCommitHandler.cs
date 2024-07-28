// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.AddCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class AddCommitHandler : FeedIndexCommitHandler
  {
    public AddCommitHandler(FeedIndexCommitHandler successor = null)
      : base(successor)
    {
    }

    public override Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      return commitLogEntry.CommitOperationData is IAddOperationData commitOperationData ? AddCommitHandler.ApplyAddOperation(context, feed, commitOperationData, commitLogEntry) : base.ApplyCommitAsync(context, feed, commitLogEntry);
    }

    internal static async Task ApplyAddOperation(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      IAddOperationData op,
      ICommitLogEntry commitLogEntry)
    {
      PackageIndexEntryResponse response = await context.FeedIndexClient.SetIndexEntryAsync(feed, op.ConvertToIndexEntry(commitLogEntry, feed));
      if (op is IAddOperationDataSupportsAddAsDelisted supportsAddAsDelisted && supportsAddAsDelisted.AddAsDelisted)
        await context.FeedIndexClient.UpdatePackageVersionAsync(feed, context.Protocol.ToString(), op.Identity.Name.NormalizedName, op.Identity.Version.NormalizedVersion, new bool?(false));
      FeedView viewOrDefaultAsync = await context.FeedService.GetLocalViewOrDefaultAsync(feed);
      if (viewOrDefaultAsync == null)
      {
        response = (PackageIndexEntryResponse) null;
      }
      else
      {
        await context.FeedIndexClient.PackageVersionViewOperationAsync(feed, response.PackageId, op.Identity.Version.NormalizedVersion, (IEnumerable<string>) new string[1]
        {
          viewOrDefaultAsync.Id.ToString()
        }, false);
        response = (PackageIndexEntryResponse) null;
      }
    }
  }
}
