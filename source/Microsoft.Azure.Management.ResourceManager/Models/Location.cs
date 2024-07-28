// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.Location
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class Location
  {
    public Location()
    {
    }

    public Location(
      string id = null,
      string subscriptionId = null,
      string name = null,
      string displayName = null,
      string latitude = null,
      string longitude = null)
    {
      this.Id = id;
      this.SubscriptionId = subscriptionId;
      this.Name = name;
      this.DisplayName = displayName;
      this.Latitude = latitude;
      this.Longitude = longitude;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "subscriptionId")]
    public string SubscriptionId { get; private set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; private set; }

    [JsonProperty(PropertyName = "displayName")]
    public string DisplayName { get; private set; }

    [JsonProperty(PropertyName = "latitude")]
    public string Latitude { get; private set; }

    [JsonProperty(PropertyName = "longitude")]
    public string Longitude { get; private set; }
  }
}
