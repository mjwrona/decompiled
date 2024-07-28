// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestMatchValueStartsWith
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestMatchValueStartsWith : ITelemetryManifestMatchValue
  {
    [JsonProperty(PropertyName = "startsWith", Required = Required.Always)]
    public string StartsWith { get; set; }

    public bool IsMatch(object valueToCompare) => valueToCompare != null && valueToCompare.ToString().StartsWith(this.StartsWith, StringComparison.OrdinalIgnoreCase);

    public void Validate()
    {
      if (string.IsNullOrEmpty(this.StartsWith))
        throw new TelemetryManifestValidationException("'startsWith' can't be null or empty");
    }
  }
}
