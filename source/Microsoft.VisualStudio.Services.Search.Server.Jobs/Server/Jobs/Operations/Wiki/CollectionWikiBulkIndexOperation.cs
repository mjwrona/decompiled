// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki.CollectionWikiBulkIndexOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki
{
  internal class CollectionWikiBulkIndexOperation : CollectionBulkIndexOperationBase
  {
    public CollectionWikiBulkIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, 1080711)
    {
    }

    protected override EntityFinalizerBase FinalizeHelper => (EntityFinalizerBase) new CollectionWikiFinalizeHelper();

    internal override IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> CreateBulkIndexOperationForSubEntities(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessage)
    {
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> list = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>().Union<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>(this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "Git_Repository", (IEntityType) WikiEntityType.GetInstance(), -1).Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) (repo =>
      {
        return new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId)
        {
          IndexingUnitId = repo.IndexingUnitId,
          ChangeData = (ChangeEventData) new WikiBulkIndexEventData((ExecutionContext) indexingExecutionContext)
          {
            Trigger = this.IndexingUnitChangeEvent.ChangeData.Trigger
          },
          ChangeType = "BeginBulkIndex",
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = 0
        };
      }))).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> operationForSubEntities = this.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) indexingExecutionContext, list);
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully created '{0}' operation for '{1}' repositories.", (object) "BeginBulkIndex", (object) operationForSubEntities.Count)));
      return operationForSubEntities;
    }

    internal override void CreateIndexPublishEvent(
      IndexingExecutionContext executionContext,
      IndexingUnitChangeEventPrerequisites indexPublishPreReq)
    {
      if (((WikiBulkIndexEventData) this.IndexingUnitChangeEvent.ChangeData).Finalize)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent();
        indexingUnitChangeEvent1.IndexingUnitId = this.IndexingUnit.IndexingUnitId;
        indexingUnitChangeEvent1.ChangeType = "CompleteBulkIndex";
        WikiIndexPublishData indexPublishData = new WikiIndexPublishData((ExecutionContext) executionContext);
        indexPublishData.Trigger = this.IndexingUnitChangeEvent.ChangeData.Trigger;
        indexingUnitChangeEvent1.ChangeData = (ChangeEventData) indexPublishData;
        indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
        indexingUnitChangeEvent1.AttemptCount = (byte) 0;
        indexingUnitChangeEvent1.Prerequisites = indexPublishPreReq;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
        if (executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          indexingUnitChangeEvent2.LeaseId = this.IndexingUnitChangeEvent.LeaseId;
        this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent2);
      }
      else
        Tracer.TraceInfo(this.Tracepoint, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Not triggering operation: {0} as it is disabled", (object) "CompleteBulkIndex")));
    }

    internal override bool CanFinalize(IndexingExecutionContext executionContext) => ((WikiBulkIndexEventData) this.IndexingUnitChangeEvent.ChangeData).Finalize && this.FinalizeHelper.CanFinalizeIndex(executionContext);
  }
}
