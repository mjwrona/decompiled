// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskOrchestrationOwnerBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class TaskOrchestrationOwnerBinder : ObjectBinder<TaskOrchestrationOwner>
  {
    private SqlColumnBinder m_columnBinder;

    public TaskOrchestrationOwnerBinder(TaskSqlComponentBase sqlComponent, string columnName)
    {
      this.SqlComponent = sqlComponent;
      this.m_columnBinder = new SqlColumnBinder(columnName);
    }

    protected override TaskOrchestrationOwner Bind()
    {
      string json = this.m_columnBinder.GetString((IDataReader) this.Reader, false);
      return !string.IsNullOrEmpty(json) ? JsonUtilities.Deserialize<TaskOrchestrationOwner>(json) : (TaskOrchestrationOwner) null;
    }

    private TaskSqlComponentBase SqlComponent { get; }
  }
}
