// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.AnalyticsComponent5
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  internal class AnalyticsComponent5 : AnalyticsComponent4
  {
    public override List<TaskTimelineRecord> QueryTimelineRecordsByChangedDateWithIssues(
      int dataspaceId,
      int batchSize,
      TaskPlanWatermark fromWatermark)
    {
      this.PrepareStoredProcedure("Task.prc_QueryTimelineRecordsByChangedDateWithIssues");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt("internalId", fromWatermark.InternalId);
      this.BindDateTime("fromLastUpdatedDate", fromWatermark.LastUpdated, true);
      this.BindInt(nameof (batchSize), batchSize);
      List<TaskTimelineRecord> taskTimelineRecordList = new List<TaskTimelineRecord>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TaskTimelineRecord>((ObjectBinder<TaskTimelineRecord>) this.GetTaskTimelineRecordBinder(dataspaceId));
        resultCollection.AddBinder<TaskTimelineRecord>((ObjectBinder<TaskTimelineRecord>) this.GetTaskTimelineRecordBinder(dataspaceId));
        taskTimelineRecordList.AddRange((IEnumerable<TaskTimelineRecord>) resultCollection.GetCurrent<TaskTimelineRecord>().Items);
        if (resultCollection.TryNextResult())
          taskTimelineRecordList.AddRange((IEnumerable<TaskTimelineRecord>) resultCollection.GetCurrent<TaskTimelineRecord>().Items);
      }
      return taskTimelineRecordList;
    }
  }
}
