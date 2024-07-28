// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.CanaryDeploymentStrategy
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal sealed class CanaryDeploymentStrategy : DeploymentStrategyBase2
  {
    private IList<int> m_increments;

    public CanaryDeploymentStrategy(List<int> increments)
      : base(DeploymentStrategyType.Canary)
    {
      this.m_increments = (IList<int>) increments;
    }

    [DataMember]
    public IList<int> DeploymentIncrements
    {
      get
      {
        if (this.m_increments == null)
          this.m_increments = (IList<int>) new List<int>();
        return this.m_increments;
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      IList<int> increments = this.m_increments;
      if ((increments != null ? (increments.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_increments = (IList<int>) null;
    }
  }
}
