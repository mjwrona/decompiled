// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunCheckpoint2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Checkpoints;
using Microsoft.VisualStudio.Services.Orchestration;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public class RunCheckpoint2 : TaskOrchestration<CheckpointResult, RunCheckpointInput, object, bool>
  {
    private CheckpointContext m_checkpointContext;
    private TaskCompletionSource<CheckpointDecision> m_checkpointSource = new TaskCompletionSource<CheckpointDecision>();
    private TaskCompletionSource<CanceledEvent> m_cancellationSource = new TaskCompletionSource<CanceledEvent>();
    private const string c_teamName = "Checks";

    public Func<Exception, bool> RetryException { get; set; }

    public bool EnableTimeout { get; set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      switch (name)
      {
        case "CheckpointEvent":
          if (input is CheckpointDecision result)
          {
            Guid? id1 = this.m_checkpointContext?.Id;
            Guid id2 = result.Id;
            if ((id1.HasValue ? (id1.HasValue ? (id1.GetValueOrDefault() != id2 ? 1 : 0) : 0) : 1) != 0)
            {
              context.TraceError(string.Format("Received unexpected {0} with Id: {1}", (object) "CheckpointDecision", (object) result.Id));
              break;
            }
            this.m_checkpointSource.TrySetResult(result);
            break;
          }
          context.TraceError("Received incorrect input type for event name '" + name + "'. Expected: '" + typeof (CheckpointDecision).FullName + "', actual: '" + input?.GetType().FullName + "'");
          break;
        case "Canceled":
          this.m_cancellationSource.TrySetResult(input as CanceledEvent);
          break;
      }
    }

    protected Task<TResult> ExecuteAsync<TResult>(
      OrchestrationContext context,
      Func<Task<TResult>> operation)
    {
      return context.ExecuteAsync<TResult>(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceException(ex)));
    }

    protected Task ExecuteAsync(OrchestrationContext context, Func<Task> operation) => context.ExecuteAsync(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceException(ex)));

    public override async Task<CheckpointResult> RunTask(
      OrchestrationContext context,
      RunCheckpointInput input)
    {
      ICheckpointController checkpointController = context.CreateShardedClient<ICheckpointController>(true, input.ActivityDispatcherShardsCount, (IActivityShardKey) input.ShardKey);
      context.TraceStartLinearOrchestration();
      context.TraceStartLinearPhase("Checks", "CreateCheckpoint");
      this.m_checkpointContext = await this.ExecuteAsync<CheckpointContext>(context, (Func<Task<CheckpointContext>>) (() => checkpointController.Create(input.ScopeId, input.PlanId, context.OrchestrationInstance.InstanceId, input.NodeInstanceName, input.NodeName, input.NodeAttempt)));
      if (this.m_checkpointContext == null)
      {
        context.TraceCompleteLinearOrchestration("Checks", "CheckpointComplete");
        return CheckpointResult.Approved;
      }
      context.TraceStartLinearPhase("Checks", "AddCheckpoint");
      CheckpointDecision decision = await checkpointController.Add(input.ScopeId, input.PlanId, this.m_checkpointContext);
      if (decision == null)
      {
        CancellationTokenSource timerCancellationTokenSource = new CancellationTokenSource();
        List<Task> taskList = new List<Task>()
        {
          (Task) this.m_checkpointSource.Task,
          (Task) this.m_cancellationSource.Task
        };
        if (this.EnableTimeout && input.PlanVersion >= 6)
          taskList.Add((Task) context.CreateTimer<string>(context.CurrentUtcDateTime.Add(TimeSpan.FromMinutes((double) input.TimeoutInMinutes)), (string) null, timerCancellationTokenSource.Token));
        context.TraceStartLinearPhase("Checks", "WaitOnChecks");
        Task task1 = await Task.WhenAny((IEnumerable<Task>) taskList);
        timerCancellationTokenSource.Cancel();
        Task<CheckpointDecision> task2 = this.m_checkpointSource.Task;
        if (task1 != task2)
        {
          await this.ExecuteAsync(context, (Func<Task>) (() => checkpointController.Cancel(input.ScopeId, input.PlanId, this.m_checkpointContext)));
          context.TraceCompleteLinearOrchestration("Checks", "CheckpointCancelled");
          return CheckpointResult.Denied;
        }
        decision = this.m_checkpointSource.Task.Result;
        await this.ExecuteAsync(context, (Func<Task>) (() => checkpointController.Resolve(input.ScopeId, input.PlanId, this.m_checkpointContext, decision)));
        timerCancellationTokenSource = (CancellationTokenSource) null;
      }
      context.TraceCompleteLinearOrchestration("Checks", "CheckpointComplete");
      return this.GetCheckpointResult(decision);
    }

    private CheckpointResult GetCheckpointResult(CheckpointDecision decision)
    {
      switch (decision?.Result)
      {
        case "Approved":
          return CheckpointResult.Approved;
        case "Canceled":
          return CheckpointResult.Canceled;
        case "TimedOut":
          return CheckpointResult.TimedOut;
        default:
          return CheckpointResult.Denied;
      }
    }
  }
}
