// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.Events.TelemetryCounterEvent`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.Telemetry.Metrics.Events
{
  public class TelemetryCounterEvent<T> : TelemetryMetricEvent where T : struct
  {
    private const string CounterPrefix = "Counter.";

    private IVSCounter<T> Counter { get; set; }

    public TelemetryCounterEvent(TelemetryEvent telemetryEvent, ICounter<T> counter)
      : base(telemetryEvent, (IInstrument) counter)
    {
      if (!(counter is IVSCounter<T>))
        return;
      this.Counter = counter as IVSCounter<T>;
    }

    protected override void SetMetricProperties()
    {
      if (this.Counter == null)
        return;
      this.SetCustomMetricProperty("Counter.Sum", (object) this.Counter.Sum);
      this.SetCustomMetricProperty("Counter.Count", (object) this.Counter.Count);
    }
  }
}
