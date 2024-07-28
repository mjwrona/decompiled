// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CollectionCustomCodeMetadataCrawler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class CollectionCustomCodeMetadataCrawler : AbstractCollectionCodeMetadataCrawler
  {
    private bool m_isReindexingIsProgress;
    private ICustomRepositoryDataAccess m_customRepositoryDataAccess;

    internal CollectionCustomCodeMetadataCrawler(
      IndexingExecutionContext executionContext,
      IDataAccessFactory dataAccessFactory,
      CodeFileContract codeFileContract)
      : base(executionContext, dataAccessFactory, "CustomRepository", codeFileContract)
    {
      this.m_customRepositoryDataAccess = dataAccessFactory.GetCustomRepositoryDataAccess();
      this.m_isReindexingIsProgress = executionContext.IsReindexingFailedOrInProgress(executionContext.DataAccessFactory, this.EntityType);
    }

    public override List<IndexingUnitWithSize> CrawlMetadata(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnit collectionIndexingUnit,
      bool isShadowCrawlingRequired = false)
    {
      float currentHostConfigValue = indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<float>("/Service/ALMSearch/Settings/Routing/Code/CodeEntityCustomRepositoryGrowthFactor", true, 0.1f);
      IEnumerable<Guid> existingCustomRepoIds = this.GetExistingCustomRepoIds(collectionIndexingUnit.TFSEntityId);
      List<IndexingUnitWithSize> indexingUnitWithSizeList = new List<IndexingUnitWithSize>();
      if (existingCustomRepoIds != null)
      {
        IDictionary<Guid, IndexingUnit> repoIndexingUnits = this.GetExistingRepoIndexingUnits(isShadowCrawlingRequired);
        foreach (Guid key in existingCustomRepoIds)
        {
          IndexingUnit indexingUnit;
          if (repoIndexingUnits != null && repoIndexingUnits.TryGetValue(key, out indexingUnit))
          {
            int currentEstimatedDocCount;
            int estimatedDocCountGrowth;
            this.GetSizeEstimates(indexingExecutionContext, indexingUnit, currentHostConfigValue, out currentEstimatedDocCount, out estimatedDocCountGrowth);
            IndexingUnitWithSize indexingUnitWithSize = new IndexingUnitWithSize(indexingUnit, currentEstimatedDocCount, estimatedDocCountGrowth, true)
            {
              ActualInitialDocCount = currentEstimatedDocCount
            };
            indexingUnitWithSizeList.Add(indexingUnitWithSize);
          }
        }
      }
      return indexingUnitWithSizeList;
    }

    internal void GetSizeEstimates(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnit customRepoIndexingUnit,
      float growthFactor,
      out int currentEstimatedDocCount,
      out int estimatedDocCountGrowth)
    {
      CustomRepoCodeIndexingProperties properties = customRepoIndexingUnit.Properties as CustomRepoCodeIndexingProperties;
      currentEstimatedDocCount = properties.RepositorySize;
      if (this.m_isReindexingIsProgress)
      {
        int repoDocCount;
        if (!this.TryGetRepoDocCountFromOlderIndex(indexingExecutionContext, customRepoIndexingUnit, out repoDocCount))
          repoDocCount = 0;
        currentEstimatedDocCount = Math.Max(currentEstimatedDocCount, repoDocCount);
      }
      double num = Math.Min((double) currentEstimatedDocCount * (double) growthFactor, (double) int.MaxValue);
      estimatedDocCountGrowth = Convert.ToInt32(num);
    }

    private IEnumerable<Guid> GetExistingCustomRepoIds(Guid collectionId)
    {
      List<Guid> existingCustomRepoIds = new List<Guid>();
      IEnumerable<string> projects = this.m_customRepositoryDataAccess.GetProjects(this.CodeIndexingExecutionContext.RequestContext, collectionId);
      if (projects != null && projects.Any<string>())
      {
        foreach (string projectName in projects)
        {
          IEnumerable<string> repositories = this.m_customRepositoryDataAccess.GetRepositories(this.CodeIndexingExecutionContext.RequestContext, collectionId, projectName);
          if (repositories != null && repositories.Any<string>())
          {
            foreach (string repositoryName in repositories)
            {
              Guid repositoryId = this.m_customRepositoryDataAccess.GetRepositoryId(this.CodeIndexingExecutionContext.RequestContext, collectionId, projectName, repositoryName);
              if (repositoryId != Guid.Empty)
                existingCustomRepoIds.Add(repositoryId);
            }
          }
        }
      }
      return (IEnumerable<Guid>) existingCustomRepoIds;
    }
  }
}
