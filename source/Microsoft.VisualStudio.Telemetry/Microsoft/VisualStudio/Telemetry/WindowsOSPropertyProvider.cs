// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsOSPropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class WindowsOSPropertyProvider : IPropertyProvider
  {
    private static readonly long MbInBytes = 1048576;
    private const string OnValue = "HighContrastModeOn";
    private const string OffValue = "HighContrastModeOff";
    private const string OSCurrentVersionRegistryPath = "Software\\Microsoft\\Windows NT\\CurrentVersion";
    private const string BuildLabRegistryKey = "BuildLabEx";
    private const string CurrentBuildRegistryKey = "CurrentBuildNumber";
    private const string ProductNameRegistryKey = "ProductName";
    private const string UBRRegistryKey = "UBR";
    private const string ClrInstalledVersionRegistryPath = "Software\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full";
    private const string ReleaseKey = "Release";
    private readonly IEnvironmentTools environmentTools;
    private readonly IRegistryTools registryTools;
    private readonly Lazy<string> buildLabInfo;
    private readonly Lazy<WindowsOSPropertyProvider.DisplayInformation> displayInfo;
    private readonly Lazy<WindowsOSPropertyProvider.OSVersionInfo> operatingSystemVersionInfo;
    private readonly Lazy<string> productNameInfo;
    private readonly Lazy<WindowsOSPropertyProvider.RootDriveInfo> rootDriveInfo;
    private readonly Lazy<long?> totalVolumesSize;
    private readonly Lazy<int> clrInstalledVersion;
    private readonly Lazy<string> clrRunningVersion;

    public WindowsOSPropertyProvider(IEnvironmentTools envTools, IRegistryTools regTools)
    {
      envTools.RequiresArgumentNotNull<IEnvironmentTools>(nameof (envTools));
      regTools.RequiresArgumentNotNull<IRegistryTools>(nameof (regTools));
      this.environmentTools = envTools;
      this.registryTools = regTools;
      this.buildLabInfo = new Lazy<string>((Func<string>) (() => this.InitializeBuildLabInfo()), false);
      this.displayInfo = new Lazy<WindowsOSPropertyProvider.DisplayInformation>((Func<WindowsOSPropertyProvider.DisplayInformation>) (() => this.InitializeDisplayInfo()), false);
      this.operatingSystemVersionInfo = new Lazy<WindowsOSPropertyProvider.OSVersionInfo>((Func<WindowsOSPropertyProvider.OSVersionInfo>) (() => this.InitializeOSVersionInfo()), false);
      this.productNameInfo = new Lazy<string>((Func<string>) (() => this.InitializeProductNameInfo()), false);
      this.rootDriveInfo = new Lazy<WindowsOSPropertyProvider.RootDriveInfo>((Func<WindowsOSPropertyProvider.RootDriveInfo>) (() => this.InitializeRootDriveInfo()), false);
      this.totalVolumesSize = new Lazy<long?>((Func<long?>) (() => this.InitializeTotalVolumeSize()), false);
      this.clrInstalledVersion = new Lazy<int>((Func<int>) (() => this.InitializeClrInstalledVersion()), false);
      this.clrRunningVersion = new Lazy<string>((Func<string>) (() => this.InitializeClrRunningVersion()), false);
    }

    public void AddSharedProperties(
      List<KeyValuePair<string, object>> sharedProperties,
      TelemetryContext telemetryContext)
    {
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.OS.Version", (object) this.operatingSystemVersionInfo.Value.Version));
    }

    public void PostProperties(TelemetryContext telemetryContext, CancellationToken token)
    {
      if (token.IsCancellationRequested)
        return;
      if (this.buildLabInfo.Value != null)
      {
        telemetryContext.PostProperty("VS.Core.OS.BuildLab", (object) this.buildLabInfo.Value);
        if (token.IsCancellationRequested)
          return;
      }
      telemetryContext.PostProperty("VS.Core.OS.ClrInstalledVersion", (object) this.clrInstalledVersion.Value);
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.OS.ClrRunningVersion", (object) this.clrRunningVersion.Value);
      if (token.IsCancellationRequested)
        return;
      if (this.productNameInfo.Value != null)
      {
        telemetryContext.PostProperty("VS.Core.OS.ProductName", (object) this.productNameInfo.Value);
        if (token.IsCancellationRequested)
          return;
      }
      bool highContrast = SystemInformation.HighContrast;
      telemetryContext.PostProperty("VS.Core.OS.HighContrastId", (object) highContrast);
      if (token.IsCancellationRequested)
        return;
      string propertyValue = highContrast ? "HighContrastModeOn" : "HighContrastModeOff";
      telemetryContext.PostProperty("VS.Core.OS.HighContrastName", (object) propertyValue);
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.OS.Display.Dpi", (object) this.displayInfo.Value.Dpi);
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.OS.Display.ScalingFactor", (object) this.displayInfo.Value.ScalingFactor);
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.OS.Display.Count", (object) SystemInformation.MonitorCount);
      if (token.IsCancellationRequested)
        return;
      Size primaryMonitorSize = SystemInformation.PrimaryMonitorSize;
      telemetryContext.PostProperty("VS.Core.OS.Display.Resolution", (object) (primaryMonitorSize.Width * primaryMonitorSize.Height));
      if (token.IsCancellationRequested)
        return;
      Rectangle virtualScreen = SystemInformation.VirtualScreen;
      telemetryContext.PostProperty("VS.Core.OS.Display.XY", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}x{1}", new object[2]
      {
        (object) primaryMonitorSize.Width,
        (object) primaryMonitorSize.Height
      }));
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.OS.Display.VirtualXY", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}x{1}", new object[2]
      {
        (object) virtualScreen.Width,
        (object) virtualScreen.Height
      }));
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.OS.Display.ColorDepth", (object) Screen.PrimaryScreen.BitsPerPixel);
      if (token.IsCancellationRequested)
        return;
      if (this.totalVolumesSize.Value.HasValue)
      {
        telemetryContext.PostProperty("VS.Core.OS.Drive.AllVolumesSize", (object) this.totalVolumesSize.Value);
        if (token.IsCancellationRequested)
          return;
      }
      if (this.rootDriveInfo.Value == null)
        return;
      telemetryContext.PostProperty("VS.Core.OS.Drive.VolumeSize", (object) this.rootDriveInfo.Value.VolumeSize);
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.OS.Drive.FreeVolumeSpace", (object) this.rootDriveInfo.Value.FreeVolumeSpace);
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.OS.Drive.FileSystem", (object) this.rootDriveInfo.Value.FileSystem);
    }

    private string InitializeBuildLabInfo()
    {
      object localMachineRoot = this.registryTools.GetRegistryValueFromLocalMachineRoot("Software\\Microsoft\\Windows NT\\CurrentVersion", "BuildLabEx");
      return localMachineRoot != null && localMachineRoot is string ? (string) localMachineRoot : (string) null;
    }

    private int InitializeClrInstalledVersion() => this.registryTools.GetRegistryIntValueFromLocalMachineRoot("Software\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full", "Release").GetValueOrDefault();

    private string InitializeClrRunningVersion()
    {
      using (Process currentProcess = Process.GetCurrentProcess())
        return currentProcess.Modules.OfType<ProcessModule>().Where<ProcessModule>((Func<ProcessModule, bool>) (m => string.Equals(m.ModuleName, "clr.dll", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<ProcessModule>()?.FileVersionInfo.FileVersion ?? "Unknown";
    }

    private WindowsOSPropertyProvider.DisplayInformation InitializeDisplayInfo()
    {
      WindowsOSPropertyProvider.DisplayInformation displayInformation = new WindowsOSPropertyProvider.DisplayInformation();
      using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
      {
        IntPtr hdc = graphics.GetHdc();
        int deviceCaps1 = NativeMethods.GetDeviceCaps(hdc, 10);
        int deviceCaps2 = NativeMethods.GetDeviceCaps(hdc, 117);
        displayInformation.ScalingFactor = (float) deviceCaps2 / (float) deviceCaps1;
        displayInformation.Dpi = NativeMethods.GetDeviceCaps(hdc, 90);
        graphics.ReleaseHdc(hdc);
      }
      return displayInformation;
    }

    private WindowsOSPropertyProvider.OSVersionInfo InitializeOSVersionInfo()
    {
      WindowsOSPropertyProvider.OSVersionInfo osVersionInfo = new WindowsOSPropertyProvider.OSVersionInfo();
      NativeMethods.OSVersionInfo versionInfo = new NativeMethods.OSVersionInfo()
      {
        InfoSize = Marshal.SizeOf(typeof (NativeMethods.OSVersionInfo))
      };
      NativeMethods.RtlGetVersion(ref versionInfo);
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

    private string InitializeProductNameInfo()
    {
      object localMachineRoot = this.registryTools.GetRegistryValueFromLocalMachineRoot("Software\\Microsoft\\Windows NT\\CurrentVersion", "ProductName");
      return localMachineRoot != null && localMachineRoot is string ? (string) localMachineRoot : (string) null;
    }

    private WindowsOSPropertyProvider.RootDriveInfo InitializeRootDriveInfo()
    {
      try
      {
        DirectoryInfo info = new DirectoryInfo(".");
        DriveInfo driveInfo = ((IEnumerable<DriveInfo>) DriveInfo.GetDrives()).FirstOrDefault<DriveInfo>((Func<DriveInfo, bool>) (x => x.IsReady && x.DriveType == DriveType.Fixed && x.Name.Equals(info.Root.Name, StringComparison.InvariantCultureIgnoreCase)));
        if (driveInfo != null)
          return new WindowsOSPropertyProvider.RootDriveInfo()
          {
            VolumeSize = driveInfo.TotalSize / WindowsOSPropertyProvider.MbInBytes,
            FreeVolumeSpace = driveInfo.AvailableFreeSpace / WindowsOSPropertyProvider.MbInBytes,
            FileSystem = driveInfo.DriveFormat
          };
      }
      catch (IOException ex)
      {
      }
      catch (SecurityException ex)
      {
      }
      catch (UnauthorizedAccessException ex)
      {
      }
      return (WindowsOSPropertyProvider.RootDriveInfo) null;
    }

    private long? InitializeTotalVolumeSize()
    {
      try
      {
        return new long?(((IEnumerable<DriveInfo>) DriveInfo.GetDrives()).Where<DriveInfo>((Func<DriveInfo, bool>) (x => x.IsReady && x.DriveType == DriveType.Fixed)).Sum<DriveInfo>((Func<DriveInfo, long>) (y => y.TotalSize)) / WindowsOSPropertyProvider.MbInBytes);
      }
      catch (IOException ex)
      {
      }
      catch (SecurityException ex)
      {
      }
      catch (UnauthorizedAccessException ex)
      {
      }
      return new long?();
    }

    private struct DisplayInformation
    {
      public int Dpi;
      public float ScalingFactor;
    }

    private class RootDriveInfo
    {
      public long VolumeSize { get; set; }

      public long FreeVolumeSpace { get; set; }

      public string FileSystem { get; set; }
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
