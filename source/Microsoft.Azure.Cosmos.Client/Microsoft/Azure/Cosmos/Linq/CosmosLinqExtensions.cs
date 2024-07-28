// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.CosmosLinqExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Diagnostics;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Linq
{
  public static class CosmosLinqExtensions
  {
    public static bool IsDefined(this object obj) => throw new NotImplementedException(ClientResources.TypeCheckExtensionFunctionsNotImplemented);

    public static bool IsNull(this object obj) => throw new NotImplementedException(ClientResources.TypeCheckExtensionFunctionsNotImplemented);

    public static bool IsPrimitive(this object obj) => throw new NotImplementedException(ClientResources.TypeCheckExtensionFunctionsNotImplemented);

    internal static QueryDefinition ToQueryDefinition<T>(
      this IQueryable<T> query,
      IDictionary<object, string> namedParameters)
    {
      if (namedParameters == null)
        throw new ArgumentException("namedParameters dictionary cannot be empty for this overload, please use ToQueryDefinition<T>(IQueryable<T> query) instead", nameof (namedParameters));
      return query is CosmosLinqQuery<T> cosmosLinqQuery ? cosmosLinqQuery.ToQueryDefinition(namedParameters) : throw new ArgumentException("ToQueryDefinition is only supported on Cosmos LINQ query operations", nameof (query));
    }

    public static QueryDefinition ToQueryDefinition<T>(this IQueryable<T> query) => query is CosmosLinqQuery<T> cosmosLinqQuery ? cosmosLinqQuery.ToQueryDefinition() : throw new ArgumentException("ToQueryDefinition is only supported on Cosmos LINQ query operations", nameof (query));

    public static FeedIterator<T> ToFeedIterator<T>(this IQueryable<T> query) => query is CosmosLinqQuery<T> cosmosLinqQuery ? cosmosLinqQuery.ToFeedIterator() : throw new ArgumentOutOfRangeException("linqQuery", "ToFeedIterator is only supported on Cosmos LINQ query operations");

    public static FeedIterator ToStreamIterator<T>(this IQueryable<T> query) => query is CosmosLinqQuery<T> cosmosLinqQuery ? cosmosLinqQuery.ToStreamIterator() : throw new ArgumentOutOfRangeException("linqQuery", "ToStreamFeedIterator is only supported on cosmos LINQ query operations");

    public static Task<Response<TSource>> MaxAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<TSource>(Queryable.Max<TSource>(source)) : provider.ExecuteAggregateAsync<TSource>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<TSource>, TSource>(new Func<IQueryable<TSource>, TSource>(Queryable.Max<TSource>)), source.Expression), cancellationToken);
    }

    public static Task<Response<TSource>> MinAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<TSource>(Queryable.Min<TSource>(source)) : provider.ExecuteAggregateAsync<TSource>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<TSource>, TSource>(new Func<IQueryable<TSource>, TSource>(Queryable.Min<TSource>)), source.Expression), cancellationToken);
    }

    public static Task<Response<Decimal>> AverageAsync(
      this IQueryable<Decimal> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<Decimal>(Queryable.Average(source)) : provider.ExecuteAggregateAsync<Decimal>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<Decimal>, Decimal>(new Func<IQueryable<Decimal>, Decimal>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<Response<Decimal?>> AverageAsync(
      this IQueryable<Decimal?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<Decimal?>(Queryable.Average(source)) : provider.ExecuteAggregateAsync<Decimal?>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<Decimal?>, Decimal?>(new Func<IQueryable<Decimal?>, Decimal?>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<Response<double>> AverageAsync(
      this IQueryable<double> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<double>(Queryable.Average(source)) : provider.ExecuteAggregateAsync<double>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<double>, double>(new Func<IQueryable<double>, double>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<Response<double?>> AverageAsync(
      this IQueryable<double?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<double?>(Queryable.Average(source)) : provider.ExecuteAggregateAsync<double?>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<double?>, double?>(new Func<IQueryable<double?>, double?>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<Response<float>> AverageAsync(
      this IQueryable<float> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<float>(Queryable.Average(source)) : provider.ExecuteAggregateAsync<float>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<float>, float>(new Func<IQueryable<float>, float>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<Response<float?>> AverageAsync(
      this IQueryable<float?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<float?>(Queryable.Average(source)) : provider.ExecuteAggregateAsync<float?>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<float?>, float?>(new Func<IQueryable<float?>, float?>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<Response<double>> AverageAsync(
      this IQueryable<int> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<double>(Queryable.Average(source)) : provider.ExecuteAggregateAsync<double>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<int>, double>(new Func<IQueryable<int>, double>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<Response<double?>> AverageAsync(
      this IQueryable<int?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<double?>(Queryable.Average(source)) : provider.ExecuteAggregateAsync<double?>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<int?>, double?>(new Func<IQueryable<int?>, double?>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<Response<double>> AverageAsync(
      this IQueryable<long> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<double>(Queryable.Average(source)) : provider.ExecuteAggregateAsync<double>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<long>, double>(new Func<IQueryable<long>, double>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<Response<double?>> AverageAsync(
      this IQueryable<long?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<double?>(Queryable.Average(source)) : provider.ExecuteAggregateAsync<double?>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<long?>, double?>(new Func<IQueryable<long?>, double?>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<Response<Decimal>> SumAsync(
      this IQueryable<Decimal> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<Decimal>(Queryable.Sum(source)) : provider.ExecuteAggregateAsync<Decimal>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<Decimal>, Decimal>(new Func<IQueryable<Decimal>, Decimal>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<Response<Decimal?>> SumAsync(
      this IQueryable<Decimal?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<Decimal?>(Queryable.Sum(source)) : provider.ExecuteAggregateAsync<Decimal?>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<Decimal?>, Decimal?>(new Func<IQueryable<Decimal?>, Decimal?>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<Response<double>> SumAsync(
      this IQueryable<double> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<double>(Queryable.Sum(source)) : provider.ExecuteAggregateAsync<double>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<double>, double>(new Func<IQueryable<double>, double>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<Response<double?>> SumAsync(
      this IQueryable<double?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<double?>(Queryable.Sum(source)) : provider.ExecuteAggregateAsync<double?>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<double?>, double?>(new Func<IQueryable<double?>, double?>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<Response<float>> SumAsync(
      this IQueryable<float> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<float>(Queryable.Sum(source)) : provider.ExecuteAggregateAsync<float>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<float>, float>(new Func<IQueryable<float>, float>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<Response<float?>> SumAsync(
      this IQueryable<float?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<float?>(Queryable.Sum(source)) : provider.ExecuteAggregateAsync<float?>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<float?>, float?>(new Func<IQueryable<float?>, float?>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<Response<int>> SumAsync(
      this IQueryable<int> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<int>(Queryable.Sum(source)) : provider.ExecuteAggregateAsync<int>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<int>, int>(new Func<IQueryable<int>, int>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<Response<int?>> SumAsync(
      this IQueryable<int?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider) ? CosmosLinqExtensions.ResponseHelperAsync<int?>(Queryable.Sum(source)) : ((CosmosLinqQueryProvider) source.Provider).ExecuteAggregateAsync<int?>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<int?>, int?>(new Func<IQueryable<int?>, int?>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<Response<long>> SumAsync(
      this IQueryable<long> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<long>(Queryable.Sum(source)) : provider.ExecuteAggregateAsync<long>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<long>, long>(new Func<IQueryable<long>, long>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<Response<long?>> SumAsync(
      this IQueryable<long?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<long?>(Queryable.Sum(source)) : provider.ExecuteAggregateAsync<long?>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<long?>, long?>(new Func<IQueryable<long?>, long?>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<Response<int>> CountAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return !(source.Provider is CosmosLinqQueryProvider provider) ? CosmosLinqExtensions.ResponseHelperAsync<int>(Queryable.Count<TSource>(source)) : provider.ExecuteAggregateAsync<int>((Expression) Expression.Call(CosmosLinqExtensions.GetMethodInfoOf<IQueryable<TSource>, int>(new Func<IQueryable<TSource>, int>(Queryable.Count<TSource>)), source.Expression), cancellationToken);
    }

    private static Task<Response<T>> ResponseHelperAsync<T>(T value) => Task.FromResult<Response<T>>((Response<T>) new ItemResponse<T>(HttpStatusCode.OK, new Microsoft.Azure.Cosmos.Headers(), value, (CosmosDiagnostics) new CosmosTraceDiagnostics((ITrace) NoOpTrace.Singleton), (RequestMessage) null));

    private static MethodInfo GetMethodInfoOf<T1, T2>(Func<T1, T2> func) => func.GetMethodInfo();
  }
}
