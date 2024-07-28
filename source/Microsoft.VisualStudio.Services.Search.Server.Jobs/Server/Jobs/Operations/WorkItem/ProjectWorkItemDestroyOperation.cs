// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.ProjectWorkItemDestroyOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.SQL;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class ProjectWorkItemDestroyOperation : AbstractIndexingOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1080672, "Indexing Pipeline", "IndexingOperation");
    public const int WorkItemDestroyBatchSize = 500;
    private readonly IItemLevelFailureDataAccess m_itemLevelFailureRecordDataAccess;

    public ProjectWorkItemDestroyOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_itemLevelFailureRecordDataAccess = (IItemLevelFailureDataAccess) new ItemLevelFailureDataAccess();
    }

    internal ProjectWorkItemDestroyOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IItemLevelFailureDataAccess itemLevelFailureDataAccess)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_itemLevelFailureRecordDataAccess = itemLevelFailureDataAccess;
    }

    public override OperationResult RunOperation(CoreIndexingExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(ProjectWorkItemDestroyOperation.s_traceMetaData, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        List<string> list = ((WorkItemDestroyedEventData) this.IndexingUnitChangeEvent.ChangeData).WorkItemIds.ToList<string>();
        stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Attempting to destroy work items: [{0}]", (object) string.Join(",", (IEnumerable<string>) list))));
        if (list.Count > 500)
          throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("Tried to destroy {0} work items which is more than the limit({1}).", (object) list.Count, (object) 500)));
        BulkDeleteByQueryRequest<AbstractSearchDocumentContract> bulkDeleteByQueryRequest = new BulkDeleteByQueryRequest<AbstractSearchDocumentContract>()
        {
          ContractType = DocumentContractType.WorkItemContract,
          Query = (IExpression) new AndExpression(new IExpression[2]
          {
            (IExpression) new TermExpression("collectionId", Operator.Equals, executionContext.RequestContext.GetCollectionIdAsNormalizedString()),
            (IExpression) new TermsExpression(WorkItemContract.PlatformFieldNames.Id, Operator.In, (IEnumerable<string>) list)
          })
        };
        IndexOperationsResponse operationsResponse = this.GetIndex(executionContext as IndexingExecutionContext).BulkDeleteByQuery<AbstractSearchDocumentContract>((ExecutionContext) executionContext, bulkDeleteByQueryRequest, true);
        if ((this.IndexingUnit.IsShadow || !executionContext.IsReindexingFailedOrInProgress(executionContext.DataAccessFactory, this.IndexingUnit.EntityType) ? 0 : (executionContext.RequestContext.IsWorkItemReindexingWithZeroStalenessFeatureEnabled() ? 1 : 0)) != 0)
          this.QueueDestroyOperationForCorrespondingShadowIndexingUnit(executionContext);
        stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} IndexOperationsResponse: {1}", (object) nameof (ProjectWorkItemDestroyOperation), (object) operationsResponse.ToString())));
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = stringBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(ProjectWorkItemDestroyOperation.s_traceMetaData, nameof (RunOperation));
      }
      return operationResult;
    }

    private void QueueDestroyOperationForCorrespondingShadowIndexingUnit(
      CoreIndexingExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(executionContext.RequestContext, this.IndexingUnit.TFSEntityId, this.IndexingUnit.IndexingUnitType, this.IndexingUnit.EntityType, true);
      if (indexingUnit == null)
        return;
      WorkItemDestroyedEventData changeData = (WorkItemDestroyedEventData) this.IndexingUnitChangeEvent.ChangeData;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingUnit.IndexingUnitId,
        ChangeType = "Destroy",
        ChangeData = (ChangeEventData) changeData,
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent);
    }

    protected internal override void HandleOperationFailure(
      IndexingExecutionContext indexingExecutionContext,
      OperationResult result,
      Exception e)
    {
      List<string> list = ((WorkItemDestroyedEventData) this.IndexingUnitChangeEvent.ChangeData).WorkItemIds.ToList<string>();
      if (list.Count<string>() > 500)
      {
        result.Status = OperationStatus.Failed;
      }
      else
      {
        base.HandleOperationFailure(indexingExecutionContext, result, e);
        if (result.Status != OperationStatus.Failed)
          return;
        List<ItemLevelFailureRecord> failedRecords = new List<ItemLevelFailureRecord>();
        foreach (string str in list)
        {
          ItemLevelFailureRecord levelFailureRecord = new ItemLevelFailureRecord()
          {
            Item = str,
            Metadata = (FailureMetadata) new WorkItemFailureMetadata(),
            Stage = "Destroy"
          };
          failedRecords.Add(levelFailureRecord);
        }
        this.m_itemLevelFailureRecordDataAccess.MergeFailedRecords(indexingExecutionContext.RequestContext, this.IndexingUnit, (IEnumerable<ItemLevelFailureRecord>) failedRecords);
      }
    }

    internal virtual ISearchIndex GetIndex(IndexingExecutionContext executionContext) => executionContext.ProvisioningContext.SearchPlatform.GetIndex(executionContext.GetIndex());
  }
}
