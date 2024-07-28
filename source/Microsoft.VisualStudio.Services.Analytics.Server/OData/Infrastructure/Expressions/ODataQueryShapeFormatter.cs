// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.Expressions.ODataQueryShapeFormatter
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.Expressions
{
  public class ODataQueryShapeFormatter
  {
    private static readonly ReplaceWithAbstractTokensNodeRewriter s_replaceWithAbstractTokens = new ReplaceWithAbstractTokensNodeRewriter("ProjectSK");
    private readonly CombineBinaryOperatorNodesNodeRewriter m_combineBinaryOperatorNodes;

    public ODataQueryShapeFormatter(IEdmModel model) => this.m_combineBinaryOperatorNodes = new CombineBinaryOperatorNodesNodeRewriter(model);

    public string Format(ODataQueryOptions queryOptions)
    {
      StringBuilder builder = new StringBuilder(queryOptions.Request.RequestUri.OriginalString.Length);
      builder.Append((object) queryOptions.Context.Path);
      Func<string> getQueryDelimiter = (Func<string>) (() =>
      {
        getQueryDelimiter = (Func<string>) (() => "&");
        return "?";
      });
      this.AppendApplyClause(builder, queryOptions.Apply?.ApplyClause, getQueryDelimiter);
      this.AppendFilterClause(builder, queryOptions.Filter?.FilterClause, getQueryDelimiter);
      this.AppendSelectExpandClause(builder, queryOptions.SelectExpand?.SelectExpandClause, getQueryDelimiter);
      this.AppendOrderByClause(builder, queryOptions.OrderBy?.OrderByClause, getQueryDelimiter);
      this.AppendRawClause(builder, "top", queryOptions.RawValues?.Top, getQueryDelimiter);
      this.AppendRawClause(builder, "skip", queryOptions.RawValues?.Skip, getQueryDelimiter);
      this.AppendRawClause(builder, "skiptoken", queryOptions.RawValues?.SkipToken, getQueryDelimiter);
      this.AppendRawClause(builder, "deltatoken", queryOptions.RawValues?.DeltaToken, getQueryDelimiter);
      return HttpUtility.UrlDecode(builder.ToString());
    }

    private void FormatOrderByClause(StringBuilder builder, OrderByClause orderByClause)
    {
      Func<string> getOrderByDelimiter = (Func<string>) (() =>
      {
        getOrderByDelimiter = (Func<string>) (() => ",");
        return string.Empty;
      });
      for (; orderByClause != null; orderByClause = orderByClause.ThenBy)
      {
        builder.Append(getOrderByDelimiter());
        builder.Append(this.FormatNode(this.Rewrite(orderByClause.Expression)));
      }
    }

    private SingleValueNode Rewrite(SingleValueNode node) => (SingleValueNode) node.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) ODataQueryShapeFormatter.s_replaceWithAbstractTokens).Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this.m_combineBinaryOperatorNodes);

    private void AppendOrderByClause(
      StringBuilder builder,
      OrderByClause orderByClause,
      Func<string> getQueryDelimiter)
    {
      if (orderByClause == null)
        return;
      builder.Append(getQueryDelimiter());
      builder.Append("$orderby=");
      this.FormatOrderByClause(builder, orderByClause);
    }

    private void AppendRawClause(
      StringBuilder builder,
      string optionName,
      string optionValue,
      Func<string> getQueryDelimiter)
    {
      if (optionValue == null)
        return;
      builder.Append(getQueryDelimiter());
      builder.Append("$" + optionName + "=");
      builder.Append("{constant}");
    }

    private void AppendFilterClause(
      StringBuilder builder,
      FilterClause filterClause,
      Func<string> getQueryDelimiter)
    {
      if (filterClause == null)
        return;
      builder.Append(getQueryDelimiter());
      builder.Append("$filter=");
      SingleValueNode node = (SingleValueNode) filterClause.Expression.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) ODataQueryShapeFormatter.s_replaceWithAbstractTokens).Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this.m_combineBinaryOperatorNodes);
      builder.Append(this.FormatNode(node));
    }

    private void AppendSelectExpandClause(
      StringBuilder builder,
      SelectExpandClause clause,
      Func<string> getQueryDelimiter)
    {
      if (clause == null)
        return;
      foreach (ExpandedNavigationSelectItem navigationSelectItem in clause.SelectedItems.OfType<ExpandedNavigationSelectItem>())
      {
        Func<string> expandOptionDelimiter = (Func<string>) (() =>
        {
          expandOptionDelimiter = (Func<string>) (() => ";");
          return "";
        });
        builder.Append(getQueryDelimiter());
        builder.Append("$expand=");
        builder.Append(navigationSelectItem.PathToNavigationProperty.FirstSegment.Identifier);
        builder.Append("(");
        this.AppendApplyClause(builder, navigationSelectItem.ApplyOption, expandOptionDelimiter);
        this.AppendFilterClause(builder, navigationSelectItem.FilterOption, expandOptionDelimiter);
        this.AppendSelectExpandClause(builder, navigationSelectItem.SelectAndExpand, expandOptionDelimiter);
        this.AppendOrderByClause(builder, navigationSelectItem.OrderByOption, expandOptionDelimiter);
        builder.Append(")");
      }
      if (!clause.SelectedItems.OfType<PathSelectItem>().Where<PathSelectItem>((Func<PathSelectItem, bool>) (x => x.SelectedPath.FirstSegment.GetType() == typeof (PropertySegment))).Any<PathSelectItem>())
        return;
      builder.Append(getQueryDelimiter());
      builder.Append("$select=");
      builder.Append("{property}");
    }

    private void AppendApplyClause(
      StringBuilder builder,
      ApplyClause applyClause,
      Func<string> getQueryDelimiter)
    {
      bool? nullable;
      if (applyClause == null)
      {
        nullable = new bool?();
      }
      else
      {
        IEnumerable<TransformationNode> transformations = applyClause.Transformations;
        nullable = transformations != null ? new bool?(transformations.Any<TransformationNode>()) : new bool?();
      }
      if (!nullable.GetValueOrDefault())
        return;
      builder.Append(getQueryDelimiter());
      builder.Append("$apply=");
      Func<string> getApplyDelimiter = (Func<string>) (() =>
      {
        getApplyDelimiter = (Func<string>) (() => "/");
        return string.Empty;
      });
      foreach (TransformationNode transformation in applyClause.Transformations)
      {
        builder.Append(getApplyDelimiter());
        this.AppendTransformation(builder, transformation);
      }
    }

    private void AppendTransformation(StringBuilder builder, TransformationNode transformation)
    {
      switch (transformation.Kind)
      {
        case TransformationNodeKind.Aggregate:
          AggregateTransformationNode transformationNode1 = (AggregateTransformationNode) transformation;
          builder.Append("aggregate(");
          Func<string> getAggregateDelimiter = (Func<string>) (() =>
          {
            getAggregateDelimiter = (Func<string>) (() => ", ");
            return string.Empty;
          });
          foreach (AggregateExpression aggregateExpression in transformationNode1.AggregateExpressions.OfType<AggregateExpression>())
          {
            builder.Append(getAggregateDelimiter());
            if (aggregateExpression.Method != AggregationMethod.VirtualPropertyCount)
            {
              builder.Append(this.FormatNode(this.Rewrite(aggregateExpression.Expression)));
              builder.Append(" with ");
            }
            builder.Append(this.FormatAggregationMethod(aggregateExpression.Method));
            builder.Append(" as ");
            builder.Append("{alias}");
          }
          builder.Append(")");
          break;
        case TransformationNodeKind.GroupBy:
          GroupByTransformationNode transformationNode2 = (GroupByTransformationNode) transformation;
          builder.Append("groupby((");
          builder.Append(string.Join(",", transformationNode2.GroupingProperties.SelectMany<GroupByPropertyNode, string>(new Func<GroupByPropertyNode, IEnumerable<string>>(this.FormatGroupByPropertyNode))));
          builder.Append(')');
          if (transformationNode2.ChildTransformations != null)
          {
            builder.Append(", ");
            this.AppendTransformation(builder, transformationNode2.ChildTransformations);
          }
          builder.Append(")");
          break;
        case TransformationNodeKind.Filter:
          FilterTransformationNode transformationNode3 = (FilterTransformationNode) transformation;
          builder.Append("filter(");
          SingleValueNode node1 = (SingleValueNode) transformationNode3.FilterClause.Expression.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) ODataQueryShapeFormatter.s_replaceWithAbstractTokens).Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this.m_combineBinaryOperatorNodes);
          builder.Append(this.FormatNode(node1));
          builder.Append(")");
          break;
        case TransformationNodeKind.Compute:
          ComputeTransformationNode transformationNode4 = (ComputeTransformationNode) transformation;
          builder.Append("compute(");
          Func<string> getComputeDelimiter = (Func<string>) (() =>
          {
            getComputeDelimiter = (Func<string>) (() => ",");
            return string.Empty;
          });
          foreach (ComputeExpression expression in transformationNode4.Expressions)
          {
            SingleValueNode node2 = (SingleValueNode) expression.Expression.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) ODataQueryShapeFormatter.s_replaceWithAbstractTokens).Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this.m_combineBinaryOperatorNodes);
            builder.Append(getComputeDelimiter());
            builder.Append(this.FormatNode(node2));
            builder.Append(" as ");
            builder.Append("{alias}");
          }
          builder.Append(")");
          break;
        default:
          throw new ArgumentOutOfRangeException(AnalyticsResources.UNSUPPORTED_TRANSFORMATION((object) transformation.Kind));
      }
    }

    private IEnumerable<string> FormatGroupByPropertyNode(GroupByPropertyNode node)
    {
      if (node.Expression == null)
        return node.ChildTransformations.SelectMany<GroupByPropertyNode, string>(new Func<GroupByPropertyNode, IEnumerable<string>>(this.FormatGroupByPropertyNode));
      return (IEnumerable<string>) new string[1]
      {
        this.FormatNode((SingleValueNode) node.Expression.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) ODataQueryShapeFormatter.s_replaceWithAbstractTokens).Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this.m_combineBinaryOperatorNodes))
      };
    }

    private string FormatNode(SingleValueNode node)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      Uri uri = new ODataUri()
      {
        ServiceRoot = new Uri("https://host"),
        Path = new ODataPath((ODataPathSegment[]) new CountSegment[1]
        {
          CountSegment.Instance
        }),
        Filter = new FilterClause(node, (RangeVariable) new NonResourceRangeVariable("$it", (IEdmTypeReference) null, (CollectionNode) null))
      }.BuildUri(ODataUrlKeyDelimiter.Parentheses);
      return uri.Query.StartsWith("?$filter=") ? uri.Query.Substring(9) : throw new ArgumentException(AnalyticsResources.INVALID_NODE(), nameof (node));
    }

    private string FormatAggregationMethod(AggregationMethod method)
    {
      switch (method)
      {
        case AggregationMethod.Sum:
          return "sum";
        case AggregationMethod.Min:
          return "min";
        case AggregationMethod.Max:
          return "max";
        case AggregationMethod.Average:
          return "average";
        case AggregationMethod.CountDistinct:
          return "countdistinct";
        case AggregationMethod.VirtualPropertyCount:
          return "$count";
        default:
          throw new ArgumentOutOfRangeException(AnalyticsResources.UNSUPPORTED_AGGREGATION_METHOD((object) method));
      }
    }
  }
}
