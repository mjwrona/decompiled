// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunJob`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Checkpoints;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public abstract class RunJob<TInput> : 
    TaskOrchestration<JobExecutionState, TInput, object, JobExecutionState>
  {
    protected JobExecutionState m_executionState;

    public ICheckpointController CheckpointController { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public virtual async Task<bool> EnforcePolicyForJobResources(
      OrchestrationContext context,
      Guid scopeId,
      Guid planId,
      int planVersion,
      JobParameters parameters,
      int activityDispatcherShardsCount,
      PipelineActivityShardKey shardKey,
      int maxAttempts = 5)
    {
      if (planVersion < 10)
        return true;
      this.EnsureExtensions(context, activityDispatcherShardsCount, (IActivityShardKey) shardKey);
      bool decision = true;
      try
      {
        string jobInstanceName = PipelineUtilities.GetJobInstanceName(parameters.StageName, parameters.PhaseName, parameters.Name, parameters.Attempt);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        CheckpointDecision checkpointDecision = await context.ExecuteAsync<CheckpointDecision>((Func<Task<CheckpointDecision>>) (() => this.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.CheckpointController.GetCheckpointDecision(scopeId, planId, jobInstanceName, parameters.StageName, parameters.StageAttempt)), maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceException(ex)));
        if (checkpointDecision.Result == "Denied")
        {
          this.m_executionState.Result = new TaskResult?(TaskResult.Failed);
          this.m_executionState.State = PipelineState.Completed;
          this.m_executionState.Error = new JobError()
          {
            Message = checkpointDecision.Message
          };
          decision = false;
        }
      }
      catch (Exception ex)
      {
        this.m_executionState.Result = new TaskResult?(TaskResult.Failed);
        this.m_executionState.State = PipelineState.Completed;
        this.m_executionState.Error = new JobError()
        {
          Message = ex.Message
        };
        decision = false;
      }
      return decision;
    }

    private void EnsureExtensions(
      OrchestrationContext context,
      int activityDispatcherShardsCount,
      IActivityShardKey shardKey)
    {
      this.CheckpointController = context.CreateShardedClient<ICheckpointController>(true, activityDispatcherShardsCount, shardKey);
    }
  }
}
