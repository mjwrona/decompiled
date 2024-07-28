// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.CollectionQueryingUnit
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  public class CollectionQueryingUnit : QueryingUnit
  {
    public bool CollectionWithLargeRepo { get; set; }

    public ISet<string> LargeRepoIds { get; set; }

    public IDictionary<string, IDictionary<string, List<IndexInfo>>> QueryIndexInfoMap { get; set; }

    public CollectionQueryingUnit(
      bool collectionWithLargeRepo,
      IEnumerable<IndexInfo> queryIndexInfos,
      ISet<string> largeRepoIds)
    {
      this.CollectionWithLargeRepo = collectionWithLargeRepo;
      this.QueryIndexInfos = queryIndexInfos;
      this.LargeRepoIds = largeRepoIds;
      this.QueryIndexInfoMap = (IDictionary<string, IDictionary<string, List<IndexInfo>>>) new Dictionary<string, IDictionary<string, List<IndexInfo>>>();
    }

    public CollectionQueryingUnit(IndexingUnit indexingUnit)
      : base(indexingUnit)
    {
      this.QueryIndexInfos = (IEnumerable<IndexInfo>) indexingUnit.Properties.QueryIndices;
      this.QueryIndexInfoMap = (IDictionary<string, IDictionary<string, List<IndexInfo>>>) new Dictionary<string, IDictionary<string, List<IndexInfo>>>();
    }

    public CollectionQueryingUnit() => this.QueryIndexInfoMap = (IDictionary<string, IDictionary<string, List<IndexInfo>>>) new Dictionary<string, IDictionary<string, List<IndexInfo>>>();

    public override object Clone()
    {
      CollectionQueryingUnit collectionQueryingUnit = (CollectionQueryingUnit) base.Clone();
      collectionQueryingUnit.CollectionWithLargeRepo = this.CollectionWithLargeRepo;
      collectionQueryingUnit.LargeRepoIds = this.LargeRepoIds;
      collectionQueryingUnit.QueryIndexInfoMap = this.QueryIndexInfoMap;
      return (object) collectionQueryingUnit;
    }

    public List<IndexInfo> GetIndexInfo(IndexInfo indexInfo)
    {
      if (indexInfo == null)
        throw new ArgumentNullException(nameof (indexInfo));
      string key = indexInfo.Routing ?? indexInfo.IndexName;
      if (!this.QueryIndexInfoMap.ContainsKey(indexInfo.IndexName))
      {
        IDictionary<string, List<IndexInfo>> dictionary = (IDictionary<string, List<IndexInfo>>) new Dictionary<string, List<IndexInfo>>();
        this.QueryIndexInfoMap.Add(indexInfo.IndexName, dictionary);
      }
      if (!this.QueryIndexInfoMap[indexInfo.IndexName].ContainsKey(key))
        this.QueryIndexInfoMap[indexInfo.IndexName].Add(key, new List<IndexInfo>()
        {
          (IndexInfo) indexInfo.Clone()
        });
      return this.QueryIndexInfoMap[indexInfo.IndexName][key];
    }
  }
}
