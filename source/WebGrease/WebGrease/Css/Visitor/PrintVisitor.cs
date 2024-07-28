// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Visitor.PrintVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using WebGrease.Css.Ast;
using WebGrease.Css.Ast.Animation;
using WebGrease.Css.Ast.MediaQuery;
using WebGrease.Css.Ast.Selectors;
using WebGrease.Css.Extensions;

namespace WebGrease.Css.Visitor
{
  public class PrintVisitor : NodeVisitor
  {
    private readonly PrinterFormatter _printerFormatter = new PrinterFormatter();

    private PrintVisitor()
    {
      PrintVisitor.IndentSize = 2;
      PrintVisitor.IndentCharacter = ' ';
    }

    public static char IndentCharacter { get; set; }

    public static int IndentSize { get; set; }

    public static string Print(AstNode node, bool prettyPrint) => new PrintVisitor().Print(prettyPrint, node);

    public override AstNode VisitStyleSheetNode(StyleSheetNode styleSheet)
    {
      if (styleSheet == null)
        return (AstNode) null;
      if (!string.IsNullOrWhiteSpace(styleSheet.CharSetString))
      {
        this._printerFormatter.Append("@charset ");
        this._printerFormatter.Append(styleSheet.CharSetString);
        this._printerFormatter.AppendLine(';');
      }
      styleSheet.Imports.ForEach<ImportNode>((Action<ImportNode>) (importNode => importNode.Accept((NodeVisitor) this)));
      styleSheet.Namespaces.ForEach<NamespaceNode>((Action<NamespaceNode>) (namespaceNode => namespaceNode.Accept((NodeVisitor) this)));
      styleSheet.StyleSheetRules.ForEach<StyleSheetRuleNode>((Action<StyleSheetRuleNode>) (styleSheetRuleNode => styleSheetRuleNode.Accept((NodeVisitor) this)));
      return (AstNode) styleSheet;
    }

    public override AstNode VisitImportNode(ImportNode importNode)
    {
      this._printerFormatter.Append("@import ");
      switch (importNode.AllowedImportDataType)
      {
        case AllowedImportData.String:
        case AllowedImportData.Uri:
          this._printerFormatter.Append(importNode.ImportDataValue);
          break;
      }
      if (importNode.MediaQueries.Count > 0)
      {
        this._printerFormatter.Append(' ');
        importNode.MediaQueries.ForEach<MediaQueryNode>((Action<MediaQueryNode, bool>) ((mediaQuery, last) =>
        {
          mediaQuery.Accept((NodeVisitor) this);
          if (last)
            return;
          this._printerFormatter.Append(',');
        }));
      }
      this._printerFormatter.AppendLine(';');
      return (AstNode) importNode;
    }

    public override AstNode VisitNamespaceNode(NamespaceNode namespaceNode)
    {
      this._printerFormatter.Append("@namespace");
      if (!string.IsNullOrWhiteSpace(namespaceNode.Prefix))
      {
        this._printerFormatter.Append(' ');
        this._printerFormatter.Append(namespaceNode.Prefix);
      }
      this._printerFormatter.Append(' ');
      this._printerFormatter.Append(namespaceNode.Value);
      this._printerFormatter.AppendLine(';');
      return (AstNode) namespaceNode;
    }

    public override AstNode VisitRulesetNode(RulesetNode rulesetNode)
    {
      rulesetNode.SelectorsGroupNode.Accept((NodeVisitor) this);
      this._printerFormatter.WriteIndent();
      this._printerFormatter.AppendLine('{');
      this._printerFormatter.IncrementIndentLevel();
      rulesetNode.Declarations.ForEach<DeclarationNode>((Action<DeclarationNode, bool>) ((declaration, last) =>
      {
        AstNode astNode = declaration.Accept((NodeVisitor) this);
        if (last || astNode == null)
          return;
        this._printerFormatter.AppendLine(';');
      }));
      rulesetNode.ImportantComments.ForEach<ImportantCommentNode>((Action<ImportantCommentNode>) (comment => comment.Accept((NodeVisitor) this)));
      this._printerFormatter.DecrementIndentLevel();
      this._printerFormatter.AppendLine();
      this._printerFormatter.WriteIndent();
      this._printerFormatter.AppendLine('}');
      return (AstNode) rulesetNode;
    }

    public override AstNode VisitSelectorsGroupNode(SelectorsGroupNode selectorsGroupNode)
    {
      selectorsGroupNode.SelectorNodes.ForEach<SelectorNode>((Action<SelectorNode, bool>) ((selector, last) =>
      {
        selector.Accept((NodeVisitor) this);
        if (last)
          return;
        this._printerFormatter.AppendLine(',');
      }));
      this._printerFormatter.AppendLine();
      return (AstNode) selectorsGroupNode;
    }

    public override AstNode VisitSelectorNode(SelectorNode selectorNode)
    {
      this._printerFormatter.WriteIndent();
      selectorNode.SimpleSelectorSequenceNode.Accept((NodeVisitor) this);
      selectorNode.CombinatorSimpleSelectorSequenceNodes.ForEach<CombinatorSimpleSelectorSequenceNode>((Action<CombinatorSimpleSelectorSequenceNode, bool>) ((combinatorSimpleSelectorSequenceNode, selectorIndex) =>
      {
        if (combinatorSimpleSelectorSequenceNode.Combinator == Combinator.SingleSpace && this._printerFormatter.ToString().EndsWith(' '.ToString((IFormatProvider) CultureInfo.InvariantCulture), StringComparison.Ordinal))
          this._printerFormatter.Remove(this._printerFormatter.Length() - 1, 1);
        combinatorSimpleSelectorSequenceNode.Accept((NodeVisitor) this);
      }));
      return (AstNode) selectorNode;
    }

    public override AstNode VisitSimpleSelectorSequenceNode(
      SimpleSelectorSequenceNode simpleSelectorSequenceNode)
    {
      if (simpleSelectorSequenceNode.TypeSelectorNode != null)
        simpleSelectorSequenceNode.TypeSelectorNode.Accept((NodeVisitor) this);
      if (simpleSelectorSequenceNode.UniversalSelectorNode != null)
        simpleSelectorSequenceNode.UniversalSelectorNode.Accept((NodeVisitor) this);
      if (simpleSelectorSequenceNode.HashClassAttribPseudoNegationNodes.Count > 0)
        this._printerFormatter.Append(simpleSelectorSequenceNode.Separator);
      simpleSelectorSequenceNode.HashClassAttribPseudoNegationNodes.ForEach<HashClassAtNameAttribPseudoNegationNode>((Action<HashClassAtNameAttribPseudoNegationNode>) (hashClassAttribPseudoNegationNode => hashClassAttribPseudoNegationNode.Accept((NodeVisitor) this)));
      return (AstNode) simpleSelectorSequenceNode;
    }

    public override AstNode VisitUniversalSelectorNode(UniversalSelectorNode universalSelectorNode)
    {
      if (universalSelectorNode.SelectorNamespacePrefixNode != null)
        universalSelectorNode.SelectorNamespacePrefixNode.Accept((NodeVisitor) this);
      this._printerFormatter.Append("*");
      return (AstNode) universalSelectorNode;
    }

    public override AstNode VisitTypeSelectorNode(TypeSelectorNode typeSelectorNode)
    {
      if (typeSelectorNode.SelectorNamespacePrefixNode != null)
        typeSelectorNode.SelectorNamespacePrefixNode.Accept((NodeVisitor) this);
      this._printerFormatter.Append(typeSelectorNode.ElementName);
      return (AstNode) typeSelectorNode;
    }

    public override AstNode VisitSelectorNamespacePrefixNode(
      SelectorNamespacePrefixNode selectorNamespacePrefixNode)
    {
      this._printerFormatter.Append(selectorNamespacePrefixNode.Prefix);
      this._printerFormatter.Append("|");
      return (AstNode) selectorNamespacePrefixNode;
    }

    public override AstNode VisitHashClassAtNameAttribPseudoNegationNode(
      HashClassAtNameAttribPseudoNegationNode hashClassAtNameAttribPseudoNegationNode)
    {
      if (!string.IsNullOrWhiteSpace(hashClassAtNameAttribPseudoNegationNode.Hash))
        this._printerFormatter.Append(hashClassAtNameAttribPseudoNegationNode.Hash);
      else if (!string.IsNullOrWhiteSpace(hashClassAtNameAttribPseudoNegationNode.CssClass))
        this._printerFormatter.Append(hashClassAtNameAttribPseudoNegationNode.CssClass);
      else if (!string.IsNullOrWhiteSpace(hashClassAtNameAttribPseudoNegationNode.AtName))
        this._printerFormatter.Append(hashClassAtNameAttribPseudoNegationNode.AtName);
      else if (!string.IsNullOrWhiteSpace(hashClassAtNameAttribPseudoNegationNode.ReplacementToken))
        this._printerFormatter.Append(hashClassAtNameAttribPseudoNegationNode.ReplacementToken);
      else if (hashClassAtNameAttribPseudoNegationNode.AttribNode != null)
        hashClassAtNameAttribPseudoNegationNode.AttribNode.Accept((NodeVisitor) this);
      else if (hashClassAtNameAttribPseudoNegationNode.PseudoNode != null)
        hashClassAtNameAttribPseudoNegationNode.PseudoNode.Accept((NodeVisitor) this);
      else if (hashClassAtNameAttribPseudoNegationNode.NegationNode != null)
        hashClassAtNameAttribPseudoNegationNode.NegationNode.Accept((NodeVisitor) this);
      return (AstNode) hashClassAtNameAttribPseudoNegationNode;
    }

    public override AstNode VisitAttribNode(AttribNode attrib)
    {
      this._printerFormatter.Append('[');
      if (attrib.SelectorNamespacePrefixNode != null)
        attrib.SelectorNamespacePrefixNode.Accept((NodeVisitor) this);
      this._printerFormatter.Append(attrib.Ident);
      if (attrib.OperatorAndValueNode != null)
        attrib.OperatorAndValueNode.Accept((NodeVisitor) this);
      this._printerFormatter.Append(']');
      return (AstNode) attrib;
    }

    public override AstNode VisitAttribOperatorAndValueNode(
      AttribOperatorAndValueNode attribOperatorAndValueNode)
    {
      if (string.IsNullOrWhiteSpace(attribOperatorAndValueNode.IdentOrString))
        return (AstNode) attribOperatorAndValueNode;
      switch (attribOperatorAndValueNode.AttribOperatorKind)
      {
        case AttribOperatorKind.Prefix:
          this._printerFormatter.Append("^=");
          break;
        case AttribOperatorKind.Suffix:
          this._printerFormatter.Append("$=");
          break;
        case AttribOperatorKind.Substring:
          this._printerFormatter.Append("*=");
          break;
        case AttribOperatorKind.Equal:
          this._printerFormatter.Append("=");
          break;
        case AttribOperatorKind.Includes:
          this._printerFormatter.Append("~=");
          break;
        case AttribOperatorKind.DashMatch:
          this._printerFormatter.Append("|=");
          break;
      }
      this._printerFormatter.Append(attribOperatorAndValueNode.IdentOrString);
      return (AstNode) attribOperatorAndValueNode;
    }

    public override AstNode VisitPseudoNode(PseudoNode pseudoNode)
    {
      for (int index = 0; index < pseudoNode.NumberOfColons; ++index)
        this._printerFormatter.Append(':');
      if (pseudoNode.FunctionalPseudoNode != null)
        pseudoNode.FunctionalPseudoNode.Accept((NodeVisitor) this);
      else if (!string.IsNullOrWhiteSpace(pseudoNode.Ident))
      {
        this._printerFormatter.Append(pseudoNode.Ident);
        if (pseudoNode.Ident == "first-letter" || pseudoNode.Ident == "first-line")
          this._printerFormatter.Append(' ');
      }
      return (AstNode) pseudoNode;
    }

    public override AstNode VisitNegationNode(NegationNode negationNode)
    {
      this._printerFormatter.Append(':');
      this._printerFormatter.Append("not");
      this._printerFormatter.Append('(');
      negationNode.NegationArgNode.Accept((NodeVisitor) this);
      this._printerFormatter.Append(')');
      return (AstNode) negationNode;
    }

    public override AstNode VisitNegationArgNode(NegationArgNode negationArgNode)
    {
      if (negationArgNode.TypeSelectorNode != null)
        negationArgNode.TypeSelectorNode.Accept((NodeVisitor) this);
      else if (negationArgNode.UniversalSelectorNode != null)
        negationArgNode.UniversalSelectorNode.Accept((NodeVisitor) this);
      else if (!string.IsNullOrWhiteSpace(negationArgNode.Hash))
        this._printerFormatter.Append(negationArgNode.Hash);
      else if (!string.IsNullOrWhiteSpace(negationArgNode.CssClass))
        this._printerFormatter.Append(negationArgNode.CssClass);
      else if (negationArgNode.AttribNode != null)
        negationArgNode.AttribNode.Accept((NodeVisitor) this);
      else if (negationArgNode.PseudoNode != null)
        negationArgNode.PseudoNode.Accept((NodeVisitor) this);
      return (AstNode) negationArgNode;
    }

    public override AstNode VisitDeclarationNode(DeclarationNode declarationNode)
    {
      bool flag1 = declarationNode.Property.StartsWith("/", StringComparison.OrdinalIgnoreCase);
      bool flag2 = declarationNode.Property.StartsWith("-wg-", StringComparison.OrdinalIgnoreCase);
      if (!this._printerFormatter.PrettyPrint && (flag1 || flag2))
        return (AstNode) null;
      foreach (AstNode importantComment in declarationNode.ImportantComments)
        importantComment.Accept((NodeVisitor) this);
      this._printerFormatter.WriteIndent();
      this._printerFormatter.Append(declarationNode.Property);
      this._printerFormatter.Append(':');
      declarationNode.ExprNode.Accept((NodeVisitor) this);
      if (flag1)
      {
        this._printerFormatter.AppendLine();
        return (AstNode) null;
      }
      this._printerFormatter.Append(declarationNode.Prio);
      return (AstNode) declarationNode;
    }

    public override AstNode VisitExprNode(ExprNode exprNode)
    {
      foreach (AstNode importantComment in exprNode.ImportantComments)
        importantComment.Accept((NodeVisitor) this);
      exprNode.TermNode.Accept((NodeVisitor) this);
      exprNode.TermsWithOperators.ForEach<TermWithOperatorNode>((Action<TermWithOperatorNode>) (termWithOperator => termWithOperator.Accept((NodeVisitor) this)));
      return (AstNode) exprNode;
    }

    public override AstNode VisitTermNode(TermNode termNode)
    {
      this._printerFormatter.Append(termNode.UnaryOperator);
      if (termNode.IsBinary && FunctionNode.IsBinaryOperator(termNode.UnaryOperator))
        this._printerFormatter.Append(" ");
      if (!string.IsNullOrWhiteSpace(termNode.NumberBasedValue))
        this._printerFormatter.Append(termNode.NumberBasedValue);
      else if (!string.IsNullOrWhiteSpace(termNode.StringBasedValue))
        this._printerFormatter.Append(termNode.StringBasedValue);
      else if (!string.IsNullOrWhiteSpace(termNode.ReplacementTokenBasedValue))
        this._printerFormatter.Append(termNode.ReplacementTokenBasedValue);
      else if (!string.IsNullOrWhiteSpace(termNode.Hexcolor))
        this._printerFormatter.Append(termNode.Hexcolor);
      else if (termNode.FunctionNode != null)
        termNode.FunctionNode.Accept((NodeVisitor) this);
      foreach (AstNode importantComment in termNode.ImportantComments)
        importantComment.Accept((NodeVisitor) this);
      return (AstNode) termNode;
    }

    public override AstNode VisitImportantCommentNode(ImportantCommentNode commentNode)
    {
      this._printerFormatter.Append(commentNode.Text);
      return base.VisitImportantCommentNode(commentNode);
    }

    public override AstNode VisitTermWithOperatorNode(TermWithOperatorNode termWithOperatorNode)
    {
      this._printerFormatter.Append(termWithOperatorNode.Operator);
      termWithOperatorNode.TermNode.Accept((NodeVisitor) this);
      return (AstNode) termWithOperatorNode;
    }

    public override AstNode VisitMediaNode(MediaNode mediaNode)
    {
      this._printerFormatter.Append("@media ");
      mediaNode.MediaQueries.ForEach<MediaQueryNode>((Action<MediaQueryNode, bool>) ((mediaQuery, last) =>
      {
        mediaQuery.Accept((NodeVisitor) this);
        if (last)
          return;
        this._printerFormatter.Append(',');
      }));
      this._printerFormatter.AppendLine();
      this._printerFormatter.AppendLine('{');
      this._printerFormatter.IncrementIndentLevel();
      foreach (AstNode ruleset in mediaNode.Rulesets)
        ruleset.Accept((NodeVisitor) this);
      foreach (PageNode pageNode in mediaNode.PageNodes)
      {
        this._printerFormatter.WriteIndent();
        pageNode.Accept((NodeVisitor) this);
      }
      this._printerFormatter.DecrementIndentLevel();
      this._printerFormatter.AppendLine('}');
      return (AstNode) mediaNode;
    }

    public override AstNode VisitPageNode(PageNode pageNode)
    {
      this._printerFormatter.Append("@page");
      if (!string.IsNullOrWhiteSpace(pageNode.PseudoPage))
      {
        if (!pageNode.PseudoPage.StartsWith(':'.ToString((IFormatProvider) CultureInfo.InvariantCulture), StringComparison.Ordinal))
          this._printerFormatter.Append(' ');
        this._printerFormatter.Append(pageNode.PseudoPage);
      }
      this._printerFormatter.AppendLine();
      this._printerFormatter.WriteIndent();
      this._printerFormatter.AppendLine('{');
      this._printerFormatter.IncrementIndentLevel();
      pageNode.Declarations.ForEach<DeclarationNode>((Action<DeclarationNode, bool>) ((declaration, last) =>
      {
        AstNode astNode = declaration.Accept((NodeVisitor) this);
        if (last || astNode == null)
          return;
        this._printerFormatter.AppendLine(';');
      }));
      this._printerFormatter.AppendLine();
      this._printerFormatter.DecrementIndentLevel();
      this._printerFormatter.WriteIndent();
      this._printerFormatter.AppendLine('}');
      return (AstNode) pageNode;
    }

    public override AstNode VisitDocumentQueryNode(DocumentQueryNode documentQueryNode)
    {
      this._printerFormatter.Append(documentQueryNode.DocumentSymbol);
      this._printerFormatter.Append(' ');
      this._printerFormatter.Append(documentQueryNode.MatchFunctionName);
      this._printerFormatter.AppendLine();
      this._printerFormatter.AppendLine('{');
      this._printerFormatter.IncrementIndentLevel();
      foreach (AstNode ruleset in documentQueryNode.Rulesets)
        ruleset.Accept((NodeVisitor) this);
      this._printerFormatter.DecrementIndentLevel();
      this._printerFormatter.AppendLine('}');
      return (AstNode) documentQueryNode;
    }

    public override AstNode VisitCombinatorSimpleSelectorSequenceNode(
      CombinatorSimpleSelectorSequenceNode combinatorSimpleSelectorSequenceNode)
    {
      switch (combinatorSimpleSelectorSequenceNode.Combinator)
      {
        case Combinator.PlusSign:
          this._printerFormatter.Append("+");
          break;
        case Combinator.GreaterThanSign:
          this._printerFormatter.Append(">");
          break;
        case Combinator.Tilde:
          this._printerFormatter.Append("~");
          break;
        case Combinator.SingleSpace:
          this._printerFormatter.Append(' ');
          break;
      }
      combinatorSimpleSelectorSequenceNode.SimpleSelectorSequenceNode.Accept((NodeVisitor) this);
      return (AstNode) combinatorSimpleSelectorSequenceNode;
    }

    public override AstNode VisitFunctionNode(FunctionNode functionNode)
    {
      if (functionNode.FunctionName == "rgb")
      {
        string content = functionNode.ExprNode.MinifyPrint();
        if (content.StartsWith('#'.ToString((IFormatProvider) CultureInfo.InvariantCulture), StringComparison.Ordinal))
          this._printerFormatter.Append(content);
      }
      this._printerFormatter.Append(functionNode.FunctionName);
      this._printerFormatter.Append('(');
      if (functionNode.ExprNode != null)
        functionNode.ExprNode.Accept((NodeVisitor) this);
      this._printerFormatter.Append(')');
      return (AstNode) functionNode;
    }

    public override AstNode VisitFunctionalPseudoNode(FunctionalPseudoNode functionalPseudoNode)
    {
      this._printerFormatter.Append(functionalPseudoNode.FunctionName);
      this._printerFormatter.Append('(');
      functionalPseudoNode.SelectorExpressionNode.Accept((NodeVisitor) this);
      this._printerFormatter.Append(')');
      return (AstNode) functionalPseudoNode;
    }

    public override AstNode VisitSelectorExpressionNode(
      SelectorExpressionNode selectorExpressionNode)
    {
      foreach (string selectorExpression in selectorExpressionNode.SelectorExpressions)
        this._printerFormatter.Append(selectorExpression);
      return (AstNode) selectorExpressionNode;
    }

    public override AstNode VisitMediaQueryNode(MediaQueryNode mediaQueryNode)
    {
      if (!string.IsNullOrWhiteSpace(mediaQueryNode.OnlyText))
      {
        this._printerFormatter.Append(mediaQueryNode.OnlyText);
        this._printerFormatter.Append(' ');
      }
      else if (!string.IsNullOrWhiteSpace(mediaQueryNode.NotText))
      {
        this._printerFormatter.Append(mediaQueryNode.NotText);
        this._printerFormatter.Append(' ');
      }
      if (!string.IsNullOrWhiteSpace(mediaQueryNode.MediaType))
      {
        this._printerFormatter.Append(mediaQueryNode.MediaType);
        if (mediaQueryNode.MediaExpressions.Count > 0)
          mediaQueryNode.MediaExpressions.ForEach<MediaExpressionNode>((Action<MediaExpressionNode>) (mediaExpression =>
          {
            this._printerFormatter.Append(' ');
            this._printerFormatter.Append("and");
            this._printerFormatter.Append(' ');
            mediaExpression.Accept((NodeVisitor) this);
          }));
      }
      else
        mediaQueryNode.MediaExpressions.ForEach<MediaExpressionNode>((Action<MediaExpressionNode, bool>) ((mediaExpression, last) =>
        {
          mediaExpression.Accept((NodeVisitor) this);
          if (last)
            return;
          this._printerFormatter.Append(' ');
          this._printerFormatter.Append("and");
          this._printerFormatter.Append(' ');
        }));
      return (AstNode) mediaQueryNode;
    }

    public override AstNode VisitMediaExpressionNode(MediaExpressionNode mediaExpressionNode)
    {
      this._printerFormatter.Append('(');
      this._printerFormatter.Append(mediaExpressionNode.MediaFeature);
      if (mediaExpressionNode.ExprNode != null)
      {
        this._printerFormatter.Append(':');
        mediaExpressionNode.ExprNode.Accept((NodeVisitor) this);
      }
      this._printerFormatter.Append(')');
      return (AstNode) mediaExpressionNode;
    }

    public override AstNode VisitKeyFramesNode(KeyFramesNode keyFramesNode)
    {
      this._printerFormatter.Append(keyFramesNode.KeyFramesSymbol);
      this._printerFormatter.Append(' ');
      if (!string.IsNullOrWhiteSpace(keyFramesNode.IdentValue))
        this._printerFormatter.Append(keyFramesNode.IdentValue);
      else if (!string.IsNullOrWhiteSpace(keyFramesNode.StringValue))
        this._printerFormatter.Append(keyFramesNode.StringValue);
      this._printerFormatter.AppendLine();
      this._printerFormatter.WriteIndent();
      this._printerFormatter.Append('{');
      keyFramesNode.KeyFramesBlockNodes.ForEach<KeyFramesBlockNode>((Action<KeyFramesBlockNode>) (keyFramesBlockNode => keyFramesBlockNode.Accept((NodeVisitor) this)));
      this._printerFormatter.AppendLine();
      this._printerFormatter.WriteIndent();
      this._printerFormatter.AppendLine('}');
      return (AstNode) keyFramesNode;
    }

    public override AstNode VisitKeyFramesBlockNode(KeyFramesBlockNode keyFramesBlockNode)
    {
      this._printerFormatter.AppendLine();
      this._printerFormatter.IncrementIndentLevel();
      this._printerFormatter.WriteIndent();
      this._printerFormatter.Append(string.Join(','.ToString(), (IEnumerable<string>) keyFramesBlockNode.KeyFramesSelectors));
      this._printerFormatter.AppendLine();
      this._printerFormatter.WriteIndent();
      this._printerFormatter.Append('{');
      this._printerFormatter.AppendLine();
      this._printerFormatter.IncrementIndentLevel();
      keyFramesBlockNode.DeclarationNodes.ForEach<DeclarationNode>((Action<DeclarationNode, bool>) ((declarationNode, last) =>
      {
        AstNode astNode = declarationNode.Accept((NodeVisitor) this);
        if (last || astNode == null)
          return;
        this._printerFormatter.AppendLine(';');
      }));
      this._printerFormatter.AppendLine();
      this._printerFormatter.DecrementIndentLevel();
      this._printerFormatter.WriteIndent();
      this._printerFormatter.Append('}');
      this._printerFormatter.DecrementIndentLevel();
      return (AstNode) keyFramesBlockNode;
    }

    internal string Print(bool prettyPrint, AstNode node)
    {
      this._printerFormatter.PrettyPrint = prettyPrint;
      this._printerFormatter.IndentCharacter = PrintVisitor.IndentCharacter;
      this._printerFormatter.IndentSize = PrintVisitor.IndentSize;
      node?.Accept((NodeVisitor) this);
      return this._printerFormatter.ToString();
    }
  }
}
