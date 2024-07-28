// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.SQL.ItemLevelFailureDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.SQL
{
  internal class ItemLevelFailureDataAccess : SqlAzureDataAccess, IItemLevelFailureDataAccess
  {
    public IDictionary<int, int> GetCountOfRecordsByIndexingUnit(IVssRequestContext requestContext)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        return component.GetCountOfRecordsByIndexingUnit();
    }

    public int GetCountOfRecordsForIndexingUnit(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        return component.GetCountOfRecordsForIndexingUnit(indexingUnit);
    }

    public void MergeFailedRecords(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<ItemLevelFailureRecord> failedRecords)
    {
      int configValue = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FailedItemsMaxRetryCount");
      int attributesIfGitRepo = indexingUnit.GetBranchCountFromTFSAttributesIfGitRepo();
      if (attributesIfGitRepo > 0)
        configValue *= attributesIfGitRepo;
      int num = 0;
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        num = component.MergeItemFailureRecords(indexingUnit, this.ModifyRecordsWhileWriting(failedRecords), configValue);
      if (num <= 0)
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("TotalNumberOfFailedItemsExceedingRetryLimit", "Indexing Pipeline", (double) num, true);
    }

    public void RemoveSuccessfullyIndexedItemsFromFailedRecords(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<ItemLevelFailureRecord> successfullyIndexedRecords)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        component.RemoveSuccessfullyIndexedItemsFromFailedRecords(indexingUnit, this.ModifyRecordsWhileWriting(successfullyIndexedRecords));
    }

    public IEnumerable<ItemLevelFailureRecord> GetFailedItems(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int topCount,
      long startingId)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        return this.ModifyRecordsWhileReading(component.GetFailedItems(indexingUnit, topCount, startingId));
    }

    public int GetCountOfRecordsCreatedBeforeGivenHoursForIndexingUnitId(
      IVssRequestContext requestContext,
      int indexingUnitId,
      List<RejectionCode> rejectionCodesToExclude,
      int hoursToLookBack)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        return component.GetCountOfRecordsCreatedBeforeGivenHoursForIndexingUnitId(indexingUnitId, rejectionCodesToExclude, hoursToLookBack);
    }

    public IDictionary<string, IDictionary<int, int>> GetCountOfRecordsCreatedBeforeGivenHoursGroupedByEntityAndIndexingUnitId(
      IVssRequestContext requestContext,
      List<RejectionCode> rejectionCodesToExclude,
      int hoursToLookBack)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        return component.GetCountOfRecordsCreatedBeforeGivenHoursGroupedByEntityAndIndexingUnitId(rejectionCodesToExclude, hoursToLookBack);
    }

    public IEnumerable<ItemLevelFailureRecord> GetFailedItemsWithRejectionCodes(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int topCount,
      long startingId,
      List<RejectionCode> rejectionCodes)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        return this.ModifyRecordsWhileReading(component.GetFailedItemsWithRejectionCodes(indexingUnit, topCount, rejectionCodes, startingId));
    }

    public IEnumerable<ItemLevelFailureRecord> GetFailedItemsWithoutRejectionCodes(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int topCount,
      long startingId,
      List<RejectionCode> rejectionCodes)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        return this.ModifyRecordsWhileReading(component.GetFailedItemsWithoutRejectionCodes(indexingUnit, topCount, rejectionCodes, startingId));
    }

    public IEnumerable<ItemLevelFailureRecord> GetItemsWithMaxAttemptCount(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int maxAttemptCount,
      int maxNumberOfTopRowsForFailedItems)
    {
      return this.GetItemsWithMaxAttemptCount(requestContext, indexingUnit.IndexingUnitId, maxAttemptCount, maxNumberOfTopRowsForFailedItems);
    }

    public virtual IEnumerable<ItemLevelFailureRecord> GetItemsWithMaxAttemptCount(
      IVssRequestContext requestContext,
      int indexingUnitId,
      int maxAttemptCount,
      int maxNumberOfTopRowsForFailedItems)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        return this.ModifyRecordsWhileReading(component.GetItemsWithMaxAttemptCount(indexingUnitId, maxAttemptCount, maxNumberOfTopRowsForFailedItems));
    }

    public int GetCountOfRecordsWithMaxAttemptCount(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int maxAttemptCount)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        return component.GetCountOfRecordsWithMaxAttemptCount(indexingUnit, maxAttemptCount);
    }

    public IDictionary<int, int> GetCountOfRecordsByIndexingUnit(
      IVssRequestContext requestContext,
      IEntityType entityType,
      int maxAttemptCount,
      IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        return component.GetCountOfRecordsByIndexingUnit(entityType, maxAttemptCount);
    }

    public void DeleteRecordsByIds(
      IVssRequestContext requestContext,
      int indexingUnitId,
      IList<long> idsToDelete)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        component.DeleteRecordsById(indexingUnitId, idsToDelete);
    }

    public void DeleteRecordsForIndexingUnit(IVssRequestContext requestContext, int indexingUnitId)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        component.DeleteRecordsForIndexingUnit(indexingUnitId);
    }

    public int GetCountOfFailedItemsModifiedBefore(
      IVssRequestContext requestContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits,
      int hoursToLookBack,
      int maxAttemptCount)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        return component.GetCountOfFailedItemsModifiedBefore(indexingUnits, hoursToLookBack, maxAttemptCount);
    }

    public IEnumerable<ItemLevelFailureRecord> GetFailedItemsForABranch(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      string branchName,
      int topCount,
      long startingId)
    {
      using (ItemLevelFailuresComponent component = requestContext.CreateComponent<ItemLevelFailuresComponent>())
        return this.ModifyRecordsWhileReading((IEnumerable<ItemLevelFailureRecord>) component.GetFailedItemsForABranch(indexingUnit, branchName, topCount, startingId));
    }

    internal virtual IEnumerable<ItemLevelFailureRecord> ModifyRecordsWhileReading(
      IEnumerable<ItemLevelFailureRecord> records)
    {
      foreach (ItemLevelFailureRecord record in records)
      {
        if (record.Metadata is FileFailureMetadata metadata && !string.IsNullOrWhiteSpace(metadata.FilePath))
        {
          record.Item = metadata.FilePath;
          metadata.FilePath = (string) null;
        }
      }
      return records;
    }

    internal virtual IEnumerable<ItemLevelFailureRecord> ModifyRecordsWhileWriting(
      IEnumerable<ItemLevelFailureRecord> records)
    {
      List<ItemLevelFailureRecord> levelFailureRecordList = new List<ItemLevelFailureRecord>();
      foreach (ItemLevelFailureRecord record in records)
      {
        if (record.Metadata is FileFailureMetadata metadata)
        {
          if (string.IsNullOrWhiteSpace(metadata.FilePath))
          {
            ItemLevelFailureRecord levelFailureRecord = (ItemLevelFailureRecord) record.Clone();
            ((FileFailureMetadata) levelFailureRecord.Metadata).FilePath = record.Item;
            levelFailureRecord.Item = FileAttributes.GetSHA2Hash(record.Item).HexHash;
            levelFailureRecordList.Add(levelFailureRecord);
          }
          else if (record.Item != FileAttributes.GetSHA2Hash(metadata.FilePath).HexHash)
            throw new Exception("Unexpected Filepath present in metadata.");
        }
        else
          levelFailureRecordList.Add(record);
      }
      return (IEnumerable<ItemLevelFailureRecord>) levelFailureRecordList;
    }
  }
}
