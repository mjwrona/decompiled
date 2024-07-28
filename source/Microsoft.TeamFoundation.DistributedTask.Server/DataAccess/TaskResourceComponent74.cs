// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent74
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent74 : TaskResourceComponent73
  {
    public override List<AgentPoolData> GetAgentPoolsByLastModifiedDate(
      int batchSize,
      DateTime? fromDate)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPoolsByLastModifiedDate)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentPoolsByLastModifiedDate");
        this.BindInt("@batchSize", batchSize);
        this.BindNullableDateTime("@fromDate", fromDate);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AgentPoolData>(this.CreateAgentPoolAnalyticsDataBinder());
          return resultCollection.GetCurrent<AgentPoolData>().Items;
        }
      }
    }

    protected virtual ObjectBinder<AgentPoolData> CreateAgentPoolAnalyticsDataBinder() => (ObjectBinder<AgentPoolData>) new AgentPoolDataBinder();
  }
}
