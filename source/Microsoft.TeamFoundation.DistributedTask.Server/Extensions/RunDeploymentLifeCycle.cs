// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Extensions.RunDeploymentLifeCycle
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
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Extensions
{
  internal class RunDeploymentLifeCycle : 
    TaskOrchestration<TaskResult, RunDeploymentLifeCycleInput, object, string>
  {
    private readonly TaskCompletionSource<CanceledEvent> m_cancellationSource;
    private List<Task<TaskResult>> m_currentlyRunningHooks;
    private TaskResult m_lifeCycleResult;
    private DeploymentLifeCycleExecutor m_lifeCycleExecutor;
    private int m_jobOrder;

    public RunDeploymentLifeCycle()
    {
      this.m_cancellationSource = new TaskCompletionSource<CanceledEvent>();
      this.m_currentlyRunningHooks = new List<Task<TaskResult>>();
    }

    public Func<Exception, bool> RetryException { get; set; }

    public IJobSchedulerManager JobSchedulerManager { get; set; }

    public ILifeCycleManager LifeCycleManager { get; set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      context.Trace(0, TraceLevel.Info, "RunDeploymentLifeCycle: Received event {0}", (object) name);
      if (!(name == "LifeCycleCanceled"))
        return;
      this.m_lifeCycleResult = TaskResult.Canceled;
      this.m_cancellationSource.TrySetResult((CanceledEvent) input);
    }

    public override async Task<TaskResult> RunTask(
      OrchestrationContext context,
      RunDeploymentLifeCycleInput input)
    {
      this.EnsureExtensions(context);
      context.Trace(0, TraceLevel.Info, "Started life cycle orchestration for phase " + input.Phase.Name + " and cycle " + input.LifeCycleInstanceName);
      this.m_lifeCycleResult = TaskResult.Succeeded;
      this.m_lifeCycleExecutor = new DeploymentLifeCycleExecutor(input.LifeCycleHooks);
      this.m_jobOrder = input.JobOrderStart;
      try
      {
        this.QueueNextLifeCycleHook(context, input);
        while (this.m_currentlyRunningHooks.Count > 0)
        {
          try
          {
            Task<TaskResult> task1 = await Task.WhenAny<TaskResult>((IEnumerable<Task<TaskResult>>) this.m_currentlyRunningHooks);
            if (task1 != null)
            {
              Task<TaskResult> task2 = task1;
              this.m_currentlyRunningHooks.RemoveAt(this.m_currentlyRunningHooks.IndexOf(task2));
              TaskResult hookResult;
              if (task2.IsFaulted)
              {
                hookResult = TaskResult.Failed;
                if (input.Version >= 4)
                  await this.UpdateErrorsInTimeline(context, input, task2.Exception?.InnerException?.Message);
              }
              else
                hookResult = task2.Result;
              context.Trace(0, TraceLevel.Info, string.Format("Notifying LifeCycleExecutor on life cycle hook completion. HookResult: {0}", (object) hookResult));
              this.m_lifeCycleExecutor.OnHookCompleted(hookResult);
              if (!this.m_lifeCycleExecutor.IsDeploymentCompleted())
                this.QueueNextLifeCycleHook(context, input);
            }
          }
          catch (Exception ex)
          {
            context.Trace(0, TraceLevel.Error, "LifeCycle orchestration failed with exception: " + ex.ToString());
            this.m_lifeCycleResult = TaskResult.Failed;
            if (input.Version >= 3)
            {
              await this.UpdateErrorsInTimeline(context, input, ex.Message);
              break;
            }
            break;
          }
        }
        TaskResult? deploymentResult = this.m_lifeCycleExecutor.GetDeploymentResult();
        if (deploymentResult.HasValue)
          this.m_lifeCycleResult = PipelineUtilities.MergeResult(this.m_lifeCycleResult, deploymentResult.Value);
      }
      catch (Exception ex)
      {
        context.Trace(0, TraceLevel.Error, ex.ToString());
        this.m_lifeCycleResult = TaskResult.Failed;
        if (input.Version >= 3)
          await this.UpdateErrorsInTimeline(context, input, ex.Message);
      }
      finally
      {
        context.Trace(0, TraceLevel.Info, string.Format("Completed life cycle orchestration for phase {0} and cycle {1} with result {2}", (object) input.Phase.Name, (object) input.LifeCycleInstanceName, (object) this.m_lifeCycleResult));
      }
      return this.m_lifeCycleResult;
    }

    protected virtual void EnsureExtensions(OrchestrationContext context)
    {
      this.JobSchedulerManager = context.CreateClient<IJobSchedulerManager>(true);
      this.LifeCycleManager = context.CreateClient<ILifeCycleManager>(true);
    }

    protected async Task<TaskResult> ExecuteHookAsync(
      OrchestrationContext context,
      RunDeploymentLifeCycleInput input,
      DeploymentLifeCycleHookBase hook)
    {
      RunDeploymentLifeCycle deploymentLifeCycle1 = this;
      TaskResult jobResult = TaskResult.Succeeded;
      try
      {
        context.Trace(0, TraceLevel.Info, string.Format("Creating sub-orchestration RunDeploymentLifeCycleHook for hook type: {0}", (object) hook.Type));
        RunDeploymentLifeCycleHookInput lifeCycleHookInput1 = LifeCycleHookHelper.GetRunDeploymentLifeCycleHookInput(input, hook);
        string orchestrationId = LifeCycleHookHelper.GetLifeCycleHookOrchestrationId(lifeCycleHookInput1);
        RunDeploymentLifeCycleHookInput lifeCycleHookInput2 = lifeCycleHookInput1;
        RunDeploymentLifeCycle deploymentLifeCycle2 = deploymentLifeCycle1;
        int jobOrder = deploymentLifeCycle1.m_jobOrder;
        int num1 = jobOrder + 1;
        deploymentLifeCycle2.m_jobOrder = num1;
        int num2 = jobOrder;
        lifeCycleHookInput2.Order = num2;
        lifeCycleHookInput1.Version = input.Version;
        Task<TaskResult> runHookTask = context.CreateSubOrchestrationInstance<TaskResult>("RunDeploymentLifeCycleHook", "1.0", orchestrationId, (object) lifeCycleHookInput1);
        if (await Task.WhenAny((Task) deploymentLifeCycle1.m_cancellationSource.Task, (Task) runHookTask) == deploymentLifeCycle1.m_cancellationSource.Task)
        {
          context.Trace(0, TraceLevel.Info, string.Format("Processing cancellation event. Attempting to terminate RunDeploymentLifeCycleHook for hook type: {0}", (object) hook.Type));
          CanceledEvent canceledEvent = deploymentLifeCycle1.m_cancellationSource.Task.Result;
          await deploymentLifeCycle1.ExecuteAsync(context, input, (Func<Task>) (() => this.LifeCycleManager.CancelLifeCycleHookAsync(orchestrationId, canceledEvent)));
        }
        jobResult = await runHookTask;
        runHookTask = (Task<TaskResult>) null;
      }
      catch (SubOrchestrationFailedException ex)
      {
        context.Trace(0, TraceLevel.Error, "Sub orchestration RunDeploymentLifeCycleHook failed with exception: " + ex.ToString());
        jobResult = TaskResult.Failed;
        if (input.Version >= 3)
          await deploymentLifeCycle1.UpdateErrorsInTimeline(context, input, ex.Message);
      }
      return jobResult;
    }

    protected Task<TResult> ExecuteAsync<TResult>(
      OrchestrationContext context,
      RunDeploymentLifeCycleInput input,
      Func<Task<TResult>> operation)
    {
      return context.ExecuteAsync<TResult>(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => RetryHelper.TracePhaseException(context, ex)));
    }

    private void QueueNextLifeCycleHook(
      OrchestrationContext context,
      RunDeploymentLifeCycleInput input)
    {
      DeploymentLifeCycleHookBase nextLifeCycleHook = this.m_lifeCycleExecutor.GetNextLifeCycleHook();
      if (nextLifeCycleHook == null)
        return;
      context.Trace(0, TraceLevel.Info, string.Format("LifeCycleExecutor.GetNextLifeCycleHook returned hook type: {0}", (object) nextLifeCycleHook.Type));
      this.m_currentlyRunningHooks.Add(this.ExecuteHookAsync(context, input, nextLifeCycleHook));
    }

    private async Task UpdateErrorsInTimeline(
      OrchestrationContext context,
      RunDeploymentLifeCycleInput input,
      string errorMessage)
    {
      if (string.IsNullOrWhiteSpace(errorMessage))
        return;
      List<TimelineRecord> timelineRecords = new List<TimelineRecord>()
      {
        this.CreateErrorTimelineRecord(input, errorMessage)
      };
      await this.ExecuteAsync(context, input, (Func<Task>) (() => this.JobSchedulerManager.UpdateTimeline(input.ScopeId, input.PlanType, input.PlanId, (IList<TimelineRecord>) timelineRecords)));
    }

    private TimelineRecord CreateErrorTimelineRecord(
      RunDeploymentLifeCycleInput input,
      string errorMessage)
    {
      DateTime utcNow = DateTime.UtcNow;
      PipelineIdGenerator pipelineIdGenerator = new PipelineIdGenerator();
      string phaseName = string.IsNullOrWhiteSpace(input.Phase.Name) ? "Default" : input.Phase.Name;
      Guid stageInstanceId = PipelineUtilities.GetStageInstanceId((StageInstance) input.Stage.Name);
      Guid phaseInstanceId = PipelineUtilities.GetPhaseInstanceId(input.Stage.Name, phaseName, input.Phase.Attempt);
      return new TimelineRecord()
      {
        Id = phaseInstanceId,
        Name = phaseName,
        StartTime = new DateTime?(utcNow),
        FinishTime = new DateTime?(utcNow),
        RecordType = "Phase",
        State = new TimelineRecordState?(TimelineRecordState.Completed),
        Result = new TaskResult?(TaskResult.Failed),
        ParentId = new Guid?(stageInstanceId),
        RefName = pipelineIdGenerator.GetPhaseInstanceName(input.Stage.Name, phaseName, input.Phase.Attempt),
        Identifier = pipelineIdGenerator.GetPhaseIdentifier(input.Stage.Name, phaseName),
        ErrorCount = new int?(1),
        Issues = {
          new Issue()
          {
            Type = IssueType.Error,
            Message = errorMessage
          }
        }
      };
    }

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunDeploymentLifeCycleInput input,
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
