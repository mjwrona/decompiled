// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.WikiRelevanceRulesProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.RulesProviders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance
{
  public class WikiRelevanceRulesProvider : RelevanceRuleProviderBase
  {
    public override IEnumerable<RelevanceRule> GetRelevanceRules(IVssRequestContext requestContext)
    {
      double configValueOrDefault = requestContext.GetConfigValueOrDefault("/service/ALMSearch/Settings/WikiDocumentNoContentBoostValue", 0.05);
      List<RelevanceRule> relevanceRuleList = new List<RelevanceRule>();
      TermBoostRule termBoostRule = new TermBoostRule();
      termBoostRule.Description = "Negative boost wiki documents, if content of the wiki page is empty.";
      termBoostRule.Target = Target.RootNode;
      termBoostRule.TermsDescriptor = new List<TermBoostExpression.TermExpression>()
      {
        new TermBoostExpression.TermExpression()
        {
          Terms = new List<string>() { "Empty" },
          Boost = configValueOrDefault,
          FieldName = "contentMetadata"
        }
      };
      relevanceRuleList.Add((RelevanceRule) termBoostRule);
      List<RelevanceRule> relevanceRules = relevanceRuleList;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
      {
        RelevanceRule searchRelevanceRule = this.GetOrgWikiSearchRelevanceRule(requestContext);
        if (searchRelevanceRule != null)
          relevanceRules.Add(searchRelevanceRule);
      }
      return (IEnumerable<RelevanceRule>) relevanceRules;
    }

    private RelevanceRule GetOrgWikiSearchRelevanceRule(IVssRequestContext requestContext)
    {
      string configValue = requestContext.GetConfigValue("/service/OrgSearch/Settings/BoostWikiDocumentsFromCollections");
      if (!string.IsNullOrWhiteSpace(configValue))
      {
        List<string> list1 = ((IEnumerable<string>) configValue.Split(';')).Select<string, string>((Func<string, string>) (x => x.ToLower(CultureInfo.InvariantCulture))).ToList<string>();
        if (list1.Count > 0)
        {
          int boostValue = requestContext.GetConfigValueOrDefault("/service/OrgSearch/Settings/WikiDocumentsFromCollectionsBoostValue", 1);
          List<TermBoostExpression.TermExpression> list2 = list1.Select<string, TermBoostExpression.TermExpression>((Func<string, TermBoostExpression.TermExpression>) (collectionId => new TermBoostExpression.TermExpression()
          {
            Terms = new List<string>() { collectionId },
            Boost = (double) boostValue,
            FieldName = nameof (collectionId)
          })).ToList<TermBoostExpression.TermExpression>();
          TermBoostRule searchRelevanceRule = new TermBoostRule();
          searchRelevanceRule.Description = "Boost wiki documents present in specified collections.";
          searchRelevanceRule.Target = Target.RootNode;
          searchRelevanceRule.TermsDescriptor = list2;
          return (RelevanceRule) searchRelevanceRule;
        }
      }
      return (RelevanceRule) null;
    }
  }
}
