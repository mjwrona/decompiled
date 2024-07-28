// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.CopyAzureDatabaseComponent
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class CopyAzureDatabaseComponent : TeamFoundationSqlResourceComponent
  {
    protected override ITFLogger GetDefaultLogger() => (ITFLogger) new ServerTraceLogger();

    public void ReplaceStringInTableColumn(
      string tableName,
      string columnName,
      string oldValue,
      string newValue)
    {
      string sqlStatement = string.Format("\r\n                IF(EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}'))\r\n                BEGIN\r\n                    UPDATE [{0}] SET [{1}] = REPLACE([{1}], @oldValue, @newValue)\r\n                    WHERE  [{1}] LIKE '%' + @oldValue +'%'\r\n                END", (object) tableName, (object) columnName);
      this.Logger.Info("ReplaceStringInTableColumn '{0}' -> '{1}' table: {2} column: {3}, statement: {4}", (object) oldValue, (object) newValue, (object) tableName, (object) columnName, (object) sqlStatement);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.BindString("@oldValue", oldValue, oldValue.Length, false, SqlDbType.NVarChar);
      this.BindString("@newValue", newValue, newValue.Length, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public void ZeroOutStorageAccountIdInTableServiceHost()
    {
      string sqlStatement = "\r\n                UPDATE Tbl_ServiceHost\r\n                SET StorageAccountId = 0";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
    }

    internal void CleanUpServiceHosts()
    {
      this.Logger.Info("Executing prc_DetectInactiveProcesses");
      this.PrepareStoredProcedure("prc_DetectInactiveProcesses");
      this.ExecuteNonQuery();
    }

    internal void TruncateTable(string tableName)
    {
      this.Logger.Info("Truncating table {0}", (object) tableName);
      string sqlStatement = string.Format("TRUNCATE TABLE [{0}]", (object) tableName);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
    }

    internal static string StripPrefix(string prefix, string text)
    {
      if (string.IsNullOrEmpty(text))
        return string.Empty;
      return !text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ? text : text.Substring(prefix.Length);
    }

    internal static string StripAppPrefix(string hostUrl) => string.IsNullOrEmpty(hostUrl) || !Uri.IsWellFormedUriString(hostUrl, UriKind.RelativeOrAbsolute) ? string.Empty : CopyAzureDatabaseComponent.StripPrefix("app.", new Uri(hostUrl).Host);

    internal void UpdateAccountWatermark()
    {
      string sqlStatement = "UPDATE tbl_RegistryItems SET RegValue = convert(nvarchar, @@DBTS, 1)  WHERE ParentPath = '#\\Configuration\\Account\\' AND ChildItem = 'PublishWatermark\\' AND PartitionId = 1";
      this.Logger.Info("Updating account watermark  with script = {0}", (object) sqlStatement);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteScalar();
    }

    internal List<StringInDatabaseMatch> FindStringInDatabase(
      ITFLogger logger,
      string textToFind,
      string databaseName,
      List<string> tablesToExclude)
    {
      logger.Info("Searching for string {0} on database {1}", (object) textToFind, (object) databaseName);
      List<StringInDatabaseMatch> stringInDatabase = new List<StringInDatabaseMatch>();
      try
      {
        string sqlStatement = string.Format("\r\n                DECLARE @table_name SYSNAME, @table_id INT, @column_name SYSNAME, @sql_string VARCHAR(2000)                \r\n                CREATE TABLE #tmp_invalidReferences(table_name nvarchar(100), column_name nvarchar(100))\r\n\r\n                DECLARE tables_cur CURSOR FOR SELECT name, object_id FROM sys.objects WHERE type = 'U' AND name NOT IN ('{0}')\r\n                OPEN tables_cur\r\n                FETCH NEXT FROM tables_cur INTO @table_name, @table_id\r\n                WHILE (@@FETCH_STATUS = 0)\r\n                BEGIN\r\n                    DECLARE columns_cur CURSOR FOR SELECT name FROM sys.columns WHERE object_id = @table_id AND system_type_id IN (167, 175, 231, 239)\r\n                    OPEN columns_cur\r\n                    FETCH NEXT FROM columns_cur INTO @column_name\r\n                    WHILE (@@FETCH_STATUS = 0)\r\n                    BEGIN\r\n                        SET @sql_string = 'IF EXISTS (SELECT * FROM ' + @table_name + ' WHERE [' + @column_name + '] LIKE ''%' + @search_string + '%'') INSERT INTO #tmp_invalidReferences ([table_name],[column_name]) VALUES ('''+ @table_name+''','''+ @column_name+''')'\r\n                        EXECUTE(@sql_string)\r\n                        FETCH NEXT FROM columns_cur INTO @column_name\r\n                    END\r\n                    CLOSE columns_cur\r\n                    DEALLOCATE columns_cur\r\n                    FETCH NEXT FROM tables_cur INTO @table_name, @table_id\r\n                END\r\n                CLOSE tables_cur\r\n                DEALLOCATE tables_cur\r\n\r\n                SELECT * FROM #tmp_invalidReferences\r\n                DROP TABLE #tmp_invalidReferences", (object) string.Join("','", (IEnumerable<string>) tablesToExclude));
        this.PrepareSqlBatch(sqlStatement.Length);
        this.AddStatement(sqlStatement);
        this.BindString("@search_string", textToFind, textToFind.Length, false, SqlDbType.NVarChar);
        using (SqlDataReader sqlDataReader = this.ExecuteReader())
        {
          while (sqlDataReader.Read())
          {
            StringInDatabaseMatch stringInDatabaseMatch = new StringInDatabaseMatch()
            {
              DatabaseName = databaseName,
              Text = textToFind,
              Table = sqlDataReader.GetString(0),
              Column = sqlDataReader.GetString(1)
            };
            if (tablesToExclude.Contains(stringInDatabaseMatch.Table))
            {
              logger.Info("Match found and excluded: {0} ", (object) stringInDatabaseMatch.ToString());
            }
            else
            {
              logger.Info("Match found {0}", (object) stringInDatabaseMatch.ToString());
              stringInDatabase.Add(stringInDatabaseMatch);
            }
          }
        }
      }
      catch (Exception ex)
      {
        logger.Info("Exception searching for text {0} on database {1}, Exception: {0}", (object) textToFind, (object) databaseName, (object) ex.Message);
      }
      return stringInDatabase;
    }
  }
}
