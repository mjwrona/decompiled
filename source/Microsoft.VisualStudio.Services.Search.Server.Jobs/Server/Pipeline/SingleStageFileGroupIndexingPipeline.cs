// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.SingleStageFileGroupIndexingPipeline
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Feeder.Plugins;
using Microsoft.VisualStudio.Services.Search.Server.Jobs;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal class SingleStageFileGroupIndexingPipeline : CodeIndexingPipeline
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1083085, "Indexing Pipeline", "Pipeline");

    public SingleStageFileGroupIndexingPipeline(CodeIndexingPipelineContext pipelineContext)
      : base(SingleStageFileGroupIndexingPipeline.s_traceMetaData, nameof (SingleStageFileGroupIndexingPipeline), pipelineContext, CorePipelineFlowHandler.DefaultNoOpInstance)
    {
      if (pipelineContext.RemainingExecutionTime > TimeSpan.Zero)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Pipeline context for pipeline [{0}] must not have positive remaining execution time", (object) this.Name)));
    }

    protected internal override OperationStatus PostRun(OperationStatus opStatus)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, "Triggering FileMetaDataStoreService.");
      object obj;
      this.PipelineContext.PropertyBag.TryGetValue("FeedResponse", out obj);
      if (!(obj is ESIndexFeedResponseData esIndexFeedResponseData))
        throw new SearchServiceException("Feed response data not populated in pipeline's property bag.");
      this.GetFileMetaDataStoreService().Run(esIndexFeedResponseData);
      return opStatus;
    }

    internal virtual IFileMetaDataStoreService GetFileMetaDataStoreService() => (IFileMetaDataStoreService) new FileMetaDataStoreService(this.PipelineContext.IndexingExecutionContext, this.PipelineContext.StorageContext, (CoreCrawlSpec) this.PipelineContext.CrawlSpec);
  }
}
