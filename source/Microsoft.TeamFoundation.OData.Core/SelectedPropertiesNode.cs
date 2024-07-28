// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.SelectedPropertiesNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData
{
  internal sealed class SelectedPropertiesNode
  {
    internal static readonly SelectedPropertiesNode Empty = new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.Empty);
    internal static readonly SelectedPropertiesNode EntireSubtree = new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree);
    private static readonly Dictionary<string, IEdmStructuralProperty> EmptyStreamProperties = new Dictionary<string, IEdmStructuralProperty>((IEqualityComparer<string>) StringComparer.Ordinal);
    private static readonly IEnumerable<IEdmNavigationProperty> EmptyNavigationProperties = Enumerable.Empty<IEdmNavigationProperty>();
    private readonly SelectedPropertiesNode.SelectionType selectionType;
    private const char PathSeparator = '/';
    private const char ItemSeparator = ',';
    private const string StarSegment = "*";
    private HashSet<string> selectedProperties;
    private Dictionary<string, SelectedPropertiesNode> children;
    private bool hasWildcard;
    private string nodeName;

    internal SelectedPropertiesNode(string selectClause)
      : this(SelectedPropertiesNode.SelectionType.PartialSubtree)
    {
      string str1 = selectClause;
      char[] chArray1 = new char[1]{ ',' };
      foreach (string str2 in str1.Split(chArray1))
      {
        char[] chArray2 = new char[1]{ '/' };
        this.ParsePathSegment(str2.Split(chArray2), 0);
      }
    }

    private SelectedPropertiesNode(SelectedPropertiesNode.SelectionType selectionType) => this.selectionType = selectionType;

    internal static SelectedPropertiesNode Create(string selectQueryOption)
    {
      if (selectQueryOption == null)
        return SelectedPropertiesNode.EntireSubtree;
      selectQueryOption = selectQueryOption.Trim();
      return selectQueryOption.Length == 0 ? SelectedPropertiesNode.Empty : new SelectedPropertiesNode(selectQueryOption);
    }

    internal static SelectedPropertiesNode Create(
      SelectExpandClause selectExpandClause,
      ODataVersion version)
    {
      return selectExpandClause.AllSelected ? SelectedPropertiesNode.EntireSubtree : SelectedPropertiesNode.CreateFromSelectExpandClause(selectExpandClause, version);
    }

    internal static SelectedPropertiesNode CombineNodes(
      SelectedPropertiesNode left,
      SelectedPropertiesNode right)
    {
      if (left.selectionType == SelectedPropertiesNode.SelectionType.EntireSubtree || right.selectionType == SelectedPropertiesNode.SelectionType.EntireSubtree)
        return SelectedPropertiesNode.EntireSubtree;
      if (left.selectionType == SelectedPropertiesNode.SelectionType.Empty)
        return right;
      if (right.selectionType == SelectedPropertiesNode.SelectionType.Empty)
        return left;
      SelectedPropertiesNode selectedPropertiesNode = new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.PartialSubtree)
      {
        hasWildcard = left.hasWildcard | right.hasWildcard
      };
      if (left.selectedProperties != null && right.selectedProperties != null)
        selectedPropertiesNode.selectedProperties = SelectedPropertiesNode.CreateSelectedPropertiesHashSet(left.selectedProperties.AsEnumerable<string>().Concat<string>((IEnumerable<string>) right.selectedProperties));
      else if (left.selectedProperties != null)
        selectedPropertiesNode.selectedProperties = SelectedPropertiesNode.CreateSelectedPropertiesHashSet((IEnumerable<string>) left.selectedProperties);
      else if (right.selectedProperties != null)
        selectedPropertiesNode.selectedProperties = SelectedPropertiesNode.CreateSelectedPropertiesHashSet((IEnumerable<string>) right.selectedProperties);
      if (left.children != null && right.children != null)
      {
        selectedPropertiesNode.children = new Dictionary<string, SelectedPropertiesNode>((IDictionary<string, SelectedPropertiesNode>) left.children);
        foreach (KeyValuePair<string, SelectedPropertiesNode> child in right.children)
        {
          SelectedPropertiesNode left1;
          selectedPropertiesNode.children[child.Key] = !selectedPropertiesNode.children.TryGetValue(child.Key, out left1) ? child.Value : SelectedPropertiesNode.CombineNodes(left1, child.Value);
        }
      }
      else if (left.children != null)
        selectedPropertiesNode.children = new Dictionary<string, SelectedPropertiesNode>((IDictionary<string, SelectedPropertiesNode>) left.children);
      else if (right.children != null)
        selectedPropertiesNode.children = new Dictionary<string, SelectedPropertiesNode>((IDictionary<string, SelectedPropertiesNode>) right.children);
      return selectedPropertiesNode;
    }

    internal SelectedPropertiesNode GetSelectedPropertiesForNavigationProperty(
      IEdmStructuredType structuredType,
      string navigationPropertyName)
    {
      if (this.selectionType == SelectedPropertiesNode.SelectionType.Empty)
        return SelectedPropertiesNode.Empty;
      if (this.selectionType == SelectedPropertiesNode.SelectionType.EntireSubtree || structuredType == null)
        return SelectedPropertiesNode.EntireSubtree;
      if (this.selectedProperties == null)
        this.selectedProperties = SelectedPropertiesNode.CreateSelectedPropertiesHashSet();
      if (this.selectedProperties.Contains(navigationPropertyName))
        return SelectedPropertiesNode.EntireSubtree;
      if (this.children == null)
        return SelectedPropertiesNode.Empty;
      SelectedPropertiesNode empty;
      if (!this.children.TryGetValue(navigationPropertyName, out empty))
        empty = SelectedPropertiesNode.Empty;
      return this.GetMatchingTypeSegments(structuredType).Select<SelectedPropertiesNode, SelectedPropertiesNode>((Func<SelectedPropertiesNode, SelectedPropertiesNode>) (typeSegmentChild => typeSegmentChild.GetSelectedPropertiesForNavigationProperty(structuredType, navigationPropertyName))).Aggregate<SelectedPropertiesNode, SelectedPropertiesNode>(empty, new Func<SelectedPropertiesNode, SelectedPropertiesNode, SelectedPropertiesNode>(SelectedPropertiesNode.CombineNodes));
    }

    internal IEnumerable<IEdmNavigationProperty> GetSelectedNavigationProperties(
      IEdmStructuredType structuredType)
    {
      if (this.selectionType == SelectedPropertiesNode.SelectionType.Empty || structuredType == null)
        return SelectedPropertiesNode.EmptyNavigationProperties;
      if (this.selectionType == SelectedPropertiesNode.SelectionType.EntireSubtree || this.hasWildcard)
        return structuredType.NavigationProperties();
      IEnumerable<string> strings = (IEnumerable<string>) (this.selectedProperties ?? SelectedPropertiesNode.CreateSelectedPropertiesHashSet());
      if (this.children != null)
        strings = this.children.Keys.Concat<string>(strings);
      IEnumerable<IEdmNavigationProperty> navigationProperties = strings.Select<string, IEdmProperty>(new Func<string, IEdmProperty>(structuredType.FindProperty)).OfType<IEdmNavigationProperty>();
      foreach (SelectedPropertiesNode matchingTypeSegment in this.GetMatchingTypeSegments(structuredType))
        navigationProperties = navigationProperties.Concat<IEdmNavigationProperty>(matchingTypeSegment.GetSelectedNavigationProperties(structuredType));
      return navigationProperties.Distinct<IEdmNavigationProperty>();
    }

    internal IDictionary<string, IEdmStructuralProperty> GetSelectedStreamProperties(
      IEdmEntityType entityType)
    {
      if (this.selectionType == SelectedPropertiesNode.SelectionType.Empty)
        return (IDictionary<string, IEdmStructuralProperty>) SelectedPropertiesNode.EmptyStreamProperties;
      if (entityType == null)
        return (IDictionary<string, IEdmStructuralProperty>) SelectedPropertiesNode.EmptyStreamProperties;
      if (this.selectionType == SelectedPropertiesNode.SelectionType.EntireSubtree || this.hasWildcard)
        return (IDictionary<string, IEdmStructuralProperty>) entityType.StructuralProperties().Where<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (sp => sp.Type.IsStream())).ToDictionary<IEdmStructuralProperty, string>((Func<IEdmStructuralProperty, string>) (sp => sp.Name), (IEqualityComparer<string>) StringComparer.Ordinal);
      IDictionary<string, IEdmStructuralProperty> streamProperties = this.selectedProperties == null ? (IDictionary<string, IEdmStructuralProperty>) new Dictionary<string, IEdmStructuralProperty>() : (IDictionary<string, IEdmStructuralProperty>) this.selectedProperties.Select<string, IEdmProperty>(new Func<string, IEdmProperty>(((IEdmStructuredType) entityType).FindProperty)).OfType<IEdmStructuralProperty>().Where<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (p => p.Type.IsStream())).ToDictionary<IEdmStructuralProperty, string>((Func<IEdmStructuralProperty, string>) (p => p.Name), (IEqualityComparer<string>) StringComparer.Ordinal);
      foreach (SelectedPropertiesNode matchingTypeSegment in this.GetMatchingTypeSegments((IEdmStructuredType) entityType))
      {
        foreach (KeyValuePair<string, IEdmStructuralProperty> selectedStreamProperty in (IEnumerable<KeyValuePair<string, IEdmStructuralProperty>>) matchingTypeSegment.GetSelectedStreamProperties(entityType))
          streamProperties[selectedStreamProperty.Key] = selectedStreamProperty.Value;
      }
      return streamProperties;
    }

    internal bool IsOperationSelected(
      IEdmStructuredType structuredType,
      IEdmOperation operation,
      bool mustBeNamespaceQualified)
    {
      mustBeNamespaceQualified = mustBeNamespaceQualified || structuredType.FindProperty(operation.Name) != null;
      return this.IsOperationSelectedAtThisLevel(operation, mustBeNamespaceQualified) || this.GetMatchingTypeSegments(structuredType).Any<SelectedPropertiesNode>((Func<SelectedPropertiesNode, bool>) (typeSegment => typeSegment.IsOperationSelectedAtThisLevel(operation, mustBeNamespaceQualified)));
    }

    private static IEnumerable<IEdmStructuredType> GetBaseTypesAndSelf(
      IEdmStructuredType structuredType)
    {
      IEdmStructuredType currentType;
      for (currentType = structuredType; currentType != null; currentType = currentType.BaseType())
        yield return currentType;
      currentType = (IEdmStructuredType) null;
    }

    private static HashSet<string> CreateSelectedPropertiesHashSet(IEnumerable<string> properties)
    {
      HashSet<string> propertiesHashSet = SelectedPropertiesNode.CreateSelectedPropertiesHashSet();
      foreach (string property in properties)
        propertiesHashSet.Add(property);
      return propertiesHashSet;
    }

    private static HashSet<string> CreateSelectedPropertiesHashSet() => new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);

    private static IEnumerable<string> GetPossibleMatchesForSelectedOperation(
      IEdmOperation operation,
      bool mustBeNamespaceQualified)
    {
      string operationName = operation.Name;
      string operationNameWithParameters = operation.NameWithParameters();
      if (!mustBeNamespaceQualified)
      {
        yield return operationName;
        yield return operationNameWithParameters;
      }
      string qualifiedContainerName = operation.Namespace + ".";
      yield return qualifiedContainerName + "*";
      yield return qualifiedContainerName + operationName;
      yield return qualifiedContainerName + operationNameWithParameters;
    }

    private IEnumerable<SelectedPropertiesNode> GetMatchingTypeSegments(
      IEdmStructuredType structuredType)
    {
      if (this.children != null)
      {
        foreach (IEdmType type in SelectedPropertiesNode.GetBaseTypesAndSelf(structuredType))
        {
          SelectedPropertiesNode matchingTypeSegment;
          if (this.children.TryGetValue(type.FullTypeName(), out matchingTypeSegment))
          {
            if (matchingTypeSegment.hasWildcard)
              throw new ODataException(Strings.SelectedPropertiesNode_StarSegmentAfterTypeSegment);
            yield return matchingTypeSegment;
          }
        }
      }
    }

    private void ParsePathSegment(string[] segments, int index)
    {
      string str = segments[index].Trim();
      if (this.selectedProperties == null)
        this.selectedProperties = SelectedPropertiesNode.CreateSelectedPropertiesHashSet();
      bool flag = string.CompareOrdinal("*", str) == 0;
      if (index != segments.Length - 1)
      {
        if (flag)
          throw new ODataException(Strings.SelectedPropertiesNode_StarSegmentNotLastSegment);
        this.EnsureChildAnnotation(str).ParsePathSegment(segments, index + 1);
      }
      else
        this.selectedProperties.Add(str);
      this.hasWildcard |= flag;
    }

    private SelectedPropertiesNode EnsureChildAnnotation(string segmentName)
    {
      if (this.children == null)
        this.children = new Dictionary<string, SelectedPropertiesNode>((IEqualityComparer<string>) StringComparer.Ordinal);
      SelectedPropertiesNode selectedPropertiesNode;
      if (!this.children.TryGetValue(segmentName, out selectedPropertiesNode))
      {
        selectedPropertiesNode = new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.PartialSubtree);
        this.children.Add(segmentName, selectedPropertiesNode);
      }
      return selectedPropertiesNode;
    }

    private bool IsOperationSelectedAtThisLevel(
      IEdmOperation operation,
      bool mustBeNamespaceQualified)
    {
      if (this.selectionType == SelectedPropertiesNode.SelectionType.EntireSubtree)
        return true;
      return this.selectionType != SelectedPropertiesNode.SelectionType.Empty && this.selectedProperties != null && SelectedPropertiesNode.GetPossibleMatchesForSelectedOperation(operation, mustBeNamespaceQualified).Any<string>((Func<string, bool>) (possibleMatch => this.selectedProperties.Contains(possibleMatch)));
    }

    private static SelectedPropertiesNode CreateFromSelectExpandClause(
      SelectExpandClause selectExpandClause,
      ODataVersion version)
    {
      SelectedPropertiesNode result;
      selectExpandClause.Traverse<SelectedPropertiesNode>(new Func<string, SelectedPropertiesNode, ODataVersion, SelectedPropertiesNode>(SelectedPropertiesNode.ProcessSubExpand), new Func<IList<string>, IList<SelectedPropertiesNode>, SelectedPropertiesNode>(SelectedPropertiesNode.CombineSelectAndExpandResult), (Func<ApplyClause, SelectedPropertiesNode>) null, version, out result);
      return result;
    }

    private static SelectedPropertiesNode ProcessSubExpand(
      string nodeName,
      SelectedPropertiesNode subExpandNode,
      ODataVersion version)
    {
      if (subExpandNode != null)
        subExpandNode.nodeName = nodeName;
      return subExpandNode;
    }

    private static SelectedPropertiesNode CombineSelectAndExpandResult(
      IEnumerable<string> selectList,
      IEnumerable<SelectedPropertiesNode> expandList)
    {
      List<string> list = selectList.ToList<string>();
      list.RemoveAll(new Predicate<string>(((Enumerable) expandList.Select<SelectedPropertiesNode, string>((Func<SelectedPropertiesNode, string>) (m => m.nodeName))).Contains<string>));
      SelectedPropertiesNode selectedPropertiesNode = new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.PartialSubtree)
      {
        selectedProperties = list.Count > 0 ? SelectedPropertiesNode.CreateSelectedPropertiesHashSet() : (HashSet<string>) null,
        children = new Dictionary<string, SelectedPropertiesNode>((IEqualityComparer<string>) StringComparer.Ordinal)
      };
      foreach (string str in list)
      {
        if ("*" == str)
          selectedPropertiesNode.hasWildcard = true;
        else
          selectedPropertiesNode.selectedProperties.Add(str);
      }
      foreach (SelectedPropertiesNode expand in expandList)
        selectedPropertiesNode.children[expand.nodeName] = expand;
      return selectedPropertiesNode;
    }

    private enum SelectionType
    {
      Empty,
      EntireSubtree,
      PartialSubtree,
    }
  }
}
