// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent10
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
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent10 : TaskResourceComponent9
  {
    public override AgentQueueOrMachineGroup AddAgentQueue(
      Guid projectId,
      string name,
      int poolId,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      string description = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddAgentQueue)))
      {
        if (queueType == TaskAgentQueueType.Deployment)
          throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 25);
        this.PrepareStoredProcedure("Task.prc_AddQueue");
        this.BindString("@queueName", name, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@poolId", poolId);
        this.BindGuid("@writerId", this.Author);
        AgentQueueOrMachineGroup queueOrMachineGroup = new AgentQueueOrMachineGroup();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder());
          queueOrMachineGroup.Queue = resultCollection.GetCurrent<TaskAgentQueue>().FirstOrDefault<TaskAgentQueue>();
          return queueOrMachineGroup;
        }
      }
    }

    public override DeleteAgentQueueResult DeleteAgentQueue(
      Guid projectId,
      int queueId,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteAgentQueue)))
      {
        if (queueType == TaskAgentQueueType.Deployment)
          throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 25);
        this.PrepareStoredProcedure("Task.prc_DeleteQueues");
        this.BindInt32Table("@queueIds", (IEnumerable<int>) new int[1]
        {
          queueId
        });
        this.BindGuid("@writerId", this.Author);
        DeleteAgentQueueResult agentQueueResult = new DeleteAgentQueueResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder());
          agentQueueResult.Queue = resultCollection.GetCurrent<TaskAgentQueue>().FirstOrDefault<TaskAgentQueue>();
          return agentQueueResult;
        }
      }
    }

    public override AgentQueueOrMachineGroup GetAgentQueue(
      Guid projectId,
      int queueId,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      bool includeMachines = true,
      bool includeTags = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentQueue)))
      {
        AgentQueueOrMachineGroup agentQueue = new AgentQueueOrMachineGroup();
        if (queueType == TaskAgentQueueType.Deployment)
          return agentQueue;
        this.PrepareStoredProcedure("Task.prc_GetQueuesById");
        this.BindInt32Table("@queueIds", (IEnumerable<int>) new int[1]
        {
          queueId
        });
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder());
          agentQueue.Queue = resultCollection.GetCurrent<TaskAgentQueue>().FirstOrDefault<TaskAgentQueue>();
          return agentQueue;
        }
      }
    }

    public override GetAgentQueuesResult GetAgentQueues(
      Guid projectId,
      string queueName,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      bool includeMachines = false,
      bool includeTags = true,
      string lastQueueName = null,
      int maxQueuesCount = 10000)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentQueues)))
      {
        GetAgentQueuesResult agentQueues = new GetAgentQueuesResult();
        if (queueType == TaskAgentQueueType.Deployment)
          return agentQueues;
        this.PrepareStoredProcedure("Task.prc_GetQueues");
        this.BindString("@queueName", queueName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder());
          agentQueues.Queues.AddRange<TaskAgentQueue, IList<TaskAgentQueue>>((IEnumerable<TaskAgentQueue>) resultCollection.GetCurrent<TaskAgentQueue>().Items);
          return agentQueues;
        }
      }
    }

    public override async Task<ExpiredAgentRequestsResult> GetExpiredAgentRequestsAsync(int poolId)
    {
      TaskResourceComponent10 component = this;
      ExpiredAgentRequestsResult agentRequestsAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetExpiredAgentRequestsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetExpiredAgentJobRequests");
        component.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder6());
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          IList<TaskAgentJobRequest> items = (IList<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          resultCollection.NextResult();
          agentRequestsAsync = new ExpiredAgentRequestsResult(resultCollection.GetCurrent<bool>().First<bool>(), items);
        }
      }
      return agentRequestsAsync;
    }
  }
}
