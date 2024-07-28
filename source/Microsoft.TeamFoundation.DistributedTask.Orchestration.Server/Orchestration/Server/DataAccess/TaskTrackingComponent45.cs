// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent45
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
  internal class TaskTrackingComponent45 : TaskTrackingComponent44
  {
    public override async Task<Timeline> UpdateTimelineAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid requestedBy,
      IList<TimelineRecord> records,
      int blockingPeriod)
    {
      TaskTrackingComponent45 component = this;
      Timeline timeline1;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateTimelineAsync)))
      {
        bool parameterValue = blockingPeriod != 0;
        if (parameterValue)
        {
          component.PrepareStoredProcedure("Task.prc_UpdateTimelineV2");
          component.BindBoolean("@blockTimelineUpdates", parameterValue);
          component.BindInt("@blockingPeriodForTimelineUpdates", blockingPeriod);
        }
        else
          component.PrepareStoredProcedure("Task.prc_UpdateTimeline");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindGuid("@timelineId", timelineId);
        component.BindGuid("@requestedBy", requestedBy);
        component.BindTimelineRecordTable("@records", (IEnumerable<TimelineRecord>) records);
        List<TimelineRecordVariableData> rows = new List<TimelineRecordVariableData>();
        if (records == null)
          records = (IList<TimelineRecord>) Array.Empty<TimelineRecord>();
        foreach (TimelineRecord record in (IEnumerable<TimelineRecord>) records)
        {
          if (record.Variables.Count > 0)
          {
            foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) record.Variables)
            {
              TimelineRecordVariableData recordVariableData = new TimelineRecordVariableData()
              {
                RecordId = record.Id,
                Name = variable.Key,
                IsSecret = variable.Value.IsSecret,
                Value = variable.Value.Value
              };
              rows.Add(recordVariableData);
            }
          }
        }
        component.BindTimelineRecordVariableTable("@recordVariables", (IEnumerable<TimelineRecordVariableData>) rows);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>(component.GetTimelineRecordBinder());
          resultCollection.AddBinder<TimelineAttemptData>(component.GetTimelineAttemptBinder());
          resultCollection.AddBinder<TimelineRecordVariableData>(component.GetTimelineRecordVariableBinder());
          Timeline timeline2 = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (timeline2 != null && resultCollection.TryNextResult())
          {
            Dictionary<Guid, TimelineRecord> dictionary = resultCollection.GetCurrent<TimelineRecord>().Items.ToDictionary<TimelineRecord, Guid, TimelineRecord>((System.Func<TimelineRecord, Guid>) (k => k.Id), (System.Func<TimelineRecord, TimelineRecord>) (v => v));
            if (resultCollection.TryNextResult())
            {
              foreach (TimelineAttemptData timelineAttemptData in resultCollection.GetCurrent<TimelineAttemptData>())
                dictionary[timelineAttemptData.TargetId].PreviousAttempts.Add(timelineAttemptData.Attempt);
              if (resultCollection.TryNextResult())
              {
                foreach (TimelineRecordVariableData recordVariableData in resultCollection.GetCurrent<TimelineRecordVariableData>().Items ?? new List<TimelineRecordVariableData>())
                  dictionary[recordVariableData.RecordId].Variables[recordVariableData.Name] = new VariableValue(recordVariableData.Value, recordVariableData.IsSecret);
              }
            }
            timeline2.Records.AddRange((IEnumerable<TimelineRecord>) dictionary.Values);
          }
          timeline1 = timeline2;
        }
      }
      return timeline1;
    }
  }
}
