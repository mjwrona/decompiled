// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestActionExclude
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestActionExclude : ITelemetryManifestAction, IEventProcessorAction
  {
    [JsonIgnore]
    private bool excludeAll;
    [JsonIgnore]
    private IEnumerable<string> channelsToExclude;

    [JsonProperty(PropertyName = "excludeForChannels", Required = Required.Always)]
    public IEnumerable<string> ExcludeForChannels
    {
      get => this.channelsToExclude;
      set
      {
        value.RequiresArgumentNotNull<IEnumerable<string>>(nameof (value));
        this.channelsToExclude = value;
        this.excludeAll = value.Count<string>() == 1 && value.First<string>() == "*";
      }
    }

    [JsonIgnore]
    public int Priority => 0;

    public bool Execute(IEventProcessorContext eventProcessorContext)
    {
      if (this.excludeAll)
        return false;
      eventProcessorContext.RequiresArgumentNotNull<IEventProcessorContext>(nameof (eventProcessorContext));
      foreach (string excludeForChannel in this.ExcludeForChannels)
      {
        eventProcessorContext.Router.DisableChannel(excludeForChannel);
        if (eventProcessorContext.HostTelemetrySession.UseCollector && excludeForChannel == "aivortex")
          eventProcessorContext.Router.DisableChannel("collector");
      }
      return true;
    }

    public void Validate()
    {
      int num = this.ExcludeForChannels != null ? this.ExcludeForChannels.Count<string>() : throw new TelemetryManifestValidationException("excludeForChannels must not be null");
      if (num == 0)
        throw new TelemetryManifestValidationException("excludeForChannels must have at least one channel");
      foreach (string excludeForChannel in this.ExcludeForChannels)
      {
        if (string.IsNullOrEmpty(excludeForChannel))
          throw new TelemetryManifestValidationException("excludeForChannels must not contain empty/null channels");
        if (excludeForChannel == "*" && num > 1)
          throw new TelemetryManifestValidationException("excludeForChannels can contain only one element if '*' is in array");
      }
    }
  }
}
