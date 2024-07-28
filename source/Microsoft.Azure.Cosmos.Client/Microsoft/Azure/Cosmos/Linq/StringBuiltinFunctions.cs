// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.StringBuiltinFunctions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal static class StringBuiltinFunctions
  {
    private static Dictionary<string, BuiltinFunctionVisitor> StringBuiltinFunctionDefinitions { get; set; }

    static StringBuiltinFunctions() => StringBuiltinFunctions.StringBuiltinFunctionDefinitions = new Dictionary<string, BuiltinFunctionVisitor>()
    {
      {
        "Concat",
        (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringVisitConcat()
      },
      {
        "Contains",
        (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringVisitContains()
      },
      {
        "EndsWith",
        (BuiltinFunctionVisitor) new StringBuiltinFunctions.SqlStringWithComparisonVisitor("ENDSWITH")
      },
      {
        "IndexOf",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("INDEX_OF", false, new List<Type[]>()
        {
          new Type[1]{ typeof (char) },
          new Type[1]{ typeof (string) },
          new Type[2]{ typeof (char), typeof (int) },
          new Type[2]{ typeof (string), typeof (int) }
        })
      },
      {
        "Count",
        (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringVisitCount()
      },
      {
        "ToLower",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("LOWER", false, new List<Type[]>()
        {
          new Type[0]
        })
      },
      {
        "TrimStart",
        (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringVisitTrimStart()
      },
      {
        "Replace",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("REPLACE", false, new List<Type[]>()
        {
          new Type[2]{ typeof (char), typeof (char) },
          new Type[2]{ typeof (string), typeof (string) }
        })
      },
      {
        "Reverse",
        (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringVisitReverse()
      },
      {
        "TrimEnd",
        (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringVisitTrimEnd()
      },
      {
        "StartsWith",
        (BuiltinFunctionVisitor) new StringBuiltinFunctions.SqlStringWithComparisonVisitor("STARTSWITH")
      },
      {
        "Substring",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("SUBSTRING", false, new List<Type[]>()
        {
          new Type[2]{ typeof (int), typeof (int) }
        })
      },
      {
        "ToUpper",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("UPPER", false, new List<Type[]>()
        {
          new Type[0]
        })
      },
      {
        "get_Chars",
        (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringGetCharsVisitor()
      },
      {
        "Equals",
        (BuiltinFunctionVisitor) new StringBuiltinFunctions.StringEqualsVisitor()
      }
    };

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
        if (methodCallExpression.Arguments.Count != 1 || !(methodCallExpression.Arguments[0] is NewArrayExpression newArrayExpression))
          return (SqlScalarExpression) null;
        ReadOnlyCollection<Expression> expressions = newArrayExpression.Expressions;
        List<SqlScalarExpression> items = new List<SqlScalarExpression>();
        foreach (Expression expression in expressions)
          items.Add(ExpressionToSql.VisitScalarExpression(expression, context));
        return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("CONCAT", items.ToImmutableArray<SqlScalarExpression>());
      }
    }

    private class StringVisitContains : SqlBuiltinFunctionVisitor
    {
      public StringVisitContains()
        : base("CONTAINS", false, new List<Type[]>()
        {
          new Type[1]{ typeof (string) },
          new Type[1]{ typeof (char) }
        })
      {
      }

      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        if (methodCallExpression.Arguments.Count != 2)
          return (SqlScalarExpression) null;
        return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("CONTAINS", ExpressionToSql.VisitScalarExpression(methodCallExpression.Object, context), ExpressionToSql.VisitScalarExpression(methodCallExpression.Arguments[0], context), StringBuiltinFunctions.SqlStringWithComparisonVisitor.GetCaseSensitivityExpression(methodCallExpression.Arguments[1]));
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
        bool flag1 = false;
        bool flag2 = false;
        if (methodCallExpression.Arguments.Count == 1 && methodCallExpression.Arguments[0].NodeType == ExpressionType.Constant && methodCallExpression.Arguments[0].Type == typeof (char[]))
        {
          if (((char[]) ((ConstantExpression) methodCallExpression.Arguments[0]).Value).Length == 0)
            flag1 = true;
        }
        else if (methodCallExpression.Arguments.Count == 0)
          flag2 = true;
        if (!(flag1 | flag2))
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

    private sealed class SqlStringWithComparisonVisitor : BuiltinFunctionVisitor
    {
      private static readonly IImmutableSet<StringComparison> SensitiveCaseComparisons = (IImmutableSet<StringComparison>) ImmutableHashSet.Create<StringComparison>(StringComparison.CurrentCulture, StringComparison.InvariantCulture, StringComparison.Ordinal);
      private static readonly IImmutableSet<StringComparison> IgnoreCaseComparisons = (IImmutableSet<StringComparison>) ImmutableHashSet.Create<StringComparison>(StringComparison.CurrentCultureIgnoreCase, StringComparison.InvariantCultureIgnoreCase, StringComparison.OrdinalIgnoreCase);

      public string SqlName { get; }

      public SqlStringWithComparisonVisitor(string sqlName) => this.SqlName = sqlName ?? throw new ArgumentNullException(nameof (sqlName));

      public static SqlScalarExpression GetCaseSensitivityExpression(Expression expression)
      {
        if (expression is ConstantExpression constantExpression && constantExpression.Value is StringComparison stringComparison)
        {
          if (StringBuiltinFunctions.SqlStringWithComparisonVisitor.SensitiveCaseComparisons.Contains(stringComparison))
            return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlBooleanLiteral.Create(false));
          if (StringBuiltinFunctions.SqlStringWithComparisonVisitor.IgnoreCaseComparisons.Contains(stringComparison))
            return (SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlBooleanLiteral.Create(true));
        }
        return (SqlScalarExpression) null;
      }

      protected override SqlScalarExpression VisitImplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        int count = methodCallExpression.Arguments.Count;
        if (count == 0 || count > 2)
          return (SqlScalarExpression) null;
        List<SqlScalarExpression> scalarExpressionList = new List<SqlScalarExpression>()
        {
          ExpressionToSql.VisitNonSubqueryScalarExpression(methodCallExpression.Object, context),
          ExpressionToSql.VisitNonSubqueryScalarExpression(methodCallExpression.Arguments[0], context)
        };
        if (count > 1)
          scalarExpressionList.Add(StringBuiltinFunctions.SqlStringWithComparisonVisitor.GetCaseSensitivityExpression(methodCallExpression.Arguments[1]));
        return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin(this.SqlName, scalarExpressionList.ToArray());
      }

      protected override SqlScalarExpression VisitExplicit(
        MethodCallExpression methodCallExpression,
        TranslationContext context)
      {
        return (SqlScalarExpression) null;
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
        bool flag1 = false;
        bool flag2 = false;
        if (methodCallExpression.Arguments.Count == 1 && methodCallExpression.Arguments[0].NodeType == ExpressionType.Constant && methodCallExpression.Arguments[0].Type == typeof (char[]))
        {
          if (((char[]) ((ConstantExpression) methodCallExpression.Arguments[0]).Value).Length == 0)
            flag1 = true;
        }
        else if (methodCallExpression.Arguments.Count == 0)
          flag2 = true;
        if (!(flag1 | flag2))
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
        if (methodCallExpression.Arguments.Count == 1)
          return (SqlScalarExpression) SqlBinaryScalarExpression.Create(SqlBinaryScalarOperatorKind.Equal, ExpressionToSql.VisitScalarExpression(methodCallExpression.Object, context), ExpressionToSql.VisitScalarExpression(methodCallExpression.Arguments[0], context));
        if (methodCallExpression.Arguments.Count != 2)
          return (SqlScalarExpression) null;
        return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin("StringEquals", ExpressionToSql.VisitScalarExpression(methodCallExpression.Object, context), ExpressionToSql.VisitScalarExpression(methodCallExpression.Arguments[0], context), StringBuiltinFunctions.SqlStringWithComparisonVisitor.GetCaseSensitivityExpression(methodCallExpression.Arguments[1]));
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
