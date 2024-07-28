// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution.AfdSubnetConfig
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution
{
  internal static class AfdSubnetConfig
  {
    public static readonly List<IPv4Subnet> FallbackAfdSubnets = new List<string>()
    {
      "10.1.148.0/23",
      "10.1.19.128/25",
      "10.1.2.0/24",
      "10.1.34.0/23",
      "10.1.44.0/23",
      "10.1.62.0/23",
      "10.117.208.0/20",
      "10.16.144.0/20",
      "10.140.0.0/17",
      "10.174.192.0/21",
      "10.175.96.0/20",
      "10.186.64.0/21",
      "10.186.72.0/21",
      "10.186.80.0/22",
      "10.186.84.0/26",
      "10.186.84.128/25",
      "10.186.85.128/25",
      "10.186.86.0/23",
      "10.186.88.0/22",
      "10.186.92.0/22",
      "10.186.96.0/21",
      "10.186.104.0/21",
      "10.186.112.0/22",
      "10.186.120.0/21",
      "10.201.192.0/18",
      "10.207.80.0/20",
      "10.232.160.0/20",
      "10.234.144.0/20",
      "10.235.80.0/20",
      "10.24.218.0/23",
      "10.24.222.0/25",
      "10.243.240.0/20",
      "10.254.16.0/20",
      "10.26.242.0/23",
      "10.26.244.0/22",
      "10.26.248.0/22",
      "10.26.253.0/24",
      "10.3.240.0/20",
      "10.40.142.0/23",
      "10.40.245.0/25",
      "10.45.16.192/26",
      "10.45.17.0/24",
      "10.45.32.128/25",
      "10.45.33.0/24",
      "10.45.5.64/26",
      "10.45.96.0/23",
      "10.55.208.0/20",
      "10.58.12.0/22",
      "10.58.88.0/22",
      "10.63.104.0/22",
      "10.63.108.0/23",
      "10.63.80.0/21",
      "10.64.128.0/19",
      "10.64.168.0/21",
      "10.73.104.0/22",
      "10.73.16.0/20",
      "10.74.96.0/21",
      "10.78.212.0/22",
      "147.243.0.0/16",
      "51.4.1.0/24",
      "51.4.86.64/26",
      "51.5.87.0/25"
    }.ConvertAll<IPv4Subnet>((Converter<string, IPv4Subnet>) (x => IPv4Subnet.Parse(x)));
    public static readonly IConfigPrototype<List<IPv4Subnet>> AfdSubnetsPrototype = ConfigPrototype.Create<List<IPv4Subnet>>("AzureFrontDoor.IpRange", AfdSubnetConfig.FallbackAfdSubnets);
  }
}
