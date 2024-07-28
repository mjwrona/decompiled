// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.MarketplaceOfferContract
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

namespace Microsoft.VisualStudio.Services.WebApi
{
  public class MarketplaceOfferContract
  {
    public string Type { get; set; }

    public string AssetId { get; set; }

    public long AssetVersion { get; set; }

    public string PlanId { get; set; }

    public RESTApiRequestOperationType Operation { get; set; }

    public string CustomerSupportEmail { get; set; }

    public string CustomerSupportPhoneNumber { get; set; }

    public string IntegrationContactEmail { get; set; }

    public string IntegrationContactPhoneNumber { get; set; }

    public AssetDetailObject AssetDetails { get; set; }

    public RestApiResponseStatusModel operationStatus { get; set; }
  }
}
