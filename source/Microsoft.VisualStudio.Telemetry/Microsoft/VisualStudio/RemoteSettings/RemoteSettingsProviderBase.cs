// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.RemoteSettingsProviderBase
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal abstract class RemoteSettingsProviderBase : 
    TelemetryDisposableObject,
    IRemoteSettingsProvider,
    ISettingsCollection,
    IDisposable
  {
    protected IRemoteSettingsStorageHandler currentStorageHandler;
    protected Task<GroupedRemoteSettings> startTask = Task.FromResult<GroupedRemoteSettings>((GroupedRemoteSettings) null);
    protected IRemoteSettingsLogger logger;

    public abstract string Name { get; }

    public RemoteSettingsProviderBase(
      IRemoteSettingsStorageHandler remoteSettingsStorageHandler,
      IRemoteSettingsLogger logger = null)
    {
      this.currentStorageHandler = remoteSettingsStorageHandler;
      this.logger = logger;
    }

    public bool TryGetValueKind(string collectionPath, string key, out ValueKind kind) => this.currentStorageHandler.TryGetValueKind(collectionPath, key, out kind);

    public IEnumerable<string> GetPropertyNames(string collectionPath) => this.currentStorageHandler.GetPropertyNames(collectionPath);

    public IEnumerable<string> GetSubCollectionNames(string collectionPath) => this.currentStorageHandler.GetSubCollectionNames(collectionPath);

    public bool CollectionExists(string collectionPath) => this.currentStorageHandler.CollectionExists(collectionPath);

    public bool PropertyExists(string collectionPath, string propertyName) => this.currentStorageHandler.PropertyExists(collectionPath, propertyName);

    public async Task<RemoteSettingsProviderResult<T>> TryGetValueAsync<T>(
      string collectionPath,
      string key)
    {
      RemoteSettingsProviderBase settingsProviderBase = this;
      settingsProviderBase.RequiresNotDisposed();
      GroupedRemoteSettings groupedRemoteSettings = await settingsProviderBase.startTask.ConfigureAwait(false);
      return await settingsProviderBase.currentStorageHandler.TryGetValueAsync<T>(collectionPath, key).ConfigureAwait(false);
    }

    public virtual async Task<IEnumerable<ActionWrapper<T>>> GetActionsAsync<T>(string actionPath)
    {
      RemoteSettingsProviderBase settingsProviderBase = this;
      settingsProviderBase.RequiresNotDisposed();
      GroupedRemoteSettings groupedRemoteSettings = await settingsProviderBase.startTask.ConfigureAwait(false);
      return await Task.FromResult<IEnumerable<ActionWrapper<T>>>(Enumerable.Empty<ActionWrapper<T>>());
    }

    public virtual async Task SubscribeActionsAsync<T>(
      string actionPath,
      Action<ActionWrapper<T>> callback)
    {
      RemoteSettingsProviderBase settingsProviderBase = this;
      if (settingsProviderBase.IsDisposed)
        return;
      GroupedRemoteSettings groupedRemoteSettings = await settingsProviderBase.startTask.ConfigureAwait(false);
    }

    public virtual void UnsubscribeActions(string actionPath) => this.RequiresNotDisposed();

    public bool TryGetValue<T>(string collectionPath, string key, out T value)
    {
      this.RequiresNotDisposed();
      return this.currentStorageHandler.TryGetValue<T>(collectionPath, key, out value);
    }

    public abstract Task<GroupedRemoteSettings> Start();
  }
}
