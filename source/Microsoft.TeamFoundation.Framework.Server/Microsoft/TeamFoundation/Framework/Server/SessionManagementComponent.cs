// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SessionManagementComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SessionManagementComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly string s_queryBlockingSession = EmbeddedResourceUtil.GetResourceAsString("stmt_QueryBlockingSession.sql");
    private static readonly string s_queryBlockedSessionsByMe = EmbeddedResourceUtil.GetResourceAsString("stmt_QueryBlockedSessionsByMe.sql");
    private static readonly string s_querySessionsBySqlLogin = EmbeddedResourceUtil.GetResourceAsString("stmt_QuerySessionsBySqlLogin.sql");
    private static readonly string s_querySessionsWithLongRunningRequest = EmbeddedResourceUtil.GetResourceAsString("stmt_QuerySessionsWithLongRunningRequest.sql");
    private static readonly string s_querySessionsWithApplicationLock = EmbeddedResourceUtil.GetResourceAsString("stmt_QuerySessionsWithApplicationLock.sql");
    private static readonly string s_queryWhatsRunning = EmbeddedResourceUtil.GetResourceAsString("stmt_QueryWhatsRunning.sql");
    private const int c_sessionQueryTimeout = 30;

    public SessionManagementComponent() => this.ContainerErrorCode = 50000;

    public DmvSessionRequestInfo QueryBlockingSession(int sessionId, int maxBlockingTimeSeconds)
    {
      this.Logger.Info("Querying session blocking SPID {0}", (object) sessionId);
      this.PrepareSqlBatch(SessionManagementComponent.s_queryBlockingSession.Length, 30);
      this.AddStatement(SessionManagementComponent.s_queryBlockingSession);
      this.BindInt("@blockedSessionId", sessionId);
      this.BindInt("@blockingMilliseconds", maxBlockingTimeSeconds * 1000);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, (IVssRequestContext) null);
      resultCollection.AddBinder<DmvSessionRequestInfo>((ObjectBinder<DmvSessionRequestInfo>) new SessionManagementComponent.DmvSessionRequestInfoColumn());
      return resultCollection.GetCurrent<DmvSessionRequestInfo>().Items.FirstOrDefault<DmvSessionRequestInfo>();
    }

    public List<DmvBlockedSessionInfo> QueryBlockedSession(
      int sessionId,
      int maxBlockingTimeSeconds)
    {
      this.Logger.Info("Querying sessions blocked by SPID {0}", (object) sessionId);
      this.PrepareSqlBatch(SessionManagementComponent.s_queryBlockedSessionsByMe.Length, 30);
      this.AddStatement(SessionManagementComponent.s_queryBlockedSessionsByMe);
      this.BindInt("@blockingSessionId", sessionId);
      this.BindInt("@blockingMilliseconds", maxBlockingTimeSeconds * 1000);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, (IVssRequestContext) null);
      resultCollection.AddBinder<DmvBlockedSessionInfo>((ObjectBinder<DmvBlockedSessionInfo>) new SessionManagementComponent.DmvBlockedSessionInfoColumn());
      return resultCollection.GetCurrent<DmvBlockedSessionInfo>().Items;
    }

    public List<DmvTranLockSessionInfo> QuerySessionsHoldingApplicationLock(string lockName)
    {
      this.Logger.Info("Querying sessions holding the application lock " + lockName);
      this.PrepareSqlBatch(SessionManagementComponent.s_querySessionsWithApplicationLock.Length, 30);
      this.AddStatement(SessionManagementComponent.s_querySessionsWithApplicationLock);
      this.BindString("@acquireLock", lockName, 128, false, SqlDbType.NVarChar);
      this.BindString("@lockRequestStatus", "Grant", 128, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, (IVssRequestContext) null);
      resultCollection.AddBinder<DmvTranLockSessionInfo>((ObjectBinder<DmvTranLockSessionInfo>) new SessionManagementComponent.DmvTranLockSessionInfoColumn());
      return resultCollection.GetCurrent<DmvTranLockSessionInfo>().Items;
    }

    public List<DmvTranLockSessionInfo> QuerySessionsBlockedByApplicationLock(string lockName)
    {
      this.Logger.Info("Querying sessions blocked behind the application lock " + lockName);
      this.PrepareSqlBatch(SessionManagementComponent.s_querySessionsWithApplicationLock.Length, 30);
      this.AddStatement(SessionManagementComponent.s_querySessionsWithApplicationLock);
      this.BindString("@acquireLock", lockName, 128, false, SqlDbType.NVarChar);
      this.BindString("@lockRequestStatus", "Wait", 128, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, (IVssRequestContext) null);
      resultCollection.AddBinder<DmvTranLockSessionInfo>((ObjectBinder<DmvTranLockSessionInfo>) new SessionManagementComponent.DmvTranLockSessionInfoColumn());
      return resultCollection.GetCurrent<DmvTranLockSessionInfo>().Items;
    }

    public bool Kill(int sessionId)
    {
      this.Logger.Info("Attempt to kill session {0}", (object) sessionId);
      string sqlStatement = string.Format("KILL {0}", (object) sessionId);
      this.PrepareSqlBatch(sqlStatement.Length, 30);
      this.AddStatement(sqlStatement);
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        if (ex.Number == 6106)
        {
          this.Logger.Info("Session {0} does not exist", (object) sessionId);
          return false;
        }
        throw;
      }
      finally
      {
        stopwatch.Stop();
        this.Logger.Info(string.Format("Kill session took {0} milliseconds.", (object) stopwatch.Elapsed.TotalMilliseconds));
      }
      return true;
    }

    public List<DmvSession> QuerySessionsByLogin(string loginName)
    {
      this.Logger.Info("Querying sessions owned by login {0}", (object) loginName);
      this.PrepareSqlBatch(SessionManagementComponent.s_querySessionsBySqlLogin.Length, 30);
      this.AddStatement(SessionManagementComponent.s_querySessionsBySqlLogin);
      this.BindString("@loginName", loginName, 128, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, (IVssRequestContext) null);
      resultCollection.AddBinder<DmvSession>((ObjectBinder<DmvSession>) new SessionManagementComponent.DmvSessionColumn());
      return resultCollection.GetCurrent<DmvSession>().Items;
    }

    public List<DmvSessionRequestInfo> QuerySessionWithLongRunningRequests(DateTime dateTimeSince)
    {
      this.Logger.Info("Querying sessions with request running earlier than {0}", (object) dateTimeSince.ToString("yyyy-MM-dd HH:mm:ss"));
      this.PrepareSqlBatch(SessionManagementComponent.s_querySessionsWithLongRunningRequest.Length, 30);
      this.AddStatement(SessionManagementComponent.s_querySessionsWithLongRunningRequest);
      this.BindDateTime2("@datetimeSince", dateTimeSince);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, (IVssRequestContext) null);
      resultCollection.AddBinder<DmvSessionRequestInfo>((ObjectBinder<DmvSessionRequestInfo>) new SessionManagementComponent.DmvSessionRequestInfoColumn());
      return resultCollection.GetCurrent<DmvSessionRequestInfo>().Items;
    }

    public List<DatabaseManagementViewResult> QueryWhatsRunning()
    {
      this.Logger.Info("Querying what is running in database");
      this.PrepareSqlBatch(SessionManagementComponent.s_queryWhatsRunning.Length, 30);
      this.AddStatement(SessionManagementComponent.s_queryWhatsRunning);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, (IVssRequestContext) null);
      resultCollection.AddBinder<DatabaseManagementViewResult>((ObjectBinder<DatabaseManagementViewResult>) new DmvRunningSessionInfoColumn());
      return resultCollection.GetCurrent<DatabaseManagementViewResult>().Items;
    }

    internal class DmvSessionRequestInfoColumn : ObjectBinder<DmvSessionRequestInfo>
    {
      private SqlColumnBinder m_sessionIdColumn = new SqlColumnBinder("session_id");
      private SqlColumnBinder m_sessionStatusColumn = new SqlColumnBinder("session_status");
      private SqlColumnBinder m_requestStatusColumn = new SqlColumnBinder("request_status");
      private SqlColumnBinder m_connectionTimeColumn = new SqlColumnBinder("connect_time");
      private SqlColumnBinder m_connectionIdColumn = new SqlColumnBinder("connection_id");
      private SqlColumnBinder m_parentConnectionIdColumn = new SqlColumnBinder("parent_connection_id");
      private SqlColumnBinder m_hostNameColumn = new SqlColumnBinder("host_name");
      private SqlColumnBinder m_loginTimeColumn = new SqlColumnBinder("login_time");
      private SqlColumnBinder m_loginNameColumn = new SqlColumnBinder("login_name");
      private SqlColumnBinder m_startTimeColumn = new SqlColumnBinder("start_time");
      private SqlColumnBinder m_cpuTimeColumn = new SqlColumnBinder("cpu_time");
      private SqlColumnBinder m_totalTimeColumn = new SqlColumnBinder("total_elapsed_time");
      private SqlColumnBinder m_contentColumn = new SqlColumnBinder("content");
      private SqlColumnBinder m_blockingStmtColumn = new SqlColumnBinder("stmt");

      protected override DmvSessionRequestInfo Bind() => new DmvSessionRequestInfo()
      {
        SessionId = this.m_sessionIdColumn.GetInt32((IDataReader) this.Reader, 0),
        SessionStatus = this.m_sessionStatusColumn.GetString((IDataReader) this.Reader, false),
        RequestStatus = this.m_requestStatusColumn.GetString((IDataReader) this.Reader, true),
        ConnectionTime = this.m_connectionTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue),
        ConnectionId = this.m_connectionIdColumn.GetGuid((IDataReader) this.Reader, false),
        ParentConnectionId = this.m_parentConnectionIdColumn.GetGuid((IDataReader) this.Reader, true),
        HostName = this.m_hostNameColumn.GetString((IDataReader) this.Reader, string.Empty),
        LoginTime = this.m_loginTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue),
        LoginName = this.m_loginNameColumn.GetString((IDataReader) this.Reader, false),
        RequestStartTime = this.m_startTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue),
        RequestCpuTime = this.m_cpuTimeColumn.GetInt32((IDataReader) this.Reader, 0, -1),
        RequestTotalTime = this.m_totalTimeColumn.GetInt32((IDataReader) this.Reader, 0, -1),
        Content = this.m_contentColumn.GetString((IDataReader) this.Reader, true),
        BlockingStmt = this.m_blockingStmtColumn.GetString((IDataReader) this.Reader, true)
      };
    }

    internal class DmvBlockedSessionInfoColumn : ObjectBinder<DmvBlockedSessionInfo>
    {
      private SqlColumnBinder m_sessionIdColumn = new SqlColumnBinder("session_id");
      private SqlColumnBinder m_blockingSessionIdColumn = new SqlColumnBinder("blocking_session_id");
      private SqlColumnBinder m_sessionStatusColumn = new SqlColumnBinder("session_status");
      private SqlColumnBinder m_requestStatusColumn = new SqlColumnBinder("request_status");
      private SqlColumnBinder m_startTimeColumn = new SqlColumnBinder("start_time");
      private SqlColumnBinder m_elapsedTimeColumn = new SqlColumnBinder("elapsed_milliseconds");
      private SqlColumnBinder m_commandColumn = new SqlColumnBinder("command");
      private SqlColumnBinder m_waitTypeColumn = new SqlColumnBinder("wait_type");
      private SqlColumnBinder m_waitTimeColumn = new SqlColumnBinder("wait_time");
      private SqlColumnBinder m_waitResourceColumn = new SqlColumnBinder("wait_resource");
      private SqlColumnBinder m_contentColumn = new SqlColumnBinder("content");

      protected override DmvBlockedSessionInfo Bind() => new DmvBlockedSessionInfo()
      {
        SessionId = (int) this.m_sessionIdColumn.GetInt16((IDataReader) this.Reader, (short) 0),
        BlockingSessionId = (int) this.m_blockingSessionIdColumn.GetInt16((IDataReader) this.Reader, (short) 0),
        SessionStatus = this.m_sessionStatusColumn.GetString((IDataReader) this.Reader, false),
        RequestStatus = this.m_requestStatusColumn.GetString((IDataReader) this.Reader, false),
        StartTime = this.m_startTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue),
        ElapsedMilliseconds = this.m_elapsedTimeColumn.GetInt32((IDataReader) this.Reader, -1),
        Command = this.m_commandColumn.GetString((IDataReader) this.Reader, false),
        WaitType = this.m_waitTypeColumn.GetString((IDataReader) this.Reader, true),
        WaitTime = this.m_waitTimeColumn.GetInt32((IDataReader) this.Reader, -1),
        WaitResource = this.m_waitResourceColumn.GetString((IDataReader) this.Reader, false),
        Content = this.m_contentColumn.GetString((IDataReader) this.Reader, string.Empty)
      };
    }

    internal class DmvSessionColumn : ObjectBinder<DmvSession>
    {
      private SqlColumnBinder m_sessionIdColumn = new SqlColumnBinder("session_id");
      private SqlColumnBinder m_sessionStatusColumn = new SqlColumnBinder("status");
      private SqlColumnBinder m_loginTimeColumn = new SqlColumnBinder("login_time");
      private SqlColumnBinder m_loginNameColumn = new SqlColumnBinder("login_name");
      private SqlColumnBinder m_hostNameColumn = new SqlColumnBinder("host_name");
      private SqlColumnBinder m_hostProcessIdColumn = new SqlColumnBinder("host_process_id");
      private SqlColumnBinder m_programNameColumn = new SqlColumnBinder("program_name");

      protected override DmvSession Bind() => new DmvSession()
      {
        SessionId = (int) this.m_sessionIdColumn.GetInt16((IDataReader) this.Reader, (short) 0),
        SessionStatus = this.m_sessionStatusColumn.GetString((IDataReader) this.Reader, false),
        LoginTime = this.m_loginTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue),
        LoginName = this.m_loginNameColumn.GetString((IDataReader) this.Reader, false),
        HostName = this.m_hostNameColumn.GetString((IDataReader) this.Reader, false),
        HostProcessId = this.m_hostProcessIdColumn.GetInt32((IDataReader) this.Reader, 0),
        ProgramName = this.m_programNameColumn.GetString((IDataReader) this.Reader, false)
      };
    }

    internal class DmvTranLockSessionInfoColumn : ObjectBinder<DmvTranLockSessionInfo>
    {
      private SqlColumnBinder m_sessionIdColumn = new SqlColumnBinder("request_session_id");
      private SqlColumnBinder m_lockStatusColumn = new SqlColumnBinder("request_status");
      private SqlColumnBinder m_lockTypeColumn = new SqlColumnBinder("resource_type");
      private SqlColumnBinder m_lockModeColumn = new SqlColumnBinder("request_mode");
      private SqlColumnBinder m_lockOwnerColumn = new SqlColumnBinder("request_owner_type");
      private SqlColumnBinder m_lockDescColumn = new SqlColumnBinder("resource_description");
      private SqlColumnBinder m_databaseIdColumn = new SqlColumnBinder("resource_database_id");
      private SqlColumnBinder m_requestStatusColumn = new SqlColumnBinder("status");
      private SqlColumnBinder m_blockingSessionIdColumn = new SqlColumnBinder("blocking_session_id");
      private SqlColumnBinder m_startTimeColumn = new SqlColumnBinder("start_time");
      private SqlColumnBinder m_elapsedTimeColumn = new SqlColumnBinder("total_elapsed_time");
      private SqlColumnBinder m_contentColumn = new SqlColumnBinder("content");
      private SqlColumnBinder m_stmtColumn = new SqlColumnBinder("stmt");

      protected override DmvTranLockSessionInfo Bind() => new DmvTranLockSessionInfo()
      {
        SessionId = this.m_sessionIdColumn.GetInt32((IDataReader) this.Reader, 0),
        LockStatus = this.m_lockStatusColumn.GetString((IDataReader) this.Reader, true),
        LockType = this.m_lockTypeColumn.GetString((IDataReader) this.Reader, true),
        LockMode = this.m_lockModeColumn.GetString((IDataReader) this.Reader, true),
        LockDesc = this.m_lockDescColumn.GetString((IDataReader) this.Reader, true),
        LockOwner = this.m_lockOwnerColumn.GetString((IDataReader) this.Reader, true),
        DatabaseId = this.m_databaseIdColumn.GetInt32((IDataReader) this.Reader, 0),
        RequestStatus = this.m_requestStatusColumn.GetString((IDataReader) this.Reader, true),
        BlockingSessionId = (int) this.m_blockingSessionIdColumn.GetInt16((IDataReader) this.Reader, (short) 0),
        StartTime = this.m_startTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue),
        ElapsedMilliseconds = this.m_elapsedTimeColumn.GetInt32((IDataReader) this.Reader, -1),
        Content = this.m_contentColumn.GetString((IDataReader) this.Reader, true),
        Stmt = this.m_stmtColumn.GetString((IDataReader) this.Reader, true)
      };
    }

    internal enum ApplicationLockRequestStatus
    {
      Grant,
      Wait,
    }
  }
}
