// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders.WorkItemEntityRoutingProvider
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
  internal class WorkItemEntityRoutingProvider : IRoutingDataProvider
  {
    private readonly string m_routingId;
    private readonly ProvisionerConfigAndConstantsProvider m_provisionerConfigAndConstantsProvider;
    private readonly CreateSearchIndexHelper m_createSearchIndexHelper;
    private IIndexProvisionerAndManager m_indexProvisionerAndManager;

    [StaticSafe]
    public static IEntityType EntityType => (IEntityType) WorkItemEntityType.GetInstance();

    internal WorkItemEntityRoutingProvider(string routing)
      : this(new CreateSearchIndexHelper(), EntityProvisionerFactory.GetIndexProvisioner(WorkItemEntityRoutingProvider.EntityType), routing)
    {
    }

    internal WorkItemEntityRoutingProvider(
      CreateSearchIndexHelper createSearchIndexHelper,
      ProvisionerConfigAndConstantsProvider provisionerConfigAndConstantsProvider,
      string routing)
    {
      this.m_routingId = routing;
      this.m_createSearchIndexHelper = createSearchIndexHelper;
      this.m_provisionerConfigAndConstantsProvider = provisionerConfigAndConstantsProvider;
    }

    internal static string GetRoutingFromIndexInfo(IndexingExecutionContext indexingExecutionContext) => (indexingExecutionContext.CollectionIndexingUnit == null ? 0 : (indexingExecutionContext.CollectionIndexingUnit.IsLargeCollection(indexingExecutionContext.RequestContext) ? 1 : 0)) != 0 ? string.Empty : indexingExecutionContext.RequestContext.GetCollectionID().ToString();

    public string GetRouting(IndexingExecutionContext indexingExecutionContext, string item) => this.m_routingId;

    public List<ShardAssignmentDetails> AssignIndex(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSizeEstimates)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080200, "Indexing Pipeline", "Indexer", nameof (AssignIndex));
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit = indexingExecutionContext.CollectionIndexingUnit;
      if (collectionIndexingUnit.IsLargeCollection(indexingExecutionContext.RequestContext))
      {
        this.AssignIndexToLargeCollection(indexingExecutionContext, collectionIndexingUnit);
        return (List<ShardAssignmentDetails>) null;
      }
      long maxDocsPerSharedIndex = indexingExecutionContext.GetConfigValue<long>("/Service/ALMSearch/Settings/MaxWorkItemsPerSharedIndexShard") * (long) indexingExecutionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkItemSharedIndexPrimaries", TeamFoundationHostType.Deployment, 10);
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
      IDictionary<string, IList<ShardDetails>> availableIndices = this.FilterOutDedicatedIndices(indicesForAssignment);
      string indexName = this.GetIndexProvisionerAndManager(indexingExecutionContext, maxDocsPerSharedIndex).SelectIndex(indexingExecutionContext, availableIndices);
      if (indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/service/ALMSearch/Settings/Routing/AlwaysCreateNewSharedIndex", true) || string.IsNullOrWhiteSpace(indexName))
      {
        indexName = this.CreateIndex(indexingExecutionContext, IndexProvisionType.Shared);
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
      return (List<ShardAssignmentDetails>) null;
    }

    public List<ShardAssignmentDetails> AssignShards(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSize)
    {
      throw new NotImplementedException();
    }

    internal virtual string CreateIndex(
      IndexingExecutionContext indexingExecutionContext,
      IndexProvisionType indexProvisionType)
    {
      DocumentContractType contractType = indexingExecutionContext.ProvisioningContext.ContractType;
      IndexIdentity indexIdentity = IndexIdentity.CreateIndexIdentity(indexProvisionType != IndexProvisionType.Shared ? this.m_provisionerConfigAndConstantsProvider.GetDedicatedIndexName(contractType, indexingExecutionContext.CollectionIndexingUnit.TFSEntityId) : this.m_provisionerConfigAndConstantsProvider.GetSharedIndexName(contractType));
      this.m_createSearchIndexHelper.CreateIndex((ExecutionContext) indexingExecutionContext, indexIdentity, indexingExecutionContext.ProvisioningContext.SearchPlatform, indexingExecutionContext.ProvisioningContext.SearchClusterManagementService, indexingExecutionContext.DataAccessFactory, indexProvisionType, contractType, this.m_provisionerConfigAndConstantsProvider);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("NewIndexCreated", 1.0);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("Indexing Pipeline", "IndexingOperation", properties);
      return indexIdentity.Name;
    }

    internal virtual IIndexProvisionerAndManager GetIndexProvisionerAndManager(
      IndexingExecutionContext indexingExecutionContext,
      long maxDocsPerSharedIndex)
    {
      if (this.m_indexProvisionerAndManager == null)
        this.m_indexProvisionerAndManager = (IIndexProvisionerAndManager) new DocumentCountBasedIndexProvisionerAndManager(maxDocsPerSharedIndex);
      return this.m_indexProvisionerAndManager;
    }

    private void AssignIndexToLargeCollection(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit)
    {
      string index = this.CreateIndex(indexingExecutionContext, IndexProvisionType.Dedicated);
      collectionIndexingUnit.Properties.IndexIndices = new List<IndexInfo>()
      {
        new IndexInfo()
        {
          IndexName = index,
          Version = new int?(indexingExecutionContext.GetIndexVersion(index)),
          DocumentContractType = indexingExecutionContext.ProvisioningContext.ContractType
        }
      };
      long maxDocsPerSharedIndex = indexingExecutionContext.GetConfigValue<long>("/Service/ALMSearch/Settings/MaxWorkItemsPerSharedIndexShard") * (long) indexingExecutionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkItemSharedIndexPrimaries", TeamFoundationHostType.Deployment, 10);
      this.GetIndexProvisionerAndManager(indexingExecutionContext, maxDocsPerSharedIndex).MarkIndexInactive(indexingExecutionContext, WorkItemEntityRoutingProvider.EntityType, index);
      indexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, collectionIndexingUnit);
    }

    private IDictionary<string, IList<ShardDetails>> FilterOutDedicatedIndices(
      IDictionary<string, IList<ShardDetails>> indices)
    {
      IDictionary<string, IList<ShardDetails>> dictionary = (IDictionary<string, IList<ShardDetails>>) new Dictionary<string, IList<ShardDetails>>();
      foreach (KeyValuePair<string, IList<ShardDetails>> index in (IEnumerable<KeyValuePair<string, IList<ShardDetails>>>) indices)
      {
        if (this.m_provisionerConfigAndConstantsProvider.GetIndexProvisionType(index.Key) == IndexProvisionType.Shared)
          dictionary.Add(index.Key, index.Value);
      }
      return dictionary;
    }
  }
}
