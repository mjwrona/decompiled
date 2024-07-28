// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.AggregationBinder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal class AggregationBinder : TransformationBinderBase
  {
    private const string GroupByContainerProperty = "GroupByContainer";
    private TransformationNode _transformation;
    private SelectExpandClause _selectExpandClause;
    private ODataQueryContext _context;
    private IEnumerable<AggregateExpressionBase> _aggregateExpressions;
    private IEnumerable<GroupByPropertyNode> _groupingProperties;
    private Type _groupByClrType;
    private Dictionary<SingleValueNode, Expression> _preFlattenedMap = new Dictionary<SingleValueNode, Expression>();
    private Dictionary<string, FilterClause> _filtersPushDown;

    internal AggregationBinder(
      ODataQuerySettings settings,
      IWebApiAssembliesResolver assembliesResolver,
      Type elementType,
      IEdmModel model,
      TransformationNode transformation,
      ODataQueryContext context,
      SelectExpandClause selectExpandClause = null)
      : base(settings, assembliesResolver, elementType, model)
    {
      this._transformation = transformation;
      this._selectExpandClause = selectExpandClause;
      this._context = context;
      switch (transformation.Kind)
      {
        case TransformationNodeKind.Aggregate:
          this._aggregateExpressions = this.FixCustomMethodReturnTypes((this._transformation as AggregateTransformationNode).AggregateExpressions);
          this.ResultClrType = typeof (NoGroupByAggregationWrapper);
          break;
        case TransformationNodeKind.GroupBy:
          GroupByTransformationNode transformation1 = this._transformation as GroupByTransformationNode;
          this._groupingProperties = transformation1.GroupingProperties;
          if (transformation1.ChildTransformations != null)
          {
            if (transformation1.ChildTransformations.Kind != TransformationNodeKind.Aggregate)
              throw new NotImplementedException();
            this._aggregateExpressions = this.FixCustomMethodReturnTypes(((AggregateTransformationNode) transformation1.ChildTransformations).AggregateExpressions);
          }
          this._groupByClrType = typeof (GroupByWrapper);
          this.ResultClrType = typeof (AggregationWrapper);
          break;
        default:
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SRResources.NotSupportedTransformationKind, new object[1]
          {
            (object) transformation.Kind
          }));
      }
      Type type = this._groupByClrType;
      if ((object) type == null)
        type = typeof (NoGroupByWrapper);
      this._groupByClrType = type;
    }

    private static Expression WrapDynamicCastIfNeeded(Expression propertyAccessor)
    {
      if (!(propertyAccessor.Type == typeof (object)))
        return propertyAccessor;
      return (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.ConvertToDecimal, propertyAccessor);
    }

    private IEnumerable<AggregateExpressionBase> FixCustomMethodReturnTypes(
      IEnumerable<AggregateExpressionBase> aggregateExpressions)
    {
      return aggregateExpressions.Select<AggregateExpressionBase, AggregateExpressionBase>((Func<AggregateExpressionBase, AggregateExpressionBase>) (x => !(x is AggregateExpression expression) ? x : (AggregateExpressionBase) this.FixCustomMethodReturnType(expression)));
    }

    private AggregateExpression FixCustomMethodReturnType(AggregateExpression expression)
    {
      if (expression.Method != AggregationMethod.Custom)
        return expression;
      IEdmPrimitiveTypeReference typeReferenceOrNull = EdmLibHelpers.GetEdmPrimitiveTypeReferenceOrNull(this.GetCustomMethod(expression).ReturnType);
      return new AggregateExpression(expression.Expression, expression.MethodDefinition, expression.Alias, (IEdmTypeReference) typeReferenceOrNull);
    }

    private MethodInfo GetCustomMethod(AggregateExpression expression)
    {
      Type type = Expression.Lambda(this.BindAccessor((QueryNode) expression.Expression), this._lambdaParameter).Body.Type;
      string methodLabel = expression.MethodDefinition.MethodLabel;
      MethodInfo methodInfo;
      if (!this.Model.GetAnnotationValue<CustomAggregateMethodAnnotation>((IEdmElement) this.Model).GetMethodInfo(methodLabel, type, out methodInfo))
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.AggregationNotSupportedForType, (object) expression.Method, (object) expression.Expression, (object) type));
      return methodInfo;
    }

    public IQueryable Bind(IQueryable query)
    {
      this._classicEF = this.IsClassicEF(query);
      this.PreprocessQuery(query);
      query = this.FlattenReferencedProperties(query);
      return this.BindSelect(this.BindGroupBy(query));
    }

    private IQueryable FlattenReferencedProperties(IQueryable query)
    {
      if (this._aggregateExpressions != null && this._aggregateExpressions.OfType<AggregateExpression>().Any<AggregateExpression>((Func<AggregateExpression, bool>) (e => e.Method != AggregationMethod.VirtualPropertyCount)) && this._groupingProperties != null && this._groupingProperties.Any<GroupByPropertyNode>() && (this.FlattenedPropertyContainer == null || !this.FlattenedPropertyContainer.Any<KeyValuePair<string, Expression>>()))
      {
        Type type1 = typeof (FlatteningWrapper<>).MakeGenericType(this._elementType);
        PropertyInfo property1 = type1.GetProperty("Source");
        List<MemberAssignment> bindings = new List<MemberAssignment>();
        bindings.Add(Expression.Bind((MemberInfo) property1, (Expression) this._lambdaParameter));
        List<AggregateExpression> list = this._aggregateExpressions.OfType<AggregateExpression>().Where<AggregateExpression>((Func<AggregateExpression, bool>) (e => e.Method != AggregationMethod.VirtualPropertyCount)).ToList<AggregateExpression>();
        NamedPropertyExpression[] properties = new NamedPropertyExpression[list.Count];
        int index = list.Count - 1;
        ParameterExpression parameterExpression = Expression.Parameter(type1, "$it");
        MemberExpression memberExpression = Expression.Property((Expression) parameterExpression, "GroupByContainer");
        foreach (AggregateExpression aggregateExpression in list)
        {
          string str = "Property" + index.ToString((IFormatProvider) CultureInfo.CurrentCulture);
          Expression expression1 = this.BindAccessor((QueryNode) aggregateExpression.Expression);
          Type type2 = expression1.Type;
          Expression expression2 = this.WrapConvert(expression1);
          properties[index] = new NamedPropertyExpression((Expression) Expression.Constant((object) str), expression2);
          UnaryExpression unaryExpression = Expression.Convert((Expression) Expression.Property((Expression) memberExpression, "Value"), type2);
          memberExpression = Expression.Property((Expression) memberExpression, "Next");
          this._preFlattenedMap.Add(aggregateExpression.Expression, (Expression) unaryExpression);
          --index;
        }
        PropertyInfo property2 = this.ResultClrType.GetProperty("GroupByContainer");
        bindings.Add(Expression.Bind((MemberInfo) property2, AggregationPropertyContainer.CreateNextNamedPropertyContainer((IList<NamedPropertyExpression>) properties)));
        LambdaExpression expression = Expression.Lambda((Expression) Expression.MemberInit(Expression.New(type1), (IEnumerable<MemberBinding>) bindings), this._lambdaParameter);
        query = ExpressionHelpers.Select(query, expression, this._elementType);
        this._lambdaParameter = parameterExpression;
        this._elementType = type1;
      }
      return query;
    }

    private IQueryable BindSelect(IQueryable grouping)
    {
      Type type = typeof (IGrouping<,>).MakeGenericType(this._groupByClrType, this._elementType);
      ParameterExpression accum = Expression.Parameter(type, "$it");
      List<MemberAssignment> bindings = new List<MemberAssignment>();
      if (this._groupingProperties != null && this._groupingProperties.Any<GroupByPropertyNode>())
      {
        PropertyInfo property = this.ResultClrType.GetProperty("GroupByContainer");
        bindings.Add(Expression.Bind((MemberInfo) property, (Expression) Expression.Property((Expression) Expression.Property((Expression) accum, "Key"), "GroupByContainer")));
      }
      if (this._aggregateExpressions != null)
      {
        List<NamedPropertyExpression> properties = new List<NamedPropertyExpression>();
        foreach (AggregateExpressionBase aggregateExpression in this._aggregateExpressions)
          properties.Add(new NamedPropertyExpression((Expression) Expression.Constant((object) aggregateExpression.Alias), this.CreateAggregationExpression(accum, aggregateExpression, this._elementType)));
        PropertyInfo property = this.ResultClrType.GetProperty("Container");
        bindings.Add(Expression.Bind((MemberInfo) property, AggregationPropertyContainer.CreateNextNamedPropertyContainer((IList<NamedPropertyExpression>) properties)));
      }
      LambdaExpression expression = Expression.Lambda((Expression) Expression.MemberInit(Expression.New(this.ResultClrType), (IEnumerable<MemberBinding>) bindings), accum);
      return ExpressionHelpers.Select(grouping, expression, type);
    }

    private List<MemberAssignment> CreateSelectMemberAssigments(
      Type type,
      MemberExpression propertyAccessor,
      IEnumerable<GroupByPropertyNode> properties)
    {
      List<MemberAssignment> memberAssigments = new List<MemberAssignment>();
      if (this._groupingProperties != null)
      {
        foreach (GroupByPropertyNode property in properties)
        {
          MemberExpression propertyAccessor1 = Expression.Property((Expression) propertyAccessor, property.Name);
          MemberInfo member = ((IEnumerable<MemberInfo>) type.GetMember(property.Name)).Single<MemberInfo>();
          if (property.Expression != null)
          {
            memberAssigments.Add(Expression.Bind(member, (Expression) propertyAccessor1));
          }
          else
          {
            Type propertyType = (member as PropertyInfo).PropertyType;
            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(propertyType), (IEnumerable<MemberBinding>) this.CreateSelectMemberAssigments(propertyType, propertyAccessor1, (IEnumerable<GroupByPropertyNode>) property.ChildTransformations));
            memberAssigments.Add(Expression.Bind(member, (Expression) memberInitExpression));
          }
        }
      }
      return memberAssigments;
    }

    private Expression CreateAggregationExpression(
      ParameterExpression accum,
      AggregateExpressionBase expression,
      Type baseType)
    {
      switch (expression.AggregateKind)
      {
        case AggregateExpressionKind.PropertyAggregate:
          return this.CreatePropertyAggregateExpression(accum, expression as AggregateExpression, baseType);
        case AggregateExpressionKind.EntitySetAggregate:
          return this.CreateEntitySetAggregateExpression(accum, expression as EntitySetAggregateExpression, baseType);
        default:
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.AggregateKindNotSupported, (object) expression.AggregateKind));
      }
    }

    private Expression CreateEntitySetAggregateExpression(
      ParameterExpression accum,
      EntitySetAggregateExpression expression,
      Type baseType)
    {
      List<MemberAssignment> bindings = new List<MemberAssignment>();
      Expression expression1 = (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.QueryableAsQueryable.MakeGenericMethod(baseType), (Expression) accum);
      Expression expression2 = this.BindAccessor((QueryNode) expression.Expression.Source);
      MemberExpression memberExpression = Expression.Property(expression2, EdmLibHelpers.GetClrPropertyName((IEdmProperty) expression.Expression.NavigationProperty, this.Model));
      Type type1 = expression2.Type;
      Type type2 = ((IEnumerable<Type>) memberExpression.Type.GenericTypeArguments).Single<Type>();
      MethodInfo method1 = ExpressionHelperMethods.EnumerableSelectManyGeneric.MakeGenericMethod(type1, type2);
      ParameterExpression parameterExpression = Expression.Parameter(type1, "$it");
      LambdaExpression lambdaExpression1 = Expression.Lambda((Expression) Expression.Property((Expression) parameterExpression, expression.Expression.NavigationProperty.Name), parameterExpression);
      MethodCallExpression methodCallExpression1 = Expression.Call((Expression) null, method1, expression1, (Expression) lambdaExpression1);
      this.EnsurePushedDownFilters();
      FilterClause filterClause;
      if (this._filtersPushDown.TryGetValue(expression.Expression.NavigationSource.Path.Path, out filterClause))
      {
        Expression expression3 = FilterBinder.Bind((IQueryable) null, filterClause, type2, this._context, this.QuerySettings);
        methodCallExpression1 = Expression.Call((Expression) null, ExpressionHelperMethods.QueryableWhereGeneric.MakeGenericMethod(type2), (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.QueryableAsQueryable.MakeGenericMethod(type2), (Expression) methodCallExpression1), expression3);
      }
      Type type3 = typeof (bool);
      MethodInfo method2 = ExpressionHelperMethods.EnumerableGroupByGeneric.MakeGenericMethod(type2, type3);
      LambdaExpression lambdaExpression2 = Expression.Lambda((Expression) Expression.New(type3), Expression.Parameter(type2, "$gr"));
      MethodCallExpression methodCallExpression2 = Expression.Call((Expression) null, method2, (Expression) methodCallExpression1, (Expression) lambdaExpression2);
      Type type4 = typeof (IGrouping<,>).MakeGenericType(type3, type2);
      ParameterExpression accum1 = Expression.Parameter(type4, "$p");
      List<NamedPropertyExpression> properties = new List<NamedPropertyExpression>();
      foreach (AggregateExpressionBase child in expression.Children)
        properties.Add(new NamedPropertyExpression((Expression) Expression.Constant((object) child.Alias), this.CreateAggregationExpression(accum1, child, type2)));
      Type type5 = typeof (EntitySetAggregationWrapper);
      PropertyInfo property = type5.GetProperty("Container");
      bindings.Add(Expression.Bind((MemberInfo) property, AggregationPropertyContainer.CreateNextNamedPropertyContainer((IList<NamedPropertyExpression>) properties)));
      LambdaExpression lambdaExpression3 = Expression.Lambda((Expression) Expression.MemberInit(Expression.New(type5), (IEnumerable<MemberBinding>) bindings), accum1);
      return (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.EnumerableSelectGeneric.MakeGenericMethod(type4, lambdaExpression3.Body.Type), (Expression) methodCallExpression2, (Expression) lambdaExpression3);
    }

    private void EnsurePushedDownFilters()
    {
      if (this._filtersPushDown == null)
        this._filtersPushDown = new Dictionary<string, FilterClause>();
      if (this._selectExpandClause == null)
        return;
      foreach (ExpandedNavigationSelectItem navigationSelectItem in this._selectExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>().Where<ExpandedNavigationSelectItem>((Func<ExpandedNavigationSelectItem, bool>) (e => e.FilterOption != null)))
        this._filtersPushDown[navigationSelectItem.NavigationSource.Path.Path] = navigationSelectItem.FilterOption;
    }

    private Expression CreatePropertyAggregateExpression(
      ParameterExpression accum,
      AggregateExpression expression,
      Type baseType)
    {
      Expression expression1;
      if (this._classicEF)
      {
        expression1 = (Expression) Expression.Call((Expression) null, ExpressionHelperMethods.QueryableAsQueryable.MakeGenericMethod(baseType), (Expression) accum);
      }
      else
      {
        Type type = typeof (IEnumerable<>).MakeGenericType(baseType);
        expression1 = (Expression) Expression.Convert((Expression) accum, type);
      }
      if (expression.Method == AggregationMethod.VirtualPropertyCount)
        return this.WrapConvert((Expression) Expression.Call((Expression) null, (this._classicEF ? ExpressionHelperMethods.QueryableCountGeneric : ExpressionHelperMethods.EnumerableCountGeneric).MakeGenericMethod(baseType), expression1));
      ParameterExpression baseElement = baseType == this._elementType ? this._lambdaParameter : Expression.Parameter(baseType, "$it");
      Expression expression2;
      if (!this._preFlattenedMap.TryGetValue(expression.Expression, out expression2))
        expression2 = this.BindAccessor((QueryNode) expression.Expression, (Expression) baseElement);
      LambdaExpression lambdaExpression1 = Expression.Lambda(expression2, baseElement);
      Expression expression3;
      switch (expression.Method)
      {
        case AggregationMethod.Sum:
          Expression body1 = AggregationBinder.WrapDynamicCastIfNeeded(expression2);
          LambdaExpression lambdaExpression2 = Expression.Lambda(body1, baseElement);
          MethodInfo methodInfo1;
          if (!(this._classicEF ? ExpressionHelperMethods.QueryableSumGenerics : ExpressionHelperMethods.EnumerableSumGenerics).TryGetValue(body1.Type, out methodInfo1))
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.AggregationNotSupportedForType, (object) expression.Method, (object) expression.Expression, (object) body1.Type));
          expression3 = (Expression) Expression.Call((Expression) null, methodInfo1.MakeGenericMethod(baseType), expression1, (Expression) lambdaExpression2);
          if (lambdaExpression2.Type == typeof (object))
          {
            expression3 = (Expression) Expression.Convert(expression3, typeof (object));
            break;
          }
          break;
        case AggregationMethod.Min:
          expression3 = (Expression) Expression.Call((Expression) null, (this._classicEF ? ExpressionHelperMethods.QueryableMin : ExpressionHelperMethods.EnumerableMin).MakeGenericMethod(baseType, lambdaExpression1.Body.Type), expression1, (Expression) lambdaExpression1);
          break;
        case AggregationMethod.Max:
          expression3 = (Expression) Expression.Call((Expression) null, (this._classicEF ? ExpressionHelperMethods.QueryableMax : ExpressionHelperMethods.EnumerableMax).MakeGenericMethod(baseType, lambdaExpression1.Body.Type), expression1, (Expression) lambdaExpression1);
          break;
        case AggregationMethod.Average:
          Expression body2 = AggregationBinder.WrapDynamicCastIfNeeded(expression2);
          LambdaExpression lambdaExpression3 = Expression.Lambda(body2, baseElement);
          MethodInfo methodInfo2;
          if (!(this._classicEF ? ExpressionHelperMethods.QueryableAverageGenerics : ExpressionHelperMethods.EnumerableAverageGenerics).TryGetValue(body2.Type, out methodInfo2))
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.AggregationNotSupportedForType, (object) expression.Method, (object) expression.Expression, (object) body2.Type));
          expression3 = (Expression) Expression.Call((Expression) null, methodInfo2.MakeGenericMethod(baseType), expression1, (Expression) lambdaExpression3);
          if (lambdaExpression3.Type == typeof (object))
          {
            expression3 = (Expression) Expression.Convert(expression3, typeof (object));
            break;
          }
          break;
        case AggregationMethod.CountDistinct:
          Expression expression4 = (Expression) Expression.Call((Expression) null, (this._classicEF ? ExpressionHelperMethods.QueryableSelectGeneric : ExpressionHelperMethods.EnumerableSelectGeneric).MakeGenericMethod(this._elementType, lambdaExpression1.Body.Type), expression1, (Expression) lambdaExpression1);
          Expression expression5 = (Expression) Expression.Call((Expression) null, (this._classicEF ? ExpressionHelperMethods.QueryableDistinct : ExpressionHelperMethods.EnumerableDistinct).MakeGenericMethod(lambdaExpression1.Body.Type), expression4);
          expression3 = (Expression) Expression.Call((Expression) null, (this._classicEF ? ExpressionHelperMethods.QueryableCountGeneric : ExpressionHelperMethods.EnumerableCountGeneric).MakeGenericMethod(lambdaExpression1.Body.Type), expression5);
          break;
        case AggregationMethod.Custom:
          expression3 = (Expression) Expression.Call((Expression) null, this.GetCustomMethod(expression), (Expression) Expression.Call((Expression) null, (this._classicEF ? ExpressionHelperMethods.QueryableSelectGeneric : ExpressionHelperMethods.EnumerableSelectGeneric).MakeGenericMethod(this._elementType, lambdaExpression1.Body.Type), expression1, (Expression) lambdaExpression1));
          break;
        default:
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.AggregationMethodNotSupported, (object) expression.Method));
      }
      return this.WrapConvert(expression3);
    }

    private IQueryable BindGroupBy(IQueryable query)
    {
      Type elementType = query.ElementType;
      LambdaExpression lambdaExpression;
      if (this._groupingProperties != null && this._groupingProperties.Any<GroupByPropertyNode>())
      {
        List<NamedPropertyExpression> memberAssignments = this.CreateGroupByMemberAssignments(this._groupingProperties);
        PropertyInfo property = typeof (GroupByWrapper).GetProperty("GroupByContainer");
        lambdaExpression = Expression.Lambda((Expression) Expression.MemberInit(Expression.New(typeof (GroupByWrapper)), (IEnumerable<MemberBinding>) new List<MemberAssignment>()
        {
          Expression.Bind((MemberInfo) property, AggregationPropertyContainer.CreateNextNamedPropertyContainer((IList<NamedPropertyExpression>) memberAssignments))
        }), this._lambdaParameter);
      }
      else
        lambdaExpression = Expression.Lambda((Expression) Expression.New(this._groupByClrType), this._lambdaParameter);
      return ExpressionHelpers.GroupBy(query, (Expression) lambdaExpression, elementType, this._groupByClrType);
    }

    private List<NamedPropertyExpression> CreateGroupByMemberAssignments(
      IEnumerable<GroupByPropertyNode> nodes)
    {
      List<NamedPropertyExpression> memberAssignments = new List<NamedPropertyExpression>();
      foreach (GroupByPropertyNode node in nodes)
      {
        string name = node.Name;
        if (node.Expression != null)
        {
          memberAssignments.Add(new NamedPropertyExpression((Expression) Expression.Constant((object) name), this.WrapConvert(this.BindAccessor((QueryNode) node.Expression))));
        }
        else
        {
          PropertyInfo property = typeof (GroupByWrapper).GetProperty("GroupByContainer");
          memberAssignments.Add(new NamedPropertyExpression((Expression) Expression.Constant((object) name), (Expression) Expression.MemberInit(Expression.New(typeof (GroupByWrapper)), (IEnumerable<MemberBinding>) new List<MemberAssignment>()
          {
            Expression.Bind((MemberInfo) property, AggregationPropertyContainer.CreateNextNamedPropertyContainer((IList<NamedPropertyExpression>) this.CreateGroupByMemberAssignments((IEnumerable<GroupByPropertyNode>) node.ChildTransformations)))
          })));
        }
      }
      return memberAssignments;
    }
  }
}
