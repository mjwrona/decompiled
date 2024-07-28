// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors.CompositeTermCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors
{
  public class CompositeTermCorrector : TermCorrector
  {
    private ICorrector[] m_correctors;

    public CompositeTermCorrector(ICorrector[] correctors) => this.m_correctors = correctors;

    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      IExpression expression1 = termExpression != null ? (IExpression) termExpression : throw new ArgumentNullException(nameof (termExpression));
      for (int index = 0; index < this.m_correctors.Length; ++index)
      {
        IExpression expression2 = !(expression1 is TermExpression termExpression1) ? CorrectorTraverser.CorrectTerms(requestContext, expression1, this.m_correctors[index]) : this.m_correctors[index].CorrectTerm(requestContext, termExpression1);
        if (expression2 != null)
          expression1 = expression2;
      }
      return expression1;
    }
  }
}
