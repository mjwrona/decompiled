// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemComments4Controller
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "comments", ResourceVersion = 3)]
  [ControllerApiVersion(5.1)]
  [FeatureEnabled("WorkItemTracking.Server.CommentService.EnableEditAndDelete.F4")]
  public class WorkItemComments4Controller : WorkItemTrackingApiController
  {
    protected const int TraceRange = 5930000;

    protected virtual WorkItemCommentService WorkItemCommentService => this.TfsRequestContext.GetService<WorkItemCommentService>();

    public override string TraceArea => "comments";

    [TraceFilter(5930000, 5930010)]
    [HttpGet]
    [PublicProjectRequestRestrictions("5.1")]
    [ClientExample("GET__wit_workitems__issueId__comments__commentId_1.json", "Get a comment", null, null)]
    public Comment GetComment(
      int workItemId,
      int commentId,
      [FromUri(Name = "includeDeleted")] bool? includeDeleted = null,
      [FromUri(Name = "$expand")] CommentExpandOptions expandOptions = CommentExpandOptions.None)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateWorkItemId(workItemId);
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateCommentId(commentId);
      return WorkItemCommentFactory.Create(this.TfsRequestContext, this.ProjectId, this.WorkItemCommentService.GetComment(this.TfsRequestContext, this.ProjectId, workItemId, commentId, (WorkItemCommentExpandOptions) expandOptions, includeDeleted.GetValueOrDefault()));
    }

    [TraceFilter(5930011, 5930020)]
    [HttpGet]
    [PublicProjectRequestRestrictions("5.1")]
    [ClientExample("GET__wit_workitems__issueId__comments_1.json", "Get the first page of comments", null, null)]
    [ClientExample("GET__wit_workitems__issueId__comments_2.json", "Get the next page of comments", null, null)]
    public CommentList GetComments(
      int workItemId,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri] string continuationToken = null,
      [FromUri(Name = "includeDeleted")] bool? includeDeleted = null,
      [FromUri(Name = "$expand")] CommentExpandOptions expandOptions = CommentExpandOptions.None,
      [FromUri(Name = "order")] Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentSortOrder? order = null)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateWorkItemId(workItemId);
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateTop(top);
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComments comments = this.WorkItemCommentService.GetComments(this.TfsRequestContext, this.ProjectId, workItemId, top, continuationToken, (WorkItemCommentExpandOptions) expandOptions, includeDeleted.GetValueOrDefault(), order.HasValue ? (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.CommentSortOrder) order.Value : Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.CommentSortOrder.Desc);
      return WorkItemCommentsFactory.Create(this.WitRequestContext, this.ProjectId, workItemId, comments, top: top, includeDeleted: includeDeleted, expandOptions: expandOptions, order: order);
    }

    [TraceFilter(5930021, 5930030)]
    [HttpGet]
    [PublicProjectRequestRestrictions("5.1")]
    [ClientExample("GET__wit_workitems__issueId__comments_3.json", "Get a list of comments by ids", null, null)]
    public CommentList GetCommentsBatch(
      [FromUri] int workItemId,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string ids,
      [FromUri(Name = "includeDeleted")] bool? includeDeleted = null,
      [FromUri(Name = "$expand")] CommentExpandOptions expandOptions = CommentExpandOptions.None)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateWorkItemId(workItemId);
      ISet<int> intSet = (ISet<int>) new HashSet<int>(ParsingHelper.ParseIds(ids).Distinct<int>());
      int count = intSet.Count;
      if (count < 1 || count > WorkItemCommentService.MaxAllowedPageSize)
        throw new VssPropertyValidationException("ids.length", ResourceStrings.QueryParameterOutOfRangeWithRangeValues((object) "ids.length", (object) 1, (object) WorkItemCommentService.MaxAllowedPageSize));
      IEnumerable<int> ints = intSet.Where<int>((Func<int, bool>) (cid => cid < 1));
      if (ints.Any<int>())
        throw new VssPropertyValidationException(nameof (ids), ResourceStrings.QueryParameterOutOfRange((object) string.Join<int>(",", ints)));
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComments comments = this.WorkItemCommentService.GetComments(this.TfsRequestContext, this.ProjectId, workItemId, intSet, (WorkItemCommentExpandOptions) expandOptions, includeDeleted.GetValueOrDefault());
      return WorkItemCommentsFactory.Create(this.WitRequestContext, this.ProjectId, workItemId, comments);
    }

    [TraceFilter(5930031, 5930040)]
    [HttpPost]
    [ClientExample("POST__wit_workitems__taskId__comments_create.json", "Add a comment", null, null)]
    public Comment AddComment([FromUri] int workItemId, CommentCreate request)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateWorkItemId(workItemId);
      if (string.IsNullOrWhiteSpace(request?.Text))
        throw new VssPropertyValidationException("text", ResourceStrings.NullOrEmptyParameter((object) "text"));
      return WorkItemCommentFactory.Create(this.TfsRequestContext, this.ProjectId, this.WorkItemCommentService.AddComment(this.TfsRequestContext, this.ProjectId, new AddWorkItemComment(workItemId, request.Text)));
    }

    [TraceFilter(5930041, 5930050)]
    [HttpPatch]
    [ClientExample("PATCH__wit_workitems__taskId__comments_1.json", "Update a comment", null, null)]
    public Comment UpdateComment([FromUri] int workItemId, [FromUri] int commentId, CommentUpdate request)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateWorkItemId(workItemId);
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateCommentId(commentId);
      if (string.IsNullOrWhiteSpace(request?.Text))
        throw new VssPropertyValidationException("text", ResourceStrings.NullOrEmptyParameter((object) "text"));
      return WorkItemCommentFactory.Create(this.TfsRequestContext, this.ProjectId, this.WorkItemCommentService.UpdateComment(this.TfsRequestContext, this.ProjectId, new UpdateWorkItemComment(workItemId, commentId, request.Text)));
    }

    [TraceFilter(5930051, 5930060)]
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("DELETE__wit_workitems__taskId__comments_1.json", "Delete a comment", null, null)]
    public HttpResponseMessage DeleteComment([FromUri] int workItemId, [FromUri] int commentId)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateWorkItemId(workItemId);
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateCommentId(commentId);
      this.WorkItemCommentService.DeleteComment(this.TfsRequestContext, this.ProjectId, workItemId, commentId);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}
