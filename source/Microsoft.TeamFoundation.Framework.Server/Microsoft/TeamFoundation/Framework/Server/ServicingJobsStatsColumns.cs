// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingJobsStatsColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingJobsStatsColumns : ObjectBinder<ServicingJobsStatsRaw>
  {
    private SqlColumnBinder m_jobResultColumn = new SqlColumnBinder("JobResult");
    private SqlColumnBinder m_jobStatusColumn = new SqlColumnBinder("JobStatus");
    private SqlColumnBinder m_avgDurationInMillisecondsColumn = new SqlColumnBinder("AvgDurationInMilliseconds");
    private SqlColumnBinder m_minDurationInMillisecondsColumn = new SqlColumnBinder("MinDurationInMilliseconds");
    private SqlColumnBinder m_maxDurationInMillisecondsColumn = new SqlColumnBinder("MaxDurationInMilliseconds");
    private SqlColumnBinder m_avgQueueWaitTimeInMillisecondsColumn = new SqlColumnBinder("AvgQueueWaitTimeInMilliseconds");
    private SqlColumnBinder m_jobCountColumn = new SqlColumnBinder("JobCount");

    protected override ServicingJobsStatsRaw Bind() => new ServicingJobsStatsRaw()
    {
      JobResult = (ServicingJobResult) this.m_jobResultColumn.GetInt32((IDataReader) this.Reader, 0),
      JobStatus = (ServicingJobStatus) this.m_jobStatusColumn.GetInt32((IDataReader) this.Reader),
      AvgDurationMilliseconds = this.m_avgDurationInMillisecondsColumn.GetInt32((IDataReader) this.Reader, 0),
      MinDurationMilliseconds = this.m_minDurationInMillisecondsColumn.GetInt32((IDataReader) this.Reader, 0),
      MaxDurationMilliseconds = this.m_maxDurationInMillisecondsColumn.GetInt32((IDataReader) this.Reader, 0),
      AvgQueueWaitTimeMilliseconds = this.m_avgQueueWaitTimeInMillisecondsColumn.GetInt32((IDataReader) this.Reader, 0),
      JobCount = this.m_jobCountColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
