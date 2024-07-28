// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.JobReportingResultsOverTimeColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class JobReportingResultsOverTimeColumns : 
    ObjectBinder<TeamFoundationJobReportingResultsOverTime>
  {
    private SqlColumnBinder JobIdColumn = new SqlColumnBinder("JobId");
    private SqlColumnBinder SucceededCountColumn = new SqlColumnBinder("SucceededCount");
    private SqlColumnBinder PartiallySucceededCountColumn = new SqlColumnBinder("PartiallySucceededCount");
    private SqlColumnBinder FailedCountColumn = new SqlColumnBinder("FailedCount");
    private SqlColumnBinder StoppedCountColumn = new SqlColumnBinder("StoppedCount");
    private SqlColumnBinder KilledCountColumn = new SqlColumnBinder("KilledCount");
    private SqlColumnBinder BlockedCountColumn = new SqlColumnBinder("BlockedCount");
    private SqlColumnBinder ExtensionNotFoundCountColumn = new SqlColumnBinder("ExtensionNotFoundCount");
    private SqlColumnBinder InactiveCountColumn = new SqlColumnBinder("InactiveCount");
    private SqlColumnBinder DisabledCountColumn = new SqlColumnBinder("DisabledCount");
    private SqlColumnBinder TotalCountColumn = new SqlColumnBinder("TotalCount");

    protected override TeamFoundationJobReportingResultsOverTime Bind() => new TeamFoundationJobReportingResultsOverTime()
    {
      JobId = this.JobIdColumn.GetGuid((IDataReader) this.Reader),
      SucceededCount = this.SucceededCountColumn.GetInt32((IDataReader) this.Reader),
      PartiallySucceededCount = this.PartiallySucceededCountColumn.GetInt32((IDataReader) this.Reader),
      FailedCount = this.FailedCountColumn.GetInt32((IDataReader) this.Reader),
      StoppedCount = this.StoppedCountColumn.GetInt32((IDataReader) this.Reader),
      KilledCount = this.KilledCountColumn.GetInt32((IDataReader) this.Reader),
      BlockedCount = this.BlockedCountColumn.GetInt32((IDataReader) this.Reader),
      ExtensionNotFoundCount = this.ExtensionNotFoundCountColumn.GetInt32((IDataReader) this.Reader),
      InactiveCount = this.InactiveCountColumn.GetInt32((IDataReader) this.Reader),
      DisabledCount = this.DisabledCountColumn.GetInt32((IDataReader) this.Reader),
      TotalCount = this.TotalCountColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
