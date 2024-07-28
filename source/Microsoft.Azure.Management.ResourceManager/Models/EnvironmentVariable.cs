// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.EnvironmentVariable
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using Newtonsoft.Json;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class EnvironmentVariable
  {
    public EnvironmentVariable()
    {
    }

    public EnvironmentVariable(string name, string value = null, string secureValue = null)
    {
      this.Name = name;
      this.Value = value;
      this.SecureValue = secureValue;
    }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "value")]
    public string Value { get; set; }

    [JsonProperty(PropertyName = "secureValue")]
    public string SecureValue { get; set; }

    public virtual void Validate()
    {
      if (this.Name == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Name");
    }
  }
}
