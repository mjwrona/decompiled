// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.PostfixWildcardTermCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class PostfixWildcardTermCorrector : TermCorrector
  {
    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      if ((!termExpression.Value.EndsWith("*", StringComparison.Ordinal) ? 0 : (!termExpression.Value.Contains(" ") ? 1 : 0)) == 0)
        return (IExpression) termExpression;
      return (IExpression) new OrExpression(new IExpression[2]
      {
        (IExpression) termExpression,
        (IExpression) new TermExpression(termExpression.Type, termExpression.Operator, termExpression.Value.TrimEnd('*'))
      });
    }
  }
}
