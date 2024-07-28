// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.SecretItem
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class SecretItem
  {
    public SecretIdentifier Identifier => !string.IsNullOrWhiteSpace(this.Id) ? new SecretIdentifier(this.Id) : (SecretIdentifier) null;

    public SecretItem()
    {
    }

    public SecretItem(
      string id = null,
      SecretAttributes attributes = null,
      IDictionary<string, string> tags = null,
      string contentType = null,
      bool? managed = null)
    {
      this.Id = id;
      this.Attributes = attributes;
      this.Tags = tags;
      this.ContentType = contentType;
      this.Managed = managed;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "attributes")]
    public SecretAttributes Attributes { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }

    [JsonProperty(PropertyName = "contentType")]
    public string ContentType { get; set; }

    [JsonProperty(PropertyName = "managed")]
    public bool? Managed { get; private set; }
  }
}
