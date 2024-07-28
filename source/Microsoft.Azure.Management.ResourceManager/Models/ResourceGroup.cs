// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ResourceGroup
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class ResourceGroup : IResource
  {
    public ResourceGroup()
    {
    }

    public ResourceGroup(
      string location,
      string id = null,
      string name = null,
      string type = null,
      ResourceGroupProperties properties = null,
      string managedBy = null,
      IDictionary<string, string> tags = null)
    {
      this.Id = id;
      this.Name = name;
      this.Type = type;
      this.Properties = properties;
      this.Location = location;
      this.ManagedBy = managedBy;
      this.Tags = tags;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; private set; }

    [JsonProperty(PropertyName = "type")]
    public string Type { get; private set; }

    [JsonProperty(PropertyName = "properties")]
    public ResourceGroupProperties Properties { get; set; }

    [JsonProperty(PropertyName = "location")]
    public string Location { get; set; }

    [JsonProperty(PropertyName = "managedBy")]
    public string ManagedBy { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }

    public virtual void Validate()
    {
      if (this.Location == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Location");
    }
  }
}
