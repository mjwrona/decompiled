// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent12
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
  internal class TaskResourceComponent12 : TaskResourceComponent11
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
          resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
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
        this.PrepareStoredProcedure("Task.prc_DeleteQueue");
        this.BindInt("@queueId", queueId);
        this.BindGuid("@writerId", this.Author);
        DeleteAgentQueueResult agentQueueResult = new DeleteAgentQueueResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          agentQueueResult.Queue = resultCollection.GetCurrent<TaskAgentQueue>().FirstOrDefault<TaskAgentQueue>();
          resultCollection.NextResult();
          agentQueueResult.PoolUnreferenced = resultCollection.GetCurrent<bool>().FirstOrDefault<bool>();
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
          resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
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
          resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
          agentQueues.Queues.AddRange<TaskAgentQueue, IList<TaskAgentQueue>>((IEnumerable<TaskAgentQueue>) resultCollection.GetCurrent<TaskAgentQueue>().Items);
          return agentQueues;
        }
      }
    }

    public override AgentQueueOrMachineGroup UpdateAgentQueue(
      Guid projectId,
      int queueId,
      string name,
      Guid? groupScopeId = null,
      bool? provisioned = null,
      int? poolId = null,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      string description = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateAgentQueue)))
      {
        if (queueType == TaskAgentQueueType.Deployment)
          throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 26);
        this.PrepareStoredProcedure("Task.prc_UpdateQueue");
        this.BindInt("@queueId", queueId);
        this.BindString("@name", name, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindNullableGuid("@groupScopeId", groupScopeId);
        this.BindNullableBoolean("@provisioned", provisioned);
        this.BindGuid("@writerId", this.Author);
        AgentQueueOrMachineGroup queueOrMachineGroup = new AgentQueueOrMachineGroup();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
          queueOrMachineGroup.Queue = resultCollection.GetCurrent<TaskAgentQueue>().Items.FirstOrDefault<TaskAgentQueue>();
          return queueOrMachineGroup;
        }
      }
    }
  }
}
