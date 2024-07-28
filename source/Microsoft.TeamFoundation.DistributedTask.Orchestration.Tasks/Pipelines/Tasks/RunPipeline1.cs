// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunPipeline1
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public class RunPipeline1 : 
    TaskOrchestration<TaskResult, RunPipelineInput, object, PipelineExecutionState>
  {
    private PipelineExecutionState m_executionState;
    private IDictionary<string, PhaseExecutionState> m_phaseLookup;
    private readonly TaskCompletionSource<CanceledEvent> m_cancellationSource;

    public RunPipeline1() => this.m_cancellationSource = new TaskCompletionSource<CanceledEvent>();

    public IConditionEvaluator Evaluator { get; private set; }

    public IPhaseController PhaseControl { get; private set; }

    public IPipelineLogger Logger { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public List<string> PendingEvents { get; set; }

    public List<string> CompletedEvents { get; set; }

    public override PipelineExecutionState OnGetStatus() => this.m_executionState;

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      if (!(name == "Canceled"))
        return;
      this.m_executionState.State = PipelineState.Canceling;
      this.m_cancellationSource.TrySetResult((CanceledEvent) input);
    }

    public override async Task<TaskResult> RunTask(
      OrchestrationContext context,
      RunPipelineInput input)
    {
      this.EnsureExtensions(context, input.ActivityDispatcherShardsCount, (IActivityShardKey) input.ShardKey);
      this.m_executionState = input.Pipeline;
      this.m_executionState.State = PipelineState.InProgress;
      this.m_executionState.StartTime = new DateTime?(context.CurrentUtcDateTime);
      context.TracePlanStarted(input.ScopeId, input.PlanId);
      string resultCode = "";
      TaskResult result = TaskResult.Failed;
      try
      {
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.PlanStarted(input.ScopeId, input.PlanId, context.CurrentUtcDateTime)));
        try
        {
          result = await this.ExecutePipelineAsync(context, input);
        }
        catch (TaskFailedException ex)
        {
          context.TracePlanException(input.ScopeId, input.PlanId, (Exception) ex);
          result = TaskResult.Failed;
          resultCode = ex.InnerException.GetType().Name;
        }
        catch (Exception ex)
        {
          context.TracePlanException(input.ScopeId, input.PlanId, ex);
          result = TaskResult.Failed;
          resultCode = ex.GetType().Name;
        }
      }
      finally
      {
        if (this.m_executionState.State == PipelineState.Canceling)
          result = TaskResult.Canceled;
        this.m_executionState.Result = new TaskResult?(result);
        this.m_executionState.State = PipelineState.Completed;
        this.m_executionState.FinishTime = new DateTime?(context.CurrentUtcDateTime);
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.PlanCompleted(input.ScopeId, input.PlanId, context.CurrentUtcDateTime, result, resultCode)));
        context.TracePlanCompleted(input.ScopeId, input.PlanId, result);
      }
      return result;
    }

    private async Task<TaskResult> ExecutePipelineAsync(
      OrchestrationContext context,
      RunPipelineInput input)
    {
      List<PhaseExecutionState> remainingPhases = new List<PhaseExecutionState>();
      if (input.Pipeline.Stages.Count == 1 && input.Pipeline.Stages[0].Name.Equals(PipelineConstants.DefaultJobName, StringComparison.OrdinalIgnoreCase))
        remainingPhases.AddRange((IEnumerable<PhaseExecutionState>) input.Pipeline.Stages[0].Phases);
      TaskResult pipelineResult = TaskResult.Succeeded;
      List<Task<TaskResult>> runningPhaseTasks = new List<Task<TaskResult>>();
      List<PhaseExecutionState> runningPhases = new List<PhaseExecutionState>();
      List<PhaseExecutionState> phaseExecutionStateList = new List<PhaseExecutionState>(remainingPhases.Where<PhaseExecutionState>((Func<PhaseExecutionState, bool>) (x => x.Dependencies.Unsatisfied.Count == 0)));
      while (remainingPhases.Count > 0 || runningPhases.Count > 0)
      {
        foreach (PhaseExecutionState phase in phaseExecutionStateList)
        {
          runningPhases.Add(phase);
          remainingPhases.Remove(phase);
          runningPhaseTasks.Add(this.ExecutePhaseAsync(context, input, phase));
        }
        phaseExecutionStateList.Clear();
        List<Task> allTasks = runningPhaseTasks.Cast<Task>().ToList<Task>();
        if (this.m_executionState.State == PipelineState.InProgress)
          allTasks.Add((Task) this.m_cancellationSource.Task);
        int index1 = allTasks.IndexOf(await Task.WhenAny((IEnumerable<Task>) allTasks));
        if (allTasks[index1] != this.m_cancellationSource.Task)
        {
          PhaseExecutionState phaseExecutionState = runningPhases[index1];
          Task<TaskResult> task = runningPhaseTasks[index1];
          runningPhases.RemoveAt(index1);
          runningPhaseTasks.RemoveAt(index1);
          pipelineResult = PipelineUtilities.MergeResult(pipelineResult, task.Result);
          this.PendingEvents.Add(phaseExecutionState.Name);
        }
        else
        {
          List<Task<bool>> conditionResults = new List<Task<bool>>();
          foreach (PhaseExecutionState phase in runningPhases)
            conditionResults.Add(this.EvaluateCondition(context, input, phase));
          bool[] flagArray = await Task.WhenAll<bool>((IEnumerable<Task<bool>>) conditionResults);
          List<string> instanceIds = new List<string>();
          for (int index2 = 0; index2 < runningPhases.Count; ++index2)
          {
            if (!conditionResults[index2].Result)
              instanceIds.Add(RunPipeline1.GetPhaseInstanceId(input.PlanId, runningPhases[index2].Name));
          }
          await this.PhaseControl.Cancel(input.PlanId, (IList<string>) instanceIds, this.m_cancellationSource.Task.Result.Reason);
          conditionResults = (List<Task<bool>>) null;
        }
        phaseExecutionStateList = this.ProcessDependencies(context, (IList<PhaseExecutionState>) remainingPhases);
        allTasks = (List<Task>) null;
      }
      TaskResult taskResult = pipelineResult;
      remainingPhases = (List<PhaseExecutionState>) null;
      runningPhaseTasks = (List<Task<TaskResult>>) null;
      runningPhases = (List<PhaseExecutionState>) null;
      return taskResult;
    }

    private async Task<TaskResult> ExecutePhaseAsync(
      OrchestrationContext context,
      RunPipelineInput input,
      PhaseExecutionState phase)
    {
      if (await this.EvaluateCondition(context, input, phase))
      {
        phase.State = PipelineState.InProgress;
        RunPhaseInput input1 = new RunPhaseInput()
        {
          PlanId = input.PlanId,
          PlanVersion = input.PlanVersion,
          ScopeId = input.ScopeId,
          Phase = phase
        };
        string phaseInstanceId = RunPipeline1.GetPhaseInstanceId(input.PlanId, phase.Name);
        PhaseExecutionState orchestrationInstance;
        switch (phase.Target.Type)
        {
          case PhaseTargetType.Queue:
            orchestrationInstance = await context.CreateSubOrchestrationInstance<PhaseExecutionState>("RunAgentPhase", "1.0", phaseInstanceId, (object) input1);
            break;
          case PhaseTargetType.Server:
            orchestrationInstance = await context.CreateSubOrchestrationInstance<PhaseExecutionState>("RunServerPhase", "1.0", phaseInstanceId, (object) input1);
            break;
          default:
            throw new NotSupportedException();
        }
        phase.CopyFrom(orchestrationInstance);
      }
      else
      {
        phase.State = PipelineState.Completed;
        phase.Result = new TaskResult?(TaskResult.Skipped);
        TimelineRecord phaseRecord = new TimelineRecord()
        {
          Id = PipelineUtilities.GetPhaseInstanceId((string) null, phase.Name, 1, true),
          State = new TimelineRecordState?(TimelineRecordState.Completed),
          Result = new TaskResult?(TaskResult.Skipped)
        };
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.UpdateTimeline(input.ScopeId, input.PlanId, (IList<TimelineRecord>) new TimelineRecord[1]
        {
          phaseRecord
        })));
      }
      return phase.Result.Value;
    }

    private static string GetPhaseInstanceId(Guid planId, string name) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:D}_{1}", (object) planId, (object) name);

    private void EnsureExtensions(
      OrchestrationContext context,
      int activityDispatcherShardsCount,
      IActivityShardKey shardKey)
    {
      this.PendingEvents = new List<string>();
      this.CompletedEvents = new List<string>();
      this.Logger = context.CreateShardedClient<IPipelineLogger>(true, activityDispatcherShardsCount, shardKey);
      this.Evaluator = context.CreateShardedClient<IConditionEvaluator>(true, activityDispatcherShardsCount, shardKey);
      this.PhaseControl = context.CreateShardedClient<IPhaseController>(true, activityDispatcherShardsCount, shardKey);
    }

    private async Task<bool> EvaluateCondition(
      OrchestrationContext context,
      RunPipelineInput input,
      PhaseExecutionState phase)
    {
      if (phase.State == PipelineState.NotStarted && this.m_executionState.State == PipelineState.Canceling && this.m_executionState.Stages[0].Phases.All<PhaseExecutionState>((Func<PhaseExecutionState, bool>) (x =>
      {
        if (x.State == PipelineState.NotStarted)
          return true;
        TaskResult? result = x.Result;
        TaskResult taskResult = TaskResult.Skipped;
        return result.GetValueOrDefault() == taskResult & result.HasValue;
      })))
        return false;
      Guid phaseInstanceId = PipelineUtilities.GetPhaseInstanceId((string) null, phase.Name, 1, true);
      PhaseConditionContext context1 = new PhaseConditionContext(this.m_executionState.State, (string) null, phase.Name, phaseInstanceId);
      if (phase.Dependencies.Satisfied.Count > 0)
      {
        if (this.m_phaseLookup == null)
          this.m_phaseLookup = (IDictionary<string, PhaseExecutionState>) this.m_executionState.Stages[0].Phases.ToDictionary<PhaseExecutionState, string>((Func<PhaseExecutionState, string>) (x => x.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        Queue<string> stringQueue = new Queue<string>((IEnumerable<string>) phase.Dependencies.Satisfied);
        while (stringQueue.Count > 0)
        {
          PhaseExecutionState phaseExecutionState;
          if (this.m_phaseLookup.TryGetValue(stringQueue.Dequeue(), out phaseExecutionState))
          {
            context1.Dependencies[phaseExecutionState.Name] = new PhaseExecutionState()
            {
              Name = phaseExecutionState.Name,
              Result = phaseExecutionState.Result
            };
            foreach (string str in (IEnumerable<string>) phaseExecutionState.Dependencies.Satisfied)
            {
              if (stringSet.Add(str))
                stringQueue.Enqueue(str);
            }
          }
        }
      }
      return await this.Evaluator.EvaluateCondition(input.ScopeId, input.PlanId, phase.Condition, context1);
    }

    private List<PhaseExecutionState> ProcessDependencies(
      OrchestrationContext context,
      IList<PhaseExecutionState> phases)
    {
      List<PhaseExecutionState> phaseExecutionStateList = new List<PhaseExecutionState>();
      foreach (string pendingEvent in this.PendingEvents)
      {
        foreach (PhaseExecutionState phase in (IEnumerable<PhaseExecutionState>) phases)
        {
          if (phase.Dependencies.Unsatisfied.Remove(pendingEvent))
          {
            phase.Dependencies.Satisfied.Add(pendingEvent);
            if (phase.Dependencies.Unsatisfied.Count == 0)
              phaseExecutionStateList.Add(phase);
          }
        }
        this.CompletedEvents.Add(pendingEvent);
      }
      this.PendingEvents.Clear();
      return phaseExecutionStateList;
    }

    protected Task<TResult> ExecuteAsync<TResult>(
      OrchestrationContext context,
      RunPipelineInput input,
      Func<Task<TResult>> operation)
    {
      return context.ExecuteAsync<TResult>(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TracePlanException(input.ScopeId, input.PlanId, ex)));
    }

    protected Task ExecuteAsync(
      OrchestrationContext context,
      RunPipelineInput input,
      Func<Task> operation)
    {
      return context.ExecuteAsync(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TracePlanException(input.ScopeId, input.PlanId, ex)));
    }

    protected async Task LogIssue(
      OrchestrationContext context,
      RunPipelineInput input,
      Guid instanceId,
      IssueType issueType,
      string message)
    {
      try
      {
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.LogIssue(input.ScopeId, input.PlanId, instanceId, context.CurrentUtcDateTime, issueType, message)));
      }
      catch (TaskFailedException ex)
      {
      }
    }
  }
}
