// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [DataContract]
  public enum SubscriptionStatus
  {
    [EnumMember] Enabled = 0,
    [EnumMember] OnProbation = 10, // 0x0000000A
    [EnumMember] DisabledByUser = 20, // 0x00000014
    [EnumMember] DisabledBySystem = 30, // 0x0000001E
    [EnumMember] DisabledByInactiveIdentity = 40, // 0x00000028
  }
}
