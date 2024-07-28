// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryOptimizationInstanceComparer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class QueryOptimizationInstanceComparer : IEqualityComparer<QueryOptimizationInstance>
  {
    public static QueryOptimizationInstanceComparer Instance = new QueryOptimizationInstanceComparer();

    public bool Equals(QueryOptimizationInstance x, QueryOptimizationInstance y)
    {
      if (x == null && y == null)
        return true;
      if (x == null || y == null)
        return false;
      Guid? queryId1 = x.QueryId;
      Guid? queryId2 = y.QueryId;
      if ((queryId1.HasValue == queryId2.HasValue ? (queryId1.HasValue ? (queryId1.GetValueOrDefault() == queryId2.GetValueOrDefault() ? 1 : 0) : 1) : 0) == 0)
      {
        Guid? queryId3 = x.QueryId;
        Guid empty1 = Guid.Empty;
        if ((queryId3.HasValue ? (queryId3.HasValue ? (queryId3.GetValueOrDefault() == empty1 ? 1 : 0) : 1) : 0) == 0 || y.QueryId.HasValue)
        {
          queryId3 = y.QueryId;
          Guid empty2 = Guid.Empty;
          if ((queryId3.HasValue ? (queryId3.HasValue ? (queryId3.GetValueOrDefault() == empty2 ? 1 : 0) : 1) : 0) == 0 || x.QueryId.HasValue)
            return false;
        }
      }
      return x.QueryHash.Equals(y.QueryHash, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(QueryOptimizationInstance obj) => !string.IsNullOrEmpty(obj.QueryHash) ? obj.QueryHash.GetHashCode() : 0;
  }
}
