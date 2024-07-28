// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders.BoardEntityRoutingProvider
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
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders
{
  internal class BoardEntityRoutingProvider : IRoutingDataProvider
  {
    private readonly string m_routingId;
    private IIndexProvisionerAndManager m_indexProvisionerAndManager;
    private readonly ProvisionerConfigAndConstantsProvider m_provisionerConfigAndConstantsProvider;
    private readonly CreateSearchIndexHelper m_createSearchIndexHelper;

    [StaticSafe]
    public static IEntityType EntityType => (IEntityType) BoardEntityType.GetInstance();

    internal BoardEntityRoutingProvider(string routing)
      : this(new CreateSearchIndexHelper(), EntityProvisionerFactory.GetIndexProvisioner(BoardEntityRoutingProvider.EntityType), routing)
    {
    }

    internal BoardEntityRoutingProvider(
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
      long maxDocsPerSharedIndex = 500000L * (long) indexingExecutionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BoardSharedIndexPrimaries", TeamFoundationHostType.Deployment, 1);
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
      if (string.IsNullOrWhiteSpace(indexName))
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
          DocumentContractType = indexingExecutionContext.ProvisioningContext.ContractType
        }
      };
      indexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, collectionIndexingUnit);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080204, "Indexing Pipeline", "Indexer", nameof (AssignIndex));
      return indexingUnitsWithSizeEstimates.IsNullOrEmpty<IndexingUnitWithSize>() ? new List<ShardAssignmentDetails>() : this.AssignShards(indexingExecutionContext, indexingUnitsWithSizeEstimates);
    }

    public virtual List<ShardAssignmentDetails> AssignShards(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSize)
    {
      if (indexingUnitsWithSize.IsNullOrEmpty<IndexingUnitWithSize>())
        throw new ArgumentException("Null or empty list of indexing units is not allowed.", nameof (indexingUnitsWithSize));
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit = indexingExecutionContext.CollectionIndexingUnit;
      IVssRequestContext vssRequestContext = indexingExecutionContext.RequestContext.To(TeamFoundationHostType.Deployment);
      IRoutingLookupService service = vssRequestContext.GetService<IRoutingLookupService>();
      string connectionString = indexingExecutionContext.ProvisioningContext.SearchPlatformConnectionString;
      string indexName = collectionIndexingUnit.GetIndexInfo()?.IndexName;
      if (string.IsNullOrWhiteSpace(indexName) || string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Neither CollectionIndexName nor connectionString can be null or whitespace, " + FormattableString.Invariant(FormattableStringFactory.Create("CollectionIndexName :{0}, ConnectionString : {1}", (object) indexName, (object) connectionString)));
      string normalizedString = collectionIndexingUnit.GetTfsEntityIdAsNormalizedString();
      int shardId = service.GetShardId(vssRequestContext, connectionString, indexName, normalizedString);
      string clusterName = indexingExecutionContext.ProvisioningContext.SearchClusterManagementService.GetClusterName();
      return new List<ShardAssignmentDetails>()
      {
        this.UpdateSQLTables(indexingExecutionContext, indexingUnitsWithSize, clusterName, indexName, normalizedString, shardId)
      };
    }

    internal virtual ShardAssignmentDetails UpdateSQLTables(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSize,
      string clusterName,
      string indexName,
      string routingId,
      int shardId)
    {
      List<IndexingUnitDetails> indexingUnitIndexingInformationList = new List<IndexingUnitDetails>();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> source = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      int shardDensity1 = indexingExecutionContext.ProvisioningContext.ContractType.GetShardDensity(indexingExecutionContext.RequestContext);
      foreach (IndexingUnitWithSize indexingUnitWithSize in indexingUnitsWithSize)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitWithSize.IndexingUnit;
        int indexingUnitId = indexingUnit.IndexingUnitId;
        Guid tfsEntityId = indexingUnit.TFSEntityId;
        BoardEntityType instance = BoardEntityType.GetInstance();
        string indexingUnitType = indexingUnit.IndexingUnitType;
        List<int> shardIds = new List<int>();
        shardIds.Add(shardId);
        List<string> routingIds = new List<string>();
        routingIds.Add(routingId);
        double shardDensity2 = (double) shardDensity1;
        string esClusterName = clusterName;
        string indexName1 = indexName;
        long actualInitialSize = indexingUnitWithSize.ActualInitialSize;
        int actualInitialDocCount = indexingUnitWithSize.ActualInitialDocCount;
        IndexingUnitDetails indexingUnitDetails = new IndexingUnitDetails(indexingUnitId, tfsEntityId, (IEntityType) instance, indexingUnitType, false, shardIds, routingIds, 0, 0, (float) shardDensity2, esClusterName, indexName1, actualInitialSize, actualInitialDocCount);
        indexingUnitIndexingInformationList.Add(indexingUnitDetails);
        IndexInfo indexInfo = new IndexInfo()
        {
          IndexName = indexName,
          Version = new int?(indexingExecutionContext.GetIndexVersion(indexName)),
          Routing = routingId,
          DocumentContractType = indexingExecutionContext.ProvisioningContext.ContractType
        };
        indexingUnit.Properties.IndexIndices = new List<IndexInfo>()
        {
          indexInfo
        };
        source.Add(indexingUnit);
      }
      indexingExecutionContext.IndexingUnitDataAccess.AddOrResetIndexingUnitDetailsAndUpdateShardDetails(indexingExecutionContext.RequestContext, (IList<IndexingUnitDetails>) indexingUnitIndexingInformationList, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) source);
      return new ShardAssignmentDetails(shardId, 0, 0, 0, -1, source.ToHashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>());
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

    internal static string GetRoutingFromIndexInfo(IndexingExecutionContext indexingExecutionContext) => indexingExecutionContext.RequestContext.GetCollectionID().ToString().ToLowerInvariant();
  }
}
