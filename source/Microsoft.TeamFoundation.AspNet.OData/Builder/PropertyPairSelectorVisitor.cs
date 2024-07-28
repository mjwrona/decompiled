// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.PropertyPairSelectorVisitor
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  internal class PropertyPairSelectorVisitor : ExpressionVisitor
  {
    private readonly IDictionary<PropertyInfo, PropertyInfo> _properties = (IDictionary<PropertyInfo, PropertyInfo>) new Dictionary<PropertyInfo, PropertyInfo>();

    public IDictionary<PropertyInfo, PropertyInfo> Properties => this._properties;

    public static IDictionary<PropertyInfo, PropertyInfo> GetSelectedProperty(Expression exp)
    {
      PropertyPairSelectorVisitor pairSelectorVisitor = new PropertyPairSelectorVisitor();
      pairSelectorVisitor.Visit(exp);
      return pairSelectorVisitor.Properties;
    }

    public override Expression Visit(Expression exp)
    {
      if (exp == null)
        return (Expression) null;
      switch (exp.NodeType)
      {
        case ExpressionType.And:
        case ExpressionType.AndAlso:
          BinaryExpression binaryExpression = (BinaryExpression) exp;
          this.Visit(binaryExpression.Left);
          return this.Visit(binaryExpression.Right);
        case ExpressionType.Equal:
          return this.VisitEqual(exp);
        case ExpressionType.Lambda:
          return base.Visit(exp);
        default:
          throw Error.NotSupported(SRResources.UnsupportedExpressionNodeTypeWithName, (object) exp.NodeType);
      }
    }

    protected override Expression VisitLambda<T>(Expression<T> lambda)
    {
      if (lambda == null)
        throw Error.ArgumentNull(nameof (lambda));
      if (lambda.Parameters.Count != 2)
        throw Error.InvalidOperation(SRResources.LambdaExpressionMustHaveExactlyTwoParameters);
      Expression body = this.Visit(lambda.Body);
      return body != lambda.Body ? (Expression) Expression.Lambda(lambda.Type, body, (IEnumerable<ParameterExpression>) lambda.Parameters) : (Expression) lambda;
    }

    private Expression VisitEqual(Expression exp)
    {
      BinaryExpression binaryExpression = (BinaryExpression) exp;
      PropertyInfo key = this.VisitMemberProperty(binaryExpression.Left);
      PropertyInfo propertyInfo = this.VisitMemberProperty(binaryExpression.Right);
      if (key != (PropertyInfo) null && propertyInfo != (PropertyInfo) null)
      {
        Type type1 = Nullable.GetUnderlyingType(key.PropertyType);
        if ((object) type1 == null)
          type1 = key.PropertyType;
        Type type2 = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
        if ((object) type2 == null)
          type2 = propertyInfo.PropertyType;
        if (type1 != type2)
          throw Error.InvalidOperation(SRResources.EqualExpressionsMustHaveSameTypes, (object) TypeHelper.GetReflectedType((MemberInfo) key).FullName, (object) key.Name, (object) key.PropertyType.FullName, (object) TypeHelper.GetReflectedType((MemberInfo) propertyInfo).FullName, (object) propertyInfo.Name, (object) propertyInfo.PropertyType.FullName);
        this._properties.Add(key, propertyInfo);
      }
      return exp;
    }

    private PropertyInfo VisitMemberProperty(Expression node)
    {
      switch (node.NodeType)
      {
        case ExpressionType.Convert:
          return this.VisitMemberProperty(((UnaryExpression) node).Operand);
        case ExpressionType.MemberAccess:
          return PropertyPairSelectorVisitor.GetPropertyInfo((MemberExpression) node);
        default:
          return (PropertyInfo) null;
      }
    }

    private static PropertyInfo GetPropertyInfo(MemberExpression memberNode)
    {
      PropertyInfo member = memberNode.Member as PropertyInfo;
      if (member == (PropertyInfo) null)
        throw Error.InvalidOperation(SRResources.MemberExpressionsMustBeProperties, (object) TypeHelper.GetReflectedType(memberNode.Member).FullName, (object) memberNode.Member.Name);
      if (memberNode.Expression.NodeType != ExpressionType.Parameter)
        throw Error.InvalidOperation(SRResources.MemberExpressionsMustBeBoundToLambdaParameter);
      return member;
    }
  }
}
