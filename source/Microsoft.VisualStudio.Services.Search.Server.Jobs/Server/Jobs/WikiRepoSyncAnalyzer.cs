// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.WikiRepoSyncAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class WikiRepoSyncAnalyzer : GitRepoSyncAnalyzer
  {
    private WikiHttpClientWrapper m_wikiHttpClientWrapper;
    private IIndexingUnitWikisDataAccess m_indexingUnitWikisDataAccess;

    public WikiRepoSyncAnalyzer(
      ExecutionContext executionContext,
      TraceMetaData traceMetaData,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
      : base(executionContext, traceMetaData, indexingUnitChangeEventHandler)
    {
      this.m_indexingUnitWikisDataAccess = this.m_dataAccessFactory.GetIndexingUnitWikisDataAccess();
    }

    internal WikiRepoSyncAnalyzer(
      ExecutionContext executionContext,
      TraceMetaData traceMetaData,
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      IndexMetadataStateAnalyser indexMetadataStateAnalyser,
      IIndexingUnitWikisDataAccess indexingUnitWikisDataAccess)
      : base(executionContext, traceMetaData, dataAccessFactory, indexingUnitChangeEventHandler, indexMetadataStateAnalyser)
    {
      this.m_indexingUnitWikisDataAccess = indexingUnitWikisDataAccess ?? this.m_dataAccessFactory.GetIndexingUnitWikisDataAccess();
    }

    internal override EntityFinalizerBase FinalizeHelper => (EntityFinalizerBase) new CollectionWikiFinalizeHelper();

    public WikiHttpClientWrapper WikiHttpClient
    {
      get
      {
        if (this.m_wikiHttpClientWrapper == null)
          this.m_wikiHttpClientWrapper = new WikiHttpClientWrapper(this.ExecutionContext, this.TraceMetadata);
        return this.m_wikiHttpClientWrapper;
      }
      set => this.m_wikiHttpClientWrapper = value;
    }

    internal override bool CreateRepositoryIndexingUnitIfNeeded(
      ExecutionContext executionContext,
      GitRepository gitRepository,
      Func<ExecutionContext, GitRepository, ProjectHttpClientWrapper, bool> isDisabled,
      string sourceForTraceInfo,
      out Microsoft.VisualStudio.Services.Search.Common.IndexingUnit projectIndexingUnit,
      out Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit,
      out bool repoIndexingUnitCreated,
      bool isShadow = false)
    {
      IEnumerable<WikiV2> wikisInTfsRepo = this.GetWikisInTfsRepo(gitRepository.Id, gitRepository.ProjectReference.Id);
      if (wikisInTfsRepo.Count<WikiV2>() == 1 && wikisInTfsRepo.First<WikiV2>().Type == WikiType.ProjectWiki)
      {
        WikiV2 wikiV2 = wikisInTfsRepo.First<WikiV2>();
        gitRepository.Name = wikiV2.Name;
        GitRepository gitRepository1 = gitRepository;
        IEnumerable<GitVersionDescriptor> versions = wikiV2.Versions;
        string str = (versions != null ? (versions.Count<GitVersionDescriptor>() > 0 ? 1 : 0) : 0) == 0 || string.IsNullOrWhiteSpace(wikiV2.Versions.First<GitVersionDescriptor>().Version) ? "" : "refs/heads/" + wikiV2.Versions.First<GitVersionDescriptor>().Version;
        gitRepository1.DefaultBranch = str;
      }
      if (!base.CreateRepositoryIndexingUnitIfNeeded(executionContext, gitRepository, isDisabled, sourceForTraceInfo, out projectIndexingUnit, out repoIndexingUnit, out repoIndexingUnitCreated))
        return false;
      if (this.RequestContext.IsFeatureEnabled("Search.Server.Wiki.ProductDocumentationSearch"))
        this.m_indexingUnitWikisDataAccess.AddIndexingUnitWikisEntry(this.RequestContext, new IndexingUnitWikisEntry()
        {
          Wikis = this.GetWikisAddedInTfs(Enumerable.Empty<WikiV2>(), this.GetWikisInTfsRepo(gitRepository.Id, gitRepository.ProjectReference.Id)).ToList<WikiV2>(),
          IndexingUnitId = repoIndexingUnit.IndexingUnitId
        });
      return true;
    }

    internal override List<GitRepository> GetTfsRepos()
    {
      if (this.RequestContext.IsFeatureEnabled("Search.Server.Wiki.ProductDocumentationSearch"))
      {
        IEnumerable<Guid> wikiRepoIds = this.WikiHttpClient.GetAllWikisAsync().Select<WikiV2, Guid>((Func<WikiV2, Guid>) (wiki => wiki.RepositoryId));
        this.GitHttpClient = this.GitHttpClient ?? new GitHttpClientWrapper(this.ExecutionContext, this.TraceMetadata);
        IEnumerable<GitRepository> source = this.GitHttpClient.GetRepositoriesAsync(true).Where<GitRepository>((Func<GitRepository, bool>) (repo => wikiRepoIds.Contains<Guid>(repo.Id)));
        return source == null ? (List<GitRepository>) null : source.ToList<GitRepository>();
      }
      IEnumerable<GitRepository> repositoriesAsync = this.WikiHttpClient.GetWikiRepositoriesAsync();
      return repositoriesAsync == null ? (List<GitRepository>) null : repositoriesAsync.ToList<GitRepository>();
    }

    internal virtual IEnumerable<WikiV2> GetWikisInTfsRepo(Guid repositoryId, Guid projectId) => this.WikiHttpClient.GetAllWikisAsync(projectId.ToString()).Where<WikiV2>((Func<WikiV2, bool>) (wiki => wiki.RepositoryId == repositoryId));

    internal virtual IEnumerable<WikiV2> GetWikisAddedInTfs(
      IEnumerable<WikiV2> wikisInSearch,
      IEnumerable<WikiV2> wikisInTfs)
    {
      wikisInSearch = wikisInSearch ?? Enumerable.Empty<WikiV2>();
      wikisInTfs = wikisInTfs ?? Enumerable.Empty<WikiV2>();
      HashSet<Guid> wikisInSearchIds = new HashSet<Guid>(wikisInSearch.Select<WikiV2, Guid>((Func<WikiV2, Guid>) (w => w.Id)));
      return wikisInTfs.Where<WikiV2>((Func<WikiV2, bool>) (w => !wikisInSearchIds.Contains(w.Id))).Select<WikiV2, WikiV2>((Func<WikiV2, WikiV2>) (w =>
      {
        WikiV2 wikisAddedInTfs = new WikiV2();
        wikisAddedInTfs.Id = w.Id;
        wikisAddedInTfs.Name = w.Name;
        wikisAddedInTfs.MappedPath = w.MappedPath;
        wikisAddedInTfs.Type = w.Type;
        IEnumerable<GitVersionDescriptor> versions = w.Versions;
        wikisAddedInTfs.Versions = versions != null ? versions.Take<GitVersionDescriptor>(1) : (IEnumerable<GitVersionDescriptor>) null;
        return wikisAddedInTfs;
      }));
    }

    internal virtual IEnumerable<WikiV2> GetWikisRemovedInTfs(
      IEnumerable<WikiV2> wikisInSearch,
      IEnumerable<WikiV2> wikisInTfs)
    {
      wikisInSearch = wikisInSearch ?? Enumerable.Empty<WikiV2>();
      wikisInTfs = wikisInTfs ?? Enumerable.Empty<WikiV2>();
      HashSet<Guid> wikisInTfsIds = new HashSet<Guid>(wikisInTfs.Select<WikiV2, Guid>((Func<WikiV2, Guid>) (w => w.Id)));
      return wikisInSearch.Where<WikiV2>((Func<WikiV2, bool>) (w => !wikisInTfsIds.Contains(w.Id)));
    }

    internal override List<string> GetBranchesToIndex(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepoIndexingUnit)
    {
      IndexingUnitWikisEntry indexingUnitWikisEntry = this.m_indexingUnitWikisDataAccess.GetIndexingUnitWikisEntry(this.ExecutionContext.RequestContext, gitRepoIndexingUnit.IndexingUnitId);
      return indexingUnitWikisEntry == null ? base.GetBranchesToIndex(gitRepoIndexingUnit) : indexingUnitWikisEntry.GetBranchesToBeIndexed();
    }

    protected override IEntityType EntityType => (IEntityType) WikiEntityType.GetInstance();
  }
}
