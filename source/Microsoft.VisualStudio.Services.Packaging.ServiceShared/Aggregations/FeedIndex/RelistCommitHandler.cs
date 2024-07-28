// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.FeedIndex.RelistCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.FeedIndex
{
  public class RelistCommitHandler : FeedIndexCommitHandler
  {
    public RelistCommitHandler(FeedIndexCommitHandler successor = null)
      : base(successor)
    {
    }

    public override Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      return commitLogEntry.CommitOperationData is IRelistOperationData commitOperationData ? this.ApplyRelistOperation(context, feed, commitOperationData) : base.ApplyCommitAsync(context, feed, commitLogEntry);
    }

    private async Task ApplyRelistOperation(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      IRelistOperationData operationData)
    {
      try
      {
        await context.FeedIndexClient.UpdatePackageVersionAsync(feed, context.Protocol.ToString(), operationData.Identity.Name.NormalizedName, operationData.Identity.Version.NormalizedVersion, new bool?(true));
      }
      catch (PackageNotFoundException ex)
      {
      }
      catch (PackageVersionNotFoundException ex)
      {
      }
    }
  }
}
