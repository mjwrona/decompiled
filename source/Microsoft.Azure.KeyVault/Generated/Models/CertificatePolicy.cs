// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.CertificatePolicy
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class CertificatePolicy
  {
    public CertificatePolicy()
    {
    }

    public CertificatePolicy(
      string id = null,
      KeyProperties keyProperties = null,
      SecretProperties secretProperties = null,
      X509CertificateProperties x509CertificateProperties = null,
      IList<LifetimeAction> lifetimeActions = null,
      IssuerParameters issuerParameters = null,
      CertificateAttributes attributes = null)
    {
      this.Id = id;
      this.KeyProperties = keyProperties;
      this.SecretProperties = secretProperties;
      this.X509CertificateProperties = x509CertificateProperties;
      this.LifetimeActions = lifetimeActions;
      this.IssuerParameters = issuerParameters;
      this.Attributes = attributes;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "key_props")]
    public KeyProperties KeyProperties { get; set; }

    [JsonProperty(PropertyName = "secret_props")]
    public SecretProperties SecretProperties { get; set; }

    [JsonProperty(PropertyName = "x509_props")]
    public X509CertificateProperties X509CertificateProperties { get; set; }

    [JsonProperty(PropertyName = "lifetime_actions")]
    public IList<LifetimeAction> LifetimeActions { get; set; }

    [JsonProperty(PropertyName = "issuer")]
    public IssuerParameters IssuerParameters { get; set; }

    [JsonProperty(PropertyName = "attributes")]
    public CertificateAttributes Attributes { get; set; }

    public virtual void Validate()
    {
      if (this.X509CertificateProperties != null)
        this.X509CertificateProperties.Validate();
      if (this.LifetimeActions == null)
        return;
      foreach (LifetimeAction lifetimeAction in (IEnumerable<LifetimeAction>) this.LifetimeActions)
        lifetimeAction?.Validate();
    }
  }
}
