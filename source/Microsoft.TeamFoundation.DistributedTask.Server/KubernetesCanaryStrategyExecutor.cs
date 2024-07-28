// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.KubernetesCanaryStrategyExecutor
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
  internal sealed class KubernetesCanaryStrategyExecutor : BaseStrategyExecutorBase
  {
    private int m_iterationCount;
    private int m_currentIteration;
    private IList<int> m_deploymentIncrements;

    public KubernetesCanaryStrategyExecutor(CanaryDeploymentStrategy strategy, ProviderPhase phase)
      : base((DeploymentStrategyBase2) strategy, phase)
    {
      this.m_deploymentIncrements = strategy.DeploymentIncrements;
      this.m_iterationCount = strategy.DeploymentIncrements.Count + 1;
      this.m_currentIteration = 1;
    }

    protected override RunDeploymentLifeCycleInput CreateIterationLifeCycleInput(
      RunDeploymentPhaseInput2 phaseInput)
    {
      string cycleName = "Iteration" + this.m_currentIteration.ToString();
      List<DeploymentLifeCycleHookType> iterationHookTypes = new List<DeploymentLifeCycleHookType>()
      {
        DeploymentLifeCycleHookType.Deploy,
        DeploymentLifeCycleHookType.RouteTraffic,
        DeploymentLifeCycleHookType.PostRouteTraffic
      };
      RunDeploymentLifeCycleInput lifeCycleInput = this.CreateLifeCycleInput(phaseInput, (IList<DeploymentLifeCycleHookBase>) this.m_strategy.Hooks.Where<DeploymentLifeCycleHookBase>((Func<DeploymentLifeCycleHookBase, bool>) (h => iterationHookTypes.Contains(h.Type))).ToList<DeploymentLifeCycleHookBase>(), cycleName);
      this.AddIterationVariables(lifeCycleInput);
      ++this.m_currentIteration;
      this.m_deploymentStage = this.m_currentIteration > this.m_iterationCount ? "PostIteration" : "Iteration";
      return lifeCycleInput;
    }

    protected override void SetLifeCycleInstanceNameFormat(
      RunDeploymentLifeCycleInput lifeCycleInput,
      string cycleName)
    {
      if (this.IsIterationCycle(cycleName))
      {
        int num = this.m_currentIteration < this.m_iterationCount ? this.m_deploymentIncrements[this.m_currentIteration - 1] : 100;
        lifeCycleInput.LifeCycleInstanceNameFormat = "{0}_" + (object) num;
      }
      else
        lifeCycleInput.LifeCycleInstanceNameFormat = "{0}";
    }

    protected override void SetHookInstanceDisplayNameFormat(
      RunDeploymentLifeCycleInput lifeCycleInput,
      string cycleName)
    {
      if (!this.IsIterationCycle(cycleName))
      {
        base.SetHookInstanceDisplayNameFormat(lifeCycleInput, cycleName);
      }
      else
      {
        string str = string.IsNullOrWhiteSpace(this.m_providerPhase.DisplayName) ? this.m_providerPhase.Name : this.m_providerPhase.DisplayName;
        if (this.m_currentIteration < this.m_iterationCount)
          lifeCycleInput.HookInstanceDisplayNameFormat = str + "_" + "{0}" + "_" + (object) this.m_deploymentIncrements[this.m_currentIteration - 1];
        else
          lifeCycleInput.HookInstanceDisplayNameFormat = str + "_" + "{0}" + "_" + (object) 100;
      }
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
          Value = "canary"
        }
      });
      if (!(lifeCycleInput.Variables is List<IVariable> variables))
        return;
      variables.AddRange((IEnumerable<IVariable>) collection);
    }

    private bool IsIterationCycle(string cycleName) => cycleName.StartsWith("Iteration", StringComparison.InvariantCultureIgnoreCase);

    private void AddIterationVariables(RunDeploymentLifeCycleInput lifeCycleInput)
    {
      if (lifeCycleInput == null)
        return;
      int num;
      string str;
      if (this.m_currentIteration == this.m_iterationCount)
      {
        num = 100;
        str = "promote";
      }
      else
      {
        num = this.m_deploymentIncrements[this.m_currentIteration - 1];
        str = "deploy";
      }
      List<Variable> collection = new List<Variable>()
      {
        new Variable()
        {
          Name = "Strategy.Increment",
          Value = num.ToString()
        },
        new Variable() { Name = "Strategy.Action", Value = str }
      };
      if (!(lifeCycleInput.Variables is List<IVariable> variables))
        return;
      variables.AddRange((IEnumerable<IVariable>) collection);
    }

    protected override void AddPostIterationVariables(
      RunDeploymentLifeCycleInput lifeCycleInput,
      DeploymentLifeCycleHookType hookType)
    {
      if (lifeCycleInput == null || hookType != DeploymentLifeCycleHookType.OnFailure)
        return;
      List<Variable> collection = new List<Variable>()
      {
        new Variable()
        {
          Name = "Strategy.Action",
          Value = "reject"
        }
      };
      if (!(lifeCycleInput.Variables is List<IVariable> variables))
        return;
      variables.AddRange((IEnumerable<IVariable>) collection);
    }
  }
}
