// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Visitor.NodeTransformVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Linq;
using WebGrease.Css.Ast;
using WebGrease.Css.Ast.Animation;
using WebGrease.Css.Ast.MediaQuery;
using WebGrease.Css.Ast.Selectors;
using WebGrease.Css.Extensions;
using WebGrease.Extensions;

namespace WebGrease.Css.Visitor
{
  public class NodeTransformVisitor : NodeVisitor
  {
    public override AstNode VisitStyleSheetNode(StyleSheetNode styleSheet)
    {
      if (styleSheet == null)
        throw new ArgumentNullException(nameof (styleSheet));
      return (AstNode) new StyleSheetNode(styleSheet.CharSetString, styleSheet.Imports, styleSheet.Namespaces, styleSheet.StyleSheetRules.Select<StyleSheetRuleNode, StyleSheetRuleNode>((Func<StyleSheetRuleNode, StyleSheetRuleNode>) (styleSheetRule => (StyleSheetRuleNode) styleSheetRule.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<StyleSheetRuleNode>());
    }

    public override AstNode VisitImportNode(ImportNode importNode) => (AstNode) new ImportNode(importNode.AllowedImportDataType, importNode.ImportDataValue, importNode.MediaQueries.Select<MediaQueryNode, MediaQueryNode>((Func<MediaQueryNode, MediaQueryNode>) (mediaQueryNode => (MediaQueryNode) mediaQueryNode.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<MediaQueryNode>());

    public override AstNode VisitRulesetNode(RulesetNode rulesetNode) => (AstNode) new RulesetNode(rulesetNode.SelectorsGroupNode.Accept((NodeVisitor) this) as SelectorsGroupNode, rulesetNode.Declarations.Select<DeclarationNode, DeclarationNode>((Func<DeclarationNode, DeclarationNode>) (declarationNode => (DeclarationNode) declarationNode.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<DeclarationNode>(), rulesetNode.ImportantComments);

    public override AstNode VisitMediaNode(MediaNode mediaNode) => (AstNode) new MediaNode(mediaNode.MediaQueries.Select<MediaQueryNode, MediaQueryNode>((Func<MediaQueryNode, MediaQueryNode>) (ruleset => (MediaQueryNode) ruleset.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<MediaQueryNode>(), mediaNode.Rulesets.Select<RulesetNode, RulesetNode>((Func<RulesetNode, RulesetNode>) (ruleset => (RulesetNode) ruleset.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<RulesetNode>(), mediaNode.PageNodes.Select<PageNode, PageNode>((Func<PageNode, PageNode>) (pages => (PageNode) pages.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<PageNode>());

    public override AstNode VisitPageNode(PageNode pageNode) => (AstNode) new PageNode(pageNode.PseudoPage, pageNode.Declarations.Select<DeclarationNode, DeclarationNode>((Func<DeclarationNode, DeclarationNode>) (declaration => (DeclarationNode) declaration.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<DeclarationNode>());

    public override AstNode VisitDocumentQueryNode(DocumentQueryNode documentQueryNode) => (AstNode) new DocumentQueryNode(documentQueryNode.MatchFunctionName, documentQueryNode.DocumentSymbol, documentQueryNode.Rulesets.Select<RulesetNode, RulesetNode>((Func<RulesetNode, RulesetNode>) (ruleset => (RulesetNode) ruleset.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<RulesetNode>());

    public override AstNode VisitAttribNode(AttribNode attrib) => (AstNode) new AttribNode(attrib.SelectorNamespacePrefixNode != null ? (SelectorNamespacePrefixNode) attrib.SelectorNamespacePrefixNode.Accept((NodeVisitor) this) : (SelectorNamespacePrefixNode) null, attrib.Ident, (AttribOperatorAndValueNode) attrib.OperatorAndValueNode.Accept((NodeVisitor) this));

    public override AstNode VisitAttribOperatorAndValueNode(
      AttribOperatorAndValueNode attribOperatorAndValueNode)
    {
      return (AstNode) new AttribOperatorAndValueNode(attribOperatorAndValueNode.AttribOperatorKind, attribOperatorAndValueNode.IdentOrString);
    }

    public override AstNode VisitDeclarationNode(DeclarationNode declarationNode) => (AstNode) new DeclarationNode(declarationNode.Property, (ExprNode) declarationNode.ExprNode.Accept((NodeVisitor) this), declarationNode.Prio, declarationNode.ImportantComments);

    public override AstNode VisitExprNode(ExprNode exprNode) => (AstNode) new ExprNode((TermNode) exprNode.TermNode.Accept((NodeVisitor) this), exprNode.TermsWithOperators.Select<TermWithOperatorNode, TermWithOperatorNode>((Func<TermWithOperatorNode, TermWithOperatorNode>) (termWithOperatorNode => (TermWithOperatorNode) termWithOperatorNode.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<TermWithOperatorNode>(), exprNode.ImportantComments);

    public override AstNode VisitFunctionNode(FunctionNode functionNode)
    {
      AstNode astNode = functionNode.ExprNode != null ? functionNode.ExprNode.Accept((NodeVisitor) this) : (AstNode) null;
      return (AstNode) new FunctionNode(functionNode.FunctionName, (ExprNode) astNode);
    }

    public override AstNode VisitPseudoNode(PseudoNode pseudoNode) => (AstNode) new PseudoNode(pseudoNode.NumberOfColons, pseudoNode.Ident, pseudoNode.FunctionalPseudoNode != null ? (FunctionalPseudoNode) pseudoNode.FunctionalPseudoNode.Accept((NodeVisitor) this) : (FunctionalPseudoNode) null);

    public override AstNode VisitSelectorNode(SelectorNode selectorNode) => (AstNode) new SelectorNode((SimpleSelectorSequenceNode) selectorNode.SimpleSelectorSequenceNode.Accept((NodeVisitor) this), selectorNode.CombinatorSimpleSelectorSequenceNodes.Select<CombinatorSimpleSelectorSequenceNode, CombinatorSimpleSelectorSequenceNode>((Func<CombinatorSimpleSelectorSequenceNode, CombinatorSimpleSelectorSequenceNode>) (combinatorSimpleSelectorSequenceNode => (CombinatorSimpleSelectorSequenceNode) combinatorSimpleSelectorSequenceNode.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<CombinatorSimpleSelectorSequenceNode>());

    public override AstNode VisitTermNode(TermNode termNode) => (AstNode) new TermNode(termNode.UnaryOperator, termNode.NumberBasedValue, termNode.StringBasedValue, termNode.Hexcolor, (FunctionNode) termNode.FunctionNode.NullSafeAction<FunctionNode, AstNode>((Func<FunctionNode, AstNode>) (nsa => nsa.Accept((NodeVisitor) this))), termNode.ImportantComments, termNode.ReplacementTokenBasedValue);

    public override AstNode VisitTermWithOperatorNode(TermWithOperatorNode termWithOperatorNode) => (AstNode) new TermWithOperatorNode(termWithOperatorNode.Operator, (TermNode) termWithOperatorNode.TermNode.Accept((NodeVisitor) this));

    public override AstNode VisitFunctionalPseudoNode(FunctionalPseudoNode functionalPseudoNode) => (AstNode) new FunctionalPseudoNode(functionalPseudoNode.FunctionName, (SelectorExpressionNode) functionalPseudoNode.SelectorExpressionNode.Accept((NodeVisitor) this));

    public override AstNode VisitHashClassAtNameAttribPseudoNegationNode(
      HashClassAtNameAttribPseudoNegationNode hashClassAtNameAttribPseudoNegationNode)
    {
      return (AstNode) new HashClassAtNameAttribPseudoNegationNode(hashClassAtNameAttribPseudoNegationNode.Hash, hashClassAtNameAttribPseudoNegationNode.CssClass, hashClassAtNameAttribPseudoNegationNode.ReplacementToken, hashClassAtNameAttribPseudoNegationNode.AtName, hashClassAtNameAttribPseudoNegationNode.AttribNode != null ? (AttribNode) hashClassAtNameAttribPseudoNegationNode.AttribNode.Accept((NodeVisitor) this) : (AttribNode) null, hashClassAtNameAttribPseudoNegationNode.PseudoNode != null ? (PseudoNode) hashClassAtNameAttribPseudoNegationNode.PseudoNode.Accept((NodeVisitor) this) : (PseudoNode) null, hashClassAtNameAttribPseudoNegationNode.NegationNode != null ? (NegationNode) hashClassAtNameAttribPseudoNegationNode.NegationNode.Accept((NodeVisitor) this) : (NegationNode) null);
    }

    public override AstNode VisitSelectorNamespacePrefixNode(
      SelectorNamespacePrefixNode selectorNamespacePrefixNode)
    {
      return (AstNode) new SelectorNamespacePrefixNode(selectorNamespacePrefixNode.Prefix);
    }

    public override AstNode VisitNegationArgNode(NegationArgNode negationArgNode) => (AstNode) new NegationArgNode(negationArgNode.TypeSelectorNode != null ? (TypeSelectorNode) negationArgNode.TypeSelectorNode.Accept((NodeVisitor) this) : (TypeSelectorNode) null, negationArgNode.UniversalSelectorNode != null ? (UniversalSelectorNode) negationArgNode.UniversalSelectorNode.Accept((NodeVisitor) this) : (UniversalSelectorNode) null, negationArgNode.Hash, negationArgNode.CssClass, negationArgNode.AttribNode != null ? (AttribNode) negationArgNode.AttribNode.Accept((NodeVisitor) this) : (AttribNode) null, negationArgNode.PseudoNode != null ? (PseudoNode) negationArgNode.PseudoNode.Accept((NodeVisitor) this) : (PseudoNode) null);

    public override AstNode VisitNegationNode(NegationNode negationNode) => (AstNode) new NegationNode((NegationArgNode) negationNode.NegationArgNode.Accept((NodeVisitor) this));

    public override AstNode VisitSelectorExpressionNode(
      SelectorExpressionNode selectorExpressionNode)
    {
      return (AstNode) new SelectorExpressionNode(selectorExpressionNode.SelectorExpressions);
    }

    public override AstNode VisitSelectorsGroupNode(SelectorsGroupNode selectorsGroupNode) => (AstNode) new SelectorsGroupNode(selectorsGroupNode.SelectorNodes.Select<SelectorNode, SelectorNode>((Func<SelectorNode, SelectorNode>) (selectorNode => (SelectorNode) selectorNode.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<SelectorNode>());

    public override AstNode VisitSimpleSelectorSequenceNode(
      SimpleSelectorSequenceNode simpleSelectorSequenceNode)
    {
      return (AstNode) new SimpleSelectorSequenceNode(simpleSelectorSequenceNode.TypeSelectorNode != null ? (TypeSelectorNode) simpleSelectorSequenceNode.TypeSelectorNode.Accept((NodeVisitor) this) : (TypeSelectorNode) null, simpleSelectorSequenceNode.UniversalSelectorNode != null ? (UniversalSelectorNode) simpleSelectorSequenceNode.UniversalSelectorNode.Accept((NodeVisitor) this) : (UniversalSelectorNode) null, simpleSelectorSequenceNode.Separator, simpleSelectorSequenceNode.HashClassAttribPseudoNegationNodes.Select<HashClassAtNameAttribPseudoNegationNode, HashClassAtNameAttribPseudoNegationNode>((Func<HashClassAtNameAttribPseudoNegationNode, HashClassAtNameAttribPseudoNegationNode>) (hashClassAtNameAttribPseudoNegationNode => (HashClassAtNameAttribPseudoNegationNode) hashClassAtNameAttribPseudoNegationNode.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<HashClassAtNameAttribPseudoNegationNode>());
    }

    public override AstNode VisitTypeSelectorNode(TypeSelectorNode typeSelectorNode) => (AstNode) new TypeSelectorNode(typeSelectorNode.SelectorNamespacePrefixNode != null ? (SelectorNamespacePrefixNode) typeSelectorNode.SelectorNamespacePrefixNode.Accept((NodeVisitor) this) : (SelectorNamespacePrefixNode) null, typeSelectorNode.ElementName);

    public override AstNode VisitUniversalSelectorNode(UniversalSelectorNode universalSelectorNode) => (AstNode) new UniversalSelectorNode(universalSelectorNode.SelectorNamespacePrefixNode != null ? (SelectorNamespacePrefixNode) universalSelectorNode.SelectorNamespacePrefixNode.Accept((NodeVisitor) this) : (SelectorNamespacePrefixNode) null);

    public override AstNode VisitCombinatorSimpleSelectorSequenceNode(
      CombinatorSimpleSelectorSequenceNode combinatorSimpleSelectorSequenceNode)
    {
      return (AstNode) new CombinatorSimpleSelectorSequenceNode(combinatorSimpleSelectorSequenceNode.Combinator, (SimpleSelectorSequenceNode) combinatorSimpleSelectorSequenceNode.SimpleSelectorSequenceNode.Accept((NodeVisitor) this));
    }

    public override AstNode VisitNamespaceNode(NamespaceNode namespaceNode) => (AstNode) new NamespaceNode(namespaceNode.Prefix, namespaceNode.Value);

    public override AstNode VisitMediaQueryNode(MediaQueryNode mediaQueryNode) => (AstNode) new MediaQueryNode(mediaQueryNode.OnlyText, mediaQueryNode.NotText, mediaQueryNode.MediaType, mediaQueryNode.MediaExpressions.Select<MediaExpressionNode, MediaExpressionNode>((Func<MediaExpressionNode, MediaExpressionNode>) (mediaExpressionNode => (MediaExpressionNode) mediaExpressionNode.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<MediaExpressionNode>());

    public override AstNode VisitMediaExpressionNode(MediaExpressionNode mediaExpressionNode) => (AstNode) new MediaExpressionNode(mediaExpressionNode.MediaFeature, mediaExpressionNode.ExprNode != null ? (ExprNode) mediaExpressionNode.ExprNode.Accept((NodeVisitor) this) : (ExprNode) null);

    public override AstNode VisitKeyFramesNode(KeyFramesNode keyFramesNode) => (AstNode) new KeyFramesNode(keyFramesNode.KeyFramesSymbol, keyFramesNode.IdentValue, keyFramesNode.StringValue, keyFramesNode.KeyFramesBlockNodes.Select<KeyFramesBlockNode, KeyFramesBlockNode>((Func<KeyFramesBlockNode, KeyFramesBlockNode>) (keyFramesBlockNode => (KeyFramesBlockNode) keyFramesBlockNode.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<KeyFramesBlockNode>());

    public override AstNode VisitKeyFramesBlockNode(KeyFramesBlockNode keyFramesBlockNode) => (AstNode) new KeyFramesBlockNode(keyFramesBlockNode.KeyFramesSelectors, keyFramesBlockNode.DeclarationNodes.Select<DeclarationNode, DeclarationNode>((Func<DeclarationNode, DeclarationNode>) (declarationNode => (DeclarationNode) declarationNode.Accept((NodeVisitor) this))).ToSafeReadOnlyCollection<DeclarationNode>());
  }
}
