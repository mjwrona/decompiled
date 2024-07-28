// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.RulesetNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using WebGrease.Css.Ast.Selectors;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast
{
  public sealed class RulesetNode : StyleSheetRuleNode
  {
    public RulesetNode(
      SelectorsGroupNode selectorsGroupNode,
      ReadOnlyCollection<DeclarationNode> declarations,
      ReadOnlyCollection<ImportantCommentNode> importantComments)
    {
      this.SelectorsGroupNode = selectorsGroupNode;
      this.Declarations = declarations ?? new List<DeclarationNode>(0).AsReadOnly();
      this.ImportantComments = importantComments ?? new List<ImportantCommentNode>(0).AsReadOnly();
    }

    public ReadOnlyCollection<ImportantCommentNode> ImportantComments { get; private set; }

    public SelectorsGroupNode SelectorsGroupNode { get; private set; }

    public ReadOnlyCollection<DeclarationNode> Declarations { get; private set; }

    public bool HasConflictingDeclaration(OrderedDictionary declarationDictionary)
    {
      foreach (DeclarationNode declaration in this.Declarations)
      {
        if (declarationDictionary.Contains((object) declaration.Property))
          return !((DeclarationNode) declarationDictionary[(object) declaration.Property]).Equals(declaration);
      }
      return false;
    }

    public bool ShouldMergeWith(RulesetNode rulesetNode)
    {
      int num = 0;
      foreach (DeclarationNode declaration1 in this.Declarations)
      {
        foreach (DeclarationNode declaration2 in rulesetNode.Declarations)
        {
          if (declaration1.Equals(declaration2))
          {
            ++num;
            break;
          }
        }
        if (num > 1)
          break;
      }
      if (num > 1)
        return true;
      if (num != 1)
        return false;
      return this.Declarations.Count == 1 || rulesetNode.Declarations.Count == 1;
    }

    public RulesetNode GetMergedRulesetNode(RulesetNode otherRulesetNode)
    {
      ReadOnlyCollection<SelectorNode> selectorNodes = new List<SelectorNode>((IEnumerable<SelectorNode>) this.SelectorsGroupNode.SelectorNodes).Union<SelectorNode>((IEnumerable<SelectorNode>) new List<SelectorNode>((IEnumerable<SelectorNode>) otherRulesetNode.SelectorsGroupNode.SelectorNodes)).ToList<SelectorNode>().AsReadOnly();
      List<DeclarationNode> declarationNodeList1 = new List<DeclarationNode>((IEnumerable<DeclarationNode>) this.Declarations);
      List<DeclarationNode> declarationNodeList2 = new List<DeclarationNode>((IEnumerable<DeclarationNode>) otherRulesetNode.Declarations);
      List<DeclarationNode> declarationNodeList3 = new List<DeclarationNode>();
      foreach (DeclarationNode declaration1 in this.Declarations)
      {
        bool flag = true;
        foreach (DeclarationNode declaration2 in otherRulesetNode.Declarations)
        {
          if (declaration1.Equals(declaration2))
          {
            flag = false;
            declarationNodeList2.Remove(declaration2);
            break;
          }
        }
        if (!flag)
        {
          declarationNodeList1.Remove(declaration1);
          declarationNodeList3.Add(declaration1);
        }
      }
      this.Declarations = declarationNodeList1.AsReadOnly();
      otherRulesetNode.Declarations = declarationNodeList2.AsReadOnly();
      return new RulesetNode(new SelectorsGroupNode(selectorNodes), declarationNodeList3.AsReadOnly(), this.ImportantComments);
    }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitRulesetNode(this);
  }
}
