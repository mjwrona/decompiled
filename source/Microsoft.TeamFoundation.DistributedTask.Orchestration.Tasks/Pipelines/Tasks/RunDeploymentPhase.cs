// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunDeploymentPhase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Orchestration;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  internal sealed class RunDeploymentPhase : RunPhase
  {
    private RunPhaseInput m_input;
    private DeploymentGroupTarget m_target;
    private string m_phaseIdentifier;
    private Guid m_phaseRecordId;
    private readonly Dictionary<Guid, DeploymentJobDetails> m_deploymentUnassignedJobs;
    private readonly Dictionary<Task<TaskResult>, Guid> m_deploymentsInProgress;
    private readonly DeploymentPoolState m_deploymentPoolState;
    private uint? m_maxUnassignedJobs;
    private bool m_isRetry;

    public RunDeploymentPhase()
    {
      this.m_deploymentPoolState = new DeploymentPoolState();
      this.m_deploymentUnassignedJobs = new Dictionary<Guid, DeploymentJobDetails>();
      this.m_deploymentsInProgress = new Dictionary<Task<TaskResult>, Guid>();
    }

    public IDeploymentPoolManager DeploymentPoolManager { get; private set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      context.TraceEvent(name);
      switch (name)
      {
        case "Canceled":
          this.m_executionState.State = PipelineState.Canceling;
          this.m_cancellationSource.TrySetResult((CanceledEvent) input);
          break;
        case "JobAssigned":
          JobAssignedEventData result = (JobAssignedEventData) input;
          DeploymentJobDetails deploymentJobDetails;
          DeploymentMachineState deploymentMachineState1;
          if (!this.m_deploymentUnassignedJobs.TryGetValue(result.JobId, out deploymentJobDetails) || !this.m_deploymentPoolState.Machines.TryGetValue(result.AgentRequest.ReservedAgent.Id, out deploymentMachineState1))
            break;
          deploymentMachineState1.DeploymentAttempted = true;
          deploymentMachineState1.JobId = new Guid?(result.JobId);
          foreach (int candidateAgent in deploymentJobDetails.CandidateAgents)
          {
            DeploymentMachineState deploymentMachineState2;
            if (this.m_deploymentPoolState.Machines.TryGetValue(candidateAgent, out deploymentMachineState2))
              deploymentMachineState2.DeploymentInQueue = false;
          }
          deploymentJobDetails.Request.TrySetResult(result);
          break;
      }
    }

    public override async Task<PhaseExecutionState> RunTask(
      OrchestrationContext context,
      RunPhaseInput input)
    {
      RunDeploymentPhase runDeploymentPhase = this;
      runDeploymentPhase.EnsureExtensions(context, input.ActivityDispatcherShardsCount, (IActivityShardKey) input.ShardKey);
      runDeploymentPhase.m_idGenerator = (IPipelineIdGenerator) new PipelineIdGenerator(input.PlanVersion < 4);
      runDeploymentPhase.m_executionState = input.Phase;
      runDeploymentPhase.m_executionState.State = PipelineState.InProgress;
      runDeploymentPhase.m_executionState.StartTime = new DateTime?(context.CurrentUtcDateTime);
      runDeploymentPhase.m_input = input;
      runDeploymentPhase.m_target = runDeploymentPhase.m_executionState.Target as DeploymentGroupTarget;
      runDeploymentPhase.m_phaseIdentifier = runDeploymentPhase.m_idGenerator.GetPhaseInstanceName(input.StageName, input.Phase.Name, input.Phase.Attempt);
      runDeploymentPhase.m_phaseRecordId = runDeploymentPhase.m_idGenerator.GetInstanceId(runDeploymentPhase.m_phaseIdentifier);
      runDeploymentPhase.m_isRetry = input.Phase.Jobs.Any<JobExecutionState>();
      await runDeploymentPhase.InitializeAgentPoolState(context);
      List<string> jobNames = runDeploymentPhase.GetJobNames(input);
      PhaseExecutionState expandedPhase = (PhaseExecutionState) null;
      try
      {
        expandedPhase = await runDeploymentPhase.PhaseController.Expand(input.ScopeId, input.PlanId, input.StageName, input.Phase.Name, input.DependsOn, input.StageAttempt, input.Phase.Attempt, (IList<string>) jobNames);
      }
      catch (Exception ex) when (ex is TaskFailedException taskFailedException && taskFailedException.InnerException is PipelineValidationException)
      {
        await runDeploymentPhase.DeploymentPlanCompleted(context, new Queue<JobExecutionState>());
        runDeploymentPhase.m_executionState.State = PipelineState.Completed;
        runDeploymentPhase.m_executionState.Result = new TaskResult?(TaskResult.Failed);
        return runDeploymentPhase.m_executionState;
      }
      runDeploymentPhase.m_executionState.Jobs.Clear();
      runDeploymentPhase.m_executionState.Jobs.AddRange<JobExecutionState, IList<JobExecutionState>>((IEnumerable<JobExecutionState>) expandedPhase.Jobs);
      if (!runDeploymentPhase.GetDeploymentPoolStatus(context).CanQueueAgentRequest())
      {
        await runDeploymentPhase.DeploymentPlanCompleted(context, new Queue<JobExecutionState>((IEnumerable<JobExecutionState>) runDeploymentPhase.m_executionState.Jobs));
        runDeploymentPhase.m_executionState.State = PipelineState.Completed;
        runDeploymentPhase.m_executionState.Result = new TaskResult?(TaskResult.Failed);
        return runDeploymentPhase.m_executionState;
      }
      int count = (int) runDeploymentPhase.m_maxUnassignedJobs ?? 1;
      int num = runDeploymentPhase.m_deploymentPoolState.Machines.Values.Count<DeploymentMachineState>((Func<DeploymentMachineState, bool>) (x => x.CanAttemptDeployment())) / count;
      Queue<JobExecutionState> pendingExecution = new Queue<JobExecutionState>(runDeploymentPhase.m_executionState.Jobs.Skip<JobExecutionState>(count));
      foreach (JobExecutionState job in runDeploymentPhase.m_executionState.Jobs.Take<JobExecutionState>(count))
        runDeploymentPhase.QueueJob(context, input, job, new int?(num));
      for (int attempt = 0; attempt < 5; ++attempt)
      {
        context.TracePlanInformation(input.ScopeId, input.PlanId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "RDP : Phase {0} Starting Attempt#{1}.", (object) runDeploymentPhase.m_phaseIdentifier, (object) attempt));
        do
        {
          jobCompletedTask = (Task<TaskResult>) null;
          Task<JobAssignedEventData> jobAssignedTask = (Task<JobAssignedEventData>) null;
          TaskResult jobResult = TaskResult.Failed;
          try
          {
            IEnumerable<Task> tasks = ((IEnumerable<Task>) runDeploymentPhase.m_deploymentUnassignedJobs.Values.Select<DeploymentJobDetails, Task<JobAssignedEventData>>((Func<DeploymentJobDetails, Task<JobAssignedEventData>>) (cs => cs.Request.Task))).Concat<Task>(runDeploymentPhase.m_deploymentsInProgress.Keys.Select<Task<TaskResult>, Task>((Func<Task<TaskResult>, Task>) (t => (Task) t)));
            if (tasks.Any<Task>())
            {
              Task task = await Task.WhenAny(tasks);
              if (task is Task<TaskResult> jobCompletedTask)
                jobResult = jobCompletedTask.Result;
              else
                jobAssignedTask = task as Task<JobAssignedEventData>;
            }
          }
          catch (AggregateException ex)
          {
            context.TracePlanException(input.ScopeId, input.PlanId, (Exception) ex);
            if (ex.InnerException is TaskFailedException)
              jobResult = TaskResult.Failed;
            else
              throw;
          }
          finally
          {
            if (jobCompletedTask != null)
            {
              Guid jobInstanceId;
              if (runDeploymentPhase.m_deploymentsInProgress.TryGetValue(jobCompletedTask, out jobInstanceId))
              {
                runDeploymentPhase.m_deploymentsInProgress.Remove(jobCompletedTask);
                DeploymentMachineState deploymentMachineState1 = runDeploymentPhase.m_deploymentPoolState.Machines.Values.SingleOrDefault<DeploymentMachineState>((Func<DeploymentMachineState, bool>) (m =>
                {
                  Guid? jobId = m.JobId;
                  Guid guid = jobInstanceId;
                  if (!jobId.HasValue)
                    return false;
                  return !jobId.HasValue || jobId.GetValueOrDefault() == guid;
                }));
                if (deploymentMachineState1 != null)
                {
                  deploymentMachineState1.DeploymentResult = new TaskResult?(jobResult);
                }
                else
                {
                  context.TracePlanError(input.ScopeId, input.PlanId, "RDP : {0} Unable to find agent for which job : {1} is assigned", (object) input.PlanId, (object) jobInstanceId);
                  DeploymentJobDetails deploymentJobDetails;
                  if (runDeploymentPhase.m_deploymentUnassignedJobs.TryGetValue(jobInstanceId, out deploymentJobDetails))
                  {
                    foreach (int candidateAgent in deploymentJobDetails.CandidateAgents)
                    {
                      DeploymentMachineState deploymentMachineState2;
                      if (runDeploymentPhase.m_deploymentPoolState.Machines.TryGetValue(candidateAgent, out deploymentMachineState2))
                      {
                        deploymentMachineState2.DeploymentInQueue = false;
                        deploymentMachineState2.Excluded = true;
                      }
                    }
                    runDeploymentPhase.m_deploymentUnassignedJobs.Remove(jobInstanceId);
                  }
                }
              }
            }
            else if (jobAssignedTask != null)
            {
              runDeploymentPhase.m_deploymentUnassignedJobs.Remove(jobAssignedTask.Result.JobId);
              context.TracePlanInformation(input.ScopeId, input.PlanId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "RDP : {0} jobId : {1} reservedAgent : {2}.", (object) runDeploymentPhase.m_phaseIdentifier, (object) jobAssignedTask.Result.JobId, (object) jobAssignedTask.Result.AgentRequest?.ReservedAgent?.Id));
            }
          }
          DeploymentPoolStatus deploymentPoolStatus = runDeploymentPhase.GetDeploymentPoolStatus(context);
          if (runDeploymentPhase.m_executionState.State == PipelineState.InProgress && deploymentPoolStatus.CanQueueAgentRequest() && pendingExecution.Any<JobExecutionState>())
            runDeploymentPhase.QueueJob(context, input, pendingExecution.Dequeue());
          jobCompletedTask = (Task<TaskResult>) null;
          jobAssignedTask = (Task<JobAssignedEventData>) null;
        }
        while (runDeploymentPhase.m_deploymentsInProgress.Count > 0);
        bool flag = await runDeploymentPhase.UpdateAgentPoolState(context, attempt);
        DeploymentPoolStatus deploymentPoolStatus1 = runDeploymentPhase.GetDeploymentPoolStatus(context);
        if (!(runDeploymentPhase.m_executionState.State == PipelineState.InProgress & flag) || !deploymentPoolStatus1.CanQueueAgentRequest())
          break;
      }
      await runDeploymentPhase.DeploymentPlanCompleted(context, pendingExecution);
      runDeploymentPhase.m_executionState.State = PipelineState.Completed;
      runDeploymentPhase.m_executionState.Result = new TaskResult?(runDeploymentPhase.m_deploymentPoolState.GetDeploymentResult());
      return runDeploymentPhase.m_executionState;
    }

    protected override Task<JobExecutionState> RunJob(
      OrchestrationContext context,
      PhaseExecutionState phase,
      JobExecutionState job,
      string jobInstanceId,
      RunPhaseInput input)
    {
      RunAgentJobInput input1 = new RunAgentJobInput()
      {
        ActivityDispatcherShardsCount = input.ActivityDispatcherShardsCount,
        ShardKey = input.ShardKey,
        ScopeId = input.ScopeId,
        PlanId = input.PlanId,
        PlanVersion = input.PlanVersion,
        StageName = input.StageName,
        StageAttempt = input.StageAttempt,
        PhaseName = input.Phase.Name,
        PhaseAttempt = input.Phase.Attempt,
        QueueId = this.m_target.DeploymentGroup.Id,
        Job = job,
        NotifyJobAssigned = true
      };
      input1.AgentIds.AddRange((IEnumerable<int>) this.GetTargetAgentIds(this.GetJobInstanceId(job)));
      return context.CreateSubOrchestrationInstance<JobExecutionState>("RunPipelineAgentJob", "1.0", jobInstanceId, (object) input1);
    }

    protected override void EnsureExtensions(
      OrchestrationContext context,
      int activityDispatcherShardsCount,
      IActivityShardKey shardKey)
    {
      base.EnsureExtensions(context, activityDispatcherShardsCount, shardKey);
      this.DeploymentPoolManager = context.CreateShardedClient<IDeploymentPoolManager>(true, activityDispatcherShardsCount, shardKey);
    }

    private void QueueJob(
      OrchestrationContext context,
      RunPhaseInput input,
      JobExecutionState job,
      int? setSize = null)
    {
      Guid jobInstanceId = this.GetJobInstanceId(job);
      List<int> candidateAgents = this.GetCandidateAgents(setSize);
      DeploymentJobDetails deploymentJobDetails = new DeploymentJobDetails()
      {
        Request = new TaskCompletionSource<JobAssignedEventData>()
      };
      deploymentJobDetails.CandidateAgents.AddRange((IEnumerable<int>) candidateAgents);
      this.m_deploymentUnassignedJobs.Add(jobInstanceId, deploymentJobDetails);
      this.m_deploymentsInProgress.Add(this.ExecuteJobAsync(context, input, input.Phase, job), jobInstanceId);
      string format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "RDP : {0} Queue job {1} targeting agents {2}.", (object) this.m_phaseIdentifier, (object) jobInstanceId, (object) string.Join<int>(", ", (IEnumerable<int>) candidateAgents));
      context.TracePlanInformation(input.ScopeId, input.PlanId, format);
    }

    private async Task InitializeAgentPoolState(OrchestrationContext context)
    {
      RunDeploymentPhase runDeploymentPhase1 = this;
      // ISSUE: reference to a compiler-generated method
      IList<TaskAgent> agents = await runDeploymentPhase1.ExecuteAsync<IList<TaskAgent>>(context, runDeploymentPhase1.m_input, new Func<Task<IList<TaskAgent>>>(runDeploymentPhase1.\u003CInitializeAgentPoolState\u003Eb__10_4));
      if (runDeploymentPhase1.m_target.TargetIds.Any<int>())
      {
        // ISSUE: reference to a compiler-generated method
        agents = (IList<TaskAgent>) agents.Where<TaskAgent>(new Func<TaskAgent, bool>(runDeploymentPhase1.\u003CInitializeAgentPoolState\u003Eb__10_0)).ToList<TaskAgent>();
      }
      if (runDeploymentPhase1.m_isRetry)
      {
        RunDeploymentPhase runDeploymentPhase = runDeploymentPhase1;
        Guid phaseRecordId = runDeploymentPhase1.m_idGenerator.GetPhaseInstanceId(runDeploymentPhase1.m_input.StageName, runDeploymentPhase1.m_input.Phase.Name, runDeploymentPhase1.m_input.Phase.Attempt - 1);
        IList<string> agentNames = await runDeploymentPhase1.ExecuteAsync<IList<string>>(context, runDeploymentPhase1.m_input, (Func<Task<IList<string>>>) (() => runDeploymentPhase.DeploymentPoolManager.GetJobWorkerNames(runDeploymentPhase.m_input.ScopeId, runDeploymentPhase.m_input.PlanId, phaseRecordId, (IList<string>) runDeploymentPhase.m_input.Phase.Jobs.Select<JobExecutionState, string>((Func<JobExecutionState, string>) (j => j.Name)).ToList<string>())));
        agents = (IList<TaskAgent>) agents.Where<TaskAgent>((Func<TaskAgent, bool>) (x => agentNames.Contains(x.Name))).ToList<TaskAgent>();
      }
      IList<int> agentIds = await runDeploymentPhase1.DeploymentPoolManager.GetAgentIds(runDeploymentPhase1.m_target.DeploymentGroup.Pool.Id, (IList<Demand>) runDeploymentPhase1.m_target.Demands.ToList<Demand>());
      HashSet<int> hashSet = agents.Select<TaskAgent, int>((Func<TaskAgent, int>) (x => x.Id)).Except<int>((IEnumerable<int>) agentIds).ToHashSet<int, int>((Func<int, int>) (x => x));
      foreach (TaskAgent agent in (IEnumerable<TaskAgent>) agents)
        runDeploymentPhase1.AddNewAgent(context, agent, !hashSet.Contains(agent.Id));
      runDeploymentPhase1.m_maxUnassignedJobs = new uint?(runDeploymentPhase1.GetMaxUnassignedJobsCount(context, runDeploymentPhase1.m_target, agents.Any<TaskAgent>((Func<TaskAgent, bool>) (x => x.AssignedRequest != null))));
      agents = (IList<TaskAgent>) null;
    }

    private async Task<bool> UpdateAgentPoolState(OrchestrationContext context, int attempt)
    {
      RunDeploymentPhase runDeploymentPhase = this;
      bool hasChanged = false;
      IList<TaskAgent> agents = await runDeploymentPhase.ExecuteAsync<IList<TaskAgent>>(context, runDeploymentPhase.m_input, (Func<Task<IList<TaskAgent>>>) (() => this.DeploymentPoolManager.GetTaskAgents(this.m_input.ScopeId, this.m_target.DeploymentGroup.Id, (IList<string>) this.m_target.Tags.ToList<string>())));
      IList<int> agentIds = await runDeploymentPhase.DeploymentPoolManager.GetAgentIds(runDeploymentPhase.m_target.DeploymentGroup.Pool.Id, (IList<Demand>) runDeploymentPhase.m_target.Demands.ToList<Demand>());
      ICollection<int> keys = runDeploymentPhase.m_deploymentPoolState.Machines.Keys;
      IEnumerable<int> ints = agents.Select<TaskAgent, int>((Func<TaskAgent, int>) (x => x.Id));
      IEnumerable<int> updates = keys.Intersect<int>(ints);
      HashSet<int> hashSet = ints.Except<int>((IEnumerable<int>) agentIds).ToHashSet<int, int>((Func<int, int>) (x => x));
      foreach (TaskAgent agent in agents.Where<TaskAgent>((Func<TaskAgent, bool>) (x => updates.Contains<int>(x.Id))))
        hasChanged |= runDeploymentPhase.UpdateExistingAgent(context, agent, false, !hashSet.Contains(agent.Id), "Updated in attempt #" + attempt.ToString());
      bool flag = hasChanged;
      agents = (IList<TaskAgent>) null;
      return flag;
    }

    private bool AddNewAgent(OrchestrationContext context, TaskAgent agent, bool demandsMatched)
    {
      DeploymentMachineState deploymentMachineState = new DeploymentMachineState()
      {
        Id = agent.Id,
        Name = agent.Name,
        AgentStatus = agent.Status,
        DeploymentInQueue = false,
        DeploymentAttempted = false,
        DemandsMatched = demandsMatched
      };
      this.m_deploymentPoolState.Machines.Add(agent.Id, deploymentMachineState);
      string format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "RDP : {0} Included agent {1} running on deployment group {2}.", (object) this.m_phaseIdentifier, (object) agent.Id, (object) this.m_target.DeploymentGroup.Id);
      context.TracePlanInformation(this.m_input.ScopeId, this.m_input.PlanId, format);
      return true;
    }

    private bool UpdateExistingAgent(
      OrchestrationContext context,
      TaskAgent agent,
      bool excludeFromPool,
      bool demandsMatched,
      string reason)
    {
      DeploymentMachineState deploymentMachineState;
      this.m_deploymentPoolState.Machines.TryGetValue(agent.Id, out deploymentMachineState);
      bool flag = !agent.Enabled.GetValueOrDefault(false) | excludeFromPool;
      if (flag && !deploymentMachineState.Excluded)
        this.ExcludeAgent(context, agent.Id, reason);
      if (deploymentMachineState.AgentStatus == agent.Status && deploymentMachineState.Excluded == flag && deploymentMachineState.DemandsMatched == demandsMatched)
        return false;
      this.m_deploymentPoolState.Machines[agent.Id].AgentStatus = agent.Status;
      this.m_deploymentPoolState.Machines[agent.Id].Excluded = flag;
      this.m_deploymentPoolState.Machines[agent.Id].DemandsMatched = demandsMatched;
      string format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "RDP : {0} Updated agent {1} running on deployment group {2}. Excluded : {3}. Status : {4}. demandsMatched: {5}. Reason = {6}", (object) this.m_phaseIdentifier, (object) agent.Id, (object) this.m_target.DeploymentGroup.Id, (object) flag, (object) agent.Status, (object) demandsMatched, (object) reason);
      context.TracePlanInformation(this.m_input.ScopeId, this.m_input.PlanId, format);
      return true;
    }

    private bool ExcludeAgent(OrchestrationContext context, int agentId, string reason)
    {
      if (this.m_deploymentPoolState.Machines[agentId].Excluded)
        return false;
      this.m_deploymentPoolState.Machines[agentId].Excluded = true;
      KeyValuePair<Guid, DeploymentJobDetails> keyValuePair = this.m_deploymentUnassignedJobs.FirstOrDefault<KeyValuePair<Guid, DeploymentJobDetails>>((Func<KeyValuePair<Guid, DeploymentJobDetails>, bool>) (x => x.Value.CandidateAgents.Contains(agentId)));
      if (!keyValuePair.Equals((object) new KeyValuePair<Guid, DeploymentJobDetails>()) && keyValuePair.Value.CandidateAgents.All<int>((Func<int, bool>) (x => this.m_deploymentPoolState.Machines[x].Excluded)))
      {
        string jobInstanceId = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:N}_{1:N}", (object) this.m_input.PlanId, (object) keyValuePair.Key);
        context.ExecuteAsync((Func<Task>) (() => this.JobController.CancelJob(jobInstanceId, TimeSpan.FromSeconds(60.0), "All matched targets are marked as excluded.")), canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TracePlanException(this.m_input.ScopeId, this.m_input.PlanId, ex)));
      }
      string format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "RDP : {0} Excluded agent {1} running on deployment group {2}. Reason = {3}", (object) this.m_phaseIdentifier, (object) agentId, (object) this.m_target.DeploymentGroup.Id, (object) reason);
      context.TracePlanInformation(this.m_input.ScopeId, this.m_input.PlanId, format);
      return true;
    }

    private uint GetMaxUnassignedJobsCount(
      OrchestrationContext context,
      DeploymentGroupTarget deploymentGroupTarget,
      bool isPoolBusy)
    {
      if (this.m_deploymentPoolState == null)
        context.TracePlanError(this.m_input.ScopeId, this.m_input.PlanId, "RDP : {0} GetDeploymentPoolStatus : DeploymentPoolState is not initialized.", (object) this.m_input.PlanId);
      IEnumerable<DeploymentMachineState> source1 = this.m_deploymentPoolState.Machines.Values.Where<DeploymentMachineState>((Func<DeploymentMachineState, bool>) (x => x.CanAttemptDeployment()));
      if (this.m_isRetry)
        return (uint) source1.Count<DeploymentMachineState>();
      if (isPoolBusy && !this.CanDeployToAllTargetInParallel(deploymentGroupTarget))
        return 1;
      IEnumerable<DeploymentMachineState> source2 = this.m_deploymentPoolState.Machines.Values.Where<DeploymentMachineState>((Func<DeploymentMachineState, bool>) (x => !x.IsHealthy()));
      return (uint) Math.Min((long) this.GetMaxUnhealthyTargetsCount(deploymentGroupTarget) - (long) source2.Count<DeploymentMachineState>(), (long) source1.Count<DeploymentMachineState>());
    }

    private uint GetMaxUnhealthyTargetsCount(DeploymentGroupTarget deploymentGroupTarget) => deploymentGroupTarget.Execution.RollingOption == DeploymentRollingOption.Absolute ? deploymentGroupTarget.Execution.RollingValue : (uint) ((ulong) this.m_deploymentPoolState.Machines.Values.Where<DeploymentMachineState>((Func<DeploymentMachineState, bool>) (x => !x.Excluded)).Count<DeploymentMachineState>() * (ulong) deploymentGroupTarget.Execution.RollingValue) / 100U;

    private bool CanDeployToAllTargetInParallel(DeploymentGroupTarget input)
    {
      if (input.Execution.RollingOption == DeploymentRollingOption.Percentage)
        return input.Execution.RollingValue == 100U;
      IEnumerable<DeploymentMachineState> source = this.m_deploymentPoolState.Machines.Values.Where<DeploymentMachineState>((Func<DeploymentMachineState, bool>) (x => x.CanAttemptDeployment()));
      return (long) input.Execution.RollingValue >= (long) source.Count<DeploymentMachineState>();
    }

    private List<int> GetCandidateAgents(int? count = null)
    {
      List<int> candidateAgents = new List<int>();
      int num1 = 0;
      foreach (DeploymentMachineState machine in (IEnumerable<DeploymentMachineState>) this.m_deploymentPoolState.Machines.Values)
      {
        if (machine.CanAttemptDeployment())
        {
          machine.DeploymentInQueue = true;
          candidateAgents.Add(machine.Id);
          ++num1;
          if (count.HasValue)
          {
            int num2 = num1;
            int? nullable = count;
            int valueOrDefault = nullable.GetValueOrDefault();
            if (num2 == valueOrDefault & nullable.HasValue)
              break;
          }
        }
      }
      return candidateAgents;
    }

    private List<int> GetTargetAgentIds(Guid jobId)
    {
      DeploymentJobDetails deploymentJobDetails;
      return this.m_deploymentUnassignedJobs.TryGetValue(jobId, out deploymentJobDetails) ? deploymentJobDetails.CandidateAgents : new List<int>();
    }

    private DeploymentPoolStatus GetDeploymentPoolStatus(OrchestrationContext context)
    {
      IEnumerable<DeploymentMachineState> deploymentMachineStates = this.m_deploymentPoolState.Machines.Values.Where<DeploymentMachineState>((Func<DeploymentMachineState, bool>) (x => !x.Excluded));
      int num = 0;
      bool flag1 = false;
      bool flag2 = true;
      foreach (DeploymentMachineState machine in deploymentMachineStates)
      {
        if (!machine.IsHealthy())
          ++num;
        if (!machine.DeploymentAttempted)
          flag2 = false;
        if (machine.CanAttemptDeployment())
          flag1 = true;
      }
      uint unhealthyTargetsCount = this.GetMaxUnhealthyTargetsCount(this.m_target);
      bool flag3 = (long) num < (long) unhealthyTargetsCount;
      return new DeploymentPoolStatus()
      {
        AgentAvailable = flag1,
        DeploymentPoolHealthy = flag3,
        DeploymentComplete = flag2
      };
    }

    private async Task DeploymentPlanCompleted(
      OrchestrationContext context,
      Queue<JobExecutionState> pendingJobs)
    {
      RunDeploymentPhase runDeploymentPhase = this;
      DeploymentPoolStatus deploymentPoolStatus = runDeploymentPhase.GetDeploymentPoolStatus(context);
      if (deploymentPoolStatus.DeploymentComplete && runDeploymentPhase.m_deploymentPoolState.Machines.Any<KeyValuePair<int, DeploymentMachineState>>() && runDeploymentPhase.m_deploymentPoolState.Machines.Values.All<DeploymentMachineState>((Func<DeploymentMachineState, bool>) (a => a.DeploymentAttempted)))
        return;
      IList<TimelineRecord> recordsToUpdate = (IList<TimelineRecord>) new List<TimelineRecord>();
      DateTime currentUtcDateTime = context.CurrentUtcDateTime;
      if (!runDeploymentPhase.m_deploymentPoolState.Machines.Any<KeyValuePair<int, DeploymentMachineState>>())
      {
        string machineNotFoundMessage = runDeploymentPhase.m_target.Tags.Any<string>() ? Resources.NoTargetFoundWithGivenTags((object) string.Join(", ", (IEnumerable<string>) runDeploymentPhase.m_target.Tags)) : Resources.NoTargetFound();
        Guid recordId = runDeploymentPhase.m_phaseRecordId;
        await runDeploymentPhase.ExecuteAsync(context, runDeploymentPhase.m_input, (Func<Task>) (() => this.LogIssue(context, this.m_input, recordId, IssueType.Error, machineNotFoundMessage)));
      }
      else
      {
        foreach (DeploymentMachineState machine in runDeploymentPhase.m_deploymentPoolState.Machines.Values.Where<DeploymentMachineState>((Func<DeploymentMachineState, bool>) (a => !a.DeploymentAttempted)))
        {
          if (pendingJobs.Any<JobExecutionState>())
          {
            JobExecutionState job = pendingJobs.Dequeue();
            if (machine.Excluded)
            {
              string logMessage = Resources.TargetExcluded((object) machine.Name);
              recordsToUpdate.Add(runDeploymentPhase.UpdateJobTimelineRecord(machine.Name, currentUtcDateTime, job, logMessage));
            }
            else if (machine.AgentStatus == TaskAgentStatus.Offline)
            {
              string logMessage = Resources.TargetOffline((object) machine.Name);
              recordsToUpdate.Add(runDeploymentPhase.UpdateJobTimelineRecord(machine.Name, currentUtcDateTime, job, logMessage));
            }
            else if (!machine.DemandsMatched)
            {
              string logMessage = Resources.DemandsNotMet((object) machine.Name, (object) string.Join<Demand>(", ", (IEnumerable<Demand>) runDeploymentPhase.m_target.Demands));
              recordsToUpdate.Add(runDeploymentPhase.UpdateJobTimelineRecord(machine.Name, currentUtcDateTime, job, logMessage));
            }
            else if (!deploymentPoolStatus.DeploymentPoolHealthy)
            {
              string logMessage = Resources.DeploymentHealthNotMet((object) machine.Name);
              recordsToUpdate.Add(runDeploymentPhase.UpdateJobTimelineRecord(machine.Name, currentUtcDateTime, job, logMessage));
            }
            else if (machine.IsHealthy())
            {
              string logMessage = Resources.DeploymentHealthNotMet((object) machine.Name);
              recordsToUpdate.Add(runDeploymentPhase.UpdateJobTimelineRecord(machine.Name, currentUtcDateTime, job, logMessage));
            }
          }
          else
            break;
        }
        recordsToUpdate.Add(new TimelineRecord()
        {
          Id = runDeploymentPhase.m_phaseRecordId,
          FinishTime = new DateTime?(context.CurrentUtcDateTime),
          State = new TimelineRecordState?(TimelineRecordState.Completed),
          Result = new TaskResult?(runDeploymentPhase.m_deploymentPoolState.GetDeploymentResult())
        });
        await runDeploymentPhase.ExecuteAsync(context, runDeploymentPhase.m_input, (Func<Task>) (() => this.Logger.UpdateTimeline(this.m_input.ScopeId, this.m_input.PlanId, recordsToUpdate)));
      }
    }

    private TimelineRecord UpdateJobTimelineRecord(
      string agentName,
      DateTime currentUtcTime,
      JobExecutionState job,
      string logMessage)
    {
      Guid jobInstanceId = this.GetJobInstanceId(job);
      Guid phaseRecordId = this.m_phaseRecordId;
      return new TimelineRecord()
      {
        Id = jobInstanceId,
        Name = job.Name,
        ParentId = new Guid?(phaseRecordId),
        WorkerName = agentName,
        StartTime = new DateTime?(currentUtcTime),
        FinishTime = new DateTime?(currentUtcTime),
        State = new TimelineRecordState?(TimelineRecordState.Completed),
        Result = new TaskResult?(TaskResult.Skipped),
        Issues = {
          new Issue() { Type = IssueType.Error, Message = logMessage }
        }
      };
    }

    private Guid GetJobInstanceId(JobExecutionState job) => this.m_idGenerator.GetJobInstanceId(this.m_input.StageName, this.m_input.Phase.Name, job.Name, job.Attempt);

    private List<string> GetJobNames(RunPhaseInput input)
    {
      if (this.m_isRetry)
        return input.Phase.Jobs.Select<JobExecutionState, string>((Func<JobExecutionState, string>) (j => j.Name)).ToList<string>();
      List<string> jobNames = new List<string>();
      for (int order = 1; order <= this.m_deploymentPoolState.Machines.Values.Count<DeploymentMachineState>(); ++order)
        jobNames.Add(RunDeploymentPhase.GetDeploymentJobName(order));
      return jobNames;
    }

    private static string GetDeploymentJobName(int order) => string.Format("Deploy_{0}", (object) order);
  }
}
