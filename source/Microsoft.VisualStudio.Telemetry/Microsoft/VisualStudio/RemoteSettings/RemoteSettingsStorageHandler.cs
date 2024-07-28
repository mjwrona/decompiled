// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.RemoteSettingsStorageHandler
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class RemoteSettingsStorageHandler : 
    IVersionedRemoteSettingsStorageHandler,
    IRemoteSettingsStorageHandler,
    ISettingsCollection,
    IScopesStorageHandler
  {
    private readonly ICollectionKeyValueStorage remoteSettingsStorage;
    private readonly IScopeParserFactory scopeParserFactory;
    private readonly IRemoteSettingsLogger logger;
    private readonly bool isVersioned;
    private const string FileVersionKey = "FileVersion";
    private const string SettingsVersionKey = "SettingsVersion";
    private const string Separator = ":";
    private const string MultipleValueIndicator = "*";
    internal const int SettingsVersion = 2;
    internal readonly string CollectionPathPrefix;

    public RemoteSettingsStorageHandler(
      ICollectionKeyValueStorage storage,
      string collectionPathPrefix,
      IScopeParserFactory scopeParserFactory,
      bool isVersioned,
      IRemoteSettingsLogger logger)
    {
      storage.RequiresArgumentNotNull<ICollectionKeyValueStorage>(nameof (storage));
      collectionPathPrefix.RequiresArgumentNotNull<string>(nameof (collectionPathPrefix));
      scopeParserFactory.RequiresArgumentNotNull<IScopeParserFactory>(nameof (scopeParserFactory));
      this.remoteSettingsStorage = storage;
      this.CollectionPathPrefix = collectionPathPrefix;
      this.isVersioned = isVersioned;
      this.scopeParserFactory = scopeParserFactory;
      this.logger = logger;
    }

    public string FileVersion
    {
      get => this.remoteSettingsStorage.GetValue<string>(this.CollectionPathPrefix, nameof (FileVersion), string.Empty);
      set => this.remoteSettingsStorage.SetValue<string>(this.CollectionPathPrefix, nameof (FileVersion), value);
    }

    public int StoredSettingsVersion
    {
      get => this.remoteSettingsStorage.GetValue<int>(this.CollectionPathPrefix, "SettingsVersion", 0);
      set => this.remoteSettingsStorage.SetValue<int>(this.CollectionPathPrefix, "SettingsVersion", value);
    }

    private string CurrentCollectionPath
    {
      get
      {
        if (this.isVersioned)
        {
          string fileVersion = this.FileVersion;
          if (!string.IsNullOrEmpty(fileVersion))
            return Path.Combine(this.CollectionPathPrefix, fileVersion);
        }
        return this.CollectionPathPrefix;
      }
    }

    public IEnumerable<string> GetSubCollectionNames(string collectionPath) => this.remoteSettingsStorage.GetSubCollectionNames(Path.Combine(this.CurrentCollectionPath, collectionPath)).Where<string>((Func<string, bool>) (x => !x.EndsWith("*")));

    public bool CollectionExists(string collectionPath) => this.remoteSettingsStorage.CollectionExists(Path.Combine(this.CurrentCollectionPath, collectionPath));

    public bool PropertyExists(string collectionPath, string propertyName)
    {
      string str = Path.Combine(this.CurrentCollectionPath, collectionPath);
      return this.remoteSettingsStorage.PropertyExists(str, propertyName) || this.remoteSettingsStorage.CollectionExists(Path.Combine(str, propertyName + "*"));
    }

    public IEnumerable<string> GetPropertyNames(string collectionPath)
    {
      string collectionPath1 = Path.Combine(this.CurrentCollectionPath, collectionPath);
      return this.remoteSettingsStorage.GetPropertyNames(collectionPath1).Union<string>(this.remoteSettingsStorage.GetSubCollectionNames(collectionPath1).Where<string>((Func<string, bool>) (x => x.EndsWith("*"))).Select<string, string>((Func<string, string>) (x => x.Substring(0, x.Length - 1))));
    }

    public async Task<RemoteSettingsProviderResult<T>> TryGetValueAsync<T>(
      string collectionPath,
      string key)
    {
      string setting = collectionPath + " $" + key;
      foreach (RemoteSettingsStorageHandler.SplitKey remoteSettingKey in this.GetPossibleRemoteSettingKeys(collectionPath, key))
      {
        RemoteSettingsStorageHandler.SplitKey possibleKey = remoteSettingKey;
        if (possibleKey.RemoteSettingName == key)
        {
          bool flag1 = possibleKey.IsScoped;
          if (flag1)
            flag1 = !await this.EvaluateScopedSettingAsync(new LoggingContext<string>(setting, possibleKey.ScopeString)).ConfigureAwait(false);
          if (!flag1)
          {
            T obj;
            bool flag2 = this.remoteSettingsStorage.TryGetValue<T>(possibleKey.StorageCollectionPath, possibleKey.StorageKey, out obj);
            return new RemoteSettingsProviderResult<T>()
            {
              RetrievalSuccessful = flag2,
              Value = obj
            };
          }
        }
        else
          possibleKey = (RemoteSettingsStorageHandler.SplitKey) null;
      }
      return (RemoteSettingsProviderResult<T>) null;
    }

    public bool TryGetValue<T>(string collectionPath, string key, out T value)
    {
      string context = collectionPath + " $" + key;
      foreach (RemoteSettingsStorageHandler.SplitKey remoteSettingKey in this.GetPossibleRemoteSettingKeys(collectionPath, key))
      {
        if (remoteSettingKey.RemoteSettingName == key && (!remoteSettingKey.IsScoped || this.EvaluateScopedSetting(new LoggingContext<string>(context, remoteSettingKey.ScopeString))))
          return this.remoteSettingsStorage.TryGetValue<T>(remoteSettingKey.StorageCollectionPath, remoteSettingKey.StorageKey, out value);
      }
      value = default (T);
      return false;
    }

    public bool TryGetValueKind(string collectionPath, string key, out ValueKind kind)
    {
      string str = Path.Combine(this.CurrentCollectionPath, collectionPath);
      string fullCollectionPathWithKey = Path.Combine(str, key + "*");
      kind = ValueKind.Unknown;
      if (this.remoteSettingsStorage.CollectionExists(fullCollectionPathWithKey))
      {
        IEnumerable<string> propertyNames = this.remoteSettingsStorage.GetPropertyNames(fullCollectionPathWithKey);
        if (propertyNames.Count<string>() != 0)
        {
          ValueKind firstKind;
          ValueKind currentKind;
          if (this.remoteSettingsStorage.TryGetValueKind(fullCollectionPathWithKey, propertyNames.First<string>(), out firstKind) && propertyNames.All<string>((Func<string, bool>) (x => this.remoteSettingsStorage.TryGetValueKind(fullCollectionPathWithKey, x, out currentKind) && currentKind == firstKind)))
            kind = firstKind;
          return true;
        }
      }
      return this.remoteSettingsStorage.TryGetValueKind(str, key, out kind);
    }

    public bool DoSettingsNeedToBeUpdated(string newFileVersion) => !this.FileVersion.Equals(newFileVersion, StringComparison.InvariantCultureIgnoreCase) || this.StoredSettingsVersion != 2 || !this.remoteSettingsStorage.CollectionExists(this.CurrentCollectionPath);

    public void DeleteSettingsForFileVersion(string fileVersion)
    {
      string collectionPath = Path.Combine(this.CollectionPathPrefix, fileVersion);
      if (!this.remoteSettingsStorage.CollectionExists(collectionPath))
        return;
      this.remoteSettingsStorage.DeleteCollection(collectionPath);
    }

    public void SaveNonScopedSetting(RemoteSetting setting)
    {
      if (setting.HasScope)
        throw new InvalidOperationException("Cannot save setting that has scope");
      this.remoteSettingsStorage.SetValue<object>(setting.Path, setting.Name, setting.Value);
    }

    public void SaveSettings(GroupedRemoteSettings remoteSettings) => this.SaveSettingsInternal(this.CollectionPathPrefix, remoteSettings);

    public void SaveNonScopedSettings(GroupedRemoteSettings groupedSettings)
    {
      foreach (KeyValuePair<string, RemoteSettingPossibilities> groupedSetting in (Dictionary<string, RemoteSettingPossibilities>) groupedSettings)
      {
        foreach (KeyValuePair<string, List<RemoteSetting>> keyValuePair in (Dictionary<string, List<RemoteSetting>>) groupedSetting.Value)
        {
          foreach (RemoteSetting remoteSetting in keyValuePair.Value)
          {
            if (!remoteSetting.HasScope)
              this.remoteSettingsStorage.SetValue<object>(remoteSetting.Path, remoteSetting.Name, remoteSetting.Value);
          }
        }
      }
    }

    public void SaveSettings(VersionedDeserializedRemoteSettings remoteSettings)
    {
      string str = Path.Combine(this.CollectionPathPrefix, remoteSettings.FileVersion);
      this.SaveSettingsInternal(str, new GroupedRemoteSettings((DeserializedRemoteSettings) remoteSettings, (string) null));
      if (remoteSettings.Scopes != null)
      {
        foreach (Scope scope in remoteSettings.Scopes)
          this.remoteSettingsStorage.SetValue<string>(Path.Combine(str, "Scopes"), scope.Name, scope.ScopeString);
      }
      if (!this.remoteSettingsStorage.CollectionExists(str))
        return;
      this.FileVersion = remoteSettings.FileVersion;
      this.StoredSettingsVersion = 2;
    }

    public void CleanUpOldFileVersions(string newFileVersion)
    {
      foreach (string subCollectionName in this.remoteSettingsStorage.GetSubCollectionNames(this.CollectionPathPrefix))
      {
        if (subCollectionName != newFileVersion)
          this.remoteSettingsStorage.DeleteCollection(Path.Combine(this.CollectionPathPrefix, subCollectionName));
      }
    }

    public void DeleteAllSettings()
    {
      foreach (string subCollectionName in this.remoteSettingsStorage.GetSubCollectionNames(this.CollectionPathPrefix))
        this.remoteSettingsStorage.DeleteCollection(Path.Combine(this.CollectionPathPrefix, subCollectionName));
      foreach (string propertyName in this.remoteSettingsStorage.GetPropertyNames(this.CollectionPathPrefix))
        this.remoteSettingsStorage.DeleteProperty(this.CollectionPathPrefix, propertyName);
    }

    public void InvalidateFileVersion() => this.remoteSettingsStorage.DeleteProperty(this.CollectionPathPrefix, "FileVersion");

    public IEnumerable<string> GetAllScopes() => this.remoteSettingsStorage.GetPropertyNames(Path.Combine(this.CurrentCollectionPath, "Scopes"));

    public string GetScope(string scopeName) => this.remoteSettingsStorage.GetValue<string>(Path.Combine(this.CurrentCollectionPath, "Scopes"), scopeName, (string) null);

    private IEnumerable<RemoteSettingsStorageHandler.SplitKey> GetPossibleRemoteSettingKeys(
      string collectionPath,
      string key)
    {
      string str = collectionPath + " $" + key;
      string fullCollectionPath = Path.Combine(this.CurrentCollectionPath, collectionPath);
      string fullCollectionPathWithKey = Path.Combine(fullCollectionPath, key + "*");
      if (this.remoteSettingsStorage.CollectionExists(fullCollectionPathWithKey))
      {
        this.logger.LogVerbose(str + " has multiple possible values");
        foreach (string storageName in (IEnumerable<string>) this.remoteSettingsStorage.GetPropertyNames(fullCollectionPathWithKey).OrderBy<string, string>((Func<string, string>) (s => s)))
          yield return RemoteSettingsStorageHandler.SplitKey.CreateFromStorageName(fullCollectionPathWithKey, storageName);
      }
      yield return new RemoteSettingsStorageHandler.SplitKey(fullCollectionPath, key);
    }

    private async Task<bool> EvaluateScopedSettingAsync(LoggingContext<string> context)
    {
      bool async = await this.scopeParserFactory.EvaluateAsync(context.Value);
      this.logger.LogVerbose(string.Format("Evaluating scope {0} for setting {1}, result: {2}", (object) context.Value, (object) context.Context, (object) async));
      return async;
    }

    private bool EvaluateScopedSetting(LoggingContext<string> context)
    {
      bool scopedSetting = this.scopeParserFactory.Evaluate(context.Value);
      this.logger.LogVerbose(string.Format("Evaluating scope {0} for setting {1}, result: {2}", (object) context.Value, (object) context.Context, (object) scopedSetting));
      return scopedSetting;
    }

    private void SaveSettingsInternal(
      string newCollectionPath,
      GroupedRemoteSettings groupedSettings)
    {
      foreach (KeyValuePair<string, RemoteSettingPossibilities> groupedSetting in (Dictionary<string, RemoteSettingPossibilities>) groupedSettings)
      {
        string str1 = Path.Combine(newCollectionPath, groupedSetting.Key);
        foreach (KeyValuePair<string, List<RemoteSetting>> keyValuePair in (Dictionary<string, List<RemoteSetting>>) groupedSetting.Value)
        {
          bool flag = false;
          string str2 = str1;
          if (keyValuePair.Value.Count > 1 || keyValuePair.Value[0].HasScope)
          {
            flag = true;
            str2 = Path.Combine(str2, keyValuePair.Key + "*");
          }
          for (int index = 0; index < keyValuePair.Value.Count; ++index)
          {
            RemoteSetting remoteSetting = keyValuePair.Value[index];
            string key = remoteSetting.Name;
            if (flag)
              key = index.ToString() + ":" + key;
            if (remoteSetting.HasScope)
              key = key + ":" + remoteSetting.ScopeString;
            this.remoteSettingsStorage.SetValue<object>(str2, key, remoteSetting.Value);
          }
        }
      }
    }

    internal class SplitKey
    {
      public string StorageCollectionPath { get; }

      public string StorageKey { get; }

      public string RemoteSettingName { get; }

      public string ScopeString { get; }

      public bool IsScoped => this.ScopeString != null;

      public SplitKey(string collectionPath, string storageKey)
        : this(collectionPath, storageKey, storageKey, (string) null)
      {
      }

      private SplitKey(
        string collectionPath,
        string storageKey,
        string remoteSettingName,
        string scopeString)
      {
        this.StorageCollectionPath = collectionPath;
        this.StorageKey = storageKey;
        this.RemoteSettingName = remoteSettingName;
        this.ScopeString = scopeString;
      }

      public static RemoteSettingsStorageHandler.SplitKey CreateFromStorageName(
        string collectionPath,
        string storageName)
      {
        int num1 = storageName.IndexOf(':');
        if (num1 == -1)
          return new RemoteSettingsStorageHandler.SplitKey(collectionPath, storageName);
        int num2 = storageName.IndexOf(':', num1 + 1);
        return num2 == -1 ? new RemoteSettingsStorageHandler.SplitKey(collectionPath, storageName, storageName.Substring(num1 + 1), (string) null) : new RemoteSettingsStorageHandler.SplitKey(collectionPath, storageName, storageName.Substring(num1 + 1, num2 - num1 - 1), storageName.Substring(num2 + 1));
      }
    }
  }
}
