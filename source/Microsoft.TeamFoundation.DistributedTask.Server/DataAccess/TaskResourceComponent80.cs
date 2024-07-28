// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent80
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
  internal class TaskResourceComponent80 : TaskResourceComponent79
  {
    public override IList<TaskAgentJobRequest> GetAgentRequestsForPlan(Guid planId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequestsForPlan)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAllAgentRequestsForPlan");
        this.BindGuid("@planId", planId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<MatchedAgent>(this.CreateMatchedAgentBinder());
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
          return (IList<TaskAgentJobRequest>) items;
        }
      }
    }
  }
}
