// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlScriptResourceComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SqlScriptResourceComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly SqlMetaData[] typ_SqlBatch2 = new SqlMetaData[3]
    {
      new SqlMetaData("BatchIndex", SqlDbType.Int),
      new SqlMetaData("Batch", SqlDbType.VarChar, -1L),
      new SqlMetaData("BatchN", SqlDbType.NVarChar, -1L)
    };
    private static readonly ConcurrentDictionary<string, SqlScriptResourceComponent.SqlServerCapabilities> s_sqlServerCapabilities = new ConcurrentDictionary<string, SqlScriptResourceComponent.SqlServerCapabilities>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<int, SqlExceptionFactory> s_exceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        800070,
        new SqlExceptionFactory(typeof (FailedToAcquireServicingLockException))
      }
    };
    private const int c_lostConnectionRetries = 10;
    private static readonly string s_typSqlBatch = EmbeddedResourceUtil.GetResourceAsString("typ_SqlBatch2.sql");
    private static readonly string s_executeScriptsStmt = EmbeddedResourceUtil.GetResourceAsString("stmt_ExecuteScripts.sql");
    private static readonly string s_queryCapabilitiesStmt = EmbeddedResourceUtil.GetResourceAsString("stmt_QueryCapabilities.sql");
    internal static readonly int s_defaultMaxLockTimeSeconds = 60;
    internal static readonly int s_defaulBlockingSessionTimeSeconds = 30;
    private static readonly string s_area = "SqlScriptResource";
    private static readonly string s_layer = "Component";
    private const int c_batchExecTimeLimit = 2000;
    private const int c_slowBatchesListSize = 5;
    private const int c_secondsToCheckBlockingLockHeld = 5;
    private const int c_defaultAcquireLockTimeoutInSeconds = 5;
    private const int c_defaultAcquireLockMaxAttempts = 30;
    private const int c_longRunningRequestThresholdSeconds = 60;
    private const int c_lastAttemptsToKillSessions = 8;
    private List<SqlError> m_sqlErrors;
    private List<SqlBatch> m_batches;
    private Dictionary<string, int> m_scriptsExecTime;
    private List<KeyValuePair<int, int>> m_slowBatchesList;
    private int m_batchIndex;
    private Stopwatch m_lockAcquisitionStopwatch;
    private Timer m_lockTimer;
    private int m_maxLockTimeSeconds = SqlScriptResourceComponent.s_defaultMaxLockTimeSeconds;
    private int m_maxBlockingTimeSeconds = SqlScriptResourceComponent.s_defaulBlockingSessionTimeSeconds;
    private bool m_lockTimedOut;
    private DateTime m_previousBatchStartTime;
    private bool m_testRerunnability;
    private bool m_acquireLock;
    private TimeSpan m_clockSkew;
    private Timer m_blockingDetectionTimer;
    private int m_blockingDetectionTimerCounter;
    private bool m_inProcessAcquiringLock;
    private int m_servicingLockAttempt;
    private int m_acquireLockMaxAttempts = 30;
    private int m_acquireLockTimeoutInSeconds = 5;
    private int m_longRunningRequestThresholdSeconds = 60;
    private readonly object m_blockingTimerLock = new object();
    private int m_sessionId;
    private bool? m_isSqlInjectionEnabled;
    private ISqlInstrumentator m_sqlInstrumentator;

    public SqlScriptResourceComponent() => this.ContainerErrorCode = 50000;

    public override void Dispose()
    {
      base.Dispose();
      this.m_lockTimer?.Dispose();
      this.m_lockTimer = (Timer) null;
      lock (this.m_blockingTimerLock)
      {
        this.m_blockingDetectionTimer?.Dispose();
        this.m_blockingDetectionTimer = (Timer) null;
      }
    }

    public int AcquireLockMaxAttempts
    {
      set => this.m_acquireLockMaxAttempts = value;
      get => this.m_acquireLockMaxAttempts;
    }

    public int AcquireLockTimeoutInSeconds
    {
      set => this.m_acquireLockTimeoutInSeconds = value;
      get => this.m_acquireLockTimeoutInSeconds;
    }

    public int LongRunningRequestThresholdSeconds
    {
      set => this.m_longRunningRequestThresholdSeconds = value;
      get => this.m_longRunningRequestThresholdSeconds;
    }

    protected virtual SqlParameter BindSqlBatchTable(
      string parameterName,
      IEnumerable<SqlBatch> rows)
    {
      if (rows == null)
        rows = Enumerable.Empty<SqlBatch>();
      System.Func<SqlBatch, SqlDataRecord> selector = (System.Func<SqlBatch, SqlDataRecord>) (batch =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SqlScriptResourceComponent.typ_SqlBatch2);
        sqlDataRecord.SetInt32(0, batch.Index);
        if (batch.HasNonAsciiCharacters)
          sqlDataRecord.SetString(2, batch.Batch);
        else
          sqlDataRecord.SetString(1, batch.Batch);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_SqlBatch2", rows.Select<SqlBatch, SqlDataRecord>(selector));
    }

    public void ExecuteStatement(string sqlStatement, int sqlCommandTimeout) => this.ExecuteStatement(sqlStatement, (SqlParameter[]) null, sqlCommandTimeout);

    public void ExecuteStatement(
      string sqlStatement,
      SqlParameter[] parameters,
      int sqlCommandTimeout)
    {
      this.ExecuteStatementImpl(sqlStatement, parameters, SqlScriptResourceComponent.ExecutionType.NonQuery, true, sqlCommandTimeout);
    }

    public object ExecuteStatementScalar(string sqlStatement, int sqlCommandTimeout) => this.ExecuteStatementImpl(sqlStatement, (SqlParameter[]) null, SqlScriptResourceComponent.ExecutionType.Scalar, true, sqlCommandTimeout);

    public object ExecuteStatementScalar(
      string sqlStatement,
      SqlParameter[] parameters,
      int sqlCommandTimeout)
    {
      return this.ExecuteStatementImpl(sqlStatement, parameters, SqlScriptResourceComponent.ExecutionType.Scalar, true, sqlCommandTimeout);
    }

    public SqlDataReader ExecuteStatementReader(string sqlStatement, int sqlCommandTimeout) => this.ExecuteStatementReader(sqlStatement, (SqlParameter[]) null, sqlCommandTimeout);

    public SqlDataReader ExecuteStatementReader(
      string sqlStatement,
      SqlParameter[] parameters,
      int sqlCommandTimeout)
    {
      return (SqlDataReader) this.ExecuteStatementImpl(sqlStatement, parameters, SqlScriptResourceComponent.ExecutionType.Reader, true, sqlCommandTimeout);
    }

    public void ExecuteScript(SqlScript script, int sqlCommandTimeout) => this.ExecuteScript(script, (SqlParameter[]) null, sqlCommandTimeout);

    public void ExecuteScript(
      SqlScript script,
      SqlParameter[] sqlParameters,
      int sqlCommandTimeout)
    {
      this.ExecuteScript(script, sqlParameters, (List<ServiceVersionEntry>) null, sqlCommandTimeout);
    }

    public void ExecuteScript(
      SqlScript script,
      SqlParameter[] sqlParameters,
      List<ServiceVersionEntry> serviceVersions,
      int sqlCommandTimeout)
    {
      this.ExecuteScripts(new List<SqlScript>() { script }, sqlParameters, serviceVersions, false, sqlCommandTimeout);
    }

    public void ExecuteScripts(List<SqlScript> scripts, int sqlCommandTimeout) => this.ExecuteScripts(scripts, (SqlParameter[]) null, (List<ServiceVersionEntry>) null, false, sqlCommandTimeout);

    public void ExecuteScripts(
      List<SqlScript> scripts,
      SqlParameter[] sqlParameters,
      List<ServiceVersionEntry> serviceVersions,
      bool acquireLock,
      int sqlCommandTimeout)
    {
      this.ExecuteScripts(scripts, sqlParameters, serviceVersions, acquireLock, TimeSpan.FromSeconds((double) SqlScriptResourceComponent.s_defaultMaxLockTimeSeconds), TimeSpan.FromSeconds((double) SqlScriptResourceComponent.s_defaulBlockingSessionTimeSeconds), sqlCommandTimeout);
    }

    public void ExecuteScripts(
      List<SqlScript> scripts,
      SqlParameter[] sqlParameters,
      List<ServiceVersionEntry> serviceVersions,
      bool acquireLock,
      TimeSpan maxServicingLockTime,
      TimeSpan maxBlockingSessionTime,
      int sqlCommandTimeout)
    {
      this.ExecuteScripts(scripts, sqlParameters, serviceVersions, acquireLock, acquireLock, maxServicingLockTime, maxBlockingSessionTime, sqlCommandTimeout, false, false, false);
    }

    public void ExecuteScripts(
      List<SqlScript> scripts,
      SqlParameter[] sqlParameters,
      List<ServiceVersionEntry> serviceVersions,
      bool acquireLock,
      bool inTransaction,
      TimeSpan maxServicingLockTime,
      TimeSpan maxBlockingSessionTime,
      int sqlCommandTimeout,
      bool testRerunnability,
      bool monitorBlocking,
      bool snapshotOnly)
    {
      if (acquireLock && !inTransaction)
        throw new ArgumentException("Since acquireLock is true, inTransaction must also be true", nameof (inTransaction));
      ArgumentUtility.CheckForNull<List<SqlScript>>(scripts, nameof (scripts));
      int commandTimeout = sqlCommandTimeout >= 0 ? sqlCommandTimeout : 3600;
      try
      {
        this.InfoMessage += new SqlInfoMessageEventHandler(this.OnInfoMessage);
        this.m_testRerunnability = testRerunnability;
        int num = 10;
        string xml = SqlScriptResourceComponent.SerializeToXml(serviceVersions);
        this.m_maxLockTimeSeconds = (int) maxServicingLockTime.TotalSeconds;
        this.m_maxBlockingTimeSeconds = (int) maxBlockingSessionTime.TotalSeconds;
        this.m_acquireLock = acquireLock;
        this.m_batches = new List<SqlBatch>();
        this.m_scriptsExecTime = new Dictionary<string, int>();
        this.m_slowBatchesList = new List<KeyValuePair<int, int>>(5);
        bool replaceOnlineOn;
        if (this.IsSqlAzure)
        {
          replaceOnlineOn = false;
        }
        else
        {
          SqlScriptResourceComponent.SqlServerCapabilities serverCapabilities = this.GetSqlServerCapabilities();
          replaceOnlineOn = !(this.ConnectionInfo is ISupportSqlCredential) || (serverCapabilities & SqlScriptResourceComponent.SqlServerCapabilities.OnlineIndexing) == SqlScriptResourceComponent.SqlServerCapabilities.None;
        }
        foreach (SqlScript script in scripts)
        {
          this.m_scriptsExecTime.Add(script.Name, 0);
          this.m_batches.AddRange((IEnumerable<SqlBatch>) script.GetBatches(replaceOnlineOn, this.SqlInstrumentator));
        }
        if (snapshotOnly)
          this.m_batches = this.m_batches.Where<SqlBatch>((System.Func<SqlBatch, bool>) (b => b.InSnapshot)).ToList<SqlBatch>();
        for (int index = this.m_batches.Count - 1; index >= 0; --index)
          this.m_batches[index].Index = index;
        this.Connection.FireInfoMessageEventOnUserErrors = true;
        string sqlStatement = SqlScriptResourceComponent.s_executeScriptsStmt;
        string parameterDef = SqlScriptResourceComponent.GetParameterDef(sqlParameters);
        if (!string.IsNullOrEmpty(parameterDef))
          sqlStatement = sqlStatement.Replace("EXEC sp_ExecuteSql @_batch", "EXEC sp_executeSql @_batch, " + parameterDef);
        bool flag = false;
        SqlScriptException sqlScriptException = (SqlScriptException) null;
        Stopwatch stopwatch = new Stopwatch();
        do
        {
          stopwatch.Restart();
          try
          {
            this.m_sqlErrors = (List<SqlError>) null;
            this.m_batchIndex = -1;
            this.m_previousBatchStartTime = DateTime.MinValue;
            if (flag)
            {
              this.PrepareSqlBatch(SqlScriptResourceComponent.s_typSqlBatch.Length, commandTimeout);
              this.AddStatement(SqlScriptResourceComponent.s_typSqlBatch);
              this.ExecuteNonQuery();
            }
            if (this.m_sqlErrors == null)
            {
              if (!acquireLock & monitorBlocking && !(this.ConnectionInfo is SqlConnectionInfoFactory.DatabaseConnectionInfoIntegratedSecurity) && this.m_maxBlockingTimeSeconds > 0)
              {
                lock (this.m_blockingTimerLock)
                {
                  this.m_blockingDetectionTimer?.Dispose();
                  this.m_inProcessAcquiringLock = false;
                  this.m_blockingDetectionTimerCounter = 0;
                  this.m_blockingDetectionTimer = new Timer(new TimerCallback(this.OnBlockingSessionTimer), (object) null, this.m_maxBlockingTimeSeconds * 1000, -1);
                }
              }
              this.PrepareSqlBatch(sqlStatement.Length, commandTimeout);
              this.AddStatement(sqlStatement, sqlParameters != null ? sqlParameters.Length : 0, false, false);
              if (sqlParameters != null)
                this.Command.Parameters.AddRange(sqlParameters);
              this.BindSqlBatchTable("@_batches", (IEnumerable<SqlBatch>) this.m_batches);
              string verifyVersionLock = acquireLock ? FrameworkServerConstants.VerifyVersionLock : "";
              this.BindString("@_acquireLock", verifyVersionLock, verifyVersionLock.Length, BindStringBehavior.Unchanged, SqlDbType.VarChar);
              this.BindBoolean("@_inTransaction", inTransaction);
              this.BindXml("@_serviceVersions", xml);
              this.BindBoolean("@_testRerunnability", testRerunnability);
              this.BindInt("@_acquireLockMilliseconds", this.AcquireLockTimeoutInSeconds * 1000);
              this.BindInt("@_acquireLockMaxAttempts", this.AcquireLockMaxAttempts);
              this.BindInt("@_longRunningThresholdMilliSeconds", this.LongRunningRequestThresholdSeconds * 1000);
              this.BindInt("@_lastAttemptsToKillSessions", 8);
              this.BindBoolean("@_isHosted", !(this.ConnectionInfo is SqlConnectionInfoFactory.DatabaseConnectionInfoIntegratedSecurity));
              this.ExecuteNonQuery();
              if (this.m_lockAcquisitionStopwatch != null)
              {
                TimeSpan elapsed = this.m_lockAcquisitionStopwatch.Elapsed;
                if (elapsed >= TimeSpan.FromSeconds(20.0))
                  this.Logger.Warning("Servicing lock was held for {0}.", (object) elapsed);
              }
            }
            if (this.m_sqlErrors == null)
              return;
            if (this.m_sqlErrors.FirstOrDefault<SqlError>((System.Func<SqlError, bool>) (se => se.Number == 351 && se.Message.Contains("typ_SqlBatch2"))) != null && --num >= 0)
            {
              flag = true;
            }
            else
            {
              SqlError sqlError = this.m_sqlErrors.FirstOrDefault<SqlError>((System.Func<SqlError, bool>) (se => this.CanRetryOnSqlError(se)));
              SqlBatch batch = (SqlBatch) null;
              if (this.m_batchIndex >= 0 && this.m_batchIndex < this.m_batches.Count)
              {
                batch = this.m_batches[this.m_batchIndex];
                this.RecordFailedBatchExecutionTime();
              }
              sqlScriptException = new SqlScriptException((ICollection<SqlError>) this.m_sqlErrors, batch);
              if (sqlError != null)
              {
                if (--num >= 0)
                {
                  this.Logger.Info("The following recoverable error was reported: {0}. Message: {1}", (object) sqlError.Number, (object) sqlError.Message);
                  this.Logger.Info(sqlScriptException.Message);
                  this.Connection.Close();
                }
              }
            }
          }
          catch (Exception ex)
          {
            this.Logger.Info("Caught exception in ExecuteScripts: {0}", (object) ex);
            if (this.m_batchIndex >= 0 && this.m_batchIndex < this.m_batches.Count)
              this.RecordFailedBatchExecutionTime();
            lock (this.m_blockingTimerLock)
              this.m_inProcessAcquiringLock = false;
            if (ex is DatabaseOperationCanceledException && this.m_lockTimedOut)
            {
              string str = string.Join(", ", scripts.Select<SqlScript, string>((System.Func<SqlScript, string>) (s => s.Name)));
              string message = FrameworkResources.ServicingLockHeldTimeoutAdminDetails((object) this.m_maxLockTimeSeconds, (object) str);
              this.Logger.Error(message);
              TeamFoundationEventLog.Default.Log((IVssRequestContext) null, message, TeamFoundationEventId.UpgradeJobFailedLockWasHeldForTooLong, EventLogEntryType.Error, (object) str);
              this.m_lockTimedOut = false;
              throw new ServicingTimeoutException(FrameworkResources.ServicingLockHeldTimeout(), ex);
            }
            if (this.ShouldRetryLostConnection(ex) && num > 0)
            {
              this.Logger.Warning(FrameworkResources.SqlScriptSqlConnectionLost((object) stopwatch.Elapsed, (object) this.ConnectionInfo.DataSource, (object) this.ConnectionInfo.InitialCatalog, (object) ex.Message));
              --num;
            }
            else
              throw;
          }
          finally
          {
            lock (this.m_blockingTimerLock)
            {
              if (this.m_blockingDetectionTimer != null)
                this.m_blockingDetectionTimer.Change(-1, -1);
              using (this.m_blockingDetectionTimer)
                this.m_blockingDetectionTimer = (Timer) null;
            }
          }
        }
        while (sqlScriptException == null);
        throw sqlScriptException;
      }
      finally
      {
        this.Connection.FireInfoMessageEventOnUserErrors = false;
        this.InfoMessage -= new SqlInfoMessageEventHandler(this.OnInfoMessage);
        if (!this.m_testRerunnability)
          this.LogScriptRunningTime();
        this.m_testRerunnability = false;
      }
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) SqlScriptResourceComponent.s_exceptionFactories;

    private bool CanRetryOnSqlError(SqlError sqlError)
    {
      if (sqlError.Number == 0 && sqlError.Class == (byte) 11)
      {
        if (--this.RetriesRemaining >= 0)
          return true;
      }
      TimeSpan sleepTime;
      int num = this.CanRetryOnSqlError(sqlError.Number, sqlError.Class, out sleepTime) ? 1 : 0;
      if (!(sleepTime > TimeSpan.Zero))
        return num != 0;
      this.Sleep((int) sleepTime.TotalMilliseconds);
      return num != 0;
    }

    private bool ShouldRetryLostConnection(Exception ex)
    {
      bool flag = false;
      for (Exception exception = ex; exception != null; exception = exception.InnerException)
      {
        if (exception is SqlException sqlException)
        {
          foreach (SqlError error in sqlException.Errors)
          {
            if (this.IsSqlInstrumentationEnabled && string.Compare("prc_InjectChaosMonkeyFailure", error.Procedure, StringComparison.InvariantCulture) == 0)
              return false;
            if (error.Class >= (byte) 20)
              flag = true;
            else if (error.Number == -2 || error.Number == 701)
              flag = true;
          }
        }
      }
      return flag;
    }

    private SqlScriptResourceComponent.SqlServerCapabilities GetSqlServerCapabilities()
    {
      string dataSource = this.DataSource;
      SqlScriptResourceComponent.SqlServerCapabilities serverCapabilities;
      if (!SqlScriptResourceComponent.s_sqlServerCapabilities.TryGetValue(dataSource, out serverCapabilities))
      {
        this.PrepareSqlBatch(SqlScriptResourceComponent.s_queryCapabilitiesStmt.Length);
        this.AddStatement(SqlScriptResourceComponent.s_queryCapabilitiesStmt);
        serverCapabilities = (SqlScriptResourceComponent.SqlServerCapabilities) (int) this.ExecuteScalar();
        SqlScriptResourceComponent.s_sqlServerCapabilities[dataSource] = serverCapabilities;
      }
      return serverCapabilities;
    }

    private static string SerializeToXml(List<ServiceVersionEntry> serviceVersions)
    {
      string xml = (string) null;
      if (serviceVersions != null && serviceVersions.Count > 0)
      {
        StringBuilder output = new StringBuilder();
        XmlWriterSettings settings = new XmlWriterSettings()
        {
          OmitXmlDeclaration = true,
          ConformanceLevel = ConformanceLevel.Fragment,
          Indent = true
        };
        using (XmlWriter xmlWriter1 = XmlWriter.Create(output, settings))
        {
          foreach (ServiceVersionEntry serviceVersion in serviceVersions)
          {
            xmlWriter1.WriteStartElement("svc");
            xmlWriter1.WriteAttributeString("name", serviceVersion.ServiceName);
            XmlWriter xmlWriter2 = xmlWriter1;
            int num = serviceVersion.Version;
            string str1 = num.ToString();
            xmlWriter2.WriteAttributeString("version", str1);
            XmlWriter xmlWriter3 = xmlWriter1;
            num = serviceVersion.MinVersion;
            string str2 = num.ToString();
            xmlWriter3.WriteAttributeString("minVersion", str2);
            xmlWriter1.WriteEndElement();
          }
        }
        xml = output.ToString();
      }
      return xml;
    }

    private static string GetParameterDef(SqlParameter[] parameters)
    {
      if (parameters == null || parameters.Length == 0)
        return "";
      new StringBuilder().Append("'");
      return "N'" + string.Join(", ", ((IEnumerable<SqlParameter>) parameters).Select<SqlParameter, string>((System.Func<SqlParameter, string>) (parameter => SqlScriptResourceComponent.GetParameterName(parameter) + " " + SqlScriptResourceComponent.GetTypeDeclaration(parameter)))) + "', " + string.Join(", ", ((IEnumerable<SqlParameter>) parameters).Select<SqlParameter, string>((System.Func<SqlParameter, string>) (parameter =>
      {
        string parameterName = SqlScriptResourceComponent.GetParameterName(parameter);
        if (parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.InputOutput)
          parameterName += " OUTPUT";
        return parameterName;
      })));
    }

    private static string GetParameterName(SqlParameter parameter)
    {
      string parameterName = parameter.ParameterName;
      if (!parameterName.StartsWith("@", StringComparison.Ordinal))
        parameterName = "@" + parameterName;
      return parameterName;
    }

    private static string GetTypeDeclaration(SqlParameter parameter)
    {
      string typeDeclaration;
      switch (parameter.SqlDbType)
      {
        case SqlDbType.BigInt:
          typeDeclaration = "BIGINT";
          break;
        case SqlDbType.Binary:
          typeDeclaration = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "BINARY({0})", (object) parameter.Size);
          break;
        case SqlDbType.Bit:
          typeDeclaration = "BIT";
          break;
        case SqlDbType.Char:
          typeDeclaration = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CHAR({0})", (object) parameter.Size);
          break;
        case SqlDbType.Int:
          typeDeclaration = "INT";
          break;
        case SqlDbType.NChar:
          typeDeclaration = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "NCHAR({0})", (object) parameter.Size);
          break;
        case SqlDbType.NVarChar:
          typeDeclaration = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "NVARCHAR({0})", parameter.Size == -1 ? (object) "MAX" : (object) parameter.Size.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          break;
        case SqlDbType.UniqueIdentifier:
          typeDeclaration = "UNIQUEIDENTIFIER";
          break;
        case SqlDbType.SmallInt:
          typeDeclaration = "SMALLINT";
          break;
        case SqlDbType.TinyInt:
          typeDeclaration = "TINYINT";
          break;
        case SqlDbType.VarBinary:
          typeDeclaration = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "VARBINARY({0})", parameter.Size == -1 ? (object) "MAX" : (object) parameter.Size.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          break;
        case SqlDbType.VarChar:
          typeDeclaration = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "VARCHAR({0})", parameter.Size == -1 ? (object) "MAX" : (object) parameter.Size.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          break;
        case SqlDbType.Xml:
          typeDeclaration = "XML";
          break;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} parameters are not supported.", (object) parameter.SqlDbType));
      }
      if (parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.InputOutput)
        typeDeclaration += " OUTPUT";
      return typeDeclaration;
    }

    private object ExecuteStatementImpl(
      string statement,
      SqlParameter[] sqlParameters,
      SqlScriptResourceComponent.ExecutionType executionType,
      bool logStatementInfo,
      int sqlCommandTimeout)
    {
      int commandTimeout = sqlCommandTimeout >= 0 ? sqlCommandTimeout : 3600;
      object obj = (object) null;
      if (logStatementInfo)
      {
        this.Logger.Info("Executing SQL statement: {0}", (object) statement);
        this.Logger.Info("Data source: {0}", (object) this.DataSource);
        this.Logger.Info("Database name: {0}", (object) this.Database);
      }
      try
      {
        this.m_testRerunnability = false;
        this.InfoMessage += new SqlInfoMessageEventHandler(this.OnInfoMessage);
        this.PrepareSqlBatch(statement.Length, commandTimeout);
        this.AddStatement(statement, sqlParameters != null ? sqlParameters.Length : 0, false, false);
        if (sqlParameters != null)
          this.Command.Parameters.AddRange(sqlParameters);
        switch (executionType)
        {
          case SqlScriptResourceComponent.ExecutionType.Scalar:
            obj = this.ExecuteScalar();
            break;
          case SqlScriptResourceComponent.ExecutionType.Reader:
            obj = (object) this.ExecuteReader();
            break;
          default:
            obj = (object) this.ExecuteNonQuery();
            break;
        }
      }
      catch (SqlException ex)
      {
        throw new TeamFoundationServicingException(FrameworkResources.SqlScriptResourceComponentSqlError((object) string.Empty, (object) ex.LineNumber, (object) ex.Message), (Exception) ex);
      }
      catch (Exception ex)
      {
        throw new TeamFoundationServicingException(ex.Message, ex);
      }
      finally
      {
        this.InfoMessage -= new SqlInfoMessageEventHandler(this.OnInfoMessage);
      }
      return obj;
    }

    private void LogInfo(string message) => this.Logger.Info(message);

    private void LogError(string message) => this.Logger.Error(message);

    private void OnInfoMessage(object sender, SqlInfoMessageEventArgs e)
    {
      try
      {
        foreach (SqlError error in e.Errors)
        {
          if (error.Class == (byte) 0 && error.State == (byte) 231)
          {
            string message1 = error.Message;
            try
            {
              if (message1.StartsWith("@b=", StringComparison.Ordinal))
              {
                DateTime utcNow = DateTime.UtcNow;
                string[] strArray = message1.Split(new char[1]
                {
                  '@'
                }, StringSplitOptions.RemoveEmptyEntries);
                this.m_batchIndex = int.Parse(strArray[0].Substring("b=".Length));
                if (!this.m_testRerunnability)
                {
                  DateTime universalTime = DateTime.Parse(strArray[1].Substring("bt=".Length), (IFormatProvider) DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                  this.m_clockSkew = utcNow - universalTime;
                  if (this.m_previousBatchStartTime != DateTime.MinValue && this.m_batchIndex == 0 || this.m_batchIndex > this.m_batches.Count)
                    TeamFoundationTracingService.TraceRaw(99134, TraceLevel.Error, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, string.Format("Batch index is in unexpected range. Batch Index: {0}; Batch count: {1}", (object) this.m_batchIndex, (object) this.m_batches.Count));
                  if (this.m_previousBatchStartTime != DateTime.MinValue && this.m_batchIndex > 0)
                    this.RecordBatchExecutionTime(this.m_batchIndex - 1, (int) (universalTime - this.m_previousBatchStartTime).TotalMilliseconds);
                  this.m_previousBatchStartTime = universalTime;
                  continue;
                }
                continue;
              }
              if (message1.StartsWith("Lock acquired:") && this.m_maxLockTimeSeconds > 0)
              {
                this.m_lockAcquisitionStopwatch = Stopwatch.StartNew();
                using (this.m_lockTimer)
                  ;
                this.m_lockTimer = new Timer(new TimerCallback(this.OnLockTimer), (object) null, this.m_maxLockTimeSeconds * 1000, -1);
                if (!(this.ConnectionInfo is SqlConnectionInfoFactory.DatabaseConnectionInfoIntegratedSecurity))
                {
                  lock (this.m_blockingTimerLock)
                  {
                    this.m_inProcessAcquiringLock = false;
                    if (this.m_blockingDetectionTimer != null)
                      this.m_blockingDetectionTimer.Change(-1, -1);
                    using (this.m_blockingDetectionTimer)
                      ;
                    this.m_blockingDetectionTimerCounter = 0;
                    this.m_blockingDetectionTimer = new Timer(new TimerCallback(this.OnBlockingSessionTimer), (object) null, 5000, -1);
                  }
                }
              }
              else if (message1.StartsWith("@SPID=", StringComparison.Ordinal))
                int.TryParse(message1.Substring("@SPID=".Length), out this.m_sessionId);
              else if (message1.StartsWith("Acquiring servicing lock", StringComparison.Ordinal))
              {
                if (this.IsHosted)
                {
                  this.ParseAndUpdateAttempt(message1);
                  lock (this.m_blockingTimerLock)
                  {
                    this.m_inProcessAcquiringLock = true;
                    this.m_blockingDetectionTimer?.Dispose();
                    this.m_blockingDetectionTimer = new Timer(new TimerCallback(this.OnServicingLockTimer), (object) null, 500, -1);
                  }
                }
              }
              else if (message1.Contains("Timed out while acquiring the lock"))
              {
                lock (this.m_blockingTimerLock)
                {
                  this.m_inProcessAcquiringLock = false;
                  if (this.m_blockingDetectionTimer != null)
                    this.m_blockingDetectionTimer.Change(-1, -1);
                  using (this.m_blockingDetectionTimer)
                    this.m_blockingDetectionTimer = (Timer) null;
                }
              }
            }
            catch (Exception ex)
            {
              string message2 = string.Format("Encounter Exception {0} in processing Sql Message {1}", (object) ex, (object) message1);
              TeamFoundationTracingService.TraceRaw(99133, TraceLevel.Error, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message2);
              this.Logger.Warning(message2);
              throw;
            }
          }
          if (error.IsInformational())
          {
            if (error.Number != 2007 && error.Number != 1945 && !error.IsStatistical())
              this.Logger.Info("SQL Info: {0}", (object) e.Message);
          }
          else
          {
            if (this.m_sqlErrors == null)
              this.m_sqlErrors = new List<SqlError>();
            this.m_sqlErrors.Add(error);
          }
        }
      }
      catch (Exception ex)
      {
        string message = string.Format("OnInfoMessage failed with Exception {0}", (object) ex);
        TeamFoundationTracingService.TraceRaw(99129, TraceLevel.Error, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message);
        this.Logger.Warning(message);
      }
    }

    private void ParseAndUpdateAttempt(string sqlMessage)
    {
      string[] strArray = sqlMessage.Split(new char[1]
      {
        '@'
      }, StringSplitOptions.RemoveEmptyEntries);
      int.TryParse(strArray[1].Substring(strArray[1].IndexOf('=') + 1), out this.m_servicingLockAttempt);
    }

    private void OnLockTimer(object sender)
    {
      try
      {
        if (this.Command == null)
          return;
        this.m_lockTimedOut = true;
        this.Cancel();
        SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Error), FrameworkResources.ServicingLockHeldTimeout());
      }
      catch (Exception ex)
      {
        string message = string.Format("OnLockTimer failed with Exception {0}", (object) ex);
        TeamFoundationTracingService.TraceRaw(99128, TraceLevel.Error, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message);
        SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Warning), message);
      }
    }

    private void LogScriptRunningTime()
    {
      foreach (KeyValuePair<string, int> keyValuePair in this.m_scriptsExecTime)
      {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds((double) keyValuePair.Value);
        if (timeSpan.TotalMilliseconds > 0.0)
          this.Logger.Info("Execution of script \"{0}\" took {1:g}.", (object) keyValuePair.Key, (object) timeSpan);
      }
      foreach (KeyValuePair<int, int> slowBatches in this.m_slowBatchesList)
      {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds((double) slowBatches.Key);
        int index = slowBatches.Value;
        this.Logger.Info("Execution of batch {0} in script \"{1}\" (Line: {2}) took {3:g}.", (object) index, (object) this.m_batches[index].ScriptName, (object) this.m_batches[index].LineNumber, (object) timeSpan);
      }
    }

    private void RecordFailedBatchExecutionTime()
    {
      if (!(this.m_previousBatchStartTime != DateTime.MinValue))
        return;
      this.RecordBatchExecutionTime(this.m_batchIndex, (int) (DateTime.UtcNow - this.m_clockSkew - this.m_previousBatchStartTime).TotalMilliseconds);
    }

    private void RecordBatchExecutionTime(int batchIndex, int batchTimeMilliseconds)
    {
      this.m_scriptsExecTime[this.m_batches[batchIndex].ScriptName] += batchTimeMilliseconds;
      if (batchTimeMilliseconds < 2000)
        return;
      if (this.m_slowBatchesList.Count < 5)
      {
        this.m_slowBatchesList.Add(new KeyValuePair<int, int>(batchTimeMilliseconds, batchIndex));
        this.m_slowBatchesList = this.m_slowBatchesList.OrderByDescending<KeyValuePair<int, int>, int>((System.Func<KeyValuePair<int, int>, int>) (x => x.Key)).ToList<KeyValuePair<int, int>>();
      }
      else
      {
        if (batchTimeMilliseconds <= this.m_slowBatchesList.Last<KeyValuePair<int, int>>().Key)
          return;
        this.m_slowBatchesList.RemoveAt(this.m_slowBatchesList.Count - 1);
        this.m_slowBatchesList.Add(new KeyValuePair<int, int>(batchTimeMilliseconds, batchIndex));
        this.m_slowBatchesList = this.m_slowBatchesList.OrderByDescending<KeyValuePair<int, int>, int>((System.Func<KeyValuePair<int, int>, int>) (x => x.Key)).ToList<KeyValuePair<int, int>>();
      }
    }

    private void OnBlockingSessionTimer(object sender)
    {
      try
      {
        bool flag = this.CheckBlockingScenario();
        lock (this.m_blockingTimerLock)
        {
          if (flag && this.m_blockingDetectionTimer != null)
          {
            if (this.m_acquireLock)
              this.m_blockingDetectionTimer.Change(5000, -1);
            else if (this.m_maxBlockingTimeSeconds > 0)
              this.m_blockingDetectionTimer.Change(this.m_maxBlockingTimeSeconds * 1000, -1);
          }
          ++this.m_blockingDetectionTimerCounter;
        }
      }
      catch (Exception ex)
      {
        string message = string.Format("Check blocking scenarios failed with Exception {0}", (object) ex);
        TeamFoundationTracingService.TraceRaw(99124, TraceLevel.Error, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message);
        SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Warning), message);
      }
    }

    private void OnServicingLockTimer(object sender)
    {
      try
      {
        this.CheckSessionsHoldingApplicationLock();
        if (this.m_servicingLockAttempt > this.AcquireLockMaxAttempts - 8)
          return;
        this.CheckBlockedUserRequests();
      }
      catch (Exception ex)
      {
        string message = string.Format("Check blocked sessions by servicing lock failed with Exception {0}", (object) ex);
        TeamFoundationTracingService.TraceRaw(99127, TraceLevel.Error, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message);
        SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Warning), message);
      }
    }

    private bool CheckBlockingScenario()
    {
      DmvSessionRequestInfo blockingSession = (DmvSessionRequestInfo) null;
      if (this.m_acquireLock && this.m_blockingDetectionTimerCounter > 1)
      {
        int num = 0;
        List<DatabaseManagementViewResult> source;
        using (SessionManagementComponent componentRaw = this.ConnectionInfo.CreateComponentRaw<SessionManagementComponent>())
          source = componentRaw.QueryWhatsRunning();
        if (!this.IsSqlAzure)
          source = source.Where<DatabaseManagementViewResult>((System.Func<DatabaseManagementViewResult, bool>) (x => x.DatabaseName == null || x.DatabaseName.Equals(this.ConnectionInfo.InitialCatalog, StringComparison.OrdinalIgnoreCase))).ToList<DatabaseManagementViewResult>();
        foreach (DatabaseManagementViewResult managementViewResult in source)
        {
          if (managementViewResult.WaitType == "LCK_M_S" && managementViewResult.WaitResource.Contains(FrameworkServerConstants.VerifyVersionLock))
            ++num;
          else if ((int) managementViewResult.SessionId != this.m_sessionId)
            SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Info), string.Format("QueryWhatsRunning result during servicing lock:\r\n{0}", (object) managementViewResult));
        }
        SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Info), string.Format("QueryWhatsRunning result during servicing lock: {0} sessions are blocked by current session {1} with servicing lock", (object) num, (object) this.m_sessionId));
      }
      int maxBlockingTimeSeconds = this.m_acquireLock ? 5 : this.m_maxBlockingTimeSeconds;
      using (SessionManagementComponent componentRaw = this.ConnectionInfo.CreateComponentRaw<SessionManagementComponent>())
        blockingSession = componentRaw.QueryBlockingSession(this.m_sessionId, maxBlockingTimeSeconds);
      bool flag;
      if (blockingSession != null)
      {
        string message = string.Format("Session {0} (root of the blocking chain) is blocking servicing session {1} more than {2} seconds. ", (object) blockingSession.SessionId, (object) this.m_sessionId, (object) maxBlockingTimeSeconds) + string.Format("See session Details: {0}", (object) blockingSession);
        TeamFoundationTracingService.TraceRaw(99125, TraceLevel.Warning, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message);
        SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Warning), message);
        flag = this.CheckBlockingSession(blockingSession);
      }
      else
        flag = this.CheckBlockedUserRequests();
      return flag;
    }

    private bool CheckBlockingSession(DmvSessionRequestInfo blockingSession)
    {
      if (this.m_acquireLock)
      {
        string message = string.Format("Kill blocking session {0} to unblock session {1} with servicing lock.", (object) blockingSession.SessionId, (object) this.m_sessionId);
        TeamFoundationTracingService.TraceRaw(99141, TraceLevel.Warning, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message);
        SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Warning), message);
        int num = (int) this.KillSession(blockingSession.SessionId);
      }
      else if (blockingSession.SessionStatus.Equals("Sleeping", StringComparison.OrdinalIgnoreCase))
      {
        int num1 = (int) this.KillSession(blockingSession.SessionId);
      }
      return true;
    }

    public KillSqlSessionResult KillSession(int sessionId)
    {
      try
      {
        using (SessionManagementComponent componentRaw = this.ConnectionInfo.CreateComponentRaw<SessionManagementComponent>())
        {
          if (componentRaw.Kill(sessionId))
          {
            string message = string.Format("Session {0} has been killed.", (object) sessionId);
            TeamFoundationTracingService.TraceRaw(99121, TraceLevel.Warning, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message);
            SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Warning), message);
            return KillSqlSessionResult.Success;
          }
          string message1 = string.Format("Session {0} does not exist.", (object) sessionId);
          TeamFoundationTracingService.TraceRaw(99140, TraceLevel.Warning, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message1);
          SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Warning), message1);
          return KillSqlSessionResult.DidNotExist;
        }
      }
      catch (Exception ex)
      {
        string message = string.Format("Attempt to KILL session {0} failed. Exception details: {1}", (object) sessionId, (object) ex.Message);
        TeamFoundationTracingService.TraceRaw(99122, TraceLevel.Error, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message);
        SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Warning), message);
        return KillSqlSessionResult.Fail;
      }
    }

    private void CheckSessionsHoldingApplicationLock()
    {
      List<DmvTranLockSessionInfo> list;
      using (SessionManagementComponent componentRaw = this.ConnectionInfo.CreateComponentRaw<SessionManagementComponent>())
        list = componentRaw.QuerySessionsHoldingApplicationLock(FrameworkServerConstants.VerifyVersionLock).Where<DmvTranLockSessionInfo>((System.Func<DmvTranLockSessionInfo, bool>) (s => s.SessionId != this.m_sessionId)).OrderByDescending<DmvTranLockSessionInfo, int>((System.Func<DmvTranLockSessionInfo, int>) (s => s.ElapsedMilliseconds)).ToList<DmvTranLockSessionInfo>();
      string message = string.Format("Attempt {0} blocked by {1} sessions", (object) this.m_servicingLockAttempt, (object) list.Count);
      TeamFoundationTracingService.TraceRaw(99130, TraceLevel.Warning, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message);
      SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Warning), message);
      foreach (DmvTranLockSessionInfo tranLockSessionInfo in list.Take<DmvTranLockSessionInfo>(5))
      {
        message = string.Format("Session {0} blocks attempt to acquire servicing lock by {1:0.00} seconds, details: {2}", (object) tranLockSessionInfo.SessionId, (object) ((double) tranLockSessionInfo.ElapsedMilliseconds / 1000.0), (object) tranLockSessionInfo);
        SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Warning), message);
      }
      if (!list.Any<DmvTranLockSessionInfo>() || !this.AggressivelyAcquireLock)
        return;
      ParallelOptions parallelOptions = new ParallelOptions()
      {
        MaxDegreeOfParallelism = 4
      };
      Parallel.ForEach<DmvTranLockSessionInfo>((IEnumerable<DmvTranLockSessionInfo>) list, parallelOptions, (Action<DmvTranLockSessionInfo, ParallelLoopState>) ((s, loopState) =>
      {
        try
        {
          if (!this.m_inProcessAcquiringLock)
            loopState.Stop();
          using (SessionManagementComponent componentRaw = this.ConnectionInfo.CreateComponentRaw<SessionManagementComponent>())
          {
            if (!componentRaw.Kill(s.SessionId))
              return;
            message = string.Format("In attempt {0}, session {1} was killed to unblock acquiring servicing lock.", (object) this.m_servicingLockAttempt, (object) s.SessionId);
            TeamFoundationTracingService.TraceRaw(99131, TraceLevel.Warning, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message);
            SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Warning), message);
          }
        }
        catch (Exception ex)
        {
          message = string.Format("Attempt to kill blocking session {0} failed. Exception details: {1}", (object) s.SessionId, (object) ex.Message);
          TeamFoundationTracingService.TraceRaw(99132, TraceLevel.Error, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message);
          SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Warning), message);
        }
      }));
    }

    private bool CheckBlockedUserRequests()
    {
      bool flag = true;
      if (this.m_acquireLock)
      {
        List<DmvTranLockSessionInfo> source;
        using (SessionManagementComponent componentRaw = this.ConnectionInfo.CreateComponentRaw<SessionManagementComponent>())
          source = componentRaw.QuerySessionsBlockedByApplicationLock(FrameworkServerConstants.VerifyVersionLock);
        if (source.Any<DmvTranLockSessionInfo>())
        {
          string message = string.Format("There are {0} sessions blocked by servicing lock, session ids are as follows:", (object) source.Count) + string.Join<int>(", ", source.Select<DmvTranLockSessionInfo, int>((System.Func<DmvTranLockSessionInfo, int>) (bs => bs.SessionId)));
          SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Warning), message);
          TeamFoundationTracingService.TraceRaw(99126, TraceLevel.Error, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message);
          int num1 = 0;
          int num2 = 0;
          int num3 = 0;
          int num4 = 0;
          int num5 = 0;
          int num6 = 0;
          int num7 = 0;
          int num8 = 0;
          for (int index = 0; index < source.Count; ++index)
          {
            int elapsedMilliseconds = source[index].ElapsedMilliseconds;
            if (elapsedMilliseconds > 10000)
            {
              if (elapsedMilliseconds > 30000)
              {
                if (elapsedMilliseconds > 40000)
                  ++num1;
                else
                  ++num2;
              }
              else if (elapsedMilliseconds > 20000)
                ++num3;
              else
                ++num4;
            }
            else if (elapsedMilliseconds > 3000)
            {
              if (elapsedMilliseconds > 5000)
                ++num5;
              else
                ++num6;
            }
            else if (elapsedMilliseconds > 1000)
              ++num7;
            else
              ++num8;
          }
          SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Info), string.Format("Blocked sessions (time in seconds). >40: {0}, >30: {1}, >20: {2}, >10: {3}, >5: {4}, >3: {5}, >1: {6}, <=1: {7}", (object) num1, (object) num2, (object) num3, (object) num4, (object) num5, (object) num6, (object) num7, (object) num8));
        }
      }
      else
      {
        List<DmvBlockedSessionInfo> blockedSessionInfoList;
        using (SessionManagementComponent componentRaw = this.ConnectionInfo.CreateComponentRaw<SessionManagementComponent>())
          blockedSessionInfoList = componentRaw.QueryBlockedSession(this.m_sessionId, this.m_maxBlockingTimeSeconds);
        if (blockedSessionInfoList.Count >= 10)
        {
          string message = string.Format(string.Format("{0} sessions are blocked by current session {1} by more than {2} seconds, cancel the current request. Cancel the current operation to unblock user requests.", (object) blockedSessionInfoList.Count, (object) this.m_sessionId, (object) this.m_maxBlockingTimeSeconds));
          SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Error), message);
          TeamFoundationTracingService.TraceRaw(99123, TraceLevel.Error, SqlScriptResourceComponent.s_area, SqlScriptResourceComponent.s_layer, message);
          foreach (object obj in blockedSessionInfoList)
            SqlScriptResourceComponent.TryLog(new Action<string>(this.Logger.Info), string.Format("Blocked session detail: {0}", obj));
          this.Cancel();
          flag = false;
        }
      }
      return flag;
    }

    private static void TryLog(Action<string> action, string message)
    {
      try
      {
        action(message);
      }
      catch (ObjectDisposedException ex)
      {
      }
    }

    private bool IsSqlInstrumentationEnabled
    {
      get
      {
        if (!this.m_isSqlInjectionEnabled.HasValue)
          this.m_isSqlInjectionEnabled = new bool?(!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TFS_INSERT_SQL_FAILURE_TEST")));
        return this.m_isSqlInjectionEnabled.Value;
      }
    }

    private ISqlInstrumentator SqlInstrumentator
    {
      get
      {
        if (this.m_sqlInstrumentator == null && this.IsSqlInstrumentationEnabled)
        {
          string environmentVariable = Environment.GetEnvironmentVariable("TFS_INSERT_SQL_FAILURE_TEST");
          try
          {
            string[] strArray = environmentVariable.Split(';');
            this.m_sqlInstrumentator = (ISqlInstrumentator) Activator.CreateInstanceFrom(strArray[0], strArray[1]).Unwrap();
          }
          catch (Exception ex)
          {
            this.Logger?.Warning("[BEST EFFORT] Failed to load create Sql Injection Object from {0}: {1}", (object) environmentVariable, (object) ex.Message);
          }
        }
        return this.m_sqlInstrumentator;
      }
    }

    private bool IsHosted => !(this.ConnectionInfo is SqlConnectionInfoFactory.DatabaseConnectionInfoIntegratedSecurity);

    private bool AggressivelyAcquireLock => this.m_servicingLockAttempt > this.AcquireLockMaxAttempts - 8;

    private enum ExecutionType
    {
      NonQuery,
      Scalar,
      Reader,
    }

    private class ServicingLockRequestInfo
    {
      public string Spid { get; set; }

      public string LockRequestStatus { get; set; }

      public string ResourceType { get; set; }

      public string RequestOwnerType { get; set; }

      public string RequestMode { get; set; }

      public string DatabaseId { get; set; }

      public string Description { get; set; }

      public string Status { get; set; }

      public string BlockingSpid { get; set; }

      public string StartTime { get; set; }

      public string DurationInMilliseconds { get; set; }

      public string HostName { get; set; }

      public string LoginName { get; set; }

      public string Content { get; set; }

      public string Stmt { get; set; }

      public override string ToString() => "Application Lock Request Info:\r\n    Session Id:             " + this.Spid + "\r\n    Lock Request Status:    " + this.LockRequestStatus + "\r\n    Resource Type:          " + this.ResourceType + "\r\n    Request Owner Type:     " + this.RequestOwnerType + "\r\n    Request Mode:           " + this.RequestMode + "\r\n    Database Id:            " + this.DatabaseId + "\r\n    Description:            " + this.Description + "\r\n    Request Status:         " + this.Status + "\r\n    Blocking Spid:          " + this.BlockingSpid + "\r\n    Start Time:             " + this.StartTime + "\r\n    Duration (ms):          " + this.DurationInMilliseconds + "\r\n    HostName:               " + this.HostName + "\r\n    LoginName:              " + this.LoginName + "\r\n    Running Statement:      " + this.Stmt + "\r\n    Content:                " + this.Content;
    }

    [Flags]
    private enum SqlServerCapabilities : byte
    {
      None = 0,
      PageCompression = 1,
      OnlineIndexing = 2,
    }
  }
}
