// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.Relevance.IRelevanceRuleCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors.Relevance
{
  internal interface IRelevanceRuleCorrector
  {
    RelevanceRule Correct(IVssRequestContext requestContext, RelevanceRule rule);
  }
}
