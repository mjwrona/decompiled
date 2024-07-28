// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.SecretSetParameters
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class SecretSetParameters
  {
    public SecretSetParameters()
    {
    }

    public SecretSetParameters(
      string value,
      IDictionary<string, string> tags = null,
      string contentType = null,
      SecretAttributes secretAttributes = null)
    {
      this.Value = value;
      this.Tags = tags;
      this.ContentType = contentType;
      this.SecretAttributes = secretAttributes;
    }

    [JsonProperty(PropertyName = "value")]
    public string Value { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }

    [JsonProperty(PropertyName = "contentType")]
    public string ContentType { get; set; }

    [JsonProperty(PropertyName = "attributes")]
    public SecretAttributes SecretAttributes { get; set; }

    public virtual void Validate()
    {
      if (this.Value == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "Value");
    }
  }
}
