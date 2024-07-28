// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.ConfigurationHelper
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceProcess;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ConfigurationHelper
  {
    private static readonly string s_logonAsServiceName = "SeServiceLogonRight";
    [CLSCompliant(false)]
    public const uint ServiceNoChange = 4294967295;
    private static ConfigurationHelper.HTTPAPI_VERSION HttpApiVersion = new ConfigurationHelper.HTTPAPI_VERSION((ushort) 1, (ushort) 0);
    private const uint HTTP_INITIALIZE_CONFIG = 2;

    public static bool FreeUrlPrefix(string urlPrefix)
    {
      int error1 = 0;
      try
      {
        error1 = ConfigurationHelper.HttpInitialize(ConfigurationHelper.HttpApiVersion, 2U, IntPtr.Zero);
        if (error1 == 0)
        {
          ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_KEY serviceConfigUrlaclKey = new ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_KEY(urlPrefix);
          ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_SET pConfigInformation = new ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_SET();
          pConfigInformation.KeyDesc = serviceConfigUrlaclKey;
          int ConfigInformationLength = Marshal.SizeOf<ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_SET>(pConfigInformation);
          error1 = ConfigurationHelper.HttpDeleteServiceConfiguration(IntPtr.Zero, ConfigurationHelper.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo, ref pConfigInformation, ConfigInformationLength, IntPtr.Zero);
        }
      }
      finally
      {
        int error2 = ConfigurationHelper.HttpTerminate(2U, IntPtr.Zero);
        if (error2 != 0)
          throw new Win32Exception(error2);
      }
      if (error1 == 0)
        return true;
      if (error1 == 2)
        return false;
      throw new Win32Exception(error1);
    }

    private static bool ReserveUrlPrefixInternal(string urlPrefix, string securityDescriptor)
    {
      bool flag = true;
      int error1 = 0;
      try
      {
        error1 = ConfigurationHelper.HttpInitialize(ConfigurationHelper.HttpApiVersion, 2U, IntPtr.Zero);
        if (error1 == 0)
        {
          ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_SET pConfigInformation = new ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_SET();
          pConfigInformation.KeyDesc = new ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_KEY(urlPrefix);
          pConfigInformation.ParamDesc = new ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_PARAM(securityDescriptor);
          error1 = ConfigurationHelper.HttpSetServiceConfiguration(IntPtr.Zero, ConfigurationHelper.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo, ref pConfigInformation, Marshal.SizeOf<ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_SET>(pConfigInformation), IntPtr.Zero);
          if (error1 == 183)
          {
            error1 = ConfigurationHelper.HttpDeleteServiceConfiguration(IntPtr.Zero, ConfigurationHelper.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo, ref pConfigInformation, Marshal.SizeOf<ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_SET>(pConfigInformation), IntPtr.Zero);
            if (error1 == 0 || error1 == 2)
            {
              error1 = ConfigurationHelper.HttpSetServiceConfiguration(IntPtr.Zero, ConfigurationHelper.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo, ref pConfigInformation, Marshal.SizeOf<ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_SET>(pConfigInformation), IntPtr.Zero);
              error1 = 0;
            }
            flag = false;
          }
        }
      }
      finally
      {
        int error2 = ConfigurationHelper.HttpTerminate(2U, IntPtr.Zero);
        if (error2 != 0)
          throw new Win32Exception(error2);
      }
      if (error1 != 0)
        throw new Win32Exception(error1);
      return flag;
    }

    public static bool ReserveUrlPrefix(string userName, string urlPrefix)
    {
      SecurityIdentifier sid;
      if (userName.Equals("LocalSystem", StringComparison.OrdinalIgnoreCase) || userName.EndsWith("\\LocalSystem", StringComparison.OrdinalIgnoreCase))
      {
        sid = new SecurityIdentifier("S-1-5-18");
      }
      else
      {
        string name = userName;
        if (userName.StartsWith(".\\", StringComparison.OrdinalIgnoreCase))
          name = Environment.MachineName + userName.Substring(1);
        try
        {
          sid = (SecurityIdentifier) new NTAccount(name).Translate(typeof (SecurityIdentifier));
        }
        catch (Exception ex)
        {
          throw new ArgumentException(BuildTypeResource.InvalidAccount((object) userName), ex);
        }
      }
      DiscretionaryAcl discretionaryAcl = new DiscretionaryAcl(false, false, 4);
      discretionaryAcl.AddAccess(AccessControlType.Allow, sid, 536870912, InheritanceFlags.None, PropagationFlags.None);
      CommonSecurityDescriptor securityDescriptor = new CommonSecurityDescriptor(false, false, ControlFlags.OwnerDefaulted | ControlFlags.GroupDefaulted | ControlFlags.DiscretionaryAclPresent, (SecurityIdentifier) null, (SecurityIdentifier) null, (SystemAcl) null, discretionaryAcl);
      return ConfigurationHelper.ReserveUrlPrefixInternal(urlPrefix, securityDescriptor.GetSddlForm(AccessControlSections.Access));
    }

    public static bool AddSslCertificate(
      IPAddress address,
      int port,
      X509Certificate certificate,
      Guid appId,
      bool requireClientCertificates,
      StoreName storeName)
    {
      bool flag = true;
      int error1 = 0;
      try
      {
        error1 = ConfigurationHelper.HttpInitialize(ConfigurationHelper.HttpApiVersion, 2U, IntPtr.Zero);
        if (error1 == 0)
        {
          byte[] certHash = certificate.GetCertHash();
          ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_KEY serviceConfigSslKey = new ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_KEY(new ConfigurationHelper.SOCKADDR_IN(address, (ushort) port));
          try
          {
            ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_PARAM serviceConfigSslParam = new ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_PARAM()
            {
              pSslHash = certHash,
              DefaultRevocationFreshnessTime = 0,
              DefaultRevocationUrlRetrievalTimeout = 0,
              DefaultFlags = requireClientCertificates ? ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_FLAGS.NEGOTIATE_CLIENT_CERT : (ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_FLAGS) 0,
              DefaultCertCheckMode = ConfigurationHelper.CertificateCheckMode.RevocationCheckEnabled,
              AppId = appId,
              pSslCertStoreName = storeName.ToString(),
              pDefaultSslCtlIdentifier = (string) null,
              pDefaultSslCtlStoreName = (string) null
            };
            try
            {
              ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_SET pConfigInformation = new ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_SET()
              {
                KeyDesc = serviceConfigSslKey,
                ParamDesc = serviceConfigSslParam
              };
              error1 = ConfigurationHelper.HttpSetServiceConfiguration(IntPtr.Zero, ConfigurationHelper.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigSSLCertInfo, ref pConfigInformation, Marshal.SizeOf<ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_SET>(pConfigInformation), IntPtr.Zero);
              if (error1 == 183)
              {
                error1 = ConfigurationHelper.HttpDeleteServiceConfiguration(IntPtr.Zero, ConfigurationHelper.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigSSLCertInfo, ref pConfigInformation, Marshal.SizeOf<ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_SET>(pConfigInformation), IntPtr.Zero);
                if (error1 == 0 || error1 == 2)
                {
                  error1 = ConfigurationHelper.HttpSetServiceConfiguration(IntPtr.Zero, ConfigurationHelper.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigSSLCertInfo, ref pConfigInformation, Marshal.SizeOf<ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_SET>(pConfigInformation), IntPtr.Zero);
                  error1 = 0;
                }
                flag = false;
              }
            }
            finally
            {
              serviceConfigSslParam.Dispose();
            }
          }
          finally
          {
            serviceConfigSslKey.Dispose();
          }
        }
      }
      finally
      {
        int error2 = ConfigurationHelper.HttpTerminate(2U, IntPtr.Zero);
        if (error2 != 0)
          throw new Win32Exception(error2);
      }
      if (error1 != 0)
        throw new Win32Exception(error1);
      return flag;
    }

    public static bool DeleteSslCertificate(IPAddress address, int port)
    {
      bool flag = true;
      try
      {
        if (ConfigurationHelper.HttpInitialize(ConfigurationHelper.HttpApiVersion, 2U, IntPtr.Zero) == 0)
        {
          ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_KEY serviceConfigSslKey = new ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_KEY(new ConfigurationHelper.SOCKADDR_IN(address, (ushort) port));
          try
          {
            ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_PARAM serviceConfigSslParam = new ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_PARAM();
            try
            {
              ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_SET pConfigInformation = new ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_SET()
              {
                KeyDesc = serviceConfigSslKey,
                ParamDesc = serviceConfigSslParam
              };
              flag = ConfigurationHelper.HttpDeleteServiceConfiguration(IntPtr.Zero, ConfigurationHelper.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigSSLCertInfo, ref pConfigInformation, Marshal.SizeOf<ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_SET>(pConfigInformation), IntPtr.Zero) == 0;
            }
            finally
            {
              serviceConfigSslParam.Dispose();
            }
          }
          finally
          {
            serviceConfigSslKey.Dispose();
          }
        }
      }
      finally
      {
        int error = ConfigurationHelper.HttpTerminate(2U, IntPtr.Zero);
        if (error != 0)
          throw new Win32Exception(error);
      }
      return flag;
    }

    internal static byte[] GetSidBinaryFromWindows(string domain, string user)
    {
      try
      {
        SecurityIdentifier securityIdentifier = (SecurityIdentifier) new NTAccount(domain + "\\" + user).Translate(typeof (SecurityIdentifier));
        byte[] binaryForm = new byte[securityIdentifier.BinaryLength];
        securityIdentifier.GetBinaryForm(binaryForm, 0);
        return binaryForm;
      }
      catch
      {
        return (byte[]) null;
      }
    }

    public static bool GrantLogonAsService(string domain, string username)
    {
      if (ConfigurationHelper.GetUserHasLogonAsService(domain, username))
        return false;
      using (LsaPolicy lsaPolicy = new LsaPolicy())
        return Microsoft.TeamFoundation.Common.Internal.NativeMethods.LsaAddAccountRights(lsaPolicy.Handle, ConfigurationHelper.GetSidBinaryFromWindows(domain, username), ConfigurationHelper.LogonAsServiceRights, 1U) == 0U;
    }

    public static bool RevokeLogonAsService(string domain, string username)
    {
      if (!ConfigurationHelper.GetUserHasLogonAsService(domain, username))
        return false;
      using (LsaPolicy lsaPolicy = new LsaPolicy())
        return Microsoft.TeamFoundation.Common.Internal.NativeMethods.LsaRemoveAccountRights(lsaPolicy.Handle, ConfigurationHelper.GetSidBinaryFromWindows(domain, username), (byte) 0, ConfigurationHelper.LogonAsServiceRights, 1U) == 0U;
    }

    public static bool GetUserHasLogonAsService(string domain, string username)
    {
      bool hasLogonAsService = false;
      using (LsaPolicy lsaPolicy = new LsaPolicy())
      {
        IntPtr UserRights;
        uint CountOfRights;
        uint num1 = Microsoft.TeamFoundation.Common.Internal.NativeMethods.LsaEnumerateAccountRights(lsaPolicy.Handle, ConfigurationHelper.GetSidBinaryFromWindows(domain, username), out UserRights, out CountOfRights);
        try
        {
          if (num1 == 0U)
          {
            IntPtr ptr = UserRights;
            for (int index = 0; (long) index < (long) CountOfRights; ++index)
            {
              Microsoft.TeamFoundation.Common.Internal.NativeMethods.LSA_UNICODE_STRING structure = (Microsoft.TeamFoundation.Common.Internal.NativeMethods.LSA_UNICODE_STRING) Marshal.PtrToStructure(ptr, typeof (Microsoft.TeamFoundation.Common.Internal.NativeMethods.LSA_UNICODE_STRING));
              if (string.Equals(Marshal.PtrToStringAuto(structure.Buffer), ConfigurationHelper.s_logonAsServiceName, StringComparison.OrdinalIgnoreCase))
                hasLogonAsService = true;
              ptr += Marshal.SizeOf<Microsoft.TeamFoundation.Common.Internal.NativeMethods.LSA_UNICODE_STRING>(structure);
            }
          }
        }
        finally
        {
          int num2 = (int) Microsoft.TeamFoundation.Common.Internal.NativeMethods.LsaFreeMemory(UserRights);
        }
      }
      return hasLogonAsService;
    }

    internal static Microsoft.TeamFoundation.Common.Internal.NativeMethods.LSA_UNICODE_STRING[] LogonAsServiceRights => new Microsoft.TeamFoundation.Common.Internal.NativeMethods.LSA_UNICODE_STRING[1]
    {
      new Microsoft.TeamFoundation.Common.Internal.NativeMethods.LSA_UNICODE_STRING()
      {
        Buffer = Marshal.StringToHGlobalUni(ConfigurationHelper.s_logonAsServiceName),
        Length = (ushort) (ConfigurationHelper.s_logonAsServiceName.Length * 2),
        MaximumLength = (ushort) ((ConfigurationHelper.s_logonAsServiceName.Length + 1) * 2)
      }
    };

    public static bool TryQueryServiceConfig(
      string serviceName,
      out ConfigurationHelper.QUERY_SERVICE_CONFIG serviceConfig)
    {
      serviceConfig = new ConfigurationHelper.QUERY_SERVICE_CONFIG();
      using (ServiceController serviceController = new ServiceController(serviceName))
      {
        try
        {
          int pcbBytesNeeded;
          if (!ConfigurationHelper.QueryServiceConfig(serviceController.ServiceHandle, IntPtr.Zero, 0, out pcbBytesNeeded))
          {
            if (Marshal.GetLastWin32Error() == 122)
            {
              IntPtr num = IntPtr.Zero;
              try
              {
                num = Marshal.AllocHGlobal(pcbBytesNeeded);
                if (ConfigurationHelper.QueryServiceConfig(serviceController.ServiceHandle, num, pcbBytesNeeded, out pcbBytesNeeded))
                {
                  serviceConfig = (ConfigurationHelper.QUERY_SERVICE_CONFIG) Marshal.PtrToStructure(num, typeof (ConfigurationHelper.QUERY_SERVICE_CONFIG));
                  if (!string.IsNullOrEmpty(serviceConfig.lpBinaryPathName))
                    serviceConfig.lpBinaryPathName = serviceConfig.lpBinaryPathName.Trim('"');
                  return true;
                }
              }
              finally
              {
                Marshal.FreeHGlobal(num);
              }
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
      return false;
    }

    [DllImport("Ws2_32.dll")]
    private static extern ushort htons(ushort hostshort);

    [DllImport("Ws2_32.dll")]
    private static extern ushort ntohs(ushort hostshort);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool QueryServiceConfig(
      SafeHandle hService,
      IntPtr intPtrQueryConfig,
      int cbBufSize,
      out int pcbBytesNeeded);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [CLSCompliant(false)]
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int ChangeServiceConfig(
      SafeHandle handle,
      uint type,
      uint startType,
      uint errorControl,
      string binaryPathName,
      string loadOrderGroup,
      string tagId,
      string dependencies,
      string accountName,
      string password,
      string displayName);

    [DllImport("httpapi.dll", SetLastError = true)]
    private static extern int HttpDeleteServiceConfiguration(
      IntPtr ServiceIntPtr,
      ConfigurationHelper.HTTP_SERVICE_CONFIG_ID ConfigId,
      ref ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_SET pConfigInformation,
      int ConfigInformationLength,
      IntPtr pOverlapped);

    [DllImport("httpapi.dll", SetLastError = true)]
    private static extern int HttpDeleteServiceConfiguration(
      IntPtr ServiceIntPtr,
      ConfigurationHelper.HTTP_SERVICE_CONFIG_ID ConfigId,
      ref ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_SET pConfigInformation,
      int ConfigInformationLength,
      IntPtr pOverlapped);

    [DllImport("httpapi.dll", SetLastError = true)]
    private static extern int HttpInitialize(
      ConfigurationHelper.HTTPAPI_VERSION Version,
      uint Flags,
      IntPtr pReserved);

    [DllImport("httpapi.dll", SetLastError = true)]
    private static extern int HttpSetServiceConfiguration(
      IntPtr ServiceIntPtr,
      ConfigurationHelper.HTTP_SERVICE_CONFIG_ID ConfigId,
      ref ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_SET pConfigInformation,
      int ConfigInformationLength,
      IntPtr pOverlapped);

    [DllImport("httpapi.dll", SetLastError = true)]
    private static extern int HttpSetServiceConfiguration(
      IntPtr ServiceIntPtr,
      ConfigurationHelper.HTTP_SERVICE_CONFIG_ID ConfigId,
      ref ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_SET pConfigInformation,
      int ConfigInformationLength,
      IntPtr pOverlapped);

    [DllImport("httpapi.dll", SetLastError = true)]
    private static extern int HttpTerminate(uint Flags, IntPtr pReserved);

    internal static class ERROR
    {
      public const int ERR_SUCCESS = 0;
      public const int ERR_FILE_NOT_FOUND = 2;
      public const int ERR_INVALID_PARAMETER = 87;
      public const int ERR_INSUFFICIENT_BUFFER = 122;
      public const int ERR_ALREADY_EXISTS = 183;
      public const int ERR_NO_MORE_ITEMS = 259;
    }

    internal static class PermissionConstants
    {
      public const int GENERIC_WRITE = 1073741824;
      public const int GENERIC_EXECUTE = 536870912;
      public const int GENERIC_ALL = 268435456;
    }

    internal enum HTTP_SERVICE_CONFIG_ID
    {
      HttpServiceConfigIPListenList,
      HttpServiceConfigSSLCertInfo,
      HttpServiceConfigUrlAclInfo,
      HttpServiceConfigMax,
    }

    internal enum HTTP_SERVICE_CONFIG_QUERY_TYPE
    {
      HttpServiceConfigQueryExact,
      HttpServiceConfigQueryNext,
      HttpServiceConfigQueryMax,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct HTTP_SERVICE_CONFIG_URLACL_KEY
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pUrlPrefix;

      public HTTP_SERVICE_CONFIG_URLACL_KEY(string urlPrefix) => this.pUrlPrefix = urlPrefix;
    }

    internal struct HTTP_SERVICE_CONFIG_SSL_SET
    {
      [MarshalAs(UnmanagedType.Struct)]
      public ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_KEY KeyDesc;
      [MarshalAs(UnmanagedType.Struct)]
      public ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_PARAM ParamDesc;
    }

    internal struct HTTP_SERVICE_CONFIG_SSL_QUERY
    {
      public ConfigurationHelper.HTTP_SERVICE_CONFIG_QUERY_TYPE QueryDesc;
      [MarshalAs(UnmanagedType.Struct)]
      public ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_KEY KeyDesc;
      public uint dwToken;
    }

    private class SafeHGlobalHandle : SafeHandle
    {
      public SafeHGlobalHandle()
        : base(new IntPtr(-1), true)
      {
      }

      public SafeHGlobalHandle(IntPtr preexistingHandle)
        : this()
      {
        this.SetHandle(preexistingHandle);
      }

      public override bool IsInvalid => this.handle == new IntPtr(-1);

      protected override bool ReleaseHandle()
      {
        if (!this.IsInvalid)
        {
          Marshal.FreeHGlobal(this.handle);
          this.SetHandleAsInvalid();
        }
        return true;
      }

      public static implicit operator IntPtr(ConfigurationHelper.SafeHGlobalHandle handle) => handle.handle;

      public static explicit operator ConfigurationHelper.SafeHGlobalHandle(IntPtr handle) => new ConfigurationHelper.SafeHGlobalHandle(handle);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct HTTP_SERVICE_CONFIG_SSL_KEY
    {
      private IntPtr _pIpPort;

      public ConfigurationHelper.SOCKADDR_IN pIpPort
      {
        get => this._pIpPort == IntPtr.Zero ? new ConfigurationHelper.SOCKADDR_IN() : (ConfigurationHelper.SOCKADDR_IN) Marshal.PtrToStructure(this._pIpPort, typeof (ConfigurationHelper.SOCKADDR_IN));
        set
        {
          if (this._pIpPort == IntPtr.Zero)
          {
            this._pIpPort = Marshal.AllocHGlobal(Marshal.SizeOf<ConfigurationHelper.SOCKADDR_IN>(value));
            Marshal.StructureToPtr<ConfigurationHelper.SOCKADDR_IN>(value, this._pIpPort, false);
          }
          else
            Marshal.StructureToPtr<ConfigurationHelper.SOCKADDR_IN>(value, this._pIpPort, true);
        }
      }

      public HTTP_SERVICE_CONFIG_SSL_KEY(ConfigurationHelper.SOCKADDR_IN ipport)
      {
        this._pIpPort = IntPtr.Zero;
        this.pIpPort = ipport;
      }

      public void Dispose()
      {
        if (!(this._pIpPort != IntPtr.Zero))
          return;
        Marshal.FreeHGlobal(this._pIpPort);
        this._pIpPort = IntPtr.Zero;
      }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct SOCKADDR_IN
    {
      public ushort sin_family;
      private ushort _sin_port;
      public byte s_b1;
      public byte s_b2;
      public byte s_b3;
      public byte s_b4;
      private long sin_zero;

      public ushort sin_port
      {
        get => ConfigurationHelper.ntohs(this._sin_port);
        set => this._sin_port = ConfigurationHelper.htons(value);
      }

      public SOCKADDR_IN(IPAddress addr, ushort port)
      {
        this.sin_family = addr.AddressFamily == AddressFamily.InterNetwork ? (ushort) addr.AddressFamily : throw new NotSupportedException();
        this._sin_port = (ushort) 0;
        byte[] addressBytes = addr.GetAddressBytes();
        this.s_b1 = addressBytes[0];
        this.s_b2 = addressBytes[1];
        this.s_b3 = addressBytes[2];
        this.s_b4 = addressBytes[3];
        this.sin_zero = 0L;
        this.sin_port = port;
      }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct HTTP_SERVICE_CONFIG_SSL_PARAM
    {
      private uint SslHashLength;
      private IntPtr _pSslHash;
      public Guid AppId;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pSslCertStoreName;
      public ConfigurationHelper.CertificateCheckMode DefaultCertCheckMode;
      public uint DefaultRevocationFreshnessTime;
      public uint DefaultRevocationUrlRetrievalTimeout;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pDefaultSslCtlIdentifier;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pDefaultSslCtlStoreName;
      public ConfigurationHelper.HTTP_SERVICE_CONFIG_SSL_FLAGS DefaultFlags;

      public byte[] pSslHash
      {
        get
        {
          if (this._pSslHash == IntPtr.Zero)
            return (byte[]) null;
          byte[] destination = new byte[(int) this.SslHashLength];
          Marshal.Copy(this._pSslHash, destination, 0, (int) this.SslHashLength);
          return destination;
        }
        set
        {
          if (this._pSslHash != IntPtr.Zero)
          {
            Marshal.FreeHGlobal(this._pSslHash);
            this._pSslHash = IntPtr.Zero;
          }
          if (value == null)
          {
            this.SslHashLength = 0U;
          }
          else
          {
            this._pSslHash = Marshal.AllocHGlobal(value.Length);
            Marshal.Copy(value, 0, this._pSslHash, value.Length);
            this.SslHashLength = (uint) value.Length;
          }
        }
      }

      public void Dispose()
      {
        if (!(this._pSslHash != IntPtr.Zero))
          return;
        Marshal.FreeHGlobal(this._pSslHash);
        this._pSslHash = IntPtr.Zero;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum ServiceStartMode
    {
      SERVICE_BOOT_START,
      SERVICE_SYSTEM_START,
      SERVICE_AUTO_START,
      SERVICE_DEMAND_START,
      SERVICE_DISABLED,
    }

    internal enum CertificateCheckMode
    {
      RevocationCheckEnabled = 0,
      RevocationCheckDisabled = 1,
      CachedRevocationOnly = 2,
      DefaultRevocationFreshnessTime = 4,
      None = 65536, // 0x00010000
    }

    [Flags]
    internal enum HTTP_SERVICE_CONFIG_SSL_FLAGS : uint
    {
      USE_DS_MAPPER = 1,
      NEGOTIATE_CLIENT_CERT = 2,
      NO_RAW_FILTER = 4,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct HTTP_SERVICE_CONFIG_URLACL_PARAM
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pStringSecurityDescriptor;

      public HTTP_SERVICE_CONFIG_URLACL_PARAM(string securityDescriptor) => this.pStringSecurityDescriptor = securityDescriptor;
    }

    internal struct HTTP_SERVICE_CONFIG_URLACL_SET
    {
      public ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_KEY KeyDesc;
      public ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_PARAM ParamDesc;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct HTTPAPI_VERSION
    {
      public ushort HttpApiMajorVersion;
      public ushort HttpApiMinorVersion;

      public HTTPAPI_VERSION(ushort majorVersion, ushort minorVersion)
      {
        this.HttpApiMajorVersion = majorVersion;
        this.HttpApiMinorVersion = minorVersion;
      }
    }

    internal struct HTTP_SERVICE_CONFIG_URLACL_QUERY
    {
      public ConfigurationHelper.HTTP_SERVICE_CONFIG_QUERY_TYPE QueryDesc;
      public ConfigurationHelper.HTTP_SERVICE_CONFIG_URLACL_KEY KeyDesc;
      public uint dwToken;
    }

    public struct QUERY_SERVICE_CONFIG
    {
      [MarshalAs(UnmanagedType.I4)]
      public int dwServiceType;
      [MarshalAs(UnmanagedType.I4)]
      public int dwStartType;
      [MarshalAs(UnmanagedType.I4)]
      public int dwErrorControl;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpBinaryPathName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpLoadOrderGroup;
      [MarshalAs(UnmanagedType.I4)]
      public int dwTagID;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpDependencies;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpServiceStartName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpDisplayName;
    }
  }
}
