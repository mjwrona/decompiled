// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IOfferSubscriptionCachedAccessService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DefaultServiceImplementation(typeof (OfferSubscriptionCachedAccessService))]
  internal interface IOfferSubscriptionCachedAccessService : IVssFrameworkService
  {
    AggregateUsageEventResult AggregateUsageEvents(
      IVssRequestContext requestContext,
      OfferMeter meter,
      ResourceRenewalGroup renewalGroup,
      UsageEvent usageEvent,
      Guid lastUpdatedBy,
      DateTime executionDate);

    IEnumerable<OfferSubscriptionInternal> GetOfferSubscriptions(
      IVssRequestContext requestContext,
      int? meterId);

    IEnumerable<OfferSubscriptionInternal> GetOfferSubscriptions(
      IVssRequestContext requestContext,
      int? meterId,
      bool useCache);

    MeterResetEvents ResetResourceUsage(
      IVssRequestContext requestContext,
      bool monthlyReset,
      Guid subscriptionId,
      IEnumerable<KeyValuePair<int, int>> includedQuantities,
      IEnumerable<KeyValuePair<int, string>> billingModes,
      bool isResetOnlyCurrentQuantities,
      DateTime executionDate);

    void UpdateAccountQuantities(
      IVssRequestContext requestContext,
      int meterId,
      ResourceRenewalGroup renewalGroup,
      int? includedQuantity,
      int? maximumQuantity,
      int defaultIncludedQuantity,
      int defaultMaxQuantity,
      int absoluteMaxQuantity,
      ResourceBillingMode billingMode,
      Guid userIdentityId);

    void UpdateAccountQuantities(
      IVssRequestContext requestContext,
      int meterId,
      ResourceRenewalGroup renewalGroup,
      int? includedQuantity,
      int? maximumQuantity,
      int defaultIncludedQuantity,
      int defaultMaxQuantity,
      int absoluteMaxQuantity,
      ResourceBillingMode billingMode,
      Guid userIdentityId,
      bool resetUsage);

    int UpdateCommittedAndCurrentQuantities(
      IVssRequestContext requestContext,
      int meterId,
      byte resourceSeq,
      int committedQuantity,
      int currentQuantity,
      Guid userIdentityId);

    void UpdatePaidBillingMode(
      IVssRequestContext requestContext,
      int meterId,
      bool paidBillingEnabled,
      Guid userIdentityId);

    void CreateTrialOrPreview(
      IVssRequestContext requestContext,
      int meterId,
      ResourceRenewalGroup renewalGroup,
      Guid lastUpdatedBy,
      int includedQuantity = 0);

    void RemoveTrialForPaidOfferSubscription(
      IVssRequestContext requestContext,
      int meterId,
      ResourceRenewalGroup renewalGroup,
      int includedQuantity,
      Guid lastUpdatedBy);

    void Invalidate(IVssRequestContext requestContext);

    void ExtendTrialExpiryDate(
      IVssRequestContext requestContext,
      int meterId,
      ResourceRenewalGroup renewalGroup,
      Guid lastUpdatedBy,
      int trialDays);

    void ResetCloudLoadTestUsage(
      IVssRequestContext collectionContext,
      int meterId,
      ResourceRenewalGroup renewalGroup,
      Guid lastUpdateByUserGuid);
  }
}
