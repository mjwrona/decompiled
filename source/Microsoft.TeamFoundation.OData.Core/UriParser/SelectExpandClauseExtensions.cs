// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectExpandClauseExtensions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.OData.UriParser
{
  internal static class SelectExpandClauseExtensions
  {
    internal static void GetSubSelectExpandClause(
      this SelectExpandClause clause,
      string propertyName,
      out SelectExpandClause subSelectExpand,
      out TypeSegment typeSegment)
    {
      subSelectExpand = (SelectExpandClause) null;
      typeSegment = (TypeSegment) null;
      ExpandedNavigationSelectItem navigationSelectItem = clause.SelectedItems.OfType<ExpandedNavigationSelectItem>().FirstOrDefault<ExpandedNavigationSelectItem>((Func<ExpandedNavigationSelectItem, bool>) (m => m.PathToNavigationProperty.LastSegment != null && m.PathToNavigationProperty.LastSegment.TranslateWith<string>((PathSegmentTranslator<string>) PathSegmentToStringTranslator.Instance) == propertyName));
      if (navigationSelectItem == null)
        return;
      subSelectExpand = navigationSelectItem.SelectAndExpand;
      typeSegment = navigationSelectItem.PathToNavigationProperty.FirstSegment as TypeSegment;
    }

    internal static void GetSelectExpandPaths(
      this SelectExpandClause selectExpandClause,
      ODataVersion version,
      out string selectClause,
      out string expandClause)
    {
      StringBuilder selectClause1;
      StringBuilder expandClause1;
      selectExpandClause.GetSelectExpandPaths(version, out selectClause1, out expandClause1);
      selectClause = selectClause1.ToString();
      expandClause = expandClause1.ToString();
    }

    internal static void GetSelectExpandPaths(
      this SelectExpandClause selectExpandClause,
      ODataVersion version,
      out StringBuilder selectClause,
      out StringBuilder expandClause)
    {
      selectClause = new StringBuilder();
      expandClause = new StringBuilder();
      selectClause.Append(SelectExpandClauseExtensions.BuildTopLevelSelect(selectExpandClause));
      expandClause.Append(SelectExpandClauseExtensions.BuildExpandsForNode(selectExpandClause, version));
    }

    internal static List<string> GetCurrentLevelSelectList(
      this SelectExpandClause selectExpandClause)
    {
      return selectExpandClause.AllAutoSelected ? selectExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>().Select<ExpandedNavigationSelectItem, string>((Func<ExpandedNavigationSelectItem, string>) (i => string.Join("/", i.PathToNavigationProperty.WalkWith<string>((PathSegmentTranslator<string>) PathSegmentToStringTranslator.Instance).ToArray<string>()))).ToList<string>() : selectExpandClause.SelectedItems.Select<SelectItem, string>(new Func<SelectItem, string>(SelectExpandClauseExtensions.GetSelectString)).Where<string>((Func<string, bool>) (i => i != null)).ToList<string>();
    }

    internal static void Traverse<T>(
      this SelectExpandClause selectExpandClause,
      Func<string, T, ODataVersion, T> processSubResult,
      Func<IList<string>, IList<T>, T> combineSelectAndExpand,
      Func<ApplyClause, T> processApply,
      ODataVersion version,
      out T result)
    {
      List<string> currentLevelSelectList = selectExpandClause.GetCurrentLevelSelectList();
      List<T> objList = new List<T>();
      foreach (ExpandedNavigationSelectItem navigationSelectItem in selectExpandClause.SelectedItems.Where<SelectItem>((Func<SelectItem, bool>) (I => I.GetType() == typeof (ExpandedNavigationSelectItem))))
      {
        string str = string.Join("/", navigationSelectItem.PathToNavigationProperty.WalkWith<string>((PathSegmentTranslator<string>) PathSegmentToStringTranslator.Instance).ToArray<string>());
        T result1 = default (T);
        if (navigationSelectItem.SelectAndExpand.SelectedItems.Any<SelectItem>())
          navigationSelectItem.SelectAndExpand.Traverse<T>(processSubResult, combineSelectAndExpand, processApply, version, out result1);
        if (navigationSelectItem.ApplyOption != null && processApply != null)
          result1 = processApply(navigationSelectItem.ApplyOption);
        T obj = processSubResult(str, result1, version);
        if ((object) obj != null)
          objList.Add(obj);
      }
      foreach (ExpandedReferenceSelectItem referenceSelectItem in selectExpandClause.SelectedItems.Where<SelectItem>((Func<SelectItem, bool>) (I => I.GetType() == typeof (ExpandedReferenceSelectItem))))
      {
        string str = string.Join("/", referenceSelectItem.PathToNavigationProperty.WalkWith<string>((PathSegmentTranslator<string>) PathSegmentToStringTranslator.Instance).ToArray<string>()) + "/$ref";
        T obj = processSubResult(str, default (T), version);
        if ((object) obj != null)
          objList.Add(obj);
      }
      result = combineSelectAndExpand((IList<string>) currentLevelSelectList, (IList<T>) objList);
    }

    private static string BuildTopLevelSelect(SelectExpandClause selectExpandClause) => string.Join(",", selectExpandClause.GetCurrentLevelSelectList().ToArray());

    private static string GetSelectString(SelectItem selectedItem)
    {
      WildcardSelectItem wildcardSelectItem1 = selectedItem as WildcardSelectItem;
      NamespaceQualifiedWildcardSelectItem wildcardSelectItem2 = selectedItem as NamespaceQualifiedWildcardSelectItem;
      PathSelectItem pathSelectItem = selectedItem as PathSelectItem;
      if (wildcardSelectItem1 != null)
        return "*";
      if (wildcardSelectItem2 != null)
        return wildcardSelectItem2.Namespace + ".*";
      return pathSelectItem != null ? string.Join("/", pathSelectItem.SelectedPath.WalkWith<string>((PathSegmentTranslator<string>) PathSegmentToStringTranslator.Instance).ToArray<string>()) : (string) null;
    }

    private static string BuildExpandsForNode(
      SelectExpandClause selectExpandClause,
      ODataVersion version)
    {
      List<string> stringList = new List<string>();
      foreach (ExpandedNavigationSelectItem navigationSelectItem in selectExpandClause.SelectedItems.Where<SelectItem>((Func<SelectItem, bool>) (I => I.GetType() == typeof (ExpandedNavigationSelectItem))))
      {
        string str = string.Join("/", navigationSelectItem.PathToNavigationProperty.WalkWith<string>((PathSegmentTranslator<string>) PathSegmentToStringTranslator.Instance).ToArray<string>());
        string result;
        navigationSelectItem.SelectAndExpand.Traverse<string>(new Func<string, string, ODataVersion, string>(SelectExpandClauseExtensions.ProcessSubExpand), new Func<IList<string>, IList<string>, string>(SelectExpandClauseExtensions.CombineSelectAndExpandResult), (Func<ApplyClause, string>) null, version, out result);
        if (!string.IsNullOrEmpty(result))
          str = str + "(" + result + ")";
        stringList.Add(str);
      }
      foreach (ExpandedReferenceSelectItem referenceSelectItem in selectExpandClause.SelectedItems.Where<SelectItem>((Func<SelectItem, bool>) (I => I.GetType() == typeof (ExpandedReferenceSelectItem))))
      {
        string str = string.Join("/", referenceSelectItem.PathToNavigationProperty.WalkWith<string>((PathSegmentTranslator<string>) PathSegmentToStringTranslator.Instance).ToArray<string>()) + "/$ref";
        stringList.Add(str);
      }
      return string.Join(",", stringList.ToArray());
    }

    private static string ProcessSubExpand(
      string expandNode,
      string subExpand,
      ODataVersion version)
    {
      return !string.IsNullOrEmpty(subExpand) ? expandNode + "(" + subExpand + ")" : expandNode;
    }

    private static string CombineSelectAndExpandResult(
      IList<string> selectList,
      IList<string> expandList)
    {
      string str = "";
      if (selectList.Any<string>())
        str = str + "$select=" + string.Join(",", selectList.ToArray<string>());
      if (expandList.Any<string>())
      {
        if (!string.IsNullOrEmpty(str))
          str += ";";
        str = str + "$expand=" + string.Join(",", expandList.ToArray<string>());
      }
      return str;
    }
  }
}
