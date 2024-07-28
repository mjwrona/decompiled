// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Visitor.ResourceResolutionVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using WebGrease.Activities;
using WebGrease.Css.Ast;
using WebGrease.Css.Ast.MediaQuery;
using WebGrease.Css.Ast.Selectors;

namespace WebGrease.Css.Visitor
{
  public class ResourceResolutionVisitor : NodeTransformVisitor
  {
    private readonly IEnumerable<IDictionary<string, string>> resources;
    private static char[] numberChars = new char[10]
    {
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9'
    };
    private static char[] hexChars = new char[22]
    {
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      'a',
      'b',
      'c',
      'd',
      'e',
      'f',
      'A',
      'B',
      'C',
      'D',
      'E',
      'F'
    };

    public ResourceResolutionVisitor(IEnumerable<IDictionary<string, string>> resources)
    {
      if (resources == null)
        throw new ArgumentNullException(nameof (resources));
      this.resources = resources.Any<IDictionary<string, string>>() ? resources : throw new ArgumentException("The resources should have at least 1 item.");
    }

    public override AstNode VisitHashClassAtNameAttribPseudoNegationNode(
      HashClassAtNameAttribPseudoNegationNode hashClassAtNameAttribPseudoNegationNode)
    {
      if (string.IsNullOrWhiteSpace(hashClassAtNameAttribPseudoNegationNode.ReplacementToken))
        return base.VisitHashClassAtNameAttribPseudoNegationNode(hashClassAtNameAttribPseudoNegationNode);
      string str = ResourceResolutionVisitor.ReplaceTokens(hashClassAtNameAttribPseudoNegationNode.ReplacementToken, this.resources);
      if (str.StartsWith("#", StringComparison.OrdinalIgnoreCase))
        return (AstNode) new HashClassAtNameAttribPseudoNegationNode(str, (string) null, (string) null, (string) null, (AttribNode) null, (PseudoNode) null, (NegationNode) null);
      if (str.StartsWith(".", StringComparison.OrdinalIgnoreCase))
        return (AstNode) new HashClassAtNameAttribPseudoNegationNode((string) null, str, (string) null, (string) null, (AttribNode) null, (PseudoNode) null, (NegationNode) null);
      return str.StartsWith(".", StringComparison.OrdinalIgnoreCase) ? (AstNode) new HashClassAtNameAttribPseudoNegationNode((string) null, str, (string) null, (string) null, (AttribNode) null, (PseudoNode) null, (NegationNode) null) : (AstNode) new HashClassAtNameAttribPseudoNegationNode((string) null, (string) null, str, (string) null, (AttribNode) null, (PseudoNode) null, (NegationNode) null);
    }

    public override AstNode VisitTermNode(TermNode termNode)
    {
      if (!string.IsNullOrWhiteSpace(termNode.ReplacementTokenBasedValue))
      {
        string newValue = ResourceResolutionVisitor.ReplaceTokens(termNode.ReplacementTokenBasedValue, this.resources);
        return ResourceResolutionVisitor.CreateTermNode(termNode, newValue);
      }
      if (!ResourceResolutionVisitor.HasTokens(termNode.StringBasedValue))
        return base.VisitTermNode(termNode);
      string newValue1 = ResourceResolutionVisitor.ReplaceTokens(termNode.StringBasedValue, this.resources);
      return ResourceResolutionVisitor.CreateTermNode(termNode, newValue1);
    }

    private static AstNode CreateTermNode(TermNode termNode, string newValue)
    {
      newValue = newValue.Trim();
      if (ResourceResolutionVisitor.IsNumberBasedValue(newValue))
        return (AstNode) new TermNode(termNode.UnaryOperator, newValue, (string) null, (string) null, (FunctionNode) null, (ReadOnlyCollection<ImportantCommentNode>) null);
      return ResourceResolutionVisitor.IsHexColor(newValue) ? (AstNode) new TermNode(termNode.UnaryOperator, (string) null, (string) null, newValue, (FunctionNode) null, (ReadOnlyCollection<ImportantCommentNode>) null) : (AstNode) new TermNode(termNode.UnaryOperator, (string) null, newValue, (string) null, (FunctionNode) null, (ReadOnlyCollection<ImportantCommentNode>) null);
    }

    private static bool IsNumberBasedValue(string newValue)
    {
      newValue = newValue.TrimStart('-');
      return newValue != null && newValue.Length > 0 && ResourceResolutionVisitor.IsNumber(newValue[0]);
    }

    private static bool IsNumber(char c) => ((IEnumerable<char>) ResourceResolutionVisitor.numberChars).Contains<char>(c);

    private static bool IsHexColor(string newValue) => newValue != null && newValue.Length > 3 && newValue[0] == '#' && ResourceResolutionVisitor.IsHexColorValue(newValue.Substring(1));

    private static bool IsHexColorValue(string value) => value.All<char>(new Func<char, bool>(((Enumerable) ResourceResolutionVisitor.hexChars).Contains<char>));

    public override AstNode VisitDeclarationNode(DeclarationNode declarationNode) => ResourceResolutionVisitor.HasTokens(declarationNode.Property) ? (AstNode) new DeclarationNode(ResourceResolutionVisitor.ReplaceTokens(declarationNode.Property, this.resources), declarationNode.ExprNode.Accept((NodeVisitor) this) as ExprNode, declarationNode.Prio, declarationNode.ImportantComments) : base.VisitDeclarationNode(declarationNode);

    public override AstNode VisitMediaExpressionNode(MediaExpressionNode mediaExpressionNode) => ResourceResolutionVisitor.HasTokens(mediaExpressionNode.MediaFeature) ? (AstNode) new MediaExpressionNode(ResourceResolutionVisitor.ReplaceTokens(mediaExpressionNode.MediaFeature, this.resources), mediaExpressionNode.ExprNode.Accept((NodeVisitor) this) as ExprNode) : base.VisitMediaExpressionNode(mediaExpressionNode);

    private static bool HasTokens(string stringBasedValue) => !string.IsNullOrWhiteSpace(stringBasedValue) && stringBasedValue.Contains("%");

    private static string ReplaceTokens(
      string value,
      IEnumerable<IDictionary<string, string>> resources)
    {
      return ResourcesResolver.LocalizationResourceKeyRegex.Replace(value, (MatchEvaluator) (match =>
      {
        string key = match.Result("$1");
        foreach (IDictionary<string, string> resource in resources)
        {
          string str;
          if (resource.TryGetValue(key, out str))
          {
            if (str.Contains("%"))
              str = ResourceResolutionVisitor.ReplaceTokens(str, resources);
            return str;
          }
        }
        return match.Value;
      }));
    }
  }
}
