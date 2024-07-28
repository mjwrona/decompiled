// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.Dependency
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class Dependency
  {
    public Dependency()
    {
    }

    public Dependency(
      IList<BasicDependency> dependsOn = null,
      string id = null,
      string resourceType = null,
      string resourceName = null)
    {
      this.DependsOn = dependsOn;
      this.Id = id;
      this.ResourceType = resourceType;
      this.ResourceName = resourceName;
    }

    [JsonProperty(PropertyName = "dependsOn")]
    public IList<BasicDependency> DependsOn { get; set; }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "resourceType")]
    public string ResourceType { get; set; }

    [JsonProperty(PropertyName = "resourceName")]
    public string ResourceName { get; set; }
  }
}
