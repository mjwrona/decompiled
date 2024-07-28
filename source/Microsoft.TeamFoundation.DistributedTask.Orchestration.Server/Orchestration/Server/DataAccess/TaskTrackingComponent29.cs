// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent29
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
  internal class TaskTrackingComponent29 : TaskTrackingComponent28
  {
    public override async Task<Timeline> AddJobsAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid requestedBy,
      IList<TaskOrchestrationJob> jobs,
      IList<TaskReferenceData> tasks,
      IList<TimelineRecord> records,
      IEnumerable<TimelineAttempt> attempts = null)
    {
      TaskTrackingComponent29 component = this;
      Timeline timeline1;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AddJobsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_AddJobs");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindGuid("@requestedBy", requestedBy);
        component.BindJobTable("@jobs", (IEnumerable<TaskOrchestrationJob>) jobs);
        component.BindTaskReferenceTable("@tasks", (IEnumerable<TaskReferenceData>) tasks);
        component.BindTimelineRecordTable("@records", (IEnumerable<TimelineRecord>) records);
        component.BindTimelineAttemptTable("@attempts", attempts);
        Timeline timeline2;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>(component.GetTimelineRecordBinder());
          timeline2 = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (timeline2 != null)
          {
            if (resultCollection.TryNextResult())
              timeline2.Records.AddRange((IEnumerable<TimelineRecord>) resultCollection.GetCurrent<TimelineRecord>().Items);
          }
        }
        timeline1 = timeline2;
      }
      return timeline1;
    }
  }
}
