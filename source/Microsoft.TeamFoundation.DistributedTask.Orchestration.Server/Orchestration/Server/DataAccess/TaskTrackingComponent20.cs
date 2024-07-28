// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent20
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent20 : TaskTrackingComponent19
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
      TaskTrackingComponent20 component = this;
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

    public override TaskOrchestrationPlan CreatePlan(
      TaskOrchestrationPlan plan,
      Timeline timeline,
      IEnumerable<TaskOrchestrationJob> jobs,
      IEnumerable<TaskReferenceData> tasks,
      IEnumerable<TimelineAttempt> attempts,
      bool createInitializationLog,
      bool createExpandedYaml)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CreatePlan)))
      {
        this.PrepareStoredProcedure("Task.prc_CreatePlan");
        this.BindDataspaceId(plan.ScopeIdentifier);
        this.BindString("@planType", plan.PlanType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@planVersion", plan.Version);
        this.BindGuid("@planId", plan.PlanId);
        this.BindString("@planGroup", plan.PlanGroup, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@artifactUri", plan.ArtifactUri.ToString(), 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindLong("@containerId", plan.ContainerId);
        this.BindByte("@processType", (byte) plan.Process.ProcessType);
        this.BindBinary("@environment", JsonUtility.Serialize((object) plan.ProcessEnvironment), SqlDbType.VarBinary);
        this.BindBinary("@implementation", JsonUtility.Serialize((object) plan.Process), SqlDbType.VarBinary);
        this.BindJobTable("@jobs", jobs);
        this.BindTaskReferenceTable("@tasks", tasks);
        this.BindGuid("@requestedBy", plan.RequestedById);
        this.BindGuid("@requestedFor", plan.RequestedForId);
        this.BindGuid("@timelineId", timeline.Id);
        this.BindTimelineRecordTable("@timelineRecords", (IEnumerable<TimelineRecord>) timeline.Records);
        this.BindNullableInt("@definitionId", plan.Definition?.Id);
        this.BindString("@definitionName", plan.Definition?.Name ?? string.Empty, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindBinary("@definitionReference", JsonUtility.Serialize((object) plan.Definition, false), int.MaxValue, SqlDbType.VarBinary);
        this.BindNullableInt("@ownerId", plan.Owner?.Id);
        this.BindString("@ownerName", plan.Owner?.Name ?? string.Empty, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindBinary("@ownerReference", JsonUtility.Serialize((object) plan.Owner, false), int.MaxValue, SqlDbType.VarBinary);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlan>(this.GetPlanBinder());
          return resultCollection.GetCurrent<TaskOrchestrationPlan>().FirstOrDefault<TaskOrchestrationPlan>();
        }
      }
    }

    protected override ObjectBinder<TaskOrchestrationPlan> GetPlanBinder() => (ObjectBinder<TaskOrchestrationPlan>) new TaskOrchestrationPlanBinder6((TaskTrackingComponent) this);

    protected override ObjectBinder<TaskOrchestrationPlanReference> GetPlanReferenceBinder() => (ObjectBinder<TaskOrchestrationPlanReference>) new TaskOrchestrationPlanReferenceBinder4((TaskTrackingComponent) this);
  }
}
