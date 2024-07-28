// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.PageNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.ObjectModel;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast
{
  public sealed class PageNode : StyleSheetRuleNode
  {
    public PageNode(string pseudoPage, ReadOnlyCollection<DeclarationNode> declarations)
    {
      this.PseudoPage = pseudoPage;
      this.Declarations = declarations;
    }

    public string PseudoPage { get; private set; }

    public ReadOnlyCollection<DeclarationNode> Declarations { get; private set; }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitPageNode(this);
  }
}
