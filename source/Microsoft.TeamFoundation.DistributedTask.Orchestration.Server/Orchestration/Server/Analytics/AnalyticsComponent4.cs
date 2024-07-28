// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.AnalyticsComponent4
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  internal class AnalyticsComponent4 : AnalyticsComponent3
  {
    public override List<ShallowTaskPlan> QueryShallowPlansByChangedDate(
      int dataspaceId,
      int batchSize,
      TaskPlanWatermark fromWatermark)
    {
      this.PrepareStoredProcedure("Task.prc_QueryPlanIdsAndLastUpdatedByChangedDate");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt(nameof (batchSize), batchSize);
      this.BindInt("internalId", fromWatermark.InternalId);
      this.BindDateTime("fromChangedDate", fromWatermark.LastUpdated, true);
      List<ShallowTaskPlan> shallowTaskPlanList = new List<ShallowTaskPlan>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ShallowTaskPlan>((ObjectBinder<ShallowTaskPlan>) this.GetShallowTaskPlanBinder(dataspaceId));
        resultCollection.AddBinder<ShallowTaskPlan>((ObjectBinder<ShallowTaskPlan>) this.GetShallowTaskPlanBinder(dataspaceId));
        shallowTaskPlanList.AddRange((IEnumerable<ShallowTaskPlan>) resultCollection.GetCurrent<ShallowTaskPlan>().Items);
        if (resultCollection.TryNextResult())
          shallowTaskPlanList.AddRange((IEnumerable<ShallowTaskPlan>) resultCollection.GetCurrent<ShallowTaskPlan>().Items);
      }
      return shallowTaskPlanList;
    }
  }
}
