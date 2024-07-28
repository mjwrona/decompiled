// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.SecretRestoreParameters
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace Microsoft.Azure.KeyVault.Models
{
  public class SecretRestoreParameters
  {
    public SecretRestoreParameters()
    {
    }

    public SecretRestoreParameters(byte[] secretBundleBackup) => this.SecretBundleBackup = secretBundleBackup;

    [JsonConverter(typeof (Base64UrlJsonConverter))]
    [JsonProperty(PropertyName = "value")]
    public byte[] SecretBundleBackup { get; set; }

    public virtual void Validate()
    {
      if (this.SecretBundleBackup == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "SecretBundleBackup");
    }
  }
}
