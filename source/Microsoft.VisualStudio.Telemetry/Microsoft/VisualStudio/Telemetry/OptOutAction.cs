// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.OptOutAction
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class OptOutAction : IEventProcessorAction
  {
    private readonly HashSet<string> optoutFriendlyEvents = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> optoutFriendlyProperties = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public int Priority => 2;

    public bool Execute(IEventProcessorContext eventProcessorContext)
    {
      if (!eventProcessorContext.HostTelemetrySession.IsOptedIn)
      {
        TelemetryEvent telemetryEvent = eventProcessorContext.TelemetryEvent;
        if (!telemetryEvent.IsOptOutFriendly && !this.optoutFriendlyEvents.Contains(telemetryEvent.Name))
          eventProcessorContext.IsEventDropped = true;
        foreach (string propertyName in new List<string>((IEnumerable<string>) telemetryEvent.Properties.Keys))
        {
          if ((!telemetryEvent.IsOptOutFriendly || TelemetryEvent.IsPropertyNameReserved(propertyName) || TelemetryContext.IsPropertyNameReserved(propertyName)) && !this.optoutFriendlyProperties.Contains(propertyName))
            eventProcessorContext.ExcludePropertyFromEvent(propertyName);
        }
      }
      return true;
    }

    public void AddOptOutFriendlyEventName(string eventName)
    {
      eventName.RequiresArgumentNotNull<string>(nameof (eventName));
      this.optoutFriendlyEvents.Add(eventName);
    }

    public void AddOptOutFriendlyPropertiesList(IEnumerable<string> propertyNameList)
    {
      propertyNameList.RequiresArgumentNotNull<IEnumerable<string>>(nameof (propertyNameList));
      this.optoutFriendlyProperties.UnionWith(propertyNameList);
    }
  }
}
