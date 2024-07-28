// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.CommonTreeTransformer
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WebGrease.Css.Ast;
using WebGrease.Css.Ast.Animation;
using WebGrease.Css.Ast.MediaQuery;
using WebGrease.Css.Ast.Selectors;
using WebGrease.Css.Extensions;

namespace WebGrease.Css
{
  internal static class CommonTreeTransformer
  {
    internal static StyleSheetNode CreateStyleSheetNode(CommonTree commonTree)
    {
      CommonTree styleSheetTree = commonTree.Children(CommonTreeTransformer.T(180)).FirstOrDefault<CommonTree>();
      return new StyleSheetNode(CommonTreeTransformer.CreateCharsetNode(styleSheetTree), CommonTreeTransformer.CreateImportNodes(styleSheetTree), CommonTreeTransformer.CreateNamespaceNodes(styleSheetTree), CommonTreeTransformer.CreateStyleSheetRulesNodes(styleSheetTree));
    }

    private static string CreateCharsetNode(CommonTree styleSheetTree)
    {
      if (styleSheetTree == null)
        return (string) null;
      CommonTree commonTree = styleSheetTree.Children(CommonTreeTransformer.T(116)).FirstOrDefault<CommonTree>();
      return commonTree == null ? (string) null : CommonTreeTransformer.StringOrUriBasedValue(commonTree.Children(CommonTreeTransformer.T(179)).FirstChildText());
    }

    private static ReadOnlyCollection<StyleSheetRuleNode> CreateStyleSheetRulesNodes(
      CommonTree styleSheetTree)
    {
      if (styleSheetTree == null)
        return Enumerable.Empty<StyleSheetRuleNode>().ToSafeReadOnlyCollection<StyleSheetRuleNode>();
      List<StyleSheetRuleNode> styleSheetRuleNodeList = new List<StyleSheetRuleNode>();
      foreach (CommonTree child in styleSheetTree.Children())
      {
        if (child.Text == CommonTreeTransformer.T(171))
          styleSheetRuleNodeList.Add((StyleSheetRuleNode) CommonTreeTransformer.CreateRulesetNode(child));
        else if (child.Text == CommonTreeTransformer.T(147))
          styleSheetRuleNodeList.Add((StyleSheetRuleNode) CommonTreeTransformer.CreateMediaNode(child));
        else if (child.Text == CommonTreeTransformer.T(163))
          styleSheetRuleNodeList.Add((StyleSheetRuleNode) CommonTreeTransformer.CreatePageNode(child));
        else if (child.Text == CommonTreeTransformer.T(141))
          styleSheetRuleNodeList.Add((StyleSheetRuleNode) CommonTreeTransformer.CreateKeyFramesNode(child));
        else if (child.Text == CommonTreeTransformer.T(124))
          styleSheetRuleNodeList.Add((StyleSheetRuleNode) CommonTreeTransformer.CreateDocumentQueryNode(child));
        else if (child.Type.Equals(42))
          styleSheetRuleNodeList.Add((StyleSheetRuleNode) new StyleSheetRuleOrCommentNode(new ImportantCommentNode(child.Text), true));
      }
      return styleSheetRuleNodeList.AsReadOnly();
    }

    private static ReadOnlyCollection<ImportNode> CreateImportNodes(CommonTree styleSheetTree) => styleSheetTree == null ? Enumerable.Empty<ImportNode>().ToSafeReadOnlyCollection<ImportNode>() : styleSheetTree.GrandChildren(CommonTreeTransformer.T(140)).Select<CommonTree, ImportNode>((Func<CommonTree, ImportNode>) (import =>
    {
      CommonTree commonTree = import.Children().FirstOrDefault<CommonTree>();
      if (commonTree == null)
        return (ImportNode) null;
      AllowedImportData allowedImportDataType = AllowedImportData.None;
      string importDataValue = (string) null;
      if (commonTree.Text == CommonTreeTransformer.T(179))
      {
        allowedImportDataType = AllowedImportData.String;
        importDataValue = CommonTreeTransformer.StringOrUriBasedValue(commonTree.FirstChildText());
      }
      else if (commonTree.Text == CommonTreeTransformer.T(187))
      {
        allowedImportDataType = AllowedImportData.Uri;
        importDataValue = CommonTreeTransformer.StringOrUriBasedValue(commonTree.FirstChildText());
      }
      return new ImportNode(allowedImportDataType, importDataValue, import.GrandChildren(CommonTreeTransformer.T(152)).Select<CommonTree, MediaQueryNode>(new Func<CommonTree, MediaQueryNode>(CommonTreeTransformer.CreateMediaQueryNode)).ToSafeReadOnlyCollection<MediaQueryNode>());
    })).ToSafeReadOnlyCollection<ImportNode>();

    private static MediaQueryNode CreateMediaQueryNode(CommonTree mediaQueryTree) => new MediaQueryNode(mediaQueryTree.Children(CommonTreeTransformer.T(161)).FirstChildText(), mediaQueryTree.Children(CommonTreeTransformer.T(159)).FirstChildText(), mediaQueryTree.Children(CommonTreeTransformer.T(153)).FirstChildText(), mediaQueryTree.GrandChildren(CommonTreeTransformer.T(149)).Select<CommonTree, MediaExpressionNode>(new Func<CommonTree, MediaExpressionNode>(CommonTreeTransformer.CreateMediaExpressionNode)).ToSafeReadOnlyCollection<MediaExpressionNode>());

    private static MediaExpressionNode CreateMediaExpressionNode(CommonTree mediaExpressionTree) => new MediaExpressionNode(mediaExpressionTree.Children(CommonTreeTransformer.T(150)).FirstChildText(), CommonTreeTransformer.CreateExpressionNode(mediaExpressionTree.Children(CommonTreeTransformer.T(128)).FirstOrDefault<CommonTree>()));

    private static ReadOnlyCollection<NamespaceNode> CreateNamespaceNodes(CommonTree styleSheetTree) => styleSheetTree == null ? Enumerable.Empty<NamespaceNode>().ToSafeReadOnlyCollection<NamespaceNode>() : styleSheetTree.GrandChildren(CommonTreeTransformer.T(155)).Select<CommonTree, NamespaceNode>((Func<CommonTree, NamespaceNode>) (ns =>
    {
      string str = CommonTreeTransformer.StringOrUriBasedValue(ns.Children(CommonTreeTransformer.T(179)).FirstChildText());
      if (string.IsNullOrWhiteSpace(str))
        str = CommonTreeTransformer.StringOrUriBasedValue(ns.Children(CommonTreeTransformer.T(187)).FirstChildText());
      return new NamespaceNode(ns.Children(CommonTreeTransformer.T(156)).FirstChildText(), str);
    })).ToSafeReadOnlyCollection<NamespaceNode>();

    private static RulesetNode CreateRulesetNode(CommonTree rulesetTree) => rulesetTree == null ? (RulesetNode) null : new RulesetNode(CommonTreeTransformer.CreateSelectorsGroupNode(rulesetTree.GrandChildren(CommonTreeTransformer.T(174))), CommonTreeTransformer.CreateDeclarationNodes(rulesetTree.GrandChildren(CommonTreeTransformer.T(123))).ToSafeReadOnlyCollection<DeclarationNode>(), CommonTreeTransformer.CreateImportantCommentNodes(rulesetTree));

    private static ReadOnlyCollection<ImportantCommentNode> CreateImportantCommentNodes(
      CommonTree commonTree)
    {
      List<ImportantCommentNode> importantCommentNodeList = new List<ImportantCommentNode>();
      foreach (ITree child in (IEnumerable<ITree>) commonTree.Children)
      {
        if (child.Type.Equals(42))
          importantCommentNodeList.Add(new ImportantCommentNode(child.Text));
      }
      return importantCommentNodeList.AsReadOnly();
    }

    private static MediaNode CreateMediaNode(CommonTree mediaTree) => mediaTree == null ? (MediaNode) null : new MediaNode(mediaTree.GrandChildren(CommonTreeTransformer.T(152)).Select<CommonTree, MediaQueryNode>(new Func<CommonTree, MediaQueryNode>(CommonTreeTransformer.CreateMediaQueryNode)).ToSafeReadOnlyCollection<MediaQueryNode>(), mediaTree.GrandChildren(CommonTreeTransformer.T(172)).Select<CommonTree, RulesetNode>(new Func<CommonTree, RulesetNode>(CommonTreeTransformer.CreateRulesetNode)).ToSafeReadOnlyCollection<RulesetNode>(), mediaTree.GrandChildren(CommonTreeTransformer.T(163)).Select<CommonTree, PageNode>(new Func<CommonTree, PageNode>(CommonTreeTransformer.CreatePageNode)).ToSafeReadOnlyCollection<PageNode>());

    private static PageNode CreatePageNode(CommonTree pageTree) => pageTree == null ? (PageNode) null : new PageNode(string.Join(string.Empty, pageTree.GrandChildren(CommonTreeTransformer.T(167)).Select<CommonTree, string>((Func<CommonTree, string>) (pseudo => pseudo.Text))), CommonTreeTransformer.CreateDeclarationNodes(pageTree.GrandChildren(CommonTreeTransformer.T(123))).ToSafeReadOnlyCollection<DeclarationNode>());

    private static DocumentQueryNode CreateDocumentQueryNode(CommonTree documentTree) => new DocumentQueryNode(string.Join(string.Empty, documentTree.GrandChildren(CommonTreeTransformer.T(125)).Select<CommonTree, string>((Func<CommonTree, string>) (_ => _.Text))), documentTree.Children(CommonTreeTransformer.T(126)).FirstChildText(), documentTree.GrandChildren(CommonTreeTransformer.T(172)).Select<CommonTree, RulesetNode>(new Func<CommonTree, RulesetNode>(CommonTreeTransformer.CreateRulesetNode)).ToSafeReadOnlyCollection<RulesetNode>());

    private static KeyFramesNode CreateKeyFramesNode(CommonTree styleSheetChild) => new KeyFramesNode(styleSheetChild.Children(CommonTreeTransformer.T(146)).FirstChildText(), styleSheetChild.Children(CommonTreeTransformer.T(137)).FirstChildText(), CommonTreeTransformer.StringOrUriBasedValue(styleSheetChild.Children(CommonTreeTransformer.T(179)).FirstChildText()), styleSheetChild.GrandChildren(CommonTreeTransformer.T(143)).Select<CommonTree, KeyFramesBlockNode>(new Func<CommonTree, KeyFramesBlockNode>(CommonTreeTransformer.CreateKeyFramesBlockNode)).ToSafeReadOnlyCollection<KeyFramesBlockNode>());

    private static KeyFramesBlockNode CreateKeyFramesBlockNode(CommonTree keyFramesBlockTree) => new KeyFramesBlockNode(keyFramesBlockTree.GrandChildren(CommonTreeTransformer.T(145)).Select<CommonTree, string>((Func<CommonTree, string>) (keyFramesSelector => keyFramesSelector.FirstChildText())).ToSafeReadOnlyCollection<string>(), CommonTreeTransformer.CreateDeclarationNodes(keyFramesBlockTree.GrandChildren(CommonTreeTransformer.T(123))).ToSafeReadOnlyCollection<DeclarationNode>());

    private static IEnumerable<DeclarationNode> CreateDeclarationNodes(
      IEnumerable<CommonTree> declarationTreeNodes)
    {
      return declarationTreeNodes.Select<CommonTree, DeclarationNode>((Func<CommonTree, DeclarationNode>) (declaration => new DeclarationNode(string.Join(string.Empty, declaration.GrandChildren(CommonTreeTransformer.T(164)).Select<CommonTree, string>((Func<CommonTree, string>) (_ => _.Text))), CommonTreeTransformer.CreateExpressionNode(declaration.Children(CommonTreeTransformer.T(128)).FirstOrDefault<CommonTree>()), declaration.Children(CommonTreeTransformer.T(139)).FirstChildText(), CommonTreeTransformer.CreateImportantCommentNodes(declaration))));
    }

    private static ExprNode CreateExpressionNode(CommonTree exprTree) => exprTree == null ? (ExprNode) null : new ExprNode(CommonTreeTransformer.CreateTermNode(exprTree.Children(CommonTreeTransformer.T(181)).FirstOrDefault<CommonTree>()), CommonTreeTransformer.CreateTermWithOperatorsNode(exprTree.GrandChildren(CommonTreeTransformer.T(183))).ToSafeReadOnlyCollection<TermWithOperatorNode>(), CommonTreeTransformer.CreateImportantCommentNodes(exprTree));

    private static IEnumerable<TermWithOperatorNode> CreateTermWithOperatorsNode(
      IEnumerable<CommonTree> termWithOperatorTreeNodes)
    {
      return termWithOperatorTreeNodes.Select<CommonTree, TermWithOperatorNode>((Func<CommonTree, TermWithOperatorNode>) (termWithOperatorNode => new TermWithOperatorNode(termWithOperatorNode.Children(CommonTreeTransformer.T(162)).FirstChildText(), CommonTreeTransformer.CreateTermNode(termWithOperatorNode.Children(CommonTreeTransformer.T(181)).FirstOrDefault<CommonTree>()))));
    }

    private static TermNode CreateTermNode(CommonTree termTree)
    {
      if (termTree == null)
        return (TermNode) null;
      string unaryOperator = termTree.Children(CommonTreeTransformer.T(185)).FirstChildText();
      string numberBasedValue = termTree.Children(CommonTreeTransformer.T(160)).FirstChildText();
      string replacementTokenBasedValue = termTree.Children(CommonTreeTransformer.T(169)).FirstChildText();
      string stringBasedValue = CommonTreeTransformer.StringOrUriBasedValue(termTree.Children(CommonTreeTransformer.T(187)).FirstChildText());
      if (string.IsNullOrWhiteSpace(stringBasedValue))
        stringBasedValue = CommonTreeTransformer.StringOrUriBasedValue(termTree.Children(CommonTreeTransformer.T(179)).FirstChildText());
      if (string.IsNullOrWhiteSpace(stringBasedValue))
        stringBasedValue = CommonTreeTransformer.StringOrUriBasedValue(termTree.Children(CommonTreeTransformer.T(137)).FirstChildText());
      CommonTree commonTree = termTree.Children(CommonTreeTransformer.T(136)).FirstOrDefault<CommonTree>();
      string hexColor = commonTree != null ? commonTree.Children(CommonTreeTransformer.T(135)).FirstChildText() : (string) null;
      ReadOnlyCollection<ImportantCommentNode> importantCommentNodes = CommonTreeTransformer.CreateImportantCommentNodes(termTree);
      return new TermNode(unaryOperator, numberBasedValue, stringBasedValue, hexColor, CommonTreeTransformer.CreateFunctionNode(termTree.Children(CommonTreeTransformer.T(130)).FirstOrDefault<CommonTree>()), importantCommentNodes, replacementTokenBasedValue);
    }

    private static FunctionNode CreateFunctionNode(CommonTree functionTree) => functionTree == null ? (FunctionNode) null : new FunctionNode(functionTree.Children(CommonTreeTransformer.T(131)).FirstChildText(), CommonTreeTransformer.CreateExpressionNode(functionTree.Children(CommonTreeTransformer.T(128)).FirstOrDefault<CommonTree>()));

    private static SelectorsGroupNode CreateSelectorsGroupNode(
      IEnumerable<CommonTree> selectorTreeNodes)
    {
      return new SelectorsGroupNode(selectorTreeNodes.Select<CommonTree, SelectorNode>((Func<CommonTree, SelectorNode>) (selector => new SelectorNode(CommonTreeTransformer.CreateSimpleSelectorSequenceNode(selector.Children(CommonTreeTransformer.T(177)).FirstOrDefault<CommonTree>()), CommonTreeTransformer.CreateCombinatorSimpleSelectorSequenceNode(selector.GrandChildren(CommonTreeTransformer.T(121))).ToSafeReadOnlyCollection<CombinatorSimpleSelectorSequenceNode>()))).ToSafeReadOnlyCollection<SelectorNode>());
    }

    private static IEnumerable<CombinatorSimpleSelectorSequenceNode> CreateCombinatorSimpleSelectorSequenceNode(
      IEnumerable<CommonTree> combinatorSimpleSelectorSequenceTreeNodes)
    {
      return combinatorSimpleSelectorSequenceTreeNodes.Select<CommonTree, CombinatorSimpleSelectorSequenceNode>((Func<CommonTree, CombinatorSimpleSelectorSequenceNode>) (combinatorSimpleSelectorSequenceNode => new CombinatorSimpleSelectorSequenceNode(CommonTreeTransformer.CreateCombinatorNode(combinatorSimpleSelectorSequenceNode.Children(CommonTreeTransformer.T(119)).FirstOrDefault<CommonTree>()), CommonTreeTransformer.CreateSimpleSelectorSequenceNode(combinatorSimpleSelectorSequenceNode.Children(CommonTreeTransformer.T(177)).FirstOrDefault<CommonTree>()))));
    }

    private static Combinator CreateCombinatorNode(CommonTree combinatorTree)
    {
      Combinator combinatorNode = Combinator.None;
      if (combinatorTree == null)
        return combinatorNode;
      switch (combinatorTree.FirstChildText())
      {
        case "+":
          return Combinator.PlusSign;
        case ">":
          return Combinator.GreaterThanSign;
        case "~":
          return Combinator.Tilde;
        case "WHITESPACE":
          return CommonTreeTransformer.GetWhitespaceCount(combinatorTree) > 0 ? Combinator.SingleSpace : Combinator.ZeroSpace;
        default:
          throw new AstException("Encountered an invalid combinator.");
      }
    }

    private static int GetWhitespaceCount(CommonTree commonTree)
    {
      int result;
      return !int.TryParse(commonTree.Children(CommonTreeTransformer.T(190)).FirstChildText(), out result) ? 0 : result;
    }

    private static SimpleSelectorSequenceNode CreateSimpleSelectorSequenceNode(
      CommonTree simpleSelectorSequenceTree)
    {
      return simpleSelectorSequenceTree != null ? new SimpleSelectorSequenceNode(CommonTreeTransformer.CreateTypeSelectorNode(simpleSelectorSequenceTree.Children(CommonTreeTransformer.T(184)).FirstOrDefault<CommonTree>()), CommonTreeTransformer.CreateUniversalSelectorNode(simpleSelectorSequenceTree.Children(CommonTreeTransformer.T(186)).FirstOrDefault<CommonTree>()), CommonTreeTransformer.GetWhitespaceCount(simpleSelectorSequenceTree) > 0 ? ' '.ToString() : (string) null, CommonTreeTransformer.CreateHashClassAttribPseudoNegationNodes(simpleSelectorSequenceTree.GrandChildren(CommonTreeTransformer.T(134))).ToSafeReadOnlyCollection<HashClassAtNameAttribPseudoNegationNode>()) : (SimpleSelectorSequenceNode) null;
    }

    private static UniversalSelectorNode CreateUniversalSelectorNode(
      CommonTree universalSelectorTree)
    {
      return universalSelectorTree != null ? new UniversalSelectorNode(CommonTreeTransformer.CreateNamespacePrefixNode(universalSelectorTree.Children(CommonTreeTransformer.T(176)).FirstOrDefault<CommonTree>())) : (UniversalSelectorNode) null;
    }

    private static TypeSelectorNode CreateTypeSelectorNode(CommonTree typeSelectorTree) => typeSelectorTree != null ? new TypeSelectorNode(CommonTreeTransformer.CreateNamespacePrefixNode(typeSelectorTree.Children(CommonTreeTransformer.T(176)).FirstOrDefault<CommonTree>()), typeSelectorTree.Children(CommonTreeTransformer.T((int) sbyte.MaxValue)).FirstChildText()) : (TypeSelectorNode) null;

    private static SelectorNamespacePrefixNode CreateNamespacePrefixNode(
      CommonTree namespacePrefixTree)
    {
      return namespacePrefixTree != null ? new SelectorNamespacePrefixNode(namespacePrefixTree.Children(CommonTreeTransformer.T((int) sbyte.MaxValue)).FirstChildTextOrDefault(string.Empty)) : (SelectorNamespacePrefixNode) null;
    }

    private static IEnumerable<HashClassAtNameAttribPseudoNegationNode> CreateHashClassAttribPseudoNegationNodes(
      IEnumerable<CommonTree> hashClassAttribPseudoNegationTreeNodes)
    {
      return hashClassAttribPseudoNegationTreeNodes.Select<CommonTree, HashClassAtNameAttribPseudoNegationNode>((Func<CommonTree, HashClassAtNameAttribPseudoNegationNode>) (hashClassAttribPseudoNegationNode =>
      {
        CommonTree commonTree = hashClassAttribPseudoNegationNode.Children().FirstOrDefault<CommonTree>();
        string hash = (string) null;
        string replacementToken = (string) null;
        string cssClass = (string) null;
        string atName = (string) null;
        AttribNode attribNode = (AttribNode) null;
        PseudoNode pseudoNode = (PseudoNode) null;
        NegationNode negationNode = (NegationNode) null;
        if (commonTree != null)
        {
          string text = commonTree.Text;
          if (text == CommonTreeTransformer.T(135))
            hash = commonTree.FirstChildText();
          else if (text == CommonTreeTransformer.T(117))
            cssClass = commonTree.FirstChildText();
          else if (text == CommonTreeTransformer.T(110))
            atName = commonTree.FirstChildText();
          else if (text == CommonTreeTransformer.T(111))
            attribNode = CommonTreeTransformer.CreateAttribNode(commonTree);
          else if (text == CommonTreeTransformer.T(165))
            pseudoNode = CommonTreeTransformer.CreatePseudoNode(commonTree);
          else if (text == CommonTreeTransformer.T(157))
            negationNode = CommonTreeTransformer.CreateNegationNode(commonTree);
          else if (text == CommonTreeTransformer.T(170))
            replacementToken = commonTree.FirstChildText();
        }
        return new HashClassAtNameAttribPseudoNegationNode(hash, cssClass, replacementToken, atName, attribNode, pseudoNode, negationNode);
      }));
    }

    private static NegationNode CreateNegationNode(CommonTree negationTree) => negationTree != null ? new NegationNode(CommonTreeTransformer.CreateNegationArgNode(negationTree.Children(CommonTreeTransformer.T(158)).FirstOrDefault<CommonTree>())) : (NegationNode) null;

    private static NegationArgNode CreateNegationArgNode(CommonTree negationArgTree) => negationArgTree != null ? new NegationArgNode(CommonTreeTransformer.CreateTypeSelectorNode(negationArgTree.Children(CommonTreeTransformer.T(184)).FirstOrDefault<CommonTree>()), CommonTreeTransformer.CreateUniversalSelectorNode(negationArgTree.Children(CommonTreeTransformer.T(186)).FirstOrDefault<CommonTree>()), negationArgTree.Children(CommonTreeTransformer.T(135)).FirstChildText(), negationArgTree.Children(CommonTreeTransformer.T(117)).FirstChildText(), CommonTreeTransformer.CreateAttribNode(negationArgTree.Children(CommonTreeTransformer.T(111)).FirstOrDefault<CommonTree>()), CommonTreeTransformer.CreatePseudoNode(negationArgTree.Children(CommonTreeTransformer.T(165)).FirstOrDefault<CommonTree>())) : (NegationArgNode) null;

    private static PseudoNode CreatePseudoNode(CommonTree pseudoTree) => pseudoTree != null ? new PseudoNode(pseudoTree.GrandChildren(CommonTreeTransformer.T(118)).Count<CommonTree>(), pseudoTree.Children(CommonTreeTransformer.T(166)).FirstChildText(), CommonTreeTransformer.CreateFunctionalPseudoNode(pseudoTree.Children(CommonTreeTransformer.T(129)).FirstOrDefault<CommonTree>())) : (PseudoNode) null;

    private static FunctionalPseudoNode CreateFunctionalPseudoNode(CommonTree functionalPseudoTree) => functionalPseudoTree != null ? new FunctionalPseudoNode(functionalPseudoTree.Children(CommonTreeTransformer.T(131)).FirstChildText(), CommonTreeTransformer.CreateSelectorExpressionNode(functionalPseudoTree.Children(CommonTreeTransformer.T(175)).FirstOrDefault<CommonTree>())) : (FunctionalPseudoNode) null;

    private static SelectorExpressionNode CreateSelectorExpressionNode(
      CommonTree selectorExpressionTree)
    {
      return selectorExpressionTree != null ? new SelectorExpressionNode(selectorExpressionTree.Children().Select<CommonTree, string>((Func<CommonTree, string>) (_ => _.TextOrDefault())).ToSafeReadOnlyCollection<string>()) : (SelectorExpressionNode) null;
    }

    private static AttribNode CreateAttribNode(CommonTree attribTree) => attribTree != null ? new AttribNode(CommonTreeTransformer.CreateNamespacePrefixNode(attribTree.Children(CommonTreeTransformer.T(176)).FirstOrDefault<CommonTree>()), attribTree.Children(CommonTreeTransformer.T(112)).FirstChildText(), CommonTreeTransformer.CreateAttribOperatorValueNode(attribTree.Children(CommonTreeTransformer.T(114)).FirstOrDefault<CommonTree>())) : (AttribNode) null;

    private static AttribOperatorAndValueNode CreateAttribOperatorValueNode(
      CommonTree attribOperatorAndValueTree)
    {
      if (attribOperatorAndValueTree == null)
        return (AttribOperatorAndValueNode) null;
      AttribOperatorKind operatorKind = AttribOperatorKind.None;
      switch (attribOperatorAndValueTree.Children(CommonTreeTransformer.T(113)).FirstChildText())
      {
        case "^=":
          operatorKind = AttribOperatorKind.Prefix;
          break;
        case "$=":
          operatorKind = AttribOperatorKind.Suffix;
          break;
        case "*=":
          operatorKind = AttribOperatorKind.Substring;
          break;
        case "=":
          operatorKind = AttribOperatorKind.Equal;
          break;
        case "~=":
          operatorKind = AttribOperatorKind.Includes;
          break;
        case "|=":
          operatorKind = AttribOperatorKind.DashMatch;
          break;
      }
      string identityOrString = (string) null;
      CommonTree commonTree = attribOperatorAndValueTree.Children(CommonTreeTransformer.T(115)).FirstOrDefault<CommonTree>();
      if (commonTree != null)
        identityOrString = commonTree.FirstChildText() == CommonTreeTransformer.T(179) ? CommonTreeTransformer.StringOrUriBasedValue(commonTree.Children(CommonTreeTransformer.T(179)).FirstChildText()) : commonTree.FirstChildText();
      return new AttribOperatorAndValueNode(operatorKind, identityOrString);
    }

    private static string StringOrUriBasedValue(string text)
    {
      if (!string.IsNullOrWhiteSpace(text))
        text = text.Replace("\\\n", string.Empty).Replace("\\\r\n", string.Empty).Replace("\\\f", string.Empty);
      return text;
    }

    private static string T(int tokenIndex) => CssParser.tokenNames[tokenIndex];
  }
}
