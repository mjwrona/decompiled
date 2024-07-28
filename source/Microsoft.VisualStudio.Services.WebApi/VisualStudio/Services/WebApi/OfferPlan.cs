// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.OfferPlan
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public class OfferPlan
  {
    [JsonProperty("monthlyPricing")]
    public MonthlyPricing MonthlyPricing { get; set; }

    [JsonProperty("planId")]
    public string PlanId { get; set; }

    [JsonProperty("vs-marketplace-extensions.skuDescription")]
    public string VsMarketplaceExtensionsSkuDescription { get; set; }

    [JsonProperty("vs-marketplace-extensions.skuSummary")]
    public string VsMarketplaceExtensionsSkuSummary { get; set; }

    [JsonProperty("vs-marketplace-extensions.skuTitle")]
    public string VsMarketplaceExtensionsSkuTitle { get; set; }

    [JsonProperty("vs-marketplace-extensions.skuUsers")]
    public int VsMarketplaceExtensionsSkuUsers { get; set; }
  }
}
