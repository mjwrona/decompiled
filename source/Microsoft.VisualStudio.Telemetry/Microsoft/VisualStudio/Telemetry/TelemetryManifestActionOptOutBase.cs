// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestActionOptOutBase
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Telemetry
{
  internal abstract class TelemetryManifestActionOptOutBase : 
    ITelemetryManifestAction,
    IEventProcessorAction
  {
    [JsonIgnore]
    public int Priority => 3;

    public bool Execute(IEventProcessorContext eventProcessorContext)
    {
      if (!eventProcessorContext.HostTelemetrySession.IsOptedIn)
        this.ExecuteOptOutAction(eventProcessorContext);
      return true;
    }

    public virtual void Validate()
    {
    }

    protected abstract void ExecuteOptOutAction(IEventProcessorContext eventProcessorContext);
  }
}
