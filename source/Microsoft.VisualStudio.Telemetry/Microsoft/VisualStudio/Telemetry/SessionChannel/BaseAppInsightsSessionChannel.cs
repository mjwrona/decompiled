// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.SessionChannel.BaseAppInsightsSessionChannel
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry.SessionChannel
{
  internal abstract class BaseAppInsightsSessionChannel : 
    TelemetryDisposableObject,
    ISessionChannel,
    IDisposeAndTransmit
  {
    protected readonly string InstrumentationKey;
    protected readonly string UserId;
    private readonly Lazy<string> transportUsed;
    private const string SequenceNumberPropertyName = "Reserved.SequenceNumber";
    private bool isChannelStarted;
    private IAppInsightsClientWrapper appInsightsClient;
    private int eventCounter;
    private ChannelProperties channelProperties;

    public abstract string ChannelId { get; }

    public string TransportUsed => this.transportUsed.Value;

    public ChannelProperties Properties
    {
      get => this.channelProperties;
      set => this.channelProperties = value;
    }

    public BaseAppInsightsSessionChannel(
      string instrumentationKey,
      string userId,
      IAppInsightsClientWrapper overridedClientWrapper = null,
      ChannelProperties defaultChannelProperties = ChannelProperties.NotForUnitTest)
    {
      this.appInsightsClient = overridedClientWrapper;
      this.InstrumentationKey = instrumentationKey;
      this.UserId = userId;
      this.channelProperties = defaultChannelProperties;
      this.transportUsed = new Lazy<string>((Func<string>) (() =>
      {
        this.appInsightsClient.RequiresArgumentNotNull<IAppInsightsClientWrapper>(nameof (appInsightsClient));
        string str = this.ChannelId;
        string transportUsed;
        if (this.appInsightsClient.TryGetTransport(out transportUsed))
          str = str + "." + transportUsed;
        return str;
      }));
    }

    public void Start(string sessionId)
    {
      this.isChannelStarted = !this.isChannelStarted ? true : throw new InvalidOperationException("AppInsightsSessionChannel.Start must be called only once");
      if (this.appInsightsClient == null)
        this.appInsightsClient = this.CreateAppInsightsClientWrapper();
      this.appInsightsClient.Initialize(sessionId, this.UserId);
    }

    public async Task DisposeAndTransmitAsync(CancellationToken token)
    {
      base.DisposeManagedResources();
      if (this.appInsightsClient == null)
        return;
      await this.appInsightsClient.DisposeAndTransmitAsync(token).ConfigureAwait(false);
      this.appInsightsClient = (IAppInsightsClientWrapper) null;
    }

    public bool IsStarted => this.isChannelStarted;

    internal abstract string IKey { get; }

    protected string PersistenceFolderName => "vstel" + this.FolderNameSuffix;

    protected abstract string FolderNameSuffix { get; }

    public void PostEvent(TelemetryEvent telemetryEvent)
    {
      this.RequiresNotDisposed();
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      this.EnsureChannelIsStarted();
      EventTelemetry eventTelemetry = new EventTelemetry(telemetryEvent.Name);
      eventTelemetry.Timestamp = telemetryEvent.PostTimestamp;
      foreach (KeyValuePair<string, object> keyValuePair in telemetryEvent.Properties.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (keyValue => keyValue.Value != null)))
      {
        if (TypeTools.IsNumericType(keyValuePair.Value.GetType()))
        {
          double d = Convert.ToDouble(keyValuePair.Value, (IFormatProvider) null);
          if (!double.IsNaN(d) && !double.IsInfinity(d))
            eventTelemetry.Metrics.Add(keyValuePair.Key, d);
        }
        else
          eventTelemetry.Properties.Add(keyValuePair.Key, keyValuePair.Value.ToString());
      }
      int num = Interlocked.Increment(ref this.eventCounter);
      eventTelemetry.Metrics["Reserved.SequenceNumber"] = (double) num;
      this.AppendCommonSchemaVersion(eventTelemetry);
      this.appInsightsClient.TrackEvent(eventTelemetry);
    }

    public void PostEvent(
      TelemetryEvent telemetryEvent,
      IEnumerable<ITelemetryManifestRouteArgs> args)
    {
      this.PostEvent(telemetryEvent);
    }

    internal abstract void AppendCommonSchemaVersion(EventTelemetry eventTelemetry);

    protected abstract IAppInsightsClientWrapper CreateAppInsightsClientWrapper();

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      if (this.appInsightsClient == null)
        return;
      this.appInsightsClient.Dispose();
      this.appInsightsClient = (IAppInsightsClientWrapper) null;
    }

    private void EnsureChannelIsStarted()
    {
      if (!this.isChannelStarted)
        throw new InvalidOperationException("AppInsightsSessionChannel.Start must be called before this method");
    }
  }
}
