// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.DefaultRemoteSettingsFilterProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Telemetry.Services;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.RemoteSettings
{
  public class DefaultRemoteSettingsFilterProvider : RemoteSettingsFilterProvider
  {
    private readonly TelemetrySession session;
    private readonly IProcessInformationProvider processInformationProvider;
    private readonly IOSInformationProvider osInformationProvider;
    private readonly IRegistryTools registryTools;
    private readonly Lazy<string> partnerId;

    public DefaultRemoteSettingsFilterProvider(TelemetrySession telemetrySession)
      : this(telemetrySession, (IProcessInformationProvider) new ProcessInformationProvider(), (IOSInformationProvider) new OSInformationProvider(Platform.IsWindows ? (IRegistryTools) new RegistryTools() : (IRegistryTools) new FileBasedRegistryTools()), Platform.IsWindows ? (IRegistryTools) new RegistryTools() : (IRegistryTools) new FileBasedRegistryTools())
    {
    }

    internal DefaultRemoteSettingsFilterProvider(
      TelemetrySession telemetrySession,
      IProcessInformationProvider processInformationProvider,
      IOSInformationProvider osInformationProvider,
      IRegistryTools registryTools)
    {
      telemetrySession.RequiresArgumentNotNull<TelemetrySession>(nameof (telemetrySession));
      this.session = telemetrySession;
      processInformationProvider.RequiresArgumentNotNull<IProcessInformationProvider>(nameof (processInformationProvider));
      this.processInformationProvider = processInformationProvider;
      osInformationProvider.RequiresArgumentNotNull<IOSInformationProvider>(nameof (osInformationProvider));
      this.osInformationProvider = osInformationProvider;
      registryTools.RequiresArgumentNotNull<IRegistryTools>(nameof (registryTools));
      this.registryTools = registryTools;
      this.partnerId = new Lazy<string>((Func<string>) (() => this.InitializePartnerId()));
    }

    public override Guid GetMachineId() => this.session.MachineId;

    public override Guid GetUserId() => this.session.UserId;

    public override string GetCulture() => CultureInfo.CurrentUICulture.ToString();

    public override string GetApplicationName() => this.processInformationProvider.GetExeName() ?? string.Empty;

    public override string GetApplicationVersion()
    {
      FileVersion processVersionInfo = this.processInformationProvider.GetProcessVersionInfo();
      return processVersionInfo == null ? string.Empty : processVersionInfo.ToString();
    }

    public override string GetMacAddressHash() => this.session.MacAddressHash;

    public override string GetOsType() => "Windows";

    public override string GetOsVersion() => this.osInformationProvider.GetOSVersion();

    public override bool GetIsUserInternal() => this.session.IsUserMicrosoftInternal;

    public override string GetSessionRole() => this.session.DefaultContext.SharedProperties.GetOrDefault<string, object>("VS.Core.SessionRole") is string orDefault ? orDefault : string.Empty;

    public override string GetClrVersion()
    {
      using (Process currentProcess = Process.GetCurrentProcess())
        return currentProcess.Modules.OfType<ProcessModule>().Where<ProcessModule>((Func<ProcessModule, bool>) (m => string.Equals(m.ModuleName, "clr.dll", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<ProcessModule>()?.FileVersionInfo.FileVersion ?? string.Empty;
    }

    public override string GetProcessArchitecture()
    {
      if (!(this.session.DefaultContext.SharedProperties.GetOrDefault<string, object>("VS.Core.Process.Architecture") is string str))
        str = string.Empty;
      string processArch = str;
      if (string.IsNullOrWhiteSpace(processArch))
        ArchitectureTools.GetImageFileMachineArchitectures(out processArch, out string _);
      return processArch;
    }

    public override string GetClientSourceType() => !string.IsNullOrWhiteSpace(this.partnerId.Value) ? this.partnerId.Value : "standard";

    private string InitializePartnerId()
    {
      if (Platform.IsWindows)
      {
        string win365PartnerId = WindowsMachinePropertyProvider.GetWin365PartnerId(this.registryTools);
        Guid result;
        if (!string.IsNullOrWhiteSpace(win365PartnerId) && Guid.TryParse(win365PartnerId, out result))
          return result.ToString("D").ToLowerInvariant();
      }
      return string.Empty;
    }
  }
}
