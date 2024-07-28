// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent60
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent60 : TaskResourceComponent59
  {
    public override TaskAgentRequestData GetAgentRequest(
      int poolId,
      long requestId,
      bool includeStatus = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequest)))
      {
        TaskAgentRequestData agentRequest1 = (TaskAgentRequestData) null;
        this.PrepareStoredProcedure("Task.prc_GetAgentRequest");
        this.BindInt("@poolId", poolId);
        this.BindLong("@requestId", requestId);
        this.BindBoolean("@includeStatus", includeStatus);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<MatchedAgent>(this.CreateMatchedAgentBinder());
          resultCollection.AddBinder<TaskAgentCloudRequest>(this.CreateTaskAgentCloudRequestBinder());
          resultCollection.AddBinder<int>((ObjectBinder<int>) new Int32Binder());
          resultCollection.AddBinder<int>((ObjectBinder<int>) new Int32Binder());
          resultCollection.AddBinder<int>((ObjectBinder<int>) new Int32Binder());
          TaskAgentJobRequest agentRequest2 = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          if (agentRequest2 != null)
          {
            agentRequest1 = new TaskAgentRequestData(agentRequest2);
            resultCollection.NextResult();
            foreach (MatchedAgent matchedAgent in resultCollection.GetCurrent<MatchedAgent>())
              agentRequest1.AgentRequest.MatchedAgents.Add(matchedAgent.Agent);
            resultCollection.NextResult();
            agentRequest1.CloudRequest = resultCollection.GetCurrent<TaskAgentCloudRequest>().FirstOrDefault<TaskAgentCloudRequest>();
            if (includeStatus)
            {
              if (resultCollection.TryNextResult())
              {
                agentRequest1.AvailableAgentCount = new int?(resultCollection.GetCurrent<int>().FirstOrDefault<int>());
                resultCollection.NextResult();
                agentRequest1.PoolQueuePosition = new int?(resultCollection.GetCurrent<int>().FirstOrDefault<int>());
                resultCollection.NextResult();
                agentRequest1.ThrottlingQueuePosition = new int?(resultCollection.GetCurrent<int>().FirstOrDefault<int>());
              }
            }
          }
        }
        return agentRequest1;
      }
    }

    public override async Task<TaskAgentRequestData> GetAgentRequestAsync(
      int poolId,
      long requestId,
      bool includeStatus = false)
    {
      TaskResourceComponent60 component = this;
      TaskAgentRequestData agentRequestAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentRequestAsync)))
      {
        TaskAgentRequestData data = (TaskAgentRequestData) null;
        component.PrepareStoredProcedure("Task.prc_GetAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindBoolean("@includeStatus", includeStatus);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          resultCollection.AddBinder<MatchedAgent>((ObjectBinder<MatchedAgent>) new MatchedAgentBinder2());
          resultCollection.AddBinder<TaskAgentCloudRequest>(component.CreateTaskAgentCloudRequestBinder());
          resultCollection.AddBinder<int>((ObjectBinder<int>) new Int32Binder());
          resultCollection.AddBinder<int>((ObjectBinder<int>) new Int32Binder());
          resultCollection.AddBinder<int>((ObjectBinder<int>) new Int32Binder());
          TaskAgentJobRequest agentRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          if (agentRequest != null)
          {
            data = new TaskAgentRequestData(agentRequest);
            resultCollection.NextResult();
            foreach (MatchedAgent matchedAgent in resultCollection.GetCurrent<MatchedAgent>())
              agentRequest.MatchedAgents.Add(matchedAgent.Agent);
            resultCollection.NextResult();
            data.CloudRequest = resultCollection.GetCurrent<TaskAgentCloudRequest>().FirstOrDefault<TaskAgentCloudRequest>();
            if (includeStatus)
            {
              if (resultCollection.TryNextResult())
              {
                data.AvailableAgentCount = new int?(resultCollection.GetCurrent<int>().FirstOrDefault<int>());
                resultCollection.NextResult();
                data.PoolQueuePosition = new int?(resultCollection.GetCurrent<int>().FirstOrDefault<int>());
                resultCollection.NextResult();
                data.ThrottlingQueuePosition = new int?(resultCollection.GetCurrent<int>().FirstOrDefault<int>());
              }
            }
          }
        }
        agentRequestAsync = data;
      }
      return agentRequestAsync;
    }
  }
}
