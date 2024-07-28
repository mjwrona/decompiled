// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ProxyNativeMethods
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ProxyNativeMethods
  {
    public static readonly IntPtr INVALID_HANDLE = new IntPtr(-1);
    public const int ERROR_NO_MORE_FILES = 18;
    public const int ERROR_SHARING_VIOLATION = 32;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool GetDiskFreeSpaceEx(
      string lpDirectoryName,
      out ulong lpFreeBytesAvailable,
      out ulong lpTotalNumberOfBytes,
      out ulong lpTotalNumberOfFreeBytes);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern SafeFileHandle CreateFile(
      string lpFileName,
      uint dwDesiredAccess,
      uint dwShareMode,
      IntPtr lpSecurityAttributes,
      uint dwCreationDisposition,
      uint dwFlagsAndAttributes,
      IntPtr hTemplateFile);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr FindFirstFile(
      string lpFileName,
      out ProxyNativeMethods.WIN32_FIND_DATA lpFindFileData);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool FindNextFile(
      IntPtr hFindFile,
      out ProxyNativeMethods.WIN32_FIND_DATA lpFindFileData);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool FindClose(IntPtr hFindFile);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WIN32_FIND_DATA
    {
      public FileAttributes dwFileAttributes;
      public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
      public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
      public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
      public uint nFileSizeHigh;
      public uint nFileSizeLow;
      public uint dwReserved0;
      public uint dwReserved1;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string cFileName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
      public string cAlternateFileName;
    }
  }
}
