// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryEventMatchByName
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  public class TelemetryEventMatchByName : ITelemetryEventMatch
  {
    public string EventName { get; private set; }

    public bool IsFullNameCheck { get; private set; }

    public TelemetryEventMatchByName(string eventName, bool isFullNameCheck)
    {
      eventName.RequiresArgumentNotNull<string>(nameof (eventName));
      this.EventName = eventName;
      this.IsFullNameCheck = isFullNameCheck;
    }

    public bool IsEventMatch(TelemetryEvent telemetryEvent)
    {
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      return this.IsFullNameCheck ? telemetryEvent.Name.Equals(this.EventName, StringComparison.OrdinalIgnoreCase) : telemetryEvent.Name.StartsWith(this.EventName, StringComparison.OrdinalIgnoreCase);
    }
  }
}
