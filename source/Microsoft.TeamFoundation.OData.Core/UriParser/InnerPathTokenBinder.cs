// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.InnerPathTokenBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal sealed class InnerPathTokenBinder : BinderBase
  {
    internal InnerPathTokenBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
      : base(bindMethod, state)
    {
    }

    internal static SingleResourceNode EnsureParentIsResourceForNavProp(SingleValueNode parent)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValueNode>(parent, nameof (parent));
      return parent is SingleResourceNode singleResourceNode ? singleResourceNode : throw new ODataException(Microsoft.OData.Strings.MetadataBinder_NavigationPropertyNotFollowingSingleEntityType);
    }

    internal static IEdmProperty BindProperty(
      IEdmTypeReference parentReference,
      string propertyName,
      ODataUriResolver resolver)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataUriResolver>(resolver, nameof (resolver));
      IEdmStructuredTypeReference type = parentReference == null ? (IEdmStructuredTypeReference) null : parentReference.AsStructuredOrNull();
      return type != null ? resolver.ResolveProperty(type.StructuredDefinition(), propertyName) : (IEdmProperty) null;
    }

    internal static QueryNode GetNavigationNode(
      IEdmNavigationProperty property,
      SingleResourceNode parent,
      IEnumerable<NamedValue> namedValues,
      BindingState state,
      KeyBinder keyBinder,
      out IEdmNavigationSource navigationSource)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmNavigationProperty>(property, nameof (property));
      ExceptionUtils.CheckArgumentNotNull<SingleResourceNode>(parent, nameof (parent));
      ExceptionUtils.CheckArgumentNotNull<BindingState>(state, nameof (state));
      ExceptionUtils.CheckArgumentNotNull<KeyBinder>(keyBinder, nameof (keyBinder));
      if (property.TargetMultiplicity() == EdmMultiplicity.Many)
      {
        CollectionNavigationNode collectionNode = new CollectionNavigationNode(parent, property, state.ParsedSegments);
        navigationSource = collectionNode.NavigationSource;
        return namedValues != null ? keyBinder.BindKeyValues((CollectionResourceNode) collectionNode, namedValues, state.Model) : (QueryNode) collectionNode;
      }
      SingleNavigationNode navigationNode = new SingleNavigationNode(parent, property, state.ParsedSegments);
      navigationSource = navigationNode.NavigationSource;
      return (QueryNode) navigationNode;
    }

    internal QueryNode BindInnerPathSegment(InnerPathToken segmentToken)
    {
      FunctionCallBinder functionCallBinder = new FunctionCallBinder(this.bindMethod, this.state);
      QueryNode parentNode = this.DetermineParentNode(segmentToken, this.state);
      if (!(parentNode is SingleValueNode singleValueNode))
      {
        QueryNode boundFunction;
        if (functionCallBinder.TryBindInnerPathAsFunctionCall(segmentToken, parentNode, out boundFunction))
          return boundFunction;
        if (parentNode is CollectionNavigationNode source)
        {
          IEdmProperty property = this.Resolver.ResolveProperty(source.EntityItemType.StructuredDefinition(), segmentToken.Identifier);
          if (property != null && property.PropertyKind == EdmPropertyKind.Structural)
            return (QueryNode) new AggregatedCollectionPropertyNode(source, property);
        }
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_PropertyAccessSourceNotSingleValue((object) segmentToken.Identifier));
      }
      IEdmProperty property1 = InnerPathTokenBinder.BindProperty(singleValueNode.TypeReference, segmentToken.Identifier, this.Resolver);
      if (property1 == null)
      {
        QueryNode boundFunction;
        if (functionCallBinder.TryBindInnerPathAsFunctionCall(segmentToken, parentNode, out boundFunction))
          return boundFunction;
        if (singleValueNode.TypeReference != null && !singleValueNode.TypeReference.Definition.IsOpen())
          throw ExceptionUtil.CreatePropertyNotFoundException(segmentToken.Identifier, parentNode.GetEdmTypeReference().FullName());
        return (QueryNode) new SingleValueOpenPropertyAccessNode(singleValueNode, segmentToken.Identifier);
      }
      IEdmStructuralProperty property2 = property1 as IEdmStructuralProperty;
      if (property1.Type.IsComplex())
      {
        this.state.ParsedSegments.Add((ODataPathSegment) new PropertySegment(property2));
        return (QueryNode) new SingleComplexNode(singleValueNode as SingleResourceNode, property1);
      }
      if (property1.Type.IsPrimitive())
        return (QueryNode) new SingleValuePropertyAccessNode(singleValueNode, property1);
      if (property1.Type.IsNonEntityCollectionType())
      {
        if (!property1.Type.IsStructuredCollectionType())
          return (QueryNode) new CollectionPropertyAccessNode(singleValueNode, property1);
        this.state.ParsedSegments.Add((ODataPathSegment) new PropertySegment(property2));
        return (QueryNode) new CollectionComplexNode(singleValueNode as SingleResourceNode, property1);
      }
      if (!(property1 is IEdmNavigationProperty navigationProperty))
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_IllegalSegmentType((object) property1.Name));
      SingleResourceNode parent = InnerPathTokenBinder.EnsureParentIsResourceForNavProp(singleValueNode);
      IEdmNavigationSource navigationSource;
      QueryNode navigationNode = InnerPathTokenBinder.GetNavigationNode(navigationProperty, parent, segmentToken.NamedValues, this.state, new KeyBinder(this.bindMethod), out navigationSource);
      this.state.ParsedSegments.Add((ODataPathSegment) new NavigationPropertySegment(navigationProperty, navigationSource));
      return navigationNode;
    }

    private QueryNode DetermineParentNode(InnerPathToken segmentToken, BindingState state)
    {
      ExceptionUtils.CheckArgumentNotNull<InnerPathToken>(segmentToken, nameof (segmentToken));
      ExceptionUtils.CheckArgumentNotNull<BindingState>(state, nameof (state));
      return segmentToken.NextToken != null ? this.bindMethod(segmentToken.NextToken) : (QueryNode) NodeFactory.CreateRangeVariableReferenceNode(state.ImplicitRangeVariable);
    }
  }
}
