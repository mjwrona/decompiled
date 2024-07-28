// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PurchaseEventsManipulation
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class PurchaseEventsManipulation : BillingEventsManipulationBase
  {
    public override void DenyEventsForBilling(
      BillableEvent billableEvent,
      bool additionalMetersToSkip = false)
    {
      if (billableEvent == null || !this.ShouldSkipBilling(billableEvent.MeterPlatformGuid, additionalMetersToSkip))
        return;
      billableEvent.MeterPlatformGuid = Guid.Empty;
    }

    private bool ShouldSkipBilling(Guid meterGuid, bool additionalMetersToSkip)
    {
      if (DeniedBillingMeters.DeniedMeterGuidsForPurchase.Contains(meterGuid))
        return true;
      return additionalMetersToSkip && DeniedBillingMeters.AdditionalMeterGuidsForPurchase.Contains(meterGuid);
    }

    public override bool EnableSkipBilling(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled(this.FeatureFlag);

    public override bool EnableAdditionalMetersSkipBilling(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled(this.AdditionalMetersFeatureFlag);

    private string FeatureFlag => "Microsoft.Azure.DevOps.Commerce.DisableBillingForDeniedMeters";

    private string AdditionalMetersFeatureFlag => "Microsoft.Azure.DevOps.Commerce.DisableBillingForAdditionalDeniedMetersV2";
  }
}
