// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent67
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent67 : TaskResourceComponent66
  {
    public override GetAgentQueuesResult GetAgentQueuesByPoolId(
      Guid projectId,
      IEnumerable<int> poolIds,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      bool includeAgents = false,
      bool includeTags = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentQueuesByPoolId)))
      {
        this.PrepareStoredProcedure("Task.prc_GetQueuesByPoolId");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindByte("@queueType", (byte) queueType);
        this.BindInt32Table("@poolIds", poolIds);
        this.BindBoolean("@includeAgents", includeAgents);
        this.BindBoolean("@includeTags", includeTags);
        GetAgentQueuesResult agentQueuesByPoolId = new GetAgentQueuesResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          if (queueType == TaskAgentQueueType.Automation)
          {
            resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
            agentQueuesByPoolId.Queues.AddRange<TaskAgentQueue, IList<TaskAgentQueue>>((IEnumerable<TaskAgentQueue>) resultCollection.GetCurrent<TaskAgentQueue>().Items);
          }
          else
          {
            resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
            agentQueuesByPoolId.MachineGroups.AddRange<DeploymentGroup, IList<DeploymentGroup>>((IEnumerable<DeploymentGroup>) resultCollection.GetCurrent<DeploymentGroup>().Items);
            if (includeAgents && agentQueuesByPoolId.MachineGroups != null)
            {
              resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
              resultCollection.NextResult();
              ILookup<int, string> lookup = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
              resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) this));
              resultCollection.NextResult();
              IDictionary<int, IList<QueueAgent>> mappingsPerQueue = this.GetQueueAgentMappingsPerQueue((IList<QueueAgent>) resultCollection.GetCurrent<QueueAgent>().Items);
              foreach (DeploymentGroup machineGroup in (IEnumerable<DeploymentGroup>) agentQueuesByPoolId.MachineGroups)
              {
                if (mappingsPerQueue.ContainsKey(machineGroup.Id))
                  machineGroup.Machines.AddRange<DeploymentMachine, IList<DeploymentMachine>>(this.CreateDeploymentMachinesWithTags((IEnumerable<QueueAgent>) mappingsPerQueue[machineGroup.Id], lookup));
              }
            }
          }
          return agentQueuesByPoolId;
        }
      }
    }
  }
}
