// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation.TwoLevelDedupProcessingCache
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using Microsoft.VisualStudio.Services.Content.Server.Common;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation
{
  public class TwoLevelDedupProcessingCache : IDedupProcessingCache
  {
    private IDedupProcessingCache l1Cache;
    private IDedupProcessingCache l2Cache;
    private IOperationProfiler m_profiler;

    public TwoLevelDedupProcessingCache(
      IVssRequestContext requestContext,
      IOperationProfiler profiler,
      int dedupGCRedisMaxBufferCount)
    {
      this.m_profiler = profiler;
      requestContext.GetService<IVssRegistryService>();
      this.l1Cache = (IDedupProcessingCache) requestContext.GetService<DedupProcessingCacheService>();
      this.l2Cache = (IDedupProcessingCache) new DedupRedisCache(requestContext, dedupGCRedisMaxBufferCount);
    }

    public void AddValidatedParentChild(DedupIdentifier parent, DedupIdentifier child)
    {
      this.l1Cache.AddValidatedParentChild(parent, child);
      this.l2Cache.AddValidatedParentChild(parent, child);
    }

    public IDedupInfo GetDedupInfo(VssRequestPump.Processor processor, DedupIdentifier dedupId)
    {
      IDedupInfo dedupInfo1 = this.l1Cache.GetDedupInfo(processor, dedupId);
      if (dedupInfo1 != null)
      {
        this.m_profiler.Increment(ProfilingCategory.Agg_L1MemoryCacheHits);
        return dedupInfo1;
      }
      this.m_profiler.Increment(ProfilingCategory.Agg_L1MemoryCacheMisses);
      IDedupInfo dedupInfo2 = this.l2Cache.GetDedupInfo(processor, dedupId);
      if (dedupInfo2 != null)
      {
        this.m_profiler.Increment(ProfilingCategory.Agg_L2RedisCacheHits);
        this.l1Cache.SetDedupInfo(processor, dedupId, dedupInfo2);
      }
      else
        this.m_profiler.Increment(ProfilingCategory.Agg_L2RedisCacheMisses);
      return dedupInfo2;
    }

    public void SetDedupInfo(
      VssRequestPump.Processor processor,
      DedupIdentifier dedupId,
      IDedupInfo info)
    {
      this.l1Cache.SetDedupInfo(processor, dedupId, info);
      this.l2Cache.SetDedupInfo(processor, dedupId, info);
    }

    public bool IsParentChildValidated(DedupIdentifier parent, DedupIdentifier child) => this.l1Cache.IsParentChildValidated(parent, child) || this.l2Cache.IsParentChildValidated(parent, child);

    public void Reset()
    {
      this.l1Cache.Reset();
      this.l2Cache.Reset();
    }
  }
}
