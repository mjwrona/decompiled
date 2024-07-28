// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Extensions.RunDeploymentPhaseBase2
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
  internal abstract class RunDeploymentPhaseBase2 : 
    TaskOrchestration<TaskResult, RunDeploymentPhaseInput2, object, string>
  {
    private readonly TaskCompletionSource<CanceledEvent> m_cancellationSource;
    private Dictionary<Task<TaskResult>, string> m_currentlyRunningLifeCycles;
    private TaskResult m_phaseResult;
    private int m_jobOrder;

    public RunDeploymentPhaseBase2()
    {
      this.m_jobOrder = 1;
      this.m_cancellationSource = new TaskCompletionSource<CanceledEvent>();
      this.m_currentlyRunningLifeCycles = new Dictionary<Task<TaskResult>, string>();
    }

    public Func<Exception, bool> RetryException { get; set; }

    public IJobSchedulerManager JobSchedulerManager { get; set; }

    public ILifeCycleManager LifeCycleManager { get; set; }

    public IDeploymentRequestManager DeploymentRequestManager { get; set; }

    protected abstract IStrategyExecutor2 StrategyExecutor { get; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      context.Trace(0, TraceLevel.Info, "RunDeploymentPhaseBase2: Received event {0}", (object) name);
      if (!(name == "PhaseCanceled"))
        return;
      this.m_phaseResult = TaskResult.Canceled;
      this.m_cancellationSource.TrySetResult((CanceledEvent) input);
    }

    public override async Task<TaskResult> RunTask(
      OrchestrationContext context,
      RunDeploymentPhaseInput2 input)
    {
      this.EnsureExtensions(context);
      context.Trace(0, TraceLevel.Info, string.Format("Started deployment phase orchestration for phase identifier {0}.", (object) input.RequestId));
      this.m_phaseResult = TaskResult.Succeeded;
      try
      {
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.DeploymentRequestManager.DeploymentRequestStarted(input.ProviderPhase.EnvironmentTarget.EnvironmentId, input.RequestId, context.CurrentUtcDateTime)));
        await this.InitializeStrategyExecutor(context, input);
        await this.QueueNextLifeCycles(context, input);
        while (this.m_currentlyRunningLifeCycles.Count > 0)
        {
          try
          {
            Task<TaskResult> key1 = await Task.WhenAny<TaskResult>((IEnumerable<Task<TaskResult>>) this.m_currentlyRunningLifeCycles.Keys);
            if (key1 != null)
            {
              string runningLifeCycle = this.m_currentlyRunningLifeCycles[key1];
              this.m_currentlyRunningLifeCycles.Remove(key1);
              TaskResult result = !key1.IsFaulted ? key1.Result : TaskResult.Failed;
              context.Trace(0, TraceLevel.Info, string.Format("Notifying StrategyExecutor on life cycle complete. CycleInstanceName: {0}, lifeCycleResult: {1}", (object) runningLifeCycle, (object) result));
              Dictionary<int, TaskResult> resourceDeploymentResult;
              this.StrategyExecutor.OnLifeCycleCompleted(runningLifeCycle, result, out resourceDeploymentResult);
              if (resourceDeploymentResult != null)
              {
                foreach (int key2 in resourceDeploymentResult.Keys)
                {
                  int key = key2;
                  await this.ExecuteAsync(context, input, (Func<Task>) (() => this.DeploymentRequestManager.EnvironmentResourceDeploymentRequestCompleted(input.ProviderPhase.EnvironmentTarget.EnvironmentId, input.RequestId, key, context.CurrentUtcDateTime, resourceDeploymentResult[key])));
                }
              }
              if (!this.IsDeploymentCompleted())
                await this.QueueNextLifeCycles(context, input);
            }
          }
          catch (Exception ex)
          {
            context.Trace(0, TraceLevel.Error, ex.ToString());
            this.m_phaseResult = TaskResult.Failed;
            if (input.Version >= 3)
            {
              await this.UpdateErrorsInTimeline(context, input, ex.Message);
              break;
            }
            break;
          }
        }
        IList<TimelineRecord> timelineRecords;
        TaskResult? deploymentResult = this.GetDeploymentResult(out timelineRecords);
        if (deploymentResult.HasValue)
          this.m_phaseResult = PipelineUtilities.MergeResult(this.m_phaseResult, deploymentResult.Value);
        if (timelineRecords != null && timelineRecords.Any<TimelineRecord>())
        {
          foreach (TimelineRecord timelineRecord in (IEnumerable<TimelineRecord>) timelineRecords)
          {
            timelineRecord.StartTime = new DateTime?(context.CurrentUtcDateTime);
            timelineRecord.FinishTime = new DateTime?(context.CurrentUtcDateTime);
          }
          await this.ExecuteAsync(context, input, (Func<Task>) (() => this.JobSchedulerManager.UpdateTimeline(input.ScopeId, input.PlanType, input.PlanId, timelineRecords)));
        }
      }
      catch (Exception ex)
      {
        context.Trace(0, TraceLevel.Error, ex.ToString());
        this.m_phaseResult = TaskResult.Failed;
        if (input.Version >= 3)
          await this.UpdateErrorsInTimeline(context, input, ex.Message);
      }
      finally
      {
        if (this.m_phaseResult == TaskResult.Failed && input.ProviderPhase.ContinueOnError == (ExpressionValue<bool>) true)
          this.m_phaseResult = TaskResult.SucceededWithIssues;
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.DeploymentRequestManager.DeploymentRequestCompleted(input.ProviderPhase.EnvironmentTarget.EnvironmentId, input.RequestId, context.CurrentUtcDateTime, this.m_phaseResult)));
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.JobSchedulerManager.CompletePhaseAsync(input.ScopeId, input.PlanType, input.PlanId, input.Stage.Name, input.Stage.Attempt, input.Phase.Name, input.Phase.Attempt, this.m_phaseResult)));
        context.Trace(0, TraceLevel.Info, string.Format("Completed orchestration with result {0}.", (object) this.m_phaseResult));
      }
      return this.m_phaseResult;
    }

    protected virtual void EnsureExtensions(OrchestrationContext context)
    {
      this.JobSchedulerManager = context.CreateClient<IJobSchedulerManager>(true);
      this.DeploymentRequestManager = context.CreateClient<IDeploymentRequestManager>(true);
      this.LifeCycleManager = context.CreateClient<ILifeCycleManager>(true);
    }

    protected abstract Task InitializeStrategyExecutor(
      OrchestrationContext context,
      RunDeploymentPhaseInput2 input);

    private void ConfigureEnvironmentVariables(
      OrchestrationContext context,
      RunDeploymentPhaseInput2 input,
      RunDeploymentLifeCycleInput lifeCycleInput)
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
          },
          (IVariable) new Variable()
          {
            Name = "Environment.DeploymentPhaseIdentifier",
            Value = context.OrchestrationInstance.InstanceId
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
      if (!(lifeCycleInput.Variables is List<IVariable> variables))
        return;
      variables.AddRange((IEnumerable<IVariable>) collection);
    }

    private async Task QueueNextLifeCycles(
      OrchestrationContext context,
      RunDeploymentPhaseInput2 input)
    {
      List<int> newTargetResourceIds;
      IList<RunDeploymentLifeCycleInput> nextLifeCycles = this.StrategyExecutor.GetNextLifeCycles(input, out newTargetResourceIds);
      context.Trace(0, TraceLevel.Info, string.Format("StrategyExecutor.GetNextLifeCycles returned {0} lifeCycles.", (object) nextLifeCycles.Count));
      foreach (RunDeploymentLifeCycleInput lifeCycleInput in (IEnumerable<RunDeploymentLifeCycleInput>) nextLifeCycles)
      {
        lifeCycleInput.JobOrderStart = this.m_jobOrder;
        this.m_jobOrder += lifeCycleInput.LifeCycleHooks.Count;
        this.ConfigureEnvironmentVariables(context, input, lifeCycleInput);
        this.m_currentlyRunningLifeCycles.Add(this.ExecuteLifeCycleAsync(context, input, lifeCycleInput), lifeCycleInput.LifeCycleInstanceName);
      }
      if (newTargetResourceIds == null)
        ;
      else
      {
        foreach (int num in newTargetResourceIds)
        {
          int resourceId = num;
          await this.ExecuteAsync(context, input, (Func<Task>) (() => this.DeploymentRequestManager.EnvironmentResourceDeploymentRequestQueued(input.ProviderPhase.EnvironmentTarget.EnvironmentId, input.RequestId, resourceId, context.CurrentUtcDateTime)));
        }
      }
    }

    private bool IsDeploymentCompleted() => this.StrategyExecutor.IsDeploymentCompleted();

    private TaskResult? GetDeploymentResult(out IList<TimelineRecord> timelineRecords) => this.StrategyExecutor.GetDeploymentResult(out timelineRecords);

    protected async Task<TaskResult> ExecuteLifeCycleAsync(
      OrchestrationContext context,
      RunDeploymentPhaseInput2 input,
      RunDeploymentLifeCycleInput lifeCycleInput)
    {
      TaskResult taskResult;
      try
      {
        context.Trace(0, TraceLevel.Info, "Creating RunDeploymentLifeCycle sub orchestration for lifeCycleInstance: " + lifeCycleInput.LifeCycleInstanceName + ".");
        string orchestrationId = LifeCycleHookHelper.GetLifeCycleOrchestrationId(lifeCycleInput);
        Task<TaskResult> runCycleTask = context.CreateSubOrchestrationInstance<TaskResult>("RunDeploymentLifeCycle", "1.0", orchestrationId, (object) lifeCycleInput);
        if (await Task.WhenAny((Task) this.m_cancellationSource.Task, (Task) runCycleTask) == this.m_cancellationSource.Task)
        {
          context.Trace(0, TraceLevel.Info, "Processing cancellation event. Attempting to abort lifeCycleInstance: " + lifeCycleInput.LifeCycleInstanceName + ".");
          CanceledEvent canceledEvent = this.m_cancellationSource.Task.Result;
          await this.ExecuteAsync(context, input, (Func<Task>) (() => this.LifeCycleManager.CancelLifeCycleAsync(orchestrationId, canceledEvent)));
        }
        taskResult = await runCycleTask;
        runCycleTask = (Task<TaskResult>) null;
      }
      catch (SubOrchestrationFailedException ex)
      {
        context.Trace(0, TraceLevel.Error, "RunDeploymentLifeCycle orchestration for lifeCycleInstance: " + lifeCycleInput.LifeCycleInstanceName + " failed with error: " + ex.ToString());
        taskResult = TaskResult.Failed;
      }
      return taskResult;
    }

    protected Task<TResult> ExecuteAsync<TResult>(
      OrchestrationContext context,
      RunDeploymentPhaseInput2 input,
      Func<Task<TResult>> operation)
    {
      return context.ExecuteAsync<TResult>(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => RetryHelper.TracePhaseException(context, ex)));
    }

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunDeploymentPhaseInput2 input,
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

    private async Task UpdateErrorsInTimeline(
      OrchestrationContext context,
      RunDeploymentPhaseInput2 input,
      string errorMessage)
    {
      List<TimelineRecord> timelineRecords = new List<TimelineRecord>()
      {
        this.CreateErrorTimelineRecord(input, errorMessage)
      };
      await this.ExecuteAsync(context, input, (Func<Task>) (() => this.JobSchedulerManager.UpdateTimeline(input.ScopeId, input.PlanType, input.PlanId, (IList<TimelineRecord>) timelineRecords)));
    }

    private TimelineRecord CreateErrorTimelineRecord(
      RunDeploymentPhaseInput2 input,
      string errorMessage)
    {
      DateTime utcNow = DateTime.UtcNow;
      string phaseName = string.IsNullOrWhiteSpace(input.Phase.Name) ? "Default" : input.Phase.Name;
      PipelineIdGenerator pipelineIdGenerator = new PipelineIdGenerator();
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
  }
}
