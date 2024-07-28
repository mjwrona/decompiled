// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.IItemLevelFailureDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer
{
  public interface IItemLevelFailureDataAccess
  {
    IDictionary<int, int> GetCountOfRecordsByIndexingUnit(IVssRequestContext requestContext);

    int GetCountOfRecordsForIndexingUnit(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit);

    void MergeFailedRecords(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      IEnumerable<ItemLevelFailureRecord> failedRecords);

    void RemoveSuccessfullyIndexedItemsFromFailedRecords(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      IEnumerable<ItemLevelFailureRecord> successfullyIndexedRecords);

    IEnumerable<ItemLevelFailureRecord> GetFailedItems(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      int topCount,
      long startingId);

    IEnumerable<ItemLevelFailureRecord> GetItemsWithMaxAttemptCount(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      int maxAttemptCount,
      int topRowsCount);

    IEnumerable<ItemLevelFailureRecord> GetItemsWithMaxAttemptCount(
      IVssRequestContext requestContext,
      int indexingUnitId,
      int maxAttemptCount,
      int topRowsCount);

    int GetCountOfRecordsWithMaxAttemptCount(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      int maxAttemptCount);

    IDictionary<int, int> GetCountOfRecordsByIndexingUnit(
      IVssRequestContext requestContext,
      IEntityType entityType,
      int maxAttemptCount,
      IIndexingUnitDataAccess indexingUnitDataAccess);

    IDictionary<string, IDictionary<int, int>> GetCountOfRecordsCreatedBeforeGivenHoursGroupedByEntityAndIndexingUnitId(
      IVssRequestContext requestContext,
      List<RejectionCode> rejectionCodesToExclude,
      int hoursToLookBack);

    int GetCountOfRecordsCreatedBeforeGivenHoursForIndexingUnitId(
      IVssRequestContext requestContext,
      int indexingUnitId,
      List<RejectionCode> rejectionCodesToExclude,
      int hoursToLookBack);

    IEnumerable<ItemLevelFailureRecord> GetFailedItemsWithRejectionCodes(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      int topCount,
      long startingId,
      List<RejectionCode> rejectionCodes);

    IEnumerable<ItemLevelFailureRecord> GetFailedItemsWithoutRejectionCodes(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      int topCount,
      long startingId,
      List<RejectionCode> rejectionCodes);

    void DeleteRecordsByIds(
      IVssRequestContext requestContext,
      int indexingUnitId,
      IList<long> idsToDelete);

    void DeleteRecordsForIndexingUnit(IVssRequestContext requestContext, int indexingUnitId);

    int GetCountOfFailedItemsModifiedBefore(
      IVssRequestContext requestContext,
      List<IndexingUnit> indexingUnits,
      int hoursToLookBack,
      int maxAttemptCount);

    IEnumerable<ItemLevelFailureRecord> GetFailedItemsForABranch(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      string branchName,
      int topCount,
      long startingId);
  }
}
