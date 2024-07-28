// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.LongPathUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class LongPathUtility
  {
    private static Regex AbsolutePathRegEx = new Regex("^([a-zA-Z]:\\\\|\\\\\\\\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private const int ERROR_FILE_NOT_FOUND = 2;

    public static IEnumerable<string> EnumerateDirectories(string path, bool recursiveSearch)
    {
      List<string> directoryPaths = new List<string>();
      LongPathUtility.EnumerateDirectoriesInternal(directoryPaths, path, recursiveSearch);
      return (IEnumerable<string>) directoryPaths;
    }

    public static IEnumerable<string> EnumerateFiles(string path, bool recursiveSearch) => LongPathUtility.EnumerateFiles(path, "*", recursiveSearch);

    public static IEnumerable<string> EnumerateFiles(
      string path,
      string matchPattern,
      bool recursiveSearch)
    {
      if (!LongPathUtility.DirectoryExists(path))
        throw new DirectoryNotFoundException("The path '" + path + "' is not a valid directory.");
      List<string> filePaths = new List<string>();
      LongPathUtility.EnumerateFilesInternal(filePaths, path, matchPattern, recursiveSearch);
      return (IEnumerable<string>) filePaths;
    }

    public static bool FileExists(string filePath) => LongPathUtility.FileOrDirectoryExists(filePath, false);

    public static bool DirectoryExists(string directoryPath) => LongPathUtility.FileOrDirectoryExists(directoryPath, true);

    private static bool FileOrDirectoryExists(string filePath, bool isDirectory)
    {
      if (string.IsNullOrWhiteSpace(filePath))
        throw new ArgumentException("A path to the file is required and cannot be null, empty or whitespace", nameof (filePath));
      bool flag = false;
      LongPathUtility.FlagsAndAttributes fileAttributes = (LongPathUtility.FlagsAndAttributes) LongPathUtility.NativeMethods.GetFileAttributes(filePath);
      if (fileAttributes != LongPathUtility.FlagsAndAttributes.InvalidFileAttributes && (fileAttributes & LongPathUtility.FlagsAndAttributes.Directory) == LongPathUtility.FlagsAndAttributes.Directory == isDirectory)
        flag = true;
      return flag;
    }

    public static string GetFullNormalizedPath(string path)
    {
      uint num = !string.IsNullOrWhiteSpace(path) ? LongPathUtility.NativeMethods.GetFullPathName(path, 0U, (StringBuilder) null, (StringBuilder) null) : throw new ArgumentException("A path is required and cannot be null, empty or whitespace", nameof (path));
      int lastWin32Error1 = Marshal.GetLastWin32Error();
      StringBuilder lpBuffer = num > 0U ? new StringBuilder((int) num) : throw new Win32Exception(lastWin32Error1, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Path normalization/expansion failed. A full path was not returned by the Kernel32 subsystem for '{0}'.", (object) path));
      int fullPathName = (int) LongPathUtility.NativeMethods.GetFullPathName(path, num, lpBuffer, (StringBuilder) null);
      int lastWin32Error2 = Marshal.GetLastWin32Error();
      if ((uint) fullPathName <= 0U)
        throw new Win32Exception(lastWin32Error2, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Path normalization/expansion failed.  The path length was not returned by the Kernel32 subsystem for '{0}'.", (object) path));
      return lpBuffer.ToString()?.TrimEnd('\\');
    }

    public static bool IsAbsolutePath(string path) => LongPathUtility.AbsolutePathRegEx.Match(path).Success;

    public static string RemoveExtendedLengthPathPrefix(string inPath)
    {
      string str = inPath;
      if (!string.IsNullOrWhiteSpace(inPath) && inPath.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
        str = inPath.Replace("\\\\?\\UNC", "\\").Replace("\\\\?\\", string.Empty);
      return str;
    }

    private static string CombinePaths(string pathA, string pathB)
    {
      if (pathA == null)
        throw new ArgumentNullException(nameof (pathA));
      return pathB != null ? Path.Combine(pathA.TrimEnd('\\'), pathB.TrimStart('\\')) : throw new ArgumentNullException(nameof (pathB));
    }

    private static string ConvertToExtendedLengthPath(string path)
    {
      string extendedLengthPath = LongPathUtility.GetFullNormalizedPath(path);
      if (!string.IsNullOrWhiteSpace(extendedLengthPath) && !extendedLengthPath.StartsWith("\\\\?", StringComparison.OrdinalIgnoreCase))
        extendedLengthPath = !extendedLengthPath.StartsWith("\\\\", StringComparison.OrdinalIgnoreCase) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\\\\?\\{0}", (object) extendedLengthPath) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\\\\?\\UNC{0}", (object) extendedLengthPath.Substring(1));
      return extendedLengthPath;
    }

    private static IEnumerable<string> EnumerateDirectoriesInPath(string path)
    {
      LongPathUtility.FindData findData = new LongPathUtility.FindData();
      List<string> stringList = new List<string>();
      LongPathUtility.SafeFindHandle firstFile;
      using (firstFile = LongPathUtility.NativeMethods.FindFirstFile(LongPathUtility.CombinePaths(LongPathUtility.ConvertToExtendedLengthPath(path), "*"), findData))
      {
        if (!firstFile.IsInvalid)
        {
          bool flag = false;
          do
          {
            if (!findData.fileName.Equals(".") && !findData.fileName.Equals("..") && (findData.fileAttributes & 16) != 0)
              stringList.Add(LongPathUtility.RemoveExtendedLengthPathPrefix(LongPathUtility.CombinePaths(path, findData.fileName)));
            if (LongPathUtility.NativeMethods.FindNextFile(firstFile, findData))
            {
              if (firstFile.IsInvalid)
                throw new Win32Exception(Marshal.GetLastWin32Error(), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Enumerating subdirectories for path '{0}' failed.", (object) path));
            }
            else
              flag = true;
          }
          while (!flag);
        }
      }
      return (IEnumerable<string>) stringList;
    }

    private static IEnumerable<string> EnumerateFilesInPath(string path, string matchPattern)
    {
      LongPathUtility.FindData findData = new LongPathUtility.FindData();
      List<string> stringList = new List<string>();
      LongPathUtility.SafeFindHandle firstFile;
      using (firstFile = LongPathUtility.NativeMethods.FindFirstFile(LongPathUtility.CombinePaths(LongPathUtility.ConvertToExtendedLengthPath(path), matchPattern), findData))
      {
        int lastWin32Error1 = Marshal.GetLastWin32Error();
        if (firstFile.IsInvalid)
        {
          if (lastWin32Error1 != 2)
            throw new Win32Exception(lastWin32Error1, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Enumerating files for path '{0}' failed.", (object) path));
        }
        else
        {
          bool flag = false;
          do
          {
            if (!findData.fileName.Equals(".") && !findData.fileName.Equals("..") && (findData.fileAttributes & 16) == 0)
              stringList.Add(LongPathUtility.RemoveExtendedLengthPathPrefix(LongPathUtility.CombinePaths(path, findData.fileName)));
            if (LongPathUtility.NativeMethods.FindNextFile(firstFile, findData))
            {
              int lastWin32Error2 = Marshal.GetLastWin32Error();
              if (firstFile.IsInvalid)
                throw new Win32Exception(lastWin32Error2, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Enumerating subdirectories for path '{0}' failed.", (object) path));
            }
            else
              flag = true;
          }
          while (!flag);
        }
      }
      return (IEnumerable<string>) stringList;
    }

    private static void EnumerateFilesInternal(
      List<string> filePaths,
      string path,
      string matchPattern,
      bool recursiveSearch)
    {
      IEnumerable<string> strings = LongPathUtility.EnumerateFilesInPath(path, matchPattern);
      if (strings.Any<string>())
      {
        lock (filePaths)
          filePaths.AddRange(strings);
      }
      if (!recursiveSearch)
        return;
      IEnumerable<string> source = LongPathUtility.EnumerateDirectoriesInPath(path);
      if (!source.Any<string>())
        return;
      Parallel.ForEach<string>(source, (Action<string>) (searchPath => LongPathUtility.EnumerateFilesInternal(filePaths, searchPath, matchPattern, recursiveSearch)));
    }

    public static void EnumerateDirectoriesInternal(
      List<string> directoryPaths,
      string path,
      bool recursiveSearch)
    {
      IEnumerable<string> strings = LongPathUtility.EnumerateDirectoriesInPath(path);
      if (!strings.Any<string>())
        return;
      lock (directoryPaths)
        directoryPaths.AddRange(strings);
      if (!recursiveSearch)
        return;
      Parallel.ForEach<string>(strings, (Action<string>) (searchPath => LongPathUtility.EnumerateDirectoriesInternal(directoryPaths, searchPath, recursiveSearch)));
    }

    private static class NativeMethods
    {
      private const string Kernel32Dll = "kernel32.dll";

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true, BestFitMapping = false)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool FindClose(IntPtr hFindFile);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
      public static extern LongPathUtility.SafeFindHandle FindFirstFile(
        [MarshalAs(UnmanagedType.LPTStr)] string fileName,
        [In, Out] LongPathUtility.FindData findFileData);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool FindNextFile(
        LongPathUtility.SafeFindHandle hFindFile,
        [In, Out] LongPathUtility.FindData lpFindFileData);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
      public static extern int GetFileAttributes(string lpFileName);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
      public static extern uint GetFullPathName(
        [MarshalAs(UnmanagedType.LPTStr)] string lpFileName,
        uint nBufferLength,
        [Out] StringBuilder lpBuffer,
        StringBuilder lpFilePart);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private sealed class FindData
    {
      public int fileAttributes;
      public System.Runtime.InteropServices.ComTypes.FILETIME creationTime;
      public System.Runtime.InteropServices.ComTypes.FILETIME lastAccessTime;
      public System.Runtime.InteropServices.ComTypes.FILETIME lastWriteTime;
      public int nFileSizeHigh;
      public int nFileSizeLow;
      public int dwReserved0;
      public int dwReserved1;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string fileName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
      public string alternateFileName;
    }

    private sealed class SafeFindHandle : SafeHandleMinusOneIsInvalid
    {
      public SafeFindHandle()
        : base(true)
      {
      }

      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      protected override bool ReleaseHandle() => LongPathUtility.NativeMethods.FindClose(this.handle);
    }

    [Flags]
    private enum FlagsAndAttributes : uint
    {
      None = 0,
      Readonly = 1,
      Hidden = 2,
      System = 4,
      Directory = 16, // 0x00000010
      Archive = 32, // 0x00000020
      Device = 64, // 0x00000040
      Normal = 128, // 0x00000080
      Temporary = 256, // 0x00000100
      SparseFile = 512, // 0x00000200
      ReparsePoint = 1024, // 0x00000400
      Compressed = 2048, // 0x00000800
      Offline = 4096, // 0x00001000
      NotContentIndexed = 8192, // 0x00002000
      Encrypted = 16384, // 0x00004000
      Write_Through = 2147483648, // 0x80000000
      Overlapped = 1073741824, // 0x40000000
      NoBuffering = 536870912, // 0x20000000
      RandomAccess = 268435456, // 0x10000000
      SequentialScan = 134217728, // 0x08000000
      DeleteOnClose = 67108864, // 0x04000000
      BackupSemantics = 33554432, // 0x02000000
      PosixSemantics = 16777216, // 0x01000000
      OpenReparsePoint = 2097152, // 0x00200000
      OpenNoRecall = 1048576, // 0x00100000
      FirstPipeInstance = 524288, // 0x00080000
      InvalidFileAttributes = 4294967295, // 0xFFFFFFFF
    }
  }
}
