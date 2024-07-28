// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.NodeFactory
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal static class NodeFactory
  {
    internal static RangeVariable CreateImplicitRangeVariable(ODataPath path)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataPath>(path, nameof (path));
      IEdmTypeReference edmTypeReference = path.EdmType();
      if (edmTypeReference == null)
        return (RangeVariable) null;
      if (edmTypeReference.IsCollection())
        edmTypeReference = edmTypeReference.AsCollection().ElementType();
      return edmTypeReference.IsStructured() ? (RangeVariable) new ResourceRangeVariable("$it", edmTypeReference.AsStructured(), path.NavigationSource()) : (RangeVariable) new NonResourceRangeVariable("$it", edmTypeReference, (CollectionNode) null);
    }

    internal static RangeVariable CreateImplicitRangeVariable(
      IEdmTypeReference elementType,
      IEdmNavigationSource navigationSource)
    {
      return elementType.IsStructured() ? (RangeVariable) new ResourceRangeVariable("$it", elementType as IEdmStructuredTypeReference, navigationSource) : (RangeVariable) new NonResourceRangeVariable("$it", elementType, (CollectionNode) null);
    }

    internal static SingleValueNode CreateRangeVariableReferenceNode(RangeVariable rangeVariable)
    {
      if (rangeVariable.Kind == 1)
        return (SingleValueNode) new NonResourceRangeVariableReferenceNode(rangeVariable.Name, (NonResourceRangeVariable) rangeVariable);
      ResourceRangeVariable rangeVariable1 = (ResourceRangeVariable) rangeVariable;
      return (SingleValueNode) new ResourceRangeVariableReferenceNode(rangeVariable1.Name, rangeVariable1);
    }

    internal static RangeVariable CreateParameterNode(
      string parameter,
      CollectionNode nodeToIterateOver)
    {
      IEdmTypeReference itemType = nodeToIterateOver.ItemType;
      if (itemType == null || !itemType.IsStructured())
        return (RangeVariable) new NonResourceRangeVariable(parameter, itemType, (CollectionNode) null);
      CollectionResourceNode collectionResourceNode = nodeToIterateOver as CollectionResourceNode;
      return (RangeVariable) new ResourceRangeVariable(parameter, itemType as IEdmStructuredTypeReference, collectionResourceNode);
    }

    internal static LambdaNode CreateLambdaNode(
      BindingState state,
      CollectionNode parent,
      SingleValueNode lambdaExpression,
      RangeVariable newRangeVariable,
      QueryTokenKind queryTokenKind)
    {
      LambdaNode lambdaNode;
      if (queryTokenKind == QueryTokenKind.Any)
      {
        AnyNode anyNode = new AnyNode(new Collection<RangeVariable>((IList<RangeVariable>) state.RangeVariables.ToList<RangeVariable>()), newRangeVariable);
        anyNode.Body = lambdaExpression;
        anyNode.Source = parent;
        lambdaNode = (LambdaNode) anyNode;
      }
      else
      {
        AllNode allNode = new AllNode(new Collection<RangeVariable>((IList<RangeVariable>) state.RangeVariables.ToList<RangeVariable>()), newRangeVariable);
        allNode.Body = lambdaExpression;
        allNode.Source = parent;
        lambdaNode = (LambdaNode) allNode;
      }
      return lambdaNode;
    }
  }
}
