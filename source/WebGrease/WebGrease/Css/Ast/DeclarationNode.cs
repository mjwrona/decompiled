// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.DeclarationNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast
{
  public sealed class DeclarationNode : AstNode
  {
    public DeclarationNode(
      string property,
      ExprNode exprNode,
      string prio,
      ReadOnlyCollection<ImportantCommentNode> importantComments)
    {
      this.Property = property;
      this.ExprNode = exprNode;
      this.Prio = prio ?? string.Empty;
      this.ImportantComments = importantComments ?? new List<ImportantCommentNode>().AsReadOnly();
    }

    public ReadOnlyCollection<ImportantCommentNode> ImportantComments { get; private set; }

    public string Property { get; private set; }

    public ExprNode ExprNode { get; private set; }

    public string Prio { get; private set; }

    public bool Equals(DeclarationNode declarationNode) => declarationNode.Property.Equals(this.Property) && declarationNode.ExprNode.Equals(this.ExprNode) && declarationNode.Prio.Equals(this.Prio);

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitDeclarationNode(this);
  }
}
