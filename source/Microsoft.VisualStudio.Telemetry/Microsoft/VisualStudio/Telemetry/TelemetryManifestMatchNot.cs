// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestMatchNot
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestMatchNot : ITelemetryManifestMatch, ITelemetryEventMatch
  {
    [JsonProperty(PropertyName = "not", Required = Required.Always)]
    public ITelemetryManifestMatch Not { get; set; }

    public IEnumerable<ITelemetryManifestMatch> GetChildren()
    {
      yield return this.Not;
    }

    public bool IsEventMatch(TelemetryEvent telemetryEvent)
    {
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      return !this.Not.IsEventMatch(telemetryEvent);
    }

    void ITelemetryManifestMatch.ValidateItself()
    {
    }
  }
}
