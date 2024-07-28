// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.CounterBase`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry.Metrics
{
  internal abstract class CounterBase<T> : Instrument<T>, ICounter<T>, IInstrument, IVSCounter<T> where T : struct
  {
    public T Sum { get; private set; }

    public long Count { get; private set; }

    internal CounterBase(IMeter meter, string name, string unit = null, string description = null)
      : base(meter, name, unit, description)
    {
      this.Sum = default (T);
      this.Count = 0L;
    }

    public void Add(T delta) => this.RecordMeasurement(delta);

    public void Add(T delta, KeyValuePair<string, object> tag) => this.RecordMeasurement(delta, tag);

    public void Add(T delta, KeyValuePair<string, object> tag1, KeyValuePair<string, object> tag2) => this.RecordMeasurement(delta, tag1, tag2);

    public void Add(
      T delta,
      KeyValuePair<string, object> tag1,
      KeyValuePair<string, object> tag2,
      KeyValuePair<string, object> tag3)
    {
      this.RecordMeasurement(delta, tag1, tag2, tag3);
    }

    public void Add(T delta, params KeyValuePair<string, object>[] tags) => this.RecordMeasurement(delta, (ReadOnlySpan<KeyValuePair<string, object>>) tags.AsSpan<KeyValuePair<string, object>>());

    public void Add(T delta, ReadOnlySpan<KeyValuePair<string, object>> tags) => this.RecordMeasurement(delta, tags);

    protected override void RecordMeasurement(
      T measurement,
      ReadOnlySpan<KeyValuePair<string, object>> tags)
    {
      this.Sum = GenericNumericUtility<T>.Add(this.Sum, measurement);
      ++this.Count;
    }
  }
}
