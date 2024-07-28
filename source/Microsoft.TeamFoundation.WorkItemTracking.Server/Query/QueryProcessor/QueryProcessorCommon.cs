// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor.QueryProcessorCommon
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor
{
  internal static class QueryProcessorCommon
  {
    public static bool CanQueryAsText(
      QueryProcessorContext context,
      QueryComparisonExpressionNode node)
    {
      return QueryProcessorCommon.CanQueryAsText(context, node.Field, node.Operator);
    }

    public static bool CanQueryAsText(
      QueryProcessorContext context,
      FieldEntry field,
      QueryExpressionOperator queryOperator)
    {
      if (field.FieldId == 80)
        return false;
      if (field.IsLongText && queryOperator.IsContains() || field.OftenQueriedAsText && queryOperator.IsContainsWords() && context.m_supportsFullTextSearch)
        return true;
      return field.IsLongText && queryOperator.UsesIsEmpty();
    }

    public static string EncodeSqlString(string value) => "N'" + value.Replace("'", "''") + "'";

    public static string FixContainsValue(string value, string referenceName)
    {
      value = "%" + value.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]") + "%";
      QueryProcessorCommon.ValidateContainsValueLength(value, referenceName);
      return value;
    }

    public static void ValidateContainsValueLength(string value, string referenceName)
    {
      if (value.Length > 4000)
        throw new WorkItemTrackingQueryException(ServerResources.QueryInvalidValueLength((object) referenceName, (object) 4000));
    }

    public static string FixContainsValueFullText(string value, string referenceName)
    {
      if (true)
        value = "\"" + value.Replace("\"", "\"\"") + "\"";
      QueryProcessorCommon.ValidateContainsValueLength(value, referenceName);
      return value;
    }
  }
}
