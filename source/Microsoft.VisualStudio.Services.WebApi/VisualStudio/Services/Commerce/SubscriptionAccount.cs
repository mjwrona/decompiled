// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.SubscriptionAccount
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class SubscriptionAccount : ISubscriptionAccount
  {
    public Guid AccountId { get; set; }

    public int AccountHostType { get; set; }

    public string AccountName { get; set; }

    public Guid? SubscriptionId { get; set; }

    public SubscriptionStatus SubscriptionStatus { get; set; }

    public string ResourceGroupName { get; set; }

    public string GeoLocation { get; set; }

    public string Locale { get; set; }

    public string RegionDisplayName { get; set; }

    public IDictionary<Guid, Uri> ServiceUrls { get; set; }

    public Guid AccountTenantId { get; set; }

    public bool IsAccountOwner { get; set; }

    public string ResourceName { get; set; }

    public string SubscriptionName { get; set; }

    public bool IsEligibleForPurchase { get; set; }

    public bool IsPrepaidFundSubscription { get; set; }

    public bool IsPricingAvailable { get; set; }

    public string SubscriptionOfferCode { get; set; }

    public AzureOfferType? OfferType { get; set; }

    public Guid? SubscriptionTenantId { get; set; }

    public Guid? SubscriptionObjectId { get; set; }

    public PurchaseErrorReason FailedPurchaseReason { get; set; }
  }
}
