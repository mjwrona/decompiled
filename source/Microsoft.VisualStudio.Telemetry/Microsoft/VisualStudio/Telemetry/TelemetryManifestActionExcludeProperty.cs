// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestActionExcludeProperty
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestActionExcludeProperty : 
    ITelemetryManifestAction,
    IEventProcessorAction
  {
    [JsonProperty(PropertyName = "excludeProperty", Required = Required.Always)]
    public string ExcludeProperty { get; set; }

    [JsonIgnore]
    public int Priority => 1;

    public bool Execute(IEventProcessorContext eventProcessorContext)
    {
      eventProcessorContext.RequiresArgumentNotNull<IEventProcessorContext>(nameof (eventProcessorContext));
      eventProcessorContext.ExcludePropertyFromEvent(this.ExcludeProperty);
      return true;
    }

    public void Validate()
    {
      if (string.IsNullOrWhiteSpace(this.ExcludeProperty))
        throw new TelemetryManifestValidationException("Name of excluded property must not be null or white space.");
    }
  }
}
