// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.InParallelAndCacheSkippingCommitApplier
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class InParallelAndCacheSkippingCommitApplier : IAggregationCommitApplier
  {
    private readonly ITracerService tracer;

    public InParallelAndCacheSkippingCommitApplier(ITracerService tracer) => this.tracer = tracer;

    public async Task<AggregationApplyTimings> ApplyCommitAsync(
      IReadOnlyList<IAggregationAccessor> aggregationAccessors,
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      InParallelAndCacheSkippingCommitApplier sendInTheThisObject = this;
      using (ITracerBlock tracerBlock = sendInTheThisObject.tracer.Enter((object) sendInTheThisObject, nameof (ApplyCommitAsync)))
      {
        tracerBlock.TraceInfo(sendInTheThisObject.GetTraceLineHeader(feedRequest.Feed, commitLogEntries) + "Processing all non-cache accessors in parallel " + string.Join(", ", aggregationAccessors.Select<IAggregationAccessor, string>(new Func<IAggregationAccessor, string>(sendInTheThisObject.GetPrintableAggregationVersion))));
        await Task.WhenAll(aggregationAccessors.Where<IAggregationAccessor>((Func<IAggregationAccessor, bool>) (x => !(x.Aggregation.Definition is CacheAggregationDefinition))).Select<IAggregationAccessor, Task>((Func<IAggregationAccessor, Task>) (aggAccessor => aggAccessor.ApplyCommitAsync(feedRequest, commitLogEntries))));
      }
      AggregationApplyTimings aggregationApplyTimings;
      return aggregationApplyTimings;
    }

    private string GetTraceLineHeader(
      FeedCore feed,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      return string.Format("[f: {0}][firstcommit: {1}][lastcommit: {2}]", (object) feed.Id, (object) commitLogEntries.First<ICommitLogEntry>().CommitId, (object) commitLogEntries.Last<ICommitLogEntry>().CommitId);
    }

    private string GetPrintableAggregationVersion(IAggregationAccessor aggAccessor)
    {
      IAggregation aggregation = aggAccessor.Aggregation;
      return aggregation.Definition.Name + "." + aggregation.VersionName;
    }
  }
}
