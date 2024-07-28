// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VMRunOnceStrategyExecutor3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class VMRunOnceStrategyExecutor3 : VMStrategyExecutorBase3
  {
    public VMRunOnceStrategyExecutor3(
      RunDeploymentPhaseInput2 phaseInput,
      VMDeploymentExecutionState vmDeploymentExecutionState)
      : base(phaseInput, vmDeploymentExecutionState)
    {
    }

    protected override bool CanQueueNextIteration() => this.m_vmDeploymentExecutionState.VMResources.Any<VMResourceState>((Func<VMResourceState, bool>) (vm => vm.CanAttemptDeployment()));

    protected override bool IsVMGroupHealthy() => true;
  }
}
