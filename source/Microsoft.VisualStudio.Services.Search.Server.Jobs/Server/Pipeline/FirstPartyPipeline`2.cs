// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.FirstPartyPipeline`2
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Feeder;
using Microsoft.VisualStudio.Services.Search.Feeder.Plugins;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  public abstract class FirstPartyPipeline<TId, TDoc> : CorePipeline<TId, TDoc>
    where TId : IEquatable<TId>
    where TDoc : CorePipelineDocument<TId>
  {
    private readonly CorePipelineFlowHandler m_flowHandler;

    protected FirstPartyPipeline(
      TraceMetaData traceMetaData,
      string name,
      FirstPartyPipelineContext<TId, TDoc> pipelineContext)
      : this(traceMetaData, name, pipelineContext, CorePipelinePluginsFactory.GetPipelineFlowHandler((CoreIndexingExecutionContext) pipelineContext.IndexingExecutionContext, pipelineContext.IndexingUnit, pipelineContext.IndexingUnitChangeEvent, traceMetaData))
    {
    }

    protected FirstPartyPipeline(
      TraceMetaData traceMetaData,
      string name,
      FirstPartyPipelineContext<TId, TDoc> pipelineContext,
      CorePipelineFlowHandler flowHandler)
      : base(traceMetaData, name, (CorePipelineContext<TId, TDoc>) pipelineContext)
    {
      this.PipelineContext = pipelineContext;
      this.m_flowHandler = flowHandler;
    }

    protected override void Prepare()
    {
      base.Prepare();
      this.m_flowHandler.Prepare((CoreIndexingExecutionContext) this.PipelineContext.IndexingExecutionContext);
    }

    protected internal override void PrePreRun()
    {
      base.PrePreRun();
      this.m_flowHandler.PrePipelineRun((CoreIndexingExecutionContext) this.PipelineContext.IndexingExecutionContext, this.PipelineContext.CrawlSpec);
    }

    protected internal override void PreRun()
    {
      this.PipelineContext.CrawlSpec.MaxCrawlExecutionTime = new TimeSpan(this.PipelineContext.RemainingExecutionTime.Ticks / 4L);
      if (this.PipelineContext.CrawlSpec.JobYieldData == null)
        return;
      this.PipelineContext.CrawlSpec.JobYieldData.IncompleteTreeCrawl = false;
    }

    protected internal override OperationStatus PostRun(OperationStatus opStatus)
    {
      object obj1;
      this.PipelineContext.PropertyBag.TryGetValue("FeedResponse", out obj1);
      object obj2;
      this.PipelineContext.PropertyBag.TryGetValue("CrawledItemsCount", out obj2);
      if (obj1 is ESIndexFeedResponseData feedResponseData && obj2 is int totalItems)
      {
        this.AnalyzeFeederResponse((CorePipelineContext<TId, TDoc>) this.PipelineContext, feedResponseData, totalItems);
        this.PipelineContext.FailureRecordService().Run(this.PipelineContext.IndexingExecutionContext, feedResponseData);
      }
      else if (obj2 == null)
        throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Crawler did not set {0}.{1}[{2}] to number of documents crawled.", (object) "PipelineContext", (object) "PropertyBag", (object) "CrawledItemsCount")));
      return opStatus;
    }

    internal override bool ShouldRestartPipeline() => this.PipelineContext.RemainingExecutionTime > TimeSpan.Zero && !this.AllDocumentsAreProcessed();

    protected internal override OperationStatus PostPostRun(OperationStatus opStatus)
    {
      CorePipelineResultData pipelineResultData = this.PipelineResultData;
      ICorePipelineFailureHandler operationFailureHandler = this.PipelineContext.IndexingExecutionContext.OperationFailureHandler;
      int num = operationFailureHandler != null ? operationFailureHandler.HandleItemLevelErrors((CoreIndexingExecutionContext) this.PipelineContext.IndexingExecutionContext) : 0;
      pipelineResultData.ItemLevelFailuresCount = num;
      this.m_flowHandler.PostPipelineRun((CoreIndexingExecutionContext) this.PipelineContext.IndexingExecutionContext, this.PipelineContext.CrawlSpec);
      if (!this.AllDocumentsAreProcessed())
      {
        opStatus = OperationStatus.PartiallySucceeded;
        this.PipelineContext.IndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0}: Queued chunked operation with change event Id {1} on Indexing Unit: {2} with CrawlSpec: {3}", (object) this.Name, (object) this.QueueContinuationOperation().Id, (object) this.IndexingUnit, (object) this.PipelineContext.CrawlSpec)));
      }
      return opStatus;
    }

    protected internal virtual bool AllDocumentsAreProcessed() => this.PipelineContext.CrawlSpec.JobYieldData == null || !this.PipelineContext.CrawlSpec.JobYieldData.IncompleteTreeCrawl;

    protected internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent QueueContinuationOperation() => this.PipelineContext.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) this.PipelineContext.IndexingExecutionContext, new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.PipelineContext.IndexingUnitChangeEvent.LeaseId)
    {
      IndexingUnitId = this.IndexingUnit.IndexingUnitId,
      ChangeType = this.PipelineContext.IndexingUnitChangeEvent.ChangeType,
      ChangeData = this.PipelineContext.IndexingUnitChangeEvent.ChangeData,
      State = IndexingUnitChangeEventState.Pending,
      AttemptCount = (byte) 0
    });

    protected virtual void AnalyzeFeederResponse(
      CorePipelineContext<TId, TDoc> pipelineContext,
      ESIndexFeedResponseData responseData,
      int totalItems)
    {
      List<FeedResponseDocument> failedFeedDocuments = ((CoreFeedResponseData) responseData).FailedFeedDocuments;
      int num1;
      float num2 = (float) (num1 = failedFeedDocuments.Count<FeedResponseDocument>((Func<FeedResponseDocument, bool>) (x => x.FailureSource == FeedErrorConstants.SourceRejectedByES))) / (float) totalItems;
      if (num1 > 0 && (double) num2 > (double) pipelineContext.IndexingExecutionContext.ServiceSettings.PipelineSettings.AcceptableMaxFractionOfFailedDocs)
      {
        string str = FormattableString.Invariant(FormattableStringFactory.Create("{0}[SSEC:{1}]", (object) typeof (FeederException).FullName, (object) SearchServiceErrorCode.ElasticSearch_FilesRejectedAboveThreshold_Error));
        pipelineContext.IndexingExecutionContext.ExecutionTracerContext.PublishClientTrace(this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, "FeederServiceFailureReason", (object) str, true);
        throw new FeederException(FormattableString.Invariant(FormattableStringFactory.Create("Lots of files rejected by Elasticsearch, failing this job. Failure Reason : {0}", (object) ((CoreFeedResponseData) responseData).FailedFeedDocuments.First<FeedResponseDocument>((Func<FeedResponseDocument, bool>) (x => x.FailureSource == FeedErrorConstants.SourceRejectedByES)).FailureReason.ToString())));
      }
    }

    protected internal bool TryGetFirstPartyFeeder(
      string feederStageName,
      out CorePipelineStage<TId, TDoc> feeder)
    {
      IndexInfo indexInfo = this.GetIndexInfo(this.PipelineContext.IndexingExecutionContext.RequestContext);
      if (indexInfo == null)
      {
        feeder = (CorePipelineStage<TId, TDoc>) null;
        return false;
      }
      ref CorePipelineStage<TId, TDoc> local = ref feeder;
      IndexingExecutionContext executionContext = this.PipelineContext.IndexingExecutionContext;
      string stageName = feederStageName;
      object[] objArray = new object[2]
      {
        (object) this.PipelineContext.IndexingExecutionContext,
        null
      };
      ESIndexFeedRequestData indexFeedRequestData = new ESIndexFeedRequestData();
      ((CoreFeedRequestData) indexFeedRequestData).StorageContext = this.PipelineContext.StorageContext;
      indexFeedRequestData.IndexInfo = indexInfo;
      objArray[1] = (object) indexFeedRequestData;
      CorePipelineStage<TId, TDoc> pipelineStage = CorePipelinePluginsFactory.GetPipelineStage<TId, TDoc>((CoreIndexingExecutionContext) executionContext, stageName, objArray);
      local = pipelineStage;
      return true;
    }

    internal virtual void PublishIndexingCompletionSLA(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      CoreCrawlSpec crawlSpec)
    {
      this.m_flowHandler.PublishIndexingCompletionSLA(coreIndexingExecutionContext, crawlSpec);
    }

    private IndexInfo GetIndexInfo(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.IndexingUnit;
      if (this.IndexingUnit.IndexingUnitType == "TemporaryIndexingUnit")
        indexingUnit = DataAccessFactory.GetInstance().GetIndexingUnitDataAccess().GetIndexingUnit(requestContext, this.IndexingUnit.ParentUnitId);
      return indexingUnit.GetIndexInfo();
    }

    protected FirstPartyPipelineContext<TId, TDoc> PipelineContext { get; }
  }
}
