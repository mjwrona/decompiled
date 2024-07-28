// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki.WikiRepositoryPatchOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki
{
  internal class WikiRepositoryPatchOperation : RepositoryCodePatchOperation
  {
    private IndexingUnitWikisEntry m_indexingUnitWikisEntry;

    public WikiRepositoryPatchOperation(
      ExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, (IndexingUnitWikisEntry) null)
    {
    }

    internal WikiRepositoryPatchOperation(
      ExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      IndexingUnitWikisEntry indexingUnitWikisEntry)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_indexingUnitWikisEntry = indexingUnitWikisEntry ?? this.DataAccessFactory.GetIndexingUnitWikisDataAccess().GetIndexingUnitWikisEntry(executionContext.RequestContext, indexingUnit.IndexingUnitId);
    }

    internal override CodeCrawlSpec CreateCrawlSpec(
      IndexingExecutionContext iexContext,
      ref string branchName,
      in List<string> branches)
    {
      IVssRequestContext requestContext = iexContext.RequestContext;
      if (!(this.IndexingUnit.IndexingUnitType == "Git_Repository"))
        return (CodeCrawlSpec) null;
      GitCodeRepoTFSAttributes entityAttributes = this.IndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
      BranchInfo branchInfo = new BranchInfo()
      {
        BranchName = branchName,
        ChangeId = RepositoryConstants.DefaultLastIndexCommitId,
        ChangeTime = RepositoryConstants.DefaultLastIndexChangeUtcTime
      };
      WikiPatchIndexCrawlSpec crawlSpec = new WikiPatchIndexCrawlSpec();
      ((CoreCrawlSpec) crawlSpec).CollectionName = iexContext.CollectionName;
      Guid guid = iexContext.CollectionId;
      ((CoreCrawlSpec) crawlSpec).CollectionId = guid.ToString();
      ((CodeCrawlSpec) crawlSpec).ProjectName = iexContext.ProjectName;
      ((CodeCrawlSpec) crawlSpec).ProjectId = iexContext.ProjectId.ToString();
      ((GitPatchIndexCrawlSpec) crawlSpec).ProjectVisibility = iexContext.ProjectVisibility;
      ((CodeCrawlSpec) crawlSpec).RepositoryName = iexContext.RepositoryName;
      guid = this.IndexingUnit.TFSEntityId;
      ((CodeCrawlSpec) crawlSpec).RepositoryId = guid.ToString();
      ((GitCrawlSpec) crawlSpec).CurrentBranchesInfo = new List<BranchInfo>()
      {
        branchInfo
      };
      ((GitCrawlSpec) crawlSpec).BranchName = branchName;
      ((CodeCrawlSpec) crawlSpec).VcType = VersionControlType.Git;
      ((GitPatchIndexCrawlSpec) crawlSpec).PatchDescriptions = this.Patches;
      ((GitCrawlSpec) crawlSpec).GitRepoRemoteUrl = entityAttributes.RemoteUrl;
      ((GitPatchIndexCrawlSpec) crawlSpec).ShouldUpdateLastChangeInfo = false;
      ((GitCrawlSpec) crawlSpec).IsDefaultBranch = branchName == entityAttributes.DefaultBranch;
      ((CoreCrawlSpec) crawlSpec).JobYieldData = (AbstractJobYieldData) new GitIndexJobYieldData();
      ((CodeCrawlSpec) crawlSpec).DefaultBranchName = entityAttributes.DefaultBranch;
      crawlSpec.Wikis = this.GetWikis(iexContext, branchName);
      return (CodeCrawlSpec) crawlSpec;
    }

    internal override string HandleGitRepoBranchUpdates(
      IndexingExecutionContext indexingExecutionContext)
    {
      GitRepoSyncAnalyzer repoSyncAnalyzer = new GitRepoSyncAnalyzerFactory().GetGitRepoSyncAnalyzer((ExecutionContext) indexingExecutionContext, this.TraceMetaData, this.IndexingUnitChangeEventHandler, (IEntityType) WikiEntityType.GetInstance());
      if (indexingExecutionContext.RequestContext.IsFeatureEnabled("Search.Server.Wiki.ProductDocumentationSearch"))
        return new WikiRepoScopesAndVersionsUpdateTask(this.IndexingUnit, this.IndexingUnitDataAccess, this.DataAccessFactory.GetIndexingUnitWikisDataAccess(), this.IndexingUnitChangeEvent, repoSyncAnalyzer as WikiRepoSyncAnalyzer).HandleScopeAndVersionsUpdates(indexingExecutionContext);
      return string.IsNullOrWhiteSpace((this.IndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes).DefaultBranch) ? new GitBranchUpdatesHandlerTask(this.IndexingUnit, this.IndexingUnitDataAccess, this.IndexingUnitChangeEvent, repoSyncAnalyzer).HandleGitBranchUpdates(indexingExecutionContext) : FormattableString.Invariant(FormattableStringFactory.Create("For wiki, default branch change for the backing git repository is not required to be processed"));
    }

    internal override List<string> GetBranchesToIndex()
    {
      List<string> branchesToIndex = new List<string>();
      if (this.IndexingUnit.IndexingUnitType == "Git_Repository")
      {
        if (this.m_indexingUnitWikisEntry == null)
        {
          GitCodeRepoTFSAttributes entityAttributes = this.IndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
          branchesToIndex.Add(entityAttributes.DefaultBranch);
        }
        else
          branchesToIndex = this.m_indexingUnitWikisEntry.GetBranchesToBeIndexed();
      }
      return branchesToIndex;
    }

    private List<WikiV2> GetWikis(IndexingExecutionContext iexContext, string branchName)
    {
      List<WikiV2> wikiV2List1 = new List<WikiV2>();
      List<WikiV2> wikis;
      if (this.m_indexingUnitWikisEntry == null)
      {
        List<WikiV2> wikiV2List2 = new List<WikiV2>();
        WikiV2 wikiV2 = new WikiV2();
        wikiV2.Id = this.IndexingUnit.TFSEntityId;
        wikiV2.Name = iexContext.RepositoryName;
        wikiV2.MappedPath = "/";
        wikiV2.Type = WikiType.ProjectWiki;
        wikiV2.Versions = (IEnumerable<GitVersionDescriptor>) new List<GitVersionDescriptor>()
        {
          new GitVersionDescriptor()
          {
            VersionType = GitVersionType.Branch,
            Version = branchName.Substring("refs/heads/".Length)
          }
        };
        wikiV2List2.Add(wikiV2);
        wikis = wikiV2List2;
      }
      else
        wikis = this.m_indexingUnitWikisEntry.Wikis;
      return wikis;
    }
  }
}
