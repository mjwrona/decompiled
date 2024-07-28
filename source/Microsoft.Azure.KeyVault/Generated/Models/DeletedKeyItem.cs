// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.DeletedKeyItem
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class DeletedKeyItem : KeyItem
  {
    public DeletedKeyIdentifier RecoveryIdentifier => !string.IsNullOrWhiteSpace(this.RecoveryId) ? new DeletedKeyIdentifier(this.RecoveryId) : (DeletedKeyIdentifier) null;

    public DeletedKeyItem()
    {
    }

    public DeletedKeyItem(
      string kid = null,
      KeyAttributes attributes = null,
      IDictionary<string, string> tags = null,
      bool? managed = null,
      string recoveryId = null,
      DateTime? scheduledPurgeDate = null,
      DateTime? deletedDate = null)
      : base(kid, attributes, tags, managed)
    {
      this.RecoveryId = recoveryId;
      this.ScheduledPurgeDate = scheduledPurgeDate;
      this.DeletedDate = deletedDate;
    }

    [JsonProperty(PropertyName = "recoveryId")]
    public string RecoveryId { get; set; }

    [JsonConverter(typeof (UnixTimeJsonConverter))]
    [JsonProperty(PropertyName = "scheduledPurgeDate")]
    public DateTime? ScheduledPurgeDate { get; private set; }

    [JsonConverter(typeof (UnixTimeJsonConverter))]
    [JsonProperty(PropertyName = "deletedDate")]
    public DateTime? DeletedDate { get; private set; }
  }
}
