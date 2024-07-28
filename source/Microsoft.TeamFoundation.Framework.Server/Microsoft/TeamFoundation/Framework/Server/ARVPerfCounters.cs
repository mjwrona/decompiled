// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ARVPerfCounters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class ARVPerfCounters
  {
    private const string UriBase = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_";
    internal const string CacheHits = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_cache_hits";
    internal const string CacheMisses = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_cache_misses";
    internal const string RequestsPerSec = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_request_persec";
    internal const string SkippedValidations = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_skipped";
    internal const string AuthoritativeRequests = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_authoritative";
    internal const string CacheMissesPerSec = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_cache_misses_persec";
    internal const string NonAuthoritativeRequests = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_nonauthoritative";
    internal const string CacheInvalidations = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_invalidated";
    internal const string CacheInvalidationsPerSec = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_invalidated_persec";
    internal const string L2CacheHits = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_l2_cache_hits";
    internal const string L2CacheMisses = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_l2_cache_misses";
    internal const string L2CachePutsPerSec = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_l2_cache_puts_persec";
    internal const string L2CacheGetsPerSec = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_l2_cache_gets_persec";
    internal const string L2CacheRemovesPerSec = "Microsoft.VisualStudio.Services.Compliance.Perf_arv_l2_cache_removes_persec";
  }
}
