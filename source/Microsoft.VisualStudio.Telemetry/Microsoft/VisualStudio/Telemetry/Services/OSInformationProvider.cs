// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Services.OSInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Telemetry.Services
{
  internal class OSInformationProvider : IOSInformationProvider
  {
    private const string OSCurrentVersionRegistryPath = "Software\\Microsoft\\Windows NT\\CurrentVersion";
    private const string UBRRegistryKey = "UBR";
    private const string BuildLabRegistryKey = "BuildLabEx";
    private IRegistryTools registryTools;

    public OSInformationProvider(IRegistryTools registryTools)
    {
      registryTools.RequiresArgumentNotNull<IRegistryTools>(nameof (registryTools));
      this.registryTools = registryTools;
    }

    public string GetOSVersion() => this.InitializeOSVersionInfo().Version;

    private OSInformationProvider.OSVersionInfo InitializeOSVersionInfo()
    {
      OSInformationProvider.OSVersionInfo osVersionInfo = new OSInformationProvider.OSVersionInfo();
      Microsoft.VisualStudio.Telemetry.NativeMethods.OSVersionInfo versionInfo = new Microsoft.VisualStudio.Telemetry.NativeMethods.OSVersionInfo()
      {
        InfoSize = Marshal.SizeOf(typeof (Microsoft.VisualStudio.Telemetry.NativeMethods.OSVersionInfo))
      };
      Microsoft.VisualStudio.Telemetry.NativeMethods.RtlGetVersion(ref versionInfo);
      osVersionInfo.MajorVersion = (ulong) versionInfo.MajorVersion;
      osVersionInfo.MinorVersion = (ulong) versionInfo.MinorVersion;
      osVersionInfo.BuildNumber = (ulong) versionInfo.BuildNumber;
      bool flag = false;
      int? localMachineRoot1 = this.registryTools.GetRegistryIntValueFromLocalMachineRoot("Software\\Microsoft\\Windows NT\\CurrentVersion", "UBR");
      if (localMachineRoot1.HasValue)
      {
        try
        {
          osVersionInfo.RevisionNumber = Convert.ToUInt64((object) localMachineRoot1);
          flag = true;
        }
        catch (FormatException ex)
        {
        }
      }
      if (!flag)
      {
        object localMachineRoot2 = this.registryTools.GetRegistryValueFromLocalMachineRoot("Software\\Microsoft\\Windows NT\\CurrentVersion", "BuildLabEx");
        if (localMachineRoot2 != null && localMachineRoot2 is string)
        {
          string[] strArray = ((string) localMachineRoot2).Split('.');
          if (strArray.Length >= 2)
          {
            try
            {
              osVersionInfo.RevisionNumber = Convert.ToUInt64(strArray[1]);
            }
            catch (FormatException ex)
            {
            }
          }
        }
      }
      return osVersionInfo;
    }

    private class OSVersionInfo
    {
      public string Version => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", (object) this.MajorVersion, (object) this.MinorVersion, (object) this.BuildNumber, (object) this.RevisionNumber);

      public ulong MajorVersion { get; set; }

      public ulong MinorVersion { get; set; }

      public ulong BuildNumber { get; set; }

      public ulong RevisionNumber { get; set; }
    }
  }
}
