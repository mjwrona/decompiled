// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.SessionChannel.TelemetryLogToFileChannel
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry.SessionChannel
{
  internal class TelemetryLogToFileChannel : TelemetryDisposableObject, ISessionChannel
  {
    private const string TelemetryLogFolderName = "VSTelemetryLog";
    private readonly ITelemetryLogFile<TelemetryEvent> logFile;
    private readonly ITelemetryLogSettingsProvider settingsProvider;
    private ChannelProperties channelProperties = ChannelProperties.NotForUnitTest;
    private bool isChannelStarted;

    public string ChannelId => "fileLogger";

    public string TransportUsed => this.ChannelId;

    public ChannelProperties Properties
    {
      get => this.channelProperties;
      set => this.channelProperties = value;
    }

    public TelemetryLogToFileChannel()
      : this((ITelemetryLogSettingsProvider) new TelemetryLogSettingsProvider(), (ITelemetryLogFile<TelemetryEvent>) new TelemetryJsonLogFile())
    {
    }

    internal TelemetryLogToFileChannel(
      ITelemetryLogSettingsProvider settingsProvider,
      ITelemetryLogFile<TelemetryEvent> logFile)
    {
      settingsProvider.RequiresArgumentNotNull<ITelemetryLogSettingsProvider>(nameof (settingsProvider));
      logFile.RequiresArgumentNotNull<ITelemetryLogFile<TelemetryEvent>>(nameof (logFile));
      this.logFile = logFile;
      this.settingsProvider = settingsProvider;
    }

    public void PostEvent(TelemetryEvent telemetryEvent)
    {
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      this.logFile.WriteAsync(telemetryEvent);
    }

    public void PostEvent(
      TelemetryEvent telemetryEvent,
      IEnumerable<ITelemetryManifestRouteArgs> args)
    {
      this.PostEvent(telemetryEvent);
    }

    public void Start(string sessionID)
    {
      this.settingsProvider.MainIdentifiers = (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
      {
        new KeyValuePair<string, string>("session_id", sessionID)
      };
      this.settingsProvider.Path = System.IO.Path.GetTempPath();
      this.settingsProvider.Folder = "VSTelemetryLog";
      this.isChannelStarted = true;
      this.logFile.Initialize(this.settingsProvider);
    }

    public bool IsStarted => this.isChannelStarted;

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      if (!(this.logFile is IDisposable logFile))
        return;
      logFile.Dispose();
    }
  }
}
