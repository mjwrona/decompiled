// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IRatingAndReviewService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (RatingAndReviewService))]
  public interface IRatingAndReviewService : IVssFrameworkService
  {
    ReviewsResult GetReviews(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      int count = 10,
      ReviewFilterOptions filterOptions = ReviewFilterOptions.None,
      DateTime? beforeDate = null,
      DateTime? afterDate = null);

    List<Review> GetReviewsByUserId(
      IVssRequestContext requestContext,
      Guid userId,
      PublishedExtension extension = null);

    Review CreateReview(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Review review);

    ReviewPatch PatchReview(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      long reviewId,
      ReviewPatch reviewPatch);

    ReviewSummary GetReviewSummary(
      IVssRequestContext requestContext,
      string productId,
      DateTime? beforeDate = null,
      DateTime? afterDate = null);

    ReviewSummary GetReviewSummary(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      DateTime? beforeDate = null,
      DateTime? afterDate = null);

    int DeleteReplies(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      long? reviewId = null);

    int DeleteReplies(IVssRequestContext requestContext, string productId, long? reviewId = null);

    int DeleteReviews(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      long? reviewId = null);

    int DeleteReviews(IVssRequestContext requestContext, string productId, long? reviewId = null);

    void IgnoreReview(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      long reviewId,
      bool setIgnoreState);

    List<FlaggedReview> GetFlaggedReviews(
      IVssRequestContext requestContext,
      PublishedExtension extension = null);

    void DeleteReply(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      long reviewId,
      bool isAdminReply);

    int AnonymizeReviews(IVssRequestContext requestContext, Guid userId);

    IEnumerable<ReviewReply> GetPublisherReplyByUserId(
      IVssRequestContext requestContext,
      Guid userId);

    int AnonymizePublisherReply(IVssRequestContext requestContext, Guid userId);

    void PublishReCaptchaTokenCI(
      IVssRequestContext requestContext,
      IDictionary<string, object> ciData);
  }
}
