// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.RepositoryCodePatchPipelineFlowHandler
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

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal class RepositoryCodePatchPipelineFlowHandler : CorePipelineFlowHandler
  {
    internal RepositoryCodePatchPipelineFlowHandler(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      TraceMetaData traceMetaData)
      : base(indexingUnit, traceMetaData)
    {
    }

    public override void PostPipelineRun(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      CoreCrawlSpec crawlSpec)
    {
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      if (!(crawlSpec as IPatchIndexCrawlSpec).ShouldUpdateLastChangeInfo)
        return;
      TfvcCodeRepoIndexingProperties properties = this.IndexingUnit.Properties as TfvcCodeRepoIndexingProperties;
      TfvcHttpClientWrapper tfvcHttpClientWrapper = new TfvcHttpClientWrapper((ExecutionContext) executionContext, this.TraceMetaData);
      this.UpdateRepoPropertiesWithLastIndexedInfo(properties, (crawlSpec as CodeCrawlSpec).LastIndexedChangeId, tfvcHttpClientWrapper, executionContext.RepositoryIndexingUnit.TFSEntityId.ToString(), nameof (RepositoryCodePatchPipelineFlowHandler));
      executionContext.RepositoryIndexingUnit = coreIndexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, executionContext.RepositoryIndexingUnit);
      this.IndexingUnit = executionContext.RepositoryIndexingUnit;
    }
  }
}
