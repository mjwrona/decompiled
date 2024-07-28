// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.TodayCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class TodayCorrector : TermCorrector
  {
    [StaticSafe]
    private static Regex s_macroRegex = new Regex("^@today\\s*(?<Sign>[+-])\\s*(?<Days>\\d+\\.?\\d*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private IVssDateTimeProvider m_dateTimeProvider;

    public TodayCorrector() => this.m_dateTimeProvider = VssDateTimeProvider.DefaultProvider;

    public TodayCorrector(IVssDateTimeProvider dateTimeProvider) => this.m_dateTimeProvider = dateTimeProvider;

    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      string input = termExpression != null ? termExpression.Value.ToString().Trim() : throw new ArgumentNullException(nameof (termExpression));
      TermExpression termExpression1 = (TermExpression) null;
      if (input.Equals("@today", StringComparison.OrdinalIgnoreCase))
      {
        termExpression1 = new TermExpression(termExpression.Type, termExpression.Operator, this.m_dateTimeProvider.UtcNow.Date.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
      }
      else
      {
        Match match = TodayCorrector.s_macroRegex.Match(input);
        if (match.Success)
        {
          int num1 = (int) char.Parse(match.Groups["Sign"].Value);
          double num2 = double.Parse(match.Groups["Days"].Value, (IFormatProvider) CultureInfo.InvariantCulture);
          if (num1 == 45)
            num2 = -num2;
          string type = termExpression.Type;
          int op = (int) termExpression.Operator;
          DateTime dateTime = this.m_dateTimeProvider.UtcNow.Date;
          dateTime = dateTime.AddDays(num2);
          string str = dateTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);
          termExpression1 = new TermExpression(type, (Operator) op, str);
        }
      }
      return (IExpression) termExpression1;
    }
  }
}
