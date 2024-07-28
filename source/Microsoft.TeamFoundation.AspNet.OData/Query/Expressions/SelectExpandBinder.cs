// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.SelectExpandBinder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal class SelectExpandBinder
  {
    private ODataQueryContext _context;
    private IEdmModel _model;
    private ODataQuerySettings _settings;
    internal bool HasInstancePropertyContainer;
    internal bool InputCollapsed;
    internal IQueryable BaseQuery;
    internal IDictionary<string, Expression> FlattenedPropertyContainer;

    public SelectExpandBinder(ODataQuerySettings settings, ODataQueryContext context)
    {
      this._context = context;
      this._model = this._context.Model;
      this._settings = settings;
    }

    internal IQueryProvider QueryProvider { get; set; }

    public static IQueryable Bind(
      IQueryable queryable,
      ODataQuerySettings settings,
      SelectExpandQueryOption selectExpandQuery)
    {
      return new SelectExpandBinder(settings, selectExpandQuery.Context).Bind(queryable, selectExpandQuery);
    }

    public static object Bind(
      object entity,
      ODataQuerySettings settings,
      SelectExpandQueryOption selectExpandQuery)
    {
      return new SelectExpandBinder(settings, selectExpandQuery.Context).Bind(entity, selectExpandQuery);
    }

    private object Bind(object entity, SelectExpandQueryOption selectExpandQuery) => this.GetProjectionLambda(Expression.Parameter(selectExpandQuery.Context.ElementClrType), selectExpandQuery).Compile().DynamicInvoke(entity);

    private IQueryable Bind(IQueryable queryable, SelectExpandQueryOption selectExpandQuery)
    {
      this.QueryProvider = queryable.Provider;
      this.BaseQuery = queryable;
      Type elementType = this.BaseQuery.ElementType;
      ParameterExpression source = Expression.Parameter(elementType);
      this.EnsureFlattenedPropertyContainer(source);
      LambdaExpression projectionLambda = this.GetProjectionLambda(source, selectExpandQuery);
      return ExpressionHelperMethods.QueryableSelectGeneric.MakeGenericMethod(elementType, projectionLambda.Body.Type).Invoke((object) null, new object[2]
      {
        (object) queryable,
        (object) projectionLambda
      }) as IQueryable;
    }

    protected void EnsureFlattenedPropertyContainer(ParameterExpression source)
    {
      if (this.BaseQuery == null)
        return;
      this.HasInstancePropertyContainer = this.BaseQuery.ElementType.IsGenericType && this.BaseQuery.ElementType.GetGenericTypeDefinition() == typeof (ComputeWrapper<>);
      this.InputCollapsed = ExpressionHelpers.HasGroupBy(this.BaseQuery.Expression);
      this.FlattenedPropertyContainer = this.FlattenedPropertyContainer ?? ExpressionBinderBase.GetFlattenedProperties(this.BaseQuery, this.HasInstancePropertyContainer, source);
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

    private LambdaExpression GetProjectionLambda(
      ParameterExpression source,
      SelectExpandQueryOption selectExpandQuery)
    {
      IEdmNavigationSource navigationSource = selectExpandQuery.Context.NavigationSource;
      return Expression.Lambda(this.ProjectElement((Expression) source, selectExpandQuery.SelectExpandClause, this._context.ElementType as IEdmStructuredType, navigationSource), source);
    }

    internal Expression ProjectAsWrapper(
      Expression source,
      SelectExpandClause selectExpandClause,
      IEdmStructuredType structuredType,
      IEdmNavigationSource navigationSource,
      OrderByClause orderByClause = null,
      long? topOption = null,
      long? skipOption = null,
      int? modelBoundPageSize = null)
    {
      Type elementType;
      if (!TypeHelper.IsCollection(source.Type, out elementType))
        return this.ProjectElement(source, selectExpandClause, structuredType, navigationSource);
      return EdmLibHelpers.IsDynamicTypeWrapper(elementType) ? source : this.ProjectCollection(source, elementType, selectExpandClause, structuredType, navigationSource, orderByClause, topOption, skipOption, modelBoundPageSize);
    }

    internal Expression CreatePropertyNameExpression(
      IEdmStructuredType elementType,
      IEdmProperty property,
      Expression source)
    {
      IEdmStructuredType declaringType = property.DeclaringType;
      if (elementType != declaringType)
      {
        Type clrType1 = EdmLibHelpers.GetClrType((IEdmType) elementType, this._model);
        Type clrType2 = EdmLibHelpers.GetClrType((IEdmType) declaringType, this._model);
        if (clrType2 == (Type) null)
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.MappingDoesNotContainResourceType, (object) declaringType.FullTypeName()));
        if (!clrType2.IsAssignableFrom(clrType1))
          return (Expression) Expression.Condition((Expression) Expression.TypeIs(source, clrType2), (Expression) Expression.Constant((object) property.Name), (Expression) Expression.Constant((object) null, typeof (string)));
      }
      return (Expression) Expression.Constant((object) property.Name);
    }

    internal Expression CreatePropertyValueExpression(
      IEdmEntityType elementType,
      IEdmProperty property,
      Expression source)
    {
      return this.CreatePropertyValueExpressionWithClauses((IEdmStructuredType) elementType, property, source, (FilterClause) null, (ApplyClause) null, (ComputeClause) null);
    }

    internal Expression CreatePropertyAccessExpression(
      Expression source,
      IEdmProperty property,
      string propertyPath = null)
    {
      string clrPropertyName = EdmLibHelpers.GetClrPropertyName(property, this._model);
      propertyPath = propertyPath ?? clrPropertyName;
      return this.GetFlattenedPropertyExpression(propertyPath) ?? ExpressionBinderBase.GetPropertyExpression(source, clrPropertyName, propertyPath);
    }

    internal Expression CreatePropertyValueExpressionWithClauses(
      IEdmStructuredType elementType,
      IEdmProperty property,
      Expression source,
      FilterClause filterClause,
      ApplyClause applyClause,
      ComputeClause computeClause)
    {
      if (elementType != property.DeclaringType)
      {
        Type clrType = EdmLibHelpers.GetClrType((IEdmType) property.DeclaringType, this._model);
        source = !(clrType == (Type) null) ? (Expression) Expression.TypeAs(source, clrType) : throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.MappingDoesNotContainResourceType, (object) property.DeclaringType.FullTypeName()));
      }
      string clrPropertyName = EdmLibHelpers.GetClrPropertyName(property, this._model);
      Expression accessExpression = this.CreatePropertyAccessExpression(source, property, clrPropertyName);
      Type type1 = TypeHelper.ToNullable(accessExpression.Type);
      Expression expression1 = ExpressionHelpers.ToNullable(accessExpression);
      if (filterClause != null || applyClause != null || computeClause != null)
      {
        bool flag = property.Type.IsCollection();
        IEdmTypeReference edmTypeReference = flag ? property.Type.AsCollection().ElementType() : property.Type;
        Type type2 = EdmLibHelpers.GetClrType(edmTypeReference, this._model);
        if (type2 == (Type) null)
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.MappingDoesNotContainResourceType, (object) edmTypeReference.FullName()));
        ODataQuerySettings odataQuerySettings = new ODataQuerySettings()
        {
          HandleNullPropagation = HandleNullPropagationOption.True
        };
        if (applyClause != null)
        {
          if (flag)
          {
            Expression expression2;
            if (!typeof (IQueryable).IsAssignableFrom(expression1.Type))
              expression2 = (Expression) Expression.Call(ExpressionHelperMethods.QueryableAsQueryable.MakeGenericMethod(type2), expression1);
            else
              expression2 = expression1;
            IQueryable query = this.QueryProvider.CreateQuery(expression2);
            IQueryable queryable = new ApplyQueryOptionsBinder(this._context, odataQuerySettings, type2).Bind(query, applyClause);
            type2 = queryable.ElementType;
            expression1 = queryable.Expression;
            type1 = expression1.Type;
          }
          else
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.AggregationNotSupportedForSingleProperty, (object) property.Name));
        }
        if (computeClause != null)
        {
          if (flag)
          {
            Expression expression3;
            if (!typeof (IQueryable).IsAssignableFrom(expression1.Type))
              expression3 = (Expression) Expression.Call(ExpressionHelperMethods.QueryableAsQueryable.MakeGenericMethod(type2), expression1);
            else
              expression3 = expression1;
            IQueryable query = this.QueryProvider.CreateQuery(expression3);
            IWebApiAssembliesResolver assembliesResolver = WebApiAssembliesResolver.Default;
            IQueryable queryable = new ComputeBinder(odataQuerySettings, assembliesResolver, type2, this._context.Model, computeClause.ComputedItems).Bind(query);
            type2 = queryable.ElementType;
            type1 = queryable.Expression.Type;
            expression1 = this._settings.HandleNullPropagation != HandleNullPropagationOption.True ? queryable.Expression : (Expression) Expression.Condition((Expression) Expression.Equal(expression1, (Expression) Expression.Constant((object) null)), (Expression) Expression.Constant((object) null, type1), queryable.Expression);
          }
          else
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.AggregationNotSupportedForSingleProperty, (object) property.Name));
        }
        if (filterClause != null)
        {
          Expression ifFalse = expression1;
          if (flag)
          {
            Expression expression4;
            if (!typeof (IEnumerable).IsAssignableFrom(source.Type.GetProperty(clrPropertyName).PropertyType))
              expression4 = expression1;
            else
              expression4 = (Expression) Expression.Call(ExpressionHelperMethods.QueryableAsQueryable.MakeGenericMethod(type2), expression1);
            Expression expression5 = expression4;
            Expression expression6 = FilterBinder.Bind(this.QueryProvider?.CreateQuery(expression5), filterClause, type2, this._context, odataQuerySettings);
            ifFalse = (Expression) Expression.Call(ExpressionHelperMethods.QueryableWhereGeneric.MakeGenericMethod(type2), expression5, expression6);
            type1 = ifFalse.Type;
          }
          else if (this._settings.HandleReferenceNavigationPropertyExpandFilter)
          {
            if (!(FilterBinder.Bind((IQueryable) null, filterClause, type2, this._context, odataQuerySettings) is LambdaExpression lambdaExpression))
              throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.ExpandFilterExpressionNotLambdaExpression, (object) property.Name, (object) "LambdaExpression"));
            ifFalse = (Expression) Expression.Condition(new SelectExpandBinder.ReferenceNavigationPropertyExpandFilterVisitor(lambdaExpression.Parameters.First<ParameterExpression>(), expression1).Visit(lambdaExpression.Body), expression1, (Expression) Expression.Constant((object) null, type1));
          }
          expression1 = this._settings.HandleNullPropagation != HandleNullPropagationOption.True ? ifFalse : (Expression) Expression.Condition((Expression) Expression.Equal(expression1, (Expression) Expression.Constant((object) null)), (Expression) Expression.Constant((object) null, type1), ifFalse);
        }
      }
      return this._settings.HandleNullPropagation != HandleNullPropagationOption.True ? expression1 : (Expression) Expression.Condition((Expression) Expression.Equal(source, (Expression) Expression.Constant((object) null)), (Expression) Expression.Constant((object) null, type1), expression1);
    }

    internal Expression ProjectElement(
      Expression source,
      SelectExpandClause selectExpandClause,
      IEdmStructuredType structuredType,
      IEdmNavigationSource navigationSource)
    {
      if (structuredType == null)
        return source;
      Type type1 = source.Type;
      Type type2 = typeof (SelectExpandWrapper<>).MakeGenericType(type1);
      List<MemberAssignment> bindings = new List<MemberAssignment>();
      bool isInstancePropertySet = false;
      bool isTypeNamePropertySet = false;
      bool isContainerPropertySet = false;
      if (SelectExpandBinder.IsSelectAll(selectExpandClause))
      {
        PropertyInfo property1 = type2.GetProperty("Instance");
        bindings.Add(Expression.Bind((MemberInfo) property1, source));
        PropertyInfo property2 = type2.GetProperty("UseInstanceForProperties");
        bindings.Add(Expression.Bind((MemberInfo) property2, (Expression) Expression.Constant((object) true)));
        isInstancePropertySet = true;
      }
      else
      {
        Expression typeNameExpression = SelectExpandBinder.CreateTypeNameExpression(source, structuredType, this._model);
        if (typeNameExpression != null)
        {
          isTypeNamePropertySet = true;
          PropertyInfo property = type2.GetProperty("InstanceType");
          bindings.Add(Expression.Bind((MemberInfo) property, typeNameExpression));
        }
      }
      if (selectExpandClause != null)
      {
        IDictionary<IEdmStructuralProperty, PathSelectItem> propertiesToInclude;
        IDictionary<IEdmNavigationProperty, ExpandedReferenceSelectItem> propertiesToExpand;
        ISet<IEdmStructuralProperty> autoSelectedProperties;
        bool isSelectingOpenTypeSegments = SelectExpandBinder.GetSelectExpandProperties(this._model, structuredType, navigationSource, selectExpandClause, this.InputCollapsed, out propertiesToInclude, out propertiesToExpand, out autoSelectedProperties) || SelectExpandBinder.IsSelectAllOnOpenType(selectExpandClause, structuredType);
        ISet<DynamicPathSegment> dynamicProperties = (ISet<DynamicPathSegment>) new HashSet<DynamicPathSegment>(selectExpandClause.SelectedItems.OfType<PathSelectItem>().Select<PathSelectItem, ODataPathSegment>((Func<PathSelectItem, ODataPathSegment>) (p => p.SelectedPath.LastSegment)).OfType<DynamicPathSegment>());
        if (((propertiesToExpand != null || propertiesToInclude != null ? 1 : (autoSelectedProperties != null ? 1 : 0)) | (isSelectingOpenTypeSegments ? 1 : 0)) != 0)
        {
          Expression expression = this.BuildPropertyContainer(source, structuredType, propertiesToExpand, propertiesToInclude, autoSelectedProperties, dynamicProperties, isSelectingOpenTypeSegments);
          if (expression != null)
          {
            PropertyInfo property = type2.GetProperty("Container");
            bindings.Add(Expression.Bind((MemberInfo) property, expression));
            isContainerPropertySet = true;
          }
        }
      }
      return (Expression) Expression.MemberInit(Expression.New(SelectExpandBinder.GetWrapperGenericType(isInstancePropertySet, isTypeNamePropertySet, isContainerPropertySet).MakeGenericType(type1)), (IEnumerable<MemberBinding>) bindings);
    }

    internal static bool GetSelectExpandProperties(
      IEdmModel model,
      IEdmStructuredType structuredType,
      IEdmNavigationSource navigationSource,
      SelectExpandClause selectExpandClause,
      bool inputCollapsed,
      out IDictionary<IEdmStructuralProperty, PathSelectItem> propertiesToInclude,
      out IDictionary<IEdmNavigationProperty, ExpandedReferenceSelectItem> propertiesToExpand,
      out ISet<IEdmStructuralProperty> autoSelectedProperties)
    {
      propertiesToInclude = (IDictionary<IEdmStructuralProperty, PathSelectItem>) null;
      propertiesToExpand = (IDictionary<IEdmNavigationProperty, ExpandedReferenceSelectItem>) null;
      autoSelectedProperties = (ISet<IEdmStructuralProperty>) null;
      bool expandProperties = false;
      Dictionary<IEdmStructuralProperty, SelectExpandIncludedProperty> dictionary = new Dictionary<IEdmStructuralProperty, SelectExpandIncludedProperty>();
      foreach (SelectItem selectedItem in selectExpandClause.SelectedItems)
      {
        if (selectedItem is ExpandedReferenceSelectItem expandedItem)
          SelectExpandBinder.ProcessExpandedItem(expandedItem, navigationSource, (IDictionary<IEdmStructuralProperty, SelectExpandIncludedProperty>) dictionary, ref propertiesToExpand);
        else if (selectedItem is PathSelectItem pathSelectItem && SelectExpandBinder.ProcessSelectedItem(pathSelectItem, navigationSource, (IDictionary<IEdmStructuralProperty, SelectExpandIncludedProperty>) dictionary))
          expandProperties = true;
      }
      if (!SelectExpandBinder.IsSelectAll(selectExpandClause) && !inputCollapsed)
      {
        if (structuredType is IEdmEntityType type)
        {
          foreach (IEdmStructuralProperty structuralProperty in type.Key())
          {
            if (!dictionary.Keys.Contains<IEdmStructuralProperty>(structuralProperty))
            {
              if (autoSelectedProperties == null)
                autoSelectedProperties = (ISet<IEdmStructuralProperty>) new HashSet<IEdmStructuralProperty>();
              autoSelectedProperties.Add(structuralProperty);
            }
          }
        }
        if (navigationSource != null && model != null)
        {
          foreach (IEdmStructuralProperty concurrencyProperty1 in model.GetConcurrencyProperties(navigationSource))
          {
            IEdmStructuralProperty concurrencyProperty = concurrencyProperty1;
            if (structuredType.Properties().Any<IEdmProperty>((Func<IEdmProperty, bool>) (p => p == concurrencyProperty)) && !dictionary.Keys.Contains<IEdmStructuralProperty>(concurrencyProperty))
            {
              if (autoSelectedProperties == null)
                autoSelectedProperties = (ISet<IEdmStructuralProperty>) new HashSet<IEdmStructuralProperty>();
              autoSelectedProperties.Add(concurrencyProperty);
            }
          }
        }
      }
      if (dictionary.Any<KeyValuePair<IEdmStructuralProperty, SelectExpandIncludedProperty>>())
      {
        propertiesToInclude = (IDictionary<IEdmStructuralProperty, PathSelectItem>) new Dictionary<IEdmStructuralProperty, PathSelectItem>();
        foreach (KeyValuePair<IEdmStructuralProperty, SelectExpandIncludedProperty> keyValuePair in dictionary)
          propertiesToInclude[keyValuePair.Key] = keyValuePair.Value == null ? (PathSelectItem) null : keyValuePair.Value.ToPathSelectItem();
      }
      return expandProperties;
    }

    private static void ProcessExpandedItem(
      ExpandedReferenceSelectItem expandedItem,
      IEdmNavigationSource navigationSource,
      IDictionary<IEdmStructuralProperty, SelectExpandIncludedProperty> currentLevelPropertiesInclude,
      ref IDictionary<IEdmNavigationProperty, ExpandedReferenceSelectItem> propertiesToExpand)
    {
      IList<ODataPathSegment> remainingSegments;
      ODataPathSegment nonTypeCastSegment = expandedItem.PathToNavigationProperty.GetFirstNonTypeCastSegment(out remainingSegments);
      if (nonTypeCastSegment is PropertySegment propertySegment)
      {
        SelectExpandIncludedProperty includedProperty;
        if (!currentLevelPropertiesInclude.TryGetValue(propertySegment.Property, out includedProperty))
        {
          includedProperty = new SelectExpandIncludedProperty(propertySegment, navigationSource);
          currentLevelPropertiesInclude[propertySegment.Property] = includedProperty;
        }
        includedProperty.AddSubExpandItem(remainingSegments, expandedItem);
      }
      else
      {
        NavigationPropertySegment navigationPropertySegment = nonTypeCastSegment as NavigationPropertySegment;
        if (propertiesToExpand == null)
          propertiesToExpand = (IDictionary<IEdmNavigationProperty, ExpandedReferenceSelectItem>) new Dictionary<IEdmNavigationProperty, ExpandedReferenceSelectItem>();
        propertiesToExpand[navigationPropertySegment.NavigationProperty] = expandedItem;
      }
    }

    private static bool ProcessSelectedItem(
      PathSelectItem pathSelectItem,
      IEdmNavigationSource navigationSource,
      IDictionary<IEdmStructuralProperty, SelectExpandIncludedProperty> currentLevelPropertiesInclude)
    {
      IList<ODataPathSegment> remainingSegments;
      switch (pathSelectItem.SelectedPath.GetFirstNonTypeCastSegment(out remainingSegments))
      {
        case PropertySegment propertySegment:
          SelectExpandIncludedProperty includedProperty;
          if (!currentLevelPropertiesInclude.TryGetValue(propertySegment.Property, out includedProperty))
          {
            includedProperty = new SelectExpandIncludedProperty(propertySegment, navigationSource);
            currentLevelPropertiesInclude[propertySegment.Property] = includedProperty;
          }
          includedProperty.AddSubSelectItem(remainingSegments, pathSelectItem);
          break;
        case DynamicPathSegment _:
          return true;
      }
      return false;
    }

    private static bool IsSelectAllOnOpenType(
      SelectExpandClause selectExpandClause,
      IEdmStructuredType structuredType)
    {
      return structuredType != null && structuredType.IsOpen && SelectExpandBinder.IsSelectAll(selectExpandClause);
    }

    private Expression CreateTotalCountExpression(Expression source, bool? countOption)
    {
      Expression totalCountExpression = (Expression) Expression.Constant((object) null, typeof (long?));
      Type elementType;
      if (!countOption.HasValue || !countOption.Value || !TypeHelper.IsCollection(source.Type, out elementType))
        return totalCountExpression;
      MethodInfo method;
      if (typeof (IQueryable).IsAssignableFrom(source.Type))
        method = ExpressionHelperMethods.QueryableCountGeneric.MakeGenericMethod(elementType);
      else
        method = ExpressionHelperMethods.EnumerableCountGeneric.MakeGenericMethod(elementType);
      Expression expression = (Expression) Expression.Call((Expression) null, method, source);
      return this._settings.HandleNullPropagation == HandleNullPropagationOption.True ? (Expression) Expression.Condition((Expression) Expression.Equal(source, (Expression) Expression.Constant((object) null)), (Expression) Expression.Constant((object) null, typeof (long?)), ExpressionHelpers.ToNullable(expression)) : expression;
    }

    private Expression BuildPropertyContainer(
      Expression source,
      IEdmStructuredType structuredType,
      IDictionary<IEdmNavigationProperty, ExpandedReferenceSelectItem> propertiesToExpand,
      IDictionary<IEdmStructuralProperty, PathSelectItem> propertiesToInclude,
      ISet<IEdmStructuralProperty> autoSelectedProperties,
      ISet<DynamicPathSegment> dynamicProperties,
      bool isSelectingOpenTypeSegments)
    {
      IList<NamedPropertyExpression> propertyExpressionList = (IList<NamedPropertyExpression>) new List<NamedPropertyExpression>();
      if (propertiesToExpand != null)
      {
        foreach (KeyValuePair<IEdmNavigationProperty, ExpandedReferenceSelectItem> keyValuePair in (IEnumerable<KeyValuePair<IEdmNavigationProperty, ExpandedReferenceSelectItem>>) propertiesToExpand)
          this.BuildExpandedProperty(source, structuredType, keyValuePair.Key, keyValuePair.Value, propertyExpressionList);
      }
      if (propertiesToInclude != null)
      {
        foreach (KeyValuePair<IEdmStructuralProperty, PathSelectItem> keyValuePair in (IEnumerable<KeyValuePair<IEdmStructuralProperty, PathSelectItem>>) propertiesToInclude)
          this.BuildSelectedProperty(source, structuredType, keyValuePair.Key, keyValuePair.Value, propertyExpressionList);
      }
      if (autoSelectedProperties != null)
      {
        foreach (IEdmStructuralProperty selectedProperty in (IEnumerable<IEdmStructuralProperty>) autoSelectedProperties)
        {
          Expression propertyNameExpression = this.CreatePropertyNameExpression(structuredType, (IEdmProperty) selectedProperty, source);
          Expression expressionWithClauses = this.CreatePropertyValueExpressionWithClauses(structuredType, (IEdmProperty) selectedProperty, source, (FilterClause) null, (ApplyClause) null, (ComputeClause) null);
          propertyExpressionList.Add(new NamedPropertyExpression(propertyNameExpression, expressionWithClauses)
          {
            AutoSelected = true
          });
        }
      }
      if (dynamicProperties != null && this.FlattenedPropertyContainer != null)
      {
        foreach (DynamicPathSegment dynamicProperty in (IEnumerable<DynamicPathSegment>) dynamicProperties)
        {
          Expression expression1;
          if (this.FlattenedPropertyContainer.TryGetValue(dynamicProperty.Identifier, out expression1))
          {
            Expression name = (Expression) Expression.Constant((object) dynamicProperty.Identifier);
            Expression expression2 = expression1;
            propertyExpressionList.Add(new NamedPropertyExpression(name, expression2));
          }
        }
      }
      if (isSelectingOpenTypeSegments)
        this.BuildDynamicProperty(source, structuredType, propertyExpressionList);
      return PropertyContainer.CreatePropertyContainer(propertyExpressionList);
    }

    internal void BuildExpandedProperty(
      Expression source,
      IEdmStructuredType structuredType,
      IEdmNavigationProperty navigationProperty,
      ExpandedReferenceSelectItem expandedItem,
      IList<NamedPropertyExpression> includedProperties)
    {
      IEdmEntityType entityType = navigationProperty.ToEntityType();
      ModelBoundQuerySettings boundQuerySettings = EdmLibHelpers.GetModelBoundQuerySettings((IEdmProperty) navigationProperty, (IEdmStructuredType) entityType, this._model);
      Expression propertyNameExpression = this.CreatePropertyNameExpression(structuredType, (IEdmProperty) navigationProperty, source);
      Expression expressionWithClauses = this.CreatePropertyValueExpressionWithClauses(structuredType, (IEdmProperty) navigationProperty, source, expandedItem.FilterOption, expandedItem.ApplyOption, expandedItem.ComputeOption);
      SelectExpandClause selectExpandClause = SelectExpandBinder.GetOrCreateSelectExpandClause(navigationProperty, expandedItem);
      Expression nullCheckExpression = this.GetNullCheckExpression(navigationProperty, expressionWithClauses, selectExpandClause);
      Expression totalCountExpression = this.CreateTotalCountExpression(expressionWithClauses, expandedItem.CountOption);
      int? modelBoundPageSize = boundQuerySettings == null ? new int?() : boundQuerySettings.PageSize;
      Expression expression = this.ProjectAsWrapper(expressionWithClauses, selectExpandClause, (IEdmStructuredType) entityType, expandedItem.NavigationSource, expandedItem.OrderByOption, expandedItem.TopOption, expandedItem.SkipOption, modelBoundPageSize);
      NamedPropertyExpression propertyExpression1 = new NamedPropertyExpression(propertyNameExpression, expression);
      if (selectExpandClause != null)
      {
        if (!navigationProperty.Type.IsCollection())
        {
          propertyExpression1.NullCheck = nullCheckExpression;
        }
        else
        {
          int? pageSize = this._settings.PageSize;
          if (pageSize.HasValue)
          {
            NamedPropertyExpression propertyExpression2 = propertyExpression1;
            pageSize = this._settings.PageSize;
            int? nullable = new int?(pageSize.Value);
            propertyExpression2.PageSize = nullable;
          }
          else if (boundQuerySettings != null)
          {
            pageSize = boundQuerySettings.PageSize;
            if (pageSize.HasValue)
            {
              NamedPropertyExpression propertyExpression3 = propertyExpression1;
              pageSize = boundQuerySettings.PageSize;
              int? nullable = new int?(pageSize.Value);
              propertyExpression3.PageSize = nullable;
            }
          }
        }
        propertyExpression1.TotalCount = totalCountExpression;
        propertyExpression1.CountOption = expandedItem.CountOption;
      }
      includedProperties.Add(propertyExpression1);
    }

    internal void BuildSelectedProperty(
      Expression source,
      IEdmStructuredType structuredType,
      IEdmStructuralProperty structuralProperty,
      PathSelectItem pathSelectItem,
      IList<NamedPropertyExpression> includedProperties)
    {
      Expression propertyNameExpression = this.CreatePropertyNameExpression(structuredType, (IEdmProperty) structuralProperty, source);
      if (pathSelectItem == null)
      {
        Expression expressionWithClauses = this.CreatePropertyValueExpressionWithClauses(structuredType, (IEdmProperty) structuralProperty, source, (FilterClause) null, (ApplyClause) null, (ComputeClause) null);
        includedProperties.Add(new NamedPropertyExpression(propertyNameExpression, expressionWithClauses));
      }
      else
      {
        SelectExpandClause selectAndExpand = pathSelectItem.SelectAndExpand;
        Expression expressionWithClauses = this.CreatePropertyValueExpressionWithClauses(structuredType, (IEdmProperty) structuralProperty, source, pathSelectItem.FilterOption, (ApplyClause) null, (ComputeClause) null);
        Type type = expressionWithClauses.Type;
        if (type == typeof (char[]) || type == typeof (byte[]))
        {
          includedProperties.Add(new NamedPropertyExpression(propertyNameExpression, expressionWithClauses));
        }
        else
        {
          Expression nullCheckExpression = SelectExpandBinder.GetNullCheckExpression(structuralProperty, expressionWithClauses, selectAndExpand);
          Expression totalCountExpression = this.CreateTotalCountExpression(expressionWithClauses, pathSelectItem.CountOption);
          IEdmStructuredType structuredType1 = structuralProperty.Type.ToStructuredType();
          ModelBoundQuerySettings boundQuerySettings = (ModelBoundQuerySettings) null;
          if (structuredType1 != null)
            boundQuerySettings = EdmLibHelpers.GetModelBoundQuerySettings((IEdmProperty) structuralProperty, structuredType1, this._context.Model);
          int? modelBoundPageSize = boundQuerySettings == null ? new int?() : boundQuerySettings.PageSize;
          Expression expression = this.ProjectAsWrapper(expressionWithClauses, selectAndExpand, structuralProperty.Type.ToStructuredType(), pathSelectItem.NavigationSource, pathSelectItem.OrderByOption, pathSelectItem.TopOption, pathSelectItem.SkipOption, modelBoundPageSize);
          NamedPropertyExpression propertyExpression = new NamedPropertyExpression(propertyNameExpression, expression);
          if (selectAndExpand != null)
          {
            if (!structuralProperty.Type.IsCollection())
              propertyExpression.NullCheck = nullCheckExpression;
            else if (this._settings.PageSize.HasValue)
              propertyExpression.PageSize = new int?(this._settings.PageSize.Value);
            else if (boundQuerySettings != null && boundQuerySettings.PageSize.HasValue)
              propertyExpression.PageSize = new int?(boundQuerySettings.PageSize.Value);
            propertyExpression.TotalCount = totalCountExpression;
            propertyExpression.CountOption = pathSelectItem.CountOption;
          }
          includedProperties.Add(propertyExpression);
        }
      }
    }

    internal void BuildDynamicProperty(
      Expression source,
      IEdmStructuredType structuredType,
      IList<NamedPropertyExpression> includedProperties)
    {
      PropertyInfo propertyDictionary = EdmLibHelpers.GetDynamicPropertyDictionary(structuredType, this._model);
      if (!(propertyDictionary != (PropertyInfo) null))
        return;
      Expression name = (Expression) Expression.Constant((object) propertyDictionary.Name);
      Expression expression1 = (Expression) Expression.Property(source, propertyDictionary.Name);
      Expression nullable = ExpressionHelpers.ToNullable(expression1);
      Expression expression2 = this._settings.HandleNullPropagation != HandleNullPropagationOption.True ? nullable : (Expression) Expression.Condition((Expression) Expression.Equal(source, (Expression) Expression.Constant((object) null)), (Expression) Expression.Constant((object) null, TypeHelper.ToNullable(expression1.Type)), nullable);
      includedProperties.Add(new NamedPropertyExpression(name, expression2));
    }

    private static SelectExpandClause GetOrCreateSelectExpandClause(
      IEdmNavigationProperty navigationProperty,
      ExpandedReferenceSelectItem expandedItem)
    {
      if (expandedItem is ExpandedNavigationSelectItem navigationSelectItem)
        return navigationSelectItem.SelectAndExpand;
      IList<SelectItem> selectedItems = (IList<SelectItem>) new List<SelectItem>();
      foreach (IEdmStructuralProperty property in navigationProperty.ToEntityType().Key())
        selectedItems.Add((SelectItem) new PathSelectItem(new ODataSelectPath(new ODataPathSegment[1]
        {
          (ODataPathSegment) new PropertySegment(property)
        })));
      return new SelectExpandClause((IEnumerable<SelectItem>) selectedItems, false);
    }

    private Expression AddOrderByQueryForSource(
      Expression source,
      OrderByClause orderbyClause,
      Type elementType)
    {
      if (orderbyClause != null)
      {
        ODataQuerySettings querySettings = new ODataQuerySettings()
        {
          HandleNullPropagation = HandleNullPropagationOption.True
        };
        LambdaExpression orderByLambda = FilterBinder.Bind((IQueryable) null, orderbyClause, elementType, this._context, querySettings);
        source = ExpressionHelpers.OrderBy(source, orderByLambda, elementType, orderbyClause.Direction);
      }
      return source;
    }

    private static Expression GetNullCheckExpression(
      IEdmStructuralProperty propertyToInclude,
      Expression propertyValue,
      SelectExpandClause projection)
    {
      if (projection == null || propertyToInclude.Type.IsCollection())
        return (Expression) null;
      return SelectExpandBinder.IsSelectAll(projection) && propertyToInclude.Type.IsComplex() ? (Expression) Expression.Equal(propertyValue, (Expression) Expression.Constant((object) null)) : (Expression) null;
    }

    private Expression GetNullCheckExpression(
      IEdmNavigationProperty propertyToExpand,
      Expression propertyValue,
      SelectExpandClause projection)
    {
      if (projection == null || propertyToExpand.Type.IsCollection())
        return (Expression) null;
      if (SelectExpandBinder.IsSelectAll(projection) || !propertyToExpand.ToEntityType().Key().Any<IEdmStructuralProperty>())
        return (Expression) Expression.Equal(propertyValue, (Expression) Expression.Constant((object) null));
      Expression left = (Expression) null;
      foreach (IEdmStructuralProperty property in propertyToExpand.ToEntityType().Key())
      {
        Expression expressionWithClauses = this.CreatePropertyValueExpressionWithClauses((IEdmStructuredType) propertyToExpand.ToEntityType(), (IEdmProperty) property, propertyValue, (FilterClause) null, (ApplyClause) null, (ComputeClause) null);
        BinaryExpression right = Expression.Equal(expressionWithClauses, (Expression) Expression.Constant((object) null, expressionWithClauses.Type));
        left = left == null ? (Expression) right : (Expression) Expression.And(left, (Expression) right);
      }
      return left;
    }

    private Expression ProjectCollection(
      Expression source,
      Type elementType,
      SelectExpandClause selectExpandClause,
      IEdmStructuredType structuredType,
      IEdmNavigationSource navigationSource,
      OrderByClause orderByClause,
      long? topOption,
      long? skipOption,
      int? modelBoundPageSize)
    {
      ParameterExpression source1 = Expression.Parameter(elementType);
      Expression body = structuredType == null ? (Expression) source1 : this.ProjectElement((Expression) source1, selectExpandClause, structuredType, navigationSource);
      LambdaExpression lambdaExpression = Expression.Lambda(body, source1);
      if (orderByClause != null)
        source = this.AddOrderByQueryForSource(source, orderByClause, elementType);
      bool flag1 = topOption.HasValue && topOption.HasValue;
      bool flag2 = skipOption.HasValue && skipOption.HasValue;
      int? nullable;
      if (structuredType is IEdmEntityType type)
      {
        nullable = this._settings.PageSize;
        if (((nullable.HasValue ? 1 : (modelBoundPageSize.HasValue ? 1 : 0)) | (flag1 ? 1 : 0) | (flag2 ? 1 : 0)) != 0)
        {
          IEnumerable<IEdmStructuralProperty> structuralProperties = type.Key().Any<IEdmStructuralProperty>() ? type.Key() : (IEnumerable<IEdmStructuralProperty>) type.StructuralProperties().Where<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (property => property.Type.IsPrimitive() && !property.Type.IsStream())).OrderBy<IEdmStructuralProperty, string>((Func<IEdmStructuralProperty, string>) (property => property.Name));
          if (orderByClause == null)
          {
            bool alreadyOrdered = false;
            foreach (IEdmStructuralProperty structuralProperty in structuralProperties)
            {
              source = ExpressionHelpers.OrderByPropertyExpression(source, structuralProperty.Name, elementType, alreadyOrdered);
              if (!alreadyOrdered)
                alreadyOrdered = true;
            }
          }
        }
      }
      if (flag2)
        source = ExpressionHelpers.Skip(source, (int) skipOption.Value, elementType, this._settings.EnableConstantParameterization);
      if (flag1)
        source = ExpressionHelpers.Take(source, (int) topOption.Value, elementType, this._settings.EnableConstantParameterization);
      nullable = this._settings.PageSize;
      if (((nullable.HasValue ? 1 : (modelBoundPageSize.HasValue ? 1 : 0)) | (flag1 ? 1 : 0) | (flag2 ? 1 : 0)) != 0 && !this._settings.EnableCorrelatedSubqueryBuffering)
      {
        nullable = this._settings.PageSize;
        if (nullable.HasValue)
        {
          Expression source2 = source;
          nullable = this._settings.PageSize;
          int count = nullable.Value + 1;
          Type elementType1 = elementType;
          int num = this._settings.EnableConstantParameterization ? 1 : 0;
          source = ExpressionHelpers.Take(source2, count, elementType1, num != 0);
        }
        else
        {
          nullable = this._settings.ModelBoundPageSize;
          if (nullable.HasValue)
            source = ExpressionHelpers.Take(source, modelBoundPageSize.Value + 1, elementType, this._settings.EnableConstantParameterization);
        }
      }
      Expression ifFalse = (Expression) Expression.Call(SelectExpandBinder.GetSelectMethod(elementType, body.Type), source, (Expression) lambdaExpression);
      if (this._settings.EnableCorrelatedSubqueryBuffering)
        ifFalse = (Expression) Expression.Call(ExpressionHelperMethods.QueryableToList.MakeGenericMethod(body.Type), ifFalse);
      return this._settings.HandleNullPropagation == HandleNullPropagationOption.True ? (Expression) Expression.Condition((Expression) Expression.Equal(source, (Expression) Expression.Constant((object) null)), (Expression) Expression.Constant((object) null, ifFalse.Type), ifFalse) : ifFalse;
    }

    internal static Expression CreateTypeNameExpression(
      Expression source,
      IEdmStructuredType elementType,
      IEdmModel model)
    {
      IReadOnlyList<IEdmStructuredType> allDerivedTypes = SelectExpandBinder.GetAllDerivedTypes(elementType, model);
      if (allDerivedTypes.Count == 0)
        return (Expression) null;
      Expression ifFalse = (Expression) Expression.Constant((object) elementType.FullTypeName());
      for (int index = 0; index < allDerivedTypes.Count; ++index)
      {
        Type clrType = EdmLibHelpers.GetClrType((IEdmType) allDerivedTypes[index], model);
        if (clrType == (Type) null)
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.MappingDoesNotContainResourceType, (object) allDerivedTypes[0].FullTypeName()));
        ifFalse = (Expression) Expression.Condition((Expression) Expression.TypeIs(source, clrType), (Expression) Expression.Constant((object) allDerivedTypes[index].FullTypeName()), ifFalse);
      }
      return ifFalse;
    }

    private static IReadOnlyList<IEdmStructuredType> GetAllDerivedTypes(
      IEdmStructuredType baseType,
      IEdmModel model)
    {
      IEnumerable<IEdmStructuredType> edmStructuredTypes = model.SchemaElements.OfType<IEdmStructuredType>();
      List<Tuple<int, IEdmStructuredType>> source = new List<Tuple<int, IEdmStructuredType>>();
      foreach (IEdmStructuredType type in edmStructuredTypes)
      {
        int num = SelectExpandBinder.IsDerivedTypeOf(type, baseType);
        if (num > 0)
          source.Add(Tuple.Create<int, IEdmStructuredType>(num, type));
      }
      return (IReadOnlyList<IEdmStructuredType>) source.OrderBy<Tuple<int, IEdmStructuredType>, int>((Func<Tuple<int, IEdmStructuredType>, int>) (tuple => tuple.Item1)).Select<Tuple<int, IEdmStructuredType>, IEdmStructuredType>((Func<Tuple<int, IEdmStructuredType>, IEdmStructuredType>) (tuple => tuple.Item2)).ToList<IEdmStructuredType>();
    }

    private static int IsDerivedTypeOf(IEdmStructuredType type, IEdmStructuredType baseType)
    {
      int num = 0;
      while (type != null)
      {
        if (baseType == type)
          return num;
        type = type.BaseType();
        ++num;
      }
      return -1;
    }

    private static MethodInfo GetSelectMethod(Type elementType, Type resultType) => ExpressionHelperMethods.EnumerableSelectGeneric.MakeGenericMethod(elementType, resultType);

    private static bool IsSelectAll(SelectExpandClause selectExpandClause) => selectExpandClause == null || selectExpandClause.AllSelected || selectExpandClause.SelectedItems.OfType<WildcardSelectItem>().Any<WildcardSelectItem>();

    private static Type GetWrapperGenericType(
      bool isInstancePropertySet,
      bool isTypeNamePropertySet,
      bool isContainerPropertySet)
    {
      return isInstancePropertySet ? (!isContainerPropertySet ? typeof (SelectExpandBinder.SelectAll<>) : typeof (SelectExpandBinder.SelectAllAndExpand<>)) : (!isTypeNamePropertySet ? typeof (SelectExpandBinder.SelectSome<>) : typeof (SelectExpandBinder.SelectSomeAndInheritance<>));
    }

    private class ReferenceNavigationPropertyExpandFilterVisitor : ExpressionVisitor
    {
      private Expression _source;
      private ParameterExpression _parameterExpression;

      public ReferenceNavigationPropertyExpandFilterVisitor(
        ParameterExpression parameterExpression,
        Expression source)
      {
        this._source = source;
        this._parameterExpression = parameterExpression;
      }

      protected override Expression VisitParameter(ParameterExpression node)
      {
        if (node != this._parameterExpression)
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.ReferenceNavigationPropertyExpandFilterVisitorUnexpectedParameter, (object) node.Name));
        return this._source;
      }
    }

    private class SelectAllAndExpand<TEntity> : SelectExpandWrapper<TEntity>
    {
    }

    private class SelectAll<TEntity> : SelectExpandWrapper<TEntity>
    {
    }

    private class SelectSomeAndInheritance<TEntity> : SelectExpandWrapper<TEntity>
    {
    }

    private class SelectSome<TEntity> : SelectExpandBinder.SelectAllAndExpand<TEntity>
    {
    }
  }
}
