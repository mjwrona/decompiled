// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.SharedIndexProvisioner.DocumentCountBasedIndexProvisionerAndManager
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.SharedIndexProvisioner
{
  internal class DocumentCountBasedIndexProvisionerAndManager : IIndexProvisionerAndManager
  {
    private readonly long m_indexDocCountThreshold;

    public DocumentCountBasedIndexProvisionerAndManager(long maxDocsPerSharedIndex) => this.m_indexDocCountThreshold = maxDocsPerSharedIndex;

    public IDictionary<string, IList<ShardDetails>> GetAvailableIndicesForAssignment(
      IndexingExecutionContext indexingExecutionContext)
    {
      int indexVersion1 = indexingExecutionContext.ProvisioningContext.EntityProvisionProvider.IndexVersion;
      DocumentContractType contractType = indexingExecutionContext.ProvisioningContext.ContractType;
      IEntityType entityType = indexingExecutionContext.ProvisioningContext.EntityProvisionProvider.EntityType;
      string clusterName = indexingExecutionContext.ProvisioningContext.SearchClusterManagementService.GetClusterName();
      IDictionary<string, IList<ShardDetails>> indicesForAssignment = (IDictionary<string, IList<ShardDetails>>) new FriendlyDictionary<string, IList<ShardDetails>>();
      IDictionary<string, IList<ShardDetails>> dictionary = (IDictionary<string, IList<ShardDetails>>) new FriendlyDictionary<string, IList<ShardDetails>>();
      IDictionary<string, IList<ShardDetails>> activeShards = indexingExecutionContext.DataAccessFactory.GetShardDetailsDataAccess().GetActiveShards(indexingExecutionContext.RequestContext, clusterName, entityType);
      if (!activeShards.IsNullOrEmpty<KeyValuePair<string, IList<ShardDetails>>>())
      {
        ISearchPlatform searchPlatform = indexingExecutionContext.ProvisioningContext.SearchPlatform;
        foreach (KeyValuePair<string, IList<ShardDetails>> keyValuePair in (IEnumerable<KeyValuePair<string, IList<ShardDetails>>>) activeShards)
        {
          IndexIdentity indexIdentity = IndexIdentity.CreateIndexIdentity(keyValuePair.Key);
          if (!searchPlatform.IndexExists((ExecutionContext) indexingExecutionContext, indexIdentity))
          {
            dictionary.Add(keyValuePair);
          }
          else
          {
            long indexedDocumentCount = searchPlatform.GetIndex(indexIdentity).GetIndexedDocumentCount();
            int indexVersion2 = this.GetIndexVersion(indexingExecutionContext, indexIdentity.Name);
            IEnumerable<DocumentContractType> documentContracts = searchPlatform.GetIndex(indexIdentity).GetDocumentContracts((ExecutionContext) indexingExecutionContext);
            if (indexedDocumentCount > this.m_indexDocCountThreshold || indexVersion2 != indexVersion1 || !documentContracts.Contains<DocumentContractType>(contractType))
              dictionary.Add(keyValuePair);
            else
              indicesForAssignment.Add(keyValuePair);
          }
        }
      }
      try
      {
        foreach (KeyValuePair<string, IList<ShardDetails>> keyValuePair in (IEnumerable<KeyValuePair<string, IList<ShardDetails>>>) dictionary)
          indexingExecutionContext.DataAccessFactory.GetShardDetailsDataAccess().MarkShardsInactive(indexingExecutionContext.RequestContext, clusterName, entityType, keyValuePair.Key);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("IndexMarkedInactive", (double) dictionary.Count);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("Indexing Pipeline", "IndexingOperation", properties);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083070, "Indexing Pipeline", "Indexer", ex);
      }
      return indicesForAssignment;
    }

    public string SelectIndex(
      IndexingExecutionContext indexingExecutionContext,
      IDictionary<string, IList<ShardDetails>> availableIndices)
    {
      return availableIndices.IsNullOrEmpty<KeyValuePair<string, IList<ShardDetails>>>() ? (string) null : availableIndices.First<KeyValuePair<string, IList<ShardDetails>>>().Key;
    }

    public bool MarkIndexInactive(
      IndexingExecutionContext ieContext,
      IEntityType entityType,
      string indexName)
    {
      try
      {
        string clusterName = ieContext.ProvisioningContext.SearchClusterManagementService.GetClusterName();
        ieContext.DataAccessFactory.GetShardDetailsDataAccess().MarkShardsInactive(ieContext.RequestContext, clusterName, entityType, indexName);
        return true;
      }
      catch
      {
        return false;
      }
    }

    internal virtual int GetIndexVersion(
      IndexingExecutionContext indexingExecutionContext,
      string indexName)
    {
      return indexingExecutionContext.GetIndexVersion(indexName);
    }
  }
}
