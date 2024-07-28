// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.DiffMetadataCrawler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Crawler.Definitions;
using Microsoft.VisualStudio.Services.Search.Feeder.Plugins;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal class DiffMetadataCrawler : CodeIndexingPipeline
  {
    private CodeCrawlSpec m_crawlSpec;
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1083094, "Indexing Pipeline", "Pipeline");

    public DiffMetadataCrawler(CodeIndexingPipelineContext pipelineContext, CodeCrawlSpec crawlSpec)
      : base(DiffMetadataCrawler.s_traceMetaData, nameof (DiffMetadataCrawler), pipelineContext)
    {
      this.m_crawlSpec = crawlSpec;
    }

    protected internal override OperationStatus PostPostRun(OperationStatus opStatus)
    {
      IndexingExecutionContext executionContext = this.PipelineContext.IndexingExecutionContext;
      try
      {
        int num = this.GetTreeCrawler().Run((CoreIndexingExecutionContext) this.PipelineContext.IndexingExecutionContext);
        this.ValidatePipelineDocs(executionContext);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Total Items Crawled by {0} = {1}.", (object) this.Name, (object) num)));
        this.PipelineContext.IndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} crawled {1} items for repository '{2}'.", (object) this.Name, (object) num, (object) this.m_crawlSpec.RepositoryId)));
        return OperationStatus.Succeeded;
      }
      catch (Exception ex)
      {
        string str = string.Empty;
        if (executionContext.RepositoryIndexingUnit.IndexingUnitType == "Git_Repository")
          str = string.Join(",", ((GitCrawlSpec) this.m_crawlSpec).CurrentBranchesInfo.Select<BranchInfo, string>((Func<BranchInfo, string>) (x => x.BranchName)));
        executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} failed while crawling metadata for branches ({1}) with error : {2}. ", (object) this.Name, (object) str, (object) ex.Message)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(this.TraceMetaData, ex);
        return OperationStatus.Failed;
      }
    }

    protected internal override sealed bool AllDocumentsAreProcessed() => true;

    protected override sealed void AnalyzeFeederResponse(
      CorePipelineContext<CodePipelineDocumentId, CodeDocument> pipelineContext,
      ESIndexFeedResponseData responseData,
      int totalItems)
    {
    }

    internal override sealed void HandlePipelineError(Exception ex)
    {
    }

    internal override sealed bool IsPrimaryRun() => true;

    protected internal override sealed OperationStatus PostRun(OperationStatus opStatus) => opStatus;

    protected override sealed void Prepare()
    {
    }

    protected internal override sealed void PrePreRun()
    {
    }

    protected internal override sealed void PreRun()
    {
    }

    internal override sealed IReadOnlyList<CorePipelineStage<CodePipelineDocumentId, CodeDocument>> RegisterStages() => (IReadOnlyList<CorePipelineStage<CodePipelineDocumentId, CodeDocument>>) new CorePipelineStage<CodePipelineDocumentId, CodeDocument>[0];

    internal override sealed bool ShouldRestartPipeline() => false;

    internal void ValidatePipelineDocs(IndexingExecutionContext indexingExecutionContext)
    {
      if (!indexingExecutionContext.RequestContext.IsDLITDocumentCreationEnabled())
        return;
      foreach (CodeDocument pipelineDoc in this.GetPipelineDocs())
      {
        if (pipelineDoc.CurrentState != PipelineDocumentState.MetaDataCrawled)
        {
          string message = FormattableString.Invariant(FormattableStringFactory.Create("DLIT_ERROR : Document{0} not found in expected state {1}.", (object) pipelineDoc, (object) PipelineDocumentState.MetaDataCrawled));
          if (indexingExecutionContext.RequestContext.IsDLITStrictValidationEnabled())
            throw new SearchServiceException(message);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082703, "Indexing Pipeline", "Indexing Pipeline", message);
        }
      }
    }

    internal virtual ICrawler GetTreeCrawler() => ((AbstractTreeCrawlerFactory) new TreeCrawlerFactory()).GetTreeCrawler(this.PipelineContext.IndexingExecutionContext, this.m_crawlSpec, this.PipelineContext.StorageContext);

    internal virtual PipelineDocumentCollection<CodePipelineDocumentId, CodeDocument> GetPipelineDocs() => GlobalPipelineContext.Get<CodePipelineDocumentId, CodeDocument>().PipelineDocs;
  }
}
