// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.DeploymentPoolState
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  internal class DeploymentPoolState
  {
    private IDictionary<int, DeploymentMachineState> m_machines;

    public IDictionary<int, DeploymentMachineState> Machines
    {
      get
      {
        if (this.m_machines == null)
          this.m_machines = (IDictionary<int, DeploymentMachineState>) new Dictionary<int, DeploymentMachineState>();
        return this.m_machines;
      }
    }
  }
}
