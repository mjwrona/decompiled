// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.PersistentStorageBase
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using Microsoft.VisualStudio.LocalLogger;
using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Telemetry.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal abstract class PersistentStorageBase : StorageBase
  {
    private readonly ConcurrentDictionary<string, string> filesToDelete;
    private object peekLockObj = new object();
    private DirectoryInfo storageFolder;
    private int transmissionsDropped;
    private string storageFolderName;
    private bool storageFolderInitialized;
    private object storageFolderLock = new object();

    internal PersistentStorageBase(string uniqueFolderName)
    {
      this.peekedTransmissions = (IDictionary<string, string>) new ConcurrentDictionary<string, string>();
      this.filesToDelete = new ConcurrentDictionary<string, string>();
      this.storageFolderName = uniqueFolderName;
      if (string.IsNullOrEmpty(uniqueFolderName))
        this.storageFolderName = PersistentStorageBase.GetSHA256Hash(PersistentStorageBase.GetApplicationIdentity(), "Storage");
      this.CapacityInBytes = 10485760UL;
      this.MaxFiles = 5000U;
      Task.Factory.StartNew(new Action(this.DeleteObsoleteFiles), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).ContinueWith((Action<Task>) (task =>
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Storage: Unhandled exception in DeleteObsoleteFiles: {0}", new object[1]
        {
          (object) task.Exception
        });
        CoreEventSource.Log.LogVerbose(message);
      }), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
    }

    internal override string FolderName => this.storageFolderName;

    internal bool StorageFolderInitialized
    {
      get => this.storageFolderInitialized;
      set => this.storageFolderInitialized = value;
    }

    internal override DirectoryInfo StorageFolder
    {
      get
      {
        if (!this.storageFolderInitialized)
        {
          lock (this.storageFolderLock)
          {
            if (!this.storageFolderInitialized)
            {
              try
              {
                this.storageFolder = this.GetApplicationFolder();
              }
              catch (Exception ex)
              {
                this.storageFolder = (DirectoryInfo) null;
                string str = string.Format("Failed to create storage folder: {0}", (object) ex);
                CoreEventSource.Log.LogVerbose(str);
                LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Telemetry", str);
              }
              this.storageFolderInitialized = true;
              string str1 = string.Format("Storage folder: {0}", this.storageFolder == null ? (object) "null" : (object) this.storageFolder.FullName);
              CoreEventSource.Log.LogVerbose(str1);
              LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", str1);
            }
          }
        }
        return this.storageFolder;
      }
    }

    internal override IEnumerable<StorageTransmission> PeekAll(CancellationToken token)
    {
      List<StorageTransmission> storageTransmissionList = new List<StorageTransmission>();
      lock (this.peekLockObj)
      {
        foreach (FileInfo filteredFile in this.GetFilteredFiles())
        {
          token.ThrowIfCancellationRequested();
          StorageTransmission storageTransmission = this.BuildTransmissionFromFile(filteredFile);
          if (storageTransmission != null)
            storageTransmissionList.Add(storageTransmission);
        }
      }
      if (LocalFileLoggerService.Default.Enabled)
        LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PersistenceStorageBase.PeekAll peeked {0} transmissions", new object[1]
        {
          (object) storageTransmissionList.Count
        }));
      return (IEnumerable<StorageTransmission>) storageTransmissionList;
    }

    internal override StorageTransmission Peek()
    {
      lock (this.peekLockObj)
      {
        foreach (FileInfo filteredFile in this.GetFilteredFiles())
        {
          StorageTransmission storageTransmission = this.BuildTransmissionFromFile(filteredFile);
          if (storageTransmission != null)
          {
            if (LocalFileLoggerService.Default.Enabled)
              LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Transmission ({0}): PersistenceStorageBase.Peek peeked transmission", new object[1]
              {
                (object) storageTransmission
              }));
            return storageTransmission;
          }
        }
      }
      return (StorageTransmission) null;
    }

    internal override void Delete(StorageTransmission item)
    {
      if (this.StorageFolder == null)
        return;
      this.filesToDelete[item.FileName] = item.FullFilePath;
      if (LocalFileLoggerService.Default.Enabled)
        LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Transmission ({0}): PersistenceStorageBase.Delete try to delete transmission", new object[1]
        {
          (object) item
        }));
      this.TryRemoveFilesToDelete();
    }

    private void TryRemoveFilesToDelete()
    {
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<string, string> keyValuePair in this.filesToDelete)
      {
        try
        {
          if (LocalFileLoggerService.Default.Enabled)
            LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PersistenceStorageBase.TryRemoveFilesToDelete try to delete file {0}", new object[1]
            {
              (object) keyValuePair.Value
            }));
          ReparsePointAware.DeleteFile(keyValuePair.Value);
          stringList.Add(keyValuePair.Key);
        }
        catch (Exception ex)
        {
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to delete a file. file: {0} Exception: {1}", new object[2]
          {
            string.IsNullOrEmpty(keyValuePair.Value) ? (object) "null" : (object) keyValuePair.Value,
            (object) ex
          });
          CoreEventSource.Log.LogVerbose(message);
          LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PersistenceStorageBase.TryRemoveFilesToDelete exception happens when deleting file {0}. Exception: {1}", new object[2]
          {
            (object) (keyValuePair.Value ?? "null"),
            (object) ex.Message
          }));
        }
      }
      foreach (string key in stringList)
        this.filesToDelete.TryRemove(key, out string _);
    }

    internal override async Task EnqueueAsync(Transmission transmission)
    {
      PersistentStorageBase persistentStorageBase = this;
      try
      {
        if (persistentStorageBase.StorageFolder == null)
          return;
        if (transmission == null)
          CoreEventSource.Log.LogVerbose("transmission is null. EnqueueAsync is skipped");
        else if (persistentStorageBase.IsStorageLimitsReached())
        {
          if (Interlocked.Increment(ref persistentStorageBase.transmissionsDropped) % 100 != 0)
            return;
          CoreEventSource.Log.LogVerbose("Total transmissions dropped: " + persistentStorageBase.transmissionsDropped.ToString());
        }
        else
        {
          string fileName = persistentStorageBase.BuildFullFileNameWithoutExtension();
          string tempFullFilePath = fileName + ".tmp";
          await PersistentStorageBase.SaveTransmissionToFileAsync(transmission, tempFullFilePath).ConfigureAwait(false);
          string str = fileName + ".trn";
          if (LocalFileLoggerService.Default.Enabled)
            LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Transmission ({0}): PersistenceStorageBase.EnqueueAsync about to rename {1} to {2}", new object[3]
            {
              (object) transmission,
              (object) System.IO.Path.GetFileName(tempFullFilePath),
              (object) System.IO.Path.GetFileName(str)
            }));
          ReparsePointAware.MoveFile(tempFullFilePath, str);
          fileName = (string) null;
          tempFullFilePath = (string) null;
        }
      }
      catch (Exception ex)
      {
        CoreEventSource.Log.LogVerbose(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "EnqueueAsync: Exception: {0}", new object[1]
        {
          (object) ex
        }));
        LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Transmission ({0}): PersistenceStorageBase.EnqueueAsync rename failed with exception: {1}", new object[2]
        {
          (object) transmission,
          (object) ex.Message
        }));
      }
    }

    private StorageTransmission BuildTransmissionFromFile(FileInfo file)
    {
      try
      {
        string str = this.BuildNewFullFileNameWithSameDate(file.Name);
        ReparsePointAware.MoveFile(file.FullName, str);
        FileInfo newfile = new FileInfo(str);
        StorageTransmission result = PersistentStorageBase.LoadTransmissionFromFileAsync(newfile).ConfigureAwait(false).GetAwaiter().GetResult();
        if (LocalFileLoggerService.Default.Enabled)
          LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Transmission ({0}): PersistenceStorageBase.BuildTransmissionFromFile renamed file from {1} to {2}", new object[3]
          {
            (object) result,
            (object) file.Name,
            (object) newfile.Name
          }));
        result.Disposing = (Action<StorageTransmission>) (item => this.OnPeekedItemDisposed(newfile.Name));
        this.peekedTransmissions.Add(newfile.Name, newfile.FullName);
        return result;
      }
      catch (Exception ex)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to load an item from the storage. file: {0} Exception: {1}", new object[2]
        {
          (object) file,
          (object) ex
        });
        CoreEventSource.Log.LogVerbose(message);
      }
      return (StorageTransmission) null;
    }

    private IEnumerable<FileInfo> GetFilteredFiles()
    {
      PersistentStorageBase persistentStorageBase = this;
      IEnumerable<FileInfo> files = persistentStorageBase.GetFiles("*.trn");
      List<FileInfo> fileInfoList = new List<FileInfo>();
      foreach (FileInfo fileInfo in files)
      {
        bool flag = false;
        try
        {
          if (!persistentStorageBase.peekedTransmissions.ContainsKey(fileInfo.Name))
          {
            if (!persistentStorageBase.filesToDelete.ContainsKey(fileInfo.Name))
            {
              if (persistentStorageBase.CanDelete(fileInfo))
                flag = true;
            }
          }
        }
        catch (Exception ex)
        {
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to get information about an item from the storage. file: {0} Exception: {1}", new object[2]
          {
            (object) fileInfo,
            (object) ex
          });
          CoreEventSource.Log.LogVerbose(message);
        }
        if (flag)
          yield return fileInfo;
      }
    }

    private string BuildFullFileNameWithoutExtension()
    {
      string str = Guid.NewGuid().ToString("N");
      return System.IO.Path.Combine(this.StorageFolder.FullName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", new object[2]
      {
        (object) DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
        (object) str
      }));
    }

    private string BuildNewFullFileNameWithSameDate(string name)
    {
      string[] source = name.Split(new char[1]{ '_' }, StringSplitOptions.RemoveEmptyEntries);
      if (((IEnumerable<string>) source).Count<string>() != 2)
        return name;
      string extension = System.IO.Path.GetExtension(name);
      string str = Guid.NewGuid().ToString("N");
      return System.IO.Path.Combine(this.StorageFolder.FullName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}{2}", new object[3]
      {
        (object) source[0],
        (object) str,
        (object) extension
      }));
    }

    private static async Task SaveTransmissionToFileAsync(
      Transmission transmission,
      string fileFullName)
    {
      try
      {
        using (FileStream stream = ReparsePointAware.OpenWrite(fileFullName))
        {
          try
          {
            GC.SuppressFinalize((object) stream);
            if (LocalFileLoggerService.Default.Enabled)
              LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Transmission ({0}): PersistenceStorageBase.SaveTransmissionToFileAsync to file {1}", new object[2]
              {
                (object) transmission,
                (object) System.IO.Path.GetFileName(fileFullName)
              }));
            await StorageTransmission.SaveAsync(transmission, (Stream) stream).ConfigureAwait(false);
          }
          finally
          {
            GC.ReRegisterForFinalize((object) stream);
          }
        }
      }
      catch (UnauthorizedAccessException ex)
      {
        string message = string.Format("Failed to save transmission to file. UnauthorizedAccessException. File full path: {0}", (object) fileFullName);
        CoreEventSource.Log.LogVerbose(message);
        LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Transmission ({0}): PersistenceStorageBase.SaveTransmissionToFileAsync UnauthorizedAccessException", new object[1]
        {
          (object) transmission
        }));
        throw;
      }
    }

    private static async Task<StorageTransmission> LoadTransmissionFromFileAsync(FileInfo file)
    {
      StorageTransmission storageTransmission;
      try
      {
        using (FileStream stream = ReparsePointAware.OpenRead(file.FullName))
          storageTransmission = await StorageTransmission.CreateFromStreamAsync((Stream) stream, file.FullName).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        string str = string.Format("Failed to load transmission from file. File full path: {0}, Exception: {1}", (object) file.FullName, (object) ex);
        CoreEventSource.Log.LogVerbose(str);
        LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Transmission", str);
        throw;
      }
      return storageTransmission;
    }

    private static string GetApplicationIdentity()
    {
      string str1 = string.Empty;
      try
      {
        str1 = WindowsIdentity.GetCurrent().Name;
      }
      catch (Exception ex)
      {
        CoreEventSource.Log.LogVerbose(string.Format("GetApplicationIdentity: Failed to read user identity. Exception: {0}", (object) ex));
      }
      string str2 = string.Empty;
      try
      {
        str2 = AppDomain.CurrentDomain.BaseDirectory;
      }
      catch (AppDomainUnloadedException ex)
      {
        CoreEventSource.Log.LogVerbose(string.Format("GetApplicationIdentity: Failed to read the domain's base directory. Exception: {0}", (object) ex));
      }
      string str3 = string.Empty;
      try
      {
        using (Process currentProcess = Process.GetCurrentProcess())
          str3 = currentProcess.ProcessName;
      }
      catch (Exception ex)
      {
        CoreEventSource.Log.LogVerbose(string.Format("GetApplicationIdentity: Failed to read the process name. Exception: {0}", (object) ex));
      }
      return string.Format("{0}@{1}{2}", (object) str1, (object) str2, (object) str3);
    }

    internal static string GetSHA256Hash(string input, string defaultValue = null)
    {
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        byte[] bytes = Encoding.Unicode.GetBytes(input);
        foreach (byte num in FipsCompliantSha.Sha256.Value.ComputeHash(bytes))
          stringBuilder.Append(num.ToString("x2", (IFormatProvider) CultureInfo.InvariantCulture));
        return stringBuilder.ToString();
      }
      catch (Exception ex)
      {
        string str = string.Format("GetSHA256Hash('{0}'): Failed to hash. Change string to Base64. Exception: {1}", (object) input, (object) ex);
        CoreEventSource.Log.LogVerbose(str);
        LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Warning, "Telemetry", str);
        if (defaultValue != null)
          return defaultValue;
        throw;
      }
    }

    protected abstract DirectoryInfo GetApplicationFolder();

    private bool IsStorageLimitsReached()
    {
      if (this.MaxFiles == uint.MaxValue && this.CapacityInBytes == ulong.MaxValue)
        return false;
      FileInfo[] files = this.StorageFolder.GetFiles();
      if ((long) files.Length >= (long) this.MaxFiles)
        return true;
      ulong num = 0;
      foreach (FileInfo fileInfo in files)
      {
        try
        {
          ulong length = (ulong) fileInfo.Length;
          num += length;
        }
        catch
        {
        }
      }
      return num >= this.CapacityInBytes;
    }

    private IEnumerable<FileInfo> GetFiles(string filter)
    {
      IEnumerable<FileInfo> source = (IEnumerable<FileInfo>) new List<FileInfo>();
      try
      {
        if (this.StorageFolder != null)
        {
          source = (IEnumerable<FileInfo>) this.StorageFolder.GetFiles(filter, SearchOption.TopDirectoryOnly);
          return (IEnumerable<FileInfo>) source.OrderBy<FileInfo, string>((Func<FileInfo, string>) (fileInfo => fileInfo.Name));
        }
      }
      catch (Exception ex)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Peek failed while getting files from storage. Exception: " + ex?.ToString());
        CoreEventSource.Log.LogVerbose(message);
      }
      return source;
    }

    private void DeleteObsoleteFiles()
    {
      try
      {
        foreach (FileInfo file in this.GetFiles("*.tmp"))
        {
          if (DateTime.UtcNow - file.CreationTimeUtc >= TimeSpan.FromMinutes(5.0))
            ReparsePointAware.DeleteFile(file.FullName);
        }
      }
      catch (Exception ex)
      {
        CoreEventSource.Log.LogVerbose("Failed to delete tmp files. Exception: " + ex?.ToString());
      }
    }

    protected abstract bool CanDelete(FileInfo fileInfo);
  }
}
