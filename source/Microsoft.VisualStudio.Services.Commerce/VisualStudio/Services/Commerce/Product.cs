// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Product
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class Product
  {
    public string id { get; set; }

    public string publisher { get; set; }

    public string offer { get; set; }

    public string plan { get; set; }

    public List<Meter> meters { get; set; }

    public Features features { get; set; }

    public EstimatedPrices estimatedPrices { get; set; }

    public UnitPrices unitPrices { get; set; }
  }
}
