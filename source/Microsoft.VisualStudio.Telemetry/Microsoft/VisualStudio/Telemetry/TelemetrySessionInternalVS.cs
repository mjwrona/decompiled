// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetrySessionInternalVS
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Tools;
using Microsoft.VisualStudio.Utilities.Internal;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetrySessionInternalVS : TelemetrySessionInternalBase
  {
    internal TelemetrySessionInternalVS(
      TelemetrySessionSettings settings,
      bool isCloned,
      TelemetrySessionInitializer initializerObject,
      TelemetrySession session)
      : base(settings, isCloned, initializerObject, session)
    {
      this.ProductScenario = ProductTarget.Other;
    }

    protected override string InitializeReliabilityRegistryPath()
    {
      if (!Platform.IsWindows)
        return string.Empty;
      string str = string.Format("{0}_{1}", (object) this.sessionInitializer?.HostInformationProvider?.ProcessId, (object) this.ProcessStartTime);
      return string.Format("Software\\Microsoft\\VSCommon\\{0}.0\\SQM\\PIDs\\{1}", (object) typeof (TelemetrySession).Assembly.GetName().Version.Major, (object) str);
    }

    protected override string GetUseCollectorRegKeyPath() => string.Format("{0}\\{1}\\v{2}", (object) "Software\\Microsoft\\VisualStudio\\Telemetry", (object) this.sessionSettings.HostName, (object) 2U);

    public override bool GetCachedUseCollectorFromRegistry()
    {
      IRegistryTools registryTools = (IRegistryTools) this.sessionInitializer.RegistryTools;
      string regKeyPath = "Software\\Microsoft\\VisualStudio\\Telemetry";
      object fromCurrentUserRoot = registryTools.GetRegistryValueFromCurrentUserRoot(this.useCollectorRegKeyPath, "UseCollector");
      bool result = false;
      if (fromCurrentUserRoot == null)
      {
        result = false;
        registryTools.SetRegistryFromCurrentUserRoot(regKeyPath, "UseCollector", (object) result);
      }
      else if (!bool.TryParse(fromCurrentUserRoot.ToString(), out result))
        this.sessionInitializer.RegistryTools.DeleteRegistryKeyFromCurrentUserRoot(this.useCollectorRegKeyPath);
      return result;
    }
  }
}
