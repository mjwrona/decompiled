// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestMatchValueExists
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestMatchValueExists : ITelemetryManifestMatchValue
  {
    [JsonProperty(PropertyName = "exists", Required = Required.Always)]
    public bool Exists { get; set; }

    public bool IsMatch(object valueToCompare) => true;

    public void Validate()
    {
      if (!this.Exists)
        throw new TelemetryManifestValidationException("the only allowable value for 'exists' is true");
    }
  }
}
