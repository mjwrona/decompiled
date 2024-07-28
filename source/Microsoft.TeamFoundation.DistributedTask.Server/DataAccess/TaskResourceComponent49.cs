// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent49
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
  internal class TaskResourceComponent49 : TaskResourceComponent48
  {
    public override IList<TaskAgentJobRequest> GetAgentRequestsForAgents(
      int poolId,
      IList<int> agentIds,
      Guid hostId,
      Guid scopeId,
      int completedRequestCount,
      int? ownerId,
      DateTime? completedOn)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequestsForAgents)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentRequestsByFilter");
        this.BindInt("@poolId", poolId);
        this.BindNullableInt("@ownerId", ownerId);
        this.BindNullableDateTime("@completedOn", completedOn);
        this.BindInt32Table("@agentIds", (IEnumerable<int>) agentIds);
        this.BindGuid("@hostId", hostId);
        this.BindGuid("@scopeId", scopeId);
        this.BindInt("@completedRequestCount", completedRequestCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<MatchedAgent>(this.CreateMatchedAgentBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          List<TaskAgentJobRequest> items = resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          resultCollection.NextResult();
          if (items.Count > 0)
          {
            Dictionary<long, TaskAgentJobRequest> dictionary = items.ToDictionary<TaskAgentJobRequest, long>((System.Func<TaskAgentJobRequest, long>) (x => x.RequestId));
            foreach (MatchedAgent matchedAgent in resultCollection.GetCurrent<MatchedAgent>())
            {
              TaskAgentJobRequest taskAgentJobRequest;
              if (dictionary.TryGetValue(matchedAgent.RequestId, out taskAgentJobRequest))
                taskAgentJobRequest.MatchedAgents.Add(matchedAgent.Agent);
            }
          }
          resultCollection.NextResult();
          items.AddRange((IEnumerable<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
          resultCollection.NextResult();
          items.AddRange((IEnumerable<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
          return (IList<TaskAgentJobRequest>) items;
        }
      }
    }

    public override int GetResourceUsage(
      Guid hostId,
      string resourceType,
      bool poolIsHosted,
      bool includeRunningRequests,
      int maxRequestsCount,
      out List<TaskAgentJobRequest> runningRequests)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetResourceUsage)))
      {
        this.PrepareStoredProcedure("Task.prc_GetResourceUsage");
        this.BindGuid("@hostId", hostId);
        this.BindString("@resourceType", resourceType, 160, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindBoolean("@includeRunningRequests", includeRunningRequests);
        this.BindInt("@maxRequestsCount", maxRequestsCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          SqlColumnBinder runningRequestsCountBinder = new SqlColumnBinder("RunningRequestsCount");
          resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => runningRequestsCountBinder.GetInt32(reader))));
          if (includeRunningRequests)
            resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          int resourceUsage = resultCollection.GetCurrent<int>().First<int>();
          runningRequests = new List<TaskAgentJobRequest>();
          if (includeRunningRequests)
          {
            resultCollection.NextResult();
            runningRequests.AddRange((IEnumerable<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
          }
          return resourceUsage;
        }
      }
    }
  }
}
