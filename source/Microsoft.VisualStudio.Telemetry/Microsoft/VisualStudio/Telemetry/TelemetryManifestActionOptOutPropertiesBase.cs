// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestActionOptOutPropertiesBase
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal abstract class TelemetryManifestActionOptOutPropertiesBase : 
    TelemetryManifestActionOptOutBase
  {
    [JsonIgnore]
    protected IEnumerable<string> PropertiesImpl { get; set; }

    public override void Validate()
    {
      if (!this.PropertiesImpl.Any<string>())
        throw new TelemetryManifestValidationException("properties must have at least one property");
      if (this.PropertiesImpl.Any<string>(new Func<string, bool>(string.IsNullOrEmpty)))
        throw new TelemetryManifestValidationException("properties must not contain empty/null property name");
    }

    protected override void ExecuteOptOutAction(IEventProcessorContext eventProcessorContext)
    {
      foreach (string propertyName in this.PropertiesImpl)
        this.ProcessPropertyName(propertyName, eventProcessorContext);
    }

    protected abstract void ProcessPropertyName(
      string propertyName,
      IEventProcessorContext eventProcessorContext);
  }
}
