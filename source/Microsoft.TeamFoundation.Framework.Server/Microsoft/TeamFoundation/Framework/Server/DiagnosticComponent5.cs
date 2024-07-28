// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent5 : DiagnosticComponent4
  {
    public override List<LogspaceSummary> GetLogUtilization(
      int transactionsOlderThanSeconds,
      long transactionsMinimumBytes,
      out List<LogspaceDetails> details)
    {
      this.PrepareStoredProcedure("DIAGNOSTIC.prc_QueryLogSpace");
      this.BindInt("@transactionsOlderThanSeconds", transactionsOlderThanSeconds);
      this.BindLong("@transactionsMinimumBytes", transactionsMinimumBytes);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) reader, "DIAGNOSTIC.prc_QueryLogSpace", this.RequestContext))
        {
          resultCollection.AddBinder<LogspaceSummary>((ObjectBinder<LogspaceSummary>) new DiagnosticComponent5.LogSpaceSummaryBinder());
          resultCollection.AddBinder<LogspaceDetails>((ObjectBinder<LogspaceDetails>) new DiagnosticComponent5.LogspaceDetailsBinder());
          List<LogspaceSummary> items = resultCollection.GetCurrent<LogspaceSummary>().Items;
          resultCollection.NextResult();
          details = resultCollection.GetCurrent<LogspaceDetails>().Items;
          return items;
        }
      }
    }

    internal class LogSpaceSummaryBinder : ObjectBinder<LogspaceSummary>
    {
      private SqlColumnBinder m_db = new SqlColumnBinder("Database Name");
      private SqlColumnBinder m_logSize = new SqlColumnBinder("Log Size (MB)");
      private SqlColumnBinder m_logSpaceUsed = new SqlColumnBinder("Log Space Used (%)");

      protected override LogspaceSummary Bind() => new LogspaceSummary()
      {
        DatabaseName = this.m_db.GetString((IDataReader) this.Reader, false),
        LogSize = this.m_logSize.GetFloat((IDataReader) this.Reader),
        LogspaceUsedPercent = this.m_logSpaceUsed.GetFloat((IDataReader) this.Reader)
      };
    }

    internal class LogspaceDetailsBinder : ObjectBinder<LogspaceDetails>
    {
      private SqlColumnBinder m_sessionId = new SqlColumnBinder("SessionId");
      private SqlColumnBinder m_transactionBeginTime = new SqlColumnBinder("TransactionBeginTime");
      private SqlColumnBinder m_transactionSeconds = new SqlColumnBinder("TransactionSeconds");
      private SqlColumnBinder m_logBytesUsed = new SqlColumnBinder("LogBytesUsed");
      private SqlColumnBinder m_logBytesReserved = new SqlColumnBinder("LogBytesReserved");
      private SqlColumnBinder m_hostName = new SqlColumnBinder("HostName");
      private SqlColumnBinder m_clientNetAddress = new SqlColumnBinder("ClientNetAddress");
      private SqlColumnBinder m_programName = new SqlColumnBinder("ProgramName");
      private SqlColumnBinder m_hostProcessId = new SqlColumnBinder("HostProcessId");
      private SqlColumnBinder m_loginName = new SqlColumnBinder("LoginName");
      private SqlColumnBinder m_loginTime = new SqlColumnBinder("LoginTime");
      private SqlColumnBinder m_loginSeconds = new SqlColumnBinder("LoginSeconds");
      private SqlColumnBinder m_status = new SqlColumnBinder("Status");
      private SqlColumnBinder m_cpuTime = new SqlColumnBinder("CpuTime");
      private SqlColumnBinder m_reads = new SqlColumnBinder("Reads");
      private SqlColumnBinder m_writes = new SqlColumnBinder("Writes");
      private SqlColumnBinder m_text = new SqlColumnBinder("Text");

      protected override LogspaceDetails Bind() => new LogspaceDetails()
      {
        SessionId = this.m_sessionId.GetInt32((IDataReader) this.Reader),
        TransactionBeginTime = this.m_transactionBeginTime.GetDateTime((IDataReader) this.Reader),
        TransactionSeconds = this.m_transactionSeconds.GetInt32((IDataReader) this.Reader),
        LogBytesUsed = this.m_logBytesUsed.GetInt64((IDataReader) this.Reader, 0L),
        LogBytesReserved = this.m_logBytesReserved.GetInt64((IDataReader) this.Reader, 0L),
        HostName = this.m_hostName.GetString((IDataReader) this.Reader, true),
        ClientNetAddress = this.m_clientNetAddress.GetString((IDataReader) this.Reader, true),
        ProgramName = this.m_programName.GetString((IDataReader) this.Reader, true),
        HostProcessId = this.m_hostProcessId.GetInt32((IDataReader) this.Reader, 0),
        LoginName = this.m_loginName.GetString((IDataReader) this.Reader, false),
        LoginTime = this.m_loginTime.GetDateTime((IDataReader) this.Reader),
        LoginSeconds = this.m_loginSeconds.GetInt32((IDataReader) this.Reader),
        Status = this.m_status.GetString((IDataReader) this.Reader, false),
        CpuTime = this.m_cpuTime.GetInt32((IDataReader) this.Reader),
        Reads = this.m_reads.GetInt64((IDataReader) this.Reader),
        Writes = this.m_writes.GetInt64((IDataReader) this.Reader),
        Text = this.m_text.GetString((IDataReader) this.Reader, true)
      };
    }
  }
}
