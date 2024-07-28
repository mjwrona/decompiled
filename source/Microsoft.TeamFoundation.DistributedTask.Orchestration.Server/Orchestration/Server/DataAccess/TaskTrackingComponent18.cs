// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent18
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent18 : TaskTrackingComponent17
  {
    public override IList<TaskHub> GetHubs()
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetHubs)))
      {
        this.PrepareStoredProcedure("Task.prc_GetHubs");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskHub>((ObjectBinder<TaskHub>) new TaskHubBinder3());
          return (IList<TaskHub>) resultCollection.GetCurrent<TaskHub>().ToArray<TaskHub>();
        }
      }
    }

    public override TaskHub CreateHub(string name, string dataspaceCategory)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CreateHub)))
      {
        this.PrepareStoredProcedure("Task.prc_CreateHub");
        this.BindString("@name", name, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@dataspaceCategory", dataspaceCategory, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@licenseHubName", name, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskHub>((ObjectBinder<TaskHub>) new TaskHubBinder3());
          return resultCollection.GetCurrent<TaskHub>().FirstOrDefault<TaskHub>();
        }
      }
    }
  }
}
