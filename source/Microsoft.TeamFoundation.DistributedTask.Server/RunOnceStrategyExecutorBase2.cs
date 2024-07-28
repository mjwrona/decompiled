// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.RunOnceStrategyExecutorBase2
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
  internal abstract class RunOnceStrategyExecutorBase2 : BaseStrategyExecutorBase
  {
    public RunOnceStrategyExecutorBase2(RunOnceDeploymentStrategy2 strategy, ProviderPhase phase)
      : base((DeploymentStrategyBase2) strategy, phase)
    {
    }

    protected override RunDeploymentLifeCycleInput CreateIterationLifeCycleInput(
      RunDeploymentPhaseInput2 phaseInput)
    {
      List<DeploymentLifeCycleHookType> iterationHookTypes = new List<DeploymentLifeCycleHookType>()
      {
        DeploymentLifeCycleHookType.Deploy,
        DeploymentLifeCycleHookType.RouteTraffic,
        DeploymentLifeCycleHookType.PostRouteTraffic
      };
      RunDeploymentLifeCycleInput lifeCycleInput = this.CreateLifeCycleInput(phaseInput, (IList<DeploymentLifeCycleHookBase>) this.m_strategy.Hooks.Where<DeploymentLifeCycleHookBase>((Func<DeploymentLifeCycleHookBase, bool>) (h => iterationHookTypes.Contains(h.Type))).ToList<DeploymentLifeCycleHookBase>(), "Iteration");
      this.m_deploymentStage = "PostIteration";
      return lifeCycleInput;
    }

    protected override void SetLifeCycleVariables(
      RunDeploymentLifeCycleInput lifeCycleInput,
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
          Value = "runOnce"
        }
      });
      if (!(lifeCycleInput.Variables is List<IVariable> variables))
        return;
      variables.AddRange((IEnumerable<IVariable>) collection);
    }

    protected override void SetLifeCycleInstanceNameFormat(
      RunDeploymentLifeCycleInput lifeCycleInput,
      string cycleName)
    {
      if (this.StrategyContainsOnlyDeployHook())
        lifeCycleInput.LifeCycleInstanceNameFormat = this.m_providerPhase.Name;
      else
        lifeCycleInput.LifeCycleInstanceNameFormat = "{0}";
    }

    protected override void SetHookInstanceDisplayNameFormat(
      RunDeploymentLifeCycleInput lifeCycleInput,
      string cycleName)
    {
      if (lifeCycleInput.Version < 4 || !this.StrategyContainsOnlyDeployHook())
        base.SetHookInstanceDisplayNameFormat(lifeCycleInput, cycleName);
      else
        lifeCycleInput.HookInstanceDisplayNameFormat = string.IsNullOrWhiteSpace(this.m_providerPhase.DisplayName) ? this.m_providerPhase.Name : this.m_providerPhase.DisplayName;
    }

    private bool StrategyContainsOnlyDeployHook() => this.m_strategy.Hooks.Count == 1 && this.m_strategy.Hooks[0].Type == DeploymentLifeCycleHookType.Deploy;

    protected override void AddPostIterationVariables(
      RunDeploymentLifeCycleInput lifeCycleInput,
      DeploymentLifeCycleHookType hookType)
    {
    }
  }
}
