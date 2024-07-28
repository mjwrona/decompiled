// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestInvalidMatchItem
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestInvalidMatchItem : ITelemetryManifestMatch, ITelemetryEventMatch
  {
    public IEnumerable<ITelemetryManifestMatch> GetChildren() => Enumerable.Empty<ITelemetryManifestMatch>();

    public bool IsEventMatch(TelemetryEvent telemetryEvent) => throw new NotImplementedException();

    void ITelemetryManifestMatch.ValidateItself() => throw new TelemetryManifestValidationException("invalid matching item");
  }
}
