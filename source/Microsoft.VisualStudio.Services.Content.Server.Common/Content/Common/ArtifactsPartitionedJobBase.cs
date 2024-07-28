// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ArtifactsPartitionedJobBase
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public abstract class ArtifactsPartitionedJobBase : VssAsyncJobExtension
  {
    private readonly IJobParallelismProvider jobParallelismProvider;
    public static readonly int[] DefaultNumStorageAccounts = new int[5]
    {
      2,
      24,
      30,
      60,
      90
    };
    private readonly ConcurrentDictionary<IDomainId, VssJobResult> multiParentDomainJobResult = new ConcurrentDictionary<IDomainId, VssJobResult>();
    protected const int ArtifactsPartitionedJobBaseTraceNumber = 6307671;
    protected const string ArtifactsPartitionedJobBaseTraceLayer = "RunMultiDomainParentJob";

    public ArtifactsPartitionedJobBase() => this.jobParallelismProvider = (IJobParallelismProvider) new RegistryBasedJobParallelismProvider(this.RegistryBasePath);

    protected ArtifactsPartitionedJobSettings Settings { get; private set; }

    protected Stopwatch JobStopwatch { get; private set; }

    protected DateTimeOffset ParentJobStartTime { get; private set; }

    protected abstract Task<string> RunJobAsync(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer);

    protected abstract string RegistryBasePath { get; }

    protected abstract Guid ParentJobId { get; }

    public abstract bool IsValidPartitionSize(
      IVssRequestContext rc,
      int partitionSize,
      int numStorageAccounts);

    protected abstract string JobNamePrefix { get; }

    protected abstract (TraceData traceData, int tracePoint) TraceInfo { get; }

    protected abstract Task<IEnumerable<PhysicalDomainInfo>> GetDomains(
      IVssRequestContext requestContext);

    protected IEnumerable<PhysicalDomainInfo> Domains { get; set; }

    protected virtual bool IsCPUThrottlingDisabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Blobstore.Features.DisableJobsCPUThrottling");

    protected virtual bool ShouldReuseJobResultForInactiveHost => false;

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      this.JobStopwatch = Stopwatch.StartNew();
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.TraceInfo.traceData, this.TraceInfo.tracePoint, nameof (RunAsync)))
      {
        try
        {
          this.Settings = new ArtifactsPartitionedJobSettings(requestContext, this.RegistryBasePath);
          if (this.Settings.ParentAggregationTimeout >= this.Settings.JobExecutionTimeBudget)
            return new VssJobResult(TeamFoundationJobExecutionResult.Failed, "Parent aggregation timeout: " + string.Format("{0}s must be lower than the enforced execution timeout ", (object) this.Settings.ParentAggregationTimeout) + string.Format("{0}: {1}s.", (object) "/EnforcedJobExecutionTimeout", (object) this.Settings.JobExecutionTimeBudget));
          requestContext.RequestTimeout = this.Settings.JobExecutionTimeBudget;
          if (JobHelper.IsJobDisabled(requestContext, this.Settings.JobExecutionState, jobDefinition.JobId))
            return new VssJobResult(TeamFoundationJobExecutionResult.Blocked, string.Format("Job is disabled by registry setting. State: {0}", (object) this.Settings.JobExecutionState));
          if (jobDefinition.JobId != this.ParentJobId)
            return await this.RunChildJobAsync(requestContext, jobDefinition, tracer, this.Settings.IsEnabledForMultiDomain);
          string reusableJobResult;
          if (this.CanOptimizeExecutionForInactiveHosts(requestContext, jobDefinition.JobId, tracer, out reusableJobResult))
          {
            tracer.TraceAlways(string.Format("[{0}]: Current host {1} is inactive. ", (object) nameof (RunAsync), (object) requestContext.ServiceHost.InstanceId) + "Copied previously succeeded job result for current host.");
            return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, reusableJobResult);
          }
          VssJobResult vssJobResult1;
          if (this.Settings.IsEnabledForMultiDomain)
            vssJobResult1 = await this.RunMultiDomainParentJob(requestContext, jobDefinition, tracer, this.Settings.JobSchedulingInterval);
          else
            vssJobResult1 = await this.RunNonMultiDomainParentJob(requestContext, jobDefinition, tracer, this.Settings.JobSchedulingInterval);
          VssJobResult vssJobResult2 = vssJobResult1;
          if (vssJobResult2.Result == TeamFoundationJobExecutionResult.Succeeded)
            requestContext.GetService<IVssRegistryService>().SetValue<DateTime>(requestContext, this.RegistryBasePath + "/LastSucceedJobExecutionTime", DateTime.UtcNow);
          return vssJobResult2;
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          return new VssJobResult(TeamFoundationJobExecutionResult.Failed, "Error: Exception " + JobHelper.GetNestedExceptionMessage(ex));
        }
      }
    }

    private bool CanOptimizeExecutionForInactiveHosts(
      IVssRequestContext requestContext,
      Guid jobId,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      out string reusableJobResult)
    {
      reusableJobResult = string.Empty;
      bool flag = false;
      if (this.IsInactiveHost(requestContext, tracer) && this.ShouldReuseJobResultForInactiveHost && this.HasSuccessfulResultInLookbackWindow(requestContext, tracer))
      {
        TeamFoundationJobHistoryEntry foundationJobHistoryEntry = requestContext.GetService<ITeamFoundationJobService>().QueryLatestJobHistory(requestContext, jobId);
        if (foundationJobHistoryEntry != null && foundationJobHistoryEntry.Result == TeamFoundationJobResult.Succeeded)
        {
          reusableJobResult = foundationJobHistoryEntry.ResultMessage;
          flag = true;
        }
      }
      tracer.TraceAlways(string.Format("[{0}]: Evaluation recommends job reuse: {1}. ", (object) nameof (CanOptimizeExecutionForInactiveHosts), (object) flag) + string.Format("JobId:{0} opted into {1}: {2}.", (object) jobId, (object) "ShouldReuseJobResultForInactiveHost", (object) this.ShouldReuseJobResultForInactiveHost));
      return flag;
    }

    private bool IsJobSpecificAutoFanoutEnabled(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) (this.RegistryBasePath + "/AutoFanout"), true, false);

    private bool HasSuccessfulResultInLookbackWindow(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      DateTime dateTime = requestContext.GetService<IVssRegistryService>().GetValue<DateTime>(requestContext, (RegistryQuery) (this.RegistryBasePath + "/LastSucceedJobExecutionTime"), false, JobConfig.DefaultLastSucceededJobEndTime);
      double totalSeconds = (DateTime.UtcNow - dateTime).TotalSeconds;
      tracer.TraceAlways(string.Format("[{0}]: Last successful job run was {1}s ago. ", (object) nameof (HasSuccessfulResultInLookbackWindow), (object) totalSeconds) + string.Format("Evaluating against default of: {0}s.", (object) 604800));
      return (DateTime.UtcNow - dateTime).TotalSeconds < 604800.0;
    }

    private bool IsInactiveHost(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      DateTime lastUserAccess = requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext, requestContext.ServiceHost.InstanceId).LastUserAccess;
      double totalSeconds = (DateTime.UtcNow - lastUserAccess).TotalSeconds;
      tracer.TraceAlways(string.Format("[{0}]: Host: {1} was last active at: {2}. ", (object) nameof (IsInactiveHost), (object) requestContext.ServiceHost.InstanceId, (object) lastUserAccess) + string.Format("Time since activity: {0}s. ", (object) totalSeconds) + string.Format("Evaluating against inactivity window of: {0}s.", (object) 259200));
      return totalSeconds >= 259200.0;
    }

    private async Task<VssJobResult> RunMultiDomainParentJob(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      TimeSpan jobSchedulingInterval)
    {
      this.Domains = await this.GetDomains(requestContext).ConfigureAwait(true);
      if (this.Domains == null)
        return new VssJobResult(TeamFoundationJobExecutionResult.Failed, "Job cannot execute because multi-domain back-end is not setup.");
      int storageAccountCount = this.GetStorageAccountCountPerDomain(requestContext);
      await requestContext.ForkChildrenAsync<IDomainId, ArtifactsPartitionedJobBase.IHandleParentJobTaskService>(Environment.ProcessorCount, this.Domains.Select<PhysicalDomainInfo, IDomainId>((Func<PhysicalDomainInfo, IDomainId>) (d => d.DomainId)), (Func<IVssRequestContext, IDomainId, Task>) (async (rc, domainId) =>
      {
        int totalJobPartitions = this.GetTotalJobPartitions(rc, domainId, jobDefinition.JobId, storageAccountCount, tracer);
        VssJobResult vssJobResult = await this.RunDomainSpecificParentJobAsync(rc, domainId, this.Domains.Single<PhysicalDomainInfo>((Func<PhysicalDomainInfo, bool>) (d => d.DomainId.Equals(domainId))).Shards.Count, totalJobPartitions, jobDefinition, tracer, jobSchedulingInterval).ConfigureAwait(true);
        if (!this.multiParentDomainJobResult.TryAdd(domainId, vssJobResult))
          throw new InvalidOperationException("Multi-parent job result for dedup-domain: " + domainId.Serialize() + " enabled environment couldn't be recorded.");
      })).ConfigureAwait(true);
      string message = "[" + this.multiParentDomainJobResult.Aggregate<KeyValuePair<IDomainId, VssJobResult>, string>(string.Empty, (Func<string, KeyValuePair<IDomainId, VssJobResult>, string>) ((current, result) => current + ", " + result.Value.Message)).TrimStart(',') + "]";
      return !this.multiParentDomainJobResult.Any<KeyValuePair<IDomainId, VssJobResult>>((Func<KeyValuePair<IDomainId, VssJobResult>, bool>) (r => r.Value.Result == TeamFoundationJobExecutionResult.Blocked)) ? new VssJobResult(!this.multiParentDomainJobResult.All<KeyValuePair<IDomainId, VssJobResult>>((Func<KeyValuePair<IDomainId, VssJobResult>, bool>) (r => r.Value.Result == TeamFoundationJobExecutionResult.Succeeded)) ? (!this.multiParentDomainJobResult.Any<KeyValuePair<IDomainId, VssJobResult>>((Func<KeyValuePair<IDomainId, VssJobResult>, bool>) (r => r.Value.Result == TeamFoundationJobExecutionResult.PartiallySucceeded || r.Value.Result == TeamFoundationJobExecutionResult.Succeeded)) ? TeamFoundationJobExecutionResult.Failed : TeamFoundationJobExecutionResult.PartiallySucceeded) : TeamFoundationJobExecutionResult.Succeeded, message) : new VssJobResult(TeamFoundationJobExecutionResult.Blocked, message);
    }

    private async Task<VssJobResult> RunNonMultiDomainParentJob(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      TimeSpan jobSchedulingInterval)
    {
      int jobPartitions = this.GetTotalJobPartitions(requestContext, WellKnownDomainIds.DefaultDomainId, jobDefinition.JobId, this.Settings.DefaultMultiPartitionSize, tracer);
      JobParameters jobParameters = JobParameters.CreateNew(0, jobPartitions);
      tracer.TraceAlways("Job Started, RunId: {0}, TotalPartitions: {1}, cpuThresholdFullyThrottling: {2}, MaxParallelism: {3}, JobSchedulingDelay: {4}", (object) jobParameters.RunId, (object) jobPartitions, (object) this.Settings.CpuThreshold, (object) this.Settings.MaxParallelism, (object) jobSchedulingInterval);
      if (jobPartitions == 1)
        return await this.YieldAndRunJobAsync(requestContext, jobParameters, tracer);
      if ((this.Settings.DefaultMultiPartitionSize != 0 ? (this.IsValidPartitionSize(requestContext, jobPartitions, this.Settings.DefaultMultiPartitionSize) ? 1 : 0) : (((IEnumerable<int>) ArtifactsPartitionedJobBase.DefaultNumStorageAccounts).Any<int>((Func<int, bool>) (sa => this.IsValidPartitionSize(requestContext, jobPartitions, sa))) ? 1 : 0)) == 0)
        throw new InvalidJobConfigurationException((Exception) new InvalidPartitionSizeException(string.Format("The Job partitions is not valid, received a value of {0}. ", (object) jobPartitions) + string.Format("Multi-partition set to: {0}", (object) this.Settings.DefaultMultiPartitionSize)));
      IEnumerable<Guid> childJobIds = await new MultiJobHelper(jobParameters, jobDefinition.ExtensionName, this.JobNamePrefix, jobSchedulingInterval).ScheduleMultipleJobsAsync(requestContext, tracer, false).ConfigureAwait(true);
      return await this.HandleChildJobsAsync(requestContext, childJobIds, jobParameters, tracer).ConfigureAwait(true);
    }

    private Task<VssJobResult> RunChildJobAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      bool shouldLogDomainId)
    {
      return this.YieldAndRunJobAsync(requestContext, MultiJobHelper.GetPartitionInfo(jobDefinition.Data), tracer, (Action<Microsoft.VisualStudio.Services.Content.Server.Common.Tracer, JobParameters>) ((t, jp) => t.TraceAlways("Child Job Started,  RunId: " + jp.RunId + ", " + string.Format("PartitionId: {0}, ", (object) jp.PartitionId) + (shouldLogDomainId ? "DomainId: " + jp.DomainId + " " : string.Empty) + string.Format("cpuThresholdFullyThrottling: {0}", (object) this.Settings.CpuThreshold))));
    }

    private async Task<VssJobResult> YieldAndRunJobAsync(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      Action<Microsoft.VisualStudio.Services.Content.Server.Common.Tracer, JobParameters> traceAction = null)
    {
      if (!this.IsCPUThrottlingDisabled(requestContext))
      {
        int num = await CpuThrottleHelper.Instance.Yield(this.Settings.CpuThreshold, requestContext.CancellationToken).ConfigureAwait(true);
      }
      Action<Microsoft.VisualStudio.Services.Content.Server.Common.Tracer, JobParameters> action = traceAction;
      if (action != null)
        action(tracer, jobParameters);
      return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, await this.RunJobAsync(requestContext, jobParameters, tracer).ConfigureAwait(true));
    }

    private async Task<VssJobResult> RunDomainSpecificParentJobAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      int shardCount,
      int jobPartitions,
      TeamFoundationJobDefinition jobDefinition,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      TimeSpan jobSchedulingInterval)
    {
      JobParameters jobParameters = JobParameters.CreateNew(0, jobPartitions, domainId);
      tracer.TraceAlways("Job Started, RunId: " + jobParameters.RunId + ", " + string.Format("TotalPartitions: {0}, ", (object) jobPartitions) + string.Format("TotalDomains: {0}, ", (object) this.Domains.Count<PhysicalDomainInfo>()) + "DomainId: " + domainId.Serialize() + " " + string.Format("CpuThreshold: {0}, ", (object) this.Settings.CpuThreshold) + string.Format("MaxParallelism: {0}, ", (object) this.Settings.MaxParallelism) + string.Format("JobSchedulingDelay: {0}", (object) jobSchedulingInterval));
      if (jobPartitions == 1)
        return await this.YieldAndRunJobAsync(requestContext, jobParameters, tracer);
      if (!this.IsValidPartitionSize(requestContext, jobPartitions, this.Settings.DefaultMultiPartitionSize != 0 ? this.Settings.DefaultMultiPartitionSize : shardCount))
        throw new InvalidJobConfigurationException((Exception) new InvalidPartitionSizeException(string.Format("The Job partitions is not valid, received a value of {0} : ", (object) jobPartitions) + string.Format("storage accounts: {0}. ", (object) shardCount) + string.Format("Multi-partition set to: {0}", (object) this.Settings.DefaultMultiPartitionSize)));
      tracer.TraceAlways("RunDomainSpecificParentJobAsync: Parent asynchronous wait is disabled.");
      IEnumerable<Guid> childJobIds = await new MultiJobHelper(jobParameters, jobDefinition.ExtensionName, this.JobNamePrefix, jobSchedulingInterval).ScheduleMultipleJobsAsync(requestContext, tracer, true).ConfigureAwait(true);
      return await this.HandleChildJobsAsync(requestContext, childJobIds, jobParameters, tracer).ConfigureAwait(true);
    }

    private bool IsAutoJobFanoutEvaluationEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Blobstore.Features.EnableAutoJobFanoutEvaluation");

    private bool IsAutoJobFanoutApplicationEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Blobstore.Features.EnableAutoJobFanout");

    private int GetTotalJobPartitions(
      IVssRequestContext requestContext,
      IDomainId domainId,
      Guid jobId,
      int storageAccounts,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      int totalPartitions = this.jobParallelismProvider.GetTotalPartitions(requestContext, domainId, 1);
      bool isFeatureEnabled = this.IsAutoJobFanoutEvaluationEnabled(requestContext);
      bool evaluationJobScalePartitions = this.IsJobSpecificAutoFanoutEnabled(requestContext);
      if (isFeatureEnabled & evaluationJobScalePartitions)
      {
        tracer.TraceAlways(this.FormatMessage("Before job fanout calculation", totalPartitions, isFeatureEnabled, evaluationJobScalePartitions, jobId));
        AutoJobFanoutEngine autoJobFanoutEngine = new AutoJobFanoutEngine(this.RegistryBasePath, this.ParentJobId, this, tracer);
        JobFanoutInfo recommendation = autoJobFanoutEngine.ComputeRecommendation(requestContext, totalPartitions, storageAccounts);
        tracer.TraceAlways(this.FormatMessage("Recommended TotalPartitions", recommendation.RecommendedTotalPartitions, isFeatureEnabled, evaluationJobScalePartitions, jobId) + "JobFanout:  " + JsonSerializer.Serialize<JobFanoutInfo>(recommendation));
        bool flag = this.IsAutoJobFanoutApplicationEnabled(requestContext);
        if (recommendation.IsScaleOutRecommended & flag)
        {
          totalPartitions = recommendation.RecommendedTotalPartitions;
          tracer.TraceAlways("function:GetTotalJobPartitions," + string.Format("After application, auto job scale jobPartitions: {0}, ", (object) totalPartitions) + string.Format("{0}: {1}, ", (object) "EnableAutoJobFanout", (object) flag) + string.Format("JobId :  {0}", (object) jobId));
          autoJobFanoutEngine.SetRecommendation(requestContext, recommendation);
        }
        tracer.TraceAlways(this.FormatMessage("Before job fanout calculation", totalPartitions, isFeatureEnabled, evaluationJobScalePartitions, jobId));
      }
      return totalPartitions;
    }

    private int GetStorageAccountCountPerDomain(IVssRequestContext requestContext) => this.GetDomains(requestContext).Result.First<PhysicalDomainInfo>().Shards.Count;

    private string FormatMessage(
      string prefixMessage,
      int totalPartitions,
      bool isFeatureEnabled,
      bool evaluationJobScalePartitions,
      Guid jobId)
    {
      return string.Format("function: {0}, job scale jobPartitions: {1},", (object) prefixMessage, (object) totalPartitions) + string.Format("JobId :  {0}, ", (object) jobId) + string.Format("{0}: {1}, ", (object) "EnableAutoJobFanoutEvaluation", (object) isFeatureEnabled) + string.Format("{0}: {1}", (object) nameof (evaluationJobScalePartitions), (object) evaluationJobScalePartitions);
    }

    protected virtual Task<VssJobResult> HandleChildJobsAsync(
      IVssRequestContext requestContext,
      IEnumerable<Guid> childJobIds,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      return Task.FromResult<VssJobResult>(new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, JsonSerializer.Serialize(new
      {
        TotalPartitions = jobParameters.TotalPartitions,
        RunId = jobParameters.RunId,
        DomainId = jobParameters.DomainId,
        TotalChildJobs = childJobIds.Count<Guid>()
      })));
    }

    protected virtual void TracePartitionInfo(
      IVssRequestContext requestContext,
      Guid jobId,
      int partitions)
    {
    }

    [DefaultServiceImplementation(typeof (ArtifactsPartitionedJobBase.HandleParentJobTaskService))]
    public interface IHandleParentJobTaskService : IVssTaskService, IVssFrameworkService
    {
    }

    private class HandleParentJobTaskService : 
      VssTaskService,
      ArtifactsPartitionedJobBase.IHandleParentJobTaskService,
      IVssTaskService,
      IVssFrameworkService
    {
      protected override int DefaultThreadCount => 32;

      protected override TimeSpan DefaultTaskTimeout => DefaultThreadPool.DefaultDefaultTaskTimeout;
    }
  }
}
