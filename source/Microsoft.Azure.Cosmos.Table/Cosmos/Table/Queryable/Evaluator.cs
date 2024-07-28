// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.Evaluator
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal static class Evaluator
  {
    internal static Expression PartialEval(
      Expression expression,
      Func<Expression, bool> canBeEvaluated)
    {
      return new Evaluator.SubtreeEvaluator(new Evaluator.Nominator(canBeEvaluated).Nominate(expression)).Eval(expression);
    }

    internal static Expression PartialEval(Expression expression) => Evaluator.PartialEval(expression, new Func<Expression, bool>(Evaluator.CanBeEvaluatedLocally));

    private static bool CanBeEvaluatedLocally(Expression expression) => expression.NodeType != ExpressionType.Parameter && expression.NodeType != ExpressionType.Lambda && expression.NodeType != (ExpressionType) 10000;

    internal class SubtreeEvaluator : DataServiceALinqExpressionVisitor
    {
      private HashSet<Expression> candidates;

      internal SubtreeEvaluator(HashSet<Expression> candidates) => this.candidates = candidates;

      internal Expression Eval(Expression exp) => this.Visit(exp);

      internal override Expression Visit(Expression exp)
      {
        if (exp == null)
          return (Expression) null;
        return this.candidates.Contains(exp) ? Evaluator.SubtreeEvaluator.Evaluate(exp) : base.Visit(exp);
      }

      private static Expression Evaluate(Expression e)
      {
        if (e.NodeType == ExpressionType.Constant)
          return e;
        object obj = Expression.Lambda(e).Compile().DynamicInvoke((object[]) null);
        Type type = e.Type;
        if (obj != null && type.IsArray && type.GetElementType() == obj.GetType().GetElementType())
          type = obj.GetType();
        return (Expression) Expression.Constant(obj, type);
      }
    }

    internal class Nominator : DataServiceALinqExpressionVisitor
    {
      private Func<Expression, bool> functionCanBeEvaluated;
      private HashSet<Expression> candidates;
      private bool cannotBeEvaluated;

      internal Nominator(Func<Expression, bool> functionCanBeEvaluated) => this.functionCanBeEvaluated = functionCanBeEvaluated;

      internal HashSet<Expression> Nominate(Expression expression)
      {
        this.candidates = new HashSet<Expression>((IEqualityComparer<Expression>) EqualityComparer<Expression>.Default);
        this.Visit(expression);
        return this.candidates;
      }

      internal override Expression Visit(Expression expression)
      {
        if (expression != null)
        {
          bool cannotBeEvaluated = this.cannotBeEvaluated;
          this.cannotBeEvaluated = false;
          base.Visit(expression);
          if (!this.cannotBeEvaluated)
          {
            if (this.functionCanBeEvaluated(expression))
              this.candidates.Add(expression);
            else
              this.cannotBeEvaluated = true;
          }
          this.cannotBeEvaluated |= cannotBeEvaluated;
        }
        return expression;
      }
    }
  }
}
