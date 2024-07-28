// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Models.BaseExtensionItem
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Models
{
  public abstract class BaseExtensionItem
  {
    [JsonProperty("a")]
    public string Author { get; set; }

    [JsonProperty("cc")]
    public int CostCategory { get; set; }

    [JsonProperty("l")]
    public string Link { get; set; }

    [JsonProperty("s")]
    public string Summary { get; set; }

    [JsonProperty("i")]
    public string Thumbnail { get; set; }

    [JsonProperty("fi")]
    public string FallbackThumbnail { get; set; }

    [JsonProperty("t")]
    public string Title { get; set; }

    [JsonProperty("r")]
    public double Rating { get; set; }

    [JsonProperty("pc")]
    public bool IsPublisherCertified { get; set; }

    [JsonProperty("pd")]
    public string PublisherDomain { get; set; }

    [JsonProperty("pdv")]
    public bool IsPublisherDomainVerified { get; set; }
  }
}
