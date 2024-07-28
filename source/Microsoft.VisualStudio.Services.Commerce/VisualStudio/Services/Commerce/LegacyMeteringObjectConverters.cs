// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.LegacyMeteringObjectConverters
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class LegacyMeteringObjectConverters
  {
    internal static ISubscriptionResource ToSubscriptionResource(
      this IOfferSubscription billingResource)
    {
      ResourceName result;
      if (!Enum.TryParse<ResourceName>(billingResource.OfferMeter.Name, out result))
        return (ISubscriptionResource) null;
      return (ISubscriptionResource) new SubscriptionResource()
      {
        Name = result,
        CommittedQuantity = billingResource.CommittedQuantity,
        DisabledResourceActionLink = billingResource.DisabledResourceActionLink,
        DisabledReason = billingResource.DisabledReason,
        IncludedQuantity = billingResource.IncludedQuantity,
        IsUseable = billingResource.IsUseable,
        IsPaidBillingEnabled = billingResource.IsPaidBillingEnabled,
        MaximumQuantity = billingResource.MaximumQuantity,
        ResetDate = billingResource.ResetDate
      };
    }

    internal static MeteredResource ToMeteredResource(this OfferMeter meter)
    {
      ResourceName result;
      if (!Enum.TryParse<ResourceName>(meter.Name, out result))
        return (MeteredResource) null;
      return new MeteredResource()
      {
        ResourceName = result,
        CommercePlatformMeterId = meter.PlatformMeterId,
        IncludedQuantity = meter.IncludedQuantity,
        CurrentQuantity = meter.CurrentQuantity,
        CommittedQuantity = meter.CommittedQuantity,
        MaximumQuantity = meter.MaximumQuantity,
        AbsoluteMaximumQuantity = meter.AbsoluteMaximumQuantity,
        Unit = meter.Unit
      };
    }
  }
}
