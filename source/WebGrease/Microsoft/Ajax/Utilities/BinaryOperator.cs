// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.BinaryOperator
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class BinaryOperator : Expression
  {
    private AstNode m_operand1;
    private AstNode m_operand2;

    public AstNode Operand1
    {
      get => this.m_operand1;
      set
      {
        this.m_operand1.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_operand1 = value;
        this.m_operand1.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public AstNode Operand2
    {
      get => this.m_operand2;
      set
      {
        this.m_operand2.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = n.Parent == this ? (AstNode) null : n.Parent));
        this.m_operand2 = value;
        this.m_operand2.IfNotNull<AstNode, AstNode>((Func<AstNode, AstNode>) (n => n.Parent = (AstNode) this));
      }
    }

    public JSToken OperatorToken { get; set; }

    public Context OperatorContext { get; set; }

    public override Context TerminatingContext => base.TerminatingContext ?? this.Operand2.IfNotNull<AstNode, Context>((Func<AstNode, Context>) (n => n.TerminatingContext));

    public BinaryOperator(Context context)
      : base(context)
    {
    }

    public override OperatorPrecedence Precedence
    {
      get
      {
        switch (this.OperatorToken)
        {
          case JSToken.FirstBinaryOperator:
          case JSToken.Minus:
            return OperatorPrecedence.Additive;
          case JSToken.Multiply:
          case JSToken.Divide:
          case JSToken.Modulo:
            return OperatorPrecedence.Multiplicative;
          case JSToken.BitwiseAnd:
            return OperatorPrecedence.BitwiseAnd;
          case JSToken.BitwiseOr:
            return OperatorPrecedence.BitwiseOr;
          case JSToken.BitwiseXor:
            return OperatorPrecedence.BitwiseXor;
          case JSToken.LeftShift:
          case JSToken.RightShift:
          case JSToken.UnsignedRightShift:
            return OperatorPrecedence.Shift;
          case JSToken.Equal:
          case JSToken.NotEqual:
          case JSToken.StrictEqual:
          case JSToken.StrictNotEqual:
            return OperatorPrecedence.Equality;
          case JSToken.LessThan:
          case JSToken.LessThanEqual:
          case JSToken.GreaterThan:
          case JSToken.GreaterThanEqual:
          case JSToken.InstanceOf:
          case JSToken.In:
            return OperatorPrecedence.Relational;
          case JSToken.LogicalAnd:
            return OperatorPrecedence.LogicalAnd;
          case JSToken.LogicalOr:
            return OperatorPrecedence.LogicalOr;
          case JSToken.Comma:
            return OperatorPrecedence.Comma;
          case JSToken.Assign:
          case JSToken.PlusAssign:
          case JSToken.MinusAssign:
          case JSToken.MultiplyAssign:
          case JSToken.DivideAssign:
          case JSToken.ModuloAssign:
          case JSToken.BitwiseAndAssign:
          case JSToken.BitwiseOrAssign:
          case JSToken.BitwiseXorAssign:
          case JSToken.LeftShiftAssign:
          case JSToken.RightShiftAssign:
          case JSToken.UnsignedRightShiftAssign:
            return OperatorPrecedence.Assignment;
          default:
            return OperatorPrecedence.None;
        }
      }
    }

    public override PrimitiveType FindPrimitiveType()
    {
      switch (this.OperatorToken)
      {
        case JSToken.FirstBinaryOperator:
        case JSToken.PlusAssign:
          PrimitiveType primitiveType1 = this.Operand1.FindPrimitiveType();
          PrimitiveType primitiveType2 = this.Operand2.FindPrimitiveType();
          if (primitiveType1 == PrimitiveType.String || primitiveType2 == PrimitiveType.String)
            return PrimitiveType.String;
          return primitiveType1 == PrimitiveType.Other || primitiveType2 == PrimitiveType.Other ? PrimitiveType.Other : PrimitiveType.Number;
        case JSToken.Minus:
        case JSToken.Multiply:
        case JSToken.Divide:
        case JSToken.Modulo:
        case JSToken.BitwiseAnd:
        case JSToken.BitwiseOr:
        case JSToken.BitwiseXor:
        case JSToken.LeftShift:
        case JSToken.RightShift:
        case JSToken.UnsignedRightShift:
        case JSToken.MinusAssign:
        case JSToken.MultiplyAssign:
        case JSToken.DivideAssign:
        case JSToken.ModuloAssign:
        case JSToken.BitwiseAndAssign:
        case JSToken.BitwiseOrAssign:
        case JSToken.BitwiseXorAssign:
        case JSToken.LeftShiftAssign:
        case JSToken.RightShiftAssign:
        case JSToken.UnsignedRightShiftAssign:
          return PrimitiveType.Number;
        case JSToken.Equal:
        case JSToken.NotEqual:
        case JSToken.StrictEqual:
        case JSToken.StrictNotEqual:
        case JSToken.LessThan:
        case JSToken.LessThanEqual:
        case JSToken.GreaterThan:
        case JSToken.GreaterThanEqual:
        case JSToken.InstanceOf:
        case JSToken.In:
          return PrimitiveType.Boolean;
        case JSToken.LogicalAnd:
        case JSToken.LogicalOr:
          PrimitiveType primitiveType3 = this.Operand1.FindPrimitiveType();
          return primitiveType3 != PrimitiveType.Other && primitiveType3 == this.Operand2.FindPrimitiveType() ? primitiveType3 : PrimitiveType.Other;
        case JSToken.Comma:
        case JSToken.Assign:
          return this.Operand2.FindPrimitiveType();
        default:
          return PrimitiveType.Other;
      }
    }

    public override IEnumerable<AstNode> Children => AstNode.EnumerateNonNullNodes(this.Operand1, this.Operand2);

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
    {
      if (this.Operand1 == oldNode)
      {
        this.Operand1 = newNode;
        return true;
      }
      if (this.Operand2 != oldNode)
        return false;
      this.Operand2 = newNode;
      return true;
    }

    public override AstNode LeftHandSide
    {
      get
      {
        if (this.OperatorToken != JSToken.Comma)
          return this.Operand1.LeftHandSide;
        return this.Operand2 is AstNodeList operand2 && operand2.Count > 0 ? operand2[operand2.Count - 1].LeftHandSide : this.Operand2.LeftHandSide;
      }
    }

    public void SwapOperands()
    {
      AstNode operand1 = this.m_operand1;
      this.m_operand1 = this.m_operand2;
      this.m_operand2 = operand1;
    }

    public override bool IsEquivalentTo(AstNode otherNode) => otherNode is BinaryOperator binaryOperator && this.OperatorToken == binaryOperator.OperatorToken && this.Operand1.IsEquivalentTo(binaryOperator.Operand1) && this.Operand2.IsEquivalentTo(binaryOperator.Operand2);

    public bool IsAssign
    {
      get
      {
        switch (this.OperatorToken)
        {
          case JSToken.Assign:
          case JSToken.PlusAssign:
          case JSToken.MinusAssign:
          case JSToken.MultiplyAssign:
          case JSToken.DivideAssign:
          case JSToken.ModuloAssign:
          case JSToken.BitwiseAndAssign:
          case JSToken.BitwiseOrAssign:
          case JSToken.BitwiseXorAssign:
          case JSToken.LeftShiftAssign:
          case JSToken.RightShiftAssign:
          case JSToken.UnsignedRightShiftAssign:
            return true;
          default:
            return false;
        }
      }
    }

    internal override string GetFunctionGuess(AstNode target)
    {
      if (this.Operand2 != target)
        return string.Empty;
      return !this.IsAssign ? this.Parent.GetFunctionGuess((AstNode) this) : this.Operand1.GetFunctionGuess((AstNode) this);
    }

    public override bool ContainsInOperator => this.OperatorToken == JSToken.In || this.Operand1.ContainsInOperator || this.Operand2.ContainsInOperator;

    public override bool IsConstant => this.Operand1.IfNotNull<AstNode, bool>((Func<AstNode, bool>) (o => o.IsConstant)) && this.Operand2.IfNotNull<AstNode, bool>((Func<AstNode, bool>) (o => o.IsConstant));

    public override string ToString() => (this.Operand1 == null ? (object) "<null>" : (object) this.Operand1.ToString()).ToString() + (object) ' ' + OutputVisitor.OperatorString(this.OperatorToken) + (object) ' ' + (this.Operand2 == null ? (object) "<null>" : (object) this.Operand2.ToString());
  }
}
