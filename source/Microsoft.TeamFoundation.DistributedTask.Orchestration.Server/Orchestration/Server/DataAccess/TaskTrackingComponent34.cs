// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent34
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent34 : TaskTrackingComponent33
  {
    private static readonly SqlMetaData[] Task_typ_StringTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.NVarChar, -1L)
    };

    protected override SqlParameter BindTaskStringTable(
      string parameterName,
      IEnumerable<string> rows)
    {
      rows = rows ?? Enumerable.Empty<string>();
      return this.BindTable(parameterName, "Task.typ_TimelineStringTableV2", rows.Select<string, SqlDataRecord>(new System.Func<string, SqlDataRecord>(rowBinder)));

      static SqlDataRecord rowBinder(string row)
      {
        SqlDataRecord record = new SqlDataRecord(TaskTrackingComponent34.Task_typ_StringTable);
        record.SetNullableString(0, row);
        return record;
      }
    }
  }
}
