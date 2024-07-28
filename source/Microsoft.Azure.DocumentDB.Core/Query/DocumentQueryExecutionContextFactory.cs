// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.DocumentQueryExecutionContextFactory
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Query.ParallelQuery;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query
{
  internal static class DocumentQueryExecutionContextFactory
  {
    public static bool ForceGatewayMode;
    private const int PageSizeFactorForTop = 5;

    public static Task<IDocumentQueryExecutionContext> CreateDocumentQueryExecutionContextAsync(
      IDocumentQueryClient client,
      ResourceType resourceTypeEnum,
      Type resourceType,
      Expression expression,
      FeedOptions feedOptions,
      IEnumerable<string> documentFeedLinks,
      bool isContinuationExpected,
      CancellationToken token,
      Guid correlatedActivityId)
    {
      return MultiCollectionDocumentQueryExecutionContext.CreateAsync(client, resourceTypeEnum, resourceType, expression, feedOptions, documentFeedLinks, isContinuationExpected, token, correlatedActivityId);
    }

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
      DocumentCollection collection = (DocumentCollection) null;
      if (resourceTypeEnum.IsCollectionChild())
      {
        using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Query, resourceTypeEnum, resourceLink, AuthorizationTokenType.Invalid))
        {
          collection = await (await client.GetCollectionCacheAsync()).ResolveCollectionAsync(request, token);
          if (feedOptions != null)
          {
            if (feedOptions.PartitionKey != null)
            {
              if (feedOptions.PartitionKey.Equals((object) PartitionKey.None))
                feedOptions.PartitionKey = PartitionKey.FromInternalKey(collection.NonePartitionKeyValue);
            }
          }
        }
      }
      DocumentQueryExecutionContextBase.InitParams constructorParams = new DocumentQueryExecutionContextBase.InitParams(client, resourceTypeEnum, resourceType, expression, feedOptions, resourceLink, false, correlatedActivityId);
      DefaultDocumentQueryExecutionContext queryExecutionContext = await DefaultDocumentQueryExecutionContext.CreateAsync(constructorParams, isContinuationExpected, token);
      if (resourceTypeEnum == ResourceType.Document && queryExecutionContext.QuerySpec != null)
      {
        PartitionedQueryExecutionInfo partitionedQueryExecutionInfo;
        if (CustomTypeExtensions.ByPassQueryParsing() || DocumentQueryExecutionContextFactory.ForceGatewayMode)
          partitionedQueryExecutionInfo = await QueryPlanRetriever.GetQueryPlanThroughGatewayAsync(client, collection, queryExecutionContext.QuerySpec, feedOptions.PartitionKey == null ? (feedOptions.PartitionKeyRangeId == null ? (PartitionKeyInfo) null : (PartitionKeyInfo) new PhysicalPartitionKeyRangeId(feedOptions.PartitionKeyRangeId)) : (PartitionKeyInfo) new LogicalPartitionKey(feedOptions.PartitionKey), token);
        else
          partitionedQueryExecutionInfo = QueryPlanRetriever.GetQueryPlanWithServiceInterop(await client.GetQueryPartitionProviderAsync(token), queryExecutionContext.QuerySpec, collection.PartitionKey, feedOptions.PartitionKey != null);
        if (DocumentQueryExecutionContextFactory.ShouldCreateSpecializedDocumentQueryExecutionContext(resourceTypeEnum, feedOptions, partitionedQueryExecutionInfo, collection.PartitionKey, isContinuationExpected))
        {
          List<PartitionKeyRange> targetRanges;
          if (!string.IsNullOrEmpty(feedOptions.PartitionKeyRangeId))
          {
            List<PartitionKeyRange> partitionKeyRangeList1 = new List<PartitionKeyRange>();
            List<PartitionKeyRange> partitionKeyRangeList2 = partitionKeyRangeList1;
            partitionKeyRangeList2.Add(await queryExecutionContext.GetTargetPartitionKeyRangeById(collection.ResourceId, feedOptions.PartitionKeyRangeId));
            targetRanges = partitionKeyRangeList1;
            partitionKeyRangeList2 = (List<PartitionKeyRange>) null;
            partitionKeyRangeList1 = (List<PartitionKeyRange>) null;
          }
          else
          {
            List<Range<string>> providedRanges = partitionedQueryExecutionInfo.QueryRanges;
            if (feedOptions.PartitionKey != null)
              providedRanges = new List<Range<string>>()
              {
                Range<string>.GetPointRange(feedOptions.PartitionKey.InternalKey.GetEffectivePartitionKeyString(collection.PartitionKey))
              };
            targetRanges = await queryExecutionContext.GetTargetPartitionKeyRanges(collection.ResourceId, providedRanges);
          }
          return await DocumentQueryExecutionContextFactory.CreateSpecializedDocumentQueryExecutionContext(constructorParams, partitionedQueryExecutionInfo, targetRanges, collection.ResourceId, isContinuationExpected, token);
        }
        partitionedQueryExecutionInfo = (PartitionedQueryExecutionInfo) null;
      }
      return (IDocumentQueryExecutionContext) queryExecutionContext;
    }

    public static Task<IDocumentQueryExecutionContext> CreateSpecializedDocumentQueryExecutionContext(
      DocumentQueryExecutionContextBase.InitParams constructorParams,
      PartitionedQueryExecutionInfo partitionedQueryExecutionInfo,
      List<PartitionKeyRange> targetRanges,
      string collectionRid,
      bool isContinuationExpected,
      CancellationToken cancellationToken)
    {
      long num1 = (long) constructorParams.FeedOptions.MaxItemCount.GetValueOrDefault(ParallelQueryConfig.GetConfig().ClientInternalPageSize);
      if (num1 < -1L || num1 == 0L)
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid MaxItemCount {0}", (object) num1));
      QueryInfo queryInfo = partitionedQueryExecutionInfo.QueryInfo;
      int num2 = queryInfo.HasTop ? 1 : 0;
      if (queryInfo.HasOrderBy)
      {
        if (queryInfo.HasTop)
        {
          int? top = partitionedQueryExecutionInfo.QueryInfo.Top;
          int val2;
          if ((val2 = top.Value) > 0)
          {
            long val1 = (long) Math.Min(Math.Ceiling((double) val2 / (double) targetRanges.Count) * 5.0, (double) val2);
            num1 = num1 <= 0L ? val1 : Math.Min(val1, num1);
            goto label_10;
          }
        }
        if (isContinuationExpected)
        {
          if (num1 < 0L)
            num1 = Math.Max((long) constructorParams.FeedOptions.MaxBufferedItemCount, ParallelQueryConfig.GetConfig().DefaultMaximumBufferSize);
          num1 = (long) Math.Min(Math.Ceiling((double) num1 / (double) targetRanges.Count) * 5.0, (double) num1);
        }
      }
label_10:
      return PipelinedDocumentQueryExecutionContext.CreateAsync(constructorParams, collectionRid, partitionedQueryExecutionInfo, targetRanges, (int) num1, constructorParams.FeedOptions.RequestContinuation, cancellationToken);
    }

    private static bool ShouldCreateSpecializedDocumentQueryExecutionContext(
      ResourceType resourceTypeEnum,
      FeedOptions feedOptions,
      PartitionedQueryExecutionInfo partitionedQueryExecutionInfo,
      PartitionKeyDefinition partitionKeyDefinition,
      bool isContinuationExpected)
    {
      return DocumentQueryExecutionContextFactory.IsCrossPartitionQuery(resourceTypeEnum, feedOptions, partitionKeyDefinition, partitionedQueryExecutionInfo) && (DocumentQueryExecutionContextFactory.IsTopOrderByQuery(partitionedQueryExecutionInfo) || DocumentQueryExecutionContextFactory.IsAggregateQuery(partitionedQueryExecutionInfo) || DocumentQueryExecutionContextFactory.IsOffsetLimitQuery(partitionedQueryExecutionInfo) || DocumentQueryExecutionContextFactory.IsParallelQuery(feedOptions)) || !string.IsNullOrEmpty(feedOptions.PartitionKeyRangeId) || DocumentQueryExecutionContextFactory.IsAggregateQueryWithoutContinuation(partitionedQueryExecutionInfo, isContinuationExpected) || DocumentQueryExecutionContextFactory.IsDistinctQuery(partitionedQueryExecutionInfo) || DocumentQueryExecutionContextFactory.IsGroupByQuery(partitionedQueryExecutionInfo);
    }

    private static bool IsCrossPartitionQuery(
      ResourceType resourceTypeEnum,
      FeedOptions feedOptions,
      PartitionKeyDefinition partitionKeyDefinition,
      PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      if (!resourceTypeEnum.IsPartitioned() || feedOptions.PartitionKey != null || !feedOptions.EnableCrossPartitionQuery || partitionKeyDefinition.Paths.Count <= 0)
        return false;
      return partitionedQueryExecutionInfo.QueryRanges.Count != 1 || !partitionedQueryExecutionInfo.QueryRanges[0].IsSingleValue;
    }

    private static bool IsTopOrderByQuery(
      PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      if (partitionedQueryExecutionInfo.QueryInfo == null)
        return false;
      return partitionedQueryExecutionInfo.QueryInfo.HasOrderBy || partitionedQueryExecutionInfo.QueryInfo.HasTop;
    }

    private static bool IsAggregateQuery(
      PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      return partitionedQueryExecutionInfo.QueryInfo != null && partitionedQueryExecutionInfo.QueryInfo.HasAggregates;
    }

    private static bool IsAggregateQueryWithoutContinuation(
      PartitionedQueryExecutionInfo partitionedQueryExecutionInfo,
      bool isContinuationExpected)
    {
      return DocumentQueryExecutionContextFactory.IsAggregateQuery(partitionedQueryExecutionInfo) && !isContinuationExpected;
    }

    private static bool IsDistinctQuery(
      PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      return partitionedQueryExecutionInfo.QueryInfo.HasDistinct;
    }

    private static bool IsParallelQuery(FeedOptions feedOptions) => feedOptions.MaxDegreeOfParallelism != 0;

    private static bool IsOffsetLimitQuery(
      PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      return partitionedQueryExecutionInfo.QueryInfo.HasOffset && partitionedQueryExecutionInfo.QueryInfo.HasLimit;
    }

    private static bool IsGroupByQuery(
      PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
    {
      return partitionedQueryExecutionInfo.QueryInfo.HasGroupBy;
    }
  }
}
