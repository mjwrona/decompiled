// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.CombinatorSimpleSelectorSequenceNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using WebGrease.Css.Ast.Selectors;
using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast
{
  public sealed class CombinatorSimpleSelectorSequenceNode : AstNode
  {
    public CombinatorSimpleSelectorSequenceNode(
      Combinator combinator,
      SimpleSelectorSequenceNode simpleSelectorSequenceNode)
    {
      this.Combinator = combinator;
      this.SimpleSelectorSequenceNode = simpleSelectorSequenceNode;
    }

    public Combinator Combinator { get; private set; }

    public SimpleSelectorSequenceNode SimpleSelectorSequenceNode { get; private set; }

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitCombinatorSimpleSelectorSequenceNode(this);
  }
}
