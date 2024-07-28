// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationDataTierComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationDataTierComponent : TeamFoundationSqlResourceComponent
  {
    private const int c_maxDatabaseCreationAttempts = 32767;
    private const string c_stmtGetDatabaseCount = "SELECT COUNT(*) dbCount FROM sys.databases";
    private const string c_stmtGetDatabaseNames = "SELECT name FROM sys.databases ORDER BY database_id";
    private const string c_stmtGetDatabaseCreateDate = "SELECT create_date FROM sys.databases WHERE name=@databaseName";
    private const string c_stmtCheckIfDatabaseExist = "SELECT CONVERT(BIT, COUNT(*)) FROM sys.databases WHERE name=@databaseName";
    private const string c_stmtGetServerName = "SELECT SERVERPROPERTY('ServerName')";
    private const string c_stmtAlterDatabaseFileSizeFormat = "ALTER DATABASE {0} MODIFY FILE ( NAME = N{1}, SIZE = {2}MB )";
    private static readonly string s_area = "DataTier";
    private static readonly string s_layer = "Component";

    protected override ITFLogger GetDefaultLogger() => (ITFLogger) new ServerTraceLogger();

    public void AlterSqlAzureDatabaseMaxSize(string databaseName, int maxSizeInGB)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      this.VerifyInMasterDbOnAzure();
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ALTER DATABASE {0} MODIFY (MAXSIZE = {1}GB)", (object) StringUtil.QuoteName(databaseName), (object) maxSizeInGB);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
    }

    public void AlterDatabaseName(string databaseName, string newDatabaseName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      ArgumentUtility.CheckStringForNullOrEmpty(newDatabaseName, nameof (newDatabaseName));
      this.VerifyInMasterDbOnAzure();
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ALTER DATABASE [{0}] MODIFY Name = [{1}]", (object) databaseName, (object) newDatabaseName);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
    }

    public void AlterDatabaseServiceObjectives(
      string databaseName,
      int maxSizeInMB,
      DatabaseServiceEdition edition,
      DatabaseServiceObjective objective)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      this.VerifyInMasterDbOnAzure();
      string sqlStatement;
      if (maxSizeInMB == 0)
        sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ALTER DATABASE {0} MODIFY(edition = '{1}', service_objective = '{2}')", (object) StringUtil.QuoteName(databaseName), (object) edition.ToString(), (object) objective.ToString());
      else
        sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ALTER DATABASE {0} MODIFY(maxsize ={1}MB, edition = '{2}', service_objective = '{3}')", (object) StringUtil.QuoteName(databaseName), (object) maxSizeInMB, (object) edition.ToString(), (object) objective.ToString());
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
    }

    public void AlterDatabaseOperationMode(string databaseName, DatabaseOperationMode operationMode)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      this.VerifyInMasterDbOnAzure();
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, string.Format("ALTER  DATABASE {0} SET {1} WITH ROLLBACK IMMEDIATE", (object) StringUtil.QuoteName(databaseName), (object) operationMode));
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
    }

    public void AlterDatabaseServiceObjectives(
      string databaseName,
      DatabaseServiceObjective objective)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      this.VerifyInMasterDbOnAzure();
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ALTER  DATABASE {0} MODIFY(service_objective = '{1}')", (object) StringUtil.QuoteName(databaseName), (object) objective.ToString());
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
    }

    public void CreateDatabase(
      string databaseName,
      VsoCreateDatabaseOptions options,
      out bool databaseCreated)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      ArgumentUtility.CheckForNull<VsoCreateDatabaseOptions>(options, nameof (options));
      try
      {
        TeamFoundationTracingService.TraceRawAlwaysOn(99003, TraceLevel.Info, TeamFoundationDataTierComponent.s_area, TeamFoundationDataTierComponent.s_layer, "Start TeamFoundationDataTierComponent.CreateDatabase. DatabaseName:" + databaseName);
        if (this.IsSqlAzure)
          this.CreateSqlAzureDatabase(databaseName, options);
        else
          this.CreateSqlServerDatabase(databaseName, options);
      }
      catch (SqlException ex)
      {
        System.Func<SqlError, bool> predicate = (System.Func<SqlError, bool>) (error => error.Number == 1801 || error.Message.IndexOf("Msg 1801", StringComparison.InvariantCultureIgnoreCase) >= 0);
        if (ex.Errors.Cast<SqlError>().Any<SqlError>(predicate))
        {
          TeamFoundationTracingService.TraceRawAlwaysOn(99005, TraceLevel.Info, TeamFoundationDataTierComponent.s_area, TeamFoundationDataTierComponent.s_layer, "Hit DatabaseAlreadyExistsException during TeamFoundationDataTierComponent.CreateDatabase. DatabaseName:" + databaseName);
          if (this.CheckIfDatabaseExists(databaseName))
          {
            TeamFoundationTracingService.TraceRawAlwaysOn(99006, TraceLevel.Info, TeamFoundationDataTierComponent.s_area, TeamFoundationDataTierComponent.s_layer, "Database " + databaseName + " is already created, continue.");
          }
          else
          {
            TeamFoundationTracingService.TraceRaw(99007, TraceLevel.Error, TeamFoundationDataTierComponent.s_area, TeamFoundationDataTierComponent.s_layer, "Database " + databaseName + " does not exist even we hit DatabaseAlreadyExistsException!");
            throw;
          }
        }
        else
          throw;
      }
      databaseCreated = true;
      if (!this.IsSqlAzure)
        this.GetDboSid(databaseName);
      TeamFoundationTracingService.TraceRawAlwaysOn(99901, TraceLevel.Info, TeamFoundationDataTierComponent.s_area, TeamFoundationDataTierComponent.s_layer, "CreateDatabase;DatabaseName:" + databaseName + ";ServerName:" + TeamFoundationDataTierService.ManipulateDataSource(this.DataSource, DataSourceOptions.RemoveProtocol));
      options.QueryStoreOptions.DatabaseName = databaseName;
      if (!new SqlConnectionStringBuilder(this.ConnectionInfo.ConnectionString).IntegratedSecurity)
        this.SetQueryStoreOptions(options.QueryStoreOptions);
      TeamFoundationTracingService.TraceRawAlwaysOn(99004, TraceLevel.Info, TeamFoundationDataTierComponent.s_area, TeamFoundationDataTierComponent.s_layer, "End TeamFoundationDataTierComponent.CreateDatabase. DatabaseName:" + databaseName);
    }

    public void EnableTde()
    {
      this.VerifySqlAzure();
      this.VerifyNotInMaster();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_EnableTde.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.Logger.Info("Enabling TDE on " + this.Database);
      this.ExecuteNonQuery();
    }

    public DatabaseEncryptionState QueryTde()
    {
      this.VerifySqlAzure();
      this.VerifyNotInMaster();
      this.PrepareSqlBatch("SELECT encryption_state FROM sys.dm_database_encryption_keys WHERE database_id = (SELECT database_id FROM sys.databases WHERE name = DB_NAME())".Length);
      this.AddStatement("SELECT encryption_state FROM sys.dm_database_encryption_keys WHERE database_id = (SELECT database_id FROM sys.databases WHERE name = DB_NAME())");
      object obj = this.ExecuteScalar();
      if (obj == null)
        return DatabaseEncryptionState.NoEncryption;
      return Enum.IsDefined(typeof (DatabaseEncryptionState), obj) ? (DatabaseEncryptionState) obj : throw new InvalidCastException(string.Format("Invalid value for {0}: {1}", (object) "DatabaseEncryptionState", obj));
    }

    public void SetQueryStoreOptions(Microsoft.TeamFoundation.Framework.Server.SetQueryStoreOptions options)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(options.DatabaseName, "DatabaseName");
      ArgumentUtility.CheckForOutOfRange(options.StaleQueryThresholdDays, "StaleQueryThresholdDays", 0L);
      ArgumentUtility.CheckForOutOfRange(options.MaxStorageSizeMB, "MaxStorageSizeMB", 0L);
      ArgumentUtility.CheckForOutOfRange(options.IntervalLengthMinutes, "IntervalLengthMinutes", 0L);
      ArgumentUtility.CheckForOutOfRange(options.FlushIntervalSeconds, "FlushIntervalSeconds", 0L);
      ArgumentUtility.CheckForOutOfRange(options.MaxPlansPerQuery, "MaxPlansPerQuery", 0L);
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_SetQueryStoreOptions.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@databaseName", options.DatabaseName, false);
      this.BindInt("@operationMode", (int) options.OperationMode);
      this.BindLong("@staleQueryThresholdDays", options.StaleQueryThresholdDays);
      this.BindLong("@flushIntervalSeconds", options.FlushIntervalSeconds);
      this.BindLong("@maxStorageSizeMB", options.MaxStorageSizeMB);
      this.BindLong("@intervalLengthMinutes", options.IntervalLengthMinutes);
      this.BindInt("@sizeBasedCleanupMode", (int) options.SizeBasedCleanupMode);
      this.BindInt("@queryCaptureMode", (int) options.QueryCaptureMode);
      this.BindLong("@maxPlansPerQuery", options.MaxPlansPerQuery);
      this.Logger.Info(string.Format("Setting SQL Server Query Store options: {0}", (object) options));
      this.Logger.Info("Setting SQL Server Query Store with statement: " + resourceAsString);
      this.ExecuteNonQuery();
    }

    public byte[] GetDboSid(string databaseName)
    {
      string sqlStatement = "SELECT sid FROM " + StringUtil.QuoteName(databaseName) + ".sys.database_principals WHERE name = 'dbo'";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      return (byte[]) this.ExecuteScalar();
    }

    public QueryStoreOptions GetQueryStoreOptions()
    {
      this.VerifyNotInMaster();
      this.Logger.Info("Getting query store options for: " + this.DataSource + ";" + this.Database);
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetQueryStoreOptions.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueryStoreOptions>((ObjectBinder<QueryStoreOptions>) new QueryStoreOptionsColumns());
      return resultCollection.GetCurrent<QueryStoreOptions>().Items.FirstOrDefault<QueryStoreOptions>();
    }

    public void ClearQueryStore()
    {
      this.Logger.Info("Clearing query store for: " + this.DataSource + ";" + this.Database);
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_ClearQueryStore.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@databaseName", this.Database, false);
      this.ExecuteNonQuery();
    }

    public void DisableQueryStore()
    {
      this.Logger.Info("Disabling query store for: " + this.DataSource + ";" + this.Database);
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_DisableQueryStore.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@databaseName", this.InitialCatalog, false);
      this.ExecuteNonQuery();
    }

    public bool CheckIfDatabaseExists(string databaseName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      this.VerifyInMasterDbOnAzure();
      this.Logger.Info("Checking if the following database exist: {0}. Data Source: {1}.", (object) databaseName, (object) this.DataSource);
      this.PrepareSqlBatch("SELECT CONVERT(BIT, COUNT(*)) FROM sys.databases WHERE name=@databaseName".Length);
      this.AddStatement("SELECT CONVERT(BIT, COUNT(*)) FROM sys.databases WHERE name=@databaseName");
      this.BindSysname("@databaseName", databaseName, false);
      bool flag = (bool) this.ExecuteScalar();
      this.Logger.Info("Database exists: {0}", (object) flag);
      return flag;
    }

    public bool CheckIfDatabaseIsEmpty()
    {
      this.Logger.Info("Checking if the following database is empty: {0}. Data Source: {1}.", (object) this.Database, (object) this.DataSource);
      this.PrepareSqlBatch("SELECT COUNT(*) FROM sys.tables WHERE type='U'".Length);
      this.AddStatement("SELECT COUNT(*) FROM sys.tables WHERE type='U'");
      int num = (int) this.ExecuteScalar();
      this.Logger.Info("Found {0} tables.", (object) num);
      return num == 0;
    }

    public void DropDatabase(string databaseName) => this.DropDatabase(databaseName, DropDatabaseOptions.None);

    public void DropDatabase(string databaseName, DropDatabaseOptions dropDatabaseOptions)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      this.VerifyInMasterDbOnAzure();
      this.Logger.Info("Dropping the following database: {0}. Data Source: {1}.", (object) databaseName, (object) this.DataSource);
      List<DatabaseFileProperties> databaseFiles = this.IsLocalSqlInstance() ? this.GetDatabaseFiles(databaseName) : new List<DatabaseFileProperties>(0);
      if (this.IsSqlAzure || (dropDatabaseOptions & DropDatabaseOptions.CloseExistingConnections) == DropDatabaseOptions.None)
      {
        this.Logger.Info("Dropping without closing existing connections");
        string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_DropDatabase.sql");
        this.PrepareSqlBatch(resourceAsString.Length);
        this.AddStatement(resourceAsString);
        this.BindSysname("@databaseName", databaseName, false);
      }
      else
      {
        this.Logger.Info("Closing existing connections and dropping");
        string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_DropDatabaseCloseExistingConnection.sql");
        this.PrepareSqlBatch(resourceAsString.Length);
        this.AddStatement(resourceAsString);
        this.BindSysname("@databaseName", databaseName, false);
      }
      this.ExecuteNonQuery();
      TeamFoundationTracingService.TraceRawAlwaysOn(99902, TraceLevel.Info, TeamFoundationDataTierComponent.s_area, TeamFoundationDataTierComponent.s_layer, "DropDatabase;DatabaseName:" + databaseName + ";ServerName:" + TeamFoundationDataTierService.ManipulateDataSource(this.DataSource, DataSourceOptions.RemoveProtocol));
      this.DeleteDatabaseFiles(databaseFiles);
    }

    public bool IsLocalSqlInstance()
    {
      if (this.IsSqlAzure)
        return false;
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_IsLocalSqlInstance.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      return (bool) this.ExecuteScalar();
    }

    public int GetDatabaseCount()
    {
      this.VerifyInMasterDbOnAzure();
      this.Logger.Info("Getting database count on the following instance: {0}.", (object) this.DataSource);
      this.PrepareSqlBatch("SELECT COUNT(*) dbCount FROM sys.databases".Length);
      this.AddStatement("SELECT COUNT(*) dbCount FROM sys.databases");
      int databaseCount = (int) this.ExecuteScalar();
      this.Logger.Info("Database count: {0}.", (object) databaseCount);
      return databaseCount;
    }

    public DateTime GetDatabaseCreateDate(string databaseName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      this.VerifyInMasterDbOnAzure();
      this.Logger.Info("Getting create_date for {0}. Data Source: {1}", (object) databaseName, (object) this.DataSource);
      this.PrepareSqlBatch("SELECT create_date FROM sys.databases WHERE name=@databaseName".Length);
      this.AddStatement("SELECT create_date FROM sys.databases WHERE name=@databaseName");
      this.BindSysname("@databaseName", databaseName, false);
      DateTime databaseCreateDate = (DateTime) this.ExecuteScalar();
      this.Logger.Info("Database create_date: {0}", (object) databaseCreateDate);
      return databaseCreateDate;
    }

    public List<DatabaseInformation> GetDatabases()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetDatabases.sql");
      this.Logger.Info("Getting databases on the following instance: {0}.", (object) this.DataSource);
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DatabaseInformation>((ObjectBinder<DatabaseInformation>) new DatabaseInformationColumns());
      return resultCollection.GetCurrent<DatabaseInformation>().Items;
    }

    public DatabaseInformation GetDatabaseInfo(string databaseName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetDatabaseByName.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@databaseName", databaseName, false);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DatabaseInformation>((ObjectBinder<DatabaseInformation>) new DatabaseInformationColumns());
      return resultCollection.GetCurrent<DatabaseInformation>().Items.FirstOrDefault<DatabaseInformation>();
    }

    public TableSizeProperties GetTableSize(string schemaName, string tableName)
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetTableSize.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@schemaName", schemaName, false);
      this.BindSysname("@tableName", tableName, false);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TableSizeProperties>((ObjectBinder<TableSizeProperties>) new TableSizeColumns());
      return resultCollection.GetCurrent<TableSizeProperties>().Items.FirstOrDefault<TableSizeProperties>();
    }

    public virtual List<TableSizeProperties> GetTablesBySizeInDescendingOrder()
    {
      string sqlStatement = "SELECT    t.name  TableName,\r\n            s.name  SchemaName,\r\n            SUM(CASE\r\n                    WHEN p.index_id <= 1 THEN row_count\r\n                    ELSE 0\r\n                END) Row_Count,\r\n            SUM(reserved_page_count) * 8192 SizeInBytes\r\n            FROM    sys.tables t\r\n            JOIN    sys.schemas s\r\n            ON      t.schema_id = s.schema_id\r\n            JOIN    sys.dm_db_partition_stats p\r\n            ON      p.object_id = t.object_id\r\nGROUP BY    t.name, s.name\r\nORDER BY    SizeInBytes DESC";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "GetTablesBySizeInDescendingOrder - BATCH", this.RequestContext);
      resultCollection.AddBinder<TableSizeProperties>((ObjectBinder<TableSizeProperties>) new TableSizeColumns());
      return resultCollection.GetCurrent<TableSizeProperties>().Items;
    }

    public virtual List<IndexSizeProperties> GetIndexSizeProperties()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetIndexSizeProperties.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "GetIndexsizeProperties", this.RequestContext);
      resultCollection.AddBinder<IndexSizeProperties>((ObjectBinder<IndexSizeProperties>) new IndexSizePropertiesColumns());
      return resultCollection.GetCurrent<IndexSizeProperties>().Items;
    }

    public List<DatabaseFileProperties> GetDatabaseFiles()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetDatabaseFiles.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DatabaseFileProperties>((ObjectBinder<DatabaseFileProperties>) new DatabaseFileColumns());
      return resultCollection.GetCurrent<DatabaseFileProperties>().Items;
    }

    public List<DatabaseFileProperties> GetDatabaseFiles(string databaseName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(databaseName, nameof (databaseName));
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetDatabaseFilesFromMaster.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@databaseName", databaseName, false);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DatabaseFileProperties>((ObjectBinder<DatabaseFileProperties>) new DatabaseFileColumns());
      return resultCollection.GetCurrent<DatabaseFileProperties>().Items;
    }

    public DatabaseLogFileUsage GetDatabaseLogFileUsage()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetDatabaseLogFileUsage.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DatabaseLogFileUsage>((ObjectBinder<DatabaseLogFileUsage>) new DatabaseLogFileUsageColumns());
      return resultCollection.GetCurrent<DatabaseLogFileUsage>().Items.FirstOrDefault<DatabaseLogFileUsage>();
    }

    public ResultCollection GetDatabaseNames()
    {
      this.VerifyInMasterDbOnAzure();
      this.Logger.Info("Getting databases on the following instance: {0}.", (object) this.DataSource);
      this.PrepareSqlBatch("SELECT name FROM sys.databases ORDER BY database_id".Length);
      this.AddStatement("SELECT name FROM sys.databases ORDER BY database_id");
      ResultCollection databaseNames = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      databaseNames.AddBinder<string>((ObjectBinder<string>) new DatabaseNameColumns());
      return databaseNames;
    }

    public AzureDatabaseProperties GetDatabaseProperties()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetDatabaseProperties.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<AzureDatabaseProperties>((ObjectBinder<AzureDatabaseProperties>) new DatabasePropertiesColumns());
      return resultCollection.GetCurrent<AzureDatabaseProperties>().Items.FirstOrDefault<AzureDatabaseProperties>();
    }

    public ServiceObjectiveAssignmentState GetDatabaseServiceObjectiveAssignmentState(
      string databaseName,
      DateTime? startTime = null)
    {
      this.VerifyInMasterDbOnAzure();
      string sqlStatement = "SELECT state_desc, last_modify_time, start_time, percent_complete, error_code, error_desc\r\nFROM  sys.dm_operation_status\r\nWHERE major_resource_id = @databaseName\r\nAND operation = 'ALTER DATABASE'";
      if (startTime.HasValue)
        sqlStatement += "\r\nAND start_time > @startTime";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.BindSysname("@databaseName", databaseName, false);
      if (startTime.HasValue)
        this.BindDateTime("@startTime", startTime.Value - TimeSpan.FromSeconds(5.0));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DatabaseOperationStatus>((ObjectBinder<DatabaseOperationStatus>) new DatabaseOperationStatusColumns());
      List<DatabaseOperationStatus> items = resultCollection.GetCurrent<DatabaseOperationStatus>().Items;
      if (items.Count == 0)
        return ServiceObjectiveAssignmentState.None;
      if (!items.All<DatabaseOperationStatus>((System.Func<DatabaseOperationStatus, bool>) (s => !s.State.Equals("IN_PROGRESS", StringComparison.OrdinalIgnoreCase) && !s.State.Equals("PENDING", StringComparison.OrdinalIgnoreCase))))
        return ServiceObjectiveAssignmentState.Pending;
      DatabaseOperationStatus databaseOperationStatus = items.OrderByDescending<DatabaseOperationStatus, DateTime>((System.Func<DatabaseOperationStatus, DateTime>) (s => s.LastModifyTime)).First<DatabaseOperationStatus>();
      this.Logger.Info(string.Format("The latest database objective change took {0} seconds to transition to {1}, Error code: {2}, Error Description: {3}.", (object) (databaseOperationStatus.LastModifyTime - databaseOperationStatus.StartTime).Seconds, (object) databaseOperationStatus.State, (object) databaseOperationStatus.ErrorCode, (object) databaseOperationStatus.ErrorDesc));
      return !databaseOperationStatus.State.Equals("FAILED") ? ServiceObjectiveAssignmentState.Complete : ServiceObjectiveAssignmentState.Failed;
    }

    public ServiceObjectiveEnableState GetServiceObjectiveEnableState(
      DatabaseServiceObjective objective)
    {
      this.VerifyInMasterDbOnAzure();
      string sqlStatement = string.Format("SELECT state_desc FROM slo_service_objectives WHERE name = @objective");
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.BindString("@objective", objective.ToString(), (int) sbyte.MaxValue, false, SqlDbType.NVarChar);
      this.AddStatement("SELECT name FROM sys.databases ORDER BY database_id");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ServiceObjectiveEnableState>((ObjectBinder<ServiceObjectiveEnableState>) new DatabaseServiceObjectiveEnableStateColumns());
      return resultCollection.GetCurrent<ServiceObjectiveEnableState>().Items.FirstOrDefault<ServiceObjectiveEnableState>();
    }

    public string RenameDatabase(string databaseName, string newDatabaseName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      ArgumentUtility.CheckStringForNullOrEmpty(newDatabaseName, nameof (newDatabaseName));
      this.VerifyNotSqlAzure();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_RenameDatabase.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@dbName", databaseName, false);
      SqlParameter sqlParameter = this.BindString("@newName", newDatabaseName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      sqlParameter.Direction = ParameterDirection.InputOutput;
      sqlParameter.Size = 256;
      this.ExecuteNonQuery();
      return (string) sqlParameter.Value;
    }

    internal void SetDatabaseParameterization(
      string databaseName,
      DatabaseParameterization databaseParameterization)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      this.VerifyInMasterDbOnAzure();
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ALTER DATABASE {0} SET PARAMETERIZATION {1}  WITH ROLLBACK IMMEDIATE", (object) StringUtil.QuoteName(databaseName), (object) databaseParameterization.ToString().ToUpperInvariant());
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
    }

    public void CopyAzureDatabase(string sourceSqlInstanceName, string databaseName) => this.CopyAzureDatabase(sourceSqlInstanceName, databaseName, databaseName);

    public void CopyAzureDatabase(
      string sourceSqlInstanceName,
      string sourceDatabaseName,
      string targetDatabaseName)
    {
      if (this.CheckIfDatabaseExists(targetDatabaseName))
      {
        this.Logger.Info("The database exists, Skipping the copy");
      }
      else
      {
        string str1;
        if (!string.IsNullOrWhiteSpace(sourceSqlInstanceName))
        {
          string str2 = sourceSqlInstanceName;
          str1 = string.Format("{0}.{1}", (object) StringUtil.QuoteName(str2.Substring(str2.IndexOf(":") + 1).Split('.')[0]), (object) StringUtil.QuoteName(sourceDatabaseName));
        }
        else
          str1 = string.Format("{0}", (object) StringUtil.QuoteName(sourceDatabaseName));
        string sqlStatement1 = string.Format("CREATE DATABASE {0} AS COPY OF {1}", (object) StringUtil.QuoteName(targetDatabaseName), (object) str1);
        this.Logger.Info("Creating Azure Database {0} as copy of {1}", (object) targetDatabaseName, (object) str1);
        this.PrepareSqlBatch(sqlStatement1.Length);
        this.AddStatement(sqlStatement1);
        if (this.ExecuteNonQuery() != 0)
          return;
        try
        {
          string sqlStatement2 = "SELECT getutcdate(), @@SPID";
          this.PrepareSqlBatch(sqlStatement2.Length);
          this.AddStatement(sqlStatement2);
          using (SqlDataReader sqlDataReader = this.ExecuteReader())
          {
            while (sqlDataReader.Read())
              this.Logger.Info("UTC DateTime: '{0}', SPID: '{1}'", (object) sqlDataReader.GetDateTime(0), (object) sqlDataReader.GetInt16(1));
          }
        }
        catch (Exception ex)
        {
          this.Logger.Warning("Caught error '{0}' when logging additional information about the SQL Azure database copy.", (object) ex.Message);
        }
      }
    }

    public List<string> GetDatatierLogins()
    {
      string sqlStatement = "SELECT name FROM sys.sql_logins";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<string>((ObjectBinder<string>) new DatabaseNameColumns());
      return resultCollection.GetCurrent<string>().Items;
    }

    public List<string> GetDatatierUsers()
    {
      string sqlStatement = "SELECT name FROM sys.database_principals WHERE type_desc='SQL_USER' AND default_schema_name='dbo'";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<string>((ObjectBinder<string>) new DatabaseNameColumns());
      return resultCollection.GetCurrent<string>().Items;
    }

    public bool IsDatabaseCopyComplete(string databaseName)
    {
      this.Logger.Info("Checking if the database copy {0} is complete, executing", (object) databaseName);
      string str = "SELECT STATE FROM sys.databases WHERE Name=@dbName";
      this.PrepareSqlBatch(str.Length);
      this.AddStatement(str);
      this.BindSysname("@dbName", databaseName, false);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext);
      resultCollection.AddBinder<AzureDatabaseCopyState>((ObjectBinder<AzureDatabaseCopyState>) new DatabaseCopyStateBinder());
      return resultCollection.GetCurrent<AzureDatabaseCopyState>().FirstOrDefault<AzureDatabaseCopyState>() == AzureDatabaseCopyState.Online;
    }

    public void AlterDatabaseFileSize(string databaseName, string fileLogicalName, int sizeMB)
    {
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ALTER DATABASE {0} MODIFY FILE ( NAME = N{1}, SIZE = {2}MB )", (object) StringUtil.QuoteName(databaseName), (object) StringUtil.QuoteName(fileLogicalName, '\''), (object) sizeMB);
      this.Logger.Info("Altering file size of {0} with command: {1}", (object) fileLogicalName, (object) sqlStatement);
      this.PrepareSqlBatch(sqlStatement.Length, false, 0);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
    }

    public void BackupDatabaseToDisk(
      string databaseName,
      string backupFile,
      bool shrinkDatabase = false,
      bool disablePrefixCompression = false)
    {
      this.VerifyNotSqlAzure();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_BackupDatabase.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@databaseName", databaseName, 128, false, SqlDbType.NVarChar);
      this.BindString("@backupDir", backupFile, 260, false, SqlDbType.NVarChar);
      this.BindBoolean("@shrinkDatabase", shrinkDatabase);
      this.BindBoolean("@disablePrefixCompression", disablePrefixCompression);
      this.ExecuteNonQuery();
    }

    public void CreateXEventSession(
      string blobStoragePath,
      char pathSeparator,
      string sharedAccessSignature)
    {
      this.CreateXEventSession(XEventsConstants.MainXEventFilename, blobStoragePath, pathSeparator, sharedAccessSignature, XEventsConstants.MainXEventSessionName);
    }

    public void CreateReadScaleOutXEventSession(
      string blobStoragePath,
      char pathSeparator,
      string sharedAccessSignature)
    {
      this.CreateXEventSession(XEventsConstants.ReadScaleOutXEventFilename, blobStoragePath, pathSeparator, sharedAccessSignature, XEventsConstants.ReadScaleOutXEventSessionName);
    }

    public void CreateXEventSession(
      string xeventsFilename,
      string blobStoragePath,
      char pathSeparator,
      string sharedAccessSignature,
      string dbSessionName)
    {
      this.VerifyNotInMaster();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_CreateXEventSession.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@xeventsFilename", xeventsFilename, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@blobStoragePath", blobStoragePath, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@pathSeparator", pathSeparator.ToString(), 1, false, SqlDbType.NChar);
      this.BindString("@sharedAccessSignature", sharedAccessSignature, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@dbSessionName", dbSessionName, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public void AlterXEventSession(bool includeStatement) => this.AlterXEventSession(includeStatement, XEventsConstants.MainXEventSessionName);

    public void AlterReadScaleOutXEventSession(bool includeStatement) => this.AlterXEventSession(includeStatement, XEventsConstants.ReadScaleOutXEventSessionName);

    public void DropDatabaseEventSession(string sessionName)
    {
      this.VerifySqlAzure();
      this.VerifyNotInMaster();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_DropDatabaseEventSession.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@sessionName", sessionName, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public void DropServerEventSession(string sessionName)
    {
      this.VerifyNotSqlAzure();
      this.VerifyInMaster();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_DropServerEventSession.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@sessionName", sessionName, 250, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public void DropAllXEventSessions(string configurationName, string collectionPrefix)
    {
      this.VerifyNotSqlAzure();
      this.VerifyInMaster();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_DropAllXEventSessions.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@configurationName", configurationName, 250, false, SqlDbType.NVarChar);
      this.BindString("@collectionPrefix", collectionPrefix, 250, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public void StartXEventSession() => this.StartXEventSession(XEventsConstants.MainXEventSessionName, false);

    public void StartReadScaleOutXEventSession()
    {
      this.VerifyInReadScaleOut();
      this.StartXEventSession(XEventsConstants.ReadScaleOutXEventSessionName, true);
    }

    public bool IsXEventSessionDroppingBuffers(string dbSessionName, bool enforceReadScaleOut)
    {
      this.VerifyNotInMaster();
      this.VerifySqlAzure();
      if (enforceReadScaleOut)
        this.VerifyInReadScaleOut();
      ArgumentUtility.CheckStringForNullOrEmpty(dbSessionName, nameof (dbSessionName));
      string str1 = "stmt_GetXEventSessionTarget.sql";
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString(str1);
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@dbSessionName", dbSessionName, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection(this.ExecuteReaderTestable(), str1, (IVssRequestContext) null);
      resultCollection.AddBinder<XEventTargetProperties>((ObjectBinder<XEventTargetProperties>) new XEventSessionTargetColumns());
      XEventTargetProperties targetProperties = resultCollection.GetCurrent<XEventTargetProperties>().Items.FirstOrDefault<XEventTargetProperties>();
      if (targetProperties == null)
      {
        this.Trace(99910, TraceLevel.Error, string.Format("Can't get XEvent session target data. Session is (probably) stopped or does not exist. {0};{1}", (object) 0, (object) 1), (object) this.DataSource, (object) this.Database);
        return false;
      }
      if (enforceReadScaleOut && !targetProperties.IsRunningInReadScaleOut)
      {
        this.Trace(99913, TraceLevel.Error, "Executed read scale out operation in primary db. " + this.DataSource + ";" + this.Database);
        throw new InvalidOperationException("Executed read scale out operation in primary db. " + this.DataSource + ";" + this.Database);
      }
      string str2 = "dropped=\"";
      int num1 = targetProperties.TargetData.IndexOf(str2) + str2.Length;
      if (num1 >= str2.Length)
      {
        int num2 = Math.Max(targetProperties.TargetData.IndexOf("\"", num1), num1);
        int result;
        if (int.TryParse(targetProperties.TargetData.Substring(num1, num2 - num1), out result) && result > 0)
        {
          this.Trace(99912, TraceLevel.Info, string.Format("Session {0} is dropping events. Dropped buffers = {1}. {2};{3}", (object) 0, (object) 1, (object) 2, (object) 3), (object) dbSessionName, (object) result, (object) this.DataSource, (object) this.Database);
          return true;
        }
        this.Trace(99911, TraceLevel.Error, string.Format("Can't parse XEvent session target data. TargetData={0}. {1};{2}", (object) 0, (object) 1, (object) 2), (object) targetProperties.TargetData, (object) this.DataSource, (object) this.Database);
      }
      else
        this.Trace(99911, TraceLevel.Error, string.Format("Can't parse XEvent session target data. TargetData={0}. {1};{2}", (object) 0, (object) 1, (object) 2), (object) targetProperties.TargetData, (object) this.DataSource, (object) this.Database);
      return false;
    }

    public void StopXEventSession() => this.StopXEventSession(XEventsConstants.MainXEventSessionName, false);

    public void StopReadScaleOutXEventSession(bool dangerouslyIgnoreReadScaleOutVerification = false)
    {
      if (!dangerouslyIgnoreReadScaleOutVerification)
        this.VerifyInReadScaleOut();
      this.StopXEventSession(XEventsConstants.ReadScaleOutXEventSessionName, !dangerouslyIgnoreReadScaleOutVerification);
    }

    public void UpdateXEventSessionCredentials(string blobStoragePath, string sharedAccessSignature)
    {
      this.VerifyNotInMaster();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_UpdateXEventSessionCredentials.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@blobStoragePath", blobStoragePath, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@sharedAccessSignature", sharedAccessSignature, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public void RemoveSqlAzureReadReplica(string databaseName, string targetDestinationServer)
    {
      this.VerifySqlAzure();
      string sqlStatement = "ALTER DATABASE " + StringUtil.QuoteName(databaseName) + " REMOVE SECONDARY ON SERVER " + StringUtil.QuoteName(targetDestinationServer) + ";";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
    }

    public bool IsSqlAzureReplicaTerminationComplete(string databaseName)
    {
      this.VerifyInMasterDbOnAzure();
      string sqlStatement = "SELECT state_desc, last_modify_time, start_time, percent_complete, error_code, error_desc\r\nFROM  sys.dm_operation_status\r\nWHERE major_resource_id = @databaseName \r\nAND operation = 'TERMINATE CONTINUOUS DATABASE COPY'";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.BindSysname("@databaseName", databaseName, false);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DatabaseOperationStatus>((ObjectBinder<DatabaseOperationStatus>) new DatabaseOperationStatusColumns());
      List<DatabaseOperationStatus> items = resultCollection.GetCurrent<DatabaseOperationStatus>().Items;
      return items.Count == 0 || items.All<DatabaseOperationStatus>((System.Func<DatabaseOperationStatus, bool>) (s => !s.State.Equals("IN_PROGRESS", StringComparison.OrdinalIgnoreCase) && !s.State.Equals("PENDING", StringComparison.OrdinalIgnoreCase)));
    }

    public void FailoverGeoReplicatedSqlAzureDatabase(string databaseName, bool allowDataLoss)
    {
      this.VerifySqlAzure();
      string sqlStatement = string.Format("ALTER DATABASE {0} {1};", (object) StringUtil.QuoteName(databaseName), !allowDataLoss ? (object) "FAILOVER" : (object) "FORCE_FAILOVER_ALLOW_DATA_LOSS");
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
    }

    public byte[] GetDatatierLoginSid(string loginName)
    {
      string sqlStatement = "SELECT Sid FROM sys.sql_logins where name = @loginName";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.BindString("@loginName", loginName, 256, false, SqlDbType.NVarChar);
      return (byte[]) this.ExecuteScalar();
    }

    public Tuple<string, string> GetDataAndLogFolders()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetDataAndLogFolders.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Tuple<string, string>>((ObjectBinder<Tuple<string, string>>) new FolderNameColumns());
      return resultCollection.GetCurrent<Tuple<string, string>>().Items[0];
    }

    public string GetServerName()
    {
      string sqlStatement = "SELECT SERVERPROPERTY('ServerName')";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      return (string) this.ExecuteScalar();
    }

    public int GetAllocatedStorageInGB(string databaseName)
    {
      this.VerifyInMaster();
      this.VerifySqlAzure();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetAllocatedStorageInGB.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@databaseName", databaseName, false);
      return (int) this.ExecuteScalar();
    }

    protected override bool HandleCustomException(SqlException ex)
    {
      foreach (int sqlErrorNumber in this.GetSqlErrorNumbers(ex))
      {
        if (sqlErrorNumber == 5069 && this.RetriesRemaining > 0)
        {
          --this.RetriesRemaining;
          this.TraceException(64067, (Exception) ex);
          return true;
        }
      }
      return base.HandleCustomException(ex);
    }

    private void StartXEventSession(string dbSessionName, bool enforceReadScaleOut)
    {
      this.VerifyNotInMaster();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_StartXEventSession.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@dbSessionName", dbSessionName, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean("@enforceReadScaleOut", enforceReadScaleOut);
      bool flag = (bool) this.ExecuteScalar();
      if (enforceReadScaleOut && !flag)
      {
        this.Trace(99908, TraceLevel.Error, "Executed read scale out operation in primary db. " + this.DataSource + ";" + this.Database);
        throw new InvalidOperationException("Executed read scale out operation in primary db. " + this.DataSource + ";" + this.Database);
      }
    }

    private void StopXEventSession(string dbSessionName, bool enforceReadScaleOut)
    {
      this.VerifyNotInMaster();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_StopXEventSession.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@dbSessionName", dbSessionName, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean("@enforceReadScaleOut", enforceReadScaleOut);
      bool flag = (bool) this.ExecuteScalar();
      if (enforceReadScaleOut && !flag)
      {
        this.Trace(99908, TraceLevel.Error, "Executed read scale out operation in primary db");
        throw new InvalidOperationException("Executed read scale out operation in primary db");
      }
    }

    private void AlterXEventSession(bool includeStatement, string dbSessionName)
    {
      this.VerifyNotInMaster();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_AlterXEventSession.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindBoolean("@includeStatement", includeStatement);
      this.BindString("@dbSessionName", dbSessionName, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    private void CreateSqlAzureDatabase(string databaseName, VsoCreateDatabaseOptions options)
    {
      if (this.CheckIfDatabaseExists(databaseName))
        throw new DatabaseAlreadyExistsException(databaseName, this.ConnectionInfo.DataSource);
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_CreateDatabaseAzure.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      if (!string.IsNullOrEmpty(options.ServiceObjective) && AzureDatabaseProperties.GetServiceObjectiveFromString(options.ServiceObjective).GetEdition().IsVCoreBased())
        options.MaxSizeInGB = 128;
      string collation = !string.IsNullOrEmpty(options.Collation) ? options.Collation : (string) null;
      string serviceObjective = !string.IsNullOrEmpty(options.ServiceObjective) ? options.ServiceObjective : (string) null;
      this.BindSysname("@databaseName", databaseName, false);
      this.BindSysname("@serviceObjective", serviceObjective, false);
      this.BindSysname("@collation", collation, true);
      this.BindInt("@maxSizeInGB", options.MaxSizeInGB);
      this.Logger.Info("Creating Azure Database " + databaseName);
      try
      {
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRaw(99007, TraceLevel.Error, TeamFoundationDataTierComponent.s_area, TeamFoundationDataTierComponent.s_layer, string.Format("Exception encountered while creating a new database. Exception Details: {0}", (object) ex));
        throw;
      }
      if (!this.CheckIfDatabaseExists(databaseName))
        throw new DatabaseNotFoundException(databaseName);
    }

    private void CreateSqlServerDatabase(string databaseName, VsoCreateDatabaseOptions options)
    {
      this.Logger.Info("Creating database " + databaseName + ". Sql Instance: " + this.DataSource + ", Collation: " + options.Collation);
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_CreateDatabase.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@databaseName", databaseName, false);
      this.BindSysname("@collation", options.Collation, true);
      this.ExecuteNonQuery();
    }

    private void DeleteDatabaseFiles(List<DatabaseFileProperties> databaseFiles)
    {
      if (databaseFiles == null)
        return;
      for (int index = 0; index < databaseFiles.Count; ++index)
      {
        string physicalName = databaseFiles[index].PhysicalName;
        this.DeleteFile(physicalName);
        if (string.Equals(Path.GetExtension(physicalName), ".mdf", StringComparison.OrdinalIgnoreCase) && Guid.TryParse(Path.GetFileNameWithoutExtension(physicalName), out Guid _))
          this.DeleteFile(Path.ChangeExtension(physicalName, ".bak"));
      }
    }

    private void DeleteFile(string file)
    {
      if (!File.Exists(file))
        return;
      this.Logger.Info("Deleting file: {0}", (object) file);
      try
      {
        File.Delete(file);
      }
      catch (Exception ex)
      {
        this.Logger.Warning("Failed to delete '{0}' file. The following error was reported: {1}", (object) file, (object) ex.Message);
      }
    }

    private void VerifyInReadScaleOut()
    {
      if (this.ApplicationIntent != ApplicationIntent.ReadOnly)
      {
        this.Trace(99908, TraceLevel.Error, "Executed read scale out operation with non ReadOnly ApplicationIntent");
        throw new InvalidOperationException("Executed read scale out operation with non ReadOnly ApplicationIntent");
      }
      if (!this.IsSqlAzure)
      {
        this.Trace(99909, TraceLevel.Error, "Executed read scale out operation not in Azure Sql");
        throw new InvalidOperationException("Executed read scale out operation not in Azure Sql");
      }
    }
  }
}
