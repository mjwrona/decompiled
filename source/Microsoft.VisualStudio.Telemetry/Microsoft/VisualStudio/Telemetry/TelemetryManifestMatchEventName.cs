// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestMatchEventName
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestMatchEventName : ITelemetryManifestMatch, ITelemetryEventMatch
  {
    [JsonIgnore]
    private TelemetryEventMatchByName eventMatchFilter;

    [JsonProperty(PropertyName = "event", Required = Required.Always)]
    public string EventName
    {
      get
      {
        if (this.eventMatchFilter == null)
          return (string) null;
        return !this.eventMatchFilter.IsFullNameCheck ? this.eventMatchFilter.EventName + "*" : this.eventMatchFilter.EventName;
      }
      set
      {
        value.RequiresArgumentNotNullAndNotEmpty(nameof (value));
        string lower = value.ToLower(CultureInfo.InvariantCulture);
        if (lower.EndsWith("*", StringComparison.Ordinal))
          this.eventMatchFilter = new TelemetryEventMatchByName(lower.Remove(lower.Length - 1), false);
        else
          this.eventMatchFilter = new TelemetryEventMatchByName(lower, true);
      }
    }

    public bool IsEventMatch(TelemetryEvent telemetryEvent)
    {
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      return this.eventMatchFilter.IsEventMatch(telemetryEvent);
    }

    public IEnumerable<ITelemetryManifestMatch> GetChildren() => Enumerable.Empty<ITelemetryManifestMatch>();

    void ITelemetryManifestMatch.ValidateItself()
    {
      if (string.IsNullOrEmpty(this.EventName))
        throw new TelemetryManifestValidationException("'event' name can't be null");
    }
  }
}
