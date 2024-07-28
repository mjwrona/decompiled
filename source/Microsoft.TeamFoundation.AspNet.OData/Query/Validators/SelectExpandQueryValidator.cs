// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Validators.SelectExpandQueryValidator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Query.Validators
{
  public class SelectExpandQueryValidator
  {
    private readonly DefaultQuerySettings _defaultQuerySettings;
    private readonly FilterQueryValidator _filterQueryValidator;
    private OrderByModelLimitationsValidator _orderByQueryValidator;
    private SelectExpandQueryOption _selectExpandQueryOption;

    public SelectExpandQueryValidator(DefaultQuerySettings defaultQuerySettings)
    {
      this._defaultQuerySettings = defaultQuerySettings;
      this._filterQueryValidator = new FilterQueryValidator(this._defaultQuerySettings);
    }

    public virtual void Validate(
      SelectExpandQueryOption selectExpandQueryOption,
      ODataValidationSettings validationSettings)
    {
      if (selectExpandQueryOption == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (selectExpandQueryOption));
      if (validationSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (validationSettings));
      this._orderByQueryValidator = new OrderByModelLimitationsValidator(selectExpandQueryOption.Context, this._defaultQuerySettings.EnableOrderBy);
      this._selectExpandQueryOption = selectExpandQueryOption;
      this.ValidateRestrictions(new int?(), 0, selectExpandQueryOption.SelectExpandClause, (IEdmNavigationProperty) null, validationSettings);
      if (validationSettings.MaxExpansionDepth <= 0)
        return;
      if (selectExpandQueryOption.LevelsMaxLiteralExpansionDepth < 0)
        selectExpandQueryOption.LevelsMaxLiteralExpansionDepth = validationSettings.MaxExpansionDepth;
      else if (selectExpandQueryOption.LevelsMaxLiteralExpansionDepth > validationSettings.MaxExpansionDepth)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.InvalidExpansionDepthValue, (object) "LevelsMaxLiteralExpansionDepth", (object) "MaxExpansionDepth"));
      SelectExpandQueryValidator.ValidateDepth(selectExpandQueryOption.SelectExpandClause, validationSettings.MaxExpansionDepth);
    }

    internal static SelectExpandQueryValidator GetSelectExpandQueryValidator(
      ODataQueryContext context)
    {
      if (context == null)
        return new SelectExpandQueryValidator(new DefaultQuerySettings());
      return context.RequestContainer != null ? ServiceProviderServiceExtensions.GetRequiredService<SelectExpandQueryValidator>(context.RequestContainer) : new SelectExpandQueryValidator(context.DefaultQuerySettings);
    }

    private static void ValidateDepth(SelectExpandClause selectExpand, int maxDepth)
    {
      Stack<Tuple<int, SelectExpandClause>> tupleStack = new Stack<Tuple<int, SelectExpandClause>>();
      tupleStack.Push(Tuple.Create<int, SelectExpandClause>(0, selectExpand));
      while (tupleStack.Count > 0)
      {
        Tuple<int, SelectExpandClause> tuple = tupleStack.Pop();
        int currentDepth = tuple.Item1;
        ExpandedNavigationSelectItem[] array = tuple.Item2.SelectedItems.OfType<ExpandedNavigationSelectItem>().ToArray<ExpandedNavigationSelectItem>();
        if (array.Length != 0 && (currentDepth == maxDepth && ((IEnumerable<ExpandedNavigationSelectItem>) array).Any<ExpandedNavigationSelectItem>((Func<ExpandedNavigationSelectItem, bool>) (expandItem => expandItem.LevelsOption == null || expandItem.LevelsOption.IsMaxLevel || expandItem.LevelsOption.Level != 0L)) || ((IEnumerable<ExpandedNavigationSelectItem>) array).Any<ExpandedNavigationSelectItem>((Func<ExpandedNavigationSelectItem, bool>) (expandItem =>
        {
          if (expandItem.LevelsOption == null || expandItem.LevelsOption.IsMaxLevel)
            return false;
          return expandItem.LevelsOption.Level > (long) int.MaxValue || expandItem.LevelsOption.Level + (long) currentDepth > (long) maxDepth;
        }))))
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.MaxExpandDepthExceeded, (object) maxDepth, (object) "MaxExpansionDepth"));
        foreach (ExpandedNavigationSelectItem navigationSelectItem in array)
        {
          int num = currentDepth + 1;
          if (navigationSelectItem.LevelsOption != null && !navigationSelectItem.LevelsOption.IsMaxLevel)
            num = num + (int) navigationSelectItem.LevelsOption.Level - 1;
          tupleStack.Push(Tuple.Create<int, SelectExpandClause>(num, navigationSelectItem.SelectAndExpand));
        }
      }
    }

    private void ValidateTopInExpand(
      IEdmProperty property,
      IEdmStructuredType structuredType,
      IEdmModel edmModel,
      long? topOption)
    {
      int maxTop;
      if (topOption.HasValue && EdmLibHelpers.IsTopLimitExceeded(property, structuredType, edmModel, (int) topOption.Value, this._defaultQuerySettings, out maxTop))
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.SkipTopLimitExceeded, (object) maxTop, (object) AllowedQueryOptions.Top, (object) topOption.Value));
    }

    private void ValidateCountInExpand(
      IEdmProperty property,
      IEdmStructuredType structuredType,
      IEdmModel edmModel,
      bool? countOption)
    {
      bool? nullable = countOption;
      bool flag = true;
      if (nullable.GetValueOrDefault() == flag & nullable.HasValue && EdmLibHelpers.IsNotCountable(property, structuredType, edmModel, this._defaultQuerySettings.EnableCount))
        throw new InvalidOperationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotCountablePropertyUsedForCount, (object) property.Name));
    }

    private void ValidateOrderByInExpand(
      IEdmProperty property,
      IEdmStructuredType structuredType,
      OrderByClause orderByClause)
    {
      if (orderByClause == null)
        return;
      this._orderByQueryValidator.TryValidate(property, structuredType, orderByClause, false);
    }

    private void ValidateFilterInExpand(
      IEdmProperty property,
      IEdmStructuredType structuredType,
      IEdmModel edmModel,
      FilterClause filterClause,
      ODataValidationSettings validationSettings)
    {
      if (filterClause == null)
        return;
      this._filterQueryValidator.Validate(property, structuredType, filterClause, validationSettings, edmModel);
    }

    private void ValidateSelectItem(
      SelectItem selectItem,
      IEdmProperty pathProperty,
      IEdmStructuredType pathStructuredType,
      IEdmModel edmModel)
    {
      switch (selectItem)
      {
        case PathSelectItem pathSelectItem:
          switch (pathSelectItem.SelectedPath.LastSegment)
          {
            case NavigationPropertySegment navigationPropertySegment:
              IEdmNavigationProperty navigationProperty = navigationPropertySegment.NavigationProperty;
              if (!EdmLibHelpers.IsNotNavigable((IEdmProperty) navigationProperty, edmModel))
                return;
              throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotNavigablePropertyUsedInNavigation, (object) navigationProperty.Name));
            case PropertySegment propertySegment:
              if (!EdmLibHelpers.IsNotSelectable((IEdmProperty) propertySegment.Property, pathProperty, pathStructuredType, edmModel, this._defaultQuerySettings.EnableSelect))
                return;
              throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotSelectablePropertyUsedInSelect, (object) propertySegment.Property.Name));
            default:
              return;
          }
        case WildcardSelectItem _:
          using (IEnumerator<IEdmStructuralProperty> enumerator = pathStructuredType.StructuralProperties().GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IEdmStructuralProperty current = enumerator.Current;
              if (EdmLibHelpers.IsNotSelectable((IEdmProperty) current, pathProperty, pathStructuredType, edmModel, this._defaultQuerySettings.EnableSelect))
                throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotSelectablePropertyUsedInSelect, (object) current.Name));
            }
            break;
          }
      }
    }

    private void ValidateLevelsOption(
      LevelsClause levelsClause,
      int depth,
      int currentDepth,
      IEdmModel edmModel,
      IEdmNavigationProperty property,
      bool isRoot)
    {
      ExpandConfiguration expandConfiguration;
      if (EdmLibHelpers.IsExpandable(property.Name, (IEdmProperty) property, (IEdmStructuredType) property.ToEntityType(), edmModel, out expandConfiguration))
      {
        int maxDepth = expandConfiguration.MaxDepth;
        if (maxDepth > 0 && maxDepth < depth)
          depth = maxDepth;
        if (depth == 0 && levelsClause.IsMaxLevel || (long) depth < levelsClause.Level)
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.MaxExpandDepthExceeded, (object) (currentDepth + depth - (isRoot ? 1 : 0)), (object) "MaxExpansionDepth"));
      }
      else if (!this._defaultQuerySettings.EnableExpand || expandConfiguration != null && expandConfiguration.ExpandType == SelectExpandType.Disabled)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotExpandablePropertyUsedInExpand, (object) property.Name));
    }

    private void ValidateOtherQueryOptionInExpand(
      IEdmNavigationProperty property,
      IEdmModel edmModel,
      ExpandedNavigationSelectItem expandItem,
      ODataValidationSettings validationSettings)
    {
      this.ValidateTopInExpand((IEdmProperty) property, (IEdmStructuredType) property.ToEntityType(), edmModel, expandItem.TopOption);
      this.ValidateCountInExpand((IEdmProperty) property, (IEdmStructuredType) property.ToEntityType(), edmModel, expandItem.CountOption);
      this.ValidateOrderByInExpand((IEdmProperty) property, (IEdmStructuredType) property.ToEntityType(), expandItem.OrderByOption);
      this.ValidateFilterInExpand((IEdmProperty) property, (IEdmStructuredType) property.ToEntityType(), edmModel, expandItem.FilterOption, validationSettings);
    }

    private void ValidateRestrictions(
      int? remainDepth,
      int currentDepth,
      SelectExpandClause selectExpandClause,
      IEdmNavigationProperty navigationProperty,
      ODataValidationSettings validationSettings)
    {
      IEdmModel model = this._selectExpandQueryOption.Context.Model;
      int? nullable1 = remainDepth;
      int? nullable2 = remainDepth;
      int num1 = 0;
      if (nullable2.GetValueOrDefault() < num1 & nullable2.HasValue)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.MaxExpandDepthExceeded, (object) (currentDepth - 1), (object) "MaxExpansionDepth"));
      IEdmProperty edmProperty;
      IEdmStructuredType edmStructuredType;
      if (navigationProperty == null)
      {
        edmProperty = this._selectExpandQueryOption.Context.TargetProperty;
        edmStructuredType = this._selectExpandQueryOption.Context.TargetStructuredType;
      }
      else
      {
        edmProperty = (IEdmProperty) navigationProperty;
        edmStructuredType = (IEdmStructuredType) navigationProperty.ToEntityType();
      }
      foreach (SelectItem selectedItem in selectExpandClause.SelectedItems)
      {
        if (selectedItem is ExpandedNavigationSelectItem expandItem)
        {
          IEdmNavigationProperty navigationProperty1 = ((NavigationPropertySegment) expandItem.PathToNavigationProperty.LastSegment).NavigationProperty;
          if (EdmLibHelpers.IsNotExpandable((IEdmProperty) navigationProperty1, model))
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotExpandablePropertyUsedInExpand, (object) navigationProperty1.Name));
          int? nullable3;
          if (model != null)
          {
            this.ValidateOtherQueryOptionInExpand(navigationProperty1, model, expandItem, validationSettings);
            ExpandConfiguration expandConfiguration;
            bool flag = EdmLibHelpers.IsExpandable(navigationProperty1.Name, edmProperty, edmStructuredType, model, out expandConfiguration);
            if (flag)
            {
              int maxDepth = expandConfiguration.MaxDepth;
              if (maxDepth > 0)
              {
                if (remainDepth.HasValue)
                {
                  int num2 = maxDepth;
                  nullable3 = remainDepth;
                  int valueOrDefault = nullable3.GetValueOrDefault();
                  if (!(num2 < valueOrDefault & nullable3.HasValue))
                    goto label_18;
                }
                remainDepth = new int?(maxDepth);
              }
            }
            else if (!flag && (!this._defaultQuerySettings.EnableExpand || expandConfiguration != null && expandConfiguration.ExpandType == SelectExpandType.Disabled))
              throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotExpandablePropertyUsedInExpand, (object) navigationProperty1.Name));
          }
label_18:
          if (remainDepth.HasValue)
          {
            if (expandItem.LevelsOption != null)
              this.ValidateLevelsOption(expandItem.LevelsOption, remainDepth.Value, currentDepth + 1, model, navigationProperty1, !nullable1.HasValue);
            nullable3 = remainDepth;
            remainDepth = nullable3.HasValue ? new int?(nullable3.GetValueOrDefault() - 1) : new int?();
          }
          this.ValidateRestrictions(remainDepth, currentDepth + 1, expandItem.SelectAndExpand, navigationProperty1, validationSettings);
          remainDepth = nullable1;
        }
        this.ValidateSelectItem(selectedItem, edmProperty, edmStructuredType, model);
      }
    }
  }
}
