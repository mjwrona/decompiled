// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LastAccessTimeBasedDeletion
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LastAccessTimeBasedDeletion : IDeleteCacheItems, IDisposable
  {
    private readonly IProxyStatistics m_proxyStatistics;
    private readonly object m_deleteLock;
    private readonly TimeSpan m_deletionThresholdOffset;
    private readonly string m_cacheRoot;
    private bool m_isShuttingDown;
    private bool m_deleting;
    private long m_spaceToFreeInBytes;
    private long m_spaceFreedThisRun;
    private int m_deletedItems;
    private int m_filesAfterThreshold;
    private long m_fileTimeOfOldestFileSeenLastScan;
    private bool m_stopWhenThresholdMet;
    private bool m_cleanupFullSuccess;
    private static readonly string s_area = "FileCacheService";
    private static readonly string s_layer = "Cleanup";

    public LastAccessTimeBasedDeletion(
      IVssRequestContext requestContext,
      int deletionThresholdOffset,
      string cacheRoot,
      IProxyStatistics proxyStatistics)
    {
      this.m_deleteLock = new object();
      this.m_isShuttingDown = false;
      this.m_proxyStatistics = proxyStatistics;
      this.m_cacheRoot = cacheRoot;
      this.m_deletionThresholdOffset = TimeSpan.FromDays((double) Math.Max(deletionThresholdOffset, 0));
      this.m_stopWhenThresholdMet = false;
    }

    public void Delete(IVssRequestContext requestContext, long stateInfo, bool emergencyCleanup = false)
    {
      if (this.m_isShuttingDown || requestContext.IsCanceled || this.m_deleting)
        return;
      lock (this.m_deleteLock)
      {
        if (this.m_deleting)
          return;
        this.m_cleanupFullSuccess = true;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        requestContext.TraceAlways(13409, TraceLevel.Info, LastAccessTimeBasedDeletion.s_area, LastAccessTimeBasedDeletion.s_layer, "Starting cleanup of " + stateInfo.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture) + " bytes.");
        try
        {
          this.m_deleting = true;
          this.DeleteWithDecreasingThreshold(requestContext, stateInfo, emergencyCleanup);
        }
        catch (RequestCanceledException ex)
        {
          this.m_cleanupFullSuccess = false;
        }
        catch (Exception ex)
        {
          this.m_cleanupFullSuccess = false;
          requestContext.TraceException(13408, LastAccessTimeBasedDeletion.s_area, LastAccessTimeBasedDeletion.s_layer, ex);
        }
        finally
        {
          this.m_deleting = false;
          stopwatch.Stop();
          TeamFoundationEventLog.Default.Log(FrameworkResources.CacheCleanupComplete((object) stopwatch.ElapsedMilliseconds, (object) this.m_spaceFreedThisRun, (object) stateInfo, (object) this.m_cleanupFullSuccess), TeamFoundationEventId.CacheCleanupComplete, EventLogEntryType.Information);
        }
      }
    }

    private void DeleteWithDecreasingThreshold(
      IVssRequestContext requestContext,
      long stateInfo,
      bool emergencyCleanup)
    {
      this.m_spaceToFreeInBytes = stateInfo;
      this.m_spaceFreedThisRun = 0L;
      if (this.m_spaceToFreeInBytes <= 0L)
        return;
      DateTime startTime = DateTime.Now;
      this.m_fileTimeOfOldestFileSeenLastScan = DateTime.MaxValue.ToFileTime();
      DateTime dateTime = startTime.Subtract(this.m_deletionThresholdOffset);
      if (dateTime > startTime)
        dateTime = startTime;
      int passNumber = 1;
      this.m_filesAfterThreshold = 0;
      this.m_stopWhenThresholdMet = false;
      if (emergencyCleanup)
      {
        this.m_stopWhenThresholdMet = true;
        dateTime = DateTime.MaxValue;
      }
      do
      {
        if (passNumber != 1)
          dateTime = this.ComputeNextSearchTime(dateTime, DateTime.FromFileTime(this.m_fileTimeOfOldestFileSeenLastScan), startTime);
        try
        {
          this.DeleteImpl(requestContext, dateTime.ToFileTime());
        }
        finally
        {
          LogPass(dateTime);
        }
        ++passNumber;
        this.m_stopWhenThresholdMet = true;
      }
      while (this.Continue(requestContext) && dateTime < startTime);

      void LogPass(DateTime thresholdTime) => requestContext.TraceAlways(13410, TraceLevel.Info, LastAccessTimeBasedDeletion.s_area, LastAccessTimeBasedDeletion.s_layer, string.Format("Pass #{0}. startTime: {1}, thresholdTime: {2}, oldestFile: {3}, deletedItem: {4}, spacefreed: {5}, fileAfterThreshold: {6}", (object) passNumber, (object) startTime.ToString("O"), (object) thresholdTime.ToString("O"), (object) DateTime.FromFileTime(this.m_fileTimeOfOldestFileSeenLastScan).ToString("O"), (object) this.m_deletedItems, (object) this.m_spaceFreedThisRun, (object) this.m_filesAfterThreshold));
    }

    private DateTime ComputeNextSearchTime(
      DateTime currentSearchTime,
      DateTime oldestFileSeenTime,
      DateTime startTime)
    {
      DateTime dateTime = currentSearchTime > oldestFileSeenTime ? currentSearchTime : oldestFileSeenTime;
      DateTime nextSearchTime;
      if (dateTime < startTime - TimeSpan.FromDays(2.0))
      {
        nextSearchTime = dateTime + TimeSpan.FromDays(1.0);
      }
      else
      {
        TimeSpan timeSpan = TimeSpan.FromSeconds(0.5 * (startTime - dateTime).TotalSeconds);
        if (timeSpan < TimeSpan.FromMinutes(30.0))
          timeSpan = TimeSpan.FromMinutes(30.0);
        nextSearchTime = dateTime + timeSpan;
      }
      if (nextSearchTime > startTime)
        nextSearchTime = startTime;
      return nextSearchTime;
    }

    private void DeleteImpl(IVssRequestContext requestContext, long currentDeleteFileTime)
    {
      foreach (DirectoryInfo topLevelDirectory in FileCacheHelper.GetTopLevelDirectories(this.m_cacheRoot))
      {
        if (!this.Continue(requestContext))
          break;
        int deletedItems = this.m_deletedItems;
        long spaceFreedThisRun = this.m_spaceFreedThisRun;
        this.CleanupCacheDirectory(requestContext, topLevelDirectory.FullName, currentDeleteFileTime);
        int num1 = this.m_deletedItems - deletedItems;
        long num2 = this.m_spaceFreedThisRun - spaceFreedThisRun;
        if (num1 > 0)
          this.m_proxyStatistics.UpdateCacheSize(topLevelDirectory.Name, -num2, -num1);
      }
    }

    private void CleanupCacheDirectory(
      IVssRequestContext requestContext,
      string cacheDirectory,
      long currentDeleteFileTime)
    {
      IntPtr hFindFile = IntPtr.Zero;
      try
      {
        ProxyNativeMethods.WIN32_FIND_DATA lpFindFileData;
        if ((hFindFile = ProxyNativeMethods.FindFirstFile(Path.Combine(cacheDirectory, "*"), out lpFindFileData)) != new IntPtr(-1))
        {
          while (this.Continue(requestContext))
          {
            if ((lpFindFileData.dwFileAttributes & FileAttributes.Directory) == (FileAttributes) 0)
            {
              if (this.IsBeyondAgeThreshold(lpFindFileData, currentDeleteFileTime))
                this.DeleteFile(cacheDirectory, lpFindFileData);
            }
            else if (lpFindFileData.cFileName != "." && lpFindFileData.cFileName != "..")
              this.CleanupCacheDirectory(requestContext, Path.Combine(cacheDirectory, lpFindFileData.cFileName), currentDeleteFileTime);
            if (this.Continue(requestContext))
            {
              if (!ProxyNativeMethods.FindNextFile(hFindFile, out lpFindFileData))
                break;
            }
            else
              break;
          }
        }
      }
      finally
      {
        if (hFindFile != IntPtr.Zero)
        {
          ProxyNativeMethods.FindClose(hFindFile);
          IntPtr zero = IntPtr.Zero;
        }
      }
      this.DeleteDirectoryIfEmpty(cacheDirectory);
    }

    private void DeleteFile(string cacheDirectory, ProxyNativeMethods.WIN32_FIND_DATA findFileData)
    {
      try
      {
        File.Delete(Path.Combine(cacheDirectory, findFileData.cFileName));
        ++this.m_deletedItems;
        this.m_spaceFreedThisRun += FileCacheHelper.RoundToNearestClusterSize((long) findFileData.nFileSizeLow + ((long) findFileData.nFileSizeHigh << 32));
      }
      catch (IOException ex) when (FileCacheHelper.IsSharingViolation(ex))
      {
      }
      catch (IOException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(13407, LastAccessTimeBasedDeletion.s_area, LastAccessTimeBasedDeletion.s_layer, (Exception) ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(13406, LastAccessTimeBasedDeletion.s_area, LastAccessTimeBasedDeletion.s_layer, (Exception) ex);
      }
    }

    private void DeleteDirectoryIfEmpty(string cacheDirectory)
    {
      try
      {
        if (Directory.EnumerateFileSystemEntries(cacheDirectory).Any<string>())
          return;
        Directory.Delete(cacheDirectory, false);
      }
      catch (IOException ex) when (FileCacheHelper.IsSharingViolation(ex))
      {
      }
      catch (IOException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(13411, LastAccessTimeBasedDeletion.s_area, LastAccessTimeBasedDeletion.s_layer, (Exception) ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(13412, LastAccessTimeBasedDeletion.s_area, LastAccessTimeBasedDeletion.s_layer, (Exception) ex);
      }
    }

    private bool Continue(IVssRequestContext requestContext)
    {
      if (this.m_isShuttingDown)
      {
        this.m_cleanupFullSuccess = false;
        return false;
      }
      requestContext.RequestContextInternal().CheckCanceled();
      return !this.m_stopWhenThresholdMet || this.m_spaceFreedThisRun < this.m_spaceToFreeInBytes;
    }

    private bool IsBeyondAgeThreshold(
      ProxyNativeMethods.WIN32_FIND_DATA findFileData,
      long currentDeleteTime)
    {
      long num1 = (long) findFileData.ftLastAccessTime.dwHighDateTime << 32 | (long) findFileData.ftLastAccessTime.dwLowDateTime & (long) uint.MaxValue;
      int num2 = num1 < currentDeleteTime ? 1 : 0;
      if (num2 != 0)
        return num2 != 0;
      ++this.m_filesAfterThreshold;
      if (num1 >= this.m_fileTimeOfOldestFileSeenLastScan)
        return num2 != 0;
      this.m_fileTimeOfOldestFileSeenLastScan = num1;
      return num2 != 0;
    }

    public void Dispose() => this.m_isShuttingDown = true;
  }
}
