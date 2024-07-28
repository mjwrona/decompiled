// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.TaskPlanBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  internal class TaskPlanBinder : TaskAnalyticsDataBinderBase<TaskPlan>
  {
    private Guid m_projectGuid;
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder internalId = new SqlColumnBinder("InternalId");
    private SqlColumnBinder planId = new SqlColumnBinder("PlanId");
    private SqlColumnBinder planType = new SqlColumnBinder("PlanType");
    private SqlColumnBinder planVersion = new SqlColumnBinder("PlanVersion");
    private SqlColumnBinder planGroup = new SqlColumnBinder("PlanGroup");
    private SqlColumnBinder templateType = new SqlColumnBinder("TemplateType");
    private SqlColumnBinder changeId = new SqlColumnBinder("ChangeId");
    private SqlColumnBinder artifactUri = new SqlColumnBinder("ArtifactUri");
    private SqlColumnBinder timelineId = new SqlColumnBinder("TimelineId");
    private SqlColumnBinder state = new SqlColumnBinder("State");
    private SqlColumnBinder startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder lastUpdated = new SqlColumnBinder("LastUpdated");
    private SqlColumnBinder result = new SqlColumnBinder("Result");
    private SqlColumnBinder resultCode = new SqlColumnBinder("ResultCode");
    private SqlColumnBinder processType = new SqlColumnBinder("ProcessType");
    private SqlColumnBinder requestedById = new SqlColumnBinder("RequestedById");
    private SqlColumnBinder requestedForId = new SqlColumnBinder("RequestedForId");
    private SqlColumnBinder definitionName = new SqlColumnBinder("DefinitionName");
    private SqlColumnBinder ownerName = new SqlColumnBinder("OwnerName");

    public TaskPlanBinder(TaskSqlComponentBase sqlComponent, Guid projectGuid)
      : base(sqlComponent)
    {
      this.m_projectGuid = projectGuid;
    }

    protected override TaskPlan Bind()
    {
      TaskPlan taskPlan = new TaskPlan();
      taskPlan.ProjectGuid = this.m_projectGuid;
      taskPlan.PlanGuid = new Guid?(this.planId.GetGuid((IDataReader) this.Reader));
      taskPlan.TimelineGuid = new Guid?(this.timelineId.GetGuid((IDataReader) this.Reader));
      taskPlan.PlanId = this.internalId.GetInt32((IDataReader) this.Reader);
      taskPlan.PlanType = this.planType.GetString((IDataReader) this.Reader, true);
      taskPlan.PlanVersion = new int?(this.planVersion.GetInt32((IDataReader) this.Reader));
      taskPlan.PlanGroup = this.planGroup.GetString((IDataReader) this.Reader, true);
      taskPlan.ProcessType = new int?((int) this.processType.GetByte((IDataReader) this.Reader, (byte) 1));
      taskPlan.TemplateType = new int?((int) this.templateType.GetByte((IDataReader) this.Reader, (byte) 0));
      taskPlan.ChangeId = new int?(this.changeId.GetInt32((IDataReader) this.Reader));
      taskPlan.ArtifactUri = this.artifactUri.GetString((IDataReader) this.Reader, true);
      taskPlan.State = new int?((int) this.state.GetByte((IDataReader) this.Reader));
      taskPlan.Result = new int?((int) this.result.GetByte((IDataReader) this.Reader, (byte) 3));
      taskPlan.ResultCode = this.resultCode.GetString((IDataReader) this.Reader, true);
      taskPlan.DefinitionName = this.definitionName.GetString((IDataReader) this.Reader, true);
      taskPlan.OwnerName = this.ownerName.GetString((IDataReader) this.Reader, true);
      taskPlan.RequestedByGuid = new Guid?(this.requestedById.GetGuid((IDataReader) this.Reader, true));
      taskPlan.RequestedForGuid = new Guid?(this.requestedForId.GetGuid((IDataReader) this.Reader, true));
      taskPlan.StartTime = this.startTime.GetNullableDateTime((IDataReader) this.Reader);
      taskPlan.FinishTime = this.finishTime.GetNullableDateTime((IDataReader) this.Reader);
      taskPlan.LastUpdated = this.lastUpdated.GetDateTime((IDataReader) this.Reader);
      return taskPlan;
    }
  }
}
