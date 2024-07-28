// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.StringBuiltinFunctions
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Sql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
{
  internal static class StringBuiltinFunctions
  {
    private static Dictionary<string, BuiltinFunctionVisitor> StringBuiltinFunctionDefinitions { get; set; }

    static StringBuiltinFunctions()
    {
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions = new Dictionary<string, BuiltinFunctionVisitor>();
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("Concat", (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringVisitConcat());
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("Contains", (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringVisitContains());
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("EndsWith", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ENDSWITH", false, new List<Type[]>()
      {
        new Type[1]{ typeof (string) }
      }));
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("IndexOf", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("INDEX_OF", false, new List<Type[]>()
      {
        new Type[1]{ typeof (char) },
        new Type[1]{ typeof (string) },
        new Type[2]{ typeof (char), typeof (int) },
        new Type[2]{ typeof (string), typeof (int) }
      }));
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("Count", (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringVisitCount());
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("ToLower", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("LOWER", false, new List<Type[]>()
      {
        new Type[0]
      }));
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("TrimStart", (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringVisitTrimStart());
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("Replace", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("REPLACE", false, new List<Type[]>()
      {
        new Type[2]{ typeof (char), typeof (char) },
        new Type[2]{ typeof (string), typeof (string) }
      }));
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("Reverse", (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringVisitReverse());
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("TrimEnd", (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringVisitTrimEnd());
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("StartsWith", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("STARTSWITH", false, new List<Type[]>()
      {
        new Type[1]{ typeof (string) }
      }));
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("Substring", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("SUBSTRING", false, new List<Type[]>()
      {
        new Type[2]{ typeof (int), typeof (int) }
      }));
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("ToUpper", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("UPPER", false, new List<Type[]>()
      {
        new Type[0]
      }));
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("get_Chars", (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringGetCharsVisitor());
      StringBuiltinFunctions.StringBuiltinFunctionDefinitions.Add("Equals", (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringEqualsVisitor());
    }

    public static SqlScalarExpression Visit(
      MethodCallExpression methodCallExpression,
      TranslationContext context)
    {
      BuiltinFunctionVisitor builtinFunctionVisitor = (BuiltinFunctionVisitor) null;
      if (StringBuiltinFunctions.StringBuiltinFunctionDefinitions.TryGetValue(methodCallExpression.Method.Name, out builtinFunctionVisitor))
        return builtinFunctionVisitor.Visit(methodCallExpression, context);
      throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.MethodNotSupported, (object) methodCallExpression.Method.Name));
    }

    private class StringVisitConcat : SqlBuiltinFunctionVisitor
    {
      public StringVisitConcat()
        : base("CONCAT", true, new List<Type[]>()
        {
          new Type[2]{ typeof (string), typeof (string) },
          new Type[3]
          {
            typeof (string),
            typeof (string),
            typeof (string)
          },
          new Type[4]
          {
            typeof (string),
            typeof (string),
            typeof (string),
            typeof (string)
          }
        })
      {
      }

      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        if (methodCallExpression.Arguments.Count != 1 || !(methodCallExpression.Arguments[0] is NewArrayExpression))
          return (SqlScalarExpression) null;
        ReadOnlyCollection<Expression> expressions = ((NewArrayExpression) methodCallExpression.Arguments[0]).Expressions;
        List<SqlScalarExpression> arguments = new List<SqlScalarExpression>();
        foreach (Expression expression in expressions)
          arguments.Add(ExpressionToSql.VisitScalarExpression(expression, context));
        return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("CONCAT", (IReadOnlyList<SqlScalarExpression>) arguments);
      }
    }

    private class StringVisitContains : SqlBuiltinFunctionVisitor
    {
      public StringVisitContains()
        : base("CONTAINS", false, new List<Type[]>()
        {
          new Type[1]{ typeof (string) }
        })
      {
      }

      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        if (methodCallExpression.Arguments.Count != 2)
          return (SqlScalarExpression) null;
        return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("CONTAINS", ExpressionToSql.VisitScalarExpression(methodCallExpression.Arguments[0], context), ExpressionToSql.VisitScalarExpression(methodCallExpression.Arguments[1], context));
      }
    }

    private class StringVisitCount : SqlBuiltinFunctionVisitor
    {
      public StringVisitCount()
        : base("LENGTH", true, (List<Type[]>) null)
      {
      }

      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        if (methodCallExpression.Arguments.Count != 1)
          return (SqlScalarExpression) null;
        return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("LENGTH", ExpressionToSql.VisitScalarExpression(methodCallExpression.Arguments[0], context));
      }
    }

    private class StringVisitTrimStart : SqlBuiltinFunctionVisitor
    {
      public StringVisitTrimStart()
        : base("LTRIM", false, (List<Type[]>) null)
      {
      }

      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        if (methodCallExpression.Arguments.Count != 1 || methodCallExpression.Arguments[0].NodeType != ExpressionType.Constant || (object) methodCallExpression.Arguments[0].Type != (object) typeof (char[]) || ((char[]) ((ConstantExpression) methodCallExpression.Arguments[0]).Value).Length != 0)
          return (SqlScalarExpression) null;
        return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("LTRIM", ExpressionToSql.VisitScalarExpression(methodCallExpression.Object, context));
      }
    }

    private class StringVisitReverse : SqlBuiltinFunctionVisitor
    {
      public StringVisitReverse()
        : base("REVERSE", true, (List<Type[]>) null)
      {
      }

      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        if (methodCallExpression.Arguments.Count != 1)
          return (SqlScalarExpression) null;
        return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("REVERSE", ExpressionToSql.VisitNonSubqueryScalarExpression(methodCallExpression.Arguments[0], context));
      }
    }

    private class StringVisitTrimEnd : SqlBuiltinFunctionVisitor
    {
      public StringVisitTrimEnd()
        : base("RTRIM", false, (List<Type[]>) null)
      {
      }

      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        if (methodCallExpression.Arguments.Count != 1 || methodCallExpression.Arguments[0].NodeType != ExpressionType.Constant || (object) methodCallExpression.Arguments[0].Type != (object) typeof (char[]) || ((char[]) ((ConstantExpression) methodCallExpression.Arguments[0]).Value).Length != 0)
          return (SqlScalarExpression) null;
        return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("RTRIM", ExpressionToSql.VisitScalarExpression(methodCallExpression.Object, context));
      }
    }

    private class StringGetCharsVisitor : BuiltinFunctionVisitor
    {
      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        if (methodCallExpression.Arguments.Count != 1)
          return (SqlScalarExpression) null;
        return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("SUBSTRING", ExpressionToSql.VisitScalarExpression(methodCallExpression.Object, context), ExpressionToSql.VisitScalarExpression(methodCallExpression.Arguments[0], context), ExpressionToSql.VisitScalarExpression((Expression) Expression.Constant((object) 1), context));
      }

      protected override SqlScalarExpression VisitExplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        return (SqlScalarExpression) null;
      }
    }

    private class StringEqualsVisitor : BuiltinFunctionVisitor
    {
      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        return methodCallExpression.Arguments.Count == 1 ? (SqlScalarExpression) SqlBinaryScalarExpression.Create(SqlBinaryScalarOperatorKind.Equal, ExpressionToSql.VisitScalarExpression(methodCallExpression.Object, context), ExpressionToSql.VisitScalarExpression(methodCallExpression.Arguments[0], context)) : (SqlScalarExpression) null;
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
