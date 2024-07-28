// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.DefaultWerReportImplementation
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Telemetry.WindowsErrorReporting
{
  internal sealed class DefaultWerReportImplementation : IWerReport
  {
    public SafeWerReportHandle WerReportCreateEx(
      string pwzEventType,
      WER_REPORT_TYPE repType,
      IntPtr pReportInformation)
    {
      SafeWerReportHandle phReportHandle;
      int errorCode = NativeMethods.WerReportCreate(pwzEventType, repType, pReportInformation, out phReportHandle);
      if (errorCode != 0)
        throw Marshal.GetExceptionForHR(errorCode);
      return phReportHandle;
    }

    public int WerReportSetParameter(
      SafeWerReportHandle hReportHandle,
      int dwparamID,
      string pwzName,
      string pwzValue)
    {
      int errorCode = NativeMethods.WerReportSetParameter(hReportHandle, dwparamID, pwzName, pwzValue);
      return errorCode == 0 ? errorCode : throw Marshal.GetExceptionForHR(errorCode);
    }

    public int WerReportAddDump(
      SafeWerReportHandle hReportHandle,
      IntPtr hProcess,
      IntPtr hThread,
      WER_DUMP_TYPE dumpType,
      IntPtr pExceptionParam,
      IntPtr pDumpCustomOptions,
      int dwFlags)
    {
      int errorCode = NativeMethods.WerReportAddDump(hReportHandle, hProcess, hThread, dumpType, pExceptionParam, pDumpCustomOptions, dwFlags);
      return errorCode == 0 ? errorCode : throw Marshal.GetExceptionForHR(errorCode);
    }

    public int WerReportSubmitEx(
      SafeWerReportHandle hReportHandle,
      WER_CONSENT consent,
      int dwFlags)
    {
      WER_SUBMIT_RESULT pSubmitResult = (WER_SUBMIT_RESULT) 0;
      int errorCode = NativeMethods.WerReportSubmit(hReportHandle, consent, dwFlags, ref pSubmitResult);
      if (errorCode != 0)
        throw Marshal.GetExceptionForHR(errorCode);
      return (int) pSubmitResult;
    }

    public int WerReportAddFile(
      SafeWerReportHandle hReportHandle,
      string pwxPath,
      WER_FILE_TYPE repFileType,
      int dwFileFlags)
    {
      int errorCode = NativeMethods.WerReportAddFile(hReportHandle, pwxPath, repFileType, dwFileFlags);
      return errorCode == 0 ? errorCode : throw Marshal.GetExceptionForHR(errorCode);
    }
  }
}
