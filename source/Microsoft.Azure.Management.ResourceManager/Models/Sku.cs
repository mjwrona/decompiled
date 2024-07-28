// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.Sku
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class Sku
  {
    public Sku()
    {
    }

    public Sku(
      string name = null,
      string tier = null,
      string size = null,
      string family = null,
      string model = null,
      int? capacity = null)
    {
      this.Name = name;
      this.Tier = tier;
      this.Size = size;
      this.Family = family;
      this.Model = model;
      this.Capacity = capacity;
    }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "tier")]
    public string Tier { get; set; }

    [JsonProperty(PropertyName = "size")]
    public string Size { get; set; }

    [JsonProperty(PropertyName = "family")]
    public string Family { get; set; }

    [JsonProperty(PropertyName = "model")]
    public string Model { get; set; }

    [JsonProperty(PropertyName = "capacity")]
    public int? Capacity { get; set; }
  }
}
