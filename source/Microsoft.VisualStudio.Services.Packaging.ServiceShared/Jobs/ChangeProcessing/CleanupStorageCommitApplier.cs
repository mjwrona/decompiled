// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing.CleanupStorageCommitApplier
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing
{
  public class CleanupStorageCommitApplier : ICommitApplier
  {
    private readonly IAsyncHandler<IEnumerable<IStorageDeletionRequest>, NullResult> storageDeleter;

    public CleanupStorageCommitApplier(
      IAsyncHandler<IEnumerable<IStorageDeletionRequest>, NullResult> storageDeleter)
    {
      this.storageDeleter = storageDeleter;
    }

    public async Task<AggregationApplyTimings> ApplyCommitAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      Stopwatch s = Stopwatch.StartNew();
      foreach (ICommitLogEntry commitLogEntry in (IEnumerable<ICommitLogEntry>) commitLogEntries)
      {
        List<IStorageDeletionRequest> list = this.GetFlattenedCommits(commitLogEntry.CommitOperationData).Select<ICommitOperationData, IContentDeleteOperationData>((Func<ICommitOperationData, IContentDeleteOperationData>) (c => c as IContentDeleteOperationData)).Where<IContentDeleteOperationData>((Func<IContentDeleteOperationData, bool>) (c => c != null)).Select<IContentDeleteOperationData, IStorageDeletionRequest>((Func<IContentDeleteOperationData, IStorageDeletionRequest>) (c => StorageDeletionRequest.Create(feedRequest.Feed.Id, c.Identity, c.StorageId, c.BlobReferencesToDelete))).ToList<IStorageDeletionRequest>();
        if (list.Any<IStorageDeletionRequest>())
        {
          NullResult nullResult = await this.storageDeleter.Handle((IEnumerable<IStorageDeletionRequest>) list);
        }
      }
      AggregationApplyTimings aggregationApplyTimings = AggregationApplyTimings.FromSingleSource("StorageCleanup", s.ElapsedMilliseconds);
      s = (Stopwatch) null;
      return aggregationApplyTimings;
    }

    private IEnumerable<ICommitOperationData> GetFlattenedCommits(
      ICommitOperationData commitOperationData)
    {
      if (commitOperationData is IBatchCommitOperationData commitOperationData1)
        return commitOperationData1.Operations;
      return (IEnumerable<ICommitOperationData>) new List<ICommitOperationData>()
      {
        commitOperationData
      };
    }
  }
}
