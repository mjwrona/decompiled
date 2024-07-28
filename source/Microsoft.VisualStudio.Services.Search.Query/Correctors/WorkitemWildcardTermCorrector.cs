// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.WorkitemWildcardTermCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class WorkitemWildcardTermCorrector : TermCorrector
  {
    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      bool flag1 = termExpression.Type.Equals("*");
      bool flag2 = termExpression.Value.Equals("*");
      if (flag1 & flag2)
        return (IExpression) new EmptyExpression();
      if (flag1)
        termExpression.Operator = Operator.Matches;
      else if (flag2)
      {
        termExpression.Value = termExpression.Type;
        termExpression.Type = "*";
        termExpression.Operator = Operator.Matches;
      }
      return (IExpression) termExpression;
    }
  }
}
