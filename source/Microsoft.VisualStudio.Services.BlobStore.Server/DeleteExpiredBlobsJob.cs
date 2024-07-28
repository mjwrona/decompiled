// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DeleteExpiredBlobsJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Domain;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class DeleteExpiredBlobsJob : ArtifactsPartitionedJobBase
  {
    public static readonly Guid DeleteExpiredBlobsJobId = ArtifactReservedJobIds.DeleteExpiredBlobsJobId;
    private const int DefaultMaxExpiredBlobDeletionParallelism = 32;
    protected static readonly TimeSpan ClockBuffer = TimeSpan.FromDays(1.0);

    protected override Guid ParentJobId => ArtifactReservedJobIds.DeleteExpiredBlobsJobId;

    protected override string JobNamePrefix => "Blob store clean-up child job - ";

    protected override string RegistryBasePath => "/Configuration/BlobStore/BlobDeletionJob";

    protected override (TraceData traceData, int tracePoint) TraceInfo => (new TraceData()
    {
      Area = "BlobStore",
      Layer = "Job"
    }, 5701110);

    protected override bool ShouldReuseJobResultForInactiveHost => true;

    public override bool IsValidPartitionSize(
      IVssRequestContext rc,
      int partitionSize,
      int numStorageAccounts)
    {
      return partitionSize >= numStorageAccounts && partitionSize % numStorageAccounts == 0 && BlobIdsForPartition.SupportedNumberOfBlobIdPartitions.Contains<int>(partitionSize / numStorageAccounts);
    }

    protected override bool IsCPUThrottlingDisabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Blobstore.Features.DisableBlobDeletionJobCPUThrottling");

    protected TimeSpan CalculateTimeBudget(TimeSpan frequency, int numJobPartitions) => frequency - TimeSpan.FromMinutes(numJobPartitions >= 32 ? 60.0 : 0.0);

    protected override async Task<string> RunJobAsync(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      DeleteExpiredBlobsJob deleteExpiredBlobsJob = this;
      IVssRegistryService registryService = requestContext.GetService<IVssRegistryService>();
      string startBlobIdRegistryPath = ServiceRegistryConstants.BlobDeletionCheckpointPath;
      if (jobParameters.TotalPartitions > 0)
        startBlobIdRegistryPath += string.Format("/P{0}-{1}", (object) jobParameters.TotalPartitions, (object) jobParameters.PartitionId);
      startBlobIdRegistryPath += "/BlobId";
      BlobIdentifier startBlobIdOrNull;
      try
      {
        string valueIncludingAlgorithm = registryService.GetValue(requestContext, (RegistryQuery) startBlobIdRegistryPath, false, (string) null);
        startBlobIdOrNull = valueIncludingAlgorithm == null ? (BlobIdentifier) null : BlobIdentifier.Deserialize(valueIncludingAlgorithm);
      }
      catch (Exception ex)
      {
        startBlobIdOrNull = (BlobIdentifier) null;
        tracer.TraceException(ex);
      }
      int num = registryService.GetValue<int>(requestContext, (RegistryQuery) (deleteExpiredBlobsJob.RegistryBasePath + "/BlobIdPartitionSize"), false, 0);
      AdminPlatformBlobStore service = requestContext.GetService<AdminPlatformBlobStore>();
      TimeSpan timeBudget = deleteExpiredBlobsJob.CalculateTimeBudget(deleteExpiredBlobsJob.Settings.JobExecutionTimeBudget, jobParameters.TotalPartitions);
      DeletionJobInfo info = new DeletionJobInfo()
      {
        Result = new DeletionJobResult(),
        TimeBudget = timeBudget,
        ClockBuffer = DeleteExpiredBlobsJob.ClockBuffer,
        MaxConcurrency = deleteExpiredBlobsJob.Settings.MaxParallelism != 0 ? deleteExpiredBlobsJob.Settings.MaxParallelism : 32,
        BlobIdPartitionSize = num,
        RunId = jobParameters.RunId,
        DomainId = jobParameters.DomainId
      };
      try
      {
        info.Result.IsResumedFromCheckpoint = startBlobIdOrNull != (BlobIdentifier) null;
        info.Result.PartitionId = jobParameters.PartitionId;
        info.Result.TotalPartitions = jobParameters.TotalPartitions;
        info.Result.CpuThreshold = deleteExpiredBlobsJob.Settings.CpuThreshold;
        IteratorPartition iteratorPartition;
        BlobIdsForPartition blobIdPartitionInfo = deleteExpiredBlobsJob.GetBlobIdPartitionInfo(info, startBlobIdOrNull, tracer, out iteratorPartition);
        IDomainId domainId = DomainIdFactory.Create(jobParameters.DomainId);
        await service.CheckForDeletionAsync(requestContext, domainId, info, blobIdPartitionInfo, iteratorPartition, deleteExpiredBlobsJob.IsCPUThrottlingDisabled(requestContext)).ConfigureAwait(true);
        if (info.Result.EndBlobId == null)
          registryService.DeleteEntries(requestContext, startBlobIdRegistryPath);
        else
          registryService.SetValue<string>(requestContext, startBlobIdRegistryPath, info.Result.EndBlobId);
      }
      catch (Exception ex)
      {
        tracer.TraceException(ex);
        info.Result.Exception = ex;
        throw;
      }
      info.Result.StartBlobId = info.Result.StartBlobId?.Substring(0, 12);
      info.Result.EndBlobId = info.Result.EndBlobId?.Substring(0, 12);
      string str = JsonSerializer.Serialize<DeletionJobInfo>(info);
      registryService = (IVssRegistryService) null;
      startBlobIdRegistryPath = (string) null;
      info = new DeletionJobInfo();
      return str;
    }

    private BlobIdsForPartition GetBlobIdPartitionInfo(
      DeletionJobInfo jobInfo,
      BlobIdentifier startBlobIdOrNull,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      out IteratorPartition iteratorPartition)
    {
      int blobIdPartitionSize = jobInfo.BlobIdPartitionSize;
      BlobIdentifier blobIdentifier = startBlobIdOrNull;
      if ((object) blobIdentifier == null)
        blobIdentifier = BlobIdentifier.MinValue;
      BlobIdentifier startId1 = blobIdentifier;
      int partitionId = jobInfo.Result.PartitionId;
      int totalPartitions1 = jobInfo.Result.TotalPartitions;
      BlobIdsForPartition.ValidateBlobIdPartitionSize(blobIdPartitionSize, totalPartitions1);
      BlobIdsForPartition blobIdPartitionInfo;
      if (blobIdPartitionSize <= 1)
      {
        iteratorPartition = new IteratorPartition(0, 1);
        blobIdPartitionInfo = BlobIdsForPartition.Create((byte) 0, 1, startId1);
      }
      else
      {
        int partition = (int) (byte) (partitionId % blobIdPartitionSize);
        iteratorPartition = new IteratorPartition(partitionId / blobIdPartitionSize, totalPartitions1 / blobIdPartitionSize, PartitionStrategy.ExactOneToOne);
        int totalPartitions2 = blobIdPartitionSize;
        BlobIdentifier startId2 = startId1;
        blobIdPartitionInfo = BlobIdsForPartition.Create((byte) partition, totalPartitions2, startId2);
      }
      tracer.TraceAlways(string.Format("Job starting with Iterator: {0}, BlobIdPartition: {1}", (object) iteratorPartition, (object) blobIdPartitionInfo));
      return blobIdPartitionInfo;
    }

    protected override async Task<IEnumerable<PhysicalDomainInfo>> GetDomains(
      IVssRequestContext requestContext)
    {
      return !requestContext.AllowHostDomainAdminOperations() ? (IEnumerable<PhysicalDomainInfo>) null : await requestContext.GetService<AdminHostDomainStoreService>().GetPhysicalDomainsForOrganizationForAdminAsync(requestContext).ConfigureAwait(true);
    }
  }
}
