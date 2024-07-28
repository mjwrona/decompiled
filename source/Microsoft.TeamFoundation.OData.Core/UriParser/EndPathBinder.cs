// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.EndPathBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;

namespace Microsoft.OData.UriParser
{
  internal sealed class EndPathBinder : BinderBase
  {
    private readonly FunctionCallBinder functionCallBinder;

    internal EndPathBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
      : base(bindMethod, state)
    {
      this.functionCallBinder = new FunctionCallBinder(bindMethod, state);
    }

    internal static QueryNode GeneratePropertyAccessQueryNode(
      SingleResourceNode parentNode,
      IEdmProperty property,
      BindingState state)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleResourceNode>(parentNode, nameof (parentNode));
      ExceptionUtils.CheckArgumentNotNull<IEdmProperty>(property, nameof (property));
      if (property.Type.IsNonEntityCollectionType())
        return property.Type.IsStructuredCollectionType() ? (QueryNode) new CollectionComplexNode(parentNode, property) : (QueryNode) new CollectionPropertyAccessNode((SingleValueNode) parentNode, property);
      if (property.PropertyKind == EdmPropertyKind.Navigation)
      {
        IEdmNavigationProperty navigationProperty = (IEdmNavigationProperty) property;
        return navigationProperty.TargetMultiplicity() == EdmMultiplicity.Many ? (QueryNode) new CollectionNavigationNode(parentNode, navigationProperty, state.ParsedSegments) : (QueryNode) new SingleNavigationNode(parentNode, navigationProperty, state.ParsedSegments);
      }
      return property.Type.IsComplex() ? (QueryNode) new SingleComplexNode(parentNode, property) : (QueryNode) new SingleValuePropertyAccessNode((SingleValueNode) parentNode, property);
    }

    internal static SingleValueNode CreateParentFromImplicitRangeVariable(BindingState state)
    {
      ExceptionUtils.CheckArgumentNotNull<BindingState>(state, nameof (state));
      return state.ImplicitRangeVariable != null ? NodeFactory.CreateRangeVariableReferenceNode(state.ImplicitRangeVariable) : throw new ODataException(Microsoft.OData.Strings.MetadataBinder_PropertyAccessWithoutParentParameter);
    }

    internal SingleValueOpenPropertyAccessNode GeneratePropertyAccessQueryForOpenType(
      EndPathToken endPathToken,
      SingleValueNode parentNode)
    {
      if (parentNode.TypeReference == null || parentNode.TypeReference.Definition.IsOpen() || this.IsAggregatedProperty(endPathToken))
        return new SingleValueOpenPropertyAccessNode(parentNode, endPathToken.Identifier);
      throw ExceptionUtil.CreatePropertyNotFoundException(endPathToken.Identifier, parentNode.TypeReference.FullName(), this.state.IsCollapsed);
    }

    internal QueryNode BindEndPath(EndPathToken endPathToken)
    {
      ExceptionUtils.CheckArgumentNotNull<EndPathToken>(endPathToken, "EndPathToken");
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(endPathToken.Identifier, "EndPathToken.Identifier");
      QueryNode parentNode = this.DetermineParentNode(endPathToken);
      switch (parentNode)
      {
        case SingleValueNode singleValueNode:
          if (endPathToken.Identifier == "$count")
            return (QueryNode) new CountVirtualPropertyNode();
          if (this.state.IsCollapsed && !this.IsAggregatedProperty(endPathToken))
            throw new ODataException(Microsoft.OData.Strings.ApplyBinder_GroupByPropertyNotPropertyAccessValue((object) endPathToken.Identifier));
          IEdmStructuredTypeReference type = singleValueNode.TypeReference == null ? (IEdmStructuredTypeReference) null : singleValueNode.TypeReference.AsStructuredOrNull();
          IEdmProperty property1 = type == null ? (IEdmProperty) null : this.Resolver.ResolveProperty(type.StructuredDefinition(), endPathToken.Identifier);
          if (property1 != null)
            return EndPathBinder.GeneratePropertyAccessQueryNode(singleValueNode as SingleResourceNode, property1, this.state);
          QueryNode boundFunction1;
          return this.functionCallBinder.TryBindEndPathAsFunctionCall(endPathToken, (QueryNode) singleValueNode, this.state, out boundFunction1) ? boundFunction1 : (QueryNode) this.GeneratePropertyAccessQueryForOpenType(endPathToken, singleValueNode);
        case CollectionNode source1 when endPathToken.Identifier.Equals("$count"):
          return (QueryNode) new CountNode(source1);
        case CollectionNavigationNode source2:
          IEdmProperty property2 = this.Resolver.ResolveProperty(source2.EntityItemType.StructuredDefinition(), endPathToken.Identifier);
          if (property2.PropertyKind == EdmPropertyKind.Structural && !property2.Type.IsCollection() && this.state.InEntitySetAggregation)
            return (QueryNode) new AggregatedCollectionPropertyNode(source2, property2);
          break;
      }
      QueryNode boundFunction2;
      if (this.functionCallBinder.TryBindEndPathAsFunctionCall(endPathToken, parentNode, this.state, out boundFunction2))
        return boundFunction2;
      throw new ODataException(Microsoft.OData.Strings.MetadataBinder_PropertyAccessSourceNotSingleValue((object) endPathToken.Identifier));
    }

    private QueryNode DetermineParentNode(EndPathToken segmentToken)
    {
      ExceptionUtils.CheckArgumentNotNull<EndPathToken>(segmentToken, nameof (segmentToken));
      ExceptionUtils.CheckArgumentNotNull<BindingState>(this.state, "state");
      return segmentToken.NextToken != null ? this.bindMethod(segmentToken.NextToken) : (QueryNode) NodeFactory.CreateRangeVariableReferenceNode(this.state.ImplicitRangeVariable);
    }

    private bool IsAggregatedProperty(EndPathToken endPath) => this.state?.AggregatedPropertyNames?.Contains(endPath) ?? false;
  }
}
