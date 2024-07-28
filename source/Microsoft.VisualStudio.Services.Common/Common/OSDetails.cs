// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.OSDetails
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class OSDetails
  {
    private static Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.OSVersionInfoEx s_versionInfo;
    private static bool? s_isComputerJoinedToWorkgroup;
    private static bool? s_isComputerAadJoined;
    private static bool? s_isChineseOS;
    private static readonly Stopwatch s_isChineseOSStopwatch = new Stopwatch();

    public static bool IsClient => OSDetails.OSVersionInfo.productType == (byte) 1;

    public static bool IsChineseOS
    {
      get
      {
        if (!OSDetails.s_isChineseOS.HasValue || OSDetails.s_isChineseOSStopwatch.ElapsedMilliseconds > 10000L)
        {
          string a = Environment.GetEnvironmentVariable("AZDEV_CHINESE_OS") ?? Environment.GetEnvironmentVariable("AZDEV_CHINESE_OS", EnvironmentVariableTarget.Machine);
          OSDetails.s_isChineseOS = a == null ? new bool?(CultureInfo.InstalledUICulture.LCID == 2052) : new bool?(string.Equals(a, bool.TrueString, StringComparison.OrdinalIgnoreCase) || string.Equals(a, "Y", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "Yes", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "1", StringComparison.OrdinalIgnoreCase));
          OSDetails.s_isChineseOSStopwatch.Restart();
        }
        return OSDetails.s_isChineseOS.Value;
      }
    }

    public static bool IsDomainController => OSDetails.OSVersionInfo.productType == (byte) 2;

    public static bool IsHomeEdition => ((ulong) OSDetails.OSVersionInfo.suiteMask & 512UL) > 0UL;

    public static bool IsMachineInWorkgroup()
    {
      if (!OSDetails.s_isComputerJoinedToWorkgroup.HasValue)
      {
        Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.NetJoinStatus joinStatus;
        Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.NetGetJoinInformation((string) null, out string _, out joinStatus);
        OSDetails.s_isComputerJoinedToWorkgroup = new bool?(joinStatus != Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.NetJoinStatus.NetSetupDomainName);
      }
      return OSDetails.s_isComputerJoinedToWorkgroup.Value;
    }

    public static bool IsMachineAadJoined()
    {
      if (!OSDetails.s_isComputerAadJoined.HasValue)
        OSDetails.s_isComputerAadJoined = new bool?(Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.IsAadJoined(out DeviceJoinInfomation _));
      return OSDetails.s_isComputerAadJoined.Value;
    }

    public static bool IsMachineAadJoined(out DeviceJoinInfomation deviceJoinInfomation) => Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.IsAadJoined(out deviceJoinInfomation);

    public static bool IsServer => OSDetails.OSVersionInfo.productType != (byte) 1;

    public static bool IsServerCore => string.Equals(Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "InstallationType", (object) null) as string, "Server Core", StringComparison.OrdinalIgnoreCase);

    public static bool Is64BitOperatingSystem
    {
      get
      {
        Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.SYSTEM_INFO lpSystemInfo = new Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.SYSTEM_INFO();
        Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.GetNativeSystemInfo(ref lpSystemInfo);
        switch ((Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.ProcessorArchitecture) lpSystemInfo.wProcessorArchitecture)
        {
          case Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.ProcessorArchitecture.PROCESSOR_ARCHITECTURE_INTEL:
            return false;
          case Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.ProcessorArchitecture.PROCESSOR_ARCHITECTURE_IA64:
          case Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.ProcessorArchitecture.PROCESSOR_ARCHITECTURE_AMD64:
            return true;
          default:
            return Environment.GetEnvironmentVariable("%ProgramFiles(x86)") != null;
        }
      }
    }

    public static bool IsWow64 => Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.IsWow64;

    public static bool Is64BitNotWow64 => !OSDetails.IsWow64 && OSDetails.Is64BitOperatingSystem;

    public static WindowsVersion Version
    {
      get
      {
        WindowsVersion version = WindowsVersion.Unknown;
        if (OSDetails.IsClient)
        {
          if (OSDetails.MajorVersion == 5 && OSDetails.MinorVersion >= 1)
          {
            version = WindowsVersion.XP;
          }
          else
          {
            switch (OSDetails.MajorVersion)
            {
              case 6:
                switch (OSDetails.MinorVersion)
                {
                  case 0:
                    version = WindowsVersion.Vista;
                    break;
                  case 1:
                    version = WindowsVersion.Windows7;
                    break;
                  case 2:
                    version = WindowsVersion.Windows8Client;
                    break;
                  case 3:
                    version = WindowsVersion.WinBlueClient;
                    break;
                }
                break;
              case 10:
                version = WindowsVersion.Windows10Client;
                if (OSDetails.OSVersionInfo.buildNumber >= 22000)
                {
                  version = WindowsVersion.Windows11Client;
                  break;
                }
                break;
            }
          }
        }
        else if (OSDetails.MajorVersion == 5 && OSDetails.MinorVersion == 2)
        {
          version = WindowsVersion.Server2003;
        }
        else
        {
          switch (OSDetails.MajorVersion)
          {
            case 6:
              switch (OSDetails.MinorVersion)
              {
                case 0:
                  version = WindowsVersion.Server2008;
                  break;
                case 1:
                  version = WindowsVersion.Server2008R2;
                  break;
                case 2:
                  version = WindowsVersion.Windows8Server;
                  break;
                case 3:
                  version = WindowsVersion.WinBlueServer;
                  break;
              }
              break;
            case 10:
              version = WindowsVersion.Server2016;
              if (OSDetails.OSVersionInfo.buildNumber >= 17763)
              {
                version = WindowsVersion.Server2019;
                break;
              }
              if (OSDetails.OSVersionInfo.buildNumber >= 20348)
              {
                version = WindowsVersion.Server2022;
                break;
              }
              break;
          }
        }
        if (version == WindowsVersion.Unknown && OSDetails.MajorVersion >= 6)
          version = WindowsVersion.Future;
        return version;
      }
    }

    private static int GetVersionDetail(string var, int defaultVersion) => defaultVersion;

    public static int MajorVersion => OSDetails.GetVersionDetail("TFS_OSVERSION_MAJOR", OSDetails.OSVersionInfo.majorVersion);

    public static int MinorVersion => OSDetails.GetVersionDetail("TFS_OSVERSION_MINOR", OSDetails.OSVersionInfo.minorVersion);

    public static int ServicePackMajor => OSDetails.GetVersionDetail("TFS_SERVICEPACK_MAJOR", (int) OSDetails.OSVersionInfo.servicePackMajor);

    public static int ServicePackMinor => OSDetails.GetVersionDetail("TFS_SERVICEPACK_MINOR", (int) OSDetails.OSVersionInfo.servicePackMinor);

    public static WindowsEdition Edition
    {
      get
      {
        WindowsEdition edition1 = WindowsEdition.Other;
        OperatingSystem osVersion = Environment.OSVersion;
        Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.OSVersionInfoEx osVersionInfoEx = new Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.OSVersionInfoEx();
        int edition2;
        if (Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.GetProductInfo(osVersion.Version.Major, osVersion.Version.Minor, (int) osVersionInfoEx.servicePackMajor, (int) osVersionInfoEx.servicePackMinor, out edition2))
        {
          switch (edition2)
          {
            case 0:
              edition1 = WindowsEdition.Undefined;
              break;
            case 1:
            case 28:
              edition1 = WindowsEdition.Ultimate;
              break;
            case 2:
            case 5:
              edition1 = WindowsEdition.Basic;
              break;
            case 3:
            case 26:
            case 98:
            case 99:
            case 100:
            case 101:
              edition1 = WindowsEdition.Premium;
              break;
            case 4:
            case 10:
            case 14:
            case 15:
            case 27:
            case 38:
            case 41:
              edition1 = WindowsEdition.Enterprise;
              break;
            case 6:
            case 16:
              edition1 = WindowsEdition.Business;
              break;
            case 7:
            case 13:
            case 36:
            case 40:
            case 52:
            case 53:
              edition1 = WindowsEdition.Standard;
              break;
            case 8:
            case 12:
            case 37:
            case 39:
              edition1 = WindowsEdition.DataCenter;
              break;
            case 11:
            case 47:
              edition1 = WindowsEdition.Starter;
              break;
            case 48:
            case 49:
              edition1 = WindowsEdition.Professional;
              break;
            case 59:
            case 60:
            case 61:
            case 62:
              edition1 = WindowsEdition.Essentials;
              break;
          }
        }
        return edition1;
      }
    }

    public static bool IsUnsupportedEdition
    {
      get
      {
        switch (OSDetails.Edition)
        {
          case WindowsEdition.Starter:
          case WindowsEdition.Basic:
            return true;
          default:
            return false;
        }
      }
    }

    public static bool IsExplorerInstalled => File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe"));

    private static Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.OSVersionInfoEx OSVersionInfo => OSDetails.s_versionInfo ?? (OSDetails.s_versionInfo = Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.OSVersionInfoEx.GetOsVersionInfo());
  }
}
