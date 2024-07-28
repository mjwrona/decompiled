// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation.L1HitRateCounter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation
{
  public class L1HitRateCounter : HitRateCounterBase
  {
    public L1HitRateCounter()
      : base(ProfilingGroup.Cache, "L1HitRate")
    {
    }

    internal override double GetPercentage(CacheStats stats) => (double) stats.L1Hits * 100.0 / (double) (stats.L1Hits + stats.L1Misses);
  }
}
