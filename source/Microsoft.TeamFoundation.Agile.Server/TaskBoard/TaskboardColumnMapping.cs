// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardColumnMapping
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard;

namespace Microsoft.TeamFoundation.Agile.Server.TaskBoard
{
  public class TaskboardColumnMapping : ITaskboardColumnMapping
  {
    public TaskboardColumnMapping(string workItemType, string state)
    {
      this.WorkItemType = workItemType;
      this.State = state;
    }

    public string WorkItemType { get; private set; }

    public string State { get; private set; }

    public Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumnMapping Convert() => new Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumnMapping()
    {
      WorkItemType = this.WorkItemType,
      State = this.State
    };
  }
}
