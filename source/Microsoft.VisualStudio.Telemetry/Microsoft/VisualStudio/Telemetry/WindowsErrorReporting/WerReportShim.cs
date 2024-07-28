// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.WerReportShim
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;

namespace Microsoft.VisualStudio.Telemetry.WindowsErrorReporting
{
  internal class WerReportShim
  {
    private static IWerReport implementation = (IWerReport) new DefaultWerReportImplementation();

    internal static void SetImplementation(IWerReport implementation)
    {
      if (implementation == null)
        WerReportShim.implementation = (IWerReport) new DefaultWerReportImplementation();
      else
        WerReportShim.implementation = implementation;
    }

    public static SafeWerReportHandle WerReportCreate(
      string pwzEventType,
      WER_REPORT_TYPE repType,
      IntPtr pReportInformation)
    {
      SafeWerReportHandle safeWerReportHandle = (SafeWerReportHandle) null;
      try
      {
        safeWerReportHandle = WerReportShim.implementation.WerReportCreateEx(pwzEventType, repType, pReportInformation);
      }
      catch (Exception ex)
      {
      }
      return safeWerReportHandle ?? new SafeWerReportHandle();
    }

    public static int WerReportSetParameter(
      SafeWerReportHandle hReportHandle,
      int dwparamID,
      string pwzName,
      string pwzValue)
    {
      return WerReportShim.implementation.WerReportSetParameter(hReportHandle, dwparamID, pwzName, pwzValue);
    }

    public static int WerReportAddDump(
      SafeWerReportHandle hReportHandle,
      IntPtr hProcess,
      IntPtr hThread,
      WER_DUMP_TYPE dumpType,
      IntPtr pExceptionParam,
      IntPtr pDumpCustomOptions,
      int dwFlags)
    {
      return WerReportShim.implementation.WerReportAddDump(hReportHandle, hProcess, hThread, dumpType, pExceptionParam, pDumpCustomOptions, dwFlags);
    }

    public static int WerReportAddFile(
      SafeWerReportHandle hReportHandle,
      string pwxPath,
      WER_FILE_TYPE repFileType,
      int dwFileFlags)
    {
      return WerReportShim.implementation.WerReportAddFile(hReportHandle, pwxPath, repFileType, dwFileFlags);
    }

    public static int WerReportSubmit(
      SafeWerReportHandle hReportHandle,
      WER_CONSENT consent,
      int dwFlags)
    {
      return WerReportShim.implementation.WerReportSubmitEx(hReportHandle, consent, dwFlags);
    }
  }
}
