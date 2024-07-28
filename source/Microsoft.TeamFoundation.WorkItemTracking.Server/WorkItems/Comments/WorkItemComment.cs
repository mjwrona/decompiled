// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments
{
  public class WorkItemComment : WorkItemSecuredObject
  {
    private readonly List<WorkItemCommentReaction> reactionsList = new List<WorkItemCommentReaction>();

    private WorkItemComment(Comment comment, int permissions, string securityToken)
      : base(permissions, securityToken)
    {
      ArgumentUtility.CheckForNull<Comment>(comment, nameof (comment));
      this.CommentId = comment.CommentId;
      this.WorkItemId = int.Parse(comment.ArtifactId);
      this.Text = comment.Text;
      this.RenderedText = comment.RenderedText;
      this.Format = (WorkItemCommentFormat) comment.Format;
      this.Version = comment.Version;
      this.CreatedDate = comment.CreatedDate;
      this.CreatedBy = comment.CreatedBy;
      this.CreatedOnBehalfOf = comment.CreatedOnBehalfOf;
      this.CreatedOnBehalfDate = new DateTime?(comment.CreatedOnBehalfDate);
      this.ModifiedDate = comment.ModifiedDate;
      this.ModifiedBy = comment.ModifiedBy;
      this.IsDeleted = comment.IsDeleted;
      this.reactionsList.AddRange(comment.Reactions.Select<CommentReaction, WorkItemCommentReaction>((Func<CommentReaction, WorkItemCommentReaction>) (commentReaction => new WorkItemCommentReaction(commentReaction, permissions, securityToken))));
    }

    private WorkItemComment(
      WorkItemCommentUpdateRecord comment,
      Guid projectId,
      int permissions,
      string securityToken)
      : base(permissions, securityToken)
    {
      ArgumentUtility.CheckForNull<WorkItemCommentUpdateRecord>(comment, nameof (comment));
      this.CommentId = comment.CommentId;
      this.WorkItemId = int.Parse(comment.ArtifactId);
      this.Text = comment.Text;
      this.RenderedText = comment.RenderedText;
      this.Format = comment.Format.Value;
      this.Version = comment.Version;
      this.CreatedDate = comment.CreatedDate;
      this.CreatedBy = comment.CreatedBy;
      this.CreatedOnBehalfOf = comment.CreatedOnBehalfOf;
      this.CreatedOnBehalfDate = comment.CreatedOnBehalfDate;
      this.ModifiedDate = comment.ModifiedDate;
      this.ModifiedBy = comment.ModifiedBy;
      this.IsDeleted = comment.IsDeleted;
    }

    public int CommentId { get; }

    public int WorkItemId { get; }

    public Guid ArtifactKind { get; } = WorkItemArtifactKinds.WorkItem;

    public int Version { get; }

    public string Text { get; private set; }

    public string RenderedText { get; private set; }

    public IReadOnlyCollection<WorkItemCommentReaction> Reactions => (IReadOnlyCollection<WorkItemCommentReaction>) this.reactionsList;

    public WorkItemCommentFormat Format { get; }

    public Guid CreatedBy { get; }

    public DateTime CreatedDate { get; }

    public string CreatedOnBehalfOf { get; }

    public DateTime? CreatedOnBehalfDate { get; }

    public Guid ModifiedBy { get; }

    public DateTime ModifiedDate { get; }

    public bool IsDeleted { get; }

    internal static WorkItemComment FromComment(
      IVssRequestContext requestContext,
      Comment comment,
      Guid projectId,
      int permissions,
      string securityToken)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<string>(securityToken, nameof (securityToken));
      WorkItemComment result = new WorkItemComment(comment, permissions, securityToken);
      return WorkItemComment.ProcessInlineImagesInText(requestContext, result, projectId);
    }

    internal static WorkItemComment FromUpdateRecord(
      IVssRequestContext requestContext,
      WorkItemCommentUpdateRecord record,
      Guid projectId,
      int permissions,
      string securityToken)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      WorkItemComment result = new WorkItemComment(record, projectId, permissions, securityToken);
      return WorkItemComment.ProcessInlineImagesInText(requestContext, result, projectId);
    }

    private static WorkItemComment ProcessInlineImagesInText(
      IVssRequestContext requestContext,
      WorkItemComment result,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      TeamFoundationWorkItemService service = requestContext.GetService<TeamFoundationWorkItemService>();
      result.Text = service.ReplaceTextFieldTokens(requestContext, result.Text, new Guid?(projectId));
      return result;
    }
  }
}
