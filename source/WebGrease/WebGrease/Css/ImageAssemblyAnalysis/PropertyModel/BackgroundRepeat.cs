// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.ImageAssemblyAnalysis.PropertyModel.BackgroundRepeat
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using WebGrease.Css.Ast;
using WebGrease.Css.Extensions;

namespace WebGrease.Css.ImageAssemblyAnalysis.PropertyModel
{
  internal sealed class BackgroundRepeat
  {
    internal BackgroundRepeat()
    {
    }

    internal BackgroundRepeat(DeclarationNode declarationNode)
    {
      ExprNode exprNode = declarationNode != null ? declarationNode.ExprNode : throw new ArgumentNullException(nameof (declarationNode));
      this.ParseTerm(exprNode.TermNode);
      exprNode.TermsWithOperators.ForEach<TermWithOperatorNode>(new Action<TermWithOperatorNode>(this.ParseTermWithOperator));
    }

    internal Repeat? RepeatValue { get; private set; }

    internal bool VerifyBackgroundNoRepeat()
    {
      Repeat? repeatValue = this.RepeatValue;
      return (repeatValue.GetValueOrDefault() != Repeat.NoRepeat ? 1 : (!repeatValue.HasValue ? 1 : 0)) == 0;
    }

    internal void ParseTerm(TermNode termNode)
    {
      if (string.IsNullOrWhiteSpace(termNode.StringBasedValue))
        return;
      switch (termNode.StringBasedValue)
      {
        case "repeat":
          this.RepeatValue = new Repeat?(Repeat.Repeat);
          break;
        case "no-repeat":
          this.RepeatValue = new Repeat?(Repeat.NoRepeat);
          break;
        case "repeat-x":
          this.RepeatValue = new Repeat?(Repeat.RepeatX);
          break;
        case "repeat-y":
          this.RepeatValue = new Repeat?(Repeat.RepeatY);
          break;
      }
    }

    internal void ParseTermWithOperator(TermWithOperatorNode termWithOperatorNode) => this.ParseTerm(termWithOperatorNode.TermNode);
  }
}
