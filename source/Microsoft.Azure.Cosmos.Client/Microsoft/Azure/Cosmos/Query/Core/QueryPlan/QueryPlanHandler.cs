// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.QueryPlan.QueryPlanHandler
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.QueryPlan
{
  internal sealed class QueryPlanHandler
  {
    private readonly CosmosQueryClient queryClient;

    public QueryPlanHandler(CosmosQueryClient queryClient) => this.queryClient = queryClient ?? throw new ArgumentNullException(nameof (queryClient));

    public async Task<TryCatch<PartitionedQueryExecutionInfo>> TryGetQueryPlanAsync(
      SqlQuerySpec sqlQuerySpec,
      ResourceType resourceType,
      PartitionKeyDefinition partitionKeyDefinition,
      QueryFeatures supportedQueryFeatures,
      bool hasLogicalPartitionKey,
      bool useSystemPrefix,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (sqlQuerySpec == null)
        throw new ArgumentNullException(nameof (sqlQuerySpec));
      if (partitionKeyDefinition == null)
        throw new ArgumentNullException(nameof (partitionKeyDefinition));
      TryCatch<PartitionedQueryExecutionInfo> queryInfoAsync = await this.TryGetQueryInfoAsync(sqlQuerySpec, resourceType, partitionKeyDefinition, hasLogicalPartitionKey, useSystemPrefix, cancellationToken);
      Exception queryPlanHandlerException;
      return queryInfoAsync.Succeeded ? (!QueryPlanHandler.QueryPlanExceptionFactory.TryGetUnsupportedException(queryInfoAsync.Result.QueryInfo, supportedQueryFeatures, out queryPlanHandlerException) ? queryInfoAsync : TryCatch<PartitionedQueryExecutionInfo>.FromException(queryPlanHandlerException)) : queryInfoAsync;
    }

    public async Task<TryCatch<(PartitionedQueryExecutionInfo queryPlan, bool supported)>> TryGetQueryInfoAndIfSupportedAsync(
      QueryFeatures supportedQueryFeatures,
      SqlQuerySpec sqlQuerySpec,
      ResourceType resourceType,
      PartitionKeyDefinition partitionKeyDefinition,
      bool hasLogicalPartitionKey,
      bool useSystemPrefix,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (sqlQuerySpec == null)
        throw new ArgumentNullException(nameof (sqlQuerySpec));
      if (partitionKeyDefinition == null)
        throw new ArgumentNullException(nameof (partitionKeyDefinition));
      cancellationToken.ThrowIfCancellationRequested();
      TryCatch<PartitionedQueryExecutionInfo> queryInfoAsync = await this.TryGetQueryInfoAsync(sqlQuerySpec, resourceType, partitionKeyDefinition, hasLogicalPartitionKey, useSystemPrefix, cancellationToken);
      if (queryInfoAsync.Failed)
        return TryCatch<(PartitionedQueryExecutionInfo, bool)>.FromException(queryInfoAsync.Exception);
      QueryFeatures neededQueryFeatures = QueryPlanHandler.QueryPlanSupportChecker.GetNeededQueryFeatures(queryInfoAsync.Result.QueryInfo, supportedQueryFeatures);
      return TryCatch<(PartitionedQueryExecutionInfo, bool)>.FromResult((queryInfoAsync.Result, neededQueryFeatures == QueryFeatures.None));
    }

    private Task<TryCatch<PartitionedQueryExecutionInfo>> TryGetQueryInfoAsync(
      SqlQuerySpec sqlQuerySpec,
      ResourceType resourceType,
      PartitionKeyDefinition partitionKeyDefinition,
      bool hasLogicalPartitionKey,
      bool useSystemPrefix,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      return this.queryClient.TryGetPartitionedQueryExecutionInfoAsync(sqlQuerySpec, resourceType, partitionKeyDefinition, true, false, true, hasLogicalPartitionKey, true, useSystemPrefix, cancellationToken);
    }

    private static class QueryPlanSupportChecker
    {
      public static QueryFeatures GetNeededQueryFeatures(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures)
      {
        return QueryFeatures.None | QueryPlanHandler.QueryPlanSupportChecker.GetNeededQueryFeaturesIfAggregateQuery(queryInfo, supportedQueryFeatures) | QueryPlanHandler.QueryPlanSupportChecker.GetNeededQueryFeaturesIfDistinctQuery(queryInfo, supportedQueryFeatures) | QueryPlanHandler.QueryPlanSupportChecker.GetNeedQueryFeaturesIfGroupByQuery(queryInfo, supportedQueryFeatures) | QueryPlanHandler.QueryPlanSupportChecker.GetNeededQueryFeaturesIfOffsetLimitQuery(queryInfo, supportedQueryFeatures) | QueryPlanHandler.QueryPlanSupportChecker.GetNeededQueryFeaturesIfOrderByQuery(queryInfo, supportedQueryFeatures) | QueryPlanHandler.QueryPlanSupportChecker.GetNeededQueryFeaturesIfTopQuery(queryInfo, supportedQueryFeatures);
      }

      private static QueryFeatures GetNeededQueryFeaturesIfAggregateQuery(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures)
      {
        QueryFeatures ifAggregateQuery = QueryFeatures.None;
        if (queryInfo.HasAggregates)
        {
          if ((queryInfo.Aggregates.Count == 1 ? 1 : (queryInfo.GroupByAliasToAggregateType.Values.Where<AggregateOperator?>((Func<AggregateOperator?, bool>) (aggregateOperator => aggregateOperator.HasValue)).Count<AggregateOperator?>() == 1 ? 1 : 0)) != 0)
          {
            if (queryInfo.HasSelectValue)
            {
              if (!supportedQueryFeatures.HasFlag((Enum) QueryFeatures.Aggregate))
                ifAggregateQuery |= QueryFeatures.Aggregate;
            }
            else if (!supportedQueryFeatures.HasFlag((Enum) QueryFeatures.NonValueAggregate))
              ifAggregateQuery |= QueryFeatures.NonValueAggregate;
          }
          else
          {
            if (!supportedQueryFeatures.HasFlag((Enum) QueryFeatures.NonValueAggregate))
              ifAggregateQuery |= QueryFeatures.NonValueAggregate;
            if (!supportedQueryFeatures.HasFlag((Enum) QueryFeatures.MultipleAggregates))
              ifAggregateQuery |= QueryFeatures.MultipleAggregates;
          }
        }
        return ifAggregateQuery;
      }

      private static QueryFeatures GetNeededQueryFeaturesIfDistinctQuery(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures)
      {
        QueryFeatures featuresIfDistinctQuery = QueryFeatures.None;
        if (queryInfo.HasDistinct && !supportedQueryFeatures.HasFlag((Enum) QueryFeatures.Distinct))
          featuresIfDistinctQuery |= QueryFeatures.Distinct;
        return featuresIfDistinctQuery;
      }

      private static QueryFeatures GetNeededQueryFeaturesIfTopQuery(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures)
      {
        QueryFeatures featuresIfTopQuery = QueryFeatures.None;
        if (queryInfo.HasTop && !supportedQueryFeatures.HasFlag((Enum) QueryFeatures.Top))
          featuresIfTopQuery |= QueryFeatures.Top;
        return featuresIfTopQuery;
      }

      private static QueryFeatures GetNeededQueryFeaturesIfOffsetLimitQuery(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures)
      {
        QueryFeatures offsetLimitQuery = QueryFeatures.None;
        if ((queryInfo.HasLimit || queryInfo.HasOffset) && !supportedQueryFeatures.HasFlag((Enum) QueryFeatures.OffsetAndLimit))
          offsetLimitQuery |= QueryFeatures.OffsetAndLimit;
        return offsetLimitQuery;
      }

      private static QueryFeatures GetNeedQueryFeaturesIfGroupByQuery(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures)
      {
        QueryFeatures featuresIfGroupByQuery = QueryFeatures.None;
        if (queryInfo.HasGroupBy && !supportedQueryFeatures.HasFlag((Enum) QueryFeatures.GroupBy))
          featuresIfGroupByQuery |= QueryFeatures.GroupBy;
        return featuresIfGroupByQuery;
      }

      private static QueryFeatures GetNeededQueryFeaturesIfOrderByQuery(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures)
      {
        QueryFeatures featuresIfOrderByQuery = QueryFeatures.None;
        if (queryInfo.HasOrderBy)
        {
          if (queryInfo.OrderByExpressions.Count == 1)
          {
            if (!supportedQueryFeatures.HasFlag((Enum) QueryFeatures.OrderBy))
              featuresIfOrderByQuery |= QueryFeatures.OrderBy;
          }
          else if (!supportedQueryFeatures.HasFlag((Enum) QueryFeatures.MultipleOrderBy))
            featuresIfOrderByQuery |= QueryFeatures.MultipleOrderBy;
        }
        return featuresIfOrderByQuery;
      }
    }

    private static class QueryPlanExceptionFactory
    {
      private static readonly IReadOnlyList<QueryFeatures> QueryFeatureList = (IReadOnlyList<QueryFeatures>) Enum.GetValues(typeof (QueryFeatures));
      private static readonly ReadOnlyDictionary<QueryFeatures, ArgumentException> FeatureToUnsupportedException = new ReadOnlyDictionary<QueryFeatures, ArgumentException>((IDictionary<QueryFeatures, ArgumentException>) QueryPlanHandler.QueryPlanExceptionFactory.QueryFeatureList.ToDictionary<QueryFeatures, QueryFeatures, ArgumentException>((Func<QueryFeatures, QueryFeatures>) (x => x), (Func<QueryFeatures, ArgumentException>) (x => new ArgumentException(QueryPlanHandler.QueryPlanExceptionFactory.FormatExceptionMessage(x.ToString())))));

      public static bool TryGetUnsupportedException(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures,
        out Exception queryPlanHandlerException)
      {
        QueryFeatures neededQueryFeatures = QueryPlanHandler.QueryPlanSupportChecker.GetNeededQueryFeatures(queryInfo, supportedQueryFeatures);
        if (neededQueryFeatures != QueryFeatures.None)
        {
          List<Exception> innerExceptions = new List<Exception>();
          foreach (QueryFeatures queryFeature in (IEnumerable<QueryFeatures>) QueryPlanHandler.QueryPlanExceptionFactory.QueryFeatureList)
          {
            if ((neededQueryFeatures & queryFeature) == queryFeature)
            {
              Exception exception = (Exception) QueryPlanHandler.QueryPlanExceptionFactory.FeatureToUnsupportedException[queryFeature];
              innerExceptions.Add(exception);
            }
          }
          queryPlanHandlerException = (Exception) new QueryPlanHandler.QueryPlanExceptionFactory.QueryPlanHandlerException((IEnumerable<Exception>) innerExceptions);
          return true;
        }
        queryPlanHandlerException = (Exception) null;
        return false;
      }

      private static string FormatExceptionMessage(string feature) => "Query contained " + feature + ", which the calling client does not support.";

      private sealed class QueryPlanHandlerException : AggregateException
      {
        private const string QueryContainsUnsupportedFeaturesExceptionMessage = "Query contains 1 or more unsupported features. Upgrade your SDK to a version that does support the requested features:";

        public QueryPlanHandlerException(IEnumerable<Exception> innerExceptions)
          : base("Query contains 1 or more unsupported features. Upgrade your SDK to a version that does support the requested features:" + Environment.NewLine + string.Join(Environment.NewLine, innerExceptions.Select<Exception, string>((Func<Exception, string>) (innerException => innerException.Message))), innerExceptions)
        {
        }
      }
    }
  }
}
