// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent37
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent37 : TaskResourceComponent36
  {
    public override IEnumerable<DeploymentMachineData> GetDeploymentMachinesByAgentId(int agentId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetDeploymentMachinesByAgentId)))
      {
        this.PrepareStoredProcedure("Task.prc_GetDeploymentMachinesByAgentId");
        this.BindInt("@agentId", agentId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) this));
          return this.CreateDeploymentMachines((IEnumerable<QueueAgent>) resultCollection.GetCurrent<QueueAgent>().Items);
        }
      }
    }

    protected virtual IEnumerable<DeploymentMachineData> CreateDeploymentMachines(
      IEnumerable<QueueAgent> mappings)
    {
      return mappings.Select<QueueAgent, DeploymentMachineData>((System.Func<QueueAgent, DeploymentMachineData>) (m =>
      {
        return new DeploymentMachineData()
        {
          QueueId = m.QueueId,
          Project = m.Project,
          Machine = new DeploymentMachine()
          {
            Id = m.QueueAgentId,
            Agent = new TaskAgent() { Id = m.AgentId }
          }
        };
      }));
    }
  }
}
