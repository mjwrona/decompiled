// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.QuerySqlComponent7
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class QuerySqlComponent7 : QuerySqlComponent6
  {
    public override IEnumerable<QueryOptimizationInstance> ResetNonOptimizableQueries(
      DateTime lookBackLastStateChangeTime)
    {
      this.PrepareStoredProcedure("prc_ResetNonOptimizableQueries");
      this.BindDateTime("@lookBackLastStateChangeTime", lookBackLastStateChangeTime);
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<QueryOptimizationInstance>((ObjectBinder<QueryOptimizationInstance>) new QuerySqlComponent.QueryUniqueIdentifierBinder());
        return (IEnumerable<QueryOptimizationInstance>) resultCollection.GetCurrent<QueryOptimizationInstance>().Items;
      }
    }
  }
}
