// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VMRollingStrategyExecutor3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class VMRollingStrategyExecutor3 : VMStrategyExecutorBase3
  {
    private int m_maxUnhealthyVMs;

    public VMRollingStrategyExecutor3(
      RunDeploymentPhaseInput2 phaseInput,
      VMDeploymentExecutionState vmDeploymentExecutionState)
      : base(phaseInput, vmDeploymentExecutionState)
    {
      this.m_maxUnhealthyVMs = 0;
    }

    public override void Initialize()
    {
      base.Initialize();
      int val2 = this.m_vmDeploymentExecutionState.VMResources.Count<VMResourceState>();
      RollingDeploymentStrategy2 strategy = (RollingDeploymentStrategy2) this.m_phaseInput.Strategy;
      if (strategy.DeploymentOption == RollingDeploymentOption.Absolute)
        this.m_maxUnhealthyVMs = Math.Min(strategy.DeploymentOptionValue, val2);
      else
        this.m_maxUnhealthyVMs = val2 * strategy.DeploymentOptionValue / 100;
    }

    protected override bool CanQueueNextIteration()
    {
      int num = 0;
      bool flag = false;
      foreach (VMResourceState vmResource in this.m_vmDeploymentExecutionState.VMResources)
      {
        if (!vmResource.IsHealthy())
          ++num;
        if (vmResource.CanAttemptDeployment())
          flag = true;
      }
      return num < this.m_maxUnhealthyVMs & flag;
    }

    protected override bool IsVMGroupHealthy()
    {
      int num = 0;
      foreach (VMResourceState vmResource in this.m_vmDeploymentExecutionState.VMResources)
      {
        if (!vmResource.IsHealthy())
          ++num;
      }
      return num < this.m_maxUnhealthyVMs;
    }
  }
}
