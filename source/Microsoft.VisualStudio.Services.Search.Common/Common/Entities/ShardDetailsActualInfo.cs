// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.ShardDetailsActualInfo
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public class ShardDetailsActualInfo
  {
    public ShardDetailsActualInfo(
      string esCluster,
      string indexName,
      short shardId,
      int actualDocCount,
      int deletedDocCount,
      long actualSize)
    {
      this.EsCluster = esCluster;
      this.IndexName = indexName;
      this.ShardId = shardId;
      this.ActualDocCount = actualDocCount;
      this.DeletedDocCount = deletedDocCount;
      this.ActualSize = actualSize;
    }

    public string EsCluster { get; }

    public string IndexName { get; }

    public short ShardId { get; }

    public int ActualDocCount { get; }

    public int DeletedDocCount { get; }

    public long ActualSize { get; }
  }
}
