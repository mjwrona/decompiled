// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.Selectors.HashClassAtNameAttribPseudoNegationNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast.Selectors
{
  public sealed class HashClassAtNameAttribPseudoNegationNode : AstNode
  {
    private const string ExceptionMessage = "Only a single value out of hash or class or at name or attrib node or pseudo node or negation node can be not null.";

    public HashClassAtNameAttribPseudoNegationNode(
      string hash,
      string cssClass,
      string replacementToken,
      string atName,
      AttribNode attribNode,
      PseudoNode pseudoNode,
      NegationNode negationNode)
    {
      if (!string.IsNullOrWhiteSpace(hash))
      {
        if (!string.IsNullOrWhiteSpace(cssClass) || !string.IsNullOrWhiteSpace(atName) || !string.IsNullOrWhiteSpace(replacementToken) || attribNode != null || pseudoNode != null || negationNode != null)
          throw new AstException("Only a single value out of hash or class or at name or attrib node or pseudo node or negation node can be not null.");
      }
      else if (!string.IsNullOrWhiteSpace(cssClass))
      {
        if (!string.IsNullOrWhiteSpace(replacementToken) || !string.IsNullOrWhiteSpace(atName) || attribNode != null || pseudoNode != null || negationNode != null)
          throw new AstException("Only a single value out of hash or class or at name or attrib node or pseudo node or negation node can be not null.");
      }
      else if (!string.IsNullOrWhiteSpace(replacementToken))
      {
        if (!string.IsNullOrWhiteSpace(atName) || attribNode != null || pseudoNode != null || negationNode != null)
          throw new AstException("Only a single value out of hash or class or at name or attrib node or pseudo node or negation node can be not null.");
      }
      else if (!string.IsNullOrWhiteSpace(atName))
      {
        if (attribNode != null || pseudoNode != null || negationNode != null)
          throw new AstException("Only a single value out of hash or class or at name or attrib node or pseudo node or negation node can be not null.");
      }
      else if (attribNode != null)
      {
        if (pseudoNode != null || negationNode != null)
          throw new AstException("Only a single value out of hash or class or at name or attrib node or pseudo node or negation node can be not null.");
      }
      else if (pseudoNode != null && negationNode != null)
        throw new AstException("Only a single value out of hash or class or at name or attrib node or pseudo node or negation node can be not null.");
      this.Hash = hash;
      this.ReplacementToken = replacementToken;
      this.CssClass = cssClass;
      this.AtName = atName;
      this.AttribNode = attribNode;
      this.PseudoNode = pseudoNode;
      this.NegationNode = negationNode;
    }

    public string Hash { get; private set; }

    public string ReplacementToken { get; private set; }

    public string CssClass { get; private set; }

    public string AtName { get; private set; }

    public AttribNode AttribNode { get; private set; }

    public PseudoNode PseudoNode { get; private set; }

    public NegationNode NegationNode { get; private set; }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitHashClassAtNameAttribPseudoNegationNode(this);
  }
}
