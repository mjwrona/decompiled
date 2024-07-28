// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.UnaryOperator
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class UnaryOperator : Expression
  {
    private AstNode m_operand;

    public AstNode Operand
    {
      get => this.m_operand;
      set
      {
        this.m_operand.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_operand = value;
        this.m_operand.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Context OperatorContext { get; set; }

    public JSToken OperatorToken { get; set; }

    public bool IsPostfix { get; set; }

    public bool OperatorInConditionalCompilationComment { get; set; }

    public bool ConditionalCommentContainsOn { get; set; }

    public bool IsDelegator { get; set; }

    public UnaryOperator(Context context)
      : base(context)
    {
    }

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override PrimitiveType FindPrimitiveType()
    {
      switch (this.OperatorToken)
      {
        case JSToken.RestSpread:
        case JSToken.FirstOperator:
        case JSToken.Void:
          return PrimitiveType.Other;
        case JSToken.TypeOf:
          return PrimitiveType.String;
        case JSToken.LogicalNot:
          return PrimitiveType.Boolean;
        default:
          return PrimitiveType.Number;
      }
    }

    public override OperatorPrecedence Precedence => OperatorPrecedence.Unary;

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.Operand);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Operand != oldNode)
        return false;
      this.Operand = newNode;
      return true;
    }

    public override bool IsEquivalentTo(AstNode otherNode) => otherNode is UnaryOperator unaryOperator && this.OperatorToken == unaryOperator.OperatorToken && this.Operand.IsEquivalentTo(unaryOperator.Operand);

    public override bool IsConstant => this.Operand.IfNotNull<AstNode, bool>((Func<AstNode, bool>) (o => o.IsConstant));

    public override string ToString() => OutputVisitor.OperatorString(this.OperatorToken) + (this.Operand == null ? "<null>" : this.Operand.ToString());
  }
}
