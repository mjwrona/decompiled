// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Highlight.QueryStringBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Highlight
{
  internal static class QueryStringBuilder
  {
    private static readonly IReadOnlyDictionary<Type, IHighlightQueryBuilder> s_expressionToQueryBuilderMap = (IReadOnlyDictionary<Type, IHighlightQueryBuilder>) new Dictionary<Type, IHighlightQueryBuilder>()
    {
      [typeof (OrExpression)] = (IHighlightQueryBuilder) new OrExpressionQueryBuilder(),
      [typeof (AndExpression)] = (IHighlightQueryBuilder) new AndExpressionQueryBuilder(),
      [typeof (TermExpression)] = (IHighlightQueryBuilder) new TermExpressionQueryBuilder()
    };

    public static string ToElasticSearchHighlightQuery(
      this IExpression expression,
      string fieldName,
      out bool isFilteredQuery)
    {
      isFilteredQuery = false;
      if (expression == null)
        throw new ArgumentNullException(nameof (expression));
      if (string.IsNullOrEmpty(fieldName))
        throw new ArgumentNullException(nameof (fieldName));
      Type type = expression.GetType();
      return QueryStringBuilder.s_expressionToQueryBuilderMap.ContainsKey(type) ? QueryStringBuilder.s_expressionToQueryBuilderMap[type].Build(expression, fieldName, out isFilteredQuery) : string.Empty;
    }
  }
}
