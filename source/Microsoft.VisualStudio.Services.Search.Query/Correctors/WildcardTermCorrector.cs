// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.WildcardTermCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class WildcardTermCorrector : TermCorrector
  {
    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      if (termExpression == null)
        throw new ArgumentNullException(nameof (termExpression));
      bool flag = !RegularExpressions.WildcardOnlyRegex.IsMatch(termExpression.Type);
      if (!RegularExpressions.WildcardOnlyRegex.IsMatch(termExpression.Value))
      {
        if (!flag)
          termExpression.Type = "*";
      }
      else
      {
        if (!flag)
          return (IExpression) new EmptyExpression();
        if (!CodeSearchFilters.SupportedFilterIds.Contains(termExpression.Type))
        {
          termExpression.Value = termExpression.Type;
          termExpression.Type = "*";
        }
        else
          return termExpression.Type == CodeFileContract.CodeContractQueryableElement.FileName.InlineFilterName() && RegularExpressions.MatchAllRegex.IsMatch(termExpression.Value) ? (IExpression) termExpression : (IExpression) new EmptyExpression();
      }
      return (IExpression) termExpression;
    }
  }
}
