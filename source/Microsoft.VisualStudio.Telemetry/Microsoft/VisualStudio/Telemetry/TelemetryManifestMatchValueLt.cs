// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestMatchValueLt
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestMatchValueLt : ITelemetryManifestMatchValue
  {
    [JsonProperty(PropertyName = "lt", Required = Required.Always)]
    public double Lt { get; set; }

    public bool IsMatch(object valueToCompare) => valueToCompare != null && TypeTools.IsNumericType(valueToCompare.GetType()) && Convert.ToDouble(valueToCompare, (IFormatProvider) null) < this.Lt;

    public void Validate()
    {
      if (double.IsNaN(this.Lt) || double.IsInfinity(this.Lt))
        throw new TelemetryManifestValidationException("'lt' must be valid double value");
    }
  }
}
