// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders.PackageEntityRoutingProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.SharedIndexProvisioner;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders
{
  internal class PackageEntityRoutingProvider : IRoutingDataProvider
  {
    private readonly string m_routingId;
    private IIndexProvisionerAndManager m_indexProvisionerAndManager;
    private readonly ProvisionerConfigAndConstantsProvider m_provisionerConfigAndConstantsProvider;
    private readonly CreateSearchIndexHelper m_createSearchIndexHelper;

    [StaticSafe]
    public static IEntityType EntityType => (IEntityType) PackageEntityType.GetInstance();

    internal PackageEntityRoutingProvider(string routing)
      : this(new CreateSearchIndexHelper(), EntityProvisionerFactory.GetIndexProvisioner(PackageEntityRoutingProvider.EntityType), routing)
    {
    }

    internal PackageEntityRoutingProvider(
      CreateSearchIndexHelper createSearchIndexHelper,
      ProvisionerConfigAndConstantsProvider provisionerConfigAndConstantsProvider,
      string routing)
    {
      this.m_routingId = routing;
      this.m_createSearchIndexHelper = createSearchIndexHelper;
      this.m_provisionerConfigAndConstantsProvider = provisionerConfigAndConstantsProvider;
    }

    public string GetRouting(IndexingExecutionContext indexingExecutionContext, string item) => this.m_routingId;

    public List<ShardAssignmentDetails> AssignIndex(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSizeEstimates)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080200, "Indexing Pipeline", "Indexer", nameof (AssignIndex));
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit = indexingExecutionContext.CollectionIndexingUnit;
      long maxDocsPerSharedIndex = 2000000L * (long) indexingExecutionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/PackageSharedIndexPrimaries", TeamFoundationHostType.Deployment, 1);
      IDictionary<string, IList<ShardDetails>> indicesForAssignment = this.GetIndexProvisionerAndManager(indexingExecutionContext, maxDocsPerSharedIndex).GetAvailableIndicesForAssignment(indexingExecutionContext);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("NumberOfActiveIndices", (double) indicesForAssignment.Count);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("Indexing Pipeline", "IndexingOperation", properties);
      IList<IndexInfo> indexIndices = (IList<IndexInfo>) collectionIndexingUnit.Properties.IndexIndices;
      if (!indexIndices.IsNullOrEmpty<IndexInfo>())
      {
        foreach (IndexInfo indexInfo in (IEnumerable<IndexInfo>) indexIndices)
        {
          if (!string.IsNullOrWhiteSpace(indexInfo.IndexName))
            indicesForAssignment.Remove(indexInfo.IndexName);
        }
      }
      string indexName = this.GetIndexProvisionerAndManager(indexingExecutionContext, maxDocsPerSharedIndex).SelectIndex(indexingExecutionContext, indicesForAssignment);
      if (indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/service/ALMSearch/Settings/Routing/AlwaysCreateNewSharedIndex", true) || string.IsNullOrWhiteSpace(indexName))
      {
        indexName = this.CreateIndex(indexingExecutionContext);
        indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully assigned a new index:{0} to host for indexing.", (object) indexName)));
      }
      else
        indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully assigned {0} to host for indexing.", (object) indexName)));
      collectionIndexingUnit.Properties.IndexIndices = new List<IndexInfo>()
      {
        new IndexInfo()
        {
          IndexName = indexName,
          Version = new int?(indexingExecutionContext.GetIndexVersion(indexName)),
          DocumentContractType = indexingExecutionContext.ProvisioningContext.ContractType,
          Routing = this.GetRoutingDetails(indexingExecutionContext)
        }
      };
      indexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, collectionIndexingUnit);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080204, "Indexing Pipeline", "Indexer", nameof (AssignIndex));
      return new List<ShardAssignmentDetails>();
    }

    public List<ShardAssignmentDetails> AssignShards(
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

    private string GetRoutingDetails(IndexingExecutionContext ieContext) => ieContext.RequestContext.GetCollectionID().ToString();
  }
}
