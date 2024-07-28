// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.DataServiceALinqExpressionVisitor
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal abstract class DataServiceALinqExpressionVisitor : ALinqExpressionVisitor
  {
    internal override Expression Visit(Expression exp)
    {
      if (exp == null)
        return (Expression) null;
      switch ((ResourceExpressionType) exp.NodeType)
      {
        case ResourceExpressionType.RootResourceSet:
        case ResourceExpressionType.ResourceNavigationProperty:
          return this.VisitResourceSetExpression((ResourceSetExpression) exp);
        case ResourceExpressionType.ResourceNavigationPropertySingleton:
          return this.VisitNavigationPropertySingletonExpression((NavigationPropertySingletonExpression) exp);
        case ResourceExpressionType.InputReference:
          return this.VisitInputReferenceExpression((InputReferenceExpression) exp);
        default:
          return base.Visit(exp);
      }
    }

    internal virtual Expression VisitResourceSetExpression(ResourceSetExpression rse)
    {
      Expression source = this.Visit(rse.Source);
      if (source != rse.Source)
        rse = new ResourceSetExpression(rse.Type, source, rse.MemberExpression, rse.ResourceType, rse.ExpandPaths, rse.CountOption, rse.CustomQueryOptions, rse.Projection);
      return (Expression) rse;
    }

    internal virtual Expression VisitNavigationPropertySingletonExpression(
      NavigationPropertySingletonExpression npse)
    {
      Expression source = this.Visit(npse.Source);
      if (source != npse.Source)
        npse = new NavigationPropertySingletonExpression(npse.Type, source, (Expression) npse.MemberExpression, npse.MemberExpression.Type, npse.ExpandPaths, npse.CountOption, npse.CustomQueryOptions, npse.Projection);
      return (Expression) npse;
    }

    internal virtual Expression VisitInputReferenceExpression(InputReferenceExpression ire) => (Expression) ((ResourceExpression) this.Visit((Expression) ire.Target)).CreateReference();
  }
}
