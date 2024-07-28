// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Visitor.ColorOptimizationVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using WebGrease.Css.Ast;
using WebGrease.Css.Extensions;

namespace WebGrease.Css.Visitor
{
  public sealed class ColorOptimizationVisitor : NodeTransformVisitor
  {
    private static readonly Regex NumberBasedValue = new Regex("^(([0-9]*)(\\.[0-9]+)?)([a-z%]*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex ColorGroupCapture = new Regex("^\\#(?<r>[0-9a-f])\\k<r>(?<g>[0-9a-f])\\k<g>(?<b>[0-9a-f])\\k<b>$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public override AstNode VisitTermNode(TermNode termNode)
    {
      string str = termNode != null ? termNode.Hexcolor : throw new ArgumentNullException(nameof (termNode));
      FunctionNode functionNode = termNode.FunctionNode;
      int red;
      int green;
      int blue;
      if (functionNode != null && string.Compare(functionNode.FunctionName, "rgb", StringComparison.OrdinalIgnoreCase) == 0 && ColorOptimizationVisitor.TryGetRgb(functionNode.ExprNode, out red, out green, out blue))
      {
        functionNode = (FunctionNode) null;
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "#{0:x2}{1:x2}{2:x2}", new object[3]
        {
          (object) red,
          (object) green,
          (object) blue
        });
      }
      if (!string.IsNullOrWhiteSpace(str))
      {
        Match match = ColorOptimizationVisitor.ColorGroupCapture.Match(str);
        if (match.Success)
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "#{0}{1}{2}", new object[3]
          {
            (object) match.Result("${r}"),
            (object) match.Result("${g}"),
            (object) match.Result("${b}")
          });
        str = str.ToLowerInvariant();
      }
      return (AstNode) new TermNode(termNode.UnaryOperator, termNode.NumberBasedValue, termNode.StringBasedValue, str, functionNode, termNode.ImportantComments);
    }

    private static bool TryGetRgb(ExprNode exprNode, out int red, out int green, out int blue)
    {
      red = green = blue = 0;
      return ColorOptimizationVisitor.IsThreeNumberArguments(exprNode) && ColorOptimizationVisitor.TryGetColorFragment(exprNode.TermNode, out red) && ColorOptimizationVisitor.TryGetColorFragment(exprNode.TermsWithOperators[0].TermNode, out green) && ColorOptimizationVisitor.TryGetColorFragment(exprNode.TermsWithOperators[1].TermNode, out blue);
    }

    private static bool IsThreeNumberArguments(ExprNode exprNode) => exprNode != null && ColorOptimizationVisitor.IsNumberTerm(exprNode.TermNode) && exprNode.TermsWithOperators != null && exprNode.TermsWithOperators.Count == 2 && ColorOptimizationVisitor.IsCommaNumber(exprNode.TermsWithOperators[0]) && ColorOptimizationVisitor.IsCommaNumber(exprNode.TermsWithOperators[1]);

    private static bool IsNumberTerm(TermNode termNode) => termNode != null && !string.IsNullOrWhiteSpace(termNode.NumberBasedValue);

    private static bool IsCommaNumber(TermWithOperatorNode termWithOperatorNode) => termWithOperatorNode != null && termWithOperatorNode.Operator == "," && ColorOptimizationVisitor.IsNumberTerm(termWithOperatorNode.TermNode);

    private static bool TryGetColorFragment(TermNode termNode, out int fragment)
    {
      bool colorFragment = false;
      fragment = 0;
      Match match = ColorOptimizationVisitor.NumberBasedValue.Match(termNode.NumberBasedValue);
      if (match != null)
      {
        string strA = match.Result("$4");
        if (string.IsNullOrWhiteSpace(strA))
        {
          if (string.IsNullOrWhiteSpace(match.Result("$3")))
            colorFragment = int.TryParse(match.Result("$2"), out fragment) && 0 <= fragment && fragment <= (int) byte.MaxValue;
        }
        else if (string.CompareOrdinal(strA, "%") == 0)
        {
          fragment = (int) Math.Round((double) match.Result("$1").ParseFloat() / 100.0 * (double) byte.MaxValue, 0);
          colorFragment = 0 <= fragment && fragment <= (int) byte.MaxValue;
        }
      }
      return colorFragment;
    }
  }
}
