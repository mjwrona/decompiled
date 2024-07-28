// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.RulesProviders.RelevanceRuleProviderBase
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.RulesProviders
{
  public abstract class RelevanceRuleProviderBase : IRelevanceRulesProvider
  {
    public abstract IEnumerable<RelevanceRule> GetRelevanceRules(IVssRequestContext requestContext);

    public virtual IEnumerable<RelevanceRule> GetChildQueryRelevanceRules(
      IVssRequestContext requestContext)
    {
      return (IEnumerable<RelevanceRule>) null;
    }
  }
}
