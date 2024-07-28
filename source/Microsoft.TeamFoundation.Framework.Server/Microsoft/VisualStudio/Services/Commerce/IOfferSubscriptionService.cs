// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IOfferSubscriptionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DefaultServiceImplementation(typeof (FrameworkOfferSubscriptionService))]
  public interface IOfferSubscriptionService : IVssFrameworkService
  {
    IEnumerable<IOfferSubscription> GetOfferSubscriptions(
      IVssRequestContext requestContext,
      bool nextBillingPeriod = false);

    IEnumerable<IOfferSubscription> GetOfferSubscriptionsForAllAzureSubscription(
      IVssRequestContext requestContext,
      bool validateAzuresubscription = false,
      bool nextBillingPeriod = false);

    IEnumerable<IOfferSubscription> GetOfferSubscriptionsForGalleryItem(
      IVssRequestContext requestContext,
      Guid azureSubscriptionId,
      string galleryItemId,
      bool nextBillingPeriod = false);

    IOfferSubscription GetOfferSubscription(
      IVssRequestContext requestContext,
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      bool nextBillingPeriod = false);

    IOfferSubscription GetOfferSubscription(
      IVssRequestContext requestContext,
      string offerMeterName,
      bool nextBillingPeriod = false);

    void CreateOfferSubscription(
      IVssRequestContext requestContext,
      string offerMeterName,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      Guid? billingTarget);

    double ReportUsage(
      IVssRequestContext collectionContext,
      Guid eventUserId,
      string resourceName,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      string eventId,
      DateTime billableDate,
      bool immediate = false);

    IEnumerable<IUsageEventAggregate> GetUsage(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime,
      TimeSpan timeSpan,
      ResourceName resource);

    void CancelOfferSubscription(
      IVssRequestContext requestContext,
      string offerMeterName,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      string cancelReason,
      Guid? billingTarget,
      bool immediate = false,
      Guid? tenantId = null);

    void TogglePaidBilling(
      IVssRequestContext requestContext,
      string offerMeterName,
      bool paidBillingState);

    void SetAccountQuantity(
      IVssRequestContext requestContext,
      string offerMeterName,
      int? includedQuantity,
      int? maximumQuantity);

    AccountLicenseType GetDefaultLicenseLevel(
      IVssRequestContext requestContext,
      Guid organizationId);

    void SetAccountQuantity(
      IVssRequestContext requestContext,
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      int? includedQuantity,
      int? maximumQuantity);

    void DecreaseResourceQuantity(
      IVssRequestContext requestContext,
      Guid? azureSubscriptionId,
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      bool shouldBeImmediate);

    void EnableTrialOrPreview(
      IVssRequestContext requestContext,
      string offerMeterName,
      ResourceRenewalGroup renewalGroup);

    void ExtendTrialDate(
      IVssRequestContext requestContext,
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      DateTime endDate);

    void ManualBillingEvent(
      IVssRequestContext requestContext,
      string resourceName,
      ResourceRenewalGroup renewalGroup,
      double quantity);

    void ManualAzureStoreManagedBillingEvent(
      IVssRequestContext requestContext,
      string resourceName,
      int quantity,
      Guid? tenantId = null);

    void CreatePurchaseRequest(IVssRequestContext requestContext, PurchaseRequest purchaseRequest);

    void UpdatePurchaseRequest(IVssRequestContext requestContext, PurchaseRequest purchaseRequest);
  }
}
