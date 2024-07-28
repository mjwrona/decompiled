// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.InternalSettingsBase
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Globalization;
using System.Net.NetworkInformation;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class InternalSettingsBase : IInternalSettings
  {
    internal const string TelemetryUserRegKeyPath = "Software\\Microsoft\\VisualStudio\\Telemetry";
    internal const string CompletelyDisabledTelemetryRegKeyName = "TurnOffSwitch";
    internal const string LocalLoggerEnabledRegKeyName = "LocalLoggerEnabled";
    internal const int LocalLoggerEnabled = 1;
    internal const int CompletelyDisabledTelemetry = 1;
    private const string RegKeyChannelSettings = "\\Channels";
    private const int ChannelExplicitlyEnabled = 1;
    private const int ChannelExplicitlyDisabled = 0;
    private const string ForceExternalUserRegKeyName = "ForceExternalUser";
    private const int ForcedUserExternal = 1;
    private const string TestHostNameRegKeyName = "UseTestHostName";
    private const string TestAppIdRegKeyName = "UseTestAppId";
    protected readonly IRegistryTools registryTools;
    protected readonly IDiagnosticTelemetry diagnosticTelemetry;

    public InternalSettingsBase(
      IDiagnosticTelemetry diagnosticTelemetry,
      IRegistryTools registryTools)
    {
      diagnosticTelemetry.RequiresArgumentNotNull<IDiagnosticTelemetry>(nameof (diagnosticTelemetry));
      registryTools.RequiresArgumentNotNull<IRegistryTools>(nameof (registryTools));
      this.registryTools = registryTools;
      this.diagnosticTelemetry = diagnosticTelemetry;
    }

    public virtual ChannelInternalSetting GetChannelSettings(string channelId)
    {
      channelId.RequiresArgumentNotNullAndNotEmpty(nameof (channelId));
      int result;
      if (!TypeTools.TryConvertToInt(this.registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\Telemetry\\Channels", channelId, (object) -1), out result))
        result = -1;
      ChannelInternalSetting channelSettings = ChannelInternalSetting.Undefined;
      switch (result)
      {
        case 0:
          channelSettings = ChannelInternalSetting.ExplicitlyDisabled;
          break;
        case 1:
          channelSettings = ChannelInternalSetting.ExplicitlyEnabled;
          break;
      }
      if (channelSettings != ChannelInternalSetting.Undefined)
        this.diagnosticTelemetry.LogRegistrySettings(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Channel.{0}", new object[1]
        {
          (object) channelId
        }), channelSettings.ToString());
      return channelSettings;
    }

    public bool IsForcedUserExternal()
    {
      int result;
      TypeTools.TryConvertToInt(this.registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\Telemetry", "ForceExternalUser", (object) 0), out result);
      int num = result == 1 ? 1 : 0;
      if (num == 0)
        return num != 0;
      this.diagnosticTelemetry.LogRegistrySettings("ForcedExternalUser", "true");
      return num != 0;
    }

    public bool TryGetTestAppId(out uint testAppId)
    {
      TypeTools.TryConvertToUInt(this.registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\Telemetry", "UseTestAppId", (object) 0), out testAppId);
      if (testAppId != 0U)
        this.diagnosticTelemetry.LogRegistrySettings("TestAppId", testAppId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return testAppId > 0U;
    }

    public bool TryGetTestHostName(out string testHostName)
    {
      testHostName = TypeTools.ConvertToString(this.registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\Telemetry", "UseTestHostName"));
      int num = !string.IsNullOrEmpty(testHostName) ? 1 : 0;
      if (num == 0)
        return num != 0;
      this.diagnosticTelemetry.LogRegistrySettings("TestHostName", testHostName);
      return num != 0;
    }

    public string GetIPGlobalConfigDomainName()
    {
      string configDomainName = string.Empty;
      try
      {
        configDomainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
      }
      catch (NetworkInformationException ex)
      {
      }
      return configDomainName;
    }

    public bool IsTelemetryDisabledCompletely()
    {
      int result;
      TypeTools.TryConvertToInt(this.registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\Telemetry", "TurnOffSwitch", (object) 0), out result);
      int num = result == 1 ? 1 : 0;
      if (num == 0)
        return num != 0;
      this.diagnosticTelemetry.LogRegistrySettings("CompletelyDisabledTelemetry", "true");
      return num != 0;
    }

    public bool IsLocalLoggerEnabled()
    {
      bool flag = false;
      if (Platform.IsWindows)
      {
        int result;
        TypeTools.TryConvertToInt(this.registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\Telemetry", "LocalLoggerEnabled", (object) 0), out result);
        flag = result == 1;
      }
      if (flag)
        this.diagnosticTelemetry.LogRegistrySettings("LocalLoggerEnabled", "true");
      return flag;
    }

    public virtual int FaultEventWatsonSamplePercent() => 0;

    public virtual int FaultEventMaximumWatsonReportsPerSession() => 0;

    public virtual int FaultEventMinimumSecondsBetweenWatsonReports() => 0;
  }
}
