// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.BillingEventsManipulator
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class BillingEventsManipulator
  {
    public static BillingEventsManipulationBase CreateManipulator(BillingEventType eventType)
    {
      BillingEventsManipulationBase manipulator = (BillingEventsManipulationBase) null;
      switch (eventType)
      {
        case BillingEventType.MeterReset:
          manipulator = (BillingEventsManipulationBase) new MeterResetEventsManipulation();
          break;
        case BillingEventType.Purchase:
          manipulator = (BillingEventsManipulationBase) new PurchaseEventsManipulation();
          break;
      }
      return manipulator;
    }
  }
}
