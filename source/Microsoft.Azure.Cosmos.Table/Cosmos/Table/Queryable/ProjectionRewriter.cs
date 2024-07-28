// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.ProjectionRewriter
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal class ProjectionRewriter : ALinqExpressionVisitor
  {
    private readonly ParameterExpression newLambdaParameter;
    private ParameterExpression oldLambdaParameter;
    private bool sucessfulRebind;

    private ProjectionRewriter(Type proposedParameterType) => this.newLambdaParameter = Expression.Parameter(proposedParameterType, "it");

    internal static LambdaExpression TryToRewrite(LambdaExpression le, Type proposedParameterType) => !ResourceBinder.PatternRules.MatchSingleArgumentLambda((Expression) le, out le) || !((IEnumerable<PropertyInfo>) le.Parameters[0].Type.GetProperties()).Any<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.PropertyType == proposedParameterType)) ? le : new ProjectionRewriter(proposedParameterType).Rebind(le);

    internal LambdaExpression Rebind(LambdaExpression lambda)
    {
      this.sucessfulRebind = true;
      this.oldLambdaParameter = lambda.Parameters[0];
      Expression body = this.Visit(lambda.Body);
      if (!this.sucessfulRebind)
        throw new NotSupportedException("Can only project the last entity type in the query being translated.");
      return Expression.Lambda(typeof (Func<,>).MakeGenericType(this.newLambdaParameter.Type, lambda.Body.Type), body, this.newLambdaParameter);
    }

    internal override Expression VisitMemberAccess(MemberExpression m)
    {
      if (m.Expression == this.oldLambdaParameter)
      {
        if (m.Type == this.newLambdaParameter.Type)
          return (Expression) this.newLambdaParameter;
        this.sucessfulRebind = false;
      }
      return base.VisitMemberAccess(m);
    }
  }
}
