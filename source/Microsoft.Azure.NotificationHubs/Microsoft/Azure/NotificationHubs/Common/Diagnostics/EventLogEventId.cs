// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Diagnostics.EventLogEventId
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Common.Diagnostics
{
  internal enum EventLogEventId : uint
  {
    FailedToSetupTracing = 3221291108, // 0xC0010064
    FailedToInitializeTraceSource = 3221291109, // 0xC0010065
    FailFast = 3221291110, // 0xC0010066
    FailFastException = 3221291111, // 0xC0010067
    FailedToTraceEvent = 3221291112, // 0xC0010068
    FailedToTraceEventWithException = 3221291113, // 0xC0010069
  }
}
