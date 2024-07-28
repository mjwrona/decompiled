// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.IPAddressOrRange
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Azure.Cosmos.Table
{
  public class IPAddressOrRange
  {
    public IPAddressOrRange(string address)
    {
      CommonUtility.AssertNotNull(nameof (address), (object) address);
      IPAddressOrRange.AssertIPv4(address);
      this.Address = address;
      this.IsSingleAddress = true;
    }

    public IPAddressOrRange(string minimum, string maximum)
    {
      CommonUtility.AssertNotNull(nameof (minimum), (object) minimum);
      CommonUtility.AssertNotNull(nameof (maximum), (object) maximum);
      IPAddressOrRange.AssertIPv4(minimum);
      IPAddressOrRange.AssertIPv4(maximum);
      this.MinimumAddress = minimum;
      this.MaximumAddress = maximum;
      this.IsSingleAddress = false;
    }

    public string Address { get; private set; }

    public string MinimumAddress { get; private set; }

    public string MaximumAddress { get; private set; }

    public bool IsSingleAddress { get; private set; }

    public override string ToString() => this.IsSingleAddress ? this.Address : this.MinimumAddress + "-" + this.MaximumAddress;

    private static void AssertIPv4(string address)
    {
      IPAddress address1;
      if (!IPAddress.TryParse(address, out address1))
        throw new ArgumentException("Error when parsing IP address: IP address is invalid.");
      if (address1.AddressFamily != AddressFamily.InterNetwork)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "When specifying an IP Address in a SAS token, it must be an IPv4 address. Input address was {0}.", new object[1]
        {
          (object) address
        }));
    }
  }
}
