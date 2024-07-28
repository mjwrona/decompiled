// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EsShardDetails
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public class EsShardDetails
  {
    private readonly int m_actualDocCount;
    private readonly int m_deletedDocCount;
    private readonly long m_actualSize;
    private readonly short m_shardId;
    private readonly string m_esClusterName;
    private readonly string m_indexName;

    public EsShardDetails(
      string esClusterName,
      string indexName,
      short shardId,
      int actualDocCount,
      int deletedDocCount,
      long actualSize)
    {
      this.m_esClusterName = esClusterName;
      this.m_indexName = indexName;
      this.m_shardId = shardId;
      this.m_deletedDocCount = deletedDocCount;
      this.m_actualDocCount = actualDocCount;
      this.m_actualSize = actualSize;
    }

    public string EsClusterName => this.m_esClusterName;

    public string IndexName => this.m_indexName;

    public short ShardId => this.m_shardId;

    public int ActualDocCount => this.m_actualDocCount;

    public int DeletedDocCount => this.m_deletedDocCount;

    public long ActualSize => this.m_actualSize;
  }
}
