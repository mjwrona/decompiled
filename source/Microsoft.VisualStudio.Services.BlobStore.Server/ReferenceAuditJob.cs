// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ReferenceAuditJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Domain;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class ReferenceAuditJob : ArtifactsPartitionedWaitedJob
  {
    private static readonly TraceData TraceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "Job"
    };

    protected override string RegistryBasePath => "/Configuration/BlobStore/ReferenceAuditJob";

    protected override Guid ParentJobId => Guid.Parse("f08ee572-ebcc-4c95-ac02-0fe5e3d95522");

    protected override string JobNamePrefix => "Reference audit child job - ";

    protected override (TraceData traceData, int tracePoint) TraceInfo => (ReferenceAuditJob.TraceData, 5701171);

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
      ReferenceAuditJob referenceAuditJob = this;
      PhysicalDomainInfo physicalDomainInfo = await referenceAuditJob.GetTargetDomainInfo(requestContext, jobParameters, tracer).ConfigureAwait(false);
      ReferenceAuditJobInfo jobInfo = new ReferenceAuditJobInfo()
      {
        MaxParallelism = referenceAuditJob.Settings.MaxParallelism,
        CpuThreshold = referenceAuditJob.Settings.CpuThreshold,
        TotalPartitions = jobParameters.TotalPartitions,
        PartitionId = jobParameters.PartitionId,
        DomainId = jobParameters.DomainId,
        RunId = jobParameters.RunId
      };
      await new ReferenceAuditJobRunner(jobInfo, requestContext, new IteratorPartition(jobParameters.PartitionId, jobParameters.TotalPartitions).SelectIterators<StrongBoxConnectionString>(StorageAccountConfigurationFacade.ReadAllStorageAccounts(requestContext.GetElevatedDeploymentRequestContext(), physicalDomainInfo)), referenceAuditJob.GetResultExporter(requestContext), requestContext.ServiceHost.InstanceId, !referenceAuditJob.IsCPUThrottlingDisabled(requestContext), tracer, requestContext.CancellationToken).RunAsync().ConfigureAwait(true);
      string str = JsonSerializer.Serialize<ReferenceAuditJobInfo>(jobInfo);
      jobInfo = (ReferenceAuditJobInfo) null;
      return str;
    }

    protected virtual IReferenceAuditResultExporter GetResultExporter(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<IReferenceAuditResultExporter>();
    }

    private async Task<PhysicalDomainInfo> GetTargetDomainInfo(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      ReferenceAuditJob referenceAuditJob = this;
      if (!referenceAuditJob.Settings.IsEnabledForMultiDomain)
        return (PhysicalDomainInfo) null;
      IEnumerable<PhysicalDomainInfo> source = await referenceAuditJob.GetDomains(requestContext).ConfigureAwait(false) ?? throw new Exception("Aborting. Multi-domain environment isn't provisioned but the job is enabled for multi-domain.");
      tracer.TraceAlways(string.Format("Discovered {0} domains. Domain Id(s): {1}.", (object) source.Count<PhysicalDomainInfo>(), (object) string.Join<IDomainId>(",", source.Select<PhysicalDomainInfo, IDomainId>((Func<PhysicalDomainInfo, IDomainId>) (dom => dom.DomainId)))));
      return source.SingleOrDefault<PhysicalDomainInfo>((Func<PhysicalDomainInfo, bool>) (dom => string.Equals(dom.DomainId.ToString(), jobParameters.DomainId, StringComparison.OrdinalIgnoreCase))) ?? throw new Exception("Couldn't locate admin domain info for domain id: " + jobParameters.DomainId);
    }

    protected override VssJobResult AggregateChildJobResults(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobHistoryEntry> successfulChildJobs,
      JobParameters jobParameters)
    {
      TeamFoundationJobExecutionResult result = TeamFoundationJobExecutionResult.Succeeded;
      int num = successfulChildJobs.Count<TeamFoundationJobHistoryEntry>();
      if (num == 0)
        return new VssJobResult(TeamFoundationJobExecutionResult.Failed, "None of the child jobs succeeded.");
      if (num < jobParameters.TotalPartitions)
        result = TeamFoundationJobExecutionResult.PartiallySucceeded;
      IEnumerable<ReferenceAuditJobInfo> source = successfulChildJobs.Select<TeamFoundationJobHistoryEntry, ReferenceAuditJobInfo>((Func<TeamFoundationJobHistoryEntry, ReferenceAuditJobInfo>) (r => JsonSerializer.Deserialize<ReferenceAuditJobInfo>(r.ResultMessage)));
      ReferenceAuditJobAggregateInfo jobAggregateInfo = new ReferenceAuditJobAggregateInfo();
      jobAggregateInfo.CpuThreshold = this.Settings.CpuThreshold;
      jobAggregateInfo.TotalPartitions = jobParameters.TotalPartitions;
      jobAggregateInfo.RunId = jobParameters.RunId;
      jobAggregateInfo.DomainId = jobParameters.DomainId;
      jobAggregateInfo.TotalSucceededChildJobs = num;
      jobAggregateInfo.JobRetryCount = source.Sum<ReferenceAuditJobInfo>((Func<ReferenceAuditJobInfo, int>) (r => r.JobRetryCount));
      jobAggregateInfo.ErrorDetails = string.Join<ReferenceAuditJobInfo>("\n", source.Where<ReferenceAuditJobInfo>((Func<ReferenceAuditJobInfo, bool>) (r => !string.IsNullOrWhiteSpace(r.ErrorDetails))));
      ReferenceAuditJobAggregateInfo dataContractObject = jobAggregateInfo;
      foreach (KeyValuePair<string, ReferenceCrawlStatus> keyValuePair in source.SelectMany<ReferenceAuditJobInfo, KeyValuePair<string, ReferenceCrawlStatus>>((Func<ReferenceAuditJobInfo, IEnumerable<KeyValuePair<string, ReferenceCrawlStatus>>>) (r => (IEnumerable<KeyValuePair<string, ReferenceCrawlStatus>>) r.CrawlStatuses)))
        dataContractObject.CrawlStatuses.TryAdd(keyValuePair.Key, keyValuePair.Value);
      return new VssJobResult(result, JsonSerializer.Serialize<ReferenceAuditJobAggregateInfo>(dataContractObject));
    }

    protected override async Task<IEnumerable<PhysicalDomainInfo>> GetDomains(
      IVssRequestContext requestContext)
    {
      return !requestContext.AllowHostDomainAdminOperations() ? (IEnumerable<PhysicalDomainInfo>) null : await requestContext.GetService<AdminHostDomainStoreService>().GetPhysicalDomainsForOrganizationForAdminAsync(requestContext).ConfigureAwait(true);
    }
  }
}
