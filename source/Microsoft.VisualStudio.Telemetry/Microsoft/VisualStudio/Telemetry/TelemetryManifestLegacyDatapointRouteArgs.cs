// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestLegacyDatapointRouteArgs
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestLegacyDatapointRouteArgs : ITelemetryManifestRouteArgs
  {
    [JsonProperty(PropertyName = "datapointId", Required = Required.Always)]
    public uint DatapointId { get; set; }

    [JsonProperty(PropertyName = "dataType", Required = Required.Always)]
    public LegacyDatapointType DataType { get; set; }

    [JsonProperty(PropertyName = "parameterName", Required = Required.Always)]
    public string ParameterName { get; set; }

    [JsonProperty(PropertyName = "truncationRule")]
    public LegacyStringTruncationRule TruncationRule { get; set; }

    public void Validate()
    {
      if (this.DatapointId == 0U)
        throw new TelemetryManifestValidationException("datapointId should not be 0");
      if (this.ParameterName.IsNullOrWhiteSpace())
        throw new TelemetryManifestValidationException("parameter name must not be empty");
    }
  }
}
