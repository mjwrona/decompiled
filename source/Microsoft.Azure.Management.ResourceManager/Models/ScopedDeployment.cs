// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.ScopedDeployment
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class ScopedDeployment
  {
    public ScopedDeployment()
    {
    }

    public ScopedDeployment(string location, DeploymentProperties properties)
    {
      this.Location = location;
      this.Properties = properties;
    }

    [JsonProperty(PropertyName = "location")]
    public string Location { get; set; }

    [JsonProperty(PropertyName = "properties")]
    public DeploymentProperties Properties { get; set; }

    public virtual void Validate()
    {
      if (this.Location == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Location");
      if (this.Properties == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Properties");
      if (this.Properties == null)
        return;
      this.Properties.Validate();
    }
  }
}
