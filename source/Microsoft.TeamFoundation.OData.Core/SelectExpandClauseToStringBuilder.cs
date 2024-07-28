// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.SelectExpandClauseToStringBuilder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData
{
  internal sealed class SelectExpandClauseToStringBuilder : SelectItemTranslator<string>
  {
    private bool isFirstSelectItem = true;

    public string TranslateSelectExpandClause(SelectExpandClause selectExpandClause, bool firstFlag)
    {
      ExceptionUtils.CheckArgumentNotNull<SelectExpandClause>(selectExpandClause, nameof (selectExpandClause));
      List<string> currentLevelSelectList = selectExpandClause.GetCurrentLevelSelectList();
      string stringToEscape1 = (string) null;
      if (currentLevelSelectList.Any<string>())
        stringToEscape1 = string.Join(",", currentLevelSelectList.ToArray());
      string str = string.IsNullOrEmpty(stringToEscape1) ? (string) null : "$select" + "=" + (this.isFirstSelectItem ? Uri.EscapeDataString(stringToEscape1) : stringToEscape1);
      this.isFirstSelectItem = false;
      string stringToEscape2 = (string) null;
      foreach (ExpandedNavigationSelectItem navigationSelectItem in selectExpandClause.SelectedItems.Where<SelectItem>((Func<SelectItem, bool>) (I => I.GetType() == typeof (ExpandedNavigationSelectItem))))
      {
        stringToEscape2 = !string.IsNullOrEmpty(stringToEscape2) ? stringToEscape2 + "," : (firstFlag ? stringToEscape2 : "$expand" + "=");
        stringToEscape2 += this.Translate(navigationSelectItem);
      }
      foreach (ExpandedReferenceSelectItem referenceSelectItem in selectExpandClause.SelectedItems.Where<SelectItem>((Func<SelectItem, bool>) (I => I.GetType() == typeof (ExpandedReferenceSelectItem))))
      {
        stringToEscape2 = !string.IsNullOrEmpty(stringToEscape2) ? stringToEscape2 + "," : (firstFlag ? stringToEscape2 : "$expand" + "=");
        stringToEscape2 = stringToEscape2 + this.Translate(referenceSelectItem) + "/$ref";
      }
      if (string.IsNullOrEmpty(stringToEscape2))
        return str;
      return firstFlag ? (!string.IsNullOrEmpty(str) ? str + "&$expand=" + Uri.EscapeDataString(stringToEscape2) : "$expand=" + Uri.EscapeDataString(stringToEscape2)) : (!string.IsNullOrEmpty(str) ? str + (";" + stringToEscape2) : stringToEscape2);
    }

    public override string Translate(WildcardSelectItem item) => string.Empty;

    public override string Translate(PathSelectItem item) => string.Empty;

    public override string Translate(NamespaceQualifiedWildcardSelectItem item) => item.Namespace;

    public override string Translate(ExpandedNavigationSelectItem item)
    {
      string str1 = string.Empty;
      if (item.SelectAndExpand != null)
      {
        string str2 = this.TranslateSelectExpandClause(item.SelectAndExpand, false);
        str1 = str1 + (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2) ? (string) null : ";") + str2;
      }
      if (item.LevelsOption != null)
        str1 = str1 + (string.IsNullOrEmpty(str1) ? (string) null : ";") + "$levels" + "=" + NodeToStringBuilder.TranslateLevelsClause(item.LevelsOption);
      string str3 = this.Translate((ExpandedReferenceSelectItem) item);
      return str3.EndsWith(")", StringComparison.Ordinal) ? str3.Insert(str3.Length - 1, string.IsNullOrEmpty(str1) ? string.Empty : ";" + str1) : str3 + (string.IsNullOrEmpty(str1) ? (string) null : "(" + str1 + ")");
    }

    public override string Translate(ExpandedReferenceSelectItem item)
    {
      NodeToStringBuilder nodeToStringBuilder = new NodeToStringBuilder();
      string str1 = string.Join("/", item.PathToNavigationProperty.WalkWith<string>((PathSegmentTranslator<string>) PathSegmentToStringTranslator.Instance).ToArray<string>());
      string str2 = string.Empty;
      if (item.FilterOption != null)
        str2 = str2 + "$filter=" + nodeToStringBuilder.TranslateFilterClause(item.FilterOption);
      if (item.OrderByOption != null)
        str2 = str2 + (string.IsNullOrEmpty(str2) ? (string) null : ";") + "$orderby=" + nodeToStringBuilder.TranslateOrderByClause(item.OrderByOption);
      long? nullable = item.TopOption;
      if (nullable.HasValue)
      {
        string str3 = str2 + (string.IsNullOrEmpty(str2) ? (string) null : ";");
        nullable = item.TopOption;
        string str4 = nullable.ToString();
        str2 = str3 + "$top=" + str4;
      }
      nullable = item.SkipOption;
      if (nullable.HasValue)
      {
        string str5 = str2 + (string.IsNullOrEmpty(str2) ? (string) null : ";");
        nullable = item.SkipOption;
        string str6 = nullable.ToString();
        str2 = str5 + "$skip=" + str6;
      }
      bool? countOption = item.CountOption;
      if (countOption.HasValue)
      {
        string str7 = str2 + (string.IsNullOrEmpty(str2) ? (string) null : ";") + "$count" + "=";
        countOption = item.CountOption;
        bool flag = true;
        str2 = !(countOption.GetValueOrDefault() == flag & countOption.HasValue) ? str7 + "false" : str7 + "true";
      }
      if (item.SearchOption != null)
        str2 = str2 + (string.IsNullOrEmpty(str2) ? (string) null : ";") + "$search" + "=" + nodeToStringBuilder.TranslateSearchClause(item.SearchOption);
      if (item.ComputeOption != null)
        str2 = str2 + (string.IsNullOrEmpty(str2) ? (string) null : ";") + "$compute" + "=" + nodeToStringBuilder.TranslateComputeClause(item.ComputeOption);
      if (item.ApplyOption != null)
        str2 = str2 + (string.IsNullOrEmpty(str2) ? (string) null : ";") + new ApplyClauseToStringBuilder().TranslateApplyClause(item.ApplyOption);
      return str1 + (string.IsNullOrEmpty(str2) ? (string) null : "(" + str2 + ")");
    }
  }
}
