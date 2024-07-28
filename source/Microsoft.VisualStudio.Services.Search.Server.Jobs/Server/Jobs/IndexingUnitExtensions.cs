// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.IndexingUnitExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.SQL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public static class IndexingUnitExtensions
  {
    public static List<IndexingUnit> GetIndexingUnitTree(
      this IndexingUnit rootIndexingUnit,
      IndexingExecutionContext indexingExecutionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      if (indexingExecutionContext == null)
        throw new ArgumentNullException(nameof (indexingExecutionContext));
      if (indexingUnitDataAccess == null)
        throw new ArgumentNullException(nameof (indexingUnitDataAccess));
      List<IndexingUnit> indexingUnitTree = new List<IndexingUnit>();
      Queue<IndexingUnit> indexingUnitsInQueue = new Queue<IndexingUnit>();
      indexingUnitsInQueue.Enqueue(rootIndexingUnit);
      indexingUnitTree.Add(rootIndexingUnit);
      while (indexingUnitsInQueue.Count > 0)
      {
        IndexingUnit indexingUnit = indexingUnitsInQueue.Dequeue();
        List<IndexingUnit> unitsWithGivenParent = indexingUnitDataAccess.GetIndexingUnitsWithGivenParent(indexingExecutionContext.RequestContext, indexingUnit.IndexingUnitId, -1);
        if (unitsWithGivenParent != null && unitsWithGivenParent.Any<IndexingUnit>())
        {
          indexingUnitTree.AddRange((IEnumerable<IndexingUnit>) unitsWithGivenParent);
          unitsWithGivenParent.ForEach((Action<IndexingUnit>) (x => indexingUnitsInQueue.Enqueue(x)));
        }
      }
      return indexingUnitTree;
    }

    public static bool EraseIndexingWatermarksOfTree(
      this IndexingUnit rootIndexingUnit,
      IndexingExecutionContext indexingExecutionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      bool isShadowIndexing = false)
    {
      if (indexingExecutionContext == null)
        throw new ArgumentNullException(nameof (indexingExecutionContext));
      if (indexingUnitDataAccess == null)
        throw new ArgumentNullException(nameof (indexingUnitDataAccess));
      List<IndexingUnit> indexingUnitTree = rootIndexingUnit.GetIndexingUnitTree(indexingExecutionContext, indexingUnitDataAccess);
      List<IndexingUnit> indexingUnitList = new List<IndexingUnit>();
      foreach (IndexingUnit indexingUnit in indexingUnitTree)
      {
        if (indexingUnit.EraseIndexingWaterMarks(isShadowIndexing))
        {
          IndexingUnitExtensions.DeleteFailedDocuments(indexingExecutionContext, indexingUnit);
          if (indexingUnit.IndexingUnitType == "ScopedIndexingUnit")
          {
            indexingUnit.DeleteFileMetadataStore(indexingExecutionContext);
            indexingUnit.DeleteTempFileMetadataStore(indexingExecutionContext);
          }
          indexingUnitList.Add(indexingUnit);
        }
      }
      if (!indexingUnitList.Any<IndexingUnit>())
        return false;
      indexingUnitDataAccess.UpdateIndexingUnits(indexingExecutionContext.RequestContext, indexingUnitList);
      return true;
    }

    internal static void DeleteFailedDocuments(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnit indexingUnit)
    {
      indexingExecutionContext.ItemLevelFailureDataAccess.DeleteRecordsForIndexingUnit(indexingExecutionContext.RequestContext, indexingUnit.IndexingUnitId);
    }

    internal static void DeleteFileMetadataStore(
      this IndexingUnit indexingUnit,
      IndexingExecutionContext indexingExecutionContext)
    {
      if (!(indexingUnit.EntityType.Name == "Code"))
        return;
      IFileMetadataStoreDataAccess metadataStoreDataAccess = indexingExecutionContext.FileMetadataStoreDataAccess;
      long startingFilePathId;
      long endingFilePathId;
      metadataStoreDataAccess.GetMinAndMaxFilePathIds(indexingExecutionContext.RequestContext, indexingUnit, out startingFilePathId, out endingFilePathId);
      metadataStoreDataAccess.DeleteFileMetadataRecordsByRange(indexingExecutionContext.RequestContext, indexingUnit, startingFilePathId, endingFilePathId);
    }

    internal static void DeleteTempFileMetadataStore(
      this IndexingUnit indexingUnit,
      IndexingExecutionContext indexingExecutionContext)
    {
      if (!(indexingUnit.EntityType.Name == "Code"))
        return;
      TempFileMetadataStoreDataAccess metadataStoreDataAccess = new TempFileMetadataStoreDataAccess(indexingUnit);
      long startingId;
      long endingId;
      metadataStoreDataAccess.GetMinAndMaxIds(indexingExecutionContext.RequestContext, out startingId, out endingId);
      metadataStoreDataAccess.DeleteRecords(indexingExecutionContext.RequestContext, indexingUnit, startingId, endingId);
    }
  }
}
