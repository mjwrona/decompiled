// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.Models.EmailFormatters.CommerceEmailBaseFormatter
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.EmailNotification;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Commerce.Common.Models.EmailFormatters
{
  public abstract class CommerceEmailBaseFormatter : BaseNotificationEmailData
  {
    protected const string viewProfileProdUrl = "https://app.vssps.visualstudio.com/profile/view/";
    protected const string msdnProdUrl = "https://go.microsoft.com/fwlink/?linkid=842050";
    protected const string galleryProdUrl = "https://marketplace.visualstudio.com/";
    protected const string newsUrl = "https://www.visualstudio.com/news/news-overview-vs";
    protected const string pricingUrl = "https://www.visualstudio.com/products/visual-studio-team-services-pricing-vs";
    protected const string supportUrl = "https://www.visualstudio.com/support/support-overview-vs";
    protected const string marketPlaceItemUrl = "https://marketplace.visualstudio.com/items?itemName={0}";
    protected const string manageExtensionUsersPath = "_user/index?id={0}";
    protected const string buyMorePath = "install/{0}?accountId={1}";

    protected void PopulateAccountAndManageExtensionUsersLinks(
      IVssRequestContext collectionContext,
      OfferMeter offerMeter)
    {
      string locationServiceUrl = this.GetLocationServiceUrl(collectionContext, ServiceInstanceTypes.TFS, "https://app.vssps.visualstudio.com/profile/view/");
      this.VSOAccountLink = locationServiceUrl;
      this.ManageExtensionUsersLink = string.Equals(locationServiceUrl, "https://app.vssps.visualstudio.com/profile/view/") ? locationServiceUrl : locationServiceUrl + string.Format("_user/index?id={0}", (object) offerMeter.GalleryId);
    }

    protected string GetLocationServiceUrl(
      IVssRequestContext requestContext,
      Guid serviceAreaIdentifier,
      string defaultUrl)
    {
      string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, serviceAreaIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
      return string.IsNullOrWhiteSpace(locationServiceUrl) ? defaultUrl : new Uri(locationServiceUrl).ToString();
    }

    protected string VSOAccountLink
    {
      set => this.Attributes[nameof (VSOAccountLink)] = value;
    }

    protected string ManageExtensionUsersLink
    {
      set => this.Attributes[nameof (ManageExtensionUsersLink)] = value;
    }
  }
}
