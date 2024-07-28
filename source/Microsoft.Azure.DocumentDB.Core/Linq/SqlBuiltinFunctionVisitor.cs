// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.SqlBuiltinFunctionVisitor
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Sql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
{
  internal class SqlBuiltinFunctionVisitor : BuiltinFunctionVisitor
  {
    public SqlBuiltinFunctionVisitor(string sqlName, bool isStatic, List<Type[]> argumentLists)
    {
      this.SqlName = sqlName;
      this.IsStatic = isStatic;
      this.ArgumentLists = argumentLists;
    }

    public string SqlName { get; private set; }

    public bool IsStatic { get; private set; }

    public List<Type[]> ArgumentLists { get; private set; }

    protected override SqlScalarExpression VisitExplicit(
      MethodCallExpression methodCallExpression,
      TranslationContext context)
    {
      if (this.ArgumentLists != null)
      {
        foreach (Type[] argumentList in this.ArgumentLists)
        {
          if (this.MatchArgumentLists(methodCallExpression.Arguments, argumentList))
            return this.VisitBuiltinFunction(methodCallExpression, context);
        }
      }
      return (SqlScalarExpression) null;
    }

    protected override SqlScalarExpression VisitImplicit(
      MethodCallExpression methodCallExpression,
      TranslationContext context)
    {
      return (SqlScalarExpression) null;
    }

    private bool MatchArgumentLists(
      ReadOnlyCollection<Expression> methodCallArguments,
      Type[] expectedArguments)
    {
      if (methodCallArguments.Count != expectedArguments.Length)
        return false;
      for (int index = 0; index < expectedArguments.Length; ++index)
      {
        if ((object) methodCallArguments[index].Type != (object) expectedArguments[index] && !CustomTypeExtensions.IsAssignableFrom(expectedArguments[index], methodCallArguments[index].Type))
          return false;
      }
      return true;
    }

    private SqlScalarExpression VisitBuiltinFunction(
      MethodCallExpression methodCallExpression,
      TranslationContext context)
    {
      List<SqlScalarExpression> scalarExpressionList = new List<SqlScalarExpression>();
      if (methodCallExpression.Object != null)
        scalarExpressionList.Add(ExpressionToSql.VisitNonSubqueryScalarExpression(methodCallExpression.Object, context));
      foreach (Expression inputExpression in methodCallExpression.Arguments)
        scalarExpressionList.Add(ExpressionToSql.VisitNonSubqueryScalarExpression(inputExpression, context));
      return (SqlScalarExpression) SqlFunctionCallScalarExpression.CreateBuiltin(this.SqlName, scalarExpressionList.ToArray());
    }
  }
}
