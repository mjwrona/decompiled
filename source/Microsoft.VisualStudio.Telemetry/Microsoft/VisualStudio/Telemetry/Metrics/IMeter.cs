// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.IMeter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;

namespace Microsoft.VisualStudio.Telemetry.Metrics
{
  public interface IMeter : IDisposable
  {
    string Name { get; }

    string Version { get; }

    ICounter<T> CreateCounter<T>(string name, string unit = null, string description = null) where T : struct;

    ICounter<T> CreateUpDownCounter<T>(string name, string unit = null, string description = null) where T : struct;

    IVSCounter<T> CreateVSCounter<T>(string name, string unit = null, string description = null) where T : struct;

    IVSCounter<T> CreateVSUpDownCounter<T>(string name, string unit = null, string description = null) where T : struct;

    IHistogram<T> CreateHistogram<T>(string name, string unit = null, string description = null) where T : struct;

    IHistogram<T> CreateHistogram<T>(
      string name,
      HistogramConfiguration configuration,
      string unit = null,
      string description = null)
      where T : struct;

    IVSHistogram<T> CreateVSHistogram<T>(string name, string unit = null, string description = null) where T : struct;

    IVSHistogram<T> CreateVSHistogram<T>(
      string name,
      HistogramConfiguration configuration,
      string unit = null,
      string description = null)
      where T : struct;
  }
}
