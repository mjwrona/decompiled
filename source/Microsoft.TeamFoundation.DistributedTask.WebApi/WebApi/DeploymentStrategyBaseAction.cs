// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentStrategyBaseAction
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal abstract class DeploymentStrategyBaseAction
  {
    protected DeploymentStrategyBaseAction(DeploymentStrategyActionType type)
    {
      this.Type = type;
      this.Steps = (IList<Step>) new List<Step>();
    }

    [DataMember]
    public DeploymentStrategyActionType Type { get; set; }

    [DataMember]
    public IList<Step> Steps { get; set; }
  }
}
