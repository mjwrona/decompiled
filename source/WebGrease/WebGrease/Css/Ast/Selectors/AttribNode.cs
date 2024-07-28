// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.Selectors.AttribNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast.Selectors
{
  public sealed class AttribNode : AstNode
  {
    public AttribNode(
      SelectorNamespacePrefixNode selectorNamespacePrefixNode,
      string identity,
      AttribOperatorAndValueNode attribOperatorAndValueNode)
    {
      this.SelectorNamespacePrefixNode = selectorNamespacePrefixNode;
      this.Ident = identity;
      this.OperatorAndValueNode = attribOperatorAndValueNode ?? new AttribOperatorAndValueNode(AttribOperatorKind.None, string.Empty);
    }

    public SelectorNamespacePrefixNode SelectorNamespacePrefixNode { get; private set; }

    public string Ident { get; private set; }

    public AttribOperatorAndValueNode OperatorAndValueNode { get; private set; }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitAttribNode(this);
  }
}
