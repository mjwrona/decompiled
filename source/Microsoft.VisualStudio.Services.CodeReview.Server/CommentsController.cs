// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CommentsController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "comments")]
  public class CommentsController : CodeReviewApiControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (DiscussionComment), null, null)]
    public DiscussionComment GetComment(int reviewId, int threadId, short commentId)
    {
      ICodeReviewCommentService service = this.TfsRequestContext.GetService<ICodeReviewCommentService>();
      bool headerOptionValue = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.GetProjectId();
      int reviewId1 = reviewId;
      int threadId1 = threadId;
      int commentId1 = (int) commentId;
      int num = !headerOptionValue ? 1 : 0;
      return service.GetComment(tfsRequestContext, projectId, reviewId1, threadId1, (short) commentId1, num != 0);
    }

    [ClientResponseType(typeof (List<DiscussionComment>), null, null)]
    [HttpGet]
    public List<DiscussionComment> GetComments(int reviewId, int threadId)
    {
      CommentThread commentThread = this.TfsRequestContext.GetService<ICodeReviewCommentService>().GetCommentThread(this.TfsRequestContext, this.GetProjectId(), reviewId, threadId);
      return commentThread.Comments == null ? (List<DiscussionComment>) null : ((IEnumerable<DiscussionComment>) commentThread.Comments).ToList<DiscussionComment>();
    }

    [ClientResponseType(typeof (DiscussionComment), null, null)]
    [HttpPost]
    public HttpResponseMessage CreateComment(
      int reviewId,
      int threadId,
      DiscussionComment newComment)
    {
      ICodeReviewCommentService service = this.TfsRequestContext.GetService<ICodeReviewCommentService>();
      bool headerOptionValue = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request);
      return this.Request.CreateResponse<DiscussionComment>(HttpStatusCode.OK, service.SaveComment(this.TfsRequestContext, this.GetProjectId(), reviewId, threadId, newComment, !headerOptionValue));
    }

    [HttpPatch]
    [ClientResponseType(typeof (DiscussionComment), null, null)]
    public HttpResponseMessage UpdateComment(
      int reviewId,
      int threadId,
      short commentId,
      DiscussionComment comment)
    {
      ICodeReviewCommentService service = this.TfsRequestContext.GetService<ICodeReviewCommentService>();
      if (comment == null)
        throw new ArgumentNullException(nameof (comment), CodeReviewResources.CommentWasMalformed((object) threadId, (object) reviewId));
      bool headerOptionValue = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request);
      comment.CommentId = commentId;
      return this.Request.CreateResponse<DiscussionComment>(HttpStatusCode.OK, service.SaveComment(this.TfsRequestContext, this.GetProjectId(), reviewId, threadId, comment, !headerOptionValue));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteComment(int reviewId, int threadId, short commentId)
    {
      this.TfsRequestContext.GetService<ICodeReviewCommentService>().DeleteComment(this.TfsRequestContext, this.GetProjectId(), reviewId, threadId, commentId);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
