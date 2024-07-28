// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.VSTelemetryMeterProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Metrics.Exceptions;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Telemetry.Metrics
{
  public class VSTelemetryMeterProvider : IMeterProvider
  {
    private static readonly Regex InstrumentNameRegex = new Regex("^[a-z][a-z0-9-._]{0,62}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public IMeter CreateMeter(string name) => this.CreateMeter(name, (string) null);

    public IMeter CreateMeter(string name, string version = null) => VSTelemetryMeterProvider.IsValidInstrumentName(name) ? (IMeter) new Meter(name, version) : throw new InvalidMeterNameException();

    public static bool IsValidInstrumentName(string instrumentName) => !string.IsNullOrWhiteSpace(instrumentName) && VSTelemetryMeterProvider.InstrumentNameRegex.IsMatch(instrumentName);
  }
}
