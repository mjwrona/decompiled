// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.DataModelPropertyAction`1
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
  internal abstract class DataModelPropertyAction<T> : 
    IEventProcessorAction,
    IEventProcessorActionDiagnostics
    where T : TelemetryDataModelProperty
  {
    private readonly HashSet<string> properties = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private const string UnknownValue = "Unknown";
    private int totalPropertyCount;
    private string flagPropertyName;

    public int Priority { get; }

    public string SuffixName { get; }

    public string DiagnosticName { get; }

    public DataModelPropertyAction(
      int priority,
      string suffixName,
      string flagName,
      string diagnosticName)
    {
      this.Priority = priority;
      this.SuffixName = suffixName;
      this.DiagnosticName = diagnosticName;
      this.flagPropertyName = "Reserved.DataModel." + flagName;
    }

    public bool Execute(IEventProcessorContext eventProcessorContext)
    {
      eventProcessorContext.RequiresArgumentNotNull<IEventProcessorContext>(nameof (eventProcessorContext));
      TelemetryEvent telemetryEvent = eventProcessorContext.TelemetryEvent;
      KeyValuePair<string, T>[] array = telemetryEvent.Properties.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (prop => prop.Value is T)).Select<KeyValuePair<string, object>, KeyValuePair<string, T>>((Func<KeyValuePair<string, object>, KeyValuePair<string, T>>) (prop => new KeyValuePair<string, T>(prop.Key, prop.Value as T))).ToArray<KeyValuePair<string, T>>();
      this.totalPropertyCount += array.Length;
      if (((IEnumerable<KeyValuePair<string, T>>) array).Any<KeyValuePair<string, T>>())
        telemetryEvent.Properties[this.flagPropertyName] = (object) true;
      foreach (KeyValuePair<string, T> keyValuePair in array)
      {
        telemetryEvent.Properties[keyValuePair.Key + this.SuffixName] = keyValuePair.Value.Value;
        telemetryEvent.Properties.Remove(keyValuePair.Key);
        this.properties.Add(keyValuePair.Key);
      }
      return true;
    }

    public void PostDiagnosticInformation(
      TelemetrySession mainSession,
      TelemetryManifest newManifest)
    {
      if (this.totalPropertyCount == 0)
        return;
      string str = "Unknown";
      TelemetryManifest currentManifest = mainSession.EventProcessor.CurrentManifest;
      if (currentManifest != null)
        str = currentManifest.Version;
      mainSession.PostEvent(new TelemetryEvent("VS/TelemetryApi/" + this.DiagnosticName)
      {
        Properties = {
          ["VS.TelemetryApi.DynamicTelemetry.Manifest.Version"] = (object) str,
          ["VS.TelemetryApi.DynamicTelemetry.HostName"] = (object) mainSession.HostName,
          ["VS.TelemetryApi." + this.DiagnosticName + ".TotalCount"] = (object) this.totalPropertyCount,
          ["VS.TelemetryApi." + this.DiagnosticName + ".Properties"] = (object) this.properties.Select<string, string>((Func<string, string>) (x => x.ToLower(CultureInfo.InvariantCulture))).Join(",")
        }
      });
    }
  }
}
