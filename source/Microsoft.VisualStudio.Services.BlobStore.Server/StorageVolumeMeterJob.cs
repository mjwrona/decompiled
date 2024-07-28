// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.StorageVolumeMeterJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.AzComm.SharedContracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi.Contracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi.Enums;
using Microsoft.VisualStudio.Services.AzComm.WebApi.HttpClients;
using Microsoft.VisualStudio.Services.BlobStore.Server.Billing;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Security;
using Polly;
using Polly.Retry;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class StorageVolumeMeterJob : VssAsyncJobExtension
  {
    public static readonly Guid StorageVolumeMeterJobId = Guid.Parse("c22ba93c-0423-413b-bd2b-1df56e619231");
    private readonly TraceData traceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "Job"
    };
    private const int Tracepoint = 5701997;
    private const string JobExecutionStateRegistryKey = "/EnabledState";
    private const string FrequentReportingStateRegistryKey = "/EnableFrequentReporting";
    private const string EnableMaxQtyRetrievalKey = "/EnableMaxQtyRetrieval";
    private const string DiscountLogicalQtyRegistryKey = "/DiscountLogicalQty";
    private const string CustomerIntelligenceEventAccount = "BlobStore";
    private const string StorageVolumeMeter = "StorageVolumeMeter";
    private const string CustIntKeyEventId = "EventId";
    private const string CustIntKeyTotalLogicalVolume = "TotalLogicalVolume";
    private const string CustIntKeyTotalPhysicalVolume = "TotalPhysicalVolume";
    private const string CustIntKeyTotalLogicalNonExemptedVolume = "TotalLogicalNonExemptedVolume";
    private const string CustIntKeyMaxVolume = "MaxVolume";
    private const string CustIntKeyTotalLogicalFileVolume = "TotalLogicalFileVolume";
    private const string CustIntKeyTotalLogicalChunkVolume = "TotalLogicalChunkVolume";
    private const string CustIntKeyTotalPhysicalFileVolume = "TotalPhysicalFileVolume";
    private const string CustIntKeyTotalPhysicalChunkVolume = "TotalPhysicalChunkVolume";
    private const string CustIntKeyLogicalFileDropVolume = "LogicalFileDropVolume";
    private const string CustIntKeyLogicalFileSymbolVolume = "LogicalFileSymbolVolume";
    private const string CustIntKeyLogicalFileNugetVolume = "LogicalFileNugetVolume";
    private const string CustIntKeyLogicalFileNpmVolume = "LogicalFileNpmVolume";
    private const string CustIntKeyLogicalFileMavenVolume = "LogicalFileMavenVolume";
    private const string CustIntKeyLogicalFilePyPiVolume = "LogicalFilePyPiVolume";
    private const string CustIntKeyLogicalFileIvyVolume = "LogicalFileIvyVolume";
    private const string CustIntKeyLogicalFileOthersVolume = "LogicalFileOthersVolume";
    private const string CustIntKeyLogicalChunkDropVolume = "LogicalChunkDropVolume";
    private const string CustIntKeyLogicalChunkSymbolVolume = "LogicalChunkSymbolVolume";
    private const string CustIntKeyLogicalChunkPAVolume = "LogicalChunkPAVolume";
    private const string CustIntKeyLogicalChunkPCVolume = "LogicalChunkPCVolume";
    private const string CustIntKeyLogicalChunkUpackVolume = "LogicalChunkUpackVolume";
    private const string CustIntKeyLogicalChunkOthersVolume = "LogicalChunkOthersVolume";

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.traceData, 5701997, nameof (RunAsync)))
      {
        try
        {
          IVssRegistryService registryService = requestContext.GetService<IVssRegistryService>();
          JobHelper.JobExecutionState enabledState = registryService.GetValue<JobHelper.JobExecutionState>(requestContext, (RegistryQuery) "/Configuration/BlobStore/StorageVolumeMeterJob/EnabledState", true, JobHelper.JobExecutionState.Enabled);
          if (JobHelper.IsJobDisabled(requestContext, enabledState, jobDefinition.JobId))
            return new VssJobResult(TeamFoundationJobExecutionResult.Blocked, string.Format("[StorageVolumeMeter]: Job is disabled by registry setting. Current state: {0}", (object) enabledState));
          IVssRegistryService registryService1 = registryService;
          IVssRequestContext requestContext1 = requestContext;
          RegistryQuery registryQuery = (RegistryQuery) "/Configuration/BlobStore/StorageVolumeMeterJob/EnableFrequentReporting";
          ref RegistryQuery local1 = ref registryQuery;
          if (!registryService1.GetValue<bool>(requestContext1, in local1, true, true))
          {
            TeamFoundationJobHistoryEntry foundationJobHistoryEntry = requestContext.GetService<ITeamFoundationJobService>().QueryLatestJobHistory(requestContext, StorageVolumeMeterJob.StorageVolumeMeterJobId);
            DateTime dateTime = foundationJobHistoryEntry.ExecutionStartTime.ToDateTimeOffset().UtcDateTime;
            DateTime date1 = dateTime.Date;
            dateTime = DateTimeOffset.UtcNow.DateTime;
            DateTime date2 = dateTime.Date;
            if ((!(date1 == date2) || !foundationJobHistoryEntry.JobSource.Equals(requestContext.ServiceHost.InstanceId) ? 0 : (foundationJobHistoryEntry.Result.Equals((object) TeamFoundationJobExecutionResult.Succeeded) ? 1 : 0)) != 0)
              return new VssJobResult(TeamFoundationJobExecutionResult.Stopped);
          }
          IUsageInfoService service = requestContext.GetService<IUsageInfoService>();
          DateTimeOffset currentTime = DateTimeOffset.UtcNow;
          IVssRequestContext requestContext2 = requestContext;
          DateTimeOffset maxTime = currentTime;
          DataContracts.UsageInfo usageInfo = await service.GetStorageUsageInfo(requestContext2, maxTime).ConfigureAwait(true);
          if (usageInfo == null)
            throw new InvalidOperationException(string.Format("[StorageVolumeMeter]: Couldn't retrieve usage info from the usage info service for time: {0}.", (object) currentTime));
          IVssRegistryService registryService2 = registryService;
          IVssRequestContext requestContext3 = requestContext;
          registryQuery = (RegistryQuery) "/Configuration/BlobStore/StorageVolumeMeterJob/DiscountLogicalQty";
          ref RegistryQuery local2 = ref registryQuery;
          double num = registryService2.GetValue<double>(requestContext3, in local2, true, 1.0);
          StorageVolumeMeterJobResult jobResult = new StorageVolumeMeterJobResult()
          {
            TimeStamp = currentTime,
            DiscountMultiplier = num,
            EventId = Guid.NewGuid(),
            TotalPhysicalVolumeInGiB = BlobStoreUtils.ToBillableGiB(usageInfo.PhysicalFileStorageSize + usageInfo.PhysicalChunkStorageSize),
            TotalLogicalVolumeInGiB = BlobStoreUtils.ToBillableGiB(usageInfo.LogicalFileStorageSize + usageInfo.LogicalChunkStorageSize) * num,
            TotalLogicalVolumeNonExemptedInGiB = BlobStoreUtils.ToBillableGiB(usageInfo.LogicalFileStorageNonExemptedSize + usageInfo.LogicalChunkStorageNonExemptedSize),
            TotalFileLogicalVolumeInGiB = BlobStoreUtils.ToBillableGiB(usageInfo.LogicalFileStorageSize),
            TotalFilePhysicalVolumeInGiB = BlobStoreUtils.ToBillableGiB(usageInfo.PhysicalFileStorageSize),
            TotalChunkLogicalVolumeInGiB = BlobStoreUtils.ToBillableGiB(usageInfo.LogicalChunkStorageSize),
            TotalChunkPhysicalVolumeInGiB = BlobStoreUtils.ToBillableGiB(usageInfo.PhysicalChunkStorageSize),
            LogicalFileUsageBreakdownInfo = usageInfo.LogicalFileUsageBreakdownInfo,
            LogicalChunkUsageBreakdownInfo = usageInfo.LogicalChunkUsageBreakdownInfo,
            PhysicalFileUsageBreakdownInfo = usageInfo.PhysicalFileUsageBreakdownInfo,
            PhysicalChunkUsageBreakdownInfo = usageInfo.PhysicalChunkUsageBreakdownInfo,
            TotalRetryCount = 0
          };
          if (!BlobStoreUtils.IsInternalHost(requestContext))
          {
            if (usageInfo.LogicalFileUsageBreakdownInfo.LogicalOthersStorageSize > usageInfo.LogicalFileStorageSize)
              throw new InvalidOperationException("[StorageVolumeMeter]: File dedup :Others: category footprint is larger than total file logical volume: " + string.Format("{0} : {1} ", (object) "LogicalOthersStorageSize", (object) usageInfo.LogicalFileUsageBreakdownInfo.LogicalOthersStorageSize) + string.Format("{0} {1}.", (object) "LogicalFileStorageSize", (object) usageInfo.LogicalFileStorageSize));
            if (usageInfo.LogicalChunkUsageBreakdownInfo.LogicalOthersStorageSize > usageInfo.LogicalChunkStorageSize)
              throw new InvalidOperationException("[StorageVolumeMeter]: Chunk dedup :Others: category footprint is larger than total chunk logical volume: " + string.Format("{0} : {1} ", (object) "LogicalOthersStorageSize", (object) usageInfo.LogicalChunkUsageBreakdownInfo.LogicalOthersStorageSize) + string.Format("{0} {1}.", (object) "LogicalChunkStorageSize", (object) usageInfo.LogicalChunkStorageSize));
            double billableGiB = BlobStoreUtils.ToBillableGiB(usageInfo.LogicalFileUsageBreakdownInfo.LogicalOthersStorageSize + usageInfo.LogicalChunkUsageBreakdownInfo.LogicalOthersStorageSize);
            if (billableGiB > jobResult.TotalLogicalVolumeInGiB)
              throw new InvalidOperationException("[StorageVolumeMeter]: All UP :Others: category footprint is larger than the all UP total logical volume: " + string.Format("{0} : {1} ", (object) "TotalOthersFootprintInGB", (object) billableGiB) + string.Format("{0} : {1} ", (object) "LogicalOthersStorageSize", (object) usageInfo.LogicalFileUsageBreakdownInfo.LogicalOthersStorageSize) + string.Format("{0} : {1} ", (object) "LogicalOthersStorageSize", (object) usageInfo.LogicalChunkUsageBreakdownInfo.LogicalOthersStorageSize) + string.Format("{0} {1}.", (object) "TotalLogicalVolumeInGiB", (object) jobResult.TotalLogicalVolumeInGiB));
            tracer.TraceAlways(string.Format("[StorageVolumeMeter]: Discounting Others category : {0} GB from total footprint: {1} GB.", (object) billableGiB, (object) jobResult.TotalLogicalVolumeInGiB));
            jobResult.TotalLogicalVolumeInGiB -= billableGiB;
          }
          requestContext.GetService<FrameworkMeteringService>();
          AsyncRetryPolicy retryPolicy = AsyncRetrySyntax.WaitAndRetryAsync(Policy.Handle<Exception>((Func<Exception, bool>) (ex =>
          {
            switch (ex)
            {
              case UnexpectedHostTypeException _:
              case AccessCheckException _:
                return false;
              default:
                return !(ex is ArgumentOutOfRangeException);
            }
          })), 3, (Func<int, TimeSpan>) (retryAttempt => TimeSpan.FromSeconds(Math.Pow(5.0, (double) retryAttempt))), (Action<Exception, TimeSpan, int, Context>) ((exception, timeSpan, retryCount, context) => jobResult.TotalRetryCount = retryCount));
          MeterUsage2HttpClient azCommClient = requestContext.To(TeamFoundationHostType.Deployment).GetClient<MeterUsage2HttpClient>();
          PolicyResult policyResult1 = await ((AsyncPolicy) retryPolicy).ExecuteAndCaptureAsync((Func<Task>) (async () =>
          {
            MeterUsage2HttpClient usage2HttpClient = azCommClient;
            MeterUsageReportRequest payload = new MeterUsageReportRequest();
            payload.EventId = jobResult.EventId.ToString();
            payload.Kind = UsageReportOperation.Set;
            payload.MeterId = AzCommMeterIds.ArtifactsMeterId;
            payload.Quantity = jobResult.TotalLogicalVolumeInGiB;
            Guid instanceId = requestContext.ServiceHost.InstanceId;
            CancellationToken cancellationToken = new CancellationToken();
            await usage2HttpClient.ReportUsageAsync(payload, instanceId, cancellationToken: cancellationToken).ConfigureAwait(true);
          })).ConfigureAwait(true);
          if (policyResult1.Outcome != null)
          {
            tracer.TraceException(policyResult1.FinalException);
            return !(policyResult1.FinalException is AccessCheckException) ? new VssJobResult(TeamFoundationJobExecutionResult.Failed, "[StorageVolumeMeter]: Exception " + JobHelper.GetNestedExceptionMessage(policyResult1.FinalException)) : new VssJobResult(TeamFoundationJobExecutionResult.Blocked, "[StorageVolumeMeter]: Reporting permissions revoked. Exception " + JobHelper.GetNestedExceptionMessage(policyResult1.FinalException));
          }
          IVssRegistryService registryService3 = registryService;
          IVssRequestContext requestContext4 = requestContext;
          registryQuery = (RegistryQuery) "/Configuration/BlobStore/StorageVolumeMeterJob/EnableMaxQtyRetrieval";
          ref RegistryQuery local3 = ref registryQuery;
          if (registryService3.GetValue<bool>(requestContext4, in local3, true, false))
          {
            PolicyResult<MeterUsage2GetResponse> policyResult2 = await ((AsyncPolicy) retryPolicy).ExecuteAndCaptureAsync<MeterUsage2GetResponse>((Func<Task<MeterUsage2GetResponse>>) (async () => await azCommClient.GetMeterUsageAsync(requestContext.ServiceHost.InstanceId, AzCommMeterIds.ArtifactsMeterId).ConfigureAwait(true))).ConfigureAwait(true);
            if (policyResult2.Outcome != null)
            {
              tracer.TraceException(policyResult2.FinalException);
              return new VssJobResult(TeamFoundationJobExecutionResult.Failed, "[StorageVolumeMeter]: Meter retrieval - Exception " + JobHelper.GetNestedExceptionMessage(policyResult2.FinalException));
            }
            jobResult.TotalMaxVolumeInGiB = policyResult2.Result.MaxQuantity;
          }
          this.PushCustomerIntelligenceEvents(requestContext, jobResult);
          jobResult.ReportTime = DateTimeOffset.UtcNow;
          return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, JsonSerializer.Serialize<StorageVolumeMeterJobResult>(jobResult));
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          return new VssJobResult(TeamFoundationJobExecutionResult.Failed, "[StorageVolumeMeter]: Exception " + JobHelper.GetNestedExceptionMessage(ex));
        }
      }
    }

    private void PushCustomerIntelligenceEvents(
      IVssRequestContext requestContext,
      StorageVolumeMeterJobResult jobResult)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.traceData, 5701997, nameof (PushCustomerIntelligenceEvents)))
      {
        try
        {
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          CustomerIntelligenceData intelligenceData1 = new CustomerIntelligenceData();
          intelligenceData1.Add("EventId", jobResult.EventId.ToString("D"));
          intelligenceData1.Add("TotalLogicalVolume", jobResult.TotalLogicalVolumeInGiB.ToString());
          intelligenceData1.Add("TotalPhysicalVolume", jobResult.TotalPhysicalVolumeInGiB.ToString());
          intelligenceData1.Add("MaxVolume", jobResult.TotalMaxVolumeInGiB.ToString());
          intelligenceData1.Add("TotalLogicalFileVolume", jobResult.TotalFileLogicalVolumeInGiB.ToString());
          intelligenceData1.Add("TotalLogicalNonExemptedVolume", jobResult.TotalLogicalVolumeNonExemptedInGiB.ToString());
          intelligenceData1.Add("TotalPhysicalFileVolume", jobResult.TotalFilePhysicalVolumeInGiB.ToString());
          intelligenceData1.Add("TotalLogicalChunkVolume", jobResult.TotalChunkLogicalVolumeInGiB.ToString());
          intelligenceData1.Add("TotalPhysicalChunkVolume", jobResult.TotalChunkPhysicalVolumeInGiB.ToString());
          CustomerIntelligenceData intelligenceData2 = intelligenceData1;
          double num = jobResult.LogicalFileUsageBreakdownInfo.LogicalDropStorageSize;
          string str1 = num.ToString();
          intelligenceData2.Add("LogicalFileDropVolume", str1);
          CustomerIntelligenceData intelligenceData3 = intelligenceData1;
          num = jobResult.LogicalFileUsageBreakdownInfo.LogicalSymbolStorageSize;
          string str2 = num.ToString();
          intelligenceData3.Add("LogicalFileSymbolVolume", str2);
          CustomerIntelligenceData intelligenceData4 = intelligenceData1;
          num = jobResult.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalNuGetStorageSize;
          string str3 = num.ToString();
          intelligenceData4.Add("LogicalFileNugetVolume", str3);
          CustomerIntelligenceData intelligenceData5 = intelligenceData1;
          num = jobResult.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalNpmStorageSize;
          string str4 = num.ToString();
          intelligenceData5.Add("LogicalFileNpmVolume", str4);
          CustomerIntelligenceData intelligenceData6 = intelligenceData1;
          num = jobResult.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalMavenStorageSize;
          string str5 = num.ToString();
          intelligenceData6.Add("LogicalFileMavenVolume", str5);
          CustomerIntelligenceData intelligenceData7 = intelligenceData1;
          num = jobResult.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalPyPiStorageSize;
          string str6 = num.ToString();
          intelligenceData7.Add("LogicalFilePyPiVolume", str6);
          CustomerIntelligenceData intelligenceData8 = intelligenceData1;
          num = jobResult.LogicalFileUsageBreakdownInfo.LogicalFilePackagingUsageBreakdownInfo.LogicalIvyStorageSize;
          string str7 = num.ToString();
          intelligenceData8.Add("LogicalFileIvyVolume", str7);
          CustomerIntelligenceData intelligenceData9 = intelligenceData1;
          num = jobResult.LogicalFileUsageBreakdownInfo.LogicalOthersStorageSize;
          string str8 = num.ToString();
          intelligenceData9.Add("LogicalFileOthersVolume", str8);
          CustomerIntelligenceData intelligenceData10 = intelligenceData1;
          num = jobResult.LogicalChunkUsageBreakdownInfo.LogicalDropStorageSize;
          string str9 = num.ToString();
          intelligenceData10.Add("LogicalChunkDropVolume", str9);
          CustomerIntelligenceData intelligenceData11 = intelligenceData1;
          num = jobResult.LogicalChunkUsageBreakdownInfo.LogicalSymbolStorageSize;
          string str10 = num.ToString();
          intelligenceData11.Add("LogicalChunkSymbolVolume", str10);
          CustomerIntelligenceData intelligenceData12 = intelligenceData1;
          num = jobResult.LogicalChunkUsageBreakdownInfo.LogicalPipelineArtifactStorageSize;
          string str11 = num.ToString();
          intelligenceData12.Add("LogicalChunkPAVolume", str11);
          CustomerIntelligenceData intelligenceData13 = intelligenceData1;
          num = jobResult.LogicalChunkUsageBreakdownInfo.LogicalPipelineCacheStorageSize;
          string str12 = num.ToString();
          intelligenceData13.Add("LogicalChunkPCVolume", str12);
          CustomerIntelligenceData intelligenceData14 = intelligenceData1;
          num = jobResult.LogicalChunkUsageBreakdownInfo.LogicalChunkPackagingUsageBreakdownInfo.LogicalUPackStorageSize;
          string str13 = num.ToString();
          intelligenceData14.Add("LogicalChunkUpackVolume", str13);
          CustomerIntelligenceData intelligenceData15 = intelligenceData1;
          num = jobResult.LogicalChunkUsageBreakdownInfo.LogicalOthersStorageSize;
          string str14 = num.ToString();
          intelligenceData15.Add("LogicalChunkOthersVolume", str14);
          IVssRequestContext requestContext1 = requestContext;
          CustomerIntelligenceData properties = intelligenceData1;
          service.Publish(requestContext1, "BlobStore", "StorageVolumeMeter", properties);
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
        }
      }
    }
  }
}
