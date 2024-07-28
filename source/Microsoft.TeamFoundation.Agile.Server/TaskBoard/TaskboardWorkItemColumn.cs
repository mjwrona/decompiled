// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardWorkItemColumn
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using System;

namespace Microsoft.TeamFoundation.Agile.Server.TaskBoard
{
  public class TaskboardWorkItemColumn
  {
    public int WorkItemId { get; }

    public string State { get; }

    public string Column { get; }

    public Guid ColumnId { get; }

    public TaskboardWorkItemColumn(int workItemId, string state, string column, Guid columnId)
    {
      this.WorkItemId = workItemId;
      this.State = state;
      this.Column = column;
      this.ColumnId = columnId;
    }

    public Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardWorkItemColumn Convert() => new Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardWorkItemColumn()
    {
      WorkItemId = this.WorkItemId,
      State = this.State,
      Column = this.Column,
      ColumnId = this.ColumnId
    };
  }
}
