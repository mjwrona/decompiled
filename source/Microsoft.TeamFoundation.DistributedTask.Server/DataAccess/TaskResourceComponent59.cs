// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent59
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent59 : TaskResourceComponent58
  {
    protected virtual ObjectBinder<TaskAgentPoolStatus> CreateTaskAgentPoolStatusBinder() => (ObjectBinder<TaskAgentPoolStatus>) new TaskAgentPoolStatusBinder(this.RequestContext);

    public override IList<TaskAgentPoolStatus> GetAgentPoolStatusByIds(IEnumerable<int> poolIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPoolStatusByIds)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentPoolStatusByIds");
        this.BindInt32Table("@poolIds", poolIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolStatus>(this.CreateTaskAgentPoolStatusBinder());
          return (IList<TaskAgentPoolStatus>) resultCollection.GetCurrent<TaskAgentPoolStatus>().Items;
        }
      }
    }
  }
}
