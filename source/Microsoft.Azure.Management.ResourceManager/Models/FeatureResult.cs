// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.FeatureResult
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class FeatureResult
  {
    public FeatureResult()
    {
    }

    public FeatureResult(string name = null, FeatureProperties properties = null, string id = null, string type = null)
    {
      this.Name = name;
      this.Properties = properties;
      this.Id = id;
      this.Type = type;
    }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "properties")]
    public FeatureProperties Properties { get; set; }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }
  }
}
