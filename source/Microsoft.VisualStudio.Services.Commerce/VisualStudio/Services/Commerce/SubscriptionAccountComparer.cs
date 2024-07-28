// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.SubscriptionAccountComparer
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class SubscriptionAccountComparer : IEqualityComparer<ISubscriptionAccount>
  {
    public bool Equals(ISubscriptionAccount x, ISubscriptionAccount y)
    {
      if (x == null && y == null)
        return true;
      if (x == null || y == null || !(x.AccountId == y.AccountId) || x.AccountHostType != y.AccountHostType || !(x.AccountName == y.AccountName))
        return false;
      Guid? subscriptionId1 = x.SubscriptionId;
      Guid? subscriptionId2 = y.SubscriptionId;
      if ((subscriptionId1.HasValue == subscriptionId2.HasValue ? (subscriptionId1.HasValue ? (subscriptionId1.GetValueOrDefault() == subscriptionId2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && x.SubscriptionStatus == y.SubscriptionStatus && x.ResourceGroupName == y.ResourceGroupName && x.GeoLocation == y.GeoLocation && x.Locale == y.Locale && x.RegionDisplayName == y.RegionDisplayName && x.AccountTenantId == y.AccountTenantId && x.IsAccountOwner == y.IsAccountOwner && x.ResourceName == y.ResourceName && x.SubscriptionName == y.SubscriptionName && x.IsEligibleForPurchase == y.IsEligibleForPurchase && x.IsPrepaidFundSubscription == y.IsPrepaidFundSubscription && x.IsPricingAvailable == y.IsPricingAvailable && x.SubscriptionOfferCode == y.SubscriptionOfferCode)
      {
        AzureOfferType? offerType1 = x.OfferType;
        AzureOfferType? offerType2 = y.OfferType;
        if (offerType1.GetValueOrDefault() == offerType2.GetValueOrDefault() & offerType1.HasValue == offerType2.HasValue)
        {
          Guid? subscriptionTenantId1 = x.SubscriptionTenantId;
          Guid? subscriptionTenantId2 = y.SubscriptionTenantId;
          if ((subscriptionTenantId1.HasValue == subscriptionTenantId2.HasValue ? (subscriptionTenantId1.HasValue ? (subscriptionTenantId1.GetValueOrDefault() == subscriptionTenantId2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
          {
            Guid? subscriptionObjectId1 = x.SubscriptionObjectId;
            Guid? subscriptionObjectId2 = y.SubscriptionObjectId;
            if ((subscriptionObjectId1.HasValue == subscriptionObjectId2.HasValue ? (subscriptionObjectId1.HasValue ? (subscriptionObjectId1.GetValueOrDefault() == subscriptionObjectId2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
              return x.FailedPurchaseReason == y.FailedPurchaseReason;
          }
        }
      }
      return false;
    }

    public int GetHashCode(ISubscriptionAccount x) => x.AccountId.GetHashCode() ^ x.SubscriptionId.GetHashCode();
  }
}
