// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemCommentVersion
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments
{
  public class WorkItemCommentVersion : WorkItemSecuredObject
  {
    public WorkItemCommentVersion(int permissions, string token)
      : base(permissions, token)
    {
    }

    private WorkItemCommentVersion(CommentVersion commentVersion, int permissions, string token)
      : base(permissions, token)
    {
      ArgumentUtility.CheckForNull<CommentVersion>(commentVersion, nameof (commentVersion));
      this.WorkItemId = int.Parse(commentVersion.ArtifactId);
      this.CommentId = commentVersion.CommentId;
      this.Version = commentVersion.Version;
      this.Text = commentVersion.Text;
      this.RenderedText = commentVersion.RenderedText;
      this.Format = (WorkItemCommentFormat) commentVersion.Format;
      this.CreatedDate = commentVersion.CreatedDate;
      this.CreatedBy = commentVersion.CreatedBy;
      this.CreatedOnBehalfOf = commentVersion.CreatedOnBehalfOf;
      this.CreatedOnBehalfDate = commentVersion.CreatedOnBehalfDate;
      this.ModifiedDate = commentVersion.ModifiedDate;
      this.ModifiedBy = commentVersion.ModifiedBy;
      this.IsDeleted = commentVersion.IsDeleted;
    }

    public int CommentId { get; }

    public int WorkItemId { get; }

    public Guid ArtifactKind { get; } = WorkItemArtifactKinds.WorkItem;

    public int Version { get; }

    public string Text { get; private set; }

    public string RenderedText { get; private set; }

    public WorkItemCommentFormat Format { get; }

    public Guid CreatedBy { get; }

    public DateTime CreatedDate { get; }

    public string CreatedOnBehalfOf { get; set; }

    public DateTime CreatedOnBehalfDate { get; set; }

    public Guid ModifiedBy { get; }

    public DateTime ModifiedDate { get; }

    public bool IsDeleted { get; }

    internal static WorkItemCommentVersion FromCommentVersion(
      IVssRequestContext requestContext,
      CommentVersion commentVersion,
      Guid projectId,
      int permissions,
      string securityToken)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<string>(securityToken, nameof (securityToken));
      WorkItemCommentVersion result = new WorkItemCommentVersion(commentVersion, permissions, securityToken);
      if (result.Format == WorkItemCommentFormat.Html)
        result = WorkItemCommentVersion.ProcessInlineImagesInText(requestContext, result, projectId);
      return result;
    }

    private static WorkItemCommentVersion ProcessInlineImagesInText(
      IVssRequestContext requestContext,
      WorkItemCommentVersion result,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      TeamFoundationWorkItemService service = requestContext.GetService<TeamFoundationWorkItemService>();
      result.Text = service.ReplaceTextFieldTokens(requestContext, result.Text, new Guid?(projectId));
      result.RenderedText = result.Text;
      return result;
    }
  }
}
