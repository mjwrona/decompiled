// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.BaseStrategyExecutorBase
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
  internal abstract class BaseStrategyExecutorBase : IStrategyExecutor2
  {
    protected TaskResult m_currentResult;
    protected TaskResult? m_phaseResult;
    protected DeploymentStrategyBase2 m_strategy;
    protected ProviderPhase m_providerPhase;
    protected string m_deploymentStage;

    public BaseStrategyExecutorBase(DeploymentStrategyBase2 strategy, ProviderPhase phase)
    {
      this.m_strategy = strategy;
      this.m_providerPhase = phase;
      this.m_deploymentStage = "PreIteration";
      this.m_currentResult = TaskResult.Succeeded;
      this.m_phaseResult = new TaskResult?();
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
      RunDeploymentLifeCycleInput iterationLifeCycleInput;
      switch (this.m_deploymentStage)
      {
        case "PreIteration":
          iterationLifeCycleInput = this.CreatePreIterationLifeCycleInput(phaseInput);
          break;
        case "Iteration":
          iterationLifeCycleInput = this.CreateIterationLifeCycleInput(phaseInput);
          break;
        case "PostIteration":
          iterationLifeCycleInput = this.CreatePostIterationLifeCycleInput(phaseInput);
          break;
        default:
          return (IList<RunDeploymentLifeCycleInput>) new List<RunDeploymentLifeCycleInput>();
      }
      if (iterationLifeCycleInput != null)
        nextLifeCycles.Add(iterationLifeCycleInput);
      if (iterationLifeCycleInput == null && this.m_deploymentStage != "Completed")
        nextLifeCycles.AddRange((IEnumerable<RunDeploymentLifeCycleInput>) this.GetNextLifeCycles(phaseInput, out newTargetResourceIds));
      return (IList<RunDeploymentLifeCycleInput>) nextLifeCycles;
    }

    public void OnLifeCycleCompleted(
      string cycleInstanceName,
      TaskResult result,
      out Dictionary<int, TaskResult> resourceDeploymentResult)
    {
      resourceDeploymentResult = new Dictionary<int, TaskResult>();
      this.m_currentResult = PipelineUtilities.MergeResult(this.m_currentResult, result);
      if (this.m_currentResult == TaskResult.Failed && !this.m_deploymentStage.Equals("Completed"))
        this.m_deploymentStage = "PostIteration";
      if (this.m_currentResult != TaskResult.Canceled && !this.m_deploymentStage.Equals("Completed"))
        return;
      this.m_phaseResult = new TaskResult?(this.m_currentResult);
    }

    public bool IsDeploymentCompleted() => this.m_phaseResult.HasValue;

    public TaskResult? GetDeploymentResult(out IList<TimelineRecord> timelineRecords)
    {
      timelineRecords = (IList<TimelineRecord>) null;
      return this.m_phaseResult;
    }

    protected virtual RunDeploymentLifeCycleInput CreatePreIterationLifeCycleInput(
      RunDeploymentPhaseInput2 phaseInput)
    {
      RunDeploymentLifeCycleInput lifeCycleInput = this.CreateLifeCycleInput(phaseInput, (IList<DeploymentLifeCycleHookBase>) this.m_strategy.Hooks.Where<DeploymentLifeCycleHookBase>((Func<DeploymentLifeCycleHookBase, bool>) (h => h.Type == DeploymentLifeCycleHookType.PreDeploy)).ToList<DeploymentLifeCycleHookBase>(), "PreIteration");
      this.m_deploymentStage = "Iteration";
      return lifeCycleInput;
    }

    protected virtual RunDeploymentLifeCycleInput CreatePostIterationLifeCycleInput(
      RunDeploymentPhaseInput2 phaseInput)
    {
      DeploymentLifeCycleHookType finalHookType = this.m_currentResult == TaskResult.Failed ? DeploymentLifeCycleHookType.OnFailure : DeploymentLifeCycleHookType.OnSuccess;
      RunDeploymentLifeCycleInput lifeCycleInput = this.CreateLifeCycleInput(phaseInput, (IList<DeploymentLifeCycleHookBase>) this.m_strategy.Hooks.Where<DeploymentLifeCycleHookBase>((Func<DeploymentLifeCycleHookBase, bool>) (h => h.Type == finalHookType)).ToList<DeploymentLifeCycleHookBase>(), "PostIteration");
      this.AddPostIterationVariables(lifeCycleInput, finalHookType);
      this.m_deploymentStage = "Completed";
      if (lifeCycleInput == null)
        this.m_phaseResult = new TaskResult?(this.m_currentResult);
      return lifeCycleInput;
    }

    protected RunDeploymentLifeCycleInput CreateLifeCycleInput(
      RunDeploymentPhaseInput2 phaseInput,
      IList<DeploymentLifeCycleHookBase> hooks,
      string cycleName)
    {
      if (hooks == null || !hooks.Any<DeploymentLifeCycleHookBase>())
        return (RunDeploymentLifeCycleInput) null;
      RunDeploymentLifeCycleInput lifeCycleInput = new RunDeploymentLifeCycleInput()
      {
        ScopeId = phaseInput.ScopeId,
        PlanId = phaseInput.PlanId,
        PlanType = phaseInput.PlanType,
        Stage = phaseInput.Stage,
        Phase = phaseInput.Phase,
        LifeCycleInstanceName = this.GetDefaultLifeCycleInstanceName(cycleName),
        Attempt = phaseInput.Phase.Attempt,
        Version = phaseInput.Version
      };
      this.SetLifeCycleHooks(lifeCycleInput, hooks);
      this.SetLifeCycleInstanceNameFormat(lifeCycleInput, cycleName);
      this.SetHookInstanceDisplayNameFormat(lifeCycleInput, cycleName);
      this.SetLifeCycleVariables(lifeCycleInput, cycleName);
      return lifeCycleInput;
    }

    protected virtual void SetHookInstanceDisplayNameFormat(
      RunDeploymentLifeCycleInput lifeCycleInput,
      string cycleName)
    {
      lifeCycleInput.HookInstanceDisplayNameFormat = this.GetDefaultHookInstanceDisplayNameFormat();
    }

    protected abstract RunDeploymentLifeCycleInput CreateIterationLifeCycleInput(
      RunDeploymentPhaseInput2 phaseInput);

    protected abstract void SetLifeCycleVariables(
      RunDeploymentLifeCycleInput lifeCycleInput,
      string cycleName);

    protected abstract void SetLifeCycleInstanceNameFormat(
      RunDeploymentLifeCycleInput lifeCycleInput,
      string cycleName);

    protected abstract void AddPostIterationVariables(
      RunDeploymentLifeCycleInput lifeCycleInput,
      DeploymentLifeCycleHookType hookType);

    private void SetLifeCycleHooks(
      RunDeploymentLifeCycleInput lifeCycleInput,
      IList<DeploymentLifeCycleHookBase> hooks)
    {
      foreach (DeploymentLifeCycleHookBase hook in (IEnumerable<DeploymentLifeCycleHookBase>) hooks)
        lifeCycleInput.LifeCycleHooks.Add(hook);
    }

    private string GetDefaultLifeCycleInstanceName(string cycleName) => this.m_providerPhase.Name + "_" + cycleName;

    private string GetDefaultHookInstanceDisplayNameFormat() => (string.IsNullOrWhiteSpace(this.m_providerPhase.DisplayName) ? this.m_providerPhase.Name : this.m_providerPhase.DisplayName) + "_" + "{0}";
  }
}
