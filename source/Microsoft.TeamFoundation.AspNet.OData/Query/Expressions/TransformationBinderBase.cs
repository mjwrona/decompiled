// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.TransformationBinderBase
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal class TransformationBinderBase : ExpressionBinderBase
  {
    protected Type _elementType;
    protected ParameterExpression _lambdaParameter;
    protected bool _classicEF;

    internal TransformationBinderBase(
      ODataQuerySettings settings,
      IWebApiAssembliesResolver assembliesResolver,
      Type elementType,
      IEdmModel model)
      : base(model, assembliesResolver, settings)
    {
      this._elementType = elementType;
      this._lambdaParameter = Expression.Parameter(this._elementType, "$it");
    }

    public Type ResultClrType { get; protected set; }

    internal virtual bool IsClassicEF(IQueryable query)
    {
      string str = query.Provider.GetType().Namespace;
      return str == "System.Data.Entity.Core.Objects.ELinq" || str == "System.Data.Entity.Internal.Linq";
    }

    protected void PreprocessQuery(IQueryable query)
    {
      this._classicEF = this.IsClassicEF(query);
      this.BaseQuery = query;
      this.EnsureFlattenedPropertyContainer(this._lambdaParameter);
    }

    protected Expression WrapConvert(Expression expression) => !this._classicEF && expression.Type.IsValueType ? (Expression) Expression.Convert(expression, typeof (object)) : expression;

    public override Expression Bind(QueryNode node, Expression baseElement = null)
    {
      SingleValueNode node1 = node as SingleValueNode;
      if (node != null)
        return this.BindAccessor((QueryNode) node1, baseElement);
      throw new ArgumentException("Only SigleValueNode supported", nameof (node));
    }

    protected override ParameterExpression ItParameter => this._lambdaParameter;

    protected Expression BindAccessor(QueryNode node, Expression baseElement = null)
    {
      switch (node.Kind)
      {
        case QueryNodeKind.None:
        case QueryNodeKind.SingleNavigationNode:
          SingleNavigationNode node1 = (SingleNavigationNode) node;
          return this.CreatePropertyAccessExpression(this.BindAccessor((QueryNode) node1.Source), (IEdmProperty) node1.NavigationProperty, this.GetFullPropertyPath((SingleValueNode) node1));
        case QueryNodeKind.Constant:
          return this.BindConstantNode(node as ConstantNode);
        case QueryNodeKind.Convert:
          ConvertNode convertNode = (ConvertNode) node;
          return this.CreateConvertExpression(convertNode, this.BindAccessor((QueryNode) convertNode.Source, baseElement));
        case QueryNodeKind.BinaryOperator:
          return this.BindBinaryOperatorNode((BinaryOperatorNode) node, baseElement);
        case QueryNodeKind.SingleValuePropertyAccess:
          SingleValuePropertyAccessNode node2 = node as SingleValuePropertyAccessNode;
          return this.CreatePropertyAccessExpression(this.BindAccessor((QueryNode) node2.Source, baseElement), node2.Property, this.GetFullPropertyPath((SingleValueNode) node2));
        case QueryNodeKind.SingleValueFunctionCall:
          return this.BindSingleValueFunctionCallNode(node as SingleValueFunctionCallNode);
        case QueryNodeKind.CollectionNavigationNode:
          return baseElement ?? (Expression) this._lambdaParameter;
        case QueryNodeKind.SingleValueOpenPropertyAccess:
          SingleValueOpenPropertyAccessNode openNode = node as SingleValueOpenPropertyAccessNode;
          return this.GetFlattenedPropertyExpression(openNode.Name) ?? this.CreateOpenPropertyAccessExpression(openNode);
        case QueryNodeKind.ResourceRangeVariableReference:
          return !this._lambdaParameter.Type.IsGenericType || !(this._lambdaParameter.Type.GetGenericTypeDefinition() == typeof (FlatteningWrapper<>)) ? baseElement ?? (Expression) this._lambdaParameter : (Expression) Expression.Property((Expression) this._lambdaParameter, "Source");
        case QueryNodeKind.CollectionResourceFunctionCall:
          return this.BindCustomMethodExpressionOrNull<CollectionResourceFunctionCallNode>(node as CollectionResourceFunctionCallNode);
        case QueryNodeKind.SingleComplexNode:
          SingleComplexNode node3 = node as SingleComplexNode;
          return this.CreatePropertyAccessExpression(this.BindAccessor((QueryNode) node3.Source, baseElement), node3.Property, this.GetFullPropertyPath((SingleValueNode) node3));
        case QueryNodeKind.AggregatedCollectionPropertyNode:
          AggregatedCollectionPropertyNode collectionPropertyNode = node as AggregatedCollectionPropertyNode;
          return this.CreatePropertyAccessExpression(this.BindAccessor((QueryNode) collectionPropertyNode.Source, baseElement), collectionPropertyNode.Property);
        default:
          throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.QueryNodeBindingNotSupported, (object) node.Kind, (object) typeof (AggregationBinder).Name);
      }
    }

    private Expression CreateOpenPropertyAccessExpression(SingleValueOpenPropertyAccessNode openNode)
    {
      Expression expression = this.BindAccessor((QueryNode) openNode.Source);
      if (expression.Type.GetProperty(openNode.Name) != (PropertyInfo) null)
        return (Expression) Expression.Property(expression, openNode.Name);
      PropertyInfo propertyContainer = this.GetDynamicPropertyContainer(openNode);
      MemberExpression memberExpression = Expression.Property(expression, propertyContainer.Name);
      IndexExpression ifTrue = Expression.Property((Expression) memberExpression, ExpressionBinderBase.DictionaryStringObjectIndexerName, (Expression) Expression.Constant((object) openNode.Name));
      MethodCallExpression methodCallExpression = Expression.Call((Expression) memberExpression, memberExpression.Type.GetMethod("ContainsKey"), (Expression) Expression.Constant((object) openNode.Name));
      ConstantExpression ifFalse = Expression.Constant((object) null);
      return this.QuerySettings.HandleNullPropagation == HandleNullPropagationOption.True ? (Expression) Expression.Condition((Expression) Expression.AndAlso((Expression) Expression.NotEqual((Expression) memberExpression, (Expression) Expression.Constant((object) null)), (Expression) methodCallExpression), (Expression) ifTrue, (Expression) ifFalse) : (Expression) Expression.Condition((Expression) methodCallExpression, (Expression) ifTrue, (Expression) ifFalse);
    }
  }
}
