// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent26
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent26 : TaskResourceComponent25
  {
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
        this.PrepareStoredProcedure("Task.prc_UpdateQueue");
        this.BindInt("@queueId", queueId);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindByte("@queueType", (byte) queueType);
        this.BindString("@name", name, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindNullableGuid("@groupScopeId", groupScopeId);
        this.BindNullableBoolean("@provisioned", provisioned);
        this.BindGuid("@writerId", this.Author);
        this.BindNullableInt("@poolId", poolId);
        AgentQueueOrMachineGroup queueOrMachineGroup = new AgentQueueOrMachineGroup();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          if (queueType == TaskAgentQueueType.Automation)
          {
            resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
            queueOrMachineGroup.Queue = resultCollection.GetCurrent<TaskAgentQueue>().Items.FirstOrDefault<TaskAgentQueue>();
          }
          else
          {
            resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
            queueOrMachineGroup.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().FirstOrDefault<DeploymentGroup>();
          }
          return queueOrMachineGroup;
        }
      }
    }

    public override AgentConnectivityResult SetAgentOnline(int poolId, int agentId, int sequenceId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (SetAgentOnline)))
      {
        this.PrepareStoredProcedure("Task.prc_SetAgentOnline");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agentId);
        this.BindInt("@sequenceId", sequenceId);
        this.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        AgentConnectivityResult connectivityResult = new AgentConnectivityResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder9());
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder5());
          resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          connectivityResult.HandledEvent = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          resultCollection.NextResult();
          connectivityResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          resultCollection.NextResult();
          ref AgentConnectivityResult local = ref connectivityResult;
          TaskAgent agent = resultCollection.GetCurrent<TaskAgent>().FirstOrDefault<TaskAgent>();
          TaskAgent taskAgent = agent != null ? agent.PopulateImplicitCapabilities() : (TaskAgent) null;
          local.Agent = taskAgent;
          if (connectivityResult.Agent != null)
          {
            resultCollection.NextResult();
            foreach (TaskAgentCapability taskAgentCapability in resultCollection.GetCurrent<TaskAgentCapability>().Items.Where<TaskAgentCapability>((System.Func<TaskAgentCapability, bool>) (x => !x.IsWellKnownCapability())))
            {
              switch (taskAgentCapability.CapabilityType)
              {
                case TaskAgentCapabilityType.System:
                  connectivityResult.Agent.SystemCapabilities.Add(taskAgentCapability.Name, taskAgentCapability.Value);
                  continue;
                case TaskAgentCapabilityType.User:
                  connectivityResult.Agent.UserCapabilities.Add(taskAgentCapability.Name, taskAgentCapability.Value);
                  continue;
                default:
                  continue;
              }
            }
          }
        }
        return connectivityResult;
      }
    }

    public override AgentConnectivityResult SetAgentOffline(
      int poolId,
      int agentId,
      int sequenceId = 0,
      bool force = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (SetAgentOffline)))
      {
        this.PrepareStoredProcedure("Task.prc_SetAgentOffline");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agentId);
        this.BindInt("@sequenceId", sequenceId);
        this.BindBoolean("@force", force);
        bool flag = false;
        TaskAgent agent = (TaskAgent) null;
        TaskAgentPoolData taskAgentPoolData = (TaskAgentPoolData) null;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder5());
          resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          flag = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          taskAgentPoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          resultCollection.NextResult();
          agent = resultCollection.GetCurrent<TaskAgent>().FirstOrDefault<TaskAgent>();
          if (agent != null)
          {
            if (resultCollection.TryNextResult())
            {
              agent = agent.PopulateImplicitCapabilities();
              foreach (TaskAgentCapability taskAgentCapability in resultCollection.GetCurrent<TaskAgentCapability>().Items.Where<TaskAgentCapability>((System.Func<TaskAgentCapability, bool>) (x => !x.IsWellKnownCapability())))
              {
                switch (taskAgentCapability.CapabilityType)
                {
                  case TaskAgentCapabilityType.System:
                    agent.SystemCapabilities.Add(taskAgentCapability.Name, taskAgentCapability.Value);
                    continue;
                  case TaskAgentCapabilityType.User:
                    agent.UserCapabilities.Add(taskAgentCapability.Name, taskAgentCapability.Value);
                    continue;
                  default:
                    continue;
                }
              }
            }
          }
        }
        AgentConnectivityResult connectivityResult = new AgentConnectivityResult();
        connectivityResult.Agent = agent;
        connectivityResult.HandledEvent = flag;
        connectivityResult.PoolData = taskAgentPoolData;
        connectivityResult = connectivityResult;
        return connectivityResult;
      }
    }

    public override DeleteAgentResult DeleteAgents(int poolId, IEnumerable<int> agentIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteAgents)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteAgents");
        this.BindInt("@poolId", poolId);
        this.BindInt32Table("@agentIds", agentIds);
        this.BindGuid("@writerId", this.Author);
        DeleteAgentResult deleteAgentResult = new DeleteAgentResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder4());
          resultCollection.AddBinder<TaskAgentSessionData>((ObjectBinder<TaskAgentSessionData>) new TaskAgentSessionDataBinder2());
          deleteAgentResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
          resultCollection.NextResult();
          deleteAgentResult.DeletedAgents = (IList<TaskAgent>) resultCollection.GetCurrent<TaskAgent>().Items;
          resultCollection.NextResult();
          deleteAgentResult.DeletedSessions = (IList<TaskAgentSessionData>) resultCollection.GetCurrent<TaskAgentSessionData>().Items;
          return deleteAgentResult;
        }
      }
    }
  }
}
