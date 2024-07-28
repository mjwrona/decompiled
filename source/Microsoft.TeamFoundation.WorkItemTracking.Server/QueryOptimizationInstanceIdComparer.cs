// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryOptimizationInstanceIdComparer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class QueryOptimizationInstanceIdComparer : IEqualityComparer<QueryOptimizationInstance>
  {
    public static QueryOptimizationInstanceIdComparer Instance = new QueryOptimizationInstanceIdComparer();

    public bool Equals(QueryOptimizationInstance x, QueryOptimizationInstance y)
    {
      if (x == null && y == null)
        return true;
      if (x == null || y == null)
        return false;
      Guid? queryId1 = x.QueryId;
      Guid? queryId2 = y.QueryId;
      if ((queryId1.HasValue == queryId2.HasValue ? (queryId1.HasValue ? (queryId1.GetValueOrDefault() == queryId2.GetValueOrDefault() ? 1 : 0) : 1) : 0) == 0 || !x.QueryId.HasValue)
        return false;
      Guid? queryId3 = x.QueryId;
      Guid empty = Guid.Empty;
      if (!queryId3.HasValue)
        return true;
      return queryId3.HasValue && queryId3.GetValueOrDefault() != empty;
    }

    public int GetHashCode(QueryOptimizationInstance obj)
    {
      if (obj != null && obj.QueryId.HasValue)
      {
        Guid? queryId = obj.QueryId;
        Guid empty = Guid.Empty;
        if ((queryId.HasValue ? (queryId.HasValue ? (queryId.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
        {
          queryId = obj.QueryId;
          return queryId.GetHashCode();
        }
      }
      return 0;
    }
  }
}
