// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DevFabricRelay.DevFabricRelaySettings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud.DevFabricRelay
{
  public class DevFabricRelaySettings
  {
    public static readonly RegistryQuery RegistryQuery = (RegistryQuery) "/Configuration/DevFabricRelay/**";

    public DevFabricRelaySettings(bool? enabled = null, string version = null)
    {
      this.Enabled = enabled.GetValueOrDefault();
      this.Version = version;
      this.LinuxInstallScriptName = "InstallDevFabricRelay-Linux.sh";
      this.LinuxPackageName = "azbridge." + this.Version + ".ubuntu.18.04-x64.deb";
      this.WindowsInstallScriptName = "InstallDevFabricRelay-Windows.ps1";
      this.WindowsPackageName = "azbridge_installer." + this.Version + ".win10-x64.msi";
      this.AllFileNames = (IReadOnlyList<string>) new string[4]
      {
        this.LinuxInstallScriptName,
        this.LinuxPackageName,
        this.WindowsInstallScriptName,
        this.WindowsPackageName
      };
    }

    public static DevFabricRelaySettings FromRegistry(IVssRequestContext rc)
    {
      RegistryEntryCollection registryEntryCollection = rc.GetService<IVssRegistryService>().ReadEntries(rc, DevFabricRelaySettings.RegistryQuery);
      return new DevFabricRelaySettings(registryEntryCollection.GetValueFromPath<bool?>("Enabled", new bool?()), registryEntryCollection.GetValueFromPath<string>("Version", (string) null));
    }

    public bool Enabled { get; }

    public string Version { get; }

    public string LinuxInstallScriptName { get; }

    public string LinuxPackageName { get; }

    public string WindowsInstallScriptName { get; }

    public string WindowsPackageName { get; }

    public IReadOnlyList<string> AllFileNames { get; }
  }
}
