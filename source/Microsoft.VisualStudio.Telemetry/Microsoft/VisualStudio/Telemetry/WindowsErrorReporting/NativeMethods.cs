// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.NativeMethods
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Telemetry.WindowsErrorReporting
{
  internal static class NativeMethods
  {
    internal const uint WER_E_LENGTH_EXCEEDED = 2147943683;
    public const int WER_BUCKETPARAM_MAXLENGTH = 256;

    [DllImport("wer.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int WerReportCreate(
      string pwzEventType,
      WER_REPORT_TYPE repType,
      IntPtr pReportInformation,
      out SafeWerReportHandle phReportHandle);

    [DllImport("wer.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int WerReportSetParameter(
      SafeWerReportHandle hReportHandle,
      int dwparamID,
      string pwzName,
      string pwzValue);

    [DllImport("wer.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int WerReportSetUIOption(
      SafeWerReportHandle hReportHandle,
      WER_REPORT_UI repUITypeID,
      string pwzValue);

    [DllImport("wer.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int WerReportAddDump(
      SafeWerReportHandle hReportHandle,
      IntPtr hProcess,
      IntPtr hThread,
      WER_DUMP_TYPE dumpType,
      IntPtr pExceptionParam,
      IntPtr pDumpCustomOptions,
      int dwFlags);

    [DllImport("wer.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int WerReportSubmit(
      SafeWerReportHandle hReportHandle,
      WER_CONSENT consent,
      int dwFlags,
      ref WER_SUBMIT_RESULT pSubmitResult);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetCurrentProcess();

    [DllImport("wer.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int WerReportAddFile(
      SafeWerReportHandle hReportHandle,
      string pwxPath,
      WER_FILE_TYPE repFileType,
      int dwFileFlags);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern int WaitForMultipleObjects(
      uint handleCount,
      IntPtr[] waitHandles,
      [MarshalAs(UnmanagedType.Bool)] bool waitAll,
      uint millisecondsTimeout);

    [Flags]
    public enum WERSubmit
    {
      WER_SUBMIT_HONOR_RECOVERY = 1,
      WER_SUBMIT_HONOR_RESTART = 2,
      WER_SUBMIT_QUEUE = 4,
      WER_SUBMIT_SHOW_DEBUG = 8,
      WER_SUBMIT_ADD_REGISTERED_DATA = 16, // 0x00000010
      WER_SUBMIT_OUTOFPROCESS = 32, // 0x00000020
      WER_SUBMIT_NO_CLOSE_UI = 64, // 0x00000040
      WER_SUBMIT_NO_QUEUE = 128, // 0x00000080
      WER_SUBMIT_NO_ARCHIVE = 256, // 0x00000100
      WER_SUBMIT_START_MINIMIZED = 512, // 0x00000200
      WER_SUBMIT_OUTOFPROCESS_ASYNC = 1024, // 0x00000400
      WER_SUBMIT_BYPASS_DATA_THROTTLING = 2048, // 0x00000800
      WER_SUBMIT_ARCHIVE_PARAMETERS_ONLY = 4096, // 0x00001000
      WER_SUBMIT_REPORT_MACHINE_ID = 8192, // 0x00002000
    }
  }
}
