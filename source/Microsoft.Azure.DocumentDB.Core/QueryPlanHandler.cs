// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.QueryPlanHandler
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Query;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Documents
{
  internal sealed class QueryPlanHandler
  {
    private readonly QueryPartitionProvider queryPartitionProvider;

    public QueryPlanHandler(QueryPartitionProvider queryPartitionProvider) => this.queryPartitionProvider = queryPartitionProvider != null ? queryPartitionProvider : throw new ArgumentNullException(nameof (queryPartitionProvider));

    public Microsoft.Azure.Documents.Query.PartitionedQueryExecutionInfo GetQueryPlan(
      SqlQuerySpec sqlQuerySpec,
      PartitionKeyDefinition partitionKeyDefinition,
      bool hasLogicalPartitionKey,
      QueryFeatures supportedQueryFeatures)
    {
      if (sqlQuerySpec == null)
        throw new ArgumentNullException(nameof (sqlQuerySpec));
      if (partitionKeyDefinition == null)
        throw new ArgumentNullException(nameof (partitionKeyDefinition));
      Microsoft.Azure.Documents.Query.PartitionedQueryExecutionInfo queryExecutionInfo = this.queryPartitionProvider.GetPartitionedQueryExecutionInfo(sqlQuerySpec, partitionKeyDefinition, true, false, true, hasLogicalPartitionKey);
      if (queryExecutionInfo == null || queryExecutionInfo.QueryRanges == null || queryExecutionInfo.QueryInfo == null || queryExecutionInfo.QueryRanges.Any<Range<string>>((Func<Range<string>, bool>) (range => range.Min == null || range.Max == null)))
        throw new InvalidOperationException("partitionedQueryExecutionInfo has invalid properties");
      QueryPlanHandler.QueryPlanExceptionFactory.ThrowIfNotSupported(queryExecutionInfo.QueryInfo, supportedQueryFeatures);
      return queryExecutionInfo;
    }

    private static class QueryPlanExceptionFactory
    {
      private static readonly ArgumentException QueryContainsUnsupportedAggregates = new ArgumentException(QueryPlanHandler.QueryPlanExceptionFactory.FormatExceptionMessage("Aggregate"));
      private static readonly ArgumentException QueryContainsUnsupportedCompositeAggregate = new ArgumentException(QueryPlanHandler.QueryPlanExceptionFactory.FormatExceptionMessage("CompositeAggregate"));
      private static readonly ArgumentException QueryContainsUnsupportedGroupBy = new ArgumentException(QueryPlanHandler.QueryPlanExceptionFactory.FormatExceptionMessage("GroupBy"));
      private static readonly ArgumentException QueryContainsUnsupportedMultipleAggregates = new ArgumentException(QueryPlanHandler.QueryPlanExceptionFactory.FormatExceptionMessage("MultipleAggregates"));
      private static readonly ArgumentException QueryContainsUnsupportedDistinct = new ArgumentException(QueryPlanHandler.QueryPlanExceptionFactory.FormatExceptionMessage("Distinct"));
      private static readonly ArgumentException QueryContainsUnsupportedOffsetAndLimit = new ArgumentException(QueryPlanHandler.QueryPlanExceptionFactory.FormatExceptionMessage("OffsetAndLimit"));
      private static readonly ArgumentException QueryContainsUnsupportedOrderBy = new ArgumentException(QueryPlanHandler.QueryPlanExceptionFactory.FormatExceptionMessage("OrderBy"));
      private static readonly ArgumentException QueryContainsUnsupportedMultipleOrderBy = new ArgumentException(QueryPlanHandler.QueryPlanExceptionFactory.FormatExceptionMessage("MultipleOrderBy"));
      private static readonly ArgumentException QueryContainsUnsupportedTop = new ArgumentException(QueryPlanHandler.QueryPlanExceptionFactory.FormatExceptionMessage("Top"));
      private static readonly ArgumentException QueryContainsUnsupportedNonValueAggregate = new ArgumentException(QueryPlanHandler.QueryPlanExceptionFactory.FormatExceptionMessage("NonValueAggregate"));

      public static void ThrowIfNotSupported(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures)
      {
        Lazy<List<Exception>> exceptions = new Lazy<List<Exception>>((Func<List<Exception>>) (() => new List<Exception>()));
        QueryPlanHandler.QueryPlanExceptionFactory.AddExceptionsForAggregateQueries(queryInfo, supportedQueryFeatures, exceptions);
        QueryPlanHandler.QueryPlanExceptionFactory.AddExceptionsForDistinctQueries(queryInfo, supportedQueryFeatures, exceptions);
        QueryPlanHandler.QueryPlanExceptionFactory.AddExceptionForGroupByQueries(queryInfo, supportedQueryFeatures, exceptions);
        QueryPlanHandler.QueryPlanExceptionFactory.AddExceptionsForTopQueries(queryInfo, supportedQueryFeatures, exceptions);
        QueryPlanHandler.QueryPlanExceptionFactory.AddExceptionsForOrderByQueries(queryInfo, supportedQueryFeatures, exceptions);
        QueryPlanHandler.QueryPlanExceptionFactory.AddExceptionsForOffsetLimitQueries(queryInfo, supportedQueryFeatures, exceptions);
        if (exceptions.IsValueCreated)
          throw new QueryPlanHandler.QueryPlanHandlerException((IEnumerable<Exception>) exceptions.Value);
      }

      private static void AddExceptionsForAggregateQueries(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures,
        Lazy<List<Exception>> exceptions)
      {
        if (!queryInfo.HasAggregates)
          return;
        if ((queryInfo.Aggregates.Length == 1 ? 1 : (queryInfo.GroupByAliasToAggregateType.Values.Where<AggregateOperator?>((Func<AggregateOperator?, bool>) (aggregateOperator => aggregateOperator.HasValue)).Count<AggregateOperator?>() == 1 ? 1 : 0)) != 0)
        {
          if (queryInfo.HasSelectValue)
          {
            if (supportedQueryFeatures.HasFlag((Enum) QueryFeatures.Aggregate))
              return;
            exceptions.Value.Add((Exception) QueryPlanHandler.QueryPlanExceptionFactory.QueryContainsUnsupportedAggregates);
          }
          else
          {
            if (supportedQueryFeatures.HasFlag((Enum) QueryFeatures.NonValueAggregate))
              return;
            exceptions.Value.Add((Exception) QueryPlanHandler.QueryPlanExceptionFactory.QueryContainsUnsupportedNonValueAggregate);
          }
        }
        else
        {
          if (!supportedQueryFeatures.HasFlag((Enum) QueryFeatures.NonValueAggregate))
            exceptions.Value.Add((Exception) QueryPlanHandler.QueryPlanExceptionFactory.QueryContainsUnsupportedNonValueAggregate);
          if (supportedQueryFeatures.HasFlag((Enum) QueryFeatures.MultipleAggregates))
            return;
          exceptions.Value.Add((Exception) QueryPlanHandler.QueryPlanExceptionFactory.QueryContainsUnsupportedMultipleAggregates);
        }
      }

      private static void AddExceptionsForDistinctQueries(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures,
        Lazy<List<Exception>> exceptions)
      {
        if (!queryInfo.HasDistinct || supportedQueryFeatures.HasFlag((Enum) QueryFeatures.Distinct))
          return;
        exceptions.Value.Add((Exception) QueryPlanHandler.QueryPlanExceptionFactory.QueryContainsUnsupportedDistinct);
      }

      private static void AddExceptionsForOffsetLimitQueries(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures,
        Lazy<List<Exception>> exceptions)
      {
        if (!queryInfo.HasLimit && !queryInfo.HasOffset || supportedQueryFeatures.HasFlag((Enum) QueryFeatures.OffsetAndLimit))
          return;
        exceptions.Value.Add((Exception) QueryPlanHandler.QueryPlanExceptionFactory.QueryContainsUnsupportedOffsetAndLimit);
      }

      private static void AddExceptionsForOrderByQueries(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures,
        Lazy<List<Exception>> exceptions)
      {
        if (!queryInfo.HasOrderBy)
          return;
        if (queryInfo.OrderByExpressions.Length == 1)
        {
          if (supportedQueryFeatures.HasFlag((Enum) QueryFeatures.OrderBy))
            return;
          exceptions.Value.Add((Exception) QueryPlanHandler.QueryPlanExceptionFactory.QueryContainsUnsupportedOrderBy);
        }
        else
        {
          if (supportedQueryFeatures.HasFlag((Enum) QueryFeatures.MultipleOrderBy))
            return;
          exceptions.Value.Add((Exception) QueryPlanHandler.QueryPlanExceptionFactory.QueryContainsUnsupportedMultipleOrderBy);
        }
      }

      private static void AddExceptionForGroupByQueries(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures,
        Lazy<List<Exception>> exceptions)
      {
        if (!queryInfo.HasGroupBy || supportedQueryFeatures.HasFlag((Enum) QueryFeatures.GroupBy))
          return;
        exceptions.Value.Add((Exception) QueryPlanHandler.QueryPlanExceptionFactory.QueryContainsUnsupportedGroupBy);
      }

      private static void AddExceptionsForTopQueries(
        QueryInfo queryInfo,
        QueryFeatures supportedQueryFeatures,
        Lazy<List<Exception>> exceptions)
      {
        if (!queryInfo.HasTop || supportedQueryFeatures.HasFlag((Enum) QueryFeatures.Top))
          return;
        exceptions.Value.Add((Exception) QueryPlanHandler.QueryPlanExceptionFactory.QueryContainsUnsupportedTop);
      }

      private static string FormatExceptionMessage(string feature) => "Query contained " + feature + ", which the calling client does not support.";
    }

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
