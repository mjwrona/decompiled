// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.CertificateItem
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class CertificateItem
  {
    public CertificateIdentifier Identifier => !string.IsNullOrWhiteSpace(this.Id) ? new CertificateIdentifier(this.Id) : (CertificateIdentifier) null;

    public CertificateItem()
    {
    }

    public CertificateItem(
      string id = null,
      CertificateAttributes attributes = null,
      IDictionary<string, string> tags = null,
      byte[] x509Thumbprint = null)
    {
      this.Id = id;
      this.Attributes = attributes;
      this.Tags = tags;
      this.X509Thumbprint = x509Thumbprint;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "attributes")]
    public CertificateAttributes Attributes { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }

    [JsonConverter(typeof (Base64UrlJsonConverter))]
    [JsonProperty(PropertyName = "x5t")]
    public byte[] X509Thumbprint { get; set; }
  }
}
