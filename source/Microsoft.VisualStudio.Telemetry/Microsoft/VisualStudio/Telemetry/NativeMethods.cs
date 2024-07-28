// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.NativeMethods
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VisualStudio.Telemetry
{
  internal static class NativeMethods
  {
    internal const ushort IMAGE_FILE_MACHINE_UNKNOWN = 0;
    internal const ushort IMAGE_FILE_MACHINE_ARM64 = 43620;
    internal const ushort IMAGE_FILE_MACHINE_AMD64 = 34404;
    internal const ushort IMAGE_FILE_MACHINE_ARM = 448;
    internal const ushort IMAGE_FILE_MACHINE_ARMNT = 452;
    internal const ushort IMAGE_FILE_MACHINE_THUMB = 450;
    internal const ushort IMAGE_FILE_MACHINE_M32R = 36929;
    internal const ushort IMAGE_FILE_MACHINE_I386 = 332;
    public static uint FILE_RENAME_REPLACE_IF_EXISTS = 1;
    public static uint FILE_RENAME_POSIX_SEMANTICS = 2;
    public static uint FILE_RENAME_IGNORE_READONLY_ATTRIBUTE = 64;
    public static uint FILE_ATTRIBUTE_NORMAL = 128;

    internal static string GetFullProcessExeName()
    {
      StringBuilder filename = new StringBuilder(1000);
      int moduleFileName = (int) NativeMethods.GetModuleFileName(IntPtr.Zero, filename, filename.Capacity);
      return filename.ToString();
    }

    [DllImport("kernel32.dll")]
    internal static extern uint GetCurrentProcessId();

    [DllImport("kernel32.dll")]
    internal static extern uint GetCurrentThreadId();

    [DllImport("kernel32.dll")]
    internal static extern bool IsDebuggerPresent();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern uint GetModuleFileName(
      [In] IntPtr handleModule,
      [Out] StringBuilder filename,
      [MarshalAs(UnmanagedType.U4), In] int size);

    [DllImport("kernel32.dll")]
    internal static extern IntPtr GetCurrentProcess();

    [DllImport("ntdll.dll")]
    internal static extern bool RtlGetVersion(ref NativeMethods.OSVersionInfo versionInfo);

    [DllImport("kernel32.dll")]
    internal static extern bool GlobalMemoryStatusEx(ref NativeMethods.MemoryStatus bufferPointer);

    [DllImport("kernel32.dll")]
    internal static extern bool GetNativeSystemInfo(ref NativeMethods.SystemInfo systemInfo);

    [DllImport("shlwapi.dll")]
    internal static extern bool IsOS(NativeMethods.OSFeatureFlag featureFlag);

    [DllImport("gdi32.dll")]
    internal static extern int GetDeviceCaps(IntPtr hdc, int index);

    [DllImport("advapi32.dll")]
    internal static extern bool GetTokenInformation(
      IntPtr tokenHandle,
      int tokenInformationClass,
      IntPtr tokenInformation,
      int tokenInformationLength,
      out int returnLength);

    internal static long? GetProcessCreationTime()
    {
      System.Runtime.InteropServices.ComTypes.FILETIME creationTime;
      return NativeMethods.GetProcessTimes(NativeMethods.GetCurrentProcess(), out creationTime, out System.Runtime.InteropServices.ComTypes.FILETIME _, out System.Runtime.InteropServices.ComTypes.FILETIME _, out System.Runtime.InteropServices.ComTypes.FILETIME _) ? new long?(NativeMethods.FiletimeToDateTime(creationTime).Ticks) : new long?();
    }

    internal static ulong? GetProcessCreationFileTime()
    {
      if (Platform.IsWindows)
      {
        try
        {
          System.Runtime.InteropServices.ComTypes.FILETIME creationTime;
          if (NativeMethods.GetProcessTimes(NativeMethods.GetCurrentProcess(), out creationTime, out System.Runtime.InteropServices.ComTypes.FILETIME _, out System.Runtime.InteropServices.ComTypes.FILETIME _, out System.Runtime.InteropServices.ComTypes.FILETIME _))
            return new ulong?(NativeMethods.FiletimeToULong(creationTime));
        }
        catch
        {
        }
      }
      return new ulong?();
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetProcessTimes(
      IntPtr handleProcess,
      out System.Runtime.InteropServices.ComTypes.FILETIME creationTime,
      out System.Runtime.InteropServices.ComTypes.FILETIME exitTime,
      out System.Runtime.InteropServices.ComTypes.FILETIME kernelTime,
      out System.Runtime.InteropServices.ComTypes.FILETIME userTime);

    private static DateTime FiletimeToDateTime(System.Runtime.InteropServices.ComTypes.FILETIME fileTime) => DateTime.FromFileTimeUtc((long) NativeMethods.FiletimeToULong(fileTime));

    private static ulong FiletimeToULong(System.Runtime.InteropServices.ComTypes.FILETIME fileTime) => (ulong) (uint) fileTime.dwHighDateTime << 32 | (ulong) (uint) fileTime.dwLowDateTime;

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern int EnumSystemFirmwareTables(
      NativeMethods.FirmwareTableProviderSignature firmwareTableProviderSignature,
      IntPtr firmwareTableBuffer,
      int bufferSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern int GetSystemFirmwareTable(
      NativeMethods.FirmwareTableProviderSignature firmwareTableProviderSignature,
      int firmwareTableID,
      IntPtr firmwareTableBuffer,
      int bufferSize);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern bool IsWow64Process2(
      IntPtr process,
      out ushort processMachine,
      out ushort nativeMachine);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    internal static extern uint WerRegisterCustomMetadata(string key, string value);

    [DllImport("netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern void NetFreeAadJoinInformation(IntPtr pJoinInfo);

    [DllImport("netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int NetGetAadJoinInformation(string pcszTenantId, out IntPtr ppJoinInfo);

    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int smIndex);

    [DllImport("kernel32.dll")]
    public static extern bool ProcessIdToSessionId(uint dwProcessId, out uint pSessionId);

    internal struct OSVersionInfo
    {
      public int InfoSize;
      public uint MajorVersion;
      public uint MinorVersion;
      public uint BuildNumber;
      public uint PlatformId;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      public string StringVersion;
      public short ServicePackMajor;
      public short ServicePackMinor;
      public short SuiteMask;
      public byte ProductType;
      public byte Reserved;
    }

    internal struct MemoryStatus
    {
      public uint Length;
      public uint MemoryLoad;
      public ulong TotalPhys;
      public ulong AvailPhys;
      public ulong TotalPageFile;
      public ulong AvailPageFile;
      public ulong TotalVirtual;
      public ulong AvailVirtual;
      public ulong AvailExtendedVirtual;
    }

    internal struct SystemInfo
    {
      public ushort ProcessorArchitecture;
      public ushort Reserved;
      public uint PageSize;
      public UIntPtr MinimumApplicationAddress;
      public UIntPtr MaximumApplicationAddress;
      public UIntPtr ActiveProcessorMask;
      public uint NumberOfProcessors;
      public uint ProcessorType;
      public uint AllocationGranularity;
      public ushort ProcessorLevel;
      public ushort ProcessorRevision;
    }

    internal enum OSFeatureFlag
    {
      OSDomainMember = 28, // 0x0000001C
    }

    internal struct DisplayInformation
    {
      public int Dpi;
      public float ScalingFactor;
    }

    internal enum FirmwareTableProviderSignature
    {
      ACPI = 1094930505, // 0x41435049
      FIRM = 1179210317, // 0x4649524D
      RSMB = 1381190978, // 0x52534D42
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DSREG_JOIN_INFO
    {
      public int joinType;
      public IntPtr pJoinCertificate;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string DeviceId;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string IdpDomain;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string TenantId;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string JoinUserEmail;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string TenantDisplayName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string MdmEnrollmentUrl;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string MdmTermsOfUseUrl;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string MdmComplianceUrl;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string UserSettingSyncUrl;
      public IntPtr pUserInfo;
    }

    public struct FILE_RENAME_INFO
    {
      internal NativeMethods.FILE_RENAME_INFO._Anonymous_e__Union Anonymous;
      internal IntPtr RootDirectory;
      internal uint FileNameLength;
      internal ushort FileName;

      [StructLayout(LayoutKind.Explicit)]
      internal struct _Anonymous_e__Union
      {
        [FieldOffset(0)]
        internal byte ReplaceIfExists;
        [FieldOffset(0)]
        internal uint Flags;
      }
    }
  }
}
