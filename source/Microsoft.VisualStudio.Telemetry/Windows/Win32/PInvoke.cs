// Decompiled with JetBrains decompiler
// Type: Windows.Win32.PInvoke
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using Windows.Win32.Storage.FileSystem;
using Windows.Win32.System.Threading;

namespace Windows.Win32
{
  internal static class PInvoke
  {
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("Kernel32", SetLastError = true)]
    internal static extern BOOL FreeLibrary(HINSTANCE hLibModule);

    internal static uint GetModuleFileName(SafeHandle hModule, PWSTR lpFilename, uint nSize)
    {
      bool success = false;
      try
      {
        HINSTANCE hModule1;
        if (hModule != null)
        {
          hModule.DangerousAddRef(ref success);
          hModule1 = (HINSTANCE) hModule.DangerousGetHandle();
        }
        else
          hModule1 = new HINSTANCE();
        return PInvoke.GetModuleFileName(hModule1, lpFilename, nSize);
      }
      finally
      {
        if (success)
          hModule.DangerousRelease();
      }
    }

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("Kernel32", EntryPoint = "GetModuleFileNameW", SetLastError = true)]
    internal static extern uint GetModuleFileName(HINSTANCE hModule, PWSTR lpFilename, uint nSize);

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("Kernel32", SetLastError = true)]
    internal static extern BOOL CloseHandle(HANDLE hObject);

    internal static uint GetFinalPathNameByHandle(
      SafeHandle hFile,
      PWSTR lpszFilePath,
      uint cchFilePath,
      FILE_NAME dwFlags)
    {
      bool success = false;
      try
      {
        HANDLE hFile1;
        if (hFile != null)
        {
          hFile.DangerousAddRef(ref success);
          hFile1 = (HANDLE) hFile.DangerousGetHandle();
        }
        else
          hFile1 = new HANDLE();
        return PInvoke.GetFinalPathNameByHandle(hFile1, lpszFilePath, cchFilePath, dwFlags);
      }
      finally
      {
        if (success)
          hFile.DangerousRelease();
      }
    }

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("Kernel32", EntryPoint = "GetFinalPathNameByHandleW", SetLastError = true)]
    internal static extern uint GetFinalPathNameByHandle(
      HANDLE hFile,
      PWSTR lpszFilePath,
      uint cchFilePath,
      FILE_NAME dwFlags);

    internal static unsafe BOOL GetFileInformationByHandle(
      SafeHandle hFile,
      out BY_HANDLE_FILE_INFORMATION lpFileInformation)
    {
      bool success = false;
      try
      {
        fixed (BY_HANDLE_FILE_INFORMATION* lpFileInformation1 = &lpFileInformation)
        {
          HANDLE hFile1;
          if (hFile != null)
          {
            hFile.DangerousAddRef(ref success);
            hFile1 = (HANDLE) hFile.DangerousGetHandle();
          }
          else
            hFile1 = new HANDLE();
          return PInvoke.GetFileInformationByHandle(hFile1, lpFileInformation1);
        }
      }
      finally
      {
        if (success)
          hFile.DangerousRelease();
      }
    }

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("Kernel32", SetLastError = true)]
    internal static extern unsafe BOOL GetFileInformationByHandle(
      HANDLE hFile,
      BY_HANDLE_FILE_INFORMATION* lpFileInformation);

    internal static unsafe SafeFileHandle CreateFile(
      string lpFileName,
      FILE_ACCESS_FLAGS dwDesiredAccess,
      FILE_SHARE_MODE dwShareMode,
      SECURITY_ATTRIBUTES? lpSecurityAttributes,
      FILE_CREATION_DISPOSITION dwCreationDisposition,
      FILE_FLAGS_AND_ATTRIBUTES dwFlagsAndAttributes,
      SafeHandle hTemplateFile)
    {
      bool success = false;
      try
      {
        fixed (char* lpFileName1 = lpFileName)
        {
          SECURITY_ATTRIBUTES securityAttributes = lpSecurityAttributes.HasValue ? lpSecurityAttributes.Value : new SECURITY_ATTRIBUTES();
          HANDLE hTemplateFile1;
          if (hTemplateFile != null)
          {
            hTemplateFile.DangerousAddRef(ref success);
            hTemplateFile1 = (HANDLE) hTemplateFile.DangerousGetHandle();
          }
          else
            hTemplateFile1 = new HANDLE();
          return new SafeFileHandle((IntPtr) PInvoke.CreateFile((PCWSTR) lpFileName1, dwDesiredAccess, dwShareMode, lpSecurityAttributes.HasValue ? &securityAttributes : (SECURITY_ATTRIBUTES*) null, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile1), true);
        }
      }
      finally
      {
        if (success)
          hTemplateFile.DangerousRelease();
      }
    }

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("Kernel32", EntryPoint = "CreateFileW", SetLastError = true)]
    internal static extern unsafe HANDLE CreateFile(
      PCWSTR lpFileName,
      FILE_ACCESS_FLAGS dwDesiredAccess,
      FILE_SHARE_MODE dwShareMode,
      [Optional] SECURITY_ATTRIBUTES* lpSecurityAttributes,
      FILE_CREATION_DISPOSITION dwCreationDisposition,
      FILE_FLAGS_AND_ATTRIBUTES dwFlagsAndAttributes,
      HANDLE hTemplateFile);

    internal static unsafe BOOL GetProcessInformation(
      SafeHandle hProcess,
      PROCESS_INFORMATION_CLASS ProcessInformationClass,
      void* ProcessInformation,
      uint ProcessInformationSize)
    {
      bool success = false;
      try
      {
        HANDLE hProcess1;
        if (hProcess != null)
        {
          hProcess.DangerousAddRef(ref success);
          hProcess1 = (HANDLE) hProcess.DangerousGetHandle();
        }
        else
          hProcess1 = new HANDLE();
        return PInvoke.GetProcessInformation(hProcess1, ProcessInformationClass, ProcessInformation, ProcessInformationSize);
      }
      finally
      {
        if (success)
          hProcess.DangerousRelease();
      }
    }

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("Kernel32", SetLastError = true)]
    internal static extern unsafe BOOL GetProcessInformation(
      HANDLE hProcess,
      PROCESS_INFORMATION_CLASS ProcessInformationClass,
      void* ProcessInformation,
      uint ProcessInformationSize);

    internal static SafeFileHandle GetCurrentProcess_SafeHandle() => new SafeFileHandle((IntPtr) PInvoke.GetCurrentProcess(), true);

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("Kernel32")]
    internal static extern HANDLE GetCurrentProcess();

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("Kernel32", EntryPoint = "GetCommandLineW")]
    internal static extern PWSTR GetCommandLine();

    internal static unsafe BOOL SetFileInformationByHandle(
      SafeHandle hFile,
      FILE_INFO_BY_HANDLE_CLASS FileInformationClass,
      void* lpFileInformation,
      uint dwBufferSize)
    {
      bool success = false;
      try
      {
        HANDLE hFile1;
        if (hFile != null)
        {
          hFile.DangerousAddRef(ref success);
          hFile1 = (HANDLE) hFile.DangerousGetHandle();
        }
        else
          hFile1 = new HANDLE();
        return PInvoke.SetFileInformationByHandle(hFile1, FileInformationClass, lpFileInformation, dwBufferSize);
      }
      finally
      {
        if (success)
          hFile.DangerousRelease();
      }
    }

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("Kernel32", SetLastError = true)]
    internal static extern unsafe BOOL SetFileInformationByHandle(
      HANDLE hFile,
      FILE_INFO_BY_HANDLE_CLASS FileInformationClass,
      void* lpFileInformation,
      uint dwBufferSize);

    internal static SafeFileHandle ReOpenFile(
      SafeHandle hOriginalFile,
      FILE_ACCESS_FLAGS dwDesiredAccess,
      FILE_SHARE_MODE dwShareMode,
      FILE_FLAGS_AND_ATTRIBUTES dwFlagsAndAttributes)
    {
      bool success = false;
      try
      {
        HANDLE hOriginalFile1;
        if (hOriginalFile != null)
        {
          hOriginalFile.DangerousAddRef(ref success);
          hOriginalFile1 = (HANDLE) hOriginalFile.DangerousGetHandle();
        }
        else
          hOriginalFile1 = new HANDLE();
        return new SafeFileHandle((IntPtr) PInvoke.ReOpenFile(hOriginalFile1, dwDesiredAccess, dwShareMode, dwFlagsAndAttributes), true);
      }
      finally
      {
        if (success)
          hOriginalFile.DangerousRelease();
      }
    }

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("Kernel32", SetLastError = true)]
    internal static extern HANDLE ReOpenFile(
      HANDLE hOriginalFile,
      FILE_ACCESS_FLAGS dwDesiredAccess,
      FILE_SHARE_MODE dwShareMode,
      FILE_FLAGS_AND_ATTRIBUTES dwFlagsAndAttributes);
  }
}
