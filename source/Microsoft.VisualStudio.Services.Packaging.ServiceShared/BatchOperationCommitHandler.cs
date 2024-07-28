// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BatchOperationCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class BatchOperationCommitHandler : FeedIndexCommitHandler
  {
    private IFeedIndexAggregationAccessor aggregationAccessor;

    public BatchOperationCommitHandler(
      IFeedIndexAggregationAccessor aggregationAccessor,
      FeedIndexCommitHandler successor = null)
      : base(successor)
    {
      this.aggregationAccessor = aggregationAccessor;
    }

    public override Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      return commitLogEntry.CommitOperationData is IBatchCommitOperationData commitOperationData ? this.ApplyBatchOperation(context, feed, commitLogEntry, commitOperationData) : base.ApplyCommitAsync(context, feed, commitLogEntry);
    }

    private async Task ApplyBatchOperation(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry,
      IBatchCommitOperationData operationData)
    {
      foreach (ICommitOperationData operation in operationData.Operations)
      {
        ICommitLogEntry internalOperation = AggregationAccessorCommonUtils.GetCommitLogEntryForInternalOperation(commitLogEntry, operation);
        await this.aggregationAccessor.ApplyCommitAsync((IFeedRequest) new FeedRequest(feed, context.Protocol), (IReadOnlyList<ICommitLogEntry>) new ICommitLogEntry[1]
        {
          internalOperation
        });
      }
    }
  }
}
