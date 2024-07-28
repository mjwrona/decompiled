// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.UnsupportedCharTermCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class UnsupportedCharTermCorrector : TermCorrector
  {
    private static readonly Regex s_defaultCorrectionCharsRegex = new Regex("[^\\w\\s\\?\\*]", RegexOptions.Compiled);
    [StaticSafe]
    private static Regex s_supportedCharacterTermRegex = new Regex("[\\uD840-\\uD86D\\uD80C-\\uD9FF\\uDB80-\\uDBFF][\\uDC00-\\uDFFF\\uDE00-\\uDEDF\\uDD00-\\uDDFF]|\uD86C\uDF63|\uD86B\uDFA2|\uD86C\uDF6F|\uD86C\uDF72|\uD86D\uDEED|\uD86C\uDF00|\uD86C\uDF7D|\u9FD6|\u9FD7|\u9FDA|\u9FDB|\u9FE9|\u9FEA|\u9FEE|\u9FEF|℉|〒|㏑|﹫", RegexOptions.Compiled);
    private const string OnlyDoubleQuote = "\"\"";

    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      if (termExpression == null)
        throw new ArgumentNullException(nameof (termExpression));
      if ("\"\"".Equals(termExpression.Value.Trim()))
        return (IExpression) new EmptyExpression();
      if (this.ShouldApplyUnSupportedTermCorrector(termExpression))
      {
        Regex correctionCharsRegex = UnsupportedCharTermCorrector.s_defaultCorrectionCharsRegex;
        string replacement = " ";
        if (UnsupportedCharTermCorrector.s_supportedCharacterTermRegex.Match(termExpression.Value).Success)
          return (IExpression) termExpression;
        termExpression.Value = correctionCharsRegex.Replace(termExpression.Value, replacement).Trim();
        if (string.IsNullOrWhiteSpace(termExpression.Value))
          return (IExpression) new EmptyExpression();
      }
      return (IExpression) termExpression;
    }

    internal virtual bool ShouldApplyUnSupportedTermCorrector(TermExpression termExpression) => termExpression.IsOfType("*") || CodeSearchFilters.CEFilterIds.Contains(termExpression.Type);
  }
}
