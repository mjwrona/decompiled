// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.EnvironmentDeploymentExecutionRecordBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class EnvironmentDeploymentExecutionRecordBinder : 
    ObjectBinder<EnvironmentDeploymentExecutionRecord>
  {
    private SqlColumnBinder m_environmentId = new SqlColumnBinder("EnvironmentId");
    private SqlColumnBinder m_id = new SqlColumnBinder("Id");
    private SqlColumnBinder m_resourceId = new SqlColumnBinder("EnvironmentResourceId");
    private SqlColumnBinder m_serviceOwner = new SqlColumnBinder("ServiceOwner");
    private SqlColumnBinder m_scopeId = new SqlColumnBinder("ScopeId");
    private SqlColumnBinder m_planType = new SqlColumnBinder("PlanType");
    private SqlColumnBinder m_planId = new SqlColumnBinder("PlanId");
    private SqlColumnBinder m_requestIdentifier = new SqlColumnBinder("RequestIdentifier");
    private SqlColumnBinder m_stageName = new SqlColumnBinder("StageName");
    private SqlColumnBinder m_jobName = new SqlColumnBinder("JobName");
    private SqlColumnBinder m_stageAttempt = new SqlColumnBinder("StageAttempt");
    private SqlColumnBinder m_jobAttempt = new SqlColumnBinder("JobAttempt");
    private SqlColumnBinder m_definition = new SqlColumnBinder("Definition");
    private SqlColumnBinder m_owner = new SqlColumnBinder("Owner");
    private SqlColumnBinder m_queueTime = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder m_startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder m_finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder m_result = new SqlColumnBinder("Result");

    public EnvironmentDeploymentExecutionRecordBinder(
      EnvironmentDeploymentExecutionHistoryComponent sqlComponent)
    {
      this.SqlComponent = (TaskSqlComponentBase) sqlComponent;
    }

    protected override EnvironmentDeploymentExecutionRecord Bind()
    {
      EnvironmentDeploymentExecutionRecord deploymentExecutionRecord = new EnvironmentDeploymentExecutionRecord();
      deploymentExecutionRecord.EnvironmentId = this.m_environmentId.GetInt32((IDataReader) this.Reader);
      deploymentExecutionRecord.Id = this.m_id.GetInt64((IDataReader) this.Reader);
      deploymentExecutionRecord.ResourceId = this.m_resourceId.ColumnExists((IDataReader) this.Reader) ? this.m_resourceId.GetNullableInt32((IDataReader) this.Reader) : new int?();
      deploymentExecutionRecord.ServiceOwner = this.m_serviceOwner.GetGuid((IDataReader) this.Reader, false);
      deploymentExecutionRecord.ScopeId = this.m_scopeId.GetGuid((IDataReader) this.Reader, false);
      deploymentExecutionRecord.PlanType = this.m_planType.GetString((IDataReader) this.Reader, false);
      deploymentExecutionRecord.PlanId = this.m_planId.GetGuid((IDataReader) this.Reader, false);
      deploymentExecutionRecord.RequestIdentifier = this.m_requestIdentifier.GetString((IDataReader) this.Reader, false);
      deploymentExecutionRecord.StageName = this.m_stageName.GetString((IDataReader) this.Reader, false);
      deploymentExecutionRecord.JobName = this.m_jobName.GetString((IDataReader) this.Reader, false);
      deploymentExecutionRecord.StageAttempt = this.m_stageAttempt.GetInt32((IDataReader) this.Reader);
      deploymentExecutionRecord.JobAttempt = this.m_jobAttempt.GetInt32((IDataReader) this.Reader);
      deploymentExecutionRecord.QueueTime = this.m_queueTime.GetDateTime((IDataReader) this.Reader);
      deploymentExecutionRecord.StartTime = this.m_startTime.GetNullableDateTime((IDataReader) this.Reader);
      deploymentExecutionRecord.FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      byte? nullableByte = this.m_result.GetNullableByte((IDataReader) this.Reader);
      deploymentExecutionRecord.Result = nullableByte.HasValue ? new TaskResult?((TaskResult) nullableByte.GetValueOrDefault()) : new TaskResult?();
      string json1 = this.m_owner.GetString((IDataReader) this.Reader, false);
      string json2 = this.m_definition.GetString((IDataReader) this.Reader, false);
      deploymentExecutionRecord.Owner = string.IsNullOrEmpty(json1) ? (TaskOrchestrationOwner) null : JsonUtilities.Deserialize<TaskOrchestrationOwner>(json1);
      deploymentExecutionRecord.Definition = string.IsNullOrEmpty(json2) ? (TaskOrchestrationOwner) null : JsonUtilities.Deserialize<TaskOrchestrationOwner>(json2);
      return deploymentExecutionRecord;
    }

    private TaskSqlComponentBase SqlComponent { get; }
  }
}
