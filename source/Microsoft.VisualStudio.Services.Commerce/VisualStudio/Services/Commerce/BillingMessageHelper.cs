// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.BillingMessageHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class BillingMessageHelper : IBillingMessageHelper
  {
    private static readonly HashSet<string> ExtensionsRequiringEmailAddress = new HashSet<string>()
    {
      "ms.xamarin-university",
      "ms.hockeyapp-business-monthly-small",
      "ms.hockeyapp-business-monthly-medium",
      "ms.hockeyapp-business-monthly-large",
      "ms.hockeyapp-business-monthly-xlarge",
      "ms.hockeyapp-business-monthly-2xlarge"
    };
    private const string Area = "Commerce";
    private const string Layer = "BillingMessageHelper";

    public void CreateOfferSubscriptionUpgradeMessage(
      IVssRequestContext collectionContext,
      Guid userId,
      OfferSubscriptionInternal meteredResource,
      Guid subscriptionId,
      Guid collectionId)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      if (meteredResource == null)
        return;
      string property = Guid.Empty.ToString();
      string purchaserAccountName = string.Empty;
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment).Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      IVssRequestContext requestContext = vssRequestContext;
      List<Guid> identityIds = new List<Guid>();
      identityIds.Add(userId);
      string[] propertyNameFilters = new string[0];
      Microsoft.VisualStudio.Services.Identity.Identity requestIdentity = service.ReadIdentities(requestContext, (IList<Guid>) identityIds, QueryMembership.None, (IEnumerable<string>) propertyNameFilters).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (requestIdentity != null)
      {
        property = requestIdentity.GetProperty<string>("Domain", Guid.Empty.ToString());
        purchaserAccountName = this.ResolvePurchaserAccountName(vssRequestContext, requestIdentity, meteredResource);
      }
      OfferSubscriptionQuantityChangeMessage changeMessageContent = this.CreateOfferSubscriptionChangeMessageContent(collectionContext, OfferSubscriptionQuantityChangeEventType.Upgrade, meteredResource.CommittedQuantity, (IOfferMeter) meteredResource.Meter.ToOfferMeter(), meteredResource.RenewalGroup, subscriptionId, collectionId, property, purchaserAccountName);
      this.SendMessageForOfferSubscriptionChanges(collectionContext, new OfferSubscriptionQuantityChangeMessage[1]
      {
        changeMessageContent
      });
    }

    public void CreateOfferSubscriptionDowngradeMessage(
      IVssRequestContext collectionContext,
      IEnumerable<DowngradedResource> downgradedResources,
      Guid subscriptionId,
      Guid accountId)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      IEnumerable<IOfferMeter> offerMeters = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeters(vssRequestContext);
      if (downgradedResources.IsNullOrEmpty<DowngradedResource>())
        return;
      OfferSubscriptionQuantityChangeMessage[] array = downgradedResources.Join<DowngradedResource, IOfferMeter, int, OfferSubscriptionQuantityChangeMessage>(offerMeters, (Func<DowngradedResource, int>) (definition => definition.MeterId), (Func<IOfferMeter, int>) (downgradedResource => downgradedResource.MeterId), (Func<DowngradedResource, IOfferMeter, OfferSubscriptionQuantityChangeMessage>) ((downgradedResource, meter) => this.CreateOfferSubscriptionChangeMessageContent(collectionContext, OfferSubscriptionQuantityChangeEventType.Downgrade, downgradedResource.NewQuantity, meter, downgradedResource.RenewalGroup, subscriptionId, accountId, string.Empty, string.Empty))).ToArray<OfferSubscriptionQuantityChangeMessage>();
      this.SendMessageForOfferSubscriptionChanges(collectionContext, array);
    }

    public void SendTrialStartMessage(
      IVssRequestContext requestContext,
      OfferSubscriptionInternal offerSubscription)
    {
      OfferSubscriptionTrialStatusChangeMessage statusChangeMessage = BillingMessageHelper.SendTrialMessage(requestContext, offerSubscription, OfferSubscriptionTrialStatusChangeType.TrialStarted);
      requestContext.Trace(5108862, TraceLevel.Verbose, "Commerce", nameof (BillingMessageHelper), string.Format("OrgId:{0}, MeterName:{1}", (object) statusChangeMessage.TargetHostId, (object) statusChangeMessage.GalleryId));
    }

    public void SendTrialEndMessage(
      IVssRequestContext requestContext,
      OfferSubscriptionInternal offerSubscription)
    {
      OfferSubscriptionTrialStatusChangeMessage statusChangeMessage = BillingMessageHelper.SendTrialMessage(requestContext, offerSubscription, OfferSubscriptionTrialStatusChangeType.TrialEnded);
      requestContext.Trace(5108922, TraceLevel.Verbose, "Commerce", nameof (BillingMessageHelper), string.Format("OrgId:{0}, MeterName:{1}", (object) statusChangeMessage.TargetHostId, (object) statusChangeMessage.GalleryId));
    }

    public void SendTrialExtendedMessage(
      IVssRequestContext requestContext,
      OfferSubscriptionInternal offerSubscription)
    {
      OfferSubscriptionTrialStatusChangeMessage statusChangeMessage = BillingMessageHelper.SendTrialMessage(requestContext, offerSubscription, OfferSubscriptionTrialStatusChangeType.TrialExtended);
      requestContext.Trace(5108923, TraceLevel.Verbose, "Commerce", nameof (BillingMessageHelper), string.Format("OrgId:{0}, MeterName:{1}", (object) statusChangeMessage.TargetHostId, (object) statusChangeMessage.GalleryId));
    }

    private static OfferSubscriptionTrialStatusChangeMessage SendTrialMessage(
      IVssRequestContext requestContext,
      OfferSubscriptionInternal offerSubscription,
      OfferSubscriptionTrialStatusChangeType messageType)
    {
      ArgumentUtility.CheckForNull<OfferSubscriptionInternal>(offerSubscription, nameof (offerSubscription));
      ArgumentUtility.CheckForNull<DateTime>(offerSubscription.StartDate, "StartDate");
      ArgumentUtility.CheckForNull<DateTime>(offerSubscription.TrialExpiryDate, "TrialExpiryDate");
      ArgumentUtility.CheckStringForNullOrEmpty(offerSubscription.Meter.GalleryId, "GalleryId");
      OfferSubscriptionTrialStatusChangeMessage statusChangeMessage = new OfferSubscriptionTrialStatusChangeMessage()
      {
        TargetHostId = requestContext.ServiceHost.InstanceId,
        TrialChangeType = messageType,
        GalleryId = offerSubscription.Meter.GalleryId,
        TrialStartDate = offerSubscription.StartDate.Value,
        TrialExpiryDate = offerSubscription.TrialExpiryDate.Value
      };
      if (requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.DisableOfferSubscriptionTrialStatusChangeNotification") || CommerceUtil.IsRunningOnCommerceServiceAsBackup(requestContext))
        return statusChangeMessage;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      try
      {
        vssRequestContext.GetService<IMessageBusPublisherService>().TryPublish(vssRequestContext, "Microsoft.TeamFoundation.Services.OfferSubscriptionTrialStatusChange", (object[]) new OfferSubscriptionTrialStatusChangeMessage[1]
        {
          statusChangeMessage
        }, false);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108951, "Commerce", nameof (BillingMessageHelper), ex);
      }
      return statusChangeMessage;
    }

    private OfferSubscriptionQuantityChangeMessage CreateOfferSubscriptionChangeMessageContent(
      IVssRequestContext requestContext,
      OfferSubscriptionQuantityChangeEventType eventType,
      int newQuantity,
      IOfferMeter meter,
      ResourceRenewalGroup renewalGroup,
      Guid subscriptionId,
      Guid accountId,
      string domain,
      string purchaserAccountName)
    {
      return new OfferSubscriptionQuantityChangeMessage()
      {
        ActivityId = requestContext.ActivityId,
        SubscriptionStatus = SubscriptionStatus.Active,
        EventType = eventType,
        OfferMeterName = meter.Name,
        PurchaserAccountName = purchaserAccountName,
        TenantId = domain,
        Quantity = newQuantity,
        SubscriptionId = subscriptionId,
        VsoAccountId = accountId,
        OfferMeterCategory = meter.Category,
        MessageCreateTime = DateTime.UtcNow,
        MeterGalleryId = meter.GalleryId,
        RenewalGroup = renewalGroup
      };
    }

    public void SendMessageForOfferSubscriptionChanges(
      IVssRequestContext requestContext,
      OfferSubscriptionQuantityChangeMessage[] offerSubscriptionQuantityChangeMessages)
    {
      if (requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.DisableOfferSubscriptionChangeNotification") || CommerceUtil.IsRunningOnCommerceServiceAsBackup(requestContext) || ((IEnumerable<OfferSubscriptionQuantityChangeMessage>) offerSubscriptionQuantityChangeMessages).IsNullOrEmpty<OfferSubscriptionQuantityChangeMessage>())
        return;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      try
      {
        vssRequestContext.GetService<IMessageBusPublisherService>().TryPublish(vssRequestContext, "Microsoft.TeamFoundation.Services.OfferSubscriptionChange", (object[]) offerSubscriptionQuantityChangeMessages);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108951, "Commerce", nameof (BillingMessageHelper), ex);
      }
    }

    private string ResolvePurchaserAccountName(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity requestIdentity,
      OfferSubscriptionInternal meteredResource)
    {
      ReadOnlyOfferMeter meter = meteredResource.Meter;
      return BillingMessageHelper.ExtensionsRequiringEmailAddress.Contains(meter.GalleryId) ? IdentityHelper.GetPreferredEmailAddress(requestContext, requestIdentity.Id) : requestIdentity.GetProperty<string>("Account", string.Empty);
    }
  }
}
