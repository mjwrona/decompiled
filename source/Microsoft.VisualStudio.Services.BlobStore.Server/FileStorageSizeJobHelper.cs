// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.FileStorageSizeJobHelper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Billing;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Models;
using Microsoft.VisualStudio.Services.BlobStore.Server.DataExport;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class FileStorageSizeJobHelper
  {
    private const int MinTraceTimeInMs = 3600000;

    public async Task CollectStorageDataFromMetadataAsync(
      IVssRequestContext requestContext,
      IFileStorageSizeJobInfo jobInfo,
      JobParameters parameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      bool CPUThrottlingDisabled)
    {
      int blobIdPartitionSize = jobInfo.BlobIdPartitionSize;
      BlobIdentifier startId = jobInfo.IsResumedFromCheckpoint ? BlobIdentifier.Deserialize(jobInfo.LastProcessedBlobId) : BlobIdentifier.MinValue;
      IteratorPartition iteratorPartition;
      BlobIdsForPartition blobIdPartition;
      if (blobIdPartitionSize <= 1)
      {
        iteratorPartition = new IteratorPartition(jobInfo.PartitionId, jobInfo.TotalPartitions);
        blobIdPartition = BlobIdsForPartition.Create((byte) 0, 1, startId);
      }
      else
      {
        if (blobIdPartitionSize > 256 || blobIdPartitionSize >= jobInfo.TotalPartitions || jobInfo.TotalPartitions % blobIdPartitionSize != 0)
          throw new ArgumentException(string.Format("Invalid BlobIdPartitionSize: {0}, TotalPartitions: {1}", (object) blobIdPartitionSize, (object) jobInfo.TotalPartitions));
        byte partition = (byte) (jobInfo.PartitionId % blobIdPartitionSize);
        iteratorPartition = new IteratorPartition(jobInfo.PartitionId / blobIdPartitionSize, jobInfo.TotalPartitions / blobIdPartitionSize, PartitionStrategy.ExactOneToOne);
        blobIdPartition = BlobIdsForPartition.Create(partition, blobIdPartitionSize, startId);
      }
      tracer.TraceAlways(string.Format("Job starting with Iterator: {0}, BlobIdPartition: {1}", (object) iteratorPartition, (object) blobIdPartition));
      AdminPlatformBlobStore blobStore = requestContext.GetService<AdminPlatformBlobStore>();
      IEnumerable<IBlobIdReferenceProcessor> exporters = (IEnumerable<IBlobIdReferenceProcessor>) ReferenceExporterPluginRegistration.GetEnabledFileExporters(requestContext, parameters).ToList<IBlobIdReferenceProcessor>();
      await requestContext.PumpOrInlineFromAsync((ISecuredDomainRequest) new SecuredDomainRequest(requestContext, DomainIdFactory.Create(jobInfo.DomainId)), (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor =>
      {
        FileStorageJobInfo jobInfo1 = jobInfo as FileStorageJobInfo;
        await this.CollectStorageDataFromMetadataAsync(processor, blobStore, jobInfo1, iteratorPartition, blobIdPartition, tracer, exporters, CPUThrottlingDisabled).ConfigureAwait(false);
      }), true).ConfigureAwait(true);
    }

    private async Task CollectStorageDataFromMetadataAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      AdminPlatformBlobStore blobstore,
      FileStorageJobInfo jobInfo,
      IteratorPartition iteratorPartition,
      BlobIdsForPartition blobIdPartition,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      IEnumerable<IBlobIdReferenceProcessor> idReferenceRowProcessors,
      bool CPUthrottlingDisabled)
    {
      int numRetried = 0;
      FileStorageSizeJobResult result = jobInfo.Result;
      BlobIdentifier curBlobId = blobIdPartition.StartValue;
      Stopwatch stopwatch = Stopwatch.StartNew();
      long lastCheckTime = 0;
      bool isRetryRequired;
      do
      {
        isRetryRequired = false;
        try
        {
          DomainIdFactory.Create(jobInfo.DomainId);
          using (IConcurrentIterator<IBlobMetadataSizeInfo> enumerator = blobstore.GetBlobMetadataSizeConcurrentIterator(processor, curBlobId, blobIdPartition.MaxValue, iteratorPartition, true, (IEnumerable<IBlobIdReferenceRowVisitor>) idReferenceRowProcessors))
          {
            while (true)
            {
              if (await enumerator.MoveNextAsync(processor.CancellationToken).ConfigureAwait(false))
              {
                IBlobMetadataSizeInfo sizeInfo = enumerator.Current;
                if (jobInfo.JobMode == FileStorageJobMode.UpdateBlobLength && sizeInfo.StoredReferenceState == BlobReferenceState.AddedBlob && !sizeInfo.BlobLength.HasValue)
                {
                  try
                  {
                    result.TotalSizeUpdates += (long) await blobstore.UpdateBlobLengthForExistingMetadataAsync(processor, sizeInfo).ConfigureAwait(false);
                  }
                  catch (Exception ex)
                  {
                    ++result.TotalSizeUpdateFailures;
                  }
                }
                result.AddSizeInfo(sizeInfo);
                curBlobId = sizeInfo.BlobId;
                sizeInfo.IdReferenceCountByFeed = UsageInfoServiceExtensions.GetTopLogicalSizeByFeed(sizeInfo.IdReferenceCountByFeed);
                if (stopwatch.ElapsedMilliseconds - lastCheckTime > 3600000L)
                {
                  lastCheckTime = stopwatch.ElapsedMilliseconds;
                  tracer.TraceInfo(string.Format("P[{0}/{1}]: Progress PerMille: {2}, ", (object) jobInfo.PartitionId, (object) jobInfo.TotalPartitions, (object) blobIdPartition.CalculateJobPerMille(curBlobId)) + string.Format("TimeInMinutes: {0}", (object) stopwatch.Elapsed.TotalMinutes));
                }
                if (!CPUthrottlingDisabled)
                  result.TotalThrottleDuration += TimeSpan.FromSeconds((double) await CpuThrottleHelper.Instance.Yield(jobInfo.CpuThreshold, processor.CancellationToken).ConfigureAwait(false));
                sizeInfo = (IBlobMetadataSizeInfo) null;
              }
              else
                break;
            }
          }
          jobInfo.LastProcessedBlobId = curBlobId.ValueString;
          jobInfo.JobCompletePerMille = 1000;
          jobInfo.IsCompleteResult = true;
          foreach (IBlobIdReferenceProcessor referenceRowProcessor in idReferenceRowProcessors)
          {
            try
            {
              referenceRowProcessor.Complete();
            }
            catch (Exception ex)
            {
              tracer.TraceException(ex);
            }
          }
        }
        catch (Exception ex)
        {
          isRetryRequired = AsyncHttpRetryHelper.IsTransientException(ex, processor.CancellationToken) && ++numRetried < 10 && !processor.CancellationToken.IsCancellationRequested;
          if (!isRetryRequired)
          {
            jobInfo.LastProcessedBlobId = curBlobId?.ValueString;
            jobInfo.JobCompletePerMille = blobIdPartition.CalculateJobPerMille(curBlobId);
            throw;
          }
          await Task.Delay(JobHelper.RetryInterval).ConfigureAwait(false);
        }
      }
      while (isRetryRequired);
      result.NumJobRetried += numRetried;
      result = (FileStorageSizeJobResult) null;
      curBlobId = (BlobIdentifier) null;
      stopwatch = (Stopwatch) null;
    }
  }
}
