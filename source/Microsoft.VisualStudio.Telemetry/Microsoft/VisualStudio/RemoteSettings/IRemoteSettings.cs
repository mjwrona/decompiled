// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.IRemoteSettings
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  public interface IRemoteSettings : IDisposable
  {
    event EventHandler SettingsUpdated;

    T GetValue<T>(string collectionPath, string key, T defaultValue);

    bool TryGetValue<T>(string collectionPath, string key, out T value);

    Task<T> GetValueAsync<T>(string collectionPath, string key, T defaultValue);

    ValueKind GetValueKind(string collectionPath, string key);

    Task<IEnumerable<ActionWrapper<T>>> GetActionsAsync<T>(string actionPath);

    void Start();

    IRemoteSettings RegisterFilterProvider(IScopeFilterProvider scopeFilterProvider);

    IEnumerable<string> GetPropertyNames(string collectionPath);

    IEnumerable<string> GetSubCollectionNames(string collectionPath);

    bool CollectionExists(string collectionPath);

    bool PropertyExists(string collectionPath, string key);
  }
}
