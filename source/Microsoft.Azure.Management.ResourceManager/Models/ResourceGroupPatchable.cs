// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ResourceGroupPatchable
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class ResourceGroupPatchable
  {
    public ResourceGroupPatchable()
    {
    }

    public ResourceGroupPatchable(
      string name = null,
      ResourceGroupProperties properties = null,
      string managedBy = null,
      IDictionary<string, string> tags = null)
    {
      this.Name = name;
      this.Properties = properties;
      this.ManagedBy = managedBy;
      this.Tags = tags;
    }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "properties")]
    public ResourceGroupProperties Properties { get; set; }

    [JsonProperty(PropertyName = "managedBy")]
    public string ManagedBy { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }
  }
}
