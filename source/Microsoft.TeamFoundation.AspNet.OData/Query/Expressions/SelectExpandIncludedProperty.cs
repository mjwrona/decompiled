// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.SelectExpandIncludedProperty
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal class SelectExpandIncludedProperty
  {
    private PropertySegment _propertySegment;
    private IEdmNavigationSource _navigationSource;
    private PathSelectItem _propertySelectItem;
    private IList<SelectItem> _subSelectItems;

    public SelectExpandIncludedProperty(PropertySegment propertySegment)
      : this(propertySegment, (IEdmNavigationSource) null)
    {
    }

    public SelectExpandIncludedProperty(
      PropertySegment propertySegment,
      IEdmNavigationSource navigationSource)
    {
      this._propertySegment = propertySegment != null ? propertySegment : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (propertySegment));
      this._navigationSource = navigationSource;
    }

    public PathSelectItem ToPathSelectItem()
    {
      if (this._subSelectItems == null)
        return this._propertySelectItem;
      bool allSelected = false;
      if (this._propertySelectItem != null && this._propertySelectItem.SelectAndExpand != null)
      {
        allSelected = this._propertySelectItem.SelectAndExpand.AllSelected;
        foreach (SelectItem selectedItem in this._propertySelectItem.SelectAndExpand.SelectedItems)
          this._subSelectItems.Add(selectedItem);
      }
      if (!allSelected)
      {
        allSelected = true;
        foreach (SelectItem subSelectItem in (IEnumerable<SelectItem>) this._subSelectItems)
        {
          switch (subSelectItem)
          {
            case ExpandedNavigationSelectItem _:
            case ExpandedReferenceSelectItem _:
              continue;
            default:
              allSelected = false;
              goto label_19;
          }
        }
      }
label_19:
      SelectExpandClause selectAndExpand = new SelectExpandClause((IEnumerable<SelectItem>) this._subSelectItems, allSelected);
      if (this._propertySelectItem == null && selectAndExpand == null)
        return (PathSelectItem) null;
      return this._propertySelectItem == null ? new PathSelectItem(new ODataSelectPath(new ODataPathSegment[1]
      {
        (ODataPathSegment) this._propertySegment
      }), this._navigationSource, selectAndExpand, (FilterClause) null, (OrderByClause) null, new long?(), new long?(), new bool?(), (SearchClause) null, (ComputeClause) null) : new PathSelectItem(new ODataSelectPath(new ODataPathSegment[1]
      {
        (ODataPathSegment) this._propertySegment
      }), this._navigationSource, selectAndExpand, this._propertySelectItem.FilterOption, this._propertySelectItem.OrderByOption, this._propertySelectItem.TopOption, this._propertySelectItem.SkipOption, this._propertySelectItem.CountOption, this._propertySelectItem.SearchOption, this._propertySelectItem.ComputeOption);
    }

    public void AddSubSelectItem(
      IList<ODataPathSegment> remainingSegments,
      PathSelectItem oldSelectItem)
    {
      if (remainingSegments == null)
      {
        this._propertySelectItem = oldSelectItem;
      }
      else
      {
        if (this._subSelectItems == null)
          this._subSelectItems = (IList<SelectItem>) new List<SelectItem>();
        this._subSelectItems.Add((SelectItem) new PathSelectItem(new ODataSelectPath((IEnumerable<ODataPathSegment>) remainingSegments), oldSelectItem.NavigationSource, oldSelectItem.SelectAndExpand, oldSelectItem.FilterOption, oldSelectItem.OrderByOption, oldSelectItem.TopOption, oldSelectItem.SkipOption, oldSelectItem.CountOption, oldSelectItem.SearchOption, oldSelectItem.ComputeOption));
      }
    }

    public void AddSubExpandItem(
      IList<ODataPathSegment> remainingSegments,
      ExpandedReferenceSelectItem oldRefItem)
    {
      if (this._subSelectItems == null)
        this._subSelectItems = (IList<SelectItem>) new List<SelectItem>();
      ODataExpandPath pathToNavigationProperty = new ODataExpandPath((IEnumerable<ODataPathSegment>) remainingSegments);
      if (oldRefItem is ExpandedNavigationSelectItem navigationSelectItem)
        this._subSelectItems.Add((SelectItem) new ExpandedNavigationSelectItem(pathToNavigationProperty, navigationSelectItem.NavigationSource, navigationSelectItem.SelectAndExpand, navigationSelectItem.FilterOption, navigationSelectItem.OrderByOption, navigationSelectItem.TopOption, navigationSelectItem.SkipOption, navigationSelectItem.CountOption, navigationSelectItem.SearchOption, navigationSelectItem.LevelsOption, navigationSelectItem.ComputeOption, navigationSelectItem.ApplyOption));
      else
        this._subSelectItems.Add((SelectItem) new ExpandedReferenceSelectItem(pathToNavigationProperty, oldRefItem.NavigationSource, oldRefItem.FilterOption, oldRefItem.OrderByOption, oldRefItem.TopOption, oldRefItem.SkipOption, oldRefItem.CountOption, oldRefItem.SearchOption, oldRefItem.ComputeOption, oldRefItem.ApplyOption));
    }
  }
}
