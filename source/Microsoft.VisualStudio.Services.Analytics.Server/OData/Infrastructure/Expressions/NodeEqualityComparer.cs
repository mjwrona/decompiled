// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.Expressions.NodeEqualityComparer
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.Expressions
{
  public class NodeEqualityComparer : IEqualityComparer<QueryNode>
  {
    public static readonly IEqualityComparer<QueryNode> Instance = (IEqualityComparer<QueryNode>) new NodeEqualityComparer();

    private NodeEqualityComparer()
    {
    }

    public int GetHashCode(QueryNode obj) => throw new NotImplementedException();

    public bool Equals(QueryNode x, QueryNode y)
    {
      QueryNode queryNode1 = x;
      QueryNode queryNode2 = y;
      if (queryNode1 == null || queryNode2 == null)
        return queryNode1 == queryNode2;
      if (queryNode1.Kind != queryNode2.Kind)
        return false;
      switch (queryNode1.Kind)
      {
        case QueryNodeKind.None:
          return true;
        case QueryNodeKind.Constant:
          return ((ConstantNode) queryNode1).LiteralText == ((ConstantNode) queryNode1).LiteralText;
        case QueryNodeKind.Convert:
          ConvertNode convertNode1 = (ConvertNode) queryNode1;
          ConvertNode convertNode2 = (ConvertNode) queryNode2;
          return convertNode1.TypeReference == convertNode2.TypeReference && this.Equals((QueryNode) convertNode1.Source, (QueryNode) convertNode2.Source);
        case QueryNodeKind.BinaryOperator:
          BinaryOperatorNode binaryOperatorNode1 = (BinaryOperatorNode) queryNode1;
          BinaryOperatorNode binaryOperatorNode2 = (BinaryOperatorNode) queryNode2;
          return binaryOperatorNode1.OperatorKind == binaryOperatorNode2.OperatorKind && this.Equals((QueryNode) binaryOperatorNode1.Left, (QueryNode) binaryOperatorNode2.Left) && this.Equals((QueryNode) binaryOperatorNode1.Right, (QueryNode) binaryOperatorNode2.Right);
        case QueryNodeKind.UnaryOperator:
          UnaryOperatorNode unaryOperatorNode1 = (UnaryOperatorNode) queryNode1;
          UnaryOperatorNode unaryOperatorNode2 = (UnaryOperatorNode) queryNode2;
          return unaryOperatorNode1.OperatorKind == unaryOperatorNode2.OperatorKind && this.Equals((QueryNode) unaryOperatorNode1.Operand, (QueryNode) unaryOperatorNode2.Operand);
        case QueryNodeKind.SingleValuePropertyAccess:
          SingleValuePropertyAccessNode propertyAccessNode1 = (SingleValuePropertyAccessNode) queryNode1;
          SingleValuePropertyAccessNode propertyAccessNode2 = (SingleValuePropertyAccessNode) queryNode2;
          return propertyAccessNode1.Property == propertyAccessNode2.Property && this.Equals((QueryNode) propertyAccessNode1.Source, (QueryNode) propertyAccessNode2.Source);
        case QueryNodeKind.CollectionPropertyAccess:
          CollectionPropertyAccessNode propertyAccessNode3 = (CollectionPropertyAccessNode) queryNode1;
          CollectionPropertyAccessNode propertyAccessNode4 = (CollectionPropertyAccessNode) queryNode2;
          return propertyAccessNode3.Property == propertyAccessNode4.Property && this.Equals((QueryNode) propertyAccessNode3.Source, (QueryNode) propertyAccessNode3.Source);
        case QueryNodeKind.SingleValueFunctionCall:
          SingleValueFunctionCallNode functionCallNode1 = (SingleValueFunctionCallNode) queryNode1;
          SingleValueFunctionCallNode functionCallNode2 = (SingleValueFunctionCallNode) queryNode2;
          return functionCallNode1.Name == functionCallNode2.Name && this.Equals(functionCallNode1.Source, functionCallNode2.Source) && functionCallNode1.Parameters.SequenceEqual<QueryNode>(functionCallNode2.Parameters, NodeEqualityComparer.Instance);
        case QueryNodeKind.Any:
          AnyNode anyNode1 = (AnyNode) queryNode1;
          AnyNode anyNode2 = (AnyNode) queryNode1;
          return this.Equals((QueryNode) anyNode1.Source, (QueryNode) anyNode2.Source) && this.Equals((QueryNode) anyNode1.Body, (QueryNode) anyNode2.Body);
        case QueryNodeKind.CollectionNavigationNode:
          CollectionNavigationNode collectionNavigationNode1 = (CollectionNavigationNode) queryNode1;
          CollectionNavigationNode collectionNavigationNode2 = (CollectionNavigationNode) queryNode2;
          return collectionNavigationNode1.NavigationProperty == collectionNavigationNode2.NavigationProperty && this.Equals((QueryNode) collectionNavigationNode1.Source, (QueryNode) collectionNavigationNode2.Source);
        case QueryNodeKind.SingleNavigationNode:
          SingleNavigationNode singleNavigationNode1 = (SingleNavigationNode) queryNode1;
          SingleNavigationNode singleNavigationNode2 = (SingleNavigationNode) queryNode2;
          return singleNavigationNode1.NavigationProperty == singleNavigationNode2.NavigationProperty && this.Equals((QueryNode) singleNavigationNode1.Source, (QueryNode) singleNavigationNode2.Source);
        case QueryNodeKind.SingleValueOpenPropertyAccess:
          SingleValueOpenPropertyAccessNode propertyAccessNode5 = (SingleValueOpenPropertyAccessNode) queryNode1;
          SingleValueOpenPropertyAccessNode propertyAccessNode6 = (SingleValueOpenPropertyAccessNode) queryNode2;
          return propertyAccessNode5.Name == propertyAccessNode6.Name && this.Equals((QueryNode) propertyAccessNode5.Source, (QueryNode) propertyAccessNode6.Source);
        case QueryNodeKind.All:
          AllNode allNode1 = (AllNode) queryNode1;
          AllNode allNode2 = (AllNode) queryNode2;
          return this.Equals((QueryNode) allNode1.Source, (QueryNode) allNode1.Source) && this.Equals((QueryNode) allNode1.Body, (QueryNode) allNode2.Body);
        case QueryNodeKind.ResourceRangeVariableReference:
          ResourceRangeVariableReferenceNode variableReferenceNode1 = (ResourceRangeVariableReferenceNode) queryNode1;
          ResourceRangeVariableReferenceNode variableReferenceNode2 = (ResourceRangeVariableReferenceNode) queryNode2;
          return variableReferenceNode1.Name == variableReferenceNode2.Name && variableReferenceNode1.NavigationSource == variableReferenceNode2.NavigationSource;
        case QueryNodeKind.CollectionOpenPropertyAccess:
          CollectionOpenPropertyAccessNode propertyAccessNode7 = (CollectionOpenPropertyAccessNode) queryNode1;
          CollectionOpenPropertyAccessNode propertyAccessNode8 = (CollectionOpenPropertyAccessNode) queryNode2;
          return propertyAccessNode7.Name == propertyAccessNode8.Name && this.Equals((QueryNode) propertyAccessNode7.Source, (QueryNode) propertyAccessNode7.Source);
        default:
          return false;
      }
    }
  }
}
