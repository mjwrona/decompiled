// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.JobReportingQueuePositionsColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class JobReportingQueuePositionsColumns : 
    ObjectBinder<TeamFoundationJobReportingQueuePositions>
  {
    private int m_serviceVersion;
    private SqlColumnBinder QueuePositionColumn = new SqlColumnBinder("QueuePosition");
    private SqlColumnBinder StateTimeColumn = new SqlColumnBinder("StateTime");
    private SqlColumnBinder JobIdColumn = new SqlColumnBinder("JobId");
    private SqlColumnBinder HostIdColumn = new SqlColumnBinder("hostId");
    private SqlColumnBinder QueueTimeColumn = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder PriorityColumn = new SqlColumnBinder("Priority");

    public JobReportingQueuePositionsColumns(int serviceVersion) => this.m_serviceVersion = serviceVersion;

    protected override TeamFoundationJobReportingQueuePositions Bind()
    {
      int num = this.m_serviceVersion != 1 ? (int) this.PriorityColumn.GetByte((IDataReader) this.Reader) : this.PriorityColumn.GetInt32((IDataReader) this.Reader);
      return new TeamFoundationJobReportingQueuePositions()
      {
        QueuePosition = this.QueuePositionColumn.GetInt32((IDataReader) this.Reader),
        StateTime = this.StateTimeColumn.GetInt32((IDataReader) this.Reader, -1),
        JobId = this.JobIdColumn.GetGuid((IDataReader) this.Reader),
        HostId = this.HostIdColumn.GetGuid((IDataReader) this.Reader),
        QueueTime = this.QueueTimeColumn.GetDateTime((IDataReader) this.Reader),
        Priority = num
      };
    }
  }
}
