// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.ProjectWorkItemPatchOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class ProjectWorkItemPatchOperation : AbstractIndexingOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1080674, "Indexing Pipeline", "IndexingOperation");
    private List<WorkItemPatchDescription> m_patches;

    public ProjectWorkItemPatchOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(ProjectWorkItemPatchOperation.s_traceMetaData, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      if (!coreIndexingExecutionContext.RequestContext.IsWorkItemIndexingEnabled())
      {
        operationResult.Status = OperationStatus.Succeeded;
        operationResult.Message = "Work item indexing is disabled. Bailing out.";
        return operationResult;
      }
      IndexingExecutionContext iexContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (OperationStatus.PartiallySucceeded == this.ExecuteCrawlerAndFeeder(iexContext).OperationStatus)
          coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Partially indexed failed work items in project [{0}]. ", (object) this.IndexingUnit.TFSEntityId)));
        else
          coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully indexed failed work items in project [{0}]. ", (object) this.IndexingUnit.TFSEntityId)));
        int num = this.RemoveSuccessfullyIndexedFailureRecords(iexContext);
        coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Removed [{0}] work items failure records from DB.", (object) num)));
        new ThresholdViolationIdentifier().IdentifyItemLevelFailureViolationsForIndexingUnit((ExecutionContext) iexContext, iexContext.ItemLevelFailureDataAccess, iexContext.IndexingUnit);
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(ProjectWorkItemPatchOperation.s_traceMetaData, nameof (RunOperation));
      }
      return operationResult;
    }

    protected internal virtual CorePipelineResult ExecuteCrawlerAndFeeder(
      IndexingExecutionContext iexContext)
    {
      CorePipelineResult corePipelineResult;
      using (FirstPartyPipelineContext<Guid, WorkItemDocument> pipelineContext = this.GetPipelineContext(iexContext))
        corePipelineResult = this.ExecutePipeline(this.GetPipeline(pipelineContext));
      this.ProcessFailedWorkItems(iexContext);
      return corePipelineResult;
    }

    internal virtual FirstPartyPipelineContext<Guid, WorkItemDocument> GetPipelineContext(
      IndexingExecutionContext iexContext)
    {
      return new FirstPartyPipelineContext<Guid, WorkItemDocument>(iexContext.IndexingUnit, iexContext, (CoreCrawlSpec) this.CreateCrawlSpec(iexContext), this.IndexingUnitChangeEvent, this.IndexingUnitChangeEventHandler, true);
    }

    internal virtual WorkItemIndexingPipeline GetPipeline(
      FirstPartyPipelineContext<Guid, WorkItemDocument> pipelineContext)
    {
      return new WorkItemIndexingPipeline(pipelineContext);
    }

    internal virtual CorePipelineResult ExecutePipeline(WorkItemIndexingPipeline pipeline) => pipeline.Run();

    protected internal virtual void ProcessFailedWorkItems(
      IndexingExecutionContext indexingExecutionContext)
    {
      int withMaxAttemptCount = indexingExecutionContext.ItemLevelFailureDataAccess.GetCountOfRecordsWithMaxAttemptCount(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit, indexingExecutionContext.ServiceSettings.JobSettings.FailedItemsMaxRetryCount);
      if (withMaxAttemptCount <= 0)
        return;
      this.QueueFailedItemsPatchOperation(indexingExecutionContext);
      indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Queued patch operation for processing {0} failed work items.", (object) withMaxAttemptCount)));
    }

    protected internal virtual int RemoveSuccessfullyIndexedFailureRecords(
      IndexingExecutionContext iexContext)
    {
      if (this.m_patches == null)
        throw new SearchServiceException("Expected WorkItemPatchDescription list to be non-null.");
      List<ItemLevelFailureRecord> list = this.m_patches.Select<WorkItemPatchDescription, int>((Func<WorkItemPatchDescription, int>) (x => x.Id)).Except<int>(iexContext.FailureRecordStore.GetFailedRecords().Select<ItemLevelFailureRecord, int>((Func<ItemLevelFailureRecord, int>) (x => Convert.ToInt32(x.Item, (IFormatProvider) CultureInfo.InvariantCulture)))).Select<int, ItemLevelFailureRecord>((Func<int, ItemLevelFailureRecord>) (id => new ItemLevelFailureRecord()
      {
        Item = id.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        AttemptCount = -1,
        Metadata = (FailureMetadata) new WorkItemFailureMetadata()
      })).ToList<ItemLevelFailureRecord>();
      if (list.Count > 0)
      {
        iexContext.ItemLevelFailureDataAccess.RemoveSuccessfullyIndexedItemsFromFailedRecords(iexContext.RequestContext, iexContext.IndexingUnit, (IEnumerable<ItemLevelFailureRecord>) list);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("TotalNumberOfFailedItemsReIndexedSuccessfully", "Indexing Pipeline", (double) list.Count, true);
      }
      return list.Count;
    }

    internal List<WorkItemPatchDescription> Patches
    {
      set => this.m_patches = value;
    }

    internal static List<WorkItemPatchDescription> GetWorkItemPatchDescriptions(
      IndexingExecutionContext indexingExecutionContext)
    {
      int itemsMaxRetryCount = indexingExecutionContext.ServiceSettings.JobSettings.FailedItemsMaxRetryCount;
      int failedItemsToProcess = indexingExecutionContext.ServiceSettings.JobSettings.MaxNumberOfFailedItemsToProcess;
      IEnumerable<ItemLevelFailureRecord> withMaxAttemptCount = indexingExecutionContext.ItemLevelFailureDataAccess.GetItemsWithMaxAttemptCount(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit, itemsMaxRetryCount, failedItemsToProcess);
      List<WorkItemPatchDescription> patchDescriptions = new List<WorkItemPatchDescription>();
      foreach (ItemLevelFailureRecord levelFailureRecord in withMaxAttemptCount)
      {
        try
        {
          patchDescriptions.Add(new WorkItemPatchDescription()
          {
            Id = Convert.ToInt32(levelFailureRecord.Item, (IFormatProvider) CultureInfo.InvariantCulture),
            DiscussionRevisionStart = 1
          });
        }
        catch (FormatException ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(ProjectWorkItemPatchOperation.s_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Failed to retrieve work item failure record. [{0}] could not be converted to an integer.", (object) levelFailureRecord.Item)));
        }
      }
      return patchDescriptions;
    }

    private WorkItemPatchCrawlSpec CreateCrawlSpec(IndexingExecutionContext iexContext)
    {
      ProjectWorkItemTFSAttributes entityAttributes = (ProjectWorkItemTFSAttributes) this.IndexingUnit.TFSEntityAttributes;
      ProjectWorkItemIndexingProperties properties = (ProjectWorkItemIndexingProperties) this.IndexingUnit.Properties;
      IVssRequestContext requestContext = iexContext.RequestContext;
      this.m_patches = ProjectWorkItemPatchOperation.GetWorkItemPatchDescriptions(iexContext);
      WorkItemPatchCrawlSpec crawlSpec = new WorkItemPatchCrawlSpec();
      crawlSpec.PatchDescriptions = (IList<WorkItemPatchDescription>) this.m_patches;
      ((CoreCrawlSpec) crawlSpec).CollectionName = requestContext.GetCollectionName();
      ((CoreCrawlSpec) crawlSpec).CollectionId = requestContext.GetCollectionID().ToString();
      ((WorkItemCrawlSpec) crawlSpec).ProjectName = entityAttributes.ProjectName;
      ((WorkItemCrawlSpec) crawlSpec).ProjectId = entityAttributes.ProjectId;
      ((CoreCrawlSpec) crawlSpec).JobYieldData = (AbstractJobYieldData) new WorkItemIndexJobYieldData();
      return crawlSpec;
    }

    private void QueueFailedItemsPatchOperation(IndexingExecutionContext indexingExecutionContext)
    {
      int withMaxAttemptCount = indexingExecutionContext.ItemLevelFailureDataAccess.GetCountOfRecordsWithMaxAttemptCount(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit, 1);
      string str = (string) null;
      int num;
      if (withMaxAttemptCount > 0)
      {
        str = this.IndexingUnitChangeEvent.LeaseId;
        num = 0;
        indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Requeueing Patch operation with leaseId and zero delay since there are {0} pending ILFs with zero patch attempted.", (object) withMaxAttemptCount)));
      }
      else
      {
        num = indexingExecutionContext.ServiceSettings.JobSettings.FailedItemsPatchOperationDelayInSeconds;
        indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Requeueing Patch operation without leaseId.")));
      }
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent();
      indexingUnitChangeEvent1.IndexingUnitId = indexingExecutionContext.IndexingUnit.IndexingUnitId;
      WorkItemPatchEventData itemPatchEventData = new WorkItemPatchEventData((ExecutionContext) indexingExecutionContext);
      itemPatchEventData.Delay = TimeSpan.FromSeconds((double) num);
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) itemPatchEventData;
      indexingUnitChangeEvent1.ChangeType = "Patch";
      indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
      indexingUnitChangeEvent1.LeaseId = str;
      indexingUnitChangeEvent1.AttemptCount = (byte) 0;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
      indexingExecutionContext.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent2);
    }
  }
}
