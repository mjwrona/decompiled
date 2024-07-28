// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor.QueryExpressionNodeComparer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor
{
  internal class QueryExpressionNodeComparer : IComparer<QueryExpressionNode>
  {
    public int Compare(QueryExpressionNode x, QueryExpressionNode y)
    {
      int num = this.ComputeWeight(x) - this.ComputeWeight(y);
      if (num != 0)
        return num;
      if (x == null)
        return 0;
      return x is QueryComparisonExpressionNode ? this.CompareQueryComparisonExpressionNode((QueryComparisonExpressionNode) x, (QueryComparisonExpressionNode) y) : this.CompareQueryLogicalExpressionNode((QueryLogicalExpressionNode) x, (QueryLogicalExpressionNode) y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ComputeWeight(QueryExpressionNode n)
    {
      switch (n)
      {
        case null:
          return -1;
        case QueryComparisonExpressionNode _:
          return 1;
        case QueryLogicalExpressionNode _:
          return 2;
        default:
          return 0;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CompareQueryComparisonExpressionNode(
      QueryComparisonExpressionNode x,
      QueryComparisonExpressionNode y)
    {
      int num = x.Field.FieldId - y.Field.FieldId;
      return num != 0 ? num : x.Operator - y.Operator;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CompareQueryLogicalExpressionNode(
      QueryLogicalExpressionNode x,
      QueryLogicalExpressionNode y)
    {
      return x.Operator - y.Operator;
    }
  }
}
