// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CommentLikesController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "likes")]
  public class CommentLikesController : CodeReviewApiControllerBase
  {
    [ClientResponseType(typeof (IdentityRef), null, null)]
    [HttpPost]
    public HttpResponseMessage CreateLike(
      int reviewId,
      int threadId,
      short commentId,
      List<IdentityRef> users)
    {
      return this.Request.CreateResponse<IdentityRef>(HttpStatusCode.OK, this.TfsRequestContext.GetService<ICodeReviewCommentService>().LikeComment(this.TfsRequestContext, this.GetProjectId(), reviewId, threadId, commentId, users).FirstOrDefault<IdentityRef>());
    }

    [ClientResponseType(typeof (List<IdentityRef>), null, null)]
    [HttpGet]
    public List<IdentityRef> GetLikes(int reviewId, int threadId, short commentId) => this.TfsRequestContext.GetService<ICodeReviewCommentService>().GetLikes(this.TfsRequestContext, this.GetProjectId(), reviewId, threadId, commentId);

    [ClientResponseType(typeof (void), null, null)]
    [HttpDelete]
    public HttpResponseMessage DeleteLike(
      int reviewId,
      int threadId,
      short commentId,
      Guid userId)
    {
      this.TfsRequestContext.GetService<ICodeReviewCommentService>().WithdrawLikeComment(this.TfsRequestContext, this.GetProjectId(), reviewId, threadId, commentId, new IdentityRef()
      {
        Id = userId.ToString()
      });
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
