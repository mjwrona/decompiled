// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.SuppressEmptyPostPropertyEventAction
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class SuppressEmptyPostPropertyEventAction : IEventProcessorAction
  {
    public int Priority => 300;

    public bool Execute(IEventProcessorContext eventProcessorContext)
    {
      TelemetryEvent telemetryEvent = eventProcessorContext.TelemetryEvent;
      return !TelemetryContext.IsEventNameContextPostProperty(telemetryEvent.Name) || telemetryEvent.Properties.Any<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (p => !TelemetryEvent.IsPropertyNameReserved(p.Key) && !TelemetryContext.IsPropertyNameReserved(p.Key)));
    }
  }
}
