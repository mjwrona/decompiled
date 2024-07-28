// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.DeploymentExecutionRecordObjectBinder2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Data.Model;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class DeploymentExecutionRecordObjectBinder2 : 
    ObjectBinder<DeploymentExecutionRecordObject>
  {
    private SqlColumnBinder m_environmentId = new SqlColumnBinder("EnvironmentId");
    private SqlColumnBinder m_environmentName = new SqlColumnBinder("EnvironmentName");
    private SqlColumnBinder m_id = new SqlColumnBinder("Id");
    private SqlColumnBinder m_resourceId = new SqlColumnBinder("EnvironmentResourceId");
    private SqlColumnBinder m_resourceName = new SqlColumnBinder("EnvironmentResourceName");
    private SqlColumnBinder m_resourceType = new SqlColumnBinder("EnvironmentResourceType");
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

    public DeploymentExecutionRecordObjectBinder2(
      EnvironmentDeploymentExecutionHistoryComponent sqlComponent)
    {
      this.SqlComponent = (TaskSqlComponentBase) sqlComponent;
    }

    protected override DeploymentExecutionRecordObject Bind()
    {
      DeploymentExecutionRecordObject executionRecordObject1 = new DeploymentExecutionRecordObject();
      executionRecordObject1.EnvironmentReference = new EnvironmentReference()
      {
        Id = this.m_environmentId.GetInt32((IDataReader) this.Reader),
        Name = this.m_environmentName.GetString((IDataReader) this.Reader, true)
      };
      executionRecordObject1.Id = this.m_id.GetInt64((IDataReader) this.Reader);
      int? nullableInt32 = this.m_resourceId.GetNullableInt32((IDataReader) this.Reader);
      if (nullableInt32.HasValue)
        executionRecordObject1.ResourceReference = new EnvironmentResourceReference()
        {
          Id = nullableInt32.Value,
          Name = this.m_resourceName.GetString((IDataReader) this.Reader, true),
          Type = (EnvironmentResourceType) this.m_resourceType.GetByte((IDataReader) this.Reader)
        };
      executionRecordObject1.ServiceOwner = this.m_serviceOwner.GetGuid((IDataReader) this.Reader, false);
      executionRecordObject1.ScopeId = this.m_scopeId.GetGuid((IDataReader) this.Reader, false);
      executionRecordObject1.PlanType = this.m_planType.GetString((IDataReader) this.Reader, false);
      executionRecordObject1.PlanId = this.m_planId.GetGuid((IDataReader) this.Reader, false);
      executionRecordObject1.RequestIdentifier = this.m_requestIdentifier.GetString((IDataReader) this.Reader, false);
      executionRecordObject1.StageName = this.m_stageName.GetString((IDataReader) this.Reader, false);
      executionRecordObject1.JobName = this.m_jobName.GetString((IDataReader) this.Reader, false);
      executionRecordObject1.StageAttempt = this.m_stageAttempt.GetInt32((IDataReader) this.Reader);
      executionRecordObject1.JobAttempt = this.m_jobAttempt.GetInt32((IDataReader) this.Reader);
      executionRecordObject1.QueueTime = this.m_queueTime.GetDateTime((IDataReader) this.Reader);
      executionRecordObject1.StartTime = this.m_startTime.GetNullableDateTime((IDataReader) this.Reader);
      executionRecordObject1.FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      DeploymentExecutionRecordObject executionRecordObject2 = executionRecordObject1;
      byte? nullableByte = this.m_result.GetNullableByte((IDataReader) this.Reader);
      TaskResult? nullable = nullableByte.HasValue ? new TaskResult?((TaskResult) nullableByte.GetValueOrDefault()) : new TaskResult?();
      executionRecordObject2.Result = nullable;
      string json1 = this.m_owner.GetString((IDataReader) this.Reader, false);
      string json2 = this.m_definition.GetString((IDataReader) this.Reader, false);
      executionRecordObject1.Owner = string.IsNullOrEmpty(json1) ? (TaskOrchestrationOwner) null : JsonUtilities.Deserialize<TaskOrchestrationOwner>(json1);
      executionRecordObject1.Definition = string.IsNullOrEmpty(json2) ? (TaskOrchestrationOwner) null : JsonUtilities.Deserialize<TaskOrchestrationOwner>(json2);
      return executionRecordObject1;
    }

    private TaskSqlComponentBase SqlComponent { get; }
  }
}
