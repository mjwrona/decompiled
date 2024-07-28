// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.BypassProxyOnLocalHelper
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Client
{
  internal static class BypassProxyOnLocalHelper
  {
    private static readonly bool s_supportsIpv6;
    private static readonly string s_domainSuffix;
    private static readonly int s_sockAddrIn6Size = Marshal.SizeOf(typeof (Microsoft.TeamFoundation.Common.Internal.NativeMethods.SOCKADDR_IN6));
    private static readonly int s_sockAddrInSize = Marshal.SizeOf(typeof (Microsoft.TeamFoundation.Common.Internal.NativeMethods.SOCKADDR_IN));

    static BypassProxyOnLocalHelper()
    {
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.WsaData lpWsaData = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.WsaData();
      if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.WSAStartup((short) 514, out lpWsaData) != 0U)
        throw new SocketException();
      try
      {
        BypassProxyOnLocalHelper.s_domainSuffix = "." + IPGlobalProperties.GetIPGlobalProperties().DomainName;
      }
      catch
      {
        BypassProxyOnLocalHelper.s_domainSuffix = ".";
      }
      BypassProxyOnLocalHelper.s_supportsIpv6 = Socket.OSSupportsIPv6;
    }

    public static bool IsHostLocal(string hostName)
    {
      if (string.IsNullOrEmpty(hostName))
        return false;
      if (StringComparer.OrdinalIgnoreCase.Equals(hostName, "localhost") || StringComparer.OrdinalIgnoreCase.Equals(hostName, "loopback"))
        return true;
      if (BypassProxyOnLocalHelper.s_supportsIpv6)
      {
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.SOCKADDR_IN6 lpAddress = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.SOCKADDR_IN6();
        int sockAddrIn6Size = BypassProxyOnLocalHelper.s_sockAddrIn6Size;
        if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.WSAStringToAddressW(hostName, Microsoft.TeamFoundation.Common.Internal.NativeMethods.AF_INET6, IntPtr.Zero, ref lpAddress, ref sockAddrIn6Size) == 0U)
          return lpAddress.sin6_addr1 == 0U && lpAddress.sin6_addr2 == 0U && lpAddress.sin6_addr3 == 0U && lpAddress.sin6_addr4 == 16777216U;
      }
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.SOCKADDR_IN lpAddress1 = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.SOCKADDR_IN();
      int sockAddrInSize = BypassProxyOnLocalHelper.s_sockAddrInSize;
      if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.WSAStringToAddressW(hostName, Microsoft.TeamFoundation.Common.Internal.NativeMethods.AF_INET, IntPtr.Zero, ref lpAddress1, ref sockAddrInSize) == 0U)
        return 16777343U == lpAddress1.sin_addr;
      int indexB = hostName.IndexOf('.');
      return indexB < 0 || BypassProxyOnLocalHelper.s_domainSuffix.Length == hostName.Length - indexB && string.Compare(BypassProxyOnLocalHelper.s_domainSuffix, 0, hostName, indexB, BypassProxyOnLocalHelper.s_domainSuffix.Length, StringComparison.OrdinalIgnoreCase) == 0 || TFUtil.IsLocalDevFabric(hostName);
    }
  }
}
