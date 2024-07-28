// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ReviewsController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "reviews")]
  public class ReviewsController : CodeReviewApiControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (IPagedList<Review>), null, null)]
    [ClientLocationId("D17478C8-387D-4359-BA97-1414AE770B76")]
    public HttpResponseMessage GetReviews(
      [ModelBinder] ReviewSearchCriteria searchCriteria,
      [FromUri(Name = "$top")] int? top = null,
      string continuationToken = null)
    {
      ICodeReviewService service = this.TfsRequestContext.GetService<ICodeReviewService>();
      if (!string.IsNullOrEmpty(continuationToken))
      {
        ReviewsContinuationToken token;
        if (!ReviewsContinuationToken.TryParse(continuationToken, out token))
          throw new CodeReviewContinuationTokenInvalidException();
        bool? orderAscending = searchCriteria.OrderAscending;
        if (orderAscending.HasValue)
        {
          orderAscending = searchCriteria.OrderAscending;
          if (orderAscending.Value)
          {
            searchCriteria.MinUpdatedDate = token.UpdatedDate;
            goto label_7;
          }
        }
        searchCriteria.MaxUpdatedDate = token.UpdatedDate;
      }
label_7:
      Guid? projectId = this.GetProjectId(true);
      int topValue;
      ValidationHelper.EvaluateTop(top, out topValue);
      List<Review> source = new List<Review>();
      int totalReviewsFound;
      source.AddRange(service.QueryReviewsByFilters(this.TfsRequestContext, projectId, searchCriteria, topValue + 1, out totalReviewsFound));
      HttpResponseMessage response = this.GenerateResponse<Review>(source.Take<Review>(topValue));
      if (totalReviewsFound > topValue)
        this.SetContinuationToken(response, source.LastOrDefault<Review>());
      return response;
    }

    [HttpGet]
    [ClientResponseType(typeof (Review), null, null)]
    [ClientHeaderParameter("If-Modified-Since", typeof (DateTimeOffset), "ifModifiedSince", "Fetch latest review data if it was modified after IfModifiedSince timestamp.", true, false)]
    [ClientLocationId("EAA8EC98-2B9C-4730-96A3-4845BE1558D6")]
    public HttpResponseMessage GetReview(
      int reviewId,
      [FromUri(Name = "includeAllProperties")] bool includeAllProperties = true,
      [FromUri(Name = "maxChangesCount")] int? maxChangesCount = null)
    {
      ICodeReviewService service = this.TfsRequestContext.GetService<ICodeReviewService>();
      DateTime? modifiedSince = this.GetModifiedSince();
      CodeReviewExtendedProperties extendedProperties = includeAllProperties ? CodeReviewExtendedProperties.All : CodeReviewExtendedProperties.None;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.GetProjectId();
      int reviewId1 = reviewId;
      int num = (int) extendedProperties;
      int? maxChangesCount1 = maxChangesCount;
      Review review = service.GetReview(tfsRequestContext, projectId, reviewId1, (CodeReviewExtendedProperties) num, maxChangesCount: maxChangesCount1);
      return !this.IsModified(review.UpdatedDate, modifiedSince) ? this.Request.CreateResponse(HttpStatusCode.NotModified) : this.Request.CreateResponse<Review>(HttpStatusCode.OK, review);
    }

    [HttpPatch]
    [ClientResponseType(typeof (Review), null, null)]
    [ClientLocationId("EAA8EC98-2B9C-4730-96A3-4845BE1558D6")]
    public HttpResponseMessage UpdateReview(int reviewId, Review review)
    {
      ICodeReviewService service = this.TfsRequestContext.GetService<ICodeReviewService>();
      if (review == null)
        throw new ArgumentNullException(nameof (review), CodeReviewResources.ReviewWasMalformed());
      review.Id = reviewId;
      if (review.Properties != null && review.Properties.Count > 0)
        throw new ArgumentException(CodeReviewResources.ReviewPropertiesCannotBeUpdated(), "properties");
      if (review.Iterations != null && review.Iterations.Count > 0)
        throw new ArgumentException(CodeReviewResources.IterationsCannotBeUpdated(), "iterations");
      if (review.Reviewers != null && review.Reviewers.Any<Reviewer>())
        throw new ArgumentException(CodeReviewResources.ReviewersCannotBeUpdated(), "reviewers");
      if (review.Attachments != null && review.Attachments.Any<Attachment>())
        throw new ArgumentException(CodeReviewResources.AttachmentsCannotBeUpdated(), "attachments");
      if (review.Statuses != null && review.Statuses.Any<Status>())
        throw new ArgumentException(CodeReviewResources.StatusesCannotBeUpdated(), "statusList");
      return this.Request.CreateResponse<Review>(HttpStatusCode.OK, service.SaveReview(this.TfsRequestContext, this.GetProjectId(), review));
    }

    [HttpPut]
    [ClientResponseType(typeof (Review), null, null)]
    [ClientLocationId("EAA8EC98-2B9C-4730-96A3-4845BE1558D6")]
    public HttpResponseMessage ReplaceReview(int reviewId, Review review)
    {
      ICodeReviewService service = this.TfsRequestContext.GetService<ICodeReviewService>();
      if (review == null)
        throw new ArgumentNullException(nameof (review), CodeReviewResources.ReviewWasMalformed());
      review.Id = reviewId;
      return this.Request.CreateResponse<Review>(HttpStatusCode.OK, service.SaveReview(this.TfsRequestContext, this.GetProjectId(), review));
    }

    [HttpPost]
    [ClientResponseType(typeof (Review), null, null)]
    [ClientLocationId("EAA8EC98-2B9C-4730-96A3-4845BE1558D6")]
    public HttpResponseMessage CreateReview(Review review)
    {
      if (review == null)
        throw new ArgumentNullException(nameof (review), CodeReviewResources.ReviewWasMalformed());
      if (review.Attachments != null && review.Attachments.Any<Attachment>())
        throw new ArgumentException(CodeReviewResources.AttachmentsCannotBeCreated(), "attachments");
      if (review.Statuses != null && review.Statuses.Any<Status>())
        throw new ArgumentException(CodeReviewResources.StatusesCannotBeCreated(), "statuses");
      return this.Request.CreateResponse<Review>(HttpStatusCode.Created, this.TfsRequestContext.GetService<ICodeReviewService>().SaveReview(this.TfsRequestContext, this.GetProjectId(), review));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("EAA8EC98-2B9C-4730-96A3-4845BE1558D6")]
    public HttpResponseMessage DeleteReview(int reviewId)
    {
      this.TfsRequestContext.GetService<ICodeReviewService>().DeleteReview(this.TfsRequestContext, this.GetProjectId(), reviewId);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
