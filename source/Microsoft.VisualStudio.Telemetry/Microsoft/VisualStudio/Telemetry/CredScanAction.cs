// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.CredScanAction
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class CredScanAction : IEventProcessorAction
  {
    internal const string ElapsedTimePropertyName = "vs.telemetryapi.credscan.elapsedms";

    public int Priority => 90;

    public bool Execute(IEventProcessorContext eventProcessorContext)
    {
      eventProcessorContext.RequiresArgumentNotNull<IEventProcessorContext>(nameof (eventProcessorContext));
      TelemetryEvent telemetryEvent = eventProcessorContext.TelemetryEvent;
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      List<long> source = new List<long>();
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) telemetryEvent.Properties)
      {
        if (property.Value is TelemetryCredScanProperty credScanProperty)
        {
          keyValuePairList.Add(new KeyValuePair<string, string>(property.Key, credScanProperty.StringValue));
          source.Add(credScanProperty.ElapsedTimeInMs);
        }
      }
      foreach (KeyValuePair<string, string> keyValuePair in keyValuePairList)
        telemetryEvent.Properties[keyValuePair.Key] = (object) keyValuePair.Value;
      if (source.Any<long>())
        telemetryEvent.Properties["vs.telemetryapi.credscan.elapsedms"] = (object) JsonConvert.SerializeObject((object) source);
      return true;
    }
  }
}
