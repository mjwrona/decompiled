// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataUriExtensions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.UriParser;
using System;

namespace Microsoft.OData
{
  public static class ODataUriExtensions
  {
    public static Uri BuildUri(this ODataUri odataUri, ODataUrlKeyDelimiter urlKeyDelimiter)
    {
      NodeToStringBuilder nodeToStringBuilder = new NodeToStringBuilder();
      SelectExpandClauseToStringBuilder clauseToStringBuilder = new SelectExpandClauseToStringBuilder();
      string queryOptions = string.Empty;
      bool writeQueryPrefix = true;
      if (odataUri.Filter != null)
      {
        string str = ODataUriExtensions.WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
        writeQueryPrefix = false;
        queryOptions = str + "$filter" + "=" + Uri.EscapeDataString(nodeToStringBuilder.TranslateFilterClause(odataUri.Filter));
      }
      if (odataUri.SelectAndExpand != null)
      {
        string str1 = clauseToStringBuilder.TranslateSelectExpandClause(odataUri.SelectAndExpand, true);
        if (!string.IsNullOrEmpty(str1))
        {
          string str2 = ODataUriExtensions.WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
          writeQueryPrefix = false;
          queryOptions = str2 + str1;
        }
      }
      if (odataUri.Apply != null)
      {
        string str3 = new ApplyClauseToStringBuilder().TranslateApplyClause(odataUri.Apply);
        if (!string.IsNullOrEmpty(str3))
        {
          string str4 = ODataUriExtensions.WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
          writeQueryPrefix = false;
          queryOptions = str4 + str3;
        }
      }
      if (odataUri.OrderBy != null)
      {
        string str = ODataUriExtensions.WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
        writeQueryPrefix = false;
        queryOptions = str + "$orderby" + "=" + Uri.EscapeDataString(nodeToStringBuilder.TranslateOrderByClause(odataUri.OrderBy));
      }
      long? nullable = odataUri.Top;
      if (nullable.HasValue)
      {
        string str5 = ODataUriExtensions.WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
        writeQueryPrefix = false;
        string str6 = str5;
        nullable = odataUri.Top;
        string str7 = Uri.EscapeDataString(nullable.ToString());
        queryOptions = str6 + "$top" + "=" + str7;
      }
      nullable = odataUri.Skip;
      if (nullable.HasValue)
      {
        string str8 = ODataUriExtensions.WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
        writeQueryPrefix = false;
        string str9 = str8;
        nullable = odataUri.Skip;
        string str10 = Uri.EscapeDataString(nullable.ToString());
        queryOptions = str9 + "$skip" + "=" + str10;
      }
      nullable = odataUri.Index;
      if (nullable.HasValue)
      {
        string str11 = ODataUriExtensions.WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
        writeQueryPrefix = false;
        string str12 = str11;
        nullable = odataUri.Index;
        string str13 = Uri.EscapeDataString(nullable.ToString());
        queryOptions = str12 + "$index" + "=" + str13;
      }
      bool? queryCount = odataUri.QueryCount;
      if (queryCount.HasValue)
      {
        string str14 = ODataUriExtensions.WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
        writeQueryPrefix = false;
        string str15 = str14;
        queryCount = odataUri.QueryCount;
        bool flag = true;
        string str16 = Uri.EscapeDataString(queryCount.GetValueOrDefault() == flag & queryCount.HasValue ? "true" : "false");
        queryOptions = str15 + "$count" + "=" + str16;
      }
      if (odataUri.Search != null)
      {
        string str = ODataUriExtensions.WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
        writeQueryPrefix = false;
        queryOptions = str + "$search" + "=" + Uri.EscapeDataString(nodeToStringBuilder.TranslateSearchClause(odataUri.Search));
      }
      if (odataUri.SkipToken != null)
      {
        string str = ODataUriExtensions.WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
        writeQueryPrefix = false;
        queryOptions = str + "$skiptoken" + "=" + Uri.EscapeDataString(odataUri.SkipToken);
      }
      if (odataUri.DeltaToken != null)
      {
        string str = ODataUriExtensions.WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions);
        writeQueryPrefix = false;
        queryOptions = str + "$deltatoken" + "=" + Uri.EscapeDataString(odataUri.DeltaToken);
      }
      if (odataUri.ParameterAliasNodes != null && odataUri.ParameterAliasNodes.Count > 0)
      {
        string str = nodeToStringBuilder.TranslateParameterAliasNodes(odataUri.ParameterAliasNodes);
        queryOptions = string.IsNullOrEmpty(str) ? queryOptions : ODataUriExtensions.WriteQueryPrefixOrSeparator(writeQueryPrefix, queryOptions) + str;
      }
      string uriString = odataUri.Path.ToResourcePathString(urlKeyDelimiter) + queryOptions;
      return !(odataUri.ServiceRoot == (Uri) null) ? new Uri(odataUri.ServiceRoot, new Uri(uriString, UriKind.Relative)) : new Uri(uriString, UriKind.Relative);
    }

    private static string WriteQueryPrefixOrSeparator(bool writeQueryPrefix, string queryOptions) => writeQueryPrefix ? queryOptions + "?" : queryOptions + "&";
  }
}
