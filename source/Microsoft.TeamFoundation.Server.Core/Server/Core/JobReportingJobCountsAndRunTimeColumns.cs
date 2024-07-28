// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.JobReportingJobCountsAndRunTimeColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class JobReportingJobCountsAndRunTimeColumns : 
    ObjectBinder<TeamFoundationJobReportingHistoryQueueTime>
  {
    private SqlColumnBinder JobIdColumn = new SqlColumnBinder("JobId");
    private SqlColumnBinder CountColumn = new SqlColumnBinder("Count");
    private SqlColumnBinder TotalRunTimeMillisecondsColumn = new SqlColumnBinder("TotalRunTimeMilliseconds");

    protected override TeamFoundationJobReportingHistoryQueueTime Bind() => new TeamFoundationJobReportingHistoryQueueTime()
    {
      JobId = this.JobIdColumn.GetGuid((IDataReader) this.Reader),
      Count = this.CountColumn.GetInt32((IDataReader) this.Reader),
      TotalRunTimeMilliseconds = this.TotalRunTimeMillisecondsColumn.GetInt64((IDataReader) this.Reader)
    };
  }
}
