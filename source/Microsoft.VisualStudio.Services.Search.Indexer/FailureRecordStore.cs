// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.FailureRecordStore
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public class FailureRecordStore
  {
    private readonly IndexingUnit m_indexingUnit;
    private readonly IndexingExecutionContext m_indexingExecutionContext;
    private IItemLevelFailureDataAccess m_itemLevelFailureDataAccess;
    private readonly IDictionary<string, ItemLevelFailureRecord> m_itemLevelFailureRecords = (IDictionary<string, ItemLevelFailureRecord>) new FriendlyDictionary<string, ItemLevelFailureRecord>((IEqualityComparer<string>) StringComparer.Ordinal);

    internal FailureRecordStore()
    {
    }

    internal FailureRecordStore(
      IndexingExecutionContext indexingExecutionContext,
      IDataAccessFactory dataAccessFactory)
    {
      this.m_indexingExecutionContext = indexingExecutionContext;
      this.DataAccessFactory = dataAccessFactory;
      this.m_indexingUnit = this.GetIndexingUnit(indexingExecutionContext);
    }

    internal IDataAccessFactory DataAccessFactory { get; private set; }

    internal virtual IItemLevelFailureDataAccess ItemLevelFailureDataAccess => this.m_itemLevelFailureDataAccess ?? (this.m_itemLevelFailureDataAccess = this.DataAccessFactory.GetItemLevelFailureDataAccess());

    public virtual void AddFailedRecord(ItemLevelFailureRecord itemLevelFailureRecord)
    {
      this.m_itemLevelFailureRecords[itemLevelFailureRecord.Item] = itemLevelFailureRecord;
      if (!"CustomRepository".Equals(this.m_indexingExecutionContext.RepositoryIndexingUnit?.IndexingUnitType) || !RejectionCodeExtension.GetRetriableCodes().Contains(itemLevelFailureRecord.RejectionCode))
        return;
      itemLevelFailureRecord.RejectionCode = RejectionCode.PERSISTENT;
    }

    public virtual IEnumerable<ItemLevelFailureRecord> GetFailedRecords() => (IEnumerable<ItemLevelFailureRecord>) new List<ItemLevelFailureRecord>((IEnumerable<ItemLevelFailureRecord>) this.m_itemLevelFailureRecords.Values);

    public virtual void PersistFailedRecords()
    {
      if (this.m_itemLevelFailureRecords == null || this.m_itemLevelFailureRecords.Count <= 0)
        return;
      this.ItemLevelFailureDataAccess.MergeFailedRecords(this.m_indexingExecutionContext.RequestContext, this.m_indexingUnit, (IEnumerable<ItemLevelFailureRecord>) this.m_itemLevelFailureRecords.Values);
    }

    internal IndexingUnit GetIndexingUnit(IndexingExecutionContext indexingExecutionContext) => indexingExecutionContext.IndexingUnit.IndexingUnitType == "TemporaryIndexingUnit" ? this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit.ParentUnitId) : indexingExecutionContext.IndexingUnit;
  }
}
