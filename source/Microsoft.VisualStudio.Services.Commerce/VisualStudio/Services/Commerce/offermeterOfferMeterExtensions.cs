// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.offermeterOfferMeterExtensions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class offermeterOfferMeterExtensions
  {
    public static char GetBillingCode(this ResourceBillingMode billingMode)
    {
      if (billingMode == ResourceBillingMode.Committment)
        return 'C';
      if (billingMode == ResourceBillingMode.PayAsYouGo)
        return 'P';
      throw new ArgumentException("Invalid Value for the billing mode", nameof (billingMode));
    }

    public static OfferSubscriptionInternal ToDefaultMeterUsage(this IOfferMeter offermeter)
    {
      bool flag = offermeter.Category == MeterCategory.Legacy && offermeter.BillingMode == ResourceBillingMode.Committment;
      OfferSubscriptionInternal defaultMeterUsage = new OfferSubscriptionInternal();
      defaultMeterUsage.MeterId = offermeter.MeterId;
      defaultMeterUsage.CurrentQuantity = offermeter.CurrentQuantity;
      defaultMeterUsage.CommittedQuantity = offermeter.CommittedQuantity;
      defaultMeterUsage.IncludedQuantity = offermeter.IncludedQuantity;
      defaultMeterUsage.MaximumQuantity = offermeter.MaximumQuantity;
      defaultMeterUsage.Meter = offermeter.ToReadOnlyMeter();
      defaultMeterUsage.IsPaidBillingEnabled = flag;
      defaultMeterUsage.IsTrialOrPreview = false;
      defaultMeterUsage.StartDate = new DateTime?();
      defaultMeterUsage.IsDefaultEmptyEntry = true;
      defaultMeterUsage.BillingEntity = offermeter.BillingEntity;
      defaultMeterUsage.SetAutoAssignOnAccess(new bool?(offermeter.AutoAssignOnAccess));
      return defaultMeterUsage;
    }

    public static ReadOnlyOfferMeter ToReadOnlyMeter(this IOfferMeter offermeter) => new ReadOnlyOfferMeter(offermeter);
  }
}
