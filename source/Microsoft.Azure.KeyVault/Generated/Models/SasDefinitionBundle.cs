// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.SasDefinitionBundle
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class SasDefinitionBundle
  {
    public SasDefinitionBundle()
    {
    }

    public SasDefinitionBundle(
      string id = null,
      string secretId = null,
      string templateUri = null,
      string sasType = null,
      string validityPeriod = null,
      SasDefinitionAttributes attributes = null,
      IDictionary<string, string> tags = null)
    {
      this.Id = id;
      this.SecretId = secretId;
      this.TemplateUri = templateUri;
      this.SasType = sasType;
      this.ValidityPeriod = validityPeriod;
      this.Attributes = attributes;
      this.Tags = tags;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "sid")]
    public string SecretId { get; private set; }

    [JsonProperty(PropertyName = "templateUri")]
    public string TemplateUri { get; private set; }

    [JsonProperty(PropertyName = "sasType")]
    public string SasType { get; private set; }

    [JsonProperty(PropertyName = "validityPeriod")]
    public string ValidityPeriod { get; private set; }

    [JsonProperty(PropertyName = "attributes")]
    public SasDefinitionAttributes Attributes { get; private set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; private set; }
  }
}
