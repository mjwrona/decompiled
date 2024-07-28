// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.UnrecognizedTypeTermCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using System;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class UnrecognizedTypeTermCorrector : TermCorrector
  {
    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      if (termExpression == null)
        throw new ArgumentNullException(nameof (termExpression));
      return CodeSearchFilters.SupportedFilterIds.Contains(termExpression.Type) && !termExpression.IsOfType("regex") || termExpression.IsOfType("*") ? (IExpression) termExpression : (!termExpression.IsOfType("regex") || !requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableRegexpSearch", TeamFoundationHostType.ProjectCollection) ? (IExpression) new TermExpression("*", Operator.Matches, termExpression.Type + " " + termExpression.Value) : (IExpression) termExpression);
    }
  }
}
