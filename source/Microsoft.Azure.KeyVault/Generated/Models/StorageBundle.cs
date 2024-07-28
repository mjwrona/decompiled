// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.StorageBundle
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class StorageBundle
  {
    public StorageBundle()
    {
    }

    public StorageBundle(
      string id = null,
      string resourceId = null,
      string activeKeyName = null,
      bool? autoRegenerateKey = null,
      string regenerationPeriod = null,
      StorageAccountAttributes attributes = null,
      IDictionary<string, string> tags = null)
    {
      this.Id = id;
      this.ResourceId = resourceId;
      this.ActiveKeyName = activeKeyName;
      this.AutoRegenerateKey = autoRegenerateKey;
      this.RegenerationPeriod = regenerationPeriod;
      this.Attributes = attributes;
      this.Tags = tags;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "resourceId")]
    public string ResourceId { get; private set; }

    [JsonProperty(PropertyName = "activeKeyName")]
    public string ActiveKeyName { get; private set; }

    [JsonProperty(PropertyName = "autoRegenerateKey")]
    public bool? AutoRegenerateKey { get; private set; }

    [JsonProperty(PropertyName = "regenerationPeriod")]
    public string RegenerationPeriod { get; private set; }

    [JsonProperty(PropertyName = "attributes")]
    public StorageAccountAttributes Attributes { get; private set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; private set; }
  }
}
