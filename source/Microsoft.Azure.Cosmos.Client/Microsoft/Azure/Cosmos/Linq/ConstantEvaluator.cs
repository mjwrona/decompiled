// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.ConstantEvaluator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal static class ConstantEvaluator
  {
    public static Expression PartialEval(
      Expression expression,
      Func<Expression, bool> fnCanBeEvaluated)
    {
      return new SubtreeEvaluator(Nominator.Nominate(expression, fnCanBeEvaluated)).Evaluate(expression);
    }

    public static Expression PartialEval(Expression expression) => ConstantEvaluator.PartialEval(expression, new Func<Expression, bool>(ConstantEvaluator.CanBeEvaluated));

    private static bool CanBeEvaluated(Expression expression)
    {
      if (expression is ConstantExpression constantExpression && constantExpression.Value is IQueryable)
        return false;
      if (expression is MethodCallExpression methodCallExpression)
      {
        Type declaringType = methodCallExpression.Method.DeclaringType;
        if (declaringType == typeof (Enumerable) || declaringType == typeof (Queryable) || declaringType == typeof (CosmosLinq))
          return false;
      }
      return expression.NodeType == ExpressionType.Constant && expression.Type == typeof (object) || expression.NodeType != ExpressionType.Parameter && expression.NodeType != ExpressionType.Lambda;
    }
  }
}
