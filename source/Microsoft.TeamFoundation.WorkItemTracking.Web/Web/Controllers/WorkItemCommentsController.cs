// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemCommentsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "comments", ResourceVersion = 1)]
  [ControllerApiVersion(3.0)]
  [ClientIgnoreRouteScopes(ClientRouteScopes.Project)]
  public class WorkItemCommentsController : WorkItemTrackingApiController
  {
    protected const int TraceRange = 5918000;

    public override string TraceArea => "comments";

    protected override bool ControllerSupportsProjectScopedUrls() => false;

    [TraceFilter(5918000, 5918010)]
    [HttpGet]
    [PublicProjectRequestRestrictions("5.0")]
    [ClientExample("GET__wit_workitems__taskId__comments_2.json", "Get a page of comments", null, null)]
    [ClientExample("GET__wit_workitems__taskId__comments_2_collectionscope.json", "Get a page of comments (collection scoped)", null, null)]
    [ClientTSProjectParameterPosition(4)]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemComments GetComments(
      int id,
      [FromUri(Name = "fromRevision")] int fromRevision = 1,
      [FromUri(Name = "$top")] int top = 200,
      [FromUri(Name = "order")] CommentSortOrder order = CommentSortOrder.Asc)
    {
      if (id <= 0)
        throw new VssPropertyValidationException(nameof (id), ResourceStrings.QueryParameterOutOfRange((object) nameof (id)));
      if (fromRevision < 1)
        throw new VssPropertyValidationException(nameof (fromRevision), ResourceStrings.QueryParameterOutOfRange((object) nameof (fromRevision)));
      if (top <= 0 || top > 200)
        throw new VssPropertyValidationException("$top", ResourceStrings.QueryParameterOutOfRange((object) "$top"));
      Guid? projectId = new Guid?();
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComments workItemComments;
      if (this.ProjectId != Guid.Empty)
      {
        workItemComments = this.WorkItemService.GetWorkItemComments(this.TfsRequestContext, this.ProjectId, id, fromRevision, top, order.ToServerCommentSortOrder());
        if (workItemComments == null)
          throw new WorkItemNotFoundException(id);
        projectId = new Guid?(this.ProjectId);
      }
      else
      {
        workItemComments = this.WorkItemService.GetWorkItemComments(this.TfsRequestContext, id, fromRevision, top, order.ToServerCommentSortOrder());
        projectId = workItemComments != null ? workItemComments.ProjectId : throw new WorkItemNotFoundException(id);
      }
      return WorkItemCommentsFactory.Create(this.WitRequestContext, projectId, id, workItemComments, this.ShouldReturnProjectScopedUrls(this.TfsRequestContext));
    }

    [TraceFilter(5918000, 5918020)]
    [HttpGet]
    [PublicProjectRequestRestrictions("5.0")]
    [ClientExample("GET__wit_workitems__taskId__comments_1.json", "Get a single comment", null, null)]
    [ClientExample("GET__wit_workitems__taskId__comments_1_collectionscope.json", "Get a single comment(collection scoped)", null, null)]
    [ClientTSProjectParameterPosition(2)]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemComment GetComment(
      int id,
      int revision)
    {
      if (id <= 0)
        throw new VssPropertyValidationException(nameof (id), ResourceStrings.QueryParameterOutOfRange((object) nameof (id)));
      if (revision < 1)
        throw new VssPropertyValidationException(nameof (revision), ResourceStrings.QueryParameterOutOfRange((object) nameof (revision)));
      Guid? projectId = new Guid?();
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment workItemComment;
      if (this.ProjectId != Guid.Empty)
      {
        workItemComment = this.WorkItemService.GetWorkItemComment(this.TfsRequestContext, this.ProjectId, id, revision);
        if (workItemComment == null)
          throw new WorkItemCommentNotFoundException(id, revision);
        projectId = new Guid?(this.ProjectId);
      }
      else
      {
        workItemComment = this.WorkItemService.GetWorkItemComment(this.TfsRequestContext, id, revision);
        projectId = workItemComment != null ? workItemComment.ProjectId : throw new WorkItemCommentNotFoundException(id, revision);
      }
      IDictionary<Guid, IdentityRef> identityRefsById = IdentityRefBuilder.Create(this.WitRequestContext.RequestContext, (IEnumerable<Guid>) new Guid[1]
      {
        workItemComment.CreatedByTeamFoundationId
      }, true, true);
      return WorkItemCommentFactory.Create(this.WitRequestContext, projectId, id, workItemComment, identityRefsById, this.ShouldReturnProjectScopedUrls(this.TfsRequestContext));
    }
  }
}
