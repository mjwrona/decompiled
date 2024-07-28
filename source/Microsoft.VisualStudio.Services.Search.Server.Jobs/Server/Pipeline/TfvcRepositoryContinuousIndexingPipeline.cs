// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.TfvcRepositoryContinuousIndexingPipeline
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal class TfvcRepositoryContinuousIndexingPipeline : CodeIndexingPipeline
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1083091, "Indexing Pipeline", "Pipeline");

    internal TfvcRepositoryContinuousIndexingPipeline(CodeIndexingPipelineContext pipelineContext)
      : base(TfvcRepositoryContinuousIndexingPipeline.s_traceMetaData, nameof (TfvcRepositoryContinuousIndexingPipeline), pipelineContext)
    {
    }

    protected internal override Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent QueueContinuationOperation()
    {
      TFVCCodeContinuousIndexEventData changeData = (TFVCCodeContinuousIndexEventData) this.PipelineContext.IndexingUnitChangeEvent.ChangeData;
      return this.PipelineContext.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) this.PipelineContext.IndexingExecutionContext, new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.PipelineContext.IndexingUnitChangeEvent.LeaseId)
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = "UpdateIndex",
        ChangeData = (ChangeEventData) new TFVCCodeContinuousIndexEventData((ExecutionContext) this.PipelineContext.IndexingExecutionContext)
        {
          ChangesetId = changeData.ChangesetId
        },
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = (byte) 0
      });
    }

    internal override bool IsPrimaryRun()
    {
      TfvcCodeRepoIndexingProperties properties = this.IndexingUnit.Properties as TfvcCodeRepoIndexingProperties;
      TfvcCodeRepoTFSAttributes entityAttributes = this.IndexingUnit.TFSEntityAttributes as TfvcCodeRepoTFSAttributes;
      return properties == null || entityAttributes == null || properties.ContinuousIndexJobYieldData == null || !properties.ContinuousIndexJobYieldData.HasData();
    }

    protected internal override string GetCodeCrawlerStageName() => this.PipelineContext.CrawlSpec is ContinuousIndexTfvcCrawlSpec ? "TfvcRepositoryChangesetCrawler" : base.GetCodeCrawlerStageName();
  }
}
