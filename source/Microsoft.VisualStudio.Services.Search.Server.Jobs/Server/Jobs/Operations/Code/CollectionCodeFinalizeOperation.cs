// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CollectionCodeFinalizeOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class CollectionCodeFinalizeOperation : CollectionFinalizeOperationBase
  {
    public CollectionCodeFinalizeOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, 1080615)
    {
      this.FinalizeHelper = (EntityFinalizerBase) new CollectionCodeFinalizeHelper();
    }

    protected override string CrudOperationsFeatureName => "Search.Server.Code.CrudOperations";

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      int tracepoint = 1080615;
      Tracer.TraceEnter(tracepoint, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (executionContext.IsIndexingEnabled())
        {
          operationResult.Status = OperationStatus.Succeeded;
          operationResult = base.RunOperation(coreIndexingExecutionContext);
          if (((CodeFileContract) AbstractSearchDocumentContract.CreateContract(((CollectionIndexingProperties) executionContext.IndexingUnit.Properties).IndexContractType)).IsNGramIndexingEnabled(executionContext.RequestContext))
          {
            executionContext.RequestContext.SetCurrentHostConfigValue<string>("/Service/ALMSearch/Settings/CodeMinEdgeGramSizeForQuery", executionContext.RequestContext.GetConfigValue("/Service/ALMSearch/Settings/CodeMinEdgeGramSizeForIndexing"));
            executionContext.RequestContext.SetCurrentHostConfigValue<string>("/Service/ALMSearch/Settings/CodeInlineGramSizeForQuery", executionContext.RequestContext.GetConfigValue("/Service/ALMSearch/Settings/CodeInlineGramSizeForIndexing"));
            executionContext.RequestContext.SetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/QueryCodeNGrams", true);
          }
          else
            executionContext.RequestContext.DeleteCurrentHostConfigValue("/Service/ALMSearch/Settings/QueryCodeNGrams");
        }
        else
        {
          executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Indexing is disabled, bailing out.")));
          operationResult.Status = OperationStatus.Succeeded;
        }
      }
      finally
      {
        Tracer.TraceLeave(tracepoint, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    internal override void ErasePreReindexingState(IndexingExecutionContext executionContext)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnit1 = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      indexingUnit1.Add(executionContext.IndexingUnit);
      foreach (string supportedType in CollectionBulkIndexTypes.GetSupportedTypes(executionContext, CollectionCodeBulkIndexOperationType.MetadataCrawl))
        indexingUnit1.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) this.IndexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, supportedType, (IEntityType) CodeEntityType.GetInstance(), -1));
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 in indexingUnit1)
      {
        if (indexingUnit2.IsLargeRepository(executionContext.RequestContext))
        {
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsWithGivenParent = this.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, indexingUnit2.IndexingUnitId, -1);
          if (unitsWithGivenParent != null && unitsWithGivenParent.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
          {
            foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit3 in unitsWithGivenParent)
              indexingUnit3.Properties.ErasePreReindexingState();
            this.IndexingUnitDataAccess.UpdateIndexingUnits(executionContext.RequestContext, unitsWithGivenParent.ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>());
          }
        }
        indexingUnit2.Properties.ErasePreReindexingState();
      }
      this.IndexingUnitDataAccess.UpdateIndexingUnits(executionContext.RequestContext, indexingUnit1);
      this.ResultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Updated {0} indexing units and erased indexing state pre re-indexing. ", (object) indexingUnit1.Count)));
    }

    protected override ReindexingValidatorBase GetReindexingValidator(
      IndexingExecutionContext executionContext)
    {
      return (ReindexingValidatorBase) new CollectionCodeReindexingValidator(executionContext);
    }

    protected internal override IEnumerable<IndexInfo> GetOldDedicatedIndices(
      IndexingExecutionContext executionContext)
    {
      if (this.m_oldDedicatedIndices == null || !this.m_oldDedicatedIndices.Any<IndexInfo>())
      {
        this.m_oldDedicatedIndices = (IList<IndexInfo>) new List<IndexInfo>();
        IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitsToFinalize = this.FinalizeHelper.GetLargeChildrenIndexingUnitsToFinalize(executionContext);
        if (indexingUnitsToFinalize != null)
        {
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnitsToFinalize)
          {
            if (indexingUnit.Properties.IndexIndicesPreReindexing != null && indexingUnit.Properties.IndexIndicesPreReindexing.Any<IndexInfo>())
              this.m_oldDedicatedIndices.Add(indexingUnit.Properties.IndexIndicesPreReindexing.First<IndexInfo>());
          }
        }
        IEnumerable<IndexInfo> currentIndices = this.GetCurrentIndices();
        if (currentIndices != null)
        {
          IEnumerable<string> currentIndicesNames = currentIndices.Select<IndexInfo, string>((Func<IndexInfo, string>) (d => d.IndexName));
          this.m_oldDedicatedIndices = (IList<IndexInfo>) this.m_oldDedicatedIndices.Where<IndexInfo>((Func<IndexInfo, bool>) (o => !currentIndicesNames.Contains<string>(o.IndexName))).ToList<IndexInfo>();
        }
      }
      return (IEnumerable<IndexInfo>) this.m_oldDedicatedIndices;
    }

    internal override void FinalizeAndNotifyQueryProperties(
      IndexingExecutionContext executionContext)
    {
      base.FinalizeAndNotifyQueryProperties(executionContext);
      this.UpdatePrimaryIndexingUnits(executionContext);
    }

    internal override List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetPartiallySucceededIndexingUnitsAfterProcessingFailedFiles(
      IndexingExecutionContext indexingExecutionContext)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> processingFailedFiles = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> primaryIndexingUnitsFetchedFromDatabase = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> shadowIndexingUnitsFetchedFromDatabase = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      IDictionary<int, int> itemLevelFailuresPerIndexingUnit = (IDictionary<int, int>) new Dictionary<int, int>();
      this.PopulateFailureTableRecordsAndIndexingUnitTableRecords(indexingExecutionContext, out primaryIndexingUnitsFetchedFromDatabase, out shadowIndexingUnitsFetchedFromDatabase, out itemLevelFailuresPerIndexingUnit);
      if (shadowIndexingUnitsFetchedFromDatabase != null && shadowIndexingUnitsFetchedFromDatabase.Count > 0)
      {
        int indexingPerIndexingUnit = this.GetMaximumFailedItemsCountForSucccessfullReIndexingPerIndexingUnit(indexingExecutionContext);
        Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary = new Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        if (primaryIndexingUnitsFetchedFromDatabase != null)
          dictionary = primaryIndexingUnitsFetchedFromDatabase.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (indexingUnit => indexingUnit.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (indexingUnit => indexingUnit));
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 in shadowIndexingUnitsFetchedFromDatabase)
        {
          Guid tfsEntityId = indexingUnit1.TFSEntityId;
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2;
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
      return (ChangeEventData) new CodeIndexPublishData((ExecutionContext) indexingExecutionContext);
    }

    internal override IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> CreatePatchOperationsForPartiallySucceededIndexingUnits(
      IndexingExecutionContext iexContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> listOfIndexingUnitsPartiallySucceeded,
      StringBuilder resultMessage)
    {
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> list = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>().Union<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(listOfIndexingUnitsPartiallySucceeded.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) (indexingUnit => new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingUnit.IndexingUnitId,
        ChangeData = (ChangeEventData) new RepositoryPatchEventData((ExecutionContext) iexContext)
        {
          Patch = Patch.ReIndexFailedItems
        },
        ChangeType = "Patch",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = (byte) 0
      }))).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully created '{0}' operation for '{1}' repositories.", (object) "Patch", (object) list.Count)));
      return this.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) iexContext, list);
    }

    internal override void UpdateItemLevelFailureTableBasedOnReIndexingStatus(
      IndexingExecutionContext indexingExecutionContext,
      OperationStatus reIndexingStatus)
    {
    }

    private void PopulateFailureTableRecordsAndIndexingUnitTableRecords(
      IndexingExecutionContext indexingExecutionContext,
      out List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> primaryIndexingUnitsFetchedFromDatabase,
      out List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> shadowIndexingUnitsFetchedFromDatabase,
      out IDictionary<int, int> itemLevelFailuresPerIndexingUnit)
    {
      primaryIndexingUnitsFetchedFromDatabase = this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "Git_Repository", indexingExecutionContext.CollectionIndexingUnit.EntityType, -1);
      shadowIndexingUnitsFetchedFromDatabase = this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "Git_Repository", true, indexingExecutionContext.CollectionIndexingUnit.EntityType, -1);
      itemLevelFailuresPerIndexingUnit = this.DataAccessFactory.GetItemLevelFailureDataAccess().GetCountOfRecordsByIndexingUnit(indexingExecutionContext.RequestContext);
    }
  }
}
