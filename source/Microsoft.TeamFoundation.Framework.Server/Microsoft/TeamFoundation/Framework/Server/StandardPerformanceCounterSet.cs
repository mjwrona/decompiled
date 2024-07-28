// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StandardPerformanceCounterSet
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct StandardPerformanceCounterSet
  {
    public StandardPerformanceCounterSet(
      string totalCalls,
      string callsPerSec,
      string averageCallTime,
      string averageCallTimeBase,
      string totalSlowCalls,
      string slowCallsPerSec,
      string totalExceptions,
      string exceptionsPerSec,
      string totalFailedOperations,
      string failedOperationsPerSec,
      string totalCacheHits,
      string cacheHitsPerSec,
      string totalCacheMisses,
      string cacheMissesPerSec,
      string totalCacheInvalidations,
      string cacheInvalidationsPerSec)
    {
      ArgumentUtility.CheckForNull<string>(totalCalls, nameof (totalCalls));
      this.TotalCalls = totalCalls;
      ArgumentUtility.CheckForNull<string>(callsPerSec, nameof (callsPerSec));
      this.CallsPerSec = callsPerSec;
      ArgumentUtility.CheckForNull<string>(averageCallTime, nameof (averageCallTime));
      this.AverageCallTime = averageCallTime;
      ArgumentUtility.CheckForNull<string>(averageCallTimeBase, nameof (averageCallTimeBase));
      this.AverageCallTimeBase = averageCallTimeBase;
      ArgumentUtility.CheckForNull<string>(totalSlowCalls, nameof (totalSlowCalls));
      this.TotalSlowCalls = totalSlowCalls;
      ArgumentUtility.CheckForNull<string>(slowCallsPerSec, nameof (slowCallsPerSec));
      this.SlowCallsPerSec = slowCallsPerSec;
      ArgumentUtility.CheckForNull<string>(totalExceptions, nameof (totalExceptions));
      this.TotalExceptions = totalExceptions;
      ArgumentUtility.CheckForNull<string>(exceptionsPerSec, nameof (exceptionsPerSec));
      this.ExceptionsPerSec = exceptionsPerSec;
      ArgumentUtility.CheckForNull<string>(totalFailedOperations, nameof (totalFailedOperations));
      this.TotalFailedOperations = totalFailedOperations;
      ArgumentUtility.CheckForNull<string>(failedOperationsPerSec, nameof (failedOperationsPerSec));
      this.FailedOperationsPerSec = failedOperationsPerSec;
      ArgumentUtility.CheckForNull<string>(totalCacheHits, nameof (totalCacheHits));
      this.TotalCacheHits = totalCacheHits;
      ArgumentUtility.CheckForNull<string>(cacheHitsPerSec, nameof (cacheHitsPerSec));
      this.CacheHitsPerSec = cacheHitsPerSec;
      ArgumentUtility.CheckForNull<string>(totalCacheMisses, nameof (totalCacheMisses));
      this.TotalCacheMisses = totalCacheMisses;
      ArgumentUtility.CheckForNull<string>(cacheMissesPerSec, nameof (cacheMissesPerSec));
      this.CacheMissesPerSec = cacheMissesPerSec;
      ArgumentUtility.CheckForNull<string>(totalCacheInvalidations, nameof (totalCacheInvalidations));
      this.TotalCacheInvalidations = totalCacheInvalidations;
      ArgumentUtility.CheckForNull<string>(cacheInvalidationsPerSec, nameof (cacheInvalidationsPerSec));
      this.CacheInvalidationsPerSec = cacheInvalidationsPerSec;
    }

    public string TotalCalls { get; }

    public string CallsPerSec { get; }

    public string AverageCallTime { get; }

    public string AverageCallTimeBase { get; }

    public string TotalSlowCalls { get; }

    public string SlowCallsPerSec { get; }

    public string TotalExceptions { get; }

    public string ExceptionsPerSec { get; }

    public string TotalFailedOperations { get; }

    public string FailedOperationsPerSec { get; }

    public string TotalCacheHits { get; }

    public string CacheHitsPerSec { get; }

    public string TotalCacheMisses { get; }

    public string CacheMissesPerSec { get; }

    public string TotalCacheInvalidations { get; }

    public string CacheInvalidationsPerSec { get; }
  }
}
