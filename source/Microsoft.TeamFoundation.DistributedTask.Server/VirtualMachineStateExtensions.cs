// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VirtualMachineStateExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class VirtualMachineStateExtensions
  {
    public static bool IsHealthy(this VirtualMachineState machineState)
    {
      if (machineState.AgentStatus != TaskAgentStatus.Online || machineState.DeploymentInQueue)
        return false;
      if (!machineState.DeploymentAttempted)
        return true;
      TaskResult? deploymentResult = machineState.DeploymentResult;
      if (!deploymentResult.HasValue)
        return false;
      deploymentResult = machineState.DeploymentResult;
      return deploymentResult.Value == TaskResult.Succeeded;
    }

    public static bool CanAttemptDeployment(this VirtualMachineState machineState) => machineState.AgentStatus == TaskAgentStatus.Online && machineState.DemandsMatched && !machineState.DeploymentInQueue && !machineState.DeploymentAttempted;
  }
}
