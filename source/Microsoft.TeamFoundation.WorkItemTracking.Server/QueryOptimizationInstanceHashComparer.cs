// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryOptimizationInstanceHashComparer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class QueryOptimizationInstanceHashComparer : IEqualityComparer<QueryOptimizationInstance>
  {
    public static QueryOptimizationInstanceHashComparer Instance = new QueryOptimizationInstanceHashComparer();

    public bool Equals(QueryOptimizationInstance x, QueryOptimizationInstance y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && x.QueryHash.Equals(y.QueryHash, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(QueryOptimizationInstance obj) => obj != null && !string.IsNullOrEmpty(obj.QueryHash) ? obj.QueryHash.GetHashCode() : 0;
  }
}
