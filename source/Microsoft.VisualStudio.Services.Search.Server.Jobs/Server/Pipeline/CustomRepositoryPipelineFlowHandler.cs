// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.CustomRepositoryPipelineFlowHandler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal class CustomRepositoryPipelineFlowHandler : CorePipelineFlowHandler
  {
    internal CustomRepositoryPipelineFlowHandler(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      TraceMetaData traceMetaData)
      : base(indexingUnit, traceMetaData)
    {
    }

    public override void PostPipelineRun(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      CoreCrawlSpec crawlSpec)
    {
      CustomCrawlSpec crawlSpec1 = crawlSpec as CustomCrawlSpec;
      (this.IndexingUnit.Properties as CustomRepoCodeIndexingProperties).LastIndexedChangeTime = ((CodeCrawlSpec) crawlSpec1).LastIndexedChangeUtcTime;
      (this.IndexingUnit.Properties as CustomRepoCodeIndexingProperties).LastIndexedRequestId = crawlSpec1.RequestId;
      this.IndexingUnit = coreIndexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, this.IndexingUnit);
      if (!crawlSpec1.IsLastRequestFromIndexer)
        return;
      this.PublishIndexingCompletionSLA(coreIndexingExecutionContext, (CoreCrawlSpec) crawlSpec1);
    }
  }
}
