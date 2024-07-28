// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.Events.TelemetryMetricEvent
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.Telemetry.Metrics.Events
{
  public abstract class TelemetryMetricEvent
  {
    private const string MetricPrefix = "Reserved.Metric.";
    private const string MeterPrefix = "Reserved.Meter.";

    internal TelemetryEvent MetricEvent { get; private set; }

    internal IInstrument Metric { get; private set; }

    public TelemetryMetricEvent(TelemetryEvent telemetryEvent, IInstrument metric)
    {
      this.MetricEvent = telemetryEvent;
      this.Metric = metric;
    }

    internal void SetProperties()
    {
      this.SetCustomMetricProperty("Name", (object) this.Metric.Name);
      this.MetricEvent.ReservedProperties.AddPrefixed("Reserved.Meter.Name", (object) this.Metric.Meter.Name);
      if (!string.IsNullOrWhiteSpace(this.Metric.Meter.Version))
        this.MetricEvent.ReservedProperties.AddPrefixed("Reserved.Meter.Version", (object) this.Metric.Meter.Version);
      if (!string.IsNullOrWhiteSpace(this.Metric.Description))
        this.SetCustomMetricProperty("Description", (object) this.Metric.Description);
      if (!string.IsNullOrWhiteSpace(this.Metric.Unit))
        this.SetCustomMetricProperty("Unit", (object) this.Metric.Unit);
      this.SetMetricProperties();
    }

    protected void SetCustomMetricProperty(string propertyKey, object content) => this.MetricEvent.ReservedProperties.AddPrefixed("Reserved.Metric." + propertyKey, content);

    protected abstract void SetMetricProperties();

    public override string ToString() => this.MetricEvent.Name ?? "";
  }
}
