// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryCredScanProperty
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber;
using Microsoft.VisualStudio.Utilities.Internal;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryCredScanProperty
  {
    internal static string ReplacementText = "CRED_REDACTED";
    private CredScanDataScrubber scrubber = new CredScanDataScrubber();

    public string StringValue { get; }

    public long ElapsedTimeInMs { get; }

    public TelemetryCredScanProperty(object val)
    {
      val.RequiresArgumentNotNull<object>(nameof (val));
      string propertyValue = TypeTools.ConvertToString(val);
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      this.StringValue = this.scrubber.ContainsSensitiveData(propertyValue) ? TelemetryCredScanProperty.ReplacementText : propertyValue;
      stopwatch.Stop();
      this.ElapsedTimeInMs = stopwatch.ElapsedMilliseconds;
    }
  }
}
