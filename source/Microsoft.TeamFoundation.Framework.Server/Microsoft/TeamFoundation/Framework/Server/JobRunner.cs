// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobRunner
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobRunner
  {
    private readonly TeamFoundationJobService m_jobService;
    private readonly IJobDefinitionResolver m_jobDefinitionResolver;
    private readonly IVssDeploymentServiceHost m_deploymentServiceHost;
    private IJobRequestContext m_jobRequestContext;
    private readonly IReadOnlyDictionary<string, Type> m_extensions;
    private bool m_rescheduleJob;
    private Type m_extensionType;
    private string m_jobResultMessage;
    private string m_feature = string.Empty;
    private readonly int m_maxJobResultMessageLength;
    private const string c_assumeUpgradeRunningFeatureFlag = "VisualStudio.JobService.AssumeUpgradeRunning";
    private const string s_area = "JobAgent";
    private const string s_layer = "JobRunner";
    private static double s_microFrequency = (double) Stopwatch.Frequency / 1000000.0;

    public JobRunner(
      IVssDeploymentServiceHost deploymentServiceHost,
      TeamFoundationJobService jobService,
      int maxJobResultMessageLength,
      TeamFoundationJobQueueEntry acquiredJob,
      IJobDefinitionResolver jobDefinitionResolver,
      IReadOnlyDictionary<string, Type> extensions)
    {
      this.m_jobService = jobService;
      this.m_maxJobResultMessageLength = maxJobResultMessageLength;
      this.QueueEntry = acquiredJob;
      this.m_jobDefinitionResolver = jobDefinitionResolver;
      this.m_deploymentServiceHost = deploymentServiceHost;
      this.m_extensions = extensions;
    }

    public void StartExecution(EventHandler<EventArgs> completionEvent)
    {
      ArgumentUtility.CheckForNull<EventHandler<EventArgs>>(completionEvent, nameof (completionEvent));
      this.m_completionEvent = completionEvent;
      this.JobExecutionThread = new Thread(new ThreadStart(this.Main));
      this.JobProgress = JobProgress.StartingThread;
      this.ExecutionStartTime = Stopwatch.GetTimestamp();
      this.JobExecutionThread.Start();
      this.ExecutionStarted = true;
    }

    public void Stop(bool updateQueue, bool rescheduleJob, string reason = null)
    {
      this.m_rescheduleJob |= rescheduleJob;
      try
      {
        if (this.m_jobRequestContext == null)
          return;
        this.m_jobRequestContext.Cancel(reason, updateQueue);
      }
      catch (ObjectDisposedException ex)
      {
      }
    }

    internal TeamFoundationJobQueueEntry QueueEntry { get; private set; }

    internal TeamFoundationJobDefinition JobDefinition { get; private set; }

    private bool IsTemplateJob
    {
      get
      {
        TeamFoundationJobDefinition jobDefinition = this.JobDefinition;
        return jobDefinition != null && jobDefinition.IsTemplateJob;
      }
    }

    private long ExecutionStartTime { get; set; }

    private string ExtensionName { get; set; }

    private string JobName { get; set; }

    public bool ExecutionStarted { get; set; }

    private DateTime ExecutionCompleteTime { get; set; }

    private Thread JobExecutionThread { get; set; }

    private TeamFoundationJobResult? JobResult { get; set; }

    private JobProgress JobProgress { get; set; }

    public ReleaseJobInfo ReleaseJobInfo { get; private set; }

    private string JobResultMessage
    {
      get => this.m_jobResultMessage;
      set
      {
        if (value != null && value.Length > this.m_maxJobResultMessageLength)
        {
          TeamFoundationTracingService.TraceRaw(1441, TraceLevel.Warning, "JobAgent", nameof (JobRunner), "Truncating job result message from {0} characters to {1} characters.", (object) value.Length, (object) this.m_maxJobResultMessageLength);
          TeamFoundationTracingService.TraceRaw(1442, TraceLevel.Info, "JobAgent", nameof (JobRunner), "Before: {0}", (object) value);
          value = value.Substring(0, this.m_maxJobResultMessageLength);
          TeamFoundationTracingService.TraceRaw(1443, TraceLevel.Info, "JobAgent", nameof (JobRunner), "After: {0}", (object) value);
        }
        this.m_jobResultMessage = value;
      }
    }

    private void Main()
    {
      try
      {
        LockHelperContext.SetRequestId(LockHelperContext.NewRequestId());
        try
        {
          using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext())
          {
            try
            {
              this.m_jobRequestContext = this.Initialize(systemContext, this.QueueEntry.JobSource);
              this.Run((IVssRequestContext) this.m_jobRequestContext);
            }
            catch (HostShutdownException ex)
            {
              systemContext.TraceAlways(1422, TraceLevel.Info, "JobAgent", nameof (JobRunner), "Host is stopped, job will be released as dormant, queueEntry={0}, exception={1}", (object) this.QueueEntry, (object) ex);
              this.CompleteJob(TeamFoundationJobResult.HostShutdown, (string) null, true);
            }
            catch (HostDoesNotExistException ex)
            {
              systemContext.TraceException(1427, "JobAgent", nameof (JobRunner), (Exception) ex);
              this.CompleteJob(TeamFoundationJobResult.HostNotFound, (string) null, false);
            }
            finally
            {
              this.DisposeRequestContext((IVssRequestContext) this.m_jobRequestContext);
            }
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(1427, "JobAgent", nameof (JobRunner), ex);
          TeamFoundationEventLog.Default.LogException(FrameworkResources.InitializingJobRunnerError((object) this.QueueEntry), ex);
          this.CompleteJob(TeamFoundationJobResult.JobInitializationError, ex);
        }
        finally
        {
          this.InitializeReleaseJobInfo();
          this.JobProgress = JobProgress.Completed;
          this.m_completionEvent((object) this, EventArgs.Empty);
          this.ExecutionCompleteTime = DateTime.UtcNow;
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(1426, TraceLevel.Error, "JobAgent", nameof (JobRunner), ex);
        TeamFoundationEventLog.Default.LogException(FrameworkResources.UnexpectedJobAgentError(), ex);
        throw;
      }
      finally
      {
        LockHelperContext.ClearRequestId();
      }
    }

    private IJobRequestContext Initialize(IVssRequestContext deploymentRequestContext, Guid hostId)
    {
      IJobRequestContext requestContext = (IJobRequestContext) null;
      try
      {
        HostShutdownException shutdownException = (HostShutdownException) null;
        try
        {
          requestContext = this.CreateJobRequestContext(deploymentRequestContext, hostId, RequestContextType.SystemContext);
        }
        catch (HostShutdownException ex)
        {
          shutdownException = ex;
          requestContext = this.CreateJobRequestContext(deploymentRequestContext, hostId, RequestContextType.ServicingContext);
        }
        this.JobProgress = JobProgress.Initializing;
        this.JobDefinition = this.m_jobDefinitionResolver.GetJobDefinition((IVssRequestContext) requestContext, this.QueueEntry);
        if (this.JobDefinition != null)
        {
          if (this.JobDefinition.UseServicingContext && !requestContext.IsServicingContext)
          {
            this.DisposeRequestContext((IVssRequestContext) requestContext);
            requestContext = (IJobRequestContext) null;
            requestContext = this.CreateJobRequestContext(deploymentRequestContext, hostId, RequestContextType.ServicingContext);
          }
          if (!this.JobDefinition.UseServicingContext && requestContext.IsServicingContext)
          {
            this.DisposeRequestContext((IVssRequestContext) requestContext);
            requestContext = (IJobRequestContext) null;
            throw shutdownException ?? new HostShutdownException(string.Format("Host {0} is not started", (object) hostId));
          }
        }
        return requestContext;
      }
      catch
      {
        this.DisposeRequestContext((IVssRequestContext) requestContext);
        throw;
      }
    }

    private void Run(IVssRequestContext requestContext)
    {
      try
      {
        Thread.CurrentThread.CurrentUICulture = requestContext.ServiceHost.GetCulture(requestContext);
        try
        {
          Thread.CurrentThread.Name = "Job - " + (this.JobDefinition != null ? this.JobDefinition.Name : "[JobDefinition Not Found]");
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1460, "JobAgent", nameof (JobRunner), ex);
        }
        if (this.JobDefinition == null)
        {
          requestContext.Trace(1470, TraceLevel.Error, "JobAgent", nameof (JobRunner), "JobDefinition has been deleted, or invalid JobId was queued: {0}.", (object) this.QueueEntry);
          this.JobResult = new TeamFoundationJobResult?(TeamFoundationJobResult.JobDefinitionNotFound);
        }
        else
        {
          this.ExtensionName = this.JobDefinition.ExtensionName;
          this.JobName = this.JobDefinition.Name;
          this.m_feature = TeamFoundationTracingService.GetJobFeature(requestContext, this.ExtensionName);
          TeamFoundationJobResult result;
          string reason;
          if (this.IsJobDisabled(requestContext, this.JobDefinition, this.QueueEntry, out result, out reason))
          {
            this.JobResult = new TeamFoundationJobResult?(result);
            this.JobResultMessage = reason;
          }
          else if (!this.m_extensions.TryGetValue(this.JobDefinition.ExtensionName, out this.m_extensionType))
          {
            this.JobResult = new TeamFoundationJobResult?(TeamFoundationJobResult.ExtensionNotFound);
          }
          else
          {
            try
            {
              this.TraceJobAgentJobStarted(requestContext);
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(1479, "JobAgent", nameof (JobRunner), ex);
            }
            this.ExecuteJob(requestContext);
            TeamFoundationJobService service = requestContext.GetService<TeamFoundationJobService>();
            if (this.JobDefinition.SelfService)
            {
              if (this.JobDefinition.JobId != this.QueueEntry.JobId)
                throw new TeamFoundationValidationException(FrameworkResources.JobDataIsInvalid(), "JobId");
              this.JobDefinition.Validate(requestContext, "JobDefinition", 0, service.IsIgnoreDormancyPermitted);
            }
            else
            {
              this.JobProgress = JobProgress.RefreshingJobDefinition;
              this.JobDefinition = service.QueryJobDefinition(requestContext, this.QueueEntry.JobId);
            }
            if (this.JobDefinition == null || !this.JobDefinition.RunOnce || this.m_rescheduleJob)
              return;
            this.JobProgress = JobProgress.UpdatingLastExecutionTime;
            service.UpdateLastExecutionTime(requestContext, this.QueueEntry.JobId);
          }
        }
      }
      finally
      {
        try
        {
          this.TraceJobAgentHistory(requestContext);
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(1426, "JobAgent", nameof (JobRunner), ex);
        }
      }
    }

    private IJobRequestContext CreateJobRequestContext(
      IVssRequestContext deploymentRequestContext,
      Guid hostId,
      RequestContextType type)
    {
      IJobRequestContext requestContext = (IJobRequestContext) null;
      deploymentRequestContext.TraceEnter(1461, "JobAgent", nameof (JobRunner), nameof (CreateJobRequestContext));
      try
      {
        requestContext = (IJobRequestContext) deploymentRequestContext.GetService<IInternalTeamFoundationHostManagementService>().BeginRequest(deploymentRequestContext, hostId, type, true, (type != 0 ? 1 : 0) != 0, (IReadOnlyList<IRequestActor>) null, HostRequestType.Job, (object) this.m_jobService, (object) this.QueueEntry);
        requestContext.ServiceHost.BeginRequest((IVssRequestContext) requestContext);
        if (requestContext.IsCanceled || requestContext.CancellationToken.IsCancellationRequested)
          throw new RequestCanceledException(FrameworkResources.RequestToStartJobWasCanceled());
        requestContext.RequestTimeout = TimeSpan.MaxValue;
        return requestContext;
      }
      catch
      {
        this.DisposeRequestContext((IVssRequestContext) requestContext);
        throw;
      }
      finally
      {
        deploymentRequestContext.TraceLeave(1462, "JobAgent", nameof (JobRunner), nameof (CreateJobRequestContext));
      }
    }

    private void DisposeRequestContext(IVssRequestContext requestContext)
    {
      if (requestContext == null)
        return;
      requestContext.ServiceHost.EndRequest(requestContext);
      requestContext.Dispose();
    }

    private void CompleteJob(
      TeamFoundationJobResult result,
      Exception exception = null,
      bool rescheduleJob = true)
    {
      this.CompleteJob(result, exception?.ToString(), rescheduleJob);
    }

    private void CompleteJob(TeamFoundationJobResult result, string message = null, bool rescheduleJob = true)
    {
      this.m_rescheduleJob |= rescheduleJob;
      if (!this.JobResult.HasValue)
      {
        this.JobResult = new TeamFoundationJobResult?(result);
        this.JobResultMessage = message;
      }
      if (this.m_feature == string.Empty)
        this.m_feature = JobFeatureResult.Unset;
      JobRunner.TraceJobAgentHistory(this.QueueEntry, this.JobResult.Value, this.JobResultMessage, this.JobName, this.ExtensionName, this.m_feature);
    }

    private void ExecuteJob(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1465, "JobAgent", nameof (JobRunner), nameof (ExecuteJob));
      try
      {
        this.JobProgress = JobProgress.ExecutingJob;
        string webMethodName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.Run", (object) this.m_extensionType.FullName);
        bool flag = requestContext.IsFeatureEnabled("VisualStudio.Services.HostManagement.CountCpuForJobAsyncCalls");
        TimeSpan timeout = new TimeSpan();
        int num = flag ? 1 : 0;
        MethodInformation methodInformation = new MethodInformation(webMethodName, MethodType.SystemTask, EstimatedMethodCost.Free, false, timeout: timeout, captureAsyncResourcesUsage: num != 0);
        requestContext.EnterMethod(methodInformation);
        try
        {
          string resultMessage = (string) null;
          ITeamFoundationJobExtension instance = (ITeamFoundationJobExtension) Activator.CreateInstance(this.m_extensionType);
          try
          {
            requestContext.Trace(1420, TraceLevel.Info, "JobAgent", nameof (JobRunner), "ExecuteJob: {0}", (object) this.ToString());
            this.JobResult = new TeamFoundationJobResult?((TeamFoundationJobResult) instance.Run(requestContext, this.JobDefinition, this.QueueEntry.QueueTime, out resultMessage));
          }
          finally
          {
            this.JobResultMessage = resultMessage;
            if (instance is IDisposable disposable)
              disposable.Dispose();
          }
        }
        catch (ThreadAbortException ex)
        {
          requestContext.TraceException(1421, "JobAgent", nameof (JobRunner), (Exception) ex);
          this.JobResult = new TeamFoundationJobResult?(TeamFoundationJobResult.Killed);
          this.JobResultMessage = FrameworkResources.ExtensionThrewException((object) ex);
        }
        catch (HostShutdownException ex)
        {
          requestContext.TraceException(1422, "JobAgent", nameof (JobRunner), (Exception) ex);
          this.JobResult = new TeamFoundationJobResult?(TeamFoundationJobResult.Stopped);
        }
        catch (RequestCanceledException ex)
        {
          requestContext.TraceException(1423, "JobAgent", nameof (JobRunner), (Exception) ex);
          this.JobResult = new TeamFoundationJobResult?(TeamFoundationJobResult.Stopped);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1424, "JobAgent", nameof (JobRunner), ex);
          this.JobResult = new TeamFoundationJobResult?(TeamFoundationJobResult.Failed);
          this.JobResultMessage = FrameworkResources.ExtensionThrewException((object) ex);
        }
        finally
        {
          requestContext.LeaveMethod();
        }
      }
      finally
      {
        requestContext.TraceLeave(1466, "JobAgent", nameof (JobRunner), nameof (ExecuteJob));
      }
    }

    private void InitializeReleaseJobInfo()
    {
      TeamFoundationJobResult jobResult = (TeamFoundationJobResult) ((int) this.JobResult ?? 7);
      JobPriorityClass priorityClass = this.JobDefinition != null ? this.JobDefinition.PriorityClass : JobPriorityClass.Normal;
      bool jobDefinitionExists;
      int scheduleSeconds;
      if (this.m_rescheduleJob)
      {
        jobDefinitionExists = true;
        scheduleSeconds = 300;
      }
      else
      {
        jobDefinitionExists = this.JobDefinition != null;
        scheduleSeconds = -1;
      }
      int maxValue = this.JobDefinition == null || !this.JobDefinition.IgnoreDormancy ? 0 : int.MaxValue;
      this.ReleaseJobInfo = new ReleaseJobInfo(this.QueueEntry.JobSource, this.QueueEntry.JobId, jobResult, this.JobResultMessage, jobDefinitionExists, priorityClass, scheduleSeconds, maxValue, this.IsTemplateJob);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "[JobSource: {0}, JobId: {1}, QueueTime: {2}, Name: {3}, Extension: {4}, Thread state: {5}, Job state: {6}, Result: {7}, Result Message: {8}, Start Time: {9}, Complete Time: {10}]", (object) this.QueueEntry?.JobSource, (object) this.QueueEntry?.JobId, (object) this.QueueEntry?.QueueTime, (object) this.JobName, (object) this.ExtensionName, (object) this.JobExecutionThread?.ThreadState, (object) this.JobProgress, (object) this.JobResult, (object) this.JobResultMessage, (object) this.ExecutionStartTime, (object) this.ExecutionCompleteTime);

    internal string GetStringForExceptionMessage() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "[JobId: {0}, Name: {1}, Extension: {2}]", (object) this.QueueEntry?.JobId, (object) this.JobName, (object) this.ExtensionName);

    private bool IsJobDisabled(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      TeamFoundationJobQueueEntry queueEntry,
      out TeamFoundationJobResult result,
      out string reason)
    {
      if (jobDefinition.EnabledState == TeamFoundationJobEnabledState.FullyDisabled)
      {
        result = TeamFoundationJobResult.Disabled;
        reason = "Job is disabled";
        return true;
      }
      if (JobServiceUtil.IsServiceHostIdle(requestContext))
      {
        result = TeamFoundationJobResult.Disabled;
        reason = "ServiceHost is idle";
        return true;
      }
      if ((queueEntry.QueuedReasons & ~(TeamFoundationJobQueuedReasons.Scheduled | TeamFoundationJobQueuedReasons.PreviousExecutionResult)) == TeamFoundationJobQueuedReasons.None)
      {
        if (jobDefinition.EnabledState == TeamFoundationJobEnabledState.SchedulesDisabled)
        {
          result = TeamFoundationJobResult.Disabled;
          reason = "Job schedules are disabled";
          return true;
        }
        if (jobDefinition.DisableDuringUpgrade)
        {
          if (requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.JobService.AssumeUpgradeRunning"))
          {
            result = TeamFoundationJobResult.BlockedByUpgrade;
            reason = "AssumeUpgradeRunning feature is set";
            return true;
          }
          if (this.IsUpgradeRunning(requestContext) || this.IsDataImportHostUpgradeRunning(requestContext))
          {
            result = TeamFoundationJobResult.BlockedByUpgrade;
            reason = "Upgrade is pending or running";
            return true;
          }
        }
      }
      result = TeamFoundationJobResult.None;
      reason = (string) null;
      return false;
    }

    private bool IsDataImportHostUpgradeRunning(IVssRequestContext requestContext) => requestContext.ServiceHost.ServiceHostInternal().SubStatus == ServiceHostSubStatus.UpgradeDuringImport;

    private bool IsUpgradeRunning(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationResourceManagementService>().GetVerifyServiceVersion(requestContext, "Default");

    private void TraceJobAgentJobStarted(IVssRequestContext requestContext) => requestContext.RequestTracer.RequestTracerInternal().TracingService.TraceJobAgentJobStarted(requestContext, this.ExtensionName ?? string.Empty, this.JobName ?? string.Empty, requestContext.ServiceHost.InstanceId, this.QueueEntry.JobId, this.QueueEntry.QueueTime, this.QueueEntry.ExecutionStartTime, this.QueueEntry.AgentId, this.QueueEntry.QueuedReasonsValue, this.QueueEntry.QueueFlagsValue | (this.IsTemplateJob ? 2 : 0), (short) this.QueueEntry.Priority, requestContext.E2EId, this.QueueEntry.RequesterActivityId, this.QueueEntry.RequesterVsid);

    private void TraceJobAgentHistory(IVssRequestContext requestContext)
    {
      TeamFoundationJobResult result = (TeamFoundationJobResult) ((int) this.JobResult ?? 9);
      string message = this.JobResultMessage;
      ILogRequestInternal logRequestInternal = requestContext.RequestLogger.RequestLoggerInternal();
      if (string.IsNullOrWhiteSpace(message) && requestContext.Status != null)
        message = requestContext.Status.Message;
      WellKnownPerformanceTimings performanceTimings = PerformanceTimer.GetWellKnownParsedPerformanceTimings(requestContext);
      requestContext.RequestTracer.RequestTracerInternal().TracingService.TraceJobAgentHistory(requestContext, this.ExtensionName ?? string.Empty, this.JobName ?? string.Empty, requestContext.ServiceHost.InstanceId, this.QueueEntry.JobId, this.QueueEntry.QueueTime, this.QueueEntry.ExecutionStartTime, JobRunner.GetExecutionTime(this.ExecutionStartTime), this.QueueEntry.AgentId, (int) result, message, this.QueueEntry.QueuedReasonsValue, this.QueueEntry.QueueFlagsValue | (this.IsTemplateJob ? 2 : 0), (short) this.QueueEntry.Priority, logRequestInternal.LogicalReads, logRequestInternal.PhysicalReads, logRequestInternal.CpuTime, logRequestInternal.ElapsedTime, performanceTimings.SqlExecutionTime, performanceTimings.SqlExecutionCount, performanceTimings.RedisExecutionTime, performanceTimings.RedisExecutionCount, performanceTimings.AadGraphExecutionTime, performanceTimings.AadGraphExecutionCount, performanceTimings.AadTokenExecutionTime, performanceTimings.AadTokenExecutionCount, performanceTimings.BlobStorageExecutionTime, performanceTimings.BlobStorageExecutionCount, performanceTimings.TableStorageExecutionTime, performanceTimings.TableStorageExecutionCount, performanceTimings.ServiceBusExecutionTime, performanceTimings.ServiceBusExecutionCount, performanceTimings.VssClientExecutionTime, performanceTimings.VssClientExecutionCount, performanceTimings.SqlRetryExecutionTime, performanceTimings.SqlRetryExecutionCount, performanceTimings.SqlReadOnlyExecutionTime, performanceTimings.SqlReadOnlyExecutionCount, requestContext.CPUCycles, performanceTimings.FinalSqlCommandExecutionTime, requestContext.E2EId, performanceTimings.DocDBExecutionTime, performanceTimings.DocDBExecutionCount, performanceTimings.DocDBRUsConsumed, requestContext.AllocatedBytes, this.QueueEntry.RequesterActivityId, this.QueueEntry.RequesterVsid, requestContext.CPUCyclesAsync, requestContext.AllocatedBytesAsync);
    }

    internal static void TraceJobAgentHistory(
      TeamFoundationJobQueueEntry queueEntry,
      TeamFoundationJobResult jobResult,
      string jobResultMessage = null,
      string jobName = null,
      string extensionName = null,
      string feature = null)
    {
      TeamFoundationTracingService.TraceJobAgentHistoryRaw(extensionName ?? string.Empty, jobName ?? string.Empty, queueEntry.JobSource, queueEntry.JobId, queueEntry.QueueTime, queueEntry.ExecutionStartTime, Math.Max(JobRunner.ToMicroseconds(DateTime.UtcNow - queueEntry.ExecutionStartTime), 0L), queueEntry.AgentId, (int) jobResult, jobResultMessage, queueEntry.QueuedReasonsValue, queueEntry.QueueFlagsValue, (short) queueEntry.Priority, 0, 0, 0, 0, feature ?? JobFeatureResult.Unset, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0L, 0, Guid.Empty, 0, 0, 0, 0L, queueEntry.RequesterActivityId, queueEntry.RequesterVsid, 0L, 0L);
    }

    private static long ToMicroseconds(TimeSpan time) => (long) (time.TotalMilliseconds * 1000.0);

    private static long GetExecutionTime(long StartTime) => (long) ((double) (Stopwatch.GetTimestamp() - StartTime) / JobRunner.s_microFrequency);

    private event EventHandler<EventArgs> m_completionEvent;
  }
}
