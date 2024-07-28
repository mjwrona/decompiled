// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.DeploymentExtended
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest.Azure;
using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class DeploymentExtended : IResource
  {
    public DeploymentExtended()
    {
    }

    public DeploymentExtended(
      string id = null,
      string name = null,
      string type = null,
      string location = null,
      DeploymentPropertiesExtended properties = null)
    {
      this.Id = id;
      this.Name = name;
      this.Type = type;
      this.Location = location;
      this.Properties = properties;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; private set; }

    [JsonProperty(PropertyName = "type")]
    public string Type { get; private set; }

    [JsonProperty(PropertyName = "location")]
    public string Location { get; set; }

    [JsonProperty(PropertyName = "properties")]
    public DeploymentPropertiesExtended Properties { get; set; }

    public virtual void Validate()
    {
      if (this.Properties == null)
        return;
      this.Properties.Validate();
    }
  }
}
