// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.TfsDatabaseRestore
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public static class TfsDatabaseRestore
  {
    public static List<string> Restore(
      string sqlInstance,
      string backupDirectory,
      string label,
      ITFLogger logger)
    {
      return TfsDatabaseRestore.Restore(sqlInstance, backupDirectory, label, (Func<string, bool>) (fileNameFilter => true), 1, logger);
    }

    public static List<string> Restore(
      string sqlInstance,
      string backupDirectory,
      string label,
      Func<string, bool> fileNameFilter,
      ITFLogger logger)
    {
      return TfsDatabaseRestore.Restore(sqlInstance, backupDirectory, label, fileNameFilter, 1, logger);
    }

    public static List<string> Restore(
      string sqlInstance,
      string backupDirectory,
      string label,
      Func<string, bool> fileNameFilter,
      int threads,
      ITFLogger logger)
    {
      logger.Info("Executing TfsDatabaseRestore.Restore");
      ArgumentUtility.CheckStringForNullOrEmpty(sqlInstance, nameof (sqlInstance));
      ArgumentUtility.CheckStringForNullOrEmpty(backupDirectory, nameof (backupDirectory));
      logger.Info("Backup Directory: {0}", (object) backupDirectory);
      logger.Info("Sql Instance: {0}", (object) sqlInstance);
      ISqlConnectionInfo connectionInfo = SqlConnectionInfoFactory.Create(new SqlConnectionStringBuilder()
      {
        DataSource = sqlInstance,
        IntegratedSecurity = true
      }.ConnectionString);
      ConcurrentBag<string> restoredDatabases = new ConcurrentBag<string>();
      List<string> list;
      string dataPath;
      string logPath;
      using (connectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
      {
        using (SqlInstanceComponent componentRaw = connectionInfo.CreateComponentRaw<SqlInstanceComponent>())
        {
          SqlInstanceProperties instanceProperties = componentRaw.GetSqlInstanceProperties();
          dataPath = instanceProperties.DefaultDataPath;
          if (string.IsNullOrEmpty(dataPath))
            dataPath = instanceProperties.MasterDbPath;
          logPath = instanceProperties.DefaultLogPath;
          if (string.IsNullOrEmpty(logPath))
            logPath = instanceProperties.DefaultDataPath;
          if (string.IsNullOrEmpty(logPath))
            logPath = instanceProperties.MasterDbLogPath;
          logger.Info("Data path: {0}", (object) dataPath);
          logger.Info("Log path: {0}", (object) logPath);
          list = componentRaw.GetFiles(backupDirectory).Where<string>((Func<string, bool>) (file => file.EndsWith(".bak", StringComparison.OrdinalIgnoreCase) && fileNameFilter(file))).ToList<string>();
          logger.Info("Found {0} backup files.", (object) list.Count);
        }
      }
      ParallelOptions parallelOptions = new ParallelOptions();
      if (threads < 1)
        threads = 1;
      parallelOptions.MaxDegreeOfParallelism = threads;
      Dictionary<Guid, List<string>> source1 = new Dictionary<Guid, List<string>>();
      foreach (string path2 in list)
      {
        string backupFilePath = Path.Combine(backupDirectory, path2);
        List<SqlBackupHeader> source2;
        using (SqlDatabaseBackupRestoreComponent componentRaw = connectionInfo.CreateComponentRaw<SqlDatabaseBackupRestoreComponent>(0))
          source2 = componentRaw.ReadBackupHeaders(backupFilePath);
        Guid backupSetGuid = source2.FirstOrDefault<SqlBackupHeader>().BackupSetGUID;
        if (source1.ContainsKey(backupSetGuid))
          source1[backupSetGuid].Add(backupFilePath);
        else
          source1.Add(backupSetGuid, new List<string>()
          {
            backupFilePath
          });
      }
      using (Dictionary<Guid, List<string>>.Enumerator enumerator = source1.GetEnumerator())
      {
label_46:
        while (enumerator.MoveNext())
        {
          KeyValuePair<Guid, List<string>> current = enumerator.Current;
          TeamFoundationDataTierComponent dataTierComponent = (TeamFoundationDataTierComponent) null;
          try
          {
            dataTierComponent = connectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>(logger: logger);
            string withoutExtension = Path.GetFileNameWithoutExtension(current.Value[0]);
            string databaseName = TfsDatabaseRestore.GetDatabaseName(label, withoutExtension);
            if (dataTierComponent.CheckIfDatabaseExists(databaseName))
            {
              Stopwatch stopwatch = Stopwatch.StartNew();
              while (true)
              {
                try
                {
                  dataTierComponent.DropDatabase(databaseName, DropDatabaseOptions.CloseExistingConnections);
                  goto label_46;
                }
                catch (DBExecutingDeadlockException ex)
                {
                  if (stopwatch.Elapsed < TimeSpan.FromMinutes(5.0))
                  {
                    if (stopwatch.Elapsed > TimeSpan.FromSeconds(45.0) && string.Equals(Environment.GetEnvironmentVariable("SYSTEM_TEAMPROJECT"), "VSOnline", StringComparison.OrdinalIgnoreCase) && string.Equals(connectionInfo.DataSource, "localhost", StringComparison.Ordinal))
                    {
                      dataTierComponent.Dispose();
                      dataTierComponent = (TeamFoundationDataTierComponent) null;
                      if (WindowsServiceManagement.IsServiceInstalled(".", "MSSQLSERVER"))
                      {
                        WindowsServiceManagement.StopWindowsService("MSSQLSERVER", logger);
                        WindowsServiceManagement.StartWindowsService("MSSQLSERVER", logger);
                      }
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(10.0));
                    if (dataTierComponent == null)
                      dataTierComponent = connectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>(logger: logger);
                  }
                  else
                    throw;
                }
              }
            }
          }
          finally
          {
            dataTierComponent?.Dispose();
          }
        }
      }
      Parallel.ForEach<KeyValuePair<Guid, List<string>>>((IEnumerable<KeyValuePair<Guid, List<string>>>) source1, parallelOptions, (Action<KeyValuePair<Guid, List<string>>>) (mediaSet =>
      {
        using (connectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>(logger: logger))
        {
          string[] array = mediaSet.Value.ToArray();
          string databaseName = TfsDatabaseRestore.GetDatabaseName(label, Path.GetFileNameWithoutExtension(array[0]));
          logger.Info("Database name: {0}\nRestoring from: {1}", (object) databaseName, (object) string.Join(",", array));
          SqlDatabaseRestore sqlDatabaseRestore = new SqlDatabaseRestore(array, logger);
          sqlDatabaseRestore.Database = databaseName;
          sqlDatabaseRestore.RelocateFiles(databaseName, connectionInfo, dataPath, logPath);
          sqlDatabaseRestore.PercentCompleteNotification = 10;
          sqlDatabaseRestore.Information += (EventHandler<ServerMessageEventArgs>) ((s, e) => logger.Info(e.Error.Message));
          sqlDatabaseRestore.Restore(connectionInfo);
          restoredDatabases.Add(databaseName);
        }
      }));
      logger.Info("TfsDatabaseRestore.Restore done.");
      return restoredDatabases.ToList<string>();
    }

    public static IAsyncResult BeginRestore(
      string sqlInstance,
      string backupDirectory,
      string label,
      ITFLogger logger)
    {
      return TfsDatabaseRestore.BeginRestore(sqlInstance, backupDirectory, label, (Func<string, bool>) (dbName => true), 1, logger);
    }

    public static IAsyncResult BeginRestore(
      string sqlInstance,
      string backupDirectory,
      string label,
      Func<string, bool> dbNameFilter,
      int threads,
      ITFLogger logger)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      TfsDatabaseRestore.RestoreDatabasesDelegate databasesDelegate = TfsDatabaseRestore.\u003C\u003EO.\u003C0\u003E__Restore ?? (TfsDatabaseRestore.\u003C\u003EO.\u003C0\u003E__Restore = new TfsDatabaseRestore.RestoreDatabasesDelegate(TfsDatabaseRestore.Restore));
      return databasesDelegate.BeginInvoke(sqlInstance, backupDirectory, label, dbNameFilter, threads, logger, (AsyncCallback) null, (object) databasesDelegate);
    }

    public static List<string> EndRestore(IAsyncResult result) => ((TfsDatabaseRestore.RestoreDatabasesDelegate) result.AsyncState).EndInvoke(result);

    private static string GetDatabaseName(string label, string fileNameWithoutExtension) => !fileNameWithoutExtension.StartsWith("Tfs_DataImport", StringComparison.OrdinalIgnoreCase) ? (!fileNameWithoutExtension.StartsWith("Tfs_", StringComparison.OrdinalIgnoreCase) || !label.Contains("devfab") ? (!fileNameWithoutExtension.StartsWith("Tfs_", StringComparison.OrdinalIgnoreCase) ? (!fileNameWithoutExtension.StartsWith("AzureDevOps_", StringComparison.OrdinalIgnoreCase) ? (!fileNameWithoutExtension.StartsWith("Sps_", StringComparison.OrdinalIgnoreCase) ? (!fileNameWithoutExtension.StartsWith("DevelopmentStorage", StringComparison.OrdinalIgnoreCase) ? label + fileNameWithoutExtension : fileNameWithoutExtension) : string.Format("Sps_{0}{1}", (object) label, (object) fileNameWithoutExtension.Substring(4))) : string.Format("AzureDevOps_{0}{1}", (object) label, (object) fileNameWithoutExtension.Substring(12))) : string.Format("AzureDevOps_{0}{1}", (object) label, (object) fileNameWithoutExtension.Substring(4))) : string.Format("Tfs_{0}{1}", (object) label, (object) fileNameWithoutExtension.Substring(4))) : fileNameWithoutExtension;

    private delegate List<string> RestoreDatabasesDelegate(
      string sqlInstance,
      string backupDirectory,
      string prefix,
      Func<string, bool> dbNameFilter,
      int threads,
      ITFLogger logger);
  }
}
