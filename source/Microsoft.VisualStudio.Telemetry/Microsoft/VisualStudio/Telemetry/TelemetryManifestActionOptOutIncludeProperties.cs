// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestActionOptOutIncludeProperties
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestActionOptOutIncludeProperties : 
    TelemetryManifestActionOptOutPropertiesBase
  {
    [JsonProperty(PropertyName = "optOutIncludeProperties", Required = Required.Always)]
    public IEnumerable<string> Properties
    {
      get => this.PropertiesImpl;
      set
      {
        value.RequiresArgumentNotNull<IEnumerable<string>>(nameof (value));
        this.PropertiesImpl = value;
      }
    }

    protected override void ProcessPropertyName(
      string propertyName,
      IEventProcessorContext eventProcessorContext)
    {
      eventProcessorContext.IncludePropertyToEvent(propertyName);
    }
  }
}
