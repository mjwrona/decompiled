// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.JobReportingQueryHistoryColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class JobReportingQueryHistoryColumns : ObjectBinder<TeamFoundationJobReportingHistory>
  {
    private int m_serviceVersion;
    private SqlColumnBinder HistoryIdColumn = new SqlColumnBinder("HistoryId");
    private SqlColumnBinder JobSourceColumn = new SqlColumnBinder("JobSource");
    private SqlColumnBinder JobIdColumn = new SqlColumnBinder("JobId");
    private SqlColumnBinder QueueTimeColumn = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder StartTimeColumn = new SqlColumnBinder("StartTime");
    private SqlColumnBinder EndTimeColumn = new SqlColumnBinder("EndTime");
    private SqlColumnBinder AgentIdColumn = new SqlColumnBinder("AgentId");
    private SqlColumnBinder ResultColumn = new SqlColumnBinder("Result");
    private SqlColumnBinder ResultMessageColumn = new SqlColumnBinder("ResultMessage");
    private SqlColumnBinder QueuedReasonsColumn = new SqlColumnBinder("QueuedReasons");
    private SqlColumnBinder QueueFlagsColumn = new SqlColumnBinder("QueueFlags");
    private SqlColumnBinder PriorityColumn = new SqlColumnBinder("Priority");

    public JobReportingQueryHistoryColumns(int serviceVersion) => this.m_serviceVersion = serviceVersion;

    protected override TeamFoundationJobReportingHistory Bind()
    {
      int num = this.m_serviceVersion != 1 ? (int) this.PriorityColumn.GetByte((IDataReader) this.Reader) : this.PriorityColumn.GetInt32((IDataReader) this.Reader);
      return new TeamFoundationJobReportingHistory()
      {
        HistoryId = this.HistoryIdColumn.GetInt64((IDataReader) this.Reader),
        JobSource = this.JobSourceColumn.GetGuid((IDataReader) this.Reader),
        JobId = this.JobIdColumn.GetGuid((IDataReader) this.Reader),
        QueueTime = this.QueueTimeColumn.GetDateTime((IDataReader) this.Reader),
        StartTime = this.StartTimeColumn.GetDateTime((IDataReader) this.Reader),
        EndTime = this.EndTimeColumn.GetDateTime((IDataReader) this.Reader),
        AgentId = this.AgentIdColumn.GetGuid((IDataReader) this.Reader),
        Result = (TeamFoundationJobResult) this.ResultColumn.GetByte((IDataReader) this.Reader),
        ResultMessage = this.ResultMessageColumn.GetString((IDataReader) this.Reader, true),
        QueuedReasons = (TeamFoundationJobQueuedReasons) this.QueuedReasonsColumn.GetInt32((IDataReader) this.Reader),
        QueueFlags = this.QueueFlagsColumn.GetInt32((IDataReader) this.Reader),
        Priority = num
      };
    }
  }
}
