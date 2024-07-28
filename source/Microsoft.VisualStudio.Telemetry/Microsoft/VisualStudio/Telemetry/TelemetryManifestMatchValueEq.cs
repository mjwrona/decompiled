// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestMatchValueEq
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestMatchValueEq : ITelemetryManifestMatchValue
  {
    [JsonProperty(PropertyName = "eq", Required = Required.Always)]
    public string Eq { get; set; }

    public bool IsMatch(object valueToCompare) => valueToCompare != null && string.Compare(this.Eq, valueToCompare.ToString(), StringComparison.OrdinalIgnoreCase) == 0;

    public void Validate()
    {
      if (this.Eq == null)
        throw new TelemetryManifestValidationException("'eq' can't be null");
    }
  }
}
