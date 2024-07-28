// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.KeyItem
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class KeyItem
  {
    public KeyIdentifier Identifier => !string.IsNullOrWhiteSpace(this.Kid) ? new KeyIdentifier(this.Kid) : (KeyIdentifier) null;

    public KeyItem()
    {
    }

    public KeyItem(
      string kid = null,
      KeyAttributes attributes = null,
      IDictionary<string, string> tags = null,
      bool? managed = null)
    {
      this.Kid = kid;
      this.Attributes = attributes;
      this.Tags = tags;
      this.Managed = managed;
    }

    [JsonProperty(PropertyName = "kid")]
    public string Kid { get; set; }

    [JsonProperty(PropertyName = "attributes")]
    public KeyAttributes Attributes { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }

    [JsonProperty(PropertyName = "managed")]
    public bool? Managed { get; private set; }
  }
}
