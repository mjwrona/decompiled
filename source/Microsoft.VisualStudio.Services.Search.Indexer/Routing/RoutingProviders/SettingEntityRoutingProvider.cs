// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders.SettingEntityRoutingProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.SharedIndexProvisioner;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders
{
  internal class SettingEntityRoutingProvider : IRoutingDataProvider
  {
    private readonly string m_routingId;
    private IIndexProvisionerAndManager m_indexProvisionerAndManager;
    private ISharedIndexingPropertyDataAccess m_sharedIndexingPropertyDataAccess;
    private readonly ProvisionerConfigAndConstantsProvider m_provisionerConfigAndConstantsProvider;
    private readonly CreateSearchIndexHelper m_createSearchIndexHelper;

    [StaticSafe]
    public static IEntityType EntityType => (IEntityType) SettingEntityType.GetInstance();

    internal SettingEntityRoutingProvider(string routing)
      : this(new CreateSearchIndexHelper(), EntityProvisionerFactory.GetIndexProvisioner(SettingEntityRoutingProvider.EntityType), routing)
    {
    }

    internal SettingEntityRoutingProvider(
      CreateSearchIndexHelper createSearchIndexHelper,
      ProvisionerConfigAndConstantsProvider provisionerConfigAndConstantsProvider,
      string routing)
    {
      this.m_routingId = routing;
      this.m_createSearchIndexHelper = createSearchIndexHelper;
      this.m_provisionerConfigAndConstantsProvider = provisionerConfigAndConstantsProvider;
      this.m_sharedIndexingPropertyDataAccess = DataAccessFactory.GetInstance().GetSharedIndexingPropertyDataAccess();
    }

    public string GetRouting(IndexingExecutionContext indexingExecutionContext, string item) => this.m_routingId;

    public List<ShardAssignmentDetails> AssignIndex(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSizeEstimates)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080200, "Indexing Pipeline", "Indexer", nameof (AssignIndex));
      if (this.ShouldCreateNewIndex(indexingExecutionContext))
      {
        string index = this.CreateIndex(indexingExecutionContext);
        indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully assigned a new index:{0} to host for indexing.", (object) index)));
        indexingExecutionContext.IndexingUnit.Properties.IndexIndices = new List<IndexInfo>()
        {
          new IndexInfo()
          {
            IndexName = index,
            Version = new int?(indexingExecutionContext.GetIndexVersion(index)),
            DocumentContractType = indexingExecutionContext.ProvisioningContext.ContractType
          }
        };
        this.m_sharedIndexingPropertyDataAccess.UpdateIndexingProperties(indexingExecutionContext.RequestContext, (DeploymentIndexingProperties) indexingExecutionContext.IndexingUnit.Properties);
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080204, "Indexing Pipeline", "Indexer", nameof (AssignIndex));
      return new List<ShardAssignmentDetails>()
      {
        new ShardAssignmentDetails(0, 0, 0, 0, -1, new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>()
        {
          indexingExecutionContext.IndexingUnit
        }.ToHashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      };
    }

    public virtual List<ShardAssignmentDetails> AssignShards(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSize)
    {
      throw new NotImplementedException();
    }

    internal virtual IIndexProvisionerAndManager GetIndexProvisionerAndManager(
      IndexingExecutionContext indexingExecutionContext,
      long maxDocsPerSharedIndex)
    {
      if (this.m_indexProvisionerAndManager == null)
        this.m_indexProvisionerAndManager = (IIndexProvisionerAndManager) new DocumentCountBasedIndexProvisionerAndManager(maxDocsPerSharedIndex);
      return this.m_indexProvisionerAndManager;
    }

    internal virtual string CreateIndex(IndexingExecutionContext indexingExecutionContext)
    {
      DocumentContractType contractType = indexingExecutionContext.ProvisioningContext.ContractType;
      IndexIdentity indexIdentity = IndexIdentity.CreateIndexIdentity(this.m_provisionerConfigAndConstantsProvider.GetSharedIndexName(contractType));
      this.m_createSearchIndexHelper.CreateIndex((ExecutionContext) indexingExecutionContext, indexIdentity, indexingExecutionContext.ProvisioningContext.SearchPlatform, indexingExecutionContext.ProvisioningContext.SearchClusterManagementService, indexingExecutionContext.DataAccessFactory, IndexProvisionType.Shared, contractType, this.m_provisionerConfigAndConstantsProvider);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("NewIndexCreated", 1.0);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("Indexing Pipeline", "IndexingOperation", properties);
      return indexIdentity.Name;
    }

    protected internal virtual bool ShouldCreateNewIndex(
      IndexingExecutionContext indexingExecutionContext)
    {
      int? version = (int?) indexingExecutionContext.IndexingUnit.GetIndexInfo()?.Version;
      if (version.HasValue)
      {
        int? nullable = version;
        int indexVersion = this.m_provisionerConfigAndConstantsProvider.IndexVersion;
        if (nullable.GetValueOrDefault() == indexVersion & nullable.HasValue)
        {
          string queryIndexName = indexingExecutionContext.IndexingUnit.GetQueryIndexName();
          if (!string.IsNullOrWhiteSpace(queryIndexName))
          {
            IndexIdentity indexIdentity = IndexIdentity.CreateIndexIdentity(queryIndexName);
            if (indexingExecutionContext.ProvisioningContext.SearchPlatform.IndexExists((ExecutionContext) indexingExecutionContext, indexIdentity))
            {
              indexingExecutionContext.Log.Append("Index exists and version matches. No need to create new index. Assigning existing index back to caller. ");
              return false;
            }
            indexingExecutionContext.Log.Append("Version matches but index doesn't exist. Proceeding with index creation... ");
          }
          else
          {
            indexingExecutionContext.Log.Append("Query index doesn't exist. Proceeding with checking the indexing index. ");
            if (!string.IsNullOrWhiteSpace(indexingExecutionContext.IndexingUnit.GetIndexingIndexName()))
            {
              indexingExecutionContext.Log.Append("Indexing index exists. No need to create new index. Assigning existing index back to caller. ");
              return false;
            }
          }
        }
        else
          indexingExecutionContext.Log.Append("Version doesn't match and needs upgrade. Proceeding with index creation...  ");
      }
      return true;
    }
  }
}
