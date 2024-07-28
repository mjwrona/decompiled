// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemCommentVersionsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "commentsVersions", ResourceVersion = 1)]
  [ControllerApiVersion(5.1)]
  [FeatureEnabled("WorkItemTracking.Server.CommentService.EnableEditAndDelete.F4")]
  public class WorkItemCommentVersionsController : WorkItemTrackingApiController
  {
    protected const int TraceRange = 5931000;

    public override string TraceArea => "commentsVersions";

    protected virtual WorkItemCommentService WorkItemCommentService => this.TfsRequestContext.GetService<WorkItemCommentService>();

    [TraceFilter(5931000, 5931010)]
    [HttpGet]
    public CommentVersion GetCommentVersion(int workItemId, int commentId, int version)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateWorkItemId(workItemId);
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateCommentId(commentId);
      if (version <= 0)
        throw new VssPropertyValidationException(nameof (version), ResourceStrings.QueryParameterOutOfRange((object) nameof (version)));
      return WorkItemCommentFactory.Create(this.TfsRequestContext, this.ProjectId, this.WorkItemCommentService.GetCommentVersion(this.TfsRequestContext, this.ProjectId, workItemId, commentId, version));
    }

    [TraceFilter(5931011, 5931020)]
    [HttpGet]
    public IList<CommentVersion> GetCommentVersions(int workItemId, int commentId)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateWorkItemId(workItemId);
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateCommentId(commentId);
      return WorkItemCommentsFactory.Create(this.TfsRequestContext, this.ProjectId, (IEnumerable<WorkItemCommentVersion>) this.WorkItemCommentService.GetCommentVersions(this.TfsRequestContext, this.ProjectId, workItemId, commentId));
    }
  }
}
