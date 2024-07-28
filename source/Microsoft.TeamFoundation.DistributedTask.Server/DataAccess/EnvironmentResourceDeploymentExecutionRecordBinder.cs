// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.EnvironmentResourceDeploymentExecutionRecordBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class EnvironmentResourceDeploymentExecutionRecordBinder : 
    ObjectBinder<EnvironmentResourceDeploymentExecutionRecord>
  {
    private SqlColumnBinder m_environmentId = new SqlColumnBinder("EnvironmentId");
    private SqlColumnBinder m_requestId = new SqlColumnBinder("RequestId");
    private SqlColumnBinder m_resourceId = new SqlColumnBinder("ResourceId");
    private SqlColumnBinder m_startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder m_finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder m_result = new SqlColumnBinder("Result");

    public EnvironmentResourceDeploymentExecutionRecordBinder(
      EnvironmentDeploymentExecutionHistoryComponent sqlComponent)
    {
      this.SqlComponent = (TaskSqlComponentBase) sqlComponent;
    }

    protected override EnvironmentResourceDeploymentExecutionRecord Bind()
    {
      EnvironmentResourceDeploymentExecutionRecord deploymentExecutionRecord = new EnvironmentResourceDeploymentExecutionRecord();
      deploymentExecutionRecord.EnvironmentId = this.m_environmentId.GetInt32((IDataReader) this.Reader);
      deploymentExecutionRecord.RequestId = this.m_requestId.GetInt64((IDataReader) this.Reader);
      deploymentExecutionRecord.ResourceId = this.m_resourceId.GetInt32((IDataReader) this.Reader);
      deploymentExecutionRecord.StartTime = this.m_startTime.GetDateTime((IDataReader) this.Reader);
      deploymentExecutionRecord.FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      byte? nullableByte = this.m_result.GetNullableByte((IDataReader) this.Reader);
      deploymentExecutionRecord.Result = nullableByte.HasValue ? new TaskResult?((TaskResult) nullableByte.GetValueOrDefault()) : new TaskResult?();
      return deploymentExecutionRecord;
    }

    private TaskSqlComponentBase SqlComponent { get; }
  }
}
