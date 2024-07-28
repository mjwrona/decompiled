// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.KeyBundle
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Azure.KeyVault.WebKey;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class KeyBundle
  {
    public KeyIdentifier KeyIdentifier => this.Key != null && !string.IsNullOrWhiteSpace(this.Key.Kid) ? new KeyIdentifier(this.Key.Kid) : (KeyIdentifier) null;

    public KeyBundle()
    {
    }

    public KeyBundle(
      JsonWebKey key = null,
      KeyAttributes attributes = null,
      IDictionary<string, string> tags = null,
      bool? managed = null)
    {
      this.Key = key;
      this.Attributes = attributes;
      this.Tags = tags;
      this.Managed = managed;
    }

    [JsonProperty(PropertyName = "key")]
    public JsonWebKey Key { get; set; }

    [JsonProperty(PropertyName = "attributes")]
    public KeyAttributes Attributes { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }

    [JsonProperty(PropertyName = "managed")]
    public bool? Managed { get; private set; }
  }
}
