// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.QueryStringBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Relevance;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders
{
  internal static class QueryStringBuilder
  {
    private static readonly IReadOnlyDictionary<Type, IPlatformQueryBuilder> s_expressionToQueryBuilderMap = (IReadOnlyDictionary<Type, IPlatformQueryBuilder>) new Dictionary<Type, IPlatformQueryBuilder>()
    {
      [typeof (OrExpression)] = (IPlatformQueryBuilder) new OrExpressionQueryBuilder(),
      [typeof (AndExpression)] = (IPlatformQueryBuilder) new AndExpressionQueryBuilder(),
      [typeof (NotExpression)] = (IPlatformQueryBuilder) new NotExpressionQueryBuilder(),
      [typeof (Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.TermExpression)] = (IPlatformQueryBuilder) new TermExpressionQueryBuilder(),
      [typeof (EmptyExpression)] = (IPlatformQueryBuilder) new EmptyExpressionQueryBuilder(),
      [typeof (TermsExpression)] = (IPlatformQueryBuilder) new TermsExpressionQueryBuilder(),
      [typeof (MissingFieldExpression)] = (IPlatformQueryBuilder) new MissingFieldExpressionQueryBuilder(),
      [typeof (CodeElementFilterExpression)] = (IPlatformQueryBuilder) new CodeElementFilterExpressionBuilder()
    };
    private static readonly IReadOnlyDictionary<Type, IRelevanceQueryBuilder> s_relevanceRuleToQueryBuilderMap = (IReadOnlyDictionary<Type, IRelevanceQueryBuilder>) new Dictionary<Type, IRelevanceQueryBuilder>()
    {
      [typeof (TermBoostExpression)] = (IRelevanceQueryBuilder) new TermBoostQueryBuilder(),
      [typeof (FunctionDecayExpression<DateTime, string>)] = (IRelevanceQueryBuilder) new FunctionDecayQueryBuilder<DateTime, string>(),
      [typeof (FunctionDecayExpression<long, long>)] = (IRelevanceQueryBuilder) new FunctionDecayQueryBuilder<long, long>(),
      [typeof (FunctionDecayExpression<float, float>)] = (IRelevanceQueryBuilder) new FunctionDecayQueryBuilder<float, float>(),
      [typeof (FunctionDecayExpression<double, double>)] = (IRelevanceQueryBuilder) new FunctionDecayQueryBuilder<double, double>(),
      [typeof (FunctionDecayExpression<string, string>)] = (IRelevanceQueryBuilder) new FunctionDecayQueryBuilder<string, string>()
    };

    public static string ToElasticSearchQuery(
      this IExpression expression,
      IVssRequestContext requestContext,
      IEntityType entityType,
      DocumentContractType contractType,
      bool enableRanking,
      bool allowSpellingErrors,
      string requestId,
      ResultsCountPlatformRequest request = null)
    {
      if (expression == null)
        throw new ArgumentNullException(nameof (expression));
      if (requestId == null)
        throw new ArgumentNullException(nameof (requestId));
      if (allowSpellingErrors && entityType.Name != "WorkItem" && entityType.Name != "ProjectRepo")
        throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Handling spelling errors during search is only supported for work item and project entities. Entity found instead is [{0}].", (object) entityType.Name)));
      Type type1 = expression.GetType();
      RelevanceExpression relevanceExpression = (RelevanceExpression) expression;
      if (!QueryStringBuilder.s_expressionToQueryBuilderMap.ContainsKey(type1))
        throw new NotImplementedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Expression [{0}] is not supported yet", (object) expression.ToString()));
      string baseQuery = QueryStringBuilder.s_expressionToQueryBuilderMap[type1].Build(requestContext, expression, entityType, contractType, enableRanking, allowSpellingErrors, requestId, request);
      if (!string.IsNullOrWhiteSpace(baseQuery) && (relevanceExpression.HasRelevanceExpressions() || relevanceExpression.HasChildDocumentRelevanceExpressions()))
      {
        List<IRelevanceExpression> relevanceExpressionList = new List<IRelevanceExpression>();
        if (contractType != request.ContractType && relevanceExpression.HasChildDocumentRelevanceExpressions())
          relevanceExpressionList.AddRange(relevanceExpression.GetChildDocumentRelevanceExpressions());
        else if (relevanceExpression.HasRelevanceExpressions())
          relevanceExpressionList.AddRange(relevanceExpression.GetRelevanceExpressions());
        foreach (IRelevanceExpression rule in relevanceExpressionList)
        {
          Type type2 = rule.GetType();
          if (QueryStringBuilder.s_relevanceRuleToQueryBuilderMap.ContainsKey(type2))
            baseQuery = QueryStringBuilder.s_relevanceRuleToQueryBuilderMap[type2].Build(baseQuery, rule);
        }
      }
      return baseQuery;
    }
  }
}
