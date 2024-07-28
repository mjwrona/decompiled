// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.CollectionDedicatedIndexProvisioner
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  internal sealed class CollectionDedicatedIndexProvisioner : DedicatedIndexProvisioner
  {
    private readonly CreateSearchIndexHelper m_createSearchIndexHelper;
    private IDataAccessFactory m_dataAccessFactory;

    internal CollectionDedicatedIndexProvisioner(
      IndexingExecutionContext indexingExecutionContext,
      IEntityType entityType)
      : this(indexingExecutionContext, entityType, new CreateSearchIndexHelper())
    {
    }

    public CollectionDedicatedIndexProvisioner(
      IndexingExecutionContext indexingExecutionContext,
      IEntityType entityType,
      CreateSearchIndexHelper createSearchIndexHelper)
      : base(indexingExecutionContext, IndexProvisionType.Dedicated, entityType)
    {
      this.m_createSearchIndexHelper = createSearchIndexHelper;
      this.m_dataAccessFactory = DataAccessFactory.GetInstance();
    }

    protected override IndexIdentity OnboardIndexingUnit(
      IndexingExecutionContext executionContext,
      ISearchPlatform searchPlatform,
      ProvisionerConfigAndConstantsProvider entityProvisionProvider)
    {
      DocumentContractType contractType = executionContext.ProvisioningContext.ContractType;
      IndexIdentity indexIdentity = IndexIdentity.CreateIndexIdentity(entityProvisionProvider.GetDedicatedIndexName(contractType, executionContext.IndexingUnit.TFSEntityId));
      this.m_createSearchIndexHelper.CreateIndex((ExecutionContext) executionContext, indexIdentity, searchPlatform, this.SearchClusterManagementService, this.m_dataAccessFactory, IndexProvisionType.Dedicated, contractType, entityProvisionProvider);
      return indexIdentity;
    }

    protected override IndexIdentity MigrateIndexingUnit(
      IndexingExecutionContext executionContext,
      ISearchPlatform searchPlatform,
      ProvisionerConfigAndConstantsProvider entityProvisionerProvider,
      IndexIdentity indexToSkip)
    {
      IndexIdentity migrateIndexIdentity = this.GetMigrateIndexIdentity(executionContext, searchPlatform, entityProvisionerProvider, executionContext.IndexingUnit.TFSEntityId);
      this.m_createSearchIndexHelper.CreateIndex((ExecutionContext) executionContext, migrateIndexIdentity, searchPlatform, this.SearchClusterManagementService, this.m_dataAccessFactory, IndexProvisionType.Dedicated, executionContext.ProvisioningContext.ContractType, entityProvisionerProvider);
      return migrateIndexIdentity;
    }
  }
}
