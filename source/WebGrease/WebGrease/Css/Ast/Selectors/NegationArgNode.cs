// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.Selectors.NegationArgNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast.Selectors
{
  public sealed class NegationArgNode : AstNode
  {
    private const string ExceptionMessage = "Only a single value out of type selector, universal selector, hash or class or attrib node or pseudo node can be not null.";

    public NegationArgNode(
      TypeSelectorNode typeSelectorNode,
      UniversalSelectorNode universalSelectorNode,
      string hash,
      string cssClass,
      AttribNode attribNode,
      PseudoNode pseudoNode)
    {
      if (typeSelectorNode != null && (universalSelectorNode != null || !string.IsNullOrWhiteSpace(hash) || !string.IsNullOrWhiteSpace(cssClass) || attribNode != null || pseudoNode != null))
        throw new AstException("Only a single value out of type selector, universal selector, hash or class or attrib node or pseudo node can be not null.");
      if (universalSelectorNode != null && (!string.IsNullOrWhiteSpace(hash) || !string.IsNullOrWhiteSpace(cssClass) || attribNode != null || pseudoNode != null))
        throw new AstException("Only a single value out of type selector, universal selector, hash or class or attrib node or pseudo node can be not null.");
      if (!string.IsNullOrWhiteSpace(hash) && (!string.IsNullOrWhiteSpace(cssClass) || attribNode != null || pseudoNode != null))
        throw new AstException("Only a single value out of type selector, universal selector, hash or class or attrib node or pseudo node can be not null.");
      if (!string.IsNullOrWhiteSpace(cssClass) && (attribNode != null || pseudoNode != null))
        throw new AstException("Only a single value out of type selector, universal selector, hash or class or attrib node or pseudo node can be not null.");
      if (attribNode != null && pseudoNode != null)
        throw new AstException("Only a single value out of type selector, universal selector, hash or class or attrib node or pseudo node can be not null.");
      this.TypeSelectorNode = typeSelectorNode;
      this.UniversalSelectorNode = universalSelectorNode;
      this.Hash = hash;
      this.CssClass = cssClass;
      this.AttribNode = attribNode;
      this.PseudoNode = pseudoNode;
    }

    public TypeSelectorNode TypeSelectorNode { get; private set; }

    public UniversalSelectorNode UniversalSelectorNode { get; private set; }

    public string Hash { get; private set; }

    public string CssClass { get; private set; }

    public AttribNode AttribNode { get; private set; }

    public PseudoNode PseudoNode { get; private set; }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitNegationArgNode(this);
  }
}
