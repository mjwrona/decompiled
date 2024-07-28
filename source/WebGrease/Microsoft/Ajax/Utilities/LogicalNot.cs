// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.LogicalNot
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;

namespace Microsoft.Ajax.Utilities
{
  public class LogicalNot : TreeVisitor
  {
    private AstNode m_expression;
    private bool m_measure;
    private int m_delta;

    public bool MinifyBooleans { get; set; }

    public LogicalNot(AstNode node)
      : this(node, (CodeSettings) null)
    {
    }

    public LogicalNot(AstNode node, CodeSettings codeSettings)
    {
      this.m_expression = node;
      this.MinifyBooleans = codeSettings.IfNotNull<CodeSettings, bool>((Func<CodeSettings, bool>) (settings => settings.MinifyCode && settings.IsModificationAllowed(TreeModifications.BooleanLiteralsToNotOperators)));
    }

    public int Measure()
    {
      this.m_measure = true;
      this.m_delta = 0;
      this.m_expression.Accept((IVisitor) this);
      return this.m_delta;
    }

    public void Apply()
    {
      this.m_measure = false;
      this.m_expression.Accept((IVisitor) this);
    }

    public static void Apply(AstNode node, CodeSettings codeSettings) => new LogicalNot(node, codeSettings).Apply();

    private static void WrapWithLogicalNot(AstNode operand) => operand.Parent.ReplaceChild(operand, (AstNode) new UnaryOperator(operand.Context)
    {
      Operand = operand,
      OperatorToken = JSToken.LogicalNot
    });

    private void TypicalHandler(AstNode node)
    {
      if (node == null)
        return;
      if (this.m_measure)
        ++this.m_delta;
      else
        LogicalNot.WrapWithLogicalNot(node);
    }

    public override void Visit(AstNodeList node)
    {
      if (node == null || node.Count <= 0)
        return;
      node[node.Count - 1].Accept((IVisitor) this);
    }

    public override void Visit(ArrayLiteral node) => this.TypicalHandler((AstNode) node);

    public override void Visit(BinaryOperator node)
    {
      if (node == null)
        return;
      if (this.m_measure)
        this.MeasureBinaryOperator(node);
      else
        this.ConvertBinaryOperator(node);
    }

    private void MeasureBinaryOperator(BinaryOperator node)
    {
      switch (node.OperatorToken)
      {
        case JSToken.FirstBinaryOperator:
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
        case JSToken.LessThan:
        case JSToken.LessThanEqual:
        case JSToken.GreaterThan:
        case JSToken.GreaterThanEqual:
        case JSToken.InstanceOf:
        case JSToken.In:
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
          this.m_delta += 3;
          break;
        case JSToken.LogicalAnd:
        case JSToken.LogicalOr:
          if (node.Parent is Block || node.Parent is CommaOperator && node.Parent.Parent is Block)
          {
            if (node.Operand1 == null)
              break;
            node.Operand1.Accept((IVisitor) this);
            break;
          }
          if (node.Operand1 != null)
            node.Operand1.Accept((IVisitor) this);
          if (node.Operand2 == null)
            break;
          node.Operand2.Accept((IVisitor) this);
          break;
        case JSToken.Comma:
          node.Operand2.Accept((IVisitor) this);
          break;
      }
    }

    private void ConvertBinaryOperator(BinaryOperator node)
    {
      switch (node.OperatorToken)
      {
        case JSToken.FirstBinaryOperator:
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
        case JSToken.LessThan:
        case JSToken.LessThanEqual:
        case JSToken.GreaterThan:
        case JSToken.GreaterThanEqual:
        case JSToken.InstanceOf:
        case JSToken.In:
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
          LogicalNot.WrapWithLogicalNot((AstNode) node);
          break;
        case JSToken.Equal:
          node.OperatorToken = JSToken.NotEqual;
          break;
        case JSToken.NotEqual:
          node.OperatorToken = JSToken.Equal;
          break;
        case JSToken.StrictEqual:
          node.OperatorToken = JSToken.StrictNotEqual;
          break;
        case JSToken.StrictNotEqual:
          node.OperatorToken = JSToken.StrictEqual;
          break;
        case JSToken.LogicalAnd:
        case JSToken.LogicalOr:
          if (node.Parent is Block || node.Parent is CommaOperator && node.Parent.Parent is Block)
          {
            if (node.Operand1 != null)
              node.Operand1.Accept((IVisitor) this);
          }
          else
          {
            if (node.Operand1 != null)
              node.Operand1.Accept((IVisitor) this);
            if (node.Operand2 != null)
              node.Operand2.Accept((IVisitor) this);
          }
          node.OperatorToken = node.OperatorToken == JSToken.LogicalAnd ? JSToken.LogicalOr : JSToken.LogicalAnd;
          break;
        case JSToken.Comma:
          node.Operand2.Accept((IVisitor) this);
          break;
      }
    }

    public override void Visit(CallNode node) => this.TypicalHandler((AstNode) node);

    public override void Visit(Conditional node)
    {
      if (node == null)
        return;
      LogicalNot logicalNot1 = new LogicalNot(node.TrueExpression)
      {
        MinifyBooleans = this.MinifyBooleans
      };
      LogicalNot logicalNot2 = new LogicalNot(node.FalseExpression)
      {
        MinifyBooleans = this.MinifyBooleans
      };
      int num = logicalNot1.Measure() + logicalNot2.Measure();
      if (this.m_measure)
        this.m_delta += num > 3 ? 3 : num;
      else if (num > 3)
      {
        LogicalNot.WrapWithLogicalNot((AstNode) node);
      }
      else
      {
        node.TrueExpression.Accept((IVisitor) this);
        node.FalseExpression.Accept((IVisitor) this);
      }
    }

    public override void Visit(ConstantWrapper node)
    {
      if (node == null)
        return;
      if (node.PrimitiveType == PrimitiveType.Boolean)
      {
        if (this.m_measure)
        {
          if (this.MinifyBooleans)
            return;
          this.m_delta += node.ToBoolean() ? 1 : -1;
        }
        else
          node.Value = (object) !node.ToBoolean();
      }
      else
        this.TypicalHandler((AstNode) node);
    }

    public override void Visit(GroupingOperator node)
    {
      if (node == null)
        return;
      if (this.m_measure)
      {
        int num = this.m_delta + 1;
        node.Operand.Accept((IVisitor) this);
        if (this.m_delta <= num)
          return;
        this.m_delta = num;
      }
      else
      {
        this.m_measure = true;
        this.m_delta = 0;
        node.Operand.Accept((IVisitor) this);
        this.m_measure = false;
        if (this.m_delta > 1)
        {
          LogicalNot.WrapWithLogicalNot((AstNode) node);
        }
        else
        {
          node.Parent.ReplaceChild((AstNode) node, node.Operand);
          node.Operand.Accept((IVisitor) this);
        }
      }
    }

    public override void Visit(Lookup node) => this.TypicalHandler((AstNode) node);

    public override void Visit(Member node) => this.TypicalHandler((AstNode) node);

    public override void Visit(ObjectLiteral node) => this.TypicalHandler((AstNode) node);

    public override void Visit(RegExpLiteral node) => this.TypicalHandler((AstNode) node);

    public override void Visit(ThisLiteral node) => this.TypicalHandler((AstNode) node);

    public override void Visit(UnaryOperator node)
    {
      if (node == null || node.OperatorInConditionalCompilationComment)
        return;
      if (node.OperatorToken == JSToken.LogicalNot)
      {
        if (this.m_measure)
        {
          --this.m_delta;
          if (!(node.Operand is BinaryOperator) && !(node.Operand is Conditional) && !(node.Operand is GroupingOperator))
            return;
          this.m_delta -= 2;
        }
        else if (node.Operand is GroupingOperator operand)
          node.Parent.ReplaceChild((AstNode) node, operand.Operand);
        else
          node.Parent.ReplaceChild((AstNode) node, node.Operand);
      }
      else
        this.TypicalHandler((AstNode) node);
    }
  }
}
