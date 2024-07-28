// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobInfoQueryHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobInfoQueryHelper
  {
    public static string ReplaceWildCards(string value) => value.Replace("*", "%");

    public static string GetOrderByClause(
      IList<KeyValuePair<ServicingJobInfoColumn, SortOrder>> sortOrder)
    {
      string empty = string.Empty;
      if (sortOrder != null)
      {
        StringBuilder stringBuilder1 = new StringBuilder();
        stringBuilder1.Append("ORDER BY ");
        for (int index = 0; index < sortOrder.Count; ++index)
        {
          StringBuilder stringBuilder2 = stringBuilder1;
          KeyValuePair<ServicingJobInfoColumn, SortOrder> keyValuePair = sortOrder[index];
          string str1 = keyValuePair.Key.ToString();
          keyValuePair = sortOrder[index];
          string str2 = keyValuePair.Value == SortOrder.Ascending ? "ASC" : "DESC";
          string str3 = str1 + " " + str2;
          stringBuilder2.Append(str3);
          if (index != sortOrder.Count - 1)
            stringBuilder1.Append(", ");
        }
        empty = stringBuilder1.ToString();
      }
      return empty;
    }

    public static string GetWhereClause(
      DateTime queueTimeFrom,
      DateTime queueTimeTo,
      string operationClass,
      ServicingJobResult? result,
      ServicingJobStatus? status,
      string databaseName,
      int? databaseId,
      Guid? accountId,
      string poolName)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("( QueueTime BETWEEN @queueTimeFrom AND @queueTimeTo )\r\n");
      if (!string.IsNullOrEmpty(operationClass))
        stringBuilder.Append(" AND ( " + JobInfoQueryHelper.GetStringClause(nameof (operationClass), operationClass.Contains<char>('*')) + " )\r\n");
      if (result.HasValue)
        stringBuilder.Append(" AND ( Result = @jobResult )\r\n");
      if (status.HasValue)
        stringBuilder.Append(" AND ( JobStatus = @jobStatus )\r\n");
      if (!string.IsNullOrEmpty(databaseName))
        stringBuilder.Append(" AND ( " + JobInfoQueryHelper.GetStringClause(nameof (databaseName), databaseName.Contains<char>('*')) + " )\r\n");
      if (databaseId.HasValue)
        stringBuilder.Append(" AND ( DatabaseId = @databaseId )\r\n");
      if (accountId.HasValue)
        stringBuilder.Append(" AND ( AccountId = @accountId )\r\n");
      if (!string.IsNullOrEmpty(poolName))
        stringBuilder.Append(" AND ( " + JobInfoQueryHelper.GetStringClause(nameof (poolName), poolName.Contains<char>('*')) + " )\r\n");
      return stringBuilder.ToString();
    }

    public static string GetStringClause(string columnName, bool containsWildcards) => containsWildcards ? columnName + " LIKE @" + columnName : columnName + " = @" + columnName;

    public static string GetTopClause(int? top) => !top.HasValue ? string.Empty : "TOP " + top.Value.ToString();
  }
}
