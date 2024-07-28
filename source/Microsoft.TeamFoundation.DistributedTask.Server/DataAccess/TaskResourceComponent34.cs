// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent34
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent34 : TaskResourceComponent33
  {
    public override async Task<RequestAgentsUpdateResult> RequestAgentsUpdateAsync(
      int poolId,
      IEnumerable<int> agentIds,
      string targetVersion,
      Guid requestedBy,
      string currentState,
      TaskAgentUpdateReasonData reasonData)
    {
      TaskResourceComponent34 component = this;
      RequestAgentsUpdateResult agentsUpdateResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (RequestAgentsUpdateAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_RequestAgentsUpdate");
        component.BindInt("@poolId", poolId);
        component.BindInt32Table("@agentIds", agentIds);
        component.BindString("@targetVersion", targetVersion, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindGuid("@requestedBy", requestedBy);
        component.BindString("@currentState", currentState, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        RequestAgentsUpdateResult result = new RequestAgentsUpdateResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>(component.CreateTaskAgentBinder());
          resultCollection.AddBinder<TaskAgent>(component.CreateTaskAgentBinder());
          result.NewUpdates.AddRange((IEnumerable<TaskAgent>) resultCollection.GetCurrent<TaskAgent>().Items);
          resultCollection.NextResult();
          result.ExistingUpdates.AddRange((IEnumerable<TaskAgent>) resultCollection.GetCurrent<TaskAgent>().Items);
          agentsUpdateResult = result;
        }
      }
      return agentsUpdateResult;
    }
  }
}
