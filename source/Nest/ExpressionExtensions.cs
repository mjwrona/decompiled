// Decompiled with JetBrains decompiler
// Type: Nest.ExpressionExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public static class ExpressionExtensions
  {
    public static Expression<Func<T, object>> AppendSuffix<T>(
      this Expression<Func<T, object>> expression,
      string suffix)
    {
      return Expression.Lambda<Func<T, object>>(new ExpressionExtensions.SuffixExpressionVisitor(suffix).Visit(expression.Body), expression.Parameters[0]);
    }

    public static Expression<Func<T, TValue>> AppendSuffix<T, TValue>(
      this Expression<Func<T, TValue>> expression,
      string suffix)
    {
      return Expression.Lambda<Func<T, TValue>>(new ExpressionExtensions.SuffixExpressionVisitor(suffix).Visit(expression.Body), expression.Parameters[0]);
    }

    internal static object ComparisonValueFromExpression(
      this Expression expression,
      out Type type,
      out bool cachable)
    {
      type = (Type) null;
      cachable = false;
      switch (expression)
      {
        case null:
          return (object) null;
        case LambdaExpression lambdaExpression2:
          type = lambdaExpression2.Parameters.FirstOrDefault<ParameterExpression>()?.Type;
          break;
        case MemberExpression memberExpression:
          type = memberExpression.Member.DeclaringType;
          break;
        case MethodCallExpression methodCallExpression:
          if (!(methodCallExpression.Method.DeclaringType.FullName == "Microsoft.FSharp.Core.FuncConvert") || !(methodCallExpression.Arguments.FirstOrDefault<Expression>() is LambdaExpression lambdaExpression1))
            throw new Exception(string.Format("Unsupported {0}: {1}", (object) "MethodCallExpression", (object) expression));
          type = lambdaExpression1.Parameters.FirstOrDefault<ParameterExpression>()?.Type;
          break;
        default:
          throw new Exception("Expected LambdaExpression, MemberExpression or MethodCallExpression, received: " + expression.GetType().Name);
      }
      ToStringExpressionVisitor expressionVisitor = new ToStringExpressionVisitor();
      string str = expressionVisitor.Resolve(expression);
      cachable = expressionVisitor.Cachable;
      return (object) str;
    }

    private class SuffixExpressionVisitor : ExpressionVisitor
    {
      private readonly string _suffix;

      public SuffixExpressionVisitor(string suffix) => this._suffix = suffix;

      public override Expression Visit(Expression node) => (Expression) Expression.Call(typeof (SuffixExtensions), "Suffix", (Type[]) null, node, (Expression) Expression.Constant((object) this._suffix));

      protected override Expression VisitUnary(UnaryExpression node) => (Expression) node;
    }
  }
}
