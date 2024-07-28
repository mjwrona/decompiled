// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.TimeBoxedTfvcCIPipelineFlowHandler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal class TimeBoxedTfvcCIPipelineFlowHandler : CorePipelineFlowHandler
  {
    internal TimeBoxedTfvcCIPipelineFlowHandler(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      TraceMetaData traceMetaData)
      : base(indexingUnit, traceMetaData)
    {
    }

    public override void PrePipelineRun(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      CoreCrawlSpec crawlSpec)
    {
      base.PrePipelineRun(coreIndexingExecutionContext, crawlSpec);
      TfvcCodeRepoIndexingProperties properties = this.IndexingUnit.Properties as TfvcCodeRepoIndexingProperties;
      if (properties.ContinuousIndexJobYieldData == null)
      {
        properties.ContinuousIndexJobYieldData = new TfvcContinuousIndexJobYieldData();
        this.IndexingUnit = coreIndexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, this.IndexingUnit);
      }
      ContinuousIndexTfvcCrawlSpec indexTfvcCrawlSpec = crawlSpec as ContinuousIndexTfvcCrawlSpec;
      if (properties.ContinuousIndexJobYieldData.HasData())
      {
        ((CoreCrawlSpec) indexTfvcCrawlSpec).JobYieldData = properties.ContinuousIndexJobYieldData.Clone();
        ((CoreCrawlSpec) indexTfvcCrawlSpec).JobYieldData.InitCrawlResumeCounters();
        crawlSpec.ItemsProcessedAcrossYields = properties.TfvcIndexJobYieldStats.ItemsProcessedAcrossYields;
        crawlSpec.JobYieldCount = properties.TfvcIndexJobYieldStats.YieldCount;
      }
      Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Started TimeBoxedTfvcContinuousIndex on IndexingUnit: {0} with CrawlSpec: {1}", (object) this.IndexingUnit, (object) indexTfvcCrawlSpec)));
    }

    public override void PostPipelineRun(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      CoreCrawlSpec crawlSpec)
    {
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      ContinuousIndexTfvcCrawlSpec indexTfvcCrawlSpec = crawlSpec as ContinuousIndexTfvcCrawlSpec;
      TfvcCodeRepoIndexingProperties properties = executionContext.RepositoryIndexingUnit.Properties as TfvcCodeRepoIndexingProperties;
      if (((CoreCrawlSpec) indexTfvcCrawlSpec).JobYieldData.IncompleteTreeCrawl)
      {
        properties.ContinuousIndexJobYieldData = (TfvcContinuousIndexJobYieldData) ((CoreCrawlSpec) indexTfvcCrawlSpec).JobYieldData.Clone();
        ++crawlSpec.JobYieldCount;
        properties.TfvcIndexJobYieldStats.ItemsProcessedAcrossYields = crawlSpec.ItemsProcessedAcrossYields;
        properties.TfvcIndexJobYieldStats.YieldCount = crawlSpec.JobYieldCount;
      }
      else
      {
        TfvcHttpClientWrapper tfvcHttpClientWrapper = new TfvcHttpClientWrapper((ExecutionContext) executionContext, this.TraceMetaData);
        this.UpdateRepoPropertiesWithLastIndexedInfo(properties, ((CodeCrawlSpec) indexTfvcCrawlSpec).LastIndexedChangeId, tfvcHttpClientWrapper, executionContext.RepositoryIndexingUnit.TFSEntityId.ToString(), nameof (TimeBoxedTfvcCIPipelineFlowHandler));
        properties.TfvcIndexJobYieldStats.ItemsProcessedAcrossYields = 0;
        properties.TfvcIndexJobYieldStats.YieldCount = 0;
        properties.ContinuousIndexJobYieldData = new TfvcContinuousIndexJobYieldData();
        this.PublishIndexingCompletionSLA((CoreIndexingExecutionContext) executionContext, crawlSpec);
      }
      executionContext.RepositoryIndexingUnit = coreIndexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, executionContext.RepositoryIndexingUnit);
      this.IndexingUnit = executionContext.RepositoryIndexingUnit;
    }
  }
}
