// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Validators.OrderByModelLimitationsValidator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.AspNet.OData.Query.Validators
{
  internal class OrderByModelLimitationsValidator : QueryNodeVisitor<SingleValueNode>
  {
    private readonly IEdmModel _model;
    private readonly bool _enableOrderBy;
    private IEdmProperty _property;
    private IEdmStructuredType _structuredType;

    public OrderByModelLimitationsValidator(ODataQueryContext context, bool enableOrderBy)
    {
      this._model = context.Model;
      this._enableOrderBy = enableOrderBy;
      if (context.Path == null)
        return;
      this._property = context.TargetProperty;
      this._structuredType = context.TargetStructuredType;
    }

    public bool TryValidate(
      IEdmProperty property,
      IEdmStructuredType structuredType,
      OrderByClause orderByClause,
      bool explicitPropertiesDefined)
    {
      this._property = property;
      this._structuredType = structuredType;
      return this.TryValidate(orderByClause, explicitPropertiesDefined);
    }

    public bool TryValidate(OrderByClause orderByClause, bool explicitPropertiesDefined)
    {
      SingleValueNode node = orderByClause.Expression.Accept<SingleValueNode>((QueryNodeVisitor<SingleValueNode>) this);
      if (node != null && !explicitPropertiesDefined)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotSortablePropertyUsedInOrderBy, (object) OrderByModelLimitationsValidator.GetPropertyName(node)));
      return node == null;
    }

    public override SingleValueNode Visit(SingleValuePropertyAccessNode nodeIn)
    {
      if (nodeIn.Source != null)
      {
        if (nodeIn.Source.Kind == QueryNodeKind.SingleNavigationNode)
        {
          SingleNavigationNode source = nodeIn.Source as SingleNavigationNode;
          if (EdmLibHelpers.IsNotSortable(nodeIn.Property, (IEdmProperty) source.NavigationProperty, (IEdmStructuredType) source.NavigationProperty.ToEntityType(), this._model, this._enableOrderBy))
            return (SingleValueNode) nodeIn;
        }
        else if (nodeIn.Source.Kind == QueryNodeKind.SingleComplexNode)
        {
          SingleComplexNode source = nodeIn.Source as SingleComplexNode;
          if (EdmLibHelpers.IsNotSortable(nodeIn.Property, source.Property, nodeIn.Property.DeclaringType, this._model, this._enableOrderBy))
            return (SingleValueNode) nodeIn;
        }
        else if (EdmLibHelpers.IsNotSortable(nodeIn.Property, this._property, this._structuredType, this._model, this._enableOrderBy))
          return (SingleValueNode) nodeIn;
      }
      return nodeIn.Source != null ? nodeIn.Source.Accept<SingleValueNode>((QueryNodeVisitor<SingleValueNode>) this) : (SingleValueNode) null;
    }

    public override SingleValueNode Visit(SingleComplexNode nodeIn)
    {
      if (EdmLibHelpers.IsNotSortable(nodeIn.Property, this._property, this._structuredType, this._model, this._enableOrderBy))
        return (SingleValueNode) nodeIn;
      return nodeIn.Source != null ? nodeIn.Source.Accept<SingleValueNode>((QueryNodeVisitor<SingleValueNode>) this) : (SingleValueNode) null;
    }

    public override SingleValueNode Visit(SingleNavigationNode nodeIn)
    {
      if (EdmLibHelpers.IsNotSortable((IEdmProperty) nodeIn.NavigationProperty, this._property, this._structuredType, this._model, this._enableOrderBy))
        return (SingleValueNode) nodeIn;
      return nodeIn.Source != null ? nodeIn.Source.Accept<SingleValueNode>((QueryNodeVisitor<SingleValueNode>) this) : (SingleValueNode) null;
    }

    public override SingleValueNode Visit(ResourceRangeVariableReferenceNode nodeIn) => (SingleValueNode) null;

    public override SingleValueNode Visit(NonResourceRangeVariableReferenceNode nodeIn) => (SingleValueNode) null;

    private static string GetPropertyName(SingleValueNode node)
    {
      if (node.Kind == QueryNodeKind.SingleNavigationNode)
        return ((SingleNavigationNode) node).NavigationProperty.Name;
      if (node.Kind == QueryNodeKind.SingleValuePropertyAccess)
        return ((SingleValuePropertyAccessNode) node).Property.Name;
      return node.Kind == QueryNodeKind.SingleComplexNode ? ((SingleComplexNode) node).Property.Name : (string) null;
    }
  }
}
