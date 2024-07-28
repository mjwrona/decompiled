// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.WindowsErrorReport
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [SecurityCritical]
  [CLSCompliant(false)]
  public class WindowsErrorReport : IDisposable
  {
    private bool isDisposed;
    private IntPtr m_reportHandle;
    private IntPtr m_reportInformation;
    public const int WER_DUMP_MASK_START = 1;
    public const int WER_DUMP_MASK_DUMPTYPE = 1;
    public const int WER_DUMP_MASK_ONLY_THISTHREAD = 2;
    public const int WER_DUMP_MASK_THREADFLAGS = 4;
    public const int WER_DUMP_MASK_THREADFLAGS_EX = 8;
    public const int WER_DUMP_MASK_OTHERTHREADFLAGS = 16;
    public const int WER_DUMP_MASK_OTHERTHREADFLAGS_EX = 32;
    public const int WER_DUMP_MASK_PREFERRED_MODULESFLAGS = 64;
    public const int WER_DUMP_MASK_OTHER_MODULESFLAGS = 128;
    public const int WER_DUMP_MASK_PREFERRED_MODULE_LIST = 256;
    public const int WER_DUMP_NOHEAP_ONQUEUE = 1;

    public void WerReportCreate(
      string eventType,
      WindowsErrorReport.WER_REPORT_TYPE repType,
      WindowsErrorReport.WER_REPORT_INFORMATION reportInformation)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(this.GetType().AssemblyQualifiedName);
      this.m_reportInformation = Marshal.AllocHGlobal(Marshal.SizeOf<WindowsErrorReport.WER_REPORT_INFORMATION>(reportInformation));
      Marshal.StructureToPtr<WindowsErrorReport.WER_REPORT_INFORMATION>(reportInformation, this.m_reportInformation, true);
      IntPtr reportHandle = new IntPtr();
      int num = WindowsErrorReport.WerInterop.WerReportCreate(eventType, repType, this.m_reportInformation, ref reportHandle);
      if (HResult.Failed(num))
        Marshal.ThrowExceptionForHR(num, new IntPtr(0));
      this.m_reportHandle = reportHandle;
    }

    public void WerReportSetParameter(int paramId, string name, string value)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(this.GetType().AssemblyQualifiedName);
      int num = WindowsErrorReport.WerInterop.WerReportSetParameter(this.m_reportHandle, (uint) paramId, name, value);
      if (!HResult.Failed(num))
        return;
      Marshal.ThrowExceptionForHR(num, new IntPtr(0));
    }

    public void WerReportAddFile(
      string path,
      WindowsErrorReport.WER_FILE_TYPE repFileType,
      WindowsErrorReport.WER_FILE_FLAGS fileFlags)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(this.GetType().AssemblyQualifiedName);
      int num = WindowsErrorReport.WerInterop.WerReportAddFile(this.m_reportHandle, path, repFileType, fileFlags);
      if (!HResult.Failed(num))
        return;
      Marshal.ThrowExceptionForHR(num, new IntPtr(0));
    }

    public WindowsErrorReport.WER_SUBMIT_RESULT WerReportSubmit(
      WindowsErrorReport.WER_CONSENT consent,
      WindowsErrorReport.WER_SUBMIT_FLAGS flags)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(this.GetType().AssemblyQualifiedName);
      WindowsErrorReport.WER_SUBMIT_RESULT submitResult;
      int num = WindowsErrorReport.WerInterop.WerReportSubmit(this.m_reportHandle, consent, flags, out submitResult);
      if (HResult.Failed(num))
        Marshal.ThrowExceptionForHR(num, new IntPtr(0));
      TeamFoundationTrace.Info("WerReportSubmit() result = " + submitResult.ToString());
      return submitResult;
    }

    private void WerReportCloseHandle()
    {
      if (!(this.m_reportHandle != IntPtr.Zero))
        return;
      int num = WindowsErrorReport.WerInterop.WerReportCloseHandle(this.m_reportHandle);
      if (HResult.Failed(num))
        Marshal.ThrowExceptionForHR(num, new IntPtr(0));
      this.m_reportHandle = IntPtr.Zero;
    }

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      try
      {
        if (this.m_reportInformation != IntPtr.Zero)
        {
          Marshal.FreeHGlobal(this.m_reportInformation);
          this.m_reportInformation = IntPtr.Zero;
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException("Error freeing report information structure", "WerUtil.Dispose()", ex);
      }
      try
      {
        this.WerReportCloseHandle();
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException("Error disposing report handle.", "WerUtil.Dispose()", ex);
      }
      finally
      {
        this.isDisposed = true;
      }
    }

    private class WerInterop
    {
      [DllImport("wer.dll")]
      public static extern int WerReportCreate(
        [MarshalAs(UnmanagedType.LPWStr), In] string pwzEventType,
        [In] WindowsErrorReport.WER_REPORT_TYPE repType,
        [In] IntPtr reportInformation,
        ref IntPtr reportHandle);

      [DllImport("wer.dll")]
      public static extern int WerReportSetParameter(
        [In] IntPtr reportHandle,
        [In] uint paramID,
        [MarshalAs(UnmanagedType.LPWStr), In] string name,
        [MarshalAs(UnmanagedType.LPWStr), In] string value);

      [DllImport("wer.dll")]
      public static extern int WerReportAddFile(
        [In] IntPtr reportHandle,
        [MarshalAs(UnmanagedType.LPWStr), In] string path,
        [In] WindowsErrorReport.WER_FILE_TYPE fileType,
        [In] WindowsErrorReport.WER_FILE_FLAGS fileFlags);

      [DllImport("wer.dll")]
      public static extern int WerReportSubmit(
        [In] IntPtr reportHandle,
        [In] WindowsErrorReport.WER_CONSENT consent,
        [In] WindowsErrorReport.WER_SUBMIT_FLAGS submitFlags,
        out WindowsErrorReport.WER_SUBMIT_RESULT submitResult);

      [DllImport("wer.dll")]
      public static extern int WerReportAddDump(
        [In] IntPtr reportHandle,
        [In] IntPtr processHandle,
        [In] IntPtr threadHandle,
        [In] WindowsErrorReport.WER_DUMP_TYPE dumpType,
        [In] IntPtr exceptionParam,
        [In] IntPtr dumpCustomOptions,
        [In] uint flags);

      [DllImport("wer.dll")]
      public static extern int WerReportCloseHandle([In] IntPtr reportHandle);
    }

    [Flags]
    public enum WER_FILE_FLAGS
    {
      WER_FILE_DELETE_WHEN_DONE = 1,
      WER_FILE_ANONYMOUS_DATA = 2,
    }

    [Flags]
    public enum WER_SUBMIT_FLAGS
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

    public enum WER_REPORT_UI
    {
      WerUIAdditionalDataDlgHeader = 1,
      WerUIIconFilePath = 2,
      WerUIConsentDlgHeader = 3,
      WerUIConsentDlgBody = 4,
      WerUIOnlineSolutionCheckText = 5,
      WerUIOfflineSolutionCheckText = 6,
      WerUICloseText = 7,
      WerUICloseDlgHeader = 8,
      WerUICloseDlgBody = 9,
      WerUICloseDlgButtonText = 10, // 0x0000000A
      WerUICustomActionButtonText = 11, // 0x0000000B
      WerUIMax = 12, // 0x0000000C
    }

    public enum WER_REGISTER_FILE_TYPE
    {
      WerRegFileTypeUserDocument = 1,
      WerRegFileTypeOther = 2,
      WerRegFileTypeMax = 3,
    }

    public enum WER_FILE_TYPE
    {
      WerFileTypeMicrodump = 1,
      WerFileTypeMinidump = 2,
      WerFileTypeHeapdump = 3,
      WerFileTypeUserDocument = 4,
      WerFileTypeOther = 5,
      WerFileTypeMax = 6,
    }

    public enum WER_SUBMIT_RESULT
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
    }

    public enum WER_REPORT_TYPE
    {
      WerReportNonCritical,
      WerReportCritical,
      WerReportApplicationCrash,
      WerReportApplicationHang,
      WerReportKernel,
      WerReportInvalid,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WER_REPORT_INFORMATION
    {
      public uint dwSize;
      public IntPtr hProcess;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string wzConsentKey;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      public string wzFriendlyEventName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      public string wzApplicationName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string wzApplicationPath;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
      public string wzDescription;
      public IntPtr hwndParent;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WER_DUMP_CUSTOM_OPTIONS
    {
      public uint dwSize;
      public uint dwMask;
      public uint dwDumpFlags;
      [MarshalAs(UnmanagedType.Bool)]
      public bool bOnlyThisThread;
      public uint dwExceptionThreadFlags;
      public uint dwOtherThreadFlags;
      public uint dwExceptionThreadExFlags;
      public uint dwOtherThreadExFlags;
      public uint dwPreferredModuleFlags;
      public uint dwOtherModuleFlags;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
      public string wzPreferredModuleList;
    }

    public enum WER_CONSENT
    {
      WerConsentNotAsked = 1,
      WerConsentApproved = 2,
      WerConsentDenied = 3,
      WerConsentAlwaysPrompt = 4,
      WerConsentMax = 5,
    }

    public enum WER_DUMP_TYPE
    {
      WerDumpTypeMicroDump = 1,
      WerDumpTypeMiniDump = 2,
      WerDumpTypeHeapDump = 3,
      WerDumpTypeMax = 4,
    }
  }
}
