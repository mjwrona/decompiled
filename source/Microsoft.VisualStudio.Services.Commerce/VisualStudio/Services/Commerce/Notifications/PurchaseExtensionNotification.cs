// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Notifications.PurchaseExtensionNotification
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce.Notifications
{
  internal class PurchaseExtensionNotification : PurchaseNotification
  {
    private const string Layer = "PurchaseExtensionNotification";
    private const string ExtensionPurchaseConfirmationType = "confirmation";

    public virtual void SendPurchaseNotificationEmail(
      IVssRequestContext collectionContext,
      string offerMeterName,
      Guid azureSubscriptionId,
      int quantity,
      string offerCode,
      Guid? tenantId,
      Guid? objectId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (offerMeterName == null)
      {
        collectionContext.Trace(5109164, TraceLevel.Error, "Commerce", nameof (PurchaseExtensionNotification), string.Format("No notification is published since offerMeterName is null. Azure Subscription Id: {0}.", (object) azureSubscriptionId));
      }
      else
      {
        collectionContext.Trace(5109210, TraceLevel.Info, "Commerce", nameof (PurchaseExtensionNotification), string.Format("Azure Subscription Id: {0}. offerMeterName: {1}. Quantity: {2}.", (object) azureSubscriptionId, (object) offerMeterName, (object) quantity) + string.Format(" offerCode: {0} tenantId: {1}. objectId: {2}.", (object) offerCode, (object) tenantId, (object) objectId));
        OfferMeter offerMeter = this.GetOfferMeter(collectionContext, offerMeterName);
        string vsoAccountLink;
        string manageExtensionUsersLink;
        this.PopulateAccountAndManageExtensionUsersLinks(collectionContext, offerMeter, out vsoAccountLink, out manageExtensionUsersLink);
        ExtensionPurchaseEvent extensionPurchaseEvent = new ExtensionPurchaseEvent();
        extensionPurchaseEvent.NotificationType = "confirmation";
        extensionPurchaseEvent.ExtensionName = offerMeterName;
        extensionPurchaseEvent.Quantity = quantity.ToString();
        extensionPurchaseEvent.Price = this.GetPrice(collectionContext, offerMeterName, azureSubscriptionId, quantity, offerCode, tenantId, objectId);
        extensionPurchaseEvent.AccountName = CollectionHelper.GetCollectionName(collectionContext);
        extensionPurchaseEvent.SubscriptionName = this.GetSubscriptionName(collectionContext, azureSubscriptionId);
        extensionPurchaseEvent.AccountLink = vsoAccountLink;
        extensionPurchaseEvent.ManageExtensionUsersLink = manageExtensionUsersLink;
        extensionPurchaseEvent.BuyMoreLink = this.GetBuyMoreLink(collectionContext, offerMeter);
        ExtensionPurchaseEvent emailEvent = extensionPurchaseEvent;
        this.PublishNotificationEvent(collectionContext, identity, (object) emailEvent);
      }
    }

    private string GetBuyMoreLink(IVssRequestContext collectionContext, OfferMeter offerMeter)
    {
      IVssRequestContext requestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      string str = string.Format("install/{0}?accountId={1}", (object) offerMeter.GalleryId, (object) collectionContext.ServiceHost.InstanceId.ToString());
      return this.GetLocationServiceUrl(requestContext, CommerceConstants.GalleryGuid, "https://marketplace.visualstudio.com/") + str;
    }
  }
}
