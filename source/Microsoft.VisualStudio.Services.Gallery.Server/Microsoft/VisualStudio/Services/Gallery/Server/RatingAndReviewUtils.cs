// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.RatingAndReviewUtils
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class RatingAndReviewUtils : IRatingAndReviewUtils
  {
    public Microsoft.VisualStudio.Services.Identity.Identity GetAuthenticatedIdentity(
      IVssRequestContext requestContext)
    {
      return requestContext.GetUserIdentity();
    }

    public void ValidateReview(Review review)
    {
      ArgumentUtility.CheckForNull<Review>(review, nameof (review));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(review.ProductId, "review.ProductId");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(review.ProductVersion, "review.ProductVersion");
      ArgumentUtility.CheckForOutOfRange((int) review.Rating, "review.Rating", 1, 5);
    }

    public void ValidateReviewReply(ReviewReply reviewReply)
    {
      ArgumentUtility.CheckForNull<ReviewReply>(reviewReply, nameof (reviewReply));
      ArgumentUtility.CheckForEmptyGuid(reviewReply.UserId, "reviewReply.UserId");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(reviewReply.ReplyText, "reviewReply.ReplyText");
    }

    public void ValidateReviewPatch(ReviewPatch reviewPatch, string productId, long reviewId)
    {
      ArgumentUtility.CheckForNull<ReviewPatch>(reviewPatch, nameof (reviewPatch));
      if (reviewPatch.Operation == ReviewPatchOperation.FlagReview)
      {
        ArgumentUtility.CheckForNull<UserReportedConcern>(reviewPatch.ReportedConcern, "reviewPatch.ReportedConcern");
        reviewPatch.ReportedConcern.ReviewId = reviewId;
        this.ValidateReportedConcern(reviewPatch.ReportedConcern);
      }
      else if (reviewPatch.Operation == ReviewPatchOperation.UpdateReview)
      {
        ArgumentUtility.CheckForNull<Review>(reviewPatch.ReviewItem, "reviewPatch.ReviewItem");
        reviewPatch.ReviewItem.Id = reviewId;
        reviewPatch.ReviewItem.ProductId = productId;
        this.ValidateReview(reviewPatch.ReviewItem);
        ArgumentUtility.CheckForOutOfRange(reviewPatch.ReviewItem.Id, "reviewPatch.ReviewItem.Id", 1L, long.MaxValue);
      }
      else if (reviewPatch.Operation == ReviewPatchOperation.ReplyToReview)
      {
        ArgumentUtility.CheckForNull<Review>(reviewPatch.ReviewItem, "reviewPatch.ReviewItem");
        ArgumentUtility.CheckForNull<ReviewReply>(reviewPatch.ReviewItem.Reply, "reviewPath.ReviewItem.Reply");
        reviewPatch.ReviewItem.Id = reviewId;
        reviewPatch.ReviewItem.Reply.ReviewId = reviewId;
        reviewPatch.ReviewItem.ProductId = productId;
        this.ValidateReview(reviewPatch.ReviewItem);
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1L, long.MaxValue);
        this.ValidateReviewReply(reviewPatch.ReviewItem.Reply);
      }
      else if (reviewPatch.Operation == ReviewPatchOperation.AdminResponseForReview)
      {
        ArgumentUtility.CheckForNull<Review>(reviewPatch.ReviewItem, "reviewPatch.ReviewItem");
        ArgumentUtility.CheckForNull<ReviewReply>(reviewPatch.ReviewItem.AdminReply, "reviewPath.ReviewItem.AdminReply");
        reviewPatch.ReviewItem.Id = reviewId;
        reviewPatch.ReviewItem.AdminReply.ReviewId = reviewId;
        reviewPatch.ReviewItem.ProductId = productId;
        ArgumentUtility.CheckForNull<Review>(reviewPatch.ReviewItem, "review");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(reviewPatch.ReviewItem.ProductId, "review.ProductId");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(reviewPatch.ReviewItem.ProductVersion, "review.ProductVersion");
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1L, long.MaxValue);
        ArgumentUtility.CheckStringForNullOrWhiteSpace(reviewPatch.ReviewItem.AdminReply.ReplyText, "reviewReply.ReplyText");
      }
      else
      {
        if (reviewPatch.Operation != ReviewPatchOperation.DeleteAdminReply && reviewPatch.Operation != ReviewPatchOperation.DeletePublisherReply)
          throw new ReviewPatchOperationNotSupportedException(GalleryResources.ReviewPatchOperationNotSupported((object) reviewPatch.Operation.ToString()));
        ArgumentUtility.CheckForNull<Review>(reviewPatch.ReviewItem, "reviewPatch.ReviewItem");
        reviewPatch.ReviewItem.Id = reviewId;
        reviewPatch.ReviewItem.ProductId = productId;
        ArgumentUtility.CheckForNull<Review>(reviewPatch.ReviewItem, "review");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(reviewPatch.ReviewItem.ProductId, "review.ProductId");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(reviewPatch.ReviewItem.ProductVersion, "review.ProductVersion");
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1L, long.MaxValue);
      }
    }

    public void ValidateReportedConcern(UserReportedConcern reportedConcern)
    {
      ArgumentUtility.CheckForOutOfRange(reportedConcern.ReviewId, "reportedConcern.ReviewId", 1L, long.MaxValue);
      ArgumentUtility.CheckForEmptyGuid(reportedConcern.UserId, "reportedConcern.UserId");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(reportedConcern.ConcernText, "reportedConcern.ConcernText");
    }

    public Review SanitizeReview(Review review)
    {
      review.ProductId = review.ProductId.Trim();
      review.ProductId = GalleryServerUtil.TruncateString(review.ProductId, 512);
      review.ProductVersion = review.ProductVersion.Trim();
      review.ProductVersion = GalleryServerUtil.TruncateString(review.ProductVersion, 25);
      if (!string.IsNullOrEmpty(review.Title))
      {
        review.Title = review.Title.Trim();
        review.Title = GalleryServerUtil.TruncateString(review.Title, 512);
      }
      if (!string.IsNullOrEmpty(review.Text))
      {
        review.Text = review.Text.Trim();
        review.Text = GalleryServerUtil.TruncateString(review.Text, 2048);
      }
      return review;
    }

    public UserReportedConcern SanitizeReportedConcern(UserReportedConcern reportedConcern)
    {
      reportedConcern.ConcernText = GalleryServerUtil.TruncateString(reportedConcern.ConcernText, 2048);
      return reportedConcern;
    }

    public ReviewReply SanitizeReviewReply(ReviewReply reviewReply)
    {
      if (!string.IsNullOrEmpty(reviewReply.Title))
      {
        reviewReply.Title = reviewReply.Title.Trim();
        reviewReply.Title = GalleryServerUtil.TruncateString(reviewReply.Title, 512);
      }
      reviewReply.ReplyText = reviewReply.ReplyText.Trim();
      reviewReply.ReplyText = GalleryServerUtil.TruncateString(reviewReply.ReplyText, 2048);
      return reviewReply;
    }

    public ReviewPatch SanitizeReviewPatch(ReviewPatch reviewPatch)
    {
      if (reviewPatch.Operation == ReviewPatchOperation.FlagReview)
        reviewPatch.ReportedConcern = this.SanitizeReportedConcern(reviewPatch.ReportedConcern);
      else if (reviewPatch.Operation == ReviewPatchOperation.UpdateReview)
        reviewPatch.ReviewItem = this.SanitizeReview(reviewPatch.ReviewItem);
      else if (reviewPatch.Operation == ReviewPatchOperation.ReplyToReview)
      {
        reviewPatch.ReviewItem = this.SanitizeReview(reviewPatch.ReviewItem);
        reviewPatch.ReviewItem.Reply = this.SanitizeReviewReply(reviewPatch.ReviewItem.Reply);
      }
      else if (reviewPatch.Operation == ReviewPatchOperation.AdminResponseForReview)
      {
        reviewPatch.ReviewItem = this.SanitizeReview(reviewPatch.ReviewItem);
        reviewPatch.ReviewItem.AdminReply = this.SanitizeReviewReply(reviewPatch.ReviewItem.AdminReply);
      }
      else if (reviewPatch.Operation == ReviewPatchOperation.DeleteAdminReply || reviewPatch.Operation == ReviewPatchOperation.DeletePublisherReply)
        reviewPatch.ReviewItem = this.SanitizeReview(reviewPatch.ReviewItem);
      return reviewPatch;
    }

    public void PublishReCaptchaTokenCIForReview(
      IVssRequestContext requestContext,
      IDictionary<string, object> ciData)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(ciData, nameof (ciData));
      Guid userId = requestContext.GetUserId();
      ciData.Add("Vsid", (object) userId);
      CustomerIntelligenceData properties = new CustomerIntelligenceData(ciData);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", Convert.ToString(ciData[CustomerIntelligenceProperty.Action]), properties);
    }
  }
}
