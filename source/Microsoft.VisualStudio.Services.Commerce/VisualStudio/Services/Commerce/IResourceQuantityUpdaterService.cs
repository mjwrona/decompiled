// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IResourceQuantityUpdaterService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DefaultServiceImplementation(typeof (ResourceQuantityUpdaterService))]
  internal interface IResourceQuantityUpdaterService : IVssFrameworkService
  {
    void SetResetInternalAccountResourceQuantities(
      IVssRequestContext requestContext,
      OfferSubscriptionInternal meterUsage);

    void GetInternalAccountResourceQuantities(
      IVssRequestContext requestContext,
      OfferSubscriptionInternal meterUsage);

    void UpdateOfferSubscription(
      IVssRequestContext requestContext,
      int? maximumQuantity,
      int? includedQuantity,
      bool? isPaidBillingEnabled,
      OfferMeter meterConfig);

    void UpdateOfferSubscription(
      IVssRequestContext requestContext,
      int? maximumQuantity,
      int? includedQuantity,
      bool? isPaidBillingEnabled,
      OfferMeter meterConfig,
      ResourceRenewalGroup renewalGroup);
  }
}
