// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpressionStatistics
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class QueryExpressionStatistics
  {
    internal ISet<string> fieldsReferenced { get; }

    internal Dictionary<QueryExpressionOperator, int> operatorCounts { get; }

    internal IList<int> orExpressionChildCounts { get; }

    internal IList<int> andExpressionChildCounts { get; }

    internal IList<int> containsValueLengths { get; }

    internal IList<int> inValueLengths { get; }

    internal IList<int> nodeDepths { get; }

    internal QueryExpressionStatistics()
    {
      this.fieldsReferenced = (ISet<string>) new HashSet<string>();
      this.operatorCounts = new Dictionary<QueryExpressionOperator, int>();
      this.orExpressionChildCounts = (IList<int>) new List<int>();
      this.andExpressionChildCounts = (IList<int>) new List<int>();
      this.containsValueLengths = (IList<int>) new List<int>();
      this.inValueLengths = (IList<int>) new List<int>();
      this.nodeDepths = (IList<int>) new List<int>();
    }
  }
}
