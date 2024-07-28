// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors.CorrectorTraverser
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors
{
  public static class CorrectorTraverser
  {
    public static IExpression CorrectTerms(
      IVssRequestContext requestContext,
      IExpression expression,
      ICorrector corrector)
    {
      if (expression is TermExpression termExpression1)
      {
        IExpression expression1 = corrector.CorrectTerm(requestContext, termExpression1);
        if (expression1 != null)
          return expression1;
      }
      else
      {
        IList<IExpression> children = (IList<IExpression>) expression.Children;
        for (int index = 0; index < children.Count; ++index)
        {
          IExpression expression2 = children[index];
          if (expression2 is TermExpression termExpression)
          {
            IExpression expression3 = corrector.CorrectTerm(requestContext, termExpression);
            if (expression3 != null)
              children[index] = expression3;
          }
          else
            CorrectorTraverser.CorrectTerms(requestContext, expression2, corrector);
        }
      }
      return expression;
    }
  }
}
