// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlDatabaseBackupRestoreComponent
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Configuration
{
  internal class SqlDatabaseBackupRestoreComponent : TeamFoundationSqlResourceComponent
  {
    private SqlErrorCollection m_errors;
    private const int c_RestoreComlete = 3014;
    private const int c_PercentCompete = 3211;

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
      this.Connection.FireInfoMessageEventOnUserErrors = true;
      this.Connection.InfoMessage += new SqlInfoMessageEventHandler(this.OnInfoMessage);
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      base.Initialize(requestContext, databaseCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
      this.Connection.FireInfoMessageEventOnUserErrors = true;
      this.Connection.InfoMessage += new SqlInfoMessageEventHandler(this.OnInfoMessage);
    }

    internal event EventHandler<PercentCompleteEventArgs> PercentComplete;

    internal event EventHandler<ServerMessageEventArgs> Information;

    internal event EventHandler<ServerMessageEventArgs> Complete;

    internal void Execute(string sqlStatement)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(sqlStatement, nameof (sqlStatement));
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
      if (this.m_errors != null)
        throw SqlDatabaseBackupRestoreComponent.CreateSqlException(this.m_errors);
    }

    public List<SqlBackupContentFile> ReadFileList(string backupFilePath, int fileNumber)
    {
      string str = "RESTORE FILELISTONLY FROM DISK = @path";
      if (fileNumber != 0)
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, " WITH FILE = {0}", (object) fileNumber);
      this.PrepareSqlBatch(str.Length);
      this.AddStatement(str);
      this.BindString("@path", backupFilePath, backupFilePath.Length, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, (IVssRequestContext) null);
      resultCollection.AddBinder<SqlBackupContentFile>((ObjectBinder<SqlBackupContentFile>) new SqlBackupContentFileColumns());
      List<SqlBackupContentFile> items = resultCollection.GetCurrent<SqlBackupContentFile>().Items;
      if (this.m_errors == null)
        return items;
      throw SqlDatabaseBackupRestoreComponent.CreateSqlException(this.m_errors);
    }

    public List<SqlBackupHeader> ReadBackupHeaders(string backupFilePath)
    {
      string str = "RESTORE HEADERONLY FROM DISK = @path";
      this.PrepareSqlBatch(str.Length);
      this.AddStatement(str);
      this.BindString("@path", backupFilePath, backupFilePath.Length, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, (IVssRequestContext) null);
      resultCollection.AddBinder<SqlBackupHeader>((ObjectBinder<SqlBackupHeader>) new SqlBackupHeaderColumns(backupFilePath));
      List<SqlBackupHeader> items = resultCollection.GetCurrent<SqlBackupHeader>().Items;
      if (this.m_errors == null)
        return items;
      SqlException sqlException = SqlDatabaseBackupRestoreComponent.CreateSqlException(this.m_errors);
      if (sqlException.Errors.Cast<SqlError>().Any<SqlError>((System.Func<SqlError, bool>) (error => error.Number == 3241)))
        throw new InvalidBackupException(ConfigurationResources.MediaIncorrectlyFormed((object) backupFilePath));
      throw sqlException;
    }

    private void OnInfoMessage(object sender, SqlInfoMessageEventArgs e)
    {
      for (int index = 0; index < e.Errors.Count; ++index)
      {
        SqlError error = e.Errors[index];
        if (error.Class > (byte) 0)
        {
          if (this.m_errors == null)
            this.m_errors = SqlDatabaseBackupRestoreComponent.CreateSqlErrrorCollection();
          typeof (SqlErrorCollection).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic).Invoke((object) this.m_errors, new object[1]
          {
            (object) error
          });
        }
        else
        {
          switch (error.Number)
          {
            case 3014:
              EventHandler<ServerMessageEventArgs> complete = this.Complete;
              if (complete != null)
              {
                complete((object) this, new ServerMessageEventArgs(error));
                continue;
              }
              continue;
            case 3211:
              EventHandler<PercentCompleteEventArgs> percentComplete = this.PercentComplete;
              if (percentComplete != null)
              {
                int percent = int.Parse(error.Message.Split(' ')[0], (IFormatProvider) CultureInfo.CurrentCulture);
                percentComplete((object) this, new PercentCompleteEventArgs(percent));
                continue;
              }
              continue;
            default:
              EventHandler<ServerMessageEventArgs> information = this.Information;
              if (information != null)
              {
                information((object) this, new ServerMessageEventArgs(error));
                continue;
              }
              continue;
          }
        }
      }
    }

    private static SqlErrorCollection CreateSqlErrrorCollection() => (SqlErrorCollection) typeof (SqlErrorCollection).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, (Binder) null, Type.EmptyTypes, (ParameterModifier[]) null).Invoke((object[]) null);

    private static SqlException CreateSqlException(SqlErrorCollection errorCollection) => (SqlException) typeof (SqlException).GetMethod("CreateException", BindingFlags.Static | BindingFlags.NonPublic, (Binder) null, new Type[2]
    {
      typeof (SqlErrorCollection),
      typeof (string)
    }, (ParameterModifier[]) null).Invoke((object) null, new object[2]
    {
      (object) errorCollection,
      null
    });
  }
}
