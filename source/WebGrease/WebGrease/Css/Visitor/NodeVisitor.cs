// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Visitor.NodeVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using WebGrease.Css.Ast;
using WebGrease.Css.Ast.Animation;
using WebGrease.Css.Ast.MediaQuery;
using WebGrease.Css.Ast.Selectors;

namespace WebGrease.Css.Visitor
{
  public abstract class NodeVisitor
  {
    public virtual AstNode VisitStyleSheetNode(StyleSheetNode styleSheet) => (AstNode) styleSheet;

    public virtual AstNode VisitImportNode(ImportNode importNode) => (AstNode) importNode;

    public virtual AstNode VisitRulesetNode(RulesetNode rulesetNode) => (AstNode) rulesetNode;

    public virtual AstNode VisitMediaNode(MediaNode mediaNode) => (AstNode) mediaNode;

    public virtual AstNode VisitPageNode(PageNode pageNode) => (AstNode) pageNode;

    public virtual AstNode VisitAttribNode(AttribNode attrib) => (AstNode) attrib;

    public virtual AstNode VisitAttribOperatorAndValueNode(
      AttribOperatorAndValueNode attribOperatorAndValueNode)
    {
      return (AstNode) attribOperatorAndValueNode;
    }

    public virtual AstNode VisitDeclarationNode(DeclarationNode declarationNode) => (AstNode) declarationNode;

    public virtual AstNode VisitExprNode(ExprNode exprNode) => (AstNode) exprNode;

    public virtual AstNode VisitFunctionNode(FunctionNode functionNode) => (AstNode) functionNode;

    public virtual AstNode VisitPseudoNode(PseudoNode pseudoNode) => (AstNode) pseudoNode;

    public virtual AstNode VisitSelectorNode(SelectorNode selectorNode) => (AstNode) selectorNode;

    public virtual AstNode VisitTermNode(TermNode termNode) => (AstNode) termNode;

    public virtual AstNode VisitTermWithOperatorNode(TermWithOperatorNode termWithOperatorNode) => (AstNode) termWithOperatorNode;

    public virtual AstNode VisitFunctionalPseudoNode(FunctionalPseudoNode functionalPseudoNode) => (AstNode) functionalPseudoNode;

    public virtual AstNode VisitHashClassAtNameAttribPseudoNegationNode(
      HashClassAtNameAttribPseudoNegationNode hashClassAtNameAttribPseudoNegationNode)
    {
      return (AstNode) hashClassAtNameAttribPseudoNegationNode;
    }

    public virtual AstNode VisitSelectorNamespacePrefixNode(
      SelectorNamespacePrefixNode selectorNamespacePrefixNode)
    {
      return (AstNode) selectorNamespacePrefixNode;
    }

    public virtual AstNode VisitNegationArgNode(NegationArgNode negationArgNode) => (AstNode) negationArgNode;

    public virtual AstNode VisitNegationNode(NegationNode negationNode) => (AstNode) negationNode;

    public virtual AstNode VisitSelectorExpressionNode(SelectorExpressionNode selectorExpressionNode) => (AstNode) selectorExpressionNode;

    public virtual AstNode VisitSelectorsGroupNode(SelectorsGroupNode selectorsGroupNode) => (AstNode) selectorsGroupNode;

    public virtual AstNode VisitSimpleSelectorSequenceNode(
      SimpleSelectorSequenceNode simpleSelectorSequenceNode)
    {
      return (AstNode) simpleSelectorSequenceNode;
    }

    public virtual AstNode VisitTypeSelectorNode(TypeSelectorNode typeSelectorNode) => (AstNode) typeSelectorNode;

    public virtual AstNode VisitUniversalSelectorNode(UniversalSelectorNode universalSelectorNode) => (AstNode) universalSelectorNode;

    public virtual AstNode VisitCombinatorSimpleSelectorSequenceNode(
      CombinatorSimpleSelectorSequenceNode combinatorSimpleSelectorSequenceNode)
    {
      return (AstNode) combinatorSimpleSelectorSequenceNode;
    }

    public virtual AstNode VisitNamespaceNode(NamespaceNode namespaceNode) => (AstNode) namespaceNode;

    public virtual AstNode VisitMediaQueryNode(MediaQueryNode mediaQueryNode) => (AstNode) mediaQueryNode;

    public virtual AstNode VisitMediaExpressionNode(MediaExpressionNode mediaExpressionNode) => (AstNode) mediaExpressionNode;

    public virtual AstNode VisitKeyFramesNode(KeyFramesNode keyFramesNode) => (AstNode) keyFramesNode;

    public virtual AstNode VisitKeyFramesBlockNode(KeyFramesBlockNode keyFramesBlockNode) => (AstNode) keyFramesBlockNode;

    public virtual AstNode VisitDocumentQueryNode(DocumentQueryNode documentQueryNode) => (AstNode) documentQueryNode;

    public virtual AstNode VisitImportantCommentNode(ImportantCommentNode commentNode) => (AstNode) commentNode;
  }
}
