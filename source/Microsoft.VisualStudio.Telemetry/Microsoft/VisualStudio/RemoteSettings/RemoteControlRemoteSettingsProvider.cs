// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.RemoteControlRemoteSettingsProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class RemoteControlRemoteSettingsProvider : 
    RemoteSettingsProviderBase,
    IRemoteSettingsProvider,
    ISettingsCollection,
    IDisposable
  {
    private const int DisposingIsStarted = 1;
    private const int DisposingNotStarted = 0;
    private const string RemoteSettingsTelemetryEventPath = "VS/Core/RemoteSettings/";
    private const string RemoteSettingsTelemetryPropertyPath = "VS.Core.RemoteSettings.";
    private readonly IVersionedRemoteSettingsStorageHandler remoteSettingsStorageHandler;
    private readonly IRemoteSettingsTelemetry remoteSettingsTelemetry;
    private readonly Lazy<IRemoteFileReader> remoteFileReader;
    private readonly IRemoteSettingsParser remoteSettingsParser;
    private readonly IScopeParserFactory scopeParserFactory;
    private readonly IRemoteSettingsValidator remoteSettingsValidator;
    private string fileName;
    private int startedDisposing;
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public RemoteControlRemoteSettingsProvider(RemoteSettingsInitializer initializer)
      : base((IRemoteSettingsStorageHandler) initializer.VersionedRemoteSettingsStorageHandler, initializer.RemoteSettingsLogger)
    {
      this.remoteSettingsTelemetry = initializer.Telemetry;
      this.remoteSettingsStorageHandler = initializer.VersionedRemoteSettingsStorageHandler;
      this.fileName = initializer.RemoteSettingsFileName;
      this.remoteFileReader = new Lazy<IRemoteFileReader>((Func<IRemoteFileReader>) (() => initializer.RemoteFileReaderFactory.Instance()));
      this.remoteSettingsParser = initializer.RemoteSettingsParser;
      this.scopeParserFactory = initializer.ScopeParserFactory;
      this.remoteSettingsValidator = initializer.RemoteSettingsValidator;
    }

    public override string Name => "RemoteControl: " + this.fileName;

    public override Task<GroupedRemoteSettings> Start()
    {
      this.RequiresNotDisposed();
      this.startTask = Task.Run<GroupedRemoteSettings>((Func<Task<GroupedRemoteSettings>>) (async () =>
      {
        RemoteControlRemoteSettingsProvider settingsProvider = this;
        string settingsFileEventName = "VS/Core/RemoteSettings/GetSettingsFileContent";
        Dictionary<string, object> settingsFileEventProperties = new Dictionary<string, object>();
        using (Stream stream = await settingsProvider.remoteFileReader.Value.ReadFileAsync())
        {
          if (stream != null)
          {
            settingsFileEventProperties["VS.Core.RemoteSettings.GetContentsSucceeded"] = (object) true;
            settingsProvider.remoteSettingsTelemetry.PostEvent(settingsFileEventName, (IDictionary<string, object>) settingsFileEventProperties);
            string name = "VS/Core/RemoteSettings/ParseSettings";
            Dictionary<string, object> properties = new Dictionary<string, object>();
            VersionedDeserializedRemoteSettings versionedStream = settingsProvider.remoteSettingsParser.TryParseVersionedStream(stream);
            if (!versionedStream.Successful)
            {
              settingsProvider.logger.LogError("Error deserializing RemoteControl file: " + versionedStream.Error);
              properties["VS.Core.RemoteSettings.ErrorMessage"] = (object) versionedStream.Error;
              settingsProvider.remoteSettingsTelemetry.PostEvent(name, (IDictionary<string, object>) properties);
              settingsProvider.ValidateStoredRemoteSettings();
              return (GroupedRemoteSettings) null;
            }
            settingsProvider.logger.LogVerbose("Got " + settingsProvider.Name + " settings of version " + versionedStream.FileVersion + " with ChangesetId " + versionedStream.ChangesetId);
            properties["VS.Core.RemoteSettings.SettingsFileVersion"] = (object) versionedStream.FileVersion;
            properties["VS.Core.RemoteSettings.SettingsFileChangeSet"] = (object) versionedStream.ChangesetId;
            settingsProvider.remoteSettingsTelemetry.PostEvent(name, (IDictionary<string, object>) properties);
            settingsProvider.ProcessRemoteSettingsFile(versionedStream);
            return new GroupedRemoteSettings((DeserializedRemoteSettings) versionedStream, settingsProvider.Name);
          }
          if (settingsProvider.startedDisposing == 0)
          {
            settingsFileEventProperties["VS.Core.RemoteSettings.GetContentsSucceeded"] = (object) false;
            settingsProvider.remoteSettingsTelemetry.PostEvent(settingsFileEventName, (IDictionary<string, object>) settingsFileEventProperties);
            settingsProvider.ValidateStoredRemoteSettings();
          }
        }
        return (GroupedRemoteSettings) null;
      }));
      return this.startTask;
    }

    protected override void DisposeManagedResources()
    {
      if (Interlocked.CompareExchange(ref this.startedDisposing, 1, 0) == 1)
        return;
      this.cancellationTokenSource.Cancel();
      if (!this.remoteFileReader.IsValueCreated)
        return;
      this.remoteFileReader.Value.Dispose();
    }

    private void ProcessRemoteSettingsFile(VersionedDeserializedRemoteSettings remoteSettings)
    {
      if (this.cancellationTokenSource.IsCancellationRequested)
        return;
      if (this.remoteSettingsStorageHandler.DoSettingsNeedToBeUpdated(remoteSettings.FileVersion))
      {
        IRemoteSettingsTelemetryActivity activity = this.remoteSettingsTelemetry.CreateActivity("VS/Core/RemoteSettings/Apply");
        using (Mutex mutex = new Mutex(false, "Global\\7BCAEF5B-E7EA-428D-84AF-105BCD4D93FC-" + this.fileName.Replace('.', '-')))
        {
          bool flag;
          try
          {
            flag = mutex.WaitOne(-1, false);
          }
          catch (AbandonedMutexException ex)
          {
            flag = true;
          }
          if (flag)
          {
            if (!this.remoteSettingsStorageHandler.DoSettingsNeedToBeUpdated(remoteSettings.FileVersion))
              return;
            activity.Start();
            this.logger.LogVerbose("Applying new settings for " + this.Name);
            if (this.remoteSettingsStorageHandler.FileVersion == string.Empty)
            {
              this.remoteSettingsStorageHandler.DeleteAllSettings();
              this.remoteSettingsStorageHandler.SaveSettings(remoteSettings);
            }
            else
            {
              this.remoteSettingsStorageHandler.DeleteSettingsForFileVersion(remoteSettings.FileVersion);
              this.remoteSettingsStorageHandler.SaveSettings(remoteSettings);
              this.remoteSettingsStorageHandler.CleanUpOldFileVersions(remoteSettings.FileVersion);
            }
            activity.End();
            mutex.ReleaseMutex();
          }
        }
        activity.Post((IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "VS.Core.RemoteSettings.SettingsCount",
            (object) remoteSettings.Settings.Count
          }
        });
      }
      else
      {
        this.logger.LogVerbose("Settings for " + this.Name + " are the same as cached version");
        this.ValidateStoredRemoteSettings();
      }
    }

    private void ValidateStoredRemoteSettings()
    {
      if (this.cancellationTokenSource.IsCancellationRequested)
        return;
      try
      {
        this.remoteSettingsValidator.ValidateStored();
      }
      catch (RemoteSettingsValidationException ex)
      {
        this.logger.LogError("Stored remote settings not validated", (Exception) ex);
        this.remoteSettingsStorageHandler.InvalidateFileVersion();
      }
    }
  }
}
