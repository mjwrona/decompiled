// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.PolicySku
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class PolicySku
  {
    public PolicySku()
    {
    }

    public PolicySku(string name, string tier = null)
    {
      this.Name = name;
      this.Tier = tier;
    }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "tier")]
    public string Tier { get; set; }

    public virtual void Validate()
    {
      if (this.Name == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Name");
    }
  }
}
