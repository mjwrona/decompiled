// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.ProjectionAnalyzer
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal static class ProjectionAnalyzer
  {
    internal static bool Analyze(LambdaExpression le, ResourceExpression re, bool matchMembers)
    {
      if (le.Body.NodeType == ExpressionType.Constant)
      {
        if (CommonUtil.IsClientType(le.Body.Type))
          throw new NotSupportedException("Referencing of local entity type instances not supported when projecting results.");
        re.Projection = new ProjectionQueryOptionExpression(le.Body.Type, le, new List<string>());
        return true;
      }
      if (le.Body.NodeType == ExpressionType.Call)
      {
        MethodCallExpression body = le.Body as MethodCallExpression;
        if (body.Method == ReflectionUtil.ProjectMethodInfo.MakeGenericMethod(le.Body.Type))
        {
          ConstantExpression constantExpression = body.Arguments[1] as ConstantExpression;
          re.Projection = new ProjectionQueryOptionExpression(le.Body.Type, ProjectionQueryOptionExpression.DefaultLambda, new List<string>((IEnumerable<string>) (string[]) constantExpression.Value));
          return true;
        }
      }
      if (le.Body.NodeType == ExpressionType.MemberInit || le.Body.NodeType == ExpressionType.New)
      {
        ProjectionAnalyzer.AnalyzeResourceExpression(le, re);
        return true;
      }
      if (!matchMembers || ProjectionAnalyzer.SkipConverts(le.Body).NodeType != ExpressionType.MemberAccess)
        return false;
      ProjectionAnalyzer.AnalyzeResourceExpression(le, re);
      return true;
    }

    internal static void Analyze(LambdaExpression e, PathBox pb)
    {
      int num = CommonUtil.IsClientType(e.Body.Type) ? 1 : 0;
      pb.PushParamExpression(e.Parameters.Last<ParameterExpression>());
      if (num == 0)
      {
        ProjectionAnalyzer.NonEntityProjectionAnalyzer.Analyze(e.Body, pb);
      }
      else
      {
        switch (e.Body.NodeType)
        {
          case ExpressionType.Constant:
            throw new NotSupportedException("Referencing of local entity type instances not supported when projecting results.");
          case ExpressionType.MemberInit:
            ProjectionAnalyzer.EntityProjectionAnalyzer.Analyze((MemberInitExpression) e.Body, pb);
            break;
          case ExpressionType.New:
            throw new NotSupportedException("Construction of entity type instances must use object initializer with default constructor.");
          default:
            ProjectionAnalyzer.NonEntityProjectionAnalyzer.Analyze(e.Body, pb);
            break;
        }
      }
      pb.PopParamExpression();
    }

    internal static bool IsMethodCallAllowedEntitySequence(MethodCallExpression call) => ReflectionUtil.IsSequenceMethod(call.Method, SequenceMethod.ToList) || ReflectionUtil.IsSequenceMethod(call.Method, SequenceMethod.Select);

    internal static void CheckChainedSequence(MethodCallExpression call, Type type)
    {
      if (!ReflectionUtil.IsSequenceMethod(call.Method, SequenceMethod.Select))
        return;
      MethodCallExpression methodCallExpression = ResourceBinder.StripTo<MethodCallExpression>(call.Arguments[0]);
      if (methodCallExpression != null && ReflectionUtil.IsSequenceMethod(methodCallExpression.Method, SequenceMethod.Select))
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Constructing or initializing instances of the type {0} with the expression {1} is not supported.", (object) type, (object) call.ToString()));
    }

    private static void Analyze(MemberInitExpression mie, PathBox pb)
    {
      if (CommonUtil.IsClientType(mie.Type))
        ProjectionAnalyzer.EntityProjectionAnalyzer.Analyze(mie, pb);
      else
        ProjectionAnalyzer.NonEntityProjectionAnalyzer.Analyze((Expression) mie, pb);
    }

    private static void AnalyzeResourceExpression(
      LambdaExpression lambda,
      ResourceExpression resource)
    {
      PathBox pb = new PathBox();
      ProjectionAnalyzer.Analyze(lambda, pb);
      resource.Projection = new ProjectionQueryOptionExpression(lambda.Body.Type, lambda, pb.ProjectionPaths.ToList<string>());
      resource.ExpandPaths = pb.ExpandPaths.Union<string>((IEnumerable<string>) resource.ExpandPaths, (IEqualityComparer<string>) StringComparer.Ordinal).ToList<string>();
    }

    private static Expression SkipConverts(Expression expression)
    {
      Expression expression1 = expression;
      while (expression1.NodeType == ExpressionType.Convert || expression1.NodeType == ExpressionType.ConvertChecked)
        expression1 = ((UnaryExpression) expression1).Operand;
      return expression1;
    }

    private class EntityProjectionAnalyzer : ALinqExpressionVisitor
    {
      private readonly PathBox box;
      private readonly Type type;

      private EntityProjectionAnalyzer(PathBox pb, Type type)
      {
        this.box = pb;
        this.type = type;
      }

      internal static void Analyze(MemberInitExpression mie, PathBox pb)
      {
        ProjectionAnalyzer.EntityProjectionAnalyzer projectionAnalyzer = new ProjectionAnalyzer.EntityProjectionAnalyzer(pb, mie.Type);
        MemberAssignmentAnalysis previous = (MemberAssignmentAnalysis) null;
        foreach (MemberBinding binding in mie.Bindings)
        {
          MemberAssignment memberAssignment = binding as MemberAssignment;
          projectionAnalyzer.Visit(memberAssignment.Expression);
          if (memberAssignment != null)
          {
            MemberAssignmentAnalysis assignmentAnalysis = MemberAssignmentAnalysis.Analyze((Expression) pb.ParamExpressionInScope, memberAssignment.Expression);
            if (assignmentAnalysis.IncompatibleAssignmentsException != null)
              throw assignmentAnalysis.IncompatibleAssignmentsException;
            Type memberType = ProjectionAnalyzer.EntityProjectionAnalyzer.GetMemberType(memberAssignment.Member);
            Expression[] beyondTargetEntity = assignmentAnalysis.GetExpressionsBeyondTargetEntity();
            if (beyondTargetEntity.Length == 0)
              throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Initializing instances of the entity type {0} with the expression {1} is not supported.", (object) memberType, (object) memberAssignment.Expression));
            if (beyondTargetEntity[beyondTargetEntity.Length - 1] is MemberExpression memberExpression && memberExpression.Member.Name != memberAssignment.Member.Name)
              throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot assign the value from the {0} property to the {1} property.  When projecting results into a entity type, the property names of the source type and the target type must match for the properties being projected.", (object) memberExpression.Member.Name, (object) memberAssignment.Member.Name));
            assignmentAnalysis.CheckCompatibleAssignments(mie.Type, ref previous);
            bool flag = CommonUtil.IsClientType(memberType);
            if (CommonUtil.IsClientType(memberExpression.Type) && !flag)
              throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Constructing or initializing instances of the type {0} with the expression {1} is not supported.", (object) memberType, (object) memberAssignment.Expression));
          }
        }
      }

      internal override Expression VisitUnary(UnaryExpression u)
      {
        if (ResourceBinder.PatternRules.MatchConvertToAssignable(u))
          return base.VisitUnary(u);
        if (u.NodeType == ExpressionType.Convert || u.NodeType == ExpressionType.ConvertChecked)
        {
          Type type1 = Nullable.GetUnderlyingType(u.Operand.Type);
          if ((object) type1 == null)
            type1 = u.Operand.Type;
          Type type2 = Nullable.GetUnderlyingType(u.Type);
          if ((object) type2 == null)
            type2 = u.Type;
          Type type3 = type2;
          if (ClientConvert.IsKnownType(type1) && ClientConvert.IsKnownType(type3))
            return this.Visit(u.Operand);
        }
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Initializing instances of the entity type {0} with the expression {1} is not supported.", (object) this.type, (object) u.ToString()));
      }

      internal override Expression VisitBinary(BinaryExpression b) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Initializing instances of the entity type {0} with the expression {1} is not supported.", (object) this.type, (object) b.ToString()));

      internal override Expression VisitTypeIs(TypeBinaryExpression b) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Initializing instances of the entity type {0} with the expression {1} is not supported.", (object) this.type, (object) b.ToString()));

      internal override Expression VisitConditional(ConditionalExpression c)
      {
        ResourceBinder.PatternRules.MatchNullCheckResult matchNullCheckResult = ResourceBinder.PatternRules.MatchNullCheck((Expression) this.box.ParamExpressionInScope, c);
        if (!matchNullCheckResult.Match)
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Initializing instances of the entity type {0} with the expression {1} is not supported.", (object) this.type, (object) c.ToString()));
        this.Visit(matchNullCheckResult.AssignExpression);
        return (Expression) c;
      }

      internal override Expression VisitConstant(ConstantExpression c) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Initializing instances of the entity type {0} with the expression {1} is not supported.", (object) this.type, (object) c.ToString()));

      internal override Expression VisitMemberAccess(MemberExpression m)
      {
        if (!CommonUtil.IsClientType(m.Expression.Type))
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Initializing instances of the entity type {0} with the expression {1} is not supported.", (object) this.type, (object) m.ToString()));
        PropertyInfo propInfo = (PropertyInfo) null;
        Expression expression = ResourceBinder.PatternRules.MatchNonPrivateReadableProperty(m, out propInfo) ? base.VisitMemberAccess(m) : throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Initializing instances of the entity type {0} with the expression {1} is not supported.", (object) this.type, (object) m.ToString()));
        this.box.AppendToPath(propInfo);
        return expression;
      }

      internal override Expression VisitMethodCall(MethodCallExpression m)
      {
        if (!ProjectionAnalyzer.IsMethodCallAllowedEntitySequence(m))
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Initializing instances of the entity type {0} with the expression {1} is not supported.", (object) this.type, (object) m.ToString()));
        ProjectionAnalyzer.CheckChainedSequence(m, this.type);
        return base.VisitMethodCall(m);
      }

      internal override Expression VisitInvocation(InvocationExpression iv) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Initializing instances of the entity type {0} with the expression {1} is not supported.", (object) this.type, (object) iv.ToString()));

      internal override Expression VisitLambda(LambdaExpression lambda)
      {
        ProjectionAnalyzer.Analyze(lambda, this.box);
        return (Expression) lambda;
      }

      internal override Expression VisitListInit(ListInitExpression init) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Initializing instances of the entity type {0} with the expression {1} is not supported.", (object) this.type, (object) init.ToString()));

      internal override Expression VisitNewArray(NewArrayExpression na) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Initializing instances of the entity type {0} with the expression {1} is not supported.", (object) this.type, (object) na.ToString()));

      internal override Expression VisitMemberInit(MemberInitExpression init)
      {
        ProjectionAnalyzer.Analyze(init, this.box);
        return (Expression) init;
      }

      internal override NewExpression VisitNew(NewExpression nex) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Initializing instances of the entity type {0} with the expression {1} is not supported.", (object) this.type, (object) nex.ToString()));

      internal override Expression VisitParameter(ParameterExpression p)
      {
        if (p != this.box.ParamExpressionInScope)
          throw new NotSupportedException("Can only project the last entity type in the query being translated.");
        this.box.StartNewPath();
        return (Expression) p;
      }

      private static Type GetMemberType(MemberInfo member)
      {
        PropertyInfo propertyInfo = member as PropertyInfo;
        return propertyInfo != (PropertyInfo) null ? propertyInfo.PropertyType : (member as FieldInfo).FieldType;
      }
    }

    private class NonEntityProjectionAnalyzer : DataServiceALinqExpressionVisitor
    {
      private PathBox box;
      private Type type;

      private NonEntityProjectionAnalyzer(PathBox pb, Type type)
      {
        this.box = pb;
        this.type = type;
      }

      internal static void Analyze(Expression e, PathBox pb)
      {
        ProjectionAnalyzer.NonEntityProjectionAnalyzer projectionAnalyzer = new ProjectionAnalyzer.NonEntityProjectionAnalyzer(pb, e.Type);
        if (e is MemberInitExpression memberInitExpression)
        {
          foreach (MemberBinding binding in memberInitExpression.Bindings)
          {
            if (binding is MemberAssignment memberAssignment)
              projectionAnalyzer.Visit(memberAssignment.Expression);
          }
        }
        else
          projectionAnalyzer.Visit(e);
      }

      internal override Expression VisitUnary(UnaryExpression u)
      {
        if (!ResourceBinder.PatternRules.MatchConvertToAssignable(u) && CommonUtil.IsClientType(u.Operand.Type))
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Constructing or initializing instances of the type {0} with the expression {1} is not supported.", (object) this.type, (object) u.ToString()));
        return base.VisitUnary(u);
      }

      internal override Expression VisitBinary(BinaryExpression b)
      {
        if (CommonUtil.IsClientType(b.Left.Type) || CommonUtil.IsClientType(b.Right.Type))
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Constructing or initializing instances of the type {0} with the expression {1} is not supported.", (object) this.type, (object) b.ToString()));
        return base.VisitBinary(b);
      }

      internal override Expression VisitTypeIs(TypeBinaryExpression b)
      {
        if (CommonUtil.IsClientType(b.Expression.Type))
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Constructing or initializing instances of the type {0} with the expression {1} is not supported.", (object) this.type, (object) b.ToString()));
        return base.VisitTypeIs(b);
      }

      internal override Expression VisitConditional(ConditionalExpression c)
      {
        ResourceBinder.PatternRules.MatchNullCheckResult matchNullCheckResult = ResourceBinder.PatternRules.MatchNullCheck((Expression) this.box.ParamExpressionInScope, c);
        if (matchNullCheckResult.Match)
        {
          this.Visit(matchNullCheckResult.AssignExpression);
          return (Expression) c;
        }
        if (CommonUtil.IsClientType(c.Test.Type) || CommonUtil.IsClientType(c.IfTrue.Type) || CommonUtil.IsClientType(c.IfFalse.Type))
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Constructing or initializing instances of the type {0} with the expression {1} is not supported.", (object) this.type, (object) c.ToString()));
        return base.VisitConditional(c);
      }

      internal override Expression VisitMemberAccess(MemberExpression m)
      {
        if (ClientConvert.IsKnownNullableType(m.Expression.Type))
          return base.VisitMemberAccess(m);
        if (!CommonUtil.IsClientType(m.Expression.Type))
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Constructing or initializing instances of the type {0} with the expression {1} is not supported.", (object) this.type, (object) m.ToString()));
        PropertyInfo propInfo = (PropertyInfo) null;
        Expression expression = ResourceBinder.PatternRules.MatchNonPrivateReadableProperty(m, out propInfo) ? base.VisitMemberAccess(m) : throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Constructing or initializing instances of the type {0} with the expression {1} is not supported.", (object) this.type, (object) m.ToString()));
        this.box.AppendToPath(propInfo);
        return expression;
      }

      internal override Expression VisitMethodCall(MethodCallExpression m)
      {
        if (ProjectionAnalyzer.IsMethodCallAllowedEntitySequence(m))
        {
          ProjectionAnalyzer.CheckChainedSequence(m, this.type);
          return base.VisitMethodCall(m);
        }
        if ((m.Object != null ? (CommonUtil.IsClientType(m.Object.Type) ? 1 : 0) : 0) != 0 || m.Arguments.Any<Expression>((Func<Expression, bool>) (a => CommonUtil.IsClientType(a.Type))))
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Constructing or initializing instances of the type {0} with the expression {1} is not supported.", (object) this.type, (object) m.ToString()));
        return base.VisitMethodCall(m);
      }

      internal override Expression VisitInvocation(InvocationExpression iv)
      {
        if (CommonUtil.IsClientType(iv.Expression.Type) || iv.Arguments.Any<Expression>((Func<Expression, bool>) (a => CommonUtil.IsClientType(a.Type))))
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Constructing or initializing instances of the type {0} with the expression {1} is not supported.", (object) this.type, (object) iv.ToString()));
        return base.VisitInvocation(iv);
      }

      internal override Expression VisitLambda(LambdaExpression lambda)
      {
        ProjectionAnalyzer.Analyze(lambda, this.box);
        return (Expression) lambda;
      }

      internal override Expression VisitMemberInit(MemberInitExpression init)
      {
        ProjectionAnalyzer.Analyze(init, this.box);
        return (Expression) init;
      }

      internal override NewExpression VisitNew(NewExpression nex) => !CommonUtil.IsClientType(nex.Type) ? base.VisitNew(nex) : throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Constructing or initializing instances of the type {0} with the expression {1} is not supported.", (object) this.type, (object) nex.ToString()));

      internal override Expression VisitParameter(ParameterExpression p)
      {
        if (p != this.box.ParamExpressionInScope)
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Constructing or initializing instances of the type {0} with the expression {1} is not supported.", (object) this.type, (object) p.ToString()));
        this.box.StartNewPath();
        return (Expression) p;
      }

      internal override Expression VisitConstant(ConstantExpression c) => !CommonUtil.IsClientType(c.Type) ? base.VisitConstant(c) : throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Constructing or initializing instances of the type {0} with the expression {1} is not supported.", (object) this.type, (object) c.ToString()));
    }
  }
}
