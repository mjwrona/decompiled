// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ISubscriptionAccount
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public interface ISubscriptionAccount
  {
    Guid AccountId { get; set; }

    int AccountHostType { get; set; }

    string AccountName { get; set; }

    Guid? SubscriptionId { get; set; }

    SubscriptionStatus SubscriptionStatus { get; set; }

    string ResourceGroupName { get; set; }

    string GeoLocation { get; set; }

    string Locale { get; set; }

    string RegionDisplayName { get; set; }

    IDictionary<Guid, Uri> ServiceUrls { get; set; }

    Guid AccountTenantId { get; set; }

    bool IsAccountOwner { get; set; }

    string ResourceName { get; set; }

    string SubscriptionName { get; set; }

    bool IsEligibleForPurchase { get; set; }

    bool IsPrepaidFundSubscription { get; set; }

    bool IsPricingAvailable { get; set; }

    string SubscriptionOfferCode { get; set; }

    AzureOfferType? OfferType { get; set; }

    Guid? SubscriptionTenantId { get; set; }

    Guid? SubscriptionObjectId { get; set; }

    PurchaseErrorReason FailedPurchaseReason { get; set; }
  }
}
