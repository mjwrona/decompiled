// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.Meter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Metrics.Exceptions;
using System;

namespace Microsoft.VisualStudio.Telemetry.Metrics
{
  public class Meter : IMeter, IDisposable
  {
    private readonly string name;
    private readonly string version;

    public string Name => this.name;

    public string Version => this.version;

    public Meter(string name) => this.name = name;

    public Meter(string name, string version)
    {
      this.name = name;
      this.version = version;
    }

    public ICounter<T> CreateCounter<T>(string name, string unit = null, string description = null) where T : struct
    {
      Meter.ValidateNumericType(typeof (T));
      return (ICounter<T>) new Counter<T>((IMeter) this, name, unit, description);
    }

    public ICounter<T> CreateUpDownCounter<T>(string name, string unit = null, string description = null) where T : struct
    {
      Meter.ValidateNumericType(typeof (T));
      return (ICounter<T>) new UpDownCounter<T>((IMeter) this, name, unit, description);
    }

    public IVSCounter<T> CreateVSCounter<T>(string name, string unit = null, string description = null) where T : struct
    {
      Meter.ValidateNumericType(typeof (T));
      return (IVSCounter<T>) new Counter<T>((IMeter) this, name, unit, description);
    }

    public IVSCounter<T> CreateVSUpDownCounter<T>(string name, string unit = null, string description = null) where T : struct
    {
      Meter.ValidateNumericType(typeof (T));
      return (IVSCounter<T>) new UpDownCounter<T>((IMeter) this, name, unit, description);
    }

    public IHistogram<T> CreateHistogram<T>(string name, string unit = null, string description = null) where T : struct
    {
      Meter.ValidateNumericType(typeof (T));
      return (IHistogram<T>) new Histogram<T>((IMeter) this, name, unit, description);
    }

    public IHistogram<T> CreateHistogram<T>(
      string name,
      HistogramConfiguration configuration = null,
      string unit = null,
      string description = null)
      where T : struct
    {
      Meter.ValidateNumericType(typeof (T));
      HistogramConfiguration configuration1 = configuration ?? new HistogramConfiguration();
      return (IHistogram<T>) new Histogram<T>((IMeter) this, name, configuration1, unit, description);
    }

    public IVSHistogram<T> CreateVSHistogram<T>(string name, string unit = null, string description = null) where T : struct
    {
      Meter.ValidateNumericType(typeof (T));
      return (IVSHistogram<T>) new Histogram<T>((IMeter) this, name, unit, description);
    }

    public IVSHistogram<T> CreateVSHistogram<T>(
      string name,
      HistogramConfiguration configuration = null,
      string unit = null,
      string description = null)
      where T : struct
    {
      Meter.ValidateNumericType(typeof (T));
      HistogramConfiguration configuration1 = configuration ?? new HistogramConfiguration();
      return (IVSHistogram<T>) new Histogram<T>((IMeter) this, name, configuration1, unit, description);
    }

    public void Dispose()
    {
    }

    private static void ValidateNumericType(Type type)
    {
      if (type != typeof (byte) && type != typeof (short) && type != typeof (int) && type != typeof (long) && type != typeof (double) && type != typeof (float) && type != typeof (Decimal))
        throw new UnsupportedNumericStructException();
    }
  }
}
