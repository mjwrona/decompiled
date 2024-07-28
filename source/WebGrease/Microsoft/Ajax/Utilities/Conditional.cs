// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.Conditional
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public sealed class Conditional : Expression
  {
    private AstNode m_condition;
    private AstNode m_trueExpression;
    private AstNode m_falseExpression;

    public AstNode Condition
    {
      get => this.m_condition;
      set
      {
        this.m_condition.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_condition = value;
        this.m_condition.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public AstNode TrueExpression
    {
      get => this.m_trueExpression;
      set
      {
        this.m_trueExpression.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_trueExpression = value;
        this.m_trueExpression.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public AstNode FalseExpression
    {
      get => this.m_falseExpression;
      set
      {
        this.m_falseExpression.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_falseExpression = value;
        this.m_falseExpression.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public Context QuestionContext { get; set; }

    public Context ColonContext { get; set; }

    public Conditional(Context context)
      : base(context)
    {
    }

    public override OperatorPrecedence Precedence => OperatorPrecedence.Conditional;

    public void SwapBranches()
    {
      AstNode trueExpression = this.m_trueExpression;
      this.m_trueExpression = this.m_falseExpression;
      this.m_falseExpression = trueExpression;
    }

    public override PrimitiveType FindPrimitiveType()
    {
      if (this.TrueExpression != null && this.FalseExpression != null)
      {
        PrimitiveType primitiveType = this.TrueExpression.FindPrimitiveType();
        if (primitiveType == this.FalseExpression.FindPrimitiveType())
          return primitiveType;
      }
      return PrimitiveType.Other;
    }

    public override bool IsEquivalentTo(AstNode otherNode) => otherNode is Conditional conditional && this.Condition.IsEquivalentTo(conditional.Condition) && this.TrueExpression.IsEquivalentTo(conditional.TrueExpression) && this.FalseExpression.IsEquivalentTo(conditional.FalseExpression);

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.Condition, this.TrueExpression, this.FalseExpression);

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Condition == oldNode)
      {
        this.Condition = newNode;
        return true;
      }
      if (this.TrueExpression == oldNode)
      {
        this.TrueExpression = newNode;
        return true;
      }
      if (this.FalseExpression != oldNode)
        return false;
      this.FalseExpression = newNode;
      return true;
    }

    public override AstNode LeftHandSide => this.Condition.LeftHandSide;
  }
}
