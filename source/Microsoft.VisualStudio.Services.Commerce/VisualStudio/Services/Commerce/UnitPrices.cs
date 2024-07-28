// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.UnitPrices
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class UnitPrices
  {
    public string currency { get; set; }

    public List<UnitPrice> prices { get; set; }

    public string term { get; set; }

    public double costPerTerm { get; set; }
  }
}
