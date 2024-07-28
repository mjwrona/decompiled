// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.Relevance.CurrentUserCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors.Relevance
{
  public class CurrentUserCorrector : IRelevanceRuleCorrector
  {
    public RelevanceRule Correct(IVssRequestContext requestContext, RelevanceRule rule)
    {
      if (!(rule is TermBoostRule termBoostRule))
        return rule;
      string distinctDisplayName = this.GetDistinctDisplayName(requestContext);
      if (distinctDisplayName != null)
      {
        List<TermBoostExpression.TermExpression> termExpressionList = new List<TermBoostExpression.TermExpression>(termBoostRule.TermsDescriptor.Count);
        foreach (TermBoostExpression.TermExpression termExpression in termBoostRule.TermsDescriptor)
        {
          if (termExpression.Terms[0].Contains("@current_user"))
            termExpression.Terms = new List<string>()
            {
              termExpression.Terms[0].Replace("@current_user", distinctDisplayName.ToLowerInvariant())
            };
          termExpressionList.Add(termExpression);
        }
        termBoostRule.TermsDescriptor = termExpressionList;
      }
      return (RelevanceRule) termBoostRule;
    }

    internal virtual string GetDistinctDisplayName(IVssRequestContext requestContext)
    {
      string distinctDisplayName = (string) null;
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        if (userIdentity != null)
          distinctDisplayName = new Microsoft.VisualStudio.Services.Search.Query.IdentityHelper(userIdentity).GetDistinctDisplayName();
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1082103, "Query Pipeline", "Query", ex);
      }
      return distinctDisplayName;
    }
  }
}
