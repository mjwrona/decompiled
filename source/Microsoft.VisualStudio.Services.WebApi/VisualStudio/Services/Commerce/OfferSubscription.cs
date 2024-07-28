// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferSubscription
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DebuggerDisplay("{OfferMeter?.Name} | {IncludedQuantity} | {CommittedQuantity}")]
  public class OfferSubscription : IOfferSubscription
  {
    public OfferMeter OfferMeter { get; set; }

    public ResourceRenewalGroup RenewalGroup { get; set; }

    public int CommittedQuantity { get; set; }

    public Uri DisabledResourceActionLink { get; set; }

    public ResourceStatusReason DisabledReason { get; set; }

    public int IncludedQuantity { get; set; }

    public bool IsUseable { get; set; }

    public bool IsPaidBillingEnabled { get; set; }

    public int MaximumQuantity { get; set; }

    public DateTime ResetDate { get; set; }

    public Guid AzureSubscriptionId { get; set; }

    public Guid OfferSubscriptionId { get; set; }

    public string AzureSubscriptionName { get; set; }

    public SubscriptionStatus AzureSubscriptionState { get; set; }

    public bool IsTrialOrPreview { get; set; }

    public bool IsPreview { get; set; }

    public bool IsPurchaseCanceled { get; set; }

    public bool IsPurchasedDuringTrial { get; set; }

    public DateTime? TrialExpiryDate { get; set; }

    public DateTime? StartDate { get; set; }

    public bool AutoAssignOnAccess { get; set; }
  }
}
