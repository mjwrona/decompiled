// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsInternalSettings
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class WindowsInternalSettings : InternalSettingsBase
  {
    private const string FaultEventWatsonSampleRateRegKeyName = "FaultEventWatsonSampleRate";
    private const string FaultEventMaximumWatsonReportsPerSessionRegKeyName = "FaultEventMaximumWatsonReportsPerSession";
    private const string FaultEventMinimumSecondsBetweenWatsonReportsRegKeyName = "FaultEventMinimumSecondsBetweenWatsonReports";
    private const string GlobalPolicySqmClientRegistryPath = "Software\\Policies\\Microsoft\\SQMClient";
    private const string MsftInternalRegistryKeyName = "MSFTInternal";
    private const string EventTagTelemetryRegKeyName = "EventTag";

    public WindowsInternalSettings(
      IDiagnosticTelemetry diagnosticTelemetry,
      IRegistryTools registryTools)
      : base(diagnosticTelemetry, registryTools)
    {
      string userEventTag = this.GetUserEventTag();
      if (string.IsNullOrEmpty(userEventTag))
        return;
      diagnosticTelemetry.LogRegistrySettings("EventTag", userEventTag);
    }

    public override int FaultEventWatsonSamplePercent()
    {
      int result = 0;
      TypeTools.TryConvertToInt(this.registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\Telemetry", "FaultEventWatsonSampleRate", (object) 10), out result);
      return result;
    }

    public override int FaultEventMaximumWatsonReportsPerSession()
    {
      int result = 0;
      TypeTools.TryConvertToInt(this.registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\Telemetry", nameof (FaultEventMaximumWatsonReportsPerSession), (object) 10), out result);
      return result;
    }

    public override int FaultEventMinimumSecondsBetweenWatsonReports()
    {
      int result = 3600;
      TypeTools.TryConvertToInt(this.registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\Telemetry", nameof (FaultEventMinimumSecondsBetweenWatsonReports), (object) 3600), out result);
      return result;
    }

    public string GetUserEventTag() => this.registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\Telemetry", "EventTag", (object) "null") as string;
  }
}
