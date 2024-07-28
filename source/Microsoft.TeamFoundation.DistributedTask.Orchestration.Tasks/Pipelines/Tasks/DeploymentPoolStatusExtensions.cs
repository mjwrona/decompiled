// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.DeploymentPoolStatusExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  internal static class DeploymentPoolStatusExtensions
  {
    public static bool IsDeploymentComplete(this DeploymentPoolStatus deploymentPoolStatus) => deploymentPoolStatus != null ? deploymentPoolStatus.DeploymentComplete : throw new ArgumentNullException(nameof (deploymentPoolStatus));

    public static bool CanQueueAgentRequest(this DeploymentPoolStatus deploymentPoolStatus)
    {
      if (deploymentPoolStatus == null)
        throw new ArgumentNullException(nameof (deploymentPoolStatus));
      return !deploymentPoolStatus.IsDeploymentComplete() && deploymentPoolStatus.DeploymentPoolHealthy && deploymentPoolStatus.AgentAvailable;
    }
  }
}
