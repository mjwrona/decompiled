// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.ConstantEvaluator
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
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
        if ((object) declaringType == (object) typeof (Enumerable) || (object) declaringType == (object) typeof (Queryable) || (object) declaringType == (object) typeof (UserDefinedFunctionProvider))
          return false;
      }
      return expression.NodeType == ExpressionType.Constant && (object) expression.Type == (object) typeof (object) || expression.NodeType != ExpressionType.Parameter && expression.NodeType != ExpressionType.Lambda;
    }
  }
}
