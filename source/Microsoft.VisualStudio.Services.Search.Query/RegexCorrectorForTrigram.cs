// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.RegexCorrectorForTrigram
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class RegexCorrectorForTrigram : TermCorrector
  {
    private IRegexQueryExpressionConstructor m_regexTransformer;

    public RegexCorrectorForTrigram(IRegexQueryExpressionConstructor regexTransformer) => this.m_regexTransformer = regexTransformer;

    public RegexCorrectorForTrigram()
      : this((IRegexQueryExpressionConstructor) new RegexQueryExpressionConstructor())
    {
    }

    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      if (termExpression == null)
        throw new ArgumentNullException(nameof (termExpression));
      return termExpression.IsOfType("regex") && requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableTrigramSearch", TeamFoundationHostType.ProjectCollection) ? this.m_regexTransformer.ConstructRegexQueryExpression(requestContext, termExpression) : (IExpression) termExpression;
    }
  }
}
