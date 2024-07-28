// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskOrchestrationPlanBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal sealed class TaskOrchestrationPlanBinder : ObjectBinder<TaskOrchestrationPlan>
  {
    private SqlColumnBinder m_planId = new SqlColumnBinder("PlanId");
    private SqlColumnBinder m_artifactUri = new SqlColumnBinder("ArtifactUri");
    private SqlColumnBinder m_containerId = new SqlColumnBinder("ContainerId");
    private SqlColumnBinder m_startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder m_finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder m_state = new SqlColumnBinder("State");
    private SqlColumnBinder m_result = new SqlColumnBinder("Result");
    private SqlColumnBinder m_resultCode = new SqlColumnBinder("ResultCode");
    private SqlColumnBinder m_timelineId = new SqlColumnBinder("TimelineId");
    private SqlColumnBinder m_timelineChangeId = new SqlColumnBinder("TimelineChangeId");
    private SqlColumnBinder m_environment = new SqlColumnBinder("Environment");
    private SqlColumnBinder m_implementation = new SqlColumnBinder("Implementation");

    protected override TaskOrchestrationPlan Bind()
    {
      TaskOrchestrationPlan orchestrationPlan = new TaskOrchestrationPlan();
      orchestrationPlan.PlanId = this.m_planId.GetGuid((IDataReader) this.Reader);
      orchestrationPlan.Version = 1;
      orchestrationPlan.ArtifactUri = new Uri(this.m_artifactUri.GetString((IDataReader) this.Reader, false), UriKind.Absolute);
      orchestrationPlan.ContainerId = this.m_containerId.GetInt64((IDataReader) this.Reader);
      orchestrationPlan.StartTime = this.m_startTime.GetNullableDateTime((IDataReader) this.Reader);
      orchestrationPlan.FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      orchestrationPlan.State = (TaskOrchestrationPlanState) this.m_state.GetByte((IDataReader) this.Reader);
      byte? nullableByte = this.m_result.GetNullableByte((IDataReader) this.Reader);
      orchestrationPlan.Result = nullableByte.HasValue ? new TaskResult?((TaskResult) nullableByte.GetValueOrDefault()) : new TaskResult?();
      orchestrationPlan.ResultCode = this.m_resultCode.GetString((IDataReader) this.Reader, true);
      orchestrationPlan.Timeline = new TimelineReference()
      {
        Id = this.m_timelineId.GetGuid((IDataReader) this.Reader),
        ChangeId = this.m_timelineChangeId.GetInt32((IDataReader) this.Reader)
      };
      orchestrationPlan.ProcessEnvironment = JsonUtility.Deserialize<IOrchestrationEnvironment>(this.m_environment.GetBytes((IDataReader) this.Reader, true));
      orchestrationPlan.Process = JsonUtility.Deserialize<IOrchestrationProcess>(this.m_implementation.GetBytes((IDataReader) this.Reader, true));
      return orchestrationPlan;
    }
  }
}
