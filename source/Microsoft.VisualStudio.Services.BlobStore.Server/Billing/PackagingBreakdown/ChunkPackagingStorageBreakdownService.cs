// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Billing.PackagingBreakdown.ChunkPackagingStorageBreakdownService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Billing.PackagingBreakdown
{
  public class ChunkPackagingStorageBreakdownService : PackagingStorageBreakdownServiceBase
  {
    protected override string DedupTypeFolderName { get; } = "Chunk";

    protected override Guid AggregationJobId { get; } = ChunkVolumeByFeedAggregatorJob.JobId;

    public void SavePartitionResult(
      IVssRequestContext requestContext,
      ChunkVolumeByFeedResult result,
      JobParameters jobParameters)
    {
      this.SavePartitionResult<ChunkVolumeByFeedResult>(requestContext, result, jobParameters);
    }

    public IEnumerable<ChunkVolumeByFeedResult> GetPartitionResults(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      return this.GetPartitionResults<ChunkVolumeByFeedResult>(requestContext, domainId);
    }
  }
}
