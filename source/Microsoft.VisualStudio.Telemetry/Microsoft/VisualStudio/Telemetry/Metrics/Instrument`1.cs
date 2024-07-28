// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.Instrument`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry.Metrics
{
  public abstract class Instrument<T> : IInstrument where T : struct
  {
    private readonly string name;
    private readonly string description;
    private readonly IMeter meter;
    private readonly string unit;
    protected static readonly KeyValuePair<string, object>[] EmptyTags = new KeyValuePair<string, object>[0];
    private KeyValuePair<string, object>[] internalTags;
    private const int MAX_TAGS = 8;

    public string Name => this.name;

    public string Description => this.description;

    public IMeter Meter => this.meter;

    public string Unit => this.unit;

    public bool Enabled => true;

    public bool IsObservable => false;

    protected KeyValuePair<string, object>[] Tags
    {
      get => this.internalTags ?? new KeyValuePair<string, object>[8];
      set => this.internalTags = value;
    }

    internal Instrument(IMeter meter, string name, string unit = null, string description = null)
    {
      this.meter = meter ?? throw new ArgumentNullException(nameof (meter));
      this.name = name ?? throw new ArgumentNullException(nameof (name));
      this.unit = unit ?? string.Empty;
      this.description = description ?? string.Empty;
    }

    protected void Publish()
    {
    }

    protected virtual void RecordMeasurement(T measurement) => this.RecordMeasurement(measurement, (ReadOnlySpan<KeyValuePair<string, object>>) Instrument<T>.EmptyTags.AsSpan<KeyValuePair<string, object>>());

    protected virtual void RecordMeasurement(T measurement, KeyValuePair<string, object> tag)
    {
      this.Tags[0] = tag;
      this.RecordMeasurement(measurement, (ReadOnlySpan<KeyValuePair<string, object>>) this.Tags.AsSpan<KeyValuePair<string, object>>(0, 1));
    }

    protected virtual void RecordMeasurement(
      T measurement,
      KeyValuePair<string, object> tag1,
      KeyValuePair<string, object> tag2)
    {
      this.Tags[0] = tag1;
      this.Tags[1] = tag2;
      this.RecordMeasurement(measurement, (ReadOnlySpan<KeyValuePair<string, object>>) this.Tags.AsSpan<KeyValuePair<string, object>>(0, 2));
    }

    protected virtual void RecordMeasurement(
      T measurement,
      KeyValuePair<string, object> tag1,
      KeyValuePair<string, object> tag2,
      KeyValuePair<string, object> tag3)
    {
      this.Tags[0] = tag1;
      this.Tags[1] = tag2;
      this.Tags[2] = tag3;
      this.RecordMeasurement(measurement, (ReadOnlySpan<KeyValuePair<string, object>>) this.Tags.AsSpan<KeyValuePair<string, object>>(0, 3));
    }

    protected abstract void RecordMeasurement(
      T measurement,
      ReadOnlySpan<KeyValuePair<string, object>> tags);
  }
}
