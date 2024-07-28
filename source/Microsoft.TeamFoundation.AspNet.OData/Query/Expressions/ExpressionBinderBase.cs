// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.ExpressionBinderBase
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  public abstract class ExpressionBinderBase
  {
    internal static readonly MethodInfo StringCompareMethodInfo = typeof (string).GetMethod("Compare", new Type[3]
    {
      typeof (string),
      typeof (string),
      typeof (StringComparison)
    });
    internal static readonly MethodInfo GuidCompareMethodInfo = typeof (ExpressionBinderBase).GetMethod("GuidCompare", new Type[2]
    {
      typeof (Guid),
      typeof (Guid)
    });
    internal static readonly string DictionaryStringObjectIndexerName = typeof (Dictionary<string, object>).GetDefaultMembers()[0].Name;
    internal static readonly Expression NullConstant = (Expression) Expression.Constant((object) null);
    internal static readonly Expression FalseConstant = (Expression) Expression.Constant((object) false);
    internal static readonly Expression TrueConstant = (Expression) Expression.Constant((object) true);
    internal static readonly Expression ZeroConstant = (Expression) Expression.Constant((object) 0);
    internal static readonly Expression OrdinalStringComparisonConstant = (Expression) Expression.Constant((object) StringComparison.Ordinal);
    internal static readonly MethodInfo EnumTryParseMethod = ((IEnumerable<MethodInfo>) typeof (Enum).GetMethods()).Single<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == "TryParse" && m.GetParameters().Length == 2));
    internal static readonly Dictionary<BinaryOperatorKind, ExpressionType> BinaryOperatorMapping = new Dictionary<BinaryOperatorKind, ExpressionType>()
    {
      {
        BinaryOperatorKind.Add,
        ExpressionType.Add
      },
      {
        BinaryOperatorKind.And,
        ExpressionType.AndAlso
      },
      {
        BinaryOperatorKind.Divide,
        ExpressionType.Divide
      },
      {
        BinaryOperatorKind.Equal,
        ExpressionType.Equal
      },
      {
        BinaryOperatorKind.GreaterThan,
        ExpressionType.GreaterThan
      },
      {
        BinaryOperatorKind.GreaterThanOrEqual,
        ExpressionType.GreaterThanOrEqual
      },
      {
        BinaryOperatorKind.LessThan,
        ExpressionType.LessThan
      },
      {
        BinaryOperatorKind.LessThanOrEqual,
        ExpressionType.LessThanOrEqual
      },
      {
        BinaryOperatorKind.Modulo,
        ExpressionType.Modulo
      },
      {
        BinaryOperatorKind.Multiply,
        ExpressionType.Multiply
      },
      {
        BinaryOperatorKind.NotEqual,
        ExpressionType.NotEqual
      },
      {
        BinaryOperatorKind.Or,
        ExpressionType.OrElse
      },
      {
        BinaryOperatorKind.Subtract,
        ExpressionType.Subtract
      }
    };
    internal IQueryable BaseQuery;
    internal IDictionary<string, Expression> FlattenedPropertyContainer;
    internal bool HasInstancePropertyContainer;
    private static HashSet<string> _skippableMethods = new HashSet<string>()
    {
      "AsQueryable",
      "Where",
      "OrderBy",
      "OrderByDescending",
      "ThenBy",
      "ThenByDescending"
    };

    internal IEdmModel Model { get; set; }

    internal ODataQuerySettings QuerySettings { get; set; }

    internal IWebApiAssembliesResolver InternalAssembliesResolver { get; set; }

    protected ExpressionBinderBase(IServiceProvider requestContainer)
    {
      this.QuerySettings = ServiceProviderServiceExtensions.GetRequiredService<ODataQuerySettings>(requestContainer);
      this.Model = ServiceProviderServiceExtensions.GetRequiredService<IEdmModel>(requestContainer);
      this.InternalAssembliesResolver = ServiceProviderServiceExtensions.GetService<IWebApiAssembliesResolver>(requestContainer) ?? WebApiAssembliesResolver.Default;
    }

    internal ExpressionBinderBase(
      IEdmModel model,
      IWebApiAssembliesResolver assembliesResolver,
      ODataQuerySettings querySettings)
      : this(model, querySettings)
    {
      this.InternalAssembliesResolver = assembliesResolver;
    }

    internal ExpressionBinderBase(IEdmModel model, ODataQuerySettings querySettings)
    {
      this.QuerySettings = querySettings;
      this.Model = model;
    }

    public Expression BindBinaryOperatorNode(BinaryOperatorNode binaryOperatorNode) => this.BindBinaryOperatorNode(binaryOperatorNode, (Expression) null);

    public Expression BindBinaryOperatorNode(
      BinaryOperatorNode binaryOperatorNode,
      Expression baseElement)
    {
      Expression left = this.Bind((QueryNode) binaryOperatorNode.Left, baseElement);
      Expression right = this.Bind((QueryNode) binaryOperatorNode.Right, baseElement);
      ExpressionBinderBase.PromoteExpressionTypes(binaryOperatorNode, ref left, ref right);
      if ((this.QuerySettings.HandleNullPropagation != HandleNullPropagationOption.True ? 0 : (ExpressionBinderBase.IsNullable(left.Type) ? 1 : (ExpressionBinderBase.IsNullable(right.Type) ? 1 : 0))) == 0)
        return this.CreateBinaryExpression(binaryOperatorNode.OperatorKind, left, right, false);
      Expression nullable1 = ExpressionBinderBase.ToNullable(left);
      Expression nullable2 = ExpressionBinderBase.ToNullable(right);
      bool liftToNull = true;
      if (nullable1 == ExpressionBinderBase.NullConstant || nullable2 == ExpressionBinderBase.NullConstant)
        liftToNull = false;
      return this.CreateBinaryExpression(binaryOperatorNode.OperatorKind, nullable1, nullable2, liftToNull);
    }

    private static void PromoteExpressionTypes(
      BinaryOperatorNode binaryOperatorNode,
      ref Expression left,
      ref Expression right)
    {
      Type underlyingTypeOrSelf1 = TypeHelper.GetUnderlyingTypeOrSelf(left.Type);
      Type underlyingTypeOrSelf2 = TypeHelper.GetUnderlyingTypeOrSelf(right.Type);
      if (!(underlyingTypeOrSelf1 != underlyingTypeOrSelf2) || binaryOperatorNode.Left.TypeReference != null || binaryOperatorNode.Right.TypeReference != null)
        return;
      if (underlyingTypeOrSelf1.CanPromoteValueTypeTo(underlyingTypeOrSelf2))
      {
        left = (Expression) Expression.Convert(left, TypeHelper.IsNullable(right.Type) ? TypeHelper.ToNullable(underlyingTypeOrSelf2) : underlyingTypeOrSelf2);
      }
      else
      {
        if (!underlyingTypeOrSelf2.CanPromoteValueTypeTo(underlyingTypeOrSelf1))
          return;
        right = (Expression) Expression.Convert(right, TypeHelper.IsNullable(left.Type) ? TypeHelper.ToNullable(underlyingTypeOrSelf1) : underlyingTypeOrSelf1);
      }
    }

    internal Expression CreateBinaryExpression(
      BinaryOperatorKind binaryOperator,
      Expression left,
      Expression right,
      bool liftToNull)
    {
      Type type1 = Nullable.GetUnderlyingType(left.Type);
      if ((object) type1 == null)
        type1 = left.Type;
      Type type2 = type1;
      Type type3 = Nullable.GetUnderlyingType(right.Type);
      if ((object) type3 == null)
        type3 = right.Type;
      Type type4 = type3;
      if ((TypeHelper.IsEnum(type2) || TypeHelper.IsEnum(type4)) && binaryOperator != BinaryOperatorKind.Has)
      {
        Type enumType = TypeHelper.IsEnum(type2) ? type2 : type4;
        Type underlyingType = Enum.GetUnderlyingType(enumType);
        left = ExpressionBinderBase.ConvertToEnumUnderlyingType(left, enumType, underlyingType);
        right = ExpressionBinderBase.ConvertToEnumUnderlyingType(right, enumType, underlyingType);
      }
      if (type2 == typeof (DateTime) && type4 == typeof (DateTimeOffset))
        right = ExpressionBinderBase.DateTimeOffsetToDateTime(right);
      else if (type4 == typeof (DateTime) && type2 == typeof (DateTimeOffset))
        left = ExpressionBinderBase.DateTimeOffsetToDateTime(left);
      if (ExpressionBinderBase.IsDateOrOffset(type2) && ExpressionBinderBase.IsDate(type4) || ExpressionBinderBase.IsDate(type2) && ExpressionBinderBase.IsDateOrOffset(type4))
      {
        left = this.CreateDateBinaryExpression(left);
        right = this.CreateDateBinaryExpression(right);
      }
      if (ExpressionBinderBase.IsDateOrOffset(type2) && ExpressionBinderBase.IsTimeOfDay(type4) || ExpressionBinderBase.IsTimeOfDay(type2) && ExpressionBinderBase.IsDateOrOffset(type4) || ExpressionBinderBase.IsTimeSpan(type2) && ExpressionBinderBase.IsTimeOfDay(type4) || ExpressionBinderBase.IsTimeOfDay(type2) && ExpressionBinderBase.IsTimeSpan(type4))
      {
        left = this.CreateTimeBinaryExpression(left);
        right = this.CreateTimeBinaryExpression(right);
      }
      if (left.Type != right.Type)
      {
        left = ExpressionBinderBase.ToNullable(left);
        right = ExpressionBinderBase.ToNullable(right);
      }
      if (left.Type == typeof (Guid) || right.Type == typeof (Guid))
      {
        left = ExpressionBinderBase.ConvertNull(left, typeof (Guid));
        right = ExpressionBinderBase.ConvertNull(right, typeof (Guid));
        switch (binaryOperator)
        {
          case BinaryOperatorKind.GreaterThan:
          case BinaryOperatorKind.GreaterThanOrEqual:
          case BinaryOperatorKind.LessThan:
          case BinaryOperatorKind.LessThanOrEqual:
            left = (Expression) Expression.Call(ExpressionBinderBase.GuidCompareMethodInfo, left, right);
            right = ExpressionBinderBase.ZeroConstant;
            break;
        }
      }
      if (left.Type == typeof (string) || right.Type == typeof (string))
      {
        left = ExpressionBinderBase.ConvertNull(left, typeof (string));
        right = ExpressionBinderBase.ConvertNull(right, typeof (string));
        switch (binaryOperator)
        {
          case BinaryOperatorKind.GreaterThan:
          case BinaryOperatorKind.GreaterThanOrEqual:
          case BinaryOperatorKind.LessThan:
          case BinaryOperatorKind.LessThanOrEqual:
            left = (Expression) Expression.Call(ExpressionBinderBase.StringCompareMethodInfo, left, right, ExpressionBinderBase.OrdinalStringComparisonConstant);
            right = ExpressionBinderBase.ZeroConstant;
            break;
        }
      }
      ExpressionType binaryType;
      if (ExpressionBinderBase.BinaryOperatorMapping.TryGetValue(binaryOperator, out binaryType))
      {
        if (!(left.Type == typeof (byte[])) && !(right.Type == typeof (byte[])))
          return (Expression) Expression.MakeBinary(binaryType, left, right, liftToNull, (MethodInfo) null);
        left = ExpressionBinderBase.ConvertNull(left, typeof (byte[]));
        right = ExpressionBinderBase.ConvertNull(right, typeof (byte[]));
        if (binaryType == ExpressionType.Equal)
          return (Expression) Expression.MakeBinary(binaryType, left, right, liftToNull, Linq2ObjectsComparisonMethods.AreByteArraysEqualMethodInfo);
        if (binaryType == ExpressionType.NotEqual)
          return (Expression) Expression.MakeBinary(binaryType, left, right, liftToNull, Linq2ObjectsComparisonMethods.AreByteArraysNotEqualMethodInfo);
        IEdmPrimitiveType primitiveTypeOrNull = EdmLibHelpers.GetEdmPrimitiveTypeOrNull(typeof (byte[]));
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.BinaryOperatorNotSupported, (object) primitiveTypeOrNull.FullName(), (object) primitiveTypeOrNull.FullName(), (object) binaryOperator));
      }
      if (TypeHelper.IsEnum(left.Type) && TypeHelper.IsEnum(right.Type) && binaryOperator == BinaryOperatorKind.Has)
      {
        UnaryExpression flag = Expression.Convert(right, typeof (Enum));
        return this.BindHas(left, (Expression) flag);
      }
      throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.QueryNodeBindingNotSupported, (object) binaryOperator, (object) typeof (FilterBinder).Name);
    }

    internal Expression CreateConvertExpression(ConvertNode convertNode, Expression source)
    {
      Type clrType = EdmLibHelpers.GetClrType(convertNode.TypeReference, this.Model, this.InternalAssembliesResolver);
      if (clrType == typeof (bool?) && source.Type == typeof (bool) || clrType == typeof (Date?) && (source.Type == typeof (DateTimeOffset?) || source.Type == typeof (DateTime?)) || clrType == typeof (TimeOfDay?) && source.Type == typeof (TimeOfDay) || clrType == typeof (Date?) && source.Type == typeof (Date) || clrType == typeof (TimeOfDay?) && (source.Type == typeof (DateTimeOffset?) || source.Type == typeof (DateTime?) || source.Type == typeof (TimeSpan?)) || ExpressionBinderBase.IsDateAndTimeRelated(clrType) && ExpressionBinderBase.IsDateAndTimeRelated(source.Type) || source == ExpressionBinderBase.NullConstant || TypeHelper.IsEnum(source.Type))
        return source;
      return this.QuerySettings.HandleNullPropagation == HandleNullPropagationOption.True && ExpressionBinderBase.IsNullable(source.Type) && !ExpressionBinderBase.IsNullable(clrType) ? (Expression) Expression.Condition(ExpressionBinderBase.CheckForNull(source), (Expression) Expression.Constant((object) null, ExpressionBinderBase.ToNullable(clrType)), (Expression) Expression.Convert(ExpressionBinderBase.ExtractValueFromNullableExpression(source), ExpressionBinderBase.ToNullable(clrType))) : (Expression) Expression.Convert(source, clrType);
    }

    internal Expression ConvertNonStandardPrimitives(Expression source)
    {
      bool isNonstandardEdmPrimitive;
      Type type = EdmLibHelpers.IsNonstandardEdmPrimitive(source.Type, out isNonstandardEdmPrimitive);
      if (!isNonstandardEdmPrimitive)
        return source;
      Type underlyingTypeOrSelf = TypeHelper.GetUnderlyingTypeOrSelf(source.Type);
      Expression expression = (Expression) null;
      if (TypeHelper.IsEnum(underlyingTypeOrSelf))
      {
        expression = source;
      }
      else
      {
        switch (Type.GetTypeCode(underlyingTypeOrSelf))
        {
          case TypeCode.Object:
            if (underlyingTypeOrSelf == typeof (char[]))
            {
              expression = (Expression) Expression.New(typeof (string).GetConstructor(new Type[1]
              {
                typeof (char[])
              }), source);
              break;
            }
            if (underlyingTypeOrSelf == typeof (XElement))
            {
              expression = (Expression) Expression.Call(source, "ToString", (Type[]) null, (Expression[]) null);
              break;
            }
            if (underlyingTypeOrSelf == typeof (Binary))
            {
              expression = (Expression) Expression.Call(source, "ToArray", (Type[]) null, (Expression[]) null);
              break;
            }
            break;
          case TypeCode.Char:
            expression = (Expression) Expression.Call(ExpressionBinderBase.ExtractValueFromNullableExpression(source), "ToString", (Type[]) null, (Expression[]) null);
            break;
          case TypeCode.UInt16:
          case TypeCode.UInt32:
          case TypeCode.UInt64:
            expression = (Expression) Expression.Convert(ExpressionBinderBase.ExtractValueFromNullableExpression(source), type);
            break;
          case TypeCode.DateTime:
            expression = source;
            break;
        }
      }
      return this.QuerySettings.HandleNullPropagation == HandleNullPropagationOption.True && ExpressionBinderBase.IsNullable(source.Type) ? (Expression) Expression.Condition(ExpressionBinderBase.CheckForNull(source), (Expression) Expression.Constant((object) null, ExpressionBinderBase.ToNullable(expression.Type)), ExpressionBinderBase.ToNullable(expression)) : expression;
    }

    internal Expression MakePropertyAccess(PropertyInfo propertyInfo, Expression argument)
    {
      Expression source = argument;
      if (this.QuerySettings.HandleNullPropagation == HandleNullPropagationOption.True)
        source = this.RemoveInnerNullPropagation(argument);
      return (Expression) Expression.Property(ExpressionBinderBase.ExtractValueFromNullableExpression(source), propertyInfo);
    }

    internal Expression MakeFunctionCall(MemberInfo member, params Expression[] arguments) => this.MakeFunctionCall(member, true, arguments);

    internal Expression MakeFunctionCall(
      MemberInfo member,
      bool canonical,
      params Expression[] arguments)
    {
      IEnumerable<Expression> expressions = (IEnumerable<Expression>) arguments;
      if (this.QuerySettings.HandleNullPropagation == HandleNullPropagationOption.True)
        expressions = ((IEnumerable<Expression>) arguments).Select<Expression, Expression>((Func<Expression, Expression>) (a => this.RemoveInnerNullPropagation(a)));
      if (canonical)
        expressions = ExpressionBinderBase.ExtractValueFromNullableArguments(expressions);
      Expression functionCall;
      if (member.MemberType == MemberTypes.Method)
      {
        MethodInfo method = member as MethodInfo;
        functionCall = !method.IsStatic ? (Expression) Expression.Call(expressions.First<Expression>(), method, expressions.Skip<Expression>(1)) : (Expression) Expression.Call((Expression) null, method, expressions);
      }
      else
        functionCall = (Expression) Expression.Property(expressions.First<Expression>(), member as PropertyInfo);
      return this.CreateFunctionCallWithNullPropagation(functionCall, arguments);
    }

    internal Expression CreateFunctionCallWithNullPropagation(
      Expression functionCall,
      Expression[] arguments)
    {
      if (this.QuerySettings.HandleNullPropagation != HandleNullPropagationOption.True)
        return functionCall;
      Expression test = ExpressionBinderBase.CheckIfArgumentsAreNull(arguments);
      return test == ExpressionBinderBase.FalseConstant ? functionCall : (Expression) Expression.Condition(test, (Expression) Expression.Constant((object) null, ExpressionBinderBase.ToNullable(functionCall.Type)), ExpressionBinderBase.ToNullable(functionCall));
    }

    internal Expression RemoveInnerNullPropagation(Expression expression)
    {
      if (this.QuerySettings.HandleNullPropagation == HandleNullPropagationOption.True && expression.NodeType == ExpressionType.Conditional)
      {
        ConditionalExpression conditionalExpression = (ConditionalExpression) expression;
        if (conditionalExpression.Test.NodeType != ExpressionType.OrElse)
        {
          expression = conditionalExpression.IfFalse;
          if (expression.NodeType == ExpressionType.Convert)
          {
            UnaryExpression unaryExpression = expression as UnaryExpression;
            if (Nullable.GetUnderlyingType(unaryExpression.Type) == unaryExpression.Operand.Type)
              expression = unaryExpression.Operand;
          }
        }
      }
      return expression;
    }

    internal string GetFullPropertyPath(SingleValueNode node)
    {
      string fullPropertyPath1 = (string) null;
      SingleValueNode node1 = (SingleValueNode) null;
      switch (node.Kind)
      {
        case QueryNodeKind.SingleValuePropertyAccess:
          SingleValuePropertyAccessNode propertyAccessNode = (SingleValuePropertyAccessNode) node;
          fullPropertyPath1 = propertyAccessNode.Property.Name;
          node1 = propertyAccessNode.Source;
          break;
        case QueryNodeKind.SingleNavigationNode:
          SingleNavigationNode singleNavigationNode = (SingleNavigationNode) node;
          fullPropertyPath1 = singleNavigationNode.NavigationProperty.Name;
          node1 = (SingleValueNode) singleNavigationNode.Source;
          break;
        case QueryNodeKind.SingleComplexNode:
          SingleComplexNode singleComplexNode = (SingleComplexNode) node;
          fullPropertyPath1 = singleComplexNode.Property.Name;
          node1 = (SingleValueNode) singleComplexNode.Source;
          break;
      }
      if (node1 != null)
      {
        string fullPropertyPath2 = this.GetFullPropertyPath(node1);
        if (fullPropertyPath2 != null)
          fullPropertyPath1 = fullPropertyPath2 + "\\" + fullPropertyPath1;
      }
      return fullPropertyPath1;
    }

    protected PropertyInfo GetDynamicPropertyContainer(SingleValueOpenPropertyAccessNode openNode)
    {
      IEdmTypeReference typeReference = openNode.Source.TypeReference;
      IEdmStructuredType edmType;
      if (typeReference.IsEntity())
        edmType = (IEdmStructuredType) typeReference.AsEntity().EntityDefinition();
      else
        edmType = typeReference.IsComplex() ? (IEdmStructuredType) typeReference.AsComplex().ComplexDefinition() : throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.QueryNodeBindingNotSupported, (object) openNode.Kind, (object) typeof (FilterBinder).Name);
      return EdmLibHelpers.GetDynamicPropertyDictionary(edmType, this.Model);
    }

    private static Expression CheckIfArgumentsAreNull(Expression[] arguments)
    {
      if (((IEnumerable<Expression>) arguments).Any<Expression>((Func<Expression, bool>) (arg => arg == ExpressionBinderBase.NullConstant)))
        return ExpressionBinderBase.TrueConstant;
      arguments = ((IEnumerable<Expression>) arguments).Select<Expression, Expression>((Func<Expression, Expression>) (arg => ExpressionBinderBase.CheckForNull(arg))).Where<Expression>((Func<Expression, bool>) (arg => arg != null)).ToArray<Expression>();
      return ((IEnumerable<Expression>) arguments).Any<Expression>() ? ((IEnumerable<Expression>) arguments).Aggregate<Expression>((Func<Expression, Expression, Expression>) ((left, right) => (Expression) Expression.OrElse(left, right))) : ExpressionBinderBase.FalseConstant;
    }

    internal static Expression CheckForNull(Expression expression) => ExpressionBinderBase.IsNullable(expression.Type) && expression.NodeType != ExpressionType.Constant ? (Expression) Expression.Equal(expression, (Expression) Expression.Constant((object) null)) : (Expression) null;

    private static IEnumerable<Expression> ExtractValueFromNullableArguments(
      IEnumerable<Expression> arguments)
    {
      return arguments.Select<Expression, Expression>((Func<Expression, Expression>) (arg => ExpressionBinderBase.ExtractValueFromNullableExpression(arg)));
    }

    internal static Expression ExtractValueFromNullableExpression(Expression source) => !(Nullable.GetUnderlyingType(source.Type) != (Type) null) ? source : (Expression) Expression.Property(source, "Value");

    internal Expression BindHas(Expression left, Expression flag)
    {
      Expression[] expressionArray = new Expression[2]
      {
        left,
        flag
      };
      return this.MakeFunctionCall((MemberInfo) ClrCanonicalFunctions.HasFlag, expressionArray);
    }

    protected void EnsureFlattenedPropertyContainer(ParameterExpression source)
    {
      if (this.BaseQuery == null)
        return;
      this.HasInstancePropertyContainer = ExpressionBinderBase.TypeHasInstancePropertyContainer(this.BaseQuery.ElementType);
      this.FlattenedPropertyContainer = this.FlattenedPropertyContainer ?? ExpressionBinderBase.GetFlattenedProperties(this.BaseQuery, this.HasInstancePropertyContainer, source);
    }

    private static bool TypeHasInstancePropertyContainer(Type source) => source.IsGenericType && source.GetGenericTypeDefinition() == typeof (ComputeWrapper<>);

    internal static IDictionary<string, Expression> GetFlattenedProperties(
      IQueryable baseQuery,
      bool hasInstancePropertyContainer,
      ParameterExpression source)
    {
      if (baseQuery == null)
        return (IDictionary<string, Expression>) null;
      if (!typeof (GroupByWrapper).IsAssignableFrom(baseQuery.ElementType))
        return (IDictionary<string, Expression>) null;
      if (!(ExpressionBinderBase.SkipWrappers(baseQuery.Expression) is MethodCallExpression expression1))
        return (IDictionary<string, Expression>) null;
      if (expression1 == null)
        return (IDictionary<string, Expression>) null;
      Dictionary<string, Expression> result = new Dictionary<string, Expression>();
      ExpressionBinderBase.CollectContainerAssigments((Expression) source, expression1, result);
      if (hasInstancePropertyContainer)
      {
        MemberExpression source1 = Expression.Property((Expression) source, "Instance");
        if (typeof (DynamicTypeWrapper).IsAssignableFrom(source1.Type) && ExpressionBinderBase.SkipWrappers(expression1.Arguments.FirstOrDefault<Expression>()) is MethodCallExpression expression2)
          ExpressionBinderBase.CollectContainerAssigments((Expression) source1, expression2, result);
      }
      return (IDictionary<string, Expression>) result;
    }

    private static Expression SkipWrappers(Expression expression)
    {
      switch (expression)
      {
        case MethodCallExpression methodCallExpression when ExpressionBinderBase._skippableMethods.Contains(methodCallExpression.Method.Name):
          return ExpressionBinderBase.SkipWrappers(methodCallExpression.Arguments.FirstOrDefault<Expression>());
        case ConditionalExpression conditionalExpression:
          return conditionalExpression.IfFalse;
        default:
          return expression;
      }
    }

    private static void CollectContainerAssigments(
      Expression source,
      MethodCallExpression expression,
      Dictionary<string, Expression> result)
    {
      ExpressionBinderBase.CollectAssigments((IDictionary<string, Expression>) result, (Expression) Expression.Property(source, "GroupByContainer"), ExpressionBinderBase.ExtractContainerExpression(expression.Arguments.FirstOrDefault<Expression>() as MethodCallExpression, "GroupByContainer"));
      ExpressionBinderBase.CollectAssigments((IDictionary<string, Expression>) result, (Expression) Expression.Property(source, "Container"), ExpressionBinderBase.ExtractContainerExpression(expression, "Container"));
    }

    private static MemberInitExpression ExtractContainerExpression(
      MethodCallExpression expression,
      string containerName)
    {
      if (expression == null || expression.Arguments.Count < 2)
        return (MemberInitExpression) null;
      return ((expression.Arguments[1] as UnaryExpression).Operand as LambdaExpression).Body is MemberInitExpression body && body.Bindings.FirstOrDefault<MemberBinding>((Func<MemberBinding, bool>) (m => m.Member.Name == containerName)) is MemberAssignment memberAssignment ? memberAssignment.Expression as MemberInitExpression : (MemberInitExpression) null;
    }

    private static void CollectAssigments(
      IDictionary<string, Expression> flattenPropertyContainer,
      Expression source,
      MemberInitExpression expression,
      string prefix = null)
    {
      if (expression == null)
        return;
      string str = (string) null;
      Type type = (Type) null;
      MemberInitExpression expression1 = (MemberInitExpression) null;
      Expression expression2 = (Expression) null;
      foreach (MemberAssignment memberAssignment in expression.Bindings.OfType<MemberAssignment>())
      {
        if (memberAssignment.Expression is MemberInitExpression expression3 && memberAssignment.Member.Name == "Next")
          expression1 = expression3;
        else if (memberAssignment.Member.Name == "Name")
          str = (memberAssignment.Expression as ConstantExpression).Value as string;
        else if (memberAssignment.Member.Name == "Value" || memberAssignment.Member.Name == "NestedValue")
        {
          type = memberAssignment.Expression.Type;
          if (type == typeof (object) && memberAssignment.Expression.NodeType == ExpressionType.Convert)
            type = ((UnaryExpression) memberAssignment.Expression).Operand.Type;
          if (typeof (GroupByWrapper).IsAssignableFrom(type))
            expression2 = memberAssignment.Expression;
        }
      }
      if (prefix != null)
        str = prefix + "\\" + str;
      if (typeof (GroupByWrapper).IsAssignableFrom(type))
        flattenPropertyContainer.Add(str, (Expression) Expression.Property(source, "NestedValue"));
      else
        flattenPropertyContainer.Add(str, (Expression) Expression.Convert((Expression) Expression.Property(source, "Value"), type));
      if (expression1 != null)
        ExpressionBinderBase.CollectAssigments(flattenPropertyContainer, (Expression) Expression.Property(source, "Next"), expression1, prefix);
      if (expression2 == null)
        return;
      MemberInitExpression expression4 = ((expression2 as MemberInitExpression).Bindings.First<MemberBinding>() as MemberAssignment).Expression as MemberInitExpression;
      MemberExpression source1 = Expression.Property((Expression) Expression.Property(source, "NestedValue"), "GroupByContainer");
      ExpressionBinderBase.CollectAssigments(flattenPropertyContainer, (Expression) source1, expression4, str);
    }

    protected Expression GetFlattenedPropertyExpression(string propertyPath)
    {
      if (this.FlattenedPropertyContainer == null)
        return (Expression) null;
      Expression propertyExpression;
      if (this.FlattenedPropertyContainer.TryGetValue(propertyPath, out propertyExpression))
        return propertyExpression;
      if (this.HasInstancePropertyContainer)
        return (Expression) null;
      throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.PropertyOrPathWasRemovedFromContext, (object) propertyPath));
    }

    private Expression GetProperty(Expression source, string propertyName)
    {
      if (ExpressionBinderBase.IsDateOrOffset(source.Type))
        return ExpressionBinderBase.IsDateTime(source.Type) ? this.MakePropertyAccess(ClrCanonicalFunctions.DateTimeProperties[propertyName], source) : this.MakePropertyAccess(ClrCanonicalFunctions.DateTimeOffsetProperties[propertyName], source);
      if (ExpressionBinderBase.IsDate(source.Type))
        return this.MakePropertyAccess(ClrCanonicalFunctions.DateProperties[propertyName], source);
      if (ExpressionBinderBase.IsTimeOfDay(source.Type))
        return this.MakePropertyAccess(ClrCanonicalFunctions.TimeOfDayProperties[propertyName], source);
      return ExpressionBinderBase.IsTimeSpan(source.Type) ? this.MakePropertyAccess(ClrCanonicalFunctions.TimeSpanProperties[propertyName], source) : source;
    }

    private Expression CreateDateBinaryExpression(Expression source)
    {
      source = ExpressionBinderBase.ConvertToDateTimeRelatedConstExpression(source);
      Expression property1 = this.GetProperty(source, "year");
      Expression property2 = this.GetProperty(source, "month");
      Expression property3 = this.GetProperty(source, "day");
      ConstantExpression right = Expression.Constant((object) 10000);
      return this.CreateFunctionCallWithNullPropagation((Expression) Expression.Add((Expression) Expression.Add((Expression) Expression.Multiply(property1, (Expression) right), (Expression) Expression.Multiply(property2, (Expression) Expression.Constant((object) 100))), property3), new Expression[1]
      {
        source
      });
    }

    private Expression CreateTimeBinaryExpression(Expression source)
    {
      source = ExpressionBinderBase.ConvertToDateTimeRelatedConstExpression(source);
      Expression property1 = this.GetProperty(source, "hour");
      Expression property2 = this.GetProperty(source, "minute");
      Expression property3 = this.GetProperty(source, "second");
      Expression property4 = this.GetProperty(source, "millisecond");
      Type type = typeof (long);
      return this.CreateFunctionCallWithNullPropagation((Expression) Expression.Add((Expression) Expression.Multiply((Expression) Expression.Convert(property1, type), (Expression) Expression.Constant((object) 36000000000L)), (Expression) Expression.Add((Expression) Expression.Multiply((Expression) Expression.Convert(property2, typeof (long)), (Expression) Expression.Constant((object) 600000000L)), (Expression) Expression.Add((Expression) Expression.Multiply((Expression) Expression.Convert(property3, typeof (long)), (Expression) Expression.Constant((object) 10000000L)), (Expression) Expression.Convert(property4, typeof (long))))), new Expression[1]
      {
        source
      });
    }

    private static Expression ConvertToDateTimeRelatedConstExpression(Expression source)
    {
      object parameterizedConstant = ExpressionBinderBase.ExtractParameterizedConstant(source);
      if (parameterizedConstant != null && TypeHelper.IsNullable(source.Type))
      {
        DateTimeOffset? nullable1 = parameterizedConstant as DateTimeOffset?;
        if (nullable1.HasValue)
          return (Expression) Expression.Constant((object) nullable1.Value, typeof (DateTimeOffset));
        DateTime? nullable2 = parameterizedConstant as DateTime?;
        if (nullable2.HasValue)
          return (Expression) Expression.Constant((object) nullable2.Value, typeof (DateTime));
        Date? nullable3 = parameterizedConstant as Date?;
        if (nullable3.HasValue)
          return (Expression) Expression.Constant((object) nullable3.Value, typeof (Date));
        TimeOfDay? nullable4 = parameterizedConstant as TimeOfDay?;
        if (nullable4.HasValue)
          return (Expression) Expression.Constant((object) nullable4.Value, typeof (TimeOfDay));
      }
      return source;
    }

    internal static Expression ConvertToEnumUnderlyingType(
      Expression expression,
      Type enumType,
      Type enumUnderlyingType)
    {
      object parameterizedConstant = ExpressionBinderBase.ExtractParameterizedConstant(expression);
      if (parameterizedConstant != null)
        return parameterizedConstant is string str ? (Expression) Expression.Constant(Convert.ChangeType(Enum.Parse(enumType, str), enumUnderlyingType, (IFormatProvider) CultureInfo.InvariantCulture)) : (Expression) Expression.Constant(Convert.ChangeType(parameterizedConstant, enumUnderlyingType, (IFormatProvider) CultureInfo.InvariantCulture));
      if (expression.Type == enumType)
        return (Expression) Expression.Convert(expression, enumUnderlyingType);
      if (Nullable.GetUnderlyingType(expression.Type) == enumType)
        return (Expression) Expression.Convert(expression, typeof (Nullable<>).MakeGenericType(enumUnderlyingType));
      if (expression.NodeType == ExpressionType.Constant && ((ConstantExpression) expression).Value == null)
        return expression;
      throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.ConvertToEnumFailed, (object) enumType, (object) expression.Type);
    }

    public static object ExtractParameterizedConstant(Expression expression)
    {
      if (expression.NodeType == ExpressionType.MemberAccess)
      {
        MemberExpression memberExpression = expression as MemberExpression;
        PropertyInfo member = memberExpression.Member as PropertyInfo;
        if (member != (PropertyInfo) null && member.GetMethod.IsStatic)
          return member.GetValue(new object());
        if (memberExpression.Expression.NodeType == ExpressionType.Constant)
          return ((memberExpression.Expression as ConstantExpression).Value as LinqParameterContainer).Property;
      }
      return (object) null;
    }

    internal static Expression DateTimeOffsetToDateTime(Expression expression)
    {
      if (expression is UnaryExpression unaryExpression && Nullable.GetUnderlyingType(unaryExpression.Type) == unaryExpression.Operand.Type)
        expression = unaryExpression.Operand;
      DateTimeOffset? parameterizedConstant = ExpressionBinderBase.ExtractParameterizedConstant(expression) as DateTimeOffset?;
      if (parameterizedConstant.HasValue)
        expression = (Expression) Expression.Constant(EdmPrimitiveHelpers.ConvertPrimitiveValue((object) parameterizedConstant.Value, typeof (DateTime)));
      return expression;
    }

    internal static bool IsNullable(Type t) => !TypeHelper.IsValueType(t) || t.IsGenericType() && t.GetGenericTypeDefinition() == typeof (Nullable<>);

    internal static Type ToNullable(Type t)
    {
      if (ExpressionBinderBase.IsNullable(t))
        return t;
      return typeof (Nullable<>).MakeGenericType(t);
    }

    internal static Expression ToNullable(Expression expression) => !ExpressionBinderBase.IsNullable(expression.Type) ? (Expression) Expression.Convert(expression, ExpressionBinderBase.ToNullable(expression.Type)) : expression;

    internal static bool IsIQueryable(Type type) => typeof (IQueryable).IsAssignableFrom(type);

    internal static bool IsDoubleOrDecimal(Type type) => ExpressionBinderBase.IsType<double>(type) || ExpressionBinderBase.IsType<Decimal>(type);

    internal static bool IsDateAndTimeRelated(Type type) => ExpressionBinderBase.IsType<Date>(type) || ExpressionBinderBase.IsType<DateTime>(type) || ExpressionBinderBase.IsType<DateTimeOffset>(type) || ExpressionBinderBase.IsType<TimeOfDay>(type) || ExpressionBinderBase.IsType<TimeSpan>(type);

    internal static bool IsDateRelated(Type type) => ExpressionBinderBase.IsType<Date>(type) || ExpressionBinderBase.IsType<DateTime>(type) || ExpressionBinderBase.IsType<DateTimeOffset>(type);

    internal static bool IsTimeRelated(Type type) => ExpressionBinderBase.IsType<TimeOfDay>(type) || ExpressionBinderBase.IsType<DateTime>(type) || ExpressionBinderBase.IsType<DateTimeOffset>(type) || ExpressionBinderBase.IsType<TimeSpan>(type);

    internal static bool IsDateOrOffset(Type type) => ExpressionBinderBase.IsType<DateTime>(type) || ExpressionBinderBase.IsType<DateTimeOffset>(type);

    internal static bool IsDateTime(Type type) => ExpressionBinderBase.IsType<DateTime>(type);

    internal static bool IsTimeSpan(Type type) => ExpressionBinderBase.IsType<TimeSpan>(type);

    internal static bool IsTimeOfDay(Type type) => ExpressionBinderBase.IsType<TimeOfDay>(type);

    internal static bool IsDate(Type type) => ExpressionBinderBase.IsType<Date>(type);

    internal static bool IsInteger(Type type) => ExpressionBinderBase.IsType<short>(type) || ExpressionBinderBase.IsType<int>(type) || ExpressionBinderBase.IsType<long>(type);

    internal static bool IsType<T>(Type type) where T : struct => type == typeof (T) || type == typeof (T?);

    internal static Expression ConvertNull(Expression expression, Type type) => expression is ConstantExpression constantExpression && constantExpression.Value == null ? (Expression) Expression.Constant((object) null, type) : expression;

    internal Expression CreatePropertyAccessExpression(
      Expression source,
      IEdmProperty property,
      string propertyPath = null)
    {
      string clrPropertyName = EdmLibHelpers.GetClrPropertyName(property, this.Model);
      propertyPath = propertyPath ?? clrPropertyName;
      if (this.QuerySettings.HandleNullPropagation != HandleNullPropagationOption.True || !ExpressionBinderBase.IsNullable(source.Type) || source == this.ItParameter)
        return this.GetFlattenedPropertyExpression(propertyPath) ?? this.ConvertNonStandardPrimitives(ExpressionBinderBase.GetPropertyExpression(source, clrPropertyName, propertyPath));
      Expression source1 = this.RemoveInnerNullPropagation(source);
      Expression nullable = ExpressionBinderBase.ToNullable(this.ConvertNonStandardPrimitives(this.GetFlattenedPropertyExpression(propertyPath) ?? ExpressionBinderBase.GetPropertyExpression(source1, clrPropertyName, propertyPath)));
      return (Expression) Expression.Condition((Expression) Expression.Equal(source, ExpressionBinderBase.NullConstant), (Expression) Expression.Constant((object) null, nullable.Type), nullable);
    }

    internal static Expression GetPropertyExpression(
      Expression source,
      string propertyName,
      string propertyPath)
    {
      propertyPath = !ExpressionBinderBase.TypeHasInstancePropertyContainer(source.Type) || propertyPath != null && propertyPath.Contains("Instance\\") ? propertyName : "Instance\\" + propertyName;
      return ExpressionBinderBase.GetPropertyExpression(source, propertyPath);
    }

    internal static Expression GetPropertyExpression(Expression source, string propertyPath)
    {
      string[] strArray = propertyPath.Split('\\');
      Expression expression = source;
      foreach (string name in strArray)
      {
        PropertyInfo property = expression.Type.GetProperty(name);
        expression = !(property == (PropertyInfo) null) ? (Expression) Expression.Property(expression, property) : throw new ArgumentException("No property named " + name + " is defined in " + expression.Type?.ToString() + " or its base types.");
      }
      return expression;
    }

    public Expression Bind(QueryNode node) => this.Bind(node, (Expression) null);

    public abstract Expression Bind(QueryNode node, Expression baseElement);

    protected abstract ParameterExpression ItParameter { get; }

    public virtual Expression BindConstantNode(ConstantNode constantNode)
    {
      if (constantNode.Value == null)
        return ExpressionBinderBase.NullConstant;
      Type type1 = EdmLibHelpers.GetClrType(constantNode.TypeReference, this.Model, this.InternalAssembliesResolver);
      object obj = constantNode.Value;
      if (constantNode.TypeReference != null && constantNode.TypeReference.IsEnum())
      {
        string str = ((ODataEnumValue) obj).Value;
        Type type2 = Nullable.GetUnderlyingType(type1);
        if ((object) type2 == null)
          type2 = type1;
        type1 = type2;
        obj = Enum.Parse(type1, str);
      }
      if (constantNode.TypeReference != null && constantNode.TypeReference.IsNullable && (constantNode.TypeReference.IsDate() || constantNode.TypeReference.IsTimeOfDay()))
      {
        Type type3 = Nullable.GetUnderlyingType(type1);
        if ((object) type3 == null)
          type3 = type1;
        type1 = type3;
      }
      return this.QuerySettings.EnableConstantParameterization ? LinqParameterContainer.Parameterize(type1, obj) : (Expression) Expression.Constant(obj, type1);
    }

    public virtual Expression BindSingleValueFunctionCallNode(SingleValueFunctionCallNode node)
    {
      switch (node.Name)
      {
        case "cast":
          return this.BindCastSingleValue(node);
        case "ceiling":
          return this.BindCeiling(node);
        case "concat":
          return this.BindConcat(node);
        case "contains":
          return this.BindContains(node);
        case "date":
          return this.BindDate(node);
        case "day":
        case "month":
        case "year":
          return this.BindDateRelatedProperty(node);
        case "endswith":
          return this.BindEndsWith(node);
        case "floor":
          return this.BindFloor(node);
        case "fractionalseconds":
          return this.BindFractionalSeconds(node);
        case "hour":
        case "minute":
        case "second":
          return this.BindTimeRelatedProperty(node);
        case "iif":
          return this.BindIif(node);
        case "indexof":
          return this.BindIndexOf(node);
        case "isof":
          return this.BindIsOf(node);
        case "length":
          return this.BindLength(node);
        case "now":
          return this.BindNow(node);
        case "round":
          return this.BindRound(node);
        case "startswith":
          return this.BindStartsWith(node);
        case "substring":
          return this.BindSubstring(node);
        case "time":
          return this.BindTime(node);
        case "tolower":
          return this.BindToLower(node);
        case "toupper":
          return this.BindToUpper(node);
        case "trim":
          return this.BindTrim(node);
        default:
          return this.BindCustomMethodExpressionOrNull<SingleValueFunctionCallNode>(node) ?? throw new NotImplementedException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.ODataFunctionNotSupported, (object) node.Name));
      }
    }

    private Expression BindIif(SingleValueFunctionCallNode node)
    {
      Expression[] expressionArray = this.BindArguments(node.Parameters);
      if (this.QuerySettings.HandleNullPropagation == HandleNullPropagationOption.True)
      {
        for (int index = 0; index < expressionArray.Length; ++index)
          expressionArray[index] = this.RemoveInnerNullPropagation(expressionArray[index]);
      }
      if (expressionArray[0].Type == typeof (bool?))
        expressionArray[0] = (Expression) Expression.Convert(expressionArray[0], typeof (bool));
      if (expressionArray[1] == ExpressionBinderBase.NullConstant && expressionArray[2] == ExpressionBinderBase.NullConstant)
        return ExpressionBinderBase.NullConstant;
      Expression expression1 = ExpressionBinderBase.NormalizeNullConstant(expressionArray[1], expressionArray[2]);
      Expression expression2 = ExpressionBinderBase.NormalizeNullConstant(expressionArray[2], expressionArray[1]);
      if (expression1.Type != expression2.Type)
      {
        if (TypeHelper.IsNullable(expression1.Type))
          expression2 = ExpressionBinderBase.ToNullable(expression2);
        else
          expression1 = ExpressionBinderBase.ToNullable(expression1);
      }
      return (Expression) Expression.Condition(expressionArray[0], expression1, expression2);
    }

    private static Expression NormalizeNullConstant(Expression first, Expression second) => first != ExpressionBinderBase.NullConstant ? first : (Expression) Expression.Constant((object) null, TypeHelper.ToNullable(second.Type));

    private Expression BindCastSingleValue(SingleValueFunctionCallNode node)
    {
      Expression[] expressionArray = this.BindArguments(node.Parameters);
      Expression expression = expressionArray.Length == 1 ? (Expression) this.ItParameter : expressionArray[0];
      IEdmType type1 = (IEdmType) this.Model.FindType((string) ((ConstantNode) node.Parameters.Last<QueryNode>()).Value);
      Type type2 = (Type) null;
      if (type1 != null)
      {
        IEdmTypeReference edmTypeReference = type1.ToEdmTypeReference(false);
        type2 = EdmLibHelpers.GetClrType(edmTypeReference, this.Model);
        if (expression != ExpressionBinderBase.NullConstant)
        {
          if (expression.Type == type2)
            return expression;
          if (!edmTypeReference.IsPrimitive() && !edmTypeReference.IsEnum() || EdmLibHelpers.GetEdmPrimitiveTypeOrNull(expression.Type) == null && !TypeHelper.IsEnum(expression.Type))
            return ExpressionBinderBase.NullConstant;
        }
      }
      if (type2 == (Type) null || expression == ExpressionBinderBase.NullConstant)
        return ExpressionBinderBase.NullConstant;
      if (type2 == typeof (string))
        return ExpressionBinderBase.BindCastToStringType(expression);
      if (TypeHelper.IsEnum(type2))
        return this.BindCastToEnumType(expression.Type, type2, node.Parameters.First<QueryNode>(), expressionArray.Length);
      if (TypeHelper.IsNullable(expression.Type) && !TypeHelper.IsNullable(type2))
        type2 = typeof (Nullable<>).MakeGenericType(type2);
      try
      {
        return (Expression) Expression.Convert(expression, type2);
      }
      catch (InvalidOperationException ex)
      {
        return ExpressionBinderBase.NullConstant;
      }
    }

    private static Expression BindCastToStringType(Expression source)
    {
      if (!source.Type.IsGenericType || !(source.Type.GetGenericTypeDefinition() == typeof (Nullable<>)))
        return (Expression) Expression.Call(TypeHelper.IsEnum(source.Type) ? (Expression) Expression.Convert(source, Enum.GetUnderlyingType(source.Type)) : source, "ToString", (Type[]) null, (Expression[]) null);
      Expression instance = !TypeHelper.IsEnum(source.Type) ? (Expression) Expression.Property(source, "Value") : (Expression) Expression.Convert((Expression) Expression.Property(source, "Value"), Enum.GetUnderlyingType(TypeHelper.GetUnderlyingTypeOrSelf(source.Type)));
      return (Expression) Expression.Condition((Expression) Expression.Property(source, "HasValue"), (Expression) Expression.Call(instance, "ToString", (Type[]) null, (Expression[]) null), (Expression) Expression.Constant((object) null, typeof (string)));
    }

    private Expression BindCastToEnumType(
      Type sourceType,
      Type targetClrType,
      QueryNode firstParameter,
      int parameterLength)
    {
      Type underlyingTypeOrSelf = TypeHelper.GetUnderlyingTypeOrSelf(targetClrType);
      ConstantNode constantNode = firstParameter as ConstantNode;
      if (parameterLength == 1 || constantNode == null || sourceType != typeof (string))
        return ExpressionBinderBase.NullConstant;
      object[] parameters = new object[2]
      {
        constantNode.Value,
        Enum.ToObject(underlyingTypeOrSelf, 0)
      };
      if (!(bool) ExpressionBinderBase.EnumTryParseMethod.MakeGenericMethod(underlyingTypeOrSelf).Invoke((object) null, parameters))
        return ExpressionBinderBase.NullConstant;
      return this.QuerySettings.EnableConstantParameterization ? LinqParameterContainer.Parameterize(targetClrType, parameters[1]) : (Expression) Expression.Constant(parameters[1], targetClrType);
    }

    private Expression BindIsOf(SingleValueFunctionCallNode node)
    {
      Expression[] expressionArray = this.BindArguments(node.Parameters);
      Expression expression = expressionArray.Length == 1 ? (Expression) this.ItParameter : expressionArray[0];
      if (expression == ExpressionBinderBase.NullConstant)
        return ExpressionBinderBase.FalseConstant;
      IEdmType type1 = (IEdmType) this.Model.FindType((string) ((ConstantNode) node.Parameters.Last<QueryNode>()).Value);
      Type type2 = (Type) null;
      if (type1 != null)
        type2 = EdmLibHelpers.GetClrType(type1.ToEdmTypeReference(false), this.Model);
      if (type2 == (Type) null)
        return ExpressionBinderBase.FalseConstant;
      if (((EdmLibHelpers.GetEdmPrimitiveTypeOrNull(expression.Type) != null ? 1 : (TypeHelper.IsEnum(expression.Type) ? 1 : 0)) & (EdmLibHelpers.GetEdmPrimitiveTypeOrNull(type2) != null ? (true ? 1 : 0) : (TypeHelper.IsEnum(type2) ? 1 : 0))) != 0 && TypeHelper.IsNullable(expression.Type))
        type2 = TypeHelper.ToNullable(type2);
      return (Expression) Expression.Condition((Expression) Expression.TypeIs(expression, type2), ExpressionBinderBase.TrueConstant, ExpressionBinderBase.FalseConstant);
    }

    private Expression BindCeiling(SingleValueFunctionCallNode node)
    {
      Expression[] expressionArray = this.BindArguments(node.Parameters);
      return this.MakeFunctionCall(ExpressionBinderBase.IsType<double>(expressionArray[0].Type) ? (MemberInfo) ClrCanonicalFunctions.CeilingOfDouble : (MemberInfo) ClrCanonicalFunctions.CeilingOfDecimal, expressionArray);
    }

    private Expression BindFloor(SingleValueFunctionCallNode node)
    {
      Expression[] expressionArray = this.BindArguments(node.Parameters);
      return this.MakeFunctionCall(ExpressionBinderBase.IsType<double>(expressionArray[0].Type) ? (MemberInfo) ClrCanonicalFunctions.FloorOfDouble : (MemberInfo) ClrCanonicalFunctions.FloorOfDecimal, expressionArray);
    }

    private Expression BindRound(SingleValueFunctionCallNode node)
    {
      Expression[] expressionArray = this.BindArguments(node.Parameters);
      return this.MakeFunctionCall(ExpressionBinderBase.IsType<double>(expressionArray[0].Type) ? (MemberInfo) ClrCanonicalFunctions.RoundOfDouble : (MemberInfo) ClrCanonicalFunctions.RoundOfDecimal, expressionArray);
    }

    private Expression BindDate(SingleValueFunctionCallNode node) => this.BindArguments(node.Parameters)[0];

    private Expression BindNow(SingleValueFunctionCallNode node)
    {
      this.BindArguments(node.Parameters);
      return (Expression) Expression.Property((Expression) null, typeof (DateTimeOffset), "UtcNow");
    }

    private Expression BindTime(SingleValueFunctionCallNode node) => this.BindArguments(node.Parameters)[0];

    private Expression BindFractionalSeconds(SingleValueFunctionCallNode node)
    {
      Expression[] arguments = this.BindArguments(node.Parameters);
      Expression expression = arguments[0];
      return this.CreateFunctionCallWithNullPropagation((Expression) Expression.Divide((Expression) Expression.Convert(this.MakePropertyAccess(!ExpressionBinderBase.IsTimeOfDay(expression.Type) ? (!ExpressionBinderBase.IsDateTime(expression.Type) ? (!ExpressionBinderBase.IsTimeSpan(expression.Type) ? ClrCanonicalFunctions.DateTimeOffsetProperties["millisecond"] : ClrCanonicalFunctions.TimeSpanProperties["millisecond"]) : ClrCanonicalFunctions.DateTimeProperties["millisecond"]) : ClrCanonicalFunctions.TimeOfDayProperties["millisecond"], expression), typeof (Decimal)), (Expression) Expression.Constant((object) 1000M, typeof (Decimal))), arguments);
    }

    private Expression BindDateRelatedProperty(SingleValueFunctionCallNode node)
    {
      Expression bindArgument = this.BindArguments(node.Parameters)[0];
      return this.MakeFunctionCall((MemberInfo) (!ExpressionBinderBase.IsDate(bindArgument.Type) ? (!ExpressionBinderBase.IsDateTime(bindArgument.Type) ? ClrCanonicalFunctions.DateTimeOffsetProperties[node.Name] : ClrCanonicalFunctions.DateTimeProperties[node.Name]) : ClrCanonicalFunctions.DateProperties[node.Name]), bindArgument);
    }

    private Expression BindTimeRelatedProperty(SingleValueFunctionCallNode node)
    {
      Expression bindArgument = this.BindArguments(node.Parameters)[0];
      return this.MakeFunctionCall((MemberInfo) (!ExpressionBinderBase.IsTimeOfDay(bindArgument.Type) ? (!ExpressionBinderBase.IsDateTime(bindArgument.Type) ? (!ExpressionBinderBase.IsTimeSpan(bindArgument.Type) ? ClrCanonicalFunctions.DateTimeOffsetProperties[node.Name] : ClrCanonicalFunctions.TimeSpanProperties[node.Name]) : ClrCanonicalFunctions.DateTimeProperties[node.Name]) : ClrCanonicalFunctions.TimeOfDayProperties[node.Name]), bindArgument);
    }

    private Expression BindConcat(SingleValueFunctionCallNode node)
    {
      Expression[] arguments = this.BindArguments(node.Parameters);
      ExpressionBinderBase.ValidateAllStringArguments(node.Name, arguments);
      return this.MakeFunctionCall((MemberInfo) ClrCanonicalFunctions.Concat, arguments);
    }

    private Expression BindTrim(SingleValueFunctionCallNode node)
    {
      Expression[] arguments = this.BindArguments(node.Parameters);
      ExpressionBinderBase.ValidateAllStringArguments(node.Name, arguments);
      return this.MakeFunctionCall((MemberInfo) ClrCanonicalFunctions.Trim, arguments);
    }

    private Expression BindToUpper(SingleValueFunctionCallNode node)
    {
      Expression[] arguments = this.BindArguments(node.Parameters);
      ExpressionBinderBase.ValidateAllStringArguments(node.Name, arguments);
      return this.MakeFunctionCall((MemberInfo) ClrCanonicalFunctions.ToUpper, arguments);
    }

    private Expression BindToLower(SingleValueFunctionCallNode node)
    {
      Expression[] arguments = this.BindArguments(node.Parameters);
      ExpressionBinderBase.ValidateAllStringArguments(node.Name, arguments);
      return this.MakeFunctionCall((MemberInfo) ClrCanonicalFunctions.ToLower, arguments);
    }

    private Expression BindIndexOf(SingleValueFunctionCallNode node)
    {
      Expression[] arguments = this.BindArguments(node.Parameters);
      ExpressionBinderBase.ValidateAllStringArguments(node.Name, arguments);
      return this.MakeFunctionCall((MemberInfo) ClrCanonicalFunctions.IndexOf, arguments);
    }

    private Expression BindSubstring(SingleValueFunctionCallNode node)
    {
      Expression[] expressionArray = this.BindArguments(node.Parameters);
      if (expressionArray[0].Type != typeof (string))
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.FunctionNotSupportedOnEnum, (object) node.Name));
      return expressionArray.Length != 2 ? (this.QuerySettings.HandleNullPropagation != HandleNullPropagationOption.True ? this.MakeFunctionCall((MemberInfo) ClrCanonicalFunctions.SubstringStartAndLength, expressionArray) : this.MakeFunctionCall((MemberInfo) ClrCanonicalFunctions.SubstringStartAndLengthNoThrow, expressionArray)) : (this.QuerySettings.HandleNullPropagation != HandleNullPropagationOption.True ? this.MakeFunctionCall((MemberInfo) ClrCanonicalFunctions.SubstringStart, expressionArray) : this.MakeFunctionCall((MemberInfo) ClrCanonicalFunctions.SubstringStartNoThrow, expressionArray));
    }

    private Expression BindLength(SingleValueFunctionCallNode node)
    {
      Expression[] arguments = this.BindArguments(node.Parameters);
      ExpressionBinderBase.ValidateAllStringArguments(node.Name, arguments);
      return this.MakeFunctionCall((MemberInfo) ClrCanonicalFunctions.Length, arguments);
    }

    private Expression BindContains(SingleValueFunctionCallNode node)
    {
      Expression[] arguments = this.BindArguments(node.Parameters);
      ExpressionBinderBase.ValidateAllStringArguments(node.Name, arguments);
      return this.MakeFunctionCall((MemberInfo) ClrCanonicalFunctions.Contains, arguments[0], arguments[1]);
    }

    private Expression BindStartsWith(SingleValueFunctionCallNode node)
    {
      Expression[] arguments = this.BindArguments(node.Parameters);
      ExpressionBinderBase.ValidateAllStringArguments(node.Name, arguments);
      return this.MakeFunctionCall((MemberInfo) ClrCanonicalFunctions.StartsWith, arguments);
    }

    private Expression BindEndsWith(SingleValueFunctionCallNode node)
    {
      Expression[] arguments = this.BindArguments(node.Parameters);
      ExpressionBinderBase.ValidateAllStringArguments(node.Name, arguments);
      return this.MakeFunctionCall((MemberInfo) ClrCanonicalFunctions.EndsWith, arguments);
    }

    protected Expression[] BindArguments(IEnumerable<QueryNode> nodes) => nodes.OfType<SingleValueNode>().Select<SingleValueNode, Expression>((Func<SingleValueNode, Expression>) (n => this.Bind((QueryNode) n))).ToArray<Expression>();

    private static void ValidateAllStringArguments(string functionName, Expression[] arguments)
    {
      if (((IEnumerable<Expression>) arguments).Any<Expression>((Func<Expression, bool>) (arg => arg.Type != typeof (string))))
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.FunctionNotSupportedOnEnum, (object) functionName));
    }

    internal Expression BindCustomMethodExpressionOrNull<T>(T node) where T : QueryNode, IFunctionCallNode
    {
      Expression[] source1 = this.BindArguments(node.Parameters);
      IEnumerable<Type> methodArgumentsType1 = ((IEnumerable<Expression>) source1).Select<Expression, Type>((Func<Expression, Type>) (argument => argument.Type));
      MethodInfo methodInfo;
      if (UriFunctionsBinder.TryGetMethodInfo(node.Name, methodArgumentsType1, out methodInfo))
        return this.MakeFunctionCall((MemberInfo) methodInfo, false, source1);
      if (node.Source != null)
      {
        Expression[] source2 = this.BindArguments(new List<QueryNode>()
        {
          node.Source
        }.Concat<QueryNode>(node.Parameters.OfType<NamedFunctionParameterNode>().Select<NamedFunctionParameterNode, QueryNode>((Func<NamedFunctionParameterNode, QueryNode>) (p => p.Value))));
        if (typeof (ISelectExpandWrapper).IsAssignableFrom(source2[0].Type))
          source2[0] = (Expression) Expression.Property(source2[0], "Instance");
        IEnumerable<Type> methodArgumentsType2 = ((IEnumerable<Expression>) source2).Select<Expression, Type>((Func<Expression, Type>) (argument => argument.Type));
        if (UriFunctionsBinder.TryGetMethodInfo(node.Name, methodArgumentsType2, out methodInfo))
          return this.MakeFunctionCall((MemberInfo) methodInfo, false, source2);
      }
      return (Expression) null;
    }

    public static int GuidCompare(Guid firstValue, Guid secondValue) => firstValue.CompareTo(secondValue);
  }
}
