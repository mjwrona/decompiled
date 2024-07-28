// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.SessionChannel.BaseAppInsightsClientWrapper
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights;
using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry.SessionChannel
{
  internal abstract class BaseAppInsightsClientWrapper : 
    TelemetryDisposableObject,
    IAppInsightsClientWrapper,
    IDisposable,
    IDisposeAndTransmit
  {
    private readonly string instrumentationKey;
    private TelemetryClient appInsightsClient;
    private ITelemetryChannel appInsightsChannel;

    public string InstrumentationKey => this.instrumentationKey;

    public abstract bool TryGetTransport(out string transportUsed);

    public BaseAppInsightsClientWrapper(string instrumentationKey)
    {
      instrumentationKey.RequiresArgumentNotNullAndNotWhiteSpace(nameof (instrumentationKey));
      this.instrumentationKey = instrumentationKey;
    }

    public void Initialize(string sessionId, string userId)
    {
      sessionId.RequiresArgumentNotNullAndNotWhiteSpace(nameof (sessionId));
      TelemetryConfiguration telemetryConfiguration = TelemetryConfiguration.CreateDefault();
      if (telemetryConfiguration.TelemetryChannel != null)
      {
        telemetryConfiguration.TelemetryChannel.Dispose();
        telemetryConfiguration.TelemetryChannel = (ITelemetryChannel) null;
      }
      telemetryConfiguration.TelemetryInitializers.Remove(telemetryConfiguration.TelemetryInitializers.FirstOrDefault<ITelemetryInitializer>((Func<ITelemetryInitializer, bool>) (o => o is TimestampPropertyInitializer)));
      this.appInsightsChannel = this.CreateAppInsightsChannel(telemetryConfiguration);
      TelemetryClient telemetryClient = new TelemetryClient(telemetryConfiguration);
      Microsoft.VisualStudio.ApplicationInsights.DataContracts.TelemetryContext context = telemetryClient.Context;
      context.InstrumentationKey = this.InstrumentationKey;
      context.Session.Id = sessionId;
      context.User.Id = userId;
      context.Device.Type = "0";
      context.Device.Id = "0";
      this.appInsightsClient = telemetryClient;
    }

    public void TrackEvent(EventTelemetry ev)
    {
      if (this.appInsightsClient == null || this.IsDisposed)
        return;
      this.appInsightsClient.TrackEvent(ev);
    }

    public async Task DisposeAndTransmitAsync(CancellationToken token)
    {
      base.DisposeManagedResources();
      if (this.appInsightsChannel == null)
        return;
      try
      {
        await this.appInsightsClient.FlushAndTransmitAsync(token).ConfigureAwait(false);
      }
      catch (FileNotFoundException ex)
      {
      }
      this.DisposeChannel();
    }

    protected abstract ITelemetryChannel CreateAppInsightsChannel(TelemetryConfiguration config);

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      if (this.appInsightsChannel == null)
        return;
      try
      {
        this.appInsightsClient.Flush();
      }
      catch (FileNotFoundException ex)
      {
      }
      this.DisposeChannel();
    }

    private void DisposeChannel()
    {
      try
      {
        this.appInsightsChannel.Dispose();
      }
      catch (InvalidOperationException ex)
      {
      }
      catch (FileNotFoundException ex)
      {
      }
    }

    internal enum Transport
    {
      Utc,
      Vortex,
      Collector,
    }
  }
}
