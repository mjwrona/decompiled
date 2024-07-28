// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.ApplyPathCollector
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData
{
  public class ApplyPathCollector : AnalyticsQueryNodeVisitor<string>
  {
    private List<string> _paths;
    private string _rootEntitySetName;
    private Dictionary<string, HashSet<string>> _navigationToSkipValidation;
    private Dictionary<string, HashSet<string>> _visibleCrossProject;
    private bool _inProjectScope;

    public List<string> Process(
      ApplyClause applyClause,
      Dictionary<string, HashSet<string>> navigationToSkipValidation,
      Dictionary<string, HashSet<string>> visibleCrossProject,
      string rootEntitySetName,
      bool inProjectScope)
    {
      this._navigationToSkipValidation = navigationToSkipValidation;
      this._visibleCrossProject = visibleCrossProject;
      this._inProjectScope = inProjectScope;
      this._rootEntitySetName = rootEntitySetName;
      this._paths = new List<string>();
      if (applyClause != null && applyClause.Transformations != null)
      {
        IEnumerable<TransformationNodeKind> transformationNodeKinds = applyClause.Transformations.Select<TransformationNode, TransformationNodeKind>((Func<TransformationNode, TransformationNodeKind>) (t => t.Kind)).Distinct<TransformationNodeKind>().Where<TransformationNodeKind>((Func<TransformationNodeKind, bool>) (t => t != TransformationNodeKind.Aggregate && t != TransformationNodeKind.Filter && t != TransformationNodeKind.GroupBy && t != TransformationNodeKind.Compute && t != TransformationNodeKind.Expand));
        if (transformationNodeKinds != null && transformationNodeKinds.Any<TransformationNodeKind>())
          throw new InvalidOperationException(AnalyticsResources.UNSUPPORTED_TRANSFORMATION((object) string.Join<TransformationNodeKind>(", ", transformationNodeKinds)));
        foreach (GroupByPropertyNode node in applyClause.Transformations.OfType<GroupByTransformationNode>().SelectMany<GroupByTransformationNode, GroupByPropertyNode>((Func<GroupByTransformationNode, IEnumerable<GroupByPropertyNode>>) (g => g.GroupingProperties)))
          this.ProcessGroupByPropertyNode(node);
        foreach (AggregateExpressionBase expressionBase in applyClause.Transformations.OfType<AggregateTransformationNode>().SelectMany<AggregateTransformationNode, AggregateExpressionBase>((Func<AggregateTransformationNode, IEnumerable<AggregateExpressionBase>>) (a => a.AggregateExpressions)))
          this.ProcessAggregateExpression(expressionBase);
        foreach (ExpandedNavigationSelectItem node in applyClause.Transformations.OfType<ExpandTransformationNode>().SelectMany<ExpandTransformationNode, SelectItem>((Func<ExpandTransformationNode, IEnumerable<SelectItem>>) (g => g.ExpandClause.SelectedItems)).OfType<ExpandedNavigationSelectItem>())
          this.ProcessExpandNode(node, this._rootEntitySetName);
        foreach (ComputeTransformationNode node in applyClause.Transformations.OfType<ComputeTransformationNode>())
          this.ProcessComputeNode(node);
        foreach (FilterTransformationNode node in applyClause.Transformations.OfType<FilterTransformationNode>())
          this.ProcessFilterNode(node);
      }
      return this._paths;
    }

    private void ProcessExpandNode(
      ExpandedNavigationSelectItem node,
      string entitySetName,
      string path = null)
    {
      if (!typeof (IProjectScoped).IsAssignableFrom(EdmTypeUtils.GetType(node.NavigationSource.Type)))
        return;
      string str = path + "/" + string.Join("/", node.PathToNavigationProperty.Select<ODataPathSegment, string>((Func<ODataPathSegment, string>) (p => p.Identifier)));
      HashSet<string> stringSet = (HashSet<string>) null;
      this._visibleCrossProject.TryGetValue(entitySetName, out stringSet);
      if (!this._inProjectScope || stringSet != null)
      {
        string identifier = node.PathToNavigationProperty.First<ODataPathSegment>().Identifier;
        if (stringSet != null && stringSet.Contains(identifier))
          return;
        this.AddPath(identifier, str, entitySetName);
      }
      if (node.SelectAndExpand == null)
        return;
      foreach (ExpandedNavigationSelectItem node1 in node.SelectAndExpand.SelectedItems.OfType<ExpandedNavigationSelectItem>())
        this.ProcessExpandNode(node1, node.NavigationSource.Name, str);
    }

    private void ProcessAggregateExpression(AggregateExpressionBase expressionBase)
    {
      switch (expressionBase)
      {
        case AggregateExpression aggregateExpression1 when aggregateExpression1.Expression.Kind != QueryNodeKind.Count:
          aggregateExpression1.Expression.Accept<string>((QueryNodeVisitor<string>) this);
          break;
        case EntitySetAggregateExpression aggregateExpression2:
          using (IEnumerator<AggregateExpressionBase> enumerator = aggregateExpression2.Children.GetEnumerator())
          {
            while (enumerator.MoveNext())
              this.ProcessAggregateExpression(enumerator.Current);
            break;
          }
      }
    }

    private void ProcessComputeNode(ComputeTransformationNode node)
    {
      foreach (ComputeExpression expression in node.Expressions)
        expression.Expression.Accept<string>((QueryNodeVisitor<string>) this);
    }

    private void ProcessFilterNode(FilterTransformationNode node) => node.FilterClause.Expression.Accept<string>((QueryNodeVisitor<string>) this);

    private void ProcessGroupByPropertyNode(GroupByPropertyNode node)
    {
      if (node == null)
        return;
      if (node.Expression != null)
        node.Expression.Accept<string>((QueryNodeVisitor<string>) this);
      foreach (GroupByPropertyNode childTransformation in (IEnumerable<GroupByPropertyNode>) node.ChildTransformations)
        this.ProcessGroupByPropertyNode(childTransformation);
    }

    public override string Visit(SingleValuePropertyAccessNode nodeIn)
    {
      if (!typeof (IProjectScoped).IsAssignableFrom(EdmTypeUtils.GetType(nodeIn.Source.TypeReference)))
        return nodeIn.Source.Accept<string>((QueryNodeVisitor<string>) this);
      IEdmEntityType definition = (IEdmEntityType) nodeIn.Source.TypeReference.Definition;
      HashSet<string> stringSet = (HashSet<string>) null;
      this._visibleCrossProject.TryGetValue(definition.Name, out stringSet);
      if (this._inProjectScope && stringSet == null)
        return (string) null;
      // ISSUE: explicit non-virtual call
      return stringSet != null && __nonvirtual (stringSet.Contains(nodeIn.Property.Name)) ? (string) null : nodeIn.Source.Accept<string>((QueryNodeVisitor<string>) this);
    }

    public override string Visit(AggregatedCollectionPropertyNode nodeIn)
    {
      string name = nodeIn.Source.NavigationProperty.Name;
      string absolutePath = nodeIn.Source.Accept<string>((QueryNodeVisitor<string>) this);
      Type type = EdmTypeUtils.GetType((IEdmType) nodeIn.Property.DeclaringType);
      if (typeof (IProjectScoped).IsAssignableFrom(type))
      {
        HashSet<string> stringSet = (HashSet<string>) null;
        this._visibleCrossProject.TryGetValue(type.Name, out stringSet);
        if (!this._inProjectScope || stringSet != null)
        {
          // ISSUE: explicit non-virtual call
          if (stringSet != null && __nonvirtual (stringSet.Contains(nodeIn.Property.Name)))
            return (string) null;
          this.AddPath(name, absolutePath, this._rootEntitySetName);
        }
      }
      return absolutePath;
    }

    public override string Visit(CollectionNavigationNode nodeIn) => nodeIn.Source.Accept<string>((QueryNodeVisitor<string>) this) + "/" + nodeIn.NavigationProperty.Name;

    public override string Visit(SingleNavigationNode nodeIn)
    {
      string str = nodeIn.Source.Accept<string>((QueryNodeVisitor<string>) this);
      string name = nodeIn.NavigationProperty.Name;
      string absolutePath = str + "/" + name;
      if (typeof (IProjectScoped).IsAssignableFrom(EdmTypeUtils.GetType(nodeIn.TypeReference)))
      {
        string parentEntitySetName = (string) null;
        if (str == null)
          parentEntitySetName = this._rootEntitySetName;
        else if (nodeIn.Source.NavigationSource != null)
          parentEntitySetName = nodeIn.Source.NavigationSource.Name;
        this.AddPath(name, absolutePath, parentEntitySetName);
      }
      return absolutePath;
    }

    public override string Visit(BinaryOperatorNode nodeIn)
    {
      nodeIn.Left.Accept<string>((QueryNodeVisitor<string>) this);
      nodeIn.Right.Accept<string>((QueryNodeVisitor<string>) this);
      return (string) null;
    }

    public override string Visit(ConvertNode nodeIn) => nodeIn.Source.Accept<string>((QueryNodeVisitor<string>) this);

    private void AddPath(
      string currentPropertyName,
      string absolutePath,
      string parentEntitySetName)
    {
      HashSet<string> stringSet;
      if (parentEntitySetName != null && this._navigationToSkipValidation.TryGetValue(parentEntitySetName, out stringSet) && stringSet.Contains(currentPropertyName))
        return;
      this._paths.Add(absolutePath);
    }
  }
}
