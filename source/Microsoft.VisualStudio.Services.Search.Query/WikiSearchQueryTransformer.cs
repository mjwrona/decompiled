// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.WikiSearchQueryTransformer
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
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  internal class WikiSearchQueryTransformer : ISearchQueryTransformer
  {
    [StaticSafe]
    private static readonly ISet<string> s_supportedOperators = (ISet<string>) new HashSet<string>()
    {
      "AND",
      "OR",
      "NOT",
      "(",
      ")",
      ":",
      "\""
    };
    [StaticSafe]
    private static readonly CompositeTermCorrector s_compositeTermCorrector = new CompositeTermCorrector(new ICorrector[6]
    {
      (ICorrector) new WikiInlineFieldCorrector(),
      (ICorrector) new WildcardTermCorrector(),
      (ICorrector) new WikiNormalizationTermCorrector(),
      (ICorrector) new WildcardTermCorrector(),
      (ICorrector) new PhraseWildcardCorrector(),
      (ICorrector) new PostfixWildcardTermCorrector()
    });
    [StaticSafe]
    private static readonly EmptyExpressionCorrector s_emptyExpressionCorrector = new EmptyExpressionCorrector();
    [StaticSafe]
    private static readonly IEnumerable<IRelevanceRuleCorrector> s_relevanceRulesCorrector = (IEnumerable<IRelevanceRuleCorrector>) new List<IRelevanceRuleCorrector>();

    public IExpression CorrectQuery(IVssRequestContext requestContext, IExpression queryParseTree)
    {
      IExpression expression = CorrectorTraverser.CorrectTerms(requestContext, queryParseTree, (ICorrector) WikiSearchQueryTransformer.s_compositeTermCorrector);
      return WikiSearchQueryTransformer.s_emptyExpressionCorrector.Correct(requestContext, expression);
    }

    public IExpression ParseSearchText(string searchText) => searchText != null ? new QueryParser(WikiSearchQueryTransformer.s_supportedOperators).Parse(searchText) : throw new ArgumentNullException(nameof (searchText));

    public IEnumerable<RelevanceRule> CorrectRelevanceRules(
      IVssRequestContext requestContext,
      IEnumerable<RelevanceRule> rules)
    {
      List<RelevanceRule> relevanceRuleList = new List<RelevanceRule>();
      foreach (RelevanceRule rule1 in rules)
      {
        RelevanceRule rule2 = rule1;
        if (WikiSearchQueryTransformer.s_relevanceRulesCorrector.Any<IRelevanceRuleCorrector>())
        {
          foreach (IRelevanceRuleCorrector relevanceRuleCorrector in WikiSearchQueryTransformer.s_relevanceRulesCorrector)
            relevanceRuleList.Add(relevanceRuleCorrector.Correct(requestContext, rule2));
        }
        else
          relevanceRuleList.Add(rule1);
      }
      return (IEnumerable<RelevanceRule>) relevanceRuleList;
    }
  }
}
