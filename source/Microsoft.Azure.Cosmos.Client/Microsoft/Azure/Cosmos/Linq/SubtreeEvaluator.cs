// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.SubtreeEvaluator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal sealed class SubtreeEvaluator : ExpressionVisitor
  {
    private readonly HashSet<Expression> candidates;

    public SubtreeEvaluator(HashSet<Expression> candidates) => this.candidates = candidates;

    public Expression Evaluate(Expression expression) => this.Visit(expression);

    public override Expression Visit(Expression expression)
    {
      if (expression == null)
        return (Expression) null;
      return this.candidates.Contains(expression) ? this.EvaluateConstant(expression) : base.Visit(expression);
    }

    protected override Expression VisitMemberInit(MemberInitExpression node) => (Expression) node;

    private Expression EvaluateMemberAccess(Expression expression)
    {
      while (expression != null && expression.CanReduce)
        expression = expression.Reduce();
      if (!(expression is MemberExpression memberExpression))
        return expression;
      Expression memberAccess = this.EvaluateMemberAccess(memberExpression.Expression);
      ConstantExpression constantExpression = memberAccess as ConstantExpression;
      if (memberAccess != null && constantExpression == null)
        return expression;
      if (constantExpression != null && constantExpression.Value == null && Nullable.GetUnderlyingType(constantExpression.Type) != (Type) null && memberExpression.Member.Name == "HasValue")
        return (Expression) Expression.Constant((object) false);
      FieldInfo member1 = memberExpression.Member as FieldInfo;
      if ((object) member1 != null)
        return (Expression) Expression.Constant(member1.GetValue(constantExpression?.Value), memberExpression.Type);
      PropertyInfo member2 = memberExpression.Member as PropertyInfo;
      return (object) member2 != null ? (Expression) Expression.Constant(member2.GetValue(constantExpression?.Value), memberExpression.Type) : expression;
    }

    private Expression EvaluateConstant(Expression expression)
    {
      expression = this.EvaluateMemberAccess(expression);
      return expression.NodeType == ExpressionType.Constant ? expression : (Expression) Expression.Constant(Expression.Lambda(expression).Compile().DynamicInvoke((object[]) null), expression.Type);
    }
  }
}
