// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.CustomerSupportRequestController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.ContentSecurityPolicy;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Gallery.Server;
using Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Models;
using Microsoft.VisualStudio.Services.Gallery.Web.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  [SupportedRouteArea("", NavigationContextLevels.All)]
  [LogUserInfoFilter]
  [SetUserTokenFilter]
  [IncludeCspHeader]
  [SetXFrameOptions]
  public class CustomerSupportRequestController : GalleryController
  {
    private const string RegistryPathForCaptchaPublicKey = "/Configuration/Service/Gallery/CaptchaPublicKey";

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Support(string extensionName, string publisherName)
    {
      bool ensureSharedAccounts = this.TfsRequestContext.UserContext != (IdentityDescriptor) null;
      PublishedExtensionResult publishedExtensionResult = new PublishedExtensionResult();
      this.ControllerHelper.PopulateGeneralInfo();
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      Uri uri = new Uri(GalleryHtmlExtensions.GetGalleryUri(this.TfsRequestContext, AccessMappingConstants.ClientAccessMappingMoniker), "/items?itemName=" + fullyQualifiedName);
      PublishedExtensionResult publishedExtension;
      try
      {
        publishedExtension = this.DetailsHelper.GetPublishedExtension(fullyQualifiedName, ensureSharedAccounts);
      }
      catch (ArgumentException ex)
      {
        throw new HttpException(404, ex.Message);
      }
      if (publishedExtension.Extension == null)
        throw new HttpException(404, GalleryResources.ErrorItemDoesNotExist);
      this.ViewData["requestData"] = (object) new CustomerSupportRequest()
      {
        ExtensionName = extensionName,
        PublisherName = publisherName,
        SourceLink = "extensionDetailsPage",
        ExtensionURL = uri,
        DisplayName = publishedExtension.Extension.DisplayName
      };
      this.ViewData["ReCaptchaPublicKey"] = (object) this.getReCaptchaPublicKey();
      return (ActionResult) this.View("CustomerSupportRequest");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult FooterSupport()
    {
      this.ControllerHelper.PopulateGeneralInfo();
      this.ViewData["requestData"] = (object) new CustomerSupportRequest()
      {
        SourceLink = "footer"
      };
      this.ViewData["ReCaptchaPublicKey"] = (object) this.getReCaptchaPublicKey();
      return (ActionResult) this.View("CustomerSupportRequest");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult AppealReview(string publisherName, string extensionName, string reviewerId)
    {
      bool ensureSharedAccounts = this.TfsRequestContext.UserContext != (IdentityDescriptor) null;
      if (!ensureSharedAccounts)
        return (ActionResult) this.Redirect(this.ControllerHelper.GetSignInRedirectUrl());
      this.ControllerHelper.PopulateGeneralInfo();
      PublishedExtension extension = new PublishedExtension();
      PublisherFacts publisherFacts = new PublisherFacts();
      publisherFacts.PublisherName = publisherName;
      extension.ExtensionName = extensionName;
      extension.Publisher = publisherFacts;
      Guid userId = new Guid(reviewerId);
      List<Review> reviewsByUserId = this.TfsRequestContext.GetService<IRatingAndReviewService>().GetReviewsByUserId(this.TfsRequestContext, userId, extension);
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      PublishedExtensionResult publishedExtensionResult = new PublishedExtensionResult();
      PublishedExtensionResult publishedExtension;
      try
      {
        publishedExtension = this.DetailsHelper.GetPublishedExtension(fullyQualifiedName, ensureSharedAccounts);
      }
      catch (ArgumentException ex)
      {
        throw new HttpException(404, ex.Message);
      }
      if (publishedExtension.Extension == null)
        throw new HttpException(404, GalleryResources.ErrorItemDoesNotExist);
      try
      {
        GallerySecurity.CheckExtensionPermission(this.TfsRequestContext, publishedExtension.Extension, (string) null, PublisherPermissions.UpdateExtension, false);
      }
      catch (Exception ex)
      {
        throw new HttpException(404, ex.Message);
      }
      if (!reviewsByUserId.Any<Review>())
        throw new HttpException(404, GalleryResources.ReviewNotFound);
      Uri uri = new Uri(GalleryHtmlExtensions.GetGalleryUri(this.TfsRequestContext, AccessMappingConstants.ClientAccessMappingMoniker), "/items?itemName=" + fullyQualifiedName);
      Review review = new Review()
      {
        Id = reviewsByUserId[0].Id,
        Rating = reviewsByUserId[0].Rating,
        Text = reviewsByUserId[0].Text,
        UpdatedDate = reviewsByUserId[0].UpdatedDate,
        UserId = reviewsByUserId[0].UserId
      };
      this.ViewData["requestData"] = (object) new CustomerSupportRequest()
      {
        ExtensionName = extensionName,
        PublisherName = publisherName,
        SourceLink = "appealReview",
        Review = review,
        ExtensionURL = uri,
        DisplayName = publishedExtension.Extension.DisplayName
      };
      this.ViewData["ReCaptchaPublicKey"] = (object) this.getReCaptchaPublicKey();
      return (ActionResult) this.View("CustomerSupportRequest");
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult PublisherSupport(string publisherName)
    {
      if (!(this.TfsRequestContext.UserContext != (IdentityDescriptor) null))
        return (ActionResult) this.Redirect(this.ControllerHelper.GetSignInRedirectUrl());
      this.ControllerHelper.PopulateGeneralInfo();
      this.ViewData["requestData"] = (object) new CustomerSupportRequest()
      {
        SourceLink = "publisherManagementPage",
        PublisherName = publisherName
      };
      this.ViewData["ReCaptchaPublicKey"] = (object) this.getReCaptchaPublicKey();
      return (ActionResult) this.View("CustomerSupportRequest");
    }

    public string getReCaptchaPublicKey() => this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<string>(this.TfsRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/CaptchaPublicKey", string.Empty);
  }
}
