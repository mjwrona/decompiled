// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.InternalScopeFilterProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Utilities.Internal;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal sealed class InternalScopeFilterProvider : 
    ISingleValueScopeFilterProvider<BoolScopeValue>,
    IScopeFilterProvider
  {
    private readonly TelemetrySession telemetrySession;

    public InternalScopeFilterProvider(TelemetrySession telemetrySession)
    {
      telemetrySession.RequiresArgumentNotNull<TelemetrySession>(nameof (telemetrySession));
      this.telemetrySession = telemetrySession;
    }

    public string Name => "IsInternal";

    public BoolScopeValue Provide() => new BoolScopeValue(this.telemetrySession.IsUserMicrosoftInternal);
  }
}
