// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation.HitRateCounterBase
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation
{
  public abstract class HitRateCounterBase : DerivedCounter
  {
    public ProfilingGroup Group { get; private set; }

    public string Name { get; private set; }

    public HitRateCounterBase(ProfilingGroup group, string name)
    {
      this.Group = group;
      this.Name = name;
    }

    public string GetValue(ICounterLookup lookup)
    {
      double percentage = this.GetPercentage(this.GetStats(lookup));
      return percentage >= 0.0 && percentage <= 100.0 ? string.Format("{0:0.00}%", (object) percentage) : string.Format("invalid({0})", (object) percentage);
    }

    internal abstract double GetPercentage(CacheStats stats);

    private CacheStats GetStats(ICounterLookup lookup)
    {
      long negativeLongTotal1 = HitRateCounterBase.GetNonNegativeLongTotal(lookup, ProfilingCategory.Agg_L1MemoryCacheHits);
      long negativeLongTotal2 = HitRateCounterBase.GetNonNegativeLongTotal(lookup, ProfilingCategory.Agg_L1MemoryCacheMisses);
      long negativeLongTotal3 = HitRateCounterBase.GetNonNegativeLongTotal(lookup, ProfilingCategory.Agg_L2RedisCacheHits);
      long negativeLongTotal4 = HitRateCounterBase.GetNonNegativeLongTotal(lookup, ProfilingCategory.Agg_L2RedisCacheMisses);
      return new CacheStats()
      {
        L1Hits = negativeLongTotal1,
        L1Misses = negativeLongTotal2,
        L2Hits = negativeLongTotal3,
        L2Misses = negativeLongTotal4
      };
    }

    private static long GetNonNegativeLongTotal(ICounterLookup lookup, ProfilingCategory category)
    {
      long valueOrDefault = lookup.GetBy((IProfilingCategory) category, ProfilingResultType.Total).GetValueOrDefault();
      return valueOrDefault < 0L ? 0L : valueOrDefault;
    }
  }
}
