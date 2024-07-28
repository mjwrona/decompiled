// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ProxyStatistics
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ProxyStatistics : IDisposable, IProxyStatistics
  {
    private long m_overallCacheSize;
    private long m_overallCacheHits;
    private long m_overallCacheMisses;
    private object m_saveLock = new object();
    private bool m_scanComplete;
    private bool m_isShuttingDown;
    private DateTime m_nextPersistTime;
    private ManualResetEvent m_statisticsComputed;
    private IProxyConfigurationInternal m_proxyConfiguration;
    private bool m_initialized;
    private ReaderWriterLock m_collectionLock = new ReaderWriterLock();
    private Dictionary<string, ProxyStatistics.CollectionConfiguration> m_configuredCollections = new Dictionary<string, ProxyStatistics.CollectionConfiguration>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private const string c_area = "FileCache";
    private const string c_layer = "FileCacheStatistics";
    private const string c_statisticsFile = "ProxyStatistics.xml";
    private const string c_statisticsTempFile = "ProxyStatisticsTemp.xml";

    internal ProxyStatistics() => this.m_initialized = false;

    internal void Initialize(
      IVssRequestContext requestContext,
      IProxyConfigurationInternal proxyConfiguration,
      ScanDiskCompleted completionCallback)
    {
      if (this.m_initialized)
        return;
      lock (this.m_saveLock)
      {
        if (this.m_initialized)
          return;
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<IProxyConfigurationInternal>(proxyConfiguration, nameof (proxyConfiguration));
        this.m_proxyConfiguration = proxyConfiguration;
        this.m_nextPersistTime = DateTime.UtcNow.AddHours((double) this.m_proxyConfiguration.StatisticsPersistTime);
        this.Load();
        this.m_initialized = true;
        if (completionCallback == null)
          return;
        try
        {
          this.m_statisticsComputed = new ManualResetEvent(false);
          using (IVssRequestContext systemContext = requestContext.ServiceHost.DeploymentServiceHost.CreateSystemContext())
          {
            this.ScanDisk(systemContext);
            completionCallback(systemContext);
          }
        }
        catch (Exception ex)
        {
          TeamFoundationEventLog.Default.LogException(FrameworkResources.ErrorInitializingCacheStatistics((object) this.m_proxyConfiguration.CacheRoot), ex);
          if (this.m_statisticsComputed == null)
            return;
          this.m_statisticsComputed.Close();
          this.m_statisticsComputed = (ManualResetEvent) null;
        }
      }
    }

    public void Dispose()
    {
      this.m_isShuttingDown = true;
      if (this.m_statisticsComputed != null)
      {
        this.m_statisticsComputed.WaitOne();
        this.m_statisticsComputed.Close();
        this.m_statisticsComputed = (ManualResetEvent) null;
      }
      this.Save("ProxyStatistics.xml");
      GC.SuppressFinalize((object) this);
    }

    public ProxyStatisticsInfo[] Info
    {
      get
      {
        List<ProxyStatisticsInfo> proxyStatisticsInfoList = new List<ProxyStatisticsInfo>();
        try
        {
          this.m_collectionLock.AcquireReaderLock(-1);
          foreach (ProxyStatistics.CollectionConfiguration collectionConfiguration in this.m_configuredCollections.Values)
          {
            if (!string.IsNullOrEmpty(collectionConfiguration.CollectionId))
            {
              ProxyStatisticsInfo proxyStatisticsInfo = new ProxyStatisticsInfo()
              {
                ServerId = collectionConfiguration.CollectionId,
                ServerUrl = collectionConfiguration.ServerUri != null ? collectionConfiguration.ServerUri.ToString() : string.Empty,
                CurrentCacheSize = collectionConfiguration.CurrentCacheSize,
                NoOfRequests = collectionConfiguration.TotalDownloadRequests,
                OverallCacheHits = collectionConfiguration.TotalCacheHits,
                NoOfFilesInCache = collectionConfiguration.TotalFilesinCache,
                ScanComplete = this.m_scanComplete
              };
              proxyStatisticsInfo.OverallCacheMisses = proxyStatisticsInfo.NoOfRequests - proxyStatisticsInfo.OverallCacheHits;
              if (proxyStatisticsInfo.NoOfRequests > 0L)
              {
                proxyStatisticsInfo.CacheHitsPercentage = (float) Math.Round((double) (proxyStatisticsInfo.OverallCacheHits * 100L) / (double) proxyStatisticsInfo.NoOfRequests, 2);
                proxyStatisticsInfo.CacheMissPercentage = (float) Math.Round((double) (proxyStatisticsInfo.OverallCacheMisses * 100L) / (double) proxyStatisticsInfo.NoOfRequests, 2);
              }
              proxyStatisticsInfoList.Add(proxyStatisticsInfo);
            }
          }
        }
        finally
        {
          if (this.m_collectionLock.IsReaderLockHeld)
            this.m_collectionLock.ReleaseReaderLock();
        }
        return proxyStatisticsInfoList.ToArray();
      }
    }

    public long OverallCacheSize => Interlocked.Read(ref this.m_overallCacheSize);

    public long OverallCacheHits => this.m_overallCacheHits;

    public long OverallCacheMisses => this.m_overallCacheMisses;

    public double OverallHitRatio => (double) this.m_overallCacheHits / (double) (this.m_overallCacheHits + this.m_overallCacheMisses);

    public void IncrementDownloads(string serverId, bool cacheHit)
    {
      ProxyStatistics.CollectionConfiguration collection = this.GetCollection(serverId);
      if (collection != null)
      {
        collection.IncrementTotalDownloadRequests();
        if (cacheHit)
          collection.IncrementTotalCacheHits();
      }
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.Proxy.TotalDownloadRequests").Increment();
      if (cacheHit)
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.Proxy.TotalCacheHits").Increment();
        Interlocked.Increment(ref this.m_overallCacheHits);
      }
      else
        Interlocked.Increment(ref this.m_overallCacheMisses);
      this.SaveIfNeeded();
    }

    public void UpdateCacheSize(string serverId, long cacheSize, int fileCount)
    {
      ProxyStatistics.CollectionConfiguration collection = string.IsNullOrEmpty(serverId) ? (ProxyStatistics.CollectionConfiguration) null : this.GetCollection(serverId);
      if (collection != null)
      {
        collection.CurrentCacheSize = Math.Max(0L, collection.CurrentCacheSize + cacheSize);
        collection.TotalFilesinCache = Math.Max(0L, collection.TotalFilesinCache + (long) fileCount);
      }
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.Proxy.CurrentCacheSize").IncrementBy(cacheSize);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.Proxy.TotalFilesInCache").IncrementBy((long) fileCount);
      Interlocked.Add(ref this.m_overallCacheSize, cacheSize);
      this.SaveIfNeeded();
    }

    private ProxyStatistics.CollectionConfiguration GetCollection(string serverId)
    {
      ProxyStatistics.CollectionConfiguration collection1;
      try
      {
        this.m_collectionLock.AcquireReaderLock(-1);
        if (this.m_configuredCollections.TryGetValue(serverId, out collection1))
          return collection1;
      }
      finally
      {
        if (this.m_collectionLock.IsReaderLockHeld)
          this.m_collectionLock.ReleaseReaderLock();
      }
      try
      {
        this.m_collectionLock.AcquireWriterLock(-1);
        if (this.m_configuredCollections.TryGetValue(serverId, out collection1))
          return collection1;
        ProxyStatistics.CollectionConfiguration collection2 = new ProxyStatistics.CollectionConfiguration(serverId, this.m_proxyConfiguration.GetRepositoryLocation(serverId));
        this.m_configuredCollections[serverId] = collection2;
        return collection2;
      }
      finally
      {
        if (this.m_collectionLock.IsWriterLockHeld)
          this.m_collectionLock.ReleaseWriterLock();
      }
    }

    private void Load()
    {
      string str1 = Path.Combine(this.m_proxyConfiguration.CacheRoot, "ProxyStatistics.xml");
      try
      {
        FileInfo fileInfo = new FileInfo(str1);
        if (fileInfo.Exists)
        {
          using (Stream input = (Stream) fileInfo.OpenRead())
          {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
              DtdProcessing = DtdProcessing.Prohibit,
              XmlResolver = (XmlResolver) null
            };
            XPathNavigator navigator1 = new XPathDocument(XmlReader.Create(input, settings)).CreateNavigator();
            XPathNodeIterator xpathNodeIterator = navigator1.Select(navigator1.Compile(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}/{2}", (object) nameof (ProxyStatistics), (object) "Servers", (object) "Server")));
            if (xpathNodeIterator != null)
            {
              foreach (XPathNavigator navigator2 in xpathNodeIterator)
              {
                string serverUri;
                if (ConfigUtil.TryReadNode(navigator2, "Uri", out serverUri))
                {
                  string str2;
                  if (ConfigUtil.TryReadNode(navigator2, "ServerId", out str2))
                  {
                    try
                    {
                      this.m_configuredCollections[str2] = new ProxyStatistics.CollectionConfiguration(str2, serverUri)
                      {
                        TotalDownloadRequests = ConfigUtil.ReadLong(navigator2, "TotalDownloadRequests", 0L, 0L, long.MaxValue),
                        TotalCacheHits = ConfigUtil.ReadLong(navigator2, "TotalCacheHits", 0L, 0L, long.MaxValue)
                      };
                    }
                    catch (Exception ex)
                    {
                      TeamFoundationTrace.TraceException(ex);
                    }
                  }
                  else
                    TeamFoundationEventLog.Default.Log(FrameworkResources.ProxyStatsMissingNode((object) "ServerId"), TeamFoundationEventId.DefaultExceptionEventId, EventLogEntryType.Error);
                }
                else
                  TeamFoundationEventLog.Default.Log(FrameworkResources.ProxyStatsMissingNode((object) "Uri"), TeamFoundationEventId.DefaultExceptionEventId, EventLogEntryType.Error);
              }
            }
          }
        }
      }
      catch (XmlException ex1)
      {
        try
        {
          string str3 = Path.Combine(this.m_proxyConfiguration.CacheRoot, "ProxyStatisticsTemp.xml");
          this.Save("ProxyStatisticsTemp.xml");
          File.Delete(str1);
          File.Copy(str3, str1, true);
          File.Delete(str3);
          TeamFoundationEventLog.Default.LogException(FrameworkResources.ErrorReadingProxyStats(), (Exception) ex1, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        }
        catch (Exception ex2)
        {
          TeamFoundationEventLog.Default.LogException(ex2.Message, ex2, TeamFoundationEventId.DefaultExceptionEventId, EventLogEntryType.Error);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ex.Message, ex, TeamFoundationEventId.DefaultExceptionEventId, EventLogEntryType.Error);
      }
      foreach (ProxyStatistics.CollectionConfiguration collectionConfiguration in this.m_configuredCollections.Values)
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.Proxy.TotalDownloadRequests").IncrementBy(Math.Max(collectionConfiguration.TotalCacheHits, collectionConfiguration.TotalDownloadRequests));
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.Proxy.TotalCacheHits").IncrementBy(Math.Min(collectionConfiguration.TotalCacheHits, collectionConfiguration.TotalDownloadRequests));
      }
    }

    private void Save(string statisticsFile)
    {
      try
      {
        if (!Directory.Exists(this.m_proxyConfiguration.CacheRoot))
          Directory.CreateDirectory(this.m_proxyConfiguration.CacheRoot);
        using (XmlTextWriter xmlTextWriter = new XmlTextWriter(Path.Combine(this.m_proxyConfiguration.CacheRoot, statisticsFile), Encoding.UTF8))
        {
          xmlTextWriter.Formatting = Formatting.Indented;
          xmlTextWriter.WriteStartElement(nameof (ProxyStatistics));
          xmlTextWriter.WriteStartElement("Servers");
          try
          {
            this.m_collectionLock.AcquireReaderLock(-1);
            foreach (ProxyStatistics.CollectionConfiguration collectionConfiguration in this.m_configuredCollections.Values)
            {
              if (!string.IsNullOrEmpty(collectionConfiguration.ServerUri) && !string.IsNullOrEmpty(collectionConfiguration.CollectionId))
              {
                xmlTextWriter.WriteStartElement("Server");
                xmlTextWriter.WriteElementString("Uri", collectionConfiguration.ServerUri.ToString());
                xmlTextWriter.WriteElementString("ServerId", collectionConfiguration.CollectionId);
                xmlTextWriter.WriteElementString("CurrentCacheSize", XmlConvert.ToString(collectionConfiguration.CurrentCacheSize));
                xmlTextWriter.WriteElementString("TotalDownloadRequests", XmlConvert.ToString(collectionConfiguration.TotalDownloadRequests));
                xmlTextWriter.WriteElementString("TotalCacheHits", XmlConvert.ToString(collectionConfiguration.TotalCacheHits));
                xmlTextWriter.WriteElementString("TotalFilesInCache", XmlConvert.ToString(collectionConfiguration.TotalFilesinCache));
                xmlTextWriter.WriteEndElement();
              }
            }
          }
          finally
          {
            if (this.m_collectionLock.IsReaderLockHeld)
              this.m_collectionLock.ReleaseReaderLock();
          }
          xmlTextWriter.WriteEndElement();
          xmlTextWriter.WriteEndElement();
          xmlTextWriter.Close();
        }
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ex.Message, ex, TeamFoundationEventId.DefaultExceptionEventId, EventLogEntryType.Warning);
      }
    }

    private void SaveIfNeeded()
    {
      bool flag = false;
      if (DateTime.UtcNow > this.m_nextPersistTime)
      {
        lock (this.m_saveLock)
        {
          if (DateTime.UtcNow > this.m_nextPersistTime)
          {
            flag = true;
            this.m_nextPersistTime = DateTime.UtcNow.AddHours((double) this.m_proxyConfiguration.StatisticsPersistTime);
          }
        }
      }
      if (!flag)
        return;
      this.Save("ProxyStatistics.xml");
    }

    private void ScanDisk(IVssRequestContext requestContext)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      long num1 = 0;
      int num2 = 0;
      int num3 = 0;
      try
      {
        requestContext.Trace(12050, TraceLevel.Info, "FileCache", "FileCacheStatistics", "Starting file cache scanning in " + this.m_proxyConfiguration.CacheRoot);
        if (Directory.Exists(this.m_proxyConfiguration.CacheRoot))
        {
          List<DirectoryInfo> levelDirectories = FileCacheHelper.GetTopLevelDirectories(this.m_proxyConfiguration.CacheRoot);
          for (int index = 0; index < levelDirectories.Count; ++index)
          {
            if (this.m_isShuttingDown || requestContext.IsCanceled)
            {
              TeamFoundationEventLog.Default.Log(FrameworkResources.InfoFileCacheShutsDownWhileStatisticInitializing((object) this.m_proxyConfiguration.CacheRoot), TeamFoundationEventId.DefaultInformationalEventId, EventLogEntryType.Information);
              return;
            }
            DirectoryScanResult directoryScanResult = FileCacheHelper.ScanDirectory(levelDirectories[index].FullName, requestContext, this.m_proxyConfiguration.CacheDirectoryDepthLimit);
            if (directoryScanResult.Success)
            {
              ProxyStatistics.CollectionConfiguration collection = this.GetCollection(levelDirectories[index].Name);
              collection.TotalFilesinCache += (long) directoryScanResult.FileCount;
              VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.Proxy.TotalFilesInCache").IncrementBy((long) directoryScanResult.FileCount);
              collection.CurrentCacheSize += directoryScanResult.DirectorySize;
              VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.Proxy.CurrentCacheSize").IncrementBy(directoryScanResult.DirectorySize);
            }
            Interlocked.Add(ref this.m_overallCacheSize, directoryScanResult.DirectorySize);
            requestContext.Trace(12051, TraceLevel.Info, "FileCache", "FileCacheStatistics", string.Format("Scanned {0}. We found {1} files for a total of {2} bytes", (object) levelDirectories[index].FullName, (object) directoryScanResult.FileCount, (object) directoryScanResult.DirectorySize.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture)));
            num1 += directoryScanResult.DirectorySize;
            num2 += directoryScanResult.FileCount;
            ++num3;
            foreach (string skippedDirectory in directoryScanResult.SkippedDirectories)
            {
              try
              {
                requestContext.Trace(12226, TraceLevel.Warning, "FileCache", "FileCacheStatistics", string.Format("The directory '{0}' is below the max scan level {1}. Attempting to delete it.", (object) skippedDirectory, (object) this.m_proxyConfiguration.CacheDirectoryDepthLimit));
                Directory.Delete(skippedDirectory, true);
              }
              catch (Exception ex)
              {
                requestContext.Trace(12227, TraceLevel.Warning, "FileCache", "FileCacheStatistics", string.Format("Failed to delete the directory '{0}' that is below the max scan level {1}. The final OverallCacheSize value may be incorrect. Exception: {2}", (object) skippedDirectory, (object) this.m_proxyConfiguration.CacheDirectoryDepthLimit, (object) ex));
              }
            }
          }
        }
        this.m_scanComplete = true;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(FrameworkResources.ErrorComputingFolderStatistics((object) this.m_proxyConfiguration.CacheRoot), ex);
      }
      finally
      {
        stopwatch.Stop();
        TeamFoundationEventLog.Default.Log(FrameworkResources.FileCacheScanCompleted((object) this.m_proxyConfiguration.CacheRoot, (object) stopwatch.ElapsedMilliseconds.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture), this.m_scanComplete ? (object) "Complete" : (object) "Incomplete", (object) num3, (object) num2.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture), (object) num1.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture)), TeamFoundationEventId.DefaultInformationalEventId, EventLogEntryType.Information);
        if (this.m_statisticsComputed != null)
          this.m_statisticsComputed.Set();
      }
    }

    private class CollectionConfiguration
    {
      private long m_totalDownloadRequests;
      private long m_totalCacheHits;

      internal CollectionConfiguration(string collectionId, string serverUri)
      {
        this.CollectionId = collectionId;
        this.ServerUri = serverUri;
      }

      internal string CollectionId { get; private set; }

      internal string ServerUri { get; private set; }

      internal long TotalCacheHits
      {
        get => this.m_totalCacheHits;
        set => this.m_totalCacheHits = value;
      }

      internal long TotalFilesinCache { get; set; }

      internal long TotalDownloadRequests
      {
        get => this.m_totalDownloadRequests;
        set => this.m_totalDownloadRequests = value;
      }

      internal long CurrentCacheSize { get; set; }

      internal void IncrementTotalDownloadRequests() => Interlocked.Increment(ref this.m_totalDownloadRequests);

      internal void IncrementTotalCacheHits() => Interlocked.Increment(ref this.m_totalCacheHits);
    }
  }
}
