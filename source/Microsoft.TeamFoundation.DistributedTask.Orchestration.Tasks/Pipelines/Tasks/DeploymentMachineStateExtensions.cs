// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.DeploymentMachineStateExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  internal static class DeploymentMachineStateExtensions
  {
    public static bool IsHealthy(this DeploymentMachineState machine)
    {
      if (machine.Excluded || machine.AgentStatus != TaskAgentStatus.Online || machine.DeploymentInQueue)
        return false;
      if (!machine.DeploymentAttempted)
        return true;
      TaskResult? deploymentResult = machine.DeploymentResult;
      if (!deploymentResult.HasValue)
        return false;
      deploymentResult = machine.DeploymentResult;
      return deploymentResult.Value == TaskResult.Succeeded;
    }

    public static bool CanAttemptDeployment(this DeploymentMachineState machine) => !machine.Excluded && machine.AgentStatus == TaskAgentStatus.Online && machine.DemandsMatched && !machine.DeploymentInQueue && !machine.DeploymentAttempted;
  }
}
