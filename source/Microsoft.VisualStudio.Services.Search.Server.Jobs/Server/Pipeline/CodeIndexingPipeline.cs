// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.CodeIndexingPipeline
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Store;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  public class CodeIndexingPipeline : FirstPartyPipeline<CodePipelineDocumentId, CodeDocument>
  {
    private IStoreService m_storeService;
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1083081, "Indexing Pipeline", "Pipeline");

    public CodeIndexingPipeline(CodeIndexingPipelineContext pipelineContext)
      : this(CodeIndexingPipeline.s_traceMetaData, nameof (CodeIndexingPipeline), pipelineContext)
    {
    }

    protected CodeIndexingPipeline(
      TraceMetaData traceMetaData,
      string name,
      CodeIndexingPipelineContext pipelineContext)
      : base(traceMetaData, name, (FirstPartyPipelineContext<CodePipelineDocumentId, CodeDocument>) pipelineContext)
    {
      this.PipelineContext = pipelineContext;
    }

    protected CodeIndexingPipeline(
      TraceMetaData traceMetaData,
      string name,
      CodeIndexingPipelineContext pipelineContext,
      CorePipelineFlowHandler flowHandler)
      : base(traceMetaData, name, (FirstPartyPipelineContext<CodePipelineDocumentId, CodeDocument>) pipelineContext, flowHandler)
    {
      this.PipelineContext = pipelineContext;
    }

    protected internal override OperationStatus PostPostRun(OperationStatus opStatus)
    {
      if (this.PipelineContext.SupportsStoringFiles)
      {
        this.StoreService().Run(this.PipelineContext.IndexingExecutionContext);
        this.PipelineContext.IndexingExecutionContext.Log.Append("Executed Store service");
        this.VerifyStateOfPipelineDocuments();
      }
      return base.PostPostRun(opStatus);
    }

    internal virtual void VerifyStateOfPipelineDocuments()
    {
      IReadOnlyCollection<CodeDocument> fedToCustomStore = this.GetDocumentsThatAreNotFedToCustomStore();
      if (fedToCustomStore.Count <= 0)
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082703, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, string.Join(Environment.NewLine, fedToCustomStore.Take<CodeDocument>(100).Select<CodeDocument, string>((Func<CodeDocument, string>) (d => FormattableString.Invariant(FormattableStringFactory.Create("Document [{0}] is in state [{1}]", (object) d, (object) d.CurrentState))))));
      string str = FormattableString.Invariant(FormattableStringFactory.Create("[{0}] documents are in unexpected states after the execution of stage. ", (object) fedToCustomStore.Count));
      if (this.PipelineContext.IndexingExecutionContext.RequestContext.IsDLITStrictValidationEnabled())
        throw new SearchServiceException(str + FormattableString.Invariant(FormattableStringFactory.Create("Check ProductTrace messages of TracePoint {0} for more details.", (object) 1082703)));
    }

    private IReadOnlyCollection<CodeDocument> GetDocumentsThatAreNotFedToCustomStore()
    {
      List<CodeDocument> fedToCustomStore = new List<CodeDocument>();
      foreach (CodeDocument pipelineDoc in this.PipelineContext.PipelineDocs)
      {
        if (pipelineDoc.ShouldProcess && pipelineDoc.CurrentState != PipelineDocumentState.FedToCustomStore)
          fedToCustomStore.Add(pipelineDoc);
      }
      return (IReadOnlyCollection<CodeDocument>) fedToCustomStore;
    }

    internal virtual IStoreService StoreService()
    {
      if (this.m_storeService == null)
        this.m_storeService = (IStoreService) new Microsoft.VisualStudio.Services.Search.Server.Store.StoreService(this.PipelineContext.IndexingExecutionContext, this.PipelineContext.StorageContext);
      return this.m_storeService;
    }

    internal override IReadOnlyList<CorePipelineStage<CodePipelineDocumentId, CodeDocument>> RegisterStages()
    {
      List<CorePipelineStage<CodePipelineDocumentId, CodeDocument>> corePipelineStageList = new List<CorePipelineStage<CodePipelineDocumentId, CodeDocument>>()
      {
        CorePipelinePluginsFactory.GetPipelineStage<CodePipelineDocumentId, CodeDocument>((CoreIndexingExecutionContext) this.PipelineContext.IndexingExecutionContext, this.GetCodeCrawlerStageName(), (object) this.PipelineContext.CrawlSpec, (object) this.PipelineContext.StorageContext),
        CorePipelinePluginsFactory.GetPipelineStage<CodePipelineDocumentId, CodeDocument>((CoreIndexingExecutionContext) this.PipelineContext.IndexingExecutionContext, this.GetCodeParserStageName())
      };
      CorePipelineStage<CodePipelineDocumentId, CodeDocument> feeder;
      if (this.TryGetFirstPartyFeeder("CodeFeeder", out feeder))
        corePipelineStageList.Add(feeder);
      return (IReadOnlyList<CorePipelineStage<CodePipelineDocumentId, CodeDocument>>) corePipelineStageList;
    }

    protected internal virtual string GetCodeCrawlerStageName()
    {
      CodeCrawlSpec crawlSpec = this.PipelineContext.CrawlSpec;
      switch (crawlSpec)
      {
        case TfvcIndexCrawlSpec _:
        case CustomCrawlSpec _:
        case FileGroupCrawlSpec _:
        case GitCrawlSpec _:
          return "CodeCrawler";
        case TfvcPatchIndexCrawlSpec patchIndexCrawlSpec:
          if (patchIndexCrawlSpec.Patch == Patch.RepositoryHeal && this.PipelineContext.IndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/SearchShared/Settings/EnableMultiBranchRepoHeal", true))
            return "CodeCrawler";
          break;
      }
      if (patchIndexCrawlSpec != null && (patchIndexCrawlSpec.Patch != Patch.RepositoryHeal || !this.PipelineContext.IndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/SearchShared/Settings/EnableMultiBranchRepoHeal", true)))
        return "TfvcPatchHttpCrawler";
      throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("CrawlSpec of type [{0}] is not supported by [{1}]", (object) crawlSpec.GetType().FullName, (object) this.Name)));
    }

    internal string GetCodeParserStageName()
    {
      switch (this.PipelineContext.IndexingUnit.EntityType.Name)
      {
        case "Code":
          return "CodeParser";
        case "Wiki":
          return "WikiParser";
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Parser of entity type [{0}] is not supported by [{1}]", (object) this.PipelineContext.IndexingUnit.EntityType.Name, (object) this.Name)));
      }
    }

    protected CodeIndexingPipelineContext PipelineContext { get; }
  }
}
