// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Taskboard.TaskboardWorkItemUpdateEvent
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.SignalR;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Taskboard
{
  public class TaskboardWorkItemUpdateEvent : ISignalRObject
  {
    public const string ChangeTypeCreated = "Created";
    public const string ChangeTypeUpdated = "Updated";
    public const string ChangeTypeDeleted = "Deleted";
    public const string ParentChangeTypeAdded = "Added";
    public const string ParentChangeTypeRemoved = "Removed";
    public const string ParentChangeTypeUnchanged = "Unchanged";

    public TaskboardWorkItemUpdateEvent(
      int id,
      int revision,
      ChangeTypes changeType,
      string stackRank,
      string iterationPath,
      int parentId,
      Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Common.ParentChangeType parentChangeStatus)
    {
      this.Id = id;
      this.Revision = revision;
      this.StackRank = stackRank;
      this.IterationPath = iterationPath;
      this.ParentId = parentId;
      switch (changeType)
      {
        case ChangeTypes.New:
          this.ChangeType = "Created";
          break;
        case ChangeTypes.Change:
          this.ChangeType = "Updated";
          break;
        case ChangeTypes.Delete:
          this.ChangeType = "Deleted";
          break;
      }
      switch (parentChangeStatus)
      {
        case Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Common.ParentChangeType.Unchanged:
          this.ParentChangeType = "Unchanged";
          break;
        case Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Common.ParentChangeType.Added:
          this.ParentChangeType = "Added";
          break;
        case Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Common.ParentChangeType.Removed:
          this.ParentChangeType = "Removed";
          break;
      }
    }

    public int Id { get; set; }

    public int Revision { get; set; }

    public string ChangeType { get; set; }

    public string StackRank { get; set; }

    public string IterationPath { get; set; }

    public int ParentId { get; set; }

    public string ParentChangeType { get; set; }
  }
}
