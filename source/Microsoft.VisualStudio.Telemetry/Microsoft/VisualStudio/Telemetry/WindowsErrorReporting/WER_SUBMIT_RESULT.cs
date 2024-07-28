// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.WER_SUBMIT_RESULT
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.Telemetry.WindowsErrorReporting
{
  internal enum WER_SUBMIT_RESULT
  {
    WerReportQueued = 1,
    WerReportUploaded = 2,
    WerReportDebug = 3,
    WerReportFailed = 4,
    WerDisabled = 5,
    WerReportCancelled = 6,
    WerDisabledQueue = 7,
    WerReportAsync = 8,
    WerCustomAction = 9,
    WerThrottled = 10, // 0x0000000A
  }
}
