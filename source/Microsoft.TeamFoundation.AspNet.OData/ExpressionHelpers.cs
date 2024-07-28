// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ExpressionHelpers
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Query.Expressions;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData
{
  internal static class ExpressionHelpers
  {
    public static Func<long> Count(IQueryable query, Type type)
    {
      MethodInfo countMethod = ExpressionHelperMethods.QueryableCountGeneric.MakeGenericMethod(type);
      return (Func<long>) (() => (long) countMethod.Invoke((object) null, new object[1]
      {
        (object) query
      }));
    }

    public static IQueryable Skip(IQueryable query, int count, Type type, bool parameterize)
    {
      MethodInfo method = ExpressionHelperMethods.QueryableSkipGeneric.MakeGenericMethod(type);
      Expression expression1 = parameterize ? LinqParameterContainer.Parameterize(typeof (int), (object) count) : (Expression) Expression.Constant((object) count);
      Expression expression2 = (Expression) Expression.Call((Expression) null, method, new Expression[2]
      {
        query.Expression,
        expression1
      });
      return ExpressionHelperMethods.CreateQueryGeneric.MakeGenericMethod(type).Invoke((object) query.Provider, (object[]) new Expression[1]
      {
        expression2
      }) as IQueryable;
    }

    public static IQueryable Take(IQueryable query, int count, Type type, bool parameterize)
    {
      Expression expression = ExpressionHelpers.Take(query.Expression, count, type, parameterize);
      return ExpressionHelperMethods.CreateQueryGeneric.MakeGenericMethod(type).Invoke((object) query.Provider, (object[]) new Expression[1]
      {
        expression
      }) as IQueryable;
    }

    public static Expression Skip(Expression source, int count, Type type, bool parameterize)
    {
      MethodInfo method;
      if (typeof (IQueryable).IsAssignableFrom(source.Type))
        method = ExpressionHelperMethods.QueryableSkipGeneric.MakeGenericMethod(type);
      else
        method = ExpressionHelperMethods.EnumerableSkipGeneric.MakeGenericMethod(type);
      Expression expression = parameterize ? LinqParameterContainer.Parameterize(typeof (int), (object) count) : (Expression) Expression.Constant((object) count);
      return (Expression) Expression.Call((Expression) null, method, new Expression[2]
      {
        source,
        expression
      });
    }

    public static Expression Take(
      Expression source,
      int count,
      Type elementType,
      bool parameterize)
    {
      MethodInfo method;
      if (typeof (IQueryable).IsAssignableFrom(source.Type))
        method = ExpressionHelperMethods.QueryableTakeGeneric.MakeGenericMethod(elementType);
      else
        method = ExpressionHelperMethods.EnumerableTakeGeneric.MakeGenericMethod(elementType);
      Expression expression = parameterize ? LinqParameterContainer.Parameterize(typeof (int), (object) count) : (Expression) Expression.Constant((object) count);
      return (Expression) Expression.Call((Expression) null, method, new Expression[2]
      {
        source,
        expression
      });
    }

    public static Expression OrderByPropertyExpression(
      Expression source,
      string propertyName,
      Type elementType,
      bool alreadyOrdered = false)
    {
      LambdaExpression propertyAccessLambda = ExpressionHelpers.GetPropertyAccessLambda(elementType, propertyName);
      return ExpressionHelpers.OrderBy(source, propertyAccessLambda, elementType, OrderByDirection.Ascending, alreadyOrdered);
    }

    public static Expression OrderBy(
      Expression source,
      LambdaExpression orderByLambda,
      Type elementType,
      OrderByDirection direction,
      bool alreadyOrdered = false)
    {
      Type type = orderByLambda.Body.Type;
      MethodInfo method;
      if (!alreadyOrdered)
      {
        if (typeof (IQueryable).IsAssignableFrom(source.Type))
        {
          if (direction == OrderByDirection.Ascending)
            method = ExpressionHelperMethods.QueryableOrderByGeneric.MakeGenericMethod(elementType, type);
          else
            method = ExpressionHelperMethods.QueryableOrderByDescendingGeneric.MakeGenericMethod(elementType, type);
        }
        else if (direction == OrderByDirection.Ascending)
          method = ExpressionHelperMethods.EnumerableOrderByGeneric.MakeGenericMethod(elementType, type);
        else
          method = ExpressionHelperMethods.EnumerableOrderByDescendingGeneric.MakeGenericMethod(elementType, type);
      }
      else if (typeof (IQueryable).IsAssignableFrom(source.Type))
      {
        if (direction == OrderByDirection.Ascending)
          method = ExpressionHelperMethods.QueryableThenByGeneric.MakeGenericMethod(elementType, type);
        else
          method = ExpressionHelperMethods.QueryableThenByDescendingGeneric.MakeGenericMethod(elementType, type);
      }
      else if (direction == OrderByDirection.Ascending)
        method = ExpressionHelperMethods.EnumerableThenByGeneric.MakeGenericMethod(elementType, type);
      else
        method = ExpressionHelperMethods.EnumerableThenByDescendingGeneric.MakeGenericMethod(elementType, type);
      return (Expression) Expression.Call((Expression) null, method, new Expression[2]
      {
        source,
        (Expression) orderByLambda
      });
    }

    public static IQueryable OrderByIt(
      IQueryable query,
      OrderByDirection direction,
      Type type,
      bool alreadyOrdered = false)
    {
      ParameterExpression body = Expression.Parameter(type, "$it");
      LambdaExpression orderByLambda = Expression.Lambda((Expression) body, body);
      return ExpressionHelpers.OrderBy(query, orderByLambda, direction, type, alreadyOrdered);
    }

    public static IQueryable OrderByProperty(
      IQueryable query,
      IEdmModel model,
      IEdmProperty property,
      OrderByDirection direction,
      Type type,
      bool alreadyOrdered = false)
    {
      string clrPropertyName = EdmLibHelpers.GetClrPropertyName(property, model);
      Type elementType = query.ElementType;
      LambdaExpression orderByLambda = !EdmLibHelpers.IsComputeWrapper(elementType, out Type _) ? ExpressionHelpers.GetPropertyAccessLambda(type, clrPropertyName) : ExpressionHelpers.GetInstancePropertyAccessLambda(elementType, clrPropertyName);
      return ExpressionHelpers.OrderBy(query, orderByLambda, direction, type, alreadyOrdered);
    }

    public static IQueryable OrderBy(
      IQueryable query,
      LambdaExpression orderByLambda,
      OrderByDirection direction,
      Type type,
      bool alreadyOrdered = false)
    {
      Type type1 = orderByLambda.Body.Type;
      Type type2 = query?.ElementType;
      if ((object) type2 == null)
        type2 = type;
      type = type2;
      IOrderedQueryable orderedQueryable1;
      if (alreadyOrdered)
      {
        MethodInfo methodInfo;
        if (direction == OrderByDirection.Ascending)
          methodInfo = ExpressionHelperMethods.QueryableThenByGeneric.MakeGenericMethod(type, type1);
        else
          methodInfo = ExpressionHelperMethods.QueryableThenByDescendingGeneric.MakeGenericMethod(type, type1);
        IOrderedQueryable orderedQueryable2 = query as IOrderedQueryable;
        orderedQueryable1 = methodInfo.Invoke((object) null, new object[2]
        {
          (object) orderedQueryable2,
          (object) orderByLambda
        }) as IOrderedQueryable;
      }
      else
      {
        MethodInfo methodInfo;
        if (direction == OrderByDirection.Ascending)
          methodInfo = ExpressionHelperMethods.QueryableOrderByGeneric.MakeGenericMethod(type, type1);
        else
          methodInfo = ExpressionHelperMethods.QueryableOrderByDescendingGeneric.MakeGenericMethod(type, type1);
        orderedQueryable1 = methodInfo.Invoke((object) null, new object[2]
        {
          (object) query,
          (object) orderByLambda
        }) as IOrderedQueryable;
      }
      return (IQueryable) orderedQueryable1;
    }

    public static IQueryable GroupBy(
      IQueryable query,
      Expression expression,
      Type type,
      Type wrapperType)
    {
      return ExpressionHelperMethods.QueryableGroupByGeneric.MakeGenericMethod(type, wrapperType).Invoke((object) null, new object[2]
      {
        (object) query,
        (object) expression
      }) as IQueryable;
    }

    public static IQueryable Select(IQueryable query, LambdaExpression expression, Type type) => ExpressionHelperMethods.QueryableSelectGeneric.MakeGenericMethod(type, expression.Body.Type).Invoke((object) null, new object[2]
    {
      (object) query,
      (object) expression
    }) as IQueryable;

    public static IQueryable SelectMany(IQueryable query, LambdaExpression expression, Type type) => ExpressionHelperMethods.QueryableSelectManyGeneric.MakeGenericMethod(type, expression.Body.Type).Invoke((object) null, new object[2]
    {
      (object) query,
      (object) expression
    }) as IQueryable;

    public static IQueryable Aggregate(
      IQueryable query,
      object init,
      LambdaExpression sumLambda,
      Type type,
      Type wrapperType)
    {
      Type type1 = sumLambda.Body.Type;
      object obj = ExpressionHelperMethods.QueryableAggregateGeneric.MakeGenericMethod(type, type1).Invoke((object) null, new object[3]
      {
        (object) query,
        init,
        (object) sumLambda
      });
      return ExpressionHelperMethods.EntityAsQueryable.MakeGenericMethod(wrapperType).Invoke((object) null, new object[1]
      {
        obj
      }) as IQueryable;
    }

    public static IQueryable Where(IQueryable query, Expression where, Type type) => ExpressionHelperMethods.QueryableWhereGeneric.MakeGenericMethod(type).Invoke((object) null, new object[2]
    {
      (object) query,
      (object) where
    }) as IQueryable;

    public static Expression ToNullable(Expression expression) => !TypeHelper.IsNullable(expression.Type) ? (Expression) Expression.Convert(expression, TypeHelper.ToNullable(expression.Type)) : expression;

    public static Expression Default(Type type) => TypeHelper.IsValueType(type) ? (Expression) Expression.Constant(Activator.CreateInstance(type), type) : (Expression) Expression.Constant((object) null, type);

    public static LambdaExpression GetPropertyAccessLambda(Type type, string propertyName)
    {
      ParameterExpression parameterExpression = Expression.Parameter(type, "$it");
      return Expression.Lambda((Expression) Expression.Property((Expression) parameterExpression, propertyName), parameterExpression);
    }

    public static LambdaExpression GetInstancePropertyAccessLambda(Type type, string propertyName)
    {
      ParameterExpression parameterExpression = Expression.Parameter(type, "$it");
      return Expression.Lambda((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, "Instance"), propertyName), parameterExpression);
    }

    public static bool HasGroupBy(Expression expression)
    {
      if (!(expression is MethodCallExpression methodCallExpression))
        return false;
      return methodCallExpression.Method.Name == "GroupBy" || ExpressionHelpers.HasGroupBy(methodCallExpression.Arguments.First<Expression>());
    }
  }
}
