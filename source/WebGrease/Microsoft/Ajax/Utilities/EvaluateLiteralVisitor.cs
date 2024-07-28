// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.EvaluateLiteralVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Text;

namespace Microsoft.Ajax.Utilities
{
  internal class EvaluateLiteralVisitor : TreeVisitor
  {
    private JSParser m_parser;

    public EvaluateLiteralVisitor(JSParser parser) => this.m_parser = parser;

    private bool ReplaceMemberBracketWithDot(BinaryOperator node, ConstantWrapper newLiteral)
    {
      if (newLiteral.IsStringLiteral)
      {
        CallNode parent = node.Parent is AstNodeList ? node.Parent.Parent as CallNode : (CallNode) null;
        if (parent != null && parent.InBrackets)
        {
          string str = newLiteral.ToString();
          string newName;
          if (this.m_parser.Settings.HasRenamePairs && this.m_parser.Settings.ManualRenamesProperties && this.m_parser.Settings.IsModificationAllowed(TreeModifications.PropertyRenaming) && !string.IsNullOrEmpty(newName = this.m_parser.Settings.GetNewName(str)))
          {
            if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.BracketMemberToDotMember) && JSScanner.IsSafeIdentifier(newName) && !JSScanner.IsKeyword(newName, (parent.EnclosingScope ?? (ActivationObject) this.m_parser.GlobalScope).UseStrict))
            {
              Member newNode = new Member(parent.Context)
              {
                Root = parent.Function,
                Name = newName,
                NameContext = parent.Arguments[0].Context
              };
              parent.Parent.ReplaceChild((AstNode) parent, (AstNode) newNode);
              return true;
            }
            newLiteral.Value = (object) newName;
            newLiteral.PrimitiveType = PrimitiveType.String;
          }
          else if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.BracketMemberToDotMember) && JSScanner.IsSafeIdentifier(str) && !JSScanner.IsKeyword(str, (parent.EnclosingScope ?? (ActivationObject) this.m_parser.GlobalScope).UseStrict))
          {
            Member newNode = new Member(parent.Context)
            {
              Root = parent.Function,
              Name = str,
              NameContext = parent.Arguments[0].Context
            };
            parent.Parent.ReplaceChild((AstNode) parent, (AstNode) newNode);
            return true;
          }
        }
      }
      return false;
    }

    private static void ReplaceNodeWithLiteral(AstNode node, ConstantWrapper newLiteral)
    {
      if (node.Parent is GroupingOperator parent)
        parent.Parent.ReplaceChild((AstNode) parent, (AstNode) newLiteral);
      else
        node.Parent.ReplaceChild(node, (AstNode) newLiteral);
    }

    private static void ReplaceNodeCheckParens(AstNode oldNode, AstNode newNode)
    {
      if (oldNode.Parent is GroupingOperator parent1)
      {
        if (newNode != null)
        {
          OperatorPrecedence operatorPrecedence = parent1.Parent.Precedence;
          if (parent1.Parent is Conditional parent)
            operatorPrecedence = parent.Condition == parent1 ? OperatorPrecedence.LogicalOr : OperatorPrecedence.Assignment;
          if (newNode.Precedence >= operatorPrecedence)
            parent1.Parent.ReplaceChild((AstNode) parent1, newNode);
          else
            oldNode.Parent.ReplaceChild(oldNode, newNode);
        }
        else
          parent1.Parent.ReplaceChild((AstNode) parent1, (AstNode) null);
      }
      else
        oldNode.Parent.ReplaceChild(oldNode, newNode);
    }

    private void EvalThisOperator(BinaryOperator node, ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      switch (node.OperatorToken)
      {
        case JSToken.FirstBinaryOperator:
          constantWrapper = this.Plus(left, right);
          break;
        case JSToken.Minus:
          constantWrapper = this.Minus(left, right);
          break;
        case JSToken.Multiply:
          constantWrapper = this.Multiply(left, right);
          break;
        case JSToken.Divide:
          constantWrapper = this.Divide(left, right);
          if (constantWrapper != null && this.NodeLength((AstNode) constantWrapper) > this.NodeLength((AstNode) node))
          {
            constantWrapper = (ConstantWrapper) null;
            break;
          }
          break;
        case JSToken.Modulo:
          constantWrapper = this.Modulo(left, right);
          if (constantWrapper != null && this.NodeLength((AstNode) constantWrapper) > this.NodeLength((AstNode) node))
          {
            constantWrapper = (ConstantWrapper) null;
            break;
          }
          break;
        case JSToken.BitwiseAnd:
          constantWrapper = this.BitwiseAnd(left, right);
          break;
        case JSToken.BitwiseOr:
          constantWrapper = this.BitwiseOr(left, right);
          break;
        case JSToken.BitwiseXor:
          constantWrapper = this.BitwiseXor(left, right);
          break;
        case JSToken.LeftShift:
          constantWrapper = this.LeftShift(left, right);
          break;
        case JSToken.RightShift:
          constantWrapper = this.RightShift(left, right);
          break;
        case JSToken.UnsignedRightShift:
          constantWrapper = this.UnsignedRightShift(left, right);
          break;
        case JSToken.Equal:
          constantWrapper = this.Equal(left, right);
          break;
        case JSToken.NotEqual:
          constantWrapper = this.NotEqual(left, right);
          break;
        case JSToken.StrictEqual:
          constantWrapper = this.StrictEqual(left, right);
          break;
        case JSToken.StrictNotEqual:
          constantWrapper = this.StrictNotEqual(left, right);
          break;
        case JSToken.LessThan:
          constantWrapper = this.LessThan(left, right);
          break;
        case JSToken.LessThanEqual:
          constantWrapper = this.LessThanOrEqual(left, right);
          break;
        case JSToken.GreaterThan:
          constantWrapper = this.GreaterThan(left, right);
          break;
        case JSToken.GreaterThanEqual:
          constantWrapper = this.GreaterThanOrEqual(left, right);
          break;
        case JSToken.LogicalAnd:
          constantWrapper = this.LogicalAnd(left, right);
          break;
        case JSToken.LogicalOr:
          constantWrapper = this.LogicalOr(left, right);
          break;
      }
      if (constantWrapper == null || this.ReplaceMemberBracketWithDot(node, constantWrapper))
        return;
      EvaluateLiteralVisitor.ReplaceNodeWithLiteral((AstNode) node, constantWrapper);
    }

    private void RotateFromLeft(
      BinaryOperator node,
      BinaryOperator binaryOp,
      ConstantWrapper newLiteral)
    {
      binaryOp.Operand2 = (AstNode) newLiteral;
      node.Parent.ReplaceChild((AstNode) node, (AstNode) binaryOp);
      if (!(binaryOp.Operand1 is ConstantWrapper operand1))
        return;
      this.EvalThisOperator(binaryOp, operand1, newLiteral);
    }

    private void RotateFromRight(
      BinaryOperator node,
      BinaryOperator binaryOp,
      ConstantWrapper newLiteral)
    {
      binaryOp.Operand1 = (AstNode) newLiteral;
      node.Parent.ReplaceChild((AstNode) node, (AstNode) binaryOp);
      if (!(binaryOp.Operand2 is ConstantWrapper operand2))
        return;
      this.EvalThisOperator(binaryOp, newLiteral, operand2);
    }

    private static bool NoMultiplicativeOverOrUnderFlow(
      ConstantWrapper left,
      ConstantWrapper right,
      ConstantWrapper result)
    {
      bool flag = !result.IsInfinity;
      if (flag)
        flag = !result.IsZero || left.IsZero || right.IsZero;
      return flag;
    }

    private static bool NoOverflow(ConstantWrapper result) => !result.IsInfinity;

    private void EvalToTheLeft(
      BinaryOperator node,
      ConstantWrapper thisConstant,
      ConstantWrapper otherConstant,
      BinaryOperator leftOperator)
    {
      if (leftOperator.OperatorToken == JSToken.FirstBinaryOperator && node.OperatorToken == JSToken.FirstBinaryOperator)
      {
        if (!otherConstant.IsStringLiteral)
          return;
        ConstantWrapper newLiteral = this.StringConcat(otherConstant, thisConstant);
        if (newLiteral == null)
          return;
        this.RotateFromLeft(node, leftOperator, newLiteral);
      }
      else if (leftOperator.OperatorToken == JSToken.Minus)
      {
        if (node.OperatorToken == JSToken.FirstBinaryOperator)
        {
          if (thisConstant.IsStringLiteral)
            return;
          ConstantWrapper constantWrapper = this.Minus(otherConstant, thisConstant);
          if (constantWrapper != null && EvaluateLiteralVisitor.NoOverflow(constantWrapper))
          {
            this.RotateFromLeft(node, leftOperator, constantWrapper);
          }
          else
          {
            if (!(leftOperator.Operand1 is ConstantWrapper operand1))
              return;
            this.EvalFarToTheLeft(node, thisConstant, operand1, leftOperator);
          }
        }
        else
        {
          if (node.OperatorToken != JSToken.Minus)
            return;
          ConstantWrapper constantWrapper = this.NumericAddition(otherConstant, thisConstant);
          if (constantWrapper != null && EvaluateLiteralVisitor.NoOverflow(constantWrapper))
          {
            this.RotateFromLeft(node, leftOperator, constantWrapper);
          }
          else
          {
            if (!(leftOperator.Operand1 is ConstantWrapper operand1))
              return;
            this.EvalFarToTheLeft(node, thisConstant, operand1, leftOperator);
          }
        }
      }
      else if (leftOperator.OperatorToken == node.OperatorToken && (node.OperatorToken == JSToken.Multiply || node.OperatorToken == JSToken.Divide))
      {
        ConstantWrapper constantWrapper = this.Multiply(otherConstant, thisConstant);
        if (constantWrapper == null || !EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(otherConstant, thisConstant, constantWrapper))
          return;
        this.RotateFromLeft(node, leftOperator, constantWrapper);
      }
      else if (leftOperator.OperatorToken == JSToken.Multiply && node.OperatorToken == JSToken.Divide || leftOperator.OperatorToken == JSToken.Divide && node.OperatorToken == JSToken.Multiply)
      {
        if (!this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
          return;
        ConstantWrapper constantWrapper1 = this.Divide(otherConstant, thisConstant);
        ConstantWrapper constantWrapper2 = this.Divide(thisConstant, otherConstant);
        int num1 = constantWrapper1 != null ? this.NodeLength((AstNode) constantWrapper1) : int.MaxValue;
        int num2 = constantWrapper2 != null ? this.NodeLength((AstNode) constantWrapper2) : int.MaxValue;
        if (constantWrapper1 != null && EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(otherConstant, thisConstant, constantWrapper1) && (constantWrapper2 == null || num1 < num2))
        {
          if (num1 > this.NodeLength((AstNode) otherConstant) + this.NodeLength((AstNode) thisConstant) + 1)
            return;
          this.RotateFromLeft(node, leftOperator, constantWrapper1);
        }
        else
        {
          if (constantWrapper2 == null || !EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(thisConstant, otherConstant, constantWrapper2) || num2 > this.NodeLength((AstNode) otherConstant) + this.NodeLength((AstNode) thisConstant) + 1)
            return;
          leftOperator.OperatorToken = leftOperator.OperatorToken == JSToken.Multiply ? JSToken.Divide : JSToken.Multiply;
          this.RotateFromLeft(node, leftOperator, constantWrapper2);
        }
      }
      else
      {
        if (node.OperatorToken != leftOperator.OperatorToken || node.OperatorToken != JSToken.BitwiseAnd && node.OperatorToken != JSToken.BitwiseOr && node.OperatorToken != JSToken.BitwiseXor)
          return;
        ConstantWrapper newLiteral = (ConstantWrapper) null;
        switch (node.OperatorToken)
        {
          case JSToken.BitwiseAnd:
            newLiteral = this.BitwiseAnd(otherConstant, thisConstant);
            break;
          case JSToken.BitwiseOr:
            newLiteral = this.BitwiseOr(otherConstant, thisConstant);
            break;
          case JSToken.BitwiseXor:
            newLiteral = this.BitwiseXor(otherConstant, thisConstant);
            break;
        }
        if (newLiteral == null)
          return;
        this.RotateFromLeft(node, leftOperator, newLiteral);
      }
    }

    private void EvalFarToTheLeft(
      BinaryOperator node,
      ConstantWrapper thisConstant,
      ConstantWrapper otherConstant,
      BinaryOperator leftOperator)
    {
      if (leftOperator.OperatorToken == JSToken.Minus)
      {
        if (node.OperatorToken == JSToken.FirstBinaryOperator)
        {
          if (thisConstant.PrimitiveType == PrimitiveType.String || thisConstant.PrimitiveType == PrimitiveType.Other)
            return;
          ConstantWrapper constantWrapper = this.NumericAddition(otherConstant, thisConstant);
          if (constantWrapper == null || !EvaluateLiteralVisitor.NoOverflow(constantWrapper))
            return;
          this.RotateFromRight(node, leftOperator, constantWrapper);
        }
        else
        {
          if (node.OperatorToken != JSToken.Minus)
            return;
          ConstantWrapper constantWrapper = this.Minus(otherConstant, thisConstant);
          if (constantWrapper == null || !EvaluateLiteralVisitor.NoOverflow(constantWrapper))
            return;
          this.RotateFromRight(node, leftOperator, constantWrapper);
        }
      }
      else if (node.OperatorToken == JSToken.Multiply)
      {
        if (leftOperator.OperatorToken != JSToken.Multiply && leftOperator.OperatorToken != JSToken.Divide)
          return;
        ConstantWrapper constantWrapper = this.Multiply(otherConstant, thisConstant);
        if (constantWrapper == null || !EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(otherConstant, thisConstant, constantWrapper))
          return;
        this.RotateFromRight(node, leftOperator, constantWrapper);
      }
      else
      {
        if (node.OperatorToken != JSToken.Divide)
          return;
        if (leftOperator.OperatorToken == JSToken.Divide)
        {
          ConstantWrapper constantWrapper = this.Divide(otherConstant, thisConstant);
          if (constantWrapper == null || !EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(otherConstant, thisConstant, constantWrapper) || this.NodeLength((AstNode) constantWrapper) > this.NodeLength((AstNode) thisConstant) + this.NodeLength((AstNode) otherConstant) + 1)
            return;
          this.RotateFromRight(node, leftOperator, constantWrapper);
        }
        else
        {
          if (leftOperator.OperatorToken != JSToken.Multiply)
            return;
          ConstantWrapper constantWrapper1 = this.Divide(otherConstant, thisConstant);
          ConstantWrapper constantWrapper2 = this.Divide(thisConstant, otherConstant);
          int num1 = constantWrapper1 != null ? this.NodeLength((AstNode) constantWrapper1) : int.MaxValue;
          int num2 = constantWrapper2 != null ? this.NodeLength((AstNode) constantWrapper2) : int.MaxValue;
          if (constantWrapper1 != null && EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(otherConstant, thisConstant, constantWrapper1) && (constantWrapper2 == null || num1 < num2))
          {
            if (num1 > this.NodeLength((AstNode) thisConstant) + this.NodeLength((AstNode) otherConstant) + 1)
              return;
            this.RotateFromRight(node, leftOperator, constantWrapper1);
          }
          else
          {
            if (constantWrapper2 == null || !EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(thisConstant, otherConstant, constantWrapper2) || num2 > this.NodeLength((AstNode) thisConstant) + this.NodeLength((AstNode) otherConstant) + 1)
              return;
            leftOperator.SwapOperands();
            leftOperator.OperatorToken = JSToken.Divide;
            this.RotateFromLeft(node, leftOperator, constantWrapper2);
          }
        }
      }
    }

    private void EvalToTheRight(
      BinaryOperator node,
      ConstantWrapper thisConstant,
      ConstantWrapper otherConstant,
      BinaryOperator rightOperator)
    {
      if (node.OperatorToken == JSToken.FirstBinaryOperator)
      {
        if (rightOperator.OperatorToken == JSToken.FirstBinaryOperator && otherConstant.IsStringLiteral)
        {
          ConstantWrapper newLiteral = this.StringConcat(thisConstant, otherConstant);
          if (newLiteral == null)
            return;
          this.RotateFromRight(node, rightOperator, newLiteral);
        }
        else
        {
          if (rightOperator.OperatorToken != JSToken.Minus || thisConstant.IsStringLiteral)
            return;
          ConstantWrapper constantWrapper = this.NumericAddition(thisConstant, otherConstant);
          if (constantWrapper != null && EvaluateLiteralVisitor.NoOverflow(constantWrapper))
          {
            this.RotateFromRight(node, rightOperator, constantWrapper);
          }
          else
          {
            if (!(rightOperator.Operand2 is ConstantWrapper operand2))
              return;
            this.EvalFarToTheRight(node, thisConstant, operand2, rightOperator);
          }
        }
      }
      else if (node.OperatorToken == JSToken.Minus && rightOperator.OperatorToken == JSToken.Minus)
      {
        ConstantWrapper constantWrapper = this.Minus(otherConstant, thisConstant);
        if (constantWrapper != null && EvaluateLiteralVisitor.NoOverflow(constantWrapper))
        {
          rightOperator.SwapOperands();
          this.RotateFromLeft(node, rightOperator, constantWrapper);
        }
        else
        {
          if (!(rightOperator.Operand2 is ConstantWrapper operand2))
            return;
          this.EvalFarToTheRight(node, thisConstant, operand2, rightOperator);
        }
      }
      else if (node.OperatorToken == JSToken.Multiply && (rightOperator.OperatorToken == JSToken.Multiply || rightOperator.OperatorToken == JSToken.Divide))
      {
        ConstantWrapper constantWrapper = this.Multiply(thisConstant, otherConstant);
        if (constantWrapper == null || !EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(thisConstant, otherConstant, constantWrapper))
          return;
        this.RotateFromRight(node, rightOperator, constantWrapper);
      }
      else
      {
        if (node.OperatorToken != JSToken.Divide)
          return;
        if (rightOperator.OperatorToken == JSToken.Multiply)
        {
          ConstantWrapper constantWrapper = this.Divide(thisConstant, otherConstant);
          if (constantWrapper == null || !EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(thisConstant, otherConstant, constantWrapper) || this.NodeLength((AstNode) constantWrapper) >= this.NodeLength((AstNode) thisConstant) + this.NodeLength((AstNode) otherConstant) + 1)
            return;
          rightOperator.OperatorToken = JSToken.Divide;
          this.RotateFromRight(node, rightOperator, constantWrapper);
        }
        else
        {
          if (rightOperator.OperatorToken != JSToken.Divide)
            return;
          ConstantWrapper constantWrapper1 = this.Divide(thisConstant, otherConstant);
          ConstantWrapper constantWrapper2 = this.Divide(otherConstant, thisConstant);
          int num1 = constantWrapper1 != null ? this.NodeLength((AstNode) constantWrapper1) : int.MaxValue;
          int num2 = constantWrapper2 != null ? this.NodeLength((AstNode) constantWrapper2) : int.MaxValue;
          if (constantWrapper1 != null && EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(thisConstant, otherConstant, constantWrapper1) && (constantWrapper2 == null || num1 < num2))
          {
            if (num1 > this.NodeLength((AstNode) thisConstant) + this.NodeLength((AstNode) otherConstant) + 1)
              return;
            rightOperator.OperatorToken = JSToken.Multiply;
            this.RotateFromRight(node, rightOperator, constantWrapper1);
          }
          else
          {
            if (constantWrapper2 == null || !EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(otherConstant, thisConstant, constantWrapper2) || num2 > this.NodeLength((AstNode) thisConstant) + this.NodeLength((AstNode) otherConstant) + 1)
              return;
            rightOperator.SwapOperands();
            this.RotateFromLeft(node, rightOperator, constantWrapper2);
          }
        }
      }
    }

    private void EvalFarToTheRight(
      BinaryOperator node,
      ConstantWrapper thisConstant,
      ConstantWrapper otherConstant,
      BinaryOperator rightOperator)
    {
      if (rightOperator.OperatorToken == JSToken.Minus)
      {
        if (node.OperatorToken == JSToken.FirstBinaryOperator)
        {
          if (thisConstant.IsStringLiteral)
            return;
          ConstantWrapper constantWrapper = this.Minus(otherConstant, thisConstant);
          if (constantWrapper == null || !EvaluateLiteralVisitor.NoOverflow(constantWrapper))
            return;
          this.RotateFromLeft(node, rightOperator, constantWrapper);
        }
        else
        {
          if (node.OperatorToken != JSToken.Minus)
            return;
          ConstantWrapper constantWrapper = this.NumericAddition(thisConstant, otherConstant);
          if (constantWrapper == null || !EvaluateLiteralVisitor.NoOverflow(constantWrapper))
            return;
          rightOperator.SwapOperands();
          this.RotateFromRight(node, rightOperator, constantWrapper);
        }
      }
      else if (node.OperatorToken == JSToken.Multiply)
      {
        if (rightOperator.OperatorToken == JSToken.Multiply)
        {
          ConstantWrapper constantWrapper = this.Multiply(thisConstant, otherConstant);
          if (constantWrapper == null || !EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(thisConstant, otherConstant, constantWrapper))
            return;
          this.RotateFromLeft(node, rightOperator, constantWrapper);
        }
        else
        {
          if (rightOperator.OperatorToken != JSToken.Divide)
            return;
          ConstantWrapper constantWrapper1 = this.Divide(otherConstant, thisConstant);
          ConstantWrapper constantWrapper2 = this.Divide(thisConstant, otherConstant);
          int num1 = constantWrapper1 != null ? this.NodeLength((AstNode) constantWrapper1) : int.MaxValue;
          int num2 = constantWrapper2 != null ? this.NodeLength((AstNode) constantWrapper2) : int.MaxValue;
          if (constantWrapper1 != null && EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(otherConstant, thisConstant, constantWrapper1) && (constantWrapper2 == null || num1 < num2))
          {
            if (num1 > this.NodeLength((AstNode) thisConstant) + this.NodeLength((AstNode) otherConstant) + 1)
              return;
            this.RotateFromLeft(node, rightOperator, constantWrapper1);
          }
          else
          {
            if (constantWrapper2 == null || !EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(thisConstant, otherConstant, constantWrapper2) || num2 > this.NodeLength((AstNode) thisConstant) + this.NodeLength((AstNode) otherConstant) + 1)
              return;
            rightOperator.SwapOperands();
            rightOperator.OperatorToken = JSToken.Multiply;
            this.RotateFromRight(node, rightOperator, constantWrapper2);
          }
        }
      }
      else
      {
        if (node.OperatorToken != JSToken.Divide)
          return;
        if (rightOperator.OperatorToken == JSToken.Multiply)
        {
          ConstantWrapper constantWrapper = this.Divide(thisConstant, otherConstant);
          if (constantWrapper == null || !EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(thisConstant, otherConstant, constantWrapper) || this.NodeLength((AstNode) constantWrapper) > this.NodeLength((AstNode) thisConstant) + this.NodeLength((AstNode) otherConstant) + 1)
            return;
          rightOperator.SwapOperands();
          rightOperator.OperatorToken = JSToken.Divide;
          this.RotateFromRight(node, rightOperator, constantWrapper);
        }
        else
        {
          if (rightOperator.OperatorToken != JSToken.Divide)
            return;
          ConstantWrapper constantWrapper = this.Multiply(thisConstant, otherConstant);
          if (constantWrapper == null || !EvaluateLiteralVisitor.NoMultiplicativeOverOrUnderFlow(thisConstant, otherConstant, constantWrapper))
            return;
          rightOperator.SwapOperands();
          this.RotateFromRight(node, rightOperator, constantWrapper);
        }
      }
    }

    private ConstantWrapper Multiply(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (left.IsOkayToCombine && right.IsOkayToCombine)
      {
        if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
        {
          try
          {
            double number1 = left.ToNumber();
            double number2 = right.ToNumber();
            double numericValue = number1 * number2;
            if (ConstantWrapper.NumberIsOkayToCombine(numericValue))
            {
              constantWrapper = new ConstantWrapper((object) numericValue, PrimitiveType.Number, left.Context.FlattenToStart());
            }
            else
            {
              if (!left.IsNumericLiteral && ConstantWrapper.NumberIsOkayToCombine(number1))
                left.Parent.ReplaceChild((AstNode) left, (AstNode) new ConstantWrapper((object) number1, PrimitiveType.Number, left.Context));
              if (!right.IsNumericLiteral)
              {
                if (ConstantWrapper.NumberIsOkayToCombine(number2))
                  right.Parent.ReplaceChild((AstNode) right, (AstNode) new ConstantWrapper((object) number2, PrimitiveType.Number, right.Context));
              }
            }
          }
          catch (InvalidCastException ex)
          {
          }
        }
      }
      return constantWrapper;
    }

    private ConstantWrapper Divide(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (left.IsOkayToCombine && right.IsOkayToCombine)
      {
        if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
        {
          try
          {
            double number1 = left.ToNumber();
            double number2 = right.ToNumber();
            double numericValue = number1 / number2;
            if (ConstantWrapper.NumberIsOkayToCombine(numericValue))
            {
              constantWrapper = new ConstantWrapper((object) numericValue, PrimitiveType.Number, left.Context.FlattenToStart());
            }
            else
            {
              if (!left.IsNumericLiteral && ConstantWrapper.NumberIsOkayToCombine(number1))
                left.Parent.ReplaceChild((AstNode) left, (AstNode) new ConstantWrapper((object) number1, PrimitiveType.Number, left.Context));
              if (!right.IsNumericLiteral)
              {
                if (ConstantWrapper.NumberIsOkayToCombine(number2))
                  right.Parent.ReplaceChild((AstNode) right, (AstNode) new ConstantWrapper((object) number2, PrimitiveType.Number, right.Context));
              }
            }
          }
          catch (InvalidCastException ex)
          {
          }
        }
      }
      return constantWrapper;
    }

    private ConstantWrapper Modulo(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (left.IsOkayToCombine && right.IsOkayToCombine)
      {
        if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
        {
          try
          {
            double number1 = left.ToNumber();
            double number2 = right.ToNumber();
            double numericValue = number1 % number2;
            if (ConstantWrapper.NumberIsOkayToCombine(numericValue))
            {
              constantWrapper = new ConstantWrapper((object) numericValue, PrimitiveType.Number, left.Context.FlattenToStart());
            }
            else
            {
              if (!left.IsNumericLiteral && ConstantWrapper.NumberIsOkayToCombine(number1))
                left.Parent.ReplaceChild((AstNode) left, (AstNode) new ConstantWrapper((object) number1, PrimitiveType.Number, left.Context));
              if (!right.IsNumericLiteral)
              {
                if (ConstantWrapper.NumberIsOkayToCombine(number2))
                  right.Parent.ReplaceChild((AstNode) right, (AstNode) new ConstantWrapper((object) number2, PrimitiveType.Number, right.Context));
              }
            }
          }
          catch (InvalidCastException ex)
          {
          }
        }
      }
      return constantWrapper;
    }

    private ConstantWrapper Plus(ConstantWrapper left, ConstantWrapper right) => left.IsStringLiteral || right.IsStringLiteral ? this.StringConcat(left, right) : this.NumericAddition(left, right);

    private ConstantWrapper NumericAddition(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (left.IsOkayToCombine && right.IsOkayToCombine)
      {
        if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
        {
          try
          {
            double number1 = left.ToNumber();
            double number2 = right.ToNumber();
            double numericValue = number1 + number2;
            if (ConstantWrapper.NumberIsOkayToCombine(numericValue))
            {
              constantWrapper = new ConstantWrapper((object) numericValue, PrimitiveType.Number, left.Context.FlattenToStart());
            }
            else
            {
              if (!left.IsNumericLiteral && ConstantWrapper.NumberIsOkayToCombine(number1))
                left.Parent.ReplaceChild((AstNode) left, (AstNode) new ConstantWrapper((object) number1, PrimitiveType.Number, left.Context));
              if (!right.IsNumericLiteral)
              {
                if (ConstantWrapper.NumberIsOkayToCombine(number2))
                  right.Parent.ReplaceChild((AstNode) right, (AstNode) new ConstantWrapper((object) number2, PrimitiveType.Number, right.Context));
              }
            }
          }
          catch (InvalidCastException ex)
          {
          }
        }
      }
      return constantWrapper;
    }

    private ConstantWrapper StringConcat(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.CombineAdjacentStringLiterals) && (left.IsStringLiteral && right.IsStringLiteral || this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions)) && left.IsOkayToCombine && right.IsOkayToCombine)
        constantWrapper = new ConstantWrapper((object) (left.ToString() + right.ToString()), PrimitiveType.String, left.Context.FlattenToStart());
      return constantWrapper;
    }

    private ConstantWrapper Minus(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (left.IsOkayToCombine && right.IsOkayToCombine)
      {
        if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
        {
          try
          {
            double number1 = left.ToNumber();
            double number2 = right.ToNumber();
            double numericValue = number1 - number2;
            if (ConstantWrapper.NumberIsOkayToCombine(numericValue))
            {
              constantWrapper = new ConstantWrapper((object) numericValue, PrimitiveType.Number, left.Context.FlattenToStart());
            }
            else
            {
              if (!left.IsNumericLiteral && ConstantWrapper.NumberIsOkayToCombine(number1))
                left.Parent.ReplaceChild((AstNode) left, (AstNode) new ConstantWrapper((object) number1, PrimitiveType.Number, left.Context));
              if (!right.IsNumericLiteral)
              {
                if (ConstantWrapper.NumberIsOkayToCombine(number2))
                  right.Parent.ReplaceChild((AstNode) right, (AstNode) new ConstantWrapper((object) number2, PrimitiveType.Number, right.Context));
              }
            }
          }
          catch (InvalidCastException ex)
          {
          }
        }
      }
      return constantWrapper;
    }

    private ConstantWrapper LeftShift(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        try
        {
          constantWrapper = new ConstantWrapper((object) Convert.ToDouble(left.ToInt32() << (int) right.ToUInt32()), PrimitiveType.Number, left.Context.FlattenToStart());
        }
        catch (InvalidCastException ex)
        {
        }
      }
      return constantWrapper;
    }

    private ConstantWrapper RightShift(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        try
        {
          constantWrapper = new ConstantWrapper((object) Convert.ToDouble(left.ToInt32() >> (int) right.ToUInt32()), PrimitiveType.Number, left.Context.FlattenToStart());
        }
        catch (InvalidCastException ex)
        {
        }
      }
      return constantWrapper;
    }

    private ConstantWrapper UnsignedRightShift(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        try
        {
          constantWrapper = new ConstantWrapper((object) Convert.ToDouble(left.ToUInt32() >> (int) right.ToUInt32()), PrimitiveType.Number, left.Context.FlattenToStart());
        }
        catch (InvalidCastException ex)
        {
        }
      }
      return constantWrapper;
    }

    private ConstantWrapper LessThan(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        if (left.IsStringLiteral)
        {
          if (right.IsStringLiteral)
          {
            if (left.IsOkayToCombine && right.IsOkayToCombine)
            {
              constantWrapper = new ConstantWrapper((object) (string.CompareOrdinal(left.ToString(), right.ToString()) < 0), PrimitiveType.Boolean, left.Context.FlattenToStart());
              goto label_9;
            }
            else
              goto label_9;
          }
        }
        try
        {
          if (left.IsOkayToCombine)
          {
            if (right.IsOkayToCombine)
              constantWrapper = new ConstantWrapper((object) (left.ToNumber() < right.ToNumber()), PrimitiveType.Boolean, left.Context.FlattenToStart());
          }
        }
        catch (InvalidCastException ex)
        {
        }
      }
label_9:
      return constantWrapper;
    }

    private ConstantWrapper LessThanOrEqual(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        if (left.IsStringLiteral)
        {
          if (right.IsStringLiteral)
          {
            if (left.IsOkayToCombine && right.IsOkayToCombine)
            {
              constantWrapper = new ConstantWrapper((object) (string.CompareOrdinal(left.ToString(), right.ToString()) <= 0), PrimitiveType.Boolean, left.Context.FlattenToStart());
              goto label_9;
            }
            else
              goto label_9;
          }
        }
        try
        {
          if (left.IsOkayToCombine)
          {
            if (right.IsOkayToCombine)
              constantWrapper = new ConstantWrapper((object) (left.ToNumber() <= right.ToNumber()), PrimitiveType.Boolean, left.Context.FlattenToStart());
          }
        }
        catch (InvalidCastException ex)
        {
        }
      }
label_9:
      return constantWrapper;
    }

    private ConstantWrapper GreaterThan(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        if (left.IsStringLiteral)
        {
          if (right.IsStringLiteral)
          {
            if (left.IsOkayToCombine && right.IsOkayToCombine)
            {
              constantWrapper = new ConstantWrapper((object) (string.CompareOrdinal(left.ToString(), right.ToString()) > 0), PrimitiveType.Boolean, left.Context.FlattenToStart());
              goto label_9;
            }
            else
              goto label_9;
          }
        }
        try
        {
          if (left.IsOkayToCombine)
          {
            if (right.IsOkayToCombine)
              constantWrapper = new ConstantWrapper((object) (left.ToNumber() > right.ToNumber()), PrimitiveType.Boolean, left.Context.FlattenToStart());
          }
        }
        catch (InvalidCastException ex)
        {
        }
      }
label_9:
      return constantWrapper;
    }

    private ConstantWrapper GreaterThanOrEqual(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        if (left.IsStringLiteral)
        {
          if (right.IsStringLiteral)
          {
            if (left.IsOkayToCombine && right.IsOkayToCombine)
            {
              constantWrapper = new ConstantWrapper((object) (string.CompareOrdinal(left.ToString(), right.ToString()) >= 0), PrimitiveType.Boolean, left.Context.FlattenToStart());
              goto label_9;
            }
            else
              goto label_9;
          }
        }
        try
        {
          if (left.IsOkayToCombine)
          {
            if (right.IsOkayToCombine)
              constantWrapper = new ConstantWrapper((object) (left.ToNumber() >= right.ToNumber()), PrimitiveType.Boolean, left.Context.FlattenToStart());
          }
        }
        catch (InvalidCastException ex)
        {
        }
      }
label_9:
      return constantWrapper;
    }

    private ConstantWrapper Equal(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        PrimitiveType primitiveType = left.PrimitiveType;
        if (primitiveType == right.PrimitiveType)
        {
          switch (primitiveType)
          {
            case PrimitiveType.Null:
              constantWrapper = new ConstantWrapper((object) true, PrimitiveType.Boolean, left.Context.FlattenToStart());
              break;
            case PrimitiveType.Boolean:
              constantWrapper = new ConstantWrapper((object) (left.ToBoolean() == right.ToBoolean()), PrimitiveType.Boolean, left.Context.FlattenToStart());
              break;
            case PrimitiveType.Number:
              try
              {
                if (left.IsOkayToCombine)
                {
                  if (right.IsOkayToCombine)
                  {
                    constantWrapper = new ConstantWrapper((object) (left.ToNumber() == right.ToNumber()), PrimitiveType.Boolean, left.Context.FlattenToStart());
                    break;
                  }
                  break;
                }
                break;
              }
              catch (InvalidCastException ex)
              {
                break;
              }
            case PrimitiveType.String:
              if (left.IsOkayToCombine && right.IsOkayToCombine)
              {
                constantWrapper = new ConstantWrapper((object) (string.CompareOrdinal(left.ToString(), right.ToString()) == 0), PrimitiveType.Boolean, left.Context.FlattenToStart());
                break;
              }
              break;
          }
        }
        else if (left.IsOkayToCombine)
        {
          if (right.IsOkayToCombine)
          {
            try
            {
              constantWrapper = new ConstantWrapper((object) (left.ToNumber() == right.ToNumber()), PrimitiveType.Boolean, left.Context.FlattenToStart());
            }
            catch (InvalidCastException ex)
            {
            }
          }
        }
      }
      return constantWrapper;
    }

    private ConstantWrapper NotEqual(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        PrimitiveType primitiveType = left.PrimitiveType;
        if (primitiveType == right.PrimitiveType)
        {
          switch (primitiveType)
          {
            case PrimitiveType.Null:
              constantWrapper = new ConstantWrapper((object) false, PrimitiveType.Boolean, left.Context.FlattenToStart());
              break;
            case PrimitiveType.Boolean:
              constantWrapper = new ConstantWrapper((object) (left.ToBoolean() != right.ToBoolean()), PrimitiveType.Boolean, left.Context.FlattenToStart());
              break;
            case PrimitiveType.Number:
              try
              {
                if (left.IsOkayToCombine)
                {
                  if (right.IsOkayToCombine)
                  {
                    constantWrapper = new ConstantWrapper((object) (left.ToNumber() != right.ToNumber()), PrimitiveType.Boolean, left.Context.FlattenToStart());
                    break;
                  }
                  break;
                }
                break;
              }
              catch (InvalidCastException ex)
              {
                break;
              }
            case PrimitiveType.String:
              if (left.IsOkayToCombine && right.IsOkayToCombine)
              {
                constantWrapper = new ConstantWrapper((object) (string.CompareOrdinal(left.ToString(), right.ToString()) != 0), PrimitiveType.Boolean, left.Context.FlattenToStart());
                break;
              }
              break;
          }
        }
        else if (left.IsOkayToCombine)
        {
          if (right.IsOkayToCombine)
          {
            try
            {
              constantWrapper = new ConstantWrapper((object) (left.ToNumber() != right.ToNumber()), PrimitiveType.Boolean, left.Context.FlattenToStart());
            }
            catch (InvalidCastException ex)
            {
            }
          }
        }
      }
      return constantWrapper;
    }

    private ConstantWrapper StrictEqual(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        PrimitiveType primitiveType = left.PrimitiveType;
        if (primitiveType == right.PrimitiveType)
        {
          switch (primitiveType)
          {
            case PrimitiveType.Null:
              constantWrapper = new ConstantWrapper((object) true, PrimitiveType.Boolean, left.Context.FlattenToStart());
              break;
            case PrimitiveType.Boolean:
              constantWrapper = new ConstantWrapper((object) (left.ToBoolean() == right.ToBoolean()), PrimitiveType.Boolean, left.Context.FlattenToStart());
              break;
            case PrimitiveType.Number:
              try
              {
                if (left.IsOkayToCombine)
                {
                  if (right.IsOkayToCombine)
                  {
                    constantWrapper = new ConstantWrapper((object) (left.ToNumber() == right.ToNumber()), PrimitiveType.Boolean, left.Context.FlattenToStart());
                    break;
                  }
                  break;
                }
                break;
              }
              catch (InvalidCastException ex)
              {
                break;
              }
            case PrimitiveType.String:
              if (left.IsOkayToCombine && right.IsOkayToCombine)
              {
                constantWrapper = new ConstantWrapper((object) (string.CompareOrdinal(left.ToString(), right.ToString()) == 0), PrimitiveType.Boolean, left.Context.FlattenToStart());
                break;
              }
              break;
          }
        }
        else
          constantWrapper = new ConstantWrapper((object) false, PrimitiveType.Boolean, left.Context.FlattenToStart());
      }
      return constantWrapper;
    }

    private ConstantWrapper StrictNotEqual(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        PrimitiveType primitiveType = left.PrimitiveType;
        if (primitiveType == right.PrimitiveType)
        {
          switch (primitiveType)
          {
            case PrimitiveType.Null:
              constantWrapper = new ConstantWrapper((object) false, PrimitiveType.Boolean, left.Context.FlattenToStart());
              break;
            case PrimitiveType.Boolean:
              constantWrapper = new ConstantWrapper((object) (left.ToBoolean() != right.ToBoolean()), PrimitiveType.Boolean, left.Context.FlattenToStart());
              break;
            case PrimitiveType.Number:
              try
              {
                if (left.IsOkayToCombine)
                {
                  if (right.IsOkayToCombine)
                  {
                    constantWrapper = new ConstantWrapper((object) (left.ToNumber() != right.ToNumber()), PrimitiveType.Boolean, left.Context.FlattenToStart());
                    break;
                  }
                  break;
                }
                break;
              }
              catch (InvalidCastException ex)
              {
                break;
              }
            case PrimitiveType.String:
              if (left.IsOkayToCombine && right.IsOkayToCombine)
              {
                constantWrapper = new ConstantWrapper((object) (string.CompareOrdinal(left.ToString(), right.ToString()) != 0), PrimitiveType.Boolean, left.Context.FlattenToStart());
                break;
              }
              break;
          }
        }
        else
          constantWrapper = new ConstantWrapper((object) true, PrimitiveType.Boolean, left.Context.FlattenToStart());
      }
      return constantWrapper;
    }

    private ConstantWrapper BitwiseAnd(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        try
        {
          constantWrapper = new ConstantWrapper((object) Convert.ToDouble(left.ToInt32() & right.ToInt32()), PrimitiveType.Number, left.Context.FlattenToStart());
        }
        catch (InvalidCastException ex)
        {
        }
      }
      return constantWrapper;
    }

    private ConstantWrapper BitwiseOr(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        try
        {
          constantWrapper = new ConstantWrapper((object) Convert.ToDouble(left.ToInt32() | right.ToInt32()), PrimitiveType.Number, left.Context.FlattenToStart());
        }
        catch (InvalidCastException ex)
        {
        }
      }
      return constantWrapper;
    }

    private ConstantWrapper BitwiseXor(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        try
        {
          constantWrapper = new ConstantWrapper((object) Convert.ToDouble(left.ToInt32() ^ right.ToInt32()), PrimitiveType.Number, left.Context.FlattenToStart());
        }
        catch (InvalidCastException ex)
        {
        }
      }
      return constantWrapper;
    }

    private ConstantWrapper LogicalAnd(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        try
        {
          constantWrapper = left.ToBoolean() ? right : left;
        }
        catch (InvalidCastException ex)
        {
        }
      }
      return constantWrapper;
    }

    private ConstantWrapper LogicalOr(ConstantWrapper left, ConstantWrapper right)
    {
      ConstantWrapper constantWrapper = (ConstantWrapper) null;
      if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
      {
        try
        {
          constantWrapper = left.ToBoolean() ? left : right;
        }
        catch (InvalidCastException ex)
        {
        }
      }
      return constantWrapper;
    }

    private static bool OnlyHasConstantItems(ArrayLiteral arrayLiteral)
    {
      int count = arrayLiteral.Elements.Count;
      for (int index = 0; index < count; ++index)
      {
        if (!(arrayLiteral.Elements[index] is ConstantWrapper element) || !element.IsOkayToCombine)
          return false;
      }
      return true;
    }

    private static string ComputeJoin(ArrayLiteral arrayLiteral, ConstantWrapper separatorNode)
    {
      string str = separatorNode == null ? "," : separatorNode.ToString();
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < arrayLiteral.Elements.Count; ++index)
      {
        if (index > 0 && !string.IsNullOrEmpty(str))
          stringBuilder.Append(str);
        stringBuilder.Append(arrayLiteral.Elements[index].ToString());
      }
      return stringBuilder.ToString();
    }

    private int NodeLength(AstNode node) => OutputVisitor.Apply(node, this.m_parser.Settings).IfNotNull<string, int>((Func<string, int>) (c => c.Length));

    public override void Visit(AstNodeList node)
    {
      if (node == null)
        return;
      if (node.Parent is CommaOperator parent && parent.Operand2 is AstNodeList operand2)
      {
        for (int index = operand2.Count - (node.Parent is Block ? 1 : 2); index >= 0; --index)
        {
          if (operand2[index] is ConstantWrapper)
            operand2.RemoveAt(index);
        }
      }
      base.Visit(node);
    }

    public override void Visit(BinaryOperator node)
    {
      if (node == null)
        return;
      base.Visit(node);
      this.DoBinaryOperator(node);
    }

    private void DoBinaryOperator(BinaryOperator node)
    {
      if (!this.m_parser.Settings.EvalLiteralExpressions || node.IsAssign || node.OperatorToken == JSToken.In || node.OperatorToken == JSToken.InstanceOf)
        return;
      if (node.OperatorToken == JSToken.StrictEqual || node.OperatorToken == JSToken.StrictNotEqual)
      {
        PrimitiveType primitiveType1 = node.Operand1.FindPrimitiveType();
        if (primitiveType1 != PrimitiveType.Other)
        {
          PrimitiveType primitiveType2 = node.Operand2.FindPrimitiveType();
          if (primitiveType2 != PrimitiveType.Other)
          {
            if (primitiveType1 != primitiveType2)
            {
              EvaluateLiteralVisitor.ReplaceNodeWithLiteral((AstNode) node, new ConstantWrapper((object) (node.OperatorToken != JSToken.StrictEqual), PrimitiveType.Boolean, node.Context));
              return;
            }
            node.OperatorToken = node.OperatorToken == JSToken.StrictEqual ? JSToken.Equal : JSToken.NotEqual;
          }
        }
      }
      if (node.Operand1 is ConstantWrapper operand1_2)
      {
        if (node.OperatorToken == JSToken.Comma)
        {
          if (node.Operand2 is ConstantWrapper operand2_2)
          {
            if (this.ReplaceMemberBracketWithDot(node, operand2_2))
              return;
            EvaluateLiteralVisitor.ReplaceNodeWithLiteral((AstNode) node, operand2_2);
          }
          else if (node is CommaOperator)
          {
            if (!(node.Operand2 is AstNodeList operand2_1))
              EvaluateLiteralVisitor.ReplaceNodeCheckParens((AstNode) node, node.Operand2);
            else if (operand2_1.Count == 1)
              EvaluateLiteralVisitor.ReplaceNodeCheckParens((AstNode) node, operand2_1[0]);
            else if (operand2_1.Count == 0)
            {
              EvaluateLiteralVisitor.ReplaceNodeCheckParens((AstNode) node, (AstNode) null);
            }
            else
            {
              AstNode astNode1 = operand2_1[0];
              operand2_1.RemoveAt(0);
              node.Operand1 = astNode1;
              if (operand2_1.Count != 1)
                return;
              AstNode astNode2 = operand2_1[0];
              operand2_1.RemoveAt(0);
              node.Operand2 = astNode2;
            }
          }
          else
            EvaluateLiteralVisitor.ReplaceNodeCheckParens((AstNode) node, node.Operand2);
        }
        else if (node.Operand2 is ConstantWrapper operand2_5)
        {
          this.EvalThisOperator(node, operand1_2, operand2_5);
        }
        else
        {
          if (!(node.Operand2 is BinaryOperator operand2_3))
            return;
          if (operand2_3.Operand1 is ConstantWrapper operand1)
          {
            this.EvalToTheRight(node, operand1_2, operand1, operand2_3);
          }
          else
          {
            if (!(operand2_3.Operand2 is ConstantWrapper operand2_4))
              return;
            this.EvalFarToTheRight(node, operand1_2, operand2_4, operand2_3);
          }
        }
      }
      else
      {
        if (!(node.Operand2 is ConstantWrapper operand2_6))
          return;
        if (node.Operand1 is BinaryOperator operand1_1)
        {
          if (operand1_1.Operand2 is ConstantWrapper operand2_7)
          {
            this.EvalToTheLeft(node, operand2_6, operand2_7, operand1_1);
          }
          else
          {
            if (!(operand1_1.Operand1 is ConstantWrapper operand1))
              return;
            this.EvalFarToTheLeft(node, operand2_6, operand1, operand1_1);
          }
        }
        else
        {
          if (!this.m_parser.Settings.IsModificationAllowed(TreeModifications.SimplifyStringToNumericConversion) || !(node.Operand1 is Lookup operand1) || node.OperatorToken != JSToken.Minus || !operand2_6.IsIntegerLiteral || operand2_6.ToNumber() != 0.0)
            return;
          UnaryOperator newNode = new UnaryOperator(node.Context)
          {
            Operand = (AstNode) operand1,
            OperatorToken = JSToken.FirstBinaryOperator
          };
          EvaluateLiteralVisitor.ReplaceNodeCheckParens((AstNode) node, (AstNode) newNode);
        }
      }
    }

    public override void Visit(CallNode node)
    {
      if (node == null)
        return;
      base.Visit(node);
      if (node.IsConstructor || node.InBrackets || !(node.Function is Member function) || string.CompareOrdinal(function.Name, "join") != 0 || node.Arguments.Count > 1 || !this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateLiteralJoins) || !(function.Root is ArrayLiteral root) || root.MayHaveIssues)
        return;
      separatorNode = (ConstantWrapper) null;
      if (node.Arguments.Count != 0 && !(node.Arguments[0] is ConstantWrapper separatorNode) || !EvaluateLiteralVisitor.OnlyHasConstantItems(root))
        return;
      string join = EvaluateLiteralVisitor.ComputeJoin(root, separatorNode);
      if (join.Length + 2 >= this.NodeLength((AstNode) node))
        return;
      EvaluateLiteralVisitor.ReplaceNodeWithLiteral((AstNode) node, new ConstantWrapper((object) join, PrimitiveType.String, node.Context));
    }

    public override void Visit(Conditional node)
    {
      if (node == null)
        return;
      base.Visit(node);
      this.DoConditional(node);
    }

    private void DoConditional(Conditional node)
    {
      if (!this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
        return;
      if (!(node.Condition is ConstantWrapper condition))
        return;
      try
      {
        EvaluateLiteralVisitor.ReplaceNodeCheckParens((AstNode) node, condition.ToBoolean() ? node.TrueExpression : node.FalseExpression);
      }
      catch (InvalidCastException ex)
      {
      }
    }

    public override void Visit(ConditionalCompilationElseIf node)
    {
      if (node == null)
        return;
      base.Visit(node);
      this.DoConditionalCompilationElseIf(node);
    }

    private void DoConditionalCompilationElseIf(ConditionalCompilationElseIf node)
    {
      if (!this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions) || !(node.Condition is ConstantWrapper condition))
        return;
      if (!condition.IsNotOneOrPositiveZero)
        return;
      try
      {
        node.Condition = (AstNode) new ConstantWrapper((object) (condition.ToBoolean() ? 1 : 0), PrimitiveType.Number, node.Condition.Context);
      }
      catch (InvalidCastException ex)
      {
      }
    }

    public override void Visit(ConditionalCompilationIf node)
    {
      if (node == null)
        return;
      base.Visit(node);
      this.DoConditionalCompilationIf(node);
    }

    private void DoConditionalCompilationIf(ConditionalCompilationIf node)
    {
      if (!this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions) || !(node.Condition is ConstantWrapper condition))
        return;
      if (!condition.IsNotOneOrPositiveZero)
        return;
      try
      {
        node.Condition = (AstNode) new ConstantWrapper((object) (condition.ToBoolean() ? 1 : 0), PrimitiveType.Number, node.Condition.Context);
      }
      catch (InvalidCastException ex)
      {
      }
    }

    public override void Visit(DoWhile node)
    {
      if (node == null)
        return;
      base.Visit(node);
      this.DoDoWhile(node);
    }

    private void DoDoWhile(DoWhile node)
    {
      if (!this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions) || !(node.Condition is ConstantWrapper condition))
        return;
      if (!condition.IsNotOneOrPositiveZero)
        return;
      try
      {
        node.Condition = (AstNode) new ConstantWrapper((object) (condition.ToBoolean() ? 1 : 0), PrimitiveType.Number, node.Condition.Context);
      }
      catch (InvalidCastException ex)
      {
      }
    }

    public override void Visit(ForNode node)
    {
      if (node == null)
        return;
      base.Visit(node);
      this.DoForNode(node);
    }

    private void DoForNode(ForNode node)
    {
      if (!this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
        return;
      if (!(node.Condition is ConstantWrapper condition))
        return;
      try
      {
        if (condition.ToBoolean())
        {
          node.Condition = (AstNode) null;
        }
        else
        {
          if (!condition.IsNotOneOrPositiveZero)
            return;
          node.Condition = (AstNode) new ConstantWrapper((object) 0, PrimitiveType.Number, node.Condition.Context);
        }
      }
      catch (InvalidCastException ex)
      {
      }
    }

    public override void Visit(IfNode node)
    {
      if (node == null)
        return;
      base.Visit(node);
      this.DoIfNode(node);
    }

    private void DoIfNode(IfNode node)
    {
      if (!this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions) || !(node.Condition is ConstantWrapper condition))
        return;
      if (!condition.IsNotOneOrPositiveZero)
        return;
      try
      {
        node.Condition = (AstNode) new ConstantWrapper((object) (condition.ToBoolean() ? 1 : 0), PrimitiveType.Number, node.Condition.Context);
      }
      catch (InvalidCastException ex)
      {
      }
    }

    public override void Visit(Member node)
    {
      if (node == null)
        return;
      base.Visit(node);
      if (string.CompareOrdinal(node.Name, "length") != 0 || !this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateLiteralLengths))
        return;
      ConstantWrapper newNode = (ConstantWrapper) null;
      if (node.Root is ConstantWrapper root2)
      {
        if (root2.PrimitiveType == PrimitiveType.String && !root2.MayHaveIssues)
          newNode = new ConstantWrapper((object) root2.ToString().Length, PrimitiveType.Number, node.Context);
      }
      else if (node.Root is ArrayLiteral root1 && !root1.MayHaveIssues)
      {
        int length = root1.Length;
        if (length >= 0)
          newNode = new ConstantWrapper((object) length, PrimitiveType.Number, node.Context);
      }
      if (newNode == null)
        return;
      node.Parent.ReplaceChild((AstNode) node, (AstNode) newNode);
    }

    public override void Visit(UnaryOperator node)
    {
      if (node == null)
        return;
      base.Visit(node);
      this.DoUnaryNode(node);
    }

    private void DoUnaryNode(UnaryOperator node)
    {
      if (node.OperatorInConditionalCompilationComment || !this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
        return;
      ConstantWrapper operand = node.Operand as ConstantWrapper;
      switch (node.OperatorToken)
      {
        case JSToken.Void:
          if (operand == null)
            break;
          node.Operand = (AstNode) new ConstantWrapper((object) 0, PrimitiveType.Number, node.Context);
          break;
        case JSToken.TypeOf:
          if (operand != null)
          {
            string str = (string) null;
            if (operand.IsStringLiteral)
              str = "string";
            else if (operand.IsNumericLiteral)
              str = "number";
            else if (operand.IsBooleanLiteral)
              str = "boolean";
            else if (operand.Value == null)
              str = "object";
            if (string.IsNullOrEmpty(str))
              break;
            EvaluateLiteralVisitor.ReplaceNodeWithLiteral((AstNode) node, new ConstantWrapper((object) str, PrimitiveType.String, node.Context));
            break;
          }
          if (!(node.Operand is ObjectLiteral))
            break;
          EvaluateLiteralVisitor.ReplaceNodeWithLiteral((AstNode) node, new ConstantWrapper((object) "object", PrimitiveType.String, node.Context));
          break;
        case JSToken.LogicalNot:
          if (operand == null)
            break;
          try
          {
            EvaluateLiteralVisitor.ReplaceNodeWithLiteral((AstNode) node, new ConstantWrapper((object) !operand.ToBoolean(), PrimitiveType.Boolean, node.Context));
            break;
          }
          catch (InvalidCastException ex)
          {
            break;
          }
        case JSToken.BitwiseNot:
          if (operand == null)
            break;
          try
          {
            EvaluateLiteralVisitor.ReplaceNodeWithLiteral((AstNode) node, new ConstantWrapper((object) Convert.ToDouble(~operand.ToInt32()), PrimitiveType.Number, node.Context));
            break;
          }
          catch (InvalidCastException ex)
          {
            break;
          }
        case JSToken.FirstBinaryOperator:
          if (operand == null)
            break;
          try
          {
            EvaluateLiteralVisitor.ReplaceNodeWithLiteral((AstNode) node, new ConstantWrapper((object) operand.ToNumber(), PrimitiveType.Number, node.Context));
            break;
          }
          catch (InvalidCastException ex)
          {
            break;
          }
        case JSToken.Minus:
          if (operand == null)
            break;
          try
          {
            EvaluateLiteralVisitor.ReplaceNodeWithLiteral((AstNode) node, new ConstantWrapper((object) -operand.ToNumber(), PrimitiveType.Number, node.Context));
            break;
          }
          catch (InvalidCastException ex)
          {
            break;
          }
      }
    }

    public override void Visit(WhileNode node)
    {
      if (node == null)
        return;
      base.Visit(node);
      this.DoWhileNode(node);
    }

    private void DoWhileNode(WhileNode node)
    {
      if (!this.m_parser.Settings.IsModificationAllowed(TreeModifications.EvaluateNumericExpressions))
        return;
      if (!(node.Condition is ConstantWrapper condition))
        return;
      try
      {
        if (condition.ToBoolean())
        {
          if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.ChangeWhileToFor))
          {
            AstNode astNode = (AstNode) null;
            if (this.m_parser.Settings.IsModificationAllowed(TreeModifications.MoveVarIntoFor) && node.Parent is Block parent)
            {
              int num = parent.IndexOf((AstNode) node);
              if (num > 0 && parent[num - 1] is Var var)
              {
                astNode = (AstNode) var;
                parent.RemoveAt(num - 1);
              }
            }
            ForNode forNode = new ForNode(node.Context);
            forNode.Initializer = astNode;
            forNode.Body = node.Body;
            ForNode newNode = forNode;
            node.Parent.ReplaceChild((AstNode) node, (AstNode) newNode);
          }
          else
            node.Condition = (AstNode) new ConstantWrapper((object) 1, PrimitiveType.Number, node.Condition.Context);
        }
        else
        {
          if (!condition.IsNotOneOrPositiveZero)
            return;
          node.Condition = (AstNode) new ConstantWrapper((object) 0, PrimitiveType.Number, node.Condition.Context);
        }
      }
      catch (InvalidCastException ex)
      {
      }
    }
  }
}
