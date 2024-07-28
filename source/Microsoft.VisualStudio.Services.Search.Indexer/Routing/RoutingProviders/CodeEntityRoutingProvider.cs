// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders.CodeEntityRoutingProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders
{
  internal class CodeEntityRoutingProvider : IRoutingDataProvider
  {
    private static readonly object s_lock = new object();
    private readonly string[] m_sortedRoutingIds;
    private readonly CreateSearchIndexHelper m_searchIndexHelper;
    private readonly ProvisionerConfigAndConstantsProvider m_provisionerConfigAndConstantsProvider;

    [StaticSafe]
    public static IEntityType EntityType => (IEntityType) CodeEntityType.GetInstance();

    internal CodeEntityRoutingProvider(
      CreateSearchIndexHelper searchIndexHelper,
      ProvisionerConfigAndConstantsProvider configAndConstantsProvider,
      string routing)
    {
      this.m_searchIndexHelper = searchIndexHelper;
      this.m_provisionerConfigAndConstantsProvider = configAndConstantsProvider;
      string[] strArray;
      if (routing == null)
        strArray = (string[]) null;
      else
        strArray = ((IEnumerable<string>) routing.Split(',')).OrderBy<string, string>((Func<string, string>) (r => r)).ToArray<string>();
      if (strArray == null)
        strArray = Array.Empty<string>();
      this.m_sortedRoutingIds = strArray;
    }

    internal CodeEntityRoutingProvider(string routing)
      : this(new CreateSearchIndexHelper(), EntityProvisionerFactory.GetIndexProvisioner(CodeEntityRoutingProvider.EntityType), routing)
    {
    }

    internal static string GetRoutingFromIndexInfo(
      IndexingExecutionContext indexingExecutionContext,
      IndexInfo indexInfo)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingExecutionContext.IndexingUnit;
      if (indexingExecutionContext.IndexingUnit.IndexingUnitType == "TemporaryIndexingUnit")
        indexInfo = indexingExecutionContext.IndexingUnitDataAccess.GetIndexingUnit(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit.ParentUnitId).GetIndexInfo();
      return indexInfo.Routing;
    }

    public string GetRouting(IndexingExecutionContext indexingExecutionContext, string item)
    {
      if (((IEnumerable<string>) this.m_sortedRoutingIds).IsNullOrEmpty<string>())
        throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("No routing information is present for the code indexing unit: {0}", (object) indexingExecutionContext.IndexingUnit.IndexingUnitId)));
      string str = this.m_sortedRoutingIds.Length != 1 ? this.m_sortedRoutingIds[(int) (this.ComputeHash(!indexingExecutionContext.ProvisioningContext.ContractType.IsNoPayloadContract() ? item : CodeDocumentId.GetFilePathToComputeHash(new FileAttributes(item, indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitType, indexingExecutionContext.ProvisioningContext.ContractType))) % (long) this.m_sortedRoutingIds.Length)] : this.m_sortedRoutingIds[0];
      return !string.IsNullOrWhiteSpace(str) ? str : throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Improper routing information is present for the code indexing unit: {0}. RoutingIds: {1}", (object) indexingExecutionContext.IndexingUnit.IndexingUnitId, (object) this.m_sortedRoutingIds)));
    }

    public List<ShardAssignmentDetails> AssignIndex(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSizeEstimates)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      int currentHostConfigValue = indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/Routing/Code/MaxAllowedShardsToACollection", true, 50);
      IVssRequestContext requestContext = indexingExecutionContext.RequestContext.To(TeamFoundationHostType.Deployment);
      int maxNumberOfDocumentsInAShard = indexingExecutionContext.ProvisioningContext.ContractType.GetMaxNumberOfDocumentsInAShard(indexingExecutionContext.RequestContext);
      float configValueOrDefault = (float) requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/Routing/Code/ThresholdToConsiderAShardFull", 0.89999997615814209);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit = indexingExecutionContext.CollectionIndexingUnit;
      string clusterName = indexingExecutionContext.ProvisioningContext.SearchClusterManagementService.GetClusterName();
      FriendlyDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> friendlyDictionary = new FriendlyDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      List<IndexingUnitWithSize> currentIndexingUnitsWithSize = new List<IndexingUnitWithSize>();
      foreach (IndexingUnitWithSize withSizeEstimate in indexingUnitsWithSizeEstimates)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 = withSizeEstimate.IndexingUnit;
        if (indexingUnit1.IsRepository())
        {
          if (indexingUnit1.IsLargeRepository(indexingExecutionContext.RequestContext))
          {
            friendlyDictionary[indexingUnit1.IndexingUnitId] = indexingUnit1;
            if (indexingUnit1.IndexingUnitType == "Git_Repository")
              this.CreateIndex(indexingExecutionContext, indexingUnit1, IndexProvisionType.Dedicated);
          }
          else
            currentIndexingUnitsWithSize.Add(withSizeEstimate);
        }
        else if (indexingUnit1.IndexingUnitType == "ScopedIndexingUnit")
        {
          int parentUnitId = indexingUnit1.ParentUnitId;
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2;
          if (!friendlyDictionary.TryGetValue(parentUnitId, out indexingUnit2))
          {
            indexingUnit2 = indexingExecutionContext.IndexingUnitDataAccess.GetIndexingUnit(indexingExecutionContext.RequestContext, parentUnitId);
            friendlyDictionary[parentUnitId] = indexingUnit2;
          }
          if (indexingUnit2.IsLargeRepository(indexingExecutionContext.RequestContext) && indexingUnit2.IndexingUnitType == "CustomRepository")
            currentIndexingUnitsWithSize.Add(withSizeEstimate);
        }
      }
      IList<IndexingUnitWithSize> unitsAfterSplitting = (IList<IndexingUnitWithSize>) this.GetAllIndexingUnitsAfterSplitting(indexingExecutionContext, currentIndexingUnitsWithSize, maxNumberOfDocumentsInAShard);
      HashSet<string> stringSet = new HashSet<string>();
      CollectionIndexingProperties properties1 = (CollectionIndexingProperties) indexingExecutionContext.CollectionIndexingUnit.Properties;
      if (!properties1.IndexIndices.IsNullOrEmpty<IndexInfo>())
        stringSet.AddRange<string, HashSet<string>>(properties1.IndexIndices.Select<IndexInfo, string>((Func<IndexInfo, string>) (x => x.IndexName)));
      if (!properties1.QueryIndices.IsNullOrEmpty<IndexInfo>())
        stringSet.AddRange<string, HashSet<string>>(properties1.QueryIndices.Select<IndexInfo, string>((Func<IndexInfo, string>) (x => x.IndexName)));
      if (!properties1.IndexIndicesPreReindexing.IsNullOrEmpty<IndexInfo>())
        stringSet.AddRange<string, HashSet<string>>(properties1.IndexIndicesPreReindexing.Select<IndexInfo, string>((Func<IndexInfo, string>) (x => x.IndexName)));
      if (!properties1.QueryIndicesPreReindexing.IsNullOrEmpty<IndexInfo>())
        stringSet.AddRange<string, HashSet<string>>(properties1.QueryIndicesPreReindexing.Select<IndexInfo, string>((Func<IndexInfo, string>) (x => x.IndexName)));
      lock (CodeEntityRoutingProvider.s_lock)
      {
        IDictionary<string, IList<ShardDetails>> activeShards = indexingExecutionContext.ShardDetailsDataAccess.GetActiveShards(indexingExecutionContext.RequestContext, clusterName, CodeEntityRoutingProvider.EntityType);
        List<string> stringList = new List<string>();
        foreach (string key in (IEnumerable<string>) activeShards.Keys)
        {
          if (this.m_provisionerConfigAndConstantsProvider.GetIndexProvisionType(key) != IndexProvisionType.Shared)
            stringList.Add(key);
        }
        foreach (string key in stringList)
          activeShards.Remove(key);
        string str = (string) null;
        foreach (KeyValuePair<string, IList<ShardDetails>> keyValuePair in (IEnumerable<KeyValuePair<string, IList<ShardDetails>>>) activeShards)
        {
          string key = keyValuePair.Key;
          if (!stringSet.Contains(key))
          {
            IndexIdentity indexIdentity = IndexIdentity.CreateIndexIdentity(key);
            if (indexingExecutionContext.ProvisioningContext.SearchPlatform.IndexExists((ExecutionContext) indexingExecutionContext, indexIdentity) && indexingExecutionContext.ProvisioningContext.SearchPlatform.GetIndex(indexIdentity).GetDocumentContracts((ExecutionContext) indexingExecutionContext).Contains<DocumentContractType>(properties1.IndexContractType))
            {
              IList<ShardDetails> shardDetailsList = keyValuePair.Value;
              List<ShardAssignmentDetails> list = shardDetailsList.Select<ShardDetails, ShardAssignmentDetails>((Func<ShardDetails, ShardAssignmentDetails>) (x => new ShardAssignmentDetails((int) x.ShardId, x.EstimatedDocCount, x.EstimatedDocCountGrowth, x.ReservedDocCount, maxNumberOfDocumentsInAShard, (HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) null))).ToList<ShardAssignmentDetails>();
              if (this.IsShardAllocationAppropriate((IList<ShardAssignmentDetails>) this.GetShardAllocator(currentHostConfigValue, shardDetailsList).ProvisionShards(indexingExecutionContext, (IList<ShardAssignmentDetails>) list, unitsAfterSplitting).ToList<ShardAssignmentDetails>()))
              {
                str = key;
                break;
              }
            }
          }
        }
        if (indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/service/ALMSearch/Settings/Routing/AlwaysCreateNewSharedIndex", true) || string.IsNullOrWhiteSpace(str))
          str = this.CreateIndex(indexingExecutionContext, collectionIndexingUnit, IndexProvisionType.Shared);
        collectionIndexingUnit.Properties.IndexIndices = new List<IndexInfo>()
        {
          new IndexInfo()
          {
            IndexName = str,
            Version = new int?(indexingExecutionContext.GetIndexVersion(str)),
            EntityName = indexingExecutionContext.CollectionName,
            DocumentContractType = indexingExecutionContext.ProvisioningContext.ContractType
          }
        };
        indexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, collectionIndexingUnit);
        indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully assigned {0} to host for indexing.", (object) str)));
        List<ShardAssignmentDetails> assignedShards = indexingUnitsWithSizeEstimates.Count <= 0 ? new List<ShardAssignmentDetails>() : this.AssignShards(indexingExecutionContext, indexingUnitsWithSizeEstimates);
        this.MarkIndicesInactive(indexingExecutionContext, clusterName, activeShards, str, (IList<ShardAssignmentDetails>) assignedShards, stringSet, maxNumberOfDocumentsInAShard, configValueOrDefault);
        stopwatch.Stop();
        IDictionary<string, object> properties2 = (IDictionary<string, object>) new FriendlyDictionary<string, object>();
        properties2.Add("NumberOfActiveIndices", (object) activeShards.Count);
        properties2.Add("TimeTakenInIndexAssignmentInMillis", (object) stopwatch.ElapsedMilliseconds);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "IndexingOperation", properties2);
        return assignedShards;
      }
    }

    internal bool IsShardAllocationAppropriate(IList<ShardAssignmentDetails> assignedShards)
    {
      foreach (ShardAssignmentDetails assignedShard in (IEnumerable<ShardAssignmentDetails>) assignedShards)
      {
        if (assignedShard.FreeSpaceAvailable < 0)
          return false;
      }
      return true;
    }

    internal virtual void MarkIndicesInactive(
      IndexingExecutionContext indexingExecutionContext,
      string esClusterName,
      IDictionary<string, IList<ShardDetails>> indexNameToShards,
      string assignedIndex,
      IList<ShardAssignmentDetails> assignedShards,
      HashSet<string> previouslyAssignedIndices,
      int maxNumberOfDocumentsInAShard,
      float thresholdToConsiderAShardFull)
    {
      foreach (string previouslyAssignedIndex in previouslyAssignedIndices)
      {
        if (!string.IsNullOrWhiteSpace(previouslyAssignedIndex))
          indexNameToShards.Remove(previouslyAssignedIndex);
      }
      IList<ShardDetails> source;
      if (indexNameToShards.TryGetValue(assignedIndex, out source))
      {
        Dictionary<short, ShardDetails> dictionary = source.ToDictionary<ShardDetails, short, ShardDetails>((Func<ShardDetails, short>) (x => x.ShardId), (Func<ShardDetails, ShardDetails>) (x => x));
        foreach (KeyValuePair<short, ShardAssignmentDetails> keyValuePair in assignedShards.ToDictionary<ShardAssignmentDetails, short, ShardAssignmentDetails>((Func<ShardAssignmentDetails, short>) (x => (short) x.ShardId), (Func<ShardAssignmentDetails, ShardAssignmentDetails>) (x => x)))
        {
          short key = keyValuePair.Key;
          ShardAssignmentDetails assignmentDetails = keyValuePair.Value;
          ShardDetails shardDetails = dictionary[key];
          shardDetails.EstimatedDocCount = assignmentDetails.CurrentEstimatedDocumentCount;
          shardDetails.EstimatedDocCountGrowth = assignmentDetails.EstimatedDocCountGrowth;
        }
      }
      else
      {
        List<ShardDetails> list = indexingExecutionContext.ShardDetailsDataAccess.QueryShardDetailsForAnIndex(indexingExecutionContext.RequestContext, esClusterName, CodeEntityRoutingProvider.EntityType, assignedIndex).ToList<ShardDetails>();
        indexNameToShards[assignedIndex] = (IList<ShardDetails>) list;
      }
      long reservedSpace;
      int reservedDocCount;
      this.m_provisionerConfigAndConstantsProvider.GetReservedSpaceInShards((ExecutionContext) indexingExecutionContext, IndexProvisionType.Shared, indexingExecutionContext.ProvisioningContext.ContractType, 0L, out reservedSpace, out reservedDocCount);
      int num1 = 0;
      foreach (KeyValuePair<string, IList<ShardDetails>> indexNameToShard in (IEnumerable<KeyValuePair<string, IList<ShardDetails>>>) indexNameToShards)
      {
        string key = indexNameToShard.Key;
        IList<ShardDetails> shardDetailsList = indexNameToShard.Value;
        bool flag1 = false;
        int num2 = 0;
        int num3 = 0;
        long num4 = 0;
        foreach (ShardDetails shardDetails in (IEnumerable<ShardDetails>) shardDetailsList)
        {
          bool flag2 = true;
          if (shardDetails.EstimatedDocCount + shardDetails.EstimatedDocCountGrowth >= (int) ((double) (maxNumberOfDocumentsInAShard - reservedDocCount) * (double) thresholdToConsiderAShardFull))
            flag2 = false;
          else if (shardDetails.ActualDocCount >= (int) ((double) (maxNumberOfDocumentsInAShard - reservedDocCount) * (double) thresholdToConsiderAShardFull))
            flag2 = false;
          else if (shardDetails.ActualSize >= (long) ((double) (42949672960L - reservedSpace) * (double) thresholdToConsiderAShardFull))
            flag2 = false;
          flag1 |= flag2;
          num2 += shardDetails.EstimatedDocCount + shardDetails.EstimatedDocCountGrowth;
          num3 += shardDetails.ActualDocCount;
          num4 += shardDetails.ActualSize;
        }
        if (flag1 && (num2 >= (int) ((double) ((maxNumberOfDocumentsInAShard - reservedDocCount) * shardDetailsList.Count) * (double) thresholdToConsiderAShardFull) || num3 >= (int) ((double) ((maxNumberOfDocumentsInAShard - reservedDocCount) * shardDetailsList.Count) * (double) thresholdToConsiderAShardFull) || num4 >= (long) ((double) ((42949672960L - reservedSpace) * (long) shardDetailsList.Count) * (double) thresholdToConsiderAShardFull)))
          flag1 = false;
        if (!flag1)
        {
          indexingExecutionContext.ShardDetailsDataAccess.MarkShardsInactive(indexingExecutionContext.RequestContext, esClusterName, CodeEntityRoutingProvider.EntityType, key);
          ++num1;
        }
      }
      IDictionary<string, object> properties = (IDictionary<string, object>) new FriendlyDictionary<string, object>();
      properties.Add("IndexMarkedInactive", (object) num1);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "IndexingOperation", properties);
    }

    public virtual List<ShardAssignmentDetails> AssignShards(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSize)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      if (indexingUnitsWithSize.IsNullOrEmpty<IndexingUnitWithSize>())
        throw new ArgumentException("Null or empty list of indexing units is not allowed.", nameof (indexingUnitsWithSize));
      int currentHostConfigValue = indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/Routing/Code/MaxAllowedShardsToACollection", true, 50);
      IVssRequestContext deploymentContext = indexingExecutionContext.RequestContext.To(TeamFoundationHostType.Deployment);
      int documentsInAshard = indexingExecutionContext.ProvisioningContext.ContractType.GetMaxNumberOfDocumentsInAShard(indexingExecutionContext.RequestContext);
      int shardDensity = indexingExecutionContext.ProvisioningContext.ContractType.GetShardDensity(indexingExecutionContext.RequestContext);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit = indexingExecutionContext.CollectionIndexingUnit;
      string indexName1 = this.GetIndexName(indexingExecutionContext, indexingUnitsWithSize);
      string connectionString = indexingExecutionContext.ProvisioningContext.SearchPlatformConnectionString;
      if (string.IsNullOrWhiteSpace(indexName1) || string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Neither CollectionIndexName nor connectionString can be null or whitespace, " + FormattableString.Invariant(FormattableStringFactory.Create("CollectionIndexName :{0}, ConnectionString : {1}", (object) indexName1, (object) connectionString)));
      indexingUnitsWithSize = this.AssignIndexName(indexingExecutionContext, deploymentContext, connectionString, indexingUnitsWithSize, collectionIndexingUnit, indexName1);
      bool isShadow = indexingUnitsWithSize.Any<IndexingUnitWithSize>() && indexingUnitsWithSize.First<IndexingUnitWithSize>().IndexingUnit.IsShadow;
      IList<IndexingUnitDetails> indexingUnitDetails = this.GetCurrentlyAssignedIndexingUnitDetails(indexingExecutionContext, isShadow);
      IDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> toIndexingUnitMap = this.GetIndexingUnitIdToIndexingUnitMap(indexingExecutionContext, indexingUnitDetails);
      IDictionary<string, List<IndexingUnitWithSize>> dictionary = (IDictionary<string, List<IndexingUnitWithSize>>) new FriendlyDictionary<string, List<IndexingUnitWithSize>>();
      foreach (IndexingUnitWithSize indexingUnitWithSize in indexingUnitsWithSize)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitWithSize.IndexingUnit;
        string indexingIndexName = indexingUnit.GetIndexingIndexName();
        if (string.IsNullOrWhiteSpace(indexingIndexName))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTraceMessage("Indexing Pipeline", "IndexingOperation", Level.Error, FormattableString.Invariant(FormattableStringFactory.Create("No index present in {0}.", (object) indexingUnit)));
        }
        else
        {
          List<IndexingUnitWithSize> indexingUnitWithSizeList;
          if (!dictionary.TryGetValue(indexingIndexName, out indexingUnitWithSizeList))
          {
            indexingUnitWithSizeList = new List<IndexingUnitWithSize>();
            dictionary[indexingIndexName] = indexingUnitWithSizeList;
          }
          indexingUnitWithSizeList.Add(indexingUnitWithSize);
        }
      }
      List<ShardAssignmentDetails> assignmentDetailsList = new List<ShardAssignmentDetails>();
      string clusterName = indexingExecutionContext.ProvisioningContext.SearchClusterManagementService.GetClusterName();
      foreach (KeyValuePair<string, List<IndexingUnitWithSize>> keyValuePair in (IEnumerable<KeyValuePair<string, List<IndexingUnitWithSize>>>) dictionary)
      {
        string indexName = keyValuePair.Key;
        List<IndexingUnitWithSize> indexingUnitsWithSizeWithValidSizeEstimate = keyValuePair.Value;
        List<IndexingUnitDetails> list = indexingUnitDetails.Where<IndexingUnitDetails>((Func<IndexingUnitDetails, bool>) (x => x.IndexName == indexName)).ToList<IndexingUnitDetails>();
        List<ShardAssignmentDetails> collection = this.AssignShards(indexingExecutionContext, deploymentContext, indexingUnitsWithSizeWithValidSizeEstimate, (IList<IndexingUnitDetails>) list, toIndexingUnitMap, clusterName, connectionString, indexName, indexName1, shardDensity, currentHostConfigValue, documentsInAshard);
        assignmentDetailsList.AddRange((IEnumerable<ShardAssignmentDetails>) collection);
      }
      IDictionary<string, object> properties = (IDictionary<string, object>) new FriendlyDictionary<string, object>();
      properties.Add("TimeTakenInShardsAssignmentInMillis", (object) stopwatch.ElapsedMilliseconds);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "IndexingOperation", properties);
      return assignmentDetailsList;
    }

    internal long ComputeHash(string item)
    {
      long hash = 0;
      foreach (char ch in item)
        hash += (long) ch;
      return hash;
    }

    internal virtual List<IndexingUnitWithSize> AssignIndexName(
      IndexingExecutionContext indexingExecutionContext,
      IVssRequestContext deploymentContext,
      string connectionString,
      List<IndexingUnitWithSize> indexingUnitsWithSizes,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      string collectionIndexName)
    {
      List<IndexingUnitWithSize> indexingUnitWithSizeList = new List<IndexingUnitWithSize>();
      FriendlyDictionary<string, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dedicatedIndexToLargeRepoIndexingUnit = new FriendlyDictionary<string, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      FriendlyDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> friendlyDictionary = new FriendlyDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> sharedIndexLargeRepoIndexingUnits = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      foreach (IndexingUnitWithSize indexingUnitsWithSiz in indexingUnitsWithSizes)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 = indexingUnitsWithSiz.IndexingUnit;
        if (indexingUnit1.IsRepository())
        {
          if (indexingUnit1.IsLargeRepository(indexingExecutionContext.RequestContext))
          {
            friendlyDictionary[indexingUnit1.IndexingUnitId] = indexingUnit1;
            if (indexingUnit1.IndexingUnitType == "Git_Repository")
            {
              string key = indexingUnit1.GetIndexingIndexName();
              if (string.IsNullOrWhiteSpace(key))
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTraceMessage("Indexing Pipeline", "IndexingOperation", Level.Error, FormattableString.Invariant(FormattableStringFactory.Create("No index name present in large repo indexing unit {0}. Provisioning a new dedicated index.", (object) indexingUnit1)));
                key = this.CreateIndex(indexingExecutionContext, indexingUnit1, IndexProvisionType.Dedicated);
              }
              dedicatedIndexToLargeRepoIndexingUnit[key] = indexingUnit1;
            }
            else
            {
              indexingUnit1.Properties.IndexIndices = new List<IndexInfo>()
              {
                new IndexInfo()
                {
                  IndexName = collectionIndexName,
                  Version = new int?(indexingExecutionContext.GetIndexVersion(collectionIndexName)),
                  DocumentContractType = this.GetDocumentContractType(indexingExecutionContext, indexingUnit1)
                }
              };
              sharedIndexLargeRepoIndexingUnits.Add(indexingUnit1);
            }
          }
          else
          {
            indexingUnit1.Properties.IndexIndices = new List<IndexInfo>()
            {
              new IndexInfo()
              {
                IndexName = collectionIndexName,
                Version = new int?(indexingExecutionContext.GetIndexVersion(collectionIndexName)),
                DocumentContractType = this.GetDocumentContractType(indexingExecutionContext, indexingUnit1)
              }
            };
            indexingUnitWithSizeList.Add(indexingUnitsWithSiz);
          }
        }
        else
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2;
          if (indexingUnit1.IndexingUnitType == "ScopedIndexingUnit" && !friendlyDictionary.TryGetValue(indexingUnit1.ParentUnitId, out indexingUnit2))
          {
            indexingUnit2 = indexingExecutionContext.IndexingUnitDataAccess.GetIndexingUnit(indexingExecutionContext.RequestContext, indexingUnit1.ParentUnitId);
            friendlyDictionary[indexingUnit2.IndexingUnitId] = indexingUnit2;
          }
        }
      }
      foreach (IndexingUnitWithSize indexingUnitsWithSiz in indexingUnitsWithSizes)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit3 = indexingUnitsWithSiz.IndexingUnit;
        if (indexingUnit3.IndexingUnitType == "ScopedIndexingUnit")
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit4 = friendlyDictionary[indexingUnit3.ParentUnitId];
          string str;
          if (indexingUnit4.IndexingUnitType == "Git_Repository")
          {
            str = indexingUnit4.GetIndexingIndexName();
            if (string.IsNullOrWhiteSpace(str))
            {
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTraceMessage("Indexing Pipeline", "IndexingOperation", Level.Error, FormattableString.Invariant(FormattableStringFactory.Create("No index name present in repo indexing unit {0}. Provisioning a new index.", (object) indexingUnit4)));
              str = this.CreateIndex(indexingExecutionContext, indexingUnit4, IndexProvisionType.Dedicated);
            }
            dedicatedIndexToLargeRepoIndexingUnit[str] = indexingUnit4;
          }
          else
            str = collectionIndexName;
          indexingUnit3.Properties.IndexIndices = new List<IndexInfo>()
          {
            new IndexInfo()
            {
              IndexName = str,
              Version = new int?(indexingExecutionContext.GetIndexVersion(str)),
              DocumentContractType = this.GetDocumentContractType(indexingExecutionContext, indexingUnit3)
            }
          };
          if (indexingUnit4.IsLargeRepository(indexingExecutionContext.RequestContext))
            indexingUnitWithSizeList.Add(indexingUnitsWithSiz);
        }
      }
      if (dedicatedIndexToLargeRepoIndexingUnit.Count > 0 || sharedIndexLargeRepoIndexingUnits.Count > 0)
        this.AssignRoutingValuesToLargeRepo(indexingExecutionContext, deploymentContext, connectionString, (IDictionary<string, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) dedicatedIndexToLargeRepoIndexingUnit, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) sharedIndexLargeRepoIndexingUnits);
      return indexingUnitWithSizeList;
    }

    internal virtual void AssignRoutingValuesToLargeRepo(
      IndexingExecutionContext indexingExecutionContext,
      IVssRequestContext deploymentContext,
      string connectionString,
      IDictionary<string, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dedicatedIndexToLargeRepoIndexingUnit,
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> sharedIndexLargeRepoIndexingUnits)
    {
      IRoutingLookupService routingLookupService = deploymentContext.GetService<IRoutingLookupService>();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> keyValuePair in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>) dedicatedIndexToLargeRepoIndexingUnit)
      {
        string dedicatedIndexName = keyValuePair.Key;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = keyValuePair.Value;
        string indexName = indexingExecutionContext.CollectionIndexingUnit.GetIndexInfo().IndexName;
        List<string> list = this.GetShardDetails(indexingExecutionContext, dedicatedIndexName, indexName).Select<ShardDetails, short>((Func<ShardDetails, short>) (x => x.ShardId)).ToList<short>().Select<short, string>((Func<short, string>) (x => routingLookupService.GetRoutingKey(deploymentContext, connectionString, dedicatedIndexName, (int) x))).ToList<string>();
        indexingUnit.Properties.IndexIndices = new List<IndexInfo>()
        {
          new IndexInfo()
          {
            IndexName = dedicatedIndexName,
            Version = new int?(indexingExecutionContext.GetIndexVersion(dedicatedIndexName)),
            DocumentContractType = this.GetDocumentContractType(indexingExecutionContext, indexingUnit),
            Routing = string.Join(",", (IEnumerable<string>) list)
          }
        };
      }
      indexingUnitList.AddRangeIfRangeNotNull<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) dedicatedIndexToLargeRepoIndexingUnit.Values.ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>());
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) sharedIndexLargeRepoIndexingUnits)
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsRoutingInfo = indexingExecutionContext.IndexingUnitDataAccess.GetChildIndexingUnitsRoutingInfo(indexingExecutionContext.RequestContext, "ScopedIndexingUnit", repoIndexingUnit.IndexingUnitId);
        HashSet<string> stringSet = new HashSet<string>();
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in unitsRoutingInfo)
        {
          string routing = indexingUnit.GetIndexInfo().Routing;
          if (!string.IsNullOrWhiteSpace(routing))
            stringSet.AddRangeIfRangeNotNull<string, HashSet<string>>((IEnumerable<string>) routing.Split(','));
        }
        repoIndexingUnit.GetIndexInfo().Routing = string.Join(",", (IEnumerable<string>) stringSet);
      }
      indexingUnitList.AddRangeIfRangeNotNull<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) sharedIndexLargeRepoIndexingUnits);
      indexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnits(indexingExecutionContext.RequestContext, indexingUnitList);
    }

    internal virtual List<ShardAssignmentDetails> AssignShards(
      IndexingExecutionContext indexingExecutionContext,
      IVssRequestContext deploymentContext,
      List<IndexingUnitWithSize> indexingUnitsWithSizeWithValidSizeEstimate,
      IList<IndexingUnitDetails> currentlyAssignedIndexingUnitDetails,
      IDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitIdToIndexingUnitMap,
      string esClusterName,
      string connectionString,
      string indexName,
      string collectionIndexName,
      int shardDensity,
      int maxAllowedShards,
      int maxNumberOfDocumentsInAShard)
    {
      IList<IndexingUnitWithSize> unitsAfterSplitting = (IList<IndexingUnitWithSize>) this.GetAllIndexingUnitsAfterSplitting(indexingExecutionContext, indexingUnitsWithSizeWithValidSizeEstimate, maxNumberOfDocumentsInAShard);
      lock (CodeEntityRoutingProvider.s_lock)
      {
        IList<ShardDetails> shardDetails = this.GetShardDetails(indexingExecutionContext, indexName, collectionIndexName);
        IList<ShardAssignmentDetails> assignedShardDetails = (IList<ShardAssignmentDetails>) this.GetCurrentlyAssignedShardDetails(indexingExecutionContext, shardDetails, currentlyAssignedIndexingUnitDetails, indexingUnitIdToIndexingUnitMap, maxNumberOfDocumentsInAShard);
        IList<ShardAssignmentDetails> assignmentDetailsList = this.GetShardAllocator(maxAllowedShards, shardDetails).ProvisionShards(indexingExecutionContext, assignedShardDetails, unitsAfterSplitting);
        this.UpdateEstimates(indexingExecutionContext, deploymentContext, (IList<IndexingUnitWithSize>) indexingUnitsWithSizeWithValidSizeEstimate, assignmentDetailsList, shardDetails, esClusterName, connectionString, indexName, shardDensity);
        return assignmentDetailsList.ToList<ShardAssignmentDetails>();
      }
    }

    internal virtual IList<ShardDetails> PopulateShardDetailsIfNullOrEmpty(
      IndexingExecutionContext indexingExecutionContext,
      string clusterName,
      string indexName,
      string collectionIndexName,
      IList<ShardDetails> shardDetails)
    {
      if (shardDetails.IsNullOrEmpty<ShardDetails>())
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTraceMessage("Indexing Pipeline", "IndexingOperation", Level.Error, FormattableString.Invariant(FormattableStringFactory.Create("Shard details not found in Db for the cluster: {0}, index: {1}, EntityType: {2}.", (object) clusterName, (object) indexName, (object) CodeEntityRoutingProvider.EntityType)));
        IndexProvisionType provisionType = indexName == collectionIndexName ? IndexProvisionType.Shared : IndexProvisionType.Dedicated;
        this.m_searchIndexHelper.PopulateShardDetails((ExecutionContext) indexingExecutionContext, indexingExecutionContext.ProvisioningContext.SearchClusterManagementService, indexingExecutionContext.DataAccessFactory, indexingExecutionContext.ProvisioningContext.EntityProvisionProvider, provisionType, IndexIdentity.CreateIndexIdentity(indexName), indexingExecutionContext.ProvisioningContext.ContractType);
        shardDetails = indexingExecutionContext.ShardDetailsDataAccess.QueryShardDetailsForAnIndex(indexingExecutionContext.RequestContext, clusterName, CodeEntityRoutingProvider.EntityType, indexName);
        if (shardDetails.IsNullOrEmpty<ShardDetails>())
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Something is wrong here, we could not find shard details of Cluster: {0}, IndexName:", (object) clusterName)) + FormattableString.Invariant(FormattableStringFactory.Create(" {0}, EntityType: {1}. Neither could we populate using ElasticSearch", (object) indexName, (object) CodeEntityRoutingProvider.EntityType)));
      }
      return shardDetails;
    }

    internal virtual IList<ShardDetails> GetShardDetails(
      IndexingExecutionContext indexingExecutionContext,
      string indexName,
      string collectionIndexName)
    {
      IShardDetailsDataAccess detailsDataAccess = indexingExecutionContext.ShardDetailsDataAccess;
      string clusterName = indexingExecutionContext.ProvisioningContext.SearchClusterManagementService.GetClusterName();
      IVssRequestContext requestContext = indexingExecutionContext.RequestContext;
      string esCluster = clusterName;
      IEntityType entityType = CodeEntityRoutingProvider.EntityType;
      string indexName1 = indexName;
      IList<ShardDetails> shardDetails = detailsDataAccess.QueryShardDetailsForAnIndex(requestContext, esCluster, entityType, indexName1);
      return this.PopulateShardDetailsIfNullOrEmpty(indexingExecutionContext, clusterName, indexName, collectionIndexName, shardDetails);
    }

    internal virtual List<IndexingUnitWithSize> GetAllIndexingUnitsAfterSplitting(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> currentIndexingUnitsWithSize,
      int maxNumberOfDocumentsInAShard)
    {
      List<IndexingUnitWithSize> unitsAfterSplitting = new List<IndexingUnitWithSize>();
      foreach (IndexingUnitWithSize indexingUnitWithSize in currentIndexingUnitsWithSize)
      {
        if (this.ShouldAssignMultipleShardsToAGivenIndexingUnit(indexingUnitWithSize, maxNumberOfDocumentsInAShard))
        {
          int num = indexingUnitWithSize.TotalSize % maxNumberOfDocumentsInAShard == 0 ? indexingUnitWithSize.TotalSize / maxNumberOfDocumentsInAShard : indexingUnitWithSize.TotalSize / maxNumberOfDocumentsInAShard + 1;
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "Indexer", "NumberOfSplitIndexingUnitsForShardAllocation", (object) num);
          for (int index = 1; index <= num; ++index)
            unitsAfterSplitting.Add(new IndexingUnitWithSize(indexingUnitWithSize.IndexingUnit, indexingUnitWithSize.CurrentEstimatedDocumentCount / num, indexingUnitWithSize.EstimatedGrowth / num, true));
        }
        else
          unitsAfterSplitting.Add(indexingUnitWithSize);
      }
      return unitsAfterSplitting;
    }

    internal bool ShouldAssignMultipleShardsToAGivenIndexingUnit(
      IndexingUnitWithSize indexingUnitWithSize,
      int maxShardSize)
    {
      return indexingUnitWithSize.TotalSize > maxShardSize;
    }

    internal virtual IDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetIndexingUnitIdToIndexingUnitMap(
      IndexingExecutionContext indexingExecutionContext,
      IList<IndexingUnitDetails> allIndexingUnitDetails)
    {
      return indexingExecutionContext.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, (IEnumerable<int>) allIndexingUnitDetails.Select<IndexingUnitDetails, int>((Func<IndexingUnitDetails, int>) (x => x.IndexingUnitId)).ToList<int>());
    }

    internal virtual DefaultSizeBasedShardAllocator GetShardAllocator(
      int maxAllowedShards,
      IList<ShardDetails> shardDetails)
    {
      maxAllowedShards = Math.Min(maxAllowedShards, shardDetails.Count);
      return new DefaultSizeBasedShardAllocator(maxAllowedShards);
    }

    internal virtual IList<IndexingUnitDetails> GetCurrentlyAssignedIndexingUnitDetails(
      IndexingExecutionContext indexingExecutionContext,
      bool isShadow = false)
    {
      List<IndexingUnitDetails> indexingUnitDetails = new List<IndexingUnitDetails>();
      List<IndexingUnitDetails> collection1 = indexingExecutionContext.IndexingUnitDataAccess.FetchIndexingUnitDetails(indexingExecutionContext.RequestContext, CodeEntityRoutingProvider.EntityType, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit.GetIndexingUnitTypeExtended("Git_Repository", isShadow));
      if (collection1 != null)
        indexingUnitDetails.AddRange((IEnumerable<IndexingUnitDetails>) collection1);
      List<IndexingUnitDetails> collection2 = indexingExecutionContext.IndexingUnitDataAccess.FetchIndexingUnitDetails(indexingExecutionContext.RequestContext, CodeEntityRoutingProvider.EntityType, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit.GetIndexingUnitTypeExtended("TFVC_Repository", isShadow));
      if (collection2 != null)
        indexingUnitDetails.AddRange((IEnumerable<IndexingUnitDetails>) collection2);
      List<IndexingUnitDetails> collection3 = indexingExecutionContext.IndexingUnitDataAccess.FetchIndexingUnitDetails(indexingExecutionContext.RequestContext, CodeEntityRoutingProvider.EntityType, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit.GetIndexingUnitTypeExtended("CustomRepository", isShadow));
      if (collection3 != null)
        indexingUnitDetails.AddRange((IEnumerable<IndexingUnitDetails>) collection3);
      List<IndexingUnitDetails> collection4 = indexingExecutionContext.IndexingUnitDataAccess.FetchIndexingUnitDetails(indexingExecutionContext.RequestContext, CodeEntityRoutingProvider.EntityType, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit.GetIndexingUnitTypeExtended("ScopedIndexingUnit", isShadow));
      if (collection4 != null)
        indexingUnitDetails.AddRange((IEnumerable<IndexingUnitDetails>) collection4);
      return (IList<IndexingUnitDetails>) indexingUnitDetails;
    }

    internal virtual List<ShardAssignmentDetails> GetCurrentlyAssignedShardDetails(
      IndexingExecutionContext indexingExecutionContext,
      IList<ShardDetails> shards,
      IList<IndexingUnitDetails> allIndexingUnitDetails,
      IDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitIdToIndexingUnitMap,
      int maxNumberOfDocumentsInAShard)
    {
      IDictionary<short, HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>> dictionary = (IDictionary<short, HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>) new FriendlyDictionary<short, HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>();
      foreach (IndexingUnitDetails indexingUnitDetail in (IEnumerable<IndexingUnitDetails>) allIndexingUnitDetails)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit;
        if (indexingUnitIdToIndexingUnitMap.TryGetValue(indexingUnitDetail.IndexingUnitId, out indexingUnit))
        {
          if (string.IsNullOrWhiteSpace(indexingUnitDetail.ShardIds))
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTraceMessage("Indexing Pipeline", "IndexingOperation", Level.Error, FormattableString.Invariant(FormattableStringFactory.Create("ShardIds {0} are null for Indexing Unit {1}.", (object) indexingUnitDetail.ShardIds, (object) indexingUnit)));
          }
          else
          {
            string shardIds = indexingUnitDetail.ShardIds;
            string[] separator = new string[1]{ "," };
            foreach (string s in shardIds.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            {
              short key = short.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
              HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitSet;
              if (!dictionary.TryGetValue(key, out indexingUnitSet))
              {
                indexingUnitSet = new HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
                dictionary[key] = indexingUnitSet;
              }
              indexingUnitSet.Add(indexingUnit);
            }
          }
        }
      }
      List<ShardAssignmentDetails> assignedShardDetails = new List<ShardAssignmentDetails>();
      foreach (ShardDetails shard in (IEnumerable<ShardDetails>) shards)
      {
        int estimatedDocCount = shard.EstimatedDocCount;
        int estimatedDocCountGrowth = shard.EstimatedDocCountGrowth;
        int reservedDocCount = shard.ReservedDocCount;
        short shardId = shard.ShardId;
        HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits;
        dictionary.TryGetValue(shardId, out indexingUnits);
        ShardAssignmentDetails assignmentDetails = new ShardAssignmentDetails((int) shardId, estimatedDocCount, estimatedDocCountGrowth, reservedDocCount, maxNumberOfDocumentsInAShard, indexingUnits);
        assignedShardDetails.Add(assignmentDetails);
      }
      return assignedShardDetails;
    }

    internal virtual void UpdateEstimates(
      IndexingExecutionContext indexingExecutionContext,
      IVssRequestContext deploymentContext,
      IList<IndexingUnitWithSize> indexingUnitsWithSizeEstimates,
      IList<ShardAssignmentDetails> assignedShards,
      IList<ShardDetails> shardEstimationsBeforeAllocation,
      string clusterName,
      string connectionString,
      string indexName,
      int shardDensity)
    {
      HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> collection = new HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      collection.AddRange<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>(indexingUnitsWithSizeEstimates.Select<IndexingUnitWithSize, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<IndexingUnitWithSize, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x.IndexingUnit)));
      IDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, List<int>> dictionary = (IDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, List<int>>) new FriendlyDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, List<int>>();
      foreach (ShardAssignmentDetails assignedShard in (IEnumerable<ShardAssignmentDetails>) assignedShards)
      {
        HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = assignedShard.IndexingUnits;
        if (indexingUnits != null)
        {
          int shardId = assignedShard.ShardId;
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit key in indexingUnits)
          {
            if (collection.Contains(key))
            {
              List<int> intList;
              if (!dictionary.TryGetValue(key, out intList))
              {
                intList = new List<int>();
                dictionary[key] = intList;
              }
              intList.Add(shardId);
            }
          }
        }
      }
      IRoutingLookupService routingLookupService = deploymentContext.GetService<IRoutingLookupService>();
      List<IndexingUnitDetails> indexingUnitIndexingInformationList = new List<IndexingUnitDetails>();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      foreach (IndexingUnitWithSize withSizeEstimate in (IEnumerable<IndexingUnitWithSize>) indexingUnitsWithSizeEstimates)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = withSizeEstimate.IndexingUnit;
        List<int> intList;
        if (dictionary.TryGetValue(indexingUnit, out intList))
        {
          List<string> list = intList.Select<int, string>((Func<int, string>) (x => routingLookupService.GetRoutingKey(deploymentContext, connectionString, indexName, x))).ToList<string>();
          int estimatedDocumentCount = withSizeEstimate.CurrentEstimatedDocumentCount;
          int estimatedGrowth = withSizeEstimate.EstimatedGrowth;
          IndexingUnitDetails indexingUnitDetails = new IndexingUnitDetails(indexingUnit.IndexingUnitId, indexingUnit.TFSEntityId, CodeEntityRoutingProvider.EntityType, indexingUnit.IndexingUnitType, indexingUnit.IsShadow, intList, list, estimatedDocumentCount, estimatedGrowth, (float) shardDensity, clusterName, indexName, withSizeEstimate.ActualInitialSize, withSizeEstimate.ActualInitialDocCount);
          indexingUnitIndexingInformationList.Add(indexingUnitDetails);
          IndexInfo indexInfo = new IndexInfo()
          {
            IndexName = indexName,
            Version = new int?(indexingExecutionContext.GetIndexVersion(indexName)),
            Routing = indexingUnitDetails.RoutingIds,
            DocumentContractType = this.GetDocumentContractType(indexingExecutionContext, indexingUnit)
          };
          indexingUnit.Properties.IndexIndices = new List<IndexInfo>()
          {
            indexInfo
          };
          indexingUnitList.Add(indexingUnit);
        }
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTraceMessage("Indexing Pipeline", "IndexingOperation", Level.Error, FormattableString.Invariant(FormattableStringFactory.Create("No Shards Assigned to {0}.", (object) indexingUnit)));
      }
      indexingExecutionContext.IndexingUnitDataAccess.AddOrResetIndexingUnitDetailsAndUpdateShardDetails(indexingExecutionContext.RequestContext, (IList<IndexingUnitDetails>) indexingUnitIndexingInformationList, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnitList, shardEstimationsBeforeAllocation);
    }

    internal virtual string CreateIndex(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IndexProvisionType indexProvisionType)
    {
      DocumentContractType documentContractType = this.GetDocumentContractType(indexingExecutionContext, indexingUnit);
      string str = indexProvisionType == IndexProvisionType.Dedicated ? this.m_provisionerConfigAndConstantsProvider.GetDedicatedIndexName(documentContractType, indexingUnit.TFSEntityId) : this.m_provisionerConfigAndConstantsProvider.GetSharedIndexName(documentContractType);
      IndexIdentity indexIdentity = IndexIdentity.CreateIndexIdentity(str);
      this.m_searchIndexHelper.CreateIndex((ExecutionContext) indexingExecutionContext, indexIdentity, indexingExecutionContext.ProvisioningContext.SearchPlatform, indexingExecutionContext.ProvisioningContext.SearchClusterManagementService, indexingExecutionContext.DataAccessFactory, indexProvisionType, documentContractType, this.m_provisionerConfigAndConstantsProvider);
      if (indexProvisionType == IndexProvisionType.Dedicated)
        indexingUnit.Properties.IndexIndices = new List<IndexInfo>()
        {
          new IndexInfo()
          {
            IndexName = str,
            Version = new int?(indexingExecutionContext.GetIndexVersion(str)),
            DocumentContractType = documentContractType
          }
        };
      IDictionary<string, object> properties = (IDictionary<string, object>) new FriendlyDictionary<string, object>();
      properties.Add("NewIndexCreated", (object) 1);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "IndexingOperation", properties);
      return indexIdentity.Name;
    }

    internal virtual string GetIndexName(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSize)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit = indexingExecutionContext.CollectionIndexingUnit;
      string indexName = collectionIndexingUnit.GetIndexInfo()?.IndexName;
      if (indexingExecutionContext.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled() && indexingExecutionContext.IsReindexingFailedOrInProgress(indexingExecutionContext.DataAccessFactory, indexingExecutionContext.IndexingUnit.EntityType) && indexingUnitsWithSize.Count == 1 && !indexingUnitsWithSize[0].IndexingUnit.IsLargeRepository(indexingExecutionContext.RequestContext) && (indexingUnitsWithSize[0].IndexingUnit.IndexingUnitType == "Git_Repository" || indexingUnitsWithSize[0].IndexingUnit.IndexingUnitType == "TFVC_Repository") && !indexingUnitsWithSize[0].IndexingUnit.IsShadow)
      {
        string str;
        if (collectionIndexingUnit.Properties is CollectionIndexingProperties properties)
        {
          int? count = properties.IndexIndicesPreReindexing?.Count;
          int num = 1;
          if (count.GetValueOrDefault() == num & count.HasValue)
          {
            str = properties.IndexIndicesPreReindexing[0].IndexName;
            goto label_5;
          }
        }
        str = indexName;
label_5:
        indexName = str;
      }
      return indexName;
    }

    internal virtual DocumentContractType GetDocumentContractType(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit = indexingExecutionContext.CollectionIndexingUnit;
      DocumentContractType documentContractType = indexingExecutionContext.ProvisioningContext.ContractType;
      if (indexingExecutionContext.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled() && indexingExecutionContext.IsReindexingFailedOrInProgress(indexingExecutionContext.DataAccessFactory, indexingExecutionContext.IndexingUnit.EntityType) && (indexingUnit.IndexingUnitType == "Git_Repository" || indexingUnit.IndexingUnitType == "TFVC_Repository") && !indexingUnit.IsShadow)
        documentContractType = (collectionIndexingUnit.Properties as CollectionIndexingProperties).QueryContractType;
      return documentContractType;
    }
  }
}
