// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.WorkItemIndexingPipeline
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal class WorkItemIndexingPipeline : FirstPartyPipeline<Guid, WorkItemDocument>
  {
    public WorkItemIndexingPipeline(
      FirstPartyPipelineContext<Guid, WorkItemDocument> pipelineContext)
      : base(new TraceMetaData(1083121, "Indexing Pipeline", "Pipeline"), nameof (WorkItemIndexingPipeline), pipelineContext)
    {
    }

    protected internal override void PrePreRun()
    {
      base.PrePreRun();
      IndexingExecutionContext executionContext = this.PipelineContext.IndexingExecutionContext;
      ProjectWorkItemIndexingProperties properties = (ProjectWorkItemIndexingProperties) executionContext.ProjectIndexingUnit.Properties;
      if (properties.WorkItemIndexJobYieldData == null)
      {
        properties.WorkItemIndexJobYieldData = new WorkItemIndexJobYieldData();
        executionContext.ProjectIndexingUnit = executionContext.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, executionContext.ProjectIndexingUnit);
        this.IndexingUnit = executionContext.ProjectIndexingUnit;
      }
      WorkItemCrawlSpec crawlSpec = (WorkItemCrawlSpec) this.PipelineContext.CrawlSpec;
      if (properties.WorkItemIndexJobYieldData.HasData())
      {
        ((CoreCrawlSpec) crawlSpec).JobYieldData = properties.WorkItemIndexJobYieldData.Clone();
        ((CoreCrawlSpec) crawlSpec).JobYieldData.InitCrawlResumeCounters();
      }
      Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Started TimeBoxed WorkItem Indexing on IndexingUnit: {0} with CrawlSpec: {1}", (object) this.IndexingUnit, (object) crawlSpec)));
    }

    internal override IReadOnlyList<CorePipelineStage<Guid, WorkItemDocument>> RegisterStages()
    {
      List<CorePipelineStage<Guid, WorkItemDocument>> corePipelineStageList = new List<CorePipelineStage<Guid, WorkItemDocument>>()
      {
        CorePipelinePluginsFactory.GetPipelineStage<Guid, WorkItemDocument>((CoreIndexingExecutionContext) this.PipelineContext.IndexingExecutionContext, this.GetWorkItemCrawlerStageName(this.PipelineContext.CrawlSpec), (object) this.PipelineContext.CrawlSpec, (object) this.PipelineContext.StorageContext)
      };
      CorePipelineStage<Guid, WorkItemDocument> feeder;
      if (this.TryGetFirstPartyFeeder("WorkItemFeeder", out feeder))
        corePipelineStageList.Add(feeder);
      return (IReadOnlyList<CorePipelineStage<Guid, WorkItemDocument>>) corePipelineStageList;
    }

    private string GetWorkItemCrawlerStageName(CoreCrawlSpec crawlSpec)
    {
      switch (crawlSpec)
      {
        case WorkItemPatchCrawlSpec _:
          return "WorkItemPatchCrawler";
        case WorkItemBulkIndexCrawlSpec _:
        case WorkItemContinuousIndexCrawlSpec _:
          return "WorkItemRevisionsCrawler";
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("CrawlSpec of type [{0}] is not supported by [{1}]", (object) crawlSpec.GetType().FullName, (object) nameof (WorkItemIndexingPipeline))));
      }
    }

    internal override bool IsPrimaryRun()
    {
      ProjectWorkItemIndexingProperties properties = (ProjectWorkItemIndexingProperties) this.IndexingUnit.Properties;
      return properties.WorkItemIndexJobYieldData == null || !properties.WorkItemIndexJobYieldData.HasData();
    }

    protected internal override OperationStatus PostPostRun(OperationStatus opStatus)
    {
      IndexingExecutionContext executionContext = this.PipelineContext.IndexingExecutionContext;
      WorkItemCrawlSpec crawlSpec = (WorkItemCrawlSpec) this.PipelineContext.CrawlSpec;
      if (!(crawlSpec is WorkItemPatchCrawlSpec))
      {
        ProjectWorkItemIndexingProperties properties = (ProjectWorkItemIndexingProperties) executionContext.ProjectIndexingUnit.Properties;
        if (((CoreCrawlSpec) crawlSpec).JobYieldData.IncompleteTreeCrawl)
        {
          properties.WorkItemIndexJobYieldData = (WorkItemIndexJobYieldData) ((CoreCrawlSpec) crawlSpec).JobYieldData.Clone();
        }
        else
        {
          properties.LastIndexedFieldsContinuationToken = crawlSpec.LastIndexedFieldsContinuationToken;
          properties.LastIndexedDiscussionContinuationToken = crawlSpec.LastIndexedDiscussionsContinuationToken;
          properties.WorkItemIndexJobYieldData = new WorkItemIndexJobYieldData();
          this.PublishIndexingCompletionSLA((CoreIndexingExecutionContext) executionContext, (CoreCrawlSpec) crawlSpec);
        }
        executionContext.ProjectIndexingUnit = executionContext.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, executionContext.ProjectIndexingUnit);
        this.IndexingUnit = executionContext.ProjectIndexingUnit;
      }
      return base.PostPostRun(opStatus);
    }
  }
}
