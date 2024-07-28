// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectExpandClauseFinisher
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal sealed class SelectExpandClauseFinisher
  {
    public static void AddExplicitNavPropLinksWhereNecessary(SelectExpandClause clause)
    {
      IEnumerable<SelectItem> selectedItems = clause.SelectedItems;
      bool flag = selectedItems.Any<SelectItem>((Func<SelectItem, bool>) (x => x is PathSelectItem));
      IEnumerable<ODataSelectPath> source = selectedItems.OfType<PathSelectItem>().Select<PathSelectItem, ODataSelectPath>((Func<PathSelectItem, ODataSelectPath>) (item => item.SelectedPath));
      foreach (ExpandedNavigationSelectItem navigationSelectItem in selectedItems.Where<SelectItem>((Func<SelectItem, bool>) (I => I.GetType() == typeof (ExpandedNavigationSelectItem))))
      {
        ExpandedNavigationSelectItem navigationSelect = navigationSelectItem;
        if (flag && !source.Any<ODataSelectPath>((Func<ODataSelectPath, bool>) (x => x.Equals((ODataPath) navigationSelect.PathToNavigationProperty.ToSelectPath()))))
          clause.AddToSelectedItems((SelectItem) new PathSelectItem(navigationSelect.PathToNavigationProperty.ToSelectPath()));
        SelectExpandClauseFinisher.AddExplicitNavPropLinksWhereNecessary(navigationSelect.SelectAndExpand);
      }
      foreach (ExpandedReferenceSelectItem referenceSelectItem in selectedItems.Where<SelectItem>((Func<SelectItem, bool>) (I => I.GetType() == typeof (ExpandedReferenceSelectItem))))
      {
        ExpandedReferenceSelectItem navigationSelect = referenceSelectItem;
        if (flag && !source.Any<ODataSelectPath>((Func<ODataSelectPath, bool>) (x => x.Equals((ODataPath) navigationSelect.PathToNavigationProperty.ToSelectPath()))))
          clause.AddToSelectedItems((SelectItem) new PathSelectItem(navigationSelect.PathToNavigationProperty.ToSelectPath()));
      }
    }
  }
}
