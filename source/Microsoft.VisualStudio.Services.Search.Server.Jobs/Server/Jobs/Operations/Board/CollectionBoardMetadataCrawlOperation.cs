// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Board.CollectionBoardMetadataCrawlOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Board
{
  internal class CollectionBoardMetadataCrawlOperation : AbstractIndexingOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1083104, "Indexing Pipeline", "IndexingOperation");

    protected internal IHttpClientWrapperFactory HttpClientWrapperFactory { get; set; }

    public CollectionBoardMetadataCrawlOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.HttpClientWrapperFactory.GetInstance())
    {
    }

    protected CollectionBoardMetadataCrawlOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IHttpClientWrapperFactory httpClientWrapperFactory)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.HttpClientWrapperFactory = httpClientWrapperFactory;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionBoardMetadataCrawlOperation.s_traceMetaData, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        if (coreIndexingExecutionContext.RequestContext.IsBoardIndexingFeatureFlagEnabled())
        {
          this.ProvisionIndexToCollection(coreIndexingExecutionContext);
          this.AddCollectionBoardBulkIndexOperation((ExecutionContext) coreIndexingExecutionContext);
        }
        operationResult.Status = OperationStatus.Succeeded;
        return operationResult;
      }
      finally
      {
        operationResult.Message = stringBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CollectionBoardMetadataCrawlOperation.s_traceMetaData, nameof (RunOperation));
      }
    }

    internal virtual void ProvisionIndexToCollection(
      CoreIndexingExecutionContext indexingExecutionContext)
    {
      indexingExecutionContext.RequestContext.GetService<IRoutingService>().AssignIndex((IndexingExecutionContext) indexingExecutionContext, new List<IndexingUnitWithSize>());
    }

    protected internal override void HandleOperationFailure(
      IndexingExecutionContext indexingExecutionContext,
      OperationResult result,
      Exception ex)
    {
      base.HandleOperationFailure(indexingExecutionContext, result, ex);
      if (result.Status != OperationStatus.Failed)
        return;
      TeamFoundationEventLog.Default.Log(result.Message, SearchEventId.CollectionBoardMetadataCrawlOperationFailed, EventLogEntryType.Error);
    }

    internal virtual void AddCollectionBoardBulkIndexOperation(ExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId);
      indexingUnitChangeEvent1.IndexingUnitId = this.IndexingUnit.IndexingUnitId;
      BoardBulkIndexEventData bulkIndexEventData = new BoardBulkIndexEventData(executionContext);
      bulkIndexEventData.Finalize = true;
      bulkIndexEventData.Trigger = this.IndexingUnitChangeEvent.ChangeData.Trigger;
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) bulkIndexEventData;
      indexingUnitChangeEvent1.ChangeType = "BeginBulkIndex";
      indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
      indexingUnitChangeEvent1.AttemptCount = (byte) 0;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
      this.IndexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent2);
    }
  }
}
