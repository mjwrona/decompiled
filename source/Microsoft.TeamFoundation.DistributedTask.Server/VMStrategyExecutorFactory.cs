// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VMStrategyExecutorFactory
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class VMStrategyExecutorFactory
  {
    public static IStrategyExecutor GetDeploymentStrategyExecutor(
      VirtualMachineGroupState virtualMachineGroupState,
      DeploymentStrategyBase strategy,
      PipelineGraphNodeReference stage,
      PipelineGraphNodeReference phase)
    {
      switch (strategy.Type)
      {
        case DeploymentStrategyType.RunOnce:
          return (IStrategyExecutor) new VMRunOnceStrategyExecutor(virtualMachineGroupState, (RunOnceDeploymentStrategy) strategy, stage, phase);
        case DeploymentStrategyType.Rolling:
          return (IStrategyExecutor) new VMRollingStrategyExecutor(virtualMachineGroupState, (RollingDeploymentStrategy) strategy, stage, phase);
        default:
          throw new NotSupportedException(TaskResources.InvalidDeploymentStrategy((object) strategy.Type, (object) "VirtualMachine"));
      }
    }
  }
}
