// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.Counter`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Metrics.Exceptions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry.Metrics
{
  internal class Counter<T> : CounterBase<T> where T : struct
  {
    private MonotonicDirection direction;

    internal Counter(IMeter meter, string name, string unit = null, string description = null)
      : base(meter, name, unit, description)
    {
      this.direction = MonotonicDirection.Unknown;
    }

    protected override void RecordMeasurement(
      T measurement,
      ReadOnlySpan<KeyValuePair<string, object>> tags)
    {
      int num = Comparer<T>.Default.Compare(measurement, default (T));
      if (this.direction == MonotonicDirection.Unknown)
        this.direction = num < 0 ? MonotonicDirection.Negative : MonotonicDirection.Positive;
      if (num < 0 && this.direction == MonotonicDirection.Positive || num > 0 && this.direction == MonotonicDirection.Negative)
        throw new NonMonotonicOperationException();
      base.RecordMeasurement(measurement, tags);
    }
  }
}
