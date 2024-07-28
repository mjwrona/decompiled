// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingJobDetailColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingJobDetailColumns : ObjectBinder<ServicingJobDetail>
  {
    private SqlColumnBinder m_hostIdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder m_jobIdColumn = new SqlColumnBinder("JobId");
    private SqlColumnBinder m_operationClassColumn = new SqlColumnBinder("OperationClass");
    private SqlColumnBinder m_resultColumn = new SqlColumnBinder("Result");
    private SqlColumnBinder m_jobStatusColumn = new SqlColumnBinder("JobStatus");
    private SqlColumnBinder m_operationsColumn = new SqlColumnBinder("Operations");
    private SqlColumnBinder m_queueTimeColumn = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder m_startTimeColumn = new SqlColumnBinder("StartTime");
    private SqlColumnBinder m_endTimeColumn = new SqlColumnBinder("EndTime");
    private SqlColumnBinder m_queuePositionColumn = new SqlColumnBinder("QueuePosition");
    private SqlColumnBinder m_completedStepCount = new SqlColumnBinder("CompletedStepCount");
    private SqlColumnBinder m_totalStepCount = new SqlColumnBinder("TotalStepCount");
    private static readonly char[] s_commaArray = new char[1]
    {
      ','
    };

    protected override ServicingJobDetail Bind() => new ServicingJobDetail()
    {
      HostId = this.m_hostIdColumn.GetGuid((IDataReader) this.Reader),
      JobId = this.m_jobIdColumn.GetGuid((IDataReader) this.Reader),
      OperationClass = this.m_operationClassColumn.GetString((IDataReader) this.Reader, false),
      Result = (ServicingJobResult) this.m_resultColumn.GetInt32((IDataReader) this.Reader, 0),
      JobStatus = (ServicingJobStatus) this.m_jobStatusColumn.GetInt32((IDataReader) this.Reader),
      Operations = this.m_operationsColumn.GetString((IDataReader) this.Reader, false).Split(ServicingJobDetailColumns.s_commaArray, StringSplitOptions.RemoveEmptyEntries),
      QueueTime = this.m_queueTimeColumn.GetDateTime((IDataReader) this.Reader),
      StartTime = this.m_startTimeColumn.GetDateTime((IDataReader) this.Reader),
      EndTime = this.m_endTimeColumn.GetDateTime((IDataReader) this.Reader),
      CompletedStepCount = this.m_completedStepCount.GetInt16((IDataReader) this.Reader, (short) -1, (short) -1),
      TotalStepCount = this.m_totalStepCount.GetInt16((IDataReader) this.Reader, (short) -1, (short) -1),
      QueuePosition = this.m_queuePositionColumn.GetInt32((IDataReader) this.Reader, -1, -1)
    };
  }
}
