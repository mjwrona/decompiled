// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.RepositoryCodeDuplicateDocumentsDeletionOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class RepositoryCodeDuplicateDocumentsDeletionOperation : AbstractIndexingOperation
  {
    public RepositoryCodeDuplicateDocumentsDeletionOperation(
      ExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public RepositoryCodeDuplicateDocumentsDeletionOperation(
      ExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      IDataAccessFactory dataAccessFactory)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, dataAccessFactory)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      IndexingExecutionContext indexingExecutionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      IndexingUnit indexingUnit = indexingExecutionContext.IndexingUnit;
      indexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, indexingUnit);
      HashSet<string> stringSet = (HashSet<string>) null;
      IndexInfo indexInfo;
      try
      {
        if (indexingUnit.IsLargeRepository(indexingExecutionContext.RequestContext))
          throw new InvalidOperationException("Duplicate document deletion from Large repositories is not supported.");
        string indexName1 = indexingExecutionContext.CollectionIndexingUnit.GetIndexInfo()?.IndexName;
        string connectionString = indexingExecutionContext.ProvisioningContext.SearchPlatformConnectionString;
        if (string.IsNullOrWhiteSpace(indexName1) || string.IsNullOrWhiteSpace(connectionString))
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Neither CollectionIndexName nor connectionString can be null or whitespace, ")) + FormattableString.Invariant(FormattableStringFactory.Create("CollectionIndexName :{0}, ConnectionString : {1}", (object) indexName1, (object) connectionString)));
        indexInfo = indexingUnit.GetIndexInfo();
        if (indexInfo == null)
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("IndexInfo is null for Indexing Unit {0}. Failing this operation.", (object) indexingUnit)));
        if (string.IsNullOrWhiteSpace(indexInfo.IndexName) || string.IsNullOrWhiteSpace(indexInfo.Routing))
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Neither IndexName nor routing information can be null or whitespace, IndexName : {0}, Routing: {1}. Failing this operation.", (object) indexInfo.IndexName, (object) indexInfo.Routing)));
        if (indexInfo.Routing.Split(',').Length > 1)
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("IndexingUnit has multiple routing ids assigned, we can't delete duplicate documents of this indexing unit. Failing this operation.")));
        IVssRequestContext deploymentContext = indexingExecutionContext.RequestContext.To(TeamFoundationHostType.Deployment);
        IRoutingLookupService routingLookupService = deploymentContext.GetService<IRoutingLookupService>();
        string indexName = indexInfo.IndexName;
        string routing = indexInfo.Routing;
        int shardId = routingLookupService.GetShardId(deploymentContext, connectionString, indexName, routing);
        List<int> list = indexingExecutionContext.ProvisioningContext.SearchClusterManagementService.GetShardsDetails((ExecutionContext) indexingExecutionContext, indexName).Select<EsShardDetails, int>((Func<EsShardDetails, int>) (x => (int) x.ShardId)).ToList<int>();
        if (!list.Remove(shardId))
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Something is wrong here, {0} is not present in the list of shards in index {1}", (object) shardId, (object) string.Join<int>(", ", (IEnumerable<int>) list))));
        if (list.Count > 0)
        {
          stringSet = list.Select<int, string>((Func<int, string>) (x => routingLookupService.GetRoutingKey(deploymentContext, connectionString, indexName, x))).ToHashSet<string>();
          if (list.Count != stringSet.Count)
            throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Count of routing ids is not same as count of shards. Shard Ids in Index: {0}, Routing Ids for deletion: {1}", (object) string.Join<int>(", ", (IEnumerable<int>) list), (object) string.Join(", ", (IEnumerable<string>) stringSet))));
          foreach (string str in stringSet)
          {
            if (string.IsNullOrWhiteSpace(str))
              throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Null value of routingId in this list of routingIds to be used for deletion. Routing Ids for deletion: {0}. Failing this operation.", (object) string.Join(", ", (IEnumerable<string>) stringSet))));
            if (str == routing)
              throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Something is wrong here, {0} is present in the list of routing ids from which the data was supposed to be deleted. ", (object) routing)) + FormattableString.Invariant(FormattableStringFactory.Create("Assigned Routing Id: {0}, Assigned Shard Id: {1}, Shard Ids in Index: {2}, Routing Ids for deletion: {3}", (object) routing, (object) shardId, (object) string.Join<int>(", ", (IEnumerable<int>) list), (object) string.Join(", ", (IEnumerable<string>) stringSet))));
          }
        }
      }
      catch (InvalidOperationException ex)
      {
        return new OperationResult()
        {
          Status = OperationStatus.Failed,
          Message = ex.Message
        };
      }
      if (stringSet != null)
        this.DeleteDuplicateDocuments(indexingExecutionContext, indexInfo, stringSet);
      return new OperationResult()
      {
        Status = OperationStatus.Succeeded,
        Message = FormattableString.Invariant(FormattableStringFactory.Create("Duplication document deletion successful for Indexing Unit {0}", (object) indexingUnit))
      };
    }

    internal virtual void DeleteDuplicateDocuments(
      IndexingExecutionContext indexingExecutionContext,
      IndexInfo indexInfo,
      HashSet<string> routingIds)
    {
      string normalizedString = this.IndexingUnit.GetTfsEntityIdAsNormalizedString();
      string indexName = indexInfo.IndexName;
      ISearchIndex index = indexingExecutionContext.ProvisioningContext.SearchPlatform.GetIndex(IndexIdentity.CreateIndexIdentity(indexName));
      if (routingIds != null && routingIds.Count > 0)
      {
        IndexOperationsResponse operationsResponse = index.BulkDeleteByQuery<AbstractSearchDocumentContract>((ExecutionContext) indexingExecutionContext, new BulkDeleteByQueryRequest<AbstractSearchDocumentContract>()
        {
          Query = (IExpression) new TermExpression("repositoryId", Operator.Equals, normalizedString),
          ContractType = indexingExecutionContext.ProvisioningContext.ContractType,
          RoutingIds = routingIds.ToArray<string>()
        }, true);
        if (!operationsResponse.Success || operationsResponse.IsOperationIncomplete)
          throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("Duplicate document deletion failed.")));
      }
      else
        throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Invalid set of routing ids provided for deletion. Routing Ids : {0}", (object) routingIds)));
    }
  }
}
