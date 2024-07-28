// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.DoubleQuotesCorrectorForExpressionValue
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using System;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class DoubleQuotesCorrectorForExpressionValue : DoubleQuotesCorrector
  {
    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      TermExpression termExpression1 = termExpression != null ? termExpression : throw new ArgumentNullException(nameof (termExpression));
      if (CodeSearchFilters.CEFilterIds.Contains(termExpression1.Type) || termExpression1.IsOfType("*") || termExpression1.IsOfType(CodeFileContract.CodeContractQueryableElement.FilePath) || termExpression1.IsOfType(CodeFileContract.CodeContractQueryableElement.FileName))
      {
        if (termExpression1.Value.StartsWith("\"", StringComparison.Ordinal) && termExpression1.Value.EndsWith("\"", StringComparison.Ordinal))
          termExpression1.Value = termExpression1.Value.CorrectEscapedDoubleQuotes();
      }
      else if (termExpression1.Value.StartsWith("\"", StringComparison.Ordinal) && termExpression1.Value.EndsWith("\"", StringComparison.Ordinal))
        termExpression1.Value = this.TrimWrappingDoubleQuotesForNonDefaultExpression(termExpression1.Value.CorrectEscapedDoubleQuotes());
      return (IExpression) termExpression1;
    }

    private string TrimWrappingDoubleQuotesForNonDefaultExpression(string term)
    {
      if (!string.IsNullOrWhiteSpace(term))
        term = term.Length > 1 ? term.Substring(1, term.Length - 2) : term;
      return term;
    }
  }
}
