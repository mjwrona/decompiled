// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.IndexingUnitDetails
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public class IndexingUnitDetails
  {
    private string m_indexingUnitTypeExtended;
    private string m_indexingUnitType;

    public IndexingUnitDetails(
      int indexingUnitId,
      Guid tfsEntityId,
      IEntityType entityType,
      string indexingUnitType,
      bool isShadow,
      List<int> shardIds,
      List<string> routingIds,
      int currentEstimatedDocCount,
      int estimatedDocCountGrowth,
      float shardDensity,
      string esClusterName,
      string indexName,
      long actualInitialSize = 0,
      int actualInitialDocCount = 0)
    {
      int indexingUnitId1 = indexingUnitId;
      Guid? nullable = new Guid?(tfsEntityId);
      string routingIds1 = routingIds.IsNullOrEmpty<string>() ? (string) null : string.Join(",", (IEnumerable<string>) routingIds);
      string shardIds1 = shardIds.IsNullOrEmpty<int>() ? (string) null : string.Join<int>(",", (IEnumerable<int>) shardIds);
      int estimatedInitialDocCount = currentEstimatedDocCount;
      int actualInitialDocCount1 = actualInitialDocCount;
      int estimatedDocCountGrowth1 = estimatedDocCountGrowth;
      long estimatedInitialSize = (long) (1073741824.0 * (double) currentEstimatedDocCount / (double) shardDensity);
      long actualInitialSize1 = actualInitialSize;
      long estimatedSizeGrowth = (long) (1073741824.0 * (double) estimatedDocCountGrowth / (double) shardDensity);
      string str1 = indexName;
      string str2 = esClusterName;
      IEntityType entityType1 = entityType;
      string indexName1 = str1;
      string esClusterName1 = str2;
      Guid? tfsEntityId1 = nullable;
      string iuType = indexingUnitType;
      int num = isShadow ? 1 : 0;
      DateTime? createdTimeStamp = new DateTime?();
      DateTime? lastModifiedTimeStamp = new DateTime?();
      // ISSUE: explicit constructor call
      this.\u002Ector(indexingUnitId1, routingIds1, shardIds1, estimatedInitialDocCount, actualInitialDocCount1, estimatedDocCountGrowth1, 0, estimatedInitialSize, actualInitialSize1, estimatedSizeGrowth, 0L, (string) null, entityType1, indexName1, esClusterName1, tfsEntityId1, iuType, num != 0, createdTimeStamp, lastModifiedTimeStamp);
    }

    public IndexingUnitDetails(
      int indexingUnitId,
      string routingIds,
      string shardIds,
      int estimatedInitialDocCount,
      int actualInitialDocCount,
      int estimatedDocCountGrowth,
      int actualDocCountGrowth,
      long estimatedInitialSize,
      long actualInitialSize,
      long estimatedSizeGrowth,
      long actualSizeGrowth,
      string lastIndexedWatermark,
      IEntityType entityType,
      string indexName,
      string esClusterName,
      Guid? tfsEntityId = null,
      string iuType = null,
      bool isShadow = false,
      DateTime? createdTimeStamp = null,
      DateTime? lastModifiedTimeStamp = null)
    {
      this.IndexingUnitId = indexingUnitId;
      this.TFSEntityId = tfsEntityId;
      this.RoutingIds = routingIds;
      this.ShardIds = shardIds;
      this.EstimatedInitialDocCount = estimatedInitialDocCount;
      this.ActualInitialDocCount = actualInitialDocCount;
      this.EstimatedDocCountGrowth = estimatedDocCountGrowth;
      this.ActualDocCountGrowth = actualDocCountGrowth;
      this.EstimatedInitialSize = estimatedInitialSize;
      this.ActualInitialSize = actualInitialSize;
      this.EstimatedSizeGrowth = estimatedSizeGrowth;
      this.ActualSizeGrowth = actualSizeGrowth;
      this.LastIndexedWatermark = lastIndexedWatermark;
      this.EntityType = entityType;
      this.IndexName = indexName;
      this.ESClusterName = esClusterName;
      this.IndexingUnitType_Extended = IndexingUnit.GetIndexingUnitTypeExtended(iuType, isShadow);
      this.CreatedTimeStamp = createdTimeStamp;
      this.LastModifiedTimeStamp = lastModifiedTimeStamp;
    }

    internal string IndexingUnitType_Extended
    {
      get => this.m_indexingUnitTypeExtended;
      set
      {
        this.m_indexingUnitTypeExtended = value;
        bool isShadow;
        this.m_indexingUnitType = IndexingUnit.ParseIndexingUnitTypeExtended(this.m_indexingUnitTypeExtended, out isShadow);
        this.IsShadow = isShadow;
      }
    }

    public int IndexingUnitId { get; set; }

    public Guid? TFSEntityId { get; set; }

    public string ESClusterName { get; set; }

    public IEntityType EntityType { get; set; }

    public string IndexName { get; set; }

    public string IndexingUnitType
    {
      get => this.m_indexingUnitType;
      set => this.IndexingUnitType_Extended = IndexingUnit.GetIndexingUnitTypeExtended(value, this.IsShadow);
    }

    public bool IsShadow { get; set; }

    public int EstimatedInitialDocCount { get; set; }

    public int ActualInitialDocCount { get; set; }

    public int EstimatedDocCountGrowth { get; set; }

    public int ActualDocCountGrowth { get; }

    public long EstimatedInitialSize { get; set; }

    public long ActualInitialSize { get; }

    public long EstimatedSizeGrowth { get; set; }

    public long ActualSizeGrowth { get; }

    public string RoutingIds { get; }

    public string ShardIds { get; }

    public string LastIndexedWatermark { get; }

    public DateTime? CreatedTimeStamp { get; }

    public DateTime? LastModifiedTimeStamp { get; }

    internal void SetIndexingUnitType(string indexingunittype, bool isShadow = false) => this.IndexingUnitType_Extended = IndexingUnit.GetIndexingUnitTypeExtended(indexingunittype, isShadow);
  }
}
