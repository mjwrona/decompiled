// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryNullEtwProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class TelemetryNullEtwProvider : ITelemetryEtwProvider
  {
    public void WriteActivityEndWithDurationEvent(TelemetryActivity activity)
    {
    }

    public void WriteActivityPostEvent(TelemetryActivity activity, TelemetrySession session)
    {
    }

    public void WriteActivityStartEvent(TelemetryActivity activity)
    {
    }

    public void WriteActivityStopEvent(TelemetryActivity activity)
    {
    }

    public void WriteTelemetryPostEvent(TelemetryEvent telemetryEvent, TelemetrySession session)
    {
    }
  }
}
