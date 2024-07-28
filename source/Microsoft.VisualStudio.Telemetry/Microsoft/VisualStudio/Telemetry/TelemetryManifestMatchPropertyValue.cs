// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestMatchPropertyValue
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestMatchPropertyValue : ITelemetryManifestMatch, ITelemetryEventMatch
  {
    [JsonIgnore]
    private string propertyName;

    [JsonProperty(PropertyName = "property", Required = Required.Always)]
    public string Property
    {
      get => this.propertyName;
      set
      {
        value.RequiresArgumentNotNullAndNotEmpty(nameof (value));
        this.propertyName = value.ToLower(CultureInfo.InvariantCulture);
      }
    }

    [JsonProperty(PropertyName = "value", Required = Required.Always)]
    public ITelemetryManifestMatchValue Value { get; set; }

    public IEnumerable<ITelemetryManifestMatch> GetChildren() => Enumerable.Empty<ITelemetryManifestMatch>();

    public bool IsEventMatch(TelemetryEvent telemetryEvent)
    {
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      object valueToCompare;
      return telemetryEvent.Properties.TryGetValue(this.Property, out valueToCompare) && this.Value.IsMatch(valueToCompare);
    }

    void ITelemetryManifestMatch.ValidateItself()
    {
      if (this.Property.IsNullOrWhiteSpace())
        throw new TelemetryManifestValidationException("'property' can't be empty");
      if (this.Value == null)
        throw new TelemetryManifestValidationException("'value' can't be null");
      this.Value.Validate();
    }
  }
}
