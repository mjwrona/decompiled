// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemComments5Controller
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "comments", ResourceVersion = 4)]
  [ControllerApiVersion(5.1)]
  [FeatureEnabled("WorkItemTracking.Server.CommentService.EnableEditAndDelete.F4")]
  public class WorkItemComments5Controller : WorkItemComments4Controller
  {
    [TraceFilter(5930031, 5930040)]
    [HttpPost]
    [ClientExample("POST__wit_workitems__taskId__comments_create.json", "Add a comment", null, null)]
    public Comment AddWorkItemComment([FromUri] int workItemId, CommentCreate request, [FromUri] CommentFormat format)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateWorkItemId(workItemId);
      if (string.IsNullOrWhiteSpace(request?.Text))
        throw new VssPropertyValidationException("text", ResourceStrings.NullOrEmptyParameter((object) "text"));
      return WorkItemCommentFactory.Create(this.TfsRequestContext, this.ProjectId, this.WorkItemCommentService.AddComment(this.TfsRequestContext, this.ProjectId, new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.AddWorkItemComment(workItemId, request.Text, format)));
    }

    [TraceFilter(5930041, 5930050)]
    [HttpPatch]
    [ClientExample("PATCH__wit_workitems__taskId__comments_1.json", "Update a comment", null, null)]
    public Comment UpdateWorkItemComment(
      [FromUri] int workItemId,
      [FromUri] int commentId,
      CommentUpdate request,
      [FromUri] CommentFormat format)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateWorkItemId(workItemId);
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateCommentId(commentId);
      if (string.IsNullOrWhiteSpace(request?.Text))
        throw new VssPropertyValidationException("text", ResourceStrings.NullOrEmptyParameter((object) "text"));
      return WorkItemCommentFactory.Create(this.TfsRequestContext, this.ProjectId, this.WorkItemCommentService.UpdateComment(this.TfsRequestContext, this.ProjectId, new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.UpdateWorkItemComment(workItemId, commentId, request.Text, format)));
    }
  }
}
