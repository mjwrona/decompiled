// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.SelectExpandQueryOption
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Query
{
  public class SelectExpandQueryOption
  {
    private SelectExpandClause _selectExpandClause;
    private ODataQueryOptionParser _queryOptionParser;
    private SelectExpandClause _processedSelectExpandClause;
    private int _levelsMaxLiteralExpansionDepth = -1;

    public SelectExpandQueryOption(
      string select,
      string expand,
      ODataQueryContext context,
      ODataQueryOptionParser queryOptionParser)
    {
      if (context == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (context));
      if (string.IsNullOrEmpty(select) && string.IsNullOrEmpty(expand))
        throw Microsoft.AspNet.OData.Common.Error.Argument(SRResources.SelectExpandEmptyOrNull);
      if (queryOptionParser == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (queryOptionParser));
      this.Context = context.ElementType is IEdmStructuredType ? context : throw Microsoft.AspNet.OData.Common.Error.Argument(SRResources.SelectNonStructured, (object) context.ElementType);
      this.RawSelect = select;
      this.RawExpand = expand;
      this.Validator = SelectExpandQueryValidator.GetSelectExpandQueryValidator(context);
      this._queryOptionParser = queryOptionParser;
    }

    internal SelectExpandQueryOption(
      string select,
      string expand,
      ODataQueryContext context,
      SelectExpandClause selectExpandClause)
      : this(select, expand, context)
    {
      this._selectExpandClause = selectExpandClause;
    }

    internal SelectExpandQueryOption(string select, string expand, ODataQueryContext context)
    {
      if (context == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (context));
      if (string.IsNullOrEmpty(select) && string.IsNullOrEmpty(expand))
        throw Microsoft.AspNet.OData.Common.Error.Argument(SRResources.SelectExpandEmptyOrNull);
      this.Context = context.ElementType is IEdmStructuredType ? context : throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (context), SRResources.SelectNonStructured, (object) context.ElementType.ToTraceString());
      this.RawSelect = select;
      this.RawExpand = expand;
      this.Validator = SelectExpandQueryValidator.GetSelectExpandQueryValidator(context);
      IEdmModel model = context.Model;
      IEdmType elementType = context.ElementType;
      IEdmNavigationSource navigationSource = context.NavigationSource;
      Dictionary<string, string> queryOptions = new Dictionary<string, string>();
      queryOptions.Add("$select", select);
      queryOptions.Add("$expand", expand);
      IServiceProvider requestContainer = context.RequestContainer;
      this._queryOptionParser = new ODataQueryOptionParser(model, elementType, navigationSource, (IDictionary<string, string>) queryOptions, requestContainer);
    }

    public ODataQueryContext Context { get; private set; }

    public string RawSelect { get; private set; }

    public string RawExpand { get; private set; }

    public SelectExpandQueryValidator Validator { get; set; }

    public SelectExpandClause SelectExpandClause
    {
      get
      {
        if (this._selectExpandClause == null)
          this._selectExpandClause = this._queryOptionParser.ParseSelectAndExpand();
        return this._selectExpandClause;
      }
    }

    internal SelectExpandClause ProcessedSelectExpandClause
    {
      get
      {
        if (this._processedSelectExpandClause != null)
          return this._processedSelectExpandClause;
        this._processedSelectExpandClause = this.ProcessLevels();
        return this._processedSelectExpandClause;
      }
    }

    public int LevelsMaxLiteralExpansionDepth
    {
      get => this._levelsMaxLiteralExpansionDepth;
      set => this._levelsMaxLiteralExpansionDepth = value >= 0 ? value : throw Microsoft.AspNet.OData.Common.Error.ArgumentMustBeGreaterThanOrEqualTo(nameof (LevelsMaxLiteralExpansionDepth), (object) value, (object) 0);
    }

    public IQueryable ApplyTo(IQueryable queryable, ODataQuerySettings settings)
    {
      if (queryable == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (queryable));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      if (this.Context.ElementClrType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.ApplyToOnUntypedQueryOption, (object) nameof (ApplyTo));
      ODataQuerySettings settings1 = this.Context.UpdateQuerySettings(settings, queryable);
      return Microsoft.AspNet.OData.Query.Expressions.SelectExpandBinder.Bind(queryable, settings1, this);
    }

    public object ApplyTo(object entity, ODataQuerySettings settings)
    {
      if (entity == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entity));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      ODataQuerySettings settings1 = !(this.Context.ElementClrType == (Type) null) ? this.Context.UpdateQuerySettings(settings, (IQueryable) null) : throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.ApplyToOnUntypedQueryOption, (object) nameof (ApplyTo));
      return Microsoft.AspNet.OData.Query.Expressions.SelectExpandBinder.Bind(entity, settings1, this);
    }

    public void Validate(ODataValidationSettings validationSettings)
    {
      if (validationSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (validationSettings));
      if (this.Validator == null)
        return;
      this.Validator.Validate(this, validationSettings);
    }

    internal SelectExpandClause ProcessLevels()
    {
      ModelBoundQuerySettings boundQuerySettings = EdmLibHelpers.GetModelBoundQuerySettings(this.Context.TargetProperty, this.Context.TargetStructuredType, this.Context.Model, this.Context.DefaultQuerySettings);
      return this.ProcessLevels(this.SelectExpandClause, this.LevelsMaxLiteralExpansionDepth < 0 ? 2 : this.LevelsMaxLiteralExpansionDepth, boundQuerySettings, out bool _, out bool _);
    }

    private SelectExpandClause ProcessLevels(
      SelectExpandClause selectExpandClause,
      int levelsMaxLiteralExpansionDepth,
      ModelBoundQuerySettings querySettings,
      out bool levelsEncountered,
      out bool isMaxLevel)
    {
      levelsEncountered = false;
      isMaxLevel = false;
      if (selectExpandClause == null)
        return (SelectExpandClause) null;
      IEnumerable<SelectItem> selectedItems = this.ProcessLevels(selectExpandClause.SelectedItems, levelsMaxLiteralExpansionDepth, querySettings, out levelsEncountered, out isMaxLevel);
      if (selectedItems == null)
        return (SelectExpandClause) null;
      return levelsEncountered ? new SelectExpandClause(selectedItems, selectExpandClause.AllSelected, selectExpandClause.AllAutoSelected) : selectExpandClause;
    }

    private IEnumerable<SelectItem> ProcessLevels(
      IEnumerable<SelectItem> selectItems,
      int levelsMaxLiteralExpansionDepth,
      ModelBoundQuerySettings querySettings,
      out bool levelsEncountered,
      out bool isMaxLevel)
    {
      levelsEncountered = false;
      isMaxLevel = false;
      IList<SelectItem> selectItemList = (IList<SelectItem>) new List<SelectItem>();
      foreach (SelectItem selectItem in selectItems)
      {
        if (!(selectItem is ExpandedNavigationSelectItem expandItem))
        {
          selectItemList.Add(selectItem);
        }
        else
        {
          bool levelsEncounteredInExpand;
          bool isMaxLevelInExpand;
          ExpandedNavigationSelectItem navigationSelectItem = this.ProcessLevels(expandItem, levelsMaxLiteralExpansionDepth, querySettings, out levelsEncounteredInExpand, out isMaxLevelInExpand);
          if (expandItem.LevelsOption != null && expandItem.LevelsOption.Level > 0L && navigationSelectItem == null)
            return (IEnumerable<SelectItem>) null;
          if (expandItem.LevelsOption != null)
            isMaxLevel |= isMaxLevelInExpand;
          levelsEncountered |= levelsEncounteredInExpand;
          if (navigationSelectItem != null)
            selectItemList.Add((SelectItem) navigationSelectItem);
        }
      }
      return (IEnumerable<SelectItem>) selectItemList;
    }

    private void GetAutoSelectExpandItems(
      IEdmEntityType baseEntityType,
      IEdmModel model,
      IEdmNavigationSource navigationSource,
      bool isAllSelected,
      ModelBoundQuerySettings modelBoundQuerySettings,
      int depth,
      out List<SelectItem> autoSelectItems,
      out List<SelectItem> autoExpandItems)
    {
      autoSelectItems = new List<SelectItem>();
      IEnumerable<IEdmStructuralProperty> selectProperties = EdmLibHelpers.GetAutoSelectProperties((IEdmProperty) null, (IEdmStructuredType) baseEntityType, model, modelBoundQuerySettings);
      foreach (IEdmStructuralProperty property in selectProperties)
      {
        PathSelectItem pathSelectItem = new PathSelectItem(new ODataSelectPath((IEnumerable<ODataPathSegment>) new List<ODataPathSegment>()
        {
          (ODataPathSegment) new PropertySegment(property)
        }));
        autoSelectItems.Add((SelectItem) pathSelectItem);
      }
      autoExpandItems = new List<SelectItem>();
      --depth;
      if (depth < 0)
        return;
      foreach (IEdmNavigationProperty navigationProperty in EdmLibHelpers.GetAutoExpandNavigationProperties((IEdmProperty) null, (IEdmStructuredType) baseEntityType, model, !isAllSelected, modelBoundQuerySettings))
      {
        IEdmNavigationSource navigationTarget = navigationSource.FindNavigationTarget(navigationProperty);
        if (navigationTarget != null)
        {
          List<ODataPathSegment> segments = new List<ODataPathSegment>()
          {
            (ODataPathSegment) new NavigationPropertySegment(navigationProperty, navigationTarget)
          };
          ODataExpandPath pathToNavigationProperty = new ODataExpandPath((IEnumerable<ODataPathSegment>) segments);
          SelectExpandClause selectExpandOption1 = new SelectExpandClause((IEnumerable<SelectItem>) new List<SelectItem>(), true);
          ExpandedNavigationSelectItem navigationSelectItem1 = new ExpandedNavigationSelectItem(pathToNavigationProperty, navigationTarget, selectExpandOption1);
          modelBoundQuerySettings = EdmLibHelpers.GetModelBoundQuerySettings((IEdmProperty) navigationProperty, (IEdmStructuredType) navigationProperty.ToEntityType(), model);
          int maxExpandDepth = SelectExpandQueryOption.GetMaxExpandDepth(modelBoundQuerySettings, navigationProperty.Name);
          if (maxExpandDepth != 0 && maxExpandDepth < depth)
            depth = maxExpandDepth;
          List<SelectItem> autoSelectItems1;
          List<SelectItem> autoExpandItems1;
          this.GetAutoSelectExpandItems(navigationTarget.EntityType(), model, navigationSelectItem1.NavigationSource, true, modelBoundQuerySettings, depth, out autoSelectItems1, out autoExpandItems1);
          SelectExpandClause selectExpandOption2 = new SelectExpandClause(autoSelectItems1.Concat<SelectItem>((IEnumerable<SelectItem>) autoExpandItems1), autoSelectItems1.Count == 0);
          ExpandedNavigationSelectItem navigationSelectItem2 = new ExpandedNavigationSelectItem(pathToNavigationProperty, navigationTarget, selectExpandOption2);
          autoExpandItems.Add((SelectItem) navigationSelectItem2);
          if (!isAllSelected || selectProperties.Count<IEdmStructuralProperty>() != 0)
          {
            PathSelectItem pathSelectItem = new PathSelectItem(new ODataSelectPath((IEnumerable<ODataPathSegment>) segments));
            autoExpandItems.Add((SelectItem) pathSelectItem);
          }
        }
      }
    }

    private ExpandedNavigationSelectItem ProcessLevels(
      ExpandedNavigationSelectItem expandItem,
      int levelsMaxLiteralExpansionDepth,
      ModelBoundQuerySettings querySettings,
      out bool levelsEncounteredInExpand,
      out bool isMaxLevelInExpand)
    {
      isMaxLevelInExpand = false;
      int num1;
      if (expandItem.LevelsOption == null)
      {
        levelsEncounteredInExpand = false;
        num1 = 1;
      }
      else
      {
        levelsEncounteredInExpand = true;
        if (expandItem.LevelsOption.IsMaxLevel)
        {
          isMaxLevelInExpand = true;
          num1 = levelsMaxLiteralExpansionDepth;
        }
        else
          num1 = (int) expandItem.LevelsOption.Level;
      }
      if (num1 <= 0 || num1 > levelsMaxLiteralExpansionDepth)
        return (ExpandedNavigationSelectItem) null;
      ExpandedNavigationSelectItem navigationSelectItem = (ExpandedNavigationSelectItem) null;
      SelectExpandClause selectExpandClause = (SelectExpandClause) null;
      bool levelsEncountered = false;
      bool isMaxLevel = false;
      IEdmEntityType edmEntityType = expandItem.NavigationSource.EntityType();
      IEdmNavigationProperty navigationProperty = (expandItem.PathToNavigationProperty.LastSegment as NavigationPropertySegment).NavigationProperty;
      ModelBoundQuerySettings boundQuerySettings1 = EdmLibHelpers.GetModelBoundQuerySettings((IEdmProperty) navigationProperty, (IEdmStructuredType) navigationProperty.ToEntityType(), this.Context.Model);
      for (; selectExpandClause == null && num1 > 0; --num1)
        selectExpandClause = this.ProcessLevels(expandItem.SelectAndExpand, levelsMaxLiteralExpansionDepth - num1, boundQuerySettings1, out levelsEncountered, out isMaxLevel);
      if (selectExpandClause == null)
        return (ExpandedNavigationSelectItem) null;
      int num2 = num1 + 1;
      int num3 = SelectExpandQueryOption.GetMaxExpandDepth(querySettings, navigationProperty.Name);
      if (num3 == 0 || levelsMaxLiteralExpansionDepth > num3)
        num3 = levelsMaxLiteralExpansionDepth;
      List<SelectItem> autoSelectItems;
      List<SelectItem> autoExpandItems;
      this.GetAutoSelectExpandItems(edmEntityType, this.Context.Model, expandItem.NavigationSource, selectExpandClause.AllSelected, boundQuerySettings1, num3 - 1, out autoSelectItems, out autoExpandItems);
      if (expandItem.SelectAndExpand.SelectedItems.Any<SelectItem>((Func<SelectItem, bool>) (it => it is PathSelectItem)))
        autoSelectItems.Clear();
      if (num2 > 1)
        SelectExpandQueryOption.RemoveSameExpandItem(navigationProperty, autoExpandItems);
      List<SelectItem> selectItemList = new List<SelectItem>((IEnumerable<SelectItem>) autoExpandItems);
      bool flag = autoSelectItems.Count<SelectItem>() + autoExpandItems.Count<SelectItem>() != 0;
      bool allSelected = autoSelectItems.Count == 0 && selectExpandClause.AllSelected;
      ModelBoundQuerySettings boundQuerySettings2 = EdmLibHelpers.GetModelBoundQuerySettings<IEdmEntityType>(edmEntityType, this.Context.Model, this.Context.DefaultQuerySettings);
      int num4;
      if (boundQuerySettings2 != null)
      {
        SelectExpandType? defaultSelectType = boundQuerySettings2.DefaultSelectType;
        SelectExpandType selectExpandType = SelectExpandType.Automatic;
        num4 = defaultSelectType.GetValueOrDefault() == selectExpandType & defaultSelectType.HasValue ? 1 : 0;
      }
      else
        num4 = 0;
      bool allAutoSelected = num4 != 0;
      while (num2 > 0)
      {
        List<SelectItem> second1 = SelectExpandQueryOption.RemoveExpandItemExceedMaxDepth(num3 - num2, (IEnumerable<SelectItem>) autoExpandItems);
        SelectExpandClause selectAndExpand;
        if (navigationSelectItem == null)
          selectAndExpand = !flag ? selectExpandClause : new SelectExpandClause(((IEnumerable<SelectItem>) new SelectItem[0]).Concat<SelectItem>(selectExpandClause.SelectedItems).Concat<SelectItem>((IEnumerable<SelectItem>) autoSelectItems).Concat<SelectItem>((IEnumerable<SelectItem>) second1), allSelected, allAutoSelected);
        else if (selectExpandClause.AllSelected)
        {
          selectAndExpand = new SelectExpandClause(((IEnumerable<SelectItem>) new SelectItem[1]
          {
            (SelectItem) navigationSelectItem
          }).Concat<SelectItem>(selectExpandClause.SelectedItems).Concat<SelectItem>((IEnumerable<SelectItem>) autoSelectItems).Concat<SelectItem>((IEnumerable<SelectItem>) second1), allSelected, allAutoSelected);
        }
        else
        {
          PathSelectItem pathSelectItem = new PathSelectItem(new ODataSelectPath((IEnumerable<ODataPathSegment>) expandItem.PathToNavigationProperty));
          SelectItem[] second2 = new SelectItem[2]
          {
            (SelectItem) navigationSelectItem,
            (SelectItem) pathSelectItem
          };
          selectAndExpand = new SelectExpandClause(((IEnumerable<SelectItem>) new SelectItem[0]).Concat<SelectItem>(selectExpandClause.SelectedItems).Concat<SelectItem>((IEnumerable<SelectItem>) second2).Concat<SelectItem>((IEnumerable<SelectItem>) autoSelectItems).Concat<SelectItem>((IEnumerable<SelectItem>) second1), allSelected, allAutoSelected);
        }
        navigationSelectItem = new ExpandedNavigationSelectItem(expandItem.PathToNavigationProperty, expandItem.NavigationSource, selectAndExpand, expandItem.FilterOption, expandItem.OrderByOption, expandItem.TopOption, expandItem.SkipOption, expandItem.CountOption, expandItem.SearchOption, (LevelsClause) null, expandItem.ComputeOption, expandItem.ApplyOption);
        --num2;
        if (isMaxLevel)
          selectExpandClause = this.ProcessLevels(expandItem.SelectAndExpand, levelsMaxLiteralExpansionDepth - num2, boundQuerySettings1, out levelsEncountered, out isMaxLevel);
      }
      levelsEncounteredInExpand = levelsEncounteredInExpand | levelsEncountered | flag;
      isMaxLevelInExpand |= isMaxLevel;
      return navigationSelectItem;
    }

    private static List<SelectItem> RemoveExpandItemExceedMaxDepth(
      int depth,
      IEnumerable<SelectItem> autoExpandItems)
    {
      List<SelectItem> selectItemList = new List<SelectItem>();
      if (depth <= 0)
      {
        foreach (SelectItem autoExpandItem in autoExpandItems)
        {
          if (!(autoExpandItem is ExpandedNavigationSelectItem))
            selectItemList.Add(autoExpandItem);
        }
      }
      else
      {
        foreach (SelectItem autoExpandItem in autoExpandItems)
        {
          if (autoExpandItem is ExpandedNavigationSelectItem navigationSelectItem1)
          {
            SelectExpandClause selectExpandOption = new SelectExpandClause((IEnumerable<SelectItem>) SelectExpandQueryOption.RemoveExpandItemExceedMaxDepth(depth - 1, navigationSelectItem1.SelectAndExpand.SelectedItems), navigationSelectItem1.SelectAndExpand.AllSelected);
            ExpandedNavigationSelectItem navigationSelectItem = new ExpandedNavigationSelectItem(navigationSelectItem1.PathToNavigationProperty, navigationSelectItem1.NavigationSource, selectExpandOption);
            selectItemList.Add((SelectItem) navigationSelectItem);
          }
          else
            selectItemList.Add(autoExpandItem);
        }
      }
      return selectItemList;
    }

    private static void RemoveSameExpandItem(
      IEdmNavigationProperty navigationProperty,
      List<SelectItem> autoExpandItems)
    {
      for (int index = 0; index < autoExpandItems.Count; ++index)
      {
        IEdmNavigationProperty navigationProperty1 = ((autoExpandItems[index] as ExpandedNavigationSelectItem).PathToNavigationProperty.LastSegment as NavigationPropertySegment).NavigationProperty;
        if (navigationProperty.Name.Equals(navigationProperty1.Name))
        {
          autoExpandItems.RemoveAt(index);
          break;
        }
      }
    }

    private static int GetMaxExpandDepth(ModelBoundQuerySettings querySettings, string propertyName)
    {
      int maxExpandDepth = 0;
      if (querySettings != null)
      {
        ExpandConfiguration expandConfiguration;
        if (querySettings.ExpandConfigurations.TryGetValue(propertyName, out expandConfiguration))
        {
          maxExpandDepth = expandConfiguration.MaxDepth;
        }
        else
        {
          SelectExpandType? defaultExpandType = querySettings.DefaultExpandType;
          if (defaultExpandType.HasValue)
          {
            defaultExpandType = querySettings.DefaultExpandType;
            SelectExpandType selectExpandType = SelectExpandType.Disabled;
            if (!(defaultExpandType.GetValueOrDefault() == selectExpandType & defaultExpandType.HasValue))
              maxExpandDepth = querySettings.DefaultMaxDepth;
          }
        }
      }
      return maxExpandDepth;
    }
  }
}
