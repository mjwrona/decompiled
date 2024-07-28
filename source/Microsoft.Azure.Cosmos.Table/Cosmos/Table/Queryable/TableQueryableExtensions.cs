// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.TableQueryableExtensions
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  public static class TableQueryableExtensions
  {
    internal static MethodInfo WithOptionsMethodInfo { get; set; }

    internal static MethodInfo WithContextMethodInfo { get; set; }

    internal static MethodInfo ResolveMethodInfo { get; set; }

    static TableQueryableExtensions()
    {
      MethodInfo[] methods = typeof (TableQueryableExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public);
      TableQueryableExtensions.WithOptionsMethodInfo = ((IEnumerable<MethodInfo>) methods).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == "WithOptions")).FirstOrDefault<MethodInfo>();
      TableQueryableExtensions.WithContextMethodInfo = ((IEnumerable<MethodInfo>) methods).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == "WithContext")).FirstOrDefault<MethodInfo>();
      TableQueryableExtensions.ResolveMethodInfo = ((IEnumerable<MethodInfo>) methods).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == "Resolve")).FirstOrDefault<MethodInfo>();
    }

    public static TableQuery<TElement> WithOptions<TElement>(
      this IQueryable<TElement> query,
      TableRequestOptions options)
    {
      CommonUtility.AssertNotNull(nameof (options), (object) options);
      if (!(query is TableQuery<TElement>))
        throw new NotSupportedException("Query must be a TableQuery<T>");
      return (TableQuery<TElement>) query.Provider.CreateQuery<TElement>((Expression) Expression.Call((Expression) null, TableQueryableExtensions.WithOptionsMethodInfo.MakeGenericMethod(typeof (TElement)), new Expression[2]
      {
        query.Expression,
        (Expression) Expression.Constant((object) options, typeof (TableRequestOptions))
      }));
    }

    public static TableQuery<TElement> WithContext<TElement>(
      this IQueryable<TElement> query,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNull(nameof (operationContext), (object) operationContext);
      if (!(query is TableQuery<TElement>))
        throw new NotSupportedException("Query must be a TableQuery<T>");
      return (TableQuery<TElement>) query.Provider.CreateQuery<TElement>((Expression) Expression.Call((Expression) null, TableQueryableExtensions.WithContextMethodInfo.MakeGenericMethod(typeof (TElement)), new Expression[2]
      {
        query.Expression,
        (Expression) Expression.Constant((object) operationContext, typeof (OperationContext))
      }));
    }

    public static TableQuery<TResolved> Resolve<TElement, TResolved>(
      this IQueryable<TElement> query,
      EntityResolver<TResolved> resolver)
    {
      CommonUtility.AssertNotNull(nameof (resolver), (object) resolver);
      if (!(query is TableQuery<TElement>))
        throw new NotSupportedException("Query must be a TableQuery<T>");
      return (TableQuery<TResolved>) query.Provider.CreateQuery<TResolved>((Expression) Expression.Call((Expression) null, TableQueryableExtensions.ResolveMethodInfo.MakeGenericMethod(typeof (TElement), typeof (TResolved)), new Expression[2]
      {
        query.Expression,
        (Expression) Expression.Constant((object) resolver, typeof (EntityResolver<TResolved>))
      }));
    }

    public static TableQuery<TElement> AsTableQuery<TElement>(this IQueryable<TElement> query) => query is TableQuery<TElement> tableQuery ? tableQuery : throw new NotSupportedException("Query must be a TableQuery<T>");
  }
}
