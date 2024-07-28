// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.SessionChannel.AsimovAppInsightsClientWrapper
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility;

namespace Microsoft.VisualStudio.Telemetry.SessionChannel
{
  internal sealed class AsimovAppInsightsClientWrapper : BaseAppInsightsClientWrapper
  {
    private const int MaxTransmissionBufferCapacity = 102400;
    private const string TransportUtc = "utc";
    private const string TransportVortex = "vortex";
    private const string UtcInstalledYes = "Yes";
    private const string UtcInstalledNo = "No";
    private const string UtcInstalledUnknown = "Unknown";
    private readonly bool isUtcEnabled;
    private readonly TelemetrySession hostTelemetrySession;
    private readonly StorageBase storage;
    private readonly IProcessLockFactory processLockFactory;
    private BaseAppInsightsClientWrapper.Transport usedTransport;

    public AsimovAppInsightsClientWrapper(
      bool isUtcEnabled,
      string instrumentationKey,
      TelemetrySession hostTelemetrySession,
      StorageBase storage,
      IProcessLockFactory processLockFactory)
      : base(instrumentationKey)
    {
      this.isUtcEnabled = isUtcEnabled;
      this.hostTelemetrySession = hostTelemetrySession;
      this.storage = storage;
      this.processLockFactory = processLockFactory;
    }

    public override bool TryGetTransport(out string transportUsed)
    {
      if (this.isUtcEnabled)
      {
        transportUsed = this.usedTransport == BaseAppInsightsClientWrapper.Transport.Vortex ? "vortex" : "utc";
        return true;
      }
      transportUsed = (string) null;
      return false;
    }

    protected override ITelemetryChannel CreateAppInsightsChannel(TelemetryConfiguration config)
    {
      config.TelemetryChannel = (ITelemetryChannel) null;
      string propertyValue = "No";
      if (this.isUtcEnabled)
      {
        int num = UniversalTelemetryChannel.IsAvailable() ? 1 : 0;
        propertyValue = num != 0 ? "Yes" : "No";
        if (num != 0)
        {
          config.TelemetryChannel = (ITelemetryChannel) new UniversalTelemetryChannel();
          this.usedTransport = BaseAppInsightsClientWrapper.Transport.Utc;
        }
      }
      if (config.TelemetryChannel == null)
      {
        PersistenceChannel persistenceChannel = new PersistenceChannel(this.storage, this.processLockFactory, this.InstrumentationKey);
        persistenceChannel.Initialize(config);
        persistenceChannel.EndpointAddress = "https://vortex.data.microsoft.com/collect/v1";
        config.TelemetryChannel = (ITelemetryChannel) persistenceChannel;
        config.TelemetryInitializers.Add((ITelemetryInitializer) new SequencePropertyInitializer());
        this.usedTransport = BaseAppInsightsClientWrapper.Transport.Vortex;
      }
      if (!this.hostTelemetrySession.IsSessionCloned)
        this.hostTelemetrySession.PostProperty("VS.TelemetryApi.IsUtcInstalled", (object) propertyValue);
      return config.TelemetryChannel;
    }
  }
}
