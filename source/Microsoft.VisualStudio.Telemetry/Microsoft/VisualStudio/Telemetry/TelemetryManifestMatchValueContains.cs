// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestMatchValueContains
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestMatchValueContains : ITelemetryManifestMatchValue
  {
    [JsonProperty(PropertyName = "contains", Required = Required.Always)]
    public string Contains { get; set; }

    public bool IsMatch(object valueToCompare) => valueToCompare != null && valueToCompare.ToString().IndexOf(this.Contains, StringComparison.OrdinalIgnoreCase) != -1;

    public void Validate()
    {
      if (string.IsNullOrEmpty(this.Contains))
        throw new TelemetryManifestValidationException("'contains' can't be null or empty");
    }
  }
}
