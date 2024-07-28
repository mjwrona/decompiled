// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.Expressions.QueryNodeRewriter
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.Expressions
{
  public class QueryNodeRewriter : QueryNodeVisitor<QueryNode>
  {
    public override QueryNode Visit(AllNode nodeIn)
    {
      SingleValueNode singleValueNode = (SingleValueNode) nodeIn.Body.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      CollectionNode collectionNode = (CollectionNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (singleValueNode != nodeIn.Body || collectionNode != nodeIn.Source)
      {
        AllNode allNode = new AllNode(nodeIn.RangeVariables, nodeIn.CurrentRangeVariable);
        allNode.Body = singleValueNode;
        allNode.Source = collectionNode;
        nodeIn = allNode;
      }
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(AnyNode nodeIn)
    {
      SingleValueNode singleValueNode = (SingleValueNode) nodeIn.Body.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      CollectionNode collectionNode = (CollectionNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (singleValueNode != nodeIn.Body || collectionNode != nodeIn.Source)
      {
        AnyNode anyNode = new AnyNode(nodeIn.RangeVariables, nodeIn.CurrentRangeVariable);
        anyNode.Body = singleValueNode;
        anyNode.Source = collectionNode;
        nodeIn = anyNode;
      }
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(BinaryOperatorNode nodeIn)
    {
      SingleValueNode left = (SingleValueNode) nodeIn.Left.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      SingleValueNode right = (SingleValueNode) nodeIn.Right.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (left != nodeIn.Left || right != nodeIn.Right)
        nodeIn = new BinaryOperatorNode(nodeIn.OperatorKind, left, right);
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(CollectionComplexNode nodeIn)
    {
      SingleResourceNode source = (SingleResourceNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (source != nodeIn.Source)
        nodeIn = new CollectionComplexNode(source, nodeIn.Property);
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(CollectionNavigationNode nodeIn)
    {
      SingleResourceNode source = (SingleResourceNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (source != nodeIn.Source)
        nodeIn = new CollectionNavigationNode(source, nodeIn.NavigationProperty, nodeIn.BindingPath);
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(CollectionOpenPropertyAccessNode nodeIn)
    {
      SingleValueNode source = (SingleValueNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (source != nodeIn.Source)
        nodeIn = new CollectionOpenPropertyAccessNode(source, nodeIn.Name);
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(CollectionPropertyAccessNode nodeIn)
    {
      SingleValueNode source = (SingleValueNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (source != nodeIn.Source)
        nodeIn = new CollectionPropertyAccessNode(source, nodeIn.Property);
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(CollectionResourceCastNode nodeIn)
    {
      CollectionResourceNode source = (CollectionResourceNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (source != nodeIn.Source)
        nodeIn = new CollectionResourceCastNode(source, (IEdmStructuredType) nodeIn.ItemType.Definition);
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(ConstantNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(ConvertNode nodeIn)
    {
      SingleValueNode source = (SingleValueNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (source != nodeIn.Source)
        nodeIn = new ConvertNode(source, nodeIn.TypeReference);
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(CountNode nodeIn)
    {
      CollectionNode source = (CollectionNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (source != nodeIn.Source)
        nodeIn = new CountNode(source);
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(ParameterAliasNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(SearchTermNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(SingleComplexNode nodeIn)
    {
      SingleResourceNode source = (SingleResourceNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (source != nodeIn.Source)
        nodeIn = new SingleComplexNode(source, nodeIn.Property);
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(SingleValueFunctionCallNode nodeIn)
    {
      QueryNode source = nodeIn.Source?.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      IEnumerable<QueryNode> parameters1 = nodeIn.Parameters;
      IEnumerable<QueryNode> parameters2 = parameters1 != null ? parameters1.Select<QueryNode, QueryNode>((Func<QueryNode, QueryNode>) (n => n.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this))) : (IEnumerable<QueryNode>) null;
      return (QueryNode) new SingleValueFunctionCallNode(nodeIn.Name, nodeIn.Functions, parameters2, nodeIn.TypeReference, source);
    }

    public override QueryNode Visit(SingleValueOpenPropertyAccessNode nodeIn)
    {
      SingleValueNode source = (SingleValueNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (source != nodeIn.Source)
        nodeIn = new SingleValueOpenPropertyAccessNode(source, nodeIn.Name);
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(SingleValuePropertyAccessNode nodeIn)
    {
      SingleValueNode source = (SingleValueNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (source != nodeIn.Source)
        nodeIn = new SingleValuePropertyAccessNode(source, nodeIn.Property);
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(UnaryOperatorNode nodeIn)
    {
      SingleValueNode operand = (SingleValueNode) nodeIn.Operand.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (operand != nodeIn.Operand)
        nodeIn = new UnaryOperatorNode(nodeIn.OperatorKind, operand);
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(CollectionFunctionCallNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(CollectionResourceFunctionCallNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(NamedFunctionParameterNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(NonResourceRangeVariableReferenceNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(ResourceRangeVariableReferenceNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(SingleNavigationNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(SingleResourceCastNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(SingleResourceFunctionCallNode nodeIn) => (QueryNode) nodeIn;
  }
}
