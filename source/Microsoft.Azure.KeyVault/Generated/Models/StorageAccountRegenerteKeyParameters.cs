// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.StorageAccountRegenerteKeyParameters
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest;
using Newtonsoft.Json;

namespace Microsoft.Azure.KeyVault.Models
{
  public class StorageAccountRegenerteKeyParameters
  {
    public StorageAccountRegenerteKeyParameters()
    {
    }

    public StorageAccountRegenerteKeyParameters(string keyName) => this.KeyName = keyName;

    [JsonProperty(PropertyName = "keyName")]
    public string KeyName { get; set; }

    public virtual void Validate()
    {
      if (this.KeyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "KeyName");
    }
  }
}
