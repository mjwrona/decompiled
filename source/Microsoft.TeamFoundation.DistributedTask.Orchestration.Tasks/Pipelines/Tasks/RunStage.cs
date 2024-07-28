// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunStage
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Orchestration;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public sealed class RunStage : RunGraph<RunStageInput, StageExecutionState, PhaseExecutionState>
  {
    private IConditionEvaluator ConditionEvaluator { get; set; }

    protected override void Copy(PhaseExecutionState from, PhaseExecutionState to) => to.CopyFrom(from);

    protected override void EnsureExtensions(
      OrchestrationContext context,
      int activityDispatcherShardsCount,
      IActivityShardKey shardKey)
    {
      base.EnsureExtensions(context, activityDispatcherShardsCount, shardKey);
      this.ConditionEvaluator = context.CreateShardedClient<IConditionEvaluator>(true, activityDispatcherShardsCount, shardKey);
    }

    protected override Task<bool> EvaluateCondition(
      OrchestrationContext context,
      RunStageInput input,
      PhaseExecutionState phase,
      IDictionary<string, PhaseExecutionState> dependencies)
    {
      PhaseConditionContext context1 = new PhaseConditionContext()
      {
        PlanId = input.PlanId,
        ScopeId = input.ScopeId,
        StageName = input.Pipeline.Name,
        StageAttempt = input.Pipeline.Attempt,
        PhaseName = phase.Name,
        PhaseAttempt = phase.Attempt,
        State = input.Pipeline.State
      };
      if (dependencies != null && dependencies.Count > 0)
        context1.Dependencies.AddRange<KeyValuePair<string, PhaseExecutionState>, IDictionary<string, PhaseExecutionState>>((IEnumerable<KeyValuePair<string, PhaseExecutionState>>) dependencies);
      return this.ConditionEvaluator.EvaluateCondition(input.ScopeId, input.PlanId, phase.Condition, context1);
    }

    protected override Task<PhaseExecutionState> ExecuteNode(
      OrchestrationContext context,
      string instanceId,
      RunStageInput input,
      PhaseExecutionState node,
      IDictionary<string, PhaseExecutionState> dependencies)
    {
      RunPhaseInput input1 = new RunPhaseInput()
      {
        ActivityDispatcherShardsCount = input.ActivityDispatcherShardsCount,
        ShardKey = input.ShardKey,
        Phase = node,
        PlanId = input.PlanId,
        PlanVersion = input.PlanVersion,
        ScopeId = input.ScopeId,
        StageName = input.Pipeline.Name,
        StageAttempt = input.Pipeline.Attempt
      };
      if (dependencies != null && dependencies.Count > 0)
        input1.DependsOn.AddRange<KeyValuePair<string, PhaseExecutionState>, IDictionary<string, PhaseExecutionState>>((IEnumerable<KeyValuePair<string, PhaseExecutionState>>) dependencies);
      if (!string.IsNullOrEmpty(node.Provider))
        return context.CreateSubOrchestrationInstance<PhaseExecutionState>("RunProviderPhase", "1.0", instanceId, (object) input1);
      switch (node.Target.Type)
      {
        case PhaseTargetType.Queue:
          return context.CreateSubOrchestrationInstance<PhaseExecutionState>("RunAgentPhase", "1.0", instanceId, (object) input1);
        case PhaseTargetType.Server:
          return context.CreateSubOrchestrationInstance<PhaseExecutionState>("RunServerPhase", "1.0", instanceId, (object) input1);
        default:
          throw new NotSupportedException(node.Target.Type.ToString());
      }
    }

    protected override async Task BeforeExecute(OrchestrationContext context, RunStageInput input)
    {
      RunStage runStage = this;
      if (input.PlanVersion < 15)
        return;
      Guid stageInstanceId = new PipelineIdGenerator(input.PlanVersion < 4).GetStageInstanceId(input.Pipeline.Name, input.Pipeline.Attempt);
      if (string.Equals(input.Pipeline.Name, PipelineConstants.DefaultJobName, StringComparison.OrdinalIgnoreCase))
        return;
      TimelineRecord stageRecord = new TimelineRecord()
      {
        Id = stageInstanceId,
        State = new TimelineRecordState?(TimelineRecordState.InProgress)
      };
      await runStage.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.UpdateTimeline(input.ScopeId, input.PlanId, (IList<TimelineRecord>) new TimelineRecord[1]
      {
        stageRecord
      })));
    }

    protected override async Task AfterExecute(
      OrchestrationContext context,
      RunStageInput input,
      StageExecutionState result)
    {
      RunStage runStage = this;
      Guid stageId = new PipelineIdGenerator(input.PlanVersion < 4).GetStageInstanceId(input.Pipeline.Name, input.Pipeline.Attempt);
      if (!string.Equals(input.Pipeline.Name, PipelineConstants.DefaultJobName, StringComparison.OrdinalIgnoreCase))
      {
        TimelineRecord timelineRecord = new TimelineRecord();
        timelineRecord.Id = stageId;
        timelineRecord.State = new TimelineRecordState?(TimelineRecordState.Completed);
        TaskResult? result1 = result.Result;
        TaskResult taskResult = TaskResult.Skipped;
        timelineRecord.FinishTime = result1.GetValueOrDefault() == taskResult & result1.HasValue ? new DateTime?() : result.FinishTime;
        timelineRecord.Result = result.Result;
        TimelineRecord stageRecord = timelineRecord;
        await runStage.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.UpdateTimeline(input.ScopeId, input.PlanId, (IList<TimelineRecord>) new TimelineRecord[1]
        {
          stageRecord
        })));
      }
      if (input.PlanVersion < 13)
        ;
      else
        await runStage.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.StageCompleted(input.ScopeId, input.PlanId, stageId, result.Result)));
    }

    protected override Guid GetId(RunStageInput input, PhaseExecutionState child) => this.IdGenerator.GetPhaseInstanceId(input.Pipeline.Name, child.Name, child.Attempt);

    protected override string GetOrchestrationId(RunStageInput input, PhaseExecutionState child)
    {
      string str = input.PlanId.ToString("D");
      string orchestrationId;
      if (input.PlanVersion < 4)
      {
        orchestrationId = str + input.Pipeline.Name + "_" + child.Name;
      }
      else
      {
        string lowerInvariant = this.IdGenerator.GetPhaseInstanceName(input.Pipeline.Name, child.Name, 1).ToLowerInvariant();
        orchestrationId = str + "." + lowerInvariant;
      }
      return orchestrationId;
    }
  }
}
