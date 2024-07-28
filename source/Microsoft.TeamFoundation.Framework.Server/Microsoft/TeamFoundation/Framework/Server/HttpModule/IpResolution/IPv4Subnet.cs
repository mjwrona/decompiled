// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution.IPv4Subnet
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution
{
  [JsonConverter(typeof (JsonIPv4SubnetConverter))]
  internal class IPv4Subnet
  {
    public int Address { get; private set; }

    public int Mask { get; private set; }

    public static IPv4Subnet Parse(string ipAddressWithMask)
    {
      string[] strArray = ipAddressWithMask.Split('/');
      return strArray.Length == 2 ? new IPv4Subnet(IPv4Subnet.ParseIpAddress(strArray[0]), IPv4Subnet.ParseIpMask(strArray[1])) : throw new ArgumentException("Submitted IP address is not separated by a '/' from mask: " + ipAddressWithMask, nameof (ipAddressWithMask));
    }

    public bool Contains(int ip) => (ip & this.Mask) == this.Address;

    private IPv4Subnet(int address, int mask)
    {
      this.Address = address & mask;
      this.Mask = mask;
    }

    public override string ToString()
    {
      int address = this.Address;
      int num1 = address & (int) byte.MaxValue;
      int num2 = address >> 8;
      int num3 = num2 & (int) byte.MaxValue;
      int num4 = num2 >> 8;
      int num5 = num4 & (int) byte.MaxValue;
      int num6 = num4 >> 8 & (int) byte.MaxValue;
      int mask = this.Mask;
      int num7;
      for (num7 = 32; num7 > 0 && (mask & 1) == 0; --num7)
        mask >>= 1;
      return string.Format("{0}.{1}.{2}.{3}/{4}", (object) num6, (object) num5, (object) num3, (object) num1, (object) num7);
    }

    internal static int ParseIpMask(string maskBits)
    {
      int result;
      if (!int.TryParse(maskBits, out result) || result < 1 || result > 32)
        throw new ArgumentException("Submitted mask is not a valid number in 1 - 32 range", nameof (maskBits));
      return int.MinValue >> result - 1;
    }

    public static int ParseIpAddress(string ip)
    {
      string[] strArray = !ip.Contains<char>(':') ? ip.Split('.') : throw new ArgumentException("Submitted IP address is not a valid IPv4 address: " + ip, nameof (ip));
      if (strArray.Length != 4)
        throw new ArgumentException("Submitted IP address does not consist of 4 blocks separated by dots: " + ip, nameof (ip));
      int ipAddress = 0;
      for (int blockNumber = 0; blockNumber < 4; ++blockNumber)
        ipAddress += IPv4Subnet.ParseBlock(strArray[blockNumber], blockNumber);
      return ipAddress;
    }

    private static int ParseBlock(string inputBlock, int blockNumber)
    {
      int result;
      if (!int.TryParse(inputBlock, out result) || result < 0 || result > (int) byte.MaxValue)
        throw new ArgumentException("Submitted IP address part is not a valid number in 0-255 range: " + inputBlock, nameof (inputBlock));
      return result << (3 - blockNumber) * 8;
    }
  }
}
