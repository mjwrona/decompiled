// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.ProjectWorkItemIndexingOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class ProjectWorkItemIndexingOperation : AbstractIndexingOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1080663, "Indexing Pipeline", "IndexingOperation");

    public ProjectWorkItemIndexingOperation(
      CoreIndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base((ExecutionContext) indexingExecutionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(ProjectWorkItemIndexingOperation.s_traceMetaData, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      IndexingExecutionContext iexContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      bool flag = true;
      string str = string.Empty;
      if (!coreIndexingExecutionContext.RequestContext.IsWorkItemIndexingEnabled())
      {
        str = "Search.Server.WorkItem.Indexing";
        flag = false;
      }
      else if (this.IndexingUnitChangeEvent.ChangeType == "UpdateIndex" && !coreIndexingExecutionContext.RequestContext.IsWorkItemCrudOperationsEnabled())
      {
        str = "Search.Server.WorkItem.CrudOperations";
        flag = false;
      }
      if (!flag)
      {
        coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Feature flag '{0}' is disabled for this account.", (object) str)));
        operationResult.Status = OperationStatus.Succeeded;
        return operationResult;
      }
      try
      {
        if (OperationStatus.PartiallySucceeded == this.ExecuteCrawlerAndFeeder(iexContext).OperationStatus)
          coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Partially indexed {0} with Id {1}", (object) this.IndexingUnit.IndexingUnitType, (object) this.IndexingUnit.TFSEntityId)));
        new ThresholdViolationIdentifier().IdentifyItemLevelFailureViolationsForIndexingUnit((ExecutionContext) iexContext, iexContext.ItemLevelFailureDataAccess, iexContext.IndexingUnit);
        coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully indexed WorkItems in Project Id {0}", (object) this.IndexingUnit.TFSEntityId)));
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(ProjectWorkItemIndexingOperation.s_traceMetaData, nameof (RunOperation));
      }
      return operationResult;
    }

    protected internal virtual CorePipelineResult ExecuteCrawlerAndFeeder(
      IndexingExecutionContext iexContext)
    {
      CorePipelineResult corePipelineResult;
      using (FirstPartyPipelineContext<Guid, WorkItemDocument> pipelineContext = this.GetPipelineContext(iexContext))
        corePipelineResult = this.ExecutePipeline(this.GetPipeline(pipelineContext));
      CorePipelineResultData data = corePipelineResult.Data;
      if ((data != null ? (data.ItemLevelFailuresCount > 0 ? 1 : 0) : 0) != 0)
      {
        this.QueueFailedItemsPatchOperation(iexContext);
        iexContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Queued patch operation for processing failed work items. Incoming ILF error count = {0}.", (object) corePipelineResult.Data?.ItemLevelFailuresCount)));
      }
      return corePipelineResult;
    }

    internal virtual FirstPartyPipelineContext<Guid, WorkItemDocument> GetPipelineContext(
      IndexingExecutionContext iexContext)
    {
      return new FirstPartyPipelineContext<Guid, WorkItemDocument>(iexContext.IndexingUnit, iexContext, (CoreCrawlSpec) this.CreateCrawlSpec(iexContext), this.IndexingUnitChangeEvent, this.IndexingUnitChangeEventHandler, false);
    }

    internal virtual WorkItemIndexingPipeline GetPipeline(
      FirstPartyPipelineContext<Guid, WorkItemDocument> pipelineContext)
    {
      return new WorkItemIndexingPipeline(pipelineContext);
    }

    internal virtual CorePipelineResult ExecutePipeline(WorkItemIndexingPipeline pipeline) => pipeline.Run();

    internal void UpdateIndexingProperties(
      IndexingExecutionContext indexingExecutionContext,
      WorkItemCrawlSpec crawlSpec)
    {
      ProjectWorkItemIndexingProperties properties = (ProjectWorkItemIndexingProperties) this.IndexingUnit.Properties;
      properties.LastIndexedFieldsContinuationToken = crawlSpec.LastIndexedFieldsContinuationToken;
      properties.LastIndexedDiscussionContinuationToken = crawlSpec.LastIndexedDiscussionsContinuationToken;
      this.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, this.IndexingUnit);
    }

    private WorkItemCrawlSpec CreateCrawlSpec(IndexingExecutionContext iexContext)
    {
      ProjectWorkItemTFSAttributes entityAttributes = (ProjectWorkItemTFSAttributes) this.IndexingUnit.TFSEntityAttributes;
      ProjectWorkItemIndexingProperties properties = (ProjectWorkItemIndexingProperties) this.IndexingUnit.Properties;
      IVssRequestContext requestContext = iexContext.RequestContext;
      WorkItemCrawlSpec crawlSpec;
      if (this.IndexingUnitChangeEvent.ChangeType == "BeginBulkIndex")
      {
        WorkItemBulkIndexCrawlSpec bulkIndexCrawlSpec = new WorkItemBulkIndexCrawlSpec();
        ((WorkItemCrawlSpec) bulkIndexCrawlSpec).LastIndexedFieldsContinuationToken = (string) null;
        ((WorkItemCrawlSpec) bulkIndexCrawlSpec).LastIndexedDiscussionsContinuationToken = (string) null;
        crawlSpec = (WorkItemCrawlSpec) bulkIndexCrawlSpec;
      }
      else
      {
        WorkItemContinuousIndexCrawlSpec continuousIndexCrawlSpec = new WorkItemContinuousIndexCrawlSpec();
        ((WorkItemCrawlSpec) continuousIndexCrawlSpec).LastIndexedFieldsContinuationToken = properties.LastIndexedFieldsContinuationToken ?? properties.LastIndexedContinuationToken;
        ((WorkItemCrawlSpec) continuousIndexCrawlSpec).LastIndexedDiscussionsContinuationToken = properties.LastIndexedDiscussionContinuationToken ?? properties.LastIndexedContinuationToken;
        crawlSpec = (WorkItemCrawlSpec) continuousIndexCrawlSpec;
      }
      ((CoreCrawlSpec) crawlSpec).CollectionName = requestContext.GetCollectionName();
      ((CoreCrawlSpec) crawlSpec).CollectionId = requestContext.GetCollectionID().ToString();
      crawlSpec.ProjectName = entityAttributes.ProjectName;
      crawlSpec.ProjectId = entityAttributes.ProjectId;
      ((CoreCrawlSpec) crawlSpec).JobYieldData = (AbstractJobYieldData) new WorkItemIndexJobYieldData();
      return crawlSpec;
    }

    private void QueueFailedItemsPatchOperation(IndexingExecutionContext indexingExecutionContext)
    {
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) indexingExecutionContext.ServiceSettings.JobSettings.FailedItemsPatchOperationDelayInSeconds);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent();
      indexingUnitChangeEvent1.IndexingUnitId = indexingExecutionContext.IndexingUnit.IndexingUnitId;
      WorkItemPatchEventData itemPatchEventData = new WorkItemPatchEventData((ExecutionContext) indexingExecutionContext);
      itemPatchEventData.Delay = timeSpan;
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) itemPatchEventData;
      indexingUnitChangeEvent1.ChangeType = "Patch";
      indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
      indexingUnitChangeEvent1.AttemptCount = (byte) 0;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
      this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent2);
    }
  }
}
