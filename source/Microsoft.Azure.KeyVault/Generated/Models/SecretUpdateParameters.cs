// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.SecretUpdateParameters
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class SecretUpdateParameters
  {
    public SecretUpdateParameters()
    {
    }

    public SecretUpdateParameters(
      string contentType = null,
      SecretAttributes secretAttributes = null,
      IDictionary<string, string> tags = null)
    {
      this.ContentType = contentType;
      this.SecretAttributes = secretAttributes;
      this.Tags = tags;
    }

    [JsonProperty(PropertyName = "contentType")]
    public string ContentType { get; set; }

    [JsonProperty(PropertyName = "attributes")]
    public SecretAttributes SecretAttributes { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }
  }
}
