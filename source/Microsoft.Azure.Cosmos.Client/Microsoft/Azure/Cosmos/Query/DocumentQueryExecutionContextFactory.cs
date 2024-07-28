// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.DocumentQueryExecutionContextFactory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query
{
  internal static class DocumentQueryExecutionContextFactory
  {
    private const int PageSizeFactorForTop = 5;

    public static async Task<IDocumentQueryExecutionContext> CreateDocumentQueryExecutionContextAsync(
      IDocumentQueryClient client,
      ResourceType resourceTypeEnum,
      Type resourceType,
      Expression expression,
      FeedOptions feedOptions,
      string resourceLink,
      bool isContinuationExpected,
      CancellationToken token,
      Guid correlatedActivityId)
    {
      ContainerProperties collection = (ContainerProperties) null;
      if (resourceTypeEnum.IsCollectionChild())
      {
        using (DocumentServiceRequest request = DocumentServiceRequest.Create(OperationType.Query, resourceTypeEnum, resourceLink, AuthorizationTokenType.Invalid))
          collection = await (await client.GetCollectionCacheAsync()).ResolveCollectionAsync(request, token, (ITrace) NoOpTrace.Singleton);
        if (feedOptions?.PartitionKey != null && feedOptions.PartitionKey.Equals((object) Microsoft.Azure.Documents.PartitionKey.None))
          feedOptions.PartitionKey = Microsoft.Azure.Documents.PartitionKey.FromInternalKey(collection.GetNoneValue());
      }
      DocumentQueryExecutionContextBase.InitParams constructorParams = new DocumentQueryExecutionContextBase.InitParams(client, resourceTypeEnum, resourceType, expression, feedOptions, resourceLink, false, correlatedActivityId);
      if (CustomTypeExtensions.ByPassQueryParsing())
        return (IDocumentQueryExecutionContext) ProxyDocumentQueryExecutionContext.Create(client, resourceTypeEnum, resourceType, expression, feedOptions, resourceLink, token, collection, isContinuationExpected, correlatedActivityId);
      DefaultDocumentQueryExecutionContext queryExecutionContext = await DefaultDocumentQueryExecutionContext.CreateAsync(constructorParams, isContinuationExpected, token);
      if (resourceTypeEnum.IsCollectionChild() && resourceTypeEnum.IsPartitioned() && (feedOptions.EnableCrossPartitionQuery || !isContinuationExpected))
      {
        Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo executionInfoAsync = await queryExecutionContext.GetPartitionedQueryExecutionInfoAsync(collection.PartitionKey, true, isContinuationExpected, true, feedOptions.PartitionKey != null, true, token);
        if (DocumentQueryExecutionContextFactory.ShouldCreateSpecializedDocumentQueryExecutionContext(resourceTypeEnum, feedOptions, executionInfoAsync, collection.PartitionKey, isContinuationExpected))
        {
          List<PartitionKeyRange> partitionKeyRangesAsync = await DocumentQueryExecutionContextFactory.GetTargetPartitionKeyRangesAsync(queryExecutionContext, executionInfoAsync, collection, feedOptions);
          throw new NotSupportedException("v2 query excution context is currently not supported.");
        }
      }
      return (IDocumentQueryExecutionContext) queryExecutionContext;
    }

    internal static async Task<List<PartitionKeyRange>> GetTargetPartitionKeyRangesAsync(
      DefaultDocumentQueryExecutionContext queryExecutionContext,
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo,
      ContainerProperties collection,
      FeedOptions feedOptions)
    {
      List<PartitionKeyRange> partitionKeyRangesAsync;
      if (!string.IsNullOrEmpty(feedOptions.PartitionKeyRangeId))
      {
        List<PartitionKeyRange> partitionKeyRangeList1 = new List<PartitionKeyRange>();
        List<PartitionKeyRange> partitionKeyRangeList2 = partitionKeyRangeList1;
        partitionKeyRangeList2.Add(await queryExecutionContext.GetTargetPartitionKeyRangeByIdAsync(collection.ResourceId, feedOptions.PartitionKeyRangeId));
        partitionKeyRangesAsync = partitionKeyRangeList1;
        partitionKeyRangeList2 = (List<PartitionKeyRange>) null;
        partitionKeyRangeList1 = (List<PartitionKeyRange>) null;
      }
      else if (feedOptions.PartitionKey != null)
      {
        partitionKeyRangesAsync = await queryExecutionContext.GetTargetPartitionKeyRangesByEpkStringAsync(collection.ResourceId, feedOptions.PartitionKey.InternalKey.GetEffectivePartitionKeyString(collection.PartitionKey));
      }
      else
      {
        string effectivePartitionKeyString;
        if (DocumentQueryExecutionContextFactory.TryGetEpkProperty(feedOptions, out effectivePartitionKeyString))
          partitionKeyRangesAsync = await queryExecutionContext.GetTargetPartitionKeyRangesByEpkStringAsync(collection.ResourceId, effectivePartitionKeyString);
        else
          partitionKeyRangesAsync = await queryExecutionContext.GetTargetPartitionKeyRangesAsync(collection.ResourceId, partitionedQueryExecutionInfo.QueryRanges);
      }
      return partitionKeyRangesAsync;
    }

    private static bool ShouldCreateSpecializedDocumentQueryExecutionContext(
      ResourceType resourceTypeEnum,
      FeedOptions feedOptions,
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo,
      PartitionKeyDefinition partitionKeyDefinition,
      bool isContinuationExpected)
    {
      return DocumentQueryExecutionContextFactory.IsCrossPartitionQuery(resourceTypeEnum, feedOptions, partitionKeyDefinition, partitionedQueryExecutionInfo) && (DocumentQueryExecutionContextFactory.IsTopOrderByQuery(partitionedQueryExecutionInfo) || DocumentQueryExecutionContextFactory.IsAggregateQuery(partitionedQueryExecutionInfo) || DocumentQueryExecutionContextFactory.IsOffsetLimitQuery(partitionedQueryExecutionInfo) || DocumentQueryExecutionContextFactory.IsParallelQuery(feedOptions)) || !string.IsNullOrEmpty(feedOptions.PartitionKeyRangeId) || DocumentQueryExecutionContextFactory.IsAggregateQueryWithoutContinuation(partitionedQueryExecutionInfo, isContinuationExpected) || DocumentQueryExecutionContextFactory.IsDistinctQuery(partitionedQueryExecutionInfo) || DocumentQueryExecutionContextFactory.IsGroupByQuery(partitionedQueryExecutionInfo) || DocumentQueryExecutionContextFactory.IsDCountQuery(partitionedQueryExecutionInfo);
    }

    private static bool IsCrossPartitionQuery(
      ResourceType resourceTypeEnum,
      FeedOptions feedOptions,
      PartitionKeyDefinition partitionKeyDefinition,
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      if (!resourceTypeEnum.IsPartitioned() || feedOptions.PartitionKey != null || !feedOptions.EnableCrossPartitionQuery || partitionKeyDefinition.Paths.Count <= 0)
        return false;
      return partitionedQueryExecutionInfo.QueryRanges.Count != 1 || !partitionedQueryExecutionInfo.QueryRanges[0].IsSingleValue;
    }

    private static bool IsParallelQuery(FeedOptions feedOptions) => feedOptions.MaxDegreeOfParallelism != 0;

    private static bool IsTopOrderByQuery(
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      if (partitionedQueryExecutionInfo.QueryInfo == null)
        return false;
      return partitionedQueryExecutionInfo.QueryInfo.HasOrderBy || partitionedQueryExecutionInfo.QueryInfo.HasTop;
    }

    private static bool IsAggregateQuery(
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      return partitionedQueryExecutionInfo.QueryInfo != null && partitionedQueryExecutionInfo.QueryInfo.HasAggregates;
    }

    private static bool IsAggregateQueryWithoutContinuation(
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo,
      bool isContinuationExpected)
    {
      return DocumentQueryExecutionContextFactory.IsAggregateQuery(partitionedQueryExecutionInfo) && !isContinuationExpected;
    }

    private static bool IsOffsetLimitQuery(
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      return partitionedQueryExecutionInfo.QueryInfo.HasOffset && partitionedQueryExecutionInfo.QueryInfo.HasLimit;
    }

    private static bool IsDistinctQuery(
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      return partitionedQueryExecutionInfo.QueryInfo.HasDistinct;
    }

    private static bool IsGroupByQuery(
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      return partitionedQueryExecutionInfo.QueryInfo.HasGroupBy;
    }

    private static bool IsDCountQuery(
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      return partitionedQueryExecutionInfo.QueryInfo.HasDCount;
    }

    private static bool TryGetEpkProperty(
      FeedOptions feedOptions,
      out string effectivePartitionKeyString)
    {
      object obj;
      if (feedOptions?.Properties != null && feedOptions.Properties.TryGetValue("x-ms-effective-partition-key-string", out obj))
      {
        effectivePartitionKeyString = obj as string;
        if (string.IsNullOrEmpty(effectivePartitionKeyString))
          throw new ArgumentOutOfRangeException(nameof (effectivePartitionKeyString));
        return true;
      }
      effectivePartitionKeyString = (string) null;
      return false;
    }
  }
}
