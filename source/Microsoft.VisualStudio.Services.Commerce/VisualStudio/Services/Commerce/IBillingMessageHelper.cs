// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IBillingMessageHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal interface IBillingMessageHelper
  {
    void CreateOfferSubscriptionUpgradeMessage(
      IVssRequestContext requestContext,
      Guid userId,
      OfferSubscriptionInternal meteredResource,
      Guid subscriptionId,
      Guid accountId);

    void CreateOfferSubscriptionDowngradeMessage(
      IVssRequestContext requestContext,
      IEnumerable<DowngradedResource> downgradedResources,
      Guid subscriptionId,
      Guid accountId);

    void SendMessageForOfferSubscriptionChanges(
      IVssRequestContext requestContext,
      OfferSubscriptionQuantityChangeMessage[] offerSubscriptionQuantityChangeMessages);

    void SendTrialStartMessage(
      IVssRequestContext requestContext,
      OfferSubscriptionInternal offerSubscription);

    void SendTrialEndMessage(
      IVssRequestContext requestContext,
      OfferSubscriptionInternal offerSubscription);

    void SendTrialExtendedMessage(
      IVssRequestContext requestContext,
      OfferSubscriptionInternal offerSubscription);
  }
}
