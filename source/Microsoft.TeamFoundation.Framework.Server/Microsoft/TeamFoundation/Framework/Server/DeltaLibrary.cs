// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DeltaLibrary
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DeltaLibrary
  {
    private const int HashAlgIdCRC32 = 32;
    internal const uint ERROR_PATCH_NOT_NECESSARY = 3222159621;
    private const string s_Area = "FileService";
    private const string s_Layer = "PatchLibrary";
    private const int c_maxDeltaSize = 4194304;

    [DllImport("msdelta.dll", SetLastError = true)]
    internal static extern bool CreateDeltaB(
      DELTA_FILE_TYPE FileTypeSet,
      DELTA_FLAG_TYPE SetFlags,
      DELTA_FLAG_TYPE ResetFlags,
      DELTA_INPUT Source,
      DELTA_INPUT Target,
      DELTA_INPUT SourceOptions,
      DELTA_INPUT TargetOptions,
      DELTA_INPUT GlobalOptions,
      IntPtr lpTargetFileTime,
      int HashAlgId,
      out DELTA_OUTPUT lpDelta);

    [DllImport("msdelta.dll", SetLastError = true)]
    internal static extern bool ApplyDeltaB(
      DELTA_FLAG_TYPE ApplyFlags,
      DELTA_INPUT Source,
      DELTA_INPUT Delta,
      out DELTA_OUTPUT lpTarget);

    [DllImport("msdelta.dll", SetLastError = true)]
    internal static extern bool DeltaFree(IntPtr lpMemory);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool WriteFile(
      [In] SafeFileHandle hFile,
      [In] IntPtr lpBuffer,
      [In] uint nNumberOfBytesToWrite,
      out uint lpNumberOfBytesWritten,
      IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool ReadFile(
      [In] SafeFileHandle hFile,
      [Out] SafeHandle lpBuffer,
      [In] uint nNumberOfBytesToRead,
      out uint lpNumberOfBytesRead,
      IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool GetFileInformationByHandle(
      [In] SafeFileHandle hFile,
      out BY_HANDLE_FILE_INFORMATION lpFileInformation);

    public static int CreateFile(
      SafeFileHandle oldHandle,
      SafeFileHandle newHandle,
      SafeFileHandle patchHandle)
    {
      uint lpNumberOfBytesWritten = 0;
      DELTA_OUTPUT lpDelta = new DELTA_OUTPUT();
      lpDelta.lpcStart = IntPtr.Zero;
      lpDelta.uSize = UIntPtr.Zero;
      TeamFoundationTracingService.TraceEnterRaw(0, "FileService", "PatchLibrary", nameof (CreateFile), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        BY_HANDLE_FILE_INFORMATION lpFileInformation1;
        if (!DeltaLibrary.GetFileInformationByHandle(oldHandle, out lpFileInformation1))
        {
          int lastWin32Error = Marshal.GetLastWin32Error();
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileService", "PatchLibrary", "Failed to get file info on oldFile with error {0}", (object) lastWin32Error);
          throw new Win32Exception(lastWin32Error);
        }
        BY_HANDLE_FILE_INFORMATION lpFileInformation2;
        if (!DeltaLibrary.GetFileInformationByHandle(newHandle, out lpFileInformation2))
        {
          int lastWin32Error = Marshal.GetLastWin32Error();
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileService", "PatchLibrary", "Failed to get file info on newFile with error {0}", (object) lastWin32Error);
          throw new Win32Exception(lastWin32Error);
        }
        if (lpFileInformation1.nFileSizeHigh > 0U || lpFileInformation1.nFileSizeLow > 4194304U)
        {
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileService", "PatchLibrary", "oldFile is {0} which is bigger than the max deltafiable size of {1}", (object) lpFileInformation1.nFileSizeLow, (object) 4194304);
          throw new ArgumentOutOfRangeException("oldInfo");
        }
        if (lpFileInformation2.nFileSizeHigh > 0U || lpFileInformation2.nFileSizeLow > 4194304U)
        {
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileService", "PatchLibrary", "newFile is {0} which is bigger than the max deltafiable size of {1}", (object) lpFileInformation2.nFileSizeLow, (object) 4194304);
          throw new ArgumentOutOfRangeException("newInfo");
        }
        using (SafeGlobalMemoryHandle lpBuffer1 = new SafeGlobalMemoryHandle(lpFileInformation1.nFileSizeLow))
        {
          using (SafeGlobalMemoryHandle lpBuffer2 = new SafeGlobalMemoryHandle(lpFileInformation2.nFileSizeLow))
          {
            if (!DeltaLibrary.ReadFile(oldHandle, (SafeHandle) lpBuffer1, lpFileInformation1.nFileSizeLow, out uint _, IntPtr.Zero))
            {
              int lastWin32Error = Marshal.GetLastWin32Error();
              TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileService", "PatchLibrary", "Failed to read file oldFile with error {0}", (object) lastWin32Error);
              throw new Win32Exception(lastWin32Error);
            }
            if (!DeltaLibrary.ReadFile(newHandle, (SafeHandle) lpBuffer2, lpFileInformation2.nFileSizeLow, out uint _, IntPtr.Zero))
            {
              int lastWin32Error = Marshal.GetLastWin32Error();
              TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileService", "PatchLibrary", "Failed to read file newFile with error {0}", (object) lastWin32Error);
              throw new Win32Exception(lastWin32Error);
            }
            DELTA_INPUT Source = new DELTA_INPUT();
            Source.Editable = true;
            Source.lpcStart = lpBuffer1;
            Source.uSize = new UIntPtr(lpFileInformation1.nFileSizeLow);
            DELTA_INPUT Target = new DELTA_INPUT();
            Target.Editable = true;
            Target.lpcStart = lpBuffer2;
            Target.uSize = new UIntPtr(lpFileInformation2.nFileSizeLow);
            DELTA_INPUT deltaInput = new DELTA_INPUT();
            deltaInput.uSize = UIntPtr.Zero;
            deltaInput.Editable = false;
            deltaInput.lpcStart = new SafeGlobalMemoryHandle();
            if (!DeltaLibrary.CreateDeltaB(DELTA_FILE_TYPE.RAW, DELTA_FLAG_TYPE.NONE, DELTA_FLAG_TYPE.NONE, Source, Target, deltaInput, deltaInput, deltaInput, IntPtr.Zero, 32, out lpDelta))
            {
              int lastWin32Error = Marshal.GetLastWin32Error();
              TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileService", "PatchLibrary", "Failed to Create Delta error:{0}", (object) lastWin32Error);
              throw new Win32Exception(lastWin32Error);
            }
            int uint32 = (int) lpDelta.uSize.ToUInt32();
            if (!DeltaLibrary.WriteFile(patchHandle, lpDelta.lpcStart, lpDelta.uSize.ToUInt32(), out lpNumberOfBytesWritten, IntPtr.Zero))
            {
              int lastWin32Error = Marshal.GetLastWin32Error();
              TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileService", "PatchLibrary", "Failed to Write Patch File with error:{0}", (object) lastWin32Error);
              throw new Win32Exception(lastWin32Error);
            }
            return 0;
          }
        }
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(0, "FileService", "PatchLibrary", nameof (CreateFile));
        if (lpDelta.lpcStart != IntPtr.Zero)
          DeltaLibrary.DeltaFree(lpDelta.lpcStart);
      }
    }

    public static int ApplyPatch(
      byte[] fullContent,
      int fullContentLength,
      byte[] patch,
      int patchLength,
      byte[] result)
    {
      DELTA_INPUT Source = new DELTA_INPUT();
      DELTA_INPUT Delta = new DELTA_INPUT();
      DELTA_OUTPUT lpTarget = new DELTA_OUTPUT();
      try
      {
        using (SafeGlobalMemoryHandle globalMemoryHandle1 = new SafeGlobalMemoryHandle((uint) fullContentLength))
        {
          using (SafeGlobalMemoryHandle globalMemoryHandle2 = new SafeGlobalMemoryHandle((uint) patchLength))
          {
            Marshal.Copy(fullContent, 0, globalMemoryHandle1.DangerousGetHandle(), fullContentLength);
            Source.Editable = true;
            Source.lpcStart = globalMemoryHandle1;
            Source.uSize = new UIntPtr((uint) fullContentLength);
            Marshal.Copy(patch, 0, globalMemoryHandle2.DangerousGetHandle(), patchLength);
            Delta.Editable = true;
            Delta.lpcStart = globalMemoryHandle2;
            Delta.uSize = new UIntPtr((uint) patchLength);
            lpTarget.lpcStart = IntPtr.Zero;
            lpTarget.uSize = UIntPtr.Zero;
            if (!DeltaLibrary.ApplyDeltaB(DELTA_FLAG_TYPE.APPLY_FLAG_ALLOW_PA19, Source, Delta, out lpTarget))
            {
              int lastWin32Error = Marshal.GetLastWin32Error();
              if (lastWin32Error == -1072807675)
              {
                if (result.Length != fullContentLength)
                  throw new ArgumentOutOfRangeException("result.Length");
                Array.Copy((Array) fullContent, (Array) result, result.Length);
                return result.Length;
              }
              TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileService", "PatchLibrary", "Failed to ApplyDelta with error {0}", (object) lastWin32Error);
              throw new Win32Exception(lastWin32Error);
            }
            Marshal.Copy(lpTarget.lpcStart, result, 0, (int) (uint) lpTarget.uSize);
            return (int) (uint) lpTarget.uSize;
          }
        }
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(0, "FileService", "PatchLibrary", nameof (ApplyPatch));
        if (lpTarget.lpcStart != IntPtr.Zero)
        {
          DeltaLibrary.DeltaFree(lpTarget.lpcStart);
          lpTarget.lpcStart = IntPtr.Zero;
        }
      }
    }

    public static void ApplyHandles(
      SafeFileHandle oldHandle,
      SafeFileHandle patchHandle,
      SafeFileHandle newHandle)
    {
      DELTA_INPUT Source = new DELTA_INPUT();
      DELTA_INPUT Delta = new DELTA_INPUT();
      DELTA_OUTPUT lpTarget = new DELTA_OUTPUT();
      uint lpNumberOfBytesWritten = 0;
      TeamFoundationTracingService.TraceEnterRaw(0, "FileService", "PatchLibrary", nameof (ApplyHandles), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        BY_HANDLE_FILE_INFORMATION lpFileInformation1;
        DeltaLibrary.GetFileInformationByHandle(oldHandle, out lpFileInformation1);
        BY_HANDLE_FILE_INFORMATION lpFileInformation2;
        DeltaLibrary.GetFileInformationByHandle(patchHandle, out lpFileInformation2);
        using (SafeGlobalMemoryHandle lpBuffer1 = new SafeGlobalMemoryHandle(lpFileInformation1.nFileSizeLow))
        {
          using (SafeGlobalMemoryHandle lpBuffer2 = new SafeGlobalMemoryHandle(lpFileInformation2.nFileSizeLow))
          {
            uint lpNumberOfBytesRead;
            if (!DeltaLibrary.ReadFile(oldHandle, (SafeHandle) lpBuffer1, lpFileInformation1.nFileSizeLow, out lpNumberOfBytesRead, IntPtr.Zero))
            {
              int lastWin32Error = Marshal.GetLastWin32Error();
              TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileService", "PatchLibrary", "Failed to ReadFile on oldFile with error {0}", (object) lastWin32Error);
              throw new Win32Exception(lastWin32Error);
            }
            if (!DeltaLibrary.ReadFile(patchHandle, (SafeHandle) lpBuffer2, lpFileInformation2.nFileSizeLow, out lpNumberOfBytesRead, IntPtr.Zero))
            {
              int lastWin32Error = Marshal.GetLastWin32Error();
              TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileService", "PatchLibrary", "Failed to ReadFile on patchFile with error {0}", (object) lastWin32Error);
              throw new Win32Exception(lastWin32Error);
            }
            Source.Editable = true;
            Source.lpcStart = lpBuffer1;
            Source.uSize = new UIntPtr(lpFileInformation1.nFileSizeLow);
            Delta.Editable = true;
            Delta.lpcStart = lpBuffer2;
            Delta.uSize = new UIntPtr(lpFileInformation2.nFileSizeLow);
            lpTarget.lpcStart = IntPtr.Zero;
            lpTarget.uSize = UIntPtr.Zero;
            if (!DeltaLibrary.ApplyDeltaB(DELTA_FLAG_TYPE.APPLY_FLAG_ALLOW_PA19, Source, Delta, out lpTarget))
            {
              int lastWin32Error = Marshal.GetLastWin32Error();
              TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileService", "PatchLibrary", "Failed to ApplyDelta with error {0}", (object) lastWin32Error);
              throw new Win32Exception(lastWin32Error);
            }
            if (!DeltaLibrary.WriteFile(newHandle, lpTarget.lpcStart, lpTarget.uSize.ToUInt32(), out lpNumberOfBytesWritten, IntPtr.Zero))
            {
              int lastWin32Error = Marshal.GetLastWin32Error();
              TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileService", "PatchLibrary", "Failed to Write target file with error {0}", (object) lastWin32Error);
              throw new Win32Exception(lastWin32Error);
            }
          }
        }
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(0, "FileService", "PatchLibrary", nameof (ApplyHandles));
        if (lpTarget.lpcStart != IntPtr.Zero)
          DeltaLibrary.DeltaFree(lpTarget.lpcStart);
      }
    }
  }
}
