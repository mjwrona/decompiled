// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.EventLogCategory
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal enum EventLogCategory : ushort
  {
    ServiceAuthorization = 1,
    MessageAuthentication = 2,
    ObjectAccess = 3,
    Tracing = 4,
    WebHost = 5,
    FailFast = 6,
    MessageLogging = 7,
    PerformanceCounter = 8,
    Wmi = 9,
    ComPlus = 10, // 0x000A
    StateMachine = 11, // 0x000B
    Wsat = 12, // 0x000C
    SharingService = 13, // 0x000D
    ListenerAdapter = 14, // 0x000E
  }
}
