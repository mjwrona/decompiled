// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Converters.DeploymentGroupConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Converters
{
  public static class DeploymentGroupConverter
  {
    public static DeploymentGroup ToDeploymentGroup(
      this DeploymentGroupCreateParameter deploymentGroupCreateParameter)
    {
      int num = 0;
      TaskAgentPoolReference agentPoolReference = (TaskAgentPoolReference) null;
      if (deploymentGroupCreateParameter.PoolId != 0)
        num = deploymentGroupCreateParameter.PoolId;
      else if (deploymentGroupCreateParameter.Pool != null)
        num = deploymentGroupCreateParameter.Pool.Id;
      if (num != 0)
        agentPoolReference = new TaskAgentPoolReference()
        {
          Id = num,
          IsHosted = false,
          PoolType = TaskAgentPoolType.Deployment
        };
      DeploymentGroup deploymentGroup = new DeploymentGroup();
      deploymentGroup.Name = deploymentGroupCreateParameter.Name;
      deploymentGroup.Description = deploymentGroupCreateParameter.Description;
      deploymentGroup.Pool = agentPoolReference;
      return deploymentGroup;
    }

    public static DeploymentGroup ToDeploymentGroup(
      this DeploymentGroupUpdateParameter deploymentGroupUpdateParameter,
      int deploymentGroupId)
    {
      DeploymentGroup deploymentGroup = new DeploymentGroup();
      deploymentGroup.Id = deploymentGroupId;
      deploymentGroup.Name = deploymentGroupUpdateParameter.Name;
      deploymentGroup.Description = deploymentGroupUpdateParameter.Description;
      return deploymentGroup;
    }
  }
}
