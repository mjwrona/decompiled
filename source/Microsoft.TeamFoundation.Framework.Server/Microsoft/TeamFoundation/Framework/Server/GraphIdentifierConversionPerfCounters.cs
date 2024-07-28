// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GraphIdentifierConversionPerfCounters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class GraphIdentifierConversionPerfCounters
  {
    private const string UriBase = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_";
    internal const string TotalCalls = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_TotalCalls";
    internal const string CallsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_CallsPerSec";
    internal const string AverageCallTime = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_AverageCallTime";
    internal const string AverageCallTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_AverageCallTimeBase";
    internal const string TotalSlowCalls = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_TotalSlowCalls";
    internal const string SlowCallsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_SlowCallsPerSec";
    internal const string TotalExceptions = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_TotalExceptions";
    internal const string ExceptionsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_ExceptionsPerSec";
    internal const string TotalFailedOperations = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_TotalFailedOperations";
    internal const string FailedOperationsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_FailedOperationsPerSec";
    internal const string TotalCacheHits = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_TotalCacheHits";
    internal const string CacheHitsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_CacheHitsPerSec";
    internal const string TotalCacheMisses = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_TotalCacheMisses";
    internal const string CacheMissesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_CacheMissesPerSec";
    internal const string TotalCacheInvalidations = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_TotalCacheInvalidations";
    internal const string CacheInvalidationsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_CacheInvalidationsPerSec";
    public static readonly StandardPerformanceCounterSet StandardSet = new StandardPerformanceCounterSet("Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_TotalCalls", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_CallsPerSec", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_AverageCallTime", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_AverageCallTimeBase", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_TotalSlowCalls", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_SlowCallsPerSec", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_TotalExceptions", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_ExceptionsPerSec", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_TotalFailedOperations", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_FailedOperationsPerSec", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_TotalCacheHits", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_CacheHitsPerSec", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_TotalCacheMisses", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_CacheMissesPerSec", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_TotalCacheInvalidations", "Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_CacheInvalidationsPerSec");
  }
}
