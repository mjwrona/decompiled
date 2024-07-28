// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.SessionChannel.DefaultAppInsightsClientWrapper
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility;

namespace Microsoft.VisualStudio.Telemetry.SessionChannel
{
  internal sealed class DefaultAppInsightsClientWrapper : BaseAppInsightsClientWrapper
  {
    private const int MaxTransmissionBufferCapacity = 102400;
    private readonly StorageBase storage;
    private readonly IProcessLockFactory processLockFactory;

    public DefaultAppInsightsClientWrapper(
      string instrumentationKey,
      StorageBase storage,
      IProcessLockFactory processLockFactory)
      : base(instrumentationKey)
    {
      this.storage = storage;
      this.processLockFactory = processLockFactory;
    }

    public override bool TryGetTransport(out string transportUsed)
    {
      transportUsed = (string) null;
      return false;
    }

    protected override ITelemetryChannel CreateAppInsightsChannel(TelemetryConfiguration config)
    {
      PersistenceChannel persistenceChannel = new PersistenceChannel(this.storage, this.processLockFactory, this.InstrumentationKey);
      persistenceChannel.Initialize(config);
      config.TelemetryChannel = (ITelemetryChannel) persistenceChannel;
      return config.TelemetryChannel;
    }
  }
}
