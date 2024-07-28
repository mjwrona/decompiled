// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing.CombiningCommitApplier
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing
{
  public class CombiningCommitApplier : ICommitApplier
  {
    private readonly ICommitApplier[] innerAppliers;

    public CombiningCommitApplier(params ICommitApplier[] innerAppliers) => this.innerAppliers = innerAppliers;

    public async Task<AggregationApplyTimings> ApplyCommitAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      AggregationApplyTimings timings = new AggregationApplyTimings();
      ICommitApplier[] commitApplierArray = this.innerAppliers;
      for (int index = 0; index < commitApplierArray.Length; ++index)
      {
        ICommitApplier commitApplier = commitApplierArray[index];
        AggregationApplyTimings aggregationApplyTimings = timings;
        IFeedRequest feedRequest1 = feedRequest;
        IReadOnlyList<ICommitLogEntry> commitLogEntries1 = commitLogEntries;
        aggregationApplyTimings.MergeWith(await commitApplier.ApplyCommitAsync(feedRequest1, commitLogEntries1));
        aggregationApplyTimings = (AggregationApplyTimings) null;
      }
      commitApplierArray = (ICommitApplier[]) null;
      AggregationApplyTimings aggregationApplyTimings1 = timings;
      timings = (AggregationApplyTimings) null;
      return aggregationApplyTimings1;
    }
  }
}
