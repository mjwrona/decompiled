// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.RatingAndReviewService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.Security.AntiXss;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class RatingAndReviewService : IRatingAndReviewService, IVssFrameworkService
  {
    private readonly IRatingAndReviewUtils _rnrUtils;
    private const string s_area = "gallery";
    private const string s_layer = "reviewsservice";
    private const byte MAXRATING = 5;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public RatingAndReviewService() => this._rnrUtils = (IRatingAndReviewUtils) new RatingAndReviewUtils();

    internal RatingAndReviewService(IRatingAndReviewUtils rnRUtils) => this._rnrUtils = rnRUtils;

    public void IgnoreReview(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      long reviewId,
      bool setIgnoreState)
    {
      try
      {
        string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension?.Publisher?.PublisherName, extension?.ExtensionName);
        using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
          component.IgnoreReview(fullyQualifiedName, reviewId, setIgnoreState);
        DailyStatsHelper.RefreshAverageRatingStat(requestContext, extension);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061007, "gallery", "reviewsservice", ex);
        throw;
      }
    }

    public ReviewsResult GetReviews(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      int count = 10,
      ReviewFilterOptions filterOptions = ReviewFilterOptions.None,
      DateTime? beforeDate = null,
      DateTime? afterDate = null)
    {
      ReviewsResult reviewsResult = (ReviewsResult) null;
      requestContext.TraceEnter(12061008, "gallery", "reviewsservice", nameof (GetReviews));
      Stopwatch stopWatch = Stopwatch.StartNew();
      count = count > 100 ? 100 : count;
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension?.Publisher?.PublisherName, extension?.ExtensionName);
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 1, 100);
      try
      {
        using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
          reviewsResult = component.GetReviews(fullyQualifiedName, count, filterOptions, beforeDate, afterDate);
        int count1 = reviewsResult.Reviews.Count;
        ReviewsResult reviews = this.PostProcessResults(this.PopulateUserInfo(requestContext, reviewsResult), filterOptions);
        int count2 = reviews.Reviews.Count;
        if (count1 > count2)
          reviews.HasMoreReviews = true;
        CustomerIntelligenceData dataForGetReviews = this.GetCIDataForGetReviews(fullyQualifiedName, count, filterOptions, beforeDate, afterDate, count1, count2, reviews.HasMoreReviews, stopWatch);
        this.PublishCustomerIntelligenceEvent(requestContext, dataForGetReviews);
        return reviews;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061007, "gallery", "reviewsservice", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12061009, "gallery", "reviewsservice", nameof (GetReviews));
      }
    }

    public List<Review> GetReviewsByUserId(
      IVssRequestContext requestContext,
      Guid userId,
      PublishedExtension extension = null)
    {
      try
      {
        string productId = (string) null;
        if (extension != null && extension.Publisher?.PublisherName != null && extension != null && extension.ExtensionName != null)
          productId = GalleryUtil.CreateFullyQualifiedName(extension?.Publisher?.PublisherName, extension?.ExtensionName);
        using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
          return component.GetReviewsByUserId(userId, productId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061007, "gallery", "reviewsservice", ex);
        throw;
      }
    }

    public int AnonymizeReviews(IVssRequestContext requestContext, Guid userId)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      int num = 0;
      using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
      {
        if (component is RatingAndReviewComponent17 reviewComponent17)
          num = reviewComponent17.AnonymizeReviews(userId);
      }
      return num;
    }

    public IEnumerable<ReviewReply> GetPublisherReplyByUserId(
      IVssRequestContext requestContext,
      Guid userId)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      IEnumerable<ReviewReply> publisherReplyByUserId = (IEnumerable<ReviewReply>) null;
      using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
      {
        if (component is RatingAndReviewComponent17 reviewComponent17)
          publisherReplyByUserId = reviewComponent17.GetPublisherReplyByUserId(userId);
      }
      return publisherReplyByUserId;
    }

    public int AnonymizePublisherReply(IVssRequestContext requestContext, Guid userId)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      int num = 0;
      using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
      {
        if (component is RatingAndReviewComponent17 reviewComponent17)
          num = reviewComponent17.AnonymizePublisherReply(userId);
      }
      return num;
    }

    public Review CreateReview(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Review review)
    {
      requestContext.TraceEnter(12061010, "gallery", "reviewsservice", nameof (CreateReview));
      Stopwatch stopWatch = Stopwatch.StartNew();
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = this._rnrUtils.GetAuthenticatedIdentity(requestContext);
        if (authenticatedIdentity == null)
          throw new AnonymousReviewSubmissionException(GalleryResources.CannotSubmitReviewAnonymously());
        this._rnrUtils.ValidateReview(review);
        review = this._rnrUtils.SanitizeReview(review);
        this.CheckAndThrowOnUserIdMismatch(review.UserId, authenticatedIdentity.Id);
        Review review1 = (Review) null;
        using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
          review1 = component.CreateReview(review.Rating, review.Title, review.Text, review.ProductId, review.ProductVersion, authenticatedIdentity.Id, DateTime.UtcNow);
        this.SendReviewMailNotificationToPublisher(requestContext, review1.ProductId, review1.Text, review1.Rating.ToString(), review1.CreatedDate, false);
        ReviewEventProperties reviewEventProperties = new ReviewEventProperties()
        {
          ReviewId = review1.Id,
          ResourceType = ReviewResourceType.Review,
          EventOperation = ReviewEventOperation.Create,
          ReviewText = review1.Text,
          ReviewDate = review1.UpdatedDate,
          UserId = review1.UserId,
          Rating = (int) review1.Rating,
          IsIgnored = review1.IsIgnored
        };
        this.LogReviewEvent(requestContext, extension, reviewEventProperties);
        DailyStatsHelper.RefreshAverageRatingStat(requestContext, extension);
        CustomerIntelligenceData dataForCreateReview = this.GetCIDataForCreateReview(review, stopWatch);
        this.PublishCustomerIntelligenceEvent(requestContext, dataForCreateReview);
        return review1;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061007, "gallery", "reviewsservice", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12061011, "gallery", "reviewsservice", nameof (CreateReview));
      }
    }

    public ReviewPatch PatchReview(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      long reviewId,
      ReviewPatch reviewPatch)
    {
      requestContext.TraceEnter(12061021, "gallery", "reviewsservice", nameof (PatchReview));
      Stopwatch stopWatch = Stopwatch.StartNew();
      string fullyQualifiedName;
      ReviewPatch reviewPatch1;
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = this._rnrUtils.GetAuthenticatedIdentity(requestContext);
        if (authenticatedIdentity == null)
          throw new AnonymousReviewUpdationException(GalleryResources.CannotUpdateReviewAnonymously());
        fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension?.Publisher?.PublisherName, extension?.ExtensionName);
        this._rnrUtils.ValidateReviewPatch(reviewPatch, fullyQualifiedName, reviewId);
        reviewPatch = this._rnrUtils.SanitizeReviewPatch(reviewPatch);
        if (reviewPatch.Operation == ReviewPatchOperation.FlagReview)
        {
          this.CheckAndThrowOnUserIdMismatch(reviewPatch.ReportedConcern.UserId, authenticatedIdentity.Id);
          reviewPatch1 = this.FlagReview(requestContext, authenticatedIdentity.Id, fullyQualifiedName, reviewId, reviewPatch);
        }
        else if (reviewPatch.Operation == ReviewPatchOperation.UpdateReview)
        {
          this.CheckAndThrowOnUserIdMismatch(reviewPatch.ReviewItem.UserId, authenticatedIdentity.Id);
          reviewPatch1 = this.UpdateReview(requestContext, authenticatedIdentity.Id, fullyQualifiedName, reviewId, reviewPatch);
          this.LogReviewEvent(requestContext, extension, reviewPatch);
        }
        else if (reviewPatch.Operation == ReviewPatchOperation.ReplyToReview)
        {
          this.CheckAndThrowOnUserIdMismatch(reviewPatch.ReviewItem.Reply.UserId, authenticatedIdentity.Id);
          reviewPatch1 = this.UpdateReplyToReview(requestContext, authenticatedIdentity.Id, reviewId, reviewPatch);
          this.LogReviewEvent(requestContext, extension, reviewPatch);
        }
        else if (reviewPatch.Operation == ReviewPatchOperation.AdminResponseForReview)
        {
          if (reviewPatch.ReviewItem.IsIgnored)
            this.IgnoreReview(requestContext, extension, reviewId, true);
          reviewPatch1 = this.UpdateReplyToReview(requestContext, authenticatedIdentity.Id, reviewId, reviewPatch, true);
          this.LogReviewEvent(requestContext, extension, reviewPatch);
        }
        else if (reviewPatch.Operation == ReviewPatchOperation.DeleteAdminReply)
        {
          this.DeleteReply(requestContext, extension, reviewId, true);
          reviewPatch1 = reviewPatch;
          reviewPatch1.ReviewItem.AdminReply = (ReviewReply) null;
        }
        else
        {
          if (reviewPatch.Operation != ReviewPatchOperation.DeletePublisherReply)
            throw new ReviewPatchOperationNotSupportedException(GalleryResources.ReviewPatchOperationNotSupported((object) reviewPatch.Operation.ToString()));
          this.DeleteReply(requestContext, extension, reviewId, false);
          reviewPatch1 = reviewPatch;
          reviewPatch1.ReviewItem.Reply = (ReviewReply) null;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061007, "gallery", "reviewsservice", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12061022, "gallery", "reviewsservice", nameof (PatchReview));
      }
      DailyStatsHelper.RefreshAverageRatingStat(requestContext, extension);
      CustomerIntelligenceData dataForPatchReview = this.GetCIDataForPatchReview(fullyQualifiedName, reviewId, reviewPatch, stopWatch);
      this.PublishCustomerIntelligenceEvent(requestContext, dataForPatchReview);
      return reviewPatch1;
    }

    public ReviewSummary GetReviewSummary(
      IVssRequestContext requestContext,
      string productId,
      DateTime? beforeDate = null,
      DateTime? afterDate = null)
    {
      try
      {
        using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
          return component.GetReviewSummary(productId, beforeDate, afterDate);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061007, "gallery", "reviewsservice", ex);
        throw;
      }
    }

    public ReviewSummary GetReviewSummary(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      DateTime? beforeDate = null,
      DateTime? afterDate = null)
    {
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension?.Publisher?.PublisherName, extension?.ExtensionName);
      return this.GetReviewSummary(requestContext, fullyQualifiedName, beforeDate, afterDate);
    }

    public int DeleteReplies(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      long? reviewId = null)
    {
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension?.Publisher?.PublisherName, extension?.ExtensionName);
      return this.DeleteReplies(requestContext, fullyQualifiedName, reviewId);
    }

    public int DeleteReplies(IVssRequestContext requestContext, string productId, long? reviewId = null)
    {
      try
      {
        using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
          return component.DeleteReviewReply(productId, reviewId, true, false);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061007, "gallery", "reviewsservice", ex);
        throw;
      }
    }

    public int DeleteReviews(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      long? reviewId = null)
    {
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension?.Publisher?.PublisherName, extension?.ExtensionName);
      int num = this.DeleteReviews(requestContext, fullyQualifiedName, reviewId);
      DailyStatsHelper.RefreshAverageRatingStat(requestContext, extension);
      return num;
    }

    public int DeleteReviews(IVssRequestContext requestContext, string productId, long? reviewId = null)
    {
      try
      {
        using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
          return component.DeleteReviews(productId, reviewId, true);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061007, "gallery", "reviewsservice", ex);
        throw;
      }
    }

    public void DeleteReply(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      long reviewId,
      bool isAdminReply)
    {
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension?.Publisher?.PublisherName, extension?.ExtensionName);
      using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
      {
        if (component is RatingAndReviewComponent15)
          component.DeleteReviewReply(fullyQualifiedName, new long?(reviewId), false, isAdminReply);
      }
      if (!isAdminReply)
        return;
      this.IgnoreReview(requestContext, extension, reviewId, false);
    }

    public List<FlaggedReview> GetFlaggedReviews(
      IVssRequestContext requestContext,
      PublishedExtension extension = null)
    {
      try
      {
        string productId = (string) null;
        if (extension != null && extension.Publisher?.PublisherName != null && extension != null && extension.ExtensionName != null)
          productId = GalleryUtil.CreateFullyQualifiedName(extension?.Publisher?.PublisherName, extension?.ExtensionName);
        using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
          return component.GetFlaggedReviews(productId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061007, "gallery", "reviewsservice", ex);
        throw;
      }
    }

    public void PublishReCaptchaTokenCI(
      IVssRequestContext requestContext,
      IDictionary<string, object> ciData)
    {
      this._rnrUtils.PublishReCaptchaTokenCIForReview(requestContext, ciData);
    }

    private ReviewPatch UpdateReview(
      IVssRequestContext requestContext,
      Guid userId,
      string productId,
      long reviewId,
      ReviewPatch reviewPatch)
    {
      reviewPatch.ReviewItem.Id = reviewId;
      reviewPatch.ReviewItem.ProductId = productId;
      ReviewPatch reviewPatch1 = reviewPatch;
      Review reviewItem = reviewPatch.ReviewItem;
      reviewItem.UserId = userId;
      Review review = (Review) null;
      using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
        review = component.UpdateReview(productId, reviewId, reviewItem.Rating, reviewItem.Title, reviewItem.Text, reviewItem.ProductVersion, reviewItem.UserId, DateTime.UtcNow);
      reviewPatch1.ReviewItem = review;
      this.SendReviewMailNotificationToPublisher(requestContext, review.ProductId, review.Text, review.Rating.ToString((IFormatProvider) CultureInfo.InvariantCulture), review.CreatedDate, true);
      return reviewPatch1;
    }

    private ReviewPatch FlagReview(
      IVssRequestContext requestContext,
      Guid userId,
      string productId,
      long reviewId,
      ReviewPatch reviewPatch)
    {
      long num = 0;
      reviewPatch.ReportedConcern.ReviewId = reviewId;
      reviewPatch.ReportedConcern.UserId = userId;
      UserReportedConcern reportedConcern = reviewPatch.ReportedConcern;
      using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
        num = component.FlagReview(productId, reportedConcern.ReviewId, reportedConcern.UserId, reportedConcern.Category, reportedConcern.ConcernText);
      reviewPatch.ReportedConcern.ReviewId = num;
      return reviewPatch;
    }

    private ReviewPatch UpdateReplyToReview(
      IVssRequestContext requestContext,
      Guid userId,
      long reviewId,
      ReviewPatch reviewPatch,
      bool isAdminReply = false)
    {
      ReviewPatch review1 = reviewPatch;
      ReviewReply reviewReply = !isAdminReply ? reviewPatch.ReviewItem.Reply : reviewPatch.ReviewItem.AdminReply;
      reviewReply.ReviewId = reviewId;
      reviewReply.UserId = userId;
      Review review2 = (Review) null;
      using (RatingAndReviewComponent component = requestContext.CreateComponent<RatingAndReviewComponent>())
        review2 = component.UpdatePublisherReply(reviewReply.ReviewId, reviewReply.UserId, reviewReply.Title, reviewReply.ReplyText, DateTime.UtcNow, isAdminReply);
      review1.ReviewItem = review2;
      this.SendReviewMailNotificationToUser(requestContext, review1.ReviewItem.UserId, review1.ReviewItem.ProductId, reviewReply.ReplyText, isAdminReply);
      return review1;
    }

    private ReviewsResult PostProcessResults(
      ReviewsResult reviewsResult,
      ReviewFilterOptions reviewFilterOptions)
    {
      if (reviewsResult == null || reviewsResult.Reviews == null || reviewFilterOptions == ReviewFilterOptions.None || (reviewFilterOptions & ReviewFilterOptions.FilterEmptyUserNames) != ReviewFilterOptions.FilterEmptyUserNames)
        return reviewsResult;
      reviewsResult.Reviews.RemoveAll((Predicate<Review>) (reviewItem => string.IsNullOrEmpty(reviewItem.UserDisplayName)));
      return reviewsResult;
    }

    private ReviewsResult PopulateUserInfo(
      IVssRequestContext requestContext,
      ReviewsResult reviewsResult)
    {
      if (reviewsResult == null || reviewsResult.Reviews == null)
        return reviewsResult;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      List<Guid> userIds = new List<Guid>();
      Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.ReviewPendingProfileQuery"))
      {
        foreach (Review review in reviewsResult.Reviews)
          userIds.Add(review.UserId);
        using (PendingUserProfileComponent component = requestContext.CreateComponent<PendingUserProfileComponent>())
          dictionary = component.GetPendingProfileNames(userIds);
      }
      foreach (Review review in reviewsResult.Reviews)
      {
        review.UserDisplayName = string.Empty;
        if (dictionary.ContainsKey(review.UserId))
        {
          review.UserDisplayName = dictionary[review.UserId];
          review.UserId = Guid.Empty;
        }
        else
        {
          try
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
            if (!Guid.Empty.Equals(review.UserId))
              identity = service.ReadIdentities(vssRequestContext, (IList<Guid>) new Guid[1]
              {
                review.UserId
              }, QueryMembership.None, (IEnumerable<string>) null).First<Microsoft.VisualStudio.Services.Identity.Identity>();
            review.UserDisplayName = identity?.DisplayName;
          }
          catch (Exception ex)
          {
            vssRequestContext.TraceException(12061017, "gallery", "reviewsservice", ex);
          }
        }
        if (string.IsNullOrEmpty(review.UserDisplayName))
        {
          review.UserDisplayName = GalleryResources.DeletedUser();
          review.UserId = Guid.Empty;
        }
      }
      return reviewsResult;
    }

    private void CheckAndThrowOnUserIdMismatch(
      Guid userIdFromRequestBody,
      Guid userIdFromRequestContext)
    {
      if (userIdFromRequestBody != userIdFromRequestContext)
        throw new ArgumentException(GalleryResources.UserIdMismtach());
    }

    private void LogReviewEvent(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      ReviewPatch reviewPatch)
    {
      ReviewEventProperties reviewEventProperties1 = new ReviewEventProperties()
      {
        ReviewId = reviewPatch.ReviewItem.Id,
        EventOperation = ReviewEventOperation.Update,
        ReviewText = reviewPatch.ReviewItem.Text,
        ReviewDate = reviewPatch.ReviewItem.UpdatedDate,
        Rating = (int) reviewPatch.ReviewItem.Rating,
        UserId = reviewPatch.ReviewItem.UserId,
        IsIgnored = reviewPatch.ReviewItem.IsIgnored
      };
      switch (reviewPatch.Operation)
      {
        case ReviewPatchOperation.UpdateReview:
          reviewEventProperties1.ReplyText = reviewPatch.ReviewItem.Reply?.ReplyText;
          reviewEventProperties1.ReplyDate = reviewPatch.ReviewItem.Reply?.UpdatedDate;
          ReviewEventProperties reviewEventProperties2 = reviewEventProperties1;
          ReviewReply reply = reviewPatch.ReviewItem.Reply;
          int num;
          if (reply == null)
          {
            num = 1;
          }
          else
          {
            Guid userId = reply.UserId;
            num = 0;
          }
          Guid guid = num != 0 ? Guid.Empty : reviewPatch.ReviewItem.Reply.UserId;
          reviewEventProperties2.ReplyUserId = guid;
          reviewEventProperties1.ResourceType = ReviewResourceType.Review;
          if (reviewPatch.ReviewItem.AdminReply != null && (reviewEventProperties1.ReplyText == null || reviewPatch.ReviewItem.AdminReply.UpdatedDate > reviewEventProperties1.ReplyDate.Value))
          {
            reviewEventProperties1.ReplyText = reviewPatch.ReviewItem.AdminReply.ReplyText;
            reviewEventProperties1.ReplyDate = new DateTime?(reviewPatch.ReviewItem.AdminReply.UpdatedDate);
            reviewEventProperties1.ReplyUserId = reviewPatch.ReviewItem.AdminReply.UserId;
            reviewEventProperties1.IsAdminReply = true;
            break;
          }
          break;
        case ReviewPatchOperation.ReplyToReview:
          reviewEventProperties1.ReplyText = reviewPatch.ReviewItem.Reply.ReplyText;
          reviewEventProperties1.ReplyDate = new DateTime?(reviewPatch.ReviewItem.Reply.UpdatedDate);
          reviewEventProperties1.ReplyUserId = reviewPatch.ReviewItem.Reply.UserId;
          reviewEventProperties1.ResourceType = ReviewResourceType.PublisherReply;
          break;
        case ReviewPatchOperation.AdminResponseForReview:
          reviewEventProperties1.ReplyText = reviewPatch.ReviewItem.AdminReply.ReplyText;
          reviewEventProperties1.ReplyDate = new DateTime?(reviewPatch.ReviewItem.AdminReply.UpdatedDate);
          reviewEventProperties1.ReplyUserId = reviewPatch.ReviewItem.AdminReply.UserId;
          reviewEventProperties1.IsAdminReply = true;
          reviewEventProperties1.ResourceType = ReviewResourceType.AdminReply;
          break;
      }
      this.LogReviewEvent(requestContext, extension, reviewEventProperties1);
    }

    private void LogReviewEvent(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      ReviewEventProperties reviewEventProperties)
    {
      List<ExtensionEvents> extensionEvents1 = new List<ExtensionEvents>();
      ExtensionEvents extensionEvents2 = new ExtensionEvents();
      extensionEvents2.PublisherName = extension.Publisher.PublisherName;
      extensionEvents2.ExtensionName = extension.ExtensionName;
      extensionEvents2.ExtensionId = extension.ExtensionId;
      ReviewsResult reviewsResult = this.PopulateUserInfo(requestContext, new ReviewsResult()
      {
        Reviews = new List<Review>()
        {
          new Review() { UserId = reviewEventProperties.UserId }
        },
        TotalReviewCount = 1L
      });
      reviewEventProperties.UserDisplayName = reviewsResult.Reviews[0].UserDisplayName;
      extensionEvents2.Events = (IDictionary<string, IEnumerable<ExtensionEvent>>) new Dictionary<string, IEnumerable<ExtensionEvent>>();
      extensionEvents2.Events.Add("review", (IEnumerable<ExtensionEvent>) new List<ExtensionEvent>()
      {
        new ExtensionEvent()
        {
          Properties = JObject.FromObject((object) reviewEventProperties),
          StatisticDate = DateTime.UtcNow,
          Version = extension.Versions[0].Version
        }
      });
      extensionEvents1.Add(extensionEvents2);
      requestContext.GetService<IExtensionDailyStatsService>().AddExtensionEvents(requestContext, (IEnumerable<ExtensionEvents>) extensionEvents1);
    }

    private CustomerIntelligenceData GetCIDataForGetReviews(
      string productId,
      int count,
      ReviewFilterOptions filterOptions,
      DateTime? beforeDate,
      DateTime? afterDate,
      int dbReturnedReviewsCount,
      int postProcessReviewsCount,
      bool hasMoreReviews,
      Stopwatch stopWatch)
    {
      CustomerIntelligenceData dataForGetReviews = new CustomerIntelligenceData();
      dataForGetReviews.Add(CustomerIntelligenceProperty.Action, "GetReviews");
      dataForGetReviews.Add("ProductId", productId);
      dataForGetReviews.Add("Count", (double) count);
      dataForGetReviews.Add("FilterOptions", (double) filterOptions);
      DateTime dateTime;
      if (beforeDate.HasValue)
      {
        CustomerIntelligenceData intelligenceData = dataForGetReviews;
        dateTime = beforeDate.Value;
        // ISSUE: variable of a boxed type
        __Boxed<DateTime> universalTime = (ValueType) dateTime.ToUniversalTime();
        intelligenceData.Add("BeforeDateUTC", (object) universalTime);
      }
      if (afterDate.HasValue)
      {
        CustomerIntelligenceData intelligenceData = dataForGetReviews;
        dateTime = afterDate.Value;
        // ISSUE: variable of a boxed type
        __Boxed<DateTime> universalTime = (ValueType) dateTime.ToUniversalTime();
        intelligenceData.Add("AfterDateUTC", (object) universalTime);
      }
      dataForGetReviews.Add("DbReturnedReviewsCount", (double) dbReturnedReviewsCount);
      dataForGetReviews.Add("PostProcessReviewsCount", (double) postProcessReviewsCount);
      dataForGetReviews.Add("HasMoreReviews", hasMoreReviews);
      stopWatch.Stop();
      dataForGetReviews.Add("Duration", (double) stopWatch.ElapsedMilliseconds);
      return dataForGetReviews;
    }

    private CustomerIntelligenceData GetCIDataForCreateReview(Review review, Stopwatch stopWatch)
    {
      CustomerIntelligenceData dataForCreateReview = new CustomerIntelligenceData();
      dataForCreateReview.Add(CustomerIntelligenceProperty.Action, "CreateReview");
      dataForCreateReview.Add("Rating", (double) review.Rating);
      stopWatch.Stop();
      dataForCreateReview.Add("Duration", (double) stopWatch.ElapsedMilliseconds);
      return dataForCreateReview;
    }

    private CustomerIntelligenceData GetCIDataForPatchReview(
      string productId,
      long reviewId,
      ReviewPatch reviewPatch,
      Stopwatch stopWatch)
    {
      CustomerIntelligenceData dataForPatchReview = new CustomerIntelligenceData();
      dataForPatchReview.Add(CustomerIntelligenceProperty.Action, "PatchReview");
      dataForPatchReview.Add("ProductId", productId);
      dataForPatchReview.Add("ReviewId", (double) reviewId);
      if (reviewPatch.Operation == ReviewPatchOperation.FlagReview)
      {
        dataForPatchReview.Add("Operation", "FlagReview");
        dataForPatchReview.Add("ConcernCategory", reviewPatch.ReportedConcern.Category.ToString());
        dataForPatchReview.Add("ConcernText", reviewPatch.ReportedConcern.ConcernText);
      }
      else if (reviewPatch.Operation == ReviewPatchOperation.UpdateReview)
      {
        dataForPatchReview.Add("Operation", "UpdateReview");
        dataForPatchReview.Add("Rating", (double) reviewPatch.ReviewItem.Rating);
      }
      else if (reviewPatch.Operation == ReviewPatchOperation.ReplyToReview)
        dataForPatchReview.Add("Operation", "ReplyToReview");
      stopWatch.Stop();
      dataForPatchReview.Add("Duration", (double) stopWatch.ElapsedMilliseconds);
      return dataForPatchReview;
    }

    private void PublishCustomerIntelligenceEvent(
      IVssRequestContext requestContext,
      CustomerIntelligenceData data)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "RatingAndReviews", data);
    }

    private void SendReviewMailNotificationToUser(
      IVssRequestContext requestContext,
      Guid userId,
      string productId,
      string replyText,
      bool isAdminReply)
    {
      PublishedExtension extensionFromProdctId = this.GetExtensionFromProdctId(requestContext, productId);
      ReviewMailNotificationEvent notificationEvent = new ReviewMailNotificationEvent(requestContext);
      string displayName1 = extensionFromProdctId.DisplayName;
      string displayName2 = extensionFromProdctId.Publisher.DisplayName;
      string str1 = GalleryResources.ReviewResponseNotifyNote((object) notificationEvent.GetAnchorText(displayName1, GalleryServerUtil.GetGalleryDetailsPageUrl(requestContext, extensionFromProdctId.Publisher.PublisherName, extensionFromProdctId.ExtensionName)), (object) notificationEvent.GetAnchorText(GalleryResources.VisualStudioMarketplaceText(), GalleryServerUtil.GetGalleryUrl(requestContext)));
      string str2;
      if (isAdminReply)
      {
        notificationEvent.UserDisplayName = GalleryResources.AdminText();
        notificationEvent.Subject = GalleryResources.PublisherNotifyMailSubject((object) GalleryResources.AdminText());
        notificationEvent.HeaderNote = GalleryResources.AdminNotifyHeaderNote((object) GalleryResources.AdminText());
        str2 = str1 + GalleryResources.PublisherNotifyMailSubject((object) GalleryResources.AdminText());
      }
      else
      {
        notificationEvent.UserDisplayName = AntiXssEncoder.HtmlEncode(displayName2, false);
        notificationEvent.Subject = GalleryResources.PublisherNotifyMailSubject((object) displayName2);
        notificationEvent.HeaderNote = GalleryResources.PublisherNotifyMailSubject((object) displayName2);
        str2 = str1 + GalleryResources.PublisherNotifyMailResponseText();
      }
      notificationEvent.IntroductionNote = str2;
      notificationEvent.NotificationContent = replyText;
      notificationEvent.ActionButtonText = GalleryResources.ViewOnMarketplaceButtonText();
      notificationEvent.ActionButtonUrl = GalleryServerUtil.GetGalleryDetailsPageUrl(requestContext, extensionFromProdctId.Publisher.PublisherName, extensionFromProdctId.ExtensionName) + "#review-details";
      this.GetMailNotification().SendMailNotificationToUser(requestContext, userId, (MailNotificationEventData) notificationEvent);
    }

    private void SendReviewMailNotificationToPublisher(
      IVssRequestContext requestContext,
      string productId,
      string reviewText,
      string rating,
      DateTime createdDate,
      bool isReviewUpdated)
    {
      PublishedExtension extensionFromProdctId = this.GetExtensionFromProdctId(requestContext, productId);
      string userDisplayName = this.GetUserDisplayName(requestContext);
      ReviewMailNotificationEvent notificationEvent = new ReviewMailNotificationEvent(requestContext);
      string str1 = createdDate.ToString("dd-MMM-yyyy", (IFormatProvider) CultureInfo.InvariantCulture);
      string displayName = extensionFromProdctId.DisplayName;
      string str2 = isReviewUpdated ? GalleryResources.ReviewNotifyMailSubjectUpdated((object) displayName) : GalleryResources.ReviewNotifyMailSubject((object) displayName);
      string str3 = isReviewUpdated ? GalleryResources.ReviewNotifyHeaderNoteUpdated((object) displayName) : GalleryResources.ReviewNotifyHeaderNote((object) displayName);
      string anchorText = notificationEvent.GetAnchorText("extension", GalleryServerUtil.GetGalleryDetailsPageUrl(requestContext, extensionFromProdctId.Publisher.PublisherName, extensionFromProdctId.ExtensionName));
      string str4 = isReviewUpdated ? GalleryResources.ReviewNotifyIntroNoteUpdated((object) anchorText) : GalleryResources.ReviewNotifyIntroNote((object) anchorText);
      notificationEvent.UserDisplayName = userDisplayName;
      notificationEvent.CreatedDate = str1;
      notificationEvent.RatingText = GalleryResources.ReviewNotifyRatingText();
      notificationEvent.Rating = rating;
      notificationEvent.MaxRating = (byte) 5.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      notificationEvent.NotificationContent = reviewText;
      notificationEvent.IsReviewNotification = "true";
      notificationEvent.ActionButtonText = GalleryResources.RespondOnMarketplaceButtonText();
      notificationEvent.ActionButtonUrl = GalleryServerUtil.GetGalleryDetailsPageUrl(requestContext, extensionFromProdctId.Publisher.PublisherName, extensionFromProdctId.ExtensionName) + "#review-details";
      notificationEvent.Subject = str2;
      notificationEvent.HeaderNote = str3;
      notificationEvent.IntroductionNote = str4;
      this.GetMailNotification().SendMailNotificationToPublisher(requestContext, extensionFromProdctId, (MailNotificationEventData) notificationEvent);
    }

    public virtual IMailNotification GetMailNotification() => (IMailNotification) new MailNotification();

    internal virtual string GetUserDisplayName(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return string.IsNullOrEmpty(userIdentity.CustomDisplayName) ? userIdentity.DisplayName : userIdentity.CustomDisplayName;
    }

    private PublishedExtension GetExtensionFromProdctId(
      IVssRequestContext requestContext,
      string productId)
    {
      requestContext.TraceEnter(12061058, "gallery", "reviewsservice", nameof (GetExtensionFromProdctId));
      IVssRequestContext context = requestContext.Elevate();
      IPublishedExtensionService service = context.GetService<IPublishedExtensionService>();
      string publisherName1;
      string extensionName1;
      GalleryServerUtil.GetPublisherNameExtensionNameFromProductId(productId, out publisherName1, out extensionName1);
      IVssRequestContext requestContext1 = context;
      string publisherName2 = publisherName1;
      string extensionName2 = extensionName1;
      PublishedExtension extensionFromProdctId = service.QueryExtension(requestContext1, publisherName2, extensionName2, (string) null, ExtensionQueryFlags.None, (string) null);
      if (extensionFromProdctId == null)
      {
        string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Extension is not found for the ProductId {0}", (object) productId);
        requestContext.Trace(12061058, TraceLevel.Info, "gallery", "reviewsservice", message);
        return (PublishedExtension) null;
      }
      requestContext.TraceLeave(12061058, "gallery", "reviewsservice", nameof (GetExtensionFromProdctId));
      return extensionFromProdctId;
    }
  }
}
