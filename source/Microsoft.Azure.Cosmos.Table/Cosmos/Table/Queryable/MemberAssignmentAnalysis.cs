// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.MemberAssignmentAnalysis
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal class MemberAssignmentAnalysis : ALinqExpressionVisitor
  {
    internal static readonly Expression[] EmptyExpressionArray = new Expression[0];
    private readonly Expression entity;
    private Exception incompatibleAssignmentsException;
    private bool multiplePathsFound;
    private List<Expression> pathFromEntity;

    private MemberAssignmentAnalysis(Expression entity)
    {
      this.entity = entity;
      this.pathFromEntity = new List<Expression>();
    }

    internal Exception IncompatibleAssignmentsException => this.incompatibleAssignmentsException;

    internal bool MultiplePathsFound => this.multiplePathsFound;

    internal static MemberAssignmentAnalysis Analyze(
      Expression entityInScope,
      Expression assignmentExpression)
    {
      MemberAssignmentAnalysis assignmentAnalysis = new MemberAssignmentAnalysis(entityInScope);
      assignmentAnalysis.Visit(assignmentExpression);
      return assignmentAnalysis;
    }

    internal Exception CheckCompatibleAssignments(
      Type targetType,
      ref MemberAssignmentAnalysis previous)
    {
      if (previous == null)
      {
        previous = this;
        return (Exception) null;
      }
      Expression[] expressionsToTargetEntity1 = previous.GetExpressionsToTargetEntity();
      Expression[] expressionsToTargetEntity2 = this.GetExpressionsToTargetEntity();
      return MemberAssignmentAnalysis.CheckCompatibleAssignments(targetType, expressionsToTargetEntity1, expressionsToTargetEntity2);
    }

    internal override Expression Visit(Expression expression) => this.multiplePathsFound || this.incompatibleAssignmentsException != null ? expression : base.Visit(expression);

    internal override Expression VisitConditional(ConditionalExpression c)
    {
      ResourceBinder.PatternRules.MatchNullCheckResult matchNullCheckResult = ResourceBinder.PatternRules.MatchNullCheck(this.entity, c);
      Expression expression;
      if (matchNullCheckResult.Match)
      {
        this.Visit(matchNullCheckResult.AssignExpression);
        expression = (Expression) c;
      }
      else
        expression = base.VisitConditional(c);
      return expression;
    }

    internal override Expression VisitParameter(ParameterExpression p)
    {
      if (p == this.entity)
      {
        if (this.pathFromEntity.Count != 0)
          this.multiplePathsFound = true;
        else
          this.pathFromEntity.Add((Expression) p);
      }
      return (Expression) p;
    }

    internal override Expression VisitMemberInit(MemberInitExpression init)
    {
      Expression expression = (Expression) init;
      MemberAssignmentAnalysis previous = (MemberAssignmentAnalysis) null;
      foreach (MemberBinding binding in init.Bindings)
      {
        if (binding is MemberAssignment memberAssignment)
        {
          MemberAssignmentAnalysis assignmentAnalysis = MemberAssignmentAnalysis.Analyze(this.entity, memberAssignment.Expression);
          if (assignmentAnalysis.MultiplePathsFound)
          {
            this.multiplePathsFound = true;
            break;
          }
          Exception exception = assignmentAnalysis.CheckCompatibleAssignments(init.Type, ref previous);
          if (exception != null)
          {
            this.incompatibleAssignmentsException = exception;
            break;
          }
          if (this.pathFromEntity.Count == 0)
            this.pathFromEntity.AddRange((IEnumerable<Expression>) assignmentAnalysis.GetExpressionsToTargetEntity());
        }
      }
      return expression;
    }

    internal override Expression VisitMemberAccess(MemberExpression m)
    {
      Expression expression = base.VisitMemberAccess(m);
      if (!this.pathFromEntity.Contains(m.Expression))
        return expression;
      this.pathFromEntity.Add((Expression) m);
      return expression;
    }

    internal override Expression VisitMethodCall(MethodCallExpression call)
    {
      if (!ReflectionUtil.IsSequenceMethod(call.Method, SequenceMethod.Select))
        return base.VisitMethodCall(call);
      this.Visit(call.Arguments[0]);
      return (Expression) call;
    }

    internal Expression[] GetExpressionsBeyondTargetEntity()
    {
      if (this.pathFromEntity.Count <= 1)
        return MemberAssignmentAnalysis.EmptyExpressionArray;
      return new Expression[1]
      {
        this.pathFromEntity[this.pathFromEntity.Count - 1]
      };
    }

    internal Expression[] GetExpressionsToTargetEntity()
    {
      if (this.pathFromEntity.Count <= 1)
        return MemberAssignmentAnalysis.EmptyExpressionArray;
      Expression[] expressionsToTargetEntity = new Expression[this.pathFromEntity.Count - 1];
      for (int index = 0; index < expressionsToTargetEntity.Length; ++index)
        expressionsToTargetEntity[index] = this.pathFromEntity[index];
      return expressionsToTargetEntity;
    }

    private static Exception CheckCompatibleAssignments(
      Type targetType,
      Expression[] previous,
      Expression[] candidate)
    {
      if (previous.Length != candidate.Length)
        throw MemberAssignmentAnalysis.CheckCompatibleAssignmentsFail(targetType, previous, candidate);
      for (int index = 0; index < previous.Length; ++index)
      {
        Expression previou = previous[index];
        Expression expression = candidate[index];
        if (previou.NodeType != expression.NodeType)
          throw MemberAssignmentAnalysis.CheckCompatibleAssignmentsFail(targetType, previous, candidate);
        if (previou != expression && (previou.NodeType != ExpressionType.MemberAccess || ((MemberExpression) previou).Member.Name != ((MemberExpression) expression).Member.Name))
          return MemberAssignmentAnalysis.CheckCompatibleAssignmentsFail(targetType, previous, candidate);
      }
      return (Exception) null;
    }

    private static Exception CheckCompatibleAssignmentsFail(
      Type targetType,
      Expression[] previous,
      Expression[] candidate)
    {
      return (Exception) new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot initialize an instance of entity type '{0}' because '{1}' and '{2}' do not refer to the same source entity.", (object) targetType.FullName, (object) ((IEnumerable<Expression>) previous).LastOrDefault<Expression>(), (object) ((IEnumerable<Expression>) candidate).LastOrDefault<Expression>()));
    }
  }
}
