// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.CodeRelevanceRulesProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.RulesProviders;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance
{
  public class CodeRelevanceRulesProvider : RelevanceRuleProviderBase
  {
    private readonly DocumentContractType m_documentContactType;

    public CodeRelevanceRulesProvider(DocumentContractType documentContractType) => this.m_documentContactType = documentContractType;

    public override IEnumerable<RelevanceRule> GetRelevanceRules(IVssRequestContext requestContext)
    {
      List<RelevanceRule> relevanceRules = new List<RelevanceRule>();
      if (!requestContext.IsFeatureEnabled("Search.Server.Code.BoostRelevance"))
        return (IEnumerable<RelevanceRule>) relevanceRules;
      int currentTimeInEpoch = RelevanceUtility.GetCurrentTimeInEpoch();
      string timeStampFieldName = CodeFileContract.GetIndexedTimeStampFieldName(this.m_documentContactType);
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) currentTimeInEpoch);
      List<RelevanceRule> relevanceRuleList = relevanceRules;
      FunctionDecayRule<string, string> functionDecayRule = new FunctionDecayRule<string, string>();
      functionDecayRule.Description = "Boost Code search results based on the last indexed timestamp.";
      functionDecayRule.Target = Target.RootNode;
      functionDecayRule.Origin = str;
      functionDecayRule.Scale = "1d";
      functionDecayRule.Function = DecayFunction.BellCurve;
      functionDecayRule.Field = timeStampFieldName;
      relevanceRuleList.Add((RelevanceRule) functionDecayRule);
      return (IEnumerable<RelevanceRule>) relevanceRules;
    }
  }
}
