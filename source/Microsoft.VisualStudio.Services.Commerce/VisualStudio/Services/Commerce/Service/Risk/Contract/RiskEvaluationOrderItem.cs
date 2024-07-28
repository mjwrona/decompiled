// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Service.Risk.Contract.RiskEvaluationOrderItem
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce.Service.Risk.Contract
{
  public class RiskEvaluationOrderItem
  {
    public string line_item_id { get; set; }

    public Guid product_id { get; set; } = Guid.Empty;

    public string product_type { get; set; }

    public string sku_id { get; set; }

    public string availability_id { get; set; }

    public int quantity { get; set; }

    public double list_price { get; set; }

    public double retail_price { get; set; }

    public string price_currency_code { get; set; }

    public static string GetProductType(Guid platformMeterId)
    {
      if (platformMeterId.Equals(new Guid("8330ee7e-073d-45b3-912a-6a6332a541fa")))
        return "Visual Studio Professional Monthly";
      if (platformMeterId.Equals(new Guid("5f688460-13f4-43e4-93b8-0086c0d28efc")))
        return "Visual Studio Professional Annual";
      if (platformMeterId.Equals(new Guid("1b207df7-7922-468d-aba9-2906ef34a65d")))
        return "Visual Studio Enterprise Monthly";
      if (platformMeterId.Equals(new Guid("4e62a388-c558-44dd-be07-b7dce949efcd")))
        return "Visual Studio Enterprise Annual";
      return platformMeterId.Equals(new Guid("f3e9328c-9c6e-4e2c-86d6-9a599fc51265")) ? "Xamarin University" : "unknown";
    }
  }
}
