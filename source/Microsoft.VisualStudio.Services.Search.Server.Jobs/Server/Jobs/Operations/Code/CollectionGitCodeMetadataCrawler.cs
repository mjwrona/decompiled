// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CollectionGitCodeMetadataCrawler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class CollectionGitCodeMetadataCrawler : AbstractCollectionCodeMetadataCrawler
  {
    private GitHttpClientWrapper m_gitHttpClientWrapper;
    private TraceMetaData m_traceMetadata;
    private bool m_isReindexingIsProgress;

    internal CollectionGitCodeMetadataCrawler(
      IndexingExecutionContext executionContext,
      IDataAccessFactory dataAccessFactory,
      CodeFileContract codeFileContract,
      GitHttpClientWrapper gitHttpClientWrapper = null)
      : base(executionContext, dataAccessFactory, "Git_Repository", codeFileContract)
    {
      this.Initialize(executionContext, gitHttpClientWrapper);
    }

    private void Initialize(
      IndexingExecutionContext executionContext,
      GitHttpClientWrapper gitHttpClientWrapper)
    {
      this.m_traceMetadata = new TraceMetaData(1080611, AbstractCollectionCodeMetadataCrawler.s_traceArea, AbstractCollectionCodeMetadataCrawler.s_traceLayer);
      this.m_gitHttpClientWrapper = gitHttpClientWrapper != null ? gitHttpClientWrapper : new GitHttpClientWrapper((ExecutionContext) this.CodeIndexingExecutionContext, this.m_traceMetadata);
      this.m_isReindexingIsProgress = executionContext.IsReindexingFailedOrInProgress(executionContext.DataAccessFactory, this.EntityType);
    }

    public override List<IndexingUnitWithSize> CrawlMetadata(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      bool isShadowCrawlingRequired = false)
    {
      IEnumerable<GitRepository> repositoriesAsync = this.m_gitHttpClientWrapper.GetRepositoriesAsync();
      List<IndexingUnitWithSize> indexingUnitWithSizeList = new List<IndexingUnitWithSize>();
      if (repositoriesAsync != null)
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> projectIndexingUnits = this.GetOrCreateProjectIndexingUnits(repositoriesAsync, collectionIndexingUnit.IndexingUnitId);
        IDictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> repoIndexingUnits = this.GetExistingRepoIndexingUnits(isShadowCrawlingRequired);
        foreach (GitRepository gitRepository in repositoriesAsync)
        {
          GitRepository repo = gitRepository;
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1;
          if (!repoIndexingUnits.TryGetValue(repo.Id, out indexingUnit1))
          {
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 = projectIndexingUnits.Find((Predicate<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (item => item.TFSEntityId == repo.ProjectReference.Id));
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit3 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnit(repo.Id, "Git_Repository", this.EntityType, indexingUnit2.IndexingUnitId, isShadowCrawlingRequired);
            GitCodeRepoTFSAttributes repoTfsAttributes1;
            if (!isShadowCrawlingRequired)
            {
              GitCodeRepoTFSAttributes repoTfsAttributes2 = new GitCodeRepoTFSAttributes();
              repoTfsAttributes2.RepositoryName = repo.Name;
              repoTfsAttributes2.RemoteUrl = repo.RemoteUrl;
              repoTfsAttributes2.DefaultBranch = repo.DefaultBranch;
              repoTfsAttributes1 = repoTfsAttributes2;
            }
            else
              repoTfsAttributes1 = this.CreateGitCodeRepoTFSAttributesForShadowIndexingUnit(indexingExecutionContext, repo);
            indexingUnit3.TFSEntityAttributes = (TFSEntityAttributes) repoTfsAttributes1;
            GitCodeRepoIndexingProperties indexingProperties1;
            if (!isShadowCrawlingRequired)
            {
              GitCodeRepoIndexingProperties indexingProperties2 = new GitCodeRepoIndexingProperties();
              indexingProperties2.Name = repo.Name;
              indexingProperties2.IsDisabled = this.IsDisabled(repo);
              indexingProperties1 = indexingProperties2;
            }
            else
              indexingProperties1 = this.CreateGitCodeRepoPropertiesForShadowIndexingUnit(indexingExecutionContext, repo);
            indexingUnit3.Properties = (IndexingProperties) indexingProperties1;
            indexingUnit1 = indexingUnit3;
          }
          int num = indexingUnit1.GetBranchCountFromTFSAttributesIfGitRepo();
          if (num <= 0)
            num = 1;
          double factorForMultibranch = indexingExecutionContext.ProvisioningContext.ContractType.GetFileCountFactorForMultibranch(indexingExecutionContext.RequestContext, indexingUnit1, num);
          int estimatedDocCount;
          int estimatedDocCountGrowth;
          long actualInitialSize;
          bool sizeEstimates = this.GetSizeEstimates(indexingExecutionContext, repo, indexingUnit1, num, factorForMultibranch, out estimatedDocCount, out estimatedDocCountGrowth, out actualInitialSize);
          indexingUnitWithSizeList.Add(new IndexingUnitWithSize(indexingUnit1, estimatedDocCount, estimatedDocCountGrowth, sizeEstimates)
          {
            ActualInitialSize = actualInitialSize
          });
        }
      }
      return indexingUnitWithSizeList;
    }

    internal bool GetSizeEstimates(
      IndexingExecutionContext indexingExecutionContext,
      GitRepository gitRepository,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit,
      int numberOfBranches,
      double branchCountFactor,
      out int estimatedDocCount,
      out int estimatedDocCountGrowth,
      out long actualInitialSize)
    {
      int shardDensity = indexingExecutionContext.ProvisioningContext.ContractType.GetShardDensity(indexingExecutionContext.RequestContext);
      int num = this.m_gitHttpClientWrapper.GetGitRepoSizeEstimates(indexingExecutionContext.RequestContext, gitRepository, shardDensity, numberOfBranches, branchCountFactor, out estimatedDocCount, out estimatedDocCountGrowth, out long _, out long _, out actualInitialSize) ? 1 : 0;
      if (!this.m_isReindexingIsProgress)
        return num != 0;
      int repoDocCount;
      if (!this.TryGetRepoDocCountFromOlderIndex(indexingExecutionContext, repoIndexingUnit, out repoDocCount))
        repoDocCount = 0;
      if (repoDocCount <= estimatedDocCount)
        return num != 0;
      float currentHostConfigValue = indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<float>("/Service/ALMSearch/Settings/Routing/Code/CodeEntityGitRepositoryGrowthFactor", true, 0.3f);
      estimatedDocCount = repoDocCount;
      double val1 = (double) repoDocCount * (double) currentHostConfigValue;
      estimatedDocCountGrowth = (int) Math.Min(val1, (double) int.MaxValue);
      return num != 0;
    }

    private List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetOrCreateProjectIndexingUnits(
      IEnumerable<GitRepository> repositoryCollection,
      int collectionIndexingUnitId)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> second = this.CodeIndexingUnitDataAccess.GetIndexingUnits(this.CodeIndexingExecutionContext.RequestContext, "Project", this.EntityType, -1) ?? new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> source = repositoryCollection.Select<GitRepository, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<GitRepository, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (repo => Microsoft.VisualStudio.Services.Search.Common.IndexingUnit.CreateProjectIndexingUnitFrom(repo, collectionIndexingUnitId))).Distinct<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>().Except<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) second);
      if (source.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> collection = (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) this.CodeIndexingUnitDataAccess.AddOrUpdateIndexingUnits(this.CodeIndexingExecutionContext.RequestContext, source.ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>(), true);
        second.AddRange(collection);
      }
      return second;
    }

    private GitCodeRepoTFSAttributes CreateGitCodeRepoTFSAttributesForShadowIndexingUnit(
      IndexingExecutionContext indexingExecutionContext,
      GitRepository repo)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.CodeIndexingUnitDataAccess.GetIndexingUnit(indexingExecutionContext.RequestContext, repo.Id, "Git_Repository", this.EntityType, false);
      if (indexingUnit == null)
      {
        Tracer.TraceInfo(this.m_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Couldn't find primary IU entry for repo {0}. So creating default TFSAttribute values for shadow IU.", (object) repo.Id)));
        GitCodeRepoTFSAttributes shadowIndexingUnit = new GitCodeRepoTFSAttributes();
        shadowIndexingUnit.RepositoryName = repo.Name;
        shadowIndexingUnit.RemoteUrl = repo.RemoteUrl;
        shadowIndexingUnit.DefaultBranch = repo.DefaultBranch;
        return shadowIndexingUnit;
      }
      GitCodeRepoTFSAttributes entityAttributes = (GitCodeRepoTFSAttributes) indexingUnit.TFSEntityAttributes;
      GitCodeRepoTFSAttributes shadowIndexingUnit1 = new GitCodeRepoTFSAttributes();
      shadowIndexingUnit1.RepositoryName = repo.Name;
      shadowIndexingUnit1.RemoteUrl = repo.RemoteUrl;
      shadowIndexingUnit1.DefaultBranch = repo.DefaultBranch;
      shadowIndexingUnit1.Branches = entityAttributes.Branches;
      return shadowIndexingUnit1;
    }

    private GitCodeRepoIndexingProperties CreateGitCodeRepoPropertiesForShadowIndexingUnit(
      IndexingExecutionContext indexingExecutionContext,
      GitRepository repo)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.CodeIndexingUnitDataAccess.GetIndexingUnit(indexingExecutionContext.RequestContext, repo.Id, "Git_Repository", this.EntityType, false);
      if (indexingUnit == null)
      {
        Tracer.TraceInfo(this.m_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Couldn't find primary IU entry for repo {0}.", (object) repo.Id)) + FormattableString.Invariant(FormattableStringFactory.Create("So creating default Properties values for shadow IU.")));
        GitCodeRepoIndexingProperties shadowIndexingUnit = new GitCodeRepoIndexingProperties();
        shadowIndexingUnit.Name = repo.Name;
        shadowIndexingUnit.IsDisabled = this.IsDisabled(repo);
        return shadowIndexingUnit;
      }
      GitCodeRepoIndexingProperties properties = (GitCodeRepoIndexingProperties) indexingUnit.Properties;
      GitCodeRepoIndexingProperties shadowIndexingUnit1 = new GitCodeRepoIndexingProperties();
      shadowIndexingUnit1.Name = repo.Name;
      shadowIndexingUnit1.IsDisabled = this.IsDisabled(repo);
      shadowIndexingUnit1.IndexIndices = properties?.IndexIndices;
      return shadowIndexingUnit1;
    }

    internal bool IsDisabled(GitRepository gitRepository) => gitRepository.IsFork;
  }
}
