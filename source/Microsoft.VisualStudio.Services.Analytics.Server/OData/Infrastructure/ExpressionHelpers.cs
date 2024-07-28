// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ExpressionHelpers
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal static class ExpressionHelpers
  {
    private static MethodInfo EnumerableAsQueryableMethod = ExpressionHelpers.GenericMethodOf<IQueryable<int>>((Expression<Func<object, IQueryable<int>>>) (_ => default (IEnumerable<int>).AsQueryable<int>()));
    private static readonly object[] EmptyParamsList = new object[0];

    public static MethodInfo QueryableTakeGeneric { get; } = ExpressionHelpers.GenericMethodOf<IQueryable<int>>((Expression<Func<object, IQueryable<int>>>) (_ => default (IQueryable<int>).Take<int>(0)));

    public static MethodInfo QueryableSkipGeneric { get; } = ExpressionHelpers.GenericMethodOf<IQueryable<int>>((Expression<Func<object, IQueryable<int>>>) (_ => default (IQueryable<int>).Skip<int>(0)));

    public static MethodInfo QueryableOrderByGeneric { get; } = ExpressionHelpers.GenericMethodOf<IOrderedQueryable<int>>((Expression<Func<object, IOrderedQueryable<int>>>) (_ => default (IQueryable<int>).OrderBy<int, int>(default (Expression<Func<int, int>>))));

    public static MethodInfo QueryableThenByGeneric { get; } = ExpressionHelpers.GenericMethodOf<IOrderedQueryable<int>>((Expression<Func<object, IOrderedQueryable<int>>>) (_ => default (IOrderedQueryable<int>).ThenBy<int, int>(default (Expression<Func<int, int>>))));

    public static MethodInfo QueryableWhereGeneric { get; } = ExpressionHelpers.GenericMethodOf<IQueryable<int>>((Expression<Func<object, IQueryable<int>>>) (_ => default (IQueryable<int>).Where<int>(default (Expression<Func<int, bool>>))));

    public static MethodInfo EnumerableToListGeneric { get; } = ExpressionHelpers.GenericMethodOf<List<int>>((Expression<Func<object, List<int>>>) (_ => default (IEnumerable<int>).ToList<int>()));

    public static MethodInfo QueryableCastGeneric { get; } = ExpressionHelpers.GenericMethodOf<IQueryable<int>>((Expression<Func<object, IQueryable<int>>>) (_ => Queryable.Cast<int>(default (IQueryable))));

    public static MethodInfo QueryableCoutGeneric { get; } = ExpressionHelpers.GenericMethodOf<int>((Expression<Func<object, int>>) (_ => default (IQueryable<int>).Count<int>()));

    public static MethodInfo QueryableFirstOrDefaultGeneric { get; } = ExpressionHelpers.GenericMethodOf<int>((Expression<Func<object, int>>) (_ => default (IQueryable<int>).FirstOrDefault<int>()));

    public static MethodInfo QueryableFirstOrDefaultWithPredicateGeneric { get; } = ExpressionHelpers.GenericMethodOf<int>((Expression<Func<object, int>>) (_ => default (IQueryable<int>).FirstOrDefault<int>(default (Expression<Func<int, bool>>))));

    public static IQueryable Take(IQueryable query, int count, Type type)
    {
      ArgumentUtility.CheckForNull<IQueryable>(query, nameof (query));
      ArgumentUtility.CheckForNull<Type>(type, nameof (type));
      Expression expression = ExpressionHelpers.Take(query.Expression, count, type);
      return query.Provider.CreateQuery(expression);
    }

    private static Expression Take(Expression source, int count, Type elementType)
    {
      MethodInfo method = ExpressionHelpers.QueryableTakeGeneric.MakeGenericMethod(elementType);
      Expression expression = (Expression) Expression.Constant((object) count);
      return (Expression) Expression.Call((Expression) null, method, new Expression[2]
      {
        source,
        expression
      });
    }

    public static Expression FirstOrDefault(Expression query, Expression predicate = null)
    {
      ArgumentUtility.CheckForNull<Expression>(query, nameof (query));
      Type genericArgument = query.Type.GetGenericArguments()[0];
      return predicate == null ? (Expression) Expression.Call((Expression) null, ExpressionHelpers.QueryableFirstOrDefaultGeneric.MakeGenericMethod(genericArgument), query) : (Expression) Expression.Call((Expression) null, ExpressionHelpers.QueryableFirstOrDefaultWithPredicateGeneric.MakeGenericMethod(genericArgument), new Expression[2]
      {
        query,
        predicate
      });
    }

    public static Expression AsQueryable(Expression source)
    {
      ArgumentUtility.CheckForNull<Expression>(source, nameof (source));
      Type genericArgument = source.Type.GetGenericArguments()[0];
      return (Expression) Expression.Call((Expression) null, ExpressionHelpers.EnumerableAsQueryableMethod.MakeGenericMethod(genericArgument), source);
    }

    public static IQueryable AsNoTracking(IQueryable query)
    {
      ArgumentUtility.CheckForNull<IQueryable>(query, nameof (query));
      Type type = typeof (DbQuery<>).MakeGenericType(query.ElementType);
      return type.IsAssignableFrom(query.GetType()) ? type.GetMethod(nameof (AsNoTracking)).Invoke((object) query, ExpressionHelpers.EmptyParamsList) as IQueryable : query;
    }

    public static IList ToList(IQueryable query, Type type)
    {
      ArgumentUtility.CheckForNull<IQueryable>(query, nameof (query));
      ArgumentUtility.CheckForNull<Type>(type, nameof (type));
      return ExpressionHelpers.EnumerableToListGeneric.MakeGenericMethod(type).Invoke((object) null, new object[1]
      {
        (object) query
      }) as IList;
    }

    public static IQueryable Cast(IQueryable query, Type type)
    {
      ArgumentUtility.CheckForNull<IQueryable>(query, nameof (query));
      ArgumentUtility.CheckForNull<Type>(type, nameof (type));
      return ExpressionHelpers.QueryableCastGeneric.MakeGenericMethod(type).Invoke((object) null, new object[1]
      {
        (object) query
      }) as IQueryable;
    }

    public static int Count(IQueryable query)
    {
      ArgumentUtility.CheckForNull<IQueryable>(query, nameof (query));
      return (int) ExpressionHelpers.QueryableCoutGeneric.MakeGenericMethod(query.ElementType).Invoke((object) null, new object[1]
      {
        (object) query
      });
    }

    public static IQueryable RemoveTopSkipSorting(IQueryable query)
    {
      ArgumentUtility.CheckForNull<IQueryable>(query, nameof (query));
      return query.Provider.CreateQuery(ExpressionHelpers.RemoveTopSkipSorting(query.Expression));
    }

    private static Expression RemoveTopSkipSorting(Expression source)
    {
      if (source.NodeType == ExpressionType.Call)
      {
        MethodCallExpression methodCallExpression = source as MethodCallExpression;
        if (methodCallExpression.Method.Name == ExpressionHelpers.QueryableTakeGeneric.Name || methodCallExpression.Method.Name == ExpressionHelpers.QueryableOrderByGeneric.Name || methodCallExpression.Method.Name == ExpressionHelpers.QueryableThenByGeneric.Name || methodCallExpression.Method.Name == ExpressionHelpers.QueryableSkipGeneric.Name)
          return ExpressionHelpers.RemoveTopSkipSorting(methodCallExpression.Arguments.First<Expression>());
      }
      return source;
    }

    public static IQueryable OrderBy(IQueryable query, IEnumerable<string> properties)
    {
      Expression expression1 = query.Expression;
      ExpressionHelpers.PreSelectExpression selectExpression = ExpressionHelpers.ExtractPreSelectExpression(expression1);
      if (selectExpression != null)
      {
        Expression expression2 = ExpressionHelpers.OrderBy(selectExpression.Source, properties, ((IEnumerable<Type>) selectExpression.Source.Type.GenericTypeArguments).First<Type>());
        Expression expression3 = (Expression) Expression.Call((Expression) null, selectExpression.Method, new Expression[2]
        {
          expression2,
          selectExpression.Lambda
        });
        return query.Provider.CreateQuery(expression3);
      }
      Expression expression4 = ExpressionHelpers.OrderBy(expression1, properties, query.ElementType);
      return query.Provider.CreateQuery(expression4);
    }

    public static ExpressionHelpers.PreSelectExpression ExtractPreSelectExpression(Expression source)
    {
      if (source.NodeType == ExpressionType.Call)
      {
        MethodCallExpression methodCallExpression = source as MethodCallExpression;
        if ((methodCallExpression.Method.Name == "Select" || methodCallExpression.Method.Name == "OrderBy" || methodCallExpression.Method.Name == "ThenBy") && methodCallExpression.Arguments.Count == 2 && ((IEnumerable<Type>) methodCallExpression.Type.GenericTypeArguments).Any<Type>((Func<Type, bool>) (t => typeof (ISelectExpandWrapper).IsAssignableFrom(t))))
        {
          ExpressionHelpers.PreSelectExpression selectExpression1 = new ExpressionHelpers.PreSelectExpression(methodCallExpression.Arguments[0], methodCallExpression.Arguments[1], methodCallExpression.Method);
          ExpressionHelpers.PreSelectExpression selectExpression2 = ExpressionHelpers.ExtractPreSelectExpression(methodCallExpression.Arguments[0]);
          if (selectExpression2 == null)
            return selectExpression1;
          selectExpression2.Next = selectExpression1;
          return selectExpression2;
        }
      }
      return (ExpressionHelpers.PreSelectExpression) null;
    }

    public static (ExpressionHelpers.PreSelectExpression preSelect, IQueryable queryable) ExtractPreSelectExpression<T>(
      IQueryable queryable)
      where T : struct
    {
      ExpressionHelpers.PreSelectExpression selectExpression = ExpressionHelpers.ExtractPreSelectExpression(ExpressionHelpers.RemoveTopSkipSorting(queryable.Expression));
      if (selectExpression != null)
      {
        IQueryable query = queryable.Provider.CreateQuery(selectExpression.Source);
        if (typeof (IContinuation<T>).IsAssignableFrom(query.ElementType))
          queryable = query;
      }
      return (selectExpression, queryable);
    }

    public static Expression OrderBy(
      Expression source,
      IEnumerable<string> properties,
      Type elementType)
    {
      bool flag = true;
      foreach (string property1 in properties)
      {
        ParameterExpression parameterExpression = Expression.Parameter(elementType, "$it");
        PropertyInfo property2 = elementType.GetProperty(property1);
        LambdaExpression lambdaExpression = Expression.Lambda((Expression) Expression.MakeMemberAccess((Expression) parameterExpression, (MemberInfo) property2), parameterExpression);
        source = (Expression) Expression.Call((Expression) null, (flag ? ExpressionHelpers.QueryableOrderByGeneric : ExpressionHelpers.QueryableThenByGeneric).MakeGenericMethod(elementType, lambdaExpression.Body.Type), new Expression[2]
        {
          source,
          (Expression) lambdaExpression
        });
        flag = false;
      }
      return source;
    }

    private static MethodInfo GenericMethodOf<TReturn>(Expression<Func<object, TReturn>> expression) => ExpressionHelpers.GenericMethodOf((Expression) expression);

    private static MethodInfo GenericMethodOf(Expression expression) => ((expression as LambdaExpression).Body as MethodCallExpression).Method.GetGenericMethodDefinition();

    internal class PreSelectExpression
    {
      public Expression Source { get; }

      public Expression Lambda { get; }

      public MethodInfo Method { get; }

      public ExpressionHelpers.PreSelectExpression Next { get; set; }

      public PreSelectExpression(Expression source, Expression lambda, MethodInfo method)
      {
        this.Source = source;
        this.Lambda = lambda;
        this.Method = method;
      }
    }
  }
}
