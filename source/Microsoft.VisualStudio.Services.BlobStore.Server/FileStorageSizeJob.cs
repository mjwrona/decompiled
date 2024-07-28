// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.FileStorageSizeJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Billing;
using Microsoft.VisualStudio.Services.BlobStore.Server.Billing.PackagingBreakdown;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Domain;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class FileStorageSizeJob : ArtifactsPartitionedWaitedJob
  {
    private const string RegistryFileStorageSizeJobMode = "/JobMode";
    private const string RegistryCheckpointValidityPeriod = "/CheckpointValidityPeriod";
    private const string RegistryMaxCheckpointValidityPeriod = "/MaxCheckpointValidityPeriod";
    private const string JobCheckpointVersion = "0.5";
    public static readonly Guid FileStorageSizeJobId = ArtifactReservedJobIds.FileStorageSizeJobId;
    private static readonly TimeSpan DefaultCheckpointValidityPeriod = TimeSpan.FromHours(23.0);
    private readonly IEnumerable<int> ValidBlobIdPartitionSizes = (IEnumerable<int>) JobFanoutSettings.SubPartitionMap[ArtifactReservedJobIds.FileStorageSizeJobId];
    private readonly JobResultCheckpointManager<FileStorageJobInfo> resultCheckpointManager = new JobResultCheckpointManager<FileStorageJobInfo>("/JobOutput/BlobStore/FileStorageSizeJob", "0.5", (Func<FileStorageJobInfo, bool>) (checkpoint => checkpoint.LastProcessedBlobId != null));

    protected override Guid ParentJobId { get; } = ArtifactReservedJobIds.FileStorageSizeJobId;

    protected override string JobNamePrefix => "File storage size child job - ";

    protected override string RegistryBasePath => "/Configuration/BlobStore/FileStorageSizeJob";

    protected override bool ShouldReuseJobResultForInactiveHost => true;

    public override bool IsValidPartitionSize(
      IVssRequestContext rc,
      int partitionSize,
      int numStorageAccounts)
    {
      return numStorageAccounts % partitionSize == 0 || this.ValidBlobIdPartitionSizes.Any<int>((Func<int, bool>) (p => partitionSize == numStorageAccounts * p));
    }

    protected override bool IsCPUThrottlingDisabled(IVssRequestContext requestContext) => true;

    protected override (TraceData traceData, int tracePoint) TraceInfo => (new TraceData()
    {
      Area = "BlobStore",
      Layer = "Job"
    }, 5701150);

    protected override async Task<string> RunJobAsync(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      FileStorageSizeJob fileStorageSizeJob = this;
      FileStorageJobInfo jobInfo = fileStorageSizeJob.CreateJobInfo(requestContext, jobParameters);
      FilePackagingStorageBreakdownService filePackagingStorageBreakdownService = requestContext.GetService<FilePackagingStorageBreakdownService>();
      if (fileStorageSizeJob.IsRunningForDefaultDomain((FileStorageJobBase) jobInfo))
      {
        if (jobInfo.PartitionId == 0)
          filePackagingStorageBreakdownService.ResetResultsIfNeeded(requestContext, jobParameters);
        FileVolumeByFeedCheckpoint byFeedCheckpoint = filePackagingStorageBreakdownService.LoadPartitionCheckpoint(requestContext, jobParameters, jobInfo.CheckpointValidityPeriod);
        if (byFeedCheckpoint != null)
          jobInfo.Result.LogicalSizeByFeed = byFeedCheckpoint.Result.LogicalSizeByFeed;
      }
      if (jobInfo.IsCompleteResult)
      {
        tracer.TraceAlways(string.Format("Job skipped for Partition: {0} as checkpoint represents completed result", (object) jobParameters.PartitionId));
      }
      else
      {
        try
        {
          await new FileStorageSizeJobHelper().CollectStorageDataFromMetadataAsync(requestContext, (IFileStorageSizeJobInfo) jobInfo, jobParameters, tracer, fileStorageSizeJob.IsCPUThrottlingDisabled(requestContext)).ConfigureAwait(true);
        }
        finally
        {
          fileStorageSizeJob.resultCheckpointManager.SaveCheckpoint(requestContext, jobInfo);
          if (fileStorageSizeJob.IsRunningForDefaultDomain((FileStorageJobBase) jobInfo))
          {
            ConcurrentDictionary<string, ulong> logicalSizeByFeed = UsageInfoServiceExtensions.GetTopLogicalSizeByFeed(jobInfo.Result.LogicalSizeByFeed);
            FileVolumeByFeedResult result = new FileVolumeByFeedResult()
            {
              LogicalSizeByFeed = logicalSizeByFeed
            };
            if (jobInfo.IsCompleteResult)
            {
              filePackagingStorageBreakdownService.SavePartitionResult(requestContext, result, jobParameters);
              if (jobInfo.TotalPartitions == 1)
              {
                filePackagingStorageBreakdownService.SaveRunResult(requestContext, jobParameters);
                filePackagingStorageBreakdownService.QueueAggregationJob(requestContext);
              }
            }
            else
              filePackagingStorageBreakdownService.SavePartitionCheckpoint(requestContext, result, jobParameters);
          }
        }
      }
      string str = JsonSerializer.Serialize<FileStorageJobInfo>(jobInfo);
      jobInfo = (FileStorageJobInfo) null;
      filePackagingStorageBreakdownService = (FilePackagingStorageBreakdownService) null;
      return str;
    }

    private FileStorageJobInfo CreateJobInfo(
      IVssRequestContext requestContext,
      JobParameters jobParameters)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      FileStorageJobMode fileStorageJobMode = service.GetValue<FileStorageJobMode>(requestContext, (RegistryQuery) (this.RegistryBasePath + "/JobMode"), true, FileStorageJobMode.UpdateBlobLength);
      TimeSpan checkpointValidityPeriod = service.GetValue<TimeSpan>(requestContext, (RegistryQuery) (this.RegistryBasePath + "/CheckpointValidityPeriod"), true, FileStorageSizeJob.DefaultCheckpointValidityPeriod);
      int num = service.GetValue<int>(requestContext, (RegistryQuery) (this.RegistryBasePath + "/BlobIdPartitionSize"), false, 1);
      FileStorageJobInfo jobInfo = this.resultCheckpointManager.LoadCheckpoint(requestContext, jobParameters, checkpointValidityPeriod);
      if (jobInfo == null)
        jobInfo = new FileStorageJobInfo()
        {
          FirstJobStartTime = new DateTimeOffset?(DateTimeOffset.UtcNow),
          LastProcessedBlobId = (string) null
        };
      jobInfo.Version = "0.5";
      jobInfo.CpuThreshold = this.Settings.CpuThreshold;
      jobInfo.PartitionId = jobParameters.PartitionId;
      jobInfo.DomainId = jobParameters.DomainId;
      jobInfo.TotalPartitions = jobParameters.TotalPartitions;
      jobInfo.RunId = jobParameters.RunId;
      jobInfo.BlobIdPartitionSize = num;
      jobInfo.CheckpointValidityPeriod = checkpointValidityPeriod;
      jobInfo.JobMode = fileStorageJobMode;
      return jobInfo;
    }

    protected override VssJobResult AggregateChildJobResults(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobHistoryEntry> successfulChildJobs,
      JobParameters jobParameters)
    {
      IEnumerable<FileStorageJobInfo> source = successfulChildJobs.Select<TeamFoundationJobHistoryEntry, FileStorageJobInfo>((Func<TeamFoundationJobHistoryEntry, FileStorageJobInfo>) (r => JsonSerializer.Deserialize<FileStorageJobInfo>(r.ResultMessage))).Where<FileStorageJobInfo>((Func<FileStorageJobInfo, bool>) (jr => jr.IsCompleteResult));
      ParentFileStorageJobInfo fileStorageJobInfo1 = new ParentFileStorageJobInfo();
      fileStorageJobInfo1.CpuThreshold = this.Settings.CpuThreshold;
      fileStorageJobInfo1.TotalPartitions = jobParameters.TotalPartitions;
      fileStorageJobInfo1.RunId = jobParameters.RunId;
      fileStorageJobInfo1.DomainId = jobParameters.DomainId;
      fileStorageJobInfo1.TotalSucceededChildJobs = successfulChildJobs.Count<TeamFoundationJobHistoryEntry>();
      fileStorageJobInfo1.Result = new FileStorageSizeJobResult();
      ParentFileStorageJobInfo fileStorageJobInfo2 = fileStorageJobInfo1;
      fileStorageJobInfo2.Result.AddStorageSizeJobResult(source.Select<FileStorageJobInfo, FileStorageSizeJobResult>((Func<FileStorageJobInfo, FileStorageSizeJobResult>) (jr => jr.Result)));
      int num = source.Count<FileStorageJobInfo>();
      IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
      TimeSpan checkpointValidityPeriod = service1.GetValue<TimeSpan>(requestContext, (RegistryQuery) (this.RegistryBasePath + "/MaxCheckpointValidityPeriod"), true, TimeSpan.Zero);
      if (checkpointValidityPeriod > TimeSpan.Zero)
      {
        Dictionary<int, FileStorageJobInfo> dictionary = source.ToDictionary<FileStorageJobInfo, int>((Func<FileStorageJobInfo, int>) (jr => jr.PartitionId));
        for (int index = 0; index < jobParameters.TotalPartitions; ++index)
        {
          if (!dictionary.ContainsKey(index))
          {
            JobParameters jobParameters1 = JobParameters.CreateNew(index, jobParameters.TotalPartitions, (IDomainId) ByteDomainId.Deserialize(jobParameters.DomainId));
            FileStorageJobInfo fileStorageJobInfo3 = this.resultCheckpointManager.LoadCheckpoint(requestContext, jobParameters1, checkpointValidityPeriod);
            if (fileStorageJobInfo3 != null)
            {
              // ISSUE: explicit non-virtual call
              DateTimeOffset? firstJobStartTime = __nonvirtual (fileStorageJobInfo3.FirstJobStartTime);
              if (firstJobStartTime.HasValue)
              {
                firstJobStartTime = fileStorageJobInfo3.FirstJobStartTime;
                DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow.Subtract(checkpointValidityPeriod);
                if ((firstJobStartTime.HasValue ? (firstJobStartTime.GetValueOrDefault() > dateTimeOffset ? 1 : 0) : 0) != 0 && fileStorageJobInfo3.IsCompleteResult)
                {
                  fileStorageJobInfo2.Result.AddStorageSizeJobResult((IEnumerable<FileStorageSizeJobResult>) new FileStorageSizeJobResult[1]
                  {
                    fileStorageJobInfo3.Result
                  });
                  ++num;
                }
              }
            }
          }
        }
      }
      if (num == 0)
        throw new InvalidOperationException("No child job present.");
      double factor = (double) fileStorageJobInfo2.TotalPartitions / (double) num;
      fileStorageJobInfo2.Result.Scale(factor);
      fileStorageJobInfo2.TotalChildJobResultsUsed = num;
      List<RegistryItem> list = service1.Read(requestContext, (RegistryQuery) "/Configuration/BlobStore/FileStorageSizeJob/Checkpoint/**").ToList<RegistryItem>();
      if (list.Any<RegistryItem>())
        service1.UpdateOrDeleteEntries(requestContext, list.Select<RegistryItem, RegistryEntry>((Func<RegistryItem, RegistryEntry>) (x => new RegistryEntry(x.Path, (string) null))));
      if (this.IsRunningForDefaultDomain((FileStorageJobBase) fileStorageJobInfo2))
      {
        FilePackagingStorageBreakdownService service2 = requestContext.GetService<FilePackagingStorageBreakdownService>();
        service2.SaveRunResult(requestContext, jobParameters);
        service2.QueueAggregationJob(requestContext);
      }
      return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, JsonSerializer.Serialize<ParentFileStorageJobInfo>(fileStorageJobInfo2));
    }

    protected override async Task<IEnumerable<PhysicalDomainInfo>> GetDomains(
      IVssRequestContext requestContext)
    {
      return !requestContext.AllowHostDomainAdminOperations() ? (IEnumerable<PhysicalDomainInfo>) null : await requestContext.GetService<AdminHostDomainStoreService>().GetPhysicalDomainsForOrganizationForAdminAsync(requestContext).ConfigureAwait(true);
    }

    private bool IsRunningForDefaultDomain(FileStorageJobBase jobInfo) => jobInfo.DomainId == WellKnownDomainIds.DefaultDomainId.Serialize();
  }
}
