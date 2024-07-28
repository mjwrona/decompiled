// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemCommentReaction
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Social.WebApi;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments
{
  public class WorkItemCommentReaction : WorkItemSecuredObject
  {
    public WorkItemCommentReaction(CommentReaction reaction, int permissions, string securityToken)
      : base(permissions, securityToken)
    {
      ArgumentUtility.CheckForNull<CommentReaction>(reaction, nameof (reaction));
      this.Type = reaction.Type;
      this.Count = reaction.Count;
      this.IsCurrentUserEngaged = reaction.IsCurrentUserEngaged;
      this.CommentId = reaction.CommentId;
    }

    public WorkItemCommentReaction(
      SocialEngagementRecord engagementRecord,
      int commentId,
      int permissions,
      string securityToken)
      : base(permissions, securityToken)
    {
      ArgumentUtility.CheckForNull<SocialEngagementRecord>(engagementRecord, nameof (engagementRecord));
      this.Type = engagementRecord.EngagementType;
      SocialEngagementStatistics engagementStatistics = engagementRecord.SocialEngagementStatistics;
      this.Count = engagementStatistics != null ? engagementStatistics.UserCount : 0;
      this.IsCurrentUserEngaged = engagementRecord.CreationDate.HasValue;
      this.CommentId = commentId;
    }

    public SocialEngagementType Type { get; set; }

    public int Count { get; set; }

    public bool IsCurrentUserEngaged { get; set; }

    public int CommentId { get; set; }
  }
}
