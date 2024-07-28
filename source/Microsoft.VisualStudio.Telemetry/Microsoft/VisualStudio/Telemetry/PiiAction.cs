// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.PiiAction
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class PiiAction : IEventProcessorAction, IEventProcessorActionDiagnostics
  {
    private const string UnknownValue = "Unknown";
    private readonly HashSet<string> piiedProperties = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private readonly IPiiPropertyProcessor piiPropertyProcessor;
    private int totalPiiProperties;

    public int Priority => 100;

    public PiiAction(IPiiPropertyProcessor piiPropertyProcessor)
    {
      piiPropertyProcessor.RequiresArgumentNotNull<IPiiPropertyProcessor>(nameof (piiPropertyProcessor));
      this.piiPropertyProcessor = piiPropertyProcessor;
    }

    public bool Execute(IEventProcessorContext eventProcessorContext)
    {
      eventProcessorContext.RequiresArgumentNotNull<IEventProcessorContext>(nameof (eventProcessorContext));
      TelemetryEvent telemetryEvent = eventProcessorContext.TelemetryEvent;
      List<KeyValuePair<string, object>> keyValuePairList = new List<KeyValuePair<string, object>>();
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) telemetryEvent.Properties)
      {
        if (property.Value != null)
        {
          int num1 = property.Value.GetType() == this.piiPropertyProcessor.TypeOfHashedProperty() ? 1 : 0;
          bool flag = property.Value.GetType() == this.piiPropertyProcessor.TypeOfPiiProperty();
          int num2 = flag ? 1 : 0;
          if ((num1 | num2) != 0)
            keyValuePairList.Add(new KeyValuePair<string, object>(property.Key, (object) this.piiPropertyProcessor.ConvertToHashedValue(property.Value)));
          if (flag)
          {
            if (this.piiPropertyProcessor.CanAddRawValue(eventProcessorContext))
              keyValuePairList.Add(new KeyValuePair<string, object>(this.piiPropertyProcessor.BuildRawPropertyName(property.Key), this.piiPropertyProcessor.ConvertToRawValue(property.Value)));
            this.piiedProperties.Add(property.Key);
            ++this.totalPiiProperties;
          }
        }
      }
      foreach (KeyValuePair<string, object> keyValuePair in keyValuePairList)
        telemetryEvent.Properties[keyValuePair.Key] = keyValuePair.Value;
      return true;
    }

    public void PostDiagnosticInformation(
      TelemetrySession mainSession,
      TelemetryManifest newManifest)
    {
      if (this.totalPiiProperties == 0)
        return;
      string str = "Unknown";
      TelemetryManifest currentManifest = mainSession.EventProcessor.CurrentManifest;
      if (currentManifest != null)
        str = currentManifest.Version;
      mainSession.PostEvent(new TelemetryEvent("VS/TelemetryApi/PiiProperties")
      {
        Properties = {
          ["VS.TelemetryApi.DynamicTelemetry.Manifest.Version"] = (object) str,
          ["VS.TelemetryApi.DynamicTelemetry.HostName"] = (object) mainSession.HostName,
          ["VS.TelemetryApi.PiiProperties.TotalCount"] = (object) this.totalPiiProperties,
          ["VS.TelemetryApi.PiiProperties.Properties"] = (object) this.piiedProperties.Select<string, string>((Func<string, string>) (x => x.ToLower(CultureInfo.InvariantCulture))).Join(",")
        }
      });
    }
  }
}
