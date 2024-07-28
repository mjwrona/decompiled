// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.MarketplaceOfferContract2
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public class MarketplaceOfferContract2
  {
    [JsonProperty("assetDetails")]
    public AssetDetails AssetDetails { get; set; }

    [JsonProperty("assetId")]
    public string AssetId { get; set; }

    [JsonProperty("assetVersion")]
    public long AssetVersion { get; set; }

    [JsonProperty("customerSupportEmail")]
    public string CustomerSupportEmail { get; set; }

    [JsonProperty("customerSupportPhoneNumber")]
    public string CustomerSupportPhoneNumber { get; set; }

    [JsonProperty("integrationContactEmail")]
    public string IntegrationContactEmail { get; set; }

    [JsonProperty("integrationContactPhoneNumber")]
    public string IntegrationContactPhoneNumber { get; set; }

    [JsonProperty("operation")]
    public RESTApiRequestOperationType2 Operation { get; set; }

    [JsonProperty("planId")]
    public string PlanId { get; set; }

    [JsonProperty("publishVersion")]
    public long PublishVersion { get; set; }

    [JsonProperty("publisherDisplayName")]
    public string PublisherDisplayName { get; set; }

    [JsonProperty("publisherId")]
    public string PublisherId { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("operationStatus")]
    public RestApiResponseStatusModel OperationStatus { get; set; }
  }
}
