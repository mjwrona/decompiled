// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteControl.WinINetHelper
// Assembly: Microsoft.VisualStudio.RemoteControl, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4D9D0761-3208-49DD-A9E2-BF705DBE6B5D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.RemoteControl.dll

using System;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VisualStudio.RemoteControl
{
  internal static class WinINetHelper
  {
    private const string CacheWriteTimestampHeaderName = "Cache-Write-Timestamp";
    private const int MAX_PATH = 260;
    private const uint NORMAL_CACHE_ENTRY = 1;

    internal static bool WriteErrorResponseToCache(string url, HttpStatusCode status) => status == HttpStatusCode.NotFound ? WinINetHelper.WriteErrorResponseToCache(url, 404, "Not Found") : WinINetHelper.WriteErrorResponseToCache(url, (int) status, "Unknown");

    [DllImport("wininet", EntryPoint = "CreateUrlCacheEntryW", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern bool CreateUrlCacheEntry(
      string lpszUrlName,
      uint dwExpectedFileSize,
      string lpszFileExtension,
      StringBuilder lpszFileName,
      uint dwReserved);

    [DllImport("wininet", EntryPoint = "CommitUrlCacheEntryW", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern bool CommitUrlCacheEntry(
      string url,
      string fileName,
      System.Runtime.InteropServices.ComTypes.FILETIME ftExpiryTime,
      System.Runtime.InteropServices.ComTypes.FILETIME ftModifiedTime,
      uint cacheEntryType,
      string header,
      uint headerSize,
      string fileExt,
      string originalUrl);

    private static bool WriteErrorResponseToCache(
      string url,
      int statusCode,
      string statusCodeDescription)
    {
      string header = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "HTTP/1.0 {0} {1}\r\n{2}: {3}\r\n\r\n", (object) statusCode, (object) statusCodeDescription, (object) "Cache-Write-Timestamp", (object) DateTime.Now.Ticks);
      StringBuilder lpszFileName = new StringBuilder()
      {
        Capacity = 260
      };
      return WinINetHelper.CreateUrlCacheEntry(url, 8U, "cache", lpszFileName, 0U) && WinINetHelper.CommitUrlCacheEntry(url, lpszFileName.ToString(), new System.Runtime.InteropServices.ComTypes.FILETIME(), new System.Runtime.InteropServices.ComTypes.FILETIME(), 1U, header, (uint) header.Length, (string) null, (string) null);
    }
  }
}
