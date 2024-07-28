// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CommentThreadsController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "threads")]
  public class CommentThreadsController : CodeReviewApiControllerBase
  {
    [ClientResponseType(typeof (CommentThread), null, null)]
    [HttpPost]
    public HttpResponseMessage CreateThread(int reviewId, CommentThread newThread)
    {
      if (newThread == null)
        throw new ArgumentNullException(nameof (newThread), CodeReviewResources.ReviewCommentThreadWasMalformed());
      if (newThread.DiscussionId > 0)
        throw new CodeReviewCommentThreadCreateIdException(newThread.DiscussionId);
      ICodeReviewCommentService service = this.TfsRequestContext.GetService<ICodeReviewCommentService>();
      bool headerOptionValue = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request);
      return this.Request.CreateResponse<CommentThread>(HttpStatusCode.Created, service.SaveCommentThreads(this.TfsRequestContext, this.GetProjectId(), reviewId, (IEnumerable<CommentThread>) new CommentThread[1]
      {
        newThread
      }, (!headerOptionValue ? 1 : 0) != 0).FirstOrDefault<CommentThread>());
    }

    [HttpGet]
    [ClientResponseType(typeof (CommentThread), null, null)]
    [ClientHeaderParameter("If-Modified-Since", typeof (DateTimeOffset), "ifModifiedSince", "Fetch latest comment data if it was modified after IfModifiedSince timestamp.", true, false)]
    public HttpResponseMessage GetThread(
      int reviewId,
      int threadId,
      [ModelBinder] CommentTrackingCriteria trackingCriteria = null)
    {
      ICodeReviewCommentService service = this.TfsRequestContext.GetService<ICodeReviewCommentService>();
      bool headerOptionValue = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.GetProjectId();
      int reviewId1 = reviewId;
      int threadId1 = threadId;
      CommentTrackingCriteria trackingCriteria1 = trackingCriteria;
      int num = !headerOptionValue ? 1 : 0;
      CommentThread commentThread = service.GetCommentThread(tfsRequestContext, projectId, reviewId1, threadId1, trackingCriteria1, num != 0);
      DateTime? modifiedSince = this.GetModifiedSince();
      return !this.IsModified(new DateTime?(commentThread.LastUpdatedDate), modifiedSince) ? this.Request.CreateResponse(HttpStatusCode.NotModified) : this.Request.CreateResponse<CommentThread>(HttpStatusCode.OK, commentThread);
    }

    [HttpGet]
    [ClientResponseType(typeof (List<CommentThread>), null, null)]
    public HttpResponseMessage GetThreads(
      int reviewId,
      [FromUri(Name = "modifiedSince")] DateTime? modifiedSince = null,
      [ModelBinder] CommentTrackingCriteria trackingCriteria = null)
    {
      ICodeReviewCommentService service = this.TfsRequestContext.GetService<ICodeReviewCommentService>();
      bool headerOptionValue = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request);
      return this.GenerateResponse<CommentThread>((IEnumerable<CommentThread>) service.GetCommentThreads(this.TfsRequestContext, this.GetProjectId(), reviewId, modifiedSince, trackingCriteria, !headerOptionValue));
    }

    [ClientResponseType(typeof (CommentThread), null, null)]
    [HttpPatch]
    public HttpResponseMessage UpdateThread(int reviewId, int threadId, CommentThread thread)
    {
      if (thread == null)
        throw new ArgumentNullException(nameof (thread), CodeReviewResources.ReviewCommentThreadWasMalformed());
      thread.DiscussionId = threadId;
      ICodeReviewCommentService service = this.TfsRequestContext.GetService<ICodeReviewCommentService>();
      bool headerOptionValue = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request);
      return this.Request.CreateResponse<CommentThread>(HttpStatusCode.Created, service.SaveCommentThreads(this.TfsRequestContext, this.GetProjectId(), reviewId, (IEnumerable<CommentThread>) new CommentThread[1]
      {
        thread
      }, (!headerOptionValue ? 1 : 0) != 0).FirstOrDefault<CommentThread>());
    }
  }
}
