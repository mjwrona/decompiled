// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetrySessionInternalVSCode
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Tools;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetrySessionInternalVSCode : TelemetrySessionInternalBase
  {
    internal TelemetrySessionInternalVSCode(
      TelemetrySessionSettings settings,
      bool isCloned,
      TelemetrySessionInitializer initializerObject,
      TelemetrySession session)
      : base(settings, isCloned, initializerObject, session)
    {
      this.ProductScenario = ProductTarget.VSCode;
    }

    protected override string InitializeReliabilityRegistryPath() => "Software\\Microsoft\\VSCode\\v1\\PIDs\\" + string.Format("{0}_{1}", (object) this.sessionInitializer?.HostInformationProvider?.ProcessId, (object) this.ProcessStartTime);

    protected override string GetUseCollectorRegKeyPath() => string.Empty;

    public override bool GetCachedUseCollectorFromRegistry() => true;
  }
}
