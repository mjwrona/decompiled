// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VMRunOnceStrategyExecutor2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class VMRunOnceStrategyExecutor2 : VMStrategyExecutorBase2
  {
    private RunOnceDeploymentStrategy2 m_strategy;

    public VMRunOnceStrategyExecutor2(
      VirtualMachineGroupState virtualMachineGroupState,
      RunOnceDeploymentStrategy2 strategy,
      PipelineGraphNodeReference stage,
      PipelineGraphNodeReference phase)
      : base(virtualMachineGroupState, stage, phase, strategy.Hooks)
    {
      this.m_strategy = strategy;
    }

    protected override string JobDisplayNamePrefix => "Run Once Job";

    protected override bool CanQueueJob()
    {
      bool flag1 = false;
      bool flag2 = true;
      foreach (VirtualMachineState machineState in (IEnumerable<VirtualMachineState>) this.m_virtualMachineGroupState.VirtualMachines.Values)
      {
        if (!machineState.DeploymentAttempted)
          flag2 = false;
        if (machineState.CanAttemptDeployment())
          flag1 = true;
      }
      return !flag2 & flag1;
    }

    protected override List<int> GetCandidateAgents()
    {
      foreach (VirtualMachineState machineState in (IEnumerable<VirtualMachineState>) this.m_virtualMachineGroupState.VirtualMachines.Values)
      {
        if (machineState.CanAttemptDeployment())
        {
          machineState.DeploymentInQueue = true;
          return new List<int>() { machineState.Id };
        }
      }
      return new List<int>();
    }

    protected override bool IsVMGroupHealthy() => true;

    public override void Initialize()
    {
    }
  }
}
