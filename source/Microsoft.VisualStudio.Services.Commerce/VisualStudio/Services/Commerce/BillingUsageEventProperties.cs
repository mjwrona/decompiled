// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.BillingUsageEventProperties
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class BillingUsageEventProperties
  {
    public string subscriptionId { get; set; }

    public string usageStartTime { get; set; }

    public string usageEndTime { get; set; }

    public string meterName { get; set; }

    public string meterCategory { get; set; }

    public string meterSubCategory { get; set; }

    public string unit { get; set; }

    public string meterId { get; set; }

    public InfoFields infoFields { get; set; }

    public double quantity { get; set; }
  }
}
