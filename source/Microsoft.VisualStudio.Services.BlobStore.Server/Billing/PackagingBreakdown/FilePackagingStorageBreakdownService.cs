// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Billing.PackagingBreakdown.FilePackagingStorageBreakdownService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Billing.PackagingBreakdown
{
  public class FilePackagingStorageBreakdownService : PackagingStorageBreakdownServiceBase
  {
    private const string CheckpointVersion = "1.0";

    protected override string DedupTypeFolderName { get; } = "File";

    protected override Guid AggregationJobId { get; } = FileVolumeByFeedAggregatorJob.JobId;

    public void SavePartitionResult(
      IVssRequestContext requestContext,
      FileVolumeByFeedResult result,
      JobParameters jobParameters)
    {
      this.SavePartitionResult<FileVolumeByFeedResult>(requestContext, result, jobParameters);
    }

    public IEnumerable<FileVolumeByFeedResult> GetPartitionResults(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      return this.GetPartitionResults<FileVolumeByFeedResult>(requestContext, domainId);
    }

    public void SavePartitionCheckpoint(
      IVssRequestContext requestContext,
      FileVolumeByFeedResult result,
      JobParameters jobParameters)
    {
      JobResultCheckpointManager<FileVolumeByFeedCheckpoint> checkpointManager = new JobResultCheckpointManager<FileVolumeByFeedCheckpoint>(this.BasePath, "1.0", (Func<FileVolumeByFeedCheckpoint, bool>) (cp => true));
      FileVolumeByFeedCheckpoint byFeedCheckpoint = new FileVolumeByFeedCheckpoint()
      {
        Result = result,
        RunId = jobParameters.RunId,
        DomainId = jobParameters.DomainId,
        PartitionId = jobParameters.PartitionId,
        FirstJobStartTime = new DateTimeOffset?(jobParameters.RunDateTime),
        Version = "1.0",
        TotalPartitions = jobParameters.TotalPartitions
      };
      IVssRequestContext requestContext1 = requestContext;
      FileVolumeByFeedCheckpoint jobInfo = byFeedCheckpoint;
      checkpointManager.SaveCheckpoint(requestContext1, jobInfo);
    }

    public FileVolumeByFeedCheckpoint LoadPartitionCheckpoint(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      TimeSpan validity)
    {
      return new JobResultCheckpointManager<FileVolumeByFeedCheckpoint>(this.BasePath, "1.0", (Func<FileVolumeByFeedCheckpoint, bool>) (checkpoint => true)).LoadCheckpointDeleteStale(requestContext, jobParameters, validity);
    }
  }
}
