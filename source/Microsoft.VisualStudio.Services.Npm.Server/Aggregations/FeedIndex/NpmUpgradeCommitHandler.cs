// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Aggregations.FeedIndex.NpmUpgradeCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Aggregations.FeedIndex
{
  public class NpmUpgradeCommitHandler : FeedIndexCommitHandler
  {
    public NpmUpgradeCommitHandler(FeedIndexCommitHandler successor = null)
      : base(successor)
    {
    }

    public override async Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      if (commitLogEntry.CommitOperationData is NpmUpgradeOperationData)
        return;
      await base.ApplyCommitAsync(context, feed, commitLogEntry);
    }
  }
}
