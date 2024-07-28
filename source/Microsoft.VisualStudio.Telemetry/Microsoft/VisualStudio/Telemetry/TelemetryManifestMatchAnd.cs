// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestMatchAnd
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestMatchAnd : ITelemetryManifestMatch, ITelemetryEventMatch
  {
    [JsonProperty(PropertyName = "and", Required = Required.Always)]
    public IEnumerable<ITelemetryManifestMatch> And { get; set; }

    public IEnumerable<ITelemetryManifestMatch> GetChildren() => this.And;

    public bool IsEventMatch(TelemetryEvent telemetryEvent)
    {
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      if (this.And is List<ITelemetryManifestMatch> and)
      {
        foreach (ITelemetryEventMatch telemetryEventMatch in and)
        {
          if (!telemetryEventMatch.IsEventMatch(telemetryEvent))
            return false;
        }
      }
      else
      {
        foreach (ITelemetryEventMatch telemetryEventMatch in this.And)
        {
          if (!telemetryEventMatch.IsEventMatch(telemetryEvent))
            return false;
        }
      }
      return true;
    }

    void ITelemetryManifestMatch.ValidateItself()
    {
      if (!this.And.Any<ITelemetryManifestMatch>())
        throw new TelemetryManifestValidationException("there are no operands in the 'and' clause");
    }
  }
}
