// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Services.ReparsePointAware
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using Windows.Win32.Storage.FileSystem;

namespace Microsoft.VisualStudio.Telemetry.Services
{
  internal static class ReparsePointAware
  {
    private const string DevicePathPrefix = "\\\\?\\";

    public static void RequireSamePath(SafeFileHandle handle, string expectedPath)
    {
      if (Platform.IsWindows && ReparsePointAware.HasReparsePoints(handle, expectedPath))
        throw new UnauthorizedAccessException();
    }

    public static void RequireNoReparsePoints(string expectedPath, bool asDirectory)
    {
      if (!Platform.IsWindows)
        return;
      using (ReparsePointAware.PinAndRequireNoReparsePoints(expectedPath, asDirectory))
        ;
    }

    public static void DeleteFile(string path)
    {
      if (!Platform.IsWindows)
      {
        File.Delete(path);
      }
      else
      {
        using (ReparsePointAware.PinAndRequireNoReparsePoints(System.IO.Path.GetDirectoryName(path), true))
        {
          FILE_ACCESS_FLAGS dwDesiredAccess = (FILE_ACCESS_FLAGS) 65536;
          FILE_SHARE_MODE dwShareMode = FILE_SHARE_MODE.FILE_SHARE_NONE;
          FILE_CREATION_DISPOSITION dwCreationDisposition = FILE_CREATION_DISPOSITION.OPEN_EXISTING;
          FILE_FLAGS_AND_ATTRIBUTES dwFlagsAndAttributes = FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_OVERLAPPED;
          using (SafeFileHandle fileStub = ReparsePointAware.UnsafeCreateFileStub(path, dwDesiredAccess, dwShareMode, new SECURITY_ATTRIBUTES?(), dwCreationDisposition, dwFlagsAndAttributes, (SafeHandle) null))
          {
            ReparsePointAware.RequireSamePath(fileStub, path);
            ReparsePointAware.SetDeleteOnClose((SafeHandle) fileStub);
          }
        }
      }
    }

    public static void MoveFile(string sourceFileName, string destFileName)
    {
      if (!Platform.IsWindows)
      {
        File.Move(sourceFileName, destFileName);
      }
      else
      {
        using (ReparsePointAware.PinAndRequireNoReparsePoints(System.IO.Path.GetDirectoryName(sourceFileName), true))
        {
          using (ReparsePointAware.PinAndRequireNoReparsePoints(System.IO.Path.GetDirectoryName(destFileName), true))
          {
            FILE_ACCESS_FLAGS dwDesiredAccess = FileAccess.ReadWrite.ToFILE_ACCESS_FLAGS() | (FILE_ACCESS_FLAGS) 65536;
            using (SafeFileHandle fileStub = ReparsePointAware.UnsafeCreateFileStub(sourceFileName, dwDesiredAccess, FileShare.None.ToFILE_SHARE_MODE(), new SECURITY_ATTRIBUTES?(), FILE_CREATION_DISPOSITION.OPEN_EXISTING, FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL, (SafeHandle) null))
            {
              ReparsePointAware.RequireSamePath(fileStub, sourceFileName);
              if (!ReparsePointAware.RenameFileByHandle((SafeHandle) fileStub, destFileName))
                throw new UnauthorizedAccessException();
            }
          }
        }
      }
    }

    public static void WriteAllText(string path, string contents)
    {
      if (!Platform.IsWindows)
      {
        File.WriteAllText(path, contents);
      }
      else
      {
        using (FileStream fileStream = ReparsePointAware.OpenFile(path, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
          using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream))
          {
            fileStream.SetLength(0L);
            streamWriter.Write(contents);
          }
        }
      }
    }

    public static StreamWriter CreateText(string path) => !Platform.IsWindows ? File.CreateText(path) : new StreamWriter((Stream) ReparsePointAware.OpenFile(path, FileMode.Create, FileAccess.Write, FileShare.Read));

    public static FileStream OpenWrite(string path) => !Platform.IsWindows ? File.OpenWrite(path) : ReparsePointAware.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

    public static FileStream OpenRead(string path) => !Platform.IsWindows ? File.OpenRead(path) : ReparsePointAware.OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read);

    public static FileStream OpenFile(
      string path,
      FileMode mode,
      FileAccess access,
      FileShare share = FileShare.None)
    {
      if (!Platform.IsWindows)
        return File.Open(path, mode, access, share);
      using (ReparsePointAware.PinAndRequireNoReparsePoints(System.IO.Path.GetDirectoryName(path), true))
      {
        switch (mode)
        {
          case FileMode.CreateNew:
            return new FileStream(ReparsePointAware.SafeCreateNewFile(path, access, share), access);
          case FileMode.Create:
            FileStream orOpenFile = ReparsePointAware.SafeCreateOrOpenFile(path, access, share);
            orOpenFile.SetLength(0L);
            return orOpenFile;
          case FileMode.Open:
            return new FileStream(ReparsePointAware.SafeOpenExistingFile(path, access, share), access);
          case FileMode.OpenOrCreate:
            return ReparsePointAware.SafeCreateOrOpenFile(path, access, share);
          default:
            throw new NotImplementedException();
        }
      }
    }

    public static void SetFileAttributeNormal(string path)
    {
      if (!Platform.IsWindows)
        File.SetAttributes(path, FileAttributes.Normal);
      else
        ReparsePointAware.SafeSetFileAttributesNormal(path);
    }

    public static DirectoryInfo CreateDirectory(string path)
    {
      if (!Platform.IsWindows)
        return Directory.CreateDirectory(path);
      string fullPath = System.IO.Path.GetFullPath(path);
      if (Directory.Exists(fullPath))
      {
        using (ReparsePointAware.PinAndRequireNoReparsePoints(fullPath, true))
          return new DirectoryInfo(fullPath);
      }
      else
      {
        Stack<string> source = new Stack<string>();
        string str1 = fullPath;
        string pathRoot = System.IO.Path.GetPathRoot(fullPath);
        do
        {
          string directoryName = System.IO.Path.GetDirectoryName(str1);
          source.Push(str1.Substring(directoryName.Length).Trim('\\'));
          str1 = directoryName;
        }
        while (!Directory.Exists(str1) && !str1.Equals(pathRoot, StringComparison.OrdinalIgnoreCase));
        SafeFileHandle safeFileHandle1 = ReparsePointAware.PinAndRequireNoReparsePoints(str1, true);
        try
        {
          while (source.Any<string>())
          {
            string str2 = System.IO.Path.Combine(str1, source.Pop());
            SafeFileHandle directoryInternal = ReparsePointAware.UnsafeCreateDirectoryInternal(str2);
            if (directoryInternal.IsInvalid || ReparsePointAware.HasReparsePoints(directoryInternal, str2))
            {
              directoryInternal.Dispose();
              throw new UnauthorizedAccessException();
            }
            SafeFileHandle safeFileHandle2 = safeFileHandle1;
            safeFileHandle1 = directoryInternal;
            safeFileHandle2.Dispose();
            str1 = str2;
          }
          return new DirectoryInfo(str1);
        }
        finally
        {
          safeFileHandle1.Dispose();
        }
      }
    }

    public static SafeFileHandle PinAndRequireNoReparsePoints(string expectedPath, bool asDirectory)
    {
      if (!Platform.IsWindows)
        return (SafeFileHandle) null;
      SafeFileHandle safeHandle = ReparsePointAware.PinHandle(expectedPath, asDirectory);
      if (ReparsePointAware.HasReparsePoints(safeHandle, expectedPath))
      {
        safeHandle.Dispose();
        throw new UnauthorizedAccessException();
      }
      return safeHandle;
    }

    public static SafeFileHandle PinHandle(string fullPath, bool asDirectory)
    {
      if (!Platform.IsWindows)
        throw new PlatformNotSupportedException();
      FILE_ACCESS_FLAGS dwDesiredAccess = asDirectory ? FILE_ACCESS_FLAGS.FILE_READ_DATA : FILE_ACCESS_FLAGS.FILE_GENERIC_WRITE;
      FILE_SHARE_MODE dwShareMode = FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE;
      FILE_CREATION_DISPOSITION dwCreationDisposition = FILE_CREATION_DISPOSITION.OPEN_EXISTING;
      FILE_FLAGS_AND_ATTRIBUTES dwFlagsAndAttributes = asDirectory ? FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_BACKUP_SEMANTICS : FILE_FLAGS_AND_ATTRIBUTES.SECURITY_ANONYMOUS;
      SafeFileHandle file = PInvoke.CreateFile(fullPath, dwDesiredAccess, dwShareMode, new SECURITY_ATTRIBUTES?(), dwCreationDisposition, dwFlagsAndAttributes, (SafeHandle) null);
      return !file.IsInvalid ? file : throw new UnauthorizedAccessException();
    }

    public static unsafe string GetFinalPath(SafeFileHandle handle)
    {
      if (!Platform.IsWindows)
        throw new PlatformNotSupportedException();
      uint pathNameByHandle1 = PInvoke.GetFinalPathNameByHandle((SafeHandle) handle, (PWSTR) (char*) null, 0U, FILE_NAME.FILE_NAME_NORMALIZED);
      ReparsePointAware.ThrowIfFalse(pathNameByHandle1 > 0U);
      char[] chArray = new char[(int) pathNameByHandle1];
      fixed (char* chPtr = chArray)
      {
        PWSTR lpszFilePath = new PWSTR(chPtr);
        uint pathNameByHandle2 = PInvoke.GetFinalPathNameByHandle((SafeHandle) handle, lpszFilePath, pathNameByHandle1, FILE_NAME.FILE_NAME_NORMALIZED);
        ReparsePointAware.ThrowIfFalse(pathNameByHandle2 > 0U);
        ReparsePointAware.ThrowIfFalse((long) (pathNameByHandle2 + 1U) <= (long) chArray.Length);
        string str = lpszFilePath.ToString();
        return !str.StartsWith("\\\\?\\", StringComparison.OrdinalIgnoreCase) ? str : str.Substring("\\\\?\\".Length);
      }
    }

    public static unsafe bool HasReparsePoints(SafeFileHandle safeHandle, string expectedPath)
    {
      if (!Platform.IsWindows)
        return false;
      try
      {
        int cchFilePath = expectedPath.Length + "\\\\?\\".Length + 1;
        char[] chArray = new char[cchFilePath];
        fixed (char* chPtr = chArray)
        {
          PWSTR lpszFilePath = new PWSTR(chPtr);
          uint pathNameByHandle = PInvoke.GetFinalPathNameByHandle((SafeHandle) safeHandle, lpszFilePath, (uint) cchFilePath, FILE_NAME.FILE_NAME_NORMALIZED);
          if (pathNameByHandle <= 0U || (long) pathNameByHandle >= (long) cchFilePath)
            return true;
          string str1 = lpszFilePath.ToString();
          if (!"\\\\?\\".Equals(str1.Substring(0, "\\\\?\\".Length)))
            return true;
          string str2 = str1.Substring("\\\\?\\".Length);
          BY_HANDLE_FILE_INFORMATION lpFileInformation;
          if (!expectedPath.Equals(str2, StringComparison.OrdinalIgnoreCase) && !expectedPath.Equals(str1, StringComparison.OrdinalIgnoreCase) || !(bool) PInvoke.GetFileInformationByHandle((SafeHandle) safeHandle, out lpFileInformation))
            return true;
          if (lpFileInformation.nNumberOfLinks != 1U)
            return true;
        }
      }
      catch
      {
        return true;
      }
      return false;
    }

    private static void ThrowIfFalse(bool condition)
    {
      if (!condition)
        throw new UnauthorizedAccessException();
    }

    private static unsafe void SetDeleteOnClose(SafeHandle handle)
    {
      if (!(bool) PInvoke.SetFileInformationByHandle(handle, FILE_INFO_BY_HANDLE_CLASS.FileDispositionInfo, (void*) &new FILE_DISPOSITION_INFO()
      {
        DeleteFileA = (BOOLEAN) (byte) 1
      }, (uint) sizeof (FILE_DISPOSITION_INFO)))
        throw new Win32Exception();
    }

    private static unsafe bool RenameFileByHandle(SafeHandle handle, string targetFileName)
    {
      int num1 = targetFileName.Length * 2;
      int num2 = sizeof (Microsoft.VisualStudio.Telemetry.NativeMethods.FILE_RENAME_INFO) + (num1 + 2);
      Microsoft.VisualStudio.Telemetry.NativeMethods.FILE_RENAME_INFO* fileRenameInfoPtr = (Microsoft.VisualStudio.Telemetry.NativeMethods.FILE_RENAME_INFO*) (void*) Marshal.AllocCoTaskMem(num2);
      try
      {
        byte* numPtr = (byte*) fileRenameInfoPtr;
        for (int index = 0; index < num2; ++index)
          numPtr[index] = (byte) 0;
        fileRenameInfoPtr->Anonymous.Flags = Microsoft.VisualStudio.Telemetry.NativeMethods.FILE_RENAME_REPLACE_IF_EXISTS | Microsoft.VisualStudio.Telemetry.NativeMethods.FILE_RENAME_POSIX_SEMANTICS | Microsoft.VisualStudio.Telemetry.NativeMethods.FILE_RENAME_IGNORE_READONLY_ATTRIBUTE;
        fileRenameInfoPtr->RootDirectory = IntPtr.Zero;
        fileRenameInfoPtr->FileNameLength = (uint) num1;
        char* chPtr = (char*) &fileRenameInfoPtr->FileName;
        for (int index = 0; index < targetFileName.Length; ++index)
          chPtr[index] = targetFileName[index];
        chPtr[targetFileName.Length] = char.MinValue;
        return (bool) PInvoke.SetFileInformationByHandle(handle, FILE_INFO_BY_HANDLE_CLASS.FileRenameInfo, (void*) fileRenameInfoPtr, (uint) num2) == (bool) new BOOL(true);
      }
      finally
      {
        Marshal.FreeCoTaskMem((IntPtr) (void*) fileRenameInfoPtr);
      }
    }

    private static FileStream SafeCreateOrOpenFile(
      string path,
      FileAccess fileAccess,
      FileShare fileShare)
    {
      SafeFileHandle handle = (SafeFileHandle) null;
      try
      {
        handle = ReparsePointAware.SafeCreateNewFile(path, fileAccess, fileShare);
      }
      catch (IOException ex)
      {
      }
      if (handle == null)
        handle = ReparsePointAware.SafeOpenExistingFile(path, fileAccess, fileShare);
      return handle != null ? new FileStream(handle, fileAccess) : throw new IOException();
    }

    private static SafeFileHandle SafeCreateNewFile(
      string path,
      FileAccess fileAccess,
      FileShare fileShare)
    {
      FILE_ACCESS_FLAGS dwDesiredAccess = (FILE_ACCESS_FLAGS) 65536;
      FILE_SHARE_MODE dwShareMode = FILE_SHARE_MODE.FILE_SHARE_NONE;
      FILE_FLAGS_AND_ATTRIBUTES dwFlagsAndAttributes = FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_SEQUENTIAL_SCAN;
      SafeFileHandle fileStub = ReparsePointAware.UnsafeCreateFileStub(path, dwDesiredAccess, dwShareMode, new SECURITY_ATTRIBUTES?(), FILE_CREATION_DISPOSITION.CREATE_NEW, dwFlagsAndAttributes, (SafeHandle) null);
      if (fileStub.IsInvalid)
      {
        fileStub?.Dispose();
        throw new IOException();
      }
      if (ReparsePointAware.HasReparsePoints(fileStub, path))
      {
        ReparsePointAware.SetDeleteOnClose((SafeHandle) fileStub);
        fileStub.Dispose();
        throw new UnauthorizedAccessException();
      }
      SafeFileHandle hOriginalFile = PInvoke.ReOpenFile((SafeHandle) fileStub, (FILE_ACCESS_FLAGS) 0, fileShare.ToFILE_SHARE_MODE() | FILE_SHARE_MODE.FILE_SHARE_DELETE, FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_SEQUENTIAL_SCAN);
      fileStub.Dispose();
      if (hOriginalFile.IsInvalid)
      {
        hOriginalFile?.Dispose();
        throw new UnauthorizedAccessException();
      }
      SafeFileHandle newFile = PInvoke.ReOpenFile((SafeHandle) hOriginalFile, fileAccess.ToFILE_ACCESS_FLAGS(), fileShare.ToFILE_SHARE_MODE(), FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_SEQUENTIAL_SCAN);
      hOriginalFile.Dispose();
      if (newFile.IsInvalid)
      {
        newFile?.Dispose();
        throw new UnauthorizedAccessException();
      }
      return newFile;
    }

    private static SafeFileHandle SafeOpenExistingFile(
      string path,
      FileAccess fileAccess,
      FileShare fileShare)
    {
      FILE_ACCESS_FLAGS fileAccessFlags = fileAccess.ToFILE_ACCESS_FLAGS();
      FILE_SHARE_MODE fileShareMode = fileShare.ToFILE_SHARE_MODE();
      FILE_FLAGS_AND_ATTRIBUTES dwFlagsAndAttributes = FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_SEQUENTIAL_SCAN;
      SafeFileHandle fileStub = ReparsePointAware.UnsafeCreateFileStub(path, fileAccessFlags, fileShareMode, new SECURITY_ATTRIBUTES?(), FILE_CREATION_DISPOSITION.OPEN_EXISTING, dwFlagsAndAttributes, (SafeHandle) null);
      if (fileStub.IsInvalid)
      {
        fileStub.Dispose();
        throw new IOException();
      }
      if (ReparsePointAware.HasReparsePoints(fileStub, path))
      {
        fileStub.Dispose();
        throw new UnauthorizedAccessException();
      }
      return fileStub;
    }

    private static SafeFileHandle UnsafeCreateDirectoryInternal(string path)
    {
      FILE_ACCESS_FLAGS dwDesiredAccess = FILE_ACCESS_FLAGS.FILE_GENERIC_WRITE | FILE_ACCESS_FLAGS.FILE_READ_DATA | FILE_ACCESS_FLAGS.FILE_READ_EA | FILE_ACCESS_FLAGS.FILE_READ_ATTRIBUTES;
      FILE_SHARE_MODE dwShareMode = FILE_SHARE_MODE.FILE_SHARE_READ;
      FILE_CREATION_DISPOSITION dwCreationDisposition = FILE_CREATION_DISPOSITION.CREATE_NEW;
      FILE_FLAGS_AND_ATTRIBUTES dwFlagsAndAttributes = FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_DIRECTORY | FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_BACKUP_SEMANTICS | FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_POSIX_SEMANTICS;
      return ReparsePointAware.UnsafeCreateFileStub(path, dwDesiredAccess, dwShareMode, new SECURITY_ATTRIBUTES?(), dwCreationDisposition, dwFlagsAndAttributes, (SafeHandle) null);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static unsafe void SafeSetFileAttributesNormal(string path)
    {
      FILE_ACCESS_FLAGS dwDesiredAccess = FILE_ACCESS_FLAGS.FILE_READ_ATTRIBUTES | FILE_ACCESS_FLAGS.FILE_WRITE_ATTRIBUTES;
      FILE_SHARE_MODE dwShareMode = FILE_SHARE_MODE.FILE_SHARE_NONE;
      FILE_CREATION_DISPOSITION dwCreationDisposition = FILE_CREATION_DISPOSITION.OPEN_EXISTING;
      FILE_FLAGS_AND_ATTRIBUTES dwFlagsAndAttributes = FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL;
      using (SafeFileHandle fileStub = ReparsePointAware.UnsafeCreateFileStub(path, dwDesiredAccess, dwShareMode, new SECURITY_ATTRIBUTES?(), dwCreationDisposition, dwFlagsAndAttributes, (SafeHandle) null))
      {
        if (ReparsePointAware.HasReparsePoints(fileStub, path))
          throw new UnauthorizedAccessException();
        if (!(bool) PInvoke.SetFileInformationByHandle((SafeHandle) fileStub, FILE_INFO_BY_HANDLE_CLASS.FileBasicInfo, (void*) &new FILE_BASIC_INFO()
        {
          FileAttributes = Microsoft.VisualStudio.Telemetry.NativeMethods.FILE_ATTRIBUTE_NORMAL
        }, (uint) sizeof (FILE_BASIC_INFO)))
          throw new UnauthorizedAccessException();
      }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static SafeFileHandle UnsafeCreateFileStub(
      string lpFileName,
      FILE_ACCESS_FLAGS dwDesiredAccess,
      FILE_SHARE_MODE dwShareMode,
      SECURITY_ATTRIBUTES? lpSecurityAttributes,
      FILE_CREATION_DISPOSITION dwCreationDisposition,
      FILE_FLAGS_AND_ATTRIBUTES dwFlagsAndAttributes,
      SafeHandle hTemplateFile)
    {
      if (Platform.IsWindows)
        return PInvoke.CreateFile(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
      throw new IOException();
    }

    internal static SafeFileHandle TESTUSEONLY_CreateFileStub(
      string lpFileName,
      FILE_ACCESS_FLAGS dwDesiredAccess,
      FILE_SHARE_MODE dwShareMode,
      SECURITY_ATTRIBUTES? lpSecurityAttributes,
      FILE_CREATION_DISPOSITION dwCreationDisposition,
      FILE_FLAGS_AND_ATTRIBUTES dwFlagsAndAttributes)
    {
      if (Platform.IsWindows)
        return ReparsePointAware.UnsafeCreateFileStub(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, (SafeHandle) null);
      throw new IOException();
    }

    internal static void TESTUSEONLY_SetDeleteOnClose(SafeHandle handle) => ReparsePointAware.SetDeleteOnClose(handle);
  }
}
