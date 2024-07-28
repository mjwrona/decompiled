// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VMStrategyExecutorBase2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal abstract class VMStrategyExecutorBase2 : IStrategyExecutor
  {
    protected int jobsCount;
    protected VirtualMachineGroupState m_virtualMachineGroupState;
    protected PipelineGraphNodeReference m_stage;
    protected PipelineGraphNodeReference m_phase;
    protected Dictionary<Guid, List<int>> m_queuedJobs;
    protected IList<DeploymentLifeCycleHookBase> m_actions;
    protected const string c_jobNamePrefix = "ProviderJob";

    public VMStrategyExecutorBase2(
      VirtualMachineGroupState virtualMachineGroupState,
      PipelineGraphNodeReference stage,
      PipelineGraphNodeReference phase,
      IList<DeploymentLifeCycleHookBase> actions)
    {
      this.m_virtualMachineGroupState = virtualMachineGroupState;
      this.m_stage = stage;
      this.m_phase = phase;
      this.jobsCount = 1;
      this.m_queuedJobs = new Dictionary<Guid, List<int>>();
      this.m_actions = actions;
    }

    public virtual TaskResult? GetDeploymentResult(out IList<TimelineRecord> timelineRecords)
    {
      if (!this.IsDeploymentComplete())
      {
        timelineRecords = (IList<TimelineRecord>) null;
        return new TaskResult?();
      }
      TaskResult? aggregatedResult = new TaskResult?();
      timelineRecords = (IList<TimelineRecord>) this.GetTimelineRecords();
      foreach (VirtualMachineState virtualMachineState in (IEnumerable<VirtualMachineState>) this.m_virtualMachineGroupState.VirtualMachines.Values)
      {
        if (!virtualMachineState.DeploymentAttempted)
          return new TaskResult?(TaskResult.Failed);
        if (virtualMachineState.DeploymentAttempted)
        {
          TaskResult jobResult = (TaskResult) ((int) virtualMachineState.DeploymentResult ?? 2);
          aggregatedResult = new TaskResult?(VMStrategyExecutorBase2.MergeResult(aggregatedResult, jobResult));
        }
      }
      return new TaskResult?((TaskResult) ((int) aggregatedResult ?? 2));
    }

    public virtual IList<JobInstance> GetNextJobs()
    {
      List<JobInstance> nextJobs = new List<JobInstance>();
      while (this.CanQueueJob())
      {
        JobInstance job = this.CreateJob();
        nextJobs.Add(job);
      }
      return (IList<JobInstance>) nextJobs;
    }

    public abstract void Initialize();

    public virtual bool IsDeploymentComplete() => !this.CanQueueJob();

    public virtual void OnJobCompleted(Guid jobId, TaskResult result)
    {
      VirtualMachineState virtualMachineState = this.m_virtualMachineGroupState.VirtualMachines.Values.FirstOrDefault<VirtualMachineState>((Func<VirtualMachineState, bool>) (v =>
      {
        Guid? jobId1 = v.JobId;
        Guid guid = jobId;
        if (!jobId1.HasValue)
          return false;
        return !jobId1.HasValue || jobId1.GetValueOrDefault() == guid;
      }));
      if (virtualMachineState == null)
        return;
      virtualMachineState.DeploymentResult = new TaskResult?(result);
    }

    public virtual void OnJobStarted(JobStartedEventData eventData)
    {
      VirtualMachineState virtualMachineState1;
      List<int> intList;
      if (!(eventData.Data is AgentJobStartedData data) || !this.m_virtualMachineGroupState.VirtualMachines.TryGetValue(data.ReservedAgent.Id, out virtualMachineState1) || !this.m_queuedJobs.TryGetValue(eventData.JobId, out intList))
        return;
      virtualMachineState1.DeploymentAttempted = true;
      virtualMachineState1.JobId = new Guid?(eventData.JobId);
      foreach (int key in intList)
      {
        VirtualMachineState virtualMachineState2;
        if (this.m_virtualMachineGroupState.VirtualMachines.TryGetValue(key, out virtualMachineState2))
          virtualMachineState2.DeploymentInQueue = false;
      }
      this.m_queuedJobs.Remove(eventData.JobId);
    }

    protected abstract List<int> GetCandidateAgents();

    protected abstract bool CanQueueJob();

    protected JobInstance CreateJob()
    {
      List<int> candidateAgents = this.GetCandidateAgents();
      AgentPoolTarget agentPoolTarget = new AgentPoolTarget();
      agentPoolTarget.Pool = new AgentPoolReference()
      {
        Id = this.m_virtualMachineGroupState.VirtualMachineGroup.PoolId
      };
      agentPoolTarget.AgentIds.AddRange((IEnumerable<int>) candidateAgents);
      Job job1 = new Job()
      {
        Name = "ProviderJob" + this.jobsCount.ToString(),
        DisplayName = this.JobDisplayNamePrefix + this.jobsCount++.ToString(),
        Target = (PhaseTarget) agentPoolTarget
      };
      DeploymentLifeCycleHookBase lifeCycleHookBase = this.m_actions.First<DeploymentLifeCycleHookBase>((Func<DeploymentLifeCycleHookBase, bool>) (a => a.Type == DeploymentLifeCycleHookType.Deploy));
      if (lifeCycleHookBase != null)
      {
        foreach (Step step in lifeCycleHookBase.Steps)
        {
          if (step.Type == StepType.Task)
            job1.Steps.Add((JobStep) (step as TaskStep));
        }
      }
      JobInstance job2 = new JobInstance(job1);
      this.m_queuedJobs.Add(new PipelineIdGenerator().GetJobInstanceId(this.m_stage.Name, this.m_phase.Name, job1.Name, job2.Attempt, 1), candidateAgents);
      return job2;
    }

    protected abstract string JobDisplayNamePrefix { get; }

    protected List<TimelineRecord> GetTimelineRecords()
    {
      List<TimelineRecord> timelineRecords = new List<TimelineRecord>();
      DateTime utcNow = DateTime.UtcNow;
      if (!this.m_virtualMachineGroupState.VirtualMachines.Any<KeyValuePair<int, VirtualMachineState>>())
      {
        timelineRecords.Add(this.CreateTimelineRecord(string.Empty, utcNow, TaskResources.NoMachineFound()));
      }
      else
      {
        foreach (VirtualMachineState machineState in this.m_virtualMachineGroupState.VirtualMachines.Values.Where<VirtualMachineState>((Func<VirtualMachineState, bool>) (a => !a.DeploymentAttempted)))
        {
          if (machineState.AgentStatus == TaskAgentStatus.Offline)
            timelineRecords.Add(this.CreateTimelineRecord(machineState.Name, utcNow, TaskResources.MachineOffline((object) machineState.Name)));
          else if (!machineState.DemandsMatched)
          {
            string logMessage = TaskResources.DemandsNotMet((object) machineState.Name, (object) string.Join<Demand>(", ", (IEnumerable<Demand>) this.m_virtualMachineGroupState.Demands.ToList<Demand>()));
            timelineRecords.Add(this.CreateTimelineRecord(machineState.Name, utcNow, logMessage));
          }
          else if (!this.IsVMGroupHealthy())
            timelineRecords.Add(this.CreateTimelineRecord(machineState.Name, utcNow, TaskResources.DeploymentStrategyNotMet((object) machineState.Name)));
          else if (machineState.IsHealthy())
            timelineRecords.Add(this.CreateTimelineRecord(machineState.Name, utcNow, TaskResources.DeploymentSkippedOnVM((object) machineState.Name)));
        }
      }
      return timelineRecords;
    }

    protected abstract bool IsVMGroupHealthy();

    protected TimelineRecord CreateTimelineRecord(
      string agentName,
      DateTime currentUtcTime,
      string logMessage)
    {
      string jobName = string.IsNullOrWhiteSpace(agentName) ? "Default" : agentName;
      PipelineIdGenerator pipelineIdGenerator = new PipelineIdGenerator();
      Guid phaseInstanceId = PipelineUtilities.GetPhaseInstanceId(this.m_stage.Name, this.m_phase.Name, this.m_phase.Attempt);
      Guid jobInstanceId = PipelineUtilities.GetJobInstanceId(this.m_stage.Name, this.m_phase.Name, jobName, this.m_phase.Attempt);
      return new TimelineRecord()
      {
        Id = jobInstanceId,
        Name = jobName,
        WorkerName = agentName,
        StartTime = new DateTime?(currentUtcTime),
        FinishTime = new DateTime?(currentUtcTime),
        RecordType = "Job",
        State = new TimelineRecordState?(TimelineRecordState.Completed),
        Result = new TaskResult?(TaskResult.Skipped),
        ParentId = new Guid?(phaseInstanceId),
        RefName = pipelineIdGenerator.GetJobInstanceName(this.m_stage.Name, this.m_phase.Name, jobName, this.m_phase.Attempt, 1),
        Identifier = pipelineIdGenerator.GetJobIdentifier(this.m_stage.Name, this.m_phase.Name, jobName, 1),
        Issues = {
          new Issue() { Type = IssueType.Error, Message = logMessage }
        }
      };
    }

    private static TaskResult MergeResult(TaskResult? aggregatedResult, TaskResult jobResult)
    {
      TaskResult taskResult1 = jobResult == TaskResult.Succeeded || jobResult == TaskResult.SucceededWithIssues ? jobResult : TaskResult.Failed;
      if (!aggregatedResult.HasValue)
        return taskResult1;
      TaskResult? nullable = aggregatedResult;
      TaskResult taskResult2 = TaskResult.SucceededWithIssues;
      if (nullable.GetValueOrDefault() == taskResult2 & nullable.HasValue || taskResult1 == TaskResult.SucceededWithIssues)
        return TaskResult.SucceededWithIssues;
      nullable = aggregatedResult;
      TaskResult taskResult3 = TaskResult.Succeeded;
      if (nullable.GetValueOrDefault() == taskResult3 & nullable.HasValue && taskResult1 == TaskResult.Succeeded)
      {
        aggregatedResult = new TaskResult?(TaskResult.Succeeded);
      }
      else
      {
        nullable = aggregatedResult;
        TaskResult taskResult4 = TaskResult.Failed;
        aggregatedResult = !(nullable.GetValueOrDefault() == taskResult4 & nullable.HasValue) || taskResult1 != TaskResult.Failed ? new TaskResult?(TaskResult.SucceededWithIssues) : new TaskResult?(TaskResult.Failed);
      }
      return aggregatedResult.Value;
    }
  }
}
