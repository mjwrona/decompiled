// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ApplyClauseToStringBuilder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.OData
{
  internal sealed class ApplyClauseToStringBuilder
  {
    private readonly NodeToStringBuilder nodeToStringBuilder;
    private readonly StringBuilder query;

    public ApplyClauseToStringBuilder()
    {
      this.nodeToStringBuilder = new NodeToStringBuilder();
      this.query = new StringBuilder();
    }

    public string TranslateApplyClause(ApplyClause applyClause)
    {
      ExceptionUtils.CheckArgumentNotNull<ApplyClause>(applyClause, nameof (applyClause));
      this.query.Append("$apply");
      this.query.Append("=");
      bool appendSlash = false;
      foreach (TransformationNode transformation in applyClause.Transformations)
      {
        appendSlash = this.AppendSlash(appendSlash);
        this.Translate(transformation);
      }
      return !appendSlash ? string.Empty : this.query.ToString();
    }

    private bool AppendComma(bool appendComma)
    {
      if (appendComma)
        this.query.Append(",");
      return true;
    }

    private void AppendExpression(QueryNode expression) => this.query.Append(Uri.EscapeDataString(this.nodeToStringBuilder.TranslateNode(expression)));

    private void AppendExpression(ODataExpandPath path) => this.query.Append(path.ToContextUrlPathString());

    private bool AppendSlash(bool appendSlash)
    {
      if (appendSlash)
        this.query.Append("/");
      return true;
    }

    private void AppendWord(string word)
    {
      this.query.Append(word);
      this.query.Append("%20");
    }

    private static string GetAggregationMethodName(AggregateExpression aggExpression)
    {
      switch (aggExpression.Method)
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
        case AggregationMethod.Custom:
          return aggExpression.MethodDefinition.MethodLabel;
        default:
          throw new ArgumentOutOfRangeException(nameof (aggExpression), "unknown AggregationMethod " + aggExpression.Method.ToString());
      }
    }

    private void Translate(AggregateTransformationNode transformation) => this.Translate(transformation.AggregateExpressions);

    private void Translate(IEnumerable<AggregateExpressionBase> expressions)
    {
      bool appendComma = false;
      foreach (AggregateExpressionBase expression in expressions)
      {
        appendComma = this.AppendComma(appendComma);
        switch (expression.AggregateKind)
        {
          case AggregateExpressionKind.PropertyAggregate:
            AggregateExpression aggExpression = expression as AggregateExpression;
            if (aggExpression.Method != AggregationMethod.VirtualPropertyCount)
            {
              this.AppendExpression((QueryNode) aggExpression.Expression);
              this.query.Append("%20");
              this.AppendWord("with");
            }
            this.AppendWord(ApplyClauseToStringBuilder.GetAggregationMethodName(aggExpression));
            this.AppendWord("as");
            this.query.Append(aggExpression.Alias);
            continue;
          case AggregateExpressionKind.EntitySetAggregate:
            EntitySetAggregateExpression aggregateExpression = expression as EntitySetAggregateExpression;
            this.query.Append(aggregateExpression.Alias);
            this.query.Append("(");
            this.Translate(aggregateExpression.Children);
            this.query.Append(")");
            continue;
          default:
            continue;
        }
      }
    }

    private void Translate(ComputeTransformationNode transformation)
    {
      bool appendComma = false;
      foreach (ComputeExpression expression in transformation.Expressions)
      {
        appendComma = this.AppendComma(appendComma);
        this.AppendExpression(expression.Expression);
        this.query.Append("%20");
        this.AppendWord("as");
        this.query.Append(expression.Alias);
      }
    }

    private void Translate(ExpandTransformationNode transformation) => this.AppendExpandExpression(transformation.ExpandClause.SelectedItems.Single<SelectItem>() as ExpandedNavigationSelectItem);

    private void AppendExpandExpression(ExpandedNavigationSelectItem expandedNavigation)
    {
      this.AppendExpression(expandedNavigation.PathToNavigationProperty);
      if (expandedNavigation.FilterOption != null)
      {
        this.AppendComma(true);
        this.query.Append("%20");
        this.query.Append("filter");
        this.query.Append("(");
        this.AppendExpression((QueryNode) expandedNavigation.FilterOption.Expression);
        this.query.Append(")");
      }
      if (expandedNavigation.SelectAndExpand == null)
        return;
      foreach (ExpandedNavigationSelectItem expandedNavigation1 in expandedNavigation.SelectAndExpand.SelectedItems.OfType<ExpandedNavigationSelectItem>())
      {
        this.AppendComma(true);
        this.query.Append("%20");
        this.query.Append("expand");
        this.query.Append("(");
        this.AppendExpandExpression(expandedNavigation1);
        this.query.Append(")");
      }
    }

    private void Translate(FilterTransformationNode transformation) => this.AppendExpression((QueryNode) transformation.FilterClause.Expression);

    private void Translate(GroupByTransformationNode transformation)
    {
      bool appendComma = false;
      foreach (GroupByPropertyNode groupingProperty in transformation.GroupingProperties)
      {
        if (appendComma)
        {
          this.AppendComma(appendComma);
        }
        else
        {
          appendComma = true;
          this.query.Append("(");
        }
        this.Translate(groupingProperty);
      }
      if (appendComma)
        this.query.Append(")");
      if (transformation.ChildTransformations == null)
        return;
      this.AppendComma(true);
      this.Translate(transformation.ChildTransformations);
    }

    private void Translate(GroupByPropertyNode node)
    {
      if (node.Expression != null)
        this.AppendExpression((QueryNode) node.Expression);
      bool appendComma = false;
      foreach (GroupByPropertyNode childTransformation in (IEnumerable<GroupByPropertyNode>) node.ChildTransformations)
      {
        appendComma = this.AppendComma(appendComma);
        this.Translate(childTransformation);
      }
    }

    private void Translate(TransformationNode transformation)
    {
      switch (transformation.Kind)
      {
        case TransformationNodeKind.Aggregate:
          this.query.Append("aggregate");
          break;
        case TransformationNodeKind.GroupBy:
          this.query.Append("groupby");
          break;
        case TransformationNodeKind.Filter:
          this.query.Append("filter");
          break;
        case TransformationNodeKind.Compute:
          this.query.Append("compute");
          break;
        case TransformationNodeKind.Expand:
          this.query.Append("expand");
          break;
        default:
          throw new NotSupportedException("unknown TransformationNodeKind value " + transformation.Kind.ToString());
      }
      this.query.Append("(");
      switch (transformation)
      {
        case GroupByTransformationNode transformation1:
          this.Translate(transformation1);
          break;
        case AggregateTransformationNode transformation2:
          this.Translate(transformation2);
          break;
        case FilterTransformationNode transformation3:
          this.Translate(transformation3);
          break;
        case ComputeTransformationNode transformation4:
          this.Translate(transformation4);
          break;
        case ExpandTransformationNode transformation5:
          this.Translate(transformation5);
          break;
        default:
          throw new NotSupportedException("unknown TransformationNode type " + transformation.GetType().Name);
      }
      this.query.Append(")");
    }
  }
}
