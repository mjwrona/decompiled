// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CollectionCodeBulkIndexOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
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
  internal class CollectionCodeBulkIndexOperation : CollectionBulkIndexOperationBase
  {
    public CollectionCodeBulkIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, 1080612)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Tracer.TraceEnter(this.Tracepoint, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (executionContext.IsIndexingEnabled())
        {
          operationResult.Status = OperationStatus.Succeeded;
          operationResult = base.RunOperation(coreIndexingExecutionContext);
        }
        else
        {
          executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Indexing is disabled, bailing out.")));
          operationResult.Status = OperationStatus.Succeeded;
        }
      }
      finally
      {
        Tracer.TraceLeave(this.Tracepoint, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    protected override EntityFinalizerBase FinalizeHelper => (EntityFinalizerBase) new CollectionCodeFinalizeHelper();

    internal override IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> CreateBulkIndexOperationForSubEntities(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessage)
    {
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList1 = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList2 = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      foreach (string supportedType in CollectionBulkIndexTypes.GetSupportedTypes(indexingExecutionContext, CollectionCodeBulkIndexOperationType.CreateBulkIndexOperation))
      {
        string leaseId = this.IndexingUnitChangeEvent.LeaseId;
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits;
        if (IndexingExecutionContextExtensions.IsShadowIndexingRequired(indexingExecutionContext, this.IndexingUnitChangeEvent) && (supportedType == "Git_Repository" || supportedType == "TFVC_Repository"))
        {
          leaseId = (string) null;
          indexingUnits = this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, supportedType, true, (IEntityType) CodeEntityType.GetInstance(), -1);
        }
        else
          indexingUnits = this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, supportedType, (IEntityType) CodeEntityType.GetInstance(), -1);
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> list = indexingUnits.Where<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (repo => !repo.Properties.IsDisabled && repo.Properties.IndexIndices != null && repo.Properties.IndexIndices.Any<IndexInfo>())).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        if (supportedType == "TFVC_Repository")
          this.CreateBulkIndexChangeEventsForTfvcIndexingUnitType(list, indexingUnitChangeEventList1, indexingExecutionContext);
        else
          indexingUnitChangeEventList2 = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList2.Union<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(list.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) (repo =>
          {
            return new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
            {
              LeaseId = leaseId,
              IndexingUnitId = repo.IndexingUnitId,
              ChangeData = (ChangeEventData) new CodeBulkIndexEventData((ExecutionContext) indexingExecutionContext)
              {
                Trigger = this.IndexingUnitChangeEvent.ChangeData.Trigger
              },
              ChangeType = !(repo.IndexingUnitType != "CustomRepository") || !repo.IsLargeRepository(indexingExecutionContext.RequestContext) ? "BeginBulkIndex" : "UpdateIndex",
              State = IndexingUnitChangeEventState.Pending,
              AttemptCount = 0
            };
          }))).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      }
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> list1 = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) this.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) indexingExecutionContext, indexingUnitChangeEventList2).Union<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList1).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully created '{0}' operation for '{1}' repositories.", (object) "BeginBulkIndex", (object) list1.Count)));
      Tracer.PublishKpi("NumberOfCodeRepoBIOperationsQueued", "Indexing Pipeline", (double) list1.Count);
      return list1;
    }

    private void CreateBulkIndexChangeEventsForTfvcIndexingUnitType(
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> repositories,
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> bulkIndexChangeEvents,
      IndexingExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = (Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent) null;
      string str = this.IndexingUnitChangeEvent.LeaseId;
      if (IndexingExecutionContextExtensions.IsShadowIndexingRequired(executionContext, this.IndexingUnitChangeEvent))
        str = (string) null;
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repository in repositories)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent();
        indexingUnitChangeEvent2.LeaseId = str;
        indexingUnitChangeEvent2.IndexingUnitId = repository.IndexingUnitId;
        CodeBulkIndexEventData bulkIndexEventData = new CodeBulkIndexEventData((ExecutionContext) executionContext);
        bulkIndexEventData.Trigger = this.IndexingUnitChangeEvent.ChangeData.Trigger;
        indexingUnitChangeEvent2.ChangeData = (ChangeEventData) bulkIndexEventData;
        indexingUnitChangeEvent2.ChangeType = "BeginBulkIndex";
        indexingUnitChangeEvent2.State = IndexingUnitChangeEventState.Pending;
        indexingUnitChangeEvent2.AttemptCount = (byte) 0;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent3 = indexingUnitChangeEvent2;
        if (indexingUnitChangeEvent1 != null)
        {
          IndexingUnitChangeEventPrerequisites eventPrerequisites1 = new IndexingUnitChangeEventPrerequisites();
          eventPrerequisites1.Add(new IndexingUnitChangeEventPrerequisitesFilter()
          {
            Id = indexingUnitChangeEvent1.Id,
            Operator = IndexingUnitChangeEventFilterOperator.Contains,
            PossibleStates = new List<IndexingUnitChangeEventState>()
            {
              IndexingUnitChangeEventState.Succeeded,
              IndexingUnitChangeEventState.Failed
            }
          });
          IndexingUnitChangeEventPrerequisites eventPrerequisites2 = eventPrerequisites1;
          indexingUnitChangeEvent3.Prerequisites = eventPrerequisites2;
        }
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent4 = this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent3);
        bulkIndexChangeEvents.Add(indexingUnitChangeEvent4);
        indexingUnitChangeEvent1 = indexingUnitChangeEvent4;
      }
    }

    internal override void CreateIndexPublishEvent(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnitChangeEventPrerequisites indexPublishPreReq)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent();
      indexingUnitChangeEvent1.IndexingUnitId = this.IndexingUnit.IndexingUnitId;
      indexingUnitChangeEvent1.ChangeType = "CompleteBulkIndex";
      CodeIndexPublishData indexPublishData = new CodeIndexPublishData((ExecutionContext) indexingExecutionContext);
      indexPublishData.Trigger = this.IndexingUnitChangeEvent.ChangeData.Trigger;
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) indexPublishData;
      indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
      indexingUnitChangeEvent1.AttemptCount = (byte) 0;
      indexingUnitChangeEvent1.Prerequisites = indexPublishPreReq;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
      if (indexingExecutionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        indexingUnitChangeEvent2.LeaseId = this.IndexingUnitChangeEvent.LeaseId;
      this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent2);
    }

    internal override bool CanFinalize(IndexingExecutionContext executionContext) => ((CodeBulkIndexEventData) this.IndexingUnitChangeEvent.ChangeData).Finalize && this.FinalizeHelper.CanFinalizeIndex(executionContext);
  }
}
