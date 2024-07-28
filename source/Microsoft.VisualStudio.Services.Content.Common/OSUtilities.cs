// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.OSUtilities
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class OSUtilities
  {
    private static Dictionary<int, string> Errno = new Dictionary<int, string>()
    {
      {
        1,
        "EPERM"
      },
      {
        2,
        "ENOENT"
      },
      {
        3,
        "ESRCH"
      },
      {
        4,
        "EINTR"
      },
      {
        5,
        "EIO"
      },
      {
        6,
        "ENXIO"
      },
      {
        7,
        "E2BIG"
      },
      {
        8,
        "ENOEXEC"
      },
      {
        9,
        "EBADF"
      },
      {
        10,
        "ECHILD"
      },
      {
        11,
        "EDEADLK"
      },
      {
        12,
        "ENOMEM"
      },
      {
        13,
        "EACCES"
      },
      {
        14,
        "EFAULT"
      },
      {
        15,
        "ENOTBLK"
      },
      {
        16,
        "EBUSY"
      },
      {
        17,
        "EEXIST"
      },
      {
        18,
        "EXDEV"
      },
      {
        19,
        "ENODEV"
      },
      {
        20,
        "ENOTDIR"
      },
      {
        21,
        "EISDIR"
      },
      {
        22,
        "EINVAL"
      },
      {
        23,
        "ENFILE"
      },
      {
        24,
        "EMFILE"
      },
      {
        25,
        "ENOTTY"
      },
      {
        26,
        "ETXTBSY"
      },
      {
        27,
        "EFBIG"
      },
      {
        28,
        "ENOSPC"
      },
      {
        29,
        "ESPIPE"
      },
      {
        30,
        "EROFS"
      },
      {
        31,
        "EMLINK"
      },
      {
        32,
        "EPIPE"
      },
      {
        33,
        "EDOM"
      },
      {
        34,
        "ERANGE"
      },
      {
        35,
        "EAGAIN"
      },
      {
        62,
        "ELOOP,"
      }
    };

    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    [DllImport("libc", EntryPoint = "symlink", SetLastError = true)]
    private static extern int SymLink(string target, string linkPath);

    [DllImport("libc", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern long readlink(string link, StringBuilder buffer, long length);

    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    [CLSCompliant(false)]
    [DllImport("libc", SetLastError = true)]
    public static extern int chmod(string pathname, uint mode);

    public static bool IsWindows() => true;

    public static bool IsLinux() => false;

    public static bool IsMacOS() => false;

    public static string GetRuntimeOS()
    {
      if (OSUtilities.IsWindows())
        return "Windows";
      if (OSUtilities.IsLinux())
        return "Linux";
      if (OSUtilities.IsMacOS())
        return "Darwin";
      throw new NotSupportedException("Not supported OS.");
    }

    public static bool IsFileSymbolicLink(FileInfo fileInfo) => (fileInfo.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;

    public static void CreateSymbolicLink(string symlinkName, string target)
    {
    }

    public static string ReadSymbolicLink(string symbolicLinkFilePath) => (string) null;

    private static int CreateSymlink(string symbolicLinkFilePath, string targetFilePath)
    {
      StringBuilder buffer = new StringBuilder(1024);
      if (OSUtilities.SafeReadLink(symbolicLinkFilePath, buffer, (long) symbolicLinkFilePath.Length) >= 0L)
        return 0;
      string directoryName = Path.GetDirectoryName(symbolicLinkFilePath);
      if (!string.IsNullOrWhiteSpace(directoryName) && !Directory.Exists(directoryName))
        Directory.CreateDirectory(directoryName);
      int num = OSUtilities.SymLink(targetFilePath, symbolicLinkFilePath);
      return num != 0 ? Marshal.GetLastWin32Error() : num;
    }

    private static long SafeReadLink(
      string symbolicLinkFilePath,
      StringBuilder buffer,
      long bufferSize)
    {
      long num = OSUtilities.readlink(symbolicLinkFilePath, buffer, bufferSize);
      if (num >= 0L)
        buffer.Length = (int) num;
      return num;
    }

    [CLSCompliant(false)]
    public static uint GetFilePermissions(string path) => 0;

    [CLSCompliant(false)]
    public static void SetFilePermissions(string path, uint permissions, bool followSymlink = false)
    {
    }

    private static int SafeSetFilePermissions(string path, uint permissions, bool followSymlink) => 0;
  }
}
