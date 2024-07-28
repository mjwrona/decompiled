// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.ActionType
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Microsoft.Azure.KeyVault.Models
{
  [JsonConverter(typeof (StringEnumConverter))]
  public enum ActionType
  {
    [EnumMember(Value = "EmailContacts")] EmailContacts,
    [EnumMember(Value = "AutoRenew")] AutoRenew,
  }
}
