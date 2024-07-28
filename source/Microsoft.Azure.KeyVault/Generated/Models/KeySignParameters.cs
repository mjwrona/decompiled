// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.KeySignParameters
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace Microsoft.Azure.KeyVault.Models
{
  public class KeySignParameters
  {
    public KeySignParameters()
    {
    }

    public KeySignParameters(string algorithm, byte[] value)
    {
      this.Algorithm = algorithm;
      this.Value = value;
    }

    [JsonProperty(PropertyName = "alg")]
    public string Algorithm { get; set; }

    [JsonConverter(typeof (Base64UrlJsonConverter))]
    [JsonProperty(PropertyName = "value")]
    public byte[] Value { get; set; }

    public virtual void Validate()
    {
      if (this.Algorithm == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Algorithm");
      if (this.Value == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Value");
      if (this.Algorithm != null && this.Algorithm.Length < 1)
        throw new ValidationException(ValidationRules.MinLength, "Algorithm", (object) 1);
    }
  }
}
