// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.IWerReport
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;

namespace Microsoft.VisualStudio.Telemetry.WindowsErrorReporting
{
  internal interface IWerReport
  {
    SafeWerReportHandle WerReportCreateEx(
      string pwzEventType,
      WER_REPORT_TYPE repType,
      IntPtr pReportInformation);

    int WerReportSetParameter(
      SafeWerReportHandle hReportHandle,
      int dwparamID,
      string pwzName,
      string pwzValue);

    int WerReportAddDump(
      SafeWerReportHandle hReportHandle,
      IntPtr hProcess,
      IntPtr hThread,
      WER_DUMP_TYPE dumpType,
      IntPtr pExceptionParam,
      IntPtr pDumpCustomOptions,
      int dwFlags);

    int WerReportAddFile(
      SafeWerReportHandle hReportHandle,
      string pwxPath,
      WER_FILE_TYPE repFileType,
      int dwFileFlags);

    int WerReportSubmitEx(SafeWerReportHandle hReportHandle, WER_CONSENT consent, int dwFlags);
  }
}
