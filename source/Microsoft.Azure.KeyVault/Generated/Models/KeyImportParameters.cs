// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.KeyImportParameters
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class KeyImportParameters
  {
    public KeyImportParameters()
    {
    }

    public KeyImportParameters(
      JsonWebKey key,
      bool? hsm = null,
      KeyAttributes keyAttributes = null,
      IDictionary<string, string> tags = null)
    {
      this.Hsm = hsm;
      this.Key = key;
      this.KeyAttributes = keyAttributes;
      this.Tags = tags;
    }

    [JsonProperty(PropertyName = "Hsm")]
    public bool? Hsm { get; set; }

    [JsonProperty(PropertyName = "key")]
    public JsonWebKey Key { get; set; }

    [JsonProperty(PropertyName = "attributes")]
    public KeyAttributes KeyAttributes { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }

    public virtual void Validate()
    {
      if (this.Key == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Key");
    }
  }
}
