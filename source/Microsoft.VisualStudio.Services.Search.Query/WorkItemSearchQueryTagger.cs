// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.WorkItemSearchQueryTagger
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Expression.WorkItem;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  internal class WorkItemSearchQueryTagger
  {
    private readonly IExpression m_expression;
    private readonly IDictionary<string, IEnumerable<string>> m_searchFacets;
    private int m_numUnfilteredWords;

    public SortedSet<string> Tags { get; }

    public WorkItemSearchQueryTagger(
      IExpression expression,
      IDictionary<string, IEnumerable<string>> searchFacets)
    {
      this.m_expression = expression;
      this.m_searchFacets = searchFacets;
      this.Tags = new SortedSet<string>((IComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public void Compute()
    {
      this.TagFacets();
      this.TagExpression();
      this.TagForUnfilteredWords();
    }

    public void Publish()
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("WorkItemSearchRequestTags", this.ToString());
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("Query Pipeline", "Query Pipeline", properties);
    }

    public override string ToString() => string.Join(", ", (IEnumerable<string>) this.Tags);

    private void TagExpression()
    {
      foreach (IExpression expression in (IEnumerable<IExpression>) this.m_expression)
      {
        switch (expression)
        {
          case TermExpression termExpression:
            this.TagTermExpression(termExpression);
            continue;
          case AndExpression _:
            this.Tags.Add("And");
            continue;
          case OrExpression _:
            this.Tags.Add("Or");
            continue;
          case NotExpression _:
            this.Tags.Add("Not");
            continue;
          case EmptyExpression _:
            this.Tags.Add("NoWords");
            continue;
          default:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081321, "Query Pipeline", "Query", FormattableString.Invariant(FormattableStringFactory.Create("IExpression type [{0}] is not handled in {1}.", (object) expression.GetType(), (object) nameof (WorkItemSearchQueryTagger))));
            continue;
        }
      }
    }

    private void TagTermExpression(TermExpression termExpression)
    {
      bool flag1 = termExpression.IsOfType("*");
      if (flag1)
        ++this.m_numUnfilteredWords;
      else
        this.Tags.Add("FilteredTerm");
      if (termExpression.Value.ContainsWhitespace())
        this.Tags.Add("Phrase");
      switch (termExpression.Operator)
      {
        case Operator.Equals:
          this.Tags.Add("EqualsOp");
          break;
        case Operator.NotEquals:
          this.Tags.Add("NotEqualsOp");
          break;
        case Operator.LessThan:
          this.Tags.Add("LessThanOp");
          break;
        case Operator.LessThanOrEqual:
          this.Tags.Add("LessThanEqualsOp");
          break;
        case Operator.GreaterThan:
          this.Tags.Add("GreaterThanOp");
          break;
        case Operator.GreaterThanOrEqual:
          this.Tags.Add("GreaterThanEqualsOp");
          break;
        default:
          if (termExpression.Operator != Operator.Matches)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081322, "Query Pipeline", "Query", FormattableString.Invariant(FormattableStringFactory.Create("Unexpected operator [{0}] found.", (object) termExpression.Operator)));
            break;
          }
          break;
      }
      ExpressionBuilderUtils.DataType resolvableDataTypesOf = ExpressionBuilderUtils.GetResolvableDataTypesOf(termExpression.Value);
      switch (resolvableDataTypesOf)
      {
        case ExpressionBuilderUtils.DataType.String:
          bool flag2 = termExpression.Value.IndexOf('*') >= 0;
          if (flag2)
            this.Tags.Add("WildcardAsterisk");
          bool flag3 = termExpression.Value.IndexOf('?') >= 0;
          if (flag3)
            this.Tags.Add("WildcardQuestion");
          if (!flag1 || !(flag2 | flag3))
            break;
          switch (termExpression.Value[0])
          {
            case '*':
            case '?':
              this.Tags.Add("UnfilteredPrefixWildcard");
              break;
          }
          switch (termExpression.Value[termExpression.Value.Length - 1])
          {
            case '*':
            case '?':
              this.Tags.Add("UnfilteredPostfixWildcard");
              return;
            default:
              return;
          }
        case ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.String:
          this.Tags.Add("DateTimeValue");
          goto case ExpressionBuilderUtils.DataType.String;
        case ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.String:
          this.Tags.Add("DoubleValue");
          goto case ExpressionBuilderUtils.DataType.String;
        case ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.String:
          this.Tags.Add("DateOrDoubleValue");
          goto case ExpressionBuilderUtils.DataType.String;
        case ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.Int32 | ExpressionBuilderUtils.DataType.String:
          this.Tags.Add("IntegerValue");
          goto case ExpressionBuilderUtils.DataType.String;
        default:
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081323, "Query Pipeline", "Query", FormattableString.Invariant(FormattableStringFactory.Create("Type combination [{0}] for value [{1}] is not handled.", (object) resolvableDataTypesOf, (object) termExpression.Value)));
          goto case ExpressionBuilderUtils.DataType.String;
      }
    }

    private void TagFacets()
    {
      int num = 0;
      if (this.m_searchFacets.ContainsKey(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project))
      {
        this.Tags.Add("ProjectFacets");
        ++num;
      }
      if (this.m_searchFacets.ContainsKey(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAreaPath))
      {
        this.Tags.Add("AreaFacets");
        ++num;
      }
      if (this.m_searchFacets.ContainsKey(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAssignedTo))
      {
        this.Tags.Add("AssignedToFacets");
        ++num;
      }
      if (this.m_searchFacets.ContainsKey(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemState))
      {
        this.Tags.Add("StateFacets");
        ++num;
      }
      if (this.m_searchFacets.ContainsKey(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemType))
      {
        this.Tags.Add("TypeFacets");
        ++num;
      }
      if (this.m_searchFacets.Count == num)
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081320, "Query Pipeline", "Query", FormattableString.Invariant(FormattableStringFactory.Create("[{0}] contains unrecognized search filter category(ies).", (object) string.Join(", ", (IEnumerable<string>) this.m_searchFacets.Keys))));
    }

    private void TagForUnfilteredWords()
    {
      if (this.m_numUnfilteredWords == 1)
      {
        this.Tags.Add("SingleUnfilteredWord");
      }
      else
      {
        if (this.m_numUnfilteredWords <= 1)
          return;
        this.Tags.Add("MultipleUnfilteredWords");
      }
    }

    public static class Constants
    {
      public const string And = "And";
      public const string Or = "Or";
      public const string Not = "Not";
      public const string NoWords = "NoWords";
      public const string Phrase = "Phrase";
      public const string WildcardAsterisk = "WildcardAsterisk";
      public const string WildcardQuestion = "WildcardQuestion";
      public const string UnfilteredPostfixWildcard = "UnfilteredPostfixWildcard";
      public const string UnfilteredPrefixWildcard = "UnfilteredPrefixWildcard";
      public const string SingleUnfilteredWord = "SingleUnfilteredWord";
      public const string MultipleUnfilteredWords = "MultipleUnfilteredWords";
      public const string ProjectFacets = "ProjectFacets";
      public const string StateFacets = "StateFacets";
      public const string AssignedToFacets = "AssignedToFacets";
      public const string TypeFacets = "TypeFacets";
      public const string AreaFacets = "AreaFacets";
      public const string FilteredTerm = "FilteredTerm";
      public const string LessThanOperator = "LessThanOp";
      public const string LessThanOrEqualsOperator = "LessThanEqualsOp";
      public const string GreaterThanOperator = "GreaterThanOp";
      public const string GreaterThanOrEqualsOperator = "GreaterThanEqualsOp";
      public const string EqualsOperator = "EqualsOp";
      public const string NotEqualsOperator = "NotEqualsOp";
      public const string DateTimeValue = "DateTimeValue";
      public const string IntegerValue = "IntegerValue";
      public const string DoubleValue = "DoubleValue";
      public const string DateOrDoubleValue = "DateOrDoubleValue";
    }
  }
}
