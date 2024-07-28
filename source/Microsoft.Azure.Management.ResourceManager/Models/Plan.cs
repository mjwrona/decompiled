// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.Plan
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class Plan
  {
    public Plan()
    {
    }

    public Plan(
      string name = null,
      string publisher = null,
      string product = null,
      string promotionCode = null,
      string version = null)
    {
      this.Name = name;
      this.Publisher = publisher;
      this.Product = product;
      this.PromotionCode = promotionCode;
      this.Version = version;
    }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "publisher")]
    public string Publisher { get; set; }

    [JsonProperty(PropertyName = "product")]
    public string Product { get; set; }

    [JsonProperty(PropertyName = "promotionCode")]
    public string PromotionCode { get; set; }

    [JsonProperty(PropertyName = "version")]
    public string Version { get; set; }
  }
}
