// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlDatabaseRestore
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class SqlDatabaseRestore
  {
    private readonly string[] m_backupFiles;
    private readonly List<SqlRelocateFile> m_relocateFiles = new List<SqlRelocateFile>();

    public SqlDatabaseRestore(string backupFile, ITFLogger logger)
      : this(new string[1]{ backupFile }, logger)
    {
    }

    public SqlDatabaseRestore(string[] backupFiles, ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<string[]>(backupFiles, nameof (backupFiles));
      ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) backupFiles, nameof (backupFiles));
      this.m_backupFiles = backupFiles;
      this.PercentCompleteNotification = 10;
      this.Unload = false;
      this.Recovery = true;
      this.ContinueAfterError = true;
      this.Logger = logger ?? (ITFLogger) new NullLogger();
    }

    public int FileNumber { get; set; }

    public bool ContinueAfterError { get; set; }

    public string Database { get; set; }

    public bool Recovery { get; set; }

    public bool ReplaceDatabase { get; set; }

    public int PercentCompleteNotification { get; set; }

    public bool Unload { get; set; }

    public string StopAtMark { get; set; }

    public DateTime StopAtMarkAfterDate { get; set; }

    public string[] BackupFiles => this.m_backupFiles;

    public event EventHandler<PercentCompleteEventArgs> PercentComplete;

    public event EventHandler<ServerMessageEventArgs> Information;

    public event EventHandler<ServerMessageEventArgs> Complete;

    public List<SqlRelocateFile> RelocateFileList => this.m_relocateFiles;

    public List<SqlBackupContentFile> ReadFileList(ISqlConnectionInfo connectionInfo)
    {
      using (SqlDatabaseBackupRestoreComponent componentRaw = connectionInfo.CreateComponentRaw<SqlDatabaseBackupRestoreComponent>())
        return componentRaw.ReadFileList(((IEnumerable<string>) this.BackupFiles).First<string>(), this.FileNumber);
    }

    public List<SqlBackupHeader> ReadBackupHeaders(ISqlConnectionInfo connectionInfo)
    {
      using (SqlDatabaseBackupRestoreComponent componentRaw = connectionInfo.CreateComponentRaw<SqlDatabaseBackupRestoreComponent>())
        return componentRaw.ReadBackupHeaders(((IEnumerable<string>) this.BackupFiles).FirstOrDefault<string>());
    }

    public void RelocateFiles(
      string databaseName,
      ISqlConnectionInfo connectionInfo,
      string dataDirectory,
      string logDirectory = null,
      string fullTextCatalogDirectory = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      ArgumentUtility.CheckForNull<ISqlConnectionInfo>(connectionInfo, "connectionString");
      ArgumentUtility.CheckStringForNullOrEmpty(dataDirectory, nameof (dataDirectory));
      this.Logger.Info("Executing SqlDatabaseRestore.RelocateFiles");
      HashSet<string> stringSet1;
      HashSet<string> stringSet2;
      HashSet<string> stringSet3;
      using (SqlInstanceComponent componentRaw = connectionInfo.CreateComponentRaw<SqlInstanceComponent>())
      {
        stringSet1 = new HashSet<string>((IEnumerable<string>) componentRaw.GetFiles(dataDirectory), (IEqualityComparer<string>) VssStringComparer.FilePath);
        if (string.IsNullOrEmpty(logDirectory) || VssStringComparer.FieldName.Compare(dataDirectory, logDirectory) == 0)
        {
          logDirectory = dataDirectory;
          stringSet2 = stringSet1;
        }
        else
          stringSet2 = new HashSet<string>((IEnumerable<string>) componentRaw.GetFiles(logDirectory), (IEqualityComparer<string>) VssStringComparer.FilePath);
        if (string.IsNullOrEmpty(fullTextCatalogDirectory) || VssStringComparer.FieldName.Compare(fullTextCatalogDirectory, dataDirectory) == 0)
        {
          fullTextCatalogDirectory = dataDirectory;
          stringSet3 = stringSet1;
        }
        else
          stringSet3 = VssStringComparer.FieldName.Compare(fullTextCatalogDirectory, logDirectory) != 0 ? new HashSet<string>((IEnumerable<string>) componentRaw.GetFiles(fullTextCatalogDirectory), (IEqualityComparer<string>) VssStringComparer.FilePath) : stringSet2;
      }
      List<SqlBackupContentFile> backupContentFileList = this.ReadFileList(connectionInfo);
      this.Logger.Info("Found {0} backup content files.", (object) backupContentFileList.Count);
      foreach (SqlBackupContentFile backupContentFile in backupContentFileList)
      {
        string path2;
        string path1;
        switch (backupContentFile.FileType)
        {
          case DatabaseFileType.DataFile:
            path2 = databaseName + ".mdf";
            if (!stringSet1.Add(path2))
            {
              for (int index = 1; index < 10000; ++index)
              {
                path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}.mdf", (object) databaseName, (object) Guid.NewGuid());
                if (stringSet1.Add(path2))
                  break;
              }
            }
            path1 = dataDirectory;
            break;
          case DatabaseFileType.LogFile:
            path2 = databaseName + "_log.ldf";
            if (!stringSet2.Add(path2))
            {
              for (int index = 1; index < 10000; ++index)
              {
                path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}_log.ldf", (object) databaseName, (object) Guid.NewGuid());
                if (stringSet2.Add(path2))
                  break;
              }
            }
            path1 = logDirectory;
            break;
          case DatabaseFileType.FullTextCatalog:
            path2 = databaseName + "_fulltext.mdf";
            if (!stringSet3.Add(path2))
            {
              for (int index = 1; index < 10000; ++index)
              {
                path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}_fulltext.mdf", (object) databaseName, (object) Guid.NewGuid());
                if (stringSet3.Add(path2))
                  break;
              }
            }
            path1 = fullTextCatalogDirectory;
            break;
          default:
            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected file type: {0}", (object) backupContentFile.FileType));
        }
        string physicalFileName = Path.Combine(path1, path2);
        this.Logger.Info("{0} is relocated to {1}.", (object) backupContentFile.LogicalName, (object) physicalFileName);
        this.RelocateFileList.Add(new SqlRelocateFile(backupContentFile.LogicalName, physicalFileName));
      }
      this.Logger.Info("Exiting SqlDatabaseRestore.RelocateFiles");
    }

    public void Restore(ISqlConnectionInfo connectionInfo)
    {
      this.Logger.Info("Executing SqlDatabaseRestore.Restore");
      string sqlStatement = this.Script();
      Stopwatch stopwatch = Stopwatch.StartNew();
      using (SqlDatabaseBackupRestoreComponent componentRaw = connectionInfo.CreateComponentRaw<SqlDatabaseBackupRestoreComponent>(0, maxDeadlockRetries: 50))
      {
        componentRaw.Information += (EventHandler<ServerMessageEventArgs>) ((s, e) => this.RaiseEvent<ServerMessageEventArgs>(this.Information, e));
        componentRaw.Complete += (EventHandler<ServerMessageEventArgs>) ((s, e) => this.RaiseEvent<ServerMessageEventArgs>(this.Complete, e));
        componentRaw.PercentComplete += (EventHandler<PercentCompleteEventArgs>) ((s, e) => this.RaiseEvent<PercentCompleteEventArgs>(this.PercentComplete, e));
        componentRaw.Execute(sqlStatement);
      }
      this.Logger.Info("Restore operation completed. Elapsed time: {0}", (object) stopwatch.Elapsed);
      stopwatch.Restart();
      if (this.Recovery)
      {
        using (TeamFoundationDataTierComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
        {
          while (stopwatch.Elapsed < TimeSpan.FromMinutes(1.0))
          {
            DatabaseInformation databaseInfo = componentRaw.GetDatabaseInfo(this.Database);
            this.Logger.Info("Database name: {0}, State: {1}", (object) databaseInfo.Name, (object) databaseInfo.State);
            if (databaseInfo.State != DatabaseState.Online)
              Thread.Sleep(TimeSpan.FromSeconds(1.0));
            else
              break;
          }
        }
      }
      this.Logger.Info("Exiting SqlDatabaseRestore.Restore");
    }

    private void RaiseEvent<TEventArgs>(EventHandler<TEventArgs> eventHandler, TEventArgs args) where TEventArgs : EventArgs
    {
      if (eventHandler == null)
        return;
      eventHandler((object) this, args);
    }

    public string Script()
    {
      if (string.IsNullOrEmpty(this.Database))
        throw new InvalidOperationException("Database property not set.");
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "RESTORE DATABASE [{0}] FROM", (object) TFCommonUtil.EscapeString(this.Database, '['));
      foreach (string backupFile in this.m_backupFiles)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " DISK = N'{0}',", (object) TFCommonUtil.EscapeString(backupFile, '\''));
      --stringBuilder.Length;
      stringBuilder.AppendFormat(" WITH ");
      foreach (SqlRelocateFile relocateFile in this.RelocateFileList)
        stringBuilder.AppendFormat(" MOVE N'{0}' TO N'{1}',", (object) TFCommonUtil.EscapeString(relocateFile.LogicalFileName, '\''), (object) TFCommonUtil.EscapeString(relocateFile.PhysicalFileName, '\''));
      stringBuilder.Append(this.Recovery ? " RECOVERY," : " NORECOVERY,");
      stringBuilder.Append(this.Unload ? " UNLOAD," : " NOUNLOAD,");
      if (this.ReplaceDatabase)
        stringBuilder.Append(" REPLACE,");
      if (this.FileNumber > 0)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " FILE = {0},", (object) this.FileNumber);
      if (this.ContinueAfterError)
        stringBuilder.Append(" CONTINUE_AFTER_ERROR,");
      if (this.PercentCompleteNotification > 0 && this.PercentCompleteNotification <= 100)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " STATS = {0},", (object) this.PercentCompleteNotification);
      if (!string.IsNullOrEmpty(this.StopAtMark))
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " STOPATMARK = N'{0}'", (object) TFCommonUtil.EscapeString(this.StopAtMark, '\''));
        if (this.StopAtMarkAfterDate > DateTime.MinValue)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " AFTER N'{0}'", (object) this.StopAtMarkAfterDate.ToString("yyyy/MM/dd HH:mm:ss"));
        stringBuilder.AppendFormat(",");
      }
      return stringBuilder.ToString().Trim(',');
    }

    protected ITFLogger Logger { get; private set; }

    private static string GetString(IDataRecord record, string columnName)
    {
      object obj = record[columnName];
      return !(obj is DBNull) ? (string) obj : (string) null;
    }

    private static Guid GetGuid(IDataRecord record, string columnName)
    {
      object obj = record[columnName];
      return !(obj is DBNull) ? (Guid) obj : Guid.Empty;
    }
  }
}
