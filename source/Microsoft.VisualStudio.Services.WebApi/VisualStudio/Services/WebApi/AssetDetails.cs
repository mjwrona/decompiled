// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.AssetDetails
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public class AssetDetails
  {
    [JsonProperty("ChangedTime")]
    public DateTime ChangedTime { get; set; }

    [JsonProperty("Definition")]
    public Definition Definition { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("OfferTypeChange")]
    public long OfferTypeChange { get; set; }

    [JsonProperty("OfferTypeId")]
    public string OfferTypeId { get; set; }

    [JsonProperty("OfferTypeVersions")]
    public OfferTypeVersions OfferTypeVersions { get; set; }

    [JsonProperty("PricingNotRecalculatedRegions")]
    public List<object> PricingNotRecalculatedRegions { get; set; }

    [JsonProperty("PublisherId")]
    public string PublisherId { get; set; }

    [JsonProperty("Status")]
    public long Status { get; set; }

    [JsonProperty("Version")]
    public long Version { get; set; }
  }
}
