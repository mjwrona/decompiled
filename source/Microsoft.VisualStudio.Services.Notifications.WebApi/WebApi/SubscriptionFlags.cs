// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionFlags
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [Flags]
  [DataContract]
  public enum SubscriptionFlags
  {
    [EnumMember] None = 0,
    [EnumMember] GroupSubscription = 1,
    [EnumMember] ContributedSubscription = 2,
    [EnumMember] CanOptOut = 4,
    [EnumMember] TeamSubscription = 8,
    [EnumMember] OneActorMatches = 16, // 0x00000010
  }
}
