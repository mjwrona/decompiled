// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.JobQueueController
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler
{
  internal class JobQueueController
  {
    internal Dictionary<IndexerFaultSeverity, TimeSpan> m_delayFactor;
    internal const string JobQueueControllerRegistryPath = "SearchIndexerJobQueueController";
    private BaseIndexerFaultService m_indexerFaultService;
    private IVssRequestContext m_requestContext;
    private JobThrottleStatus m_throttleStatus;
    private int m_criticalSeverityJobDelayInSec;
    private int m_mediumSeverityJobDelayInSec;
    private int m_indexingPipelineBlockedJobDelayInSec;
    private int m_cpuHealthThreshold;
    private int m_cpuHealthJobThrottleCount;
    [StaticSafe("Grandfathered")]
    internal static int s_highCpuJobExecutionCount = 0;
    [StaticSafe("Grandfathered")]
    private static object s_jobStatusLock = new object();
    private bool m_isOnPremisesDeployment;
    private Func<ulong> m_cpuHealthFetcher;
    private const int TracePoint = 1080654;
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "Job";

    public JobQueueController(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_isOnPremisesDeployment = requestContext.ExecutionEnvironment.IsOnPremisesDeployment;
      this.m_cpuHealthFetcher = new Func<ulong>(this.GetCpuUsage);
      this.m_indexerFaultService = (BaseIndexerFaultService) new IndexerV1FaultService(requestContext, (IFaultStore) new RegistryServiceFaultStore(requestContext));
      this.InitializeSettings(requestContext);
    }

    public virtual JobThrottleStatus GetJobThrottleStatus()
    {
      if (!this.m_requestContext.IsFeatureEnabled("Search.Server.FaultManagement"))
        return new JobThrottleStatus(JobRunStatus.Execute);
      IndexerFaultStatus faultStatus = this.m_indexerFaultService.GetFaultStatus();
      if (!this.IsOnPremisesDeployment)
        return this.GetJobRunStatusInternal(faultStatus);
      lock (JobQueueController.s_jobStatusLock)
        return this.GetJobRunStatusInternal(faultStatus);
    }

    public virtual TimeSpan GetQueueDelayFactor()
    {
      if (!this.m_requestContext.IsFeatureEnabled("Search.Server.FaultManagement"))
        return new TimeSpan(0L);
      IndexerFaultStatus faultStatus = this.m_indexerFaultService.GetFaultStatus();
      return faultStatus.Severity == IndexerFaultSeverity.Healthy ? new TimeSpan(0L) : this.m_delayFactor[faultStatus.Severity];
    }

    public virtual bool ShouldRequeue(JobRunStatus status) => status == JobRunStatus.Requeue || status == JobRunStatus.RequeueOnHighCpu;

    public JobThrottleStatus QueryAndUpdateJobCountInFaultState(IndexerFaultSeverity severity)
    {
      JobThrottleStatus jobThrottleStatus = new JobThrottleStatus(JobRunStatus.Execute);
      bool flag = false;
      if (this.IsOnPremisesDeployment)
      {
        flag = this.IsCPUUsageAboveThreshold();
        if (flag && this.GetJobRunStatusWhenCpuThresholdExceeded() == JobRunStatus.RequeueOnHighCpu)
        {
          jobThrottleStatus.RunStatus = JobRunStatus.RequeueOnHighCpu;
          jobThrottleStatus.RequeueDelay = this.m_delayFactor[IndexerFaultSeverity.Medium];
          return jobThrottleStatus;
        }
      }
      TimeSpan delay;
      if (!this.m_indexerFaultService.Request("SearchIndexerJobQueueController", severity, out delay))
      {
        jobThrottleStatus.RunStatus = JobRunStatus.Requeue;
        jobThrottleStatus.RequeueDelay = delay;
      }
      if (jobThrottleStatus.RunStatus == JobRunStatus.Execute & flag)
      {
        this.PreRunUpdateCpuThresholdJobExecutionCount();
        jobThrottleStatus.RunStatus = JobRunStatus.ExecuteOnHighCpu;
      }
      return jobThrottleStatus;
    }

    public void PreRunUpdateCpuThresholdJobExecutionCount()
    {
      ++JobQueueController.s_highCpuJobExecutionCount;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080654, "Indexing Pipeline", "Job", FormattableString.Invariant(FormattableStringFactory.Create("PreRunUpdateCpuThresholdJobExecutionCount - {0}", (object) JobQueueController.s_highCpuJobExecutionCount)));
    }

    public void PostRunUpdateCpuThresholdJobExecutionCount()
    {
      lock (JobQueueController.s_jobStatusLock)
      {
        --JobQueueController.s_highCpuJobExecutionCount;
        if (JobQueueController.s_highCpuJobExecutionCount < 0)
          JobQueueController.s_highCpuJobExecutionCount = 0;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080654, "Indexing Pipeline", "Job", FormattableString.Invariant(FormattableStringFactory.Create("PostRunUpdateCpuThresholdJobExecutionCount - {0}", (object) JobQueueController.s_highCpuJobExecutionCount)));
      }
    }

    public BaseIndexerFaultService FaultService
    {
      get => this.m_indexerFaultService;
      set => this.m_indexerFaultService = value;
    }

    public JobThrottleStatus ThrottleStatus
    {
      get => this.m_throttleStatus;
      set => this.m_throttleStatus = value;
    }

    public bool IsOnPremisesDeployment
    {
      get => this.m_isOnPremisesDeployment;
      set => this.m_isOnPremisesDeployment = value;
    }

    public Func<ulong> CpuHealthFetcher
    {
      get => this.m_cpuHealthFetcher;
      set => this.m_cpuHealthFetcher = value;
    }

    private void InitializeSettings(IVssRequestContext requestContext)
    {
      this.m_criticalSeverityJobDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultCriticalSeverityJobDelayInSec");
      this.m_mediumSeverityJobDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultMediumSeverityJobDelayInSec");
      this.m_indexingPipelineBlockedJobDelayInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/IndexingPipelineBlockedJobDelayInSec");
      this.m_cpuHealthThreshold = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/JobQueueControllerCpuHealthThreshold");
      this.m_cpuHealthJobThrottleCount = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/JobQueueControllerCpuHealthJobThrottleCount");
      this.m_delayFactor = new Dictionary<IndexerFaultSeverity, TimeSpan>()
      {
        {
          IndexerFaultSeverity.Critical,
          new TimeSpan(0, 0, this.m_criticalSeverityJobDelayInSec)
        },
        {
          IndexerFaultSeverity.Medium,
          new TimeSpan(0, 0, this.m_mediumSeverityJobDelayInSec)
        },
        {
          IndexerFaultSeverity.Low,
          new TimeSpan(0L)
        },
        {
          IndexerFaultSeverity.Healthy,
          new TimeSpan(0L)
        },
        {
          IndexerFaultSeverity.Blocked,
          new TimeSpan(0, 0, this.m_indexingPipelineBlockedJobDelayInSec)
        }
      };
    }

    private JobThrottleStatus GetJobRunStatusInternal(IndexerFaultStatus faultStatus)
    {
      this.m_throttleStatus = new JobThrottleStatus(JobRunStatus.Execute);
      if (faultStatus.Severity == IndexerFaultSeverity.Blocked)
      {
        this.m_throttleStatus.RunStatus = JobRunStatus.Requeue;
        this.m_throttleStatus.RequeueDelay = this.m_delayFactor[IndexerFaultSeverity.Blocked];
      }
      else if (faultStatus.Severity == IndexerFaultSeverity.Healthy)
      {
        if (this.IsOnPremisesDeployment && this.IsCPUUsageAboveThreshold())
        {
          if ((this.m_throttleStatus.RunStatus = this.GetJobRunStatusWhenCpuThresholdExceeded()) == JobRunStatus.ExecuteOnHighCpu)
            this.PreRunUpdateCpuThresholdJobExecutionCount();
          else
            this.m_throttleStatus.RequeueDelay = this.m_delayFactor[IndexerFaultSeverity.Medium];
        }
      }
      else
        this.m_throttleStatus = this.QueryAndUpdateJobCountInFaultState(faultStatus.Severity);
      return this.m_throttleStatus;
    }

    private bool IsCPUUsageAboveThreshold()
    {
      try
      {
        return this.CpuHealthFetcher() > (ulong) this.m_cpuHealthThreshold;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    private ulong GetCpuUsage() => (ulong) new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfOS_Processor").Get().Cast<ManagementObject>().Select(mo => new
    {
      Name = mo["Name"],
      Usage = mo["PercentProcessorTime"]
    }).ToList().Where(x => x.Name.ToString() == "_Total").Select(x => x.Usage).SingleOrDefault<object>();

    private JobRunStatus GetJobRunStatusWhenCpuThresholdExceeded()
    {
      JobRunStatus thresholdExceeded = JobQueueController.s_highCpuJobExecutionCount >= this.m_cpuHealthJobThrottleCount ? JobRunStatus.RequeueOnHighCpu : JobRunStatus.ExecuteOnHighCpu;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080654, "Indexing Pipeline", "Job", FormattableString.Invariant(FormattableStringFactory.Create("JobRunStatus evaluated to {0} when CPU utilization is above threshold. Current parallel CPU execution count = {1}", (object) thresholdExceeded, (object) JobQueueController.s_highCpuJobExecutionCount)));
      return thresholdExceeded;
    }
  }
}
