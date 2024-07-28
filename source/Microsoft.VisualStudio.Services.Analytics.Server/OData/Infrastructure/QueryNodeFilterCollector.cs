// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.QueryNodeFilterCollector
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Microsoft.VisualStudio.Services.Analytics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class QueryNodeFilterCollector : AnalyticsQueryNodeVisitor<PathAndModifiedSingleValueNode>
  {
    private int m_doNotStoreDepth;
    private StringBuilder m_pathBuilder = new StringBuilder();
    private string _pathPrefix;

    public override PathAndModifiedSingleValueNode Visit(SingleValueFunctionCallNode nodeIn)
    {
      List<QueryNode> parameters = new List<QueryNode>();
      string path = (string) null;
      foreach (QueryNode parameter in nodeIn.Parameters)
      {
        PathAndModifiedSingleValueNode modifiedSingleValueNode = parameter.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
        if (modifiedSingleValueNode != null && (parameter.Kind == QueryNodeKind.Constant || path == null || path.Equals(modifiedSingleValueNode.Path)))
        {
          path = modifiedSingleValueNode.Path ?? path;
          parameters.Add((QueryNode) modifiedSingleValueNode.Node);
        }
      }
      if (parameters.Count != nodeIn.Parameters.Count<QueryNode>())
        return (PathAndModifiedSingleValueNode) null;
      SingleValueFunctionCallNode node = new SingleValueFunctionCallNode(nodeIn.Name, (IEnumerable<QueryNode>) parameters, nodeIn.TypeReference);
      if (EdmTypeUtils.IsTypeOf(node.TypeReference, typeof (bool)))
        this.StorePathToExpression(path, (SingleValueNode) node);
      return new PathAndModifiedSingleValueNode()
      {
        Node = (SingleValueNode) node,
        Path = path
      };
    }

    public override PathAndModifiedSingleValueNode Visit(UnaryOperatorNode nodeIn)
    {
      if (nodeIn.OperatorKind != UnaryOperatorKind.Negate && nodeIn.OperatorKind != UnaryOperatorKind.Not)
        throw new InvalidOperationException(AnalyticsResources.UNSUPPORTED_UNARY_OPERATOR((object) nodeIn.OperatorKind));
      ++this.m_doNotStoreDepth;
      PathAndModifiedSingleValueNode modifiedSingleValueNode = nodeIn.Operand.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
      --this.m_doNotStoreDepth;
      if (modifiedSingleValueNode != null)
      {
        modifiedSingleValueNode.Node = (SingleValueNode) new UnaryOperatorNode(nodeIn.OperatorKind, modifiedSingleValueNode.Node);
        if (EdmTypeUtils.IsTypeOf(nodeIn.TypeReference, typeof (bool)))
          this.StorePathToExpression(modifiedSingleValueNode.Path, modifiedSingleValueNode.Node);
      }
      return modifiedSingleValueNode;
    }

    public override PathAndModifiedSingleValueNode Visit(BinaryOperatorNode nodeIn)
    {
      if (nodeIn.OperatorKind == BinaryOperatorKind.NotEqual || nodeIn.OperatorKind == BinaryOperatorKind.Equal)
      {
        PathAndModifiedSingleValueNode modifiedSingleValueNode1 = nodeIn.Left.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
        PathAndModifiedSingleValueNode modifiedSingleValueNode2 = nodeIn.Right.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
        if (modifiedSingleValueNode1 != null && modifiedSingleValueNode2 != null && (modifiedSingleValueNode1.Path != null || modifiedSingleValueNode2.Path != null))
        {
          BinaryOperatorNode node = new BinaryOperatorNode(nodeIn.OperatorKind, modifiedSingleValueNode1.Node, modifiedSingleValueNode2.Node);
          PathAndModifiedSingleValueNode modifiedSingleValueNode3 = modifiedSingleValueNode1.Path != null ? modifiedSingleValueNode1 : modifiedSingleValueNode2;
          this.StorePathToExpression(modifiedSingleValueNode3.Path, (SingleValueNode) node);
          modifiedSingleValueNode3.Node = (SingleValueNode) node;
          return modifiedSingleValueNode3;
        }
      }
      else if (nodeIn.OperatorKind == BinaryOperatorKind.And)
      {
        PathAndModifiedSingleValueNode modifiedSingleValueNode4 = nodeIn.Left.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
        PathAndModifiedSingleValueNode modifiedSingleValueNode5 = nodeIn.Right.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
        if (modifiedSingleValueNode4 != null && modifiedSingleValueNode4.Path != null && modifiedSingleValueNode5 != null && modifiedSingleValueNode5.Path != null && modifiedSingleValueNode4.Path == modifiedSingleValueNode5.Path)
          return new PathAndModifiedSingleValueNode()
          {
            Node = (SingleValueNode) new BinaryOperatorNode(nodeIn.OperatorKind, modifiedSingleValueNode4.Node, modifiedSingleValueNode5.Node),
            Path = modifiedSingleValueNode4.Path
          };
        if (modifiedSingleValueNode4 != null || modifiedSingleValueNode5 != null)
          return new PathAndModifiedSingleValueNode()
          {
            Node = modifiedSingleValueNode4?.Node ?? modifiedSingleValueNode5?.Node,
            Path = modifiedSingleValueNode4?.Path ?? modifiedSingleValueNode5?.Path
          };
      }
      else if (nodeIn.OperatorKind == BinaryOperatorKind.Or)
      {
        ++this.m_doNotStoreDepth;
        PathAndModifiedSingleValueNode modifiedSingleValueNode6 = nodeIn.Left.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
        PathAndModifiedSingleValueNode modifiedSingleValueNode7 = nodeIn.Right.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
        --this.m_doNotStoreDepth;
        if (modifiedSingleValueNode6 != null && modifiedSingleValueNode7 != null && modifiedSingleValueNode6.Path == modifiedSingleValueNode7.Path)
        {
          BinaryOperatorNode node = new BinaryOperatorNode(nodeIn.OperatorKind, modifiedSingleValueNode6.Node, modifiedSingleValueNode7.Node);
          this.StorePathToExpression(modifiedSingleValueNode6.Path, (SingleValueNode) node);
          return new PathAndModifiedSingleValueNode()
          {
            Node = (SingleValueNode) node,
            Path = modifiedSingleValueNode6.Path
          };
        }
      }
      return (PathAndModifiedSingleValueNode) null;
    }

    public override PathAndModifiedSingleValueNode Visit(ConstantNode nodeIn)
    {
      PathAndModifiedSingleValueNode modifiedSingleValueNode = new PathAndModifiedSingleValueNode()
      {
        Node = (SingleValueNode) nodeIn
      };
      if (EdmTypeUtils.IsTypeOf(nodeIn.TypeReference, typeof (string)))
        modifiedSingleValueNode.Node = (SingleValueNode) new SingleValueFunctionCallNode("toupper", (IEnumerable<QueryNode>) new QueryNode[1]
        {
          (QueryNode) nodeIn
        }, nodeIn.TypeReference);
      return modifiedSingleValueNode;
    }

    public override PathAndModifiedSingleValueNode Visit(ConvertNode nodeIn)
    {
      PathAndModifiedSingleValueNode modifiedSingleValueNode = nodeIn.Source.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
      if (modifiedSingleValueNode == null)
        return (PathAndModifiedSingleValueNode) null;
      return new PathAndModifiedSingleValueNode()
      {
        Node = (SingleValueNode) new ConvertNode(modifiedSingleValueNode.Node, nodeIn.TypeReference),
        Path = modifiedSingleValueNode.Path
      };
    }

    public override PathAndModifiedSingleValueNode Visit(SingleValuePropertyAccessNode nodeIn)
    {
      if (nodeIn.Property.Name == "ProjectName" || nodeIn.Property.Name == "ProjectSK" || nodeIn.Property.Name == "ProjectId")
      {
        PathAndModifiedSingleValueNode modifiedSingleValueNode = nodeIn.Source.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
        if (modifiedSingleValueNode != null)
        {
          SingleValueNode singleValueNode = (SingleValueNode) new SingleValuePropertyAccessNode(modifiedSingleValueNode.Node, nodeIn.Property);
          if (nodeIn.Property.Name == "ProjectName")
            singleValueNode = (SingleValueNode) new SingleValueFunctionCallNode("toupper", (IEnumerable<QueryNode>) new QueryNode[1]
            {
              (QueryNode) singleValueNode
            }, nodeIn.TypeReference);
          return new PathAndModifiedSingleValueNode()
          {
            Node = singleValueNode,
            Path = modifiedSingleValueNode.Path
          };
        }
      }
      return (PathAndModifiedSingleValueNode) null;
    }

    public override PathAndModifiedSingleValueNode Visit(SingleNavigationNode nodeIn)
    {
      Type type1 = EdmTypeUtils.GetType(nodeIn.TypeReference);
      if (type1 == typeof (Project))
      {
        Type type2 = EdmTypeUtils.GetType(nodeIn.Source.TypeReference);
        if (type2 != (Type) null && typeof (IProjectScoped).IsAssignableFrom(type2))
          return nodeIn.Source.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
      }
      else if (typeof (IPartitionScoped).IsAssignableFrom(type1))
      {
        this.InsertPath(nodeIn.NavigationProperty.Name);
        return nodeIn.Source.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
      }
      return (PathAndModifiedSingleValueNode) null;
    }

    public override PathAndModifiedSingleValueNode Visit(ResourceRangeVariableReferenceNode nodeIn)
    {
      string str = this.m_pathBuilder.ToString();
      this.m_pathBuilder.Clear();
      this.ResourceRangeVariableReferenceNode = nodeIn;
      return new PathAndModifiedSingleValueNode()
      {
        Node = (SingleValueNode) nodeIn,
        Path = str
      };
    }

    public void Process(string absolutePath, FilterClause filterClause, ApplyClause applyClause)
    {
      this._pathPrefix = absolutePath;
      filterClause?.Expression.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
      if (applyClause != null)
      {
        foreach (FilterTransformationNode transformationNode in applyClause.Transformations.OfType<FilterTransformationNode>())
          transformationNode.FilterClause.Expression.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
        foreach (ExpandTransformationNode transformationNode in applyClause.Transformations.OfType<ExpandTransformationNode>())
        {
          foreach (ExpandedNavigationSelectItem expand in transformationNode.ExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>())
            this.CollectExpand(expand);
        }
      }
      this.PathToExpression = this.PathToExpression.Where<KeyValuePair<string, List<SingleValueNode>>>((Func<KeyValuePair<string, List<SingleValueNode>>, bool>) (p => p.Value.Count > 0)).ToDictionary<KeyValuePair<string, List<SingleValueNode>>, string, List<SingleValueNode>>((Func<KeyValuePair<string, List<SingleValueNode>>, string>) (p => p.Key), (Func<KeyValuePair<string, List<SingleValueNode>>, List<SingleValueNode>>) (p => p.Value));
    }

    private void CollectExpand(ExpandedNavigationSelectItem expand)
    {
      string pathPrefix = this._pathPrefix;
      this._pathPrefix = this._pathPrefix + "/" + string.Join("/", expand.PathToNavigationProperty.Select<ODataPathSegment, string>((Func<ODataPathSegment, string>) (p => p.Identifier)));
      expand.FilterOption.Expression.Accept<PathAndModifiedSingleValueNode>((QueryNodeVisitor<PathAndModifiedSingleValueNode>) this);
      if (expand.SelectAndExpand != null)
      {
        foreach (ExpandedNavigationSelectItem expand1 in expand.SelectAndExpand.SelectedItems.OfType<ExpandedNavigationSelectItem>())
          this.CollectExpand(expand1);
      }
      this._pathPrefix = pathPrefix;
    }

    private void StorePathToExpression(string path, SingleValueNode node)
    {
      if (this.m_doNotStoreDepth != 0)
        return;
      string key = this._pathPrefix + path;
      if (!this.PathToExpression.ContainsKey(key))
        this.PathToExpression[key] = new List<SingleValueNode>();
      this.PathToExpression[key].Add(node);
    }

    private void InsertPath(string name) => this.m_pathBuilder.Insert(0, "/" + name);

    public ResourceRangeVariableReferenceNode ResourceRangeVariableReferenceNode { get; private set; }

    public Dictionary<string, List<SingleValueNode>> PathToExpression { get; private set; } = new Dictionary<string, List<SingleValueNode>>();
  }
}
