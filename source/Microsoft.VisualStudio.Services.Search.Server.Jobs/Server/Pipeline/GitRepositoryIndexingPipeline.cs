// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.GitRepositoryIndexingPipeline
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  internal class GitRepositoryIndexingPipeline : CodeIndexingPipeline
  {
    private readonly GitCrawlSpec m_gitCrawlSpec;
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1083090, "Indexing Pipeline", "Pipeline");

    internal GitRepositoryIndexingPipeline(CodeIndexingPipelineContext pipelineContext)
      : base(GitRepositoryIndexingPipeline.s_traceMetaData, nameof (GitRepositoryIndexingPipeline), pipelineContext)
    {
      if (!(this.PipelineContext.CrawlSpec is GitCrawlSpec crawlSpec))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Expected crawl spec to be of type [{0}] but found [{1}]", (object) typeof (GitCrawlSpec).FullName, (object) this.PipelineContext.CrawlSpec.GetType().FullName)));
      this.m_gitCrawlSpec = crawlSpec;
    }

    protected internal override Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent QueueContinuationOperation()
    {
      GitRepositoryBIEventData repositoryBiEventData = new GitRepositoryBIEventData((ExecutionContext) this.PipelineContext.IndexingExecutionContext);
      repositoryBiEventData.BranchesToBeBulkIndexed = new List<string>();
      if (this.m_gitCrawlSpec.LastIndexedBranchesInfo != null)
      {
        foreach (BranchInfo branchInfo in this.m_gitCrawlSpec.LastIndexedBranchesInfo)
        {
          GitIndexJobYieldData indexJobYieldData;
          if (this.m_gitCrawlSpec.BranchesToJobYieldDataInfo.TryGetValue(branchInfo.BranchName, out indexJobYieldData) && indexJobYieldData.IncompleteTreeCrawl)
            repositoryBiEventData.BranchesToBeBulkIndexed.Add(branchInfo.BranchName);
        }
      }
      if (!string.IsNullOrWhiteSpace(this.m_gitCrawlSpec.BranchName) && (this.m_gitCrawlSpec.LastIndexedBranchesInfo == null || this.m_gitCrawlSpec.LastIndexedBranchesInfo != null && !this.m_gitCrawlSpec.LastIndexedBranchesInfo.Any<BranchInfo>((Func<BranchInfo, bool>) (x => x.BranchName.Equals(this.m_gitCrawlSpec.BranchName)))) && ((CoreCrawlSpec) this.m_gitCrawlSpec).JobYieldData.IncompleteTreeCrawl)
        repositoryBiEventData.BranchesToBeBulkIndexed.Add(this.m_gitCrawlSpec.BranchName);
      return this.PipelineContext.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) this.PipelineContext.IndexingExecutionContext, new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.PipelineContext.IndexingUnitChangeEvent.LeaseId)
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = this.PipelineContext.IndexingUnitChangeEvent.ChangeType,
        ChangeData = (ChangeEventData) repositoryBiEventData,
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = (byte) 0
      });
    }

    protected internal override bool AllDocumentsAreProcessed() => !this.IsExecutionIncomplete();

    private bool IsExecutionIncomplete()
    {
      if (this.m_gitCrawlSpec.LastIndexedBranchesInfo != null)
      {
        foreach (BranchInfo branchInfo in this.m_gitCrawlSpec.LastIndexedBranchesInfo)
        {
          GitIndexJobYieldData indexJobYieldData = new GitIndexJobYieldData();
          if (this.m_gitCrawlSpec.BranchesToJobYieldDataInfo.TryGetValue(branchInfo.BranchName, out indexJobYieldData))
          {
            if (indexJobYieldData.IncompleteTreeCrawl)
              return true;
          }
          else
            throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("IsIncompleteTreeCrawl_BranchesToJobYieldDataInfo do not have data for branch {0}", (object) branchInfo.BranchName)));
        }
      }
      return !string.IsNullOrWhiteSpace(this.m_gitCrawlSpec.BranchName) && (this.m_gitCrawlSpec.LastIndexedBranchesInfo == null || this.m_gitCrawlSpec.LastIndexedBranchesInfo != null && !this.m_gitCrawlSpec.LastIndexedBranchesInfo.Any<BranchInfo>((Func<BranchInfo, bool>) (x => x.BranchName.Equals(this.m_gitCrawlSpec.BranchName)))) && ((CoreCrawlSpec) this.m_gitCrawlSpec).JobYieldData.IncompleteTreeCrawl;
    }

    internal override void HandlePipelineError(Exception ex)
    {
      if (this.m_gitCrawlSpec.BranchesToJobYieldDataInfo == null)
        return;
      IndexingExecutionContext executionContext = this.PipelineContext.IndexingExecutionContext;
      GitCodeRepoIndexingProperties properties = (GitCodeRepoIndexingProperties) executionContext.RepositoryIndexingUnit.Properties;
      bool flag = false;
      foreach (KeyValuePair<string, GitIndexJobYieldData> keyValuePair in this.m_gitCrawlSpec.BranchesToJobYieldDataInfo)
      {
        GitBranchIndexInfo gitBranchIndexInfo;
        if (!properties.BranchIndexInfo.TryGetValue(keyValuePair.Key, out gitBranchIndexInfo))
        {
          gitBranchIndexInfo = new GitBranchIndexInfo();
          properties.BranchIndexInfo.Add(keyValuePair.Key, gitBranchIndexInfo);
        }
        GitIndexJobYieldData indexJobYieldData = gitBranchIndexInfo.GitIndexJobYieldData;
        if (!indexJobYieldData.HasData() && !string.IsNullOrWhiteSpace(keyValuePair.Value.TargetVersion) && !keyValuePair.Value.TargetVersion.Equals(RepositoryConstants.DefaultLastIndexCommitId))
        {
          indexJobYieldData.TargetVersion = keyValuePair.Value.TargetVersion;
          indexJobYieldData.BaseVersion = keyValuePair.Value.BaseVersion;
          flag = true;
        }
      }
      if (!flag)
        return;
      executionContext.RepositoryIndexingUnit = executionContext.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, executionContext.RepositoryIndexingUnit);
      this.IndexingUnit = executionContext.RepositoryIndexingUnit;
    }

    internal override bool IsPrimaryRun()
    {
      GitCodeRepoIndexingProperties properties = this.IndexingUnit.Properties as GitCodeRepoIndexingProperties;
      GitCodeRepoTFSAttributes entityAttributes = this.IndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
      return properties == null || entityAttributes == null || string.IsNullOrWhiteSpace(entityAttributes.DefaultBranch) || !properties.BranchIndexInfo.Any<KeyValuePair<string, GitBranchIndexInfo>>() || !properties.BranchIndexInfo.ContainsKey(entityAttributes.DefaultBranch) || properties.BranchIndexInfo[entityAttributes.DefaultBranch] == null || properties.BranchIndexInfo[entityAttributes.DefaultBranch].GitIndexJobYieldData == null || !properties.BranchIndexInfo[entityAttributes.DefaultBranch].GitIndexJobYieldData.HasData();
    }
  }
}
