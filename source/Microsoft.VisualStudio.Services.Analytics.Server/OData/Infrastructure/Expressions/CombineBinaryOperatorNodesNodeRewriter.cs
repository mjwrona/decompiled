// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.Expressions.CombineBinaryOperatorNodesNodeRewriter
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.Services.Analytics.OData.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.Expressions
{
  public class CombineBinaryOperatorNodesNodeRewriter : QueryNodeRewriter
  {
    public const bool RewriteToPredicateNode = true;
    public const string PredicateToken = "{predicate}";
    public static readonly ConstantNode PredicateNode = new ConstantNode((object) string.Empty, "{predicate}", (IEdmTypeReference) null);
    private readonly IEdmModel m_model;

    public CombineBinaryOperatorNodesNodeRewriter(IEdmModel model) => this.m_model = model;

    public override QueryNode Visit(BinaryOperatorNode nodeIn)
    {
      if (nodeIn.OperatorKind == BinaryOperatorKind.Or || nodeIn.OperatorKind == BinaryOperatorKind.And)
      {
        if (nodeIn.Left.Kind == QueryNodeKind.BinaryOperator || nodeIn.Right.Kind == QueryNodeKind.BinaryOperator)
        {
          using (IEnumerator<QueryNode> enumerator = this.Flatten(nodeIn).GetEnumerator())
          {
            enumerator.MoveNext();
            SingleValueNode x = (SingleValueNode) enumerator.Current.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
            SingleValueNode left = x;
            while (enumerator.MoveNext())
            {
              SingleValueNode singleValueNode = (SingleValueNode) enumerator.Current.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
              if (!NodeEqualityComparer.Instance.Equals((QueryNode) x, (QueryNode) singleValueNode))
              {
                x = singleValueNode;
                left = (SingleValueNode) new BinaryOperatorNode(nodeIn.OperatorKind, left, singleValueNode);
              }
            }
            return (QueryNode) left;
          }
        }
        else
        {
          SingleValueNode singleValueNode1 = (SingleValueNode) nodeIn.Left.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
          SingleValueNode singleValueNode2 = (SingleValueNode) nodeIn.Right.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
          if (NodeEqualityComparer.Instance.Equals((QueryNode) singleValueNode1, (QueryNode) singleValueNode2))
            return (QueryNode) singleValueNode1;
          if (singleValueNode1 == nodeIn.Left && singleValueNode2 == nodeIn.Right)
            return (QueryNode) nodeIn;
          nodeIn = new BinaryOperatorNode(nodeIn.OperatorKind, singleValueNode1, singleValueNode2);
        }
      }
      return nodeIn.OperatorKind == BinaryOperatorKind.Equal && nodeIn.Right == ReplaceWithAbstractTokensNodeRewriter.ConstantTokenNode && nodeIn.Left.Kind == QueryNodeKind.SingleValueOpenPropertyAccess && nodeIn.Left.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this) is SingleValueOpenPropertyAccessNode propertyAccessNode && propertyAccessNode.Name == "{property}" && propertyAccessNode.Source.Kind == QueryNodeKind.ResourceRangeVariableReference ? (QueryNode) CombineBinaryOperatorNodesNodeRewriter.PredicateNode : base.Visit(nodeIn);
    }

    private IEnumerable<QueryNode> Flatten(BinaryOperatorNode node)
    {
      SingleValueNode[] singleValueNodeArray = new SingleValueNode[2]
      {
        node.Left,
        node.Right
      };
      for (int index = 0; index < singleValueNodeArray.Length; ++index)
      {
        SingleValueNode singleValueNode = singleValueNodeArray[index];
        if (singleValueNode.Kind == QueryNodeKind.BinaryOperator)
        {
          BinaryOperatorNode node1 = (BinaryOperatorNode) singleValueNode;
          if (node1.OperatorKind == node.OperatorKind)
          {
            foreach (QueryNode queryNode in this.Flatten(node1))
              yield return queryNode;
          }
          else
            yield return (QueryNode) node1;
        }
        else
          yield return (QueryNode) singleValueNode;
      }
      singleValueNodeArray = (SingleValueNode[]) null;
    }

    public override QueryNode Visit(SingleValueOpenPropertyAccessNode nodeIn) => this.IsFlattenAllowed((QueryNode) nodeIn.Source) ? (QueryNode) new SingleValueOpenPropertyAccessNode((SingleValueNode) ((SingleNavigationNode) nodeIn.Source).Source, "{property}") : (QueryNode) new SingleValueOpenPropertyAccessNode(nodeIn.Source, "{property}");

    public override QueryNode Visit(SingleValueFunctionCallNode nodeIn)
    {
      if (nodeIn.Name == "startswith")
      {
        List<QueryNode> list = nodeIn.Parameters.ToList<QueryNode>();
        if (list.Count == 2 && list[1] == ReplaceWithAbstractTokensNodeRewriter.ConstantTokenNode && list[0].Kind == QueryNodeKind.SingleValueOpenPropertyAccess && this.IsFlattenAllowed((QueryNode) ((SingleValueOpenPropertyAccessNode) list[0]).Source))
          return (QueryNode) CombineBinaryOperatorNodesNodeRewriter.PredicateNode;
      }
      return base.Visit(nodeIn);
    }

    public override QueryNode Visit(AllNode nodeIn) => nodeIn.Body.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this) == CombineBinaryOperatorNodesNodeRewriter.PredicateNode && this.IsAllAndAnyAllowed((QueryNode) nodeIn.Source) ? (QueryNode) CombineBinaryOperatorNodesNodeRewriter.PredicateNode : base.Visit(nodeIn);

    public override QueryNode Visit(AnyNode nodeIn) => nodeIn.Body.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this) == CombineBinaryOperatorNodesNodeRewriter.PredicateNode && this.IsAllAndAnyAllowed((QueryNode) nodeIn.Source) ? (QueryNode) CombineBinaryOperatorNodesNodeRewriter.PredicateNode : base.Visit(nodeIn);

    private bool IsAllAndAnyAllowed(QueryNode node) => node.Kind == QueryNodeKind.CollectionNavigationNode && node is CollectionNavigationNode collectionNavigationNode && this.m_model.GetAnnotationValue<QueryShapeAllowAnyAllAnnotationAttribute>((IEdmElement) collectionNavigationNode.NavigationProperty) != null;

    private bool IsFlattenAllowed(QueryNode node) => node.Kind == QueryNodeKind.SingleNavigationNode && node is SingleNavigationNode singleNavigationNode && this.m_model.GetAnnotationValue<QueryShapeAllowFlattenAnnotationAttributeAttribute>((IEdmElement) singleNavigationNode.NavigationProperty) != null;
  }
}
