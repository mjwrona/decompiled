// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestActionHashed
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
  internal class TelemetryManifestActionHashed : ITelemetryManifestAction, IEventProcessorAction
  {
    [JsonProperty(PropertyName = "hashProperties")]
    public virtual IEnumerable<string> Properties { get; set; }

    [JsonIgnore]
    public virtual int Priority => 5;

    public virtual bool Execute(IEventProcessorContext eventProcessorContext)
    {
      eventProcessorContext.RequiresArgumentNotNull<IEventProcessorContext>(nameof (eventProcessorContext));
      TelemetryEvent telemetryEvent = eventProcessorContext.TelemetryEvent;
      if (this.Properties != null && this.Properties.Any<string>())
      {
        foreach (string property in this.Properties)
        {
          object val;
          if (telemetryEvent.Properties.TryGetValue(property, out val) && (!(val is TelemetryHashedProperty) || val is TelemetryPiiProperty))
            telemetryEvent.Properties[property] = (object) new TelemetryHashedProperty(val);
        }
      }
      return true;
    }

    public virtual void Validate()
    {
      if (this.Properties.Any<string>(new Func<string, bool>(StringExtensions.IsNullOrWhiteSpace)))
        throw new TelemetryManifestValidationException("an entry in 'properties' cannot be null or whitespace");
    }
  }
}
