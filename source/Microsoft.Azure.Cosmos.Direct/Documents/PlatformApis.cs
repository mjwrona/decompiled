// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PlatformApis
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Documents
{
  internal static class PlatformApis
  {
    private static readonly Lazy<Platform> _platform = new Lazy<Platform>(new Func<Platform>(PlatformApis.DetermineOSPlatform));
    private static readonly Lazy<PlatformApis.DistroInfo> _distroInfo = new Lazy<PlatformApis.DistroInfo>(new Func<PlatformApis.DistroInfo>(PlatformApis.LoadDistroInfo));

    public static string GetOSName()
    {
      switch (PlatformApis.GetOSPlatform())
      {
        case Platform.Windows:
          return "Windows";
        case Platform.Linux:
          return PlatformApis.GetDistroId() ?? "Linux";
        case Platform.Darwin:
          return "Mac OS X";
        default:
          return "Unknown";
      }
    }

    public static string GetOSVersion()
    {
      try
      {
        switch (PlatformApis.GetOSPlatform())
        {
          case Platform.Windows:
            return PlatformApis.GetWindowsVersion(RuntimeInformation.OSDescription) ?? string.Empty;
          case Platform.Linux:
            return PlatformApis.GetDistroVersionId() ?? string.Empty;
          case Platform.Darwin:
            return PlatformApis.GetDarwinVersion() ?? string.Empty;
          default:
            return string.Empty;
        }
      }
      catch
      {
        return string.Empty;
      }
    }

    public static string GetWindowsVersion(string osDescipiton) => new Regex("([0-9]+\\.*)+").Match(osDescipiton).ToString();

    private static string GetDarwinVersion()
    {
      Version result;
      return !Version.TryParse(NativeMethods.Darwin.GetKernelRelease(), out result) || result.Major < 5 ? "10.0" : string.Format("10.{0}", (object) (result.Major - 4));
    }

    public static Platform GetOSPlatform() => PlatformApis._platform.Value;

    private static string GetDistroId() => PlatformApis._distroInfo.Value?.Id;

    private static string GetDistroVersionId() => PlatformApis._distroInfo.Value?.VersionId;

    private static PlatformApis.DistroInfo LoadDistroInfo()
    {
      if (File.Exists("/etc/os-release"))
      {
        string[] strArray = File.ReadAllLines("/etc/os-release");
        PlatformApis.DistroInfo distroInfo = new PlatformApis.DistroInfo();
        foreach (string str in strArray)
        {
          if (str.StartsWith("ID=", StringComparison.Ordinal))
            distroInfo.Id = str.Substring(3).Trim('"', '\'');
          else if (str.StartsWith("VERSION_ID=", StringComparison.Ordinal))
            distroInfo.VersionId = str.Substring(11).Trim('"', '\'');
        }
        return distroInfo;
      }
      string str1 = File.Exists("/proc/sys/kernel/ostype") ? File.ReadAllText("/proc/sys/kernel/ostype") : (string) null;
      string str2 = File.Exists("/proc/sys/kernel/osrelease") ? File.ReadAllText("/proc/sys/kernel/osrelease") : (string) null;
      return new PlatformApis.DistroInfo()
      {
        Id = str1,
        VersionId = str2
      };
    }

    private static Platform DetermineOSPlatform()
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return Platform.Windows;
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        return Platform.Linux;
      return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? Platform.Darwin : Platform.Unknown;
    }

    private class DistroInfo
    {
      public string Id;
      public string VersionId;
    }
  }
}
