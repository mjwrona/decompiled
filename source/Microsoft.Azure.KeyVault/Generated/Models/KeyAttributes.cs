// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.KeyAttributes
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.KeyVault.Models
{
  public class KeyAttributes : Attributes
  {
    public KeyAttributes()
    {
    }

    public KeyAttributes(
      bool? enabled = null,
      DateTime? notBefore = null,
      DateTime? expires = null,
      DateTime? created = null,
      DateTime? updated = null,
      string recoveryLevel = null)
      : base(enabled, notBefore, expires, created, updated)
    {
      this.RecoveryLevel = recoveryLevel;
    }

    [JsonProperty(PropertyName = "recoveryLevel")]
    public string RecoveryLevel { get; private set; }
  }
}
