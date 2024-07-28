// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.StorageAccountAttributes
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.KeyVault.Models
{
  public class StorageAccountAttributes
  {
    public StorageAccountAttributes()
    {
    }

    public StorageAccountAttributes(
      bool? enabled = null,
      DateTime? created = null,
      DateTime? updated = null,
      string recoveryLevel = null)
    {
      this.Enabled = enabled;
      this.Created = created;
      this.Updated = updated;
      this.RecoveryLevel = recoveryLevel;
    }

    [JsonProperty(PropertyName = "enabled")]
    public bool? Enabled { get; set; }

    [JsonConverter(typeof (UnixTimeJsonConverter))]
    [JsonProperty(PropertyName = "created")]
    public DateTime? Created { get; private set; }

    [JsonConverter(typeof (UnixTimeJsonConverter))]
    [JsonProperty(PropertyName = "updated")]
    public DateTime? Updated { get; private set; }

    [JsonProperty(PropertyName = "recoveryLevel")]
    public string RecoveryLevel { get; private set; }
  }
}
