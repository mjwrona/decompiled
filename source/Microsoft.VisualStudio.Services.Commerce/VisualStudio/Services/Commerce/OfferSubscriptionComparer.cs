// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferSubscriptionComparer
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class OfferSubscriptionComparer : IEqualityComparer<IOfferSubscription>
  {
    public bool Equals(IOfferSubscription x, IOfferSubscription y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && x.OfferMeter == y.OfferMeter && x.RenewalGroup == y.RenewalGroup && x.CommittedQuantity == y.CommittedQuantity && x.DisabledResourceActionLink == y.DisabledResourceActionLink && x.DisabledReason == y.DisabledReason && x.IncludedQuantity == y.IncludedQuantity && x.IsUseable == y.IsUseable && x.IsPaidBillingEnabled == y.IsPaidBillingEnabled && x.MaximumQuantity == y.MaximumQuantity && x.AzureSubscriptionId == y.AzureSubscriptionId && x.AzureSubscriptionName == y.AzureSubscriptionName && x.AzureSubscriptionState == y.AzureSubscriptionState && x.IsTrialOrPreview == y.IsTrialOrPreview && x.IsPreview == y.IsPreview && x.IsPurchaseCanceled == y.IsPurchaseCanceled && x.IsPurchasedDuringTrial == y.IsPurchasedDuringTrial && x.AutoAssignOnAccess == y.AutoAssignOnAccess;
    }

    public int GetHashCode(IOfferSubscription x) => x.OfferMeter.MeterId.GetHashCode() ^ x.AzureSubscriptionId.GetHashCode();
  }
}
