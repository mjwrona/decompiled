// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectExpandClause
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public sealed class SelectExpandClause
  {
    private ReadOnlyCollection<SelectItem> selectedItems;
    private bool? allSelected;

    public SelectExpandClause(
      IEnumerable<SelectItem> selectedItems,
      bool allSelected,
      bool allAutoSelected = false)
    {
      this.selectedItems = selectedItems != null ? new ReadOnlyCollection<SelectItem>((IList<SelectItem>) selectedItems.ToList<SelectItem>()) : new ReadOnlyCollection<SelectItem>((IList<SelectItem>) new List<SelectItem>());
      this.allSelected = new bool?(allSelected);
      this.AllAutoSelected = allAutoSelected;
    }

    public IEnumerable<SelectItem> SelectedItems => this.selectedItems.AsEnumerable<SelectItem>();

    public bool AllSelected => this.allSelected.Value;

    public bool AllAutoSelected { get; set; }

    public void SetAllSelected(bool newValue) => this.allSelected = new bool?(newValue);

    internal void AddToSelectedItems(SelectItem itemToAdd)
    {
      ExceptionUtils.CheckArgumentNotNull<SelectItem>(itemToAdd, nameof (itemToAdd));
      if (this.selectedItems.Any<SelectItem>((Func<SelectItem, bool>) (x => x is WildcardSelectItem)) && SelectExpandClause.IsStructuralOrNavigationPropertySelectionItem(itemToAdd))
        return;
      bool flag = itemToAdd is WildcardSelectItem;
      List<SelectItem> list = new List<SelectItem>();
      foreach (SelectItem selectedItem in this.selectedItems)
      {
        if (flag)
        {
          if (!SelectExpandClause.IsStructuralSelectionItem(selectedItem))
            list.Add(selectedItem);
        }
        else
          list.Add(selectedItem);
      }
      list.Add(itemToAdd);
      this.selectedItems = new ReadOnlyCollection<SelectItem>((IList<SelectItem>) list);
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
