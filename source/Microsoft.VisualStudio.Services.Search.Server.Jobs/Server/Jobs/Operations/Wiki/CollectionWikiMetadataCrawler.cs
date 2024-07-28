// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki.CollectionWikiMetadataCrawler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki
{
  internal class CollectionWikiMetadataCrawler : ICollectionMetadataCrawler
  {
    private const string s_traceArea = "Indexing Pipeline";
    private const string s_traceLayer = "IndexingOperation";
    private WikiHttpClientWrapper m_wikiHttpClientWrapper;
    private GitHttpClientWrapper m_gitHttpClientWrapper;

    internal CollectionWikiMetadataCrawler(
      IndexingExecutionContext executionContext,
      IDataAccessFactory dataAccessFactory)
      : this(dataAccessFactory, new WikiHttpClientWrapper((ExecutionContext) executionContext, new TraceMetaData(1080713, "Indexing Pipeline", "IndexingOperation")), new GitHttpClientWrapper((ExecutionContext) executionContext, new TraceMetaData(1080713, "Indexing Pipeline", "IndexingOperation")))
    {
    }

    internal CollectionWikiMetadataCrawler(
      IDataAccessFactory dataAccessFactory,
      WikiHttpClientWrapper wikiHttpClientWrapper,
      GitHttpClientWrapper gitHttpClientWrapper)
    {
      this.m_wikiHttpClientWrapper = wikiHttpClientWrapper;
      this.m_gitHttpClientWrapper = gitHttpClientWrapper;
      this.IndexingUnitWikisDataAccess = dataAccessFactory.GetIndexingUnitWikisDataAccess();
      this.BaseIndexingUnitDataAccess = dataAccessFactory.GetIndexingUnitDataAccess();
    }

    internal IIndexingUnitWikisDataAccess IndexingUnitWikisDataAccess { get; set; }

    internal IIndexingUnitDataAccess BaseIndexingUnitDataAccess { get; set; }

    public List<IndexingUnitWithSize> CrawlMetadata(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      bool isShadowCrawlingRequired = false)
    {
      int currentEstimatedDocumentCount = 0;
      int estimatedGrowth = 0;
      IEnumerable<WikiV2> wikis;
      IEnumerable<GitRepository> wikiRepos = this.GetWikiRepos(indexingExecutionContext, out wikis);
      List<IndexingUnitWithSize> indexingUnitWithSizeList = new List<IndexingUnitWithSize>();
      if (wikiRepos != null)
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        Dictionary<Guid, IndexingUnitWikisEntry> dictionary1 = new Dictionary<Guid, IndexingUnitWikisEntry>();
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> projectIndexingUnits = this.GetOrCreateProjectIndexingUnits(indexingExecutionContext, wikiRepos, collectionIndexingUnit.IndexingUnitId);
        Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> repoIndexingUnits = this.GetExistingWikiRepoIndexingUnits(indexingExecutionContext);
        bool flag = indexingExecutionContext.RequestContext.IsFeatureEnabled("Search.Server.Wiki.ProductDocumentationSearch");
        foreach (GitRepository gitRepository1 in wikiRepos)
        {
          GitRepository repo = gitRepository1;
          IEnumerable<WikiV2> source = wikis.Where<WikiV2>((Func<WikiV2, bool>) (w => w.RepositoryId == repo.Id));
          if (source.Count<WikiV2>() == 1 && source.First<WikiV2>().Type == WikiType.ProjectWiki)
          {
            WikiV2 wikiV2 = source.First<WikiV2>();
            repo.Name = wikiV2.Name;
            GitRepository gitRepository2 = repo;
            IEnumerable<GitVersionDescriptor> versions = wikiV2.Versions;
            string str = (versions != null ? (versions.Count<GitVersionDescriptor>() > 0 ? 1 : 0) : 0) == 0 || string.IsNullOrWhiteSpace(wikiV2.Versions.First<GitVersionDescriptor>().Version) ? "" : "refs/heads/" + wikiV2.Versions.First<GitVersionDescriptor>().Version;
            gitRepository2.DefaultBranch = str;
          }
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1;
          if (repoIndexingUnits != null && repoIndexingUnits.TryGetValue(repo.Id, out indexingUnit1))
          {
            indexingUnitList.Add(indexingUnit1);
            indexingUnitWithSizeList.Add(new IndexingUnitWithSize(indexingUnit1, currentEstimatedDocumentCount, estimatedGrowth, true));
          }
          else
          {
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 = projectIndexingUnits.Find((Predicate<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (item => item.TFSEntityId == repo.ProjectReference.Id));
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit3 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnit(repo.Id, "Git_Repository", (IEntityType) WikiEntityType.GetInstance(), indexingUnit2.IndexingUnitId);
            GitCodeRepoTFSAttributes repoTfsAttributes = new GitCodeRepoTFSAttributes();
            repoTfsAttributes.RepositoryName = repo.Name;
            repoTfsAttributes.RemoteUrl = repo.RemoteUrl;
            repoTfsAttributes.DefaultBranch = repo.DefaultBranch;
            indexingUnit3.TFSEntityAttributes = (TFSEntityAttributes) repoTfsAttributes;
            GitCodeRepoIndexingProperties indexingProperties = new GitCodeRepoIndexingProperties();
            indexingProperties.Name = repo.Name;
            indexingUnit3.Properties = (IndexingProperties) indexingProperties;
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit4 = indexingUnit3;
            indexingUnitList.Add(indexingUnit4);
            indexingUnitWithSizeList.Add(new IndexingUnitWithSize(indexingUnit4, currentEstimatedDocumentCount, estimatedGrowth, true));
            if (flag)
              dictionary1.Add(indexingUnit4.TFSEntityId, new IndexingUnitWikisEntry()
              {
                Wikis = source.Select<WikiV2, WikiV2>((Func<WikiV2, WikiV2>) (w =>
                {
                  WikiV2 wikiV2 = new WikiV2();
                  wikiV2.Id = w.Id;
                  wikiV2.Name = w.Name;
                  wikiV2.MappedPath = w.MappedPath;
                  wikiV2.Type = w.Type;
                  IEnumerable<GitVersionDescriptor> versions = w.Versions;
                  wikiV2.Versions = versions != null ? versions.Take<GitVersionDescriptor>(1) : (IEnumerable<GitVersionDescriptor>) null;
                  return wikiV2;
                })).ToList<WikiV2>()
              });
          }
        }
        if (indexingUnitList.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        {
          indexingUnitList = this.BaseIndexingUnitDataAccess.AddOrUpdateIndexingUnits(indexingExecutionContext.RequestContext, indexingUnitList, true);
          Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary2 = indexingUnitList.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x));
          foreach (IndexingUnitWithSize indexingUnitWithSize in indexingUnitWithSizeList)
            indexingUnitWithSize.IndexingUnit = dictionary2[indexingUnitWithSize.IndexingUnit.TFSEntityId];
        }
        foreach (KeyValuePair<Guid, IndexingUnitWikisEntry> keyValuePair in dictionary1)
        {
          Guid repoId = keyValuePair.Key;
          IndexingUnitWikisEntry indexingUnitWikisEntry = keyValuePair.Value;
          IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> source = indexingUnitList.Where<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (repoIU => repoIU.TFSEntityId == repoId));
          if (source.Count<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>() == 1)
          {
            indexingUnitWikisEntry.IndexingUnitId = source.First<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>().IndexingUnitId;
            this.IndexingUnitWikisDataAccess.AddIndexingUnitWikisEntry(indexingExecutionContext.RequestContext, indexingUnitWikisEntry);
          }
        }
      }
      return indexingUnitWithSizeList;
    }

    internal virtual IEnumerable<GitRepository> GetWikiRepos(
      IndexingExecutionContext indexingExecutionContext,
      out IEnumerable<WikiV2> wikis)
    {
      wikis = this.m_wikiHttpClientWrapper.GetAllWikisAsync();
      if (!indexingExecutionContext.RequestContext.IsFeatureEnabled("Search.Server.Wiki.ProductDocumentationSearch"))
        return this.m_wikiHttpClientWrapper.GetWikiRepositoriesAsync();
      IEnumerable<Guid> wikiRepoIds = wikis.Select<WikiV2, Guid>((Func<WikiV2, Guid>) (wiki => wiki.RepositoryId));
      return this.m_gitHttpClientWrapper.GetRepositoriesAsync(true).Where<GitRepository>((Func<GitRepository, bool>) (repo => wikiRepoIds.Contains<Guid>(repo.Id)));
    }

    private List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetOrCreateProjectIndexingUnits(
      IndexingExecutionContext executionContext,
      IEnumerable<GitRepository> repositoryCollection,
      int collectionIndexingUnitId)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> second = this.BaseIndexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, "Project", (IEntityType) WikiEntityType.GetInstance(), -1) ?? new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> source = repositoryCollection.Select<GitRepository, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<GitRepository, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (repo => Microsoft.VisualStudio.Services.Search.Common.IndexingUnit.CreateProjectIndexingUnitFrom(repo, collectionIndexingUnitId, (IEntityType) WikiEntityType.GetInstance()))).Distinct<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>().Except<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) second);
      if (source.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> collection = (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) this.BaseIndexingUnitDataAccess.AddOrUpdateIndexingUnits(executionContext.RequestContext, source.ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>(), true);
        second.AddRange(collection);
      }
      return second;
    }

    private Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetExistingWikiRepoIndexingUnits(
      IndexingExecutionContext indexingExecutionContext)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.BaseIndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "Git_Repository", (IEntityType) WikiEntityType.GetInstance(), -1);
      return indexingUnits == null ? (Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) null : indexingUnits.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (indexingUnit => indexingUnit.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (indexingUnit2 => indexingUnit2));
    }
  }
}
