// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VMStrategyExecutorBase3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal abstract class VMStrategyExecutorBase3 : IStrategyExecutor2
  {
    protected RunDeploymentPhaseInput2 m_phaseInput;
    protected VMDeploymentExecutionState m_vmDeploymentExecutionState;

    public VMStrategyExecutorBase3(
      RunDeploymentPhaseInput2 phaseInput,
      VMDeploymentExecutionState vmDeploymentExecutionState)
    {
      this.m_phaseInput = phaseInput;
      this.m_vmDeploymentExecutionState = vmDeploymentExecutionState;
    }

    public virtual void Initialize()
    {
    }

    public IList<RunDeploymentLifeCycleInput> GetNextLifeCycles(
      RunDeploymentPhaseInput2 phaseInput,
      out List<int> newTargetResourceIds)
    {
      newTargetResourceIds = new List<int>();
      List<RunDeploymentLifeCycleInput> nextLifeCycles = new List<RunDeploymentLifeCycleInput>();
      RunDeploymentLifeCycleInput lifeCycleInput;
      do
      {
        lifeCycleInput = this.CreateLifeCycleInput(phaseInput, newTargetResourceIds);
        if (lifeCycleInput != null)
          nextLifeCycles.Add(lifeCycleInput);
      }
      while (lifeCycleInput != null);
      return (IList<RunDeploymentLifeCycleInput>) nextLifeCycles;
    }

    public void OnLifeCycleCompleted(
      string cycleInstanceName,
      TaskResult result,
      out Dictionary<int, TaskResult> resourceDeploymentResult)
    {
      resourceDeploymentResult = new Dictionary<int, TaskResult>();
      VMResourceState vmResourceState = this.m_vmDeploymentExecutionState.VMResources.FirstOrDefault<VMResourceState>((Func<VMResourceState, bool>) (vm => string.Equals(vm.LifeCycleInstanceName, cycleInstanceName)));
      if (vmResourceState == null)
        return;
      vmResourceState.DeploymentResult = new TaskResult?(result);
      resourceDeploymentResult.Add(vmResourceState.ResourceId, result);
    }

    public bool IsDeploymentCompleted() => !this.m_vmDeploymentExecutionState.IsIterationCycleRunning() && !this.CanQueueNextIteration();

    public TaskResult? GetDeploymentResult(out IList<TimelineRecord> timelineRecords)
    {
      timelineRecords = (IList<TimelineRecord>) new List<TimelineRecord>();
      if (!this.IsDeploymentCompleted())
        return new TaskResult?();
      timelineRecords = (IList<TimelineRecord>) this.GetTimelineRecords();
      TaskResult result = TaskResult.Succeeded;
      foreach (VMResourceState vmResource in this.m_vmDeploymentExecutionState.VMResources)
      {
        if (!vmResource.DeploymentAttempted)
          return new TaskResult?(TaskResult.Failed);
        if (vmResource.DeploymentAttempted)
        {
          TaskResult childResult = (TaskResult) ((int) vmResource.DeploymentResult ?? 2);
          result = PipelineUtilities.MergeResult(result, childResult);
        }
      }
      return new TaskResult?(result);
    }

    private RunDeploymentLifeCycleInput CreateLifeCycleInput(
      RunDeploymentPhaseInput2 phaseInput,
      List<int> targetResourceIds)
    {
      if (!this.CanQueueNextIteration())
        return (RunDeploymentLifeCycleInput) null;
      VMResourceState candidateResource = this.GetNextCandidateResource();
      if (candidateResource == null)
        return (RunDeploymentLifeCycleInput) null;
      List<DeploymentLifeCycleHookBase> hooks = this.GetLifeCycleHooks(new List<DeploymentLifeCycleHookType>()
      {
        DeploymentLifeCycleHookType.PreDeploy,
        DeploymentLifeCycleHookType.Deploy,
        DeploymentLifeCycleHookType.RouteTraffic,
        DeploymentLifeCycleHookType.PostRouteTraffic,
        DeploymentLifeCycleHookType.OnFailure,
        DeploymentLifeCycleHookType.OnSuccess
      });
      if (hooks.Count == 0)
      {
        DeployHook deployHook = new DeployHook();
        deployHook.Target = this.m_phaseInput.ProviderPhase?.Target?.Clone();
        hooks = new List<DeploymentLifeCycleHookBase>()
        {
          (DeploymentLifeCycleHookBase) deployHook
        };
      }
      foreach (DeploymentLifeCycleHookBase lifeCycleHookBase in hooks)
        lifeCycleHookBase.Target = this.GetPoolTarget(lifeCycleHookBase.Target, candidateResource);
      RunDeploymentLifeCycleInput lifeCycleInput = this.CreateLifeCycleInput(phaseInput, (IList<DeploymentLifeCycleHookBase>) hooks, candidateResource);
      candidateResource.LifeCycleInstanceName = lifeCycleInput.LifeCycleInstanceName;
      targetResourceIds.Add(candidateResource.ResourceId);
      return lifeCycleInput;
    }

    private List<DeploymentLifeCycleHookBase> GetLifeCycleHooks(
      List<DeploymentLifeCycleHookType> hookTypes)
    {
      return this.m_phaseInput.Strategy.Hooks.Where<DeploymentLifeCycleHookBase>((Func<DeploymentLifeCycleHookBase, bool>) (h => hookTypes.Contains(h.Type))).Select<DeploymentLifeCycleHookBase, DeploymentLifeCycleHookBase>((Func<DeploymentLifeCycleHookBase, DeploymentLifeCycleHookBase>) (h => h.Clone())).ToList<DeploymentLifeCycleHookBase>();
    }

    private RunDeploymentLifeCycleInput CreateLifeCycleInput(
      RunDeploymentPhaseInput2 phaseInput,
      IList<DeploymentLifeCycleHookBase> hooks,
      VMResourceState targetResource)
    {
      if (hooks == null || !hooks.Any<DeploymentLifeCycleHookBase>())
        return (RunDeploymentLifeCycleInput) null;
      string resourceName = targetResource.ResourceName;
      RunDeploymentLifeCycleInput lifeCycleInput = new RunDeploymentLifeCycleInput()
      {
        ScopeId = phaseInput.ScopeId,
        PlanId = phaseInput.PlanId,
        PlanType = phaseInput.PlanType,
        Stage = phaseInput.Stage,
        Phase = phaseInput.Phase,
        Attempt = phaseInput.Phase.Attempt,
        LifeCycleInstanceName = this.GetLifeCycleInstanceName(resourceName),
        LifeCycleInstanceNameFormat = this.GetLifeCycleInstanceNameFormat(resourceName),
        HookInstanceDisplayNameFormat = this.GetJobDisplayNameFormat(resourceName),
        Version = phaseInput.Version
      };
      this.SetLifeCycleHooks(lifeCycleInput, hooks);
      this.SetLifeCycleVariables(lifeCycleInput, targetResource, resourceName);
      return lifeCycleInput;
    }

    private void SetLifeCycleVariables(
      RunDeploymentLifeCycleInput lifeCycleInput,
      VMResourceState targetResource,
      string cycleName)
    {
      List<IVariable> collection = new List<IVariable>();
      collection.AddRange((IEnumerable<IVariable>) new List<IVariable>()
      {
        (IVariable) new Variable()
        {
          Name = "Strategy.CycleName",
          Value = cycleName
        },
        (IVariable) new Variable()
        {
          Name = "Strategy.Name",
          Value = DeploymentPhaseHelper.GetStrategyName(this.m_phaseInput.Strategy.Type)
        },
        (IVariable) new Variable()
        {
          Name = PipelineConstants.EnvironmentVariables.EnvironmentResourceId,
          Value = targetResource.ResourceId.ToString()
        },
        (IVariable) new Variable()
        {
          Name = PipelineConstants.EnvironmentVariables.EnvironmentResourceName,
          Value = targetResource.ResourceName
        }
      });
      if (!(lifeCycleInput.Variables is List<IVariable> variables))
        return;
      variables.AddRange((IEnumerable<IVariable>) collection);
    }

    private void SetLifeCycleHooks(
      RunDeploymentLifeCycleInput lifeCycleInput,
      IList<DeploymentLifeCycleHookBase> hooks)
    {
      foreach (DeploymentLifeCycleHookBase hook in (IEnumerable<DeploymentLifeCycleHookBase>) hooks)
        lifeCycleInput.LifeCycleHooks.Add(hook);
    }

    private List<TimelineRecord> GetTimelineRecords()
    {
      List<TimelineRecord> timelineRecords = new List<TimelineRecord>();
      foreach (VMResourceState resourceState in this.m_vmDeploymentExecutionState.VMResources.Where<VMResourceState>((Func<VMResourceState, bool>) (a => !a.DeploymentAttempted)))
      {
        if (resourceState.AgentStatus == TaskAgentStatus.Offline)
          timelineRecords.Add(this.CreateTimelineRecord(resourceState.AgentName, resourceState.ResourceName, TaskResources.MachineOffline((object) resourceState.ResourceName)));
        else if (!this.IsVMGroupHealthy())
          timelineRecords.Add(this.CreateTimelineRecord(resourceState.AgentName, resourceState.ResourceName, TaskResources.DeploymentStrategyNotMet((object) resourceState.ResourceName)));
        else if (resourceState.IsHealthy())
          timelineRecords.Add(this.CreateTimelineRecord(resourceState.AgentName, resourceState.ResourceName, TaskResources.DeploymentSkippedOnVM((object) resourceState.ResourceName)));
      }
      return timelineRecords;
    }

    protected abstract bool IsVMGroupHealthy();

    protected abstract bool CanQueueNextIteration();

    private TimelineRecord CreateTimelineRecord(
      string agentName,
      string resourceName,
      string logMessage)
    {
      string jobName = this.GetPhaseDisplayName() + "_" + resourceName;
      PipelineIdGenerator pipelineIdGenerator = new PipelineIdGenerator();
      Guid phaseInstanceId = PipelineUtilities.GetPhaseInstanceId(this.m_phaseInput.Stage.Name, this.m_phaseInput.Phase.Name, this.m_phaseInput.Phase.Attempt);
      Guid jobInstanceId = PipelineUtilities.GetJobInstanceId(this.m_phaseInput.Stage.Name, this.m_phaseInput.Phase.Name, jobName, this.m_phaseInput.Phase.Attempt);
      return new TimelineRecord()
      {
        Id = jobInstanceId,
        Name = jobName,
        WorkerName = agentName,
        RecordType = "Job",
        State = new TimelineRecordState?(TimelineRecordState.Completed),
        Result = new TaskResult?(TaskResult.Skipped),
        ParentId = new Guid?(phaseInstanceId),
        RefName = pipelineIdGenerator.GetJobInstanceName(this.m_phaseInput.Stage.Name, this.m_phaseInput.Phase.Name, jobName, this.m_phaseInput.Phase.Attempt, 1),
        Identifier = pipelineIdGenerator.GetJobIdentifier(this.m_phaseInput.Stage.Name, this.m_phaseInput.Phase.Name, jobName, 1),
        Issues = {
          new Issue() { Type = IssueType.Error, Message = logMessage }
        }
      };
    }

    private VMResourceState GetNextCandidateResource()
    {
      foreach (VMResourceState vmResource in this.m_vmDeploymentExecutionState.VMResources)
      {
        if (vmResource.CanAttemptDeployment())
        {
          vmResource.DeploymentAttempted = true;
          return vmResource;
        }
      }
      return (VMResourceState) null;
    }

    private PhaseTarget GetPoolTarget(PhaseTarget baseTarget, VMResourceState targetResource)
    {
      if (baseTarget is ServerTarget)
        return baseTarget;
      AgentPoolTarget poolTarget = new AgentPoolTarget();
      poolTarget.Pool = new AgentPoolReference()
      {
        Id = this.m_vmDeploymentExecutionState.DeploymentPool.Id
      };
      poolTarget.AgentIds.Add(targetResource.AgentId);
      poolTarget.TimeoutInMinutes = baseTarget?.TimeoutInMinutes;
      poolTarget.CancelTimeoutInMinutes = baseTarget?.CancelTimeoutInMinutes;
      if (baseTarget is AgentQueueTarget agentQueueTarget)
        poolTarget.Workspace = agentQueueTarget?.Workspace?.Clone();
      else if (baseTarget is AgentPoolTarget agentPoolTarget)
        poolTarget.Workspace = agentPoolTarget?.Workspace?.Clone();
      if (baseTarget != null)
      {
        int? count = baseTarget.Demands?.Count;
        int num = 0;
        if (count.GetValueOrDefault() > num & count.HasValue)
          poolTarget.Demands.UnionWith((IEnumerable<Demand>) new HashSet<Demand>(baseTarget.Demands.Select<Demand, Demand>((Func<Demand, Demand>) (x => x.Clone()))));
      }
      return (PhaseTarget) poolTarget;
    }

    private string GetLifeCycleInstanceName(string resourceName) => this.m_phaseInput.ProviderPhase.Name + "_" + resourceName;

    private string GetLifeCycleInstanceNameFormat(string resourceName) => "{0}" + "_" + resourceName;

    private string GetJobDisplayNameFormat(string resourceName) => this.GetPhaseDisplayName() + "_{0}_" + resourceName;

    private string GetPhaseDisplayName() => !string.IsNullOrWhiteSpace(this.m_phaseInput.ProviderPhase.DisplayName) ? this.m_phaseInput.ProviderPhase.DisplayName : this.m_phaseInput.ProviderPhase.Name;
  }
}
