// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.PackageIndexingPipeline
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
  internal class PackageIndexingPipeline : FirstPartyPipeline<PackageDocumentId, PackageDocument>
  {
    public PackageIndexingPipeline(
      FirstPartyPipelineContext<PackageDocumentId, PackageDocument> pipelineContext)
      : base(new TraceMetaData(1080635, "Indexing Pipeline", "Pipeline"), nameof (PackageIndexingPipeline), pipelineContext)
    {
    }

    protected internal override void PrePreRun()
    {
      base.PrePreRun();
      IndexingExecutionContext executionContext = this.PipelineContext.IndexingExecutionContext;
      CollectionPackageIndexingProperties properties = (CollectionPackageIndexingProperties) executionContext.CollectionIndexingUnit.Properties;
      if (properties.FeedIndexJobYieldData == null)
      {
        properties.FeedIndexJobYieldData = new FeedIndexJobYieldData();
        executionContext.CollectionIndexingUnit = executionContext.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, executionContext.CollectionIndexingUnit);
        this.IndexingUnit = executionContext.CollectionIndexingUnit;
      }
      PackageCrawlSpec crawlSpec = (PackageCrawlSpec) this.PipelineContext.CrawlSpec;
      if (properties.FeedIndexJobYieldData.HasData())
      {
        ((CoreCrawlSpec) crawlSpec).JobYieldData = properties.FeedIndexJobYieldData.Clone();
        ((CoreCrawlSpec) crawlSpec).JobYieldData.InitCrawlResumeCounters();
      }
      Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Started TimeBoxed Collection Package Indexing on IndexingUnit: {0} with CrawlSpec: {1}", (object) this.IndexingUnit, (object) crawlSpec)));
    }

    internal override IReadOnlyList<CorePipelineStage<PackageDocumentId, PackageDocument>> RegisterStages()
    {
      List<CorePipelineStage<PackageDocumentId, PackageDocument>> corePipelineStageList = new List<CorePipelineStage<PackageDocumentId, PackageDocument>>()
      {
        CorePipelinePluginsFactory.GetPipelineStage<PackageDocumentId, PackageDocument>((CoreIndexingExecutionContext) this.PipelineContext.IndexingExecutionContext, "PackageCrawler", (object) this.PipelineContext.CrawlSpec, (object) this.PipelineContext.StorageContext, null)
      };
      CorePipelineStage<PackageDocumentId, PackageDocument> feeder;
      if (this.TryGetFirstPartyFeeder("PackageFeeder", out feeder))
        corePipelineStageList.Add(feeder);
      return (IReadOnlyList<CorePipelineStage<PackageDocumentId, PackageDocument>>) corePipelineStageList;
    }

    internal override bool IsPrimaryRun()
    {
      CollectionPackageIndexingProperties properties = (CollectionPackageIndexingProperties) this.IndexingUnit.Properties;
      return properties.FeedIndexJobYieldData == null || !properties.FeedIndexJobYieldData.HasData();
    }

    protected internal override OperationStatus PostPostRun(OperationStatus opStatus)
    {
      IndexingExecutionContext executionContext = this.PipelineContext.IndexingExecutionContext;
      PackageCrawlSpec crawlSpec = (PackageCrawlSpec) this.PipelineContext.CrawlSpec;
      CollectionPackageIndexingProperties properties = (CollectionPackageIndexingProperties) executionContext.CollectionIndexingUnit.Properties;
      if (((CoreCrawlSpec) crawlSpec).JobYieldData.IncompleteTreeCrawl)
      {
        properties.FeedIndexJobYieldData = (FeedIndexJobYieldData) ((CoreCrawlSpec) crawlSpec).JobYieldData.Clone();
      }
      else
      {
        properties.FeedIndexJobYieldData = new FeedIndexJobYieldData();
        this.PublishIndexingCompletionSLA((CoreIndexingExecutionContext) executionContext, (CoreCrawlSpec) crawlSpec);
      }
      executionContext.CollectionIndexingUnit = executionContext.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, executionContext.CollectionIndexingUnit);
      this.IndexingUnit = executionContext.CollectionIndexingUnit;
      return base.PostPostRun(opStatus);
    }
  }
}
