// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Internal.NativeMethods
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common.Internal
{
  internal class NativeMethods
  {
    public const int ERROR_SUCCESS = 0;
    public const int ERROR_FILE_NOT_FOUND = 2;
    public const int ERROR_ACCESS_DENIED = 5;
    public const int ERROR_INVALID_HANDLE = 6;
    public const int ERROR_CANNOT_MAKE = 82;
    public const int MAX_PATH = 260;
    private const int FORMAT_MESSAGE_FROM_SYSTEM = 4096;
    public const uint CRED_TYPE_GENERIC = 1;
    public const uint CRED_TYPE_DOMAIN_PASSWORD = 2;
    public const int CREDUI_FLAGS_INCORRECT_PASSWORD = 1;
    public const int CREDUI_FLAGS_DO_NOT_PERSIST = 2;
    public const int CREDUI_FLAGS_REQUEST_ADMINISTRATOR = 4;
    public const int CREDUI_FLAGS_EXCLUDE_CERTIFICATES = 8;
    public const int CREDUI_FLAGS_REQUIRE_CERTIFICATE = 16;
    public const int CREDUI_FLAGS_SHOW_SAVE_CHECK_BOX = 64;
    public const int CREDUI_FLAGS_ALWAYS_SHOW_UI = 128;
    public const int CREDUI_FLAGS_REQUIRE_SMARTCARD = 256;
    public const int CREDUI_FLAGS_PASSWORD_ONLY_OK = 512;
    public const int CREDUI_FLAGS_VALIDATE_USERNAME = 1024;
    public const int CREDUI_FLAGS_COMPLETE_USERNAME = 2048;
    public const int CREDUI_FLAGS_PERSIST = 4096;
    public const int CREDUI_FLAGS_SERVER_CREDENTIAL = 16384;
    public const int CREDUI_FLAGS_EXPECT_CONFIRMATION = 131072;
    public const int CREDUI_FLAGS_GENERIC_CREDENTIALS = 262144;
    public const int CREDUI_FLAGS_USERNAME_TARGET_CREDENTIALS = 524288;
    public const int CREDUI_FLAGS_KEEP_USERNAME = 1048576;
    public const int CRED_PACK_PROTECTED_CREDENTIALS = 1;
    public const int NO_ERROR = 0;
    public const int CREDUI_MAX_USERNAME_LENGTH = 513;
    public const int CREDUI_MAX_PASSWORD_LENGTH = 256;
    public const int CREDUI_MAX_CAPTION_LENGTH = 128;
    public const int CREDUI_MAX_MESSAGE_LENGTH = 32767;
    public const int CREDUIWIN_CHECKBOX = 2;
    public const int CREDUIWIN_AUTHPACKAGE_ONLY = 16;
    public const int CRED_PERSIST_LOCAL_MACHINE = 2;

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWow64Process([In] IntPtr hProcess, out bool lpSystemInfo);

    public static bool IsWow64
    {
      get
      {
        bool lpSystemInfo;
        if (!NativeMethods.IsWow64Process(Process.GetCurrentProcess().Handle, out lpSystemInfo))
          throw new Win32Exception();
        return lpSystemInfo;
      }
    }

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetModuleFileName(
      IntPtr hModule,
      StringBuilder lpFilename,
      int nSize);

    public static string GetModuleFileName() => NativeMethods.GetModuleFileName(IntPtr.Zero);

    public static string GetModuleFileName(IntPtr hModule)
    {
      StringBuilder lpFilename = new StringBuilder(260);
      int moduleFileName = NativeMethods.GetModuleFileName(hModule, lpFilename, lpFilename.Capacity);
      int lastWin32Error = Marshal.GetLastWin32Error();
      if (moduleFileName == 0 || moduleFileName >= lpFilename.Capacity)
        throw new VssServiceException(NativeMethods.FormatError(lastWin32Error));
      return lpFilename.ToString();
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern int FormatMessage(
      uint dwFlags,
      IntPtr lpSource,
      uint dwMessageId,
      int dwLanguageId,
      [Out] StringBuilder lpBuffer,
      int nSize,
      IntPtr Arguments);

    public static string FormatError(int number)
    {
      uint dwMessageId = (uint) number;
      StringBuilder lpBuffer = new StringBuilder(1024);
      NativeMethods.FormatMessage(4096U, IntPtr.Zero, dwMessageId, 0, lpBuffer, lpBuffer.Capacity, IntPtr.Zero);
      return lpBuffer.ToString();
    }

    [DllImport("Kernel32.dll", EntryPoint = "GetVersionExW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool GetVersionEx([In, Out] NativeMethods.OSVersionInfoEx ver);

    [DllImport("Kernel32.dll")]
    public static extern bool GetProductInfo(
      int osMajorVersion,
      int osMinorVersion,
      int spMajorVersion,
      int spMinorVersion,
      out int edition);

    [DllImport("Netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int NetGetJoinInformation(
      string server,
      out IntPtr domain,
      out NativeMethods.NetJoinStatus status);

    public static void NetGetJoinInformation(
      string server,
      out string domain,
      out NativeMethods.NetJoinStatus joinStatus)
    {
      IntPtr domain1 = IntPtr.Zero;
      joinStatus = NativeMethods.NetJoinStatus.NetSetupUnknownStatus;
      int joinInformation = NativeMethods.NetGetJoinInformation(server, out domain1, out joinStatus);
      if (domain1 != IntPtr.Zero)
      {
        domain = Marshal.PtrToStringAuto(domain1);
        int error = NativeMethods.NetApiBufferFree(domain1);
        if (error != 0 && joinInformation == 0)
          throw new Win32Exception(error);
      }
      else
        domain = string.Empty;
      if (joinInformation != 0)
        throw new Win32Exception(joinInformation);
    }

    [DllImport("netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern void NetFreeAadJoinInformation(IntPtr pJoinInfo);

    [DllImport("netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int NetGetAadJoinInformation(string pcszTenantId, out IntPtr ppJoinInfo);

    public static bool IsAadJoined(out DeviceJoinInfomation deviceJoinInfomation)
    {
      IntPtr ppJoinInfo = IntPtr.Zero;
      deviceJoinInfomation = (DeviceJoinInfomation) null;
      try
      {
        if (NativeMethods.NetGetAadJoinInformation((string) null, out ppJoinInfo) == 0)
        {
          NativeMethods.DSREG_JOIN_INFO structure = Marshal.PtrToStructure<NativeMethods.DSREG_JOIN_INFO>(ppJoinInfo);
          deviceJoinInfomation = new DeviceJoinInfomation(ref structure);
          return true;
        }
      }
      catch (EntryPointNotFoundException ex)
      {
      }
      finally
      {
        if (ppJoinInfo != IntPtr.Zero)
          NativeMethods.NetFreeAadJoinInformation(ppJoinInfo);
      }
      return false;
    }

    [DllImport("Netapi32.dll")]
    public static extern int NetApiBufferFree(IntPtr bufferPtr);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CredRead(
      string targetName,
      uint type,
      uint flags,
      out IntPtr credential);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CredWrite(ref NativeMethods.CREDENTIAL credential, uint flags);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CredDelete(string targetName, uint type, uint flags);

    [DllImport("advapi32.dll")]
    public static extern void CredFree(IntPtr buffer);

    [DllImport("credui.dll", CharSet = CharSet.Unicode)]
    public static extern int CredUIParseUserName(
      string pszUserName,
      StringBuilder pszUser,
      uint ulUserMaxChars,
      StringBuilder pszDomain,
      uint ulDomainMaxChars);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern void ZeroMemory(IntPtr address, uint byteCount);

    [DllImport("kernel32.dll")]
    public static extern void GetNativeSystemInfo(ref NativeMethods.SYSTEM_INFO lpSystemInfo);

    public struct SECURITY_ATTRIBUTES
    {
      public int nLength;
      public IntPtr lpSecurityDescriptor;
      public bool bInheritHandle;
    }

    [BestFitMapping(false, ThrowOnUnmappableChar = true)]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal sealed class OSVersionInfoEx
    {
      internal const uint VER_NT_WORKSTATION = 1;
      internal const uint VER_NT_DOMAIN_CONTROLLER = 2;
      internal const uint VER_NT_SERVER = 3;
      internal const uint VER_SUITE_ENTERPRISE = 2;
      internal const uint VER_SUITE_DATACENTER = 128;
      internal const uint VER_SUITE_PERSONAL = 512;
      internal const uint VER_SUITE_BLADE = 1024;
      private static NativeMethods.OSVersionInfoEx s_versionInfo;
      internal int osVersionInfoSize;
      internal int majorVersion;
      internal int minorVersion;
      internal int buildNumber;
      internal int platformId;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      internal string csdVersion;
      internal short servicePackMajor;
      internal short servicePackMinor;
      internal short suiteMask;
      internal byte productType;
      internal byte reserved;

      internal static NativeMethods.OSVersionInfoEx GetOsVersionInfo()
      {
        if (NativeMethods.OSVersionInfoEx.s_versionInfo == null)
        {
          NativeMethods.OSVersionInfoEx ver = new NativeMethods.OSVersionInfoEx();
          NativeMethods.OSVersionInfoEx.s_versionInfo = NativeMethods.GetVersionEx(ver) ? ver : throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        return NativeMethods.OSVersionInfoEx.s_versionInfo;
      }

      internal OSVersionInfoEx() => this.osVersionInfoSize = Marshal.SizeOf<NativeMethods.OSVersionInfoEx>(this);
    }

    public static class WindowsEditions
    {
      public const int PRODUCT_DATACENTER_SERVER = 8;
      public const int PRODUCT_DATACENTER_SERVER_CORE = 12;
      public const int PRODUCT_DATACENTER_SERVER_CORE_V = 39;
      public const int PRODUCT_DATACENTER_SERVER_V = 37;
      public const int PRODUCT_ENTERPRISE_SERVER = 10;
      public const int PRODUCT_ENTERPRISE_SERVER_CORE = 14;
      public const int PRODUCT_ENTERPRISE_SERVER_CORE_V = 41;
      public const int PRODUCT_ENTERPRISE_SERVER_IA64 = 15;
      public const int PRODUCT_ENTERPRISE_SERVER_V = 38;
      public const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT = 59;
      public const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL = 60;
      public const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC = 61;
      public const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC = 62;
      public const int PRODUCT_UNDEFINED = 0;
      public const int PRODUCT_ULTIMATE = 1;
      public const int PRODUCT_HOME_BASIC = 2;
      public const int PRODUCT_HOME_PREMIUM = 3;
      public const int PRODUCT_HOME_BASIC_N = 5;
      public const int PRODUCT_STANDARD_SERVER = 7;
      public const int PRODUCT_STANDARD_SERVER_CORE = 13;
      public const int PRODUCT_STANDARD_SERVER_CORE_V = 40;
      public const int PRODUCT_STANDARD_SERVER_SOLUTIONS = 52;
      public const int PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE = 53;
      public const int PRODUCT_STANDARD_SERVER_V = 36;
      public const int PRODUCT_STARTER = 11;
      public const int PRODUCT_STARTER_N = 47;
      public const int PRODUCT_PROFESSIONAL = 48;
      public const int PRODUCT_PROFESSIONAL_N = 49;
      public const int PRODUCT_HOME_PREMIUM_N = 26;
      public const int PRODUCT_ULTIMATE_N = 28;
      public const int PRODUCT_ENTERPRISE = 4;
      public const int PRODUCT_ENTERPRISE_N = 27;
      public const int PRODUCT_BUSINESS = 6;
      public const int PRODUCT_BUSINESS_N = 16;
      public const int PRODUCT_CORE = 101;
      public const int PRODUCT_CORE_N = 98;
      public const int PRODUCT_CORE_COUNTRYSPECIFIC = 99;
      public const int PRODUCT_CORE_SINGLELANGUAGE = 100;
    }

    public enum NetJoinStatus
    {
      NetSetupUnknownStatus,
      NetSetupUnjoined,
      NetSetupWorkgroupName,
      NetSetupDomainName,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DSREG_USER_INFO
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      public string UserEmail;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string UserKeyId;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string UserKeyName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DSREG_JOIN_INFO
    {
      public DeviceJoinType joinType;
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

    public struct CREDUI_INFO
    {
      public int cbSize;
      public IntPtr hwndParent;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszMessageText;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszCaptionText;
      public IntPtr hbmBanner;
    }

    public struct CREDENTIAL
    {
      public int Flags;
      public int Type;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string TargetName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string Comment;
      public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
      public int CredentialBlobSize;
      public IntPtr CredentialBlob;
      public int Persist;
      public int AttributeCount;
      public IntPtr Attributes;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string TargetAlias;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string UserName;
    }

    public struct CREDENTIAL_ATTRIBUTE
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      public string Keyword;
      public int Flags;
      public int ValueSize;
      public IntPtr Value;
    }

    public struct SYSTEM_INFO
    {
      public ushort wProcessorArchitecture;
      public ushort wReserved;
      public uint dwPageSize;
      public IntPtr lpMinimumApplicationAddress;
      public IntPtr lpMaximumApplicationAddress;
      public UIntPtr dwActiveProcessorMask;
      public uint dwNumberOfProcessors;
      public uint dwProcessorType;
      public uint dwAllocationGranularity;
      public ushort wProcessorLevel;
      public ushort wProcessorRevision;
    }

    public enum ProcessorArchitecture
    {
      PROCESSOR_ARCHITECTURE_INTEL = 0,
      PROCESSOR_ARCHITECTURE_IA64 = 6,
      PROCESSOR_ARCHITECTURE_AMD64 = 9,
    }
  }
}
