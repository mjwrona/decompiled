// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryScopeSettings
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry
{
  public sealed class TelemetryScopeSettings
  {
    public IDictionary<string, object> StartEventProperties { get; set; }

    public TelemetrySeverity Severity { get; set; }

    public bool IsOptOutFriendly { get; set; }

    public TelemetryEventCorrelation[] Correlations { get; set; }

    public bool PostStartEvent { get; set; }

    public TelemetryScopeSettings()
    {
      this.StartEventProperties = (IDictionary<string, object>) null;
      this.Severity = TelemetrySeverity.Normal;
      this.IsOptOutFriendly = false;
      this.Correlations = (TelemetryEventCorrelation[]) null;
      this.PostStartEvent = true;
    }
  }
}
