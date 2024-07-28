// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ClientStorage.VssFileStorage
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Common.ClientStorage
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class VssFileStorage : 
    IVssClientStorage,
    IVssClientStorageReader,
    IVssClientStorageWriter,
    IDisposable
  {
    private readonly string m_filePath;
    private readonly VssFileStorage.VssFileStorageReader m_reader;
    private readonly IVssClientStorageWriter m_writer;
    private const char c_defaultPathSeparator = '\\';
    private const bool c_defaultIgnoreCaseInPaths = false;
    private static ConcurrentDictionary<string, VssFileStorage> s_storages = new ConcurrentDictionary<string, VssFileStorage>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public char PathSeparator { get; }

    public StringComparer PathComparer { get; }

    private VssFileStorage(string filePath, char pathSeparatorForKeys = '\\', bool ignoreCaseInPaths = false)
    {
      this.PathSeparator = pathSeparatorForKeys;
      this.PathComparer = VssFileStorage.GetAppropriateStringComparer(ignoreCaseInPaths);
      this.m_filePath = filePath;
      this.m_reader = new VssFileStorage.VssFileStorageReader(this.m_filePath, pathSeparatorForKeys, this.PathComparer);
      this.m_writer = (IVssClientStorageWriter) new VssFileStorage.VssFileStorageWriter(this.m_filePath, pathSeparatorForKeys, this.PathComparer);
    }

    public T ReadEntry<T>(string path) => this.m_reader.ReadEntry<T>(path);

    public T ReadEntry<T>(string path, T defaultValue) => this.m_reader.ReadEntry<T>(path, defaultValue);

    public IDictionary<string, T> ReadEntries<T>(string pathPrefix) => this.m_reader.ReadEntries<T>(pathPrefix);

    public void WriteEntries(IEnumerable<KeyValuePair<string, object>> entries)
    {
      this.m_writer.WriteEntries(entries);
      this.m_reader.NotifyChanged();
    }

    public void WriteEntry(string key, object value)
    {
      this.m_writer.WriteEntry(key, value);
      this.m_reader.NotifyChanged();
    }

    public void Dispose() => this.m_reader.Dispose();

    public string PathKeyCombine(params string[] paths)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string path in paths)
      {
        if (path != null)
        {
          string str = path.TrimEnd(this.PathSeparator);
          if (str.Length > 0)
          {
            if (stringBuilder.Length > 0)
              stringBuilder.Append(this.PathSeparator);
            stringBuilder.Append(str);
          }
        }
      }
      return stringBuilder.ToString();
    }

    public static IVssClientStorage GetVssLocalFileStorage(
      string fullPath,
      char pathSeparatorForKeys = '\\',
      bool ignoreCaseInPaths = false)
    {
      string fullPath1 = Path.GetFullPath(fullPath);
      VssFileStorage orAdd = VssFileStorage.s_storages.GetOrAdd(fullPath1, (Func<string, VssFileStorage>) (key => new VssFileStorage(key, pathSeparatorForKeys, ignoreCaseInPaths)));
      if ((int) orAdd.PathSeparator != (int) pathSeparatorForKeys)
        throw new ArgumentException(CommonResources.ConflictingPathSeparatorForVssFileStorage((object) pathSeparatorForKeys, (object) fullPath1, (object) orAdd.PathSeparator));
      StringComparer appropriateStringComparer = VssFileStorage.GetAppropriateStringComparer(ignoreCaseInPaths);
      if (orAdd.PathComparer != appropriateStringComparer)
      {
        string str1 = "Ordinal";
        string str2 = "OrdinalIgnoreCase";
        string str3 = ignoreCaseInPaths ? str2 : str1;
        string str4 = ignoreCaseInPaths ? str1 : str2;
        string str5 = fullPath1;
        string str6 = str4;
        throw new ArgumentException(CommonResources.ConflictingStringComparerForVssFileStorage((object) str3, (object) str5, (object) str6));
      }
      return (IVssClientStorage) orAdd;
    }

    private static StringComparer GetAppropriateStringComparer(bool ignoreCase) => !ignoreCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

    public static IVssClientStorage GetCurrentUserVssFileStorage(
      string pathSuffix,
      bool storeByVssVersion,
      char pathSeparatorForKeys = '\\',
      bool ignoreCaseInPaths = false)
    {
      return VssFileStorage.GetVssLocalFileStorage(Path.Combine(storeByVssVersion ? VssFileStorage.ClientSettingsDirectoryByVersion : VssFileStorage.ClientSettingsDirectory, pathSuffix), pathSeparatorForKeys, ignoreCaseInPaths);
    }

    internal static string ClientSettingsDirectoryByVersion => Path.Combine(VssFileStorage.ClientSettingsDirectory, "v19.0");

    internal static string ClientSettingsDirectory
    {
      get
      {
        string path2 = "Microsoft\\VisualStudio Services";
        string path1 = Environment.GetEnvironmentVariable("localappdata");
        VssFileStorage.SafeGetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrEmpty(path1))
        {
          path1 = VssFileStorage.SafeGetFolderPath(Environment.SpecialFolder.ApplicationData);
          if (string.IsNullOrEmpty(path1))
          {
            path1 = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            path2 = "Microsoft VisualStudio Services";
          }
        }
        return Path.Combine(path1, path2);
      }
    }

    private static string SafeGetFolderPath(Environment.SpecialFolder specialFolder)
    {
      try
      {
        return Environment.GetFolderPath(specialFolder);
      }
      catch (ArgumentException ex)
      {
        return (string) null;
      }
    }

    private class VssFileStorageReader : 
      VssFileStorage.VssLocalFile,
      IVssClientStorageReader,
      IDisposable
    {
      private readonly string m_path;
      private Dictionary<string, JRaw> m_settings;
      private readonly FileSystemWatcher m_watcher;
      private readonly ReaderWriterLockSlim m_lock;
      private long m_completedRefreshId;
      private long m_outstandingRefreshId;

      public VssFileStorageReader(string fullPath, char pathSeparator, StringComparer comparer)
        : base(fullPath, pathSeparator, comparer)
      {
        this.m_path = fullPath;
        this.m_lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        this.m_completedRefreshId = 0L;
        this.m_outstandingRefreshId = 1L;
        string directoryName = Path.GetDirectoryName(this.m_path);
        if (!Directory.Exists(directoryName))
          Directory.CreateDirectory(directoryName);
        this.m_watcher = new FileSystemWatcher(directoryName, Path.GetFileName(this.m_path));
        this.m_watcher.IncludeSubdirectories = false;
        this.m_watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
        this.m_watcher.Changed += new FileSystemEventHandler(this.OnCacheFileChanged);
        this.m_watcher.EnableRaisingEvents = true;
      }

      public T ReadEntry<T>(string path) => this.ReadEntry<T>(path, default (T));

      public T ReadEntry<T>(string path, T defaultValue)
      {
        path = this.NormalizePath(path);
        this.RefreshIfNeeded();
        JRaw jraw;
        return this.m_settings.TryGetValue(path, out jraw) && jraw != null ? JsonConvert.DeserializeObject<T>(jraw.ToString()) : defaultValue;
      }

      public IDictionary<string, T> ReadEntries<T>(string pathPrefix)
      {
        string prefix = this.NormalizePath(pathPrefix, true);
        this.RefreshIfNeeded();
        Dictionary<string, JRaw> settings = this.m_settings;
        Dictionary<string, T> dictionary = new Dictionary<string, T>();
        Func<KeyValuePair<string, JRaw>, bool> predicate = (Func<KeyValuePair<string, JRaw>, bool>) (kvp => kvp.Key == prefix || kvp.Key.StartsWith(prefix + this.PathSeparator.ToString()));
        foreach (KeyValuePair<string, JRaw> keyValuePair in settings.Where<KeyValuePair<string, JRaw>>(predicate))
        {
          try
          {
            dictionary[keyValuePair.Key] = JsonConvert.DeserializeObject<T>(keyValuePair.Value.ToString());
          }
          catch (JsonSerializationException ex)
          {
          }
          catch (JsonReaderException ex)
          {
          }
        }
        return (IDictionary<string, T>) dictionary;
      }

      private void OnCacheFileChanged(object sender, FileSystemEventArgs e) => this.NotifyChanged();

      public void Dispose()
      {
        this.m_watcher.Dispose();
        this.m_lock.Dispose();
      }

      public void NotifyChanged()
      {
        using (new VssFileStorage.VssFileStorageReader.ReadLockScope(this.m_lock))
          Interlocked.Increment(ref this.m_outstandingRefreshId);
      }

      private void RefreshIfNeeded()
      {
        VssFileStorage.VssFileStorageReader.ReadLockScope readLockScope = new VssFileStorage.VssFileStorageReader.ReadLockScope(this.m_lock);
        long num;
        try
        {
          num = Interlocked.Read(ref this.m_outstandingRefreshId);
          if (this.m_completedRefreshId >= num)
            return;
        }
        finally
        {
          readLockScope.Dispose();
        }
        Dictionary<string, JRaw> dictionary;
        using (this.GetNewMutexScope())
        {
          if (this.m_completedRefreshId >= num)
            return;
          dictionary = this.LoadFile();
        }
        readLockScope = new VssFileStorage.VssFileStorageReader.ReadLockScope(this.m_lock);
        try
        {
          if (this.m_completedRefreshId >= num)
            return;
        }
        finally
        {
          readLockScope.Dispose();
        }
        using (new VssFileStorage.VssFileStorageReader.WriteLockScope(this.m_lock))
        {
          if (this.m_completedRefreshId >= num)
            return;
          this.m_completedRefreshId = num;
          this.m_settings = dictionary;
        }
      }

      private struct ReadLockScope : IDisposable
      {
        private readonly ReaderWriterLockSlim m_lock;

        public ReadLockScope(ReaderWriterLockSlim @lock)
        {
          this.m_lock = @lock;
          this.m_lock.EnterReadLock();
        }

        public void Dispose() => this.m_lock.ExitReadLock();
      }

      private struct WriteLockScope : IDisposable
      {
        private readonly ReaderWriterLockSlim m_lock;

        public WriteLockScope(ReaderWriterLockSlim @lock)
        {
          this.m_lock = @lock;
          this.m_lock.EnterWriteLock();
        }

        public void Dispose() => this.m_lock.ExitWriteLock();
      }
    }

    private class VssFileStorageWriter : VssFileStorage.VssLocalFile, IVssClientStorageWriter
    {
      public VssFileStorageWriter(string fullPath, char pathSeparator, StringComparer comparer)
        : base(fullPath, pathSeparator, comparer)
      {
      }

      public void WriteEntries(IEnumerable<KeyValuePair<string, object>> entries)
      {
        if (!entries.Any<KeyValuePair<string, object>>())
          return;
        using (this.GetNewMutexScope())
        {
          bool flag = false;
          Dictionary<string, JRaw> dictionary1 = this.LoadFile();
          Dictionary<string, JRaw> dictionary2 = new Dictionary<string, JRaw>((IEqualityComparer<string>) this.PathComparer);
          if (dictionary1.Any<KeyValuePair<string, JRaw>>())
            dictionary1.Copy<string, JRaw>((IDictionary<string, JRaw>) dictionary2);
          foreach (KeyValuePair<string, object> entry in entries)
          {
            string key = this.NormalizePath(entry.Key);
            if (entry.Value != null)
            {
              JRaw other = new JRaw((object) JsonConvert.SerializeObject(entry.Value));
              if (!dictionary2.ContainsKey(key) || !dictionary2[key].Equals((JValue) other))
              {
                dictionary2[key] = other;
                flag = true;
              }
            }
            else if (dictionary2.Remove(key))
              flag = true;
          }
          if (!flag)
            return;
          this.SaveFile((IDictionary<string, JRaw>) dictionary1, (IDictionary<string, JRaw>) dictionary2);
        }
      }

      public void WriteEntry(string path, object value) => this.WriteEntries((IEnumerable<KeyValuePair<string, object>>) new KeyValuePair<string, object>[1]
      {
        new KeyValuePair<string, object>(path, value)
      });
    }

    private class VssLocalFile
    {
      private readonly string m_filePath;
      private readonly string m_bckUpFilePath;
      private readonly string m_emptyPathSegment;

      public VssLocalFile(string filePath, char pathSeparator, StringComparer comparer)
      {
        this.m_filePath = filePath;
        this.PathComparer = comparer;
        this.PathSeparator = pathSeparator;
        this.m_emptyPathSegment = new string(pathSeparator, 2);
        FileInfo fileInfo = new FileInfo(this.m_filePath);
        this.m_bckUpFilePath = Path.Combine(fileInfo.Directory.FullName, "~" + fileInfo.Name);
      }

      protected char PathSeparator { get; }

      protected string NormalizePath(string path, bool allowRootPath = false)
      {
        if (string.IsNullOrEmpty(path) || (int) path[0] != (int) this.PathSeparator || path.IndexOf(this.m_emptyPathSegment, StringComparison.Ordinal) >= 0 || !allowRootPath && path.Length == 1)
          throw new ArgumentException(CommonResources.InvalidClientStoragePath((object) path, (object) this.PathSeparator), nameof (path));
        if ((int) path[path.Length - 1] == (int) this.PathSeparator)
          path = path.Substring(0, path.Length - 1);
        return path;
      }

      protected StringComparer PathComparer { get; }

      protected Dictionary<string, JRaw> LoadFile()
      {
        Dictionary<string, JRaw> source = (Dictionary<string, JRaw>) null;
        if (File.Exists(this.m_filePath))
          source = this.LoadFile(this.m_filePath);
        if ((source == null || !source.Any<KeyValuePair<string, JRaw>>()) && File.Exists(this.m_bckUpFilePath))
          source = this.LoadFile(this.m_bckUpFilePath);
        return source ?? new Dictionary<string, JRaw>((IEqualityComparer<string>) this.PathComparer);
      }

      private Dictionary<string, JRaw> LoadFile(string path)
      {
        Dictionary<string, JRaw> dictionary = new Dictionary<string, JRaw>((IEqualityComparer<string>) this.PathComparer);
        try
        {
          string end;
          using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
          {
            using (StreamReader streamReader = new StreamReader((Stream) fileStream, Encoding.UTF8))
              end = streamReader.ReadToEnd();
          }
          IReadOnlyDictionary<string, JRaw> readOnlyDictionary = JsonConvert.DeserializeObject<IReadOnlyDictionary<string, JRaw>>(end);
          if (readOnlyDictionary != null)
          {
            foreach (KeyValuePair<string, JRaw> keyValuePair in (IEnumerable<KeyValuePair<string, JRaw>>) readOnlyDictionary)
              dictionary[keyValuePair.Key] = keyValuePair.Value;
          }
        }
        catch (DirectoryNotFoundException ex)
        {
        }
        catch (FileNotFoundException ex)
        {
        }
        catch (JsonReaderException ex)
        {
        }
        catch (JsonSerializationException ex)
        {
        }
        catch (InvalidCastException ex)
        {
        }
        return dictionary;
      }

      protected void SaveFile(
        IDictionary<string, JRaw> originalSettings,
        IDictionary<string, JRaw> newSettings)
      {
        string content = JToken.Parse(JsonConvert.SerializeObject((object) newSettings)).ToString(Formatting.Indented);
        if (originalSettings.Any<KeyValuePair<string, JRaw>>())
          this.SaveFile(this.m_bckUpFilePath, JToken.Parse(JsonConvert.SerializeObject((object) originalSettings)).ToString(Formatting.Indented));
        this.SaveFile(this.m_filePath, content);
        if (!File.Exists(this.m_bckUpFilePath))
          return;
        File.Delete(this.m_bckUpFilePath);
      }

      private void SaveFile(string path, string content)
      {
        bool flag = false;
        int num = 0;
        int millisecondsDelay = 10;
        do
        {
          try
          {
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Delete))
            {
              using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream, Encoding.UTF8))
                streamWriter.Write(content);
            }
            flag = true;
          }
          catch (IOException ex)
          {
            if (++num > 6)
            {
              throw;
            }
            else
            {
              Task.Delay(millisecondsDelay).Wait();
              millisecondsDelay *= 2;
            }
          }
        }
        while (!flag);
      }

      protected VssFileStorage.VssLocalFile.MutexScope GetNewMutexScope() => new VssFileStorage.VssLocalFile.MutexScope(this.m_filePath.Replace(Path.DirectorySeparatorChar, '_'));

      protected struct MutexScope : IDisposable
      {
        private readonly Mutex m_mutex;
        private static readonly TimeSpan s_mutexTimeout = TimeSpan.FromSeconds(10.0);

        public MutexScope(string name)
        {
          this.m_mutex = new Mutex(false, name);
          try
          {
            if (!this.m_mutex.WaitOne(VssFileStorage.VssLocalFile.MutexScope.s_mutexTimeout))
              throw new TimeoutException();
          }
          catch (AbandonedMutexException ex)
          {
          }
        }

        public void Dispose() => this.m_mutex.ReleaseMutex();
      }
    }
  }
}
