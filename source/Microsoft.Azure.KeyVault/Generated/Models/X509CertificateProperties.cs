// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.X509CertificateProperties
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class X509CertificateProperties
  {
    public X509CertificateProperties()
    {
    }

    public X509CertificateProperties(
      string subject = null,
      IList<string> ekus = null,
      SubjectAlternativeNames subjectAlternativeNames = null,
      IList<string> keyUsage = null,
      int? validityInMonths = null)
    {
      this.Subject = subject;
      this.Ekus = ekus;
      this.SubjectAlternativeNames = subjectAlternativeNames;
      this.KeyUsage = keyUsage;
      this.ValidityInMonths = validityInMonths;
    }

    [JsonProperty(PropertyName = "subject")]
    public string Subject { get; set; }

    [JsonProperty(PropertyName = "ekus")]
    public IList<string> Ekus { get; set; }

    [JsonProperty(PropertyName = "sans")]
    public SubjectAlternativeNames SubjectAlternativeNames { get; set; }

    [JsonProperty(PropertyName = "key_usage")]
    public IList<string> KeyUsage { get; set; }

    [JsonProperty(PropertyName = "validity_months")]
    public int? ValidityInMonths { get; set; }

    public virtual void Validate()
    {
      int? validityInMonths = this.ValidityInMonths;
      int num = 0;
      if (validityInMonths.GetValueOrDefault() < num & validityInMonths.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, "ValidityInMonths", (object) 0);
    }
  }
}
