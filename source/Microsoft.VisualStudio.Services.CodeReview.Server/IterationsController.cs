// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.IterationsController
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

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "iterations")]
  public class IterationsController : CodeReviewApiControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (List<Iteration>), null, null)]
    public HttpResponseMessage GetIterations(int reviewId, [FromUri(Name = "includeUnpublished")] bool includeUnpublished = false) => this.GenerateResponse<Iteration>((IEnumerable<Iteration>) this.TfsRequestContext.GetService<ICodeReviewIterationService>().GetIterations(this.TfsRequestContext, this.GetProjectId(), reviewId, includeUnpublished));

    [HttpGet]
    [ClientResponseType(typeof (Iteration), null, null)]
    public HttpResponseMessage GetIteration(int reviewId, int iterationId) => this.Request.CreateResponse<Iteration>(HttpStatusCode.OK, this.TfsRequestContext.GetService<ICodeReviewIterationService>().GetIteration(this.TfsRequestContext, this.GetProjectId(), reviewId, iterationId) ?? throw new CodeReviewIterationNotFoundException(iterationId));

    [HttpPost]
    [ClientResponseType(typeof (Iteration), null, null)]
    public HttpResponseMessage CreateIteration(int reviewId, Iteration iteration)
    {
      ICodeReviewIterationService service = this.TfsRequestContext.GetService<ICodeReviewIterationService>();
      ValidationHelper.ValidateIterationPayloadReviewId(reviewId, iteration);
      if (!iteration.Id.HasValue)
        throw new ArgumentException(CodeReviewResources.MustSpecifyIterationId(), "id");
      if (iteration.Statuses != null && iteration.Statuses.Any<Status>())
        throw new ArgumentException(CodeReviewResources.StatusesCannotBeCreated(), "statusList");
      iteration.ReviewId = reviewId;
      return this.Request.CreateResponse<Iteration>(HttpStatusCode.Created, service.SaveIteration(this.TfsRequestContext, this.GetProjectId(), iteration, false));
    }

    [HttpPatch]
    [ClientResponseType(typeof (Iteration), null, null)]
    public HttpResponseMessage UpdateIteration(int reviewId, int iterationId, Iteration iteration)
    {
      ICodeReviewIterationService service = this.TfsRequestContext.GetService<ICodeReviewIterationService>();
      ValidationHelper.ValidateIterationPayloadReviewId(reviewId, iteration);
      if (iteration.Id.HasValue)
      {
        int? id = iteration.Id;
        if (id.Value != 0)
        {
          id = iteration.Id;
          if (id.Value != iterationId)
            throw new ArgumentException(CodeReviewResources.MismatchedIterationIds((object) iterationId, (object) iteration.Id), "id");
        }
      }
      if (iteration.Properties != null && iteration.Properties.Count > 0)
        throw new ArgumentException(CodeReviewResources.IterationPropertiesCannotBeUpdated(), nameof (iteration));
      if (iteration.Statuses != null && iteration.Statuses.Any<Status>())
        throw new ArgumentException(CodeReviewResources.StatusesCannotBeUpdated(), "statusList");
      iteration.ReviewId = reviewId;
      iteration.Id = new int?(iterationId);
      return this.Request.CreateResponse<Iteration>(HttpStatusCode.OK, service.SaveIteration(this.TfsRequestContext, this.GetProjectId(), iteration, true));
    }
  }
}
