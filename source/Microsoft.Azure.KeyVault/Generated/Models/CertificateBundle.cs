// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.CertificateBundle
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class CertificateBundle
  {
    public SecretIdentifier SecretIdentifier => !string.IsNullOrWhiteSpace(this.Sid) ? new SecretIdentifier(this.Sid) : (SecretIdentifier) null;

    public KeyIdentifier KeyIdentifier => !string.IsNullOrWhiteSpace(this.Kid) ? new KeyIdentifier(this.Kid) : (KeyIdentifier) null;

    public CertificateIdentifier CertificateIdentifier => !string.IsNullOrWhiteSpace(this.Id) ? new CertificateIdentifier(this.Id) : (CertificateIdentifier) null;

    public CertificateBundle()
    {
    }

    public CertificateBundle(
      string id = null,
      string kid = null,
      string sid = null,
      byte[] x509Thumbprint = null,
      CertificatePolicy policy = null,
      byte[] cer = null,
      string contentType = null,
      CertificateAttributes attributes = null,
      IDictionary<string, string> tags = null)
    {
      this.Id = id;
      this.Kid = kid;
      this.Sid = sid;
      this.X509Thumbprint = x509Thumbprint;
      this.Policy = policy;
      this.Cer = cer;
      this.ContentType = contentType;
      this.Attributes = attributes;
      this.Tags = tags;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "kid")]
    public string Kid { get; private set; }

    [JsonProperty(PropertyName = "sid")]
    public string Sid { get; private set; }

    [JsonConverter(typeof (Base64UrlJsonConverter))]
    [JsonProperty(PropertyName = "x5t")]
    public byte[] X509Thumbprint { get; private set; }

    [JsonProperty(PropertyName = "policy")]
    public CertificatePolicy Policy { get; private set; }

    [JsonProperty(PropertyName = "cer")]
    public byte[] Cer { get; set; }

    [JsonProperty(PropertyName = "contentType")]
    public string ContentType { get; set; }

    [JsonProperty(PropertyName = "attributes")]
    public CertificateAttributes Attributes { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }

    public virtual void Validate()
    {
      if (this.Policy == null)
        return;
      this.Policy.Validate();
    }
  }
}
