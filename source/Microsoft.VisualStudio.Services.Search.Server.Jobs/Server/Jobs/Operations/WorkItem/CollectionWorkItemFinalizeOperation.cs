// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.CollectionWorkItemFinalizeOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class CollectionWorkItemFinalizeOperation : CollectionFinalizeOperationBase
  {
    public CollectionWorkItemFinalizeOperation(
      ExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, 1080671)
    {
      this.FinalizeHelper = (EntityFinalizerBase) new CollectionWorkItemFinalizeHelper();
    }

    protected override string CrudOperationsFeatureName => "Search.Server.WorkItem.CrudOperations";

    internal override List<IndexingUnit> GetPartiallySucceededIndexingUnitsAfterProcessingFailedFiles(
      IndexingExecutionContext indexingExecutionContext)
    {
      List<IndexingUnit> processingFailedFiles = new List<IndexingUnit>();
      List<IndexingUnit> primaryIndexingUnitsFetchedFromDatabase = new List<IndexingUnit>();
      List<IndexingUnit> shadowIndexingUnitsFetchedFromDatabase = new List<IndexingUnit>();
      IDictionary<int, int> itemLevelFailuresPerIndexingUnit = (IDictionary<int, int>) new Dictionary<int, int>();
      this.PopulateFailureTableRecordsAndIndexingUnitTableRecords(indexingExecutionContext, out primaryIndexingUnitsFetchedFromDatabase, out shadowIndexingUnitsFetchedFromDatabase, out itemLevelFailuresPerIndexingUnit);
      if (shadowIndexingUnitsFetchedFromDatabase != null)
      {
        int indexingPerIndexingUnit = this.GetMaximumFailedItemsCountForSucccessfullReIndexingPerIndexingUnit(indexingExecutionContext);
        Dictionary<Guid, IndexingUnit> dictionary = new Dictionary<Guid, IndexingUnit>();
        if (primaryIndexingUnitsFetchedFromDatabase != null)
          dictionary = primaryIndexingUnitsFetchedFromDatabase.ToDictionary<IndexingUnit, Guid, IndexingUnit>((Func<IndexingUnit, Guid>) (indexingUnit => indexingUnit.TFSEntityId), (Func<IndexingUnit, IndexingUnit>) (indexingUnit => indexingUnit));
        foreach (IndexingUnit indexingUnit1 in shadowIndexingUnitsFetchedFromDatabase)
        {
          Guid tfsEntityId = indexingUnit1.TFSEntityId;
          IndexingUnit indexingUnit2;
          dictionary.TryGetValue(tfsEntityId, out indexingUnit2);
          if (indexingUnit2 != null)
          {
            int num1;
            itemLevelFailuresPerIndexingUnit.TryGetValue(indexingUnit2.IndexingUnitId, out num1);
            int num2;
            itemLevelFailuresPerIndexingUnit.TryGetValue(indexingUnit1.IndexingUnitId, out num2);
            if (num1 > indexingPerIndexingUnit)
              processingFailedFiles.Add(indexingUnit2);
            if (num2 > indexingPerIndexingUnit)
              processingFailedFiles.Add(indexingUnit1);
          }
          else
            throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Primary Indexing Unit corresponsing to Shadow Indexing Unit {0} not found in database", (object) indexingUnit1)));
        }
      }
      return processingFailedFiles;
    }

    internal override ChangeEventData GetChangeDataForCollection(
      IndexingExecutionContext indexingExecutionContext)
    {
      return (ChangeEventData) new WorkItemIndexPublishData((ExecutionContext) indexingExecutionContext);
    }

    internal override IList<IndexingUnitChangeEvent> CreatePatchOperationsForPartiallySucceededIndexingUnits(
      IndexingExecutionContext iexContext,
      List<IndexingUnit> listOfIndexingUnitsPartiallySucceeded,
      StringBuilder resultMessage)
    {
      IList<IndexingUnitChangeEvent> list = (IList<IndexingUnitChangeEvent>) new List<IndexingUnitChangeEvent>().Union<IndexingUnitChangeEvent>(listOfIndexingUnitsPartiallySucceeded.Select<IndexingUnit, IndexingUnitChangeEvent>((Func<IndexingUnit, IndexingUnitChangeEvent>) (indexingUnit => new IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingUnit.IndexingUnitId,
        ChangeData = (ChangeEventData) new WorkItemPatchEventData((ExecutionContext) iexContext),
        ChangeType = "Patch",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = (byte) 0
      }))).ToList<IndexingUnitChangeEvent>();
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully created '{0}' operation for '{1}' projects.", (object) "Patch", (object) list.Count)));
      return this.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) iexContext, list);
    }

    internal override IEnumerable<IndexingUnitChangeEventPrerequisitesFilter> GetPreRequisitesToRequeueFinalize(
      IndexingExecutionContext iexContext)
    {
      return this.IndexingUnitChangeEventDataAccess.GetIndexingUnitChangeEvents(iexContext.RequestContext, new List<string>()
      {
        "Destroy"
      }, new List<IndexingUnitChangeEventState>()
      {
        IndexingUnitChangeEventState.Pending,
        IndexingUnitChangeEventState.Queued,
        IndexingUnitChangeEventState.InProgress,
        IndexingUnitChangeEventState.FailedAndRetry
      }, -1).Select<IndexingUnitChangeEventDetails, IndexingUnitChangeEvent>((Func<IndexingUnitChangeEventDetails, IndexingUnitChangeEvent>) (des => des.IndexingUnitChangeEvent)).ToList<IndexingUnitChangeEvent>().Select<IndexingUnitChangeEvent, IndexingUnitChangeEventPrerequisitesFilter>((Func<IndexingUnitChangeEvent, IndexingUnitChangeEventPrerequisitesFilter>) (dce => new IndexingUnitChangeEventPrerequisitesFilter()
      {
        Id = dce.Id,
        Operator = IndexingUnitChangeEventFilterOperator.Contains,
        PossibleStates = new List<IndexingUnitChangeEventState>()
        {
          IndexingUnitChangeEventState.Succeeded,
          IndexingUnitChangeEventState.Failed
        }
      }));
    }

    internal override void FinalizeAndNotifyQueryProperties(
      IndexingExecutionContext indexingExecutionContext)
    {
      base.FinalizeAndNotifyQueryProperties(indexingExecutionContext);
    }

    internal override void UpdateItemLevelFailureTableBasedOnReIndexingStatus(
      IndexingExecutionContext indexingExecutionContext,
      OperationStatus reIndexingStatus)
    {
      if (IndexingExecutionContextExtensions.IsShadowIndexingRequired(indexingExecutionContext, this.IndexingUnitChangeEvent))
        throw new NotImplementedException();
    }

    private void PopulateFailureTableRecordsAndIndexingUnitTableRecords(
      IndexingExecutionContext indexingExecutionContext,
      out List<IndexingUnit> primaryIndexingUnitsFetchedFromDatabase,
      out List<IndexingUnit> shadowIndexingUnitsFetchedFromDatabase,
      out IDictionary<int, int> itemLevelFailuresPerIndexingUnit)
    {
      primaryIndexingUnitsFetchedFromDatabase = this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "Project", indexingExecutionContext.CollectionIndexingUnit.EntityType, -1);
      shadowIndexingUnitsFetchedFromDatabase = this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "Project", true, indexingExecutionContext.CollectionIndexingUnit.EntityType, -1);
      itemLevelFailuresPerIndexingUnit = this.DataAccessFactory.GetItemLevelFailureDataAccess().GetCountOfRecordsByIndexingUnit(indexingExecutionContext.RequestContext);
    }
  }
}
