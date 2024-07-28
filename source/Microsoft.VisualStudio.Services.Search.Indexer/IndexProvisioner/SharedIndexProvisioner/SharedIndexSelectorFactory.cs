// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.SharedIndexProvisioner.SharedIndexSelectorFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;

namespace Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.SharedIndexProvisioner
{
  internal static class SharedIndexSelectorFactory
  {
    public static EntitySharedIndexSelector GetSharedIndexProvisioner(
      ExecutionContext executionContext,
      ProvisionerConfigAndConstantsProvider provider,
      ISearchPlatform searchPlatform,
      ISearchClusterManagementService searchClusterManagementService,
      IDataAccessFactory dataAccessFactory,
      CreateSearchIndexHelper createSearchIndexHelper)
    {
      switch (provider.EntityType.Name)
      {
        case "WorkItem":
          return SharedIndexSelectorFactory.GetDocumentCountBasedSharedIndexSelector(executionContext, provider, searchPlatform, searchClusterManagementService, createSearchIndexHelper, executionContext.GetConfigValue<long>("/Service/ALMSearch/Settings/MaxWorkItemsPerSharedIndexShard"), dataAccessFactory);
        case "ProjectRepo":
          return SharedIndexSelectorFactory.GetDocumentCountBasedSharedIndexSelector(executionContext, provider, searchPlatform, searchClusterManagementService, createSearchIndexHelper, executionContext.GetConfigValue<long>("/Service/ALMSearch/Settings/MaxProjectRepoDocsPerSharedIndexShard"), dataAccessFactory);
        case "Wiki":
          return SharedIndexSelectorFactory.GetDocumentCountBasedSharedIndexSelector(executionContext, provider, searchPlatform, searchClusterManagementService, createSearchIndexHelper, executionContext.GetConfigValue<long>("/Service/ALMSearch/Settings/MaxWikiDocsPerSharedIndexShard"), dataAccessFactory);
        case "Board":
          return SharedIndexSelectorFactory.GetDocumentCountBasedSharedIndexSelector(executionContext, provider, searchPlatform, searchClusterManagementService, createSearchIndexHelper, executionContext.GetConfigValue<long>("/Service/ALMSearch/Settings/MaxBoardDocsPerSharedIndexShard"), dataAccessFactory);
        default:
          return new EntitySharedIndexSelector(executionContext, provider, searchPlatform, searchClusterManagementService, dataAccessFactory, createSearchIndexHelper);
      }
    }

    private static EntitySharedIndexSelector GetDocumentCountBasedSharedIndexSelector(
      ExecutionContext executionContext,
      ProvisionerConfigAndConstantsProvider provider,
      ISearchPlatform searchPlatform,
      ISearchClusterManagementService searchClusterManagementService,
      CreateSearchIndexHelper createSearchIndexHelper,
      long maxDocsPerSharedIndexShard,
      IDataAccessFactory dataAccessFactory)
    {
      int configValue = executionContext.GetConfigValue<int>("/Service/ALMSearch/Settings/SharedIndexPrimaries");
      long maxDocsPerIndex = maxDocsPerSharedIndexShard * (long) configValue;
      return (EntitySharedIndexSelector) new DocumentCountBasedSharedIndexSelector(executionContext, provider, searchPlatform, searchClusterManagementService, maxDocsPerIndex, dataAccessFactory, createSearchIndexHelper);
    }
  }
}
