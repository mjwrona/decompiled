// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.RollingDeploymentStrategy2
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal sealed class RollingDeploymentStrategy2 : DeploymentStrategyBase2
  {
    public RollingDeploymentStrategy2(
      RollingDeploymentOption deploymentOption,
      int deploymentOptionValue)
      : base(DeploymentStrategyType.Rolling)
    {
      this.DeploymentOption = deploymentOption;
      this.DeploymentOptionValue = deploymentOptionValue;
    }

    [DataMember]
    public RollingDeploymentOption DeploymentOption { get; private set; }

    [DataMember]
    public int DeploymentOptionValue { get; private set; }
  }
}
