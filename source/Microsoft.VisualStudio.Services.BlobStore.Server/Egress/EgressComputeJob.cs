// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Egress.EgressComputeJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Egress
{
  public class EgressComputeJob : ArtifactsPartitionedJobBase
  {
    private readonly TraceData TraceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "Job"
    };
    private const EgressConstants.AzureBlobStorageLogs ParseScope = EgressConstants.AzureBlobStorageLogs.Blob;
    private const string RegistryEgressMetricsExport = "/EnableEgressMetricsExport";

    protected override string RegistryBasePath => "/Configuration/BlobStore/EgressComputeJob";

    protected override Guid ParentJobId => EgressConstants.EgressComputeJobId;

    protected override string JobNamePrefix => "Egress compute child job - ";

    protected override (TraceData traceData, int tracePoint) TraceInfo => (this.TraceData, 5701998);

    public override bool IsValidPartitionSize(
      IVssRequestContext rc,
      int partitionSize,
      int numStorageAccounts)
    {
      return numStorageAccounts % partitionSize == 0;
    }

    protected override async Task<string> RunJobAsync(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      EgressComputeJob egressComputeJob = this;
      EgressWorkerResult egressWorkerResult = await egressComputeJob.ComputeEgress(requestContext, jobParameters, tracer).ConfigureAwait(true);
      if (requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) (egressComputeJob.RegistryBasePath + "/EnableEgressMetricsExport"), true, false))
        egressComputeJob.AccumulateResultsAndUploadToDiagnosticStorageAccount(requestContext, egressWorkerResult.ShardMetricMap, jobParameters.PartitionId, tracer);
      return egressWorkerResult.Serialize<EgressWorkerResult>();
    }

    protected override Task<VssJobResult> HandleChildJobsAsync(
      IVssRequestContext requestContext,
      IEnumerable<Guid> childJobIds,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      TeamFoundationJobExecutionResult result = TeamFoundationJobExecutionResult.Succeeded;
      EgressComputeParentJobResult dataContractObject = new EgressComputeParentJobResult()
      {
        CpuThreshold = this.Settings.CpuThreshold,
        TotalPartitions = jobParameters.TotalPartitions,
        RunId = jobParameters.RunId,
        TotalChildJobsQueued = childJobIds.Count<Guid>(),
        PartitionId = jobParameters.PartitionId,
        ChildJobsScheduled = (IEnumerable<string>) childJobIds.Select<Guid, string>((Func<Guid, string>) (job => job.ToString())).ToArray<string>()
      };
      if (!childJobIds.Any<Guid>())
        result = TeamFoundationJobExecutionResult.Failed;
      else if (childJobIds.Count<Guid>() < jobParameters.TotalPartitions)
        result = TeamFoundationJobExecutionResult.PartiallySucceeded;
      return Task.FromResult<VssJobResult>(new VssJobResult(result, JsonSerializer.Serialize<EgressComputeParentJobResult>(dataContractObject)));
    }

    private async Task<EgressWorkerResult> ComputeEgress(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      EgressWorkerResult egressWorkerResult = new EgressWorkerResult()
      {
        JobId = jobParameters.RunId,
        PartitionId = jobParameters.PartitionId,
        LifeSpan = DateTimeOffset.UtcNow.AddMinutes(690.0)
      };
      try
      {
        EgressWorkerResult egressWorkerResult1 = await new EgressWorker(EgressConstants.AzureBlobStorageLogs.Blob, new IteratorPartition(egressWorkerResult.PartitionId, jobParameters.TotalPartitions).SelectIterators<string>(this.PopulateShardList(requestContext)), (IEgressWorkerUtilities) new EgressWorker.EgressWorkerUtilities(), (IStorageAccountAdapter) new EgressWorker.StorageAccountAdapter(), (ICloudAnalyticsClientAdapter) new CloudAnalyticsClientAdapter(), new EgressWorker.WorkingWindow(), tracer).DispatchWorkAsync(requestContext, egressWorkerResult).ConfigureAwait(true);
      }
      catch (Exception ex)
      {
        egressWorkerResult.ExceptionSet.Add("[Egress Compute]: " + JobHelper.GetNestedExceptionMessage(ex));
        throw;
      }
      EgressWorkerResult egress = egressWorkerResult;
      egressWorkerResult = (EgressWorkerResult) null;
      return egress;
    }

    private IEnumerable<string> PopulateShardList(IVssRequestContext requestContext) => StorageAccountConfigurationFacade.ReadAllStorageAccounts(requestContext.To(TeamFoundationHostType.Deployment)).Select<StrongBoxConnectionString, string>((Func<StrongBoxConnectionString, string>) (s => s.ConnectionString));

    private void AccumulateResultsAndUploadToDiagnosticStorageAccount(
      IVssRequestContext requestContext,
      Dictionary<string, Dictionary<string, EgressParserResult>> timeSlotToShardMetric,
      int partitionId,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      Dictionary<string, long> dictionary1 = new Dictionary<string, long>();
      foreach (Dictionary<string, EgressParserResult> dictionary2 in timeSlotToShardMetric.Values)
      {
        foreach (EgressParserResult egressParserResult in dictionary2.Values)
        {
          foreach (KeyValuePair<string, long> keyValuePair in egressParserResult.EgressMetricPerHost)
          {
            string key = keyValuePair.Key;
            if (!dictionary1.ContainsKey(key))
              dictionary1[key] = 0L;
            dictionary1[key] += keyValuePair.Value;
          }
        }
      }
      List<string> uploadPayload = new List<string>();
      foreach (KeyValuePair<string, long> keyValuePair in dictionary1)
        uploadPayload.Add(keyValuePair.Key + ":" + keyValuePair.Value.ToString());
      DateTimeOffset utcNow = DateTimeOffset.UtcNow;
      string blobName = "EgressComputeJob/" + utcNow.ToString("yyyy-MM-d") + "/" + requestContext.ServiceHost.InstanceId.ConvertToAzureCompatibleString() + "_" + partitionId.ToString() + "_" + utcNow.Ticks.ToString();
      requestContext.GetService<IDiagnosticBlobUploader>().TryUploadBlobToDiagnosticStorageAccount(requestContext, tracer, (IEnumerable<string>) uploadPayload, blobName, "artifactservicesdataexport");
    }

    protected override Task<IEnumerable<PhysicalDomainInfo>> GetDomains(
      IVssRequestContext requestContext)
    {
      throw new NotImplementedException();
    }
  }
}
