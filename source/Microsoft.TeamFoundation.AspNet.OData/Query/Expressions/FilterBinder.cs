// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.FilterBinder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  public class FilterBinder : ExpressionBinderBase
  {
    private const string ODataItParameterName = "$it";
    private Stack<Dictionary<string, ParameterExpression>> _parametersStack = new Stack<Dictionary<string, ParameterExpression>>();
    private Dictionary<string, ParameterExpression> _lambdaParameters;
    private Type _filterType;

    public FilterBinder(IServiceProvider requestContainer)
      : base(requestContainer)
    {
    }

    internal FilterBinder(
      ODataQuerySettings settings,
      IWebApiAssembliesResolver assembliesResolver,
      IEdmModel model)
      : base(model, assembliesResolver, settings)
    {
    }

    internal static Expression Bind(
      IQueryable baseQuery,
      FilterClause filterClause,
      Type filterType,
      ODataQueryContext context,
      ODataQuerySettings querySettings)
    {
      if (filterClause == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (filterClause));
      if (filterType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (filterType));
      FilterBinder binder = context != null ? FilterBinder.GetOrCreateFilterBinder(context, querySettings) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (context));
      FilterBinder filterBinder = binder;
      Type type = baseQuery?.ElementType;
      if ((object) type == null)
        type = filterType;
      filterBinder._filterType = type;
      binder.BaseQuery = baseQuery;
      return (Expression) FilterBinder.BindFilterClause(binder, filterClause, binder._filterType);
    }

    internal static LambdaExpression Bind(
      IQueryable baseQuery,
      OrderByClause orderBy,
      Type elementType,
      ODataQueryContext context,
      ODataQuerySettings querySettings)
    {
      FilterBinder filterBinder1 = FilterBinder.GetOrCreateFilterBinder(context, querySettings);
      FilterBinder filterBinder2 = filterBinder1;
      Type type = baseQuery?.ElementType;
      if ((object) type == null)
        type = elementType;
      filterBinder2._filterType = type;
      filterBinder1.BaseQuery = baseQuery;
      return FilterBinder.BindOrderByClause(filterBinder1, orderBy, filterBinder1._filterType);
    }

    private static FilterBinder GetOrCreateFilterBinder(
      ODataQueryContext context,
      ODataQuerySettings querySettings)
    {
      FilterBinder filterBinder = (FilterBinder) null;
      if (context.RequestContainer != null)
      {
        filterBinder = ServiceProviderServiceExtensions.GetRequiredService<FilterBinder>(context.RequestContainer);
        if (filterBinder != null && filterBinder.Model != context.Model && filterBinder.Model == EdmCoreModel.Instance)
          filterBinder.Model = context.Model;
      }
      return filterBinder ?? new FilterBinder(querySettings, WebApiAssembliesResolver.Default, context.Model);
    }

    private FilterBinder(
      IEdmModel model,
      IWebApiAssembliesResolver assembliesResolver,
      ODataQuerySettings querySettings,
      Type filterType)
      : base(model, assembliesResolver, querySettings)
    {
      this._filterType = filterType;
    }

    internal static Expression<Func<TEntityType, bool>> Bind<TEntityType>(
      FilterClause filterClause,
      IEdmModel model,
      IWebApiAssembliesResolver assembliesResolver,
      ODataQuerySettings querySettings)
    {
      return FilterBinder.Bind(filterClause, typeof (TEntityType), model, assembliesResolver, querySettings) as Expression<Func<TEntityType, bool>>;
    }

    internal static Expression Bind(
      FilterClause filterClause,
      Type filterType,
      IEdmModel model,
      IWebApiAssembliesResolver assembliesResolver,
      ODataQuerySettings querySettings)
    {
      if (filterClause == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (filterClause));
      if (filterType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (filterType));
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      if (assembliesResolver == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (assembliesResolver));
      return (Expression) FilterBinder.BindFilterClause(new FilterBinder(model, assembliesResolver, querySettings, filterType), filterClause, filterType);
    }

    private static LambdaExpression BindFilterClause(
      FilterBinder binder,
      FilterClause filterClause,
      Type filterType)
    {
      LambdaExpression lambdaExpression1 = binder.BindExpression(filterClause.Expression, filterClause.RangeVariable, filterType);
      LambdaExpression lambdaExpression2 = Expression.Lambda(binder.ApplyNullPropagationForFilterBody(lambdaExpression1.Body), (IEnumerable<ParameterExpression>) lambdaExpression1.Parameters);
      Type type = typeof (Func<,>).MakeGenericType(filterType, typeof (bool));
      if (lambdaExpression2.Type != type)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (filterType), SRResources.CannotCastFilter, (object) lambdaExpression2.Type.FullName, (object) type.FullName);
      return lambdaExpression2;
    }

    private static LambdaExpression BindOrderByClause(
      FilterBinder binder,
      OrderByClause orderBy,
      Type elementType)
    {
      return binder.BindExpression(orderBy.Expression, orderBy.RangeVariable, elementType);
    }

    public override Expression Bind(QueryNode node, Expression baseElement)
    {
      RuntimeHelpers.EnsureSufficientExecutionStack();
      CollectionNode node1 = node as CollectionNode;
      SingleValueNode node2 = node as SingleValueNode;
      if (node1 != null)
        return this.BindCollectionNode(node1);
      return node2 != null ? this.BindSingleValueNode(node2) : throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.QueryNodeBindingNotSupported, (object) node.Kind, (object) typeof (FilterBinder).Name);
    }

    private Expression BindCountNode(CountNode node)
    {
      Expression left = this.Bind((QueryNode) node.Source);
      Expression expression1 = (Expression) Expression.Constant((object) null, typeof (long?));
      Type elementType;
      if (!TypeHelper.IsCollection(left.Type, out elementType))
        return expression1;
      MethodInfo method;
      if (typeof (IQueryable).IsAssignableFrom(left.Type))
        method = ExpressionHelperMethods.QueryableCountGeneric.MakeGenericMethod(elementType);
      else
        method = ExpressionHelperMethods.EnumerableCountGeneric.MakeGenericMethod(elementType);
      Expression expression2 = (Expression) Expression.Call((Expression) null, method, left);
      return this.QuerySettings.HandleNullPropagation == HandleNullPropagationOption.True ? (Expression) Expression.Condition((Expression) Expression.Equal(left, (Expression) Expression.Constant((object) null)), (Expression) Expression.Constant((object) null, typeof (long?)), ExpressionHelpers.ToNullable(expression2)) : expression2;
    }

    public virtual Expression BindDynamicPropertyAccessQueryNode(
      SingleValueOpenPropertyAccessNode openNode)
    {
      if (EdmLibHelpers.IsDynamicTypeWrapper(this._filterType))
        return this.GetFlattenedPropertyExpression(openNode.Name) ?? (Expression) Expression.Property(this.Bind((QueryNode) openNode.Source), openNode.Name);
      PropertyInfo propertyContainer = this.GetDynamicPropertyContainer(openNode);
      Expression expression = this.BindPropertyAccessExpression(openNode, propertyContainer);
      IndexExpression ifTrue = Expression.Property(expression, ExpressionBinderBase.DictionaryStringObjectIndexerName, (Expression) Expression.Constant((object) openNode.Name));
      MethodCallExpression methodCallExpression = Expression.Call(expression, expression.Type.GetMethod("ContainsKey"), (Expression) Expression.Constant((object) openNode.Name));
      ConstantExpression ifFalse = Expression.Constant((object) null);
      return this.QuerySettings.HandleNullPropagation == HandleNullPropagationOption.True ? (Expression) Expression.Condition((Expression) Expression.AndAlso((Expression) Expression.NotEqual(expression, (Expression) Expression.Constant((object) null)), (Expression) methodCallExpression), (Expression) ifTrue, (Expression) ifFalse) : (Expression) Expression.Condition((Expression) methodCallExpression, (Expression) ifTrue, (Expression) ifFalse);
    }

    private Expression BindPropertyAccessExpression(
      SingleValueOpenPropertyAccessNode openNode,
      PropertyInfo prop)
    {
      Expression expression = this.Bind((QueryNode) openNode.Source);
      return this.QuerySettings.HandleNullPropagation != HandleNullPropagationOption.True || !ExpressionBinderBase.IsNullable(expression.Type) || expression == this._lambdaParameters["$it"] ? (Expression) Expression.Property(expression, prop.Name) : (Expression) Expression.Property(this.RemoveInnerNullPropagation(expression), prop.Name);
    }

    public virtual Expression BindSingleResourceFunctionCallNode(SingleResourceFunctionCallNode node)
    {
      switch (node.Name)
      {
        case "cast":
          return this.BindSingleResourceCastFunctionCall(node);
        default:
          throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.ODataFunctionNotSupported, (object) node.Name);
      }
    }

    private Expression BindSingleResourceCastFunctionCall(SingleResourceFunctionCallNode node)
    {
      Expression[] expressionArray = this.BindArguments(node.Parameters);
      IEdmType type1 = (IEdmType) this.Model.FindType((string) ((ConstantNode) node.Parameters.Last<QueryNode>()).Value);
      Type type2 = (Type) null;
      if (type1 != null)
        type2 = EdmLibHelpers.GetClrType(type1.ToEdmTypeReference(false), this.Model);
      return expressionArray[0].Type == type2 ? expressionArray[0] : ExpressionBinderBase.NullConstant;
    }

    public virtual Expression BindSingleResourceCastNode(SingleResourceCastNode node)
    {
      Type clrType = EdmLibHelpers.GetClrType((IEdmTypeReference) node.StructuredTypeReference, this.Model);
      return (Expression) Expression.TypeAs(this.BindCastSourceNode((QueryNode) node.Source), clrType);
    }

    public virtual Expression BindCollectionResourceCastNode(CollectionResourceCastNode node)
    {
      Type clrType = EdmLibHelpers.GetClrType((IEdmTypeReference) node.ItemStructuredType, this.Model);
      return FilterBinder.OfType(this.BindCastSourceNode((QueryNode) node.Source), clrType);
    }

    public virtual Expression BindCollectionConstantNode(CollectionConstantNode node)
    {
      ConstantNode constantNode1 = node.Collection.FirstOrDefault<ConstantNode>();
      object obj1 = (object) null;
      if (constantNode1 != null)
        obj1 = constantNode1.Value;
      Type type = this.RetrieveClrTypeForConstant(node.ItemType, ref obj1);
      IList instance = Activator.CreateInstance(typeof (List<>).MakeGenericType(type)) as IList;
      foreach (ConstantNode constantNode2 in (IEnumerable<ConstantNode>) node.Collection)
      {
        object obj2 = type.IsEnum ? EnumDeserializationHelpers.ConvertEnumValue(constantNode2.Value, type) : constantNode2.Value;
        instance.Add(obj2);
      }
      return (Expression) Expression.Constant((object) instance);
    }

    private Expression BindCastSourceNode(QueryNode sourceNode) => sourceNode != null ? this.Bind(sourceNode) : (Expression) this._lambdaParameters["$it"];

    private static Expression OfType(Expression source, Type elementType) => ExpressionBinderBase.IsIQueryable(source.Type) ? (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.QueryableOfType.MakeGenericMethod(elementType), source) : (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.EnumerableOfType.MakeGenericMethod(elementType), source);

    public virtual Expression BindNavigationPropertyNode(
      QueryNode sourceNode,
      IEdmNavigationProperty navigationProperty)
    {
      return this.BindNavigationPropertyNode(sourceNode, navigationProperty, (string) null);
    }

    public virtual Expression BindNavigationPropertyNode(
      QueryNode sourceNode,
      IEdmNavigationProperty navigationProperty,
      string propertyPath)
    {
      return this.CreatePropertyAccessExpression(sourceNode != null ? this.Bind(sourceNode) : (Expression) this._lambdaParameters["$it"], (IEdmProperty) navigationProperty, propertyPath);
    }

    public virtual Expression BindInNode(InNode inNode)
    {
      Expression expression1 = this.Bind((QueryNode) inNode.Left);
      Expression expression2 = this.Bind((QueryNode) inNode.Right);
      return ExpressionBinderBase.IsIQueryable(expression2.Type) ? (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.QueryableContainsGeneric.MakeGenericMethod(expression1.Type), expression2, expression1) : (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.EnumerableContainsGeneric.MakeGenericMethod(expression1.Type), expression2, expression1);
    }

    public virtual Expression BindConvertNode(ConvertNode convertNode)
    {
      Expression source = this.Bind((QueryNode) convertNode.Source);
      return this.CreateConvertExpression(convertNode, source);
    }

    public LambdaExpression BindExpression(
      SingleValueNode expression,
      RangeVariable rangeVariable,
      Type elementType)
    {
      ParameterExpression source = Expression.Parameter(elementType, rangeVariable.Name);
      this._lambdaParameters = new Dictionary<string, ParameterExpression>();
      this._lambdaParameters.Add(rangeVariable.Name, source);
      this.EnsureFlattenedPropertyContainer(source);
      return Expression.Lambda(this.Bind((QueryNode) expression), source);
    }

    private Expression ApplyNullPropagationForFilterBody(Expression body)
    {
      if (ExpressionBinderBase.IsNullable(body.Type))
        body = this.QuerySettings.HandleNullPropagation != HandleNullPropagationOption.True ? (Expression) Expression.Convert(body, typeof (bool)) : (Expression) Expression.Equal(body, (Expression) Expression.Constant((object) true, typeof (bool?)), false, (MethodInfo) null);
      return body;
    }

    public virtual Expression BindRangeVariable(RangeVariable rangeVariable) => this.ConvertNonStandardPrimitives((Expression) this._lambdaParameters[rangeVariable.Name]);

    public virtual Expression BindCollectionPropertyAccessNode(
      CollectionPropertyAccessNode propertyAccessNode)
    {
      return this.CreatePropertyAccessExpression(this.Bind((QueryNode) propertyAccessNode.Source), propertyAccessNode.Property);
    }

    public virtual Expression BindCollectionComplexNode(CollectionComplexNode collectionComplexNode) => this.CreatePropertyAccessExpression(this.Bind((QueryNode) collectionComplexNode.Source), collectionComplexNode.Property);

    public virtual Expression BindPropertyAccessQueryNode(
      SingleValuePropertyAccessNode propertyAccessNode)
    {
      return this.CreatePropertyAccessExpression(this.Bind((QueryNode) propertyAccessNode.Source), propertyAccessNode.Property, this.GetFullPropertyPath((SingleValueNode) propertyAccessNode));
    }

    public virtual Expression BindSingleComplexNode(SingleComplexNode singleComplexNode) => this.CreatePropertyAccessExpression(this.Bind((QueryNode) singleComplexNode.Source), singleComplexNode.Property, this.GetFullPropertyPath((SingleValueNode) singleComplexNode));

    public virtual Expression BindUnaryOperatorNode(UnaryOperatorNode unaryOperatorNode)
    {
      Expression expression = this.Bind((QueryNode) unaryOperatorNode.Operand);
      switch (unaryOperatorNode.OperatorKind)
      {
        case UnaryOperatorKind.Negate:
          return (Expression) Expression.Negate(expression);
        case UnaryOperatorKind.Not:
          return (Expression) Expression.Not(expression);
        default:
          throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.QueryNodeBindingNotSupported, (object) unaryOperatorNode.Kind, (object) typeof (FilterBinder).Name);
      }
    }

    public virtual Expression BindAllNode(AllNode allNode)
    {
      ParameterExpression parameterExpression = this.HandleLambdaParameters((IEnumerable<RangeVariable>) allNode.RangeVariables);
      Expression expression1 = this.Bind((QueryNode) allNode.Source);
      Expression filter = (Expression) Expression.Lambda(this.ApplyNullPropagationForFilterBody(this.Bind((QueryNode) allNode.Body)), parameterExpression);
      Expression expression2 = FilterBinder.All(expression1, filter);
      this.ExitLamdbaScope();
      if (this.QuerySettings.HandleNullPropagation != HandleNullPropagationOption.True || !ExpressionBinderBase.IsNullable(expression1.Type))
        return expression2;
      Expression nullable = ExpressionBinderBase.ToNullable(expression2);
      return (Expression) Expression.Condition((Expression) Expression.Equal(expression1, ExpressionBinderBase.NullConstant), (Expression) Expression.Constant((object) null, nullable.Type), nullable);
    }

    public virtual Expression BindAnyNode(AnyNode anyNode)
    {
      ParameterExpression parameterExpression = this.HandleLambdaParameters((IEnumerable<RangeVariable>) anyNode.RangeVariables);
      Expression expression1 = this.Bind((QueryNode) anyNode.Source);
      Expression filter = (Expression) null;
      if (anyNode.Body != null && anyNode.Body.Kind != QueryNodeKind.Constant)
        filter = (Expression) Expression.Lambda(this.ApplyNullPropagationForFilterBody(this.Bind((QueryNode) anyNode.Body)), parameterExpression);
      else if (anyNode.Body != null && anyNode.Body.Kind == QueryNodeKind.Constant && !(bool) (anyNode.Body as ConstantNode).Value)
      {
        this.ExitLamdbaScope();
        return ExpressionBinderBase.FalseConstant;
      }
      Expression expression2 = FilterBinder.Any(expression1, filter);
      this.ExitLamdbaScope();
      if (this.QuerySettings.HandleNullPropagation != HandleNullPropagationOption.True || !ExpressionBinderBase.IsNullable(expression1.Type))
        return expression2;
      Expression nullable = ExpressionBinderBase.ToNullable(expression2);
      return (Expression) Expression.Condition((Expression) Expression.Equal(expression1, ExpressionBinderBase.NullConstant), (Expression) Expression.Constant((object) null, nullable.Type), nullable);
    }

    protected override ParameterExpression ItParameter => this._lambdaParameters["$it"];

    private Expression BindSingleValueNode(SingleValueNode node)
    {
      switch (node.Kind)
      {
        case QueryNodeKind.Constant:
          return this.BindConstantNode(node as ConstantNode);
        case QueryNodeKind.Convert:
          return this.BindConvertNode(node as ConvertNode);
        case QueryNodeKind.NonResourceRangeVariableReference:
          return this.BindRangeVariable((RangeVariable) (node as NonResourceRangeVariableReferenceNode).RangeVariable);
        case QueryNodeKind.BinaryOperator:
          return this.BindBinaryOperatorNode(node as BinaryOperatorNode);
        case QueryNodeKind.UnaryOperator:
          return this.BindUnaryOperatorNode(node as UnaryOperatorNode);
        case QueryNodeKind.SingleValuePropertyAccess:
          return this.BindPropertyAccessQueryNode(node as SingleValuePropertyAccessNode);
        case QueryNodeKind.SingleValueFunctionCall:
          return this.BindSingleValueFunctionCallNode(node as SingleValueFunctionCallNode);
        case QueryNodeKind.Any:
          return this.BindAnyNode(node as AnyNode);
        case QueryNodeKind.SingleNavigationNode:
          SingleNavigationNode node1 = node as SingleNavigationNode;
          return this.BindNavigationPropertyNode((QueryNode) node1.Source, node1.NavigationProperty, this.GetFullPropertyPath((SingleValueNode) node1));
        case QueryNodeKind.SingleValueOpenPropertyAccess:
          return this.BindDynamicPropertyAccessQueryNode(node as SingleValueOpenPropertyAccessNode);
        case QueryNodeKind.SingleResourceCast:
          return this.BindSingleResourceCastNode(node as SingleResourceCastNode);
        case QueryNodeKind.All:
          return this.BindAllNode(node as AllNode);
        case QueryNodeKind.ResourceRangeVariableReference:
          return this.BindRangeVariable((RangeVariable) (node as ResourceRangeVariableReferenceNode).RangeVariable);
        case QueryNodeKind.SingleResourceFunctionCall:
          return this.BindSingleResourceFunctionCallNode(node as SingleResourceFunctionCallNode);
        case QueryNodeKind.SingleComplexNode:
          return this.BindSingleComplexNode(node as SingleComplexNode);
        case QueryNodeKind.Count:
          return this.BindCountNode(node as CountNode);
        case QueryNodeKind.In:
          return this.BindInNode(node as InNode);
        default:
          throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.QueryNodeBindingNotSupported, (object) node.Kind, (object) typeof (FilterBinder).Name);
      }
    }

    private Expression BindCollectionNode(CollectionNode node)
    {
      QueryNodeKind kind = node.Kind;
      switch (kind)
      {
        case QueryNodeKind.CollectionPropertyAccess:
          return this.BindCollectionPropertyAccessNode(node as CollectionPropertyAccessNode);
        case QueryNodeKind.CollectionNavigationNode:
          CollectionNavigationNode collectionNavigationNode = node as CollectionNavigationNode;
          return this.BindNavigationPropertyNode((QueryNode) collectionNavigationNode.Source, collectionNavigationNode.NavigationProperty);
        case QueryNodeKind.CollectionResourceCast:
          return this.BindCollectionResourceCastNode(node as CollectionResourceCastNode);
        case QueryNodeKind.ResourceRangeVariableReference:
        case QueryNodeKind.SingleResourceFunctionCall:
        case QueryNodeKind.CollectionFunctionCall:
        case QueryNodeKind.CollectionResourceFunctionCall:
        case QueryNodeKind.NamedFunctionParameter:
        case QueryNodeKind.ParameterAlias:
        case QueryNodeKind.EntitySet:
        case QueryNodeKind.KeyLookup:
        case QueryNodeKind.SearchTerm:
        case QueryNodeKind.CollectionOpenPropertyAccess:
          if ((uint) (kind - 18) <= 1U || kind == QueryNodeKind.CollectionOpenPropertyAccess)
            break;
          break;
        case QueryNodeKind.CollectionComplexNode:
          return this.BindCollectionComplexNode(node as CollectionComplexNode);
        case QueryNodeKind.CollectionConstant:
          return this.BindCollectionConstantNode(node as CollectionConstantNode);
      }
      throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.QueryNodeBindingNotSupported, (object) node.Kind, (object) typeof (FilterBinder).Name);
    }

    private Type RetrieveClrTypeForConstant(IEdmTypeReference edmTypeReference, ref object value)
    {
      Type type1 = EdmLibHelpers.GetClrType(edmTypeReference, this.Model, this.InternalAssembliesResolver);
      if (value != null && edmTypeReference != null && edmTypeReference.IsEnum())
      {
        string str = ((ODataEnumValue) value).Value;
        Type type2 = Nullable.GetUnderlyingType(type1);
        if ((object) type2 == null)
          type2 = type1;
        type1 = type2;
        value = Enum.Parse(type1, str);
      }
      if (edmTypeReference != null && edmTypeReference.IsNullable && (edmTypeReference.IsDate() || edmTypeReference.IsTimeOfDay()))
      {
        Type type3 = Nullable.GetUnderlyingType(type1);
        if ((object) type3 == null)
          type3 = type1;
        type1 = type3;
      }
      return type1;
    }

    private ParameterExpression HandleLambdaParameters(IEnumerable<RangeVariable> rangeVariables)
    {
      ParameterExpression parameterExpression1 = (ParameterExpression) null;
      this.EnterLambdaScope();
      Dictionary<string, ParameterExpression> dictionary = new Dictionary<string, ParameterExpression>();
      foreach (RangeVariable rangeVariable in rangeVariables)
      {
        ParameterExpression parameterExpression2;
        if (!this._lambdaParameters.TryGetValue(rangeVariable.Name, out parameterExpression2))
        {
          IEdmTypeReference edmTypeReference = rangeVariable.TypeReference;
          if (edmTypeReference is IEdmCollectionTypeReference collectionTypeReference && collectionTypeReference.Definition is IEdmCollectionType definition)
            edmTypeReference = definition.ElementType;
          parameterExpression2 = Expression.Parameter(EdmLibHelpers.GetClrType(edmTypeReference, this.Model, this.InternalAssembliesResolver), rangeVariable.Name);
          parameterExpression1 = parameterExpression2;
        }
        dictionary.Add(rangeVariable.Name, parameterExpression2);
      }
      this._lambdaParameters = dictionary;
      return parameterExpression1;
    }

    private void EnterLambdaScope() => this._parametersStack.Push(this._lambdaParameters);

    private void ExitLamdbaScope()
    {
      if (this._parametersStack.Count != 0)
        this._lambdaParameters = this._parametersStack.Pop();
      else
        this._lambdaParameters = (Dictionary<string, ParameterExpression>) null;
    }

    private static Expression Any(Expression source, Expression filter)
    {
      Type elementType;
      TypeHelper.IsCollection(source.Type, out elementType);
      return filter == null ? (ExpressionBinderBase.IsIQueryable(source.Type) ? (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.QueryableEmptyAnyGeneric.MakeGenericMethod(elementType), source) : (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.EnumerableEmptyAnyGeneric.MakeGenericMethod(elementType), source)) : (ExpressionBinderBase.IsIQueryable(source.Type) ? (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.QueryableNonEmptyAnyGeneric.MakeGenericMethod(elementType), source, filter) : (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.EnumerableNonEmptyAnyGeneric.MakeGenericMethod(elementType), source, filter));
    }

    private static Expression All(Expression source, Expression filter)
    {
      Type elementType;
      TypeHelper.IsCollection(source.Type, out elementType);
      return ExpressionBinderBase.IsIQueryable(source.Type) ? (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.QueryableAllGeneric.MakeGenericMethod(elementType), source, filter) : (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.EnumerableAllGeneric.MakeGenericMethod(elementType), source, filter);
    }
  }
}
