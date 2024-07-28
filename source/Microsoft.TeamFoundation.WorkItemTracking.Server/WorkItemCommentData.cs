// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemCommentData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemCommentData
  {
    public int CommentId { get; set; }

    public int WorkItemId { get; set; }

    public int Version { get; set; }

    public string Text { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string CreatedOnBehalfOf { get; set; }

    public DateTime? CreatedOnBehalfDate { get; set; }

    public WorkItemCommentFormat Format { get; set; }

    public Guid ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsNew { get; set; }

    public bool IsUpdated { get; set; }
  }
}
