// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingJobInfoColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingJobInfoColumns : ObjectBinder<ServicingJobInfo>
  {
    private SqlColumnBinder m_jobIdColumn = new SqlColumnBinder("JobId");
    private SqlColumnBinder m_hostIdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder m_operationClassColumn = new SqlColumnBinder("OperationClass");
    private SqlColumnBinder m_operationsColumn = new SqlColumnBinder("Operations");
    private SqlColumnBinder m_databaseIdColumn = new SqlColumnBinder("DatabaseId");
    private SqlColumnBinder m_databaseNameColumn = new SqlColumnBinder("DatabaseName");
    private SqlColumnBinder m_poolNameColumn = new SqlColumnBinder("PoolName");
    private SqlColumnBinder m_poolIdColumn = new SqlColumnBinder("PoolId");
    private SqlColumnBinder m_accountIdColumn = new SqlColumnBinder("AccountId");
    private SqlColumnBinder m_jobStatusColumn = new SqlColumnBinder("JobStatus");
    private SqlColumnBinder m_jobResultColumn = new SqlColumnBinder("JobResult");
    private SqlColumnBinder m_queueTimeColumn = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder m_startTimeColumn = new SqlColumnBinder("StartTime");
    private SqlColumnBinder m_endTimeColumn = new SqlColumnBinder("EndTime");
    private SqlColumnBinder m_completedStepCountColumn = new SqlColumnBinder("CompletedStepCount");
    private SqlColumnBinder m_totalStepCountColumn = new SqlColumnBinder("TotalStepCount");
    private SqlColumnBinder m_nameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder m_parentNameColumn = new SqlColumnBinder("ParentName");

    protected override ServicingJobInfo Bind()
    {
      ServicingJobInfo servicingJobInfo = new ServicingJobInfo()
      {
        JobId = this.m_jobIdColumn.GetGuid((IDataReader) this.Reader),
        HostId = this.m_hostIdColumn.GetGuid((IDataReader) this.Reader),
        OperationClass = this.m_operationClassColumn.GetString((IDataReader) this.Reader, true),
        Operations = this.m_operationsColumn.GetString((IDataReader) this.Reader, (string) null),
        DatabaseId = this.m_databaseIdColumn.GetInt32((IDataReader) this.Reader, 0),
        DatabaseName = this.m_databaseNameColumn.GetString((IDataReader) this.Reader, true),
        PoolName = this.m_poolNameColumn.GetString((IDataReader) this.Reader, true),
        PoolId = this.m_poolIdColumn.GetInt32((IDataReader) this.Reader, 0),
        JobStatus = (ServicingJobStatus) this.m_jobStatusColumn.GetInt32((IDataReader) this.Reader),
        JobResult = (ServicingJobResult) this.m_jobResultColumn.GetInt32((IDataReader) this.Reader, 0),
        QueueTime = this.m_queueTimeColumn.GetDateTime((IDataReader) this.Reader),
        StartTime = new DateTime?(this.m_startTimeColumn.GetDateTime((IDataReader) this.Reader)),
        EndTime = new DateTime?(this.m_endTimeColumn.GetDateTime((IDataReader) this.Reader)),
        CompletedStepCount = this.m_completedStepCountColumn.GetInt16((IDataReader) this.Reader, (short) 0),
        TotalStepCount = this.m_totalStepCountColumn.GetInt16((IDataReader) this.Reader, (short) 0),
        Name = this.m_nameColumn.GetString((IDataReader) this.Reader, false),
        ParentName = this.m_parentNameColumn.GetString((IDataReader) this.Reader, false)
      };
      servicingJobInfo.AccountId = servicingJobInfo.HostId;
      if (!this.m_startTimeColumn.IsNull((IDataReader) this.Reader))
        servicingJobInfo.StartTime = new DateTime?(this.m_startTimeColumn.GetDateTime((IDataReader) this.Reader));
      if (!this.m_endTimeColumn.IsNull((IDataReader) this.Reader))
        servicingJobInfo.EndTime = new DateTime?(this.m_endTimeColumn.GetDateTime((IDataReader) this.Reader));
      if (string.Equals(servicingJobInfo.OperationClass, "UpgradeDatabase", StringComparison.Ordinal))
        servicingJobInfo.DatabaseId = TeamFoundationServicingService.GetDatabaseId(servicingJobInfo.HostId);
      servicingJobInfo.LogUri = "/_apis/ops/servicing/logs/" + this.m_jobIdColumn.GetGuid((IDataReader) this.Reader).ToString("D");
      return servicingJobInfo;
    }
  }
}
