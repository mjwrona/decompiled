// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssCachePerformanceProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssCachePerformanceProvider : IVssCachePerformanceProvider
  {
    private static readonly IVssCachePerformanceProvider s_emptyProvider = (IVssCachePerformanceProvider) new VssCachePerformanceProvider.EmptyProvider();
    private readonly VssPerformanceCounter m_cacheCount;
    private readonly VssPerformanceCounter m_cacheSize;
    private readonly VssPerformanceCounter m_totalHits;
    private readonly VssPerformanceCounter m_hitsPerSec;
    private readonly VssPerformanceCounter m_totalMisses;
    private readonly VssPerformanceCounter m_missesPerSec;
    private readonly VssPerformanceCounter m_totalInserts;
    private readonly VssPerformanceCounter m_insertsPerSec;
    private readonly VssPerformanceCounter m_totalUpdates;
    private readonly VssPerformanceCounter m_updatesPerSec;
    private readonly VssPerformanceCounter m_totalDeletes;
    private readonly VssPerformanceCounter m_deletesPerSec;
    private readonly VssPerformanceCounter m_totalEvictions;
    private readonly VssPerformanceCounter m_evictionsPerSec;
    private readonly VssPerformanceCounter m_totalInvalidations;
    private readonly VssPerformanceCounter m_invalidationsPerSec;
    private readonly VssPerformanceCounter m_totalCleanups;
    private readonly VssPerformanceCounter m_cleanupsPerSec;
    private readonly VssPerformanceCounter m_totalResets;
    private readonly VssPerformanceCounter m_resetsPerSec;
    private readonly VssPerformanceCounter m_totalReadTime;
    private readonly VssPerformanceCounter m_totalWriteTime;

    public VssCachePerformanceProvider(string cacheName)
    {
      this.m_cacheCount = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_TotalCount", cacheName);
      this.m_cacheSize = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_TotalSize", cacheName);
      this.m_totalHits = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_TotalHits", cacheName);
      this.m_hitsPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_HitsPerSec", cacheName);
      this.m_totalMisses = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_TotalMisses", cacheName);
      this.m_missesPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_MissesPerSec", cacheName);
      this.m_totalInserts = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_TotalInserts", cacheName);
      this.m_insertsPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_InsertsPerSec", cacheName);
      this.m_totalUpdates = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_TotalUpdates", cacheName);
      this.m_updatesPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_UpdatesPerSec", cacheName);
      this.m_totalDeletes = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_TotalDeletes", cacheName);
      this.m_deletesPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_DeletesPerSec", cacheName);
      this.m_totalEvictions = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_TotalEvictions", cacheName);
      this.m_evictionsPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_EvictionsPerSec", cacheName);
      this.m_totalInvalidations = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_TotalInvalidations", cacheName);
      this.m_invalidationsPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_InvalidationsPerSec", cacheName);
      this.m_totalCleanups = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_TotalCleanups", cacheName);
      this.m_cleanupsPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_CleanupsPerSec", cacheName);
      this.m_totalResets = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_TotalResets", cacheName);
      this.m_resetsPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_ResetsPerSec", cacheName);
      this.m_totalReadTime = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_TotalReadTime", cacheName);
      this.m_totalWriteTime = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_MemoryCache_TotalWriteTime", cacheName);
    }

    public static IVssCachePerformanceProvider NoProvider => VssCachePerformanceProvider.s_emptyProvider;

    public virtual void NotifyCacheLookupSucceeded()
    {
      this.m_totalHits.Increment();
      this.m_hitsPerSec.Increment();
    }

    public virtual void NotifyCacheLookupFailed()
    {
      this.m_totalMisses.Increment();
      this.m_missesPerSec.Increment();
    }

    public virtual void NotifyCacheItemsAdded(int count, MemoryCacheOperationStatistics stats)
    {
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 1);
      ArgumentUtility.CheckForOutOfRange(stats.CountDiff, "countDiff", 1);
      ArgumentUtility.CheckForOutOfRange(stats.SizeDiff, "sizeDiff", 0L);
      this.m_cacheCount.IncrementBy((long) stats.CountDiff);
      this.m_cacheSize.IncrementBy(stats.SizeDiff);
      this.m_totalInserts.IncrementBy((long) count);
      this.m_insertsPerSec.IncrementBy((long) count);
    }

    public virtual void NotifyCacheItemsReplaced(int count, MemoryCacheOperationStatistics stats)
    {
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 1);
      ArgumentUtility.CheckForOutOfRange(stats.CountDiff, "countDiff", 0, 0);
      this.m_cacheSize.IncrementBy(stats.SizeDiff);
      this.m_totalUpdates.IncrementBy((long) count);
      this.m_updatesPerSec.IncrementBy((long) count);
    }

    public virtual void NotifyCacheItemsRemoved(int count, MemoryCacheOperationStatistics stats)
    {
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 1);
      ArgumentUtility.CheckForOutOfRange(stats.CountDiff, "countDiff", int.MinValue, -1);
      ArgumentUtility.CheckForOutOfRange(stats.SizeDiff, "sizeDiff", long.MinValue, 0L);
      this.m_cacheCount.IncrementBy((long) stats.CountDiff);
      this.m_cacheSize.IncrementBy(stats.SizeDiff);
      this.m_totalDeletes.IncrementBy((long) count);
      this.m_deletesPerSec.IncrementBy((long) count);
    }

    public virtual void NotifyCacheItemsEvicted(int count, MemoryCacheOperationStatistics stats)
    {
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 1);
      ArgumentUtility.CheckForOutOfRange(stats.CountDiff, "countDiff", int.MinValue, 0);
      ArgumentUtility.CheckForOutOfRange(stats.SizeDiff, "sizeDiff", long.MinValue, 0L);
      this.m_cacheCount.IncrementBy((long) stats.CountDiff);
      this.m_cacheSize.IncrementBy(stats.SizeDiff);
      this.m_totalEvictions.IncrementBy((long) count);
      this.m_evictionsPerSec.IncrementBy((long) count);
    }

    public virtual void NotifyCacheItemsInvalidated(int count, MemoryCacheOperationStatistics stats)
    {
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 1);
      ArgumentUtility.CheckForOutOfRange(stats.CountDiff, "countDiff", int.MinValue, -1);
      ArgumentUtility.CheckForOutOfRange(stats.SizeDiff, "sizeDiff", long.MinValue, 0L);
      this.m_cacheCount.IncrementBy((long) stats.CountDiff);
      this.m_cacheSize.IncrementBy(stats.SizeDiff);
      this.m_totalInvalidations.IncrementBy((long) count);
      this.m_invalidationsPerSec.IncrementBy((long) count);
    }

    public virtual void NotifyCacheCleared(MemoryCacheOperationStatistics stats)
    {
      ArgumentUtility.CheckForOutOfRange(stats.CountDiff, "countDiff", int.MinValue, 0);
      ArgumentUtility.CheckForOutOfRange(stats.SizeDiff, "sizeDiff", long.MinValue, 0L);
      this.m_cacheCount.IncrementBy((long) stats.CountDiff);
      this.m_cacheSize.IncrementBy(stats.SizeDiff);
      this.m_totalCleanups.Increment();
      this.m_cleanupsPerSec.Increment();
    }

    public virtual void NotifyCacheReset()
    {
      this.m_totalResets.Increment();
      this.m_resetsPerSec.Increment();
    }

    public void IncrementCacheRead(long elapsedMicroseconds)
    {
      ArgumentUtility.CheckForOutOfRange(elapsedMicroseconds, nameof (elapsedMicroseconds), 0L);
      this.m_totalReadTime.IncrementMicroseconds(elapsedMicroseconds);
    }

    public void IncrementCacheWrite(long elapsedMicroseconds)
    {
      ArgumentUtility.CheckForOutOfRange(elapsedMicroseconds, nameof (elapsedMicroseconds), 0L);
      this.m_totalWriteTime.IncrementMicroseconds(elapsedMicroseconds);
    }

    private class EmptyProvider : IVssCachePerformanceProvider
    {
      public void NotifyCacheLookupSucceeded()
      {
      }

      public void NotifyCacheLookupFailed()
      {
      }

      public void NotifyCacheItemsAdded(int count, MemoryCacheOperationStatistics stats)
      {
      }

      public void NotifyCacheItemsReplaced(int count, MemoryCacheOperationStatistics stats)
      {
      }

      public void NotifyCacheItemsRemoved(int count, MemoryCacheOperationStatistics stats)
      {
      }

      public void NotifyCacheItemsEvicted(int count, MemoryCacheOperationStatistics stats)
      {
      }

      public void NotifyCacheItemsInvalidated(int count, MemoryCacheOperationStatistics stats)
      {
      }

      public void NotifyCacheCleared(MemoryCacheOperationStatistics stats)
      {
      }

      public void NotifyCacheReset()
      {
      }

      public void IncrementCacheRead(long elapsedMicroseconds)
      {
      }

      public void IncrementCacheWrite(long elapsedMicroseconds)
      {
      }
    }
  }
}
