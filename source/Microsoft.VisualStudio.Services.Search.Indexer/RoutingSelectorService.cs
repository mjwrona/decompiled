// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.RoutingSelectorService
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public class RoutingSelectorService : IRoutingSelectorService, IVssFrameworkService
  {
    private int m_maxShardsInAnIndex;
    private int m_maxReposInOneShard;
    private int m_shardNumberForNextRepo;
    private string m_indexName;
    private Random m_rand;
    private ElasticsearchFeedbackProcessor m_elasticsearchFeedbackProcessor;
    private readonly object m_lock = new object();

    public RoutingSelectorService()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    internal RoutingSelectorService(IDataAccessFactory dataAccessFactory) => this.DataAccessFactory = dataAccessFactory;

    internal IDataAccessFactory DataAccessFactory { get; set; }

    public string SelectRoutingForNextRepository(
      IndexingExecutionContext indexingExecutionContext,
      string indexName)
    {
      if (indexingExecutionContext == null)
        throw new ArgumentNullException(nameof (indexingExecutionContext));
      return indexName != null ? this.SelectRoutingForNextRepository(indexingExecutionContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>(), indexName) : throw new ArgumentNullException(nameof (indexName));
    }

    public string SelectRoutingForNextRepository(
      IndexingExecutionContext indexingExecutionContext,
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> newIndexingUnits,
      string indexName)
    {
      if (indexingExecutionContext == null)
        throw new ArgumentNullException(nameof (indexingExecutionContext));
      if (newIndexingUnits == null)
        throw new ArgumentNullException(nameof (newIndexingUnits));
      if (indexName == null)
        throw new ArgumentNullException(nameof (indexName));
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit = indexingExecutionContext.CollectionIndexingUnit;
      if (collectionIndexingUnit.EntityType.Name != "Code" && collectionIndexingUnit.EntityType.Name != "Wiki")
        throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("EntityType: {0} is not supported by routing selector service", (object) collectionIndexingUnit.EntityType)));
      if (collectionIndexingUnit.EntityType.Name == "Wiki")
        return collectionIndexingUnit.GetTfsEntityIdAsNormalizedString();
      if (this.m_shardNumberForNextRepo == -1 && this.m_maxShardsInAnIndex == -1 || !indexName.Equals(this.m_indexName))
        this.InitializeSettings(indexingExecutionContext);
      IEnumerable<KeyValuePair<string, int>> routingKeysUsage = this.GetExistingRoutingKeysUsage(indexingExecutionContext.RequestContext, (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) newIndexingUnits);
      if (routingKeysUsage != null && routingKeysUsage.Count<KeyValuePair<string, int>>() > 0)
      {
        IEnumerable<KeyValuePair<string, int>> source = routingKeysUsage.Where<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (routingGroup => routingGroup.Value < this.m_maxReposInOneShard && !this.IsShardForGivenRoutingOverSized(indexingExecutionContext, indexName, routingGroup.Key)));
        if (source != null && source.Count<KeyValuePair<string, int>>() > 0)
          return source.First<KeyValuePair<string, int>>().Key;
      }
      return this.GetNextRouting(indexingExecutionContext, routingKeysUsage != null ? routingKeysUsage.Select<KeyValuePair<string, int>, string>((Func<KeyValuePair<string, int>, string>) (routingGroup => routingGroup.Key)) : (IEnumerable<string>) null, indexName);
    }

    public IEnumerable<KeyValuePair<string, int>> GetExistingRoutingKeysUsage(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> newIndexingUnits)
    {
      IIndexingUnitDataAccess indexingUnitDataAccess = this.DataAccessFactory.GetIndexingUnitDataAccess();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> source = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits1 = indexingUnitDataAccess.GetIndexingUnits(requestContext, "TFVC_Repository", (IEntityType) CodeEntityType.GetInstance(), -1);
      if (indexingUnits1 != null && indexingUnits1.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        source.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnits1);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits2 = indexingUnitDataAccess.GetIndexingUnits(requestContext, "Git_Repository", (IEntityType) CodeEntityType.GetInstance(), -1);
      if (indexingUnits2 != null && indexingUnits2.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        source.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnits2);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits3 = indexingUnitDataAccess.GetIndexingUnits(requestContext, "CustomRepository", (IEntityType) CodeEntityType.GetInstance(), -1);
      if (indexingUnits3 != null && indexingUnits3.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        source.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnits3);
      if (newIndexingUnits != null && newIndexingUnits.Count<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>() != 0)
        source.AddRange(newIndexingUnits);
      return source.Count > 0 ? source.GroupBy<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, string>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, string>) (indexingUnit => indexingUnit.GetIndexInfo()?.Routing)).Select<IGrouping<string, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>, KeyValuePair<string, int>>((Func<IGrouping<string, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>, KeyValuePair<string, int>>) (indexingUnitGroup => new KeyValuePair<string, int>(indexingUnitGroup.Key, indexingUnitGroup.Count<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>()))).Where<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (routingGroup => !string.IsNullOrWhiteSpace(routingGroup.Key))) : Enumerable.Empty<KeyValuePair<string, int>>();
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      ExecutionContext executionContext = systemRequestContext.GetExecutionContext(MethodBase.GetCurrentMethod().Name, 1);
      this.m_elasticsearchFeedbackProcessor = new ElasticsearchFeedbackProcessor();
      this.m_rand = new Random();
      this.m_maxReposInOneShard = executionContext.ServiceSettings.ProvisionerSettings.MaxReposInOneShard;
      this.m_maxShardsInAnIndex = this.m_shardNumberForNextRepo = -1;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private string GetNextRouting(
      IndexingExecutionContext indexingExecutionContext,
      IEnumerable<string> existingRoutingKeys,
      string indexName)
    {
      if (existingRoutingKeys != null && existingRoutingKeys.Count<string>() >= this.m_maxShardsInAnIndex)
        return this.GetNextRoutingWhenAllRoutingKeysUsed(indexingExecutionContext, existingRoutingKeys, indexName);
      string routing = (string) null;
      for (int index = 0; index < this.m_maxShardsInAnIndex; ++index)
      {
        int numberForNextRepo;
        lock (this.m_lock)
        {
          numberForNextRepo = this.m_shardNumberForNextRepo;
          this.m_shardNumberForNextRepo = (this.m_shardNumberForNextRepo + 1) % this.m_maxShardsInAnIndex;
        }
        routing = this.GetRoutingKey(indexingExecutionContext, indexName, numberForNextRepo);
        if (existingRoutingKeys == null || !existingRoutingKeys.Contains<string>(routing) && !this.IsShardForGivenRoutingOverSized(indexingExecutionContext, indexName, routing))
          break;
      }
      return routing;
    }

    private bool IsShardForGivenRoutingOverSized(
      IndexingExecutionContext indexingExecutionContext,
      string indexName,
      string routing)
    {
      try
      {
        string shard = this.GetShard(indexingExecutionContext, indexName, routing);
        return this.IsShardOverSized(indexingExecutionContext, indexName, shard);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083034, "Indexing Pipeline", "Indexer", ex);
        return false;
      }
    }

    private string GetShard(
      IndexingExecutionContext indexingExecutionContext,
      string indexName,
      string routing)
    {
      IVssRequestContext context = indexingExecutionContext.RequestContext.To(TeamFoundationHostType.Deployment);
      IRoutingLookupService service = context.GetService<IRoutingLookupService>();
      string connectionString1 = indexingExecutionContext.ProvisioningContext.SearchPlatformConnectionString;
      IVssRequestContext requestContext = context;
      string connectionString2 = connectionString1;
      string indexName1 = indexName;
      string routing1 = routing;
      return service.GetShardId(requestContext, connectionString2, indexName1, routing1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    private bool IsShardOverSized(
      IndexingExecutionContext indexingExecutionContext,
      string indexName,
      string shard)
    {
      if (indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>(indexingExecutionContext.IndexingUnit.EntityType.GetESFeedbackLoopRegKey(), true))
      {
        try
        {
          long shardSizeInBytes = indexingExecutionContext.ProvisioningContext.SearchClusterManagementService.GetShardSizeInBytes((ExecutionContext) indexingExecutionContext, indexName, shard);
          if (this.m_elasticsearchFeedbackProcessor.IsShardOverSized(indexingExecutionContext.RequestContext, shardSizeInBytes, indexingExecutionContext.IndexingUnit.EntityType))
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "Indexer", "ShardOverSized", (object) string.Join(indexName, new string[2]
            {
              "|",
              shard
            }), true);
            return true;
          }
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083034, "Indexing Pipeline", "Indexer", ex);
        }
      }
      return false;
    }

    internal string GetNextRoutingWhenAllRoutingKeysUsed(
      IndexingExecutionContext indexingExecutionContext,
      IEnumerable<string> existingRoutingKeys,
      string indexName)
    {
      if (indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>(indexingExecutionContext.IndexingUnit.EntityType.GetESFeedbackLoopRegKey(), true))
      {
        try
        {
          string smallestShard = indexingExecutionContext.ProvisioningContext.SearchClusterManagementService.GetSmallestShard((ExecutionContext) indexingExecutionContext, indexName);
          int shardNumber = int.Parse(smallestShard, (IFormatProvider) CultureInfo.InvariantCulture);
          if (shardNumber < 0)
            throw new NotSupportedException("smallestShardId is not within the expected range");
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "Indexer", "AssignSmallestShard", (object) string.Join(indexName, new string[2]
          {
            "|",
            smallestShard
          }), true);
          return this.GetRoutingKey(indexingExecutionContext, indexName, shardNumber);
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083033, "Indexing Pipeline", "Indexer", ex);
        }
      }
      Random random = new Random();
      return existingRoutingKeys.ElementAt<string>(random.Next(this.m_maxShardsInAnIndex));
    }

    private string GetRoutingKey(
      IndexingExecutionContext indexingExecutionContext,
      string indexName,
      int shardNumber)
    {
      IVssRequestContext context = indexingExecutionContext.RequestContext.To(TeamFoundationHostType.Deployment);
      IRoutingLookupService service = context.GetService<IRoutingLookupService>();
      string connectionString1 = indexingExecutionContext.ProvisioningContext.SearchPlatformConnectionString;
      IVssRequestContext requestContext = context;
      string connectionString2 = connectionString1;
      string indexName1 = indexName;
      int shardId = shardNumber;
      return service.GetRoutingKey(requestContext, connectionString2, indexName1, shardId);
    }

    private void InitializeSettings(IndexingExecutionContext indexingExecutionContext)
    {
      try
      {
        this.m_indexName = indexingExecutionContext.CollectionIndexingUnit.GetIndexingIndexName();
        IndexIdentity indexIdentity = IndexIdentity.CreateIndexIdentity(this.m_indexName);
        this.m_maxShardsInAnIndex = indexingExecutionContext.ProvisioningContext.SearchPlatform.GetIndex(indexIdentity).GetSettings().NumberOfShards.Value;
        this.m_shardNumberForNextRepo = this.m_rand.Next(this.m_maxShardsInAnIndex);
      }
      catch (Exception ex)
      {
        throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Routing Selector Service failed while initialization with exception {0}", (object) ex)));
      }
    }
  }
}
