// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent33
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent33 : TaskTrackingComponent32
  {
    private static readonly SqlMetaData[] Task_typ_StringTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.NVarChar, -1L)
    };

    public override IList<TaskOrchestrationPlan> GetPlans(
      Guid scopeIdentifier,
      IList<Guid> planIds,
      IList<string> timelineRecordTypes)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetPlans)))
      {
        this.PrepareStoredProcedure("Task.prc_GetPlans");
        this.BindDataspaceId(scopeIdentifier);
        this.BindSortedGuidTable("@planIds", planIds.Distinct<Guid>());
        this.BindTaskStringTable("@recordTypes", timelineRecordTypes.Distinct<string>((IEqualityComparer<string>) StringComparer.Ordinal));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlan>(this.GetPlanBinder());
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>(this.GetTimelineRecordBinder());
          List<TaskOrchestrationPlan> items = resultCollection.GetCurrent<TaskOrchestrationPlan>().Items;
          resultCollection.NextResult();
          Dictionary<Guid, Timeline> dictionary = resultCollection.GetCurrent<Timeline>().Items.ToDictionary<Timeline, Guid>((System.Func<Timeline, Guid>) (t => t.Id));
          foreach (TaskOrchestrationPlan orchestrationPlan1 in items)
          {
            Timeline timeline1;
            if (dictionary.TryGetValue(orchestrationPlan1.Timeline.Id, out timeline1))
            {
              orchestrationPlan1.Timeline = (TimelineReference) timeline1;
            }
            else
            {
              TaskOrchestrationPlan orchestrationPlan2 = orchestrationPlan1;
              Timeline timeline2 = new Timeline(orchestrationPlan1.Timeline.Id);
              timeline2.ChangeId = orchestrationPlan1.Timeline.ChangeId;
              timeline2.Location = orchestrationPlan1.Timeline.Location;
              orchestrationPlan2.Timeline = (TimelineReference) timeline2;
            }
          }
          resultCollection.NextResult();
          foreach (TimelineRecord timelineRecord in resultCollection.GetCurrent<TimelineRecord>().Items)
          {
            Timeline timeline;
            if (dictionary.TryGetValue(timelineRecord.TimelineId.Value, out timeline))
              timeline.Records.Add(timelineRecord);
          }
          return (IList<TaskOrchestrationPlan>) items;
        }
      }
    }

    protected virtual SqlParameter BindTaskStringTable(
      string parameterName,
      IEnumerable<string> rows)
    {
      rows = rows ?? Enumerable.Empty<string>();
      return this.BindTable(parameterName, "Task.typ_TimelineStringTable", rows.Select<string, SqlDataRecord>(new System.Func<string, SqlDataRecord>(rowBinder)));

      static SqlDataRecord rowBinder(string row)
      {
        SqlDataRecord record = new SqlDataRecord(TaskTrackingComponent33.Task_typ_StringTable);
        record.SetNullableString(0, row);
        return record;
      }
    }
  }
}
