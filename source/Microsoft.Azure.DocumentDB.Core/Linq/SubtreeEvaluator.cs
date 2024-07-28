// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.SubtreeEvaluator
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
{
  internal sealed class SubtreeEvaluator : ExpressionVisitor
  {
    private HashSet<Expression> candidates;

    public SubtreeEvaluator(HashSet<Expression> candidates) => this.candidates = candidates;

    public Expression Evaluate(Expression expression) => this.Visit(expression);

    public override Expression Visit(Expression expression)
    {
      if (expression == null)
        return (Expression) null;
      return this.candidates.Contains(expression) ? this.EvaluateConstant(expression) : base.Visit(expression);
    }

    protected override Expression VisitMemberInit(MemberInitExpression node) => (Expression) node;

    private Expression EvaluateConstant(Expression expression) => expression.NodeType == ExpressionType.Constant ? expression : (Expression) Expression.Constant(Expression.Lambda(expression).Compile().DynamicInvoke((object[]) null), expression.Type);
  }
}
