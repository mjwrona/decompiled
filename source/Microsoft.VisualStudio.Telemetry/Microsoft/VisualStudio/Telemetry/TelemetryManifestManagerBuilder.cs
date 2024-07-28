// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestManagerBuilder
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.RemoteControl;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class TelemetryManifestManagerBuilder : ITelemetryManifestManagerBuilder
  {
    private readonly object remoteControlClient;
    private readonly ITelemetryManifestManagerSettings settings;
    private readonly ITelemetryManifestParser manifestParser;
    private readonly ITelemetryScheduler scheduler;

    public TelemetryManifestManagerBuilder()
      : this((object) null, (ITelemetryManifestManagerSettings) null, (ITelemetryManifestParser) new JsonTelemetryManifestParser(), (ITelemetryScheduler) new TelemetryScheduler())
    {
    }

    public TelemetryManifestManagerBuilder(
      object theRemoteControlClient,
      ITelemetryManifestManagerSettings theSettings,
      ITelemetryManifestParser theManifestParser,
      ITelemetryScheduler theScheduler)
    {
      this.remoteControlClient = theRemoteControlClient;
      this.settings = theSettings;
      this.manifestParser = theManifestParser;
      this.scheduler = theScheduler;
    }

    public ITelemetryManifestManager Build(TelemetrySession telemetrySession) => (ITelemetryManifestManager) new TelemetryManifestManager(this.remoteControlClient as IRemoteControlClient, this.settings, this.manifestParser, this.scheduler, telemetrySession);
  }
}
