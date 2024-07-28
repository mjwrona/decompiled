// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemWithActivityFeedEntries
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  public class WorkItemWithActivityFeedEntries : WorkItemSecuredObject
  {
    public WorkItemWithActivityFeedEntries(int permissions, string securityToken)
      : base(permissions, securityToken)
    {
    }

    public int Id { get; set; }

    public Guid ProjectId { get; set; }

    public int Revision { get; set; }

    public List<ActivityFeedEntry> ActivityFeedEntries { get; set; }

    public Dictionary<string, object> LatestFieldValues { get; set; }

    public List<WorkItemLinkedItem> Links { get; set; }

    public List<WorkItemAttachment> Attachments { get; set; }

    public List<WorkItemArtifactLink> ArtifactLinks { get; set; }

    public Dictionary<string, FieldStatusFlags> FieldFlags { get; set; }
  }
}
