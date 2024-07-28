// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.PropertySelectorVisitor
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  internal class PropertySelectorVisitor : ExpressionVisitor
  {
    private List<PropertyInfo> _properties = new List<PropertyInfo>();

    internal PropertySelectorVisitor(Expression exp) => this.Visit(exp);

    public PropertyInfo Property => this._properties.SingleOrDefault<PropertyInfo>();

    public ICollection<PropertyInfo> Properties => (ICollection<PropertyInfo>) this._properties;

    protected override Expression VisitMember(MemberExpression node)
    {
      PropertyInfo propertyInfo = node != null ? node.Member as PropertyInfo : throw Error.ArgumentNull(nameof (node));
      if (propertyInfo == (PropertyInfo) null)
        throw Error.InvalidOperation(SRResources.MemberExpressionsMustBeProperties, (object) TypeHelper.GetReflectedType(node.Member).FullName, (object) node.Member.Name);
      if (node.Expression.NodeType != ExpressionType.Parameter)
        throw Error.InvalidOperation(SRResources.MemberExpressionsMustBeBoundToLambdaParameter);
      this._properties.Add(propertyInfo);
      return (Expression) node;
    }

    public static PropertyInfo GetSelectedProperty(Expression exp) => new PropertySelectorVisitor(exp).Property;

    public static ICollection<PropertyInfo> GetSelectedProperties(Expression exp) => new PropertySelectorVisitor(exp).Properties;

    public override Expression Visit(Expression exp)
    {
      if (exp == null)
        return exp;
      switch (exp.NodeType)
      {
        case ExpressionType.Lambda:
        case ExpressionType.MemberAccess:
        case ExpressionType.New:
          return base.Visit(exp);
        default:
          throw Error.NotSupported(SRResources.UnsupportedExpressionNodeType);
      }
    }

    protected override Expression VisitLambda<T>(Expression<T> lambda)
    {
      if (lambda == null)
        throw Error.ArgumentNull(nameof (lambda));
      if (lambda.Parameters.Count != 1)
        throw Error.InvalidOperation(SRResources.LambdaExpressionMustHaveExactlyOneParameter);
      Expression body = this.Visit(lambda.Body);
      return body != lambda.Body ? (Expression) Expression.Lambda(lambda.Type, body, (IEnumerable<ParameterExpression>) lambda.Parameters) : (Expression) lambda;
    }
  }
}
