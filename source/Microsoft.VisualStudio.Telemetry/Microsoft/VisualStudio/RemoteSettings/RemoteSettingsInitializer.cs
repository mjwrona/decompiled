// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.RemoteSettingsInitializer
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Experimentation;
using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Telemetry.Notification;
using Microsoft.VisualStudio.Telemetry.Services;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.RemoteSettings
{
  public sealed class RemoteSettingsInitializer
  {
    public bool? UsePathPrefix { get; set; }

    public IEnumerable<IScopeFilterProvider> ScopeFilterProviders { get; set; }

    public string RemoteSettingsFileName { get; set; }

    public ICollectionKeyValueStorage KeyValueStorage { get; set; }

    public IExperimentationService ExperimentationService { get; set; }

    public ITelemetryNotificationService TelemetryNotificationService { get; set; }

    public IRemoteSettingsTelemetry Telemetry { get; set; }

    internal ITargetedNotificationsTelemetry TargetedNotificationsTelemetry { get; set; }

    internal ITargetedNotificationsCacheStorageProvider TargetedNotificationsCacheStorage { get; set; }

    public RemoteSettingsFilterProvider FilterProvider { get; set; }

    public ICollectionKeyValueStorage NonScopedSettingsKeyValueStorage { get; set; }

    public IEnumerable<string> StableSettingRootSubCollections { get; set; }

    internal IScopeParserFactory ScopeParserFactory { get; set; }

    internal static RemoteSettingsInitializer BuildDefault() => new RemoteSettingsInitializer().FillWithDefaults();

    internal RemoteSettingsInitializer FillWithDefaults()
    {
      if (!this.UsePathPrefix.HasValue)
        this.UsePathPrefix = new bool?(true);
      if (this.RemoteSettingsFileName == null)
        this.RemoteSettingsFileName = "Default.json";
      if (this.KeyValueStorage == null)
        this.KeyValueStorage = (ICollectionKeyValueStorage) new DefaultRegistryKeyValueStorage(Platform.IsWindows ? (IRegistryTools3) new RegistryTools() : (IRegistryTools3) new FileBasedRegistryTools());
      if (this.FilterProvider == null)
        this.FilterProvider = (RemoteSettingsFilterProvider) new DefaultRemoteSettingsFilterProvider(TelemetryService.DefaultSession);
      if (this.RemoteSettingsLogger == null)
        this.RemoteSettingsLogger = (IRemoteSettingsLogger) new Microsoft.VisualStudio.RemoteSettings.RemoteSettingsLogger(this.FilterProvider, RemoteSettingsStorageHandlerFactory.BuildIsLoggingEnabled(this.KeyValueStorage, this.UsePathPrefix.HasValue && this.UsePathPrefix.Value)());
      if (this.ScopeParserFactory == null)
        this.ScopeParserFactory = (IScopeParserFactory) new Microsoft.VisualStudio.RemoteSettings.ScopeParserFactory(this);
      RemoteSettingsStorageHandlerFactory remoteSettingsStorageHandlerFactory = new RemoteSettingsStorageHandlerFactory(this.RemoteSettingsLogger);
      if (this.VersionedRemoteSettingsStorageHandler == null)
        this.VersionedRemoteSettingsStorageHandler = remoteSettingsStorageHandlerFactory.BuildVersioned(this.KeyValueStorage, this.UsePathPrefix.HasValue && this.UsePathPrefix.Value, this.RemoteSettingsFileName, this.ScopeParserFactory);
      if (this.CacheableRemoteSettingsStorageHandler == null)
        this.CacheableRemoteSettingsStorageHandler = remoteSettingsStorageHandlerFactory.Build(this.KeyValueStorage, this.UsePathPrefix.HasValue && this.UsePathPrefix.Value, this.FilterProvider, this.ScopeParserFactory);
      if (this.LocalTestRemoteSettingsStorageHandler == null)
        this.LocalTestRemoteSettingsStorageHandler = remoteSettingsStorageHandlerFactory.Build(this.KeyValueStorage, this.UsePathPrefix.HasValue && this.UsePathPrefix.Value, "LocalTest", this.ScopeParserFactory);
      if (this.LiveRemoteSettingsStorageHandlerFactory == null)
        this.LiveRemoteSettingsStorageHandlerFactory = (Func<IRemoteSettingsStorageHandler>) (() => remoteSettingsStorageHandlerFactory.Build((ICollectionKeyValueStorage) new MemoryKeyValueStorage(), false, string.Empty, this.ScopeParserFactory));
      if (this.NonScopedRemoteSettingsStorageHandler == null && this.NonScopedSettingsKeyValueStorage != null)
        this.NonScopedRemoteSettingsStorageHandler = remoteSettingsStorageHandlerFactory.Build(this.NonScopedSettingsKeyValueStorage, false, string.Empty, this.ScopeParserFactory);
      if (this.IsUpdatedDisabled == null)
        this.IsUpdatedDisabled = RemoteSettingsStorageHandlerFactory.BuildIsUpdateDisabled(this.KeyValueStorage, this.UsePathPrefix.HasValue && this.UsePathPrefix.Value);
      if (this.RemoteFileReaderFactory == null)
        this.RemoteFileReaderFactory = (IRemoteFileReaderFactory) new RemoteSettingsRemoteFileReaderFactory(this.RemoteSettingsFileName);
      if (this.RemoteSettingsValidator == null)
        this.RemoteSettingsValidator = (IRemoteSettingsValidator) new Microsoft.VisualStudio.RemoteSettings.RemoteSettingsValidator((ICycleDetection) new CycleDetection(), (IScopesStorageHandler) this.VersionedRemoteSettingsStorageHandler);
      if (this.RemoteSettingsParser == null)
        this.RemoteSettingsParser = (IRemoteSettingsParser) new Microsoft.VisualStudio.RemoteSettings.RemoteSettingsParser(this.RemoteSettingsValidator);
      if (this.TargetedNotificationsParser == null)
        this.TargetedNotificationsParser = (ITargetedNotificationsParser) new Microsoft.VisualStudio.RemoteSettings.TargetedNotificationsParser();
      if (this.ExperimentationService == null)
        this.ExperimentationService = Microsoft.VisualStudio.Experimentation.ExperimentationService.Default;
      if (this.ExperimentationService == null)
        this.ExperimentationService = Microsoft.VisualStudio.Experimentation.ExperimentationService.Default;
      if (this.TelemetryNotificationService == null)
        this.TelemetryNotificationService = Microsoft.VisualStudio.Telemetry.Notification.TelemetryNotificationService.Default;
      if (this.Telemetry == null)
        this.Telemetry = (IRemoteSettingsTelemetry) new DefaultRemoteSettingsTelemetry(TelemetryService.DefaultSession);
      if (this.TargetedNotificationsTelemetry == null)
        this.TargetedNotificationsTelemetry = (ITargetedNotificationsTelemetry) new DefaultTargetedNotificationsTelemetry(TelemetryService.DefaultSession);
      if (this.TargetedNotificationsCacheStorage == null)
        this.TargetedNotificationsCacheStorage = (ITargetedNotificationsCacheStorageProvider) new TargetedNotificationsJsonStorageProvider(this);
      if (this.HttpWebRequestFactory == null)
        this.HttpWebRequestFactory = (IHttpWebRequestFactory) new Microsoft.VisualStudio.Telemetry.Services.HttpWebRequestFactory();
      if (this.ScopeFilterProviders == null)
      {
        ProcessInformationProvider informationProvider = new ProcessInformationProvider();
        this.ScopeFilterProviders = (IEnumerable<IScopeFilterProvider>) new List<IScopeFilterProvider>()
        {
          (IScopeFilterProvider) new FlightScopeFilterProvider(this.ExperimentationService),
          (IScopeFilterProvider) new InternalScopeFilterProvider(TelemetryService.DefaultSession),
          (IScopeFilterProvider) new VersionScopeFilterProvider(this.FilterProvider),
          (IScopeFilterProvider) new ExeNameScopeFilterProvider(this.FilterProvider),
          (IScopeFilterProvider) new ScopeScopeFilterProvider((IScopesStorageHandler) this.VersionedRemoteSettingsStorageHandler, this.ScopeParserFactory)
        };
      }
      if (this.RemoteSettingsProviders == null)
        this.RemoteSettingsProviders = (IEnumerable<Func<RemoteSettingsInitializer, IRemoteSettingsProvider>>) new List<Func<RemoteSettingsInitializer, IRemoteSettingsProvider>>()
        {
          (Func<RemoteSettingsInitializer, IRemoteSettingsProvider>) (remoteSettingsInitializer => (IRemoteSettingsProvider) new LocalTestProvider(remoteSettingsInitializer)),
          (Func<RemoteSettingsInitializer, IRemoteSettingsProvider>) (remoteSettingsInitializer => (IRemoteSettingsProvider) new TargetedNotificationsProvider(remoteSettingsInitializer)),
          (Func<RemoteSettingsInitializer, IRemoteSettingsProvider>) (remoteSettingsInitializer => (IRemoteSettingsProvider) new RemoteControlRemoteSettingsProvider(remoteSettingsInitializer))
        };
      if (this.LocalTestDirectories == null)
      {
        string rootPath = System.IO.Path.Combine(this.GetLocalAppDataRoot(), "LocalTest");
        this.LocalTestDirectories = (IEnumerable<IDirectoryReader>) new List<IDirectoryReader>()
        {
          (IDirectoryReader) new DirectoryReader(rootPath, "PersistentActions", false, 0, this.RemoteSettingsLogger),
          (IDirectoryReader) new DirectoryReader(rootPath, "OneTimeActions", true, 10, this.RemoteSettingsLogger)
        };
      }
      if (this.LocalTestParser == null)
        this.LocalTestParser = (ILocalTestParser) new Microsoft.VisualStudio.RemoteSettings.LocalTestParser();
      if (this.StableRemoteSettingsProvider == null)
        this.StableRemoteSettingsProvider = (Func<RemoteSettingsInitializer, IStableRemoteSettingsProvider>) (remoteSettingsInitializer => (IStableRemoteSettingsProvider) new Microsoft.VisualStudio.RemoteSettings.StableRemoteSettingsProvider(remoteSettingsInitializer));
      if (this.StableSettingRootSubCollections == null)
        this.StableSettingRootSubCollections = Enumerable.Empty<string>();
      return this;
    }

    internal IVersionedRemoteSettingsStorageHandler VersionedRemoteSettingsStorageHandler { get; set; }

    internal IRemoteSettingsStorageHandler CacheableRemoteSettingsStorageHandler { get; set; }

    internal IRemoteSettingsStorageHandler LocalTestRemoteSettingsStorageHandler { get; set; }

    internal IEnumerable<IDirectoryReader> LocalTestDirectories { get; set; }

    internal Func<IRemoteSettingsStorageHandler> LiveRemoteSettingsStorageHandlerFactory { get; set; }

    internal IRemoteSettingsStorageHandler NonScopedRemoteSettingsStorageHandler { get; set; }

    internal IRemoteFileReaderFactory RemoteFileReaderFactory { get; set; }

    internal IRemoteSettingsParser RemoteSettingsParser { get; set; }

    internal ITargetedNotificationsParser TargetedNotificationsParser { get; set; }

    internal IRemoteSettingsValidator RemoteSettingsValidator { get; set; }

    internal IEnumerable<Func<RemoteSettingsInitializer, IRemoteSettingsProvider>> RemoteSettingsProviders { get; set; }

    internal Func<RemoteSettingsInitializer, IStableRemoteSettingsProvider> StableRemoteSettingsProvider { get; set; }

    internal IHttpWebRequestFactory HttpWebRequestFactory { get; set; }

    internal Func<bool> IsUpdatedDisabled { get; set; }

    internal ILocalTestParser LocalTestParser { get; set; }

    internal IRemoteSettingsLogger RemoteSettingsLogger { get; set; }

    internal string GetLocalAppDataRoot()
    {
      try
      {
        string path1;
        if (Platform.IsMac)
          path1 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support");
        else if (Platform.IsLinux)
        {
          string str = Environment.GetEnvironmentVariable("XDG_CACHE_HOME");
          if (string.IsNullOrEmpty(str))
            str = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache");
          path1 = str;
        }
        else
          path1 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return System.IO.Path.Combine(path1, "Microsoft", "VisualStudio", "RemoteSettings");
      }
      catch
      {
      }
      return string.Empty;
    }
  }
}
