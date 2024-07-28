// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.CodeIndexingPipelineContext
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  public class CodeIndexingPipelineContext : 
    FirstPartyPipelineContext<CodePipelineDocumentId, CodeDocument>
  {
    public CodeIndexingPipelineContext(
      IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Indexer.IndexingExecutionContext indexingExecutionContext,
      CodeCrawlSpec crawlSpec,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      string branchName,
      List<string> branches,
      bool supportsStoringFiles,
      bool isSingleExecutionPipeline)
      : this(indexingUnit, indexingExecutionContext, crawlSpec, indexingUnitChangeEvent, indexingUnitChangeEventHandler, branchName, branches, new StorageContext(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit.TFSEntityId, string.IsNullOrWhiteSpace(branchName) ? string.Join(string.Empty, (IEnumerable<string>) (branches ?? new List<string>())) : branchName, indexingExecutionContext.IndexingUnit.EntityType, indexingExecutionContext.IndexingUnit.IsShadow), supportsStoringFiles, isSingleExecutionPipeline)
    {
    }

    internal CodeIndexingPipelineContext(
      IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Indexer.IndexingExecutionContext indexingExecutionContext,
      CodeCrawlSpec crawlSpec,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      string branchName,
      List<string> branches,
      StorageContext storageContext,
      bool supportsStoringFiles,
      bool isSingleExecutionPipeline)
      : base(indexingUnit, indexingExecutionContext, (CoreCrawlSpec) crawlSpec, indexingUnitChangeEvent, indexingUnitChangeEventHandler, storageContext, isSingleExecutionPipeline)
    {
      this.SupportsStoringFiles = supportsStoringFiles;
      this.CrawlSpec = crawlSpec;
      this.BranchName = branchName ?? string.Empty;
      this.Branches = branches ?? new List<string>();
    }

    public CodeCrawlSpec CrawlSpec { get; }

    internal bool SupportsStoringFiles { get; }

    internal string BranchName { get; }

    internal List<string> Branches { get; }
  }
}
