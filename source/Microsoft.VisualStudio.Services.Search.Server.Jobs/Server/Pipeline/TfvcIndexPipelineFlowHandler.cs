// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.TfvcIndexPipelineFlowHandler
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
  internal class TfvcIndexPipelineFlowHandler : CorePipelineFlowHandler
  {
    internal TfvcIndexPipelineFlowHandler(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit, TraceMetaData traceMetaData)
      : base(indexingUnit, traceMetaData)
    {
    }

    public override void PrePipelineRun(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      CoreCrawlSpec crawlSpec)
    {
      TfvcCodeRepoIndexingProperties properties = (TfvcCodeRepoIndexingProperties) this.IndexingUnit.Properties;
      TfvcIndexCrawlSpec tfvcIndexCrawlSpec = (TfvcIndexCrawlSpec) crawlSpec;
      ((CoreCrawlSpec) tfvcIndexCrawlSpec).JobYieldData = properties.TfvcIndexJobYieldData.Clone();
      if (((CoreCrawlSpec) tfvcIndexCrawlSpec).JobYieldData.HasData())
      {
        ((CoreCrawlSpec) tfvcIndexCrawlSpec).JobYieldData.InitCrawlResumeCounters();
        crawlSpec.ItemsProcessedAcrossYields = properties.TfvcIndexJobYieldStats.ItemsProcessedAcrossYields;
        crawlSpec.JobYieldCount = properties.TfvcIndexJobYieldStats.YieldCount;
      }
      Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("TfvcIndexPipelineFlowHandler: Started TimeBoxedTfvcIndexing on IndexingUnit: {0} with CrawlSpec: {1}", (object) this.IndexingUnit, (object) tfvcIndexCrawlSpec)));
    }

    public override void PostPipelineRun(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      CoreCrawlSpec crawlSpec)
    {
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      TfvcCodeRepoIndexingProperties properties = (TfvcCodeRepoIndexingProperties) executionContext.RepositoryIndexingUnit.Properties;
      TfvcIndexCrawlSpec tfvcIndexCrawlSpec = (TfvcIndexCrawlSpec) crawlSpec;
      if (((CoreCrawlSpec) tfvcIndexCrawlSpec).JobYieldData.HasData())
      {
        ++crawlSpec.JobYieldCount;
        properties.TfvcIndexJobYieldData = (TfvcIndexJobYieldData) ((CoreCrawlSpec) tfvcIndexCrawlSpec).JobYieldData.Clone();
        properties.TfvcIndexJobYieldStats.ItemsProcessedAcrossYields = crawlSpec.ItemsProcessedAcrossYields;
        properties.TfvcIndexJobYieldStats.YieldCount = crawlSpec.JobYieldCount;
      }
      else
      {
        properties.TfvcIndexJobYieldData = new TfvcIndexJobYieldData();
        properties.TfvcIndexJobYieldStats = new TfvcIndexJobYieldStats();
        this.PublishIndexingCompletionSLA((CoreIndexingExecutionContext) executionContext, crawlSpec);
        TfvcHttpClientWrapper tfvcHttpClientWrapper = new TfvcHttpClientWrapper((ExecutionContext) executionContext, this.TraceMetaData);
        this.UpdateRepoPropertiesWithLastIndexedInfo(properties, ((CodeCrawlSpec) tfvcIndexCrawlSpec).LastIndexedChangeId, tfvcHttpClientWrapper, executionContext.RepositoryIndexingUnit.TFSEntityId.ToString(), nameof (TfvcIndexPipelineFlowHandler));
      }
      this.IndexingUnit = coreIndexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, executionContext.RepositoryIndexingUnit);
    }
  }
}
