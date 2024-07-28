// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.BasicDependency
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class BasicDependency
  {
    public BasicDependency()
    {
    }

    public BasicDependency(string id = null, string resourceType = null, string resourceName = null)
    {
      this.Id = id;
      this.ResourceType = resourceType;
      this.ResourceName = resourceName;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "resourceType")]
    public string ResourceType { get; set; }

    [JsonProperty(PropertyName = "resourceName")]
    public string ResourceName { get; set; }
  }
}
