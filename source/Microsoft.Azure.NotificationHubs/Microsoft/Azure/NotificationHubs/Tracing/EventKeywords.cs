// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.EventKeywords
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  [Flags]
  public enum EventKeywords : long
  {
    None = 0,
    WdiContext = 562949953421312, // 0x0002000000000000
    WdiDiagnostic = 1125899906842624, // 0x0004000000000000
    Sqm = 2251799813685248, // 0x0008000000000000
    AuditFailure = 4503599627370496, // 0x0010000000000000
    AuditSuccess = 9007199254740992, // 0x0020000000000000
    CorrelationHint = 18014398509481984, // 0x0040000000000000
    EventLogClassic = 36028797018963968, // 0x0080000000000000
    All = -1, // 0xFFFFFFFFFFFFFFFF
  }
}
