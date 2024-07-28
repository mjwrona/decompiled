// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryJsonLogFile
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryJsonLogFile : BaseJsonLogFile<TelemetryEvent>
  {
    public TelemetryJsonLogFile(ITelemetryWriter writer = null)
      : base(writer)
    {
    }

    protected override string ConvertEventToString(TelemetryEvent eventData)
    {
      try
      {
        return JsonConvert.SerializeObject((object) new TelemetryJsonLogFile.TelemetryLoggerEventSnapshot(eventData));
      }
      catch (JsonSerializationException ex)
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\"Name\":\"{0}\",\"Error\":\"Cannot serialize event. Error: {1}\"}}", new object[2]
        {
          (object) eventData.Name,
          (object) ex.Message
        });
      }
    }

    public sealed class TelemetryLoggerEventSnapshot
    {
      public string Name { get; }

      public IDictionary<string, string> Properties { get; }

      public TelemetryLoggerEventSnapshot(TelemetryEvent telemetryEvent)
      {
        telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
        this.Name = telemetryEvent.Name;
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) telemetryEvent.Properties)
          dictionary[property.Key] = property.Value != null ? property.Value.ToString() : "null";
        this.Properties = (IDictionary<string, string>) dictionary;
      }
    }
  }
}
