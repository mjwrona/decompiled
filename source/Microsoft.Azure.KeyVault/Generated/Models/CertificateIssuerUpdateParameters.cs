// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.CertificateIssuerUpdateParameters
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.KeyVault.Models
{
  public class CertificateIssuerUpdateParameters
  {
    public CertificateIssuerUpdateParameters()
    {
    }

    public CertificateIssuerUpdateParameters(
      string provider = null,
      IssuerCredentials credentials = null,
      OrganizationDetails organizationDetails = null,
      IssuerAttributes attributes = null)
    {
      this.Provider = provider;
      this.Credentials = credentials;
      this.OrganizationDetails = organizationDetails;
      this.Attributes = attributes;
    }

    [JsonProperty(PropertyName = "provider")]
    public string Provider { get; set; }

    [JsonProperty(PropertyName = "credentials")]
    public IssuerCredentials Credentials { get; set; }

    [JsonProperty(PropertyName = "org_details")]
    public OrganizationDetails OrganizationDetails { get; set; }

    [JsonProperty(PropertyName = "attributes")]
    public IssuerAttributes Attributes { get; set; }
  }
}
