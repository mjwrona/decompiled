// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ReviewersController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "reviewers")]
  public class ReviewersController : CodeReviewApiControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (List<Reviewer>), null, null)]
    public HttpResponseMessage GetReviewers(int reviewId) => this.GenerateResponse<Reviewer>(this.TfsRequestContext.GetService<ICodeReviewReviewerService>().GetReviewers(this.TfsRequestContext, this.GetProjectId(), reviewId));

    [HttpPost]
    [ClientResponseType(typeof (List<Reviewer>), null, null)]
    public HttpResponseMessage AddReviewers(int reviewId, IEnumerable<Reviewer> reviewers) => this.GenerateResponse<Reviewer>(this.TfsRequestContext.GetService<ICodeReviewReviewerService>().SaveReviewers(this.TfsRequestContext, this.GetProjectId(), reviewId, reviewers));

    [HttpPut]
    [ClientResponseType(typeof (Reviewer), null, null)]
    public HttpResponseMessage ReplaceReviewer(int reviewId, Guid reviewerId, Reviewer reviewer)
    {
      if (reviewer == null)
        throw new ArgumentNullException(nameof (reviewer), "Reviewer was malformed.");
      reviewer.Identity = new IdentityRef()
      {
        Id = reviewerId.ToString()
      };
      return this.Request.CreateResponse<Reviewer>(HttpStatusCode.OK, this.TfsRequestContext.GetService<ICodeReviewReviewerService>().SaveReviewer(this.TfsRequestContext, this.GetProjectId(), reviewId, reviewer));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteReviewer(int reviewId, Guid reviewerId)
    {
      this.TfsRequestContext.GetService<ICodeReviewReviewerService>().RemoveReviewer(this.TfsRequestContext, this.GetProjectId(), reviewId, reviewerId);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
