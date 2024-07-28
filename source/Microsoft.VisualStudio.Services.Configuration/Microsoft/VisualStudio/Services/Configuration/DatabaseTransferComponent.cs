// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.DatabaseTransferComponent
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class DatabaseTransferComponent : TeamFoundationSqlResourceComponent
  {
    private const string c_getDatabaseInfoStatement = "SELECT DB_NAME() AS DatabaseName, SERVERPROPERTY('MachineName') AS MachineName, @@SERVICENAME AS ServiceName";
    private static SqlColumnBinder indexColumnNameBinder = new SqlColumnBinder("column_name");
    private static SqlColumnBinder isDescendingKeyBinder = new SqlColumnBinder("is_descending_key");
    private static SqlColumnBinder isIncludedColumnBinder = new SqlColumnBinder("is_included_column");
    private const string c_DropTableFormatStatement = "DROP TABLE [{0}].[{1}]";
    private const string c_SetIdentityInsertFormatStatement = "SET IDENTITY_INSERT [{0}].[{1}] {2}";
    private const string c_TruncateTableFormatStatement = "TRUNCATE TABLE [{0}].[{1}]";
    private const string c_DeleteAllRowsFromTableFormatStatement = "DELETE [{0}].[{1}]";
    private const string c_SetRecoveryModelFormatStatement = "ALTER DATABASE [{0}] SET RECOVERY {1}";
    private const string c_MoveDataStatementTemplate = "DELETE TOP({2}) [{3}].[{0}].[{1}] WITH(TABLOCKX) OUTPUT {4} INTO [{0}].[{1}]({5})\nSELECT @@ERROR, @@ROWCOUNT";
    private const string c_CopyDataStatementTemplate = "INSERT INTO [{0}].[{1}]WITH(TABLOCKX) ({3})\nSELECT {3} FROM [{2}].[{0}].[{1}] WITH(TABLOCK)";
    private const string c_hasDataFormatStatement = "SELECT CONVERT(BIT, COUNT(*)) hasData FROM (SELECT TOP 1 * FROM [{0}].[{1}]) tmp";
    private const string c_dropTriggerFormatStatement = "DROP TRIGGER {0}.{1}";
    private const string c_area = "HostMove";
    private const string c_layer = "DatabaseTransferComponent";

    protected override void Initialize(
      ISqlConnectionInfo connectionInfo,
      int commandTimeout,
      int deadlockPause,
      int maxDeadlockRetries,
      int serviceVersion,
      ITFLogger logger,
      CircuitBreakerDatabaseProperties circuitBreakerProperties)
    {
      base.Initialize(connectionInfo, commandTimeout, deadlockPause, maxDeadlockRetries, serviceVersion, logger, circuitBreakerProperties);
      this.InfoMessage += new SqlInfoMessageEventHandler(this.OnInfoMessage);
    }

    public override void Dispose()
    {
      this.InfoMessage -= new SqlInfoMessageEventHandler(this.OnInfoMessage);
      base.Dispose();
    }

    public bool PrimaryKeyDropSupported => !this.IsSqlAzure;

    public DatabaseInfo GetDatabaseInfo()
    {
      this.PrepareSqlBatch("SELECT DB_NAME() AS DatabaseName, SERVERPROPERTY('MachineName') AS MachineName, @@SERVICENAME AS ServiceName".Length);
      this.AddStatement("SELECT DB_NAME() AS DatabaseName, SERVERPROPERTY('MachineName') AS MachineName, @@SERVICENAME AS ServiceName");
      using (SqlDataReader reader = this.ExecuteReader())
      {
        reader.Read();
        return DatabaseTransferComponent.DatabaseInfoBinder.Bind(reader);
      }
    }

    public virtual Table GetTable(string tableName, string tableSchema = "dbo")
    {
      ArgumentUtility.CheckForNull<string>(tableName, nameof (tableName));
      return this.GetTables(tableName, tableSchema).FirstOrDefault<Table>();
    }

    public virtual List<Table> GetTables() => this.GetTables((string) null, (string) null);

    private List<Table> GetTables(string nameFilter, string schemaFilter)
    {
      this.Logger.Info("Getting tables...");
      if (!string.IsNullOrEmpty(nameFilter))
        this.Logger.Info("Table filter: " + schemaFilter + "." + nameFilter);
      List<Index> indexes = this.GetIndexes(nameFilter, schemaFilter);
      List<Table> tables = new List<Table>();
      string sqlStatement = string.IsNullOrEmpty(nameFilter) ? this.ExtractStatementFromResource("stmt_QueryAllColumns.sql") : this.ExtractStatementFromResource("stmt_QueryAllColumnsByTable.sql");
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      if (!string.IsNullOrEmpty(nameFilter))
      {
        this.BindString("@name", nameFilter, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@schemaName", schemaFilter, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      }
      Table table = (Table) null;
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
        {
          Column column = DatabaseTransferComponent.ColumnBinder.Bind(reader);
          string tableName = DatabaseTransferComponent.ColumnBinder.GetTableName(reader);
          string tableSchema = DatabaseTransferComponent.ColumnBinder.GetSchema(reader);
          if (table != null && (!table.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase) || !table.Schema.Equals(tableSchema, StringComparison.OrdinalIgnoreCase)))
            table = (Table) null;
          if (table == null)
          {
            table = new Table()
            {
              Name = tableName,
              Schema = tableSchema,
              TableTextInRowLimit = DatabaseTransferComponent.ColumnBinder.GetTableTextInRowLimit(reader),
              IsSystem = DatabaseTransferComponent.ColumnBinder.IsSystemTable(reader)
            };
            table.Indexes.AddRange((IEnumerable<Index>) indexes.FindAll((Predicate<Index>) (index => index.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase) && index.TableSchema.Equals(tableSchema, StringComparison.OrdinalIgnoreCase))));
            tables.Add(table);
          }
          table.Columns.Add(column);
        }
      }
      this.Logger.Info("Found {0} tables. Table filter: {1}.{2}", (object) tables.Count, (object) schemaFilter, (object) nameFilter);
      return tables;
    }

    public List<Index> GetIndexes(string tableNameFilter, string schemaFilter = "dbo")
    {
      string parameterValue = (string) null;
      if (!string.IsNullOrEmpty(tableNameFilter))
        parameterValue = schemaFilter + "." + tableNameFilter;
      this.Logger.Info("Getting indexes. Table filter: {0}", (object) parameterValue);
      List<Index> indexes = new List<Index>();
      string sqlStatement = string.IsNullOrEmpty(parameterValue) ? this.ExtractStatementFromResource("stmt_QueryIndexes.sql") : this.ExtractStatementFromResource("stmt_QueryIndexesByName.sql");
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      if (!string.IsNullOrEmpty(parameterValue))
        this.BindString("@name", parameterValue, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      Index index = (Index) null;
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
        {
          IndexColumn indexColumn = new IndexColumn()
          {
            Name = DatabaseTransferComponent.indexColumnNameBinder.GetString((IDataReader) reader, false),
            IsDescending = DatabaseTransferComponent.isDescendingKeyBinder.GetBoolean((IDataReader) reader),
            IsIncludedColumn = DatabaseTransferComponent.isIncludedColumnBinder.GetBoolean((IDataReader) reader)
          };
          string name = DatabaseTransferComponent.IndexBinder.GetName(reader);
          if (index != null && !index.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            index = (Index) null;
          if (index == null)
          {
            index = DatabaseTransferComponent.IndexBinder.Bind(reader);
            indexes.Add(index);
          }
          index.Columns.Add(indexColumn);
        }
      }
      this.Logger.Info("Found {0} indexes. Table filter: {1}", (object) indexes.Count, (object) tableNameFilter);
      return indexes;
    }

    public void DeleteAllRows(Table table)
    {
      ArgumentUtility.CheckForNull<Table>(table, nameof (table));
      this.ExecuteNonQuery(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DELETE [{0}].[{1}]", (object) table.Schema, (object) table.Name));
    }

    public void DeleteRows(Table table, string filter)
    {
      ArgumentUtility.CheckForNull<Table>(table, nameof (table));
      if (string.IsNullOrEmpty(filter))
        this.DeleteAllRows(table);
      else
        this.ExecuteNonQuery(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DELETE [{0}].[{1}] WHERE {2}", (object) table.Schema, (object) table.Name, (object) filter));
    }

    public void DropTrigger(string schema, string triggerName)
    {
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DROP TRIGGER {0}.{1}", (object) StringUtil.QuoteName(schema), (object) StringUtil.QuoteName(triggerName));
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
    }

    public void SwapTable(string table, string suffix)
    {
      this.Logger.Info("Swapping tables: {0}, {1}", (object) (table + suffix), (object) table);
      string statementFromResource = this.ExtractStatementFromResource("stmt_SwapTable.sql");
      this.PrepareSqlBatch(statementFromResource.Length);
      this.AddStatement(statementFromResource);
      this.BindString("@targetTable", table + suffix, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@sourceTable", table, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@suffix", suffix, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      this.Logger.Info("Table swap completed");
    }

    public void TruncateTable(Table table)
    {
      ArgumentUtility.CheckForNull<Table>(table, nameof (table));
      this.ExecuteNonQuery(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TRUNCATE TABLE [{0}].[{1}]", (object) table.Schema, (object) table.Name));
    }

    public bool TableIsEmpty(Table table)
    {
      ArgumentUtility.CheckForNull<Table>(table, nameof (table));
      this.Logger.Info("Checking if the following table is empty: {0}", (object) table.Name);
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT CONVERT(BIT, COUNT(*)) hasData FROM (SELECT TOP 1 * FROM [{0}].[{1}]) tmp", (object) table.Schema, (object) table.Name);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      bool flag = (bool) this.ExecuteScalar();
      this.Logger.Info("Table {0} is empty: {1}", (object) table.Name, (object) !flag);
      return !flag;
    }

    public bool IsTableReferencedByForeignKey(Table table)
    {
      ArgumentUtility.CheckForNull<Table>(table, nameof (table));
      return this.IsTableReferencedByForeignKey(table.Schema + "." + table.Name);
    }

    public bool IsTableReferencedByForeignKey(string tableName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(tableName, nameof (tableName));
      string statementFromResource = this.ExtractStatementFromResource("stmt_IsTableReferencedByForeignKey.sql");
      this.PrepareSqlBatch(statementFromResource.Length);
      this.AddStatement(statementFromResource);
      this.BindSysname("@tableName", tableName, false);
      return (bool) this.ExecuteScalar();
    }

    public bool TriggerExists(string schema, string triggerName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(schema, nameof (schema));
      ArgumentUtility.CheckStringForNullOrEmpty(triggerName, nameof (triggerName));
      this.Logger.Info("Checking for trigger: {0}.{1}", (object) schema, (object) triggerName);
      string statementFromResource = this.ExtractStatementFromResource("stmt_TriggerExists.sql");
      this.PrepareSqlBatch(statementFromResource.Length);
      this.AddStatement(statementFromResource);
      this.BindString("@schema", schema, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@triggerName", triggerName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      bool flag = (bool) this.ExecuteScalar();
      this.Logger.Info("Trigger exists: {0}", (object) flag);
      return flag;
    }

    public List<ForeignKey> DisableAllForeignKeys()
    {
      string statementFromResource = this.ExtractStatementFromResource("stmt_DisableAllForeignKeys.sql");
      this.PrepareSqlBatch(statementFromResource.Length);
      this.AddStatement(statementFromResource);
      SqlDataReader reader = this.ExecuteReader();
      List<ForeignKey> foreignKeyList = new List<ForeignKey>();
      while (reader.Read())
        foreignKeyList.Add(DatabaseTransferComponent.ForeignKeyBinder.Bind(reader));
      return foreignKeyList;
    }

    public void EnableForeignKey(ForeignKey foreignKey, bool enable)
    {
      string statementFromResource = this.ExtractStatementFromResource("stmt_EnableForeignKey.sql");
      this.PrepareSqlBatch(statementFromResource.Length);
      this.AddStatement(statementFromResource);
      this.BindSysname("@foreignKeyName", foreignKey.Name, false);
      this.BindSysname("@parentTableName", foreignKey.TableName, false);
      this.BindSysname("@schemaName", foreignKey.Schema, false);
      this.BindBoolean("@enable", enable);
      this.ExecuteNonQuery();
    }

    public void DropDatabase(ServicingContext servicingContext)
    {
      string database = this.Connection.Database;
      try
      {
        string end = new StreamReader(servicingContext.ResourceProvider.GetServicingResource("upd_DropDatabase.sql")).ReadToEnd();
        this.PrepareSqlBatch(end.Length);
        this.AddStatement(end);
        this.ExecuteNonQuery();
        this.Connection.Close();
      }
      catch (SqlException ex)
      {
        this.Connection.ChangeDatabase(database);
        throw;
      }
    }

    public void SetRecoveryModel(RecoveryModel recoveryModel)
    {
      string str = recoveryModel != RecoveryModel.BulkLogged ? recoveryModel.ToString().ToUpper(CultureInfo.InvariantCulture) : "BULK_LOGGED";
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ALTER DATABASE [{0}] SET RECOVERY {1}", (object) this.Connection.Database, (object) str);
      this.Logger.Info("Setting database recovery model to: {0}, Database: {1}, Sql Instance: {2}", (object) str, (object) this.InitialCatalog, (object) this.DataSource);
      this.ExecuteNonQuery(sqlStatement);
    }

    public RecoveryModel GetRecoveryModel()
    {
      this.Logger.Info("Getting a database recovery model. Database: {0}, Sql Instance: {1}", (object) this.Database, (object) this.DataSource);
      string statementFromResource = this.ExtractStatementFromResource("stmt_QueryDatabaseRecoveryModel.sql");
      this.PrepareSqlBatch(statementFromResource.Length);
      this.AddStatement(statementFromResource);
      RecoveryModel recoveryModel = (RecoveryModel) this.ExecuteScalar();
      this.Logger.Info("Recovery model: {0}, Database: {1}, Sql Instance: {2}", (object) recoveryModel, (object) this.Database, (object) this.DataSource);
      return recoveryModel;
    }

    public void CopyData(string sourceDatabase, Table table)
    {
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "INSERT INTO [{0}].[{1}]WITH(TABLOCKX) ({3})\nSELECT {3} FROM [{2}].[{0}].[{1}] WITH(TABLOCK)", (object) table.Schema, (object) table.Name, (object) sourceDatabase, (object) table.GetColumnList("", true, true));
      this.CopyData(table, sqlStatement);
    }

    public SqlDataReader ReadTable(
      Table table,
      string filter,
      Dictionary<string, string> fieldExpressions)
    {
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT {0}\nFROM   {1}.{2}", (object) table.GetColumnList("", true, false, fieldExpressions), (object) StringUtil.QuoteName(table.Schema), (object) StringUtil.QuoteName(table.Name));
      if (!string.IsNullOrEmpty(filter))
        sqlStatement = sqlStatement + "\nWHERE " + filter;
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      return this.ExecuteReader();
    }

    protected void CopyData(Table table, string sqlStatement)
    {
      this.Logger.Info("Copying data, target table: {0}", (object) table.Name);
      if (table.IdentityColumn != null)
      {
        string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SET IDENTITY_INSERT [{0}].[{1}] {2}", (object) table.Schema, (object) table.Name, (object) "ON");
        string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SET IDENTITY_INSERT [{0}].[{1}] {2}", (object) table.Schema, (object) table.Name, (object) "OFF");
        sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\r\n{1}\r\n{2}", (object) str1, (object) sqlStatement, (object) str2);
      }
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      try
      {
        this.ExecuteNonQuery();
        this.Logger.Info("Table {0} has been successfully copied.", (object) table.Name);
      }
      catch (SqlException ex)
      {
        this.Logger.Error((Exception) ex);
        throw;
      }
      catch (Exception ex)
      {
        this.Logger.Error(ex);
        throw;
      }
    }

    public void MoveData(string sourceDatabase, Table table)
    {
      int num = 50000;
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DELETE TOP({2}) [{3}].[{0}].[{1}] WITH(TABLOCKX) OUTPUT {4} INTO [{0}].[{1}]({5})\nSELECT @@ERROR, @@ROWCOUNT", (object) table.Schema, (object) table.Name, (object) num, (object) sourceDatabase, (object) table.GetColumnList("DELETED.", true, true), (object) table.GetColumnList("", true, true));
      if (table.IdentityColumn != null)
      {
        string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SET IDENTITY_INSERT [{0}].[{1}] {2}", (object) table.Schema, (object) table.Name, (object) "ON");
        string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SET IDENTITY_INSERT [{0}].[{1}] {2}", (object) table.Schema, (object) table.Name, (object) "OFF");
        sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\r\n{1}\r\n{2}", (object) str1, (object) sqlStatement, (object) str2);
      }
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      try
      {
        int int32;
        do
        {
          SqlDataReader sqlDataReader = this.ExecuteReader();
          sqlDataReader.Read();
          sqlDataReader.GetInt32(0);
          int32 = sqlDataReader.GetInt32(1);
        }
        while (int32 != 0 && int32 >= num);
      }
      catch (SqlException ex)
      {
        throw;
      }
    }

    public int ExecuteNonQuery(string sqlStatement)
    {
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      return this.ExecuteNonQuery();
    }

    protected string ExtractStatementFromResource(string fileName) => EmbeddedResourceUtil.GetResourceAsString(fileName);

    private void OnInfoMessage(object sender, SqlInfoMessageEventArgs e)
    {
      try
      {
        this.LogInfoMessage(e, this.Logger);
      }
      catch (Exception ex)
      {
        this.Logger.Info("An error occurred while logging the following info message: {0}.", (object) e.Message);
        this.Logger.Error(ex);
      }
    }

    private class DatabaseInfoBinder : ObjectBinder<DatabaseInfo>
    {
      private static SqlColumnBinder databaseNameBinder = new SqlColumnBinder("DatabaseName");
      private static SqlColumnBinder machineNameBinder = new SqlColumnBinder("MachineName");
      private static SqlColumnBinder serviceNameBinder = new SqlColumnBinder("ServiceName");

      protected override DatabaseInfo Bind() => DatabaseTransferComponent.DatabaseInfoBinder.Bind(this.Reader);

      internal static DatabaseInfo Bind(SqlDataReader reader)
      {
        ArgumentUtility.CheckForNull<SqlDataReader>(reader, nameof (reader));
        return new DatabaseInfo()
        {
          DatabaseName = DatabaseTransferComponent.DatabaseInfoBinder.databaseNameBinder.GetString((IDataReader) reader, false),
          MachineName = DatabaseTransferComponent.DatabaseInfoBinder.machineNameBinder.GetString((IDataReader) reader, false),
          ServiceName = DatabaseTransferComponent.DatabaseInfoBinder.serviceNameBinder.GetString((IDataReader) reader, false)
        };
      }
    }

    private class ColumnBinder : ObjectBinder<Column>
    {
      private static SqlColumnBinder tableNameBinder = new SqlColumnBinder("table_name");
      private static SqlColumnBinder tableSchemaBinder = new SqlColumnBinder("table_schema");
      private static SqlColumnBinder isSystemTableBinder = new SqlColumnBinder("is_system_table");
      private static SqlColumnBinder tableTextInRowLimitBinder = new SqlColumnBinder("tableTextInRowLimit");
      private static SqlColumnBinder columnNameBinder = new SqlColumnBinder("column_name");
      private static SqlColumnBinder isNullableBinder = new SqlColumnBinder("is_nullable");
      private static SqlColumnBinder columnDefaultBinder = new SqlColumnBinder("column_default");
      private static SqlColumnBinder isReplicatedBinder = new SqlColumnBinder("is_replicated");
      private static SqlColumnBinder isNotForReplicationBinder = new SqlColumnBinder("is_not_for_replication");
      private static SqlColumnBinder isAnsiPaddedBinder = new SqlColumnBinder("is_ansi_padded");
      private static SqlColumnBinder isRowGuidBinder = new SqlColumnBinder("is_rowguidcol");
      private static SqlColumnBinder dataTypeBinder = new SqlColumnBinder("data_type");
      private static SqlColumnBinder maximumLengthBinder = new SqlColumnBinder("max_length");
      private static SqlColumnBinder precisionBinder = new SqlColumnBinder("precision");
      private static SqlColumnBinder scaleBinder = new SqlColumnBinder("scale");
      private static SqlColumnBinder collationNameBinder = new SqlColumnBinder("collation_name");
      private static SqlColumnBinder isIdentityBinder = new SqlColumnBinder("is_identity");
      private static SqlColumnBinder seedValueBinder = new SqlColumnBinder("seed_value");
      private static SqlColumnBinder incrementValueBinder = new SqlColumnBinder("increment_value");
      private static SqlColumnBinder isComputedBinder = new SqlColumnBinder("is_computed");
      private static SqlColumnBinder isSparseBinder = new SqlColumnBinder("is_sparse");
      private static SqlColumnBinder computedDefinitionBinder = new SqlColumnBinder("computed_definition");
      private static SqlColumnBinder isPersistedValueBinder = new SqlColumnBinder("is_persisted");
      private static SqlColumnBinder isUserDefinedBinder = new SqlColumnBinder("is_user_defined");

      protected override Column Bind() => DatabaseTransferComponent.ColumnBinder.Bind(this.Reader);

      public static Column Bind(SqlDataReader reader) => new Column()
      {
        Name = DatabaseTransferComponent.ColumnBinder.columnNameBinder.GetString((IDataReader) reader, false),
        IsNullable = DatabaseTransferComponent.ColumnBinder.isNullableBinder.GetBoolean((IDataReader) reader),
        IsReplicated = DatabaseTransferComponent.ColumnBinder.isReplicatedBinder.GetBoolean((IDataReader) reader),
        IsNotForReplication = DatabaseTransferComponent.ColumnBinder.isNotForReplicationBinder.GetBoolean((IDataReader) reader),
        IsAnsiPadded = DatabaseTransferComponent.ColumnBinder.isAnsiPaddedBinder.GetBoolean((IDataReader) reader),
        IsRowGuid = DatabaseTransferComponent.ColumnBinder.isRowGuidBinder.GetBoolean((IDataReader) reader),
        Default = DatabaseTransferComponent.ColumnBinder.columnDefaultBinder.GetString((IDataReader) reader, true),
        DataType = DatabaseTransferComponent.ColumnBinder.dataTypeBinder.GetString((IDataReader) reader, false),
        MaximumLength = (int) DatabaseTransferComponent.ColumnBinder.maximumLengthBinder.GetInt16((IDataReader) reader),
        CollationName = DatabaseTransferComponent.ColumnBinder.collationNameBinder.GetString((IDataReader) reader, true),
        Precision = DatabaseTransferComponent.ColumnBinder.precisionBinder.GetByte((IDataReader) reader, (byte) 0),
        Scale = DatabaseTransferComponent.ColumnBinder.scaleBinder.GetByte((IDataReader) reader, (byte) 0),
        IsIdentity = DatabaseTransferComponent.ColumnBinder.isIdentityBinder.GetBoolean((IDataReader) reader, false),
        SeedValue = DatabaseTransferComponent.ColumnBinder.seedValueBinder.GetInt64((IDataReader) reader, 0L),
        IncrementValue = DatabaseTransferComponent.ColumnBinder.incrementValueBinder.GetInt64((IDataReader) reader, 0L),
        IsComputed = DatabaseTransferComponent.ColumnBinder.isComputedBinder.GetBoolean((IDataReader) reader, false),
        IsSparse = DatabaseTransferComponent.ColumnBinder.isSparseBinder.GetBoolean((IDataReader) reader, false),
        ComputedDefinition = DatabaseTransferComponent.ColumnBinder.computedDefinitionBinder.GetString((IDataReader) reader, true),
        IsPersisted = DatabaseTransferComponent.ColumnBinder.isPersistedValueBinder.GetBoolean((IDataReader) reader, false),
        IsUserDefined = DatabaseTransferComponent.ColumnBinder.isUserDefinedBinder.GetBoolean((IDataReader) reader, false)
      };

      public static string GetTableName(SqlDataReader reader) => DatabaseTransferComponent.ColumnBinder.tableNameBinder.GetString((IDataReader) reader, false);

      public static string GetSchema(SqlDataReader reader) => DatabaseTransferComponent.ColumnBinder.tableSchemaBinder.GetString((IDataReader) reader, false);

      public static bool IsSystemTable(SqlDataReader reader) => DatabaseTransferComponent.ColumnBinder.isSystemTableBinder.GetBoolean((IDataReader) reader);

      public static int GetTableTextInRowLimit(SqlDataReader reader) => DatabaseTransferComponent.ColumnBinder.tableTextInRowLimitBinder.GetInt32((IDataReader) reader);
    }

    private class IndexBinder : ObjectBinder<Index>
    {
      private static SqlColumnBinder indexTableNameBinder = new SqlColumnBinder("table_name");
      private static SqlColumnBinder indexTableSchemaNameBinder = new SqlColumnBinder("schema_name");
      private static SqlColumnBinder indexNameBinder = new SqlColumnBinder("index_name");
      private static SqlColumnBinder isSystemNamedBinder = new SqlColumnBinder("is_system_named");
      private static SqlColumnBinder indexTypeBinder = new SqlColumnBinder("index_type");
      private static SqlColumnBinder isUniqueBinder = new SqlColumnBinder("is_unique");
      private static SqlColumnBinder isUniqueConstraintBinder = new SqlColumnBinder("is_unique_constraint");
      private static SqlColumnBinder ignoreDupKeyBinder = new SqlColumnBinder("ignore_dup_key");
      private static SqlColumnBinder isPrimaryKeyBinder = new SqlColumnBinder("is_primary_key");
      private static SqlColumnBinder fillFactorBinder = new SqlColumnBinder("fill_factor");
      private static SqlColumnBinder isPaddedBinder = new SqlColumnBinder("is_padded");
      private static SqlColumnBinder isDisabledBinder = new SqlColumnBinder("is_disabled");
      private static SqlColumnBinder isHypotheticalBinder = new SqlColumnBinder("is_hypothetical");
      private static SqlColumnBinder allowRowLocksBinder = new SqlColumnBinder("allow_row_locks");
      private static SqlColumnBinder allowPageLocksBinder = new SqlColumnBinder("allow_page_locks");
      private static SqlColumnBinder hasFilterBinder = new SqlColumnBinder("has_filter");
      private static SqlColumnBinder filterDefinitionBinder = new SqlColumnBinder("filter_definition");
      private static SqlColumnBinder noRecomputeBinder = new SqlColumnBinder("no_recompute");
      private static SqlColumnBinder partitionCount = new SqlColumnBinder("PartitionCount");
      private static SqlColumnBinder dataCompression = new SqlColumnBinder("DataCompression");

      protected override Index Bind() => DatabaseTransferComponent.IndexBinder.Bind(this.Reader);

      public static Index Bind(SqlDataReader reader) => new Index()
      {
        IsPrimaryKey = DatabaseTransferComponent.IndexBinder.isPrimaryKeyBinder.GetBoolean((IDataReader) reader, false),
        Name = DatabaseTransferComponent.IndexBinder.indexNameBinder.GetString((IDataReader) reader, false),
        TableName = DatabaseTransferComponent.IndexBinder.indexTableNameBinder.GetString((IDataReader) reader, false),
        TableSchema = DatabaseTransferComponent.IndexBinder.indexTableSchemaNameBinder.GetString((IDataReader) reader, false),
        IsSystemNamed = DatabaseTransferComponent.IndexBinder.isSystemNamedBinder.GetBoolean((IDataReader) reader, false),
        IsConstraint = DatabaseTransferComponent.IndexBinder.isSystemNamedBinder.GetObject((IDataReader) reader) != null,
        Type = (IndexType) DatabaseTransferComponent.IndexBinder.indexTypeBinder.GetByte((IDataReader) reader),
        IsUnique = DatabaseTransferComponent.IndexBinder.isUniqueBinder.GetBoolean((IDataReader) reader),
        IsUniqueConstraint = DatabaseTransferComponent.IndexBinder.isUniqueConstraintBinder.GetBoolean((IDataReader) reader),
        IgnoreDupKey = DatabaseTransferComponent.IndexBinder.ignoreDupKeyBinder.GetBoolean((IDataReader) reader),
        FillFactor = DatabaseTransferComponent.IndexBinder.fillFactorBinder.GetByte((IDataReader) reader),
        IsPadded = DatabaseTransferComponent.IndexBinder.isPaddedBinder.GetBoolean((IDataReader) reader),
        IsDisabled = DatabaseTransferComponent.IndexBinder.isDisabledBinder.GetBoolean((IDataReader) reader),
        IsHypothetical = DatabaseTransferComponent.IndexBinder.isHypotheticalBinder.GetBoolean((IDataReader) reader),
        AllowRowLocks = DatabaseTransferComponent.IndexBinder.allowRowLocksBinder.GetBoolean((IDataReader) reader),
        AllowPageLocks = DatabaseTransferComponent.IndexBinder.allowPageLocksBinder.GetBoolean((IDataReader) reader),
        HasFilter = DatabaseTransferComponent.IndexBinder.hasFilterBinder.GetBoolean((IDataReader) reader),
        FilterDefinition = DatabaseTransferComponent.IndexBinder.filterDefinitionBinder.GetString((IDataReader) reader, true),
        NoRecompute = DatabaseTransferComponent.IndexBinder.noRecomputeBinder.GetBoolean((IDataReader) reader),
        PartitionCount = DatabaseTransferComponent.IndexBinder.partitionCount.GetInt32((IDataReader) reader),
        DataCompression = (DataCompression) DatabaseTransferComponent.IndexBinder.dataCompression.GetByte((IDataReader) reader)
      };

      public static string GetName(SqlDataReader reader) => DatabaseTransferComponent.IndexBinder.indexNameBinder.GetString((IDataReader) reader, false);
    }

    private class ForeignKeyBinder : ObjectBinder<ForeignKey>
    {
      private static SqlColumnBinder foreignKeyBinder = new SqlColumnBinder("foreignKeyName");
      private static SqlColumnBinder foreignKeyTableNameBinder = new SqlColumnBinder("parentTableName");
      private static SqlColumnBinder foreignKeySchemaNameBinder = new SqlColumnBinder("schemaName");

      protected override ForeignKey Bind() => DatabaseTransferComponent.ForeignKeyBinder.Bind(this.Reader);

      public static ForeignKey Bind(SqlDataReader reader) => new ForeignKey()
      {
        Name = DatabaseTransferComponent.ForeignKeyBinder.foreignKeyBinder.GetString((IDataReader) reader, false),
        TableName = DatabaseTransferComponent.ForeignKeyBinder.foreignKeyTableNameBinder.GetString((IDataReader) reader, false),
        Schema = DatabaseTransferComponent.ForeignKeyBinder.foreignKeySchemaNameBinder.GetString((IDataReader) reader, false)
      };
    }
  }
}
