// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Extensions.RunDeploymentPhaseBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Extensions
{
  internal abstract class RunDeploymentPhaseBase : 
    TaskOrchestration<TaskResult, RunDeploymentPhaseInput, object, string>
  {
    private Dictionary<Guid, TaskCompletionSource<JobInstance>> m_inProgressJobs;
    private Dictionary<Guid, TaskCompletionSource<JobStartedEventData>> m_unassignedJobs;
    private Dictionary<Guid, JobInstance> m_jobs;
    private readonly TaskCompletionSource<CanceledEvent> m_cancellationSource;

    public RunDeploymentPhaseBase()
    {
      this.m_inProgressJobs = new Dictionary<Guid, TaskCompletionSource<JobInstance>>();
      this.m_unassignedJobs = new Dictionary<Guid, TaskCompletionSource<JobStartedEventData>>();
      this.m_cancellationSource = new TaskCompletionSource<CanceledEvent>();
      this.m_jobs = new Dictionary<Guid, JobInstance>();
    }

    public Func<Exception, bool> RetryException { get; set; }

    public IJobSchedulerManager JobSchedulerManager { get; set; }

    public IDeploymentRequestManager DeploymentRequestManager { get; set; }

    protected abstract IStrategyExecutor StrategyExecutor { get; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      switch (name)
      {
        case "JobStarted":
          JobStartedEventData result1 = (JobStartedEventData) input;
          TaskCompletionSource<JobStartedEventData> completionSource1;
          if (result1 == null || !this.m_unassignedJobs.TryGetValue(result1.JobId, out completionSource1))
            break;
          completionSource1.TrySetResult(result1);
          break;
        case "JobCompleted":
          JobInstance result2 = (JobInstance) input;
          TaskCompletionSource<JobInstance> completionSource2;
          if (!this.m_inProgressJobs.TryGetValue(result2.Definition.Id, out completionSource2))
            break;
          completionSource2.TrySetResult(result2);
          break;
        case "PhaseCanceled":
          this.m_cancellationSource.TrySetResult((CanceledEvent) input);
          break;
      }
    }

    public override async Task<TaskResult> RunTask(
      OrchestrationContext context,
      RunDeploymentPhaseInput input)
    {
      this.EnsureExtensions(context);
      context.Trace(0, TraceLevel.Info, string.Format("Started deployment phase orchestration for phase identifier {0}.", (object) input.RequestId));
      TaskResult phaseResult = TaskResult.Succeeded;
      List<TimelineRecord> timelineRecordList = new List<TimelineRecord>();
      try
      {
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.DeploymentRequestManager.DeploymentRequestStarted(input.ProviderPhase.EnvironmentTarget.EnvironmentId, input.RequestId, context.CurrentUtcDateTime)));
        await this.InitializeStrategyExecutor(context, input);
        await this.QueueNextJobs(context, input);
        while (this.m_inProgressJobs.Any<KeyValuePair<Guid, TaskCompletionSource<JobInstance>>>())
        {
          IEnumerable<Task> tasks = this.m_unassignedJobs.Values.Select<TaskCompletionSource<JobStartedEventData>, Task>((Func<TaskCompletionSource<JobStartedEventData>, Task>) (t => (Task) t.Task)).Concat<Task>(this.m_inProgressJobs.Values.Select<TaskCompletionSource<JobInstance>, Task>((Func<TaskCompletionSource<JobInstance>, Task>) (t => (Task) t.Task))).Concat<Task>((IEnumerable<Task>) new Task[1]
          {
            (Task) this.m_cancellationSource.Task
          });
          try
          {
            Task task1 = await Task.WhenAny(tasks);
            if (task1 == this.m_cancellationSource.Task)
            {
              phaseResult = TaskResult.Canceled;
              foreach (JobInstance jobInstance in this.m_jobs.Values)
              {
                JobInstance job = jobInstance;
                job.State = PipelineState.Canceling;
                await this.ExecuteAsync(context, input, (Func<Task>) (() => this.JobSchedulerManager.CancelJobAsync(input.ScopeId, input.PlanType, input.PlanId, input.Stage.Name, input.Stage.Attempt, input.Phase.Name, input.Phase.Attempt, job)));
              }
              break;
            }
            if (task1 is Task<JobInstance> task2)
            {
              JobInstance result = task2.Result;
              this.OnJobCompleted(result.Definition.Id, result.Result.Value);
              this.m_inProgressJobs.Remove(result.Definition.Id);
              this.m_jobs.Remove(result.Definition.Id);
            }
            if (task1 is Task<JobStartedEventData> task3)
            {
              JobStartedEventData result = task3.Result;
              this.OnJobStarted(result);
              this.m_unassignedJobs.Remove(result.JobId);
            }
            if (!this.IsDeploymentCompleted())
              await this.QueueNextJobs(context, input);
          }
          catch (Exception ex)
          {
            context.Trace(0, TraceLevel.Error, ex.ToString());
            phaseResult = TaskResult.Failed;
            break;
          }
        }
        IList<TimelineRecord> timelineRecords;
        TaskResult? deploymentResult = this.GetDeploymentResult(out timelineRecords);
        if (deploymentResult.HasValue)
          phaseResult = PipelineUtilities.MergeResult(phaseResult, deploymentResult.Value);
        if (timelineRecords != null && timelineRecords.Any<TimelineRecord>())
          await this.ExecuteAsync(context, input, (Func<Task>) (() => this.JobSchedulerManager.UpdateTimeline(input.ScopeId, input.PlanType, input.PlanId, timelineRecords)));
      }
      catch (Exception ex)
      {
        context.Trace(0, TraceLevel.Error, ex.ToString());
        phaseResult = TaskResult.Failed;
      }
      finally
      {
        if (phaseResult == TaskResult.Failed && input.ProviderPhase.ContinueOnError == (ExpressionValue<bool>) true)
          phaseResult = TaskResult.SucceededWithIssues;
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.DeploymentRequestManager.DeploymentRequestCompleted(input.ProviderPhase.EnvironmentTarget.EnvironmentId, input.RequestId, context.CurrentUtcDateTime, phaseResult)));
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.JobSchedulerManager.CompletePhaseAsync(input.ScopeId, input.PlanType, input.PlanId, input.Stage.Name, input.Stage.Attempt, input.Phase.Name, input.Phase.Attempt, phaseResult)));
        context.Trace(0, TraceLevel.Info, string.Format("Completed orchestration with result {0}.", (object) phaseResult));
      }
      return phaseResult;
    }

    protected virtual void EnsureExtensions(OrchestrationContext context)
    {
      this.JobSchedulerManager = context.CreateClient<IJobSchedulerManager>(true);
      this.DeploymentRequestManager = context.CreateClient<IDeploymentRequestManager>(true);
    }

    protected abstract Task InitializeStrategyExecutor(
      OrchestrationContext context,
      RunDeploymentPhaseInput input);

    private int GetJobTimeOutInMinutes(RunDeploymentPhaseInput input)
    {
      int? nullable = input.ProviderPhase?.Target?.TimeoutInMinutes?.GetValue()?.Value;
      return !nullable.HasValue || nullable.Value < 0 || nullable.Value >= int.MaxValue ? PipelineConstants.DefaultJobTimeoutInMinutes : nullable.Value;
    }

    private int GetJobCancelTimeOutInMinutes(RunDeploymentPhaseInput input)
    {
      int? nullable = input.ProviderPhase?.Target?.CancelTimeoutInMinutes?.GetValue()?.Value;
      return !nullable.HasValue || nullable.Value < 0 || nullable.Value >= int.MaxValue ? PipelineConstants.DefaultJobCancelTimeoutInMinutes : nullable.Value;
    }

    private void ConfigureEnvironmentVariables(RunDeploymentPhaseInput input, JobInstance job)
    {
      List<IVariable> collection = new List<IVariable>();
      if (input.ProviderPhase != null && input.ProviderPhase.EnvironmentTarget != null)
      {
        EnvironmentDeploymentTarget environmentTarget = input.ProviderPhase.EnvironmentTarget;
        string str1 = environmentTarget.EnvironmentId.ToString();
        string environmentName = environmentTarget.EnvironmentName;
        collection.AddRange((IEnumerable<IVariable>) new List<IVariable>()
        {
          (IVariable) new Variable()
          {
            Name = PipelineConstants.EnvironmentVariables.EnvironmentId,
            Value = str1
          },
          (IVariable) new Variable()
          {
            Name = PipelineConstants.EnvironmentVariables.EnvironmentName,
            Value = environmentName
          }
        });
        EnvironmentResourceReference resource = environmentTarget.Resource;
        if (resource != null)
        {
          string str2 = resource.Id.ToString();
          string name = resource.Name;
          collection.AddRange((IEnumerable<IVariable>) new List<IVariable>()
          {
            (IVariable) new Variable()
            {
              Name = PipelineConstants.EnvironmentVariables.EnvironmentResourceId,
              Value = str2
            },
            (IVariable) new Variable()
            {
              Name = PipelineConstants.EnvironmentVariables.EnvironmentResourceName,
              Value = name
            }
          });
        }
      }
      if (!(job.Definition.Variables is List<IVariable> variables))
        return;
      variables.AddRange((IEnumerable<IVariable>) collection);
    }

    private async Task QueueNextJobs(OrchestrationContext context, RunDeploymentPhaseInput input)
    {
      int timeoutInMinutes = this.GetJobTimeOutInMinutes(input);
      int cancelTimeoutInMinutes = this.GetJobCancelTimeOutInMinutes(input);
      IList<JobInstance> nextJobs = this.GetNextJobs();
      if (!nextJobs.Any<JobInstance>())
        ;
      else
      {
        foreach (JobInstance jobInstance1 in (IEnumerable<JobInstance>) nextJobs)
        {
          JobInstance job = jobInstance1;
          job.Definition.TimeoutInMinutes = timeoutInMinutes;
          job.Definition.CancelTimeoutInMinutes = cancelTimeoutInMinutes;
          job.Attempt = input.Phase.Attempt;
          this.ConfigureEnvironmentVariables(input, job);
          Guid jobId = PipelineUtilities.GetJobInstanceId(input.Stage.Name, input.Phase.Name, job.Definition.Name, job.Attempt);
          this.m_inProgressJobs.Add(jobId, new TaskCompletionSource<JobInstance>());
          this.m_unassignedJobs.Add(jobId, new TaskCompletionSource<JobStartedEventData>());
          JobInstance jobInstance2 = await this.ExecuteAsync<JobInstance>(context, input, (Func<Task<JobInstance>>) (() => this.JobSchedulerManager.QueueJobAsync(input.ScopeId, input.PlanType, input.PlanId, input.Stage.Name, input.Stage.Attempt, input.Phase.Name, input.Phase.Attempt, job)));
          this.m_jobs.Add(jobId, jobInstance2);
          jobId = new Guid();
        }
      }
    }

    private IList<JobInstance> GetNextJobs() => this.StrategyExecutor.GetNextJobs();

    private void OnJobStarted(JobStartedEventData OnJobStarted) => this.StrategyExecutor.OnJobStarted(OnJobStarted);

    private void OnJobCompleted(Guid jobId, TaskResult result) => this.StrategyExecutor.OnJobCompleted(jobId, result);

    private bool IsDeploymentCompleted() => this.StrategyExecutor.IsDeploymentComplete();

    private TaskResult? GetDeploymentResult(out IList<TimelineRecord> timelineRecords) => this.StrategyExecutor.GetDeploymentResult(out timelineRecords);

    protected Task<TResult> ExecuteAsync<TResult>(
      OrchestrationContext context,
      RunDeploymentPhaseInput input,
      Func<Task<TResult>> operation)
    {
      return context.ExecuteAsync<TResult>(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => RetryHelper.TracePhaseException(context, ex)));
    }

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunDeploymentPhaseInput input,
      Func<Task> operation)
    {
      Func<Task<object>> operation1 = (Func<Task<object>>) (async () =>
      {
        await operation();
        object obj;
        return obj;
      });
      return (Task) this.ExecuteAsync<object>(context, input, operation1);
    }
  }
}
