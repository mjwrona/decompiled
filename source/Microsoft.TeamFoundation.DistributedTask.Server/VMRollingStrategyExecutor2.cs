// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VMRollingStrategyExecutor2
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
  internal sealed class VMRollingStrategyExecutor2 : VMStrategyExecutorBase2
  {
    private RollingDeploymentStrategy2 m_strategy;
    private int m_maxUnhealthyVMs;

    public VMRollingStrategyExecutor2(
      VirtualMachineGroupState virtualMachineGroupState,
      RollingDeploymentStrategy2 strategy,
      PipelineGraphNodeReference stage,
      PipelineGraphNodeReference phase)
      : base(virtualMachineGroupState, stage, phase, strategy.Hooks)
    {
      this.m_strategy = strategy;
    }

    public override void Initialize()
    {
      int val2 = this.m_virtualMachineGroupState.VirtualMachines.Values.Count<VirtualMachineState>();
      if (this.m_strategy.DeploymentOption == RollingDeploymentOption.Absolute)
        this.m_maxUnhealthyVMs = Math.Min(this.m_strategy.DeploymentOptionValue, val2);
      else
        this.m_maxUnhealthyVMs = val2 * this.m_strategy.DeploymentOptionValue / 100;
    }

    protected override string JobDisplayNamePrefix => "Rolling Job";

    protected override bool CanQueueJob()
    {
      int num = 0;
      bool flag1 = false;
      bool flag2 = true;
      foreach (VirtualMachineState machineState in (IEnumerable<VirtualMachineState>) this.m_virtualMachineGroupState.VirtualMachines.Values)
      {
        if (!machineState.IsHealthy())
          ++num;
        if (!machineState.DeploymentAttempted)
          flag2 = false;
        if (machineState.CanAttemptDeployment())
          flag1 = true;
      }
      return ((flag2 ? 0 : (num < this.m_maxUnhealthyVMs ? 1 : 0)) & (flag1 ? 1 : 0)) != 0;
    }

    protected override bool IsVMGroupHealthy()
    {
      int num = 0;
      foreach (VirtualMachineState machineState in (IEnumerable<VirtualMachineState>) this.m_virtualMachineGroupState.VirtualMachines.Values)
      {
        if (!machineState.IsHealthy())
          ++num;
      }
      return num <= this.m_maxUnhealthyVMs;
    }

    protected override List<int> GetCandidateAgents()
    {
      List<int> candidateAgents = new List<int>();
      foreach (VirtualMachineState machineState in (IEnumerable<VirtualMachineState>) this.m_virtualMachineGroupState.VirtualMachines.Values)
      {
        if (machineState.CanAttemptDeployment())
        {
          machineState.DeploymentInQueue = true;
          candidateAgents.Add(machineState.Id);
        }
      }
      return candidateAgents;
    }
  }
}
