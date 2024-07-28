// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.ActivityFeedEntry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  public class ActivityFeedEntry
  {
    public int Id { set; get; }

    public int RevisionId { set; get; }

    public DateTime RevisedDate { set; get; }

    public IdentityRef RevisedBy { set; get; }

    public Dictionary<string, object> FieldValues { set; get; }

    public WorkItemArtifactUpdates<string> Tags { set; get; }

    public WorkItemArtifactUpdates<WorkItemLinkedItem> Links { get; set; }

    public WorkItemArtifactUpdates<WorkItemAttachment> Attachments { get; set; }

    public WorkItemArtifactUpdates<WorkItemArtifactLink> ArtifactLinks { get; set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment Comment { get; set; }
  }
}
