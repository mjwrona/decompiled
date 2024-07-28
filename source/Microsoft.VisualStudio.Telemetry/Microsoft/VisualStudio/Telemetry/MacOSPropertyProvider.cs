// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MacOSPropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Native.Mac;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class MacOSPropertyProvider : IPropertyProvider
  {
    private static readonly long MbInBytes = 1048576;
    private readonly IEnvironmentTools environmentTools;
    private readonly Lazy<MacFoundation.CoreGraphics.DisplayInformation> displayInfo;
    private readonly Lazy<MacOSPropertyProvider.OSVersionInfo> operatingSystemVersionInfo;
    private readonly Lazy<string> productNameInfo;
    private readonly Lazy<MacOSPropertyProvider.RootDriveInfo> rootDriveInfo;
    private readonly Lazy<long?> totalVolumesSize;

    public MacOSPropertyProvider(IEnvironmentTools envTools)
    {
      envTools.RequiresArgumentNotNull<IEnvironmentTools>(nameof (envTools));
      this.environmentTools = envTools;
      this.displayInfo = new Lazy<MacFoundation.CoreGraphics.DisplayInformation>((Func<MacFoundation.CoreGraphics.DisplayInformation>) (() => this.InitializeDisplayInfo()), false);
      this.operatingSystemVersionInfo = new Lazy<MacOSPropertyProvider.OSVersionInfo>((Func<MacOSPropertyProvider.OSVersionInfo>) (() => this.InitializeOSVersionInfo()), false);
      this.rootDriveInfo = new Lazy<MacOSPropertyProvider.RootDriveInfo>((Func<MacOSPropertyProvider.RootDriveInfo>) (() => this.InitializeRootDriveInfo()), false);
      this.totalVolumesSize = new Lazy<long?>((Func<long?>) (() => this.InitializeTotalVolumeSize()), false);
      this.productNameInfo = new Lazy<string>((Func<string>) (() => this.InitializeProductNameInfo()), false);
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
      if (this.operatingSystemVersionInfo.Value != null)
      {
        telemetryContext.PostProperty("VS.Core.OS.BuildLab", (object) this.operatingSystemVersionInfo.Value.KernelOSVersion);
        if (token.IsCancellationRequested)
          return;
      }
      telemetryContext.PostProperty("VS.Core.OS.ClrVersion", (object) this.environmentTools.Version);
      if (token.IsCancellationRequested)
        return;
      if (this.productNameInfo.Value != null)
      {
        telemetryContext.PostProperty("VS.Core.OS.ProductName", (object) this.productNameInfo.Value);
        if (token.IsCancellationRequested)
          return;
      }
      telemetryContext.PostProperty("VS.Core.OS.Display.Count", (object) this.displayInfo.Value.DisplayCount);
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.OS.Display.Resolution", (object) (uint) ((int) this.displayInfo.Value.MainDisplayWidth * (int) this.displayInfo.Value.MainDisplayHeight));
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.OS.Display.XY", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}x{1}", new object[2]
      {
        (object) this.displayInfo.Value.MainDisplayWidth,
        (object) this.displayInfo.Value.MainDisplayHeight
      }));
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.OS.Display.VirtualXY", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}x{1}", new object[2]
      {
        (object) this.displayInfo.Value.MainDisplayWidth,
        (object) this.displayInfo.Value.MainDisplayHeight
      }));
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

    private MacFoundation.CoreGraphics.DisplayInformation InitializeDisplayInfo()
    {
      MacFoundation.CoreGraphics.DisplayInformation info = new MacFoundation.CoreGraphics.DisplayInformation();
      MacFoundation.CoreGraphics.GetDisplayInfo(ref info);
      return info;
    }

    private MacOSPropertyProvider.OSVersionInfo InitializeOSVersionInfo()
    {
      MacOSPropertyProvider.OSVersionInfo osVersionInfo = new MacOSPropertyProvider.OSVersionInfo();
      MacNativeMethods.OSVersionInfo info = new MacNativeMethods.OSVersionInfo();
      MacNativeMethods.GetOSVersionInfo(ref info);
      osVersionInfo.VersionInfo = info;
      return osVersionInfo;
    }

    private string InitializeProductNameInfo() => "macOS";

    private MacOSPropertyProvider.RootDriveInfo InitializeRootDriveInfo()
    {
      try
      {
        DirectoryInfo info = new DirectoryInfo("/");
        DriveInfo driveInfo = ((IEnumerable<DriveInfo>) DriveInfo.GetDrives()).FirstOrDefault<DriveInfo>((Func<DriveInfo, bool>) (x => x.IsReady && x.DriveType == DriveType.Fixed && x.Name.Equals(info.Root.Name, StringComparison.InvariantCultureIgnoreCase)));
        if (driveInfo != null)
          return new MacOSPropertyProvider.RootDriveInfo()
          {
            VolumeSize = driveInfo.TotalSize / MacOSPropertyProvider.MbInBytes,
            FreeVolumeSpace = driveInfo.AvailableFreeSpace / MacOSPropertyProvider.MbInBytes,
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
      return (MacOSPropertyProvider.RootDriveInfo) null;
    }

    private long? InitializeTotalVolumeSize()
    {
      try
      {
        return new long?(((IEnumerable<DriveInfo>) DriveInfo.GetDrives()).Where<DriveInfo>((Func<DriveInfo, bool>) (x => x.IsReady && x.DriveType == DriveType.Fixed)).Sum<DriveInfo>((Func<DriveInfo, long>) (y => y.TotalSize)) / MacOSPropertyProvider.MbInBytes);
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

    private class RootDriveInfo
    {
      public long VolumeSize { get; set; }

      public long FreeVolumeSpace { get; set; }

      public string FileSystem { get; set; }
    }

    private class OSVersionInfo
    {
      public string Version => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}", new object[3]
      {
        (object) this.VersionInfo.MajorVersion,
        (object) this.VersionInfo.MinorVersion,
        (object) this.VersionInfo.BuildNumber
      });

      public MacNativeMethods.OSVersionInfo VersionInfo { get; set; }

      public string KernelOSVersion => this.VersionInfo.OSVersion;
    }
  }
}
