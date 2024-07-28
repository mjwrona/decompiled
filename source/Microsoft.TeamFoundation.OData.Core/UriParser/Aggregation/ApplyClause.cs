// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.ApplyClause
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.OData.UriParser.Aggregation
{
  public sealed class ApplyClause
  {
    private readonly IEnumerable<TransformationNode> transformations;
    private readonly IEnumerable<AggregateExpressionBase> lastAggregateExpressions;
    private readonly IEnumerable<GroupByPropertyNode> lastGroupByPropertyNodes;
    private readonly List<ComputeExpression> lastComputeExpressions;

    public ApplyClause(IList<TransformationNode> transformations)
    {
      ExceptionUtils.CheckArgumentNotNull<IList<TransformationNode>>(transformations, nameof (transformations));
      this.transformations = (IEnumerable<TransformationNode>) transformations;
      for (int index = transformations.Count - 1; index >= 0; --index)
      {
        if (transformations[index].Kind == TransformationNodeKind.Aggregate)
        {
          this.lastAggregateExpressions = (transformations[index] as AggregateTransformationNode).AggregateExpressions;
          break;
        }
        if (transformations[index].Kind == TransformationNodeKind.GroupBy)
        {
          GroupByTransformationNode transformation = transformations[index] as GroupByTransformationNode;
          this.lastGroupByPropertyNodes = transformation.GroupingProperties;
          TransformationNode childTransformations = transformation.ChildTransformations;
          if (childTransformations == null || childTransformations.Kind != TransformationNodeKind.Aggregate)
            break;
          this.lastAggregateExpressions = (childTransformations as AggregateTransformationNode).AggregateExpressions;
          break;
        }
        if (transformations[index].Kind == TransformationNodeKind.Compute)
        {
          this.lastComputeExpressions = this.lastComputeExpressions ?? new List<ComputeExpression>();
          this.lastComputeExpressions.AddRange((transformations[index] as ComputeTransformationNode).Expressions);
        }
      }
    }

    public IEnumerable<TransformationNode> Transformations => this.transformations;

    internal string GetContextUri() => this.CreatePropertiesUriSegment(this.lastGroupByPropertyNodes, this.lastAggregateExpressions, this.transformations.OfType<ComputeTransformationNode>().SelectMany<ComputeTransformationNode, ComputeExpression>((Func<ComputeTransformationNode, IEnumerable<ComputeExpression>>) (n => n.Expressions)));

    internal HashSet<EndPathToken> GetLastAggregatedPropertyNames()
    {
      if (this.lastAggregateExpressions == null && this.lastComputeExpressions == null && this.lastGroupByPropertyNodes == null)
        return (HashSet<EndPathToken>) null;
      HashSet<EndPathToken> aggregatedPropertyNames = new HashSet<EndPathToken>();
      if (this.lastAggregateExpressions != null)
        aggregatedPropertyNames.UnionWith(this.lastAggregateExpressions.Select<AggregateExpressionBase, EndPathToken>((Func<AggregateExpressionBase, EndPathToken>) (statement => new EndPathToken(statement.Alias, (QueryToken) null))));
      if (this.lastComputeExpressions != null)
        aggregatedPropertyNames.UnionWith(this.lastComputeExpressions.Select<ComputeExpression, EndPathToken>((Func<ComputeExpression, EndPathToken>) (statement => new EndPathToken(statement.Alias, (QueryToken) null))));
      if (this.lastGroupByPropertyNodes != null)
        aggregatedPropertyNames.UnionWith(this.GetGroupByPaths(this.lastGroupByPropertyNodes, (EndPathToken) null));
      return aggregatedPropertyNames;
    }

    private IEnumerable<EndPathToken> GetGroupByPaths(
      IEnumerable<GroupByPropertyNode> nodes,
      EndPathToken token)
    {
      foreach (GroupByPropertyNode node in nodes)
      {
        EndPathToken token1 = new EndPathToken(node.Name, (QueryToken) token);
        if (node.ChildTransformations == null || !node.ChildTransformations.Any<GroupByPropertyNode>())
        {
          yield return token1;
        }
        else
        {
          foreach (EndPathToken groupByPath in this.GetGroupByPaths((IEnumerable<GroupByPropertyNode>) node.ChildTransformations, token1))
            yield return groupByPath;
        }
      }
    }

    private string CreatePropertiesUriSegment(
      IEnumerable<GroupByPropertyNode> groupByPropertyNodes,
      IEnumerable<AggregateExpressionBase> aggregateExpressions,
      IEnumerable<ComputeExpression> computeExpressions)
    {
      Func<GroupByPropertyNode, string> func = (Func<GroupByPropertyNode, string>) (prop =>
      {
        string propertiesUriSegment = this.CreatePropertiesUriSegment((IEnumerable<GroupByPropertyNode>) prop.ChildTransformations, (IEnumerable<AggregateExpressionBase>) null, (IEnumerable<ComputeExpression>) null);
        return !string.IsNullOrEmpty(propertiesUriSegment) ? prop.Name + "(" + propertiesUriSegment + ")" : prop.Name;
      });
      string empty = string.Empty;
      string propertiesUriSegment1;
      if (groupByPropertyNodes != null)
      {
        string str1 = string.Join(",", groupByPropertyNodes.Select<GroupByPropertyNode, string>((Func<GroupByPropertyNode, string>) (prop => func(prop))).ToArray<string>());
        string str2;
        if (aggregateExpressions != null)
          str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}", new object[2]
          {
            (object) str1,
            (object) ApplyClause.CreateAggregatePropertiesUriSegment(aggregateExpressions)
          });
        else
          str2 = str1;
        propertiesUriSegment1 = str2;
      }
      else
        propertiesUriSegment1 = aggregateExpressions == null ? string.Empty : ApplyClause.CreateAggregatePropertiesUriSegment(aggregateExpressions);
      if (computeExpressions != null && !string.IsNullOrEmpty(propertiesUriSegment1))
      {
        string str = string.Join(",", computeExpressions.Select<ComputeExpression, string>((Func<ComputeExpression, string>) (e => e.Alias)).ToArray<string>());
        if (!string.IsNullOrEmpty(str))
          propertiesUriSegment1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}", new object[2]
          {
            (object) propertiesUriSegment1,
            (object) str
          });
      }
      return propertiesUriSegment1;
    }

    private static string CreateAggregatePropertiesUriSegment(
      IEnumerable<AggregateExpressionBase> aggregateExpressions)
    {
      return aggregateExpressions != null ? string.Join(",", aggregateExpressions.Select<AggregateExpressionBase, string>((Func<AggregateExpressionBase, string>) (statement => statement.Alias)).ToArray<string>()) : string.Empty;
    }
  }
}
