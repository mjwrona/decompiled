// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestMatchOr
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestMatchOr : ITelemetryManifestMatch, ITelemetryEventMatch
  {
    [JsonProperty(PropertyName = "or", Required = Required.Always)]
    public IEnumerable<ITelemetryManifestMatch> Or { get; set; }

    public IEnumerable<ITelemetryManifestMatch> GetChildren() => this.Or;

    public bool IsEventMatch(TelemetryEvent telemetryEvent)
    {
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      if (this.Or is List<ITelemetryManifestMatch> or)
      {
        foreach (ITelemetryEventMatch telemetryEventMatch in or)
        {
          if (telemetryEventMatch.IsEventMatch(telemetryEvent))
            return true;
        }
      }
      else
      {
        foreach (ITelemetryEventMatch telemetryEventMatch in this.Or)
        {
          if (telemetryEventMatch.IsEventMatch(telemetryEvent))
            return true;
        }
      }
      return false;
    }

    void ITelemetryManifestMatch.ValidateItself()
    {
      if (!this.Or.Any<ITelemetryManifestMatch>())
        throw new TelemetryManifestValidationException("there are no operands in the 'or' clause");
    }
  }
}
