// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskOrchestrationPlanBinder7
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
  internal sealed class TaskOrchestrationPlanBinder7 : ObjectBinder<TaskOrchestrationPlan>
  {
    private TaskTrackingComponent m_component;
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_planType = new SqlColumnBinder("PlanType");
    private SqlColumnBinder m_planVersion = new SqlColumnBinder("PlanVersion");
    private SqlColumnBinder m_planId = new SqlColumnBinder("PlanId");
    private SqlColumnBinder m_planGroup = new SqlColumnBinder("PlanGroup");
    private SqlColumnBinder m_templateType = new SqlColumnBinder("TemplateType");
    private SqlColumnBinder m_artifactUri = new SqlColumnBinder("ArtifactUri");
    private SqlColumnBinder m_containerId = new SqlColumnBinder("ContainerId");
    private SqlColumnBinder m_startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder m_finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder m_state = new SqlColumnBinder("State");
    private SqlColumnBinder m_result = new SqlColumnBinder("Result");
    private SqlColumnBinder m_resultCode = new SqlColumnBinder("ResultCode");
    private SqlColumnBinder m_timelineId = new SqlColumnBinder("TimelineId");
    private SqlColumnBinder m_timelineChangeId = new SqlColumnBinder("TimelineChangeId");
    private SqlColumnBinder m_processType = new SqlColumnBinder("ProcessType");
    private SqlColumnBinder m_environment = new SqlColumnBinder("Environment");
    private SqlColumnBinder m_implementation = new SqlColumnBinder("Implementation");
    private SqlColumnBinder m_requestedById = new SqlColumnBinder("RequestedById");
    private SqlColumnBinder m_requestedForId = new SqlColumnBinder("RequestedForId");
    private SqlColumnBinder m_definitionReference = new SqlColumnBinder("DefinitionReference");
    private SqlColumnBinder m_ownerReference = new SqlColumnBinder("OwnerReference");
    private SqlColumnBinder m_initializationLogId = new SqlColumnBinder("InitializationLogId");
    private SqlColumnBinder m_expandedYamlId = new SqlColumnBinder("ExpandedYamlId");

    public TaskOrchestrationPlanBinder7(TaskTrackingComponent component) => this.m_component = component;

    protected override TaskOrchestrationPlan Bind()
    {
      TaskOrchestrationPlan orchestrationPlan1 = new TaskOrchestrationPlan();
      orchestrationPlan1.ScopeIdentifier = this.m_component.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader));
      orchestrationPlan1.PlanType = this.m_planType.GetString((IDataReader) this.Reader, false);
      orchestrationPlan1.Version = this.m_planVersion.GetInt32((IDataReader) this.Reader);
      orchestrationPlan1.PlanId = this.m_planId.GetGuid((IDataReader) this.Reader);
      orchestrationPlan1.PlanGroup = this.m_planGroup.GetString((IDataReader) this.Reader, true);
      if (this.m_templateType.ColumnExists((IDataReader) this.Reader))
        orchestrationPlan1.TemplateType = (PlanTemplateType) this.m_templateType.GetNullableByte((IDataReader) this.Reader).GetValueOrDefault();
      if (this.m_initializationLogId.ColumnExists((IDataReader) this.Reader))
      {
        int? nullableInt32 = this.m_initializationLogId.GetNullableInt32((IDataReader) this.Reader);
        if (nullableInt32.HasValue)
          orchestrationPlan1.InitializationLog = new TaskLogReference()
          {
            Id = nullableInt32.Value
          };
      }
      if (this.m_expandedYamlId.ColumnExists((IDataReader) this.Reader))
      {
        int? nullableInt32 = this.m_expandedYamlId.GetNullableInt32((IDataReader) this.Reader);
        if (nullableInt32.HasValue)
          orchestrationPlan1.ExpandedYaml = new TaskLogReference()
          {
            Id = nullableInt32.Value
          };
      }
      orchestrationPlan1.ArtifactUri = new Uri(this.m_artifactUri.GetString((IDataReader) this.Reader, false), UriKind.Absolute);
      orchestrationPlan1.ContainerId = this.m_containerId.GetInt64((IDataReader) this.Reader);
      orchestrationPlan1.StartTime = this.m_startTime.GetNullableDateTime((IDataReader) this.Reader);
      orchestrationPlan1.FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      orchestrationPlan1.State = (TaskOrchestrationPlanState) this.m_state.GetByte((IDataReader) this.Reader);
      TaskOrchestrationPlan orchestrationPlan2 = orchestrationPlan1;
      byte? nullableByte = this.m_result.GetNullableByte((IDataReader) this.Reader);
      TaskResult? nullable = nullableByte.HasValue ? new TaskResult?((TaskResult) nullableByte.GetValueOrDefault()) : new TaskResult?();
      orchestrationPlan2.Result = nullable;
      orchestrationPlan1.ResultCode = this.m_resultCode.GetString((IDataReader) this.Reader, true);
      orchestrationPlan1.Timeline = new TimelineReference()
      {
        Id = this.m_timelineId.GetGuid((IDataReader) this.Reader),
        ChangeId = this.m_timelineChangeId.GetInt32((IDataReader) this.Reader)
      };
      orchestrationPlan1.ProcessType = (OrchestrationProcessType) this.m_processType.GetByte((IDataReader) this.Reader);
      orchestrationPlan1.ProcessEnvironmentSerialized = this.m_environment.GetBytes((IDataReader) this.Reader, true);
      orchestrationPlan1.ProcessSerialized = this.m_implementation.GetBytes((IDataReader) this.Reader, true);
      orchestrationPlan1.RequestedById = this.m_requestedById.GetGuid((IDataReader) this.Reader, true);
      orchestrationPlan1.RequestedForId = this.m_requestedForId.GetGuid((IDataReader) this.Reader, true);
      orchestrationPlan1.Definition = JsonUtility.Deserialize<TaskOrchestrationOwner>(this.m_definitionReference.GetBytes((IDataReader) this.Reader, true));
      orchestrationPlan1.Owner = JsonUtility.Deserialize<TaskOrchestrationOwner>(this.m_ownerReference.GetBytes((IDataReader) this.Reader, true));
      return orchestrationPlan1;
    }
  }
}
