// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IOfferSubscription
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public interface IOfferSubscription
  {
    OfferMeter OfferMeter { get; }

    ResourceRenewalGroup RenewalGroup { get; }

    int CommittedQuantity { get; }

    Uri DisabledResourceActionLink { get; }

    ResourceStatusReason DisabledReason { get; }

    int IncludedQuantity { get; }

    bool IsUseable { get; }

    bool IsPaidBillingEnabled { get; }

    int MaximumQuantity { get; }

    DateTime ResetDate { get; }

    Guid AzureSubscriptionId { get; set; }

    string AzureSubscriptionName { get; set; }

    SubscriptionStatus AzureSubscriptionState { get; set; }

    bool IsTrialOrPreview { get; set; }

    bool IsPreview { get; set; }

    bool IsPurchaseCanceled { get; set; }

    bool IsPurchasedDuringTrial { get; set; }

    DateTime? TrialExpiryDate { get; set; }

    DateTime? StartDate { get; set; }

    bool AutoAssignOnAccess { get; set; }
  }
}
