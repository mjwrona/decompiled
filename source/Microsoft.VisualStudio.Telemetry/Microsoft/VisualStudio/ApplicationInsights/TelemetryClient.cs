// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.TelemetryClient
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights
{
  public sealed class TelemetryClient
  {
    private readonly TelemetryConfiguration configuration;
    private TelemetryContext context;
    private ITelemetryChannel channel;

    public TelemetryClient()
      : this(TelemetryConfiguration.Active)
    {
    }

    public TelemetryClient(TelemetryConfiguration configuration)
    {
      if (configuration == null)
      {
        CoreEventSource.Log.TelemetryClientConstructorWithNoTelemetryConfiguration();
        configuration = TelemetryConfiguration.Active;
      }
      this.configuration = configuration;
    }

    public TelemetryContext Context
    {
      get => LazyInitializer.EnsureInitialized<TelemetryContext>(ref this.context, new Func<TelemetryContext>(this.CreateInitializedContext));
      internal set => this.context = value;
    }

    public string InstrumentationKey
    {
      get => this.Context.InstrumentationKey;
      set => this.Context.InstrumentationKey = value;
    }

    internal ITelemetryChannel Channel
    {
      get
      {
        ITelemetryChannel channel = this.channel;
        if (channel == null)
        {
          channel = this.configuration.TelemetryChannel;
          this.channel = channel;
        }
        return channel;
      }
      set => this.channel = value;
    }

    public bool IsEnabled() => !this.configuration.DisableTelemetry;

    public void TrackEvent(
      string eventName,
      IDictionary<string, string> properties = null,
      IDictionary<string, double> metrics = null)
    {
      EventTelemetry telemetry = new EventTelemetry(eventName);
      if (properties != null && properties.Count > 0)
        Utils.CopyDictionary<string>(properties, telemetry.Context.Properties);
      if (metrics != null && metrics.Count > 0)
        Utils.CopyDictionary<double>(metrics, telemetry.Metrics);
      this.TrackEvent(telemetry);
    }

    public void TrackEvent(EventTelemetry telemetry)
    {
      if (telemetry == null)
        telemetry = new EventTelemetry();
      this.Track((ITelemetry) telemetry);
    }

    public void TrackTrace(string message) => this.TrackTrace(new TraceTelemetry(message));

    public void TrackTrace(string message, SeverityLevel severityLevel) => this.TrackTrace(new TraceTelemetry(message, severityLevel));

    public void TrackTrace(string message, IDictionary<string, string> properties)
    {
      TraceTelemetry telemetry = new TraceTelemetry(message);
      if (properties != null && properties.Count > 0)
        Utils.CopyDictionary<string>(properties, telemetry.Context.Properties);
      this.TrackTrace(telemetry);
    }

    public void TrackTrace(
      string message,
      SeverityLevel severityLevel,
      IDictionary<string, string> properties)
    {
      TraceTelemetry telemetry = new TraceTelemetry(message, severityLevel);
      if (properties != null && properties.Count > 0)
        Utils.CopyDictionary<string>(properties, telemetry.Context.Properties);
      this.TrackTrace(telemetry);
    }

    public void TrackTrace(TraceTelemetry telemetry)
    {
      telemetry = telemetry ?? new TraceTelemetry();
      this.Track((ITelemetry) telemetry);
    }

    public void TrackMetric(string name, double value, IDictionary<string, string> properties = null)
    {
      MetricTelemetry telemetry = new MetricTelemetry(name, value);
      if (properties != null && properties.Count > 0)
        Utils.CopyDictionary<string>(properties, telemetry.Properties);
      this.TrackMetric(telemetry);
    }

    public void TrackMetric(MetricTelemetry telemetry)
    {
      if (telemetry == null)
        telemetry = new MetricTelemetry();
      this.Track((ITelemetry) telemetry);
    }

    public void TrackException(
      Exception exception,
      IDictionary<string, string> properties = null,
      IDictionary<string, double> metrics = null)
    {
      if (exception == null)
        exception = new Exception(Utils.PopulateRequiredStringValue((string) null, "message", typeof (ExceptionTelemetry).FullName));
      ExceptionTelemetry telemetry = new ExceptionTelemetry(exception)
      {
        HandledAt = ExceptionHandledAt.UserCode
      };
      if (properties != null && properties.Count > 0)
        Utils.CopyDictionary<string>(properties, telemetry.Context.Properties);
      if (metrics != null && metrics.Count > 0)
        Utils.CopyDictionary<double>(metrics, telemetry.Metrics);
      this.TrackException(telemetry);
    }

    public void TrackException(ExceptionTelemetry telemetry)
    {
      if (telemetry == null)
        telemetry = new ExceptionTelemetry(new Exception(Utils.PopulateRequiredStringValue((string) null, "message", typeof (ExceptionTelemetry).FullName)))
        {
          HandledAt = ExceptionHandledAt.UserCode
        };
      this.Track((ITelemetry) telemetry);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Track(ITelemetry telemetry)
    {
      if (!this.IsEnabled())
        return;
      string instrumentationKey = this.Context.InstrumentationKey;
      if (string.IsNullOrEmpty(instrumentationKey))
        instrumentationKey = this.configuration.InstrumentationKey;
      if (string.IsNullOrEmpty(instrumentationKey))
        return;
      if (telemetry is ISupportProperties supportProperties)
      {
        if (this.Channel.DeveloperMode)
          supportProperties.Properties.Add("DeveloperMode", "true");
        Utils.CopyDictionary<string>(this.Context.Properties, supportProperties.Properties);
      }
      telemetry.Context.Initialize(this.Context, instrumentationKey);
      foreach (ITelemetryInitializer telemetryInitializer in (IEnumerable<ITelemetryInitializer>) this.configuration.TelemetryInitializers)
      {
        try
        {
          telemetryInitializer.Initialize(telemetry);
        }
        catch (Exception ex)
        {
          CoreEventSource.Log.LogError(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Exception while initializing {0}, exception message - {1}", new object[2]
          {
            (object) telemetryInitializer.GetType().FullName,
            (object) ex.ToString()
          }));
        }
      }
      telemetry.Sanitize();
      this.Channel.Send(telemetry);
    }

    public void TrackPageView(string name) => this.Track((ITelemetry) new PageViewTelemetry(name));

    public void TrackPageView(PageViewTelemetry telemetry)
    {
      if (telemetry == null)
        telemetry = new PageViewTelemetry();
      this.Track((ITelemetry) telemetry);
    }

    public void TrackRequest(
      string name,
      DateTimeOffset timestamp,
      TimeSpan duration,
      string responseCode,
      bool success)
    {
      this.Track((ITelemetry) new RequestTelemetry(name, timestamp, duration, responseCode, success));
    }

    public void TrackRequest(RequestTelemetry request) => this.Track((ITelemetry) request);

    public void Flush() => this.Channel.Flush();

    public async Task FlushAndTransmitAsync(CancellationToken token) => await this.Channel.FlushAndTransmitAsync(token).ConfigureAwait(false);

    private TelemetryContext CreateInitializedContext()
    {
      TelemetryContext context = new TelemetryContext();
      foreach (IContextInitializer contextInitializer in (IEnumerable<IContextInitializer>) this.configuration.ContextInitializers)
        contextInitializer.Initialize(context);
      return context;
    }
  }
}
