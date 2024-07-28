// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DmvRunningSessionInfoColumn
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DmvRunningSessionInfoColumn : ObjectBinder<DatabaseManagementViewResult>
  {
    private SqlColumnBinder m_sessionId = new SqlColumnBinder("session_id");
    private SqlColumnBinder m_databaseName = new SqlColumnBinder("databaseName");
    private SqlColumnBinder m_seconds = new SqlColumnBinder("seconds");
    private SqlColumnBinder m_elapsed_time = new SqlColumnBinder("elapsed_time");
    private SqlColumnBinder m_command = new SqlColumnBinder("command");
    private SqlColumnBinder m_blocking_session_id = new SqlColumnBinder("blocking_session_id");
    private SqlColumnBinder m_text = new SqlColumnBinder("text");
    private SqlColumnBinder m_stmt = new SqlColumnBinder("stmt");
    private SqlColumnBinder m_wait_type = new SqlColumnBinder("wait_type");
    private SqlColumnBinder m_wait_time = new SqlColumnBinder("wait_time");
    private SqlColumnBinder m_last_wait_type = new SqlColumnBinder("last_wait_type");
    private SqlColumnBinder m_wait_resource = new SqlColumnBinder("wait_resource");
    private SqlColumnBinder m_reads = new SqlColumnBinder("reads");
    private SqlColumnBinder m_writes = new SqlColumnBinder("writes");
    private SqlColumnBinder m_logical_reads = new SqlColumnBinder("logical_reads");
    private SqlColumnBinder m_cpu_time = new SqlColumnBinder("cpu_time");
    private SqlColumnBinder m_granted_query_memory = new SqlColumnBinder("granted_query_memory");
    private SqlColumnBinder m_requested_memory_kb = new SqlColumnBinder("requested_memory_kb");
    private SqlColumnBinder m_max_used_memory_kb = new SqlColumnBinder("max_used_memory_kb");
    private SqlColumnBinder m_dop = new SqlColumnBinder("dop");
    private SqlColumnBinder m_query_plan = new SqlColumnBinder("query_plan");

    internal DmvRunningSessionInfoColumn()
    {
    }

    internal DmvRunningSessionInfoColumn(SqlDataReader reader)
      : base(reader, "prc_QueryWhatsRunning")
    {
    }

    protected override DatabaseManagementViewResult Bind()
    {
      DatabaseManagementViewResult managementViewResult = new DatabaseManagementViewResult();
      managementViewResult.BlockingSessionId = this.m_blocking_session_id.GetInt16((IDataReader) this.Reader);
      if (this.m_databaseName.ColumnExists((IDataReader) this.Reader))
        managementViewResult.DatabaseName = this.m_databaseName.GetString((IDataReader) this.Reader, true);
      managementViewResult.Command = this.m_command.GetString((IDataReader) this.Reader, true);
      managementViewResult.CpuTime = this.m_cpu_time.GetInt32((IDataReader) this.Reader);
      managementViewResult.ElapsedTime = this.m_elapsed_time.GetDouble((IDataReader) this.Reader);
      managementViewResult.GrantedQueryMemory = this.m_granted_query_memory.GetInt32((IDataReader) this.Reader);
      managementViewResult.LastWaitType = this.m_last_wait_type.GetString((IDataReader) this.Reader, true);
      managementViewResult.LogicalReads = this.m_logical_reads.GetInt64((IDataReader) this.Reader);
      managementViewResult.Reads = this.m_reads.GetInt64((IDataReader) this.Reader);
      managementViewResult.Seconds = this.m_seconds.GetInt32((IDataReader) this.Reader);
      managementViewResult.SessionId = this.m_sessionId.GetInt16((IDataReader) this.Reader);
      managementViewResult.Statement = this.m_stmt.GetString((IDataReader) this.Reader, true);
      managementViewResult.Text = this.m_text.GetString((IDataReader) this.Reader, true);
      managementViewResult.WaitResource = this.m_wait_resource.GetString((IDataReader) this.Reader, true);
      managementViewResult.WaitTime = this.m_wait_time.GetInt32((IDataReader) this.Reader);
      managementViewResult.WaitType = this.m_wait_type.GetString((IDataReader) this.Reader, true);
      managementViewResult.Writes = this.m_writes.GetInt64((IDataReader) this.Reader);
      managementViewResult.RequestedMemory = this.m_requested_memory_kb.GetInt64((IDataReader) this.Reader, 0L, 0L);
      managementViewResult.MaxUsedMemory = this.m_max_used_memory_kb.GetInt64((IDataReader) this.Reader, 0L, 0L);
      managementViewResult.Dop = this.m_dop.GetInt16((IDataReader) this.Reader, (short) 0, (short) 0);
      if (this.m_query_plan.ColumnExists((IDataReader) this.Reader))
        managementViewResult.QueryPlan = this.m_query_plan.GetString((IDataReader) this.Reader, true);
      return managementViewResult;
    }
  }
}
