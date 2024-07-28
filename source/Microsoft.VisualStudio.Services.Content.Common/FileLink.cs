// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.FileLink
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class FileLink
  {
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CreateHardLinkW(
      string newFileName,
      string existingFileName,
      IntPtr reservedSecurityAttributes);

    [DllImport("libc", SetLastError = true)]
    internal static extern int link(string existingFileName, string newFileName);

    private static FileLink.CreateHardLinkStatus CreateHardLinkInternal(
      string existingFileName,
      string newFileName)
    {
      return FileLink.CreateHardLinkWin(existingFileName, newFileName);
    }

    public static FileLink.CreateHardLinkStatus CreateHardLink(
      string existingFileName,
      string newFileName)
    {
      FileLink.CreateHardLinkStatus hardLinkInternal = FileLink.CreateHardLinkInternal(existingFileName, newFileName);
      if (hardLinkInternal != FileLink.CreateHardLinkStatus.Success)
      {
        if (File.Exists(newFileName))
          File.Delete(newFileName);
        hardLinkInternal = FileLink.CreateHardLinkInternal(existingFileName, newFileName);
      }
      return hardLinkInternal;
    }

    private static FileLink.CreateHardLinkStatus CreateHardLinkWin(
      string existingFileName,
      string newFileName)
    {
      existingFileName = FileLink.IsLongPath(existingFileName) ? FileLink.PrependLongPathPrefix(existingFileName) : existingFileName;
      newFileName = FileLink.IsLongPath(newFileName) ? FileLink.PrependLongPathPrefix(newFileName) : newFileName;
      if (FileLink.CreateHardLinkW(newFileName, existingFileName, IntPtr.Zero))
        return FileLink.CreateHardLinkStatus.Success;
      switch (Marshal.GetLastWin32Error())
      {
        case 2:
          return FileLink.CreateHardLinkStatus.FailedFileNotFound;
        case 3:
          return FileLink.CreateHardLinkStatus.FailedPathNotFound;
        case 5:
          return FileLink.CreateHardLinkStatus.FailedAccessDenied;
        case 17:
          return FileLink.CreateHardLinkStatus.FailedSinceDestinationIsOnDifferentVolume;
        case 50:
          return FileLink.CreateHardLinkStatus.FailedSinceNotSupportedByFilesystem;
        case 123:
          return FileLink.CreateHardLinkStatus.FailedInvalidName;
        case 1142:
          return FileLink.CreateHardLinkStatus.FailedDueToPerFileLinkLimit;
        default:
          return FileLink.CreateHardLinkStatus.Failed;
      }
    }

    private static FileLink.CreateHardLinkStatus CreateHardLinkUnix(
      string existingFileName,
      string newFileName)
    {
      if (FileLink.link(existingFileName, newFileName) == 0)
        return FileLink.CreateHardLinkStatus.Success;
      switch (Marshal.GetLastWin32Error())
      {
        case 1:
          return FileLink.CreateHardLinkStatus.FailedAccessDenied;
        case 13:
          return FileLink.CreateHardLinkStatus.FailedAccessDenied;
        case 31:
        case 62:
          return FileLink.CreateHardLinkStatus.FailedDueToPerFileLinkLimit;
        default:
          return FileLink.CreateHardLinkStatus.Failed;
      }
    }

    private static string PrependLongPathPrefix(string path)
    {
      path = ("\\\\?\\" + path).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
      return path;
    }

    private static bool IsLongPath(string path) => path.Length >= 260;

    public enum CreateHardLinkStatus
    {
      Success,
      FailedSinceDestinationIsOnDifferentVolume,
      FailedDueToPerFileLinkLimit,
      FailedSinceNotSupportedByFilesystem,
      FailedAccessDenied,
      FailedInvalidName,
      FailedPathNotFound,
      FailedFileNotFound,
      Failed,
    }

    private enum Errno
    {
      EPERM = 1,
      ENOENT = 2,
      ESRCH = 3,
      EINTR = 4,
      EIO = 5,
      ENXIO = 6,
      E2BIG = 7,
      ENOEXEC = 8,
      EBADF = 9,
      ECHILD = 10, // 0x0000000A
      EDEADLK = 11, // 0x0000000B
      ENOMEM = 12, // 0x0000000C
      EACCES = 13, // 0x0000000D
      EFAULT = 14, // 0x0000000E
      ENOTBLK = 15, // 0x0000000F
      EBUSY = 16, // 0x00000010
      EEXIST = 17, // 0x00000011
      EXDEV = 18, // 0x00000012
      ENODEV = 19, // 0x00000013
      ENOTDIR = 20, // 0x00000014
      EISDIR = 21, // 0x00000015
      EINVAL = 22, // 0x00000016
      ENFILE = 23, // 0x00000017
      EMFILE = 24, // 0x00000018
      ENOTTY = 25, // 0x00000019
      ETXTBSY = 26, // 0x0000001A
      EFBIG = 27, // 0x0000001B
      ENOSPC = 28, // 0x0000001C
      ESPIPE = 29, // 0x0000001D
      EROFS = 30, // 0x0000001E
      EMLINK = 31, // 0x0000001F
      EPIPE = 32, // 0x00000020
      EDOM = 33, // 0x00000021
      ERANGE = 34, // 0x00000022
      EAGAIN = 35, // 0x00000023
      ELOOP = 62, // 0x0000003E
    }

    private enum NativeIOConstants
    {
      ErrorFileNotFound = 2,
      ErrorPathNotFound = 3,
      ErrorAccessDenied = 5,
      ErrorNotSameDevice = 17, // 0x00000011
      ErrorNotSupported = 50, // 0x00000032
      ErrorInvalidName = 123, // 0x0000007B
      ErrorTooManyLinks = 1142, // 0x00000476
    }
  }
}
