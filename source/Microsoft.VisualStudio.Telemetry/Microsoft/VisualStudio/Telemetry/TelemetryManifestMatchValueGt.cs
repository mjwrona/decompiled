// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestMatchValueGt
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestMatchValueGt : ITelemetryManifestMatchValue
  {
    [JsonProperty(PropertyName = "gt", Required = Required.Always)]
    public double Gt { get; set; }

    public bool IsMatch(object valueToCompare) => valueToCompare != null && TypeTools.IsNumericType(valueToCompare.GetType()) && Convert.ToDouble(valueToCompare, (IFormatProvider) null) > this.Gt;

    public void Validate()
    {
      if (double.IsNaN(this.Gt) || double.IsInfinity(this.Gt))
        throw new TelemetryManifestValidationException("'gt' must be valid double value");
    }
  }
}
