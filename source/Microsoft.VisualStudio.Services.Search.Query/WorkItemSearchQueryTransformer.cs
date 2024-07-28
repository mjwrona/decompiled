// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.WorkItemSearchQueryTransformer
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Model;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules;
using Microsoft.VisualStudio.Services.Search.Query.Correctors;
using Microsoft.VisualStudio.Services.Search.Query.Correctors.Relevance;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  internal class WorkItemSearchQueryTransformer : ISearchQueryTransformer
  {
    [StaticSafe]
    private static readonly ISet<string> s_supportedOperators = (ISet<string>) new HashSet<string>()
    {
      "AND",
      "OR",
      "NOT",
      "(",
      ")",
      "[",
      "]",
      "\"",
      ":",
      "<",
      "<=",
      ">",
      ">=",
      "=",
      "!="
    };
    [StaticSafe]
    private static readonly CompositeTermCorrector s_compositeTermCorrector = new CompositeTermCorrector(new ICorrector[7]
    {
      (ICorrector) new TermExpressionValueCorrector(),
      (ICorrector) new WorkitemWildcardTermCorrector(),
      (ICorrector) new PhraseWildcardCorrector(),
      (ICorrector) new TodayCorrector(),
      (ICorrector) new MeCorrector(),
      (ICorrector) new WorkItemSearchUnsupportedCharacterTermCorrector(),
      (ICorrector) new PostfixWildcardTermCorrector()
    });
    [StaticSafe]
    private static readonly IEnumerable<IRelevanceRuleCorrector> s_relevanceRulesCorrector = (IEnumerable<IRelevanceRuleCorrector>) new List<IRelevanceRuleCorrector>()
    {
      (IRelevanceRuleCorrector) new CurrentUserCorrector()
    };
    [StaticSafe]
    private static readonly EmptyExpressionCorrector s_emptyExpressionCorrector = new EmptyExpressionCorrector();
    [StaticSafe]
    private static readonly WorkItemFieldNameCorrector s_filterAliasCorrector = new WorkItemFieldNameCorrector();

    public IExpression CorrectQuery(IVssRequestContext requestContext, IExpression queryParseTree)
    {
      IExpression expression1 = CorrectorTraverser.CorrectTerms(requestContext, queryParseTree, (ICorrector) WorkItemSearchQueryTransformer.s_compositeTermCorrector);
      IExpression expression2 = WorkItemSearchQueryTransformer.s_emptyExpressionCorrector.Correct(requestContext, expression1);
      return WorkItemSearchQueryTransformer.s_filterAliasCorrector.Correct(requestContext, expression2);
    }

    public IExpression ParseSearchText(string searchText) => searchText != null ? new QueryParser(WorkItemSearchQueryTransformer.s_supportedOperators).Parse(searchText) : throw new ArgumentNullException(nameof (searchText));

    public IEnumerable<RelevanceRule> CorrectRelevanceRules(
      IVssRequestContext requestContext,
      IEnumerable<RelevanceRule> rules)
    {
      List<RelevanceRule> relevanceRuleList = new List<RelevanceRule>();
      foreach (RelevanceRule rule in rules)
      {
        foreach (IRelevanceRuleCorrector relevanceRuleCorrector in WorkItemSearchQueryTransformer.s_relevanceRulesCorrector)
          relevanceRuleList.Add(relevanceRuleCorrector.Correct(requestContext, rule));
      }
      return (IEnumerable<RelevanceRule>) relevanceRuleList;
    }
  }
}
