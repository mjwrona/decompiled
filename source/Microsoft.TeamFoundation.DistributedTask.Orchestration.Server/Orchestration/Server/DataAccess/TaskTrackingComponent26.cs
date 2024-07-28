// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent26
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent26 : TaskTrackingComponent25
  {
    public override async Task<IList<TimelineRecord>> GetTimelineRecordsAsync(
      Guid scopeId,
      Guid planId,
      Guid timelineId,
      IEnumerable<Guid> records,
      bool includeOutputs = false)
    {
      TaskTrackingComponent26 component = this;
      IList<TimelineRecord> timelineRecordsAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetTimelineRecordsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetTimelineRecords");
        component.BindDataspaceId(scopeId);
        component.BindGuid("@planId", planId);
        component.BindNullableGuid("@timelineId", new Guid?(timelineId), true);
        component.BindGuidTable("@records", records.Distinct<Guid>());
        component.BindBoolean("@includeOutputs", includeOutputs);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TimelineRecord>(component.GetTimelineRecordBinder());
          resultCollection.AddBinder<TimelineRecordVariableData>(component.GetTimelineRecordVariableBinder());
          List<TimelineRecord> items = resultCollection.GetCurrent<TimelineRecord>().Items;
          if (includeOutputs && resultCollection.TryNextResult())
          {
            Dictionary<Guid, TimelineRecord> dictionary = items.ToDictionary<TimelineRecord, Guid>((System.Func<TimelineRecord, Guid>) (x => x.Id));
            foreach (TimelineRecordVariableData recordVariableData in resultCollection.GetCurrent<TimelineRecordVariableData>())
              dictionary[recordVariableData.RecordId].Variables[recordVariableData.Name] = new VariableValue(recordVariableData.Value, recordVariableData.IsSecret);
          }
          timelineRecordsAsync = (IList<TimelineRecord>) items;
        }
      }
      return timelineRecordsAsync;
    }
  }
}
