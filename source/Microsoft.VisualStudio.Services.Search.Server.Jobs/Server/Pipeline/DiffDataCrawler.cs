// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.DiffDataCrawler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Feeder.Plugins;
using Microsoft.VisualStudio.Services.Search.Server.Jobs;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal class DiffDataCrawler : CodeIndexingPipeline
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1083086, "Indexing Pipeline", "Pipeline");

    public DiffDataCrawler(CodeIndexingPipelineContext pipelineContext)
      : base(DiffDataCrawler.s_traceMetaData, nameof (DiffDataCrawler), pipelineContext)
    {
    }

    protected internal override OperationStatus PostPostRun(OperationStatus opStatus)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetaData, nameof (PostPostRun));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("DiffDataCrawler triggered for {0}", (object) this.IndexingUnit)));
      FileGroupCrawlSpec crawlSpec = this.PipelineContext.CrawlSpec as FileGroupCrawlSpec;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("DiffDataCrawler triggered for {0} with startId {1}, lastID {2} and takeCount {3}", (object) this.IndexingUnit, (object) crawlSpec.StartingId, (object) crawlSpec.LastId, (object) crawlSpec.TakeCount)));
      try
      {
        if (crawlSpec.StartingId + (long) crawlSpec.TakeCount - 1L <= crawlSpec.LastId)
        {
          CorePipelineResult corePipelineResult = this.TriggerIndexingForTheCurrentBatch(this.PipelineContext);
          opStatus = corePipelineResult.OperationStatus;
          this.PipelineResultData = corePipelineResult.Data;
        }
        else
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("TakeCount {0} is not in sync with StartingFilePathId {1}, LastFilePathId {2}", (object) crawlSpec.TakeCount, (object) crawlSpec.StartingId, (object) crawlSpec.LastId)));
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(this.TraceMetaData, ex);
        this.PipelineContext.IndexingExecutionContext.Log.Append(ex.ToString());
        if (ex is InvalidOperationException)
          return OperationStatus.Failed;
        opStatus = OperationStatus.PartiallySucceeded;
      }
      finally
      {
        this.QueueChangeEventForIndexingNextBatchOfTempRecords(crawlSpec, this.PipelineContext);
      }
      return opStatus;
    }

    protected internal override sealed bool AllDocumentsAreProcessed() => true;

    protected override sealed void AnalyzeFeederResponse(
      CorePipelineContext<CodePipelineDocumentId, CodeDocument> pipelineContext,
      ESIndexFeedResponseData responseData,
      int totalItems)
    {
    }

    internal override sealed void HandlePipelineError(Exception ex)
    {
    }

    internal override sealed bool IsPrimaryRun() => true;

    protected internal override sealed OperationStatus PostRun(OperationStatus opStatus) => opStatus;

    protected override sealed void Prepare()
    {
    }

    protected internal override sealed void PrePreRun()
    {
    }

    protected internal override sealed void PreRun()
    {
    }

    internal override sealed IReadOnlyList<CorePipelineStage<CodePipelineDocumentId, CodeDocument>> RegisterStages() => (IReadOnlyList<CorePipelineStage<CodePipelineDocumentId, CodeDocument>>) new CorePipelineStage<CodePipelineDocumentId, CodeDocument>[0];

    internal override sealed bool ShouldRestartPipeline() => false;

    internal virtual CorePipelineResult TriggerIndexingForTheCurrentBatch(
      CodeIndexingPipelineContext pipelineContext)
    {
      return new SingleStageFileGroupIndexingPipeline(pipelineContext).Run();
    }

    internal virtual void QueueChangeEventForIndexingNextBatchOfTempRecords(
      FileGroupCrawlSpec fgCrawlSpec,
      CodeIndexingPipelineContext pipelineContext)
    {
      long num1 = fgCrawlSpec.StartingId + (long) fgCrawlSpec.TakeCount;
      if (num1 > fgCrawlSpec.LastId)
        return;
      int num2 = CommonUtils.GetRandomNumberOfFilesToBePickedForIndexingFromTempMetadataStore((ExecutionContext) pipelineContext.IndexingExecutionContext);
      if ((long) num2 + num1 > fgCrawlSpec.LastId)
        num2 = Convert.ToInt32(fgCrawlSpec.LastId - num1) + 1;
      FileSplitGroupData fileSplitGroupData = new FileSplitGroupData((ExecutionContext) pipelineContext.IndexingExecutionContext)
      {
        StartingId = num1,
        TakeCount = num2,
        LastId = fgCrawlSpec.LastId
      };
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(pipelineContext.IndexingUnitChangeEvent.LeaseId)
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeData = (ChangeEventData) fileSplitGroupData,
        ChangeType = "UpdateIndex",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Queued event {0} to index a group of files.", (object) pipelineContext.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) pipelineContext.IndexingExecutionContext, indexingUnitChangeEvent))));
    }
  }
}
