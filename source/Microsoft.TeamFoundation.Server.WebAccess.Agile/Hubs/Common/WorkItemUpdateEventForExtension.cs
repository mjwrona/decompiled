// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Common.WorkItemUpdateEventForExtension
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.SignalR;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Common
{
  public class WorkItemUpdateEventForExtension : ISignalRObject
  {
    public const string ChangeTypeCreated = "Created";
    public const string ChangeTypeUpdated = "Updated";
    public const string ChangeTypeDeleted = "Deleted";
    public const string ChangeTypeRestored = "Restored";
    public const string ChangeTypeRemoved = "Removed";
    public const string ChangeTypeAdded = "Added";

    public WorkItemUpdateEventForExtension(
      int id,
      int revision,
      ChangeTypes changeType,
      string stackRank,
      bool hasChangedChildren,
      bool changedTypeOrTitle)
    {
      this.Id = id;
      this.Revision = revision;
      this.StackRank = stackRank;
      this.ChangedChildren = hasChangedChildren;
      this.ChangedTypeOrTitle = changedTypeOrTitle;
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
        case ChangeTypes.Restore:
          this.ChangeType = "Restored";
          break;
      }
    }

    public WorkItemUpdateEventForExtension(WorkItemUpdateEventForExtension workItemData)
    {
      this.Id = workItemData.Id;
      this.Revision = workItemData.Revision;
      this.ChangeType = workItemData.ChangeType;
      this.StackRank = workItemData.StackRank;
    }

    public int Id { get; set; }

    public int Revision { get; set; }

    public string ChangeType { get; set; }

    public string StackRank { get; set; }

    public bool ChangedChildren { get; set; }

    public bool ChangedTypeOrTitle { get; set; }
  }
}
