// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.DeploymentScript
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class DeploymentScript : AzureResourceBase
  {
    public DeploymentScript()
    {
    }

    public DeploymentScript(
      ManagedServiceIdentity identity,
      string location,
      string id = null,
      string name = null,
      string type = null,
      IDictionary<string, string> tags = null)
      : base(id, name, type)
    {
      this.Identity = identity;
      this.Location = location;
      this.Tags = tags;
    }

    [JsonProperty(PropertyName = "identity")]
    public ManagedServiceIdentity Identity { get; set; }

    [JsonProperty(PropertyName = "location")]
    public string Location { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }

    public virtual void Validate()
    {
      if (this.Identity == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Identity");
      if (this.Location == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Location");
    }
  }
}
