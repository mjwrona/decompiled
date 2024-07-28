// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.NotificationPlatform
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  public enum NotificationPlatform
  {
    [EnumMember(Value = "wns")] Wns = 1,
    [EnumMember(Value = "apns")] Apns = 2,
    [EnumMember(Value = "mpns")] Mpns = 3,
    [EnumMember(Value = "gcm")] Gcm = 4,
    [EnumMember(Value = "adm")] Adm = 5,
  }
}
