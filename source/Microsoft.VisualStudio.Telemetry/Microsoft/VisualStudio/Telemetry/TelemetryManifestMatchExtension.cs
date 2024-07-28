// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestMatchExtension
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal static class TelemetryManifestMatchExtension
  {
    public static IEnumerable<ITelemetryManifestMatch> GetDescendants(
      this ITelemetryManifestMatch match)
    {
      return (match.GetChildren() ?? Enumerable.Empty<ITelemetryManifestMatch>()).Where<ITelemetryManifestMatch>((Func<ITelemetryManifestMatch, bool>) (c => c != null)).SelectMany<ITelemetryManifestMatch, ITelemetryManifestMatch>((Func<ITelemetryManifestMatch, IEnumerable<ITelemetryManifestMatch>>) (c => c.GetDescendantsAndItself()));
    }

    public static IEnumerable<ITelemetryManifestMatch> GetDescendantsAndItself(
      this ITelemetryManifestMatch match)
    {
      return match.Enumerate<ITelemetryManifestMatch>().Concat<ITelemetryManifestMatch>(match.GetDescendants());
    }

    public static void Validate(this ITelemetryManifestMatch match)
    {
      foreach (ITelemetryManifestMatch telemetryManifestMatch in match.GetDescendantsAndItself())
        telemetryManifestMatch.ValidateItself();
    }
  }
}
