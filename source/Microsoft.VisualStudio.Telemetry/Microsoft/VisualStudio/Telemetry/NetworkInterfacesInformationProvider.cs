// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.NetworkInterfacesInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class NetworkInterfacesInformationProvider : INetworkInterfacesInformationProvider
  {
    private static Lazy<NetworkInterfacesInformationProvider.NetworkInterfaceInformation> networkInterfacesInformation;
    private const int NetworkInterfaceSelection_Tier1 = 100;
    private const int NetworkInterfaceSelection_Tier2 = 200;
    private const int NetworkInterfaceSelection_Tier3 = 300;
    private const int NetworkInterfaceSelection_ExcludedTier = 10000;
    private const int NetworkInterfaceSelection_Prioritization = -100;
    private const int NetworkInterfaceSelection_Minor_Prioritization = -50;
    private const int NetworkInterfaceSelection_Minor_Deprioritization = 200;
    private const int NetworkInterfaceSelection_Deprioritization = 300;
    private static readonly Dictionary<NetworkInterfaceType, int> NetworkInterfaceTypeIdPriorities = new Dictionary<NetworkInterfaceType, int>()
    {
      {
        NetworkInterfaceType.Wireless80211,
        100
      },
      {
        NetworkInterfaceType.Ethernet,
        200
      },
      {
        NetworkInterfaceType.GigabitEthernet,
        200
      },
      {
        NetworkInterfaceType.FastEthernetT,
        200
      },
      {
        NetworkInterfaceType.Ethernet3Megabit,
        200
      },
      {
        NetworkInterfaceType.FastEthernetFx,
        200
      },
      {
        NetworkInterfaceType.Fddi,
        200
      },
      {
        NetworkInterfaceType.TokenRing,
        300
      },
      {
        NetworkInterfaceType.Isdn,
        300
      },
      {
        NetworkInterfaceType.BasicIsdn,
        300
      },
      {
        NetworkInterfaceType.PrimaryIsdn,
        300
      },
      {
        NetworkInterfaceType.RateAdaptDsl,
        300
      },
      {
        NetworkInterfaceType.SymmetricDsl,
        300
      },
      {
        NetworkInterfaceType.VeryHighSpeedDsl,
        300
      },
      {
        NetworkInterfaceType.MultiRateSymmetricDsl,
        300
      }
    };
    private static readonly Regex ExcludedNetworkInterfaceRegex = new Regex("( Loopback )|(TAP-?)|(TEST)|(VPN)|(Remote )", RegexOptions.CultureInvariant);
    private static readonly Regex MinorDeprioritizedNetworkInterfaceDescriptionRegex = new Regex("(Microsoft Hyper-V Network Adapter|USB |Targus)", RegexOptions.CultureInvariant);
    private static readonly Regex DeprioritizedNetworkInterfaceDescriptionRegex = new Regex("([V|v]irt(IO|u(al|ální|eller)))|(仮)|(PdaNet )|(vmxnet)|(IPVanish)|(RNDIS )", RegexOptions.CultureInvariant);
    private static readonly Regex PrioritizedPhysicalManufacturersRegex = new Regex("^(Intel|Marvell|Realtek)", RegexOptions.CultureInvariant);
    private static readonly Regex KnownPhysicalManufacturersRegex = new Regex("^(Qualcomm Atheros|Killer |Surface Ethernet Adapter|Broadcom |Dell |TP-Link|Apple |D-Link)", RegexOptions.CultureInvariant);

    public string SelectedMACAddress => NetworkInterfacesInformationProvider.networkInterfacesInformation.Value.SelectedMACAddress;

    public List<NetworkInterfaceCardInformation> PrioritizedNetworkInterfaces => NetworkInterfacesInformationProvider.networkInterfacesInformation.Value.PrioritizedNetworkInterfaces;

    public NetworkInterfacesInformationProvider(
      List<TelemetryManifestMachineIdentityConfig.SmaRule> rules,
      Func<IEnumerable<NetworkInterfaceCardInformation>> getAllNetworkInterfaceCardInformation = null)
    {
      this.SelectedMacAddressRules = rules;
      this.GetAllNetworkInterfaceCardInformation = getAllNetworkInterfaceCardInformation ?? this.GetAllNetworkInterfaceCardInformation;
      NetworkInterfacesInformationProvider.networkInterfacesInformation = new Lazy<NetworkInterfacesInformationProvider.NetworkInterfaceInformation>(new Func<NetworkInterfacesInformationProvider.NetworkInterfaceInformation>(this.InitializeNetworkInterfacesInformation));
    }

    private List<TelemetryManifestMachineIdentityConfig.SmaRule> SelectedMacAddressRules { get; set; }

    private Func<IEnumerable<NetworkInterfaceCardInformation>> GetAllNetworkInterfaceCardInformation { get; } = (Func<IEnumerable<NetworkInterfaceCardInformation>>) (() => ((IEnumerable<NetworkInterface>) NetworkInterface.GetAllNetworkInterfaces()).Select<NetworkInterface, NetworkInterfaceCardInformation>((Func<NetworkInterface, NetworkInterfaceCardInformation>) (nic => new NetworkInterfaceCardInformation()
    {
      Description = nic.Description,
      MacAddress = nic.GetPhysicalAddress().ToString(),
      NetworkInterfaceType = nic.NetworkInterfaceType,
      OperationalStatus = nic.OperationalStatus
    })));

    private NetworkInterfacesInformationProvider.NetworkInterfaceInformation InitializeNetworkInterfacesInformation()
    {
      List<NetworkInterfaceCardInformation> list = this.GetAllNetworkInterfaceCardInformation().Select<NetworkInterfaceCardInformation, NetworkInterfaceCardInformation>((Func<NetworkInterfaceCardInformation, NetworkInterfaceCardInformation>) (nic =>
      {
        nic.SelectionTier = this.GetSelectionTeir(nic);
        return nic;
      })).OrderBy<NetworkInterfaceCardInformation, int>((Func<NetworkInterfaceCardInformation, int>) (nic => nic.SelectionTier)).ThenBy<NetworkInterfaceCardInformation, string>((Func<NetworkInterfaceCardInformation, string>) (nic => nic.MacAddress)).ToList<NetworkInterfaceCardInformation>();
      int rank = 0;
      list.ForEach((Action<NetworkInterfaceCardInformation>) (nic => nic.SelectionRank = rank++));
      return new NetworkInterfacesInformationProvider.NetworkInterfaceInformation()
      {
        PrioritizedNetworkInterfaces = list,
        SelectedMACAddress = list.FirstOrDefault<NetworkInterfaceCardInformation>()?.MacAddress
      };
    }

    private int GetSelectionTeir(NetworkInterfaceCardInformation nic)
    {
      int selectionTeir;
      if (!NetworkInterfacesInformationProvider.NetworkInterfaceTypeIdPriorities.TryGetValue(nic.NetworkInterfaceType, out selectionTeir))
        return 10000;
      foreach (TelemetryManifestMachineIdentityConfig.SmaRule smaRule in (IEnumerable<TelemetryManifestMachineIdentityConfig.SmaRule>) this.SelectedMacAddressRules.OrderBy<TelemetryManifestMachineIdentityConfig.SmaRule, uint>((Func<TelemetryManifestMachineIdentityConfig.SmaRule, uint>) (r => r.Position)))
      {
        string input;
        switch (smaRule.Target)
        {
          case TelemetryManifestMachineIdentityConfig.SmaRule.TargetType.Description:
            input = nic.Description;
            break;
          case TelemetryManifestMachineIdentityConfig.SmaRule.TargetType.ConnectionType:
            input = nic.NetworkInterfaceType.ToString();
            break;
          case TelemetryManifestMachineIdentityConfig.SmaRule.TargetType.MACAddress:
            input = nic.MacAddress;
            break;
          default:
            continue;
        }
        if (Regex.IsMatch(input, smaRule.Regex, RegexOptions.CultureInvariant))
        {
          selectionTeir += smaRule.TierAdjustment;
          if (smaRule.StopProcessing)
            break;
        }
      }
      return selectionTeir;
    }

    private struct NetworkInterfaceInformation
    {
      public List<NetworkInterfaceCardInformation> PrioritizedNetworkInterfaces;
      public string SelectedMACAddress;
    }
  }
}
