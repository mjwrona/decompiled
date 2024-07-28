// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Board.CollectionBoardBulkIndexOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Board
{
  internal class CollectionBoardBulkIndexOperation : CollectionBulkIndexOperationBase
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1083101, "Indexing Pipeline", "IndexingOperation");

    protected internal IHttpClientWrapperFactory HttpClientWrapperFactory { get; set; }

    public CollectionBoardBulkIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.HttpClientWrapperFactory.GetInstance())
    {
    }

    protected CollectionBoardBulkIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IHttpClientWrapperFactory httpClientWrapperFactory)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, 1083101)
    {
      this.HttpClientWrapperFactory = httpClientWrapperFactory;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionBoardBulkIndexOperation.s_traceMetaData, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext indexingExecutionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        operationResult = base.RunOperation(coreIndexingExecutionContext);
        if (operationResult.Status != OperationStatus.Succeeded)
          return operationResult;
        resultMessage.Append(operationResult.Message);
        using (FirstPartyPipelineContext<string, BoardDocument> pipelineContext = this.GetPipelineContext(indexingExecutionContext))
          this.GetPipeline(pipelineContext).Run();
        this.TriggerFinalizeIfNeeded(indexingExecutionContext, resultMessage);
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083101, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual FirstPartyPipelineContext<string, BoardDocument> GetPipelineContext(
      IndexingExecutionContext indexingExecutionContext)
    {
      return new FirstPartyPipelineContext<string, BoardDocument>(indexingExecutionContext.IndexingUnit, indexingExecutionContext, (CoreCrawlSpec) this.GetBoardCrawlSpec(indexingExecutionContext), this.IndexingUnitChangeEvent, this.IndexingUnitChangeEventHandler, false);
    }

    internal virtual BoardIndexingPipeline GetPipeline(
      FirstPartyPipelineContext<string, BoardDocument> pipelineContext)
    {
      return new BoardIndexingPipeline(pipelineContext);
    }

    internal virtual void TriggerFinalizeIfNeeded(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessage)
    {
      this.TriggerCompleteBulkIndexIfNeeded(indexingExecutionContext, resultMessage);
    }

    protected override EntityFinalizerBase FinalizeHelper => (EntityFinalizerBase) new CollectionBoardFinalizeHelper();

    internal virtual BoardCrawlSpec GetBoardCrawlSpec(
      IndexingExecutionContext indexingExecutionContext)
    {
      BoardCrawlSpec boardCrawlSpec = new BoardCrawlSpec();
      boardCrawlSpec.OrganizationName = indexingExecutionContext.RequestContext.GetOrganizationName();
      ((CoreCrawlSpec) boardCrawlSpec).CollectionName = indexingExecutionContext.RequestContext.GetCollectionName();
      ((CoreCrawlSpec) boardCrawlSpec).CollectionId = indexingExecutionContext.RequestContext.GetCollectionID().ToString();
      ((CoreCrawlSpec) boardCrawlSpec).JobYieldData = (AbstractJobYieldData) new BoardIndexJobYieldData();
      return boardCrawlSpec;
    }

    internal override bool CanFinalize(IndexingExecutionContext executionContext) => ((BoardBulkIndexEventData) this.IndexingUnitChangeEvent.ChangeData).Finalize;

    internal override IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> CreateBulkIndexOperationForSubEntities(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessage)
    {
      return (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
    }

    internal override void CreateIndexPublishEvent(
      IndexingExecutionContext executionContext,
      IndexingUnitChangeEventPrerequisites indexPublishPreReq)
    {
    }

    protected void TriggerCompleteBulkIndexIfNeeded(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessage)
    {
      if (this.IndexingUnitChangeEvent.ChangeType != "BeginBulkIndex")
        return;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingExecutionContext.IndexingUnit.IndexingUnitId,
        ChangeData = new ChangeEventData((ExecutionContext) indexingExecutionContext)
        {
          Trigger = this.IndexingUnitChangeEvent.ChangeData.Trigger
        },
        ChangeType = "CompleteBulkIndex",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(indexingExecutionContext.RequestContext, typeof (CollectionBoardBulkIndexOperation).ToString(), this.IndexingUnitChangeEvent.ChangeData.Trigger);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = this.IndexingUnitChangeEventHandler.HandleEvent(indexingExecutionContext.RequestContext.GetExecutionContext(correlationDetails), indexingUnitChangeEvent1);
      resultMessage.AppendLine(FormattableString.Invariant(FormattableStringFactory.Create("Triggered event {0}", (object) indexingUnitChangeEvent2)));
    }
  }
}
