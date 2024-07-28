// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.StorageAccountUpdateParameters
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class StorageAccountUpdateParameters
  {
    public StorageAccountUpdateParameters()
    {
    }

    public StorageAccountUpdateParameters(
      string activeKeyName = null,
      bool? autoRegenerateKey = null,
      string regenerationPeriod = null,
      StorageAccountAttributes storageAccountAttributes = null,
      IDictionary<string, string> tags = null)
    {
      this.ActiveKeyName = activeKeyName;
      this.AutoRegenerateKey = autoRegenerateKey;
      this.RegenerationPeriod = regenerationPeriod;
      this.StorageAccountAttributes = storageAccountAttributes;
      this.Tags = tags;
    }

    [JsonProperty(PropertyName = "activeKeyName")]
    public string ActiveKeyName { get; set; }

    [JsonProperty(PropertyName = "autoRegenerateKey")]
    public bool? AutoRegenerateKey { get; set; }

    [JsonProperty(PropertyName = "regenerationPeriod")]
    public string RegenerationPeriod { get; set; }

    [JsonProperty(PropertyName = "attributes")]
    public StorageAccountAttributes StorageAccountAttributes { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public IDictionary<string, string> Tags { get; set; }
  }
}
