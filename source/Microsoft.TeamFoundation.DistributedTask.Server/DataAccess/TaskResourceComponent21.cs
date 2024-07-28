// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent21
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent21 : TaskResourceComponent20
  {
    public override AgentConnectivityResult SetAgentOnline(int poolId, int agentId, int sequenceId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (SetAgentOnline)))
      {
        this.PrepareStoredProcedure("Task.prc_SetAgentOnline");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agentId);
        this.BindInt("@sequenceId", sequenceId);
        AgentConnectivityResult connectivityResult = new AgentConnectivityResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder9());
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder5());
          resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          connectivityResult.HandledEvent = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          resultCollection.NextResult();
          connectivityResult.Agent = resultCollection.GetCurrent<TaskAgent>().FirstOrDefault<TaskAgent>();
          if (connectivityResult.Agent != null)
          {
            if (resultCollection.TryNextResult())
            {
              connectivityResult.Agent = connectivityResult.Agent.PopulateImplicitCapabilities();
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
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder5());
          resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          flag = resultCollection.GetCurrent<bool>().First<bool>();
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
        TaskAgentPoolData taskAgentPoolData = this.GetTaskAgentPoolData(poolId);
        AgentConnectivityResult connectivityResult = new AgentConnectivityResult();
        connectivityResult.Agent = agent;
        connectivityResult.HandledEvent = flag;
        connectivityResult.PoolData = taskAgentPoolData;
        connectivityResult = connectivityResult;
        return connectivityResult;
      }
    }
  }
}
