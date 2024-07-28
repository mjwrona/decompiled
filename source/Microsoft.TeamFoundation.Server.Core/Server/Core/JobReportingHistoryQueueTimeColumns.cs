// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.JobReportingHistoryQueueTimeColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class JobReportingHistoryQueueTimeColumns : 
    ObjectBinder<TeamFoundationJobReportingJobCountsAndRunTime>
  {
    private SqlColumnBinder StartTimeColumn = new SqlColumnBinder("StartTime");
    private SqlColumnBinder AvgRunTimeInMsColumn = new SqlColumnBinder("AvgRunTimeInMs");
    private SqlColumnBinder AvgQueueTimeInMsColumn = new SqlColumnBinder("AvgQueueTimeInMs");
    private SqlColumnBinder CountColumn = new SqlColumnBinder("count");

    protected override TeamFoundationJobReportingJobCountsAndRunTime Bind() => new TeamFoundationJobReportingJobCountsAndRunTime()
    {
      StartTime = this.StartTimeColumn.GetDateTime((IDataReader) this.Reader),
      AvgQueueTimeInMs = this.AvgQueueTimeInMsColumn.GetInt64((IDataReader) this.Reader),
      AvgRunTimeInMs = this.AvgRunTimeInMsColumn.GetInt64((IDataReader) this.Reader),
      Count = this.CountColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
