// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemUpdateDataset
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  internal class WorkItemUpdateDataset
  {
    public WorkItemUpdateDataset()
    {
      this.PendingSetMembershipChecks = new List<PendingSetMembershipCheckRecord>();
      this.CoreFieldUpdates = new List<WorkItemCoreFieldUpdatesRecord>();
      this.CustomFieldUpdates = new List<WorkItemCustomFieldUpdateRecord>();
      this.TextFieldUpdates = new List<WorkItemTextFieldUpdateRecord>();
      this.WorkItemLinkUpdates = new List<WorkItemLinkUpdateRecord>();
      this.ResourceLinkUpdates = new List<WorkItemResourceLinkUpdateRecord>();
      this.SequenceOrderCorrelationIdMap = new Dictionary<int, string>();
      this.TeamProjectChanges = new List<WorkItemTeamProjectChangeRecord>();
      this.WorkItemCommentUpdates = new List<WorkItemCommentUpdateRecord>();
    }

    public List<PendingSetMembershipCheckRecord> PendingSetMembershipChecks { get; private set; }

    public List<WorkItemCoreFieldUpdatesRecord> CoreFieldUpdates { get; private set; }

    public List<WorkItemCustomFieldUpdateRecord> CustomFieldUpdates { get; private set; }

    public List<WorkItemTextFieldUpdateRecord> TextFieldUpdates { get; private set; }

    public List<WorkItemLinkUpdateRecord> WorkItemLinkUpdates { get; private set; }

    public List<WorkItemResourceLinkUpdateRecord> ResourceLinkUpdates { get; private set; }

    public List<WorkItemTeamProjectChangeRecord> TeamProjectChanges { get; private set; }

    public List<WorkItemCommentUpdateRecord> WorkItemCommentUpdates { get; private set; }

    public Dictionary<int, string> SequenceOrderCorrelationIdMap { get; set; }

    public bool IsEmpty() => this.CoreFieldUpdates.Count == 0 && this.CustomFieldUpdates.Count == 0 && this.TextFieldUpdates.Count == 0 && this.WorkItemLinkUpdates.Count == 0 && this.ResourceLinkUpdates.Count == 0 && this.TeamProjectChanges.Count == 0 && this.WorkItemCommentUpdates.Count == 0;
  }
}
