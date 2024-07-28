// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.DocumentQueryable
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Query;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Linq
{
  public static class DocumentQueryable
  {
    public static IDocumentQuery<T> AsDocumentQuery<T>(this IQueryable<T> query) => (IDocumentQuery<T>) query;

    public static Task<TSource> MaxAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<TSource>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<TSource>, TSource>(new Func<IQueryable<TSource>, TSource>(Queryable.Max<TSource>)), source.Expression), cancellationToken);
    }

    public static Task<TSource> MinAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<TSource>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<TSource>, TSource>(new Func<IQueryable<TSource>, TSource>(Queryable.Min<TSource>)), source.Expression), cancellationToken);
    }

    public static Task<Decimal> AverageAsync(
      this IQueryable<Decimal> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<Decimal>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<Decimal>, Decimal>(new Func<IQueryable<Decimal>, Decimal>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<Decimal?> AverageAsync(
      this IQueryable<Decimal?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<Decimal?>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<Decimal?>, Decimal?>(new Func<IQueryable<Decimal?>, Decimal?>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<double> AverageAsync(
      this IQueryable<double> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<double>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<double>, double>(new Func<IQueryable<double>, double>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<double?> AverageAsync(
      this IQueryable<double?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<double?>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<double?>, double?>(new Func<IQueryable<double?>, double?>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<float> AverageAsync(
      this IQueryable<float> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<float>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<float>, float>(new Func<IQueryable<float>, float>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<float?> AverageAsync(
      this IQueryable<float?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<float?>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<float?>, float?>(new Func<IQueryable<float?>, float?>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<double> AverageAsync(
      this IQueryable<int> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<double>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<int>, double>(new Func<IQueryable<int>, double>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<double?> AverageAsync(
      this IQueryable<int?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<double?>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<int?>, double?>(new Func<IQueryable<int?>, double?>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<double> AverageAsync(
      this IQueryable<long> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<double>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<long>, double>(new Func<IQueryable<long>, double>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<double?> AverageAsync(
      this IQueryable<long?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<double?>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<long?>, double?>(new Func<IQueryable<long?>, double?>(Queryable.Average)), source.Expression), cancellationToken);
    }

    public static Task<Decimal> SumAsync(
      this IQueryable<Decimal> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<Decimal>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<Decimal>, Decimal>(new Func<IQueryable<Decimal>, Decimal>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<Decimal?> SumAsync(
      this IQueryable<Decimal?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<Decimal?>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<Decimal?>, Decimal?>(new Func<IQueryable<Decimal?>, Decimal?>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<double> SumAsync(
      this IQueryable<double> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<double>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<double>, double>(new Func<IQueryable<double>, double>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<double?> SumAsync(
      this IQueryable<double?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<double?>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<double?>, double?>(new Func<IQueryable<double?>, double?>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<float> SumAsync(
      this IQueryable<float> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<float>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<float>, float>(new Func<IQueryable<float>, float>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<float?> SumAsync(
      this IQueryable<float?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<float?>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<float?>, float?>(new Func<IQueryable<float?>, float?>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<int> SumAsync(
      this IQueryable<int> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<int>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<int>, int>(new Func<IQueryable<int>, int>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<int?> SumAsync(
      this IQueryable<int?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<int?>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<int?>, int?>(new Func<IQueryable<int?>, int?>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<long> SumAsync(
      this IQueryable<long> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<long>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<long>, long>(new Func<IQueryable<long>, long>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<long?> SumAsync(
      this IQueryable<long?> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<long?>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<long?>, long?>(new Func<IQueryable<long?>, long?>(Queryable.Sum)), source.Expression), cancellationToken);
    }

    public static Task<int> CountAsync<TSource>(
      this IQueryable<TSource> source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return ((IDocumentQueryProvider) source.Provider).ExecuteAsync<int>((Expression) Expression.Call(DocumentQueryable.GetMethodInfoOf<IQueryable<TSource>, int>(new Func<IQueryable<TSource>, int>(Queryable.Count<TSource>)), source.Expression), cancellationToken);
    }

    internal static IQueryable<TResult> AsSQL<TSource, TResult>(
      this IOrderedQueryable<TSource> source,
      SqlQuerySpec querySpec)
    {
      if (querySpec == null)
        throw new ArgumentNullException(nameof (querySpec));
      if (string.IsNullOrEmpty(querySpec.QueryText))
        throw new ArgumentException("querySpec.QueryText");
      return source.Provider.CreateQuery<TResult>((Expression) Expression.Call((Expression) null, DocumentQueryable.GetMethodInfoOf<IQueryable<object>>((Expression<Func<IQueryable<object>>>) (() => default (IOrderedQueryable<TSource>).AsSQL<TSource>(default (SqlQuerySpec)))), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Constant((object) querySpec)
      }));
    }

    internal static IQueryable<object> AsSQL<TSource>(
      this IOrderedQueryable<TSource> source,
      SqlQuerySpec querySpec)
    {
      if (querySpec == null)
        throw new ArgumentNullException(nameof (querySpec));
      if (string.IsNullOrEmpty(querySpec.QueryText))
        throw new ArgumentException("querySpec.QueryText");
      return source.Provider.CreateQuery<object>((Expression) Expression.Call((Expression) null, DocumentQueryable.GetMethodInfoOf<IQueryable<object>>((Expression<Func<IQueryable<object>>>) (() => default (IOrderedQueryable<TSource>).AsSQL<TSource>(default (SqlQuerySpec)))), new Expression[2]
      {
        source.Expression,
        (Expression) Expression.Constant((object) querySpec)
      }));
    }

    internal static IOrderedQueryable<Document> CreateDocumentQuery(
      this IDocumentQueryClient client,
      string collectionLink,
      FeedOptions feedOptions = null,
      object partitionKey = null)
    {
      return (IOrderedQueryable<Document>) new DocumentQuery<Document>(client, ResourceType.Document, typeof (Document), collectionLink, feedOptions, partitionKey);
    }

    internal static IQueryable<object> CreateDocumentQuery(
      this IDocumentQueryClient client,
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null,
      object partitionKey = null)
    {
      return new DocumentQuery<Document>(client, ResourceType.Document, typeof (Document), collectionLink, feedOptions, partitionKey).AsSQL<Document>(querySpec);
    }

    private static MethodInfo GetMethodInfoOf<T>(Expression<Func<T>> expression) => ((MethodCallExpression) expression.Body).Method;

    private static MethodInfo GetMethodInfoOf<T1, T2>(Func<T1, T2> func) => func.GetMethodInfo();
  }
}
