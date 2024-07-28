// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.GetAgentQueuesResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  public struct GetAgentQueuesResult
  {
    private IList<TaskAgentQueue> m_queues;
    private IList<DeploymentGroup> m_machineGroups;

    public IList<TaskAgentQueue> Queues
    {
      get
      {
        if (this.m_queues == null)
          this.m_queues = (IList<TaskAgentQueue>) new List<TaskAgentQueue>();
        return this.m_queues;
      }
      internal set => this.m_queues = value;
    }

    public IList<DeploymentGroup> MachineGroups
    {
      get
      {
        if (this.m_machineGroups == null)
          this.m_machineGroups = (IList<DeploymentGroup>) new List<DeploymentGroup>();
        return this.m_machineGroups;
      }
      internal set => this.m_machineGroups = value;
    }
  }
}
