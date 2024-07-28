// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.DocumentQueryable
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Documents;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal static class DocumentQueryable
  {
    public static IDocumentQuery<T> AsDocumentQuery<T>(this IQueryable<T> query) => (IDocumentQuery<T>) query;

    internal static IQueryable<TResult> AsSQL<TSource, TResult>(
      this IOrderedQueryable<TSource> source,
      SqlQuerySpec querySpec)
    {
      if (querySpec == null)
        throw new ArgumentNullException(nameof (querySpec));
      if (string.IsNullOrEmpty(querySpec.QueryText))
        throw new ArgumentException("querySpec.QueryText");
      return source.Provider.CreateQuery<TResult>((Expression) Expression.Call((Expression) null, ((MethodInfo) MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof (TSource), typeof (TResult)), new Expression[2]
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
      return source.Provider.CreateQuery<object>((Expression) Expression.Call((Expression) null, ((MethodInfo) MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof (TSource)), new Expression[2]
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
