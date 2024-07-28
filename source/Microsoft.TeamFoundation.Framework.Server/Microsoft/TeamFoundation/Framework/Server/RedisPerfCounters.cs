// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RedisPerfCounters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class RedisPerfCounters
  {
    private const string UriBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_";
    internal const string TotalCalls = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalCalls";
    internal const string CallsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_CallsPerSec";
    internal const string TotalCacheHits = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalCacheHits";
    internal const string CacheHitsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_CacheHitsPerSec";
    internal const string TotalCacheMisses = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalCacheMisses";
    internal const string CacheMissesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_CacheMissesPerSec";
    internal const string TotalKeyInvalidations = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalKeyInvalidations";
    internal const string KeyInvalidationsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_KeyInvalidationsPerSec";
    internal const string AverageCallTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_AverageCallTime";
    internal const string AverageCallTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_AverageCallTimeBase";
    internal const string TotalFailedTransactions = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalFailedTransactions";
    internal const string FailedTransactionsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_FailedTransactionsPerSec";
    internal const string TotalKeyExpirations = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalKeyExpirations";
    internal const string KeyExpirationsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_KeyExpirationsPerSec";
    internal const string TotalKeyEvictions = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalKeyEvictions";
    internal const string KeyEvictionsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_KeyEvictionsPerSec";
    internal const string CpuUsage = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_CpuUsage";
    internal const string MemoryUsage = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_MemoryUsage";
    internal const string AverageValueSize = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_AverageValueSize";
    internal const string AverageValueSizeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_AverageValueSizeBase";
    internal const string TotalExceptions = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalExceptions";
    internal const string ExceptionsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_ExceptionsPerSec";
    internal const string TotalSlowCalls = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalSlowCalls";
    internal const string SlowCallsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Redis_SlowCallsPerSec";
  }
}
