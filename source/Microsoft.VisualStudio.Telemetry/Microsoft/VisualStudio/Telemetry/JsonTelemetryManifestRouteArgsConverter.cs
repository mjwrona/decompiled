// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.JsonTelemetryManifestRouteArgsConverter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class JsonTelemetryManifestRouteArgsConverter : 
    JsonCreationConverter<ITelemetryManifestRouteArgs>
  {
    internal override ITelemetryManifestRouteArgs Create(Type objectType, JObject jsonObject)
    {
      if (this.FieldExists("datapointId", jsonObject))
        return (ITelemetryManifestRouteArgs) new TelemetryManifestLegacyDatapointRouteArgs();
      if (this.FieldExists("streamId", jsonObject))
        return (ITelemetryManifestRouteArgs) new TelemetryManifestLegacyStreamRouteArgs();
      return this.FieldExists("propertyName", jsonObject) ? (ITelemetryManifestRouteArgs) new TelemetryManifestLegacyStreamPropertyRouteArgs() : (ITelemetryManifestRouteArgs) new TelemetryManifestInvalidRouteArgs();
    }
  }
}
