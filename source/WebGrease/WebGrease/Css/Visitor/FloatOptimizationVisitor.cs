// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Visitor.FloatOptimizationVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Text.RegularExpressions;
using WebGrease.Css.Ast;
using WebGrease.Css.Extensions;

namespace WebGrease.Css.Visitor
{
  public sealed class FloatOptimizationVisitor : NodeTransformVisitor
  {
    private static readonly Regex NumberBasedValue = new Regex("^(([0-9]+)([\\.]?[0-9]*))([a-z%]*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex LengthUnits = new Regex("^(cm|mm|in|px|pt|pc|em|ex|ch|rem|vw|vh|vmin|vmax|fr|gr)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public override AstNode VisitTermNode(TermNode termNode)
    {
      FunctionNode functionNode = termNode != null ? termNode.FunctionNode : throw new ArgumentNullException(nameof (termNode));
      string numberBasedValue = termNode.NumberBasedValue;
      if (!string.IsNullOrWhiteSpace(numberBasedValue))
      {
        Match match = FloatOptimizationVisitor.NumberBasedValue.Match(numberBasedValue);
        if (match.Success)
        {
          float num = match.Result("$1").ParseFloat();
          string input = match.Result("$4");
          if ((double) num == 0.0)
            return string.IsNullOrEmpty(input) || input == "%" || FloatOptimizationVisitor.LengthUnits.IsMatch(input) ? (AstNode) new TermNode(termNode.UnaryOperator, "0", termNode.StringBasedValue, termNode.Hexcolor, termNode.FunctionNode, termNode.ImportantComments) : (AstNode) new TermNode(termNode.UnaryOperator, "0" + input, termNode.StringBasedValue, termNode.Hexcolor, termNode.FunctionNode, termNode.ImportantComments);
          string str1 = match.Result("$2").TrimStart("0".ToCharArray());
          string str2 = match.Result("$3").TrimEnd("0".ToCharArray());
          if (str2 == '.'.ToString())
            str2 = string.Empty;
          return (AstNode) new TermNode(termNode.UnaryOperator, str1 + str2 + input, termNode.StringBasedValue, termNode.Hexcolor, termNode.FunctionNode, termNode.ImportantComments);
        }
      }
      else if (functionNode != null)
        functionNode = (FunctionNode) functionNode.Accept((NodeVisitor) this);
      return (AstNode) new TermNode(termNode.UnaryOperator, numberBasedValue, termNode.StringBasedValue, termNode.Hexcolor, functionNode, termNode.ImportantComments);
    }
  }
}
