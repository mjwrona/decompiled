// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.CertificateUpdateParameters
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class CertificateUpdateParameters
  {
    public CertificateUpdateParameters()
    {
    }

    public CertificateUpdateParameters(
      CertificatePolicy certificatePolicy = null,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null)
    {
      this.CertificatePolicy = certificatePolicy;
      this.CertificateAttributes = certificateAttributes;
      this.Tags = tags;
    }

    [JsonProperty(PropertyName = "policy")]
    public CertificatePolicy CertificatePolicy { get; set; }

    [JsonProperty(PropertyName = "attributes")]
    public CertificateAttributes CertificateAttributes { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }

    public virtual void Validate()
    {
      if (this.CertificatePolicy == null)
        return;
      this.CertificatePolicy.Validate();
    }
  }
}
