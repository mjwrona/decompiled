// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.LeadingKeyTransferComponent
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class LeadingKeyTransferComponent : DatabaseTransferComponent
  {
    private const string c_getPartitionId = "SELECT PartitionId FROM tbl_DatabasePartitionMap WHERE ServiceHostId = @id";
    private const string c_CopyDataStatementTemplate = "INSERT INTO [{0}].[{1}]WITH(TABLOCKX) ({3})\nSELECT {4} FROM [{0}].[{2}] WITH(TABLOCK)";
    private const string c_dropNonClusteredIndexesStmt = "DECLARE @stmt NVARCHAR(MAX) = ''\r\nSELECT  @stmt = @stmt + '\r\nDROP INDEX ' + QUOTENAME(name) + ' ON ' + QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName)\r\nFROM    sys.indexes\r\nWHERE   type = 2\r\n        AND is_primary_key = 0\r\n        AND is_unique_constraint = 0\r\n        AND object_id = OBJECT_ID(QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName))\r\n\r\nEXEC sp_executesql @stmt";

    public void PopulateLeadingKey(Table table, string sourceTable, int partitionId)
    {
      string columnList = table.GetColumnList(string.Empty, true, true);
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "INSERT INTO [{0}].[{1}]WITH(TABLOCKX) ({3})\nSELECT {4} FROM [{0}].[{2}] WITH(TABLOCK)", (object) table.Schema, (object) table.Name, (object) sourceTable, (object) columnList, (object) columnList.Replace("[PartitionId]", partitionId.ToString()));
      this.CopyData(table, sqlStatement);
    }

    public void AddAndPopulateLeadingKey(Table table, int partitionId)
    {
      this.Logger.Info("Executing AddAndPopulateLeadingKey. partitionId: {0}", (object) partitionId);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat(string.Format("SET XACT_ABORT ON\r\n                        BEGIN TRAN\r\n\r\n                        ALTER TABLE {0} ADD PartitionId INT NULL\r\n\r\n                        EXEC sp_executesql N'UPDATE {0} SET PartitionId = @partitionId',  \r\n                                           N'@partitionId INT', @partitionId = @partitionId\r\n\r\n                        ALTER TABLE {0} ALTER COLUMN PartitionId INT NOT NULL\r\n\r\n                        ", (object) table.Name));
      stringBuilder.AppendLine();
      foreach (Index index in table.Indexes)
      {
        StringCollection queries = new StringCollection();
        if (index.IsConstraint)
          queries.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ALTER TABLE [{0}].[{1}] DROP CONSTRAINT [{2}]", (object) table.Schema, (object) table.Name, (object) index.Name));
        if (index.PreviousConstraintName != null)
          queries.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ALTER TABLE [{0}].[{1}] DROP CONSTRAINT [{2}]", (object) table.Schema, (object) table.Name, (object) index.PreviousConstraintName));
        if (index.IsPrimaryKey && index.Type == IndexType.Clustered)
        {
          index.IsPrimaryKey = false;
          index.IsUnique = true;
          index.IsUniqueConstraint = false;
          index.IsConstraint = false;
          index.ScriptDropExisting = false;
        }
        if (index.IsConstraint)
          index.ScriptAlterTable(queries, 0);
        else
          index.ScriptCreateIndex(queries, 0);
        foreach (string str in queries)
          stringBuilder.AppendLine(str);
      }
      stringBuilder.Append("COMMIT TRAN");
      this.PrepareSqlBatch(stringBuilder.Length);
      this.AddStatement(stringBuilder.ToString());
      this.BindInt("@partitionId", partitionId);
      try
      {
        this.Logger.Info("Executing statement:");
        this.Logger.Info(stringBuilder.ToString());
        this.ExecuteNonQuery();
        this.Logger.Info("Statement completed successfully.");
      }
      catch (Exception ex)
      {
        this.Logger.Error(ex);
        throw;
      }
    }

    public void RebuildIndex(Index index)
    {
      this.Logger.Info("Rebuilding index: {0} on {1}.{2}", (object) index.Name, (object) index.TableSchema, (object) index.TableName);
      string sqlStatement = string.Format("ALTER INDEX {0} ON {1}.{2} REBUILD", (object) StringUtil.QuoteName(index.Name), (object) StringUtil.QuoteName(index.TableSchema), (object) StringUtil.QuoteName(index.TableName));
      if (index.DataCompression != DataCompression.None)
        sqlStatement += string.Format((IFormatProvider) CultureInfo.InvariantCulture, " WITH(DATA_COMPRESSION = {0})", (object) index.DataCompression.ToString().ToUpperInvariant());
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
      this.Logger.Info("Index has been rebuilt: {0} on {1}.{2}", (object) index.Name, (object) index.TableSchema, (object) index.TableName);
    }

    public int GetPartitionId(Guid id)
    {
      this.Logger.Info("Getting partition id for the following host: {0}", (object) id);
      this.PrepareSqlBatch("SELECT PartitionId FROM tbl_DatabasePartitionMap WHERE ServiceHostId = @id".Length);
      this.AddStatement("SELECT PartitionId FROM tbl_DatabasePartitionMap WHERE ServiceHostId = @id");
      this.BindGuid("@id", id);
      int partitionId = (int) this.ExecuteScalar();
      this.Logger.Info("Partition id: {0}", (object) partitionId);
      return partitionId;
    }

    public void DropNonClusteredIndexes(string schemaName, string tableName)
    {
      this.Logger.Info("Dropping non-clustered indexes on {0}.{1}", (object) schemaName, (object) tableName);
      this.PrepareSqlBatch("DECLARE @stmt NVARCHAR(MAX) = ''\r\nSELECT  @stmt = @stmt + '\r\nDROP INDEX ' + QUOTENAME(name) + ' ON ' + QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName)\r\nFROM    sys.indexes\r\nWHERE   type = 2\r\n        AND is_primary_key = 0\r\n        AND is_unique_constraint = 0\r\n        AND object_id = OBJECT_ID(QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName))\r\n\r\nEXEC sp_executesql @stmt".Length);
      this.AddStatement("DECLARE @stmt NVARCHAR(MAX) = ''\r\nSELECT  @stmt = @stmt + '\r\nDROP INDEX ' + QUOTENAME(name) + ' ON ' + QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName)\r\nFROM    sys.indexes\r\nWHERE   type = 2\r\n        AND is_primary_key = 0\r\n        AND is_unique_constraint = 0\r\n        AND object_id = OBJECT_ID(QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName))\r\n\r\nEXEC sp_executesql @stmt");
      this.BindSysname("@schemaName", schemaName, false);
      this.BindSysname("@tableName", tableName, false);
      this.ExecuteNonQuery();
    }

    public void DisableNonClusteredIndexes(
      string schemaName,
      string tableName,
      string excludeIndexes)
    {
      this.Logger.Info("Disabling non-clustered indexes on {0}.{1}", (object) schemaName, (object) tableName);
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_DisableNonClusteredIndexes.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@schemaName", schemaName, false);
      this.BindSysname("@tableName", tableName, false);
      this.BindString(nameof (excludeIndexes), excludeIndexes, -1, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public void RebuildDisabledIndexes(string schemaName, string tableName, bool online)
    {
      this.Logger.Info("Rebuilding disabled indexes on {0}.{1}", (object) schemaName, (object) tableName);
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_RebuildDisabledIndexes.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@schemaName", schemaName, false);
      this.BindSysname("@tableName", tableName, false);
      this.BindBoolean("@online", online);
      this.ExecuteNonQuery();
    }
  }
}
