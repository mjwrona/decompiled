// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.ParameterAliasNodeTranslator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Routing;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Query
{
  public class ParameterAliasNodeTranslator : QueryNodeVisitor<QueryNode>
  {
    private IDictionary<string, SingleValueNode> _parameterAliasNode;

    public ParameterAliasNodeTranslator(
      IDictionary<string, SingleValueNode> parameterAliasNodes)
    {
      this._parameterAliasNode = parameterAliasNodes != null ? parameterAliasNodes : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (parameterAliasNodes));
    }

    public override QueryNode Visit(AllNode nodeIn)
    {
      AllNode allNode = new AllNode(nodeIn.RangeVariables, nodeIn.CurrentRangeVariable);
      if (nodeIn.Source != null)
        allNode.Source = (CollectionNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (nodeIn.Body != null)
        allNode.Body = (SingleValueNode) nodeIn.Body.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      return (QueryNode) allNode;
    }

    public override QueryNode Visit(AnyNode nodeIn)
    {
      AnyNode anyNode = new AnyNode(nodeIn.RangeVariables, nodeIn.CurrentRangeVariable);
      if (nodeIn.Source != null)
        anyNode.Source = (CollectionNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (nodeIn.Body != null)
        anyNode.Body = (SingleValueNode) nodeIn.Body.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      return (QueryNode) anyNode;
    }

    public override QueryNode Visit(BinaryOperatorNode nodeIn) => (QueryNode) new BinaryOperatorNode(nodeIn.OperatorKind, (SingleValueNode) nodeIn.Left.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this), (SingleValueNode) nodeIn.Right.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this));

    public override QueryNode Visit(InNode nodeIn) => (QueryNode) new InNode((SingleValueNode) nodeIn.Left.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this), (CollectionNode) nodeIn.Right.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this));

    public override QueryNode Visit(CollectionFunctionCallNode nodeIn) => (QueryNode) new CollectionFunctionCallNode(nodeIn.Name, nodeIn.Functions, nodeIn.Parameters.Select<QueryNode, QueryNode>((Func<QueryNode, QueryNode>) (p => p.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this))), nodeIn.CollectionType, nodeIn.Source == null ? (QueryNode) null : nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this));

    public override QueryNode Visit(CollectionNavigationNode nodeIn) => nodeIn.Source != null ? (QueryNode) new CollectionNavigationNode((SingleResourceNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this), nodeIn.NavigationProperty, nodeIn.BindingPath ?? (IEdmPathExpression) new EdmPathExpression(nodeIn.NavigationProperty.Name)) : (QueryNode) nodeIn;

    public override QueryNode Visit(CollectionOpenPropertyAccessNode nodeIn) => (QueryNode) new CollectionOpenPropertyAccessNode((SingleValueNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this), nodeIn.Name);

    public override QueryNode Visit(CollectionComplexNode nodeIn) => (QueryNode) new CollectionComplexNode((SingleResourceNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this), nodeIn.Property);

    public override QueryNode Visit(CollectionPropertyAccessNode nodeIn) => (QueryNode) new CollectionPropertyAccessNode((SingleValueNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this), nodeIn.Property);

    public override QueryNode Visit(ConstantNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(CollectionConstantNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(ConvertNode nodeIn) => (QueryNode) new ConvertNode((SingleValueNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this), nodeIn.TypeReference);

    public override QueryNode Visit(CollectionResourceCastNode nodeIn) => (QueryNode) new CollectionResourceCastNode((CollectionResourceNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this), (IEdmStructuredType) nodeIn.ItemType.Definition);

    public override QueryNode Visit(CollectionResourceFunctionCallNode nodeIn) => (QueryNode) new CollectionResourceFunctionCallNode(nodeIn.Name, nodeIn.Functions, nodeIn.Parameters.Select<QueryNode, QueryNode>((Func<QueryNode, QueryNode>) (p => p.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this))), nodeIn.CollectionType, (IEdmEntitySetBase) nodeIn.NavigationSource, nodeIn.Source == null ? (QueryNode) null : nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this));

    public override QueryNode Visit(ResourceRangeVariableReferenceNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(NamedFunctionParameterNode nodeIn) => (QueryNode) new NamedFunctionParameterNode(nodeIn.Name, nodeIn.Value == null ? (QueryNode) null : nodeIn.Value.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this));

    public override QueryNode Visit(NonResourceRangeVariableReferenceNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(ParameterAliasNode nodeIn)
    {
      SingleValueNode singleValueNode = ODataPathSegmentTranslator.TranslateParameterAlias((SingleValueNode) nodeIn, this._parameterAliasNode);
      return singleValueNode == null ? (QueryNode) new ConstantNode((object) null) : singleValueNode.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
    }

    public override QueryNode Visit(SearchTermNode nodeIn) => (QueryNode) nodeIn;

    public override QueryNode Visit(SingleResourceCastNode nodeIn) => nodeIn.Source != null ? (QueryNode) new SingleResourceCastNode((SingleResourceNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this), (IEdmStructuredType) nodeIn.TypeReference.Definition) : (QueryNode) nodeIn;

    public override QueryNode Visit(SingleResourceFunctionCallNode nodeIn) => (QueryNode) new SingleResourceFunctionCallNode(nodeIn.Name, nodeIn.Functions, nodeIn.Parameters.Select<QueryNode, QueryNode>((Func<QueryNode, QueryNode>) (p => p.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this))), nodeIn.StructuredTypeReference, nodeIn.NavigationSource, nodeIn.Source == null ? (QueryNode) null : nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this));

    public override QueryNode Visit(SingleNavigationNode nodeIn) => nodeIn.Source != null ? (QueryNode) new SingleNavigationNode((SingleResourceNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this), nodeIn.NavigationProperty, nodeIn.BindingPath ?? (IEdmPathExpression) new EdmPathExpression(nodeIn.NavigationProperty.Name)) : (QueryNode) nodeIn;

    public override QueryNode Visit(SingleValueFunctionCallNode nodeIn) => (QueryNode) new SingleValueFunctionCallNode(nodeIn.Name, nodeIn.Functions, nodeIn.Parameters.Select<QueryNode, QueryNode>((Func<QueryNode, QueryNode>) (p => p.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this))), nodeIn.TypeReference, nodeIn.Source == null ? (QueryNode) null : nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this));

    public override QueryNode Visit(SingleValueOpenPropertyAccessNode nodeIn) => (QueryNode) new SingleValueOpenPropertyAccessNode((SingleValueNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this), nodeIn.Name);

    public override QueryNode Visit(SingleValuePropertyAccessNode nodeIn) => (QueryNode) new SingleValuePropertyAccessNode((SingleValueNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this), nodeIn.Property);

    public override QueryNode Visit(SingleComplexNode nodeIn) => (QueryNode) new SingleComplexNode((SingleResourceNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this), nodeIn.Property);

    public override QueryNode Visit(UnaryOperatorNode nodeIn) => (QueryNode) new UnaryOperatorNode(nodeIn.OperatorKind, (SingleValueNode) nodeIn.Operand.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this));

    public override QueryNode Visit(CountNode nodeIn) => (QueryNode) new CountNode((CollectionNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this));
  }
}
