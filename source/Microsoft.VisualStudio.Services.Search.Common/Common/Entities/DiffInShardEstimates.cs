// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.DiffInShardEstimates
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public class DiffInShardEstimates
  {
    public DiffInShardEstimates(
      string esCluster,
      IEntityType entityType,
      string indexName,
      short shardId,
      int diffInEstimatedDocCount,
      int diffInEstimatedDocCountGrowth,
      int diffInReservedDocCount)
    {
      this.EsCluster = esCluster;
      this.EntityType = entityType;
      this.IndexName = indexName;
      this.ShardId = shardId;
      this.DiffInEstimatedDocCount = diffInEstimatedDocCount;
      this.DiffInEstimatedDocCountGrowth = diffInEstimatedDocCountGrowth;
      this.DiffInReservedDocCount = diffInReservedDocCount;
    }

    public string EsCluster { get; }

    public IEntityType EntityType { get; }

    public string IndexName { get; }

    public short ShardId { get; }

    public int DiffInEstimatedDocCount { get; set; }

    public int DiffInEstimatedDocCountGrowth { get; set; }

    public int DiffInReservedDocCount { get; set; }
  }
}
