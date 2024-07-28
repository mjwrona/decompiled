// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility.ProfilingCategory
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility
{
  public class ProfilingCategory : IProfilingCategory
  {
    public static readonly ProfilingCategory Agg_FetchMetadataCount = new ProfilingCategory(ProfilingType.AggregateCounter, ProfilingGroup.Performance, "Fetching#");
    public static readonly ProfilingCategory Avg_FetchMetadataTime = new ProfilingCategory(ProfilingType.AverageCounter, ProfilingGroup.Performance, "FetchAvgTime");
    public static readonly ProfilingCategory Agg_ModifyMetadataCount = new ProfilingCategory(ProfilingType.AggregateCounter, ProfilingGroup.Performance, "Modifying#");
    public static readonly ProfilingCategory Avg_ModifyMetadataTime = new ProfilingCategory(ProfilingType.AverageCounter, ProfilingGroup.Performance, "ModifyAvgTime");
    public static readonly ProfilingCategory Man_QueuedForProcessingCount = new ProfilingCategory(ProfilingType.ManualCounter, ProfilingGroup.Performance, "Queued#");
    public static readonly ProfilingCategory Agg_ProcessingCount = new ProfilingCategory(ProfilingType.AggregateCounter, ProfilingGroup.Performance, "Processing#");
    public static readonly ProfilingCategory Avg_ProcessDedupLinkTime = new ProfilingCategory(ProfilingType.AverageCounter, ProfilingGroup.Performance, "ProcessAvgTime");
    public static readonly ProfilingCategory Man_RootCount = new ProfilingCategory(ProfilingType.ManualCounter, ProfilingGroup.Performance, "Root#");
    public static readonly ProfilingCategory Agg_L1MemoryCacheHits = new ProfilingCategory(ProfilingType.AccumulativeCounter, ProfilingGroup.Cache, "L1Hits#");
    public static readonly ProfilingCategory Agg_L1MemoryCacheMisses = new ProfilingCategory(ProfilingType.AccumulativeCounter, ProfilingGroup.Cache, "L1Misses#");
    public static readonly ProfilingCategory Agg_L2RedisCacheHits = new ProfilingCategory(ProfilingType.AccumulativeCounter, ProfilingGroup.Cache, "L2Hits#");
    public static readonly ProfilingCategory Agg_L2RedisCacheMisses = new ProfilingCategory(ProfilingType.AccumulativeCounter, ProfilingGroup.Cache, "L2Misses#");
    public static readonly ProfilingCategory Agg_DeleteCount = new ProfilingCategory(ProfilingType.AggregateCounter, ProfilingGroup.Performance, "Deleting#");
    public static readonly ProfilingCategory Avg_DeleteTime = new ProfilingCategory(ProfilingType.AverageCounter, ProfilingGroup.Performance, "DeleteAvgTime");
    public static readonly ProfilingCategory Avg_BatchDeleteTime = new ProfilingCategory(ProfilingType.AverageCounter, ProfilingGroup.Performance, "BatchDeleteAvgTime");
    public static readonly ProfilingCategory Avg_QueueingForDeletingTime = new ProfilingCategory(ProfilingType.AverageCounter, ProfilingGroup.Performance, "QueueAvgTime");

    public ProfilingGroup Group { get; private set; }

    public string Name { get; private set; }

    public ProfilingType Type { get; private set; }

    private ProfilingCategory(ProfilingType type, ProfilingGroup group, string name)
    {
      this.Type = type;
      this.Group = group;
      this.Name = name;
    }
  }
}
