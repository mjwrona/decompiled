// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.BoardIndexingPipeline
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal class BoardIndexingPipeline : FirstPartyPipeline<string, BoardDocument>
  {
    internal BoardIndexingPipeline(
      FirstPartyPipelineContext<string, BoardDocument> pipelineContext)
      : base(new TraceMetaData(1080112, "Indexing Pipeline", "Pipeline"), nameof (BoardIndexingPipeline), pipelineContext)
    {
    }

    internal override IReadOnlyList<CorePipelineStage<string, BoardDocument>> RegisterStages()
    {
      List<CorePipelineStage<string, BoardDocument>> corePipelineStageList = new List<CorePipelineStage<string, BoardDocument>>()
      {
        CorePipelinePluginsFactory.GetPipelineStage<string, BoardDocument>((CoreIndexingExecutionContext) this.PipelineContext.IndexingExecutionContext, "BoardCrawler", (object) this.PipelineContext.CrawlSpec, (object) this.PipelineContext.StorageContext, null)
      };
      CorePipelineStage<string, BoardDocument> feeder;
      if (this.TryGetFirstPartyFeeder("BoardFeeder", out feeder))
        corePipelineStageList.Add(feeder);
      return (IReadOnlyList<CorePipelineStage<string, BoardDocument>>) corePipelineStageList;
    }

    internal override bool IsPrimaryRun() => true;
  }
}
