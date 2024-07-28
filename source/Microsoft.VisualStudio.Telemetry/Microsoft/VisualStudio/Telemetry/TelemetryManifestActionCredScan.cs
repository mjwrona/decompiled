// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestActionCredScan
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestActionCredScan : ITelemetryManifestAction, IEventProcessorAction
  {
    [JsonProperty(PropertyName = "credScanProperties", Required = Required.Always)]
    public IEnumerable<string> Properties { get; set; }

    public int Priority => 7;

    public bool Execute(IEventProcessorContext eventProcessorContext)
    {
      eventProcessorContext.RequiresArgumentNotNull<IEventProcessorContext>(nameof (eventProcessorContext));
      TelemetryEvent telemetryEvent = eventProcessorContext.TelemetryEvent;
      if (this.Properties != null && this.Properties.Any<string>())
      {
        foreach (string property in this.Properties)
        {
          object val;
          if (telemetryEvent.Properties.TryGetValue(property, out val))
          {
            switch (val)
            {
              case TelemetryCredScanProperty _:
                continue;
              case TelemetryPiiProperty telemetryPiiProperty:
                telemetryEvent.Properties[property] = (object) new TelemetryCredScanProperty(telemetryPiiProperty.RawValue);
                continue;
              case TelemetryHashedProperty _:
                goto label_11;
              default:
                telemetryEvent.Properties[property] = (object) new TelemetryCredScanProperty(val);
                continue;
            }
          }
        }
      }
label_11:
      return true;
    }

    public void Validate()
    {
      if (this.Properties.Any<string>(new Func<string, bool>(StringExtensions.IsNullOrWhiteSpace)))
        throw new TelemetryManifestValidationException("'properties' are invalid");
    }
  }
}
