// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IPAddressRange
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class IPAddressRange
  {
    private static (string Value, byte[] RangeBytes, int RangeBits)[] s_IPv4SpecialPurposeAddressRanges = new (string, byte[], int)[25]
    {
      ("0.0.0.0/8", IPAddress.Parse("0.0.0.0").GetAddressBytes(), 8),
      ("10.0.0.0/8", IPAddress.Parse("10.0.0.0").GetAddressBytes(), 8),
      ("100.64.0.0/10", IPAddress.Parse("100.64.0.0").GetAddressBytes(), 10),
      ("127.0.0.0/8", IPAddress.Parse("127.0.0.0").GetAddressBytes(), 8),
      ("168.63.129.16/32", IPAddress.Parse("168.63.129.16").GetAddressBytes(), 32),
      ("169.254.0.0/16", IPAddress.Parse("169.254.0.0").GetAddressBytes(), 16),
      ("172.16.0.0/12", IPAddress.Parse("172.16.0.0").GetAddressBytes(), 12),
      ("192.0.0.0/24", IPAddress.Parse("192.0.0.0").GetAddressBytes(), 24),
      ("192.0.0.0/29", IPAddress.Parse("192.0.0.0").GetAddressBytes(), 29),
      ("192.0.0.8/32", IPAddress.Parse("192.0.0.8").GetAddressBytes(), 32),
      ("192.0.0.9/32", IPAddress.Parse("192.0.0.9").GetAddressBytes(), 32),
      ("192.0.0.10/32", IPAddress.Parse("192.0.0.10").GetAddressBytes(), 32),
      ("192.0.0.170/32", IPAddress.Parse("192.0.0.170").GetAddressBytes(), 32),
      ("192.0.0.171/32", IPAddress.Parse("192.0.0.171").GetAddressBytes(), 32),
      ("192.0.2.0/24", IPAddress.Parse("192.0.2.0").GetAddressBytes(), 24),
      ("192.31.196.0/24", IPAddress.Parse("192.31.196.0").GetAddressBytes(), 24),
      ("192.52.193.0/24", IPAddress.Parse("192.52.193.0").GetAddressBytes(), 24),
      ("192.88.99.0/24", IPAddress.Parse("192.88.99.0").GetAddressBytes(), 24),
      ("192.168.0.0/16", IPAddress.Parse("192.168.0.0").GetAddressBytes(), 16),
      ("192.175.48.0/24", IPAddress.Parse("192.175.48.0").GetAddressBytes(), 24),
      ("198.18.0.0/15", IPAddress.Parse("198.18.0.0").GetAddressBytes(), 15),
      ("198.51.100.0/24", IPAddress.Parse("198.51.100.0").GetAddressBytes(), 24),
      ("203.0.113.0/24", IPAddress.Parse("203.0.113.0").GetAddressBytes(), 24),
      ("240.0.0.0/4", IPAddress.Parse("240.0.0.0").GetAddressBytes(), 4),
      ("255.255.255.255/32", IPAddress.Parse("255.255.255.255").GetAddressBytes(), 32)
    };
    private static (string Value, byte[] RangeAddressBytes, int RangeBits)[] s_IPv6SpecialPurposeAddressRanges = new (string, byte[], int)[20]
    {
      ("::1/128", IPAddress.Parse("::1").GetAddressBytes(), 128),
      ("::ffff:0:0/96", IPAddress.Parse("::ffff:0:0").GetAddressBytes(), 96),
      ("64:ff9b::/96", IPAddress.Parse("64:ff9b::").GetAddressBytes(), 96),
      ("64:ff9b:1::/48", IPAddress.Parse("64:ff9b:1::").GetAddressBytes(), 48),
      ("100::/64", IPAddress.Parse("100::").GetAddressBytes(), 64),
      ("2001::/23", IPAddress.Parse("2001::").GetAddressBytes(), 23),
      ("2001::/32", IPAddress.Parse("2001::").GetAddressBytes(), 32),
      ("2001:1::1/128", IPAddress.Parse("2001:1::1").GetAddressBytes(), 128),
      ("2001:1::2/128", IPAddress.Parse("2001:1::2").GetAddressBytes(), 128),
      ("2001:2::/48", IPAddress.Parse("2001:2::").GetAddressBytes(), 48),
      ("2001:3::/32", IPAddress.Parse("2001:3::").GetAddressBytes(), 32),
      ("2001:4:112::/48", IPAddress.Parse("2001:4:112::").GetAddressBytes(), 48),
      ("2001:5::/32", IPAddress.Parse("2001:5::").GetAddressBytes(), 32),
      ("2001:10::/28", IPAddress.Parse("2001:10::").GetAddressBytes(), 28),
      ("2001:20::/28", IPAddress.Parse("2001:20::").GetAddressBytes(), 28),
      ("2001:db8::/32", IPAddress.Parse("2001:db8::").GetAddressBytes(), 32),
      ("2002::/16", IPAddress.Parse("2002::").GetAddressBytes(), 16),
      ("2620:4f:8000::/48", IPAddress.Parse("2620:4f:8000::").GetAddressBytes(), 48),
      ("fc00::/7", IPAddress.Parse("fc00::").GetAddressBytes(), 7),
      ("fe80::/10", IPAddress.Parse("fe80::").GetAddressBytes(), 10)
    };
    private static readonly char[] s_ipAddressRangeSeparators = new char[1]
    {
      '/'
    };
    private static readonly byte[] s_masksByBitCount = new byte[8]
    {
      byte.MaxValue,
      (byte) 128,
      (byte) 192,
      (byte) 224,
      (byte) 240,
      (byte) 248,
      (byte) 252,
      (byte) 254
    };

    public static bool IsValidRange(string range) => IPAddressRange.TryParse(range, out byte[] _, out int _);

    public static bool IsAddressInRange(string range, IPAddress address)
    {
      byte[] rangeAddressBytes;
      int rangeBits;
      return IPAddressRange.TryParse(range, out rangeAddressBytes, out rangeBits) && IPAddressRange.IsAddressInRange(rangeAddressBytes, rangeBits, address.GetAddressBytes());
    }

    public static bool IsAddressInRange(
      byte[] rangeAddressBytes,
      int rangeBits,
      byte[] addressBytes)
    {
      if (addressBytes.Length != rangeAddressBytes.Length)
        return false;
      for (int index1 = 0; index1 < addressBytes.Length && rangeBits > 0; rangeBits -= 8)
      {
        int index2 = rangeBits < 8 ? rangeBits : 0;
        byte num = IPAddressRange.s_masksByBitCount[index2];
        if (((int) rangeAddressBytes[index1] & (int) num) != ((int) addressBytes[index1] & (int) num))
          return false;
        ++index1;
      }
      return true;
    }

    private static bool TryParse(string range, out byte[] rangeAddressBytes, out int rangeBits)
    {
      string[] strArray = range.Split(IPAddressRange.s_ipAddressRangeSeparators);
      if (strArray.Length != 2)
      {
        rangeAddressBytes = (byte[]) null;
        rangeBits = 0;
        return false;
      }
      IPAddress address;
      if (!IPAddress.TryParse(strArray[0], out address))
      {
        rangeAddressBytes = (byte[]) null;
        rangeBits = 0;
        return false;
      }
      rangeAddressBytes = address.GetAddressBytes();
      if (int.TryParse(strArray[1], out rangeBits) && rangeBits >= 0 && rangeBits <= rangeAddressBytes.Length * 8)
        return true;
      rangeAddressBytes = (byte[]) null;
      rangeBits = 0;
      return false;
    }

    public static IPAddress[] GetHostAddresses(string url, bool throwIfInvalidHost = true)
    {
      Uri result;
      if (Uri.TryCreate(url, UriKind.Absolute, out result))
      {
        if (!string.IsNullOrWhiteSpace(result.Host))
        {
          try
          {
            return Dns.GetHostAddresses(result.Host);
          }
          catch (Exception ex)
          {
            if (throwIfInvalidHost)
              throw new TeamFoundationServerException(string.Format(FrameworkResources.NoDnsAddressForHost((object) result.Host, (object) ex.Message)));
            return Array.Empty<IPAddress>();
          }
        }
      }
      if (throwIfInvalidHost)
        throw new TeamFoundationServerException(FrameworkResources.UriMissingHostSegment());
      return Array.Empty<IPAddress>();
    }

    public static bool IsLoopbackIPAddress(string url) => !string.IsNullOrWhiteSpace(url) && IPAddressRange.IsLoopbackIPAddress(IPAddressRange.GetHostAddresses(url));

    public static bool IsLocalhostIPAddress(string url, out string matchingHostIPValue)
    {
      if (!string.IsNullOrWhiteSpace(url))
        return IPAddressRange.IsLocalhostIPAddress(IPAddressRange.GetHostAddresses(url), out matchingHostIPValue);
      matchingHostIPValue = (string) null;
      return false;
    }

    public static bool IsSpecialPurposeIPAddress(
      string url,
      out string matchedHostIPValue,
      out string matchedRangeValue)
    {
      if (!string.IsNullOrWhiteSpace(url))
        return IPAddressRange.IsSpecialPurposeIPAddress(IPAddressRange.GetHostAddresses(url), out matchedHostIPValue, out matchedRangeValue);
      matchedHostIPValue = (string) null;
      matchedRangeValue = (string) null;
      return false;
    }

    public static bool IsLoopbackIPAddress(IPAddress[] hostIPs) => hostIPs != null && hostIPs.Length != 0 && ((IEnumerable<IPAddress>) hostIPs).Any<IPAddress>((Func<IPAddress, bool>) (hostIP => IPAddress.IsLoopback(hostIP)));

    public static bool IsLocalhostIPAddress(IPAddress[] hostIPs, out string matchingHostIPValue)
    {
      matchingHostIPValue = (string) null;
      if (hostIPs != null)
      {
        if (hostIPs.Length != 0)
        {
          try
          {
            IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress hostIp in hostIPs)
            {
              if (((IEnumerable<IPAddress>) hostAddresses).Contains<IPAddress>(hostIp))
              {
                matchingHostIPValue = hostIp.ToString();
                return true;
              }
            }
          }
          catch
          {
          }
          return false;
        }
      }
      return false;
    }

    public static bool IsSpecialPurposeIPAddress(
      IPAddress[] hostIPs,
      out string matchedHostIPValue,
      out string matchedRangeValue)
    {
      matchedHostIPValue = (string) null;
      matchedRangeValue = (string) null;
      if (hostIPs == null || hostIPs.Length == 0)
        return false;
      foreach (IPAddress hostIp in hostIPs)
      {
        (string, byte[], int)[] valueTupleArray;
        switch (hostIp.AddressFamily)
        {
          case AddressFamily.InterNetwork:
            valueTupleArray = IPAddressRange.s_IPv4SpecialPurposeAddressRanges;
            break;
          case AddressFamily.InterNetworkV6:
            valueTupleArray = IPAddressRange.s_IPv6SpecialPurposeAddressRanges;
            break;
          default:
            valueTupleArray = ((string, byte[], int)[]) null;
            break;
        }
        if (valueTupleArray != null)
        {
          byte[] addressBytes = hostIp.GetAddressBytes();
          foreach ((string, byte[], int) valueTuple in valueTupleArray)
          {
            if (IPAddressRange.IsAddressInRange(valueTuple.Item2, valueTuple.Item3, addressBytes))
            {
              matchedHostIPValue = hostIp.ToString();
              matchedRangeValue = valueTuple.Item1;
              return true;
            }
          }
        }
      }
      return false;
    }
  }
}
