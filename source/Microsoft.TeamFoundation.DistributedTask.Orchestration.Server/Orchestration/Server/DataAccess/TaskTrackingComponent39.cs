// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent39
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent39 : TaskTrackingComponent38
  {
    protected override ObjectBinder<TaskLogPage> GetTaskLogPageBinder() => (ObjectBinder<TaskLogPage>) new TaskLogPageBinder2();

    public override async Task<TaskLog> UpdateLogPageAsync(
      Guid scopeIdentifier,
      Guid planId,
      int logId,
      int pageId,
      long lineCount,
      TaskLogPageState state,
      string blobFileId = null)
    {
      TaskTrackingComponent39 component = this;
      TaskLog taskLog;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateLogPageAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateLogPage");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindInt("@logId", logId);
        component.BindInt("@pageId", pageId);
        component.BindLong("@lineCount", lineCount);
        component.BindInt("@state", (int) state);
        component.BindString("@blobFileId", blobFileId, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskLog>((ObjectBinder<TaskLog>) new TaskLogBinder());
          taskLog = resultCollection.GetCurrent<TaskLog>().FirstOrDefault<TaskLog>();
        }
      }
      return taskLog;
    }

    public override GetLogsResult GetLogs(Guid scopeIdentifier, Guid planId, bool includePages = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetLogs)))
      {
        this.PrepareStoredProcedure("Task.prc_GetLogs");
        this.BindDataspaceId(scopeIdentifier);
        this.BindGuid("@planId", planId);
        this.BindBoolean("@includePages", includePages);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskLog>((ObjectBinder<TaskLog>) new TaskLogBinder());
          resultCollection.AddBinder<TaskLogPage>(this.GetTaskLogPageBinder());
          GetLogsResult logs = new GetLogsResult();
          logs.Logs = (IEnumerable<TaskLog>) resultCollection.GetCurrent<TaskLog>().Items;
          if (includePages)
          {
            resultCollection.NextResult();
            logs.LogPages = (IEnumerable<TaskLogPage>) resultCollection.GetCurrent<TaskLogPage>().Items;
          }
          return logs;
        }
      }
    }
  }
}
