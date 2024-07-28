// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor.QueryPredicateCommon
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor
{
  internal class QueryPredicateCommon
  {
    internal static bool GetTempTableId(
      QueryProcessorContext context,
      QueryComparisonExpressionNode expressionNode,
      out int tempTableId)
    {
      string key = expressionNode.Value.ValueType == QueryExpressionValueType.Array || expressionNode.Value.ValueType == QueryExpressionValueType.IdentityGuid ? Guid.NewGuid().ToString("D", (IFormatProvider) CultureInfo.InvariantCulture) : string.Format("{0}:{1}", (object) expressionNode.Field.FieldId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo), (object) expressionNode.Value);
      if (context.m_tempTableMap.TryGetValue(key, out tempTableId))
        return false;
      tempTableId = context.m_tempTableCounter++;
      if (string.IsNullOrWhiteSpace(key))
        context.m_tempTableMap[key] = tempTableId;
      return true;
    }
  }
}
