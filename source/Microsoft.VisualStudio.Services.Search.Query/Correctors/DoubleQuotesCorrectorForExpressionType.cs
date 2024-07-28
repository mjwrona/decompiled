// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.DoubleQuotesCorrectorForExpressionType
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class DoubleQuotesCorrectorForExpressionType : DoubleQuotesCorrector
  {
    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      TermExpression termExpression1 = termExpression != null ? termExpression : throw new ArgumentNullException(nameof (termExpression));
      if (!termExpression1.IsOfType("*") && termExpression1.Type.StartsWith("\"", StringComparison.Ordinal) && termExpression1.Type.EndsWith("\"", StringComparison.Ordinal))
        termExpression1.Type = this.TrimWrappingDoubleQuotesForNonDefaultExpression(termExpression1.Type);
      return (IExpression) termExpression1;
    }

    private string TrimWrappingDoubleQuotesForNonDefaultExpression(string term)
    {
      if (!string.IsNullOrWhiteSpace(term))
        term = term.Substring(1, term.Length - 2);
      return term;
    }
  }
}
