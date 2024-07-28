// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestRouter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestRouter
  {
    [JsonProperty(PropertyName = "channel", Required = Required.Always)]
    public string ChannelId { get; set; }

    [JsonProperty(PropertyName = "args")]
    public ITelemetryManifestRouteArgs Args { get; set; }

    public virtual void Validate()
    {
      if (this.ChannelId.IsNullOrWhiteSpace())
        throw new TelemetryManifestValidationException("'channel' must be valid non-empty channels id");
      if (this.Args == null)
        return;
      this.Args.Validate();
    }
  }
}
