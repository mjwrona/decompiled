// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Notifications.CommerceNotification
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce.Notifications
{
  public abstract class CommerceNotification
  {
    protected const string ViewProfileProdUrl = "https://app.vssps.visualstudio.com/profile/view/";
    protected const string MsdnProdUrl = "https://go.microsoft.com/fwlink/?linkid=842050";
    protected const string GalleryProdUrl = "https://marketplace.visualstudio.com/";
    protected const string NewsUrl = "https://www.visualstudio.com/news/news-overview-vs";
    protected const string PricingUrl = "https://www.visualstudio.com/products/visual-studio-team-services-pricing-vs";
    protected const string SupportUrl = "https://www.visualstudio.com/support/support-overview-vs";
    protected const string MarketPlaceItemUrl = "https://marketplace.visualstudio.com/items?itemName={0}";
    protected const string ManageExtensionUsersPath = "_user/index?id={0}";
    protected const string BuyMorePath = "install/{0}?accountId={1}";
    protected const string Area = "Commerce";
    private const string Layer = "CommerceNotification";

    protected void PublishNotificationEvent(
      IVssRequestContext collectionContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      object emailEvent)
    {
      if (identity == null)
      {
        collectionContext.Trace(5109211, TraceLevel.Error, "Commerce", nameof (CommerceNotification), "No notification is published since identity is null..");
      }
      else
      {
        INotificationEventService service = collectionContext.GetService<INotificationEventService>();
        VssNotificationEvent notificationEvent = new VssNotificationEvent()
        {
          EventType = "ms.vss-commerce-web.commerce-notification-event",
          Data = emailEvent
        };
        notificationEvent.AddActor("recipient", identity.Id);
        Guid property = identity.GetProperty<Guid>("CUID", identity.Id);
        collectionContext.Trace(5109163, TraceLevel.Info, "Commerce", nameof (CommerceNotification), string.Format("Notification event data: {0}. Recipient identity: {1}", emailEvent, (object) property));
        IVssRequestContext requestContext = collectionContext;
        VssNotificationEvent theEvent = notificationEvent;
        service.PublishSystemEvent(requestContext, theEvent);
      }
    }

    protected void PopulateAccountAndManageExtensionUsersLinks(
      IVssRequestContext collectionContext,
      OfferMeter offerMeter,
      out string vsoAccountLink,
      out string manageExtensionUsersLink)
    {
      string locationServiceUrl = this.GetLocationServiceUrl(collectionContext, ServiceInstanceTypes.TFS, "https://app.vssps.visualstudio.com/profile/view/");
      vsoAccountLink = locationServiceUrl;
      manageExtensionUsersLink = string.Equals(locationServiceUrl, "https://app.vssps.visualstudio.com/profile/view/") ? locationServiceUrl : locationServiceUrl + string.Format("_user/index?id={0}", (object) offerMeter.GalleryId);
    }

    protected string GetLocationServiceUrl(
      IVssRequestContext requestContext,
      Guid serviceAreaIdentifier,
      string defaultUrl)
    {
      string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, serviceAreaIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
      return string.IsNullOrWhiteSpace(locationServiceUrl) ? defaultUrl : new Uri(locationServiceUrl).ToString();
    }

    protected OfferMeter GetOfferMeter(IVssRequestContext collectionContext, string offerMeterName)
    {
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      return (OfferMeter) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, offerMeterName);
    }
  }
}
