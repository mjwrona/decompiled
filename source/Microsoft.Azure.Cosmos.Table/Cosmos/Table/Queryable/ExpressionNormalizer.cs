// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.ExpressionNormalizer
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal class ExpressionNormalizer : DataServiceALinqExpressionVisitor
  {
    private const bool LiftToNull = false;
    private readonly Dictionary<Expression, ExpressionNormalizer.Pattern> patterns = new Dictionary<Expression, ExpressionNormalizer.Pattern>((IEqualityComparer<Expression>) ReferenceEqualityComparer<Expression>.Instance);
    private readonly Dictionary<Expression, Expression> normalizerRewrites;
    private static readonly MethodInfo StaticRelationalOperatorPlaceholderMethod = typeof (ExpressionNormalizer).GetMethod("RelationalOperatorPlaceholder", BindingFlags.Static | BindingFlags.NonPublic);

    private ExpressionNormalizer(
      Dictionary<Expression, Expression> normalizerRewrites)
    {
      this.normalizerRewrites = normalizerRewrites;
    }

    internal Dictionary<Expression, Expression> NormalizerRewrites => this.normalizerRewrites;

    internal static Expression Normalize(
      Expression expression,
      Dictionary<Expression, Expression> rewrites)
    {
      return new ExpressionNormalizer(rewrites).Visit(expression);
    }

    internal override Expression VisitBinary(BinaryExpression b)
    {
      BinaryExpression rewritten = (BinaryExpression) base.VisitBinary(b);
      if (rewritten.NodeType == ExpressionType.Equal)
      {
        Expression left = ExpressionNormalizer.UnwrapObjectConvert(rewritten.Left);
        Expression right = ExpressionNormalizer.UnwrapObjectConvert(rewritten.Right);
        if (left != rewritten.Left || right != rewritten.Right)
          rewritten = ExpressionNormalizer.CreateRelationalOperator(ExpressionType.Equal, left, right);
      }
      ExpressionNormalizer.Pattern pattern;
      if (this.patterns.TryGetValue(rewritten.Left, out pattern) && pattern.Kind == ExpressionNormalizer.PatternKind.Compare && ExpressionNormalizer.IsConstantZero(rewritten.Right))
      {
        ExpressionNormalizer.ComparePattern comparePattern = (ExpressionNormalizer.ComparePattern) pattern;
        BinaryExpression result;
        if (ExpressionNormalizer.TryCreateRelationalOperator(rewritten.NodeType, comparePattern.Left, comparePattern.Right, out result))
          rewritten = result;
      }
      this.RecordRewrite((Expression) b, (Expression) rewritten);
      return (Expression) rewritten;
    }

    internal override Expression VisitUnary(UnaryExpression u)
    {
      Expression rewritten = base.VisitUnary(u);
      this.RecordRewrite((Expression) u, rewritten);
      return rewritten;
    }

    private static Expression UnwrapObjectConvert(Expression input)
    {
      if (input.NodeType == ExpressionType.Constant && input.Type == typeof (object))
      {
        ConstantExpression constantExpression = (ConstantExpression) input;
        if (constantExpression.Value != null && constantExpression.Value.GetType() != typeof (object))
          return (Expression) Expression.Constant(constantExpression.Value, constantExpression.Value.GetType());
      }
      while (ExpressionType.Convert == input.NodeType && typeof (object) == input.Type)
        input = ((UnaryExpression) input).Operand;
      return input;
    }

    private static bool IsConstantZero(Expression expression) => expression.NodeType == ExpressionType.Constant && ((ConstantExpression) expression).Value.Equals((object) 0);

    internal override Expression VisitMethodCall(MethodCallExpression call)
    {
      Expression rewritten = this.VisitMethodCallNoRewrite(call);
      this.RecordRewrite((Expression) call, rewritten);
      return rewritten;
    }

    internal Expression VisitMethodCallNoRewrite(MethodCallExpression call)
    {
      MethodCallExpression callExpression = (MethodCallExpression) base.VisitMethodCall(call);
      if (callExpression.Method.IsStatic && callExpression.Method.Name.StartsWith("op_", StringComparison.Ordinal))
      {
        if (callExpression.Arguments.Count == 2)
        {
          switch (callExpression.Method.Name)
          {
            case "op_Addition":
              return (Expression) Expression.Add(callExpression.Arguments[0], callExpression.Arguments[1], callExpression.Method);
            case "op_BitwiseAnd":
              return (Expression) Expression.And(callExpression.Arguments[0], callExpression.Arguments[1], callExpression.Method);
            case "op_BitwiseOr":
              return (Expression) Expression.Or(callExpression.Arguments[0], callExpression.Arguments[1], callExpression.Method);
            case "op_Division":
              return (Expression) Expression.Divide(callExpression.Arguments[0], callExpression.Arguments[1], callExpression.Method);
            case "op_Equality":
              return (Expression) Expression.Equal(callExpression.Arguments[0], callExpression.Arguments[1], false, callExpression.Method);
            case "op_ExclusiveOr":
              return (Expression) Expression.ExclusiveOr(callExpression.Arguments[0], callExpression.Arguments[1], callExpression.Method);
            case "op_GreaterThan":
              return (Expression) Expression.GreaterThan(callExpression.Arguments[0], callExpression.Arguments[1], false, callExpression.Method);
            case "op_GreaterThanOrEqual":
              return (Expression) Expression.GreaterThanOrEqual(callExpression.Arguments[0], callExpression.Arguments[1], false, callExpression.Method);
            case "op_Inequality":
              return (Expression) Expression.NotEqual(callExpression.Arguments[0], callExpression.Arguments[1], false, callExpression.Method);
            case "op_LessThan":
              return (Expression) Expression.LessThan(callExpression.Arguments[0], callExpression.Arguments[1], false, callExpression.Method);
            case "op_LessThanOrEqual":
              return (Expression) Expression.LessThanOrEqual(callExpression.Arguments[0], callExpression.Arguments[1], false, callExpression.Method);
            case "op_Modulus":
              return (Expression) Expression.Modulo(callExpression.Arguments[0], callExpression.Arguments[1], callExpression.Method);
            case "op_Multiply":
              return (Expression) Expression.Multiply(callExpression.Arguments[0], callExpression.Arguments[1], callExpression.Method);
            case "op_Subtraction":
              return (Expression) Expression.Subtract(callExpression.Arguments[0], callExpression.Arguments[1], callExpression.Method);
          }
        }
        if (callExpression.Arguments.Count == 1)
        {
          switch (callExpression.Method.Name)
          {
            case "op_UnaryNegation":
              return (Expression) Expression.Negate(callExpression.Arguments[0], callExpression.Method);
            case "op_UnaryPlus":
              return (Expression) Expression.UnaryPlus(callExpression.Arguments[0], callExpression.Method);
            case "op_Explicit":
            case "op_Implicit":
              return (Expression) Expression.Convert(callExpression.Arguments[0], callExpression.Type, callExpression.Method);
            case "op_OnesComplement":
            case "op_False":
              return (Expression) Expression.Not(callExpression.Arguments[0], callExpression.Method);
          }
        }
      }
      if (callExpression.Method.IsStatic && callExpression.Method.Name == "Equals" && callExpression.Arguments.Count > 1)
        return (Expression) Expression.Equal(callExpression.Arguments[0], callExpression.Arguments[1], false, callExpression.Method);
      if (!callExpression.Method.IsStatic && callExpression.Method.Name == "Equals" && callExpression.Arguments.Count > 0)
        return (Expression) ExpressionNormalizer.CreateRelationalOperator(ExpressionType.Equal, callExpression.Object, callExpression.Arguments[0]);
      if (callExpression.Method.IsStatic && callExpression.Method.Name == "CompareString" && callExpression.Method.DeclaringType.FullName == "Microsoft.VisualBasic.CompilerServices.Operators")
        return this.CreateCompareExpression(callExpression.Arguments[0], callExpression.Arguments[1]);
      if (!callExpression.Method.IsStatic && callExpression.Method.Name == "CompareTo" && callExpression.Arguments.Count == 1 && callExpression.Method.ReturnType == typeof (int))
        return this.CreateCompareExpression(callExpression.Object, callExpression.Arguments[0]);
      return callExpression.Method.IsStatic && callExpression.Method.Name == "Compare" && callExpression.Arguments.Count > 1 && callExpression.Method.ReturnType == typeof (int) ? this.CreateCompareExpression(callExpression.Arguments[0], callExpression.Arguments[1]) : (Expression) ExpressionNormalizer.NormalizePredicateArgument(callExpression);
    }

    private static MethodCallExpression NormalizePredicateArgument(
      MethodCallExpression callExpression)
    {
      int argumentOrdinal;
      Expression normalized;
      MethodCallExpression methodCallExpression;
      if (ExpressionNormalizer.HasPredicateArgument(callExpression, out argumentOrdinal) && ExpressionNormalizer.TryMatchCoalescePattern(callExpression.Arguments[argumentOrdinal], out normalized))
        methodCallExpression = Expression.Call(callExpression.Object, callExpression.Method, (IEnumerable<Expression>) new List<Expression>((IEnumerable<Expression>) callExpression.Arguments)
        {
          [argumentOrdinal] = normalized
        });
      else
        methodCallExpression = callExpression;
      return methodCallExpression;
    }

    private static bool HasPredicateArgument(
      MethodCallExpression callExpression,
      out int argumentOrdinal)
    {
      argumentOrdinal = 0;
      bool flag = false;
      SequenceMethod sequenceMethod;
      if (2 <= callExpression.Arguments.Count && ReflectionUtil.TryIdentifySequenceMethod(callExpression.Method, out sequenceMethod))
      {
        switch (sequenceMethod)
        {
          case SequenceMethod.Where:
          case SequenceMethod.WhereOrdinal:
          case SequenceMethod.TakeWhile:
          case SequenceMethod.TakeWhileOrdinal:
          case SequenceMethod.SkipWhile:
          case SequenceMethod.SkipWhileOrdinal:
          case SequenceMethod.FirstPredicate:
          case SequenceMethod.FirstOrDefaultPredicate:
          case SequenceMethod.LastPredicate:
          case SequenceMethod.LastOrDefaultPredicate:
          case SequenceMethod.SinglePredicate:
          case SequenceMethod.SingleOrDefaultPredicate:
          case SequenceMethod.AnyPredicate:
          case SequenceMethod.All:
          case SequenceMethod.CountPredicate:
          case SequenceMethod.LongCountPredicate:
            argumentOrdinal = 1;
            flag = true;
            break;
        }
      }
      return flag;
    }

    private static bool TryMatchCoalescePattern(Expression expression, out Expression normalized)
    {
      normalized = (Expression) null;
      bool flag = false;
      if (expression.NodeType == ExpressionType.Quote)
      {
        if (ExpressionNormalizer.TryMatchCoalescePattern(((UnaryExpression) expression).Operand, out normalized))
        {
          flag = true;
          normalized = (Expression) Expression.Quote(normalized);
        }
      }
      else if (expression.NodeType == ExpressionType.Lambda)
      {
        LambdaExpression lambdaExpression = (LambdaExpression) expression;
        if (lambdaExpression.Body.NodeType == ExpressionType.Coalesce && lambdaExpression.Body.Type == typeof (bool))
        {
          BinaryExpression body = (BinaryExpression) lambdaExpression.Body;
          if (body.Right.NodeType == ExpressionType.Constant && false.Equals(((ConstantExpression) body.Right).Value))
          {
            normalized = (Expression) Expression.Lambda(lambdaExpression.Type, (Expression) Expression.Convert(body.Left, typeof (bool)), (IEnumerable<ParameterExpression>) lambdaExpression.Parameters);
            flag = true;
          }
        }
      }
      return flag;
    }

    private static bool RelationalOperatorPlaceholder<TLeft, TRight>(TLeft left, TRight right) => (object) left == (object) right;

    private static BinaryExpression CreateRelationalOperator(
      ExpressionType op,
      Expression left,
      Expression right)
    {
      BinaryExpression result;
      ExpressionNormalizer.TryCreateRelationalOperator(op, left, right, out result);
      return result;
    }

    private static bool TryCreateRelationalOperator(
      ExpressionType op,
      Expression left,
      Expression right,
      out BinaryExpression result)
    {
      MethodInfo method = ExpressionNormalizer.StaticRelationalOperatorPlaceholderMethod.MakeGenericMethod(left.Type, right.Type);
      switch (op)
      {
        case ExpressionType.Equal:
          result = Expression.Equal(left, right, false, method);
          return true;
        case ExpressionType.GreaterThan:
          result = Expression.GreaterThan(left, right, false, method);
          return true;
        case ExpressionType.GreaterThanOrEqual:
          result = Expression.GreaterThanOrEqual(left, right, false, method);
          return true;
        case ExpressionType.LessThan:
          result = Expression.LessThan(left, right, false, method);
          return true;
        case ExpressionType.LessThanOrEqual:
          result = Expression.LessThanOrEqual(left, right, false, method);
          return true;
        case ExpressionType.NotEqual:
          result = Expression.NotEqual(left, right, false, method);
          return true;
        default:
          result = (BinaryExpression) null;
          return false;
      }
    }

    private Expression CreateCompareExpression(Expression left, Expression right)
    {
      Expression key = (Expression) Expression.Condition((Expression) ExpressionNormalizer.CreateRelationalOperator(ExpressionType.Equal, left, right), (Expression) Expression.Constant((object) 0), (Expression) Expression.Condition((Expression) ExpressionNormalizer.CreateRelationalOperator(ExpressionType.GreaterThan, left, right), (Expression) Expression.Constant((object) 1), (Expression) Expression.Constant((object) -1)));
      this.patterns[key] = (ExpressionNormalizer.Pattern) new ExpressionNormalizer.ComparePattern(left, right);
      return key;
    }

    private void RecordRewrite(Expression source, Expression rewritten)
    {
      if (source == rewritten)
        return;
      this.NormalizerRewrites.Add(rewritten, source);
    }

    private abstract class Pattern
    {
      internal abstract ExpressionNormalizer.PatternKind Kind { get; }
    }

    private enum PatternKind
    {
      Compare,
    }

    private sealed class ComparePattern : ExpressionNormalizer.Pattern
    {
      internal readonly Expression Left;
      internal readonly Expression Right;

      internal ComparePattern(Expression left, Expression right)
      {
        this.Left = left;
        this.Right = right;
      }

      internal override ExpressionNormalizer.PatternKind Kind => ExpressionNormalizer.PatternKind.Compare;
    }
  }
}
