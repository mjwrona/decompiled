// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.Management.RegisteredMetric
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Metrics.Events;
using System;

namespace Microsoft.VisualStudio.Telemetry.Metrics.Management
{
  internal class RegisteredMetric
  {
    internal string Key { get; private set; }

    internal TelemetryMetricEvent MetricEvent { get; private set; }

    internal DateTime Expiry { get; private set; }

    public RegisteredMetric(string key, TelemetryMetricEvent metricEvent)
    {
      this.Key = key;
      this.MetricEvent = metricEvent;
    }

    public void UpdateExpirationTime(TimeSpan timeout) => this.Expiry = DateTime.UtcNow + timeout;
  }
}
