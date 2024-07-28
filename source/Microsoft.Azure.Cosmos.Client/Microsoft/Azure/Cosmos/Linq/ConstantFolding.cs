// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.ConstantFolding
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal static class ConstantFolding
  {
    public static bool IsConstant(Expression inputExpression) => inputExpression == null || inputExpression.NodeType == ExpressionType.Constant;

    public static Expression Fold(Expression inputExpression)
    {
      if (inputExpression == null)
        return inputExpression;
      switch (inputExpression.NodeType)
      {
        case ExpressionType.Add:
        case ExpressionType.AddChecked:
        case ExpressionType.And:
        case ExpressionType.AndAlso:
        case ExpressionType.ArrayIndex:
        case ExpressionType.Coalesce:
        case ExpressionType.Divide:
        case ExpressionType.Equal:
        case ExpressionType.ExclusiveOr:
        case ExpressionType.GreaterThan:
        case ExpressionType.GreaterThanOrEqual:
        case ExpressionType.LeftShift:
        case ExpressionType.LessThan:
        case ExpressionType.LessThanOrEqual:
        case ExpressionType.Modulo:
        case ExpressionType.Multiply:
        case ExpressionType.MultiplyChecked:
        case ExpressionType.NotEqual:
        case ExpressionType.Or:
        case ExpressionType.OrElse:
        case ExpressionType.RightShift:
        case ExpressionType.Subtract:
        case ExpressionType.SubtractChecked:
          return ConstantFolding.FoldBinary((BinaryExpression) inputExpression);
        case ExpressionType.ArrayLength:
        case ExpressionType.Convert:
        case ExpressionType.ConvertChecked:
        case ExpressionType.Negate:
        case ExpressionType.UnaryPlus:
        case ExpressionType.NegateChecked:
        case ExpressionType.Not:
        case ExpressionType.Quote:
        case ExpressionType.TypeAs:
        case ExpressionType.Decrement:
        case ExpressionType.Increment:
        case ExpressionType.OnesComplement:
          return ConstantFolding.FoldUnary((UnaryExpression) inputExpression);
        case ExpressionType.Call:
          return ConstantFolding.FoldMethodCall((MethodCallExpression) inputExpression);
        case ExpressionType.Conditional:
          return ConstantFolding.FoldConditional((ConditionalExpression) inputExpression);
        case ExpressionType.Constant:
          return inputExpression;
        case ExpressionType.Invoke:
          return ConstantFolding.FoldInvocation((InvocationExpression) inputExpression);
        case ExpressionType.Lambda:
          return (Expression) ConstantFolding.FoldLambda((LambdaExpression) inputExpression);
        case ExpressionType.ListInit:
          return ConstantFolding.FoldListInit((ListInitExpression) inputExpression);
        case ExpressionType.MemberAccess:
          return ConstantFolding.FoldMemberAccess((MemberExpression) inputExpression);
        case ExpressionType.MemberInit:
          return ConstantFolding.FoldMemberInit((MemberInitExpression) inputExpression);
        case ExpressionType.New:
          return (Expression) ConstantFolding.FoldNew((NewExpression) inputExpression);
        case ExpressionType.NewArrayInit:
        case ExpressionType.NewArrayBounds:
          return ConstantFolding.FoldNewArray((NewArrayExpression) inputExpression);
        case ExpressionType.Parameter:
          return ConstantFolding.FoldParameter((ParameterExpression) inputExpression);
        case ExpressionType.TypeIs:
          return ConstantFolding.FoldTypeIs((TypeBinaryExpression) inputExpression);
        default:
          throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Unhandled expression type: '{0}'", (object) inputExpression.NodeType));
      }
    }

    public static MemberBinding FoldBinding(MemberBinding inputExpression)
    {
      switch (inputExpression.BindingType)
      {
        case MemberBindingType.Assignment:
          return (MemberBinding) ConstantFolding.FoldMemberAssignment((MemberAssignment) inputExpression);
        case MemberBindingType.MemberBinding:
          return (MemberBinding) ConstantFolding.FoldMemberMemberBinding((MemberMemberBinding) inputExpression);
        case MemberBindingType.ListBinding:
          return (MemberBinding) ConstantFolding.FoldMemberListBinding((MemberListBinding) inputExpression);
        default:
          throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Unhandled binding type '{0}'", (object) inputExpression.BindingType));
      }
    }

    public static ElementInit FoldElementInitializer(ElementInit inputExpression)
    {
      ReadOnlyCollection<Expression> arguments = ConstantFolding.FoldExpressionList(inputExpression.Arguments);
      return arguments != inputExpression.Arguments ? Expression.ElementInit(inputExpression.AddMethod, (IEnumerable<Expression>) arguments) : inputExpression;
    }

    public static Expression FoldUnary(UnaryExpression inputExpression)
    {
      Expression expression = ConstantFolding.Fold(inputExpression.Operand);
      Expression expr = expression == inputExpression.Operand ? (Expression) inputExpression : (Expression) Expression.MakeUnary(inputExpression.NodeType, expression, inputExpression.Type, inputExpression.Method);
      if (ConstantFolding.IsConstant(expression))
        expr = ExpressionSimplifier.EvaluateToExpression(expr);
      return expr;
    }

    public static Expression FoldBinary(BinaryExpression inputExpression)
    {
      Expression expression1 = ConstantFolding.Fold(inputExpression.Left);
      Expression expression2 = ConstantFolding.Fold(inputExpression.Right);
      LambdaExpression conversion = ConstantFolding.FoldLambda(inputExpression.Conversion);
      Expression expr = expression1 != inputExpression.Left || expression2 != inputExpression.Right || conversion != inputExpression.Conversion ? (inputExpression.NodeType != ExpressionType.Coalesce ? (Expression) Expression.MakeBinary(inputExpression.NodeType, expression1, expression2, inputExpression.IsLiftedToNull, inputExpression.Method) : (Expression) Expression.Coalesce(expression1, expression2, conversion)) : (Expression) inputExpression;
      if (ConstantFolding.IsConstant(expression1) && inputExpression.NodeType == ExpressionType.Coalesce)
      {
        object obj = ExpressionSimplifier.Evaluate(expression1);
        expr = obj != null ? (Expression) Expression.Constant(obj) : expression2;
      }
      else if (ConstantFolding.IsConstant(expression1) && ConstantFolding.IsConstant(expression2))
        expr = ExpressionSimplifier.EvaluateToExpression(expr);
      return expr;
    }

    public static Expression FoldTypeIs(TypeBinaryExpression inputExpression)
    {
      Expression expression = ConstantFolding.Fold(inputExpression.Expression);
      Expression expr = expression == inputExpression.Expression ? (Expression) inputExpression : (Expression) Expression.TypeIs(expression, inputExpression.TypeOperand);
      if (ConstantFolding.IsConstant(expression))
        expr = ExpressionSimplifier.EvaluateToExpression(expr);
      return expr;
    }

    public static Expression FoldConstant(ConstantExpression inputExpression) => (Expression) inputExpression;

    public static Expression FoldConditional(ConditionalExpression inputExpression)
    {
      Expression expression1 = ConstantFolding.Fold(inputExpression.Test);
      Expression ifTrue = ConstantFolding.Fold(inputExpression.IfTrue);
      Expression ifFalse = ConstantFolding.Fold(inputExpression.IfFalse);
      Expression expression2 = expression1 != inputExpression.Test || ifTrue != inputExpression.IfTrue || ifFalse != inputExpression.IfFalse ? (Expression) Expression.Condition(expression1, ifTrue, ifFalse) : (Expression) inputExpression;
      if (ConstantFolding.IsConstant(expression1))
        expression2 = !(bool) ExpressionSimplifier.Evaluate(expression1) ? ifFalse : ifTrue;
      return expression2;
    }

    public static Expression FoldParameter(ParameterExpression inputExpression) => (Expression) inputExpression;

    public static Expression FoldMemberAccess(MemberExpression inputExpression)
    {
      Expression expression = ConstantFolding.Fold(inputExpression.Expression);
      Expression expr = expression == inputExpression.Expression ? (Expression) inputExpression : (Expression) Expression.MakeMemberAccess(expression, inputExpression.Member);
      if (ConstantFolding.IsConstant(expression))
        expr = ExpressionSimplifier.EvaluateToExpression(expr);
      return expr;
    }

    public static Expression FoldMethodCall(MethodCallExpression inputExpression)
    {
      Expression expression = ConstantFolding.Fold(inputExpression.Object);
      ReadOnlyCollection<Expression> arguments = ConstantFolding.FoldExpressionList(inputExpression.Arguments);
      Expression expr = expression != inputExpression.Object || arguments != inputExpression.Arguments ? (Expression) Expression.Call(expression, inputExpression.Method, (IEnumerable<Expression>) arguments) : (Expression) inputExpression;
      if (!ConstantFolding.IsConstant(expression))
        return expr;
      foreach (Expression inputExpression1 in arguments)
      {
        if (!ConstantFolding.IsConstant(inputExpression1))
          return expr;
      }
      return inputExpression.Method.IsStatic && inputExpression.Method.DeclaringType.IsAssignableFrom(typeof (Queryable)) && inputExpression.Method.Name.Equals("Take") ? expr : ExpressionSimplifier.EvaluateToExpression(expr);
    }

    public static ReadOnlyCollection<Expression> FoldExpressionList(
      ReadOnlyCollection<Expression> inputExpressionList)
    {
      List<Expression> expressionList = (List<Expression>) null;
      for (int index1 = 0; index1 < inputExpressionList.Count; ++index1)
      {
        Expression expression = ConstantFolding.Fold(inputExpressionList[index1]);
        if (expressionList != null)
          expressionList.Add(expression);
        else if (expression != inputExpressionList[index1])
        {
          expressionList = new List<Expression>(inputExpressionList.Count);
          for (int index2 = 0; index2 < index1; ++index2)
            expressionList.Add(inputExpressionList[index2]);
          expressionList.Add(expression);
        }
      }
      return expressionList != null ? expressionList.AsReadOnly() : inputExpressionList;
    }

    public static MemberAssignment FoldMemberAssignment(MemberAssignment inputExpression)
    {
      Expression expression = ConstantFolding.Fold(inputExpression.Expression);
      return expression != inputExpression.Expression ? Expression.Bind(inputExpression.Member, expression) : inputExpression;
    }

    public static MemberMemberBinding FoldMemberMemberBinding(MemberMemberBinding inputExpression)
    {
      IEnumerable<MemberBinding> bindings = (IEnumerable<MemberBinding>) ConstantFolding.FoldBindingList(inputExpression.Bindings);
      return bindings != inputExpression.Bindings ? Expression.MemberBind(inputExpression.Member, bindings) : inputExpression;
    }

    public static MemberListBinding FoldMemberListBinding(MemberListBinding inputExpression)
    {
      IEnumerable<ElementInit> initializers = (IEnumerable<ElementInit>) ConstantFolding.FoldElementInitializerList(inputExpression.Initializers);
      return initializers != inputExpression.Initializers ? Expression.ListBind(inputExpression.Member, initializers) : inputExpression;
    }

    public static IList<MemberBinding> FoldBindingList(
      ReadOnlyCollection<MemberBinding> inputExpressionList)
    {
      List<MemberBinding> memberBindingList = (List<MemberBinding>) null;
      for (int index1 = 0; index1 < inputExpressionList.Count; ++index1)
      {
        MemberBinding memberBinding = ConstantFolding.FoldBinding(inputExpressionList[index1]);
        if (memberBindingList != null)
          memberBindingList.Add(memberBinding);
        else if (memberBinding != inputExpressionList[index1])
        {
          memberBindingList = new List<MemberBinding>(inputExpressionList.Count);
          for (int index2 = 0; index2 < index1; ++index2)
            memberBindingList.Add(inputExpressionList[index2]);
          memberBindingList.Add(memberBinding);
        }
      }
      return (IList<MemberBinding>) memberBindingList ?? (IList<MemberBinding>) inputExpressionList;
    }

    public static IList<ElementInit> FoldElementInitializerList(
      ReadOnlyCollection<ElementInit> inputExpressionList)
    {
      List<ElementInit> elementInitList = (List<ElementInit>) null;
      for (int index1 = 0; index1 < inputExpressionList.Count; ++index1)
      {
        ElementInit elementInit = ConstantFolding.FoldElementInitializer(inputExpressionList[index1]);
        if (elementInitList != null)
          elementInitList.Add(elementInit);
        else if (elementInit != inputExpressionList[index1])
        {
          elementInitList = new List<ElementInit>(inputExpressionList.Count);
          for (int index2 = 0; index2 < index1; ++index2)
            elementInitList.Add(inputExpressionList[index2]);
          elementInitList.Add(elementInit);
        }
      }
      return (IList<ElementInit>) elementInitList ?? (IList<ElementInit>) inputExpressionList;
    }

    public static LambdaExpression FoldLambda(LambdaExpression inputExpression)
    {
      if (inputExpression == null)
        return (LambdaExpression) null;
      Expression body = ConstantFolding.Fold(inputExpression.Body);
      return body != inputExpression.Body ? Expression.Lambda(inputExpression.Type, body, (IEnumerable<ParameterExpression>) inputExpression.Parameters) : inputExpression;
    }

    public static NewExpression FoldNew(NewExpression inputExpression)
    {
      IEnumerable<Expression> arguments = (IEnumerable<Expression>) ConstantFolding.FoldExpressionList(inputExpression.Arguments);
      if (arguments == inputExpression.Arguments)
        return inputExpression;
      return inputExpression.Members != null ? Expression.New(inputExpression.Constructor, arguments, (IEnumerable<MemberInfo>) inputExpression.Members) : Expression.New(inputExpression.Constructor, arguments);
    }

    public static Expression FoldMemberInit(MemberInitExpression inputExpression)
    {
      NewExpression newExpression = ConstantFolding.FoldNew(inputExpression.NewExpression);
      IEnumerable<MemberBinding> bindings = (IEnumerable<MemberBinding>) ConstantFolding.FoldBindingList(inputExpression.Bindings);
      return newExpression != inputExpression.NewExpression || bindings != inputExpression.Bindings ? (Expression) Expression.MemberInit(newExpression, bindings) : (Expression) inputExpression;
    }

    public static Expression FoldListInit(ListInitExpression inputExpression)
    {
      NewExpression newExpression = ConstantFolding.FoldNew(inputExpression.NewExpression);
      IEnumerable<ElementInit> initializers = (IEnumerable<ElementInit>) ConstantFolding.FoldElementInitializerList(inputExpression.Initializers);
      return newExpression != inputExpression.NewExpression || initializers != inputExpression.Initializers ? (Expression) Expression.ListInit(newExpression, initializers) : (Expression) inputExpression;
    }

    public static Expression FoldNewArray(NewArrayExpression inputExpression)
    {
      IEnumerable<Expression> expressions = (IEnumerable<Expression>) ConstantFolding.FoldExpressionList(inputExpression.Expressions);
      if (expressions == inputExpression.Expressions)
        return (Expression) inputExpression;
      return inputExpression.NodeType == ExpressionType.NewArrayInit ? (Expression) Expression.NewArrayInit(inputExpression.Type.GetElementType(), expressions) : (Expression) Expression.NewArrayBounds(inputExpression.Type.GetElementType(), expressions);
    }

    public static Expression FoldInvocation(InvocationExpression inputExpression)
    {
      IEnumerable<Expression> arguments = (IEnumerable<Expression>) ConstantFolding.FoldExpressionList(inputExpression.Arguments);
      Expression expression = ConstantFolding.Fold(inputExpression.Expression);
      Expression expr = arguments != inputExpression.Arguments || expression != inputExpression.Expression ? (Expression) Expression.Invoke(expression, arguments) : (Expression) inputExpression;
      foreach (Expression inputExpression1 in arguments)
      {
        if (!ConstantFolding.IsConstant(inputExpression1))
          return expr;
      }
      return ExpressionSimplifier.EvaluateToExpression(expr);
    }
  }
}
