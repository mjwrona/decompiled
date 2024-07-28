// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.RepositoryCodeIndexingOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal abstract class RepositoryCodeIndexingOperation : AbstractIndexingOperation
  {
    public RepositoryCodeIndexingOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      TraceMetaData traceMetadata)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.TraceMetaData = traceMetadata;
    }

    protected TraceMetaData TraceMetaData { get; set; }

    protected CodeIndexingPipeline WorkerPipeline { get; set; }

    protected bool SupportsStoringFiles { get; set; }

    protected internal RegistryManager RegistryManager { get; set; }

    protected internal virtual CorePipelineResult ExecuteCrawlerParserAndFeeder(
      IndexingExecutionContext iexContext,
      string branchName,
      List<string> branches)
    {
      CorePipelineResult corePipelineResult;
      using (CodeIndexingPipelineContext pipelineContext = this.GetPipelineContext(iexContext, branchName, branches))
        corePipelineResult = this.ExecutePipeline(this.GetPipeline(pipelineContext));
      CorePipelineResultData data = corePipelineResult.Data;
      if ((data != null ? (data.ItemLevelFailuresCount > 0 ? 1 : 0) : 0) != 0 && !iexContext.RepositoryIndexingUnit.IsLargeRepository(iexContext.RequestContext) && iexContext.RepositoryIndexingUnit.IndexingUnitType != "CustomRepository")
        this.QueueFailedItemsPatchOperation(iexContext);
      new ThresholdViolationIdentifier().IdentifyItemLevelFailureViolationsForIndexingUnit((ExecutionContext) iexContext, iexContext.ItemLevelFailureDataAccess, iexContext.IndexingUnit);
      return corePipelineResult;
    }

    internal virtual CodeIndexingPipelineContext GetPipelineContext(
      IndexingExecutionContext iexContext,
      string branchName,
      List<string> branches)
    {
      CodeCrawlSpec crawlSpec = this.CreateCrawlSpec(iexContext, ref branchName, in branches);
      return new CodeIndexingPipelineContext(iexContext.IndexingUnit, iexContext, crawlSpec, this.IndexingUnitChangeEvent, this.IndexingUnitChangeEventHandler, branchName, branches, this.SupportsStoringFiles, true);
    }

    internal virtual CodeIndexingPipeline GetPipeline(CodeIndexingPipelineContext pipelineContext) => this.WorkerPipeline = new CodeIndexingPipeline(pipelineContext);

    internal virtual CorePipelineResult ExecutePipeline(CodeIndexingPipeline pipeline) => pipeline.Run();

    internal abstract CodeCrawlSpec CreateCrawlSpec(
      IndexingExecutionContext iexContext,
      ref string branchName,
      in List<string> branches);

    internal virtual int GetCountOfFailedFiles(IndexingExecutionContext indexingExecutionContext)
    {
      int itemsMaxRetryCount = indexingExecutionContext.ServiceSettings.JobSettings.FailedItemsMaxRetryCount;
      int attributesIfGitRepo = indexingExecutionContext.IndexingUnit.GetBranchCountFromTFSAttributesIfGitRepo();
      if (attributesIfGitRepo > 0)
        itemsMaxRetryCount *= attributesIfGitRepo;
      int countOfFailedFiles = 0;
      if (!indexingExecutionContext.RepositoryIndexingUnit.IsLargeRepository(indexingExecutionContext.RequestContext) && indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitType != "CustomRepository")
        countOfFailedFiles = indexingExecutionContext.ItemLevelFailureDataAccess.GetCountOfRecordsWithMaxAttemptCount(indexingExecutionContext.RequestContext, indexingExecutionContext.RepositoryIndexingUnit, itemsMaxRetryCount);
      indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("CountOfFailedFiles = {0}.", (object) countOfFailedFiles)));
      return countOfFailedFiles;
    }

    internal virtual void QueueFailedItemsPatchOperation(
      IndexingExecutionContext indexingExecutionContext)
    {
      int operationDelayInSeconds = indexingExecutionContext.ServiceSettings.JobSettings.FailedItemsPatchOperationDelayInSeconds;
      indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Queueing Patch operation without leaseId.")));
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent();
      indexingUnitChangeEvent1.IndexingUnitId = indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitId;
      RepositoryPatchEventData repositoryPatchEventData = new RepositoryPatchEventData((ExecutionContext) indexingExecutionContext);
      repositoryPatchEventData.Patch = Patch.ReIndexFailedItems;
      repositoryPatchEventData.Delay = TimeSpan.FromSeconds((double) operationDelayInSeconds);
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) repositoryPatchEventData;
      indexingUnitChangeEvent1.ChangeType = "Patch";
      indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
      indexingUnitChangeEvent1.AttemptCount = (byte) 0;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
      this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent2);
    }
  }
}
