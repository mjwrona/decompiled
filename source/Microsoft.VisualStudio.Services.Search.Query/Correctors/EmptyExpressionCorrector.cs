// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.EmptyExpressionCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class EmptyExpressionCorrector : ICorrector
  {
    public IExpression CorrectTerm(IVssRequestContext requestContext, TermExpression termExpression) => (IExpression) termExpression;

    public IExpression Correct(IVssRequestContext requestContext, IExpression expression)
    {
      if (expression == null)
        throw new ArgumentNullException(nameof (expression));
      if (expression is TermExpression)
        return expression;
      List<IExpression> expressionList = new List<IExpression>();
      for (int index = 0; index < expression.Children.Length; ++index)
      {
        IExpression expression1 = expression.Children[index];
        if (!(expression1 is TermExpression))
          expression1 = this.Correct(requestContext, expression1);
        if (!(expression1 is EmptyExpression))
          expressionList.Add(expression1);
      }
      IExpression expression2 = (IExpression) null;
      if (expressionList.Count == 0)
        expression2 = (IExpression) new EmptyExpression();
      else if (expression is NotExpression)
        expression2 = (IExpression) new NotExpression(expressionList[0]);
      else if (expressionList.Count == 1)
      {
        expression2 = expressionList[0];
      }
      else
      {
        switch (expression)
        {
          case AndExpression _:
            expression2 = (IExpression) new AndExpression(expressionList.ToArray());
            break;
          case OrExpression _:
            expression2 = (IExpression) new OrExpression(expressionList.ToArray());
            break;
        }
      }
      return expression2;
    }
  }
}
