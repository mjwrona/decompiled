// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.ShardEstimateChange
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public class ShardEstimateChange
  {
    public ShardEstimateChange(
      string esCluster,
      IEntityType entityType,
      string indexName,
      short shardId,
      int changeInEstimatedDocCount,
      int changeInEstimatedDocCountGrowth,
      int changeInReservedDocCount,
      long changeInEstimatedSize,
      long changeInEstimatedSizeGrowth,
      long changeInReservedSpace,
      int originalEstimatedDocCount = 0,
      int originalEstimatedDocCountGrowth = 0,
      int originalReservedDocCount = 0)
    {
      this.EsCluster = esCluster;
      this.EntityType = entityType;
      this.IndexName = indexName;
      this.ShardId = shardId;
      this.ChangeInEstimatedDocCount = changeInEstimatedDocCount;
      this.ChangeInEstimatedDocCountGrowth = changeInEstimatedDocCountGrowth;
      this.ChangeInReservedDocCount = changeInReservedDocCount;
      this.ChangeInEstimatedSize = changeInEstimatedSize;
      this.ChangeInEstimatedSizeGrowth = changeInEstimatedSizeGrowth;
      this.ChangeInReservedSpace = changeInReservedSpace;
      this.OriginalEstimatedDocCount = originalEstimatedDocCount;
      this.OriginalEstimatedDocCountGrowth = originalEstimatedDocCountGrowth;
      this.OriginalReservedDocCount = originalReservedDocCount;
    }

    public string EsCluster { get; }

    public IEntityType EntityType { get; }

    public string IndexName { get; }

    public short ShardId { get; }

    public int ChangeInEstimatedDocCount { get; set; }

    public int ChangeInEstimatedDocCountGrowth { get; set; }

    public int ChangeInReservedDocCount { get; set; }

    public long ChangeInEstimatedSize { get; set; }

    public long ChangeInEstimatedSizeGrowth { get; set; }

    public long ChangeInReservedSpace { get; set; }

    public int OriginalEstimatedDocCount { get; set; }

    public int OriginalEstimatedDocCountGrowth { get; set; }

    public int OriginalReservedDocCount { get; set; }
  }
}
