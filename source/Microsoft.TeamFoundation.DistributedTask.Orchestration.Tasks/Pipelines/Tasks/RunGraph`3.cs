// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunGraph`3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Checkpoints;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Orchestration;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public abstract class RunGraph<TInput, TGraph, TNode> : 
    TaskOrchestration<TGraph, TInput, object, TGraph>
    where TInput : RunGraphInput<TGraph, TNode>
    where TGraph : IGraph<TNode>
    where TNode : IGraphNode
  {
    private TGraph m_executionState;
    private readonly TaskCompletionSource<CanceledEvent> m_cancellationSource = new TaskCompletionSource<CanceledEvent>();
    private TaskCompletionSource<RetryEvent> m_retry;
    private Dictionary<string, List<string>> m_checkpointOrchestrationIds = new Dictionary<string, List<string>>();
    private bool m_saveRetryEvents;
    private List<RetryEvent> m_retriesWaitingForNodes = new List<RetryEvent>();
    private Queue<RetryEvent> m_pendingRetryEvents = new Queue<RetryEvent>();
    private const string c_teamName = "CIPlatform";

    public IGraphController Controller { get; private set; }

    public IPipelineLogger Logger { get; private set; }

    public IPipelineIdGenerator IdGenerator { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      context.TraceEvent(name);
      switch (name)
      {
        case "Canceled":
          this.m_executionState.State = PipelineState.Canceling;
          this.m_cancellationSource.TrySetResult(input as CanceledEvent);
          break;
        case "Retry":
          if (!(input is RetryEvent result))
            break;
          IEnumerable<IGraphNode> children = result.Children;
          if ((children != null ? (children.Any<IGraphNode>() ? 1 : 0) : 0) == 0)
            break;
          if (this.m_saveRetryEvents)
            this.m_pendingRetryEvents.Enqueue(result);
          TaskCompletionSource<RetryEvent> retry = this.m_retry;
          if (retry == null)
            break;
          retry.TrySetResult(result);
          break;
      }
    }

    public override TGraph OnGetStatus() => this.m_executionState;

    protected abstract void Copy(TNode from, TNode to);

    protected abstract Task<bool> EvaluateCondition(
      OrchestrationContext context,
      TInput input,
      TNode child,
      IDictionary<string, TNode> dependencies);

    protected abstract Task<TNode> ExecuteNode(
      OrchestrationContext context,
      string instanceId,
      TInput input,
      TNode child,
      IDictionary<string, TNode> dependencies);

    protected abstract Guid GetId(TInput input, TNode child);

    protected abstract string GetOrchestrationId(TInput input, TNode child);

    protected virtual Task<CheckpointResult> EvaluateChecks(
      OrchestrationContext context,
      TInput input,
      TNode child)
    {
      return Task.FromResult<CheckpointResult>(CheckpointResult.Approved);
    }

    protected string GetCheckpointOrchestrationId(Guid planId, string nodeInstanceName) => this.IdGenerator.GetInstanceName(planId.ToString("D"), nodeInstanceName, "checkpoint").ToLower();

    protected async Task<CheckpointResult> RunCheckpoint(
      OrchestrationContext context,
      RunCheckpointInput input,
      bool useCheckpointResultVersion = true)
    {
      string checkpointOrchestrationId = this.GetCheckpointOrchestrationId(input.PlanId, input.NodeInstanceName);
      List<string> stringList;
      if (!this.m_checkpointOrchestrationIds.TryGetValue(input.NodeName, out stringList))
      {
        stringList = new List<string>();
        this.m_checkpointOrchestrationIds.Add(input.NodeName, stringList);
      }
      stringList.Add(checkpointOrchestrationId);
      CheckpointResult checkpointResult1;
      if (useCheckpointResultVersion)
        checkpointResult1 = await context.CreateSubOrchestrationInstance<CheckpointResult>(nameof (RunCheckpoint), "2.0", checkpointOrchestrationId, (object) input);
      else
        checkpointResult1 = await context.CreateSubOrchestrationInstance<bool>(nameof (RunCheckpoint), "1.0", checkpointOrchestrationId, (object) input) ? CheckpointResult.Approved : CheckpointResult.Denied;
      this.m_checkpointOrchestrationIds.GetValueOrDefault<string, List<string>>(input.NodeName, (List<string>) null).Remove(checkpointOrchestrationId);
      CheckpointResult checkpointResult2 = checkpointResult1;
      checkpointOrchestrationId = (string) null;
      return checkpointResult2;
    }

    protected virtual Task<IList<IGraphNode>> PrepareRetry(
      OrchestrationContext context,
      TInput input,
      RetryEvent retryEvent)
    {
      return Task.FromResult<IList<IGraphNode>>((IList<IGraphNode>) null);
    }

    protected virtual Task AfterExecute(OrchestrationContext context, TInput input, TGraph result) => (Task) Task.FromResult<bool>(false);

    protected virtual Task BeforeExecute(OrchestrationContext context, TInput input) => (Task) Task.FromResult<bool>(false);

    public override async Task<TGraph> RunTask(OrchestrationContext context, TInput input)
    {
      this.EnsureExtensions(context, input.ActivityDispatcherShardsCount, (IActivityShardKey) input.ShardKey);
      TaskResult result = TaskResult.Succeeded;
      try
      {
        this.IdGenerator = (IPipelineIdGenerator) new PipelineIdGenerator(input.PlanVersion < 4);
        this.m_executionState = input.Pipeline;
        this.m_executionState.State = PipelineState.InProgress;
        ref TGraph local = ref this.m_executionState;
        if ((object) default (TGraph) == null)
        {
          TGraph graph = local;
          local = ref graph;
        }
        DateTime? nullable = new DateTime?(context.CurrentUtcDateTime);
        local.StartTime = nullable;
        // ISSUE: reference to a compiler-generated field
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.\u003C\u003E4__this.BeforeExecute(context, input)));
        result = await this.ExecuteGraph(context, input);
      }
      finally
      {
        ref TGraph local1 = ref this.m_executionState;
        TGraph graph;
        if ((object) default (TGraph) == null)
        {
          graph = local1;
          local1 = ref graph;
        }
        TaskResult? nullable1 = new TaskResult?(result);
        local1.Result = nullable1;
        this.m_executionState.State = PipelineState.Completed;
        ref TGraph local2 = ref this.m_executionState;
        graph = default (TGraph);
        if ((object) graph == null)
        {
          graph = local2;
          local2 = ref graph;
        }
        DateTime? nullable2 = new DateTime?(context.CurrentUtcDateTime);
        local2.FinishTime = nullable2;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.\u003C\u003E4__this.AfterExecute(context, input, this.\u003C\u003E4__this.m_executionState)));
      }
      return this.m_executionState;
    }

    private async Task<TaskResult> ExecuteGraph(OrchestrationContext context, TInput input)
    {
      Dictionary<string, TaskResult> results = new Dictionary<string, TaskResult>();
      List<TNode> runningNodes = new List<TNode>();
      List<TNode> remainingNodes = new List<TNode>((IEnumerable<TNode>) input.Pipeline.Nodes);
      List<Task<TaskResult>> runningNodeTasks = new List<Task<TaskResult>>();
      HashSet<TNode> canceledNodes = new HashSet<TNode>();
      List<string> completedNodeNames = new List<string>();
      while (remainingNodes.Count > 0 || runningNodes.Count > 0)
      {
        foreach (TNode runnableNode in RunGraph<TInput, TGraph, TNode>.GetRunnableNodes((IList<TNode>) remainingNodes, (IList<string>) completedNodeNames))
        {
          runningNodes.Add(runnableNode);
          remainingNodes.Remove(runnableNode);
          runningNodeTasks.Add(this.ExecuteNodeAsync(context, input, runnableNode, (ISet<TNode>) canceledNodes));
        }
        List<Task> allTasks = runningNodeTasks.Cast<Task>().ToList<Task>();
        if (this.m_executionState.State == PipelineState.InProgress)
        {
          allTasks.Add((Task) this.m_cancellationSource.Task);
          this.m_retry = new TaskCompletionSource<RetryEvent>();
          allTasks.Add((Task) this.m_retry.Task);
          if (input.PlanVersion >= 17 && this.m_pendingRetryEvents.Any<RetryEvent>())
            this.m_retry.SetResult(this.m_pendingRetryEvents.Dequeue());
        }
        Task task1 = await Task.WhenAny((IEnumerable<Task>) allTasks);
        this.m_saveRetryEvents = true;
        RetryEvent retryEvent;
        if (task1 == this.m_cancellationSource.Task)
        {
          context.TraceInfo("Re-evaluate condition on cancellation.");
          List<Task<RunGraph<TInput, TGraph, TNode>.ConditionEvaluationResult>> conditionResults = new List<Task<RunGraph<TInput, TGraph, TNode>.ConditionEvaluationResult>>();
          foreach (TNode child in runningNodes)
            conditionResults.Add(this.EvaluateCondition(context, input, child));
          RunGraph<TInput, TGraph, TNode>.ConditionEvaluationResult[] evaluationResultArray = await Task.WhenAll<RunGraph<TInput, TGraph, TNode>.ConditionEvaluationResult>((IEnumerable<Task<RunGraph<TInput, TGraph, TNode>.ConditionEvaluationResult>>) conditionResults);
          List<string> instanceIds = new List<string>();
          int index = 0;
          for (int count = runningNodes.Count; index < count; ++index)
          {
            TNode child = runningNodes[index];
            if (!conditionResults[index].Result.Passed)
            {
              canceledNodes.Add(child);
              context.TraceInfo("Base on condition re-evaluate result, cancel '" + child.Name + "'.");
              instanceIds.Add(this.GetOrchestrationId(input, child));
            }
            else
              context.TraceInfo("Base on condition re-evaluate result, '" + child.Name + "' will continue running.");
          }
          instanceIds.AddRange(this.m_checkpointOrchestrationIds.SelectMany<KeyValuePair<string, List<string>>, string>((Func<KeyValuePair<string, List<string>>, IEnumerable<string>>) (x => (IEnumerable<string>) x.Value)));
          await this.Controller.Cancel(input.PlanId, (IList<string>) instanceIds, this.m_cancellationSource.Task.Result.Reason);
          conditionResults = (List<Task<RunGraph<TInput, TGraph, TNode>.ConditionEvaluationResult>>) null;
        }
        else if (task1 == this.m_retry?.Task)
        {
          retryEvent = this.m_retry.Task.Result;
          List<TNode> list = runningNodes.Join<TNode, string, string, TNode>((IEnumerable<string>) retryEvent.Children.Select<IGraphNode, string>((Func<IGraphNode, string>) (x => x.Name)).ToList<string>(), (Func<TNode, string>) (k => k.Name), (Func<string, string>) (k => k), (Func<TNode, string, TNode>) ((r, id) => r)).ToList<TNode>();
          if (input.PlanVersion >= 17 && list.Any<TNode>())
          {
            List<string> stringList = new List<string>();
            foreach (TNode child in list)
            {
              List<string> valueOrDefault = this.m_checkpointOrchestrationIds.GetValueOrDefault<string, List<string>>(child.Name, (List<string>) null);
              // ISSUE: explicit non-virtual call
              if (valueOrDefault != null && __nonvirtual (valueOrDefault.Count) > 0)
                stringList.AddRange((IEnumerable<string>) valueOrDefault);
              else
                stringList.Add(this.GetOrchestrationId(input, child));
            }
            context.TraceInfo("Cancelling nodes: " + string.Join(", ", (IEnumerable<string>) stringList) + " before retrying " + string.Join(", ", (IEnumerable<string>) (retryEvent.TargetedChildNames ?? (IList<string>) new List<string>())));
            canceledNodes.AddRange<TNode, HashSet<TNode>>((IEnumerable<TNode>) list);
            await this.Controller.Cancel(input.PlanId, (IList<string>) stringList, "Node canceled due to retry of " + string.Join(", ", (IEnumerable<string>) (retryEvent.TargetedChildNames ?? (IList<string>) new List<string>())));
          }
          this.m_retriesWaitingForNodes.Add(retryEvent);
          retryEvent = (RetryEvent) null;
        }
        else
        {
          int index = allTasks.IndexOf(task1);
          TNode completedNode = runningNodes[index];
          Task<TaskResult> task2 = runningNodeTasks[index];
          runningNodes.RemoveAt(index);
          runningNodeTasks.RemoveAt(index);
          TaskResult nodeResult = TaskResult.Succeeded;
          if (task2.IsFaulted)
          {
            nodeResult = TaskResult.Failed;
            string exceptionMessage = task2.Exception.InnerException?.Message ?? task2.Exception.Message;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            await this.ExecuteAsync(context, input, (Func<Task>) (() => this.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.Logger.LogIssue(input.ScopeId, input.PlanId, this.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.GetId(input, completedNode), context.CurrentUtcDateTime, IssueType.Error, exceptionMessage)));
          }
          else
            nodeResult = task2.Result;
          results[completedNode.Name] = nodeResult;
          completedNodeNames.Add(completedNode.Name);
          context.TraceInfo(string.Format("'{0}' completed with result {1}, current graph result {2}", (object) completedNode.Name, (object) nodeResult, (object) PipelineUtilities.AggregateResult((IEnumerable<TaskResult>) results.Values)));
        }
        if (this.m_retriesWaitingForNodes.Any<RetryEvent>())
        {
          foreach (RetryEvent retryEvent1 in this.m_retriesWaitingForNodes.ToList<RetryEvent>())
          {
            retryEvent = retryEvent1;
            context.TraceInfo("Checking retry status");
            List<TNode> list = retryEvent.Children.Cast<TNode>().ToList<TNode>();
            if (input.PlanVersion < 17 || !list.Join<TNode, TNode, string, TNode>((IEnumerable<TNode>) runningNodes, (Func<TNode, string>) (k => k.Name), (Func<TNode, string>) (k => k.Name), (Func<TNode, TNode, TNode>) ((r, id) => r)).Any<TNode>())
            {
              context.TraceInfo("Retry is able to proceed");
              await this.DoRetry(context, input, list, remainingNodes, results, completedNodeNames, retryEvent);
              this.m_retriesWaitingForNodes.Remove(retryEvent);
            }
            else
            {
              // ISSUE: reference to a compiler-generated field
              context.TraceInfo("Retry cannot proceed yet. Waiting on " + string.Join(", ", list.Join<TNode, TNode, string, TNode>((IEnumerable<TNode>) runningNodes, (Func<TNode, string>) (k => k.Name), (Func<TNode, string>) (k => k.Name), (Func<TNode, TNode, TNode>) ((r, id) => r)).Select<TNode, string>((Func<TNode, string>) (x => this.\u003C\u003E4__this.GetOrchestrationId(input, x)))));
            }
            retryEvent = (RetryEvent) null;
          }
        }
        this.m_saveRetryEvents = false;
        allTasks = (List<Task>) null;
      }
      TaskResult taskResult = !this.m_cancellationSource.Task.IsCompleted ? PipelineUtilities.AggregateResult((IEnumerable<TaskResult>) results.Values) : TaskResult.Canceled;
      results = (Dictionary<string, TaskResult>) null;
      runningNodes = (List<TNode>) null;
      remainingNodes = (List<TNode>) null;
      runningNodeTasks = (List<Task<TaskResult>>) null;
      canceledNodes = (HashSet<TNode>) null;
      completedNodeNames = (List<string>) null;
      return taskResult;
    }

    private async Task DoRetry(
      OrchestrationContext context,
      TInput input,
      List<TNode> childrenToRetry,
      List<TNode> remainingNodes,
      Dictionary<string, TaskResult> results,
      List<string> completedNodeNames,
      RetryEvent retryEvent)
    {
      if (input.PlanVersion >= 16)
        remainingNodes.RemoveAll((Predicate<TNode>) (x => childrenToRetry.Where<TNode>((Func<TNode, bool>) (y => x.Name == y.Name)).Any<TNode>()));
      else
        childrenToRetry.RemoveAll((Predicate<TNode>) (x => remainingNodes.Where<TNode>((Func<TNode, bool>) (y => y.Name == x.Name)).Any<TNode>()));
      IList<IGraphNode> source = await this.PrepareRetry(context, input, retryEvent);
      if (source != null)
        childrenToRetry = source.Cast<TNode>().ToList<TNode>();
      IList<TNode> nodes = input.Pipeline.Nodes;
      List<string> stringList = new List<string>();
      foreach (TNode node in childrenToRetry)
      {
        string name = node.Name;
        int index = 0;
        for (int count = nodes.Count; index < count; ++index)
        {
          if (nodes[index].Name == name)
          {
            nodes[index] = node;
            results.Remove(name);
            remainingNodes.Add(node);
            completedNodeNames.Remove(name);
            stringList.Add(name);
          }
        }
      }
      foreach (TNode remainingNode in remainingNodes)
      {
        ISet<string> satisfied = remainingNode.Dependencies.Satisfied;
        ISet<string> unsatisfied = remainingNode.Dependencies.Unsatisfied;
        foreach (string str in stringList)
        {
          if (satisfied.Remove(str))
            unsatisfied.Add(str);
        }
      }
      context.TraceInfo("Retrying children: [ " + string.Join(", ", (IEnumerable<string>) childrenToRetry.Select<TNode, string>((Func<TNode, string>) (x => x.Name)).OrderBy<string, string>((Func<string, string>) (x => x))) + " ]");
    }

    private async Task<TaskResult> ExecuteNodeAsync(
      OrchestrationContext context,
      TInput input,
      TNode child,
      ISet<TNode> canceledNodes)
    {
      string instanceId = this.GetOrchestrationId(input, child);
      context.TraceStartLinearOrchestration(instanceId);
      if (child.State == PipelineState.Completed)
        return child.Result.Value;
      context.TraceStartLinearPhase("CIPlatform", "EvaluateConditions", instanceId);
      RunGraph<TInput, TGraph, TNode>.ConditionEvaluationResult condition = await this.EvaluateCondition(context, input, child);
      if (canceledNodes.Contains(child) || input.PlanVersion >= 8 && condition.IsShortCircuit)
      {
        await UpdateState(input.PlanVersion >= 8 ? TaskResult.Canceled : TaskResult.Skipped);
        return child.Result.Value;
      }
      if (!condition.Passed)
      {
        await UpdateState(TaskResult.Skipped);
        return child.Result.Value;
      }
      context.TraceStartLinearPhase("CIPlatform", "EvaluateChecks", instanceId);
      CheckpointResult checkpointResult1;
      if (input.PlanVersion >= 5)
        checkpointResult1 = await this.EvaluateChecks(context, input, child);
      else
        checkpointResult1 = CheckpointResult.Approved;
      CheckpointResult checkpointResult2 = checkpointResult1;
      if (canceledNodes.Contains(child))
      {
        await UpdateState(input.PlanVersion >= 8 ? TaskResult.Canceled : TaskResult.Skipped);
        return child.Result.Value;
      }
      if (checkpointResult2 != CheckpointResult.Approved)
      {
        TaskResult taskResult = TaskResult.Failed;
        if (checkpointResult2 == CheckpointResult.Canceled)
          taskResult = TaskResult.Canceled;
        else if (input.PlanVersion >= 12 && checkpointResult2 == CheckpointResult.TimedOut)
          taskResult = TaskResult.Skipped;
        await UpdateState(taskResult);
        return child.Result.Value;
      }
      context.TraceStartLinearPhase("CIPlatform", "ExecuteNode", instanceId);
      child.State = PipelineState.InProgress;
      IDictionary<string, TNode> dependencies = this.ResolveDependencies(input.Pipeline, child, false);
      TNode from = await this.ExecuteNode(context, instanceId, input, child, dependencies);
      context.TraceCompleteLinearOrchestration("CIPlatform", "NodeExecuted", instanceId);
      this.Copy(from, child);
      return child.Result.Value;

      Task UpdateState(TaskResult taskResult)
      {
        context.TraceCompleteLinearOrchestration("CIPlatform", "NodeNotExecuted", instanceId);
        child.State = PipelineState.Completed;
        ref TNode local = ref child;
        if ((object) default (TNode) == null)
        {
          TNode node = local;
          local = ref node;
        }
        TaskResult? nullable = new TaskResult?(taskResult);
        local.Result = nullable;
        // ISSUE: reference to a compiler-generated field
        TimelineRecord nodeRecord = new TimelineRecord()
        {
          Id = this.\u003C\u003E4__this.GetId(input, child),
          State = new TimelineRecordState?(TimelineRecordState.Completed),
          Result = new TaskResult?(taskResult)
        };
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return this.\u003C\u003E4__this.ExecuteAsync(context, input, (Func<Task>) (() => this.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.Logger.UpdateTimeline(input.ScopeId, input.PlanId, (IList<TimelineRecord>) new TimelineRecord[1]
        {
          nodeRecord
        }, (input.PlanVersion >= 18 ? 1 : 0) != 0)));
      }
    }

    protected virtual void EnsureExtensions(
      OrchestrationContext context,
      int activityDispatcherShardsCount,
      IActivityShardKey shardKey)
    {
      this.Logger = context.CreateShardedClient<IPipelineLogger>(true, activityDispatcherShardsCount, shardKey);
      this.Controller = context.CreateShardedClient<IGraphController>(true, activityDispatcherShardsCount, shardKey);
    }

    private async Task<RunGraph<TInput, TGraph, TNode>.ConditionEvaluationResult> EvaluateCondition(
      OrchestrationContext context,
      TInput input,
      TNode child)
    {
      if (child.State == PipelineState.NotStarted && this.m_executionState.State == PipelineState.Canceling && input.Pipeline.Nodes.All<TNode>((Func<TNode, bool>) (x =>
      {
        if (x.State == PipelineState.NotStarted)
          return true;
        TaskResult? result = x.Result;
        TaskResult taskResult = TaskResult.Skipped;
        return result.GetValueOrDefault() == taskResult & result.HasValue;
      })))
        return new RunGraph<TInput, TGraph, TNode>.ConditionEvaluationResult(false, true);
      if (child.Skip)
        return new RunGraph<TInput, TGraph, TNode>.ConditionEvaluationResult(false, false);
      IDictionary<string, TNode> dependencies = this.ResolveDependencies(input.Pipeline, child, true);
      return new RunGraph<TInput, TGraph, TNode>.ConditionEvaluationResult(await this.EvaluateCondition(context, input, child, dependencies), false);
    }

    private IDictionary<string, TNode> ResolveDependencies(
      TGraph graph,
      TNode node,
      bool recursive)
    {
      Dictionary<string, TNode> dictionary1 = new Dictionary<string, TNode>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (node.Dependencies.Satisfied.Count > 0)
      {
        Dictionary<string, TNode> dictionary2 = this.m_executionState.Nodes.ToDictionary<TNode, string, TNode>((Func<TNode, string>) (x => x.Name), (Func<TNode, TNode>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        Queue<string> stringQueue = new Queue<string>((IEnumerable<string>) node.Dependencies.Satisfied);
        while (stringQueue.Count > 0)
        {
          string key = stringQueue.Dequeue();
          TNode node1;
          if (dictionary2.TryGetValue(key, out node1))
          {
            if (!node1.Skip)
              dictionary1[node1.Name] = graph.Trim(node1);
            if (recursive || node1.Skip)
            {
              foreach (string str in (IEnumerable<string>) node1.Dependencies.Satisfied)
              {
                if (stringSet.Add(str))
                  stringQueue.Enqueue(str);
              }
            }
          }
        }
      }
      return (IDictionary<string, TNode>) dictionary1;
    }

    private static List<TNode> GetRunnableNodes(
      IList<TNode> remainingNodes,
      IList<string> completedNodeNames)
    {
      List<TNode> runnableNodes = new List<TNode>();
      foreach (TNode remainingNode in (IEnumerable<TNode>) remainingNodes)
      {
        GraphDependencies dependencies = remainingNode.Dependencies;
        ISet<string> satisfied = dependencies.Satisfied;
        ISet<string> unsatisfied = dependencies.Unsatisfied;
        foreach (string completedNodeName in (IEnumerable<string>) completedNodeNames)
        {
          if (unsatisfied.Remove(completedNodeName))
            satisfied.Add(completedNodeName);
        }
        if (unsatisfied.Count == 0)
          runnableNodes.Add(remainingNode);
      }
      return runnableNodes;
    }

    protected Task<TResult> ExecuteAsync<TResult>(
      OrchestrationContext context,
      TInput input,
      Func<Task<TResult>> operation)
    {
      return context.ExecuteAsync<TResult>(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceException(ex)));
    }

    protected Task ExecuteAsync(OrchestrationContext context, TInput input, Func<Task> operation) => context.ExecuteAsync(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceException(ex)));

    private struct ConditionEvaluationResult
    {
      public bool Passed;
      public bool IsShortCircuit;

      public ConditionEvaluationResult(bool passed, bool isShortCircuit)
      {
        this.Passed = passed;
        this.IsShortCircuit = isShortCircuit;
      }
    }
  }
}
