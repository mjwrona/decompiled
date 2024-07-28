// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.ResourceBinder
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal class ResourceBinder : DataServiceALinqExpressionVisitor
  {
    internal static Expression Bind(Expression e)
    {
      Expression expression = new ResourceBinder().Visit(e);
      ResourceBinder.VerifyKeyPredicates(expression);
      ResourceBinder.VerifyNotSelectManyProjection(expression);
      return expression;
    }

    internal static bool IsMissingKeyPredicates(Expression expression) => expression is ResourceExpression resourceExpression && (ResourceBinder.IsMissingKeyPredicates(resourceExpression.Source) || resourceExpression.Source != null && resourceExpression.Source is ResourceSetExpression source && !source.HasKeyPredicate);

    internal static void VerifyKeyPredicates(Expression e)
    {
      if (ResourceBinder.IsMissingKeyPredicates(e))
        throw new NotSupportedException("Navigation properties can only be selected from a single resource. Specify a key predicate to restrict the entity set to a single instance.");
    }

    internal static void VerifyNotSelectManyProjection(Expression expression)
    {
      if (!(expression is ResourceSetExpression resourceSetExpression))
        return;
      ProjectionQueryOptionExpression projection = resourceSetExpression.Projection;
      if (projection != null)
      {
        MethodCallExpression methodCallExpression = ResourceBinder.StripTo<MethodCallExpression>(projection.Selector.Body);
        if (methodCallExpression != null && methodCallExpression.Method.Name == "SelectMany")
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The expression type {0} is not supported.", (object) methodCallExpression));
      }
      else if (resourceSetExpression.HasTransparentScope)
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The expression type {0} is not supported.", (object) resourceSetExpression));
    }

    private static Expression AnalyzePredicate(MethodCallExpression mce)
    {
      ResourceSetExpression input;
      LambdaExpression lambda;
      if (!ResourceBinder.TryGetResourceSetMethodArguments(mce, out input, out lambda))
      {
        ResourceBinder.ValidationRules.RequireNonSingleton(mce.Arguments[0]);
        return (Expression) mce;
      }
      List<Expression> conjuncts = new List<Expression>();
      ResourceBinder.AddConjuncts(lambda.Body, conjuncts);
      Dictionary<ResourceSetExpression, List<Expression>> dictionary = new Dictionary<ResourceSetExpression, List<Expression>>((IEqualityComparer<ResourceSetExpression>) ReferenceEqualityComparer<ResourceSetExpression>.Instance);
      List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
      foreach (Expression e in conjuncts)
      {
        Expression expression = InputBinder.Bind(e, (ResourceExpression) input, lambda.Parameters[0], referencedInputs);
        if (referencedInputs.Count > 1)
          return (Expression) mce;
        ResourceSetExpression key = referencedInputs.Count == 0 ? input : referencedInputs[0] as ResourceSetExpression;
        if (key == null)
          return (Expression) mce;
        List<Expression> expressionList = (List<Expression>) null;
        if (!dictionary.TryGetValue(key, out expressionList))
        {
          expressionList = new List<Expression>();
          dictionary[key] = expressionList;
        }
        expressionList.Add(expression);
        referencedInputs.Clear();
      }
      List<Expression> expressionList1;
      if (dictionary.TryGetValue(input, out expressionList1))
        dictionary.Remove(input);
      else
        expressionList1 = (List<Expression>) null;
      if (expressionList1 != null && expressionList1.Count > 0)
      {
        if (input.KeyPredicate != null)
        {
          Expression expression = ResourceBinder.BuildKeyPredicateFilter(input.CreateReference(), input.KeyPredicate);
          expressionList1.Add(expression);
          input.KeyPredicate = (Dictionary<PropertyInfo, ConstantExpression>) null;
        }
        int num;
        Expression expression1;
        if (input.Filter != null)
        {
          num = 0;
          expression1 = input.Filter.Predicate;
        }
        else
        {
          num = 1;
          expression1 = expressionList1[0];
        }
        for (int index = num; index < expressionList1.Count; ++index)
          expression1 = (Expression) Expression.And(expression1, expressionList1[index]);
        ResourceBinder.AddSequenceQueryOption((ResourceExpression) input, (QueryOptionExpression) new FilterQueryOptionExpression(mce.Method.ReturnType, expression1));
      }
      return (Expression) input;
    }

    private static Expression BuildKeyPredicateFilter(
      InputReferenceExpression input,
      Dictionary<PropertyInfo, ConstantExpression> keyValuesDictionary)
    {
      Expression left = (Expression) null;
      foreach (KeyValuePair<PropertyInfo, ConstantExpression> keyValues in keyValuesDictionary)
      {
        Expression right = (Expression) Expression.Equal((Expression) Expression.Property((Expression) input, keyValues.Key), (Expression) keyValues.Value);
        left = left != null ? (Expression) Expression.And(left, right) : right;
      }
      return left;
    }

    private static void AddConjuncts(Expression e, List<Expression> conjuncts)
    {
      if (ResourceBinder.PatternRules.MatchAnd(e))
      {
        BinaryExpression binaryExpression = (BinaryExpression) e;
        ResourceBinder.AddConjuncts(binaryExpression.Left, conjuncts);
        ResourceBinder.AddConjuncts(binaryExpression.Right, conjuncts);
      }
      else
        conjuncts.Add(e);
    }

    internal bool AnalyzeProjection(
      MethodCallExpression mce,
      SequenceMethod sequenceMethod,
      out Expression e)
    {
      e = (Expression) mce;
      bool matchMembers = true;
      if (!(this.Visit(mce.Arguments[0]) is ResourceExpression resourceExpression))
        return false;
      if (sequenceMethod == SequenceMethod.SelectManyResultSelector)
      {
        Expression expression = mce.Arguments[1];
        LambdaExpression lambda;
        if (!ResourceBinder.PatternRules.MatchParameterMemberAccess(expression) || !ResourceBinder.PatternRules.MatchDoubleArgumentLambda(mce.Arguments[2], out lambda) || ResourceBinder.ExpressionPresenceVisitor.IsExpressionPresent((Expression) lambda.Parameters[0], lambda.Body))
          return false;
        List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
        LambdaExpression lambdaExpression = ResourceBinder.StripTo<LambdaExpression>(expression);
        Expression potentialPropertyRef = ResourceBinder.StripCastMethodCalls(InputBinder.Bind(lambdaExpression.Body, resourceExpression, lambdaExpression.Parameters[0], referencedInputs));
        MemberExpression navigationMember;
        if (!ResourceBinder.PatternRules.MatchPropertyProjectionSet(resourceExpression, potentialPropertyRef, out navigationMember))
          return false;
        Expression memberExpression = (Expression) navigationMember;
        ResourceExpression resourceSetExpression = (ResourceExpression) ResourceBinder.CreateResourceSetExpression(mce.Method.ReturnType, resourceExpression, memberExpression, TypeSystem.GetElementType(memberExpression.Type));
        if (!ResourceBinder.PatternRules.MatchMemberInitExpressionWithDefaultConstructor((Expression) resourceSetExpression, lambda) && !ResourceBinder.PatternRules.MatchNewExpression((Expression) resourceSetExpression, lambda))
          return false;
        lambda = Expression.Lambda(lambda.Body, lambda.Parameters[1]);
        ResourceExpression cloneWithNewType = resourceSetExpression.CreateCloneWithNewType(mce.Type);
        bool flag;
        try
        {
          flag = ProjectionAnalyzer.Analyze(lambda, cloneWithNewType, false);
        }
        catch (NotSupportedException ex)
        {
          flag = false;
        }
        if (!flag)
          return false;
        e = (Expression) cloneWithNewType;
        ResourceBinder.ValidationRules.RequireCanProject((Expression) resourceSetExpression);
      }
      else
      {
        LambdaExpression lambda;
        if (!ResourceBinder.PatternRules.MatchSingleArgumentLambda(mce.Arguments[1], out lambda))
          return false;
        lambda = ProjectionRewriter.TryToRewrite(lambda, resourceExpression.ResourceType);
        ResourceExpression cloneWithNewType = resourceExpression.CreateCloneWithNewType(mce.Type);
        if (!ProjectionAnalyzer.Analyze(lambda, cloneWithNewType, matchMembers))
          return false;
        ResourceBinder.ValidationRules.RequireCanProject((Expression) resourceExpression);
        e = (Expression) cloneWithNewType;
      }
      return true;
    }

    internal static Expression AnalyzeNavigation(MethodCallExpression mce)
    {
      Expression input = mce.Arguments[0];
      LambdaExpression lambda;
      if (!ResourceBinder.PatternRules.MatchSingleArgumentLambda(mce.Arguments[1], out lambda))
        return (Expression) mce;
      if (ResourceBinder.PatternRules.MatchIdentitySelector(lambda))
        return input;
      if (ResourceBinder.PatternRules.MatchTransparentIdentitySelector(input, lambda))
        return (Expression) ResourceBinder.RemoveTransparentScope(mce.Method.ReturnType, (ResourceSetExpression) input);
      ResourceExpression sourceExpression;
      Expression bound;
      MemberExpression navigationMember;
      if (!ResourceBinder.IsValidNavigationSource(input, out sourceExpression) || !ResourceBinder.TryBindToInput(sourceExpression, lambda, out bound) || !ResourceBinder.PatternRules.MatchPropertyProjectionSingleton(sourceExpression, bound, out navigationMember))
        return (Expression) mce;
      Expression memberExpression = (Expression) navigationMember;
      return (Expression) ResourceBinder.CreateNavigationPropertySingletonExpression(mce.Method.ReturnType, sourceExpression, memberExpression);
    }

    private static bool IsValidNavigationSource(
      Expression input,
      out ResourceExpression sourceExpression)
    {
      ResourceBinder.ValidationRules.RequireCanNavigate(input);
      sourceExpression = input as ResourceExpression;
      return sourceExpression != null;
    }

    private static Expression LimitCardinality(MethodCallExpression mce, int maxCardinality)
    {
      if (mce.Arguments.Count != 1)
        return (Expression) mce;
      if (mce.Arguments[0] is ResourceSetExpression target)
      {
        if (!target.HasKeyPredicate && target.NodeType != (ExpressionType) 10001 && (target.Take == null || (int) target.Take.TakeAmount.Value > maxCardinality))
          ResourceBinder.AddSequenceQueryOption((ResourceExpression) target, (QueryOptionExpression) new TakeQueryOptionExpression(mce.Type, Expression.Constant((object) maxCardinality)));
        return mce.Arguments[0];
      }
      return mce.Arguments[0] is NavigationPropertySingletonExpression ? mce.Arguments[0] : (Expression) mce;
    }

    private static Expression AnalyzeCast(MethodCallExpression mce) => mce.Arguments[0] is ResourceExpression resourceExpression ? (Expression) resourceExpression.CreateCloneWithNewType(mce.Method.ReturnType) : (Expression) mce;

    private static ResourceSetExpression CreateResourceSetExpression(
      Type type,
      ResourceExpression source,
      Expression memberExpression,
      Type resourceType)
    {
      ResourceSetExpression resourceSetExpression = new ResourceSetExpression(typeof (IOrderedQueryable<>).MakeGenericType(TypeSystem.GetElementType(type)), (Expression) source, memberExpression, resourceType, source.ExpandPaths.ToList<string>(), source.CountOption, source.CustomQueryOptions.ToDictionary<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression, ConstantExpression>((Func<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression>) (kvp => kvp.Key), (Func<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression>) (kvp => kvp.Value)), (ProjectionQueryOptionExpression) null);
      source.ExpandPaths.Clear();
      source.CountOption = CountOption.None;
      source.CustomQueryOptions.Clear();
      return resourceSetExpression;
    }

    private static NavigationPropertySingletonExpression CreateNavigationPropertySingletonExpression(
      Type type,
      ResourceExpression source,
      Expression memberExpression)
    {
      NavigationPropertySingletonExpression singletonExpression = new NavigationPropertySingletonExpression(type, (Expression) source, memberExpression, memberExpression.Type, source.ExpandPaths.ToList<string>(), source.CountOption, source.CustomQueryOptions.ToDictionary<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression, ConstantExpression>((Func<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression>) (kvp => kvp.Key), (Func<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression>) (kvp => kvp.Value)), (ProjectionQueryOptionExpression) null);
      source.ExpandPaths.Clear();
      source.CountOption = CountOption.None;
      source.CustomQueryOptions.Clear();
      return singletonExpression;
    }

    private static ResourceSetExpression RemoveTransparentScope(
      Type expectedResultType,
      ResourceSetExpression input)
    {
      ResourceSetExpression resourceSetExpression = new ResourceSetExpression(expectedResultType, input.Source, input.MemberExpression, input.ResourceType, input.ExpandPaths, input.CountOption, input.CustomQueryOptions, input.Projection);
      resourceSetExpression.KeyPredicate = input.KeyPredicate;
      foreach (QueryOptionExpression sequenceQueryOption in input.SequenceQueryOptions)
        resourceSetExpression.AddSequenceQueryOption(sequenceQueryOption);
      resourceSetExpression.OverrideInputReference(input);
      return resourceSetExpression;
    }

    internal static Expression StripConvertToAssignable(Expression e) => !(e is UnaryExpression expression) || !ResourceBinder.PatternRules.MatchConvertToAssignable(expression) ? e : expression.Operand;

    internal static T StripTo<T>(Expression expression) where T : Expression
    {
      Expression expression1;
      do
      {
        expression1 = expression;
        expression = expression.NodeType == ExpressionType.Quote ? ((UnaryExpression) expression).Operand : expression;
        expression = ResourceBinder.StripConvertToAssignable(expression);
      }
      while (expression1 != expression);
      return expression1 as T;
    }

    internal override Expression VisitResourceSetExpression(ResourceSetExpression rse) => rse.NodeType == (ExpressionType) 10000 ? (Expression) new ResourceSetExpression(rse.Type, rse.Source, rse.MemberExpression, rse.ResourceType, (List<string>) null, CountOption.None, (Dictionary<ConstantExpression, ConstantExpression>) null, (ProjectionQueryOptionExpression) null) : (Expression) rse;

    private static bool TryGetResourceSetMethodArguments(
      MethodCallExpression mce,
      out ResourceSetExpression input,
      out LambdaExpression lambda)
    {
      input = (ResourceSetExpression) null;
      lambda = (LambdaExpression) null;
      input = mce.Arguments[0] as ResourceSetExpression;
      return input != null && ResourceBinder.PatternRules.MatchSingleArgumentLambda(mce.Arguments[1], out lambda);
    }

    private static bool TryBindToInput(
      ResourceExpression input,
      LambdaExpression le,
      out Expression bound)
    {
      List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
      bound = InputBinder.Bind(le.Body, input, le.Parameters[0], referencedInputs);
      if (referencedInputs.Count > 1 || referencedInputs.Count == 1 && referencedInputs[0] != input)
        bound = (Expression) null;
      return bound != null;
    }

    private static Expression AnalyzeResourceSetConstantMethod(
      MethodCallExpression mce,
      Func<MethodCallExpression, ResourceExpression, ConstantExpression, Expression> constantMethodAnalyzer)
    {
      ResourceExpression resourceExpression = (ResourceExpression) mce.Arguments[0];
      ConstantExpression constantExpression = ResourceBinder.StripTo<ConstantExpression>(mce.Arguments[1]);
      return constantExpression == null ? (Expression) mce : constantMethodAnalyzer(mce, resourceExpression, constantExpression);
    }

    private static Expression AnalyzeCountMethod(MethodCallExpression mce)
    {
      ResourceExpression e = (ResourceExpression) mce.Arguments[0];
      if (e == null)
        return (Expression) mce;
      ResourceBinder.ValidationRules.RequireCanAddCount((Expression) e);
      ResourceBinder.ValidationRules.RequireNonSingleton((Expression) e);
      e.CountOption = CountOption.ValueOnly;
      return (Expression) e;
    }

    private static void AddSequenceQueryOption(ResourceExpression target, QueryOptionExpression qoe)
    {
      ResourceBinder.ValidationRules.RequireNonSingleton((Expression) target);
      ResourceSetExpression resourceSetExpression = (ResourceSetExpression) target;
      if (qoe.NodeType == (ExpressionType) 10006)
      {
        if (resourceSetExpression.Take != null)
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The {0} query option cannot be specified after the {1} query option.", (object) "filter", (object) "top"));
        if (resourceSetExpression.Projection != null)
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The {0} query option cannot be specified after the {1} query option.", (object) "filter", (object) "select"));
      }
      resourceSetExpression.AddSequenceQueryOption(qoe);
    }

    internal override Expression VisitBinary(BinaryExpression b)
    {
      Expression expression = base.VisitBinary(b);
      if (!ResourceBinder.PatternRules.MatchStringAddition(expression))
        return expression;
      BinaryExpression binaryExpression = ResourceBinder.StripTo<BinaryExpression>(expression);
      return (Expression) Expression.Call(typeof (string).GetMethod("Concat", new Type[2]
      {
        typeof (string),
        typeof (string)
      }), new Expression[2]
      {
        binaryExpression.Left,
        binaryExpression.Right
      });
    }

    internal override Expression VisitMemberAccess(MemberExpression m)
    {
      Expression expression = base.VisitMemberAccess(m);
      MemberExpression me = ResourceBinder.StripTo<MemberExpression>(expression);
      PropertyInfo propInfo;
      MethodInfo mi;
      return me != null && ResourceBinder.PatternRules.MatchNonPrivateReadableProperty(me, out propInfo) && TypeSystem.TryGetPropertyAsMethod(propInfo, out mi) ? (Expression) Expression.Call(me.Expression, mi) : expression;
    }

    internal override Expression VisitMethodCall(MethodCallExpression mce)
    {
      SequenceMethod sequenceMethod;
      Expression e;
      if (ReflectionUtil.TryIdentifySequenceMethod(mce.Method, out sequenceMethod) && (sequenceMethod == SequenceMethod.Select || sequenceMethod == SequenceMethod.SelectManyResultSelector) && this.AnalyzeProjection(mce, sequenceMethod, out e))
        return e;
      Expression expression = base.VisitMethodCall(mce);
      mce = expression as MethodCallExpression;
      if (mce == null)
        return expression;
      if (ReflectionUtil.TryIdentifySequenceMethod(mce.Method, out sequenceMethod))
      {
        switch (sequenceMethod)
        {
          case SequenceMethod.Where:
            return ResourceBinder.AnalyzePredicate(mce);
          case SequenceMethod.Cast:
            return ResourceBinder.AnalyzeCast(mce);
          case SequenceMethod.Select:
            return ResourceBinder.AnalyzeNavigation(mce);
          case SequenceMethod.Take:
            return ResourceBinder.AnalyzeResourceSetConstantMethod(mce, (Func<MethodCallExpression, ResourceExpression, ConstantExpression, Expression>) ((callExp, resource, takeCount) =>
            {
              ResourceBinder.AddSequenceQueryOption(resource, (QueryOptionExpression) new TakeQueryOptionExpression(callExp.Type, takeCount));
              return (Expression) resource;
            }));
          case SequenceMethod.First:
          case SequenceMethod.FirstOrDefault:
            return ResourceBinder.LimitCardinality(mce, 1);
          case SequenceMethod.Single:
          case SequenceMethod.SingleOrDefault:
            return ResourceBinder.LimitCardinality(mce, 2);
          case SequenceMethod.Count:
          case SequenceMethod.LongCount:
            return ResourceBinder.AnalyzeCountMethod(mce);
          default:
            throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The method '{0}' is not supported", (object) mce.Method.Name));
        }
      }
      else
      {
        if (!(mce.Method.DeclaringType == typeof (TableQueryableExtensions)))
          return (Expression) mce;
        Type[] genericArguments = mce.Method.GetGenericArguments();
        Type type = genericArguments[0];
        if (mce.Method == TableQueryableExtensions.WithOptionsMethodInfo.MakeGenericMethod(type))
          return ResourceBinder.AnalyzeResourceSetConstantMethod(mce, (Func<MethodCallExpression, ResourceExpression, ConstantExpression, Expression>) ((callExp, resource, options) =>
          {
            ResourceBinder.AddSequenceQueryOption(resource, (QueryOptionExpression) new RequestOptionsQueryOptionExpression(callExp.Type, options));
            return (Expression) resource;
          }));
        if (mce.Method == TableQueryableExtensions.WithContextMethodInfo.MakeGenericMethod(type))
          return ResourceBinder.AnalyzeResourceSetConstantMethod(mce, (Func<MethodCallExpression, ResourceExpression, ConstantExpression, Expression>) ((callExp, resource, ctx) =>
          {
            ResourceBinder.AddSequenceQueryOption(resource, (QueryOptionExpression) new OperationContextQueryOptionExpression(callExp.Type, ctx));
            return (Expression) resource;
          }));
        if (genericArguments.Length > 1)
        {
          if (mce.Method == TableQueryableExtensions.ResolveMethodInfo.MakeGenericMethod(type, genericArguments[1]))
            return ResourceBinder.AnalyzeResourceSetConstantMethod(mce, (Func<MethodCallExpression, ResourceExpression, ConstantExpression, Expression>) ((callExp, resource, resolver) =>
            {
              ResourceBinder.AddSequenceQueryOption(resource, (QueryOptionExpression) new EntityResolverQueryOptionExpression(callExp.Type, resolver));
              return (Expression) resource;
            }));
        }
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The method '{0}' is not supported", (object) mce.Method.Name));
      }
    }

    private static Expression StripCastMethodCalls(Expression expression)
    {
      for (MethodCallExpression methodCallExpression = ResourceBinder.StripTo<MethodCallExpression>(expression); methodCallExpression != null && ReflectionUtil.IsSequenceMethod(methodCallExpression.Method, SequenceMethod.Cast); methodCallExpression = ResourceBinder.StripTo<MethodCallExpression>(expression))
        expression = methodCallExpression.Arguments[0];
      return expression;
    }

    internal static class PatternRules
    {
      internal static bool MatchConvertToAssignable(UnaryExpression expression) => (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked || expression.NodeType == ExpressionType.TypeAs) && expression.Type.IsAssignableFrom(expression.Operand.Type);

      internal static bool MatchParameterMemberAccess(Expression expression)
      {
        LambdaExpression lambdaExpression = ResourceBinder.StripTo<LambdaExpression>(expression);
        if (lambdaExpression == null || lambdaExpression.Parameters.Count != 1)
          return false;
        ParameterExpression parameter = lambdaExpression.Parameters[0];
        for (MemberExpression memberExpression = ResourceBinder.StripTo<MemberExpression>(ResourceBinder.StripCastMethodCalls(lambdaExpression.Body)); memberExpression != null; memberExpression = ResourceBinder.StripTo<MemberExpression>(memberExpression.Expression))
        {
          if (memberExpression.Expression == parameter)
            return true;
        }
        return false;
      }

      internal static bool MatchPropertyAccess(
        Expression e,
        out MemberExpression member,
        out Expression instance,
        out List<string> propertyPath)
      {
        instance = (Expression) null;
        propertyPath = (List<string>) null;
        MemberExpression me = ResourceBinder.StripTo<MemberExpression>(e);
        member = me;
        while (me != null)
        {
          PropertyInfo propInfo;
          if (ResourceBinder.PatternRules.MatchNonPrivateReadableProperty(me, out propInfo))
          {
            if (propertyPath == null)
              propertyPath = new List<string>();
            propertyPath.Insert(0, propInfo.Name);
            e = me.Expression;
            me = ResourceBinder.StripTo<MemberExpression>(e);
          }
          else
            me = (MemberExpression) null;
        }
        if (propertyPath == null)
          return false;
        instance = e;
        return true;
      }

      internal static bool MatchConstant(Expression e, out ConstantExpression constExpr)
      {
        constExpr = e as ConstantExpression;
        return constExpr != null;
      }

      internal static bool MatchAnd(Expression e)
      {
        if (!(e is BinaryExpression binaryExpression))
          return false;
        return binaryExpression.NodeType == ExpressionType.And || binaryExpression.NodeType == ExpressionType.AndAlso;
      }

      internal static bool MatchNonPrivateReadableProperty(Expression e, out PropertyInfo propInfo)
      {
        if (e is MemberExpression me)
          return ResourceBinder.PatternRules.MatchNonPrivateReadableProperty(me, out propInfo);
        propInfo = (PropertyInfo) null;
        return false;
      }

      internal static bool MatchNonPrivateReadableProperty(
        MemberExpression me,
        out PropertyInfo propInfo)
      {
        propInfo = (PropertyInfo) null;
        if (me.Member.MemberType == MemberTypes.Property)
        {
          PropertyInfo member = (PropertyInfo) me.Member;
          if (member.CanRead && !TypeSystem.IsPrivate(member))
          {
            propInfo = member;
            return true;
          }
        }
        return false;
      }

      internal static bool MatchReferenceEquals(Expression expression) => expression is MethodCallExpression methodCallExpression && methodCallExpression.Method == typeof (object).GetMethod("ReferenceEquals");

      internal static bool MatchResource(Expression expression, out ResourceExpression resource)
      {
        resource = expression as ResourceExpression;
        return resource != null;
      }

      internal static bool MatchDoubleArgumentLambda(
        Expression expression,
        out LambdaExpression lambda)
      {
        return ResourceBinder.PatternRules.MatchNaryLambda(expression, 2, out lambda);
      }

      internal static bool MatchIdentitySelector(LambdaExpression lambda) => lambda.Parameters[0] == ResourceBinder.StripTo<ParameterExpression>(lambda.Body);

      internal static bool MatchSingleArgumentLambda(
        Expression expression,
        out LambdaExpression lambda)
      {
        return ResourceBinder.PatternRules.MatchNaryLambda(expression, 1, out lambda);
      }

      internal static bool MatchTransparentIdentitySelector(
        Expression input,
        LambdaExpression selector)
      {
        if (selector.Parameters.Count != 1 || !(input is ResourceSetExpression resourceSetExpression) || resourceSetExpression.TransparentScope == null)
          return false;
        Expression body = selector.Body;
        ParameterExpression parameter = selector.Parameters[0];
        MemberExpression memberExpression;
        ref MemberExpression local1 = ref memberExpression;
        Expression expression;
        ref Expression local2 = ref expression;
        List<string> stringList;
        ref List<string> local3 = ref stringList;
        return ResourceBinder.PatternRules.MatchPropertyAccess(body, out local1, out local2, out local3) && expression == parameter && stringList.Count == 1 && stringList[0] == resourceSetExpression.TransparentScope.Accessor;
      }

      internal static bool MatchIdentityProjectionResultSelector(Expression e)
      {
        LambdaExpression lambdaExpression = (LambdaExpression) e;
        return lambdaExpression.Body == lambdaExpression.Parameters[1];
      }

      internal static bool MatchTransparentScopeSelector(
        ResourceSetExpression input,
        LambdaExpression resultSelector,
        out ResourceSetExpression.TransparentAccessors transparentScope)
      {
        transparentScope = (ResourceSetExpression.TransparentAccessors) null;
        if (resultSelector.Body.NodeType != ExpressionType.New)
          return false;
        NewExpression body = (NewExpression) resultSelector.Body;
        if (body.Arguments.Count < 2 || body.Type.BaseType != typeof (object))
          return false;
        ParameterInfo[] parameters = body.Constructor.GetParameters();
        if (body.Members.Count != parameters.Length)
          return false;
        ResourceSetExpression source = input.Source as ResourceSetExpression;
        int index1 = -1;
        ParameterExpression parameter1 = resultSelector.Parameters[0];
        ParameterExpression parameter2 = resultSelector.Parameters[1];
        MemberInfo[] memberInfoArray = new MemberInfo[body.Members.Count];
        PropertyInfo[] properties = body.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        Dictionary<string, Expression> sourceAccesors = new Dictionary<string, Expression>(parameters.Length - 1, (IEqualityComparer<string>) StringComparer.Ordinal);
        for (int index2 = 0; index2 < body.Arguments.Count; ++index2)
        {
          Expression expression = body.Arguments[index2];
          MemberInfo member = body.Members[index2];
          if (!ResourceBinder.PatternRules.ExpressionIsSimpleAccess(expression, resultSelector.Parameters))
            return false;
          if (member.MemberType == MemberTypes.Method)
          {
            member = (MemberInfo) ((IEnumerable<PropertyInfo>) properties).Where<PropertyInfo>((Func<PropertyInfo, bool>) (property => (MemberInfo) property.GetGetMethod() == member)).FirstOrDefault<PropertyInfo>();
            if (member == (MemberInfo) null)
              return false;
          }
          if (member.Name != parameters[index2].Name)
            return false;
          memberInfoArray[index2] = member;
          ParameterExpression parameterExpression = ResourceBinder.StripTo<ParameterExpression>(expression);
          if (parameter2 == parameterExpression)
          {
            if (index1 != -1)
              return false;
            index1 = index2;
          }
          else if (parameter1 == parameterExpression)
          {
            sourceAccesors[member.Name] = (Expression) source.CreateReference();
          }
          else
          {
            List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
            InputBinder.Bind(expression, (ResourceExpression) source, resultSelector.Parameters[0], referencedInputs);
            if (referencedInputs.Count != 1)
              return false;
            sourceAccesors[member.Name] = (Expression) referencedInputs[0].CreateReference();
          }
        }
        if (index1 == -1)
          return false;
        string name = memberInfoArray[index1].Name;
        transparentScope = new ResourceSetExpression.TransparentAccessors(name, sourceAccesors);
        return true;
      }

      internal static bool MatchPropertyProjectionSet(
        ResourceExpression input,
        Expression potentialPropertyRef,
        out MemberExpression navigationMember)
      {
        return ResourceBinder.PatternRules.MatchNavigationPropertyProjection(input, potentialPropertyRef, true, out navigationMember);
      }

      internal static bool MatchPropertyProjectionSingleton(
        ResourceExpression input,
        Expression potentialPropertyRef,
        out MemberExpression navigationMember)
      {
        return ResourceBinder.PatternRules.MatchNavigationPropertyProjection(input, potentialPropertyRef, false, out navigationMember);
      }

      private static bool MatchNavigationPropertyProjection(
        ResourceExpression input,
        Expression potentialPropertyRef,
        bool requireSet,
        out MemberExpression navigationMember)
      {
        Expression instance;
        if (ResourceBinder.PatternRules.MatchNonSingletonProperty(potentialPropertyRef) == requireSet && ResourceBinder.PatternRules.MatchPropertyAccess(potentialPropertyRef, out navigationMember, out instance, out List<string> _) && instance == input.CreateReference())
          return true;
        navigationMember = (MemberExpression) null;
        return false;
      }

      internal static bool MatchMemberInitExpressionWithDefaultConstructor(
        Expression source,
        LambdaExpression e)
      {
        MemberInitExpression memberInitExpression = ResourceBinder.StripTo<MemberInitExpression>(e.Body);
        return ResourceBinder.PatternRules.MatchResource(source, out ResourceExpression _) && memberInitExpression != null && memberInitExpression.NewExpression.Arguments.Count == 0;
      }

      internal static bool MatchNewExpression(Expression source, LambdaExpression e) => ResourceBinder.PatternRules.MatchResource(source, out ResourceExpression _) && e.Body is NewExpression;

      internal static bool MatchNot(Expression expression) => expression.NodeType == ExpressionType.Not;

      internal static bool MatchNonSingletonProperty(Expression e) => TypeSystem.FindIEnumerable(e.Type) != (Type) null && e.Type != typeof (char[]) && e.Type != typeof (byte[]);

      internal static ResourceBinder.PatternRules.MatchNullCheckResult MatchNullCheck(
        Expression entityInScope,
        ConditionalExpression conditional)
      {
        ResourceBinder.PatternRules.MatchNullCheckResult matchNullCheckResult = new ResourceBinder.PatternRules.MatchNullCheckResult();
        ResourceBinder.PatternRules.MatchEqualityCheckResult equalityCheckResult = ResourceBinder.PatternRules.MatchEquality(conditional.Test);
        if (!equalityCheckResult.Match)
          return matchNullCheckResult;
        Expression assignmentExpression1;
        if (equalityCheckResult.EqualityYieldsTrue)
        {
          if (!ResourceBinder.PatternRules.MatchNullConstant(conditional.IfTrue))
            return matchNullCheckResult;
          assignmentExpression1 = conditional.IfFalse;
        }
        else
        {
          if (!ResourceBinder.PatternRules.MatchNullConstant(conditional.IfFalse))
            return matchNullCheckResult;
          assignmentExpression1 = conditional.IfTrue;
        }
        Expression assignmentExpression2;
        if (ResourceBinder.PatternRules.MatchNullConstant(equalityCheckResult.TestLeft))
        {
          assignmentExpression2 = equalityCheckResult.TestRight;
        }
        else
        {
          if (!ResourceBinder.PatternRules.MatchNullConstant(equalityCheckResult.TestRight))
            return matchNullCheckResult;
          assignmentExpression2 = equalityCheckResult.TestLeft;
        }
        MemberAssignmentAnalysis assignmentAnalysis1 = MemberAssignmentAnalysis.Analyze(entityInScope, assignmentExpression1);
        if (assignmentAnalysis1.MultiplePathsFound)
          return matchNullCheckResult;
        MemberAssignmentAnalysis assignmentAnalysis2 = MemberAssignmentAnalysis.Analyze(entityInScope, assignmentExpression2);
        if (assignmentAnalysis2.MultiplePathsFound)
          return matchNullCheckResult;
        Expression[] expressionsToTargetEntity1 = assignmentAnalysis1.GetExpressionsToTargetEntity();
        Expression[] expressionsToTargetEntity2 = assignmentAnalysis2.GetExpressionsToTargetEntity();
        if (expressionsToTargetEntity2.Length > expressionsToTargetEntity1.Length)
          return matchNullCheckResult;
        for (int index = 0; index < expressionsToTargetEntity2.Length; ++index)
        {
          Expression expression1 = expressionsToTargetEntity1[index];
          Expression expression2 = expressionsToTargetEntity2[index];
          if (expression1 != expression2 && (expression1.NodeType != expression2.NodeType || expression1.NodeType != ExpressionType.MemberAccess || ((MemberExpression) expression1).Member != ((MemberExpression) expression2).Member))
            return matchNullCheckResult;
        }
        matchNullCheckResult.AssignExpression = assignmentExpression1;
        matchNullCheckResult.Match = true;
        matchNullCheckResult.TestToNullExpression = assignmentExpression2;
        return matchNullCheckResult;
      }

      internal static bool MatchNullConstant(Expression expression) => expression is ConstantExpression constantExpression && constantExpression.Value == null;

      internal static bool MatchBinaryExpression(Expression e) => e is BinaryExpression;

      internal static bool MatchBinaryEquality(Expression e) => ResourceBinder.PatternRules.MatchBinaryExpression(e) && e.NodeType == ExpressionType.Equal;

      internal static bool MatchStringAddition(Expression e) => e.NodeType == ExpressionType.Add && e is BinaryExpression binaryExpression && binaryExpression.Left.Type == typeof (string) && binaryExpression.Right.Type == typeof (string);

      internal static ResourceBinder.PatternRules.MatchEqualityCheckResult MatchEquality(
        Expression expression)
      {
        ResourceBinder.PatternRules.MatchEqualityCheckResult equalityCheckResult = new ResourceBinder.PatternRules.MatchEqualityCheckResult();
        equalityCheckResult.Match = false;
        equalityCheckResult.EqualityYieldsTrue = true;
        for (; !ResourceBinder.PatternRules.MatchReferenceEquals(expression); expression = ((UnaryExpression) expression).Operand)
        {
          if (ResourceBinder.PatternRules.MatchNot(expression))
            equalityCheckResult.EqualityYieldsTrue = !equalityCheckResult.EqualityYieldsTrue;
          else if (expression is BinaryExpression binaryExpression)
          {
            if (binaryExpression.NodeType == ExpressionType.NotEqual)
              equalityCheckResult.EqualityYieldsTrue = !equalityCheckResult.EqualityYieldsTrue;
            else if (binaryExpression.NodeType != ExpressionType.Equal)
              goto label_10;
            equalityCheckResult.TestLeft = binaryExpression.Left;
            equalityCheckResult.TestRight = binaryExpression.Right;
            equalityCheckResult.Match = true;
            goto label_10;
          }
          else
            goto label_10;
        }
        MethodCallExpression methodCallExpression = (MethodCallExpression) expression;
        equalityCheckResult.Match = true;
        equalityCheckResult.TestLeft = methodCallExpression.Arguments[0];
        equalityCheckResult.TestRight = methodCallExpression.Arguments[1];
label_10:
        return equalityCheckResult;
      }

      private static bool ExpressionIsSimpleAccess(
        Expression argument,
        ReadOnlyCollection<ParameterExpression> expressions)
      {
        Expression expression = argument;
        do
        {
          if (expression is MemberExpression memberExpression)
            expression = memberExpression.Expression;
        }
        while (memberExpression != null);
        return expression is ParameterExpression parameterExpression && expressions.Contains(parameterExpression);
      }

      private static bool MatchNaryLambda(
        Expression expression,
        int parameterCount,
        out LambdaExpression lambda)
      {
        lambda = (LambdaExpression) null;
        LambdaExpression lambdaExpression = ResourceBinder.StripTo<LambdaExpression>(expression);
        if (lambdaExpression != null && lambdaExpression.Parameters.Count == parameterCount)
          lambda = lambdaExpression;
        return lambda != null;
      }

      internal struct MatchNullCheckResult
      {
        internal Expression AssignExpression;
        internal bool Match;
        internal Expression TestToNullExpression;
      }

      internal struct MatchEqualityCheckResult
      {
        internal bool EqualityYieldsTrue;
        internal bool Match;
        internal Expression TestLeft;
        internal Expression TestRight;
      }
    }

    private static class ValidationRules
    {
      internal static void RequireCanNavigate(Expression e)
      {
        if (e is ResourceSetExpression resourceSetExpression && resourceSetExpression.HasSequenceQueryOptions)
          throw new NotSupportedException("Can only specify query options (orderby, where, take, skip) after last navigation.");
        ResourceExpression resource;
        if (ResourceBinder.PatternRules.MatchResource(e, out resource) && resource.Projection != null)
          throw new NotSupportedException("Can only specify query options (orderby, where, take, skip) after last navigation.");
      }

      internal static void RequireCanProject(Expression e)
      {
        ResourceExpression resource = (ResourceExpression) e;
        if (!ResourceBinder.PatternRules.MatchResource(e, out resource))
          throw new NotSupportedException("Can only project the last entity type in the query being translated.");
        if (resource.Projection != null)
          throw new NotSupportedException("Cannot translate multiple Linq Select operations in a single 'select' query option.");
        if (resource.ExpandPaths.Count > 0)
          throw new NotSupportedException("Cannot create projection while there is an explicit expansion specified on the same query.");
      }

      internal static void RequireCanAddCount(Expression e)
      {
        ResourceExpression resource = (ResourceExpression) e;
        if (!ResourceBinder.PatternRules.MatchResource(e, out resource))
          throw new NotSupportedException("Cannot add count option to the resource set.");
        if (resource.CountOption != CountOption.None)
          throw new NotSupportedException("Cannot add count option to the resource set because it would conflict with existing count options.");
      }

      internal static void RequireNonSingleton(Expression e)
      {
        if (e is ResourceExpression resourceExpression && resourceExpression.IsSingleton)
          throw new NotSupportedException("Cannot specify query options (orderby, where, take, skip) on single resource.");
      }
    }

    private sealed class PropertyInfoEqualityComparer : IEqualityComparer<PropertyInfo>
    {
      internal static readonly ResourceBinder.PropertyInfoEqualityComparer Instance = new ResourceBinder.PropertyInfoEqualityComparer();

      private PropertyInfoEqualityComparer()
      {
      }

      public bool Equals(PropertyInfo left, PropertyInfo right)
      {
        if ((object) left == (object) right)
          return true;
        return !((PropertyInfo) null == left) && !((PropertyInfo) null == right) && (object) left.DeclaringType == (object) right.DeclaringType && left.Name.Equals(right.Name);
      }

      public int GetHashCode(PropertyInfo obj) => !((PropertyInfo) null != obj) ? 0 : obj.GetHashCode();
    }

    private sealed class ExpressionPresenceVisitor : DataServiceALinqExpressionVisitor
    {
      private readonly Expression target;
      private bool found;

      private ExpressionPresenceVisitor(Expression target) => this.target = target;

      internal static bool IsExpressionPresent(Expression target, Expression tree)
      {
        ResourceBinder.ExpressionPresenceVisitor expressionPresenceVisitor = new ResourceBinder.ExpressionPresenceVisitor(target);
        expressionPresenceVisitor.Visit(tree);
        return expressionPresenceVisitor.found;
      }

      internal override Expression Visit(Expression exp)
      {
        Expression expression;
        if (this.found || this.target == exp)
        {
          this.found = true;
          expression = exp;
        }
        else
          expression = base.Visit(exp);
        return expression;
      }
    }
  }
}
