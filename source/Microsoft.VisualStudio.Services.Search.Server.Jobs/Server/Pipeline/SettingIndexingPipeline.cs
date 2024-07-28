// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.SettingIndexingPipeline
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  public class SettingIndexingPipeline : FirstPartyPipeline<string, SettingDocument>
  {
    public SettingIndexingPipeline(
      FirstPartyPipelineContext<string, SettingDocument> pipelineContext)
      : base(new TraceMetaData(1083137, "Indexing Pipeline", "Pipeline"), nameof (SettingIndexingPipeline), pipelineContext)
    {
    }

    internal override IReadOnlyList<CorePipelineStage<string, SettingDocument>> RegisterStages()
    {
      List<CorePipelineStage<string, SettingDocument>> corePipelineStageList = new List<CorePipelineStage<string, SettingDocument>>()
      {
        CorePipelinePluginsFactory.GetPipelineStage<string, SettingDocument>((CoreIndexingExecutionContext) this.PipelineContext.IndexingExecutionContext, "SettingCrawler", (object) this.PipelineContext.CrawlSpec, (object) this.PipelineContext.StorageContext, null)
      };
      CorePipelineStage<string, SettingDocument> feeder;
      if (this.TryGetFirstPartyFeeder("SettingFeeder", out feeder))
        corePipelineStageList.Add(feeder);
      return (IReadOnlyList<CorePipelineStage<string, SettingDocument>>) corePipelineStageList;
    }

    internal override bool IsPrimaryRun() => true;
  }
}
