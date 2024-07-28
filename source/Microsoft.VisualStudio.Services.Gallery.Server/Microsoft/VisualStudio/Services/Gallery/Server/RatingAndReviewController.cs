// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.RatingAndReviewController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "reviews")]
  public class RatingAndReviewController : TfsApiController
  {
    private bool isRequestFromChinaRegion;

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ReviewExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<AnonymousReviewSubmissionException>(HttpStatusCode.Unauthorized);
      exceptionMap.AddStatusCode<ReviewAlreadyReportedException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<ReviewPatchOperationNotSupportedException>(HttpStatusCode.NotImplemented);
      exceptionMap.AddStatusCode<ReviewDoesNotExistException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
    }

    public string LayerName => "RnRController";

    [HttpGet]
    [ClientLocationId("B7B44E21-209E-48F0-AE78-04727FC37D77")]
    public ReviewSummary GetReviewsSummary(
      string pubName,
      string extName,
      [FromUri] DateTime? beforeDate = null,
      [FromUri] DateTime? afterDate = null)
    {
      this.TfsRequestContext.TraceEnter(12061096, "gallery", this.LayerName, "GetReviewSummary");
      try
      {
        PublishedExtension extension = this.CheckIfExtensionExists(pubName, extName);
        return this.TfsRequestContext.GetService<IRatingAndReviewService>().GetReviewSummary(this.TfsRequestContext, extension, beforeDate, afterDate);
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(12061096, "gallery", this.LayerName, "GetReviewSummary");
      }
    }

    [HttpGet]
    [ClientLocationId("5B3F819F-F247-42AD-8C00-DD9AB9AB246D")]
    public ReviewsResult GetReviews(
      string publisherName,
      string extensionName,
      [FromUri] int count = 10,
      [FromUri] ReviewFilterOptions filterOptions = ReviewFilterOptions.None,
      [FromUri] DateTime? beforeDate = null,
      [FromUri] DateTime? afterDate = null)
    {
      this.TfsRequestContext.TraceEnter(12061001, "gallery", this.LayerName, nameof (GetReviews));
      try
      {
        PublishedExtension extension = this.CheckIfExtensionExists(publisherName, extensionName);
        return this.TfsRequestContext.GetService<IRatingAndReviewService>().GetReviews(this.TfsRequestContext, extension, count, filterOptions, beforeDate, afterDate);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(12061003, "gallery", this.LayerName, ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(12061002, "gallery", this.LayerName, nameof (GetReviews));
      }
    }

    [HttpPost]
    [ClientLocationId("E6E85B9D-AA70-40E6-AA28-D0FBF40B91A3")]
    [ClientResponseType(typeof (Review), null, null)]
    public HttpResponseMessage CreateReview(string pubName, string extName, [FromBody] Review review)
    {
      this.TfsRequestContext.TraceEnter(12061004, "gallery", this.LayerName, nameof (CreateReview));
      ArgumentUtility.CheckForNull<Review>(review, nameof (review));
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(pubName, extName);
      try
      {
        if (review != null)
          review.ProductId = fullyQualifiedName;
        IRatingAndReviewService service = this.TfsRequestContext.GetService<IRatingAndReviewService>();
        if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaInReviewAndRating"))
        {
          int num = ReCaptchaUtility.IsReCaptchaTokenValid(this.TfsRequestContext, review.ReCaptchaToken) ? 1 : 0;
          this.isRequestFromChinaRegion = GalleryServerUtil.IsRequestFromChinaRegion(this.TfsRequestContext);
          Dictionary<string, object> ciData = new Dictionary<string, object>()
          {
            {
              CustomerIntelligenceProperty.Action,
              (object) "ReCaptchaValidation"
            },
            {
              "UserAgent",
              (object) this.TfsRequestContext.UserAgent
            },
            {
              "PublisherName",
              (object) pubName
            },
            {
              "ExtensionName",
              (object) extName
            },
            {
              "Id",
              (object) review.Id
            },
            {
              "ProductId",
              (object) review.ProductId
            },
            {
              "ProductVersion",
              (object) review.ProductVersion
            },
            {
              "UserId",
              (object) review.UserId
            },
            {
              "Source",
              (object) "RatingAndReviews"
            },
            {
              "Scenario",
              (object) "CreateScenario"
            },
            {
              "ReCaptchaToken",
              (object) review.ReCaptchaToken
            },
            {
              "IsRequestFromChinaRegion",
              (object) this.isRequestFromChinaRegion
            }
          };
          if (num != 0)
          {
            ciData.Add("FeatureValidation", (object) "Valid");
            service.PublishReCaptchaTokenCI(this.TfsRequestContext, (IDictionary<string, object>) ciData);
          }
          else
          {
            ciData.Add("FeatureValidation", (object) "Invalid");
            service.PublishReCaptchaTokenCI(this.TfsRequestContext, (IDictionary<string, object>) ciData);
            throw new InvalidReCaptchaTokenException(GalleryResources.InvalidReCaptchaToken());
          }
        }
        PublishedExtension extension = this.CheckIfExtensionExists(pubName, extName, review.ProductVersion);
        this.ValidateReviewVersion(extension, review);
        ReviewSummary reviewSummary1 = service.GetReviewSummary(this.TfsRequestContext, extension);
        Review review1 = service.CreateReview(this.TfsRequestContext, extension, review);
        ReviewSummary reviewSummary2 = service.GetReviewSummary(this.TfsRequestContext, extension);
        if ((double) reviewSummary2.AverageRating > 0.0 && reviewSummary2.RatingCount > 0L)
        {
          GalleryServerUtil.UpdateExtensionRatingStatistics(this.TfsRequestContext, new KeyValuePair<string, string>(extension.Publisher.PublisherName, extension.ExtensionName), reviewSummary2.AverageRating, reviewSummary2.RatingCount);
          string str = "";
          if (extension.InstallationTargets != null)
            str = GalleryUtil.GetProductTypeForInstallationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets);
          this.CheckAndInvalidateHomePageCache(reviewSummary1.AverageRating, reviewSummary2.AverageRating, "CreateReview: " + str + " " + extension.Publisher.PublisherName + "." + extension.ExtensionName);
        }
        this.TfsRequestContext.GetService<IGalleryAuditLogService>().LogAuditEntry(this.TfsRequestContext, "PostReview", extension.ExtensionId.ToString(), "Review");
        return this.Request.CreateResponse<Review>(HttpStatusCode.Created, review1);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(12061006, "gallery", this.LayerName, ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(12061005, "gallery", this.LayerName, nameof (CreateReview));
      }
    }

    [HttpPatch]
    [ClientLocationId("E6E85B9D-AA70-40E6-AA28-D0FBF40B91A3")]
    [ClientResponseType(typeof (ReviewPatch), null, null)]
    public HttpResponseMessage UpdateReview(
      string pubName,
      string extName,
      long reviewId,
      [FromBody] ReviewPatch reviewPatch)
    {
      GalleryUtil.CreateFullyQualifiedName(pubName, extName);
      if (reviewPatch == null)
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
          ReasonPhrase = GalleryResources.InvalidReviewPatch()
        });
      string productVersion = reviewPatch.Operation != ReviewPatchOperation.FlagReview ? reviewPatch.ReviewItem.ProductVersion : (string) null;
      IRatingAndReviewService service = this.TfsRequestContext.GetService<IRatingAndReviewService>();
      if (reviewPatch.Operation != ReviewPatchOperation.FlagReview && this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaInReviewAndRating") && (!string.IsNullOrEmpty(reviewPatch.ReviewItem.ReCaptchaToken) || this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaValidationOnNullOrEmptyTokenForRnR")))
      {
        int num = ReCaptchaUtility.IsReCaptchaTokenValid(this.TfsRequestContext, reviewPatch.ReviewItem.ReCaptchaToken) ? 1 : 0;
        this.isRequestFromChinaRegion = GalleryServerUtil.IsRequestFromChinaRegion(this.TfsRequestContext);
        Dictionary<string, object> ciData = new Dictionary<string, object>()
        {
          {
            CustomerIntelligenceProperty.Action,
            (object) "ReCaptchaValidation"
          },
          {
            "UserAgent",
            (object) this.TfsRequestContext.UserAgent
          },
          {
            "PublisherName",
            (object) pubName
          },
          {
            "ExtensionName",
            (object) extName
          },
          {
            "Operation",
            (object) Convert.ToString((object) reviewPatch.Operation)
          },
          {
            "Id",
            (object) reviewPatch.ReviewItem.Id
          },
          {
            "ProductId",
            (object) reviewPatch.ReviewItem.ProductId
          },
          {
            "ProductVersion",
            (object) reviewPatch.ReviewItem.ProductVersion
          },
          {
            "UserId",
            (object) reviewPatch.ReviewItem.UserId
          },
          {
            "Source",
            (object) "RatingAndReviews"
          },
          {
            "Scenario",
            (object) "UpdateScenario"
          },
          {
            "ReCaptchaToken",
            (object) reviewPatch.ReviewItem.ReCaptchaToken
          },
          {
            "IsRequestFromChinaRegion",
            (object) this.isRequestFromChinaRegion
          }
        };
        if (num != 0)
        {
          ciData.Add("FeatureValidation", (object) "Valid");
          service.PublishReCaptchaTokenCI(this.TfsRequestContext, (IDictionary<string, object>) ciData);
        }
        else
        {
          ciData.Add("FeatureValidation", (object) "Invalid");
          service.PublishReCaptchaTokenCI(this.TfsRequestContext, (IDictionary<string, object>) ciData);
          throw new InvalidReCaptchaTokenException(GalleryResources.InvalidReCaptchaToken());
        }
      }
      PublishedExtension extension = this.CheckIfExtensionExists(pubName, extName, productVersion);
      this.ValidateReviewVersionPatch(extension, reviewPatch);
      if (reviewPatch.Operation == ReviewPatchOperation.ReplyToReview)
        GallerySecurity.CheckExtensionPermission(this.TfsRequestContext, extension, (string) null, PublisherPermissions.UpdateExtension, false);
      else if (reviewPatch.Operation == ReviewPatchOperation.AdminResponseForReview || reviewPatch.Operation == ReviewPatchOperation.DeleteAdminReply || reviewPatch.Operation == ReviewPatchOperation.DeletePublisherReply)
        GallerySecurity.CheckRootPermission(this.TfsRequestContext, PublisherPermissions.Admin);
      ReviewSummary reviewSummary1 = new ReviewSummary()
      {
        AverageRating = -1f,
        RatingCount = -1
      };
      if (reviewPatch.Operation == ReviewPatchOperation.UpdateReview || reviewPatch.Operation == ReviewPatchOperation.AdminResponseForReview && reviewPatch.ReviewItem.IsIgnored || reviewPatch.Operation == ReviewPatchOperation.DeleteAdminReply)
        reviewSummary1 = service.GetReviewSummary(this.TfsRequestContext, extension);
      ReviewPatch reviewPatch1 = service.PatchReview(this.TfsRequestContext, extension, reviewId, reviewPatch);
      if (reviewPatch.Operation == ReviewPatchOperation.UpdateReview || reviewPatch.Operation == ReviewPatchOperation.AdminResponseForReview && reviewPatch.ReviewItem.IsIgnored || reviewPatch.Operation == ReviewPatchOperation.DeleteAdminReply)
      {
        ReviewSummary reviewSummary2 = service.GetReviewSummary(this.TfsRequestContext, extension);
        if ((double) reviewSummary2.AverageRating >= 0.0 && reviewSummary2.RatingCount >= 0L)
        {
          GalleryServerUtil.UpdateExtensionRatingStatistics(this.TfsRequestContext, new KeyValuePair<string, string>(extension.Publisher.PublisherName, extension.ExtensionName), reviewSummary2.AverageRating, reviewSummary2.RatingCount);
          string str = "";
          if (extension.InstallationTargets != null)
            str = GalleryUtil.GetProductTypeForInstallationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets);
          this.CheckAndInvalidateHomePageCache(reviewSummary1.AverageRating, reviewSummary2.AverageRating, "UpdateReview: " + str + " " + extension.Publisher.PublisherName + "." + extension.ExtensionName);
        }
      }
      this.TfsRequestContext.GetService<IGalleryAuditLogService>().LogAuditEntry(this.TfsRequestContext, "PostReview", extension.ExtensionId.ToString(), "Review");
      return this.Request.CreateResponse<ReviewPatch>(HttpStatusCode.OK, reviewPatch1);
    }

    [HttpDelete]
    [ClientLocationId("E6E85B9D-AA70-40E6-AA28-D0FBF40B91A3")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteReview(string pubName, string extName, long reviewId)
    {
      GalleryUtil.CreateFullyQualifiedName(pubName, extName);
      PublishedExtension extension = this.CheckIfExtensionExists(pubName, extName, forDeletion: true);
      GallerySecurity.CheckRootPermission(this.TfsRequestContext, PublisherPermissions.Admin);
      IRatingAndReviewService service = this.TfsRequestContext.GetService<IRatingAndReviewService>();
      ReviewSummary reviewSummary1 = service.GetReviewSummary(this.TfsRequestContext, extension);
      service.DeleteReplies(this.TfsRequestContext, extension, new long?(reviewId));
      service.DeleteReviews(this.TfsRequestContext, extension, new long?(reviewId));
      ReviewSummary reviewSummary2 = service.GetReviewSummary(this.TfsRequestContext, extension);
      GalleryServerUtil.UpdateExtensionRatingStatistics(this.TfsRequestContext, new KeyValuePair<string, string>(extension.Publisher.PublisherName, extension.ExtensionName), reviewSummary2.AverageRating, reviewSummary2.RatingCount);
      string str = "";
      if (extension.InstallationTargets != null)
        str = GalleryUtil.GetProductTypeForInstallationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets);
      this.CheckAndInvalidateHomePageCache(reviewSummary1.AverageRating, reviewSummary2.AverageRating, "DeleteReview: " + str + " " + extension.Publisher.PublisherName + "." + extension.ExtensionName);
      this.TfsRequestContext.GetService<IGalleryAuditLogService>().LogAuditEntry(this.TfsRequestContext, nameof (DeleteReview), reviewId.ToString(), "Review");
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }

    private void CheckAndInvalidateHomePageCache(
      float prevAverageRating,
      float newAverageRating,
      string eventData)
    {
      float num = prevAverageRating - newAverageRating;
      if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.HomepageCacheRefreshControl") || (double) num < 1.0 && (double) num > -1.0)
        return;
      this.InvalidateHomePageCache(eventData);
    }

    private void InvalidateHomePageCache(string eventData) => GalleryServerUtil.NotifyGalleryDataChanged(this.TfsRequestContext, GalleryNotificationEventIds.ExtensionUpdateDelete, eventData);

    private PublishedExtension CheckIfExtensionExists(
      string publisherName,
      string extensionName,
      string version = null,
      bool forDeletion = false)
    {
      IPublishedExtensionService service = this.TfsRequestContext.GetService<IPublishedExtensionService>();
      ExtensionQueryFlags flags = ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeLatestVersionOnly;
      bool useCache = this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.QueryExtensionCacheExtended");
      PublishedExtension extension;
      try
      {
        extension = service.QueryExtension(this.TfsRequestContext, publisherName, extensionName, version, flags, (string) null, useCache);
      }
      catch (AccessCheckException ex)
      {
        Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = new RatingAndReviewUtils().GetAuthenticatedIdentity(this.TfsRequestContext);
        if (authenticatedIdentity == null)
        {
          throw;
        }
        else
        {
          extension = service.QueryExtension(this.TfsRequestContext.Elevate(), publisherName, extensionName, version, flags | ExtensionQueryFlags.IncludeSharedAccounts | ExtensionQueryFlags.IncludeSharedOrganizations, (string) null);
          if (!forDeletion)
          {
            if (!GalleryServerUtil.IsSharedWithUser(this.TfsRequestContext, extension, authenticatedIdentity.Id))
              throw;
          }
        }
      }
      return extension;
    }

    private void ValidateReviewVersion(PublishedExtension extension, Review review)
    {
      if (review != null && extension != null && extension.Versions != null && review.ProductVersion != null && !review.ProductVersion.Equals(extension.Versions[0].Version, StringComparison.OrdinalIgnoreCase))
        throw new VersionMismatchException(GalleryWebApiResources.ReviewProductVersionMismatch((object) review.ProductVersion, (object) extension.Versions[0].Version));
    }

    private void ValidateReviewVersionPatch(PublishedExtension extension, ReviewPatch reviewPatch)
    {
      if (reviewPatch.Operation == ReviewPatchOperation.FlagReview)
        return;
      this.ValidateReviewVersion(extension, reviewPatch.ReviewItem);
    }
  }
}
