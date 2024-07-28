// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseManagementSqlInstanceOperationsComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseManagementSqlInstanceOperationsComponent : 
    TeamFoundationSqlResourceComponent
  {
    private static readonly string s_copyDatabaseStatement = "CREATE DATABASE {0} AS COPY OF {1}.{2}";
    private static readonly string s_getDatabaseNameStatement = "\nSELECT  state \nFROM    sys.databases \nWHERE   name = @databaseName";
    private static readonly string s_getDatabaseCopyStatusStatement = "\nSELECT  dbc.database_id, \n        dbc.start_date, \n        dbc.modify_date,\n        dbc.percent_complete,\n        dbc.error_code,\n        dbc.error_desc,\n        dbc.error_severity,\n        dbc.error_state,\n        dbc.replication_state,\n        dbc.replication_state_desc\nFROM    sys.dm_database_copies dbc\nJOIN    sys.databases db\nON      db.database_id = dbc.database_id\nWHERE   db.name = @databaseName";
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static DatabaseManagementSqlInstanceOperationsComponent()
    {
      DatabaseManagementSqlInstanceOperationsComponent.s_sqlExceptionFactories.Add(800074, new SqlExceptionFactory(typeof (DatabaseNotFoundException)));
      DatabaseManagementSqlInstanceOperationsComponent.s_sqlExceptionFactories.Add(800075, new SqlExceptionFactory(typeof (DatabasePoolNotFoundException)));
      DatabaseManagementSqlInstanceOperationsComponent.s_sqlExceptionFactories.Add(800076, new SqlExceptionFactory(typeof (DatabasePoolAlreadyExistsException)));
    }

    public void BeginCopyDatabase(
      string sourceServerName,
      string sourceDatabaseName,
      string destinationDatabaseName)
    {
      sourceDatabaseName = StringUtil.QuoteName(sourceDatabaseName);
      destinationDatabaseName = StringUtil.QuoteName(destinationDatabaseName);
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, DatabaseManagementSqlInstanceOperationsComponent.s_copyDatabaseStatement, (object) destinationDatabaseName, (object) sourceServerName, (object) sourceDatabaseName);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
    }

    public SqlInstanceDatabaseState? GetDatabaseState(string databaseName)
    {
      this.PrepareSqlBatch(DatabaseManagementSqlInstanceOperationsComponent.s_getDatabaseNameStatement.Length);
      this.AddStatement(DatabaseManagementSqlInstanceOperationsComponent.s_getDatabaseNameStatement, 1);
      this.BindString("@databaseName", databaseName, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      object obj = this.ExecuteScalar();
      return obj != null ? new SqlInstanceDatabaseState?((SqlInstanceDatabaseState) (byte) obj) : new SqlInstanceDatabaseState?();
    }

    public TeamFoundationDatabaseCopyStatus GetCopyStatus(string databaseName)
    {
      this.PrepareSqlBatch(DatabaseManagementSqlInstanceOperationsComponent.s_getDatabaseCopyStatusStatement.Length);
      this.AddStatement(DatabaseManagementSqlInstanceOperationsComponent.s_getDatabaseCopyStatusStatement, 1);
      this.BindString("@databaseName", databaseName, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      SqlDataReader dataReader = this.ExecuteReader();
      if (!dataReader.Read())
        return (TeamFoundationDatabaseCopyStatus) null;
      TeamFoundationDatabaseCopyStatus result;
      new TeamFoundationDatabaseCopyStatusBinder(dataReader, this.ProcedureName).Bind(out result);
      return result;
    }
  }
}
