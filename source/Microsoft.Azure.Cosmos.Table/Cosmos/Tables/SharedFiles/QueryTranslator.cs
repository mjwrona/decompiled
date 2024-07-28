// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tables.SharedFiles.QueryTranslator
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents.Interop.Common.Schema.Edm;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.Cosmos.Tables.SharedFiles
{
  internal static class QueryTranslator
  {
    private const string FromCollectionAlias = "entity";
    private const string UnconditionalQueryFormat = "select {0} from entity {1}";
    private const string ConditionalQueryFormat = "select {0} from entity where {1} {2}";
    private const string SelectAll = "*";

    public static string GetSqlQuery(
      string selectList,
      string odataV4FilterString,
      bool isLinqExpression,
      bool isTableQuery,
      IList<OrderByItem> orderByItems,
      string tombstoneKey = null,
      bool enableTimestampQuery = false)
    {
      string str1 = tombstoneKey == null ? (string) null : "not (is_defined(entity['" + tombstoneKey + "']['$v']))";
      if (string.IsNullOrEmpty(selectList) && string.IsNullOrEmpty(odataV4FilterString))
        throw new ArgumentException("All arguments cannot be null for query");
      if (string.IsNullOrEmpty(selectList))
        selectList = "*";
      string str2 = QueryTranslator.OrderByClause(enableTimestampQuery, orderByItems);
      if (string.IsNullOrEmpty(odataV4FilterString))
        return string.IsNullOrEmpty(tombstoneKey) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "select {0} from entity {1}", (object) selectList, (object) str2) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "select {0} from entity where {1} {2}", (object) selectList, (object) str1, (object) str2);
      string str3 = odataV4FilterString;
      if (!isLinqExpression)
      {
        try
        {
          str3 = ODataFilterTranslator.ToSql(odataV4FilterString, isTableQuery, enableTimestampQuery);
          if (!string.IsNullOrEmpty(tombstoneKey))
            str3 = "(" + str3 + ") and " + str1;
        }
        catch (Exception ex)
        {
          switch (ex)
          {
            case ODataException _:
            case NotSupportedException _:
            case NotImplementedException _:
            case InvalidCastException _:
            case ArgumentNullException _:
              throw new InvalidFilterException("Invalid filter", ex);
            default:
              throw;
          }
        }
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "select {0} from entity where {1} {2}", (object) selectList, (object) str3, (object) str2);
    }

    public static string GetSelectString(string selectList)
    {
      if (string.IsNullOrEmpty(selectList))
        return selectList;
      StringBuilder stringBuilder1 = new StringBuilder(selectList.Length);
      StringBuilder stringBuilder2 = new StringBuilder(selectList.Length);
      Dictionary<string, string> propertiesMapping = EdmSchemaMapping.SystemPropertiesMapping;
      int index = 0;
      char ch = ',';
      for (; index <= selectList.Length; ++index)
      {
        if (index == selectList.Length || (int) selectList[index] == (int) ch)
        {
          string key = stringBuilder2.ToString();
          stringBuilder2.Clear();
          stringBuilder1.Append("entity").Append("[\"").Append(propertiesMapping.ContainsKey(key) ? propertiesMapping[key] : key).Append("\"]");
          if (index != selectList.Length)
            stringBuilder1.Append(ch);
        }
        else
          stringBuilder2.Append(selectList[index]);
      }
      stringBuilder1.Append(ch).Append("entity").Append("[\"").Append("_etag").Append("\"]");
      return stringBuilder1.ToString();
    }

    private static string OrderByClause(bool enableTimestampQuery, IList<OrderByItem> orderByItems = null)
    {
      if (orderByItems == null || orderByItems.Count == 0)
        return "";
      StringBuilder stringBuilder = new StringBuilder("order by");
      foreach (OrderByItem orderByItem in (IEnumerable<OrderByItem>) orderByItems)
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " {0} {1},", (object) EntityTranslator.GetPropertyName(orderByItem.PropertyName, enableTimestampQuery), (object) orderByItem.Order));
      return stringBuilder.ToString(0, stringBuilder.Length - 1);
    }
  }
}
