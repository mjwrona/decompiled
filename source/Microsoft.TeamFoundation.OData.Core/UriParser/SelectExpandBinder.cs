// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectExpandBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Microsoft.OData.UriParser
{
  internal sealed class SelectExpandBinder
  {
    private readonly ODataUriParserConfiguration configuration;
    private readonly IEdmNavigationSource navigationSource;
    private readonly IEdmStructuredType edmType;
    private List<ODataPathSegment> parsedSegments = new List<ODataPathSegment>();
    private BindingState state;

    public SelectExpandBinder(
      ODataUriParserConfiguration configuration,
      ODataPathInfo odataPathInfo,
      BindingState state)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataUriParserConfiguration>(configuration, nameof (configuration));
      ExceptionUtils.CheckArgumentNotNull<IEdmStructuredType>(odataPathInfo.TargetStructuredType, nameof (edmType));
      this.configuration = configuration;
      this.edmType = odataPathInfo.TargetStructuredType;
      this.navigationSource = odataPathInfo.TargetNavigationSource;
      this.parsedSegments = odataPathInfo.Segments.ToList<ODataPathSegment>();
      this.state = state;
    }

    public IEdmModel Model => this.configuration.Model;

    public IEdmStructuredType EdmType => this.edmType;

    public IEdmNavigationSource NavigationSource => this.navigationSource;

    private ODataUriParserSettings Settings => this.configuration.Settings;

    private ODataUriParserConfiguration Configuration => this.configuration;

    public SelectExpandClause Bind(ExpandToken expandToken, SelectToken selectToken)
    {
      List<SelectItem> selectItemList = new List<SelectItem>();
      if (expandToken != null && expandToken.ExpandTerms.Any<ExpandTermToken>())
        selectItemList.AddRange(expandToken.ExpandTerms.Select<ExpandTermToken, SelectItem>(new Func<ExpandTermToken, SelectItem>(this.GenerateExpandItem)).Where<SelectItem>((Func<SelectItem, bool>) (s => s != null)));
      bool allSelected = true;
      if (selectToken != null && selectToken.SelectTerms.Any<SelectTermToken>())
      {
        allSelected = false;
        foreach (SelectTermToken selectTerm in selectToken.SelectTerms)
        {
          SelectItem selectItem = this.GenerateSelectItem(selectTerm);
          PathSelectItem pathSelectItem1 = selectItem as PathSelectItem;
          bool flag = false;
          if (pathSelectItem1 != null)
          {
            foreach (PathSelectItem pathSelectItem2 in selectItemList.OfType<PathSelectItem>())
            {
              if (pathSelectItem1.HasOptions && SelectExpandBinder.overLaps(pathSelectItem1, pathSelectItem2) || pathSelectItem2.HasOptions && SelectExpandBinder.overLaps(pathSelectItem2, pathSelectItem1))
                throw new ODataException(Microsoft.OData.Strings.SelectTreeNormalizer_MultipleSelecTermWithSamePathFound((object) SelectExpandBinder.ToPathString(selectTerm.PathToProperty)));
              if (pathSelectItem1.SelectedPath.Equals((ODataPath) pathSelectItem2.SelectedPath))
                flag = true;
            }
          }
          if (!flag)
            SelectExpandBinder.AddToSelectedItems(selectItem, selectItemList);
        }
      }
      return new SelectExpandClause((IEnumerable<SelectItem>) selectItemList, allSelected);
    }

    internal static string ToPathString(PathSegmentToken head)
    {
      StringBuilder stringBuilder = new StringBuilder();
      PathSegmentToken pathSegmentToken = head;
      while (pathSegmentToken != null)
      {
        stringBuilder.Append(pathSegmentToken.Identifier);
        if (pathSegmentToken is NonSystemToken nonSystemToken && nonSystemToken.NamedValues != null)
        {
          stringBuilder.Append("(");
          bool flag = true;
          foreach (NamedValue namedValue in nonSystemToken.NamedValues)
          {
            if (flag)
              flag = false;
            else
              stringBuilder.Append(",");
            stringBuilder.Append(namedValue.Name).Append("=").Append(namedValue.Value.Value);
          }
          stringBuilder.Append(")");
        }
        pathSegmentToken = pathSegmentToken.NextToken;
        if (pathSegmentToken != null)
          stringBuilder.Append("/");
      }
      return stringBuilder.ToString();
    }

    private static bool overLaps(PathSelectItem firstPath, PathSelectItem secondPath)
    {
      IEnumerator<ODataPathSegment> enumerator1 = firstPath.SelectedPath.GetEnumerator();
      IEnumerator<ODataPathSegment> enumerator2 = secondPath.SelectedPath.GetEnumerator();
      bool flag;
      do
        ;
      while ((flag = enumerator1.MoveNext()) && enumerator2.MoveNext() && enumerator1.Current.Identifier == enumerator2.Current.Identifier);
      return !flag;
    }

    private SelectItem GenerateSelectItem(SelectTermToken tokenIn)
    {
      ExceptionUtils.CheckArgumentNotNull<SelectTermToken>(tokenIn, nameof (tokenIn));
      ExceptionUtils.CheckArgumentNotNull<PathSegmentToken>(tokenIn.PathToProperty, "pathToProperty");
      SelectExpandBinder.VerifySelectedPath(tokenIn);
      SelectItem newSelectItem;
      if (this.ProcessWildcardTokenPath(tokenIn, out newSelectItem))
        return newSelectItem;
      IList<ODataPathSegment> odataPathSegmentList = (IList<ODataPathSegment>) this.ProcessSelectTokenPath(tokenIn.PathToProperty);
      if (SelectExpandBinder.VerifySelectedNavigationProperty(odataPathSegmentList, tokenIn))
        return (SelectItem) new PathSelectItem(new ODataSelectPath((IEnumerable<ODataPathSegment>) odataPathSegmentList));
      IEdmNavigationSource navigationSource = this.NavigationSource;
      IEdmType type = odataPathSegmentList.Last<ODataPathSegment>().TargetEdmType;
      if (type is IEdmCollectionType edmCollectionType)
        type = edmCollectionType.ElementType.Definition;
      IEdmTypeReference typeReference = type.ToTypeReference();
      ComputeClause computeOption = this.BindCompute(tokenIn.ComputeOption, navigationSource, typeReference);
      HashSet<EndPathToken> generatedProperties = SelectExpandBinder.GetGeneratedProperties(computeOption, (ApplyClause) null);
      FilterClause filterOption = this.BindFilter(tokenIn.FilterOption, navigationSource, typeReference, generatedProperties);
      OrderByClause orderByOption = this.BindOrderby(tokenIn.OrderByOptions, navigationSource, typeReference, generatedProperties);
      SearchClause searchOption = this.BindSearch(tokenIn.SearchOption, navigationSource, typeReference);
      List<ODataPathSegment> segments = new List<ODataPathSegment>((IEnumerable<ODataPathSegment>) this.parsedSegments);
      segments.AddRange((IEnumerable<ODataPathSegment>) odataPathSegmentList);
      SelectExpandClause selectAndExpand = this.BindSelectExpand((ExpandToken) null, tokenIn.SelectOption, (IList<ODataPathSegment>) segments, navigationSource, typeReference, generatedProperties);
      return (SelectItem) new PathSelectItem(new ODataSelectPath((IEnumerable<ODataPathSegment>) odataPathSegmentList), navigationSource, selectAndExpand, filterOption, orderByOption, tokenIn.TopOption, tokenIn.SkipOption, tokenIn.CountQueryOption, searchOption, computeOption);
    }

    private SelectItem GenerateExpandItem(ExpandTermToken tokenIn)
    {
      ExceptionUtils.CheckArgumentNotNull<ExpandTermToken>(tokenIn, nameof (tokenIn));
      PathSegmentToken toNavigationProp = tokenIn.PathToNavigationProp;
      IEdmStructuredType edmType = this.EdmType;
      List<ODataPathSegment> odataPathSegmentList1 = new List<ODataPathSegment>();
      PathSegmentToken firstNonTypeToken = toNavigationProp;
      if (toNavigationProp.IsNamespaceOrContainerQualified())
        odataPathSegmentList1.AddRange(SelectExpandPathBinder.FollowTypeSegments(toNavigationProp, this.Model, this.Settings.SelectExpandLimit, this.configuration.Resolver, ref edmType, out firstNonTypeToken));
      IEdmProperty edmProperty1 = this.configuration.Resolver.ResolveProperty(edmType, firstNonTypeToken.Identifier);
      IEdmNavigationProperty navigationProperty = edmProperty1 != null ? edmProperty1 as IEdmNavigationProperty : throw ExceptionUtil.CreatePropertyNotFoundException(toNavigationProp.Identifier, edmType.FullTypeName());
      IEdmStructuralProperty edmProperty2 = edmProperty1 as IEdmStructuralProperty;
      if (navigationProperty == null && edmProperty2 == null)
        throw new ODataException(Microsoft.OData.Strings.ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty((object) toNavigationProp.Identifier, (object) edmType.FullTypeName()));
      if (edmProperty2 != null)
        navigationProperty = this.ParseComplexTypesBeforeNavigation(edmProperty2, ref firstNonTypeToken, odataPathSegmentList1);
      if (firstNonTypeToken.NextToken != null && firstNonTypeToken.NextToken.NextToken != null)
        throw new ODataException(Microsoft.OData.Strings.ExpandItemBinder_TraversingMultipleNavPropsInTheSamePath);
      bool flag = false;
      if (firstNonTypeToken.NextToken != null)
      {
        if (!(firstNonTypeToken.NextToken.Identifier == "$ref"))
          throw new ODataException(Microsoft.OData.Strings.ExpandItemBinder_TraversingMultipleNavPropsInTheSamePath);
        flag = true;
      }
      List<ODataPathSegment> odataPathSegmentList2 = new List<ODataPathSegment>((IEnumerable<ODataPathSegment>) this.parsedSegments);
      odataPathSegmentList2.AddRange((IEnumerable<ODataPathSegment>) odataPathSegmentList1);
      IEdmNavigationSource navigationSource = (IEdmNavigationSource) null;
      if (this.NavigationSource != null)
        navigationSource = this.NavigationSource.FindNavigationTarget(navigationProperty, new Func<IEdmPathExpression, List<ODataPathSegment>, bool>(BindingPathHelper.MatchBindingPath), odataPathSegmentList2, out IEdmPathExpression _);
      NavigationPropertySegment navigationPropertySegment = new NavigationPropertySegment(navigationProperty, navigationSource);
      odataPathSegmentList1.Add((ODataPathSegment) navigationPropertySegment);
      odataPathSegmentList2.Add((ODataPathSegment) navigationPropertySegment);
      ODataExpandPath pathToNavigationProperty = new ODataExpandPath((IEnumerable<ODataPathSegment>) odataPathSegmentList1);
      ApplyClause applyOption = this.BindApply(tokenIn.ApplyOptions, navigationSource);
      ComputeClause computeOption = this.BindCompute(tokenIn.ComputeOption, navigationSource);
      HashSet<EndPathToken> generatedProperties = SelectExpandBinder.GetGeneratedProperties(computeOption, applyOption);
      bool collapsed = applyOption != null && applyOption.Transformations.Any<TransformationNode>((Func<TransformationNode, bool>) (t => t.Kind == TransformationNodeKind.Aggregate || t.Kind == TransformationNodeKind.GroupBy));
      FilterClause filterOption = this.BindFilter(tokenIn.FilterOption, navigationSource, (IEdmTypeReference) null, generatedProperties, collapsed);
      OrderByClause orderByOption = this.BindOrderby(tokenIn.OrderByOptions, navigationSource, (IEdmTypeReference) null, generatedProperties, collapsed);
      SearchClause searchOption = this.BindSearch(tokenIn.SearchOption, navigationSource, (IEdmTypeReference) null);
      if (flag)
        return (SelectItem) new ExpandedReferenceSelectItem(pathToNavigationProperty, navigationSource, filterOption, orderByOption, tokenIn.TopOption, tokenIn.SkipOption, tokenIn.CountQueryOption, searchOption, computeOption, applyOption);
      SelectExpandClause selectAndExpand = this.BindSelectExpand(tokenIn.ExpandOption, tokenIn.SelectOption, (IList<ODataPathSegment>) odataPathSegmentList2, navigationSource, (IEdmTypeReference) null, generatedProperties, collapsed);
      LevelsClause levels = SelectExpandBinder.ParseLevels(tokenIn.LevelsOption, (IEdmType) edmType, navigationProperty);
      return (SelectItem) new ExpandedNavigationSelectItem(pathToNavigationProperty, navigationSource, selectAndExpand, filterOption, orderByOption, tokenIn.TopOption, tokenIn.SkipOption, tokenIn.CountQueryOption, searchOption, levels, computeOption, applyOption);
    }

    private ApplyClause BindApply(
      IEnumerable<QueryToken> applyToken,
      IEdmNavigationSource navigationSource)
    {
      if (applyToken == null || !applyToken.Any<QueryToken>())
        return (ApplyClause) null;
      MetadataBinder metadataBinder = SelectExpandBinder.BuildNewMetadataBinder(this.Configuration, navigationSource, (IEdmTypeReference) null);
      return new ApplyBinder(new MetadataBinder.QueryTokenVisitor(metadataBinder.Bind), metadataBinder.BindingState).BindApply(applyToken);
    }

    private ComputeClause BindCompute(
      ComputeToken computeToken,
      IEdmNavigationSource navigationSource,
      IEdmTypeReference elementType = null)
    {
      return computeToken != null ? new ComputeBinder(new MetadataBinder.QueryTokenVisitor(SelectExpandBinder.BuildNewMetadataBinder(this.Configuration, navigationSource, elementType).Bind)).BindCompute(computeToken) : (ComputeClause) null;
    }

    private FilterClause BindFilter(
      QueryToken filterToken,
      IEdmNavigationSource navigationSource,
      IEdmTypeReference elementType,
      HashSet<EndPathToken> generatedProperties,
      bool collapsed = false)
    {
      if (filterToken == null)
        return (FilterClause) null;
      MetadataBinder metadataBinder = SelectExpandBinder.BuildNewMetadataBinder(this.Configuration, navigationSource, elementType, generatedProperties, collapsed);
      return new FilterBinder(new MetadataBinder.QueryTokenVisitor(metadataBinder.Bind), metadataBinder.BindingState).BindFilter(filterToken);
    }

    private OrderByClause BindOrderby(
      IEnumerable<OrderByToken> orderByToken,
      IEdmNavigationSource navigationSource,
      IEdmTypeReference elementType,
      HashSet<EndPathToken> generatedProperties,
      bool collapsed = false)
    {
      if (orderByToken == null || !orderByToken.Any<OrderByToken>())
        return (OrderByClause) null;
      MetadataBinder metadataBinder = SelectExpandBinder.BuildNewMetadataBinder(this.Configuration, navigationSource, elementType, generatedProperties, collapsed);
      return new OrderByBinder(new MetadataBinder.QueryTokenVisitor(metadataBinder.Bind)).BindOrderBy(metadataBinder.BindingState, orderByToken);
    }

    private SearchClause BindSearch(
      QueryToken searchToken,
      IEdmNavigationSource navigationSource,
      IEdmTypeReference elementType)
    {
      return searchToken != null ? new SearchBinder(new MetadataBinder.QueryTokenVisitor(SelectExpandBinder.BuildNewMetadataBinder(this.Configuration, navigationSource, elementType).Bind)).BindSearch(searchToken) : (SearchClause) null;
    }

    private SelectExpandClause BindSelectExpand(
      ExpandToken expandToken,
      SelectToken selectToken,
      IList<ODataPathSegment> segments,
      IEdmNavigationSource navigationSource,
      IEdmTypeReference elementType,
      HashSet<EndPathToken> generatedProperties = null,
      bool collapsed = false)
    {
      if (expandToken == null && selectToken == null)
        return new SelectExpandClause((IEnumerable<SelectItem>) new Collection<SelectItem>(), true);
      BindingState bindingState = SelectExpandBinder.CreateBindingState(this.Configuration, navigationSource, elementType, generatedProperties, collapsed);
      return new SelectExpandBinder(this.Configuration, new ODataPathInfo(new ODataPath((IEnumerable<ODataPathSegment>) segments)), bindingState).Bind(expandToken, selectToken);
    }

    private bool ProcessWildcardTokenPath(SelectTermToken selectToken, out SelectItem newSelectItem)
    {
      newSelectItem = (SelectItem) null;
      if (selectToken == null || selectToken.PathToProperty == null)
        return false;
      PathSegmentToken pathToProperty = selectToken.PathToProperty;
      if (!SelectPathSegmentTokenBinder.TryBindAsWildcard(pathToProperty, this.Model, out newSelectItem))
        return false;
      if (pathToProperty.NextToken != null)
        throw new ODataException(Microsoft.OData.Strings.SelectExpandBinder_InvalidIdentifierAfterWildcard((object) pathToProperty.NextToken.Identifier));
      SelectExpandBinder.VerifyNoQueryOptionsNested(selectToken, pathToProperty.Identifier);
      return true;
    }

    private List<ODataPathSegment> ProcessSelectTokenPath(PathSegmentToken tokenIn)
    {
      List<ODataPathSegment> source = new List<ODataPathSegment>();
      IEdmStructuredType edmType1 = this.edmType;
      if (tokenIn.IsNamespaceOrContainerQualified() && !UriParserHelper.IsAnnotation(tokenIn.Identifier))
      {
        PathSegmentToken firstNonTypeToken;
        source.AddRange(SelectExpandPathBinder.FollowTypeSegments(tokenIn, this.Model, this.Settings.SelectExpandLimit, this.configuration.Resolver, ref edmType1, out firstNonTypeToken));
        tokenIn = (PathSegmentToken) (firstNonTypeToken as NonSystemToken);
        if (tokenIn == null)
          throw new ODataException(Microsoft.OData.Strings.SelectExpandBinder_SystemTokenInSelect((object) firstNonTypeToken.Identifier));
      }
      ODataPathSegment odataPathSegment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(tokenIn, this.Model, edmType1, this.configuration.Resolver, this.state);
      if (odataPathSegment != null)
      {
        source.Add(odataPathSegment);
        while (true)
        {
          IEdmStructuredType edmStructuredType = odataPathSegment.EdmType as IEdmStructuredType;
          IEdmCollectionType edmType2 = odataPathSegment.EdmType as IEdmCollectionType;
          IEdmPrimitiveType edmType3 = odataPathSegment.EdmType as IEdmPrimitiveType;
          DynamicPathSegment dynamicPathSegment = odataPathSegment as DynamicPathSegment;
          if ((edmStructuredType != null && edmStructuredType.TypeKind == EdmTypeKind.Complex || edmType2 != null && edmType2.ElementType.TypeKind() == EdmTypeKind.Complex || edmType3 != null && edmType3.TypeKind == EdmTypeKind.Primitive || dynamicPathSegment != null && tokenIn.NextToken != null) && tokenIn.NextToken is NonSystemToken nextToken)
          {
            if (UriParserHelper.IsAnnotation(nextToken.Identifier))
              odataPathSegment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment((PathSegmentToken) nextToken, this.Model, edmStructuredType, this.configuration.Resolver);
            else if (edmType3 == null && dynamicPathSegment == null)
            {
              if (edmStructuredType == null)
                edmStructuredType = edmType2.ElementType.Definition as IEdmStructuredType;
              odataPathSegment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment((PathSegmentToken) nextToken, this.Model, edmStructuredType, this.configuration.Resolver);
            }
            else
            {
              EdmPrimitiveTypeKind primitiveTypeKind = EdmCoreModel.Instance.GetPrimitiveTypeKind(nextToken.Identifier);
              IEdmPrimitiveType primitiveType = EdmCoreModel.Instance.GetPrimitiveType(primitiveTypeKind);
              if (primitiveType != null)
                odataPathSegment = (ODataPathSegment) new TypeSegment((IEdmType) primitiveType, (IEdmType) primitiveType, (IEdmNavigationSource) null);
              else if (dynamicPathSegment != null)
                odataPathSegment = (ODataPathSegment) new DynamicPathSegment(nextToken.Identifier);
              else
                break;
            }
            if (odataPathSegment == null)
            {
              IEdmStructuredType typeFromModel = UriEdmHelpers.FindTypeFromModel(this.Model, nextToken.Identifier, this.configuration.Resolver) as IEdmStructuredType;
              if (typeFromModel.IsOrInheritsFrom((IEdmType) edmStructuredType))
                odataPathSegment = (ODataPathSegment) new TypeSegment((IEdmType) typeFromModel, (IEdmNavigationSource) null);
            }
            if (odataPathSegment != null)
            {
              tokenIn = (PathSegmentToken) nextToken;
              source.Add(odataPathSegment);
            }
            else
              goto label_22;
          }
          else
            goto label_22;
        }
        throw new ODataException(Microsoft.OData.Strings.SelectBinder_MultiLevelPathInSelect);
      }
label_22:
      if (tokenIn.NextToken != null)
        throw new ODataException(Microsoft.OData.Strings.SelectBinder_MultiLevelPathInSelect);
      if (odataPathSegment == null)
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_InvalidIdentifierInQueryOption((object) tokenIn.Identifier));
      if (source.LastOrDefault<ODataPathSegment>() is NavigationPropertySegment && tokenIn.NextToken != null)
        throw new ODataException(Microsoft.OData.Strings.SelectBinder_MultiLevelPathInSelect);
      return source;
    }

    private static HashSet<EndPathToken> GetGeneratedProperties(
      ComputeClause computeOption,
      ApplyClause applyOption)
    {
      HashSet<EndPathToken> generatedProperties = (HashSet<EndPathToken>) null;
      if (applyOption != null)
        generatedProperties = applyOption.GetLastAggregatedPropertyNames();
      if (computeOption != null)
      {
        HashSet<EndPathToken> other = new HashSet<EndPathToken>(computeOption.ComputedItems.Select<ComputeExpression, EndPathToken>((Func<ComputeExpression, EndPathToken>) (i => new EndPathToken(i.Alias, (QueryToken) null))));
        if (generatedProperties == null)
          generatedProperties = other;
        else
          generatedProperties.UnionWith((IEnumerable<EndPathToken>) other);
      }
      return generatedProperties;
    }

    private IEdmNavigationProperty ParseComplexTypesBeforeNavigation(
      IEdmStructuralProperty edmProperty,
      ref PathSegmentToken currentToken,
      List<ODataPathSegment> pathSoFar)
    {
      pathSoFar.Add((ODataPathSegment) new PropertySegment(edmProperty));
      if (currentToken.NextToken == null)
        throw new ODataException(Microsoft.OData.Strings.ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty((object) currentToken.Identifier, (object) edmProperty.DeclaringType.FullTypeName()));
      currentToken = currentToken.NextToken;
      IEdmType definition = edmProperty.Type.Definition;
      if (definition is IEdmCollectionType edmCollectionType)
        definition = edmCollectionType.ElementType.Definition;
      if (!(definition is IEdmStructuredType currentLevelType))
        throw new ODataException(Microsoft.OData.Strings.ExpandItemBinder_InvaidSegmentInExpand((object) currentToken.Identifier));
      if (currentToken.IsNamespaceOrContainerQualified())
        pathSoFar.AddRange(SelectExpandPathBinder.FollowTypeSegments(currentToken, this.Model, this.Settings.SelectExpandLimit, this.configuration.Resolver, ref currentLevelType, out currentToken));
      IEdmProperty edmProperty1 = this.configuration.Resolver.ResolveProperty(currentLevelType, currentToken.Identifier);
      if (edmProperty == null)
        throw ExceptionUtil.CreatePropertyNotFoundException(currentToken.Identifier, this.edmType.FullTypeName());
      if (edmProperty1 is IEdmStructuralProperty edmProperty2)
        edmProperty1 = (IEdmProperty) this.ParseComplexTypesBeforeNavigation(edmProperty2, ref currentToken, pathSoFar);
      return edmProperty1 is IEdmNavigationProperty navigationProperty ? navigationProperty : throw new ODataException(Microsoft.OData.Strings.ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty((object) currentToken.Identifier, (object) currentLevelType.FullTypeName()));
    }

    private static LevelsClause ParseLevels(
      long? levelsOption,
      IEdmType sourceType,
      IEdmNavigationProperty property)
    {
      if (!levelsOption.HasValue)
        return (LevelsClause) null;
      IEdmType entityType = (IEdmType) property.ToEntityType();
      if (sourceType != null && entityType != null && !UriEdmHelpers.IsRelatedTo(sourceType, entityType))
        throw new ODataException(Microsoft.OData.Strings.ExpandItemBinder_LevelsNotAllowedOnIncompatibleRelatedType((object) property.Name, (object) entityType.FullTypeName(), (object) sourceType.FullTypeName()));
      return new LevelsClause(levelsOption.Value < 0L, levelsOption.Value);
    }

    private static MetadataBinder BuildNewMetadataBinder(
      ODataUriParserConfiguration config,
      IEdmNavigationSource targetNavigationSource,
      IEdmTypeReference elementType,
      HashSet<EndPathToken> generatedProperties = null,
      bool collapsed = false)
    {
      return new MetadataBinder(SelectExpandBinder.CreateBindingState(config, targetNavigationSource, elementType, generatedProperties, collapsed));
    }

    private static BindingState CreateBindingState(
      ODataUriParserConfiguration config,
      IEdmNavigationSource targetNavigationSource,
      IEdmTypeReference elementType,
      HashSet<EndPathToken> generatedProperties = null,
      bool collapsed = false)
    {
      if (targetNavigationSource == null && elementType == null)
        return (BindingState) null;
      BindingState bindingState = new BindingState(config)
      {
        ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(elementType != null ? elementType : targetNavigationSource.EntityType().ToTypeReference(), targetNavigationSource)
      };
      bindingState.RangeVariables.Push(bindingState.ImplicitRangeVariable);
      bindingState.AggregatedPropertyNames = generatedProperties;
      bindingState.IsCollapsed = collapsed;
      return bindingState;
    }

    private static void VerifySelectedPath(SelectTermToken selectedToken)
    {
      for (PathSegmentToken pathSegmentToken = selectedToken.PathToProperty; pathSegmentToken != null; pathSegmentToken = pathSegmentToken.NextToken)
      {
        if (pathSegmentToken is SystemToken)
          throw new ODataException(Microsoft.OData.Strings.SelectExpandBinder_SystemTokenInSelect((object) pathSegmentToken.Identifier));
      }
    }

    private static bool VerifySelectedNavigationProperty(
      IList<ODataPathSegment> selectedPath,
      SelectTermToken tokenIn)
    {
      if (!(selectedPath.LastOrDefault<ODataPathSegment>() is NavigationPropertySegment navigationPropertySegment))
        return false;
      SelectExpandBinder.VerifyNoQueryOptionsNested(tokenIn, navigationPropertySegment.Identifier);
      return true;
    }

    private static void VerifyNoQueryOptionsNested(SelectTermToken selectToken, string identifier)
    {
      if (selectToken != null && (selectToken.ComputeOption != null || selectToken.FilterOption != null || selectToken.OrderByOptions != null || selectToken.SearchOption != null || selectToken.CountQueryOption.HasValue || selectToken.SelectOption != null || selectToken.TopOption.HasValue || selectToken.SkipOption.HasValue))
        throw new ODataException(Microsoft.OData.Strings.SelectExpandBinder_InvalidQueryOptionNestedSelection((object) identifier));
    }

    internal static void AddToSelectedItems(SelectItem itemToAdd, List<SelectItem> selectItems)
    {
      if (itemToAdd == null || selectItems.Any<SelectItem>((Func<SelectItem, bool>) (x => x is WildcardSelectItem)) && SelectExpandBinder.IsStructuralOrNavigationPropertySelectionItem(itemToAdd))
        return;
      PathSelectItem pathSelectItem = itemToAdd as PathSelectItem;
      if (pathSelectItem != null && pathSelectItem.SelectedPath.LastSegment is NavigationPropertySegment && selectItems.OfType<PathSelectItem>().Any<PathSelectItem>((Func<PathSelectItem, bool>) (i => i.SelectedPath.Equals((ODataPath) pathSelectItem.SelectedPath))))
        return;
      if (itemToAdd is WildcardSelectItem)
      {
        foreach (SelectItem selectItem in selectItems.Where<SelectItem>((Func<SelectItem, bool>) (s => SelectExpandBinder.IsStructuralSelectionItem(s))).ToList<SelectItem>())
          selectItems.Remove(selectItem);
      }
      selectItems.Add(itemToAdd);
    }

    private static bool IsStructuralOrNavigationPropertySelectionItem(SelectItem selectItem)
    {
      if (!(selectItem is PathSelectItem pathSelectItem))
        return false;
      return pathSelectItem.SelectedPath.LastSegment is NavigationPropertySegment || pathSelectItem.SelectedPath.LastSegment is PropertySegment;
    }

    private static bool IsStructuralSelectionItem(SelectItem selectItem) => selectItem is PathSelectItem pathSelectItem && pathSelectItem.SelectedPath.LastSegment is PropertySegment;
  }
}
