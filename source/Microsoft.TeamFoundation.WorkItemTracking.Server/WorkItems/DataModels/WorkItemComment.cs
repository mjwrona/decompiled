// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  public class WorkItemComment : WorkItemSecuredObject
  {
    public string CreatedByDisplayName { get; private set; }

    public string CreatedByDisplayPart { get; private set; }

    public Guid CreatedByTeamFoundationId { get; private set; }

    public string Text { get; internal set; }

    public DateTime RevisionDate { get; private set; }

    public string RenderedText { get; set; }

    public CommentFormat Format { get; set; } = CommentFormat.Html;

    public int RevisionId { get; private set; }

    public int WorkItemId { get; private set; }

    public Guid? ProjectId { get; set; }

    public WorkItemComment(
      int workItemId,
      string text,
      int revisionId,
      string createdByDisplayName,
      string createdByDisplayPart,
      Guid createdByTeamFoundationId,
      DateTime revisionDate)
    {
      this.WorkItemId = workItemId;
      this.Text = text;
      this.RevisionId = revisionId;
      this.CreatedByDisplayName = createdByDisplayName;
      this.CreatedByDisplayPart = createdByDisplayPart;
      this.CreatedByTeamFoundationId = createdByTeamFoundationId;
      this.RevisionDate = revisionDate;
    }
  }
}
