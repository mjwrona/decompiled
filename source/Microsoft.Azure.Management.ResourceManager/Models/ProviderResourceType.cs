// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ProviderResourceType
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class ProviderResourceType
  {
    public ProviderResourceType()
    {
    }

    public ProviderResourceType(
      string resourceType = null,
      IList<string> locations = null,
      IList<AliasType> aliases = null,
      IList<string> apiVersions = null,
      string capabilities = null,
      IDictionary<string, string> properties = null)
    {
      this.ResourceType = resourceType;
      this.Locations = locations;
      this.Aliases = aliases;
      this.ApiVersions = apiVersions;
      this.Capabilities = capabilities;
      this.Properties = properties;
    }

    [JsonProperty(PropertyName = "resourceType")]
    public string ResourceType { get; set; }

    [JsonProperty(PropertyName = "locations")]
    public IList<string> Locations { get; set; }

    [JsonProperty(PropertyName = "aliases")]
    public IList<AliasType> Aliases { get; set; }

    [JsonProperty(PropertyName = "apiVersions")]
    public IList<string> ApiVersions { get; set; }

    [JsonProperty(PropertyName = "capabilities")]
    public string Capabilities { get; set; }

    [JsonProperty(PropertyName = "properties")]
    public IDictionary<string, string> Properties { get; set; }
  }
}
