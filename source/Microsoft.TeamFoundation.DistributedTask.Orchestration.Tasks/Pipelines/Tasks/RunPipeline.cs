// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunPipeline
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Checkpoints;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Orchestration;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public class RunPipeline : RunGraph<RunPipelineInput, PipelineExecutionState, StageExecutionState>
  {
    private IStageConditionEvaluator ConditionEvaluator { get; set; }

    protected override Task BeforeExecute(OrchestrationContext context, RunPipelineInput input) => this.Logger.PlanStarted(input.ScopeId, input.PlanId, context.CurrentUtcDateTime);

    protected override Task AfterExecute(
      OrchestrationContext context,
      RunPipelineInput input,
      PipelineExecutionState result)
    {
      return this.Logger.PlanCompleted(input.ScopeId, input.PlanId, context.CurrentUtcDateTime, result.Result.Value, (string) null);
    }

    protected override void Copy(StageExecutionState from, StageExecutionState to) => to.CopyFrom(from);

    protected override void EnsureExtensions(
      OrchestrationContext context,
      int activityDispatcherShardsCount,
      IActivityShardKey shardKey)
    {
      base.EnsureExtensions(context, activityDispatcherShardsCount, shardKey);
      this.ConditionEvaluator = context.CreateShardedClient<IStageConditionEvaluator>(true, activityDispatcherShardsCount, shardKey);
    }

    protected override async Task<bool> EvaluateCondition(
      OrchestrationContext context,
      RunPipelineInput input,
      StageExecutionState child,
      IDictionary<string, StageExecutionState> dependencies)
    {
      if (child.Name.Equals(PipelineConstants.DefaultJobName, StringComparison.OrdinalIgnoreCase))
        return input.Pipeline.State == PipelineState.InProgress;
      StageConditionContext context1 = new StageConditionContext()
      {
        PlanId = input.PlanId,
        ScopeId = input.ScopeId,
        StageName = child.Name,
        StageAttempt = child.Attempt,
        State = input.Pipeline.State
      };
      IDictionary<string, StageExecutionState> dictionary = dependencies;
      if ((dictionary != null ? (dictionary.Count > 0 ? 1 : 0) : 0) != 0)
        context1.Dependencies.AddRange<KeyValuePair<string, StageExecutionState>, IDictionary<string, StageExecutionState>>((IEnumerable<KeyValuePair<string, StageExecutionState>>) dependencies);
      return await this.ConditionEvaluator.EvaluateCondition(context1, child.Condition);
    }

    protected override Task<StageExecutionState> ExecuteNode(
      OrchestrationContext context,
      string instanceId,
      RunPipelineInput input,
      StageExecutionState child,
      IDictionary<string, StageExecutionState> dependencies)
    {
      RunStageInput runStageInput = new RunStageInput();
      runStageInput.ActivityDispatcherShardsCount = input.ActivityDispatcherShardsCount;
      runStageInput.ShardKey = input.ShardKey;
      runStageInput.Pipeline = child;
      runStageInput.PlanId = input.PlanId;
      runStageInput.PlanVersion = input.PlanVersion;
      runStageInput.ScopeId = input.ScopeId;
      RunStageInput input1 = runStageInput;
      return context.CreateSubOrchestrationInstance<StageExecutionState>("RunStage", "1.0", instanceId, (object) input1);
    }

    protected override Guid GetId(RunPipelineInput input, StageExecutionState child) => this.IdGenerator.GetStageInstanceId(child.Name, child.Attempt);

    protected override string GetOrchestrationId(RunPipelineInput input, StageExecutionState child)
    {
      if (input.PlanVersion < 4)
        return string.Format("{0:D}_{1}", (object) input.PlanId, (object) child.Name);
      string lowerInvariant = this.IdGenerator.GetStageInstanceName(child.Name, 1).ToLowerInvariant();
      return string.Format("{0:D}.{1}", (object) input.PlanId, (object) lowerInvariant);
    }

    protected override async Task<IList<IGraphNode>> PrepareRetry(
      OrchestrationContext context,
      RunPipelineInput input,
      RetryEvent retryEvent)
    {
      RunPipeline runPipeline = this;
      if (input.PlanVersion >= 18)
        return await runPipeline.Logger.CreateRetryTimelinesWithAttempts(input.ScopeId, input.PlanId, retryEvent);
      if (input.PlanVersion >= 17)
        await runPipeline.Logger.CreateRetryTimelines(input.ScopeId, input.PlanId, retryEvent);
      return (IList<IGraphNode>) null;
    }

    protected override Task<CheckpointResult> EvaluateChecks(
      OrchestrationContext context,
      RunPipelineInput input,
      StageExecutionState child)
    {
      int attempt = input.PlanVersion >= 9 ? child.Attempt : 1;
      OrchestrationContext context1 = context;
      RunCheckpointInput input1 = new RunCheckpointInput();
      input1.ScopeId = input.ScopeId;
      input1.PlanId = input.PlanId;
      input1.PlanVersion = input.PlanVersion;
      input1.NodeInstanceName = this.IdGenerator.GetStageInstanceName(child.Name, attempt).ToLowerInvariant();
      input1.NodeName = child.Name;
      input1.NodeAttempt = child.Attempt;
      input1.ActivityDispatcherShardsCount = input.ActivityDispatcherShardsCount;
      input1.ShardKey = input.ShardKey;
      int num = input.PlanVersion >= 11 ? 1 : 0;
      return this.RunCheckpoint(context1, input1, num != 0);
    }
  }
}
