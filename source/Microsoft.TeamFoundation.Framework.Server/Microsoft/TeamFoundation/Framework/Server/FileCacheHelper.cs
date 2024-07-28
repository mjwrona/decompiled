// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileCacheHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class FileCacheHelper
  {
    private const string c_area = "FileCache";
    private const string c_layer = "FileCacheHelper";

    internal static bool IsSharingViolation(IOException ioException) => ioException.HResult == -2147024864;

    public static string StringFromByteArray(byte[] bytes)
    {
      if (bytes == null || bytes.Length == 0)
        return "null";
      StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
      for (int index = 0; index < bytes.Length; ++index)
      {
        int num = (int) bytes[index];
        char ch1 = (char) ((num >> 4 & 15) + 48);
        char ch2 = (char) ((num & 15) + 48);
        stringBuilder.Append(ch1 >= ':' ? (char) ((uint) ch1 + 39U) : ch1);
        stringBuilder.Append(ch2 >= ':' ? (char) ((uint) ch2 + 39U) : ch2);
      }
      return stringBuilder.ToString();
    }

    internal static long RoundToNearestClusterSize(long fileSize)
    {
      int num1 = 4096;
      long num2 = fileSize % (long) num1;
      long nearestClusterSize = fileSize;
      if (num2 > 0L)
        nearestClusterSize = fileSize + ((long) num1 - num2);
      return nearestClusterSize;
    }

    internal static List<DirectoryInfo> GetTopLevelDirectories(string cacheRoot)
    {
      DirectoryInfo directoryInfo = new DirectoryInfo(cacheRoot);
      return directoryInfo != null ? new List<DirectoryInfo>((IEnumerable<DirectoryInfo>) directoryInfo.GetDirectories()) : new List<DirectoryInfo>();
    }

    internal static long GetAvailableFreeSpace(string cacheRoot)
    {
      ulong lpFreeBytesAvailable;
      if (ProxyNativeMethods.GetDiskFreeSpaceEx(cacheRoot, out lpFreeBytesAvailable, out ulong _, out ulong _))
        return (long) lpFreeBytesAvailable;
      int lastWin32Error = Marshal.GetLastWin32Error();
      if (lastWin32Error == 3)
      {
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileCache", nameof (FileCacheHelper), "Invalid Path {0} in GetAvailableFreeSpace", (object) cacheRoot);
        return 0;
      }
      TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "FileCache", nameof (FileCacheHelper), "Error {0} in GetAvailableFreeSpace", (object) lastWin32Error);
      throw new Win32Exception(lastWin32Error);
    }

    internal static bool CheckWin32Error(string resourceMessage)
    {
      int lastWin32Error = Marshal.GetLastWin32Error();
      if (18 == lastWin32Error)
        return true;
      TeamFoundationEventLog.Default.LogException(resourceMessage, (Exception) new Win32Exception(lastWin32Error));
      return false;
    }

    internal static DirectoryScanResult ScanDirectory(
      string directoryPath,
      IVssRequestContext requestContext,
      int maxScanLevel)
    {
      DirectoryScanResult scanResult = new DirectoryScanResult();
      ProxyNativeMethods.WIN32_FIND_DATA lpFindFileData;
      IntPtr firstFile = ProxyNativeMethods.FindFirstFile(Path.Combine(directoryPath, "*"), out lpFindFileData);
      if (firstFile == ProxyNativeMethods.INVALID_HANDLE)
      {
        scanResult.Success = FileCacheHelper.CheckWin32Error(FrameworkResources.ErrorComputingFolderStatistics((object) directoryPath));
        return scanResult;
      }
      try
      {
        do
        {
          requestContext.RequestContextInternal().CheckCanceled();
          if ((lpFindFileData.dwFileAttributes & FileAttributes.Directory) != (FileAttributes) 0)
          {
            if (lpFindFileData.cFileName != "." && lpFindFileData.cFileName != "..")
              FileCacheHelper.WalkDirectory(Path.Combine(directoryPath, lpFindFileData.cFileName), scanResult, requestContext, 2, maxScanLevel);
          }
          else
          {
            scanResult.DirectorySize += FileCacheHelper.ComputeFileSize(lpFindFileData);
            ++scanResult.FileCount;
          }
        }
        while (ProxyNativeMethods.FindNextFile(firstFile, out lpFindFileData));
        scanResult.Success = FileCacheHelper.CheckWin32Error(FrameworkResources.ErrorComputingFolderStatistics((object) directoryPath));
        return scanResult;
      }
      catch (RequestCanceledException ex)
      {
        scanResult.Success = false;
        return scanResult;
      }
      finally
      {
        ProxyNativeMethods.FindClose(firstFile);
      }
    }

    private static void WalkDirectory(
      string dir,
      DirectoryScanResult scanResult,
      IVssRequestContext requestContext,
      int currentLevel,
      int maxScanLevel)
    {
      if (currentLevel > maxScanLevel)
        return;
      ProxyNativeMethods.WIN32_FIND_DATA lpFindFileData;
      IntPtr firstFile = ProxyNativeMethods.FindFirstFile(Path.Combine(dir, "*"), out lpFindFileData);
      if (firstFile == ProxyNativeMethods.INVALID_HANDLE)
      {
        FileCacheHelper.CheckWin32Error(FrameworkResources.ErrorComputingFolderStatistics((object) dir));
      }
      else
      {
        try
        {
          do
          {
            requestContext.RequestContextInternal().CheckCanceled();
            if ((lpFindFileData.dwFileAttributes & FileAttributes.Directory) != (FileAttributes) 0)
            {
              if (!(lpFindFileData.cFileName == ".") && !(lpFindFileData.cFileName == ".."))
              {
                string dir1 = Path.Combine(dir, lpFindFileData.cFileName);
                if (currentLevel < maxScanLevel)
                  FileCacheHelper.WalkDirectory(dir1, scanResult, requestContext, currentLevel + 1, maxScanLevel);
                else
                  scanResult.SkippedDirectories.Add(dir1);
              }
            }
            else
            {
              scanResult.DirectorySize += FileCacheHelper.ComputeFileSize(lpFindFileData);
              ++scanResult.FileCount;
            }
          }
          while (ProxyNativeMethods.FindNextFile(firstFile, out lpFindFileData));
          FileCacheHelper.CheckWin32Error(FrameworkResources.ErrorComputingFolderStatistics((object) dir));
        }
        finally
        {
          ProxyNativeMethods.FindClose(firstFile);
        }
      }
    }

    private static long ComputeFileSize(ProxyNativeMethods.WIN32_FIND_DATA file) => FileCacheHelper.RoundToNearestClusterSize((long) file.nFileSizeLow + ((long) file.nFileSizeHigh << 32));

    internal static int ConvertFilePathToFileId(string folderName, string fileNameWithExtension)
    {
      string withoutExtension = Path.GetFileNameWithoutExtension(fileNameWithExtension);
      if (string.IsNullOrEmpty(withoutExtension) || withoutExtension.Length % 2 != 0)
        return 0;
      int fileId = 0;
      int num = 0;
      try
      {
        for (int index = withoutExtension.Length / 2; index > 0; --index)
        {
          fileId += (int) Convert.ToByte(withoutExtension.Substring(index * 2 - 2, 2), 16) << num;
          num += 8;
        }
        if (4 == folderName.Length)
          fileId += (int) Convert.ToByte(folderName.Substring(0, 2), 16) << 24;
      }
      catch (FormatException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(12221, "FileCache", nameof (FileCacheHelper), (Exception) ex);
        fileId = 0;
      }
      return fileId;
    }
  }
}
