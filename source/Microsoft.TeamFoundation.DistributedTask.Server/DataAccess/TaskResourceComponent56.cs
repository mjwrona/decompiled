// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent56
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent56 : TaskResourceComponent55
  {
    public override AgentConnectivityResult SetAgentOnline(int poolId, int agentId, int sequenceId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (SetAgentOnline)))
      {
        this.PrepareStoredProcedure("Task.prc_SetAgentOnline");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agentId);
        this.BindInt("@sequenceId", sequenceId);
        this.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        this.BindBoolean("@assignRequests", false);
        AgentConnectivityResult connectivityResult = new AgentConnectivityResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>(this.CreateTaskAgentBinder());
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

    public override async Task<IList<TaskAgentJobRequest>> GetUnassigableAgentRequestsAsync(
      DateTime expirationDate,
      int batchSize)
    {
      TaskResourceComponent56 component = this;
      IList<TaskAgentJobRequest> list;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetUnassigableAgentRequestsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetUnassignableAgentRequests");
        component.BindDateTime2("@expirationTime", expirationDate);
        component.BindInt("@batchSize", batchSize);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          list = (IList<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().ToList<TaskAgentJobRequest>();
        }
      }
      return list;
    }
  }
}
