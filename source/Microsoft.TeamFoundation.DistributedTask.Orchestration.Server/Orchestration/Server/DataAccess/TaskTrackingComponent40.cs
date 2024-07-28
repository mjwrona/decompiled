// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent40
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
  internal class TaskTrackingComponent40 : TaskTrackingComponent39
  {
    public override async Task<Timeline> GetTimelineAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      int changeId = 0,
      bool includeRecords = true,
      bool includePreviousAttempts = true)
    {
      TaskTrackingComponent40 component = this;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetTimelineAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetTimeline");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindNullableGuid("@timelineId", new Guid?(timelineId), true);
        component.BindInt("@changeId", changeId);
        component.BindBoolean("@includeRecords", includeRecords);
        component.BindBoolean("@includeAttempts", includePreviousAttempts);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>(component.GetTimelineRecordBinder());
          if (includePreviousAttempts)
            resultCollection.AddBinder<TimelineAttemptData>(component.GetTimelineAttemptBinder());
          resultCollection.AddBinder<TimelineRecordVariableData>(component.GetTimelineRecordVariableBinder());
          Timeline timelineAsync = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (timelineAsync == null || !resultCollection.TryNextResult())
            return timelineAsync;
          timelineAsync.Records.AddRange((IEnumerable<TimelineRecord>) resultCollection.GetCurrent<TimelineRecord>().Items);
          Dictionary<Guid, TimelineRecord> dictionary = (Dictionary<Guid, TimelineRecord>) null;
          if (includePreviousAttempts && resultCollection.TryNextResult())
          {
            List<TimelineAttemptData> items = resultCollection.GetCurrent<TimelineAttemptData>().Items;
            if (items.Count > 0)
            {
              dictionary = component.InitRecordsMap(timelineAsync.Records);
              foreach (TimelineAttemptData timelineAttemptData in items)
              {
                if (dictionary.ContainsKey(timelineAttemptData.TargetId))
                  dictionary[timelineAttemptData.TargetId].PreviousAttempts.Add(timelineAttemptData.Attempt);
              }
            }
          }
          if (resultCollection.TryNextResult())
          {
            List<TimelineRecordVariableData> items = resultCollection.GetCurrent<TimelineRecordVariableData>().Items;
            if (items.Count > 0)
            {
              if (dictionary == null)
                dictionary = component.InitRecordsMap(timelineAsync.Records);
              foreach (TimelineRecordVariableData recordVariableData in items)
              {
                if (dictionary.ContainsKey(recordVariableData.RecordId))
                  dictionary[recordVariableData.RecordId].Variables[recordVariableData.Name] = new VariableValue(recordVariableData.Value, recordVariableData.IsSecret);
              }
            }
          }
          return timelineAsync;
        }
      }
    }

    private Dictionary<Guid, TimelineRecord> InitRecordsMap(List<TimelineRecord> records)
    {
      Dictionary<Guid, TimelineRecord> dictionary = new Dictionary<Guid, TimelineRecord>(records.Count);
      foreach (TimelineRecord record in records)
        dictionary.Add(record.Id, record);
      return dictionary;
    }
  }
}
