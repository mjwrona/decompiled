// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent38
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
  internal class TaskTrackingComponent38 : TaskTrackingComponent37
  {
    protected override ObjectBinder<TimelineRecordVariableData> GetTimelineRecordVariableBinder() => (ObjectBinder<TimelineRecordVariableData>) new TimelineRecordVariableBinder2();

    protected override ObjectBinder<TimelineAttemptData> GetTimelineAttemptBinder() => (ObjectBinder<TimelineAttemptData>) new TimelineAttemptBinder2();

    public override async Task<IList<Timeline>> GetAllTimelineAttemptsAsync(
      Guid scopeIdentifier,
      Guid planId,
      string identifier,
      IList<string> includedPhases = null)
    {
      TaskTrackingComponent38 component = this;
      IList<Timeline> timelineAttemptsAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAllTimelineAttemptsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAllTimelineAttempts");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindString("@identifier", identifier, 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindStringTable("@includedPhases", (IEnumerable<string>) includedPhases);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>(component.GetTimelineRecordBinder());
          resultCollection.AddBinder<TimelineAttemptData>(component.GetTimelineAttemptBinder());
          resultCollection.AddBinder<TimelineRecordVariableData>(component.GetTimelineRecordVariableBinder());
          List<Timeline> items1 = resultCollection.GetCurrent<Timeline>().Items;
          if (items1.Count > 0 && resultCollection.TryNextResult())
          {
            List<TimelineRecord> items2 = resultCollection.GetCurrent<TimelineRecord>().Items;
            Dictionary<Guid, Timeline> dictionary1 = items1.ToDictionary<Timeline, Guid>((System.Func<Timeline, Guid>) (k => k.Id));
            Dictionary<Guid, Dictionary<Guid, TimelineRecord>> dictionary2 = new Dictionary<Guid, Dictionary<Guid, TimelineRecord>>();
            foreach (TimelineRecord timelineRecord in items2)
            {
              Dictionary<Guid, Dictionary<Guid, TimelineRecord>> dictionary3 = dictionary2;
              Guid? timelineId = timelineRecord.TimelineId;
              Guid key1 = timelineId.Value;
              Dictionary<Guid, TimelineRecord> dictionary4;
              ref Dictionary<Guid, TimelineRecord> local = ref dictionary4;
              if (!dictionary3.TryGetValue(key1, out local))
              {
                dictionary4 = new Dictionary<Guid, TimelineRecord>();
                Dictionary<Guid, Dictionary<Guid, TimelineRecord>> dictionary5 = dictionary2;
                timelineId = timelineRecord.TimelineId;
                Guid key2 = timelineId.Value;
                Dictionary<Guid, TimelineRecord> dictionary6 = dictionary4;
                dictionary5.Add(key2, dictionary6);
              }
              dictionary4.Add(timelineRecord.Id, timelineRecord);
              Dictionary<Guid, Timeline> dictionary7 = dictionary1;
              timelineId = timelineRecord.TimelineId;
              Guid key3 = timelineId.Value;
              dictionary7[key3].Records.Add(timelineRecord);
            }
            if (resultCollection.TryNextResult())
            {
              foreach (TimelineAttemptData timelineAttemptData in resultCollection.GetCurrent<TimelineAttemptData>())
                dictionary2[timelineAttemptData.TargetTimelineId.Value][timelineAttemptData.TargetId].PreviousAttempts.Add(timelineAttemptData.Attempt);
              if (resultCollection.TryNextResult())
              {
                foreach (TimelineRecordVariableData recordVariableData in resultCollection.GetCurrent<TimelineRecordVariableData>())
                  dictionary2[recordVariableData.TimelineId.Value][recordVariableData.RecordId].Variables[recordVariableData.Name] = new VariableValue(recordVariableData.Value, recordVariableData.IsSecret);
              }
            }
          }
          timelineAttemptsAsync = (IList<Timeline>) items1;
        }
      }
      return timelineAttemptsAsync;
    }
  }
}
