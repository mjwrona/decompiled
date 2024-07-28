// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.RemoteSettings
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  public class RemoteSettings : 
    TelemetryDisposableObject,
    IRemoteSettings,
    IDisposable,
    IRemoteSettings2
  {
    private static readonly Lazy<Microsoft.VisualStudio.RemoteSettings.RemoteSettings> defaultRemoteSettings = new Lazy<Microsoft.VisualStudio.RemoteSettings.RemoteSettings>((Func<Microsoft.VisualStudio.RemoteSettings.RemoteSettings>) (() => new Microsoft.VisualStudio.RemoteSettings.RemoteSettings(RemoteSettingsInitializer.BuildDefault())));
    private readonly IStableRemoteSettingsProvider stableRemoteSettingProvider;
    private readonly List<IRemoteSettingsProvider> remoteSettingsProviders;
    private readonly IScopeParserFactory scopeParserFactory;
    private readonly IRemoteSettingsStorageHandler nonScopedStorageHandler;
    private readonly Func<bool> isUpdateDisabled;
    private readonly IRemoteSettingsLogger logger;
    private bool isStarted;
    internal Task StartTask;

    public event EventHandler SettingsUpdated;

    internal IEnumerable<IRemoteSettingsProvider> AllRemoteSettingsProviders
    {
      get
      {
        yield return (IRemoteSettingsProvider) this.stableRemoteSettingProvider;
        foreach (IRemoteSettingsProvider settingsProvider in this.remoteSettingsProviders)
          yield return settingsProvider;
      }
    }

    public RemoteSettings(RemoteSettingsInitializer initializer)
    {
      initializer.FillWithDefaults();
      this.scopeParserFactory = initializer.ScopeParserFactory;
      this.nonScopedStorageHandler = initializer.NonScopedRemoteSettingsStorageHandler;
      foreach (IScopeFilterProvider scopeFilterProvider in initializer.ScopeFilterProviders)
        this.RegisterFilterProvider(scopeFilterProvider);
      this.remoteSettingsProviders = initializer.RemoteSettingsProviders.Select<Func<RemoteSettingsInitializer, IRemoteSettingsProvider>, IRemoteSettingsProvider>((Func<Func<RemoteSettingsInitializer, IRemoteSettingsProvider>, IRemoteSettingsProvider>) (x => x(initializer))).ToList<IRemoteSettingsProvider>();
      this.stableRemoteSettingProvider = initializer.StableRemoteSettingsProvider(initializer);
      this.isUpdateDisabled = initializer.IsUpdatedDisabled;
      this.logger = initializer.RemoteSettingsLogger;
    }

    public static IRemoteSettings Default => (IRemoteSettings) Microsoft.VisualStudio.RemoteSettings.RemoteSettings.defaultRemoteSettings.Value;

    public T GetValue<T>(string collectionPath, string key, T defaultValue)
    {
      T obj;
      return this.TryGetValue<T>(collectionPath, key, out obj) ? obj : defaultValue;
    }

    public bool TryGetValue<T>(string collectionPath, string key, out T value)
    {
      this.RequiresNotDisposed();
      collectionPath.RequiresArgumentNotNull<string>(nameof (collectionPath));
      key.RequiresArgumentNotNull<string>(nameof (key));
      bool flag = this.stableRemoteSettingProvider.IsStable(collectionPath);
      if (flag && this.stableRemoteSettingProvider.TryGetValue<T>(collectionPath, key, out value))
        return true;
      foreach (IRemoteSettingsProvider settingsProvider in this.remoteSettingsProviders)
      {
        if (settingsProvider.TryGetValue<T>(collectionPath, key, out value))
        {
          if (flag)
            this.stableRemoteSettingProvider.MakeStable<T>(collectionPath, key, value);
          return true;
        }
      }
      value = default (T);
      return false;
    }

    public ValueKind GetValueKind(string collectionPath, string key)
    {
      this.RequiresNotDisposed();
      collectionPath.RequiresArgumentNotNull<string>(nameof (collectionPath));
      key.RequiresArgumentNotNull<string>(nameof (key));
      ValueKind valueKind = ValueKind.Unknown;
      if (this.AllRemoteSettingsProviders.Any<IRemoteSettingsProvider>())
      {
        bool isFirst = true;
        ValueKind firstKind = valueKind;
        ValueKind currentKind;
        if (this.AllRemoteSettingsProviders.All<IRemoteSettingsProvider>((Func<IRemoteSettingsProvider, bool>) (x =>
        {
          if (!x.TryGetValueKind(collectionPath, key, out currentKind))
            return true;
          if (isFirst)
          {
            firstKind = currentKind;
            isFirst = false;
          }
          return firstKind == currentKind;
        })))
          valueKind = firstKind;
      }
      return valueKind;
    }

    public async Task<T> GetValueAsync<T>(string collectionPath, string key, T defaultValue)
    {
      Microsoft.VisualStudio.RemoteSettings.RemoteSettings remoteSettings = this;
      remoteSettings.RequiresNotDisposed();
      remoteSettings.RequiresStarted();
      collectionPath.RequiresArgumentNotNull<string>(collectionPath);
      key.RequiresArgumentNotNull<string>(key);
      foreach (IRemoteSettingsProvider settingsProvider in remoteSettings.AllRemoteSettingsProviders)
      {
        RemoteSettingsProviderResult<T> settingsProviderResult = await settingsProvider.TryGetValueAsync<T>(collectionPath, key).ConfigureAwait(false);
        if (settingsProviderResult.RetrievalSuccessful)
          return settingsProviderResult.Value;
      }
      return defaultValue;
    }

    public async Task<IEnumerable<ActionWrapper<T>>> GetActionsAsync<T>(string actionPath)
    {
      Microsoft.VisualStudio.RemoteSettings.RemoteSettings remoteSettings = this;
      remoteSettings.RequiresNotDisposed();
      remoteSettings.RequiresStarted();
      actionPath.RequiresArgumentNotNull<string>(nameof (actionPath));
      IEnumerable<ActionWrapper<T>> actionsAsync = Enumerable.Empty<ActionWrapper<T>>();
      foreach (IRemoteSettingsProvider settingsProvider in remoteSettings.AllRemoteSettingsProviders)
      {
        IEnumerable<ActionWrapper<T>> first = actionsAsync;
        string actionPath1 = actionPath;
        actionsAsync = first.Concat<ActionWrapper<T>>(await settingsProvider.GetActionsAsync<T>(actionPath1).ConfigureAwait(false));
        first = (IEnumerable<ActionWrapper<T>>) null;
      }
      return actionsAsync;
    }

    public void SubscribeActions<T>(string actionPath, Action<ActionWrapper<T>> callback)
    {
      if (this.IsDisposed)
        return;
      this.RequiresStarted();
      actionPath.RequiresArgumentNotNull<string>(nameof (actionPath));
      callback.RequiresArgumentNotNull<Action<ActionWrapper<T>>>(nameof (callback));
      foreach (IRemoteSettingsProvider settingsProvider in this.AllRemoteSettingsProviders)
        settingsProvider.SubscribeActionsAsync<T>(actionPath, callback);
    }

    public void UnsubscribeActions(string actionPath)
    {
      this.RequiresNotDisposed();
      this.RequiresStarted();
      actionPath.RequiresArgumentNotNull<string>(nameof (actionPath));
      foreach (IRemoteSettingsProvider settingsProvider in this.AllRemoteSettingsProviders)
        settingsProvider.UnsubscribeActions(actionPath);
    }

    public void Start()
    {
      this.RequiresNotDisposed();
      if (this.isStarted)
        return;
      if (this.isUpdateDisabled())
      {
        this.StartTask = (Task) Task.FromResult<bool>(false);
      }
      else
      {
        List<Task<GroupedRemoteSettings>> tasks = new List<Task<GroupedRemoteSettings>>();
        foreach (IRemoteSettingsProvider settingsProvider in this.AllRemoteSettingsProviders)
          tasks.Add(settingsProvider.Start());
        this.StartTask = Task.Run((Func<Task>) (async () =>
        {
          await this.logger.Start().ConfigureAwait(false);
          IEnumerable<GroupedRemoteSettings> source = ((IEnumerable<GroupedRemoteSettings>) await Task.WhenAll<GroupedRemoteSettings>((IEnumerable<Task<GroupedRemoteSettings>>) tasks).ConfigureAwait(false)).Where<GroupedRemoteSettings>((Func<GroupedRemoteSettings, bool>) (x => x != null));
          if ((this.nonScopedStorageHandler != null || this.logger.LoggingEnabled) && source.Any<GroupedRemoteSettings>())
          {
            GroupedRemoteSettings groupedRemoteSettings = source.Reverse<GroupedRemoteSettings>().Aggregate<GroupedRemoteSettings>((Func<GroupedRemoteSettings, GroupedRemoteSettings, GroupedRemoteSettings>) ((a, b) =>
            {
              a.Merge(b, this.logger);
              return a;
            }));
            this.logger.LogVerbose("Merged settings", (object) groupedRemoteSettings);
            if (this.nonScopedStorageHandler != null)
            {
              using (Mutex mutex = new Mutex(false, "Global\\A7B8B64E-AEB3-4053-BC8C-C187F5320352"))
              {
                try
                {
                  mutex.WaitOne(-1, false);
                }
                catch (AbandonedMutexException ex)
                {
                }
                try
                {
                  this.nonScopedStorageHandler.DeleteAllSettings();
                  this.nonScopedStorageHandler.SaveNonScopedSettings(groupedRemoteSettings);
                }
                catch
                {
                }
                finally
                {
                  mutex.ReleaseMutex();
                }
              }
            }
          }
          this.OnRemoteSettingsApplied();
        }));
      }
      this.isStarted = true;
    }

    public IRemoteSettings RegisterFilterProvider(IScopeFilterProvider scopeFilterProvider)
    {
      scopeFilterProvider.RequiresArgumentNotNull<IScopeFilterProvider>(nameof (scopeFilterProvider));
      this.scopeParserFactory.ProvidedFilters[scopeFilterProvider.Name] = scopeFilterProvider;
      return (IRemoteSettings) this;
    }

    public IEnumerable<string> GetPropertyNames(string collectionPath)
    {
      collectionPath.RequiresArgumentNotNull<string>(nameof (collectionPath));
      return this.AllRemoteSettingsProviders.SelectMany<IRemoteSettingsProvider, string>((Func<IRemoteSettingsProvider, IEnumerable<string>>) (x => x.GetPropertyNames(collectionPath))).Distinct<string>();
    }

    public IEnumerable<string> GetSubCollectionNames(string collectionPath)
    {
      collectionPath.RequiresArgumentNotNull<string>(nameof (collectionPath));
      return this.AllRemoteSettingsProviders.SelectMany<IRemoteSettingsProvider, string>((Func<IRemoteSettingsProvider, IEnumerable<string>>) (x => x.GetSubCollectionNames(collectionPath))).Distinct<string>();
    }

    public bool CollectionExists(string collectionPath)
    {
      collectionPath.RequiresArgumentNotNull<string>(nameof (collectionPath));
      return this.AllRemoteSettingsProviders.Any<IRemoteSettingsProvider>((Func<IRemoteSettingsProvider, bool>) (x => x.CollectionExists(collectionPath)));
    }

    public bool PropertyExists(string collectionPath, string key)
    {
      collectionPath.RequiresArgumentNotNull<string>(collectionPath);
      key.RequiresArgumentNotNull<string>(key);
      return this.AllRemoteSettingsProviders.Any<IRemoteSettingsProvider>((Func<IRemoteSettingsProvider, bool>) (x => x.PropertyExists(collectionPath, key)));
    }

    protected override void DisposeManagedResources()
    {
      foreach (IDisposable settingsProvider in this.AllRemoteSettingsProviders)
        settingsProvider.Dispose();
      this.logger.Dispose();
    }

    private void RequiresStarted()
    {
      if (!this.isStarted)
        throw new InvalidOperationException("Cannot access async methods until Start is called");
    }

    private void OnRemoteSettingsApplied()
    {
      EventHandler settingsUpdated = this.SettingsUpdated;
      if (settingsUpdated == null)
        return;
      settingsUpdated((object) this, EventArgs.Empty);
    }
  }
}
