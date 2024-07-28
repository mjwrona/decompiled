// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Highlight.TermExpressionQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Expression.WorkItem;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Highlight
{
  internal class TermExpressionQueryBuilder : IHighlightQueryBuilder
  {
    private string m_fieldName;

    public string Build(IExpression expression, string fieldName, out bool isFilteredQuery)
    {
      this.m_fieldName = fieldName;
      TermExpression termExpression = (TermExpression) expression;
      isFilteredQuery = false;
      switch (termExpression.Operator)
      {
        case Operator.Equals:
        case Operator.Matches:
          return this.GetHighlightQueryString(termExpression, out isFilteredQuery);
        default:
          return string.Empty;
      }
    }

    private string GetHighlightQueryString(TermExpression termExpression, out bool isFilteredQuery)
    {
      isFilteredQuery = !termExpression.IsOfType("*");
      return termExpression.Value.Contains("*") || termExpression.Value.Contains("?") ? this.GetWildcardQueryString(termExpression, isFilteredQuery) : this.GetMatchQueryStringForAllDataTypes(termExpression, isFilteredQuery);
    }

    private string GetWildcardQueryString(TermExpression termExpression, bool isFilteredQuery)
    {
      string queryString = termExpression.Value.NormalizePath();
      return !isFilteredQuery ? this.GetWildcardQueryString(queryString) : this.GetFilteredWildcardQueryString(queryString, termExpression.Type);
    }

    private string GetMatchQueryStringForAllDataTypes(
      TermExpression termExpression,
      bool isFilteredQuery)
    {
      ExpressionBuilderUtils.DataType resolvableDataTypesOf = ExpressionBuilderUtils.GetResolvableDataTypesOf(termExpression.Value);
      string queryString;
      switch (resolvableDataTypesOf)
      {
        case ExpressionBuilderUtils.DataType.String:
          queryString = termExpression.Value.NormalizePath();
          break;
        case ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.String:
        case ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.String:
        case ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.String:
        case ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.Int32 | ExpressionBuilderUtils.DataType.String:
          queryString = termExpression.Value;
          break;
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Type combination [{0}] for value [{1}] is not supported.", (object) resolvableDataTypesOf, (object) termExpression.Value)));
      }
      return !isFilteredQuery ? this.GetMultiMatchQueryString(queryString) : this.GetFilteredMultiMatchQueryString(queryString, termExpression.Type);
    }

    private string GetFilteredMultiMatchQueryString(string queryString, string fieldReferenceName)
    {
      string referenceName = WorkItemIndexedField.FromPlatformFieldName(this.m_fieldName)?.ReferenceName;
      return !string.Equals(fieldReferenceName, referenceName, StringComparison.OrdinalIgnoreCase) ? string.Empty : this.GetMultiMatchQueryString(queryString);
    }

    private string GetFilteredWildcardQueryString(string queryString, string fieldReferenceName)
    {
      string referenceName = WorkItemIndexedField.FromPlatformFieldName(this.m_fieldName)?.ReferenceName;
      return !string.Equals(fieldReferenceName, referenceName, StringComparison.OrdinalIgnoreCase) ? string.Empty : this.GetWildcardQueryString(queryString);
    }

    private string GetMultiMatchQueryString(string queryString)
    {
      queryString = this.HandleCurlyBraces(queryString);
      return FormattableString.Invariant(FormattableStringFactory.Create("{{{{\r\n                    \"match_phrase\": {{{{\r\n                        \"{{0}}\":{{{{\r\n                            \"query\": {0}\r\n                        }}}}\r\n                    }}}}\r\n                }}}}", (object) JsonConvert.SerializeObject((object) queryString)));
    }

    private string GetWildcardQueryString(string queryString)
    {
      queryString = this.HandleCurlyBraces(queryString);
      return FormattableString.Invariant(FormattableStringFactory.Create("{{{{\r\n                    \"query_string\": {{{{\r\n                        \"query\": {0},\r\n                        \"default_field\":\"{{0}}\"\r\n                    }}}}\r\n                }}}}", (object) JsonConvert.SerializeObject((object) queryString)));
    }

    private string HandleCurlyBraces(string queryString) => queryString.Replace("{", "{{").Replace("}", "}}");
  }
}
