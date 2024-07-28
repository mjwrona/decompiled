// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing.ScheduleDpjCommitApplier
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing
{
  public class ScheduleDpjCommitApplier : ICommitApplier
  {
    private readonly IFeedJobQueuer dpjQueuer;
    private readonly ITimeProvider timeProvider;
    private readonly IAsyncHandler<IDeleteOperationData, DateTime> scheduledPermanentDeleteDateResolvingHandler;

    public ScheduleDpjCommitApplier(
      IFeedJobQueuer dpjQueuer,
      ITimeProvider timeProvider,
      IAsyncHandler<IDeleteOperationData, DateTime> scheduledPermanentDeleteDateResolvingHandler)
    {
      this.dpjQueuer = dpjQueuer;
      this.timeProvider = timeProvider;
      this.scheduledPermanentDeleteDateResolvingHandler = scheduledPermanentDeleteDateResolvingHandler;
    }

    public async Task<AggregationApplyTimings> ApplyCommitAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      ScheduleDpjCommitApplier dpjCommitApplier = this;
      Stopwatch s = Stopwatch.StartNew();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated method
      DateTime? nullable = await new ConcurrentIterator<IDeleteOperationData>(commitLogEntries.SelectMany<ICommitLogEntry, ICommitLogEntry>(ScheduleDpjCommitApplier.\u003C\u003EO.\u003C0\u003E__FlattenCommitLogEntry ?? (ScheduleDpjCommitApplier.\u003C\u003EO.\u003C0\u003E__FlattenCommitLogEntry = new Func<ICommitLogEntry, IEnumerable<ICommitLogEntry>>(AggregationAccessorCommonUtils.FlattenCommitLogEntry))).Select<ICommitLogEntry, IDeleteOperationData>((Func<ICommitLogEntry, IDeleteOperationData>) (x => x.CommitOperationData as IDeleteOperationData)).Where<IDeleteOperationData>((Func<IDeleteOperationData, bool>) (x => x != null))).SelectAsync<IDeleteOperationData, DateTime>(new Func<IDeleteOperationData, Task<DateTime>>(dpjCommitApplier.\u003CApplyCommitAsync\u003Eb__4_2)).NullableMinOrDefaultAsync<DateTime>();
      if (nullable.HasValue)
      {
        int maxDelaySeconds = Math.Max(0, (int) (nullable.Value + FeedLevelDeletedPackageJobConstants.RequeueDelayAfterNextScheduledPermanentDelete - dpjCommitApplier.timeProvider.Now).TotalSeconds);
        Guid guid = await dpjCommitApplier.dpjQueuer.QueueJob(feedRequest.Feed, feedRequest.Protocol, JobPriorityLevel.Normal, maxDelaySeconds);
      }
      AggregationApplyTimings aggregationApplyTimings = AggregationApplyTimings.FromSingleSource("ScheduleDpj", s.ElapsedMilliseconds);
      s = (Stopwatch) null;
      return aggregationApplyTimings;
    }
  }
}
