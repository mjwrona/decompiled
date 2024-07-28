// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobHistoryEntryColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationJobHistoryEntryColumns : ObjectBinder<TeamFoundationJobHistoryEntry>
  {
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

    protected override TeamFoundationJobHistoryEntry Bind()
    {
      if (this.JobIdColumn.IsNull((IDataReader) this.Reader))
        return (TeamFoundationJobHistoryEntry) null;
      TeamFoundationJobHistoryEntry foundationJobHistoryEntry = new TeamFoundationJobHistoryEntry();
      foundationJobHistoryEntry.HistoryId = this.HistoryIdColumn.GetInt64((IDataReader) this.Reader);
      foundationJobHistoryEntry.JobSource = this.JobSourceColumn.GetGuid((IDataReader) this.Reader);
      foundationJobHistoryEntry.JobId = this.JobIdColumn.GetGuid((IDataReader) this.Reader);
      foundationJobHistoryEntry.QueueTime = this.QueueTimeColumn.GetDateTime((IDataReader) this.Reader);
      foundationJobHistoryEntry.ExecutionStartTime = this.StartTimeColumn.GetDateTime((IDataReader) this.Reader);
      foundationJobHistoryEntry.EndTime = this.EndTimeColumn.GetDateTime((IDataReader) this.Reader);
      foundationJobHistoryEntry.AgentId = this.AgentIdColumn.GetGuid((IDataReader) this.Reader);
      foundationJobHistoryEntry.Result = (TeamFoundationJobResult) this.ResultColumn.GetByte((IDataReader) this.Reader);
      foundationJobHistoryEntry.ResultMessage = this.ResultMessageColumn.GetString((IDataReader) this.Reader, true);
      foundationJobHistoryEntry.QueuedReasons = (TeamFoundationJobQueuedReasons) this.QueuedReasonsColumn.GetInt32((IDataReader) this.Reader);
      return foundationJobHistoryEntry;
    }
  }
}
