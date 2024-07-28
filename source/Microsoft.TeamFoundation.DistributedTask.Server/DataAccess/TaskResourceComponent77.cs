// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent77
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent77 : TaskResourceComponent76
  {
    public override List<TaskAgentPoolSizeData> GetAgentPoolSize()
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPoolSize)))
      {
        this.PrepareStoredProcedure("Task.prc_GetPoolAgents");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolSizeData>(this.TaskAgentPoolSizeDataBinder());
          return resultCollection.GetCurrent<TaskAgentPoolSizeData>().Items;
        }
      }
    }

    protected virtual ObjectBinder<TaskAgentPoolSizeData> TaskAgentPoolSizeDataBinder() => (ObjectBinder<TaskAgentPoolSizeData>) new Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentPoolSizeDataBinder();
  }
}
