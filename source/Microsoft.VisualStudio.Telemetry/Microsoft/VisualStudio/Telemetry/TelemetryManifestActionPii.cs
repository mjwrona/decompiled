// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestActionPii
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
  internal class TelemetryManifestActionPii : 
    TelemetryManifestActionHashed,
    ITelemetryManifestAction,
    IEventProcessorAction
  {
    [JsonProperty(PropertyName = "piiProperties", Required = Required.Always)]
    public override IEnumerable<string> Properties { get; set; }

    [JsonIgnore]
    public override int Priority => 6;

    public override bool Execute(IEventProcessorContext eventProcessorContext)
    {
      eventProcessorContext.RequiresArgumentNotNull<IEventProcessorContext>(nameof (eventProcessorContext));
      TelemetryEvent telemetryEvent = eventProcessorContext.TelemetryEvent;
      foreach (string property in this.Properties)
      {
        object val;
        if (telemetryEvent.Properties.TryGetValue(property, out val))
        {
          switch (val)
          {
            case TelemetryPiiProperty _:
            case TelemetryHashedProperty _:
              continue;
            default:
              telemetryEvent.Properties[property] = (object) new TelemetryPiiProperty(val);
              continue;
          }
        }
      }
      return true;
    }

    public override void Validate()
    {
      if (this.Properties == null)
        throw new TelemetryManifestValidationException("'properties' is null");
      if (!this.Properties.Any<string>())
        throw new TelemetryManifestValidationException("'properties' action must contain at least one element");
      if (this.Properties.Any<string>(new Func<string, bool>(StringExtensions.IsNullOrWhiteSpace)))
        throw new TelemetryManifestValidationException("an entry in 'properties' cannot be null or whitespace");
    }
  }
}
