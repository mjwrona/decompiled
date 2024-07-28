// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.RemoteSettingsLogger
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class RemoteSettingsLogger : TelemetryDisposableObject, IRemoteSettingsLogger, IDisposable
  {
    private const string RemoteSettingsLogFolderName = "VSRemoteSettingsLog";
    private readonly ITelemetryLogFile<RemoteSettingsLogger.RemoteSettingsLogMessage> logFile;
    private readonly ITelemetryLogSettingsProvider settingsProvider;
    private readonly RemoteSettingsFilterProvider filterProvider;
    private readonly bool loggingEnabled;
    private readonly Lazy<ConcurrentQueue<RemoteSettingsLogger.RemoteSettingsLogMessage>> buffer = new Lazy<ConcurrentQueue<RemoteSettingsLogger.RemoteSettingsLogMessage>>((Func<ConcurrentQueue<RemoteSettingsLogger.RemoteSettingsLogMessage>>) (() => new ConcurrentQueue<RemoteSettingsLogger.RemoteSettingsLogMessage>()));
    private bool isStarted;
    private object flushLock = new object();

    public RemoteSettingsLogger(
      RemoteSettingsFilterProvider remoteSettingsFilterProvider,
      bool loggingEnabled)
      : this(remoteSettingsFilterProvider, loggingEnabled, (ITelemetryLogFile<RemoteSettingsLogger.RemoteSettingsLogMessage>) new RemoteSettingsJsonLogFile(), (ITelemetryLogSettingsProvider) new TelemetryLogSettingsProvider())
    {
    }

    public RemoteSettingsLogger(
      RemoteSettingsFilterProvider filterProvider,
      bool loggingEnabled,
      ITelemetryLogFile<RemoteSettingsLogger.RemoteSettingsLogMessage> logFile,
      ITelemetryLogSettingsProvider settingsProvider)
    {
      this.filterProvider = filterProvider;
      this.loggingEnabled = loggingEnabled;
      this.logFile = logFile;
      this.settingsProvider = settingsProvider;
    }

    public bool LoggingEnabled => this.loggingEnabled;

    private ConcurrentQueue<RemoteSettingsLogger.RemoteSettingsLogMessage> Buffer => this.buffer.Value;

    public Task Start() => !this.loggingEnabled ? (Task) Task.FromResult<object>((object) null) : Task.Run((Action) (() => this.FlushBufferAndStart()));

    public void LogError(string message)
    {
      if (!this.loggingEnabled)
        return;
      this.LogMessage(new RemoteSettingsLogger.RemoteSettingsLogMessage()
      {
        Level = RemoteSettingsLogger.LoggingLevel.Error,
        Message = message
      });
    }

    public void LogError(string description, Exception exception) => this.LogError(description + ": " + exception.Message);

    public void LogInfo(string message)
    {
      if (!this.loggingEnabled)
        return;
      this.LogMessage(new RemoteSettingsLogger.RemoteSettingsLogMessage()
      {
        Level = RemoteSettingsLogger.LoggingLevel.Info,
        Message = message
      });
    }

    public void LogVerbose(string message)
    {
      if (!this.loggingEnabled)
        return;
      this.LogMessage(new RemoteSettingsLogger.RemoteSettingsLogMessage()
      {
        Level = RemoteSettingsLogger.LoggingLevel.Verbose,
        Message = message
      });
    }

    public void LogVerbose(string message, object data)
    {
      if (!this.loggingEnabled)
        return;
      this.LogMessage(new RemoteSettingsLogger.RemoteSettingsLogMessage()
      {
        Level = RemoteSettingsLogger.LoggingLevel.Verbose,
        Message = message,
        Data = data
      });
    }

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      if (!this.loggingEnabled)
        return;
      if (!this.isStarted)
        this.FlushBufferAndStart();
      if (!(this.logFile is IDisposable logFile))
        return;
      logFile.Dispose();
    }

    private void LogMessage(
      RemoteSettingsLogger.RemoteSettingsLogMessage message)
    {
      if (!this.isStarted)
        this.Buffer.Enqueue(message);
      else
        this.LogMessageNoBuffer(message);
    }

    private void LogMessageNoBuffer(
      RemoteSettingsLogger.RemoteSettingsLogMessage message)
    {
      this.logFile.WriteAsync(message);
    }

    private void FlushBufferAndStart()
    {
      RemoteSettingsLogger.RemoteSettingsLogMessage result;
      lock (this.flushLock)
      {
        if (this.isStarted)
          return;
        this.settingsProvider.MainIdentifiers = (IEnumerable<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>()
        {
          new KeyValuePair<string, string>("applicationName", this.filterProvider.GetApplicationName()),
          new KeyValuePair<string, string>("applicationVersion", this.filterProvider.GetApplicationVersion()),
          new KeyValuePair<string, string>("branchName", this.filterProvider.GetBranchBuildFrom())
        };
        this.settingsProvider.Path = System.IO.Path.GetTempPath();
        this.settingsProvider.Folder = "VSRemoteSettingsLog";
        this.logFile.Initialize(this.settingsProvider);
        while (this.Buffer.TryDequeue(out result))
          this.LogMessageNoBuffer(result);
        this.isStarted = true;
      }
      while (this.Buffer.TryDequeue(out result))
        this.LogMessageNoBuffer(result);
    }

    internal enum LoggingLevel
    {
      Verbose,
      Info,
      Error,
    }

    internal class RemoteSettingsLogMessage
    {
      public RemoteSettingsLogMessage() => this.Time = DateTime.Now;

      public DateTime Time { get; set; }

      [JsonConverter(typeof (StringEnumConverter))]
      public RemoteSettingsLogger.LoggingLevel Level { get; set; }

      public string Message { get; set; }

      public object Data { get; set; }
    }
  }
}
