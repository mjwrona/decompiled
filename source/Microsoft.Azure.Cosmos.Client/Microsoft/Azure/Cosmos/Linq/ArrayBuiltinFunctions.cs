// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.ArrayBuiltinFunctions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal static class ArrayBuiltinFunctions
  {
    private static Dictionary<string, BuiltinFunctionVisitor> ArrayBuiltinFunctionDefinitions { get; set; }

    static ArrayBuiltinFunctions() => ArrayBuiltinFunctions.ArrayBuiltinFunctionDefinitions = new Dictionary<string, BuiltinFunctionVisitor>()
    {
      {
        "Concat",
        (BuiltinFunctionVisitor) new ArrayBuiltinFunctions.ArrayConcatVisitor()
      },
      {
        "Contains",
        (BuiltinFunctionVisitor) new ArrayBuiltinFunctions.ArrayContainsVisitor()
      },
      {
        "Count",
        (BuiltinFunctionVisitor) new ArrayBuiltinFunctions.ArrayCountVisitor()
      },
      {
        "get_Item",
        (BuiltinFunctionVisitor) new ArrayBuiltinFunctions.ArrayGetItemVisitor()
      },
      {
        "ToArray",
        (BuiltinFunctionVisitor) new ArrayBuiltinFunctions.ArrayToArrayVisitor()
      },
      {
        "ToList",
        (BuiltinFunctionVisitor) new ArrayBuiltinFunctions.ArrayToArrayVisitor()
      }
    };

    public static SqlScalarExpression Visit(
      MethodCallExpression methodCallExpression,
      TranslationContext context)
    {
      BuiltinFunctionVisitor builtinFunctionVisitor;
      if (!ArrayBuiltinFunctions.ArrayBuiltinFunctionDefinitions.TryGetValue(methodCallExpression.Method.Name, out builtinFunctionVisitor))
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.MethodNotSupported, (object) methodCallExpression.Method.Name));
      return builtinFunctionVisitor.Visit(methodCallExpression, context);
    }

    private class ArrayConcatVisitor : SqlBuiltinFunctionVisitor
    {
      public ArrayConcatVisitor()
        : base("ARRAY_CONCAT", true, (List<Type[]>) null)
      {
      }

      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        if (methodCallExpression.Arguments.Count != 2)
          return (SqlScalarExpression) null;
        return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("ARRAY_CONCAT", ExpressionToSql.VisitScalarExpression(methodCallExpression.Arguments[0], context), ExpressionToSql.VisitScalarExpression(methodCallExpression.Arguments[1], context));
      }
    }

    private class ArrayContainsVisitor : SqlBuiltinFunctionVisitor
    {
      public ArrayContainsVisitor()
        : base("ARRAY_CONTAINS", true, (List<Type[]>) null)
      {
      }

      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        Expression expression1 = (Expression) null;
        Expression expression2 = (Expression) null;
        if (methodCallExpression.Arguments.Count == 1)
        {
          expression1 = methodCallExpression.Object;
          expression2 = methodCallExpression.Arguments[0];
        }
        else if (methodCallExpression.Arguments.Count == 2)
        {
          expression1 = methodCallExpression.Arguments[0];
          expression2 = methodCallExpression.Arguments[1];
        }
        if (expression1 == null || expression2 == null)
          return (SqlScalarExpression) null;
        if (expression1.NodeType == ExpressionType.Constant)
          return this.VisitIN(expression2, (ConstantExpression) expression1, context);
        return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("ARRAY_CONTAINS", ExpressionToSql.VisitScalarExpression(expression1, context), ExpressionToSql.VisitScalarExpression(expression2, context));
      }

      private SqlScalarExpression VisitIN(
        Expression expression,
        ConstantExpression constantExpressionList,
        TranslationContext context)
      {
        List<SqlScalarExpression> items = new List<SqlScalarExpression>();
        foreach (object obj in (IEnumerable) constantExpressionList.Value)
          items.Add(ExpressionToSql.VisitConstant(Expression.Constant(obj), context));
        return items.Count == 0 ? (SqlScalarExpression) SqlLiteralScalarExpression.SqlFalseLiteralScalarExpression : (SqlScalarExpression) SqlInScalarExpression.Create(ExpressionToSql.VisitNonSubqueryScalarExpression(expression, context), false, items.ToImmutableArray<SqlScalarExpression>());
      }
    }

    private class ArrayCountVisitor : SqlBuiltinFunctionVisitor
    {
      public ArrayCountVisitor()
        : base("ARRAY_LENGTH", true, (List<Type[]>) null)
      {
      }

      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        if (methodCallExpression.Arguments.Count != 1)
          return (SqlScalarExpression) null;
        return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("ARRAY_LENGTH", ExpressionToSql.VisitScalarExpression(methodCallExpression.Arguments[0], context));
      }
    }

    private class ArrayGetItemVisitor : BuiltinFunctionVisitor
    {
      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        return methodCallExpression.Object != null && methodCallExpression.Arguments.Count == 1 ? (SqlScalarExpression) SqlMemberIndexerScalarExpression.Create(ExpressionToSql.VisitScalarExpression(methodCallExpression.Object, context), ExpressionToSql.VisitScalarExpression(methodCallExpression.Arguments[0], context)) : (SqlScalarExpression) null;
      }

      protected override SqlScalarExpression VisitExplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        return (SqlScalarExpression) null;
      }
    }

    private class ArrayToArrayVisitor : BuiltinFunctionVisitor
    {
      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        return methodCallExpression.Arguments.Count != 1 ? (SqlScalarExpression) null : ExpressionToSql.VisitScalarExpression(methodCallExpression.Arguments[0], context);
      }

      protected override SqlScalarExpression VisitExplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        return (SqlScalarExpression) null;
      }
    }
  }
}
