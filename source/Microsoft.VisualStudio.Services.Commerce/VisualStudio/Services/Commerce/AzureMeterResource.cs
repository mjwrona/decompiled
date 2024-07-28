// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureMeterResource
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureMeterResource
  {
    public string MeterId { get; set; }

    public string MeterName { get; set; }

    public string MeterCategory { get; set; }

    public string MeterSubCategory { get; set; }

    public string Unit { get; set; }

    public Dictionary<double, double> MeterRates { get; set; }

    public string EffectiveDate { get; set; }

    public List<string> MeterTags { get; set; }

    public string MeterRegion { get; set; }

    public double IncludedQuantity { get; set; }
  }
}
