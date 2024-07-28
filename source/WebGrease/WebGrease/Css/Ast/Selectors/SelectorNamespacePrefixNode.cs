// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.Selectors.SelectorNamespacePrefixNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast.Selectors
{
  public sealed class SelectorNamespacePrefixNode : AstNode
  {
    public SelectorNamespacePrefixNode(string prefix)
    {
      if (string.IsNullOrWhiteSpace(prefix))
        prefix = string.Empty;
      this.Prefix = prefix;
    }

    public string Prefix { get; private set; }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitSelectorNamespacePrefixNode(this);
  }
}
