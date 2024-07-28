// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestLegacyStreamRouteArgs
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestLegacyStreamRouteArgs : ITelemetryManifestRouteArgs
  {
    [JsonProperty(PropertyName = "streamId", Required = Required.Always)]
    public uint StreamId { get; set; }

    [JsonProperty(PropertyName = "properties", Required = Required.Always)]
    public IEnumerable<ITelemetryManifestRouteArgs> Properties { get; set; }

    public void Validate()
    {
      if (this.StreamId == 0U)
        throw new TelemetryManifestValidationException("streamId should not be 0");
      if (this.Properties == null)
        throw new TelemetryManifestValidationException("properties are null");
      if (!this.Properties.Any<ITelemetryManifestRouteArgs>())
        throw new TelemetryManifestValidationException("there are no properties");
      if (this.Properties.Any<ITelemetryManifestRouteArgs>((Func<ITelemetryManifestRouteArgs, bool>) (x => x.GetType() != typeof (TelemetryManifestLegacyStreamPropertyRouteArgs))))
        throw new TelemetryManifestValidationException("properties contain incorrect children");
      foreach (ITelemetryManifestRouteArgs property in this.Properties)
        property.Validate();
    }
  }
}
