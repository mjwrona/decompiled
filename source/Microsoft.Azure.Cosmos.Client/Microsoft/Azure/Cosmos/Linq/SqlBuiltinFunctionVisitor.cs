// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.SqlBuiltinFunctionVisitor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
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
        if (this.ArgumentLists.Count == 0 && methodCallExpression.Arguments.Count == 0)
          return this.VisitBuiltinFunction(methodCallExpression, context);
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
        if (methodCallArguments[index].Type != expectedArguments[index] && !expectedArguments[index].IsAssignableFrom(methodCallArguments[index].Type))
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
