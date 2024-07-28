// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.NetUtil
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Microsoft.Azure.Documents
{
  internal static class NetUtil
  {
    public static string GetNonLoopbackIpV4Address()
    {
      foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
      {
        if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet && networkInterface.OperationalStatus == OperationalStatus.Up)
        {
          foreach (IPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
          {
            if (unicastAddress.IsDnsEligible && unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
              return unicastAddress.Address.ToString();
          }
        }
      }
      string message = "ERROR: Could not locate any usable IPv4 address";
      DefaultTrace.TraceCritical(message);
      throw new ConfigurationErrorsException(message);
    }

    public static string GetLocalEmulatorIpV4Address()
    {
      string emulatorIpV4Address = (string) null;
      foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
      {
        if ((networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) && networkInterface.OperationalStatus == OperationalStatus.Up)
        {
          foreach (IPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
          {
            if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
            {
              if (unicastAddress.IsDnsEligible)
                return unicastAddress.Address.ToString();
              if (emulatorIpV4Address == null)
                emulatorIpV4Address = unicastAddress.Address.ToString();
            }
          }
        }
      }
      if (emulatorIpV4Address != null)
        return emulatorIpV4Address;
      string message = "ERROR: Could not locate any usable IPv4 address for local emulator";
      DefaultTrace.TraceCritical(message);
      throw new ConfigurationErrorsException(message);
    }

    public static bool GetIPv6ServiceTunnelAddress(
      bool isEmulated,
      out IPAddress ipv6LoopbackAddress)
    {
      if (isEmulated)
      {
        ipv6LoopbackAddress = IPAddress.IPv6Loopback;
        return true;
      }
      foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
      {
        foreach (UnicastIPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
        {
          if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetworkV6 && NetUtil.IsServiceTunneledIPAddress(unicastAddress.Address))
          {
            DefaultTrace.TraceInformation("Found VNET service tunnel destination: {0}", (object) unicastAddress.Address.ToString());
            ipv6LoopbackAddress = unicastAddress.Address;
            return true;
          }
          DefaultTrace.TraceVerbose("{0} is skipped because it is not IPv6 or is not a service tunneled IP address.", (object) unicastAddress.Address.ToString());
        }
      }
      DefaultTrace.TraceInformation("Cannot find the IPv6 address of the Loopback NetworkInterface.");
      ipv6LoopbackAddress = (IPAddress) null;
      return false;
    }

    private static bool IsServiceTunneledIPAddress(IPAddress ipAddress) => (long) BitConverter.ToUInt64(ipAddress.GetAddressBytes(), 0) == (long) BitConverter.ToUInt64(new byte[8]
    {
      (byte) 38,
      (byte) 3,
      (byte) 16,
      (byte) 225,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 2
    }, 0);
  }
}
