// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExecutionInformationComparer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class QueryExecutionInformationComparer : IEqualityComparer<QueryExecutionInformation>
  {
    public static QueryExecutionInformationComparer Instance = new QueryExecutionInformationComparer();

    public bool Equals(QueryExecutionInformation x, QueryExecutionInformation y)
    {
      if (x == null && y == null)
        return true;
      if (x == null || y == null)
        return false;
      Guid? queryId1 = x.QueryId;
      Guid? queryId2 = y.QueryId;
      return (queryId1.HasValue == queryId2.HasValue ? (queryId1.HasValue ? (queryId1.GetValueOrDefault() != queryId2.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0 && x.QueryHash == y.QueryHash;
    }

    public int GetHashCode(QueryExecutionInformation obj) => obj != null ? obj.QueryHash.GetHashCode() : 0;
  }
}
