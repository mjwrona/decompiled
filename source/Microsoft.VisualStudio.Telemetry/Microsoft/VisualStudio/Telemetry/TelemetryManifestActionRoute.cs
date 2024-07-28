// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestActionRoute
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestActionRoute : ITelemetryManifestAction, IEventProcessorAction
  {
    [JsonProperty(PropertyName = "route", Required = Required.Always)]
    public IEnumerable<TelemetryManifestRouter> Route { get; set; }

    [JsonIgnore]
    public int Priority => 10000;

    public bool Execute(IEventProcessorContext eventProcessorContext)
    {
      eventProcessorContext.RequiresArgumentNotNull<IEventProcessorContext>(nameof (eventProcessorContext));
      foreach (TelemetryManifestRouter telemetryManifestRouter in this.Route)
      {
        if (eventProcessorContext.HostTelemetrySession.UseCollector && telemetryManifestRouter.ChannelId.Equals("aivortex"))
          telemetryManifestRouter.ChannelId = "collector";
        eventProcessorContext.Router.TryAddRouteArgument(telemetryManifestRouter.ChannelId, telemetryManifestRouter.Args);
      }
      return true;
    }

    public void Validate()
    {
      if (this.Route == null)
        throw new TelemetryManifestValidationException("'route' is null");
      if (!this.Route.Any<TelemetryManifestRouter>())
        throw new TelemetryManifestValidationException("'route' action must contain at least one element");
      foreach (TelemetryManifestRouter telemetryManifestRouter in this.Route)
        telemetryManifestRouter.Validate();
    }
  }
}
