// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.JobReportingQueuePositionsCountColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class JobReportingQueuePositionsCountColumns : 
    ObjectBinder<TeamFoundationJobReportingQueuePositionCount>
  {
    private SqlColumnBinder QueuePositionColumn = new SqlColumnBinder("QueuePosition");
    private SqlColumnBinder JobCountColumn = new SqlColumnBinder("JobCount");

    protected override TeamFoundationJobReportingQueuePositionCount Bind() => new TeamFoundationJobReportingQueuePositionCount()
    {
      QueuePosition = this.QueuePositionColumn.GetInt32((IDataReader) this.Reader),
      JobCount = this.JobCountColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
