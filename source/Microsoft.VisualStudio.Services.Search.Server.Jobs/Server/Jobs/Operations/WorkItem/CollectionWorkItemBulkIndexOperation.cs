// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.CollectionWorkItemBulkIndexOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class CollectionWorkItemBulkIndexOperation : CollectionBulkIndexOperationBase
  {
    public CollectionWorkItemBulkIndexOperation(
      ExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, 1080662)
    {
    }

    protected override EntityFinalizerBase FinalizeHelper => (EntityFinalizerBase) new CollectionWorkItemFinalizeHelper();

    internal override IList<IndexingUnitChangeEvent> CreateBulkIndexOperationForSubEntities(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessage)
    {
      List<IndexingUnit> indexingUnits = this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "Project", IndexingExecutionContextExtensions.IsShadowIndexingRequired(indexingExecutionContext, this.IndexingUnitChangeEvent), this.IndexingUnit.EntityType, -1);
      string leaseId = this.IndexingUnitChangeEvent.LeaseId;
      if (IndexingExecutionContextExtensions.IsShadowIndexingRequired(indexingExecutionContext, this.IndexingUnitChangeEvent))
      {
        if (indexingUnits == null || !indexingUnits.Any<IndexingUnit>())
          indexingUnits = this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "Project", false, this.IndexingUnit.EntityType, -1);
        else
          leaseId = (string) null;
      }
      IList<IndexingUnitChangeEvent> list = (IList<IndexingUnitChangeEvent>) indexingUnits.Select<IndexingUnit, IndexingUnitChangeEvent>((Func<IndexingUnit, IndexingUnitChangeEvent>) (indexingUnit => new IndexingUnitChangeEvent()
      {
        LeaseId = leaseId,
        IndexingUnitId = indexingUnit.IndexingUnitId,
        ChangeData = (ChangeEventData) new WorkItemBulkIndexEventData((ExecutionContext) indexingExecutionContext)
        {
          Finalize = false
        },
        ChangeType = "BeginBulkIndex",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = (byte) 0
      })).ToList<IndexingUnitChangeEvent>();
      IList<IndexingUnitChangeEvent> operationForSubEntities = this.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) indexingExecutionContext, list);
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully created BeginBulkIndex operation for [{0}] projects.", (object) indexingUnits.Count)));
      return operationForSubEntities;
    }

    internal override void CreateIndexPublishEvent(
      IndexingExecutionContext executionContext,
      IndexingUnitChangeEventPrerequisites indexPublishPreReq)
    {
      IndexingUnitChangeEvent indexingUnitChangeEvent = new IndexingUnitChangeEvent()
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = "CompleteBulkIndex",
        ChangeData = (ChangeEventData) new WorkItemIndexPublishData((ExecutionContext) executionContext),
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0,
        Prerequisites = indexPublishPreReq
      };
      this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent);
    }

    internal override bool CanFinalize(IndexingExecutionContext executionContext) => ((WorkItemBulkIndexEventData) this.IndexingUnitChangeEvent.ChangeData).Finalize;
  }
}
