// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.SasDefinitionCreateParameters
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class SasDefinitionCreateParameters
  {
    public SasDefinitionCreateParameters()
    {
    }

    public SasDefinitionCreateParameters(
      string templateUri,
      string sasType,
      string validityPeriod,
      SasDefinitionAttributes sasDefinitionAttributes = null,
      IDictionary<string, string> tags = null)
    {
      this.TemplateUri = templateUri;
      this.SasType = sasType;
      this.ValidityPeriod = validityPeriod;
      this.SasDefinitionAttributes = sasDefinitionAttributes;
      this.Tags = tags;
    }

    [JsonProperty(PropertyName = "templateUri")]
    public string TemplateUri { get; set; }

    [JsonProperty(PropertyName = "sasType")]
    public string SasType { get; set; }

    [JsonProperty(PropertyName = "validityPeriod")]
    public string ValidityPeriod { get; set; }

    [JsonProperty(PropertyName = "attributes")]
    public SasDefinitionAttributes SasDefinitionAttributes { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }

    public virtual void Validate()
    {
      if (this.TemplateUri == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "TemplateUri");
      if (this.SasType == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "SasType");
      if (this.ValidityPeriod == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "ValidityPeriod");
    }
  }
}
