// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.TermWithOperatorNode
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using WebGrease.Css.Visitor;

namespace WebGrease.Css.Ast
{
  public sealed class TermWithOperatorNode : AstNode
  {
    private bool usesBinary;

    public TermWithOperatorNode(string op, TermNode termNode)
    {
      if (string.IsNullOrWhiteSpace(op))
        op = ' '.ToString();
      this.Operator = op;
      this.TermNode = termNode;
    }

    public bool UsesBinary
    {
      get => this.usesBinary;
      set
      {
        this.usesBinary = value;
        this.TermNode.IsBinary = value;
      }
    }

    public string Operator { get; private set; }

    public TermNode TermNode { get; private set; }

    public bool Equals(TermWithOperatorNode termWithOperator) => termWithOperator.UsesBinary == this.UsesBinary && termWithOperator.TermNode.Equals(this.TermNode) && termWithOperator.Operator.Equals(this.Operator);

    public override AstNode Accept(NodeVisitor nodeVisitor) => nodeVisitor.VisitTermWithOperatorNode(this);
  }
}
