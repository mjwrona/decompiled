// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.KeyCreateParameters
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class KeyCreateParameters
  {
    public KeyCreateParameters()
    {
    }

    public KeyCreateParameters(
      string kty,
      int? keySize = null,
      IList<string> keyOps = null,
      KeyAttributes keyAttributes = null,
      IDictionary<string, string> tags = null,
      string curve = null)
    {
      this.Kty = kty;
      this.KeySize = keySize;
      this.KeyOps = keyOps;
      this.KeyAttributes = keyAttributes;
      this.Tags = tags;
      this.Curve = curve;
    }

    [JsonProperty(PropertyName = "kty")]
    public string Kty { get; set; }

    [JsonProperty(PropertyName = "key_size")]
    public int? KeySize { get; set; }

    [JsonProperty(PropertyName = "key_ops")]
    public IList<string> KeyOps { get; set; }

    [JsonProperty(PropertyName = "attributes")]
    public KeyAttributes KeyAttributes { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }

    [JsonProperty(PropertyName = "crv")]
    public string Curve { get; set; }

    public virtual void Validate()
    {
      if (this.Kty == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Kty");
      if (this.Kty != null && this.Kty.Length < 1)
        throw new ValidationException(ValidationRules.MinLength, "Kty", (object) 1);
    }
  }
}
